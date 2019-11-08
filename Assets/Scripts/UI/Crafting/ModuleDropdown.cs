using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    namespace Crafting
    {
        public class ModuleDropdown : MonoBehaviour
        {
            public enum DropdownOptions
            {
                Module,
                Material
            }
            public TextMeshProUGUI TextMeshComponent;
            public TMP_Dropdown DropdownComponent;
            [HideInInspector]
            public DropdownOptions DropdownType;
            public Enum Selected;
            readonly List<Enum> AddedOptions = new List<Enum>();

            public void Init()
            {
                SetOptions();
            }

            public void SetSelected()
            {
                Selected = AddedOptions[DropdownComponent.value];
            }

            public void SetOptions()
            {
                DropdownComponent.ClearOptions();
                List<string> Options = new List<string>();
                AddedOptions.Clear();

                if (!CraftingUI.CurrentItem) return;

                switch (DropdownType)
                {
                    case DropdownOptions.Module:
                        break;
                    case DropdownOptions.Material:
                        Options = GetMaterialOptions();
                        break;
                    default:
                        break;
                }

                DropdownComponent.AddOptions(Options);
                SetSelected();
            }

            private List<string> GetMaterialOptions()
            {
                List<string> Options = new List<string>();
                foreach (Materials.MaterialTypes MaterialType in CraftingUI.CurrentModule.CompatibleMaterials)
                {
                    Options.Add(string.Format("{0}", MaterialType));
                    AddedOptions.Add(MaterialType);
                }
                return Options;
            }

            private List<string> GetModuleOptions()
            {
                List<string> Options = new List<string>();
                foreach (Modules.AdditionalModule.ModulesEnum Module in CraftingUI.CurrentItem.BaseStats.CompatibleModules)
                {
                    Options.Add(string.Format("{0}", Module));
                    AddedOptions.Add(Module);
                }
                return Options;
            }

            public void OnValueChanged()
            {
                SetSelected();
                switch (DropdownType)
                {
                    case DropdownOptions.Module:
                        break;
                    case DropdownOptions.Material:
                        Materials.Material material = Materials.MaterialDict[(Materials.MaterialTypes)Selected];
                        CraftingUI.CurrentModule.SetStat(ItemTypes.StatsEnum.Material, material);
                        CraftingUI.UpdateKVPs(true, false);
                        break;
                    default:
                        break;
                }
            }
        }  
    }
}
