using System.Collections;
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
        public struct ConditionStruct
        {
            public string DisplayName;
            public float Chance;
            public float Severity;
            public float Duration;

            public Dictionary<string, ConditionStruct> ChildConditions_S;
            public Dictionary<Condition.ConditionTypes, ConditionStruct> ChildConditions;
            //Resolve enums and clear old dictionaries
            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                if (ChildConditions_S == null) return;

                ChildConditions = Utility.DeserializeEnumCollection<Condition.ConditionTypes, ConditionStruct>(ChildConditions_S);
                ChildConditions_S.Clear();

            }
        }

        public delegate void UpdateHandler();
        public event UpdateHandler HealthChanged;
        public event UpdateHandler InjuriesChanged;
        public event UpdateHandler ConditionsChanged;

        public static List<Injury> LoadedInjuries;

        public float MaxHitPoints = 100;
        public float HitPoints = 100;

        public float RestHealModifier = 1.0f;

        public List<Condition> ActiveConditions = new List<Condition>(); 
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

        public void AddCondition(Condition.ConditionTypes TypeEnum, ConditionStruct Data, bool Refresh = true)
        {
            Type ConditionType = TypeAttribute.GetStoredData(typeof(Condition.ConditionTypes), TypeEnum).Type;
            Condition Instance = (Condition)Activator.CreateInstance(ConditionType);
            Instance.Init(Data, this);
            ActiveConditions.Add(Instance);
            if (ConditionsChanged != null && Refresh)
            {
                ConditionsChanged.Invoke();
            }
        }
    }

}