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
    bool Enabled = true;
    Vector3 Offset = new Vector3(20,0,0);

    public delegate string UpdateValueDelegate();
    public UpdateValueDelegate UpdateValue;

    public bool DestructionMarked { get; set; } = false;
    public void Destroy()
    {
        if (ToolTipObject)
            Destroy(ToolTipObject);

        DestructionMarked = true;
        Destroy(this);
    }

    void Update()
    {
        if (!Enabled && ToolTipObject)
            Destroy(ToolTipObject);

        if (ToolTipObject)
            UpdateToolTip();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (!ToolTipObject)
        {
            ToolTipObject = UIController.InstantiateToolTip($"{TitleText} ({UpdateValue()})", DescriptionText, gameObject);
            ToolTipScript = ToolTipObject.GetComponent<ToolTip>();
            UpdateToolTip();
        }
    }
    public void OnPointerExit(PointerEventData data)
    {
        if (ToolTipObject)
            Destroy(ToolTipObject);
    }

    void OnDestroy()
    {
        if (ToolTipObject)
            Destroy(ToolTipObject);
    }
    
    void UpdateToolTip()
    {
        ToolTipObject.transform.position = Input.mousePosition + Offset;
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
}
