using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image Icon;
    public TextMeshProUGUI ItemSlotName;
    public struct InventoryInfoStruct
    {
        public Inventory Container;
        public InventoryUI UIContainer;
        public Vector2Int Location;
    }
    public struct EquipmentInfoStruct
    {
        public Equipment Container;
        public EquipmentSlot UIContainer;
    }
    public InventoryInfoStruct InventoryInfo;
    public EquipmentInfoStruct EquipmentInfo;

    public ProgressBar Quantitybar;
    RectTransform RTransform;

    GameObject ParentCanvasObj;
    RectTransform CanvasRect;
    GraphicRaycaster ParentGraphicRaycast;

    public Item RefItem;

    bool Dragging;
    bool FollowingCursor;

    public bool DisplayQuantity = false;

    public Vector3 ModifiedMousePosition;
    Vector3 MouseOffset;

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out ItemContextMenu ItemMenu))
        {
            ItemMenu.RefItem = RefItem;
            ItemMenu.Init();
        }
        
        RTransform = GetComponent<RectTransform>();
        ParentCanvasObj = UIController.CanvasObject;
        CanvasRect = ParentCanvasObj.GetComponent<RectTransform>();
        ParentGraphicRaycast = ParentCanvasObj.GetComponent<GraphicRaycaster>();

        if (RefItem.Stats.Stats.ContainsKey(ItemTypes.StatsEnum.Quantity))
        {
            DisplayQuantity = true;
            Quantitybar.Init(RefItem.Stats.GetStat<int>(ItemTypes.StatsEnum.Quantity), RefItem.Stats.GetStat<int>(ItemTypes.StatsEnum.MaxQuantity), Color.grey, Color.green);
        }
        else Quantitybar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (FollowingCursor)
        {
            //Convert mouse position (Which is in screen resolution units) to canvas position (1920 * 1080)
            ModifiedMousePosition = Utility.Vector.ScreenToCanvasSpace(Utility.Vector.FlipY(Input.mousePosition), CanvasRect);

            RTransform.anchoredPosition = ModifiedMousePosition - MouseOffset;
        }

        if (RefItem is EquippableItem Equippable && ItemSlotName.text != Equippable.ValidSlots[0].ToString())
        {
            ItemSlotName.text = Equippable.ValidSlots[0].ToString();
        }

        if (DisplayQuantity) Quantitybar.UpdateBar(RefItem.Stats.GetStat<int>(ItemTypes.StatsEnum.Quantity), false);
        ToolTipUpdate();
    }

    void RemoveItem(Item TargetItem)
    {
        if (InventoryInfo.Container)
        {
            InventoryInfo.Container.RemoveItem(TargetItem, false);
            InventoryInfo.UIContainer.RenderItems();
        }
        else if (EquipmentInfo.Container)
        {
            EquipmentInfo.Container.UnequipItem(TargetItem as EquippableItem, EquipmentInfo.UIContainer);
            EquipmentInfo.UIContainer.RenderItem();
        }
    }

    public void SetSizeToGrid()
    {
        RTransform.sizeDelta = RefItem.Stats.GetStat<Vector2Int>(ItemTypes.StatsEnum.Size) * InventoryUI.GridSize;
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Left)
        {
            ToolTipEnabled = false;
            Darken(true);
            Dragging = true;
            SetFollowCursor();
            SetSizeToGrid();
        }
        else if (data.button == PointerEventData.InputButton.Right)
        {

        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        
        if (!Dragging || data.button != PointerEventData.InputButton.Left) return;

        List<RaycastResult> Results = new List<RaycastResult>();
        ParentGraphicRaycast.Raycast(data, Results);

        InventoryCheck(Results);
        EquipmentCheck(Results);
        
        //InventoryInfo.UIContainer.RenderItems();
        Dragging = false;
        Darken(false);
        ToolTipEnabled = true;
    }

    void EquipmentCheck(List<RaycastResult> Results)
    {
        RaycastResult TargetResult = Results.Find(R => R.gameObject.GetComponent<EquipmentSlot>() != null);
        if (!TargetResult.gameObject || !(RefItem is EquippableItem)) return;
        EquipmentSlot TargetSlot = TargetResult.gameObject.GetComponent<EquipmentSlot>();

        if (Results.Count > 0 && TargetSlot)
        {
            Equipment TargetEquipment = TargetSlot.ParentUI.RefEquipment;

            bool Equipped = TargetEquipment.EquipItem(RefItem as EquippableItem, TargetSlot);

            if (Equipped)
            {
                RemoveItem(RefItem);
                RefItem.SetFollow(TargetEquipment.gameObject);
            }
        }
    }

    void InventoryCheck(List<RaycastResult> Results)
    {
        if (Results.Count > 0 && Results.Any(R => R.gameObject.name.Contains("InventoryGrid")))
        {
            RaycastResult GridResult = Results.Find(R => R.gameObject.name.Contains("InventoryGrid"));

            //Get the base inventory window information
            Transform TargetGridPanel = GridResult.gameObject.transform.parent;
            Transform TargetUI = TargetGridPanel.parent;
            InventoryUI TargetScript = TargetUI.GetComponent<InventoryUI>();

            //Find the top left corner of the GridPanel for locating inventory grids
            Vector3[] CornersArray = new Vector3[4];
            TargetGridPanel.GetComponent<RectTransform>().GetWorldCorners(CornersArray);

            //Calculate the relative mouseup position to the GridPanel
            Vector2 RelativeDropPosition = new Vector2(Input.mousePosition.x - CornersArray[1].x, Input.mousePosition.y - CornersArray[1].y);
            Vector2 DropScreenSpace = Utility.Vector.ScreenToCanvasSpace(RelativeDropPosition, CanvasRect);
            Vector2 DropOffset = new Vector2(DropScreenSpace.x - MouseOffset.x, DropScreenSpace.y + MouseOffset.y);
            Vector2Int DropInventoryLocation = new Vector2Int(Mathf.RoundToInt(DropOffset.x / InventoryUI.GridSize), Mathf.RoundToInt(DropOffset.y / InventoryUI.GridSize));

            //Attempt to move item to new location
            if (TargetScript.CurrentInventory.ItemCheck(RefItem, DropInventoryLocation))
            {
                RemoveItem(RefItem);
                TargetScript.CurrentInventory.AddItem(RefItem, DropInventoryLocation);
                RefItem.SetFollow(TargetScript.CurrentInventory.gameObject);
                TargetScript.RenderItems();
            }
        }
    }

    private void Darken(bool Dark)
    {
        if (Dark)
            Icon.color = new Color(Icon.color.r * 0.5f, Icon.color.g * 0.5f, Icon.color.b * 0.5f, 0.5f);
        else
            Icon.color = new Color(Icon.color.r * 2f, Icon.color.g * 2f, Icon.color.b * 2f, 1.0f);
    }
    private void SetFollowCursor()
    {
        RTransform.SetParent(ParentCanvasObj.transform);
        ModifiedMousePosition = Utility.Vector.ScreenToCanvasSpace(Utility.Vector.FlipY(Input.mousePosition), CanvasRect);
        MouseOffset = new Vector3(ModifiedMousePosition.x - RTransform.anchoredPosition.x, ModifiedMousePosition.y - RTransform.anchoredPosition.y, 0);
        FollowingCursor = true;
    }

    //ToolTips
    GameObject ToolTipObject;
    Vector3 ToolTipOffset = new Vector3(20, 0, 0);
    bool ToolTipEnabled = true;
    void ToolTipUpdate()
    {
        if (!ToolTipEnabled && ToolTipObject)
            Destroy(ToolTipObject);

        if (ToolTipObject)
            ToolTipObject.transform.position = Input.mousePosition + ToolTipOffset;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (!ToolTipObject)
        {
            ToolTipObject = UIController.InstantiateItemToolTip(RefItem);
            ToolTipObject.transform.position = Input.mousePosition + ToolTipOffset;
        }
    }
    public void OnPointerExit(PointerEventData data)
    {
        if (ToolTipObject)
            Destroy(ToolTipObject);
    }

}
