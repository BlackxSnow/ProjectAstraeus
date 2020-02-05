using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolTip : MonoBehaviour
{
    //Text Components
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public GameObject ParentObject;

    public void SetInfo(string Title, string Description, GameObject TargetObj)
    {
        this.Title.text = Title;
        this.Description.text = Description;
        ParentObject = TargetObj;
    }
    public void Update()
    {
        if (ParentObject == null)
        {
            Destroy(gameObject);
        }
    }
}
