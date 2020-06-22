using AI.States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Medical;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using UnityAsync;

namespace AI.States
{
	public class Attack : AIState
	{
        public IDamageable Target;
        protected CancellationTokenSource waitTokenSource;
        public delegate void AttackEventHandler(Entity attacker, Entity defender, Weapon weapon, bool blocked, bool critical, params KeyValuePair<Weapon.DamageTypesEnum, float>[] damages);
        public static event AttackEventHandler AttackEvent;

        public Attack(Entity ai,  Del callback, IDamageable target) : base(ai, callback)
		{
            Target = target;
		}

        public override void StartState()
        {
            base.StartState();
            if(EntitySelf is Actor actor)
            {
                actor.OnBlock += BlockState;
                actor.OnDodge += DodgeState;
            }
            BehaviourTokenSource = CancellationTokenSource.CreateLinkedTokenSource(StateTokenSource.Token);
            StateBehaviour(BehaviourTokenSource.Token);
        }

        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            while(Target.GetHealth() > 0)
            {
                try
                {
                    
                    waitTokenSource = CancellationTokenSource.CreateLinkedTokenSource(BehaviourTokenSource.Token);
                    behaviourToken.ThrowIfCancellationRequested();

                    GetStats(out float range, out float attackSpeed, out float damage, out StatsAndSkills.Skill[] skills, out Weapon.AttackData attackFunctions, out Weapon.DamageTypesEnum damageType, out Weapon weapon);
                    if(!ProximityCheck(range))
                    {
                        EntitySelf.animator.SetBool("MeleeStance", false);
                        SubState = MoveWithin.MoveState(EntitySelf, (Target as MonoBehaviour).gameObject, range, behaviourToken);
                        await SubState.StateCompleted.WaitAsync();
                        SubState = null;
                    }
                    EntitySelf.animator.SetBool("MeleeStance", true);
                    SubState = RotateState((Target as MonoBehaviour).transform, behaviourToken);
                    //TODO If ranged weapon, find cover

                    //Get associated skills for the action
                    const float BaseAttackTime = 2.0f;
                    const float skillImpactCoefficient = 10;

                    float primarySpeedImpact = 1 + skills[0].GetAdjustedLevel("Speed") / skillImpactCoefficient;
                    float secondarySpeedImpact = skills.Length > 1 ? 1 + skills[1].GetAdjustedLevel("Speed") / skillImpactCoefficient * skills[1].SecondaryTypeCoefficient : 1;
                    float attackTime = BaseAttackTime / (attackSpeed * primarySpeedImpact * secondarySpeedImpact);

                    //Get total modifier for functionality. If all related limbs are fine, this should be 1.
                    KeyValuePair<Health.PartFunctions, float>[] speedFunctionData = EntitySelf.EntityComponents.Health.GetPartFunctions(attackFunctions.SpeedFunctions.Keys.ToArray());
                    float totalSpeedFunctionality = 1;
                    for (int i = 0; i < speedFunctionData.Length; i++)
                    {
                        float statImpact = attackFunctions.SpeedFunctions[speedFunctionData[i].Key];
                        totalSpeedFunctionality *= Mathf.Pow(speedFunctionData[i].Value, statImpact);
                    }

                    //Wait for attack time
                    Task<bool> waitAttack = WaitForAttack(attackTime, waitTokenSource.Token);
                    await waitAttack.WaitAsync(waitTokenSource.Token);
                    behaviourToken.ThrowIfCancellationRequested();

                    //Get total modifier for functionality. If all related limbs are fine, this should be 1.
                    KeyValuePair<Health.PartFunctions, float>[] hitFunctionData = EntitySelf.EntityComponents.Health.GetPartFunctions(attackFunctions.HitFunctions.Keys.ToArray());
                    float totalHitFunctionality = 1;
                    for (int i = 0; i < hitFunctionData.Length; i++)
                    {
                        float statImpact = attackFunctions.HitFunctions[hitFunctionData[i].Key];
                        totalHitFunctionality *= Mathf.Pow(hitFunctionData[i].Value, statImpact);
                    }

                    //Get the relevant skill impacts for the action
                    float primaryHitImpact = 1 + skills[0].GetAdjustedLevel("HitChance") / skillImpactCoefficient;
                    float secondaryHitImpact = skills.Length > 1 ? 1 + skills[1].GetAdjustedLevel("HitChance") / skillImpactCoefficient * skills[1].SecondaryTypeCoefficient : 1;


                    //Finalise the hit chances
                    float userAttack = primaryHitImpact * secondaryHitImpact;

                    if (EntitySelf is Actor actor)
                    {
                        if (Target.GetDodge(userAttack, actor) || Target.GetBlock(userAttack, actor))
                        {
                            AttackEvent?.Invoke(EntitySelf, Target as Entity, weapon, true, false);
                            continue;
                        }


                    }

                    //TODO adjust critical chance based on skills and weapon
                    float criticalChance = 0.5f;
                    bool critical = UnityEngine.Random.value <= criticalChance;

                    //Get total relevant body functionality
                    //Get total relevant stat bonuses from StatsAndSkills.cs
                    //Get total relevant skill bonuses
                    KeyValuePair<Health.PartFunctions, float>[] damageFunctionData = EntitySelf.EntityComponents.Health.GetPartFunctions(attackFunctions.DamageFunctions.Keys.ToArray());
                    float totalDamageFunctionality = 1;
                    for (int i = 0; i < damageFunctionData.Length; i++)
                    {
                        float statImpact = attackFunctions.DamageFunctions[damageFunctionData[i].Key];
                        totalDamageFunctionality *= Mathf.Pow(damageFunctionData[i].Value, statImpact);
                    }

                    float primaryDamageImpact = 1 + (skills[0].GetAdjustedLevel("Damage") / skillImpactCoefficient);
                    float secondaryDamageImpact = skills.Length > 1 ? 1 + skills[1].GetAdjustedLevel("Damage") / skillImpactCoefficient * skills[1].SecondaryTypeCoefficient : 1;

                    float totalDamage = UnityEngine.Random.Range(0.75f, 1.25f) * (damage * totalDamageFunctionality * primaryDamageImpact * secondaryDamageImpact);

                    Target.Damage(damage, critical, damageType);
                    AttackEvent?.Invoke(EntitySelf, Target as Entity, weapon, false, critical, new KeyValuePair<Weapon.DamageTypesEnum, float>(damageType, damage));
                }
                catch (OperationCanceledException)
                {
                    if (EntitySelf is Actor actor)
                    {
                        actor.OnBlock -= BlockState;
                        actor.OnDodge -= DodgeState;
                    }
                    EntitySelf.animator.SetBool("MeleeStance", false);
                    break;
                }
            }
            EndState();
        }

