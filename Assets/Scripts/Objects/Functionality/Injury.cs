using Medical.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;
using static Medical.Health;

namespace Medical
{
    [Serializable]
    public class Injury
    {
        public static float MaxSeverity;
        public static float MinSeverity;

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
        public List<Condition.ConditionData> Conditions;

        //Base time to heal. 0 is never; measured in real minutes. Remaining time is in seconds
        public float BaseHealTime;

        //Percentage functionality restored on tend. eg. 0.75 value results in a 100% tend quality reducing Function Modifiers to 25% of original penalty (0.6 penalty => 0.15)
        public float MaxTendFunctionality;
        //Percentage of remaining duration negated by 100% quality tend
        public float MaxTendHealMultiplier;

        public int SeverityCost;
        public bool Overriding;

        //For internal use
        public bool Tended;
        public float TendQuality;
        public float RemainingTime;
        public float EffectiveRemainingTime;
        public Health CharacterHealth;
        public BodyPart Part;
        public List<Condition> ActiveConditions = new List<Condition>();
        public List<KeyValuePair<float, int>> ConditionStartTimes = new List<KeyValuePair<float, int>>();
        private Utility.Timer HealTimer;
        private static float HealInterval = 0.1f;

        public Injury Clone()
        {
            Injury Result = (Injury)MemberwiseClone();
            Result.ActiveConditions = new List<Condition>();
            Result.ConditionStartTimes = new List<KeyValuePair<float, int>>();
            Result.HealTimer = null;
            Result.Conditions = Utility.CloneList<Condition.ConditionData>(Conditions);
            return Result;
        }
        public void Init(Health ParentHealth, BodyPart ParentPart)
        {
            CharacterHealth = ParentHealth;
            Part = ParentPart;
            ParentHealth.Injuries.Add(this);
            RemainingTime = BaseHealTime;

            if (BaseHealTime > 0)
            {
                HealTimer = new Utility.Timer(HealInterval, new Utility.Timer.ElapsedDelegate(AdvanceHeal), true);
                HealTimer.Start();
            }
            foreach (Condition.ConditionData condition in Conditions)
            {
                if(condition.StartTime > 0)
                {
                    ConditionStartTimes.Add(new KeyValuePair<float, int>(condition.StartTime, condition.ID));
                }
            }
            TestConditions();
            RunConditions();
        }
        public void Tend(float quality)
        {
            Tended = true;
            TendQuality = quality;
            foreach (Condition condition in ActiveConditions)
            {
                if (condition.EndOnTend)
                {
                    condition.EndEffect();
                }
            }
        }

        public async void RunConditions()
        {
            while (true)
            {
                if (ActiveConditions.Count > 0)
                {
                    foreach (Condition condition in ActiveConditions)
                    {
                        condition.RunEffect();
                    }
                }
                await Await.NextUpdate();
            }
        }

        public void AdvanceHeal(float ActualInterval)
        {
            float RestModifier = CharacterHealth.RestHealModifier;
            float TendModifier = Tended ? (1f / MaxTendHealMultiplier - 1f) * TendQuality + 1f : 1f;
            float ModifiedTotal = ActualInterval * RestModifier * TendModifier;
            EffectiveRemainingTime = RemainingTime / ModifiedTotal * ActualInterval;
            RemainingTime -= ModifiedTotal;
            if (RemainingTime <= 0f)
            {
                HealInjury();
                return;
            }
            TestConditions();
        }

        private void TestConditions()
        {
            List<KeyValuePair<float, int>> ForRemoval = new List<KeyValuePair<float, int>>();
            foreach (KeyValuePair<float, int> time in ConditionStartTimes)
            {
                if ((1 - RemainingTime / BaseHealTime) >= time.Key)
                {
                    Condition.ConditionData Data = Conditions.Find(c => c.ID == time.Value);
                    CharacterHealth.AddConditionWithRoll(Data, this, true);
                    ForRemoval.Add(time);
                }
            }
            foreach (KeyValuePair<float, int> obj in ForRemoval)
            {
                ConditionStartTimes.Remove(obj);
            }
        }

        public void HealInjury()
        {
            if (HealTimer != null) HealTimer.Stop();
            ActiveConditions.Clear();
            Part.Injuries.Remove(this);
            CharacterHealth.Injuries.Remove(this);
            CharacterHealth.InjuryCount--;
            CharacterHealth.InjuryCostSum -= SeverityCost;
            CharacterHealth.RefreshInjuries();
            CharacterHealth.RefreshConditions();
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
            foreach (KeyValuePair<string, float> kvp in Time)
            {
                if (kvp.Value > 1)
                {
                    SelectedTime = new KeyValuePair<string, float>(kvp.Key, Utility.RoundToNDecimals(kvp.Value, 1, Utility.RoundType.Ceil));
                    break;
                }
            }

            if (SelectedTime.Key == "") return "-1";
            string Result = $"{SelectedTime.Value}{SelectedTime.Key}";
            return Result;
        }

        public string GetDisplayName()
        {
            string Result;
            if(AppliesToBone)
            {
                Result = $"{Name} {Part.BoneName}";
            }
            else
            {
                Result = $"{Name} {Part.Name}";
            }
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

            DamageTypes = Utility.DeserializeEnumCollection<Weapon.DamageTypesEnum>(DamageTypes_S);
            DamageTypes_S.Clear();
        }
    }
}
