using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Control;
using UI.Crafting;
using UnityEngine;
using UnityEngine.UI;

public class ModuleButton : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField]
    private TextMeshProUGUI TitleText;
    [SerializeField]
    private Button UI_Edit;
    [SerializeField]
    private ConfirmButton UI_Delete;
#pragma warning restore 0649

    public Items.Modules.PartModule RefModule;
    [HideInInspector]
    public CraftingUI UI_CraftingWindow;

    public void DeleteModule()
    {

    }


    public void EditModule()
    {

    }

    public void SetTitle(string title)
    {
        TitleText.text = title;
    }
}
