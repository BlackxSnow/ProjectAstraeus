﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static StatsAndSkills;

public class StatsUI : Window
{
    [Serializable]
    public struct InfoPanelStruct
    {
        public GameObject XPGainActivities;
        public GameObject AffectedActivities;
        public GameObject LevelAndXP;
        public GameObject Description;
    }
    public InfoPanelStruct InfoPanels;
    List<GameObject> InfoPanelObjects = new List<GameObject>();
    StatsAndSkills RefStats { get; set; }

    public Transform StatsPanel;
    public Transform SkillPanelContainer;
    Transform[] SkillPanels;

    KeyValueGroup InfoPanelGroup;
    KeyValueGroup SkillsGroup;

    public void Init(StatsAndSkills TargetStats)
    {
        InfoPanelGroup = new KeyValueGroup(8, 72);
        SkillsGroup = new KeyValueGroup(8, 72);
        RefStats = TargetStats;

        CreateSkillPanels();
        DisplayKVPs();
    }

    void CreateSkillPanels()
    {
        SkillPanels = new Transform[(Enum.GetNames(typeof(StatSkill.SkillTypes)).Length - 1)]; //-1 to skip "None"
        float Threshold = Mathf.Ceil(SkillPanels.Length / 2f);
        float XOffset = 1f / Threshold;

        for (int i = 0; i < SkillPanels.Length; i++)
        {
            SkillPanels[i] = UIController.InstantiateLayoutPanel(SkillPanelContainer, UIController.LayoutTypes.Vertical, true, false, 5f, Enum.GetName(typeof(StatSkill.SkillTypes), i + 1)).transform;
            VerticalLayoutGroup VLayout = SkillPanels[i].GetComponent<VerticalLayoutGroup>();
            RectTransform RTransform = SkillPanels[i].GetComponent<RectTransform>();

            VLayout.padding.left = 5;
            VLayout.padding.right = 5;
            VLayout.padding.bottom = 5;
            VLayout.padding.top = 5;
            VLayout.childControlHeight = false;

            float VerticalOffset = i >= Threshold ? 0 : 0.5f;
            float HorizontalOffset = i >= Threshold ? XOffset * (i - Threshold) : XOffset * i;
            RTransform.anchorMin = new Vector2(HorizontalOffset, VerticalOffset);
            RTransform.anchorMax = new Vector2(HorizontalOffset + XOffset, VerticalOffset + 0.5f);

            GameObject TitleText = UIController.InstantiateText(Enum.GetName(typeof(StatSkill.SkillTypes), i + 1), SkillPanels[i], SkillsGroup);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
        }
    }

    void DisplayKVPs()
    {
        foreach (KeyValuePair<StatsEnum, StatSkill> Stat in RefStats.Stats)
        {
            GameObject KVP = UIController.InstantiateKVP(Stat.Key.ToString(), Stat.Value.Level, StatsPanel, Group: SkillsGroup, ValueDelegate: KeyValuePanel.GetStat, RefStats: RefStats, ValueEnum: Stat.Key, PreferredHeight: 10, KeyRatio: .8f, LeftAligned: true);
            KVP.AddComponent<SkillHover>().ParentStatsWindow = this;
        }
        foreach (KeyValuePair<SkillsEnum, StatSkill> Skill in RefStats.Skills)
        {
            GameObject KVP = UIController.InstantiateKVP(Skill.Key.ToString(), Skill.Value.Level, SkillPanels[(int)Skill.Value.SkillType - 1], Group: SkillsGroup, ValueDelegate: KeyValuePanel.GetSkill, RefStats: RefStats, ValueEnum: Skill.Key, PreferredHeight: 10, KeyRatio: .8f, LeftAligned: true);
            KVP.AddComponent<SkillHover>().ParentStatsWindow = this;
        }
    }

    public void DisplaySkillInfo(Enum StatSkillEnum)
    {
        StatSkill Skill = RefStats.GetSkillInfo(StatSkillEnum);
        //XP Gain Activities
        {
            GameObject TitleText = UIController.InstantiateText("XP Gain Activities", InfoPanels.XPGainActivities.transform, InfoPanelGroup);
            InfoPanelObjects.Add(TitleText);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
            for (int i = 0; i < Skill.XPGainActivities.Length; i++)
            {
                InfoPanelObjects.Add(UIController.InstantiateText(Skill.XPGainActivities[i], InfoPanels.XPGainActivities.transform, InfoPanelGroup));
            }
        }
        //Affected Activities
        {
            GameObject TitleText = UIController.InstantiateText("Affected Activities", InfoPanels.AffectedActivities.transform, InfoPanelGroup);
            InfoPanelObjects.Add(TitleText);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
            for (int i = 0; i < Skill.AffectedActivities.Length; i++)
            {
                InfoPanelObjects.Add(UIController.InstantiateKVP(Skill.AffectedActivities[i].Key, Skill.AffectedActivities[i].Value, InfoPanels.AffectedActivities.transform, Group: InfoPanelGroup ,KeyRatio: 0.75f));
            }
        }
        //Level and XP
        {
            
        }
        //Description
        {
            GameObject TitleText = UIController.InstantiateText("Description", InfoPanels.Description.transform, InfoPanelGroup);
            InfoPanelObjects.Add(TitleText);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
            InfoPanelObjects.Add(UIController.InstantiateText(Skill.Description, InfoPanels.Description.transform, InfoPanelGroup));
        }
        InfoPanelGroup.ForceRecalculate();

    }
    public void ClearSkillInfo()
    {
        foreach(GameObject Info in InfoPanelObjects)
        {
            Destroy(Info);
        }
        InfoPanelObjects.Clear();
        InfoPanelGroup.ClearGroup();
    }
}
