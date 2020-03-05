using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Modules;
using static ItemTypes;
using static Modules.AdditionalModule;
using Medical;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;

namespace Items
{
    public class Melee : Weapon
    {
        public override void Init()
        {
            base.Init();
            Stats.AddStat(StatsEnum.Block, 0f);
            BaseStats.Stats.Add(StatsEnum.Block, 0f);
            Stats.AddStat(StatsEnum.Range, 1f);
            BaseStats.Stats.Add(StatsEnum.Range, 1f);

            BaseStats.CompatibleModules = new List<ModulesEnum>()
            {
                ModulesEnum.Handle,
                ModulesEnum.Blade,
                ModulesEnum.Head
            };
            BaseStats.RequiredModules = new List<ModulesEnum>()
            {
                ModulesEnum.Handle
            };
        }

        //TODO Animate
        public override async void AttackOrder(Actor user, IDamageable target, CancellationToken token)
        {
            CancellationTokenSource attackTokenSource = new CancellationTokenSource();
            Task AttackTask = null;
            bool Enabled = true;
            while(Enabled)
            {
                float Range = Stats.GetStat<float>(StatsEnum.Range);

                if (token.IsCancellationRequested)
                    return;

                if (Vector3.Distance(user.transform.position, (target as MonoBehaviour).transform.position) >= Range)
                {
                    attackTokenSource.Cancel();
                    attackTokenSource = new CancellationTokenSource();
                    MonoBehaviour TMB = target as MonoBehaviour;
                    GameObject TGO = TMB.gameObject;
                    await user.EntityComponents.Movement.MoveWithin(TGO, Range, null, token);
                }
                else
                {
                    if (AttackTask == null || AttackTask.IsCanceled || AttackTask.IsCompleted)
                    {
                        AttackTask = AttackInstance(user, target, attackTokenSource.Token);
                        await AttackTask;
                    }
                    await Await.NextUpdate();
                }
            }
        }

