using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static StatsAndSkills;

public class StatsUI : Window
{
    StatsAndSkills RefStats { get; set; }

    public Transform StatsPanel;
    public Transform SkillPanelContainer;
    public Transform[] SkillPanels;

    KeyValueGroup StatsGroup;
    KeyValueGroup SkillsGroup;

    public void Init(StatsAndSkills TargetStats)
    {
        StatsGroup = new KeyValueGroup(8, 72);
        SkillsGroup = new KeyValueGroup(8, 72);
        RefStats = TargetStats;
        SkillPanels = new Transform[(Enum.GetNames(typeof(StatSkill.SkillTypes)).Length - 1)]; //-1 to skip "None"
        float Threshold = Mathf.Ceil(SkillPanels.Length / 2f);
        float XOffset = 1f / Threshold;

        for (int i = 0; i < SkillPanels.Length; i++)
        {
            SkillPanels[i] = UIController.InstantiateLayoutPanel(SkillPanelContainer, UIController.LayoutTypes.Vertical, true, false, 5f, Enum.GetName(typeof(StatSkill.SkillTypes), i+1)).transform;
            VerticalLayoutGroup VLayout = SkillPanels[i].GetComponent<VerticalLayoutGroup>();
            RectTransform RTransform = SkillPanels[i].GetComponent<RectTransform>();

            VLayout.padding.left = 5;
            VLayout.padding.right = 5;
            VLayout.padding.bottom = 5;
            
            float VerticalOffset = i >= Threshold ? 0 : 0.5f;
            float HorizontalOffset = i >= Threshold ? XOffset * (i - Threshold) : XOffset * i;
            RTransform.anchorMin = new Vector2(HorizontalOffset, VerticalOffset);
            RTransform.anchorMax = new Vector2(HorizontalOffset + XOffset, VerticalOffset + 0.5f);

            GameObject TitleText = UIController.InstantiateText(Enum.GetName(typeof(StatSkill.SkillTypes), i + 1), SkillPanels[i], SkillsGroup);
            TextMeshProUGUI TitleTextMesh = TitleText.GetComponent<TextMeshProUGUI>();
            TitleTextMesh.fontStyle = FontStyles.Bold;
            TitleTextMesh.color = Color.red;
        }
        DisplayKVPs();
    }

    public void DisplayKVPs()
    {
        foreach (KeyValuePair<StatsEnum, StatSkill> Stat in RefStats.Stats)
        {
            UIController.InstantiateKVP(Stat.Key.ToString(), Stat.Value.Level, StatsPanel, Group: StatsGroup, ValueDelegate: KeyValuePanel.GetStat, RefStats: RefStats, ValueEnum: Stat.Key, PreferredHeight: 10, KeyRatio: .8f, LeftAligned: true);
        }
        foreach (KeyValuePair<SkillsEnum, StatSkill> Skill in RefStats.Skills)
        {
            UIController.InstantiateKVP(Skill.Key.ToString(), Skill.Value.Level, SkillPanels[(int)Skill.Value.SkillType - 1], Group: SkillsGroup, ValueDelegate: KeyValuePanel.GetSkill, RefStats: RefStats, ValueEnum: Skill.Key, PreferredHeight: 10, KeyRatio: .8f, LeftAligned: true);
        }
    }
}
