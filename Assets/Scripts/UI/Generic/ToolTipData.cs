using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDestroyHandler
{
    public string TitleText;
    public string DescriptionText;

    GameObject ToolTipObject;
    ToolTip ToolTipScript;
    public object RefObject;
    bool MouseOver = false;
    Utility.Timer HoverTimer;
    bool AllowToolTip = true;

    public delegate string UpdateValueDelegate();
    public UpdateValueDelegate UpdateValue;

    public bool DestructionMarked { get; set; } = false;
    public void Destroy()
    {
        if (HoverTimer != null) HoverTimer.Stop();
        DestructionMarked = true;
        Destroy(this);
    }

    void Start()
    {
        Utility.Timer.ElapsedDelegate del = ToolTipEvent;
        HoverTimer = new Utility.Timer(1, del);
    }

    void Update()
    {
        CheckHover();
        if (ToolTipObject)
        {
            UpdateToolTip();
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        MouseOver = true;
    }
    public void OnPointerExit(PointerEventData data)
    {
        MouseOver = false;
    }

    void OnDestroy()
    {
        if (ToolTipObject)
            Destroy(ToolTipObject);
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

    void ToolTipEvent(float _ = 0)
    {
        AllowToolTip = false;
        SpawnPosition = Input.mousePosition;
        ToolTipObject = UIController.InstantiateToolTip($"{TitleText} ({UpdateValue()})", DescriptionText, gameObject);
        ToolTipScript = ToolTipObject.GetComponent<ToolTip>();
    }
    
    void UpdateToolTip()
    {
        string Val = UpdateValue();
        if (int.TryParse(Val, out int i) && i == -1)
        {
            ToolTipScript.Title.text = $"{TitleText}";
        }
        else
        {
            ToolTipScript.Title.text = $"{TitleText} ({UpdateValue()})";
        }
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
