using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static StatsAndSkills;
using UI.Control;

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
    readonly List<GameObject> InfoPanelObjects = new List<GameObject>();
    StatsAndSkills RefStats { get; set; }

    public Transform StatsPanel;
    public Transform SkillPanelContainer;
    Transform[] SkillPanels;

    KeyValueGroup InfoPanelGroup;
    KeyValueGroup SkillsGroup;

    public void Init(StatsAndSkills TargetStats)
    {
        InfoPanelGroup = ScriptableObject.CreateInstance<KeyValueGroup>();
        InfoPanelGroup.Init(8, 16);
        SkillsGroup = ScriptableObject.CreateInstance<KeyValueGroup>();
        SkillsGroup.Init(8, 72);
        RefStats = TargetStats;

        CreateSkillPanels();
        DisplayKVPs();
    }

    void CreateSkillPanels()
    {
        SkillPanels = new Transform[(Enum.GetNames(typeof(Skill.SkillTypes)).Length - 1)]; //-1 to skip "None"
        float Threshold = Mathf.Ceil(SkillPanels.Length / 2f);
        float XOffset = 1f / Threshold;

        for (int i = 0; i < SkillPanels.Length; i++)
        {
            SkillPanels[i] = CreateUI.Layout.LayoutPanel(SkillPanelContainer, CreateUI.LayoutTypes.Vertical, true, false, 5f, Enum.GetName(typeof(Skill.SkillTypes), i + 1)).transform;
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

            GameObject TitleText = CreateUI.Info.TextPanel(Enum.GetName(typeof(Skill.SkillTypes), i + 1), SkillPanels[i], SkillsGroup);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
        }
    }

    void DisplayKVPs()
    {
        foreach (KeyValuePair<StatsEnum, Stat> Stat in RefStats.Stats)
        {
            CreateUI.KVPData Data = new CreateUI.KVPData(Stat.Key.ToString(), Stat.Value.Level, StatsPanel, KeyRatio: 0.8f)
            {
                Group = SkillsGroup,
                ValueDelegate = KeyValuePanel.GetStat,
                RefStats = RefStats,
                ValueEnum = Stat.Key
            };
            GameObject KVP = CreateUI.Info.KeyValuePanel(Data);
            KVP.AddComponent<SkillHover>().ParentStatsWindow = this;
        }
        foreach (KeyValuePair<SkillsEnum, Skill> Skill in RefStats.Skills)
        {
            CreateUI.KVPData Data = new CreateUI.KVPData(Skill.Key.ToString(), Skill.Value.Level, SkillPanels[(int)Skill.Value.SkillType - 1], KeyRatio: 0.8f)
            {
                Group = SkillsGroup,
                ValueDelegate = KeyValuePanel.GetSkill,
                RefStats = RefStats,
                ValueEnum = Skill.Key
            };
            GameObject KVP = CreateUI.Info.KeyValuePanel(Data);
            KVP.AddComponent<SkillHover>().ParentStatsWindow = this;
        }
    }

    public void DisplaySkillInfo(Enum StatSkillEnum)
    {
        StatSkill Skill = RefStats.GetSkillInfo(StatSkillEnum);
        //XP Gain Activities
        {
            GameObject TitleText = CreateUI.Info.TextPanel("XP Gain Activities", InfoPanels.XPGainActivities.transform, InfoPanelGroup);
            InfoPanelObjects.Add(TitleText);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
            for (int i = 0; i < Skill.XPGainActivities.Length; i++)
            {
                InfoPanelObjects.Add(CreateUI.Info.TextPanel(Skill.XPGainActivities[i], InfoPanels.XPGainActivities.transform, InfoPanelGroup));
            }
        }
        //Affected Activities
        {
            GameObject TitleText = CreateUI.Info.TextPanel("Affected Activities", InfoPanels.AffectedActivities.transform, InfoPanelGroup);
            InfoPanelObjects.Add(TitleText);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
            for (int i = 0; i < Skill.AffectedActivities.Length; i++)
            {
                CreateUI.KVPData Data = new CreateUI.KVPData(Skill.AffectedActivities[i].Key, Skill.AffectedActivities[i].Value, InfoPanels.AffectedActivities.transform, KeyRatio: 0.75f)
                {
                    Group = InfoPanelGroup
                };
                InfoPanelObjects.Add(CreateUI.Info.KeyValuePanel(Data));
            }
        }
        //Level and XP
        {
            
        }
        //Description
        {
            GameObject TitleText = CreateUI.Info.TextPanel("Description", InfoPanels.Description.transform, InfoPanelGroup);
            InfoPanelObjects.Add(TitleText);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
            InfoPanelObjects.Add(CreateUI.Info.TextPanel(Skill.Description, InfoPanels.Description.transform, InfoPanelGroup, true));
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
