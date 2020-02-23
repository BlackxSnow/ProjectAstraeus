using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public class InjuryUIIcon : MedicalUIIcon
    {

        public Image TendedIcon;

        private Medical.Injury RefInjury;

        private void Update()
        {
            if(Initialised)
                UpdateUI();
        }

        public void Init(Medical.Injury Target)
        {

            RefInjury = Target;

            float SeverityRange = Mathf.Max(Medical.Injury.MaxSeverity - Medical.Injury.MinSeverity, 1f);
            float SeverityTime = (RefInjury.SeverityCost - Medical.Injury.MinSeverity) / SeverityRange;
            Background.color = StyleSheet.Colours.Gradients.Severity.Evaluate(SeverityTime);
            Foreground.sprite = RefInjury.Icon;

            Initialised = true;
            UpdateUI();
        }

        protected override void UpdateUI()
        {
            DurationBar.fillAmount = RefInjury.RemainingTime / RefInjury.BaseHealTime;

            if (RefInjury.Tended)
            {
                TendedIcon.color = StyleSheet.Colours.Gradients.Quality.Evaluate(RefInjury.TendQuality);
            }
            else
            {
                TendedIcon.color = StyleSheet.Colours.AlphaError;
            }
        }
    }

}