        private async Task AttackInstance(Actor user, IDamageable target, CancellationToken token)
        {
            Dictionary<Health.PartFunctions, float> speedFunctions = new Dictionary<Health.PartFunctions, float>
            {
                { Health.PartFunctions.Manipulation, 1.0f }
            };
            Dictionary<Health.PartFunctions, float> hitFunctions = new Dictionary<Health.PartFunctions, float>
            {
                { Health.PartFunctions.Manipulation, 1.5f },
                { Health.PartFunctions.Vision, 1.0f }
            };

            Dictionary<Health.PartFunctions, float> damageFunctions = new Dictionary<Health.PartFunctions, float>
            {
                { Health.PartFunctions.Manipulation, 2.0f }
            };

            //Get associated skills for the action
            const float skillImpactCoefficient = 10;
            StatsAndSkills.Skill[] ItemSkills = user.EntityComponents.Stats.GetItemSkills(Subtype);

            float primarySpeedImpact = 1 + ItemSkills[0].GetAdjustedLevel("Speed") / skillImpactCoefficient;
            float secondarySpeedImpact =  1 + ItemSkills[1].GetAdjustedLevel("Speed") / skillImpactCoefficient * ItemSkills[1].SecondaryTypeCoefficient;
            float attackTime = BaseAttackTime / (Stats.GetStat<float>(StatsEnum.AttackSpeed) * primarySpeedImpact * secondarySpeedImpact);

            //Get total modifier for functionality. If all related limbs are fine, this should be 1.
            KeyValuePair<Health.PartFunctions, float>[] speedFunctionalities = user.EntityComponents.Health.GetPartFunctions(speedFunctions.Keys.ToArray());
            float totalSpeedFunctionality = 1;
            for (int i = 0; i < speedFunctionalities.Length; i++)
            {
                float statImpact = speedFunctions[speedFunctionalities[i].Key];
                totalSpeedFunctionality *= Mathf.Pow(speedFunctionalities[i].Value, statImpact);
            }
            
            //Wait for attackTime
            try
            {
                await Task.Delay(Mathf.RoundToInt(attackTime * 1000f), token);
            }
            catch (TaskCanceledException) { }

            if (token.IsCancellationRequested)
                return;

            //Get total modifier for functionality. If all related limbs are fine, this should be 1.
            KeyValuePair<Health.PartFunctions, float>[] hitFunctionalities = user.EntityComponents.Health.GetPartFunctions(hitFunctions.Keys.ToArray());
            float totalHitFunctionality = 1;
            for (int i = 0; i < hitFunctionalities.Length; i++)
            {
                float statImpact = hitFunctions[hitFunctionalities[i].Key];
                totalHitFunctionality *= Mathf.Pow(hitFunctionalities[i].Value, statImpact);
            }

            //Get the relevant skill impacts for the action
            float primaryHitImpact = 1 + ItemSkills[0].GetAdjustedLevel("HitChance") / skillImpactCoefficient;
            float secondaryHitImpact = 1 + ItemSkills[1].GetAdjustedLevel("HitChance") / skillImpactCoefficient * ItemSkills[1].SecondaryTypeCoefficient;


            //Finalise the hit chances
            float userAttack = primaryHitImpact * secondaryHitImpact;
            float dodgeHitChance = 0.5f * (userAttack / target.GetDodgeDefence());
            float blockHitChance = 0.5f * (userAttack / target.GetBlockDefence());
            
            if(Random.value > dodgeHitChance)
            {
                target.Dodge(user);
                return;
            }
            if(Random.value > blockHitChance)
            {
                target.Block(user);
                return;
            }


            float criticalChance = 0.5f;
            bool critical = Random.value <= criticalChance;

            //Get total relevant body functionality
            //Get total relevant stat bonuses from StatsAndSkills.cs
            //Get total relevant skill bonuses
            KeyValuePair<Health.PartFunctions, float>[] damageFunctionalities = user.EntityComponents.Health.GetPartFunctions(damageFunctions.Keys.ToArray());
            float totalDamageFunctionality = 1;
            for(int i = 0; i < damageFunctionalities.Length; i++)
            {
                float statImpact = damageFunctions[damageFunctionalities[i].Key];
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
            List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>();

            KVPDatas.Add(new UIController.KVPData(StatsEnum.Block.ToString(), Stats.GetStat<float>(StatsEnum.Block), Parent, 1));

            KVPDatas[0].ValueEnum = StatsEnum.Block;

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

        public override void CalculateStats()
        {
            Stats.Stats = new Dictionary<StatsEnum, object>(BaseStats.Stats);

            Dictionary<StatsEnum, float> StatMods = new Dictionary<StatsEnum, float>()
            {
                { StatsEnum.Damage, 1f },
                { StatsEnum.Block, 1f },
                { StatsEnum.ArmourPiercing, 1f },
                { StatsEnum.AttackSpeed, 1f },
                { StatsEnum.Range, 1f },
            };

            SetStats(StatMods);
        }

        public struct ThresholdStruct
        {
            public float Leniency;
            public float Dagger;
            public float Sword;
        }
        public static ThresholdStruct MaxLengths = new ThresholdStruct()
        {
            Leniency = 0.1f,
            Dagger = 0.35f,
            Sword = 0
        };

        public override bool FindSubtype()
        {
            SubTypes OldType = Subtype;
            Handle handle = (Handle)Modules.Where(M => M is Handle).ElementAtOrDefault(0);
            Blade blade = (Blade)Modules.Where(M => M is Blade).ElementAtOrDefault(0);
            MaulHead head = (MaulHead)Modules.Where(M => M is MaulHead).ElementAtOrDefault(0);

            if (handle != null)
            {
                float handleLength = handle.GetStat<float>(StatsEnum.Length);
                if (head != null) //Handle, head. Done before blade, as if it has a head it is always a hammer
                {
                    Subtype = SubTypes.Hammer;
                }
                else if (blade != null) //Handle, blade
                {
                    float bladeLength = blade.GetStat<float>(StatsEnum.Length);
                    float TotalLength = bladeLength + handleLength;
                    if (TotalLength <= MaxLengths.Dagger) //Short weapon
                    {
                        Subtype = SubTypes.Dagger;
                    }
                    else if (handleLength > bladeLength + MaxLengths.Leniency) //long handle, shorter blade
                    {
                        Subtype = SubTypes.Polearm;
                    }
                    else 
                    {
                        Subtype = SubTypes.Sword;
                    }
                }
                else //Handle, no blade
                {
                    Subtype = SubTypes.Quarterstaff;
                }
            }
            else
            {
                Subtype = SubTypes.Invalid;
            }
            if (Subtype != OldType)
            {
                return true;
            } else
            {
                return false;
            }
        }
    } 
}
