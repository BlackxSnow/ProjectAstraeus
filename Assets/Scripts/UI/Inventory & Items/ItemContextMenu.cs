﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using UI.Control;
using Items;

public class ItemContextMenu : MonoBehaviour, IPointerDownHandler
{
    enum ButtonsEnum
    {
        Use,
        Properties
    }
    struct ButtonStruct
    {
        public string Name;
        public delegate void ListenerDelegate(Actor Source);
        public ListenerDelegate Listener;
    }
    private List<ButtonStruct> Buttons = new List<ButtonStruct>();
    public Item RefItem;
    private GameObject MenuInstance;

    public void Init()
    {
        if (RefItem == null) throw new NullReferenceException($"RefItem is undefined on {gameObject.name}");
        CanvasRaycaster = UIController.CanvasObject.GetComponent<GraphicRaycaster>();
        GetValidButtons();
    }

    private void GetValidButtons()
    {
        if (RefItem.Container?.Owner.FactionID == 0 && RefItem.Container?.Owner is Character) //Ensure the item is stored in an inventory, that the inventory is owned by the player, and that it's a character inventory
        {
            if (RefItem is IUsable UsableItem)
            {
                ButtonStruct button;
                button.Name = "Use";
                button.Listener = UsableItem.Use;
                Buttons.Add(button);
            }
        }
    }

    private void BuildUI(Vector2 position)
    {
        if (MenuInstance) DestroyUI();
        MenuInstance = CreateUI.Interactable.ContextMenu(position);
        foreach (ButtonStruct ButtonData in Buttons)
        {
            CreateUI.Interactable.Button(MenuInstance.transform, out TextMeshProUGUI ButtonText, out Button UIButton);
            ButtonText.text = ButtonData.Name;
            UIButton.onClick.AddListener(delegate { ButtonData.Listener(RefItem.Container.Owner as Actor); });
        }
        MenuInstance.SetActive(true);
    }

    private void DestroyUI()
    {
        Destroy(MenuInstance);
    }
    private bool BuiltThisFrame = false;
    public void OnPointerDown(PointerEventData eventData) //Event, runs when object is clicked. Spawns a UI Menu
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            BuildUI(eventData.position);
            BuiltThisFrame = true;
        }
    }
    GraphicRaycaster CanvasRaycaster;
    private void Update()
    {

        if (Input.anyKeyDown && MenuInstance && !BuiltThisFrame) // && MouseClickInApplication && MouseIsNotOverMenu
        {
            PointerEventData Data = new PointerEventData(EventSystem.current);
            Data.position = Input.mousePosition;
            List<RaycastResult> Results = new List<RaycastResult>();
            CanvasRaycaster.Raycast(Data, Results);

            if (!Results.Exists(r => r.gameObject == MenuInstance))
            {
                DestroyUI();
            }
        }
        BuiltThisFrame = false;
    }

}
