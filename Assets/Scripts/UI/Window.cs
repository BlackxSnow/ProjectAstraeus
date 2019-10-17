using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Window : MonoBehaviour, IDraggable, IResizable, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject WindowBar;
    public GameObject ResizeBox;

    public Vector2 MinSize;
    public Vector2 MaxSize;

    List<RaycastResult> Results = new List<RaycastResult>();
    Vector3 InitialPosition;
    Vector3 InitialWindowPosition;
    Vector2 InitialWindowSize;
    bool Dragging;
    bool Resizing;

    RectTransform WindowRect;

    void Start()
    {
        WindowRect = gameObject.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData Data)
    {
        if(!WindowRect) WindowRect = WindowRect = gameObject.GetComponent<RectTransform>();
        Dragging = false;
        Resizing = false;

        EventSystem.current.RaycastAll(Data, Results);
        if (WindowBarDragCheck(Results))
        {
            Dragging = true;
            InitialPosition = Input.mousePosition;
            InitialWindowPosition = gameObject.transform.position;
        }
        else if (ResizeSquareDragCheck(Results))
        {
            Resizing = true;
            InitialPosition = Input.mousePosition;
            InitialWindowSize = WindowRect.rect.size;
        }
    }

    public bool WindowBarDragCheck(List<RaycastResult> RayResults)
    {
        foreach(RaycastResult Result in RayResults)
        {
            if (Result.gameObject == WindowBar) return true;
        }
        return false;
    }
    public bool ResizeSquareDragCheck(List<RaycastResult> RayResults)
    {
        foreach (RaycastResult Result in RayResults)
        {
            if (Result.gameObject == ResizeBox) return true;
        }
        return false;
    }

    public void OnDrag(PointerEventData Data)
    {
        if (Dragging)
        {
            Vector3 MouseDifference = Input.mousePosition - InitialPosition;
            gameObject.transform.position = InitialWindowPosition + MouseDifference;
        }
        if (Resizing)
        {
            Vector3 MouseDifference = Input.mousePosition - InitialPosition;
            Vector2 ConstrainedSize;
            Vector2 MouseDifferenceCanvas = Utility.ScreenToCanvasSpace(MouseDifference, new Vector2(1920, 1080));
            ConstrainedSize.x = Mathf.Max(InitialWindowSize.x + MouseDifferenceCanvas.x, MinSize.x);
            ConstrainedSize.y = Mathf.Max(InitialWindowSize.y + MouseDifferenceCanvas.y, MinSize.y);
            WindowRect.sizeDelta = ConstrainedSize;
        }
    }

    public void OnEndDrag(PointerEventData Data)
    {
        Dragging = false;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
        {
            if(EventSystem.current.currentSelectedGameObject != this.gameObject || EventSystem.current.currentSelectedGameObject.transform.IsChildOf(gameObject.transform))
            {

            }
        }
    }
}
