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
        int InjuryCostSum = 0;
        public int InjuryCount = 0;

        [Serializable]
        public class Injury
        {
            public string Name;
            public string InternalID;
            public string Description;
            public bool AppliesToBone;
            
            
            public Sprite Icon;
            public string IconFile;

            //Serialised and deserialised collections since JSON does not handle Enums;
            //Valid functions and their modifiers
            public Dictionary<string, float> FunctionModifiers_S;
            public Dictionary<PartFunctions, float> FunctionModifiers;

            //Valid damage types to cause this injury
            public List<string> DamageTypes_S;
            public List<Weapon.DamageTypesEnum> DamageTypes;

            //Possible conditions to be inflicted and their chances
            public Dictionary<string, ConditionStruct> Conditions_S;
            public Dictionary<Condition.ConditionTypes, ConditionStruct> Conditions;

            //Base time to heal. 0 is never; measured in real minutes. Remaining time is in seconds
            public float BaseHealTime;

            //Percentage functionality restored on tend. eg. 0.75 value results in a 100% tend quality reducing Function Modifiers to 25% of original penalty (0.6 penalty => 0.15)
            public float MaxTendReduction;
            //Percentage of remaining duration negated by 100% quality tend
            public float MaxTendDurationReduction;
            
            public int SeverityCost;
            public bool Overriding;

            //Internal
            public bool Tended;
            public float TendQuality;
            public float RemainingTime;
            public float EffectiveRemainingTime;
            public Health CharacterHealth;
            public BodyPart Part;
            private Utility.Timer HealTimer;
            public Injury Clone()
            {
                Injury Result = (Injury)MemberwiseClone();
                return Result;
            }
            public void Init(Health ParentHealth, BodyPart ParentPart)
            {
                CharacterHealth = ParentHealth;
                Part = ParentPart;
                RemainingTime = BaseHealTime;
                if (BaseHealTime == 0) return;
                HealTimer = new Utility.Timer(1, new Utility.Timer.ElapsedDelegate(AdvanceHeal), true);
                HealTimer.Start();
            }

            public void AdvanceHeal()
            {
                float RestModifier = CharacterHealth.RestHealModifier;
                float TendModifier = Tended ? (1f / MaxTendDurationReduction - 1f) * TendQuality + 1f : 1f;
                float ModifiedTotal = 1f * RestModifier * TendModifier;
                EffectiveRemainingTime = RemainingTime / ModifiedTotal;
                RemainingTime -= ModifiedTotal;
                if (RemainingTime <= 0f)
                {
                    HealInjury();
                }
            }
            public void HealInjury()
            {
                HealTimer.Stop();
                Part.Injuries.Remove(this);
                CharacterHealth.InjuryCount--;
                CharacterHealth.InjuryCostSum -= SeverityCost;
                CharacterHealth.RefreshInjuries();
            }

            public string GetRemainingTime()
            {
                Dictionary<string, float> Time = new Dictionary<string, float>()
                {
                    { "h", EffectiveRemainingTime / 60f / 60f },
                    { "m", EffectiveRemainingTime / 60f },
                    { "s", EffectiveRemainingTime }
                };
                KeyValuePair<string, float> SelectedTime = new KeyValuePair<string, float>();
                foreach(KeyValuePair<string, float> kvp in Time)
                {
                    if(kvp.Value > 1)
                    {
                        SelectedTime = new KeyValuePair<string, float>(kvp.Key, Utility.RoundToNDecimals(kvp.Value, 1, Utility.RoundType.Ceil));
                        break;
                    }
                }


                string Result = $"{SelectedTime.Value}{SelectedTime.Key}";
                return Result;
            }
            //Resolve enums and clear old dictionaries
            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                Icon = DataManager.LoadSprite(IconFile, DataManager.InjuryIconsPath);
                RemainingTime = BaseHealTime;

                FunctionModifiers = Utility.DeserializeEnumCollection<PartFunctions, float>(FunctionModifiers_S);
                FunctionModifiers_S.Clear();

                Conditions = Utility.DeserializeEnumCollection<Condition.ConditionTypes, ConditionStruct>(Conditions_S);
                Conditions_S.Clear();

                DamageTypes = Utility.DeserializeEnumCollection<Weapon.DamageTypesEnum>(DamageTypes_S);
                DamageTypes_S.Clear();
            }
        }

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