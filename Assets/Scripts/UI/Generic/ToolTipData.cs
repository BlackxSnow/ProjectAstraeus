using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string TitleText;
    public string DescriptionText;

    GameObject ToolTipObject;
    bool MouseOver = false;
    Utility.Timer HoverTimer;
    bool AllowToolTip = true;

    void Start()
    {
        Utility.Timer.ElapsedDelegate del = ToolTipEvent;
        HoverTimer = new Utility.Timer(1, del);
    }

    void Update()
    {
        CheckHover();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        MouseOver = true;
    }
    public void OnPointerExit(PointerEventData data)
    {
        MouseOver = false;
    }

    Vector3 LastMousePosition;
    Vector3 SpawnPosition;
    bool HasMouseMoved(float Threshold = 5.0f)
    {
        if (Vector3.Distance(Input.mousePosition, LastMousePosition) < Threshold)
        {
            LastMousePosition = Input.mousePosition;
            return false;
        }
        else
        {
            LastMousePosition = Input.mousePosition;
            return true;
        }
    }

    bool IsMouseBeyondDistance(float Threshold = 10.0f)
    {
        if (Vector3.Distance(Input.mousePosition, SpawnPosition) > Threshold)
            return true;
        else
            return false;
    }

    void ToolTipEvent()
    {
        AllowToolTip = false;
        SpawnPosition = Input.mousePosition;
        ToolTipObject = UIController.InstantiateToolTip(TitleText, DescriptionText);
    }

    void CheckHover()
    {
        if (ToolTipObject && IsMouseBeyondDistance())
        {
            Destroy(ToolTipObject);
            AllowToolTip = true;
        }

        if (MouseOver && AllowToolTip && !HasMouseMoved() && !HoverTimer.Enabled && !ToolTipObject)
        {
            HoverTimer.Start();
        }
        else if ((!MouseOver || HasMouseMoved() || !AllowToolTip) && HoverTimer.Enabled)
            HoverTimer.Stop();
    }
}
