using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModuleDisplayPanel : MonoBehaviour
{
    public ItemModule.AdditionalModule Module;
    public GameObject StatPanel;
    public TextMeshProUGUI Name;

    public void DeleteModule()
    {
        CraftingUI.RemoveModule(Module);
        Destroy(this.gameObject);
    }
}
