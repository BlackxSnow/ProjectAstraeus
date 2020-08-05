using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Control
{
    public class CollapsibleList : MonoBehaviour
    {
        [SerializeField]
        private GameObject ui_ContentPanel;
        public GameObject UI_ContentPanel { get; }

        private RectTransform ListRect;
        private RectTransform ParentRect;
        private RectTransform ContentRect;
        private List<GameObject> Children = new List<GameObject>();

        private void Start()
        {
            ParentRect = transform.parent.GetComponent<RectTransform>();
            ListRect = GetComponent<RectTransform>();
            ContentRect = UI_ContentPanel.GetComponent<RectTransform>();

            UpdateChildList();
        }

        private void Update()
        {
            if (UI_ContentPanel.transform.childCount != Children.Count)
            {
                UpdateChildList();
            }
        }

        private void UpdateChildList()
        {
            Children.Clear();
            for (int i = 0; i < UI_ContentPanel.transform.childCount; i++)
            {
                Children.Add(UI_ContentPanel.transform.GetChild(i).gameObject);
            }
        }

        public void ToggleCollapse()
        {
            foreach (GameObject Child in Children)
            {
                Child.SetActive(!Child.activeSelf);
            }
            if (ParentRect) LayoutRebuilder.ForceRebuildLayoutImmediate(ParentRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ListRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRect);
        }
    } 
}
