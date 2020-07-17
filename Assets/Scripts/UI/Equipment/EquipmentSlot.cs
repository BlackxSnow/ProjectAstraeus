using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.Control;
using Items;

public class EquipmentSlot : MonoBehaviour
{
    public TextMeshProUGUI Label;
    public TextMeshProUGUI MaxSizeText;
    public GameObject ItemSlot;
    public RectTransform ContainerRectTransform;
    RectTransform ItemSlotRect;

    RectTransform thisRectTransform;

    public EquipmentUI ParentUI;

    public float TextOffset;
    float SizeMultiplier;
    public Vector2Int MaxSize;
    public Equipment.Slots Slot;

    bool Initialised = false;
    private void Start()
    {
        if (!Initialised) Init();
    }
    public void Init()
    {
        thisRectTransform = GetComponent<RectTransform>();
        ItemSlotRect = ItemSlot.GetComponent<RectTransform>();
        SetSize(MaxSize, ItemSlotRect, ContainerRectTransform);
        MaxSizeText.text = string.Format("{0}", MaxSize);
        Initialised = true;
    }
    Rect LastSize;
    private void Update()
    {
        if (thisRectTransform.rect != LastSize)
        {
            SetSize(MaxSize, ItemSlotRect, ContainerRectTransform);
            LastSize = thisRectTransform.rect;
        }
    }

    public void SetSize(Vector2 Size, RectTransform Set, RectTransform Container)
    {
        
        Vector2 SizeAltered;
        if(Container.rect.width > Mathf.Abs(Container.rect.height) - TextOffset)
        {
            SizeAltered = new Vector2(Mathf.Max(Size.x, Size.y), Mathf.Min(Size.x, Size.y));
        } else
        {
            SizeAltered = new Vector2(Mathf.Min(Size.x, Size.y), Mathf.Max(Size.x, Size.y));
        }
        SizeMultiplier = Mathf.Min((Container.rect.height - TextOffset) / SizeAltered.y, Container.rect.width / SizeAltered.x);


        Set.sizeDelta = new Vector2(SizeAltered.x * SizeMultiplier, SizeAltered.y * SizeMultiplier);
    }
    GameObject ActiveItemIcon;
    public void RenderItem(Equipment.VectorResults HorizontalAxis = Equipment.VectorResults.None)
    {
        Destroy(ActiveItemIcon);
        ActiveItemIcon = null;

        Item EquippedItem = ParentUI.RefEquipment.Equipped[(int)Slot];
        if (EquippedItem)
        {
            GameObject ItemIconInstance = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ItemIconPrefab], ItemSlot.transform);
            ActiveItemIcon = ItemIconInstance;

            ItemIcon ItemIconScript = ItemIconInstance.GetComponent<ItemIcon>();
            ItemIconScript.EquipmentInfo.UIContainer = this;
            ItemIconScript.EquipmentInfo.Container = ParentUI.RefEquipment;
            ItemIconScript.RefItem = EquippedItem;

            RectTransform ItemIconRectTransform = ItemIconInstance.GetComponent<RectTransform>();
            SetSize(EquippedItem.Stats.Size, ItemIconRectTransform, ContainerRectTransform);
            //if (HorizontalAxis == Equipment.VectorResults.y)
            //{
            //    ItemIconRectTransform.Rotate(Vector3.forward, -90);
            //}
        }
    }
}
