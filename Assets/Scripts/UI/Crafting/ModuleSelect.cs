using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ItemModule.AdditionalModule;

public class ModuleSelect : MonoBehaviour
{
    public TMP_Dropdown DropdownComponent;
    [HideInInspector]
    public ModuleList SelectedModule;

    public static Dictionary<string, ModuleList> ModuleDict = new Dictionary<string, ModuleList>()
    {
        { "Plating", ModuleList.Plating },
        { "Reactor", ModuleList.Reactor },
        { "Shielding", ModuleList.Shielding },
        { "WeaponPlaceHolderModule", ModuleList.WeaponPlaceHolderModule }
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

        foreach (ModuleList Flag in Utility.GetFlags(CraftingUI.CurrentItem.Core.AvailableModules))
        {
            Options.Add(string.Format("{0}", Flag));
            Enum.GetName(typeof(ModuleList), Flag);
        }
        DropdownComponent.AddOptions(Options);
        SetSelected();
    }
}
