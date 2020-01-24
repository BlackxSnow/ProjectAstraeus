using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Medical.Conditions;

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
        public delegate void HealthChangedHandler(float Amount);
        public event HealthChangedHandler HealthChanged;

        public static List<Injury> LoadedInjuries;

        public float MaxHitPoints = 100;
        public float HitPoints = 100;

        public List<Condition> ActiveConditions = new List<Condition>(); 
        public List<BodyPart> Body = new List<BodyPart>();
        public List<PartFunctions> ValidFunctions = new List<PartFunctions>();

        public void Init()
        {
            GetValidFunctions();
        }

        private void Update()
        {
            RunConditions();
        }

        //TODO Add damagetype for injury restrictions; Die
        public void Damage(float Amount, bool Critical/*, DamageType*/)
        {
            HitPoints -= Amount;
            if (HealthChanged != null) HealthChanged.Invoke(Amount);

            if (HitPoints <= 0)
            {
                //Die
            }

            if (Critical)
            {
                float Severity = UnityEngine.Random.Range(0, 10) + Mathf.Log(Mathf.Max(Amount,1),2) + (InjuryCostSum / 10);
                AddInjury(Severity);
            }
        }

        public void AddCondition(Condition.ConditionTypes TypeEnum, ConditionStruct Data)
        {
            Type ConditionType = TypeAttribute.GetStoredData(typeof(Condition.ConditionTypes), TypeEnum).Type;
            Condition Instance = (Condition)Activator.CreateInstance(ConditionType);
            Instance.Init(Data, this);
            ActiveConditions.Add(Instance);
        }
    }

}