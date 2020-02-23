using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityAsync;

namespace Medical
{
    namespace Conditions
    {
        public class Condition
        {
            public class ConditionData : ICloneable
            {
                public int ID; //Start at 1. Only used for ParentID restriction
                public int ParentID; //0 == None, determines if a condition should only become active if its parent also did.
                public string DisplayName;
                public string ConditionType_S; //Relates to ConditionType enum
                public float Chance;
                public float Severity;
                public bool GlobalCondition; //Is the condition specific to this injury, or across the whole body?
                public float StartTime; //Flat seconds, or percentage of injury heal time before this condition is rolled
                public float Duration; //Either flat second duration, or percentage of injury heal time depending on GlobalCondition value (Global conditions are in seconds)
                public bool EndOnTend;

                //For internal use
                public ConditionTypes ConditionType;
                public bool Activated;

                [OnDeserialized]
                public void OnDeserialised(StreamingContext context)
                {
                    Activated = false;
                    ConditionType = (ConditionTypes)Enum.Parse(typeof(ConditionTypes), ConditionType_S);
                }

                public object Clone()
                {
                    object Clone = MemberwiseClone();
                    return Clone;
                }
            }

            public enum ConditionTypes
            {
                [Type(typeof(Bleeding))]
                Bleeding
            }

            public Utility.Timer HealTimer;
            public Health CharacterHealth;
            public float Severity { get; protected set; }
            public float Duration { get; protected set; }
            public float AbsoluteDuration { get; protected set; }
            public float StartTime { get; protected set; }
            public Sprite Icon;

            public bool GlobalCondition { get; protected set; }
            public Injury injury;

            public string DisplayName { get; protected set; }
            public string Description { get; protected set; }
            public bool EndOnTend { get; protected set; }
            public static float HealInterval { get; } = 0.1f;

            public virtual void RunEffect() { }
            public async virtual void EndEffect()
            {
                HealTimer.Stop();
                await Await.NextLateUpdate();
                if (GlobalCondition)
                {
                    CharacterHealth.GlobalConditions.Remove(this);
                } else
                {
                    injury.ActiveConditions.Remove(this);
                    CharacterHealth.AllConditions.Remove(this);
                }

                CharacterHealth.RefreshConditions();
            }

            public virtual Sprite GetIcon()
            {
                return null;
            }
            public float RemainingTime;
            public virtual void Init(ConditionData Data, Health CharacterHealth, Injury injury = null)
            {
                DisplayName = Data.DisplayName;
                Severity = Data.Severity;
                Duration = Data.Duration;
                StartTime = Data.StartTime;
                this.injury = injury;
                this.CharacterHealth = CharacterHealth;
                GlobalCondition = Data.GlobalCondition;
                EndOnTend = Data.EndOnTend;

                if(GlobalCondition)
                {
                    RemainingTime = Duration;
                    AbsoluteDuration = Duration;
                }
                else
                {
                    float ElapsedTimePercent = (injury.BaseHealTime - injury.RemainingTime) / injury.BaseHealTime;
                    RemainingTime = (Duration - (ElapsedTimePercent - StartTime)) * injury.BaseHealTime;
                    AbsoluteDuration = Duration * injury.BaseHealTime;
                }

                HealTimer = new Utility.Timer(HealInterval, new Utility.Timer.ElapsedDelegate(AdvanceHeal), true);
                HealTimer.Start();
            }

            public Utility.TimeSpan EffectiveRemainingTime = new Utility.TimeSpan();
            public virtual void AdvanceHeal(float ActualInterval)
            {
                float RestModifier = CharacterHealth.RestHealModifier;
                float ModifiedTotal = ActualInterval * RestModifier;
                RemainingTime -= ModifiedTotal;
                EffectiveRemainingTime = Utility.TimeSpan.FromSeconds(RemainingTime / ModifiedTotal * ActualInterval);
                if (RemainingTime <= 0f)
                {
                    EndEffect();
                }
            }
            public string GetRemainingTime()
            {
                KeyValuePair<string, float> TimeUnit = EffectiveRemainingTime.GetLargestUnit();
                string Result = $"{Utility.RoundToNDecimals(TimeUnit.Value,1)}{TimeUnit.Key}";
                return Result;
            }

            public string GetDisplayName()
            {
                string Result;
                if (GlobalCondition)
                {
                    Result = DisplayName;
                }
                else
                {
                    Result = $"{DisplayName} {injury.Part.Name}";
                }
                return Result;
            }

            public Condition() { }
        } 
    }
}
