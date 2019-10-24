﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Timers;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    Image Icon;

    public Inventory Container;
    public InventoryUI UIContainer;
    public Vector2Int Location;

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
        Icon = transform.GetChild(0).GetComponent<Image>();
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
    }

    public void OnPointerDown(PointerEventData data)
    {
        AllowToolTip = false;
        if (ToolTipObject) Destroy(ToolTipObject);
        Darken(true);
        Dragging = true;
        SetFollowCursor();
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!Dragging) return;

        List<RaycastResult> Results = new List<RaycastResult>();
        ParentGraphicRaycast.Raycast(data, Results);

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
            Vector2Int DropInventoryLocation = new Vector2Int(Mathf.RoundToInt(DropOffset.x / TargetScript.GridSize), Mathf.RoundToInt(DropOffset.y / TargetScript.GridSize));

            //Attempt to move item to new location
            if (TargetScript.CurrentInventory.ItemCheck(RefItem, DropInventoryLocation))
            {
                Container.RemoveItem(RefItem, false);
                TargetScript.CurrentInventory.AddItem(RefItem, DropInventoryLocation);
                RefItem.SetFollow(TargetScript.CurrentInventory.gameObject);
                TargetScript.RenderItems();
            }
        }
        UIContainer.RenderItems();
        Dragging = false;
        Darken(false);
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
