using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    //Item Info Components
    public TextMeshProUGUI ItemNameComp;
    public TextMeshProUGUI ItemTypeComp;
    public Image ItemImageComp;

    public GameObject StatsPanel;
    public KeyValueGroup KVPGroup;

    public void SetInfo(string Name, string Type, Sprite Icon = null)
    {
        ItemNameComp.text = Name;
        ItemTypeComp.text = Type;
        ItemImageComp.sprite = Icon;
    }
}
