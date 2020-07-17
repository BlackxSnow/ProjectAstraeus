using UI;
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
        //TODO Fix this
        //ParentStatsWindow.DisplaySkillInfo(SkillKVP.GetValueEnum);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ParentStatsWindow.ClearSkillInfo();
    }
}
