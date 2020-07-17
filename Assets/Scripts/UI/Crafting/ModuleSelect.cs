//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using Modules;

//namespace UI
//{
//    namespace Crafting
//    {
//        public class ModuleSelect : MonoBehaviour
//        {
//            public TMP_Dropdown DropdownComponent;
//            [HideInInspector]
//            public ModulesEnum SelectedModule;
//            readonly List<ModulesEnum> AddedOptions = new List<ModulesEnum>();

//            private void Start()
//            {
//                DropdownComponent = GetComponent<TMP_Dropdown>();
//                SetOptions();
//            }

//            public void SetSelected()
//            {
//                SelectedModule = AddedOptions[DropdownComponent.value];
//            }

//            public void SetOptions()
//            {
//                DropdownComponent.ClearOptions();
//                List<string> Options = new List<string>();
//                AddedOptions.Clear();

//                if (!CraftingUI.CurrentItem) return;

//                foreach (ModulesEnum Module in CraftingUI.CurrentItem.BaseStats.CompatibleModules)
//                {
//                    Options.Add(string.Format("{0}", Module));
//                    AddedOptions.Add(Module);
//                }
//                DropdownComponent.AddOptions(Options);
//                SetSelected();
//            }
//        } 
//    }
//}