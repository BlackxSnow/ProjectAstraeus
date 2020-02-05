using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Medical.Conditions;

public class MedicalDetailsHUD : MonoBehaviour
{


    public Actor SelectedActor;
    public GameObject InjuriesPanel;
    public GameObject ConditionsPanel;

    [Serializable]
    public struct HealthBarStruct
    {
        public RectTransform Background;
        public RectTransform Foreground;
        public TextMeshProUGUI Text;
    }

    public HealthBarStruct HealthBar;

    List<GameObject> DisplayedInjuries = new List<GameObject>();
    List<ToolTipData> DisplayedConditions = new List<ToolTipData>();

    public void Init()
    {
        SelectedActor.EntityComponents.Health.HealthChanged += UpdateHealthBar;
        SelectedActor.EntityComponents.Health.InjuriesChanged += DisplayInjuries;
        SelectedActor.EntityComponents.Health.ConditionsChanged += DisplayConditions;
        UpdateHealthBar();
    }
    int f = 5;
    private void Update()
    {
        if (f > 0)
        {
            UpdateHealthBar();
            f--;
        }
        if (CheckInjuries()) DisplayInjuries();
        if (CheckConditions()) DisplayConditions();
        
    }

    private void UpdateHealthBar()
    {
        if(HealthBar.Foreground == null)
        {
            SelectedActor.EntityComponents.Health.HealthChanged -= UpdateHealthBar;
            return;
        }
        if (f == 0)
        {
            f++;
        }
        float MaxHP = SelectedActor.EntityComponents.Health.MaxHitPoints;
        float CurrHP = SelectedActor.EntityComponents.Health.HitPoints;

        HealthBar.Text.text = $"{Utility.RoundToNDecimals(CurrHP,1)} / {MaxHP}";
        HealthBar.Foreground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HealthBar.Background.rect.width * CurrHP / MaxHP);
    }

    private bool CheckInjuries()
    {
        if (DisplayedInjuries.Count != SelectedActor.EntityComponents.Health.InjuryCount)
        {
            return true;
        }
        return false;
    }
    private bool CheckConditions()
    {
        if (DisplayedConditions.Count != SelectedActor.EntityComponents.Health.ActiveConditions.Count)
        {
            return true;
        }
        return false;
    }
    private void DisplayInjuries()
    {
        foreach(GameObject obj in DisplayedInjuries)
        {
            Destroy(obj);
        }
        DisplayedInjuries.Clear();
        foreach (Medical.Health.BodyPart part in SelectedActor.EntityComponents.Health.Body)
        {
            foreach (Medical.Health.Injury injury in part.Injuries)
            {
                if (InjuriesPanel == null) return;
                GameObject InjuryIcon = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ToolTippedIconPrefab], InjuriesPanel.transform);
                ToolTipData InjuryToolTip = InjuryIcon.GetComponent<ToolTipData>();
                Image InjuryImage = InjuryIcon.GetComponent<Image>();

                string PartName;
                if (injury.AppliesToBone)
                    PartName = part.BoneName;
                else
                    PartName = part.Name;

                InjuryToolTip.TitleText = $"{injury.Name} {PartName}";
                InjuryToolTip.DescriptionText = injury.Description;
                InjuryToolTip.UpdateValue = injury.GetRemainingTime;

                InjuryImage.sprite = injury.Icon;
                DisplayedInjuries.Add(InjuryIcon);
            }
        }
    }
    private void DisplayConditions()
    {
        int StartCount = DisplayedConditions.Count;
        List<ToolTipData> PreservedIcons = new List<ToolTipData>();
        foreach(ToolTipData icon in DisplayedConditions)
        {
            if(SelectedActor.EntityComponents.Health.ActiveConditions.Any(c => icon.RefObject == c))
            {
                PreservedIcons.Add(icon);
                continue;
            } else
            {
                Destroy(icon.gameObject);
            }
        }
        DisplayedConditions = PreservedIcons;
        int PreservedCount = DisplayedConditions.Count;
        
        foreach(Condition condition in SelectedActor.EntityComponents.Health.ActiveConditions)
        {
            if(!DisplayedConditions.Any(c => c.RefObject == condition))
            {
                if (ConditionsPanel == null) return;
                GameObject ConditionIcon = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ToolTippedIconPrefab], ConditionsPanel.transform);
                ToolTipData ConditionToolTip = ConditionIcon.GetComponent<ToolTipData>();
                Image ConditionImage = ConditionIcon.GetComponent<Image>();

                ConditionToolTip.TitleText = $"{condition.DisplayName}";
                ConditionToolTip.DescriptionText = condition.Description;
                ConditionToolTip.UpdateValue = condition.GetRemainingTime;
                ConditionToolTip.RefObject = condition;

                ConditionImage.sprite = condition.Icon;
                DisplayedConditions.Add(ConditionToolTip);
            }
        }
        int FinalCount = DisplayedConditions.Count;
        int ConditionCount = SelectedActor.EntityComponents.Health.ActiveConditions.Count;
        //Debug.Log($"{StartCount} => {PreservedCount} => {FinalCount} || {ConditionCount}");
    }
}
