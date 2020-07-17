using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using System.Runtime.Serialization;
using Medical.Conditions;
using System.Linq;
using Items;

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


            //TODO Shift conditions from overridden injury to overriding injury
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
            ClonedInjury.Part = ChosenPart;
            InjuryCount++;
            InjuryCostSum += ClonedInjury.SeverityCost;

            if (InjuriesChanged != null) InjuriesChanged.Invoke();
            CheckConditions(ClonedInjury);
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
        //TODO only roll for condition when its start time has passed
        private void CheckConditions(Injury injury)
        {
            List<int> ActivatedConditions = new List<int>();
            bool Added = false;
            foreach(Condition.ConditionData condition in injury.Conditions)
            {
                if (condition.Activated || condition.StartTime > 1 - (injury.RemainingTime / injury.BaseHealTime)) continue;

                Condition.ConditionData Parent = new Condition.ConditionData();
                if (condition.ParentID != 0)
                {
                    try
                    {
                        Parent = injury.Conditions.Find(c => c.ID == condition.ParentID);
                    } catch (ArgumentNullException e)
                    {
                        throw new Exception($"ParentID {condition.ParentID} does not exist in {injury.Name} conditions collection {e.Source}");
                    }
                }
                if (Parent.Activated == false && condition.ParentID != 0) continue;

                if (AddConditionWithRoll(condition, injury, false))
                {
                    Added = true;
                }
            }
            
            if (ConditionsChanged != null && Added) ConditionsChanged.Invoke();
        }

        private void RunConditions()
        {
            foreach (Condition condition in GlobalConditions)
            {
                condition.RunEffect();
            }
        }
    }

}