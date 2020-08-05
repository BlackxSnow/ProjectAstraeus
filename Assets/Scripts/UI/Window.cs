using System.Collections;
using System.Collections.Generic;
using UI.Control;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Window : MonoBehaviour, IDraggable, IResizable, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [Header("Window Settings")]
    [SerializeField]
    protected GameObject WindowBar;
    [SerializeField]
    protected GameObject ResizeBox;

    [SerializeField]
    protected Vector2 MinSize;
    [SerializeField]
    protected Vector2 MaxSize;

    List<RaycastResult> Results = new List<RaycastResult>();
    Vector2 InitialPosition;
    Vector3 InitialWindowPosition;
    Vector2 InitialWindowSize;
    bool Dragging;
    bool Resizing;

    RectTransform WindowRect;
    RectTransform CanvasRect;

    protected virtual void Start()
    {
        CanvasRect = UIController.CanvasObject.GetComponent<RectTransform>();
        WindowRect = gameObject.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData Data)
    {
        BringToFront();
        if(!WindowRect) WindowRect = gameObject.GetComponent<RectTransform>();
        Dragging = false;
        Resizing = false;

        EventSystem.current.RaycastAll(Data, Results);
        if (WindowBarDragCheck(Results))
        {
            Dragging = true;
            InitialPosition = Mouse.current.position.ReadValue();
            InitialWindowPosition = gameObject.transform.position;
        }
        else if (ResizeSquareDragCheck(Results))
        {
            Resizing = true;
            InitialPosition = Mouse.current.position.ReadValue();
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
            Vector3 MouseDifference = Mouse.current.position.ReadValue() - InitialPosition;
            gameObject.transform.position = InitialWindowPosition + MouseDifference;
        }
        if (Resizing)
        {
            Vector3 MouseDifference = Mouse.current.position.ReadValue() - InitialPosition;
            Vector2 ConstrainedSize;
            Vector2 MouseDifferenceCanvas = Utility.Vector.ScreenToCanvasSpace(MouseDifference, CanvasRect);
            ConstrainedSize.x = Mathf.Max(InitialWindowSize.x + MouseDifferenceCanvas.x, MinSize.x);
            ConstrainedSize.y = Mathf.Max(InitialWindowSize.y + MouseDifferenceCanvas.y, MinSize.y);
            WindowRect.sizeDelta = ConstrainedSize;
        }
    }

    public void OnEndDrag(PointerEventData Data)
    {
        Dragging = false;
    }

    public void BringToFront()
    {
        transform.SetAsLastSibling();
    }
    public void TogglePin()
    {
        if(transform.parent == UIController.UnpinnedPanel)
        {
            transform.SetParent(UIController.PinnedPanel);
        } else
        {
            transform.SetParent(UIController.UnpinnedPanel);
        }
    }

    public void Close()
    {
        Destroy(gameObject);
    }

}
