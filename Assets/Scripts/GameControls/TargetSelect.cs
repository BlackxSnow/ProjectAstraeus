using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityAsync;
using System.Threading.Tasks;
using UnityEngine;
using Nito.AsyncEx;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;

public class TargetSelect : MonoBehaviour//, IPointerDownHandler
{
    public static Texture2D SelectCursor;
    //private static AsyncAutoResetEvent SelectEvent = new AsyncAutoResetEvent(false);
    //private static bool WaitingForClick = false;

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (WaitingForClick)
    //    {
    //        SelectEvent.Set();
    //    }
    //}

    public static async Task<ISelectable> StartSelect()
    {
        Cursor.SetCursor(SelectCursor, new Vector2(64,64), CursorMode.Auto);
        //WaitingForClick = true;
        Selection.AllowSelection = false;
        ISelectable Result = null;

        //await SelectEvent.WaitAsync();
        await Await.Until(() => Input.GetMouseButtonDown(0));

        RaycastHit HitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out HitInfo))
        {
            HitInfo.collider.TryGetComponent(out Result);
        }

        Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
        Selection.AllowSelection = true;
        return Result;
    }
}
