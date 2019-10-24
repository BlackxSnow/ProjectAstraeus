using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentSlot : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI MaxSizeText;
    public GameObject ItemSlot;
    public RectTransform ContainerRectTransform;

    public EquipmentUI ParentUI;

    float SizeMultiplier;
    public Vector2Int MaxSize;

    bool Initialised = false;
    private void Start()
    {
        if (!Initialised) Init();
    }
    public void Init()
    {
        SetSize();
        MaxSizeText.text = string.Format("{0}", MaxSize);
        Initialised = true;
    }

    public void SetSize()
    {
        RectTransform ItemSlotRect = ItemSlot.GetComponent<RectTransform>();
        SizeMultiplier = Mathf.Abs((ContainerRectTransform.rect.y + 20) / MaxSize.y);

        ItemSlotRect.sizeDelta = new Vector2(MaxSize.x * SizeMultiplier, MaxSize.y * SizeMultiplier);
    }
}
