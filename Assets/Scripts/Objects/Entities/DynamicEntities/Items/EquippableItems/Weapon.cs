using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using static ItemTypes;
using System.Threading;
using Medical;
using System.Threading.Tasks;
using System.Linq;

public class Weapon : EquippableItem
{
    public const float BaseAttackTime = 1.0f;

    public Transform RightAnchor;
    public Transform LeftAnchor;

    public enum DamageTypesEnum
    {
        Blunt,
        Sharp,
        Explosive,
        Psionic,
        Other
    }

    //Data for an attack call
    public struct AttackData
    {
        bool Ranged;
        //Functions to check for attack speed, hit chance, and damage respectively
        public Dictionary<Health.PartFunctions, float> SpeedFunctions;
        public Dictionary<Health.PartFunctions, float> HitFunctions;
        public Dictionary<Health.PartFunctions, float> DamageFunctions;
    }

    public override void Init()
    {
        base.Init();
        Stats.AddStat(StatsEnum.Damage, 0f);
        Stats.AddStat(StatsEnum.ArmourPiercing, 0f);
        Stats.AddStat(StatsEnum.AttackSpeed, 0f);

        BaseStats = new BaseItemStats()
        {
            Stats = new Dictionary<StatsEnum, object>()
            {
                { StatsEnum.Damage, 1f },
                { StatsEnum.ArmourPiercing, 1f },
                { StatsEnum.AttackSpeed, 1f },
                { StatsEnum.Size, new Vector2Int(1, 1) },
                { StatsEnum.Mass, 1f },
                { StatsEnum.Cost, new Resources(1, 0, 0) }
            },

        };
        ValidSlots = new Equipment.Slots[] { Equipment.Slots.Weapon, Equipment.Slots.SecondaryWeapon };
    }

    public virtual async void AttackOrder(Actor User, IDamageable Target, CancellationToken token, bool isReaction)
    {
        
    }

