using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyValueList : MonoBehaviour
{
    public GameObject ContentPanel;

    public Transform[] KVPs;
    private int lastcount;

    private void Awake()
    {
        ContentPanel = transform.Find("Content").gameObject;
    }

    private void Update()
    {
        if(lastcount != ContentPanel.transform.childCount)
        {
            //SetContentSize();
            KVPs = Utility.GetChildren(ContentPanel.transform);
            lastcount = ContentPanel.transform.childCount;
        }
    }

    //void SetContentSize()
    //{
    //    float TargetSize = ContentPanel.transform.childCount * 25;
    //    ContentLayout.preferredHeight = TargetSize;
    //    ListLayout.preferredHeight = TargetSize + 25;
    //}
}
