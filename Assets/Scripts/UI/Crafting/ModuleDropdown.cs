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
                Material,
                FireMode
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
                Enum ModuleEnumValue;
                if (DropdownType == DropdownOptions.FireMode)
                {
                    ModuleEnumValue = CraftingUI.CurrentModule.GetStat<Firearm.FireModes>(ItemTypes.StatsEnum.FireMode);
                } else
                {
                    ModuleEnumValue = CraftingUI.CurrentModule.GetStat<Materials.Material>(ItemTypes.StatsEnum.Material).Name;
                }
                Selected = AddedOptions.Find(E => E.Equals(ModuleEnumValue));
                DropdownComponent.value = Selected != null ? AddedOptions.IndexOf(Selected) : 0;
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
                    case DropdownOptions.FireMode:
                        Options = GetFireModeOptions();
                        break;
                    default:
                        break;
                }

                DropdownComponent.AddOptions(Options);
                DropdownComponent.value = Selected != null ? AddedOptions.IndexOf(Selected) : 0;
            }

            private List<string> GetFireModeOptions()
            {
                List<string> Options = new List<string>();
                foreach(Firearm.FireModes FireMode in CraftingUI.CurrentModule.CompatibleFireModes)
                {
                    Options.Add(FireMode.ToString());
                    AddedOptions.Add(FireMode);
                }
                return Options;
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
                        CraftingUI.UpdateKVPs(true, true);
                        break;
                    case DropdownOptions.FireMode:
                        CraftingUI.CurrentModule.SetStat(ItemTypes.StatsEnum.FireMode, (Firearm.FireModes)Selected);
                        CraftingUI.UpdateKVPs(true, true);
                        break;
                    default:
                        break;
                }
            }
        }  
    }
}
