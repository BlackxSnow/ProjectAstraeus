using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Control;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [Flags]
    enum ContextButtons //Flags for buttons
    {
        Move,
        Trade,
        Talk,
        Use,
        Attack,
        MeleeAttack,
        Enter,
        Pickup,
        Follow,
        ForceAttack
    }
    List<ContextButtons> ActiveButtons = new List<ContextButtons>();
    Vector3 ClickPosition;
    bool UIEnabled;

    GameObject ContextGUIInstance;

    List<GameObject> Buttons = new List<GameObject>();
    Entity ObjEntity;
    RaycastHit RayHit;

    private void Start()
    {
        Init();
    }
    async void Init()
    {
        await UIController.DataLoadedEvent.WaitAsync();
        ContextGUIInstance = CreateUI.Interactable.ContextMenu(Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        MouseCheck();
    }

    void SetUIStates()
    {
        if (!ContextGUIInstance) Init();
        foreach (ContextButtons buttonEnum in ActiveButtons)
        {
            GameObject ButtonInstance = CreateUI.Interactable.Button(ContextGUIInstance.transform.GetChild(0), out TextMeshProUGUI ButtonText, out Button UIButton);
            Buttons.Add(ButtonInstance);

            switch (buttonEnum)
            {
                case ContextButtons.Move:
                    ButtonText.text = "Move";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Move(RayHit.point); });
                    break;
                case ContextButtons.Trade:
                    ButtonText.text = "Trade";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Trade(ObjEntity as Character); });
                    break;
                case ContextButtons.Talk:
                    ButtonText.text = "Talk";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Talk(ObjEntity as Character); });
                    break;
                case ContextButtons.Use:
                    ButtonText.text = "Use";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Use(ObjEntity as IUsable); }); 
                    break;
                case ContextButtons.Attack:
                    ButtonText.text = "Attack";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Attack(ObjEntity as IDamageable, false); });
                    break;
                case ContextButtons.MeleeAttack:
                    ButtonText.text = "Melee Attack";
                    UIButton.onClick.AddListener(delegate { OrderEvents.MeleeAttack(ObjEntity as Actor); });
                    break;
                case ContextButtons.Enter:
                    ButtonText.text = "Enter";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Enter(ObjEntity as ABiotic); }); //May need to be changed to a sub class of ABiotic with IEnterable
                    break;
                case ContextButtons.Pickup:
                    ButtonText.text = "Pick Up";
                    UIButton.onClick.AddListener(delegate { OrderEvents.PickUp(ObjEntity as DynamicEntity); });
                    break;
                case ContextButtons.Follow:
                    ButtonText.text = "Follow";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Follow(ObjEntity as Actor); });
                    break;
                case ContextButtons.ForceAttack:
                    ButtonText.text = "Force Attack";
                    UIButton.onClick.AddListener(delegate { OrderEvents.Attack(ObjEntity as Actor, false); });
                    break;
                default:
                    break;
            }
        }
        ContextGUIInstance.transform.position = new Vector3(ClickPosition.x, ClickPosition.y);
        ContextGUIInstance.SetActive(true);
    }

    void ClearButtons()
    {
        foreach(GameObject Button in Buttons)
        {
            Destroy(Button);
        }
        Buttons.Clear();
    }

    //Should the context menu be shown?
    void MouseCheck() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !EventSystem.current.IsPointerOverGameObject())
        {
            ClickPosition = Input.mousePosition; //Set initial position clicked
            ContextCheck();
        }

        if (Input.GetKey(KeyCode.Mouse1) && !EventSystem.current.IsPointerOverGameObject()) //Find the distance between the click and current mouse position while held.
        {
            float MouseDistance = Mathf.Sqrt(Mathf.Abs(Input.mousePosition.x - ClickPosition.x) + Mathf.Abs(Input.mousePosition.y - ClickPosition.y)); 
            if (MouseDistance > 5 && MouseDistance < 10 && !UIEnabled)
            {
                UIEnabled = true;
                SetUIStates();
            }
        }
        
        if(Input.GetKeyUp(KeyCode.Mouse1)) //Disable when Mouse1 is no longer pressed
        {
            UIEnabled = false;
            ContextGUIInstance.SetActive(false);
        }
    }

    //Raycasts for object and checks which buttons are applicable
    void ContextCheck () 
    {
        ClearButtons();
        ActiveButtons.Clear();
        Ray _ray = Camera.main.ScreenPointToRay(ClickPosition);
        
        if(Physics.Raycast(_ray, out RayHit))
        {
            GameObject _HitObject = RayHit.collider.gameObject;
            ObjEntity = _HitObject.GetComponent<DynamicEntity>();
            bool _SingleSelected = false;
            if (UI.Control.Selection.SelectedObjs.Count == 1) _SingleSelected = true;

            //Tests for relevant actions by class type
            #region ClassCheck
            if (ObjEntity is DynamicEntity)
            {
                if (ObjEntity is Item)
                {
                    if (_SingleSelected) ActiveButtons.Add(ContextButtons.Pickup);
                }
                else if (ObjEntity is Actor)
                {
                    Actor _Actor = ObjEntity as Actor;
                    if (_Actor.FactionID != 0 && FactionManager.Factions[_Actor.FactionID].GetRelations(0) < 0)
                    {
                        ActiveButtons.Add(ContextButtons.Attack);
                    } else if (_Actor.FactionID != 0)
                    {
                        ActiveButtons.Add(ContextButtons.ForceAttack);
                    }
                    if (ObjEntity is Biotic)
                    {
                        if (ObjEntity is Character)
                        {

                        }
                        else if (ObjEntity is Animal)
                        {

                        }
                    }
                    else if (ObjEntity is ABiotic)
                    {

                    }
                }
            }
            else if (ObjEntity is StaticEntity || ObjEntity == null)
            {
                ActiveButtons.Add(ContextButtons.Move);
            }
            #endregion
        }
    }
}
