using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyValueList : MonoBehaviour
{
    LayoutElement ListLayout;
    GameObject ContentPanel;
    LayoutElement ContentLayout;

    public Transform[] KVPs;
    private int lastcount;

    private void Awake()
    {
        ListLayout = GetComponent<LayoutElement>();
        ContentPanel = transform.Find("Content").gameObject;
        ContentLayout = ContentPanel.GetComponent<LayoutElement>();
    }

    private void Update()
    {
        if(lastcount != ContentPanel.transform.childCount)
        {
            SetContentSize();
            KVPs = Utility.GetChildren(ContentPanel.transform);
            lastcount = ContentPanel.transform.childCount;
        }
    }

    void SetContentSize()
    {
        float TargetSize = ContentPanel.transform.childCount * 25;
        ContentLayout.preferredHeight = TargetSize;
        ListLayout.preferredHeight = TargetSize + 25;
    }
}
