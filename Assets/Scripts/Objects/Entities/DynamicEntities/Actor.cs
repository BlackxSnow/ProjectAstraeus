using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using System.Threading.Tasks;
using Medical;
using System.Linq;
using UnityAsync;
using System;

public class Actor : DynamicEntity, IOrderable, IDamageable, IInterruptible
{
    public StateMachine.AIStateMachine StateMachine { get; protected set; }
    public struct CurrentActionStruct
    {
        public string Name;
        public bool IsReaction;
        public bool IsCommitted;
        public Entity Target;

        public void Cancel()
        {
            Name = "None";
            IsReaction = IsCommitted = false;
            Target = null;
        }
    }
    public enum CombatStances
    {
        Aggressive,
        Defensive,
        Flee
    }
    public CombatStances CurrentStance = CombatStances.Defensive;
    public CancellationTokenSource TokenSource { get; private set; } = new CancellationTokenSource();
    bool Subscribed;

    bool Movable;

    float PickupDistance = 1.5f;

    public Species Race;
    public CurrentActionStruct CurrentAction = new CurrentActionStruct();
    //public IDamageable Target;
    //public bool IsReacting = false;
    //public bool CommittedAttack = false;
    public bool InCombat = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
        if (EntityComponents.Movement != null) Movable = true; else Movable = false;
    }

    public void Interrupt()
    {
        if (CurrentAction.IsCommitted) Debug.LogWarning($"Action was interrupted despite being committed: '{CurrentAction.Name}' target '{CurrentAction.Target.Name}'");
        TokenSource.Cancel();
        CurrentAction.Cancel();
        TokenSource = new CancellationTokenSource();
    }

    #region Combat
    public void ChangeStance(CombatStances targetStance)
    {
        CurrentStance = targetStance;
    }

    public void Damage(float amount, bool critical, Weapon.DamageTypesEnum damageType)
    {
        EntityComponents.Health.Damage(amount, critical, damageType);
    }

    public async void Block(Actor attacker)
    {
        CurrentActionStruct oldAction = CurrentAction;
        Interrupt();
        CurrentAction = new CurrentActionStruct { Name = "Defend", IsReaction = oldAction.IsReaction, IsCommitted = true };
        
        await EntityComponents.IKController.BlockIK(attacker);

        if (oldAction.Name == "Attack" && oldAction.Target != null)
            Attack(oldAction.Target as IDamageable, oldAction.IsReaction);
    }

    public void Dodge(Actor attacker)
    {
        CurrentActionStruct oldAction = CurrentAction;
        Interrupt();
        CurrentAction = new CurrentActionStruct { Name = "Defend", IsReaction = oldAction.IsReaction, IsCommitted = true };

        animator.SetTrigger("Dodge");

        //Wait for animation. Implement StateMachineBehaviour on Dodge, and await AutoResetEvent?

        if (oldAction.Name == "Attack" && oldAction.Target != null)
            Attack(oldAction.Target as IDamageable, oldAction.IsReaction);
    }

    public void Attack(IDamageable target, bool isReaction)
    {
        InCombat = true;
        CurrentAction.IsReaction = isReaction;
        CurrentAction.Target = target as Entity;
        CurrentAction.Name = "Attack";
        CancellationToken token = TokenSource.Token;

        //Is character using a weapon
        Weapon currentWeapon = EntityComponents.Equipment?.Equipped?[(int)Equipment.Slots.Weapon] as Weapon;
        if (currentWeapon)
        {
            currentWeapon.AttackOrder(this, target, token, CurrentAction.IsReaction);
        }
        else UnarmedAttack(target, token, CurrentAction.IsReaction);
    }

    public void Retaliate(IDamageable target)
    {
        if(!InCombat)
        {
            Attack(target, true);
        }
    }

    private async void UnarmedAttack(IDamageable target, CancellationToken token, bool IsReacting)
    {
        Weapon.AttackData attackData = new Weapon.AttackData
        {
            SpeedFunctions = new Dictionary<Health.PartFunctions, float>
                {
                    { Health.PartFunctions.Manipulation, 1.0f }
                },
            HitFunctions = new Dictionary<Health.PartFunctions, float>
                {
                    { Health.PartFunctions.Manipulation, 1.5f },
                    { Health.PartFunctions.Vision, 1.0f }
                },

            DamageFunctions = new Dictionary<Health.PartFunctions, float>
                {
                    { Health.PartFunctions.Manipulation, 4.0f }
                }
        };
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        CancellationTokenSource attackTokenSource = new CancellationTokenSource();
        Task AttackTask = null;
        bool Enabled = true;
        while (Enabled)
        {
            float Range = 2f;

            if (token.IsCancellationRequested)
            {
                animator.SetBool("MeleeStance", false);
                return;
            }

            if (Vector3.Distance(transform.position, (target as MonoBehaviour).transform.position) >= Range)
            {
                attackTokenSource.Cancel();
                attackTokenSource = new CancellationTokenSource();
                await EntityComponents.Movement.MoveWithin((target as MonoBehaviour).gameObject, Range, null, token);
            }
            else
            {
                if (AttackTask == null || AttackTask.IsCanceled || AttackTask.IsCompleted)
                {
                    animator.SetBool("MeleeStance", true);
                    await EntityComponents.Movement.RotateTowards((target as MonoBehaviour).transform);
                    AttackTask = UnarmedAttackInstance(target, attackData, attackTokenSource.Token);
                    await AttackTask;
                }
                await Await.NextUpdate();
            }
        }
    }
    protected async Task UnarmedAttackInstance(IDamageable target, Weapon.AttackData data, CancellationToken token)
    {
        //Get associated skills for the action
        const float skillImpactCoefficient = 10;

        float primarySpeedImpact = 1f + EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Unarmed].GetAdjustedLevel("Speed") / skillImpactCoefficient;
        float attackTime = Weapon.BaseAttackTime / primarySpeedImpact;

        //Get total modifier for functionality. If all related limbs are fine, this should be 1.
        KeyValuePair<Health.PartFunctions, float>[] speedFunctionalities = EntityComponents.Health.GetPartFunctions(data.SpeedFunctions.Keys.ToArray());
        float totalSpeedFunctionality = 1;
        for (int i = 0; i < speedFunctionalities.Length; i++)
        {
            float statImpact = data.SpeedFunctions[speedFunctionalities[i].Key];
            totalSpeedFunctionality *= Mathf.Pow(speedFunctionalities[i].Value, statImpact);
        }

        //Wait for attackTime
        float attackAnimationLength = animator.runtimeAnimatorController.animationClips.First(c => c.name.Contains("Attack")).length;
        animator.SetFloat("AttackSpeed", attackAnimationLength / (attackTime / 2));
        float attackTimemiliseconds = attackTime * 1000f;
        try
        {
            await Task.Delay(Mathf.RoundToInt(attackTimemiliseconds / 2f), token);
            CurrentAction.IsCommitted = true;
            animator.SetTrigger("MeleeAttack");
            target.Retaliate(this);
            await Task.Delay(Mathf.RoundToInt(attackTimemiliseconds / 2f), token);
        }
        catch (TaskCanceledException) { }
        CurrentAction.IsCommitted = false;
        if (token.IsCancellationRequested)
            return;

        //Get total modifier for functionality. If all related limbs are fine, this should be 1.
        KeyValuePair<Health.PartFunctions, float>[] hitFunctionalities = EntityComponents.Health.GetPartFunctions(data.HitFunctions.Keys.ToArray());
        float totalHitFunctionality = 1;
        for (int i = 0; i < hitFunctionalities.Length; i++)
        {
            float statImpact = data.HitFunctions[hitFunctionalities[i].Key];
            totalHitFunctionality *= Mathf.Pow(hitFunctionalities[i].Value, statImpact);
        }

        //Get the relevant skill impacts for the action
        float primaryHitImpact = 1 + EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Unarmed].GetAdjustedLevel("HitChance") / skillImpactCoefficient;


        //Finalise the hit chances
        float userAttack = primaryHitImpact;

        if (target.GetDodge(userAttack, this))
            return;

        if (target.GetBlock(userAttack, this))
            return;


        float criticalChance = 0.5f;
        bool critical = UnityEngine.Random.value <= criticalChance;

        //Get total relevant body functionality
        //Get total relevant stat bonuses from StatsAndSkills.cs
        //Get total relevant skill bonuses
        KeyValuePair<Health.PartFunctions, float>[] damageFunctionalities = EntityComponents.Health.GetPartFunctions(data.DamageFunctions.Keys.ToArray());
        float totalDamageFunctionality = 1;
        for (int i = 0; i < damageFunctionalities.Length; i++)
        {
            float statImpact = data.DamageFunctions[damageFunctionalities[i].Key];
            totalDamageFunctionality *= Mathf.Pow(damageFunctionalities[i].Value, statImpact);
        }

        float primaryDamageImpact = 1 + EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Unarmed].GetAdjustedLevel("Damage") / skillImpactCoefficient;

        float damage = UnityEngine.Random.Range(0.75f, 1.25f) * totalDamageFunctionality * primaryDamageImpact;

        target.Damage(damage, critical, Weapon.DamageTypesEnum.Blunt);
    }


    const int DefenceCoefficient = 10;
    public bool GetDodge(float attackValue, Actor attacker)
    {
        if (CurrentAction.IsCommitted)
            return false;

        float defence = 0;
        if(EntityComponents.Stats)
        {
            defence += 1 + (EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Dodge].GetAdjustedLevel() / DefenceCoefficient);
        }
        float chance = 0.5f * (attackValue / defence);
        bool result = UnityEngine.Random.value <= chance ? true : false;
        if (result)
            Dodge(attacker);

        return result;
    }

    public bool GetBlock(float attackValue, Actor attacker)
    {
        if (CurrentAction.IsCommitted)
            return false;

        float defence = 0;
        if (EntityComponents.Stats)
        {
            defence +=  1 + (EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Block].GetAdjustedLevel() / DefenceCoefficient);
        }
        float chance = 0.5f * (attackValue / defence);
        bool result = UnityEngine.Random.value <= chance ? true : false;
        if (result)
            Block(attacker);

        return result;
    }
    #endregion
    #region Movement
    public void Move(Vector3 Destination, FlockController flockController, bool interrupt = true)
    {
        if (interrupt) Interrupt();

        _ = EntityComponents.Movement.SetDestination(Destination, flockController, TokenSource.Token);
    }

    public void Pickup(DynamicEntity Target)
    {
        Interrupt();
        PickupObject(Target);
    }

    //Behaviour to retrieve items
    private async void PickupObject(DynamicEntity Target)
    {
        Task<bool> MoveTask = EntityComponents.Movement.MoveWithin(Target.gameObject, PickupDistance, null, TokenSource.Token);
        await MoveTask;
        if (!MoveTask.Result) return;

        if (Target is Item) EntityComponents.Inventory.AddItem(Target as Item);

    }

    private bool ProximityCheck(GameObject Target)
    {
        if (Vector3.Distance(transform.position, Target.transform.position) <= PickupDistance)
            return true;
        else
            return false;
    }
    #endregion

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Selected && FactionID == 0 && !Subscribed && Movable)
        {
            OrderEvents.OnMove += Move;
            OrderEvents.OnPickup += Pickup;
            OrderEvents.OnAttack += Attack;
            Subscribed = true;
        } else if (!Selected && Subscribed)
        {
            OrderEvents.OnMove -= Move;
            OrderEvents.OnPickup -= Pickup;
            OrderEvents.OnAttack -= Attack;
            Subscribed = false;
        }
    }

    //Create medical info display for HUD
    public override List<GameObject> InstantiateStatDisplay()
    {
        GameObject MedicalPanel = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.MedicalDetailsPrefab], HUDController.HUDControl.SelectHUD.DetailsPanel.transform);
        UI.MedicalDetailsHUD MedicalPanelScript = MedicalPanel.GetComponent<UI.MedicalDetailsHUD>();
        MedicalPanelScript.SelectedActor = this;
        MedicalPanelScript.Init();

        return new List<GameObject>() { MedicalPanel };
    }
}
