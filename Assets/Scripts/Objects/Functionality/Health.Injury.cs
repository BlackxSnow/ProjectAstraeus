using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using System.Runtime.Serialization;
using Medical.Conditions;
using System.Linq;

namespace Medical
{
    public partial class Health
    {
        public int InjuryCostSum = 0;
        public int InjuryCount = 0;

        private void GetValidFunctions()
        {
            ValidFunctions.Clear();
            foreach (BodyPart part in Body)
            {
                foreach (KeyValuePair<PartFunctions, float> function in part.Functions)
                {
                    if (!ValidFunctions.Contains(function.Key))
                    {
                        ValidFunctions.Add(function.Key);
                    }
                }
            }
        }

        public void InjuryCostCount()
        {
            InjuryCount = 0;
            InjuryCostSum = 0;
            foreach (BodyPart Part in Body)
            {
                foreach (Injury injury in Part.Injuries)
                {
                    InjuryCount++;
                    InjuryCostSum += injury.SeverityCost;
                }
            }
        }

        float AccumulatedSeverity = 0;
        public void AddInjury(float Severity, Weapon.DamageTypesEnum DamageType)
        {
            Severity += AccumulatedSeverity;
            AccumulatedSeverity = 0;

            BodyPart ChosenPart = FindValidPart(Severity);
            List<int> ValidInjuryIndices = FindValidInjuries(ChosenPart, Severity, DamageType);
            if (ValidInjuryIndices == null) return;

            //Apply random injury
            int IRoll = UnityEngine.Random.Range(0, ValidInjuryIndices.Count - 1);
            int ChosenInjury = ValidInjuryIndices[IRoll];



            if (LoadedInjuries[ChosenInjury].Overriding && ChosenPart.Injuries.Count != 0)
            {
                InjuryCount -= ChosenPart.Injuries.Count;
                foreach(Injury injury in ChosenPart.Injuries)
                {
                    InjuryCostSum -= injury.SeverityCost;
                    Injuries.Remove(injury);
                }
                ChosenPart.Injuries.Clear();
            }
            Injury ClonedInjury = LoadedInjuries[ChosenInjury].Clone();
            ClonedInjury.Init(this, ChosenPart);
            ChosenPart.Injuries.Add(ClonedInjury);

            InjuryCount++;
            InjuryCostSum += LoadedInjuries[ChosenInjury].SeverityCost;

            if (InjuriesChanged != null) InjuriesChanged.Invoke();
            CheckConditions(LoadedInjuries[ChosenInjury]);
        }

        //TODO Kill character when all possible injuries are applied
        private BodyPart FindValidPart(float Severity)
        {
            //Create List of valid part indices
            //Find Part based on part hit chance
            float AccumulatedChance = 0;
            float PartChanceTotal = 0;
            BodyPart ChosenPart = null;
            List<BodyPart> ValidParts = new List<BodyPart>();
            List<float> Chances = new List<float>();
            foreach (BodyPart part in Body)
            {
                if (part.SeverityCost > Severity || part.Injuries.Any(n => n.Overriding))
                {
                    continue;
                }
                ValidParts.Add(part);
                Chances.Add(AccumulatedChance + part.InjuryChance);
                AccumulatedChance += part.InjuryChance;
                PartChanceTotal += part.InjuryChance;
            }
            float Roll = UnityEngine.Random.Range(0f, Chances.Count - 1f);
            for (int i = 0; i < Chances.Count; i++)
            {
                if (Roll <= Chances[i])
                {
                    ChosenPart = ValidParts[i];
                    break;
                }
            }

            if (ChosenPart == null)
                throw new Exception($"No part rolled for roll {Roll} in array length {Chances.Count}");
            return ChosenPart;
        }
        private List<int> FindValidInjuries(BodyPart ChosenPart, float Severity, Weapon.DamageTypesEnum DamageType)
        {
            //Create list of valid injuries
            float AdjustedSeverity = Severity - ChosenPart.SeverityCost;
            List<int> ValidInjuryIndices = new List<int>();
            int i = 0;
            bool Floor = false;
            if(LoadedInjuries.Exists(n => n.SeverityCost > AdjustedSeverity / 2f))
            {
                Floor = true;
            }
            foreach (Injury injury in LoadedInjuries)
            {

                if (injury.SeverityCost <= AdjustedSeverity && 
                    ((Floor && injury.SeverityCost >= AdjustedSeverity / 2f) || !Floor) && //Is the severity between the floor and ceiling, and do we need a floor?
                    injury.FunctionModifiers.Any(f => ChosenPart.Functions.ContainsKey(f.Key)) && //Does the injury share functionality with the target part?
                    !ChosenPart.Injuries.Exists(n => n.Name == injury.Name) && //Does the injury already exist on the part?
                    injury.DamageTypes.Contains(DamageType)) //Is the injury caused by the input damage type?
                {
                    ValidInjuryIndices.Add(i);
                }
                i++;
            }
            if (ValidInjuryIndices.Count == 0)
            {
                AccumulatedSeverity = Severity;
                return null;
            }
            return ValidInjuryIndices;
        }
        private void CheckConditions(Injury injury)
        {
            bool Added = false;
            foreach(KeyValuePair<Condition.ConditionTypes, ConditionStruct> kvp in injury.Conditions)
            {
                float Roll = UnityEngine.Random.value;
                if (Roll <= kvp.Value.Chance)
                {
                    AddCondition(kvp.Key, kvp.Value, false);
                    Added = true;
                }
            }
            if (ConditionsChanged != null && Added) ConditionsChanged.Invoke();
        }


        private void RunConditions()
        {
            foreach (Condition condition in ActiveConditions)
            {
                condition.RunEffect();
            }
        }

        //TODO Add ability to remove injuries. Reduce InjuryCount and InjuryCostSum, Invoke InjuriesChanged
        public void RemoveInjury(BodyPart Part, string InjuryName)
        {
            
        }
    }

}