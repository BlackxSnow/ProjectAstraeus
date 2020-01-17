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

    public void SetInfo(string Title, string Description)
    {
        this.Title.text = Title;
        this.Description.text = Description;
    }
}
