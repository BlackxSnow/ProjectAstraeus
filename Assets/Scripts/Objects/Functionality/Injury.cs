using Medical.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UnityEngine;
using static Medical.Health;

namespace Medical
{
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
            ParentHealth.Injuries.Add(this);
            RemainingTime = BaseHealTime;
            if (BaseHealTime == 0) return;
            HealTimer = new Utility.Timer(1, new Utility.Timer.ElapsedDelegate(AdvanceHeal), true);
            HealTimer.Start();
        }
        public void Tend(float quality)
        {
            Tended = true;
            TendQuality = quality;
            //TODO End conditions that will end on tending
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
            if (HealTimer != null) HealTimer.Stop();
            Part.Injuries.Remove(this);
            CharacterHealth.Injuries.Remove(this);
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
            foreach (KeyValuePair<string, float> kvp in Time)
            {
                if (kvp.Value > 1)
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
}
