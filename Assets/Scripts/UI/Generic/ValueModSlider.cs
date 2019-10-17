using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ItemModule.AdditionalModule;

public class ValueModSlider : MonoBehaviour
{
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI ValueText;
    public Slider ValueSlider;

    public ItemModule.AdditionalModule TargetModule;
    public ModifiableStats TargetStat;

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
        ref float TargetValue = ref GetTargetValue();

        TargetValue = ValueSlider.value;
        ValueText.text = string.Format("{0}", Utility.RoundToNDecimals(ValueSlider.value, 1));

        CraftingUI.UpdateKVPs(true, false);
    }

    ref float GetTargetValue()
    {
        switch (TargetStat)
        {
            case ModifiableStats.Thickness:
                return ref (TargetModule as Plating).Thickness;
            case ModifiableStats.Material:
                throw new ArgumentException("Material should not be passed to GetTargetValue");
            case ModifiableStats.Power:
                return ref (TargetModule as Reactor).Power;
            case ModifiableStats.Shield:
                return ref (TargetModule as Shielding).Shield;
            default:
                throw new ArgumentException(string.Format("Unhandled case in GetTargetValue: {0}", TargetStat));
        }
    }
}
