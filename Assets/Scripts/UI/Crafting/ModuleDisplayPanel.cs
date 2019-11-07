using System.Collections;
using System.Collections.Generic;
using Modules;
using UI.Crafting;
using UnityEngine;
using TMPro;

public class ModuleDisplayPanel : MonoBehaviour
{
    public AdditionalModule Module;
    public GameObject StatPanel;
    public TextMeshProUGUI Name;

    public void DeleteModule()
    {
        CraftingUI.RemoveModule(Module);
        Destroy(this.gameObject);
    }
}
