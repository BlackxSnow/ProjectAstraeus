using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyValueList : MonoBehaviour
{
    LayoutElement ListLayout;
    GameObject ContentPanel;
    LayoutElement ContentLayout;

    private int lastcount;

    private void Start()
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
