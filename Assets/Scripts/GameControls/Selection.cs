using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Control
{
    public class Selection : MonoBehaviour
    {
        public static bool AllowSelection = true;
        Rect RectSelect;
        Texture2D RectSelectTex;
        bool Selecting; //Is the user selecting?
        public static bool ViewableSelected; //Is a viewable object selected?
        public static List<ISelectable> SelectedObjs = new List<ISelectable>(); //The selected ISelectables
        Camera Cam;
        bool MultiSelected = false;

        Vector3 MousePosInit; //Initial Position at click

        void Awake()
        {
            //Create selection box texture
            RectSelectTex = new Texture2D(1, 1);
            RectSelectTex.SetPixel(0, 0, new Color(0.08f, 0.08f, 0.25f, 0.25f));
            RectSelectTex.Apply();
        }

        private void OnEnable()
        {
            Controller.InputControls.InGame.Select.performed += OnSelectPerformed;
        }

        void OnGUI()
        {
            if (Selecting)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                float _Width = mousePosition.x - MousePosInit.x;
                float _Height = (Screen.height - mousePosition.y) - MousePosInit.y;
                RectSelect = new Rect(MousePosInit.x, MousePosInit.y, _Width, _Height);
                GUI.DrawTexture(RectSelect, RectSelectTex);
                if (Mathf.Abs(_Width) > 5 || Mathf.Abs(_Height) > 5) MultiSelect(); //Only enable selection if the rectangle is large enough -- Stops instant deselection when raycasting for SingleSelect()
            }
        }
        //TODO: Fix selection
        void Update()
        {
            if (Cam != Camera.main) Cam = Camera.main;

        }

        private void OnSelectPerformed(InputAction.CallbackContext context)
        {
            if (AllowSelection)
            {
                if (!context.ReadValueAsButton())
                {
                    if (!MultiSelected && !EventSystem.current.IsPointerOverGameObject()) SingleSelect();
                    Selecting = false;
                    if (MultiSelected) SetSelectionDisplay();
                    MultiSelected = false;
                    FinaliseSelection();
                }
                else if (context.ReadValueAsButton() && !EventSystem.current.IsPointerOverGameObject())
                {
                    Selecting = true;
                    MousePosInit = Mouse.current.position.ReadValue();
                    MousePosInit.y = Screen.height - MousePosInit.y;
                }

            }
        }

        public void SelectPCByName(string Name)
        {
            if (Controller.InputControls.InGame.AdditiveModifier.ReadValue<float>() == 0 || ViewableSelected == true) Deselect();
            Character _PC = EntityManager.PlayerCharacters.Find(PlayerCharacters => PlayerCharacters.Name == Name);

            if (Controller.InputControls.InGame.AdditiveModifier.ReadValue<float>() > 0)
            {
                if (_PC.Selected) SelectedObjs.Remove(_PC); else SelectedObjs.Add(_PC);
                _PC.Selected = !_PC.Selected;
            }
            else
            {
                _PC.Selected = true;
                SelectedObjs.Add(_PC);
            }
            SetSelectionDisplay();
        }
        public void SelectPCByName(Text Name)
        {
            SelectPCByName(Name.text);
        }

        void Deselect()
        {
            ViewableSelected = false;
            foreach (ISelectable _Selectable in SelectedObjs)
            {
                _Selectable.Selected = false;
                _Selectable.FinalisedSelection = false;
            }
            SelectedObjs.Clear();
            SetSelectionDisplay();
        }

        void SingleSelect() //Select single with raycast
        {
            if (Controller.InputControls.InGame.AdditiveModifier.ReadValue<float>() == 0 || ViewableSelected == true) Deselect(); //If shift is not held or a viewable was selected, deselect

            RaycastHit _SelectRayHit;
            Ray _ray = Cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(_ray, out _SelectRayHit))
            {
                GameObject _HitObject = _SelectRayHit.collider.gameObject;
                ISelectable _HitIselectable = _HitObject.GetComponent<ISelectable>();
                if (_HitIselectable != null)
                {
                    if (_HitIselectable.ViewableOnly) //If selecting a viewableOnly unit, deselect all others.
                    {
                        Deselect();
                        ViewableSelected = true;
                    }
                    else
                    {
                        ViewableSelected = false;
                    }
                    if (Controller.InputControls.InGame.AdditiveModifier.ReadValue<float>() > 0)
                    {
                        if (_HitIselectable.Selected) _HitIselectable.FinalisedSelection = false;
                        if (_HitIselectable.Selected)
                        {
                            SelectedObjs.Remove(_HitIselectable);
                        }
                        else
                        {
                            SelectedObjs.Add(_HitIselectable);
                        }
                        _HitIselectable.Selected = !_HitIselectable.Selected;
                    }
                    else
                    {
                        SelectedObjs.Add(_HitIselectable);
                        _HitIselectable.Selected = true;
                    }
                }
            }
            SetSelectionDisplay();
        }

        void MultiSelect() //Select multiple in rectangle
        {
            MultiSelected = true;
            foreach (ISelectable _Selectable in EntityManager.Selectables)
            {
                if (_Selectable.ViewableOnly) continue; //Does not select viewables
                if (Controller.InputControls.InGame.AdditiveModifier.ReadValue<float>() > 0 && _Selectable.FinalisedSelection) continue; //Does not run on finalised selections if shift is held

                MonoBehaviour SelectableObj = _Selectable as MonoBehaviour;
                Vector3 _WTSP = Cam.WorldToScreenPoint(SelectableObj.transform.position);
                Vector3 _ScreenPoint = new Vector3(_WTSP.x, Screen.height - _WTSP.y, _WTSP.z); //WTSP y must be flipped
                if (RectSelect.Contains(_ScreenPoint, true))
                {
                    if (!SelectedObjs.Contains(_Selectable)) SelectedObjs.Add(_Selectable);
                    _Selectable.Selected = true;
                }
                else
                {
                    if (SelectedObjs.Contains(_Selectable)) SelectedObjs.Remove(_Selectable);
                    _Selectable.Selected = false;
                }
            }
        }

        //Stops MultiSelect deselecting previously selected objects when shift is held
        void FinaliseSelection()
        {
            foreach (ISelectable _Selectable in EntityManager.Selectables)
            {
                if (_Selectable.Selected) { _Selectable.FinalisedSelection = true; }
            }
        }

        void SetSelectionDisplay()
        {
            if (SelectedObjs.Count == 0)
            {
                HUDController.HUDControl.ClearSelectionUI();
                return;
            }
            HUDController.HUDControl.DisplaySelectionUI(SelectedObjs[0]);
        }
    } 
}
