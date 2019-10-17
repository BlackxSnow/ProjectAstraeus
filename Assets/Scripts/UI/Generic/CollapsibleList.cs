using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollapsibleList : MonoBehaviour
{
    public GameObject ContentPanel;
    RectTransform ParentRect;
    public RectTransform ListRect;
    public RectTransform ContentRect;

    private List<GameObject> Children = new List<GameObject>();

    private void Start()
    {
        ParentRect = transform.parent.GetComponent<RectTransform>();

        for(int i = 0; i < ContentPanel.transform.childCount; i++)
        {
            Children.Add(ContentPanel.transform.GetChild(i).gameObject);
        }
    }

    public void ToggleCollapse()
    {
        foreach(GameObject Child in Children)
        {
            Child.SetActive(!Child.activeSelf);
        }
        if (ParentRect) LayoutRebuilder.ForceRebuildLayoutImmediate(ParentRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(ListRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRect);
    }
}
