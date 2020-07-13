using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using Medical.Conditions;
using UI.Control;

namespace UI
{
    public class MedicalDetailsHUD : MonoBehaviour, IDestroyHandler
    {


        public Actor SelectedActor;
        public GameObject InjuriesPanel;
        public GameObject GlobalConditionsPanel;
        public GameObject LocalConditionsPanel;

        [Serializable]
        public struct HealthBarStruct
        {
            public RectTransform Background;
            public RectTransform Foreground;
            public TextMeshProUGUI Text;
        }

        public HealthBarStruct HealthBar;

        List<GameObject> DisplayedInjuries = new List<GameObject>();
        List<ConditionUIIcon> DisplayedGlobalConditions = new List<ConditionUIIcon>();
        List<ConditionUIIcon> DisplayedLocalConditions = new List<ConditionUIIcon>();

        Medical.Health RefHealth;
        public void Init()
        {
            RefHealth = SelectedActor.EntityComponents.Health;
            RefHealth.HealthChanged += UpdateHealthBar;
            RefHealth.InjuriesChanged += DisplayInjuries;
            RefHealth.ConditionsChanged += DisplayConditions;
            UpdateHealthBar();
        }
        int f = 5;

        public bool DestructionMarked { get; set; }

        private void Update()
        {
            if (f > 0)
            {
                UpdateHealthBar();
                f--;
            }
            if (CheckInjuries()) DisplayInjuries();
            if (CheckGlobalConditions()) DisplayGlobalConditions();
            if (CheckLocalConditions()) DisplayLocalConditions();

        }

        private void UpdateHealthBar()
        {
            if (HealthBar.Foreground == null)
            {
                RefHealth.HealthChanged -= UpdateHealthBar;
                return;
            }
            if (f == 0)
            {
                f++;
            }
            float MaxHP = RefHealth.MaxHitPoints;
            float CurrHP = RefHealth.HitPoints;

            HealthBar.Text.text = $"{Utility.Math.RoundToNDecimals(CurrHP, 1)} / {MaxHP}";
            HealthBar.Foreground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HealthBar.Background.rect.width * CurrHP / MaxHP);
        }

        private bool CheckInjuries()
        {
            if (DisplayedInjuries.Count != RefHealth.InjuryCount)
            {
                return true;
            }
            return false;
        }
        private bool CheckGlobalConditions()
        {
            if (DisplayedGlobalConditions.Count != RefHealth.GlobalConditions.Count)
            {
                return true;
            }
            return false;
        }
        private bool CheckLocalConditions()
        {
            if (DisplayedLocalConditions.Count != RefHealth.AllConditions.Count - RefHealth.GlobalConditions.Count)
            {
                return true;
            }
            return false;
        }

        private void DisplayInjuries()
        {
            foreach (GameObject obj in DisplayedInjuries)
            {
                Destroy(obj);
            }
            DisplayedInjuries.Clear();
            foreach (Medical.Health.BodyPart part in RefHealth.Body)
            {
                foreach (Medical.Injury injury in part.Injuries)
                {
                    if (InjuriesPanel == null) return;
                    GameObject InjuryIcon = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.InjuryIconPrefab], InjuriesPanel.transform);
                    InjuryUIIcon InjuryIconScript = InjuryIcon.GetComponent<InjuryUIIcon>();
                    ToolTipData InjuryToolTip = InjuryIcon.GetComponent<ToolTipData>();

                    InjuryIconScript.Init(injury);

                    InjuryToolTip.TitleText = $"{injury.GetDisplayName()}";
                    InjuryToolTip.DescriptionText = injury.Description;
                    InjuryToolTip.UpdateValue = injury.GetRemainingTime;

                    DisplayedInjuries.Add(InjuryIcon);
                }
            }
        }

        private void DisplayConditions()
        {
            DisplayLocalConditions();
            DisplayGlobalConditions();
        }

        private ConditionUIIcon CreateConditionIcon(Condition condition, Transform parent)
        {
            GameObject Icon = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ConditionIconPrefab], parent);
            ToolTipData ConditionToolTip = Icon.GetComponent<ToolTipData>();
            ConditionUIIcon IconScript = Icon.GetComponent<ConditionUIIcon>();

            IconScript.Init(condition);

            ConditionToolTip.TitleText = condition.GetDisplayName();
            ConditionToolTip.DescriptionText = condition.Description;
            ConditionToolTip.UpdateValue = condition.GetRemainingTime;
            ConditionToolTip.RefObject = condition;
            return IconScript;
        }

        //TODO Rewrite to display global conditions to the current all condition UI panel for neatness
        private void DisplayGlobalConditions()
        {
            int StartCount = DisplayedGlobalConditions.Count;
            List<ConditionUIIcon> PreservedIcons = new List<ConditionUIIcon>();
            foreach (ConditionUIIcon icon in DisplayedGlobalConditions)
            {
                if (RefHealth.GlobalConditions.Any(c => icon.RefCondition == c))
                {
                    PreservedIcons.Add(icon);
                    continue;
                }
                else
                {
                    Destroy(icon.gameObject);
                }
            }
            DisplayedGlobalConditions = PreservedIcons;
            int PreservedCount = DisplayedGlobalConditions.Count;

            foreach (Condition condition in RefHealth.GlobalConditions)
            {
                if (!DisplayedGlobalConditions.Any(c => c.RefCondition == condition))
                {
                    if (GlobalConditionsPanel == null) return;

                    ConditionUIIcon IconScript = CreateConditionIcon(condition, GlobalConditionsPanel.transform);

                    DisplayedGlobalConditions.Add(IconScript);
                }
            }
            int FinalCount = DisplayedGlobalConditions.Count;
            int ConditionCount = RefHealth.GlobalConditions.Count;
            //Debug.Log($"{StartCount} => {PreservedCount} => {FinalCount} || {ConditionCount}");
        }

        private void DisplayLocalConditions()
        {
            //Preserve valid conditions
            int StartCount = DisplayedLocalConditions.Count;
            List<ConditionUIIcon> PreservedIcons = new List<ConditionUIIcon>();
            foreach (ConditionUIIcon icon in DisplayedLocalConditions)
            {
                if (RefHealth.AllConditions.Any(c => icon.RefCondition == c))
                {
                    PreservedIcons.Add(icon);
                    continue;
                }
                else
                {
                    Destroy(icon.gameObject);
                }
            }
            DisplayedLocalConditions = PreservedIcons;
            int PreservedCount = DisplayedLocalConditions.Count;

            //Display missing conditions
            foreach (Condition condition in RefHealth.AllConditions)
            {
                if (condition.GlobalCondition) continue;

                if (!DisplayedLocalConditions.Any(c => c.RefCondition == condition))
                {
                    if (LocalConditionsPanel == null) return;

                    ConditionUIIcon IconScript = CreateConditionIcon(condition, LocalConditionsPanel.transform);

                    DisplayedLocalConditions.Add(IconScript);
                }
            }
        }

        public void Destroy()
        {
            RefHealth.HealthChanged -= UpdateHealthBar;
            RefHealth.InjuriesChanged -= DisplayInjuries;
            RefHealth.ConditionsChanged -= DisplayConditions;
            Destroy(gameObject);
        }
    }

}