using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Modules;

namespace UI
{
    namespace Crafting
    {
        public class ModuleSelect : MonoBehaviour
        {
            public TMP_Dropdown DropdownComponent;
            [HideInInspector]
            public ModuleListEnum SelectedModule;

            public static Dictionary<string, ModuleListEnum> ModuleDict = new Dictionary<string, ModuleListEnum>()
    {
        { "Plating", ModuleListEnum.Plating },
        { "Reactor", ModuleListEnum.Reactor },
        { "Shielding", ModuleListEnum.Shielding },
        { "WeaponPlaceHolderModule", ModuleListEnum.WeaponPlaceHolderModule }
    };

            private void Start()
            {
                DropdownComponent = GetComponent<TMP_Dropdown>();
                SetOptions();
            }

            public void SetSelected()
            {
                ModuleDict.TryGetValue(DropdownComponent.captionText.text, out SelectedModule);
            }

            public void SetOptions()
            {
                DropdownComponent.ClearOptions();
                List<string> Options = new List<string>();

                if (!CraftingUI.CurrentItem) return;

                foreach (ModuleListEnum Flag in Utility.GetFlags(CraftingUI.CurrentItem.Core.AvailableModules))
                {
                    Options.Add(string.Format("{0}", Flag));
                    Enum.GetName(typeof(ModuleListEnum), Flag);
                }
                DropdownComponent.AddOptions(Options);
                SetSelected();
            }
        } 
    }
}