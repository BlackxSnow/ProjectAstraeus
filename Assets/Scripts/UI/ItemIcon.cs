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

    RectTransform RTransform;

    GameObject ParentCanvasObj;
    RectTransform CanvasRect;
    GraphicRaycaster ParentGraphicRaycast;

    public Item RefItem;

    bool Dragging;
    bool FollowingCursor;

    public Vector3 ModifiedMousePosition;
    Vector3 MouseOffset;

    Utility.Timer HoverTimer;
    bool AllowToolTip = true;
    // Start is called before the first frame update
    void Start()
    {
        RTransform = GetComponent<RectTransform>();
        ParentCanvasObj = UIController.CanvasObject;
        CanvasRect = ParentCanvasObj.GetComponent<RectTransform>();
        ParentGraphicRaycast = ParentCanvasObj.GetComponent<GraphicRaycaster>();

        Utility.Timer.ElapsedDelegate del = ToolTipEvent;
        HoverTimer = new Utility.Timer(1, del);
    }

    void Update()
    {
        if (FollowingCursor)
        {
            //Convert mouse position (Which is in screen resolution units) to canvas position (1920 * 1080)
            ModifiedMousePosition = Utility.ScreenToCanvasSpace(Utility.FlipY(Input.mousePosition), CanvasRect);

            RTransform.anchoredPosition = ModifiedMousePosition - MouseOffset;
        }
        CheckHover();

        if (RefItem && ItemSlotName.text != string.Format("{0}", RefItem.Data.Core.Slot))
        {
            ItemSlotName.text = RefItem.Data.Core.Slot.ToString();
        }
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
            EquipmentInfo.Container.UnequipItem(TargetItem, EquipmentInfo.UIContainer);
            EquipmentInfo.UIContainer.RenderItem();
        }
    }

    public void SetSizeToGrid()
    {
        RTransform.sizeDelta = RefItem.Data.Stats.GetStat<Vector2Int>(ItemTypes.StatFlagsEnum.Size) * InventoryUI.GridSize;
    }

    public void OnPointerDown(PointerEventData data)
    {
        AllowToolTip = false;
        if (ToolTipObject) Destroy(ToolTipObject);
        Darken(true);
        Dragging = true;
        SetFollowCursor();
        SetSizeToGrid();
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!Dragging) return;

        List<RaycastResult> Results = new List<RaycastResult>();
        ParentGraphicRaycast.Raycast(data, Results);

        InventoryCheck(Results);
        EquipmentCheck(Results);
        
        //InventoryInfo.UIContainer.RenderItems();
        Dragging = false;
        Darken(false);
    }

    void EquipmentCheck(List<RaycastResult> Results)
    {
        RaycastResult TargetResult = Results.Find(R => R.gameObject.GetComponent<EquipmentSlot>() != null);
        if (!TargetResult.gameObject) return;
        EquipmentSlot TargetSlot = TargetResult.gameObject.GetComponent<EquipmentSlot>();

        if (Results.Count > 0 && TargetSlot)
        {
            Equipment TargetEquipment = TargetSlot.ParentUI.RefEquipment;
            bool Equipped = TargetEquipment.EquipItem(RefItem, TargetSlot);

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
            Vector2 DropScreenSpace = Utility.ScreenToCanvasSpace(RelativeDropPosition, CanvasRect);
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
        ModifiedMousePosition = Utility.ScreenToCanvasSpace(Utility.FlipY(Input.mousePosition), CanvasRect);
        MouseOffset = new Vector3(ModifiedMousePosition.x - RTransform.anchoredPosition.x, ModifiedMousePosition.y - RTransform.anchoredPosition.y, 0);
        FollowingCursor = true;
    }

    //ToolTips
    GameObject ToolTipObject;
    bool MouseOver = false;
    public void OnPointerEnter(PointerEventData data)
    {
        MouseOver = true;
    }
    public void OnPointerExit(PointerEventData data)
    {
        MouseOver = false;
    }

    Vector3 LastMousePosition;
    Vector3 SpawnPosition;
    bool HasMouseMoved(float Threshold = 5.0f)
    {
        if(Vector3.Distance(Input.mousePosition, LastMousePosition) < Threshold)
        {
            LastMousePosition = Input.mousePosition;
            return false;
        } else
        {
            LastMousePosition = Input.mousePosition;
            return true;
        }
    }

    bool IsMouseBeyondDistance(float Threshold = 10.0f)
    {
        if (Vector3.Distance(Input.mousePosition, SpawnPosition) > Threshold)
            return true;
        else
            return false;
    }

    void ToolTipEvent()
    {
        AllowToolTip = false;
        SpawnPosition = Input.mousePosition;
        ToolTipObject = UIController.InstantiateToolTip(RefItem);
    }

    void CheckHover()
    {
        if (ToolTipObject && IsMouseBeyondDistance())
        {
            Destroy(ToolTipObject);
            AllowToolTip = true;
        }

        if (MouseOver && AllowToolTip && !HasMouseMoved() && !HoverTimer.Enabled && !ToolTipObject)
        {
            HoverTimer.Start();
        } else if((!MouseOver || HasMouseMoved() || !AllowToolTip) && HoverTimer.Enabled)
            HoverTimer.Stop();
    }

}
