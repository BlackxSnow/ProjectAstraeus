using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private TextMeshProUGUI UI_Title;
    [SerializeField]
    private Slider UI_Slider;
    [SerializeField]
    private TMP_InputField UI_InputField;
#pragma warning restore 0649

    public delegate object GetDataDelegate();
    public delegate void SetDataDelegate(object data);

    public GetDataDelegate GetData;
    public SetDataDelegate SetData;

    private bool IsInitialised = false;
    public bool IsIntegerSlider { get; set; } = false;
    public void Initialise(string title, GetDataDelegate getData, SetDataDelegate setData)
    {
        if(!IsInitialised)
        {
            UI_Title.text = title;
            GetData = getData;
            SetData = setData;
            IsInitialised = true;
        }
    }

    private void Start()
    {
        UI_Slider.onValueChanged.AddListener(OnValueChanged);
        UI_InputField.onSubmit.AddListener((string s) => {  OnValueChanged(float.Parse(s)); });
        CheckReferences();
    }

    private void CheckReferences()
    {
        List<bool> checks = new List<bool>
            {
                UI_Title != null,
                UI_Slider != null,
                UI_InputField != null,
            };

        foreach (bool check in checks)
        {
            if (!check)
            {
                Debug.LogWarning("One or more serialized fields is null");
                return;
            }
        }
    }

    private void OnValueChanged(float value)
    {
        if(IsIntegerSlider)
        {
            SetData((int)value);
        }
        else
        {
            SetData(value);
        }
        
        UpdateValues();
    }

    public void UpdateValues()
    {
        float value;
        if(IsIntegerSlider)
        {
            value = (int)GetData();
        }
        else
        {
            value = (float)GetData();
        }
        UI_Slider.value = value;
        UI_InputField.text = Utility.Math.RoundToNDecimals(value, 2).ToString();
    }

    public void SetSliderBounds(float min, float max)
    {
        UI_Slider.minValue = min;
        UI_Slider.maxValue = max;
    }

    public void SetSliderMode(bool integersOnly)
    {
        UI_Slider.wholeNumbers = integersOnly;
    }
}
