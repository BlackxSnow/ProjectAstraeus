using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenu : MonoBehaviour
{
    [Flags]
    enum ContextButtons //Flags for buttons
    {
        Move = 1,
        Trade = 2,
        Talk = 4,
        Use = 8,
        Attack = 16,
        MeleeAttack = 32,
        Enter = 64,
        Pickup = 128,
        Follow = 256,
        ForceAttack = 512
    }
    ContextButtons ActiveFlags;
    Vector3 ClickPosition;
    bool UIEnabled;

    public GameObject ContextGUI;
    GameObject ContextGUIInstance;

    public GameObject OrderButton;
    List<GameObject> Buttons = new List<GameObject>();
    Entity ObjEntity;
    RaycastHit RayHit;

    Canvas canvas;

    private void Start()
    {
        Initialise();
    }
    void Initialise()
    {
        canvas = FindObjectOfType<Canvas>();
        ContextGUIInstance = Instantiate(ContextGUI, canvas.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        MouseCheck();
    }

    void SetUIStates()
    {
        if (!ContextGUIInstance) Initialise();
        foreach (string ButtonName in Enum.GetNames(typeof(ContextButtons)))
        {
            ContextButtons ButtonFlag;
            Enum.TryParse(ButtonName, out ButtonFlag);
            if(ActiveFlags.HasFlag(ButtonFlag))
            {
                GameObject ButtonInstance = Instantiate(OrderButton, ContextGUIInstance.transform.GetChild(0));
                Text ButtonText = ButtonInstance.transform.GetChild(0).GetComponent<Text>();
                Button button = ButtonInstance.GetComponent<Button>();
                Buttons.Add(ButtonInstance);

                switch (ButtonName)
                {
                    case "Move":
                        ButtonText.text = "Move";
                        button.onClick.AddListener(delegate { OrderEvents.Move(RayHit.point); });
                        break;
                    case "Trade":
                        ButtonText.text = "Trade";
                        button.onClick.AddListener(delegate { OrderEvents.Trade(ObjEntity as Character); });
                        break;
                    case "Talk":
                        ButtonText.text = "Talk";
                        button.onClick.AddListener(delegate { OrderEvents.Talk(ObjEntity as Character); });
                        break;
                    case "Use":
                        ButtonText.text = "Use";
                        button.onClick.AddListener(delegate { OrderEvents.Use(ObjEntity as StaticEntity); }); //This will need to be changed to a sub class of StaticEntity with IUsable
                        break;
                    case "Attack":
                        ButtonText.text = "Attack";
                        button.onClick.AddListener(delegate { OrderEvents.Attack(ObjEntity as Actor); });
                        break;
                    case "MeleeAttack":
                        ButtonText.text = "Melee Attack";
                        button.onClick.AddListener(delegate { OrderEvents.MeleeAttack(ObjEntity as Actor); });
                        break;
                    case "Enter":
                        ButtonText.text = "Enter";
                        button.onClick.AddListener(delegate { OrderEvents.Enter(ObjEntity as ABiotic); }); //May need to be changed to a sub class of ABiotic with IEnterable
                        break;
                    case "Pickup":
                        ButtonText.text = "Pick Up";
                        button.onClick.AddListener(delegate { OrderEvents.PickUp(ObjEntity as DynamicEntity); });
                        break;
                    case "Follow":
                        ButtonText.text = "Follow";
                        button.onClick.AddListener(delegate { OrderEvents.Follow(ObjEntity as Actor); });
                        break;
                    case "ForceAttack":
                        ButtonText.text = "Force Attack";
                        button.onClick.AddListener(delegate { OrderEvents.Attack(ObjEntity as Actor); });
                        break;
                    default:
                        break;
                }
            }
        }
        ContextGUIInstance.transform.position = new Vector3(ClickPosition.x + 50, ClickPosition.y - 50);
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
        ActiveFlags = 0; //Clear flags
        Ray _ray = Camera.main.ScreenPointToRay(ClickPosition);
        
        if(Physics.Raycast(_ray, out RayHit))
        {
            GameObject _HitObject = RayHit.collider.gameObject;
            ObjEntity = _HitObject.GetComponent<DynamicEntity>();
            bool _SingleSelected = false;
            if (Selection.SelectedObjs.Count == 1) _SingleSelected = true;

            //Tests for relevant actions by class type
            #region ClassCheck
            if (ObjEntity is DynamicEntity)
            {
                if (ObjEntity is Item)
                {
                    if (_SingleSelected) ActiveFlags |= ContextButtons.Pickup;
                }
                else if (ObjEntity is Actor)
                {
                    Actor _Actor = ObjEntity as Actor;
                    if (_Actor.FactionID != 0 && FactionManager.Factions[_Actor.FactionID].GetRelations(0) < 0)
                    {
                        ActiveFlags |= ContextButtons.Attack;
                    } else if (_Actor.FactionID != 0)
                    {
                        ActiveFlags |= ContextButtons.ForceAttack;
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
                ActiveFlags |= ContextButtons.Move;
            }
            #endregion
        }
    }
}
