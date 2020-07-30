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
using AI.States;
using UI.Control;
using Items;

public class Actor : DynamicEntity, IOrderable, IDamageable
{
    public enum CombatStances
    {
        Aggressive,
        Defensive,
        Flee
    }

    public Weapon.WeaponStats.AttackStats UnarmedAttack = new Weapon.WeaponStats.AttackStats
    {
        Damages = new List<Weapon.DamageInfo>
        {
            new Weapon.DamageInfo
            {
                Damage = 2.0f,
                DamageType = Weapon.DamageTypesEnum.Blunt,
                ArmourPiercing = 0
            }
        },
        Range = 2.0f,
        AttackSpeed = 2.0f,
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

    public CombatStances CurrentStance = CombatStances.Defensive;
    public CancellationTokenSource TokenSource { get; private set; } = new CancellationTokenSource();
    bool Subscribed;

    bool Movable;

    float PickupDistance = 1.5f;

    public Species Race;
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
        StateMachine = new AI.AIStateMachine(this);
    }

    #region Combat
    public void ChangeStance(CombatStances targetStance)
    {
        CurrentStance = targetStance;
    }

    public void Damage(Weapon.DamageInfo[] damages, bool critical)
    {
        //TODO make this only cause a maximum of one injury. Figure out how different damage types impact injury rolls.
        EntityComponents.Health.Damage(damages.Sum(d => d.Damage), critical, damages[0].DamageType);
    }
    public float GetHealth()
    {
        return EntityComponents.Health.HitPoints;
    }

    public delegate void DefendEvent(Actor attacker);
    public DefendEvent OnBlock;
    public void Block(Actor attacker)
    {
        if (OnBlock != null)
        {
            OnBlock.Invoke(attacker);
            return;
        }
        StateMachine.AddStateImmediate(new Block(this, null, attacker));
    }

    public DefendEvent OnDodge;
    public void Dodge(Actor attacker)
    {
        if (OnDodge != null)
        {
            OnDodge.Invoke(attacker);
            return;
        }
        StateMachine.AddStateImmediate(new Dodge(this, null, attacker));
    }

    public void Attack(IDamageable target, bool isReaction)
    {
        InCombat = true;
        StateMachine.SetState(new Attack(this, null, target));
    }

    public void Retaliate(IDamageable target)
    {
        if(!InCombat)
        {
            Attack(target, true);
        }
    }

    const int DefenceCoefficient = 10;
    public bool GetDodge(float attackValue, Actor attacker)
    {
        bool committed = StateMachine?.Current?.IsCommitted != null ? StateMachine.Current.IsCommitted : false;
        if (committed)
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
        bool committed = StateMachine?.Current?.IsCommitted != null ? StateMachine.Current.IsCommitted : false;
        if (committed)
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
        //if (interrupt) Interrupt();
        StateMachine.SetState(new MoveToPoint(this, null, Destination, flockController));
    }

    public void Pickup(DynamicEntity target)
    {
        StateMachine.SetState(new PickUpStorable(this, null, target as Item));
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
