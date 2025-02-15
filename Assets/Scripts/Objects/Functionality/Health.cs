﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Medical.Conditions;
using System.Runtime.Serialization;

namespace Medical
{
    public partial class Health : MonoBehaviour
    {
        public enum PartFunctions
        {
            Vision,
            Flight,
            Locomotion,
            Manipulation,
            Control
        }

        public delegate void UpdateHandler();
        public event UpdateHandler HealthChanged;
        public event UpdateHandler InjuriesChanged;
        public event UpdateHandler ConditionsChanged;

        public static List<Injury> LoadedInjuries;

        public float MaxHitPoints = 100;
        public float HitPoints = 100;

        public float RestHealModifier = 1.0f;

        public List<Condition> GlobalConditions = new List<Condition>();
        public List<Condition> AllConditions = new List<Condition>();
        public List<BodyPart> Body = new List<BodyPart>();
        public List<Injury> Injuries = new List<Injury>();
        public List<PartFunctions> ValidFunctions = new List<PartFunctions>();

        public void Init()
        {
            GetValidFunctions();
        }

        private void Update()
        {
            RunConditions();
        }

        public void RefreshConditions()
        {
            if (ConditionsChanged == null) return;
            ConditionsChanged.Invoke();
        }
        public void RefreshInjuries()
        {
            if (InjuriesChanged == null) return;
            InjuriesChanged.Invoke();
        }

        //TODO Add death
        public void Damage(float Amount, bool Critical, Weapon.DamageTypesEnum DamageType)
        {
            HitPoints -= Amount;
            if (HealthChanged != null) HealthChanged.Invoke();

            if (HitPoints <= 0)
            {
                //Die
            }

            if (Critical)
            {
                float Severity = UnityEngine.Random.Range(0, 10) + Mathf.Log(Mathf.Max(Amount,1),2) + (InjuryCostSum / 10);
                AddInjury(Severity, DamageType);
            }
        }

        public void AddCondition(Condition.ConditionData Data, Injury injury, bool Refresh = true)
        {
            Type ConditionType = TypeAttribute.GetStoredData(typeof(Condition.ConditionTypes), Data.ConditionType).Type;
            Condition Instance = (Condition)Activator.CreateInstance(ConditionType);
            Instance.Init(Data, this, injury);

            if (Data.GlobalCondition)
            {
                GlobalConditions.Add(Instance);
            }
            else
            {
                injury.ActiveConditions.Add(Instance);
            }
            AllConditions.Add(Instance);
            if (ConditionsChanged != null && Refresh)
            {
                ConditionsChanged.Invoke();
            }
        }
        public bool AddConditionWithRoll(Condition.ConditionData Data, Injury injury, bool Refresh = true)
        {
            float Roll = UnityEngine.Random.value;
            if (Roll <= Data.Chance)
            {
                AddCondition(Data, injury, Refresh);
                Data.Activated = true;
                return true;
            }
            return false;
        }
    }

}