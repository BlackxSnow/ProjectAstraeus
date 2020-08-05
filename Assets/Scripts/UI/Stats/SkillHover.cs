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
        //TODO Fix this || I don't know how this is fucking broken. I wish I'd written WHAT exactly I should be fixing when I wrote this Todo.
        //ParentStatsWindow.DisplaySkillInfo(SkillKVP.GetValueEnum);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ParentStatsWindow.ClearSkillInfo();
    }
}
