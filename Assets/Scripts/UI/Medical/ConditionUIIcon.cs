using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConditionUIIcon : MedicalUIIcon
    {
        public Medical.Conditions.Condition RefCondition;

        private void Update()
        {
            if (Initialised)
                UpdateUI();
        }

        public void Init(Medical.Conditions.Condition Target)
        {
            RefCondition = Target;

            float ConditionSeverity = RefCondition.Severity * RefCondition.AbsoluteDuration;
            float SeverityGradient = Mathf.Clamp01(ConditionSeverity / (RefCondition.CharacterHealth.MaxHitPoints * 0.5f));
            Background.color = StyleSheet.Colours.Gradients.Severity.Evaluate(SeverityGradient);
            Foreground.sprite = RefCondition.GetIcon();

            Initialised = true;
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            DurationBar.fillAmount = RefCondition.RemainingTime / RefCondition.AbsoluteDuration;
        }
    } 
}
