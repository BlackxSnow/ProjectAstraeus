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

            public Utility.Timer ConditionEnd;
            public Health CharacterHealth;
            public float Severity;
            public float Duration; //0 = until tended, duration in seconds
            public Sprite Icon;

            public string DisplayName;
            public string Description;

            public virtual void RunEffect() { }
            public virtual void EndEffect()
            {
                CharacterHealth.ActiveConditions.Remove(this);
            }

            protected virtual Sprite GetIcon()
            {
                return null;
            }
            public virtual void Init(Health.ConditionStruct Data, Health CharacterHealth)
            {
                DisplayName = Data.DisplayName;
                Severity = Data.Severity;
                Duration = Data.Duration;
                this.CharacterHealth = CharacterHealth;

                Utility.Timer.ElapsedDelegate effectDelegate = EndEffect;
                ConditionEnd = new Utility.Timer(Duration, effectDelegate);
                ConditionEnd.Start();
            }

            public Condition() { }
        } 
    }
}
