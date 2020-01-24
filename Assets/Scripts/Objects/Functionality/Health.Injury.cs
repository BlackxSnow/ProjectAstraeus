using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using Medical.Conditions;
using System.Linq;

namespace Medical
{
    public partial class Health
    {
        int InjuryCostSum = 0;
        float CostRemainder = 0;
        public int InjuryCount = 0;
        public struct ConditionStruct
        {
            public string DisplayName;
            public float Chance;
            public float Severity;
            public float Duration;
        }

        [Serializable]
        public struct Injury
        {
            //TODO Add damage type restrictions
            public string Name;
            public string InternalID;
            public string Description;
            
            public Sprite Icon;
            public string IconFile;

            //Serialised and deserialised collections since JSON cannot handle Enums;
            public Dictionary<string, float> FunctionModifiers_S;
            public Dictionary<PartFunctions, float> FunctionModifiers;

            //Possible conditions to be inflicted
            public Dictionary<string, ConditionStruct> Conditions_S;
            public Dictionary<Condition.ConditionTypes, ConditionStruct> Conditions;

            //Base time to heal. 0 is never; measured in days.
            public float BaseHealTime;
            public int SeverityCost;
            public bool Overriding;

            //Resolve enums and clear old dictionaries
            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                Icon = DataManager.LoadSprite(IconFile, DataManager.InjuryIconsPath);
                FunctionModifiers = new Dictionary<PartFunctions, float>();
                foreach (KeyValuePair<string, float> FunctionMod in FunctionModifiers_S)
                {
                    PartFunctions Function = (PartFunctions)Enum.Parse(typeof(PartFunctions), FunctionMod.Key);
                    FunctionModifiers.Add(Function, FunctionMod.Value);
                }
                FunctionModifiers_S.Clear();

                Conditions = new Dictionary<Condition.ConditionTypes, ConditionStruct>();
                if (Conditions_S == null) return;
                foreach (KeyValuePair<string, ConditionStruct> kvp in Conditions_S)
                {
                    Condition.ConditionTypes ConType = (Condition.ConditionTypes)Enum.Parse(typeof(Condition.ConditionTypes), kvp.Key);
                    Conditions.Add(ConType, kvp.Value);
                }
                Conditions_S.Clear();
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

        //TODO Restrict based on damage type; Add severity cost to hitting parts;
        float AccumulatedSeverity = 0;
        public void AddInjury(float Severity)
        {
            //Create List of valid part indices
            //Find Part based on part hit chance
            Severity += AccumulatedSeverity;
            AccumulatedSeverity = 0;
            int i = 0;
            float AccumulatedChance = 0;
            float PartChanceTotal = 0;
            BodyPart ChosenPart = null;
            float[] Chances = new float[Body.Count];
            foreach(BodyPart part in Body)
            {
                if(part.Injuries.Any(n => n.Overriding))
                {
                    i++;
                    continue;
                }
                Chances[i] = AccumulatedChance + part.InjuryChance;
                AccumulatedChance += part.InjuryChance;
                PartChanceTotal += part.InjuryChance;
                i++;
            }
            float Roll = UnityEngine.Random.Range(0f, Chances.Length - 1f);
            for(i = 0; i < Chances.Length; i++)
            {
                if (Roll <= Chances[i])
                {
                    ChosenPart = Body[i];
                    break;
                }
            }

            if (ChosenPart == null) throw new Exception($"No part rolled for roll {Roll} in array {Chances}");

            //Create list of valid injuries
            List<int> ValidInjuryIndices = new List<int>();
            i = 0;
            foreach (Injury injury in LoadedInjuries)
            {
                //If there are common functions in part and injury, and the injury does not already exist in the part
                bool one = injury.SeverityCost <= Severity;
                bool two = injury.SeverityCost >= Severity / 2;
                bool three = injury.FunctionModifiers.Any(f => ChosenPart.Functions.ContainsKey(f.Key));
                bool four = !ChosenPart.Injuries.Exists(n => n.Name == injury.Name);
                Debug.Log("Pre if");
                if (injury.SeverityCost <= Severity && injury.SeverityCost >= Severity / 2 && injury.FunctionModifiers.Any(f => ChosenPart.Functions.ContainsKey(f.Key)) && !ChosenPart.Injuries.Exists(n => n.Name == injury.Name))
                {
                    ValidInjuryIndices.Add(i);
                }
                i++;
            }
            if (ValidInjuryIndices.Count == 0)
            {
                AccumulatedSeverity = Severity;
                return;
            }

            //Apply random injury
            int IRoll = UnityEngine.Random.Range(0, ValidInjuryIndices.Count - 1);
            int ChosenInjury = ValidInjuryIndices[IRoll];
            Debug.Log($"Attempting injury '{LoadedInjuries[IRoll].Name}' at '{ChosenPart.Name}'");
            ChosenPart.Injuries.Add(LoadedInjuries[ValidInjuryIndices[IRoll]]);
            InjuryCount++;
            InjuryCostSum += LoadedInjuries[ValidInjuryIndices[IRoll]].SeverityCost;
            CheckConditions(LoadedInjuries[ValidInjuryIndices[IRoll]]);
        }

        private void CheckConditions(Injury injury)
        {
            foreach(KeyValuePair<Condition.ConditionTypes, ConditionStruct> kvp in injury.Conditions)
            {
                float Roll = UnityEngine.Random.value;
                if (Roll <= kvp.Value.Chance)
                {
                    AddCondition(kvp.Key, kvp.Value);
                }
            }
        }

        private void RunConditions()
        {
            foreach (Condition condition in ActiveConditions)
            {
                condition.RunEffect();
            }
        }

        public void RemoveInjury(BodyPart Part, string InjuryName)
        {
            //TODO add functionality, InjuryCostSum -= severity cost of injury. Reset Remainder
        }
    }

}