using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Modules;

namespace UI
{
    namespace Crafting
    {
        public class ModuleSlider : MonoBehaviour
        {
            public TextMeshProUGUI LabelText;
            public TextMeshProUGUI ValueText;
            public Slider ValueSlider;

            public AdditionalModule TargetModule;
            public ItemTypes.StatsEnum TargetStat;

            public struct MinMax
            {
                public float Min;
                public float Max;

                public MinMax(float Min, float Max)
                {
                    this.Min = Min;
                    this.Max = Max;
                }
            }

            private void Start()
            {
                TargetModule = CraftingUI.CurrentModule;
                ValueSlider.value = TargetModule.GetStat<float>(TargetStat);
                ValueText.text = string.Format("{0}", Utility.RoundToNDecimals(ValueSlider.value, 2));
            }

            public MinMax ValueBounds;

            public void SetSliderValues()
            {
                ValueSlider.minValue = ValueBounds.Min;
                ValueSlider.maxValue = ValueBounds.Max;
            }

            public void ChangeValue()
            {
                //OnValueChanged update ValueText and the associated value on TargetModule

                TargetModule.SetStat(TargetStat, ValueSlider.value);
                ValueText.text = string.Format("{0}", Utility.RoundToNDecimals(ValueSlider.value, 2));

                CraftingUI.UpdateKVPs(true, true);
            }
        }
    }
}