    protected async Task AttackInstance(Actor user, IDamageable target, AttackData data, CancellationToken token)
    {

        //Get associated skills for the action
        const float skillImpactCoefficient = 10;
        StatsAndSkills.Skill[] ItemSkills = user.EntityComponents.Stats.GetItemSkills(Subtype);

        float primarySpeedImpact = 1 + ItemSkills[0].GetAdjustedLevel("Speed") / skillImpactCoefficient;
        float secondarySpeedImpact = 1 + ItemSkills[1].GetAdjustedLevel("Speed") / skillImpactCoefficient * ItemSkills[1].SecondaryTypeCoefficient;
        float attackTime = BaseAttackTime / (Stats.GetStat<float>(StatsEnum.AttackSpeed) * primarySpeedImpact * secondarySpeedImpact);

        //Get total modifier for functionality. If all related limbs are fine, this should be 1.
        KeyValuePair<Health.PartFunctions, float>[] speedFunctionalities = user.EntityComponents.Health.GetPartFunctions(data.SpeedFunctions.Keys.ToArray());
        float totalSpeedFunctionality = 1;
        for (int i = 0; i < speedFunctionalities.Length; i++)
        {
            float statImpact = data.SpeedFunctions[speedFunctionalities[i].Key];
            totalSpeedFunctionality *= Mathf.Pow(speedFunctionalities[i].Value, statImpact);
        }

        //Wait for attackTime
        user.animator.SetBool("Attacking", true);
        float attackAnimationLength = user.animator.runtimeAnimatorController.animationClips.First(c => c.name.Contains("Attack")).length;
        user.animator.SetFloat("AttackSpeed", attackAnimationLength / attackTime);
        float attackTimemiliseconds = attackTime * 1000f;
        try
        {
            await Task.Delay(Mathf.RoundToInt(attackTimemiliseconds / 2f), token);
            user.CurrentAction.IsCommitted = true;
            target.Retaliate(user);
            await Task.Delay(Mathf.RoundToInt(attackTimemiliseconds / 2f), token);
        }
        catch (TaskCanceledException) { }
        user.CurrentAction.IsCommitted = false;
        user.animator.SetBool("Attacking", false);
        if (token.IsCancellationRequested)
            return;

        //Get total modifier for functionality. If all related limbs are fine, this should be 1.
        KeyValuePair<Health.PartFunctions, float>[] hitFunctionalities = user.EntityComponents.Health.GetPartFunctions(data.HitFunctions.Keys.ToArray());
        float totalHitFunctionality = 1;
        for (int i = 0; i < hitFunctionalities.Length; i++)
        {
            float statImpact = data.HitFunctions[hitFunctionalities[i].Key];
            totalHitFunctionality *= Mathf.Pow(hitFunctionalities[i].Value, statImpact);
        }

        //Get the relevant skill impacts for the action
        float primaryHitImpact = 1 + ItemSkills[0].GetAdjustedLevel("HitChance") / skillImpactCoefficient;
        float secondaryHitImpact = 1 + ItemSkills[1].GetAdjustedLevel("HitChance") / skillImpactCoefficient * ItemSkills[1].SecondaryTypeCoefficient;


        //Finalise the hit chances
        float userAttack = primaryHitImpact * secondaryHitImpact;

        if (target.GetDodge(userAttack, user))
            return;

        if (target.GetBlock(userAttack, user))
            return;


        float criticalChance = 0.5f;
        bool critical = Random.value <= criticalChance;

        //Get total relevant body functionality
        //Get total relevant stat bonuses from StatsAndSkills.cs
        //Get total relevant skill bonuses
        KeyValuePair<Health.PartFunctions, float>[] damageFunctionalities = user.EntityComponents.Health.GetPartFunctions(data.DamageFunctions.Keys.ToArray());
        float totalDamageFunctionality = 1;
        for (int i = 0; i < damageFunctionalities.Length; i++)
        {
            float statImpact = data.DamageFunctions[damageFunctionalities[i].Key];
            totalDamageFunctionality *= Mathf.Pow(damageFunctionalities[i].Value, statImpact);
        }

        float primaryDamageImpact = 1 + (ItemSkills[0].GetAdjustedLevel("Damage") / skillImpactCoefficient);
        float secondaryDamageImpact = 1 + ItemSkills[1].GetAdjustedLevel("Damage") / skillImpactCoefficient * ItemSkills[1].SecondaryTypeCoefficient;

        float damage = Random.Range(0.75f, 1.25f) * (Stats.GetStat<float>(StatsEnum.Damage) * totalDamageFunctionality * primaryDamageImpact * secondaryDamageImpact);

        target.Damage(damage, critical, DamageTypesEnum.Sharp);
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>
        {
            new UIController.KVPData(StatsEnum.Damage.ToString(), Stats.GetStat<float>(StatsEnum.Damage), Parent, 1),
            new UIController.KVPData(StatsEnum.ArmourPiercing.ToString(), Stats.GetStat<float>(StatsEnum.ArmourPiercing), Parent, 1),
            new UIController.KVPData(StatsEnum.AttackSpeed.ToString(), Stats.GetStat<float>(StatsEnum.AttackSpeed), Parent, 1)
        };

        KVPDatas[0].ValueEnum = StatsEnum.Damage;
        KVPDatas[1].ValueEnum = StatsEnum.ArmourPiercing;
        KVPDatas[2].ValueEnum = StatsEnum.AttackSpeed;

        foreach (UIController.KVPData Data in KVPDatas)
        {
            Data.RefItem = this;
            Data.ValueDelegate = KeyValuePanel.GetItemStat;
            Data.Group = Group;
            KVPs.Add(UIController.InstantiateKVP(Data));
        }

        List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

        List<GameObject> CombinedKVPs = Utility.CombineLists(KVPs, BaseKVPs);
        CombinedKVPLists = Utility.CombineLists(KVPLists, BaseKVPLists);

        return CombinedKVPs;
    }
}
