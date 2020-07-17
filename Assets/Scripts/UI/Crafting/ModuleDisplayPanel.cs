//using System.Collections;
//using System.Collections.Generic;
//using Modules;
//using UI.Crafting;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class ModuleDisplayPanel : MonoBehaviour
//{
//    public AdditionalModule Module;
//    public GameObject StatPanel;
//    public TextMeshProUGUI Name;

//    public Image PanelImage;

//    public void DeleteModule()
//    {
//        CraftingUI.ActiveUI.RemoveModule(Module);
//        Destroy(gameObject);
//    }

//    public void OnEdit()
//    {
//        CraftingUI.ActiveUI.SelectModule(Module, this);
//    }

//    public void Select()
//    {
//        PanelImage.color = UI.StyleSheet.Colours.Selected;
//    }
//    public void Deselect()
//    {
//        PanelImage.color = UI.StyleSheet.Colours.Base;
//    }
//}
