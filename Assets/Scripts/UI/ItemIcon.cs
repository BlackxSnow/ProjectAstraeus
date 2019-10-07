using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class ItemIcon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Image Icon;

    public Inventory Container;
    public InventoryUI UIContainer;
    public Vector2Int Location;

    RectTransform RTransform;

    GameObject ParentCanvasObj;
    CanvasScaler ParentCanvasScaler;
    GraphicRaycaster ParentGraphicRaycast;

    Vector2 RefResolution;

    EventSystem ESystem;

    public Item RefItem;

    bool Dragging;
    bool FollowingCursor;

    Vector3 ModifiedMousePosition;
    Vector3 MouseOffset;

    // Start is called before the first frame update
    void Start()
    {
        Icon = transform.GetChild(0).GetComponent<Image>();
        RTransform = GetComponent<RectTransform>();
        ParentCanvasObj = transform.parent.parent.parent.gameObject;
        ParentCanvasScaler = ParentCanvasObj.GetComponent<CanvasScaler>();
        ParentGraphicRaycast = ParentCanvasObj.GetComponent<GraphicRaycaster>();
        ESystem = EventSystem.current;

        RefResolution = ParentCanvasScaler.referenceResolution;
    }

    void Update()
    {
        if (FollowingCursor)
        {
            //Convert mouse position (Which is in screen resolution units) to canvas position (1920 * 1080)
            ModifiedMousePosition = Utility.ScreenToCanvasSpace(Utility.FlipY(Input.mousePosition), RefResolution);

            RTransform.anchoredPosition = ModifiedMousePosition - MouseOffset;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
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
            Vector2 DropScreenSpace = Utility.ScreenToCanvasSpace(RelativeDropPosition, RefResolution);
            Vector2 DropOffset = new Vector2(DropScreenSpace.x - MouseOffset.x, DropScreenSpace.y + MouseOffset.y);
            Vector2Int DropInventoryLocation = new Vector2Int(Mathf.RoundToInt(DropOffset.x / TargetScript.GridSize), Mathf.RoundToInt(DropOffset.y / TargetScript.GridSize));

            //Attempt to move item to new location
            if (TargetScript.OpenInventory.ItemCheck(RefItem, DropInventoryLocation))
            {
                Container.RemoveItem(RefItem, false);
                TargetScript.OpenInventory.AddItem(RefItem, DropInventoryLocation);
                RefItem.SetFollow(TargetScript.OpenInventory.gameObject);
                TargetScript.RenderItems();
            }
            UIContainer.RenderItems();
        }

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
        ModifiedMousePosition = Utility.ScreenToCanvasSpace(Utility.FlipY(Input.mousePosition), RefResolution);
        MouseOffset = new Vector3(ModifiedMousePosition.x - RTransform.anchoredPosition.x, ModifiedMousePosition.y - RTransform.anchoredPosition.y, 0);
        FollowingCursor = true;
    }
}