        public override void EndState()
        {
            if (EntitySelf is Actor actor)
            {
                actor.OnBlock -= BlockState;
                actor.OnDodge -= DodgeState;
            }
            EntitySelf.animator.SetBool("MeleeStance", false);
            base.EndState();
        }

        protected void GetStats(out float range, out float attackSpeed, out float damage, out StatsAndSkills.Skill[] skills, out Weapon.AttackData attackFunctions, out Weapon.DamageTypesEnum damageType, out Weapon weapon)
        {
            EquippableItem item = EntitySelf.EntityComponents.Equipment.Equipped?[(int)Equipment.Slots.Weapon];
            if(item == null) item = EntitySelf.EntityComponents.Equipment.Equipped?[(int)Equipment.Slots.SecondaryWeapon];

            if (item && item is Weapon w)
            {
                weapon = w;
                range = weapon.Stats.GetStat<float>(ItemTypes.StatsEnum.Range);
                attackSpeed = weapon.Stats.GetStat<float>(ItemTypes.StatsEnum.AttackSpeed);
                damage = weapon.Stats.GetStat<float>(ItemTypes.StatsEnum.Damage);
                skills = EntitySelf.EntityComponents.Stats.GetItemSkills(item.Subtype);
                attackFunctions = weapon.AttackFunctions;
                //TODO get actual damage type
                damageType = Weapon.DamageTypesEnum.Sharp;
            }
            else
            {
                weapon = null;
                range = (EntitySelf as Actor).UnarmedData.Range;
                attackSpeed = (EntitySelf as Actor).UnarmedData.AttackSpeed;
                damage = (EntitySelf as Actor).UnarmedData.Damage;
                skills = new StatsAndSkills.Skill[] { EntitySelf.EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Unarmed] };
                attackFunctions = (EntitySelf as Actor).UnarmedData.Data;
                damageType = (EntitySelf as Actor).UnarmedData.DamageType;
            }
        }

        protected bool ProximityCheck(float distance)
        {
            return Vector3.Distance(EntitySelf.transform.position, (Target as MonoBehaviour).transform.position) < distance;
        }

        protected async Task<bool> WaitForAttack(float attackTime, CancellationToken token)
        {
            try
            {
                float attackTimemiliseconds = attackTime * 1000f;

                await Task.Delay(Mathf.RoundToInt(attackTimemiliseconds / 2f), token);  
                IsCommitted = true;
                EntitySelf.animator.SetTrigger("MeleeAttack");
                float attackAnimationLength = EntitySelf.animator.runtimeAnimatorController.animationClips.First(c => c.name.Contains("Attack_Overhead")).length;
                EntitySelf.animator.SetFloat("AttackSpeed", attackAnimationLength / attackTime * 2);
                if (EntitySelf is IDamageable damageable) 
                {   Target.Retaliate(damageable); }
                await Task.Delay(Mathf.RoundToInt(attackTimemiliseconds / 2f), token);

                IsCommitted = false;
                EntitySelf.animator.SetBool("MeleeAttack", false);
                return true;
            }
            catch (OperationCanceledException)
            {
                IsCommitted = false;
                EntitySelf.animator.SetBool("MeleeAttack", false);
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected RotateTowards RotateState(Transform target, CancellationToken token)
        {
            RotateTowards rotateTowards = new RotateTowards(EntitySelf, null, (Target as MonoBehaviour).transform)
            {
                Token = token
            };
            rotateTowards.StartState();
            return rotateTowards;
        }

        protected async void DodgeState(Actor attacker)
        {
            if (!IsCommitted)
            {
                BehaviourTokenSource.Cancel();
                BehaviourTokenSource = CancellationTokenSource.CreateLinkedTokenSource(StateTokenSource.Token);
                Dodge dodge = new Dodge(EntitySelf, null, Target as Entity);
                {
                    dodge.Token = Token;
                }
                dodge.StartState();
                await dodge.StateCompleted.WaitAsync();
                StateBehaviour(BehaviourTokenSource.Token);
            }
        }

        protected async void BlockState(Actor attacker)
        {
            if (!IsCommitted)
            {
                BehaviourTokenSource.Cancel();
                BehaviourTokenSource = CancellationTokenSource.CreateLinkedTokenSource(StateTokenSource.Token);
                Block block = new Block(EntitySelf, null, Target as Entity);
                {
                    block.Token = Token;
                }
                block.StartState();
                await block.StateCompleted.WaitAsync();
                StateBehaviour(BehaviourTokenSource.Token);
            }
        }

        public override object Clone()
        {
            return new Attack(EntitySelf, Callback, Target) { Token = Token };
        }
    }
}
