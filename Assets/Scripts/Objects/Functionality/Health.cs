using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;

namespace Medical
{
    public class Health : MonoBehaviour
    {
        public enum PartFunctions
        {
            Vision,
            Flight,
            Locomotion,
            Manipulation,
            Control
        }
        public static List<Injury> LoadedInjuries;

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

            //Base time to heal. 0 is never; measured in days.
            public float BaseHealTime;
            public int SeverityCost;

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
            }
        }

        public struct BodyPart
        {
            public string Name;
            
            //This may be changed to allow for > 100% totals:
            //Float represents the relative impact on that function, 0-1 representing percentage
            //eg. 100% on four manipulators means each contributes 25% split. 50% on one, 100% on three results in: 12.5%, 29.16%*3
            public Dictionary<PartFunctions, float> Functions;
            public Dictionary<string, float> S_Functions;

            public List<Injury> Injuries;

            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                Functions = new Dictionary<PartFunctions, float>();
                Injuries = new List<Injury>();
                foreach(KeyValuePair<string, float> Function in S_Functions)
                {
                    PartFunctions FunctionEnum = (PartFunctions)Enum.Parse(typeof(PartFunctions), Function.Key);
                    Functions.Add(FunctionEnum, Function.Value);
                }
            }
        }

        public float MaxHitPoints = 100;
        public float HitPoints = 100;

        public List<BodyPart> Body = new List<BodyPart>();
        public List<PartFunctions> ValidFunctions = new List<PartFunctions>();

        int InjuryCostSum = 0;
        float CostRemainder = 0;

        public void Init()
        {
            GetValidFunctions();
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
            InjuryCostSum = 0;
            foreach(BodyPart Part in Body)
            {
                foreach(Injury injury in Part.Injuries)
                {
                    InjuryCostSum += injury.SeverityCost;
                }
            }
        }

        //TODO Add damagetype for injury restrictions
        public void Damage(float Amount, bool Critical/*, DamageType*/)
        {
            HitPoints -= Amount;
            if (HitPoints <= 0)
            {
                //Die
                //return;
            }

            if (Critical)
            {
                float Severity = UnityEngine.Random.Range(0, 10) + Mathf.Log(Mathf.Max(Amount,1),2) + (InjuryCostSum / 10);
                AddInjury(Severity);
            }
        }


        //TODO Disallow injuries when all valid body parts already have that injury
        public void AddInjury(float Severity)
        {
            Severity += CostRemainder;
            int ClosestSeverity = 0;
            int ClosestSeverityIndex = -1;
            //Find injury with closest severity cost
            for(int i = 0; i < LoadedInjuries.Count; i++)
            {
                //Ensure injury is valid for types of function this creature has
                bool ValidFunction = false;
                foreach(KeyValuePair<PartFunctions, float> function in LoadedInjuries[i].FunctionModifiers)
                {
                    if (ValidFunctions.Contains(function.Key)) ValidFunction = true;
                }

                int CurrentSeverity = LoadedInjuries[i].SeverityCost;
                if(CurrentSeverity > ClosestSeverity && CurrentSeverity <= Severity && ValidFunction)
                {
                    ClosestSeverity = CurrentSeverity;
                    ClosestSeverityIndex = i;
                }
            }

            if (ClosestSeverityIndex == -1) throw new Exception($"No valid injuries were found for severity {Severity}");

            InjuryCostSum += ClosestSeverity;
            CostRemainder = Severity - ClosestSeverity;
            //Construct list of valid bodyparts, add injury to random valid part
            List<int> ValidPartIndices = new List<int>();
            for(int i = 0; i < Body.Count; i++)
            {
                if (Body[i].Injuries.Exists(n => n.InternalID == LoadedInjuries[ClosestSeverityIndex].InternalID)) continue;

                foreach(KeyValuePair<PartFunctions, float> function in LoadedInjuries[ClosestSeverityIndex].FunctionModifiers)
                {
                    if (ValidFunctions.Contains(function.Key) && !ValidPartIndices.Contains(i))
                    {
                        ValidPartIndices.Add(i);
                        break;
                    }
                }
            }
            if (ValidPartIndices.Count == 0) throw new Exception($"No valid part found for injury {LoadedInjuries[ClosestSeverityIndex]}");
            int TargetIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0, ValidPartIndices.Count - 1));
            Body[TargetIndex].Injuries.Add(LoadedInjuries[ClosestSeverityIndex]);
        }

        public void RemoveInjury(BodyPart Part, string InjuryName)
        {
            //TODO add functionality, InjuryCostSum -= severity cost of injury. Reset Remainder
        }
    }

}