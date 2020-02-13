using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Medical
{
    namespace Conditions
    {
        public class Condition
        {
            public enum ConditionTypes
            {
                [Type(typeof(Bleeding))]
                Bleeding
            }

            public Utility.Timer HealTimer;
            public Health CharacterHealth;
            public float Severity;
            public float Duration; //0 = until tended, duration in seconds
            public Sprite Icon;

            public string DisplayName;
            public string Description;

            public Dictionary<ConditionTypes, Health.ConditionStruct> ChildConditions = new Dictionary<ConditionTypes, Health.ConditionStruct>();

            public virtual void RunEffect() { }
            public virtual void EndEffect(bool CreateChilden = true)
            {
                HealTimer.Stop();
                if (ChildConditions != null && CreateChilden)
                {
                    foreach (KeyValuePair<ConditionTypes, Health.ConditionStruct> condition in ChildConditions)
                    {
                        CharacterHealth.AddCondition(condition.Key, condition.Value);
                    }
                }
                CharacterHealth.ActiveConditions.Remove(this);
                CharacterHealth.RefreshConditions();
            }

            protected virtual Sprite GetIcon()
            {
                return null;
            }
            public float RemainingTime;
            public virtual void Init(Health.ConditionStruct Data, Health CharacterHealth)
            {
                DisplayName = Data.DisplayName;
                Severity = Data.Severity;
                Duration = Data.Duration;
                RemainingTime = Duration;
                ChildConditions = Data.ChildConditions;
                this.CharacterHealth = CharacterHealth;

                if (RemainingTime == 0) return;
                HealTimer = new Utility.Timer(1f, new Utility.Timer.ElapsedDelegate(AdvanceHeal), true);
                HealTimer.Start();
            }

            public float EffectiveRemainingTime;
            public virtual void AdvanceHeal()
            {
                float RestModifier = CharacterHealth.RestHealModifier;
                float ModifiedTotal = 1f * RestModifier;
                EffectiveRemainingTime = RemainingTime / ModifiedTotal;
                RemainingTime -= ModifiedTotal;
                if (RemainingTime <= 0f)
                {
                    EndEffect();
                }
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
            public Condition() { }
        } 
    }
}
