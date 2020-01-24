using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

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
    List<GameObject> DisplayedConditions = new List<GameObject>();

    public void Init()
    {
        SelectedActor.EntityComponents.Health.HealthChanged += UpdateHealthBar;
        UpdateHealthBar(0);
    }
    int f = 5;
    private void Update()
    {
        if (f > 0)
        {
            UpdateHealthBar(0);
            f--;
        }
        if (CheckInjuries()) DisplayInjuries();
        if (CheckConditions()) DisplayConditions();
        
    }

    private void UpdateHealthBar(float _)
    {
        if(HealthBar.Foreground == null)
        {
            SelectedActor.EntityComponents.Health.HealthChanged -= UpdateHealthBar;
            return;
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
                GameObject InjuryIcon = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ToolTippedIconPrefab], InjuriesPanel.transform);
                ToolTipData InjuryToolTip = InjuryIcon.GetComponent<ToolTipData>();
                Image InjuryImage = InjuryIcon.GetComponent<Image>();

                InjuryToolTip.TitleText = $"{injury.Name} {part.Name}";
                InjuryToolTip.DescriptionText = injury.Description;

                InjuryImage.sprite = injury.Icon;
                DisplayedInjuries.Add(InjuryIcon);
            }
        }
    }
    private void DisplayConditions()
    {
        foreach(GameObject obj in DisplayedConditions)
        {
            Destroy(obj);
        }
        DisplayedConditions.Clear();
        foreach (Medical.Conditions.Condition condition in SelectedActor.EntityComponents.Health.ActiveConditions)
        {
            GameObject ConditionIcon = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ToolTippedIconPrefab], ConditionsPanel.transform);
            ToolTipData ConditionToolTip = ConditionIcon.GetComponent<ToolTipData>();
            Image ConditionImage = ConditionIcon.GetComponent<Image>();

            ConditionToolTip.TitleText = $"{condition.DisplayName}";
            ConditionToolTip.DescriptionText = condition.Description;

            ConditionImage.sprite = condition.Icon;
            DisplayedConditions.Add(ConditionIcon);
        }
    }
}
