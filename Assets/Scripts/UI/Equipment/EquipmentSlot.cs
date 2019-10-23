using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentSlot : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public GameObject ItemSlot;
    public RectTransform ContainerRectTransform;

    public EquipmentUI ParentUI;

    public Vector2Int MaxSize;

    bool Initialised = false;
    private void Start()
    {
        if (!Initialised) Init();
    }
    public void Init()
    {
        SetSize();
        Initialised = true;
    }

    public void SetSize()
    {
        RectTransform ItemSlotRect = ItemSlot.GetComponent<RectTransform>();
        float SizeMultiplier = Mathf.Abs((ContainerRectTransform.rect.y + 20) / MaxSize.y); 

        ItemSlotRect.sizeDelta = new Vector2(MaxSize.x * SizeMultiplier, MaxSize.y * SizeMultiplier);
    }
}
