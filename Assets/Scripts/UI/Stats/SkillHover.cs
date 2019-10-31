using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    KeyValuePanel SkillKVP;
    public StatsUI ParentStatsWindow;

    void Start()
    {
        SkillKVP = GetComponent<KeyValuePanel>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ParentStatsWindow.DisplaySkillInfo(SkillKVP.GetValueEnum);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ParentStatsWindow.ClearSkillInfo();
    }
}
