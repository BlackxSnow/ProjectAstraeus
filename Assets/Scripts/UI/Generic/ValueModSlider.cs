using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ItemModule.AdditionalModule;
using static ItemTypes;

public class ValueModSlider : MonoBehaviour
{
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI ValueText;
    public Slider ValueSlider;

    public ItemModule.AdditionalModule TargetModule;
    public ItemTypes.StatFlagsEnum TargetStat;

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
        ChangeValue();
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
        ValueText.text = string.Format("{0}", Utility.RoundToNDecimals(ValueSlider.value, 1));

        CraftingUI.UpdateKVPs(true, false);
    }
}
