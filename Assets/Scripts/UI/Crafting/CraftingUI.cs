using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ItemModule.AdditionalModule;

public class CraftingUI : Window
{
    [Header("Display UI Elements")]
    public GameObject UI_ModuleStatsPanel;
    public GameObject UI_TotalStatsPanel;
    public GameObject UI_ModuleDisplayList;
    public GameObject UI_ModificationUIPanel;
    [Space(10)]
    [Header("Interactive UI Elements")]
    public GameObject UI_ItemListPanel;
    private ToggleGroup ItemToggles;
    public ModuleSelect ModuleDropdown;
    [Space(10)]

    [HideInInspector]
    public static ItemData CurrentItem;
    public static ItemModule.AdditionalModule CurrentModule;

    public struct KVPStruct
    {
        public List<GameObject> gameObjects;
        public List<KeyValuePanel> Scripts;
        public List<KeyValueList> ListScripts;
        public KeyValueGroup Group;

        public KVPStruct(KeyValueGroup Group)
        {
            gameObjects = new List<GameObject>();
            Scripts = new List<KeyValuePanel>();
            ListScripts = new List<KeyValueList>();
            this.Group = Group;
        }

        public void AddKVP(GameObject KVP)
        {
            gameObjects.Add(KVP);
            Scripts.Add(KVP.GetComponent<KeyValuePanel>());
        }
        public void AddKVP(GameObject[] KVPs)
        {
            for (int i = 0; i < KVPs.Length; i++)
            {
                AddKVP(KVPs[i]);
            }
        }
        public void AddKVP(List<GameObject> KVPs)
        {
            foreach(GameObject KVP in KVPs)
            {
                AddKVP(KVP);
            }
        }
        public void AddKVPList(GameObject KVPList)
        {
            gameObjects.Add(KVPList);
            KeyValueList Script = KVPList.GetComponent<KeyValueList>();
            ListScripts.Add(Script);
            for (int i = 0; i < Script.KVPs.Length; i++)
            {
                Scripts.Add(Script.KVPs[i].GetComponent<KeyValuePanel>());
            }
        }
        public void AddKVPList(GameObject[] KVPLists)
        {
            for (int i = 0; i < KVPLists.Length; i++)
            {
                AddKVPList(KVPLists[i]);
            }
        }
        public void AddKVPList(List<GameObject> KVPLists)
        {
            foreach (GameObject KVPList in KVPLists)
            {
                AddKVPList(KVPList);
            }
        }
        public void Clear()
        {
            gameObjects.Clear();
            Scripts.Clear();
            Group.ClearGroup();
        }
    }

    private static KVPStruct TotalStatKVPInfo;
    private static KVPStruct ModuleStatKVPInfo;

    private List<GameObject> ModuleDisplays = new List<GameObject>();
    private List<GameObject> ModificationUI = new List<GameObject>();

    private void Awake()
    {
        ItemToggles = UI_ItemListPanel.GetComponent<ToggleGroup>();
        TotalStatKVPInfo = new KVPStruct(new KeyValueGroup());
        ModuleStatKVPInfo = new KVPStruct(new KeyValueGroup());
    }
    public void ClearAll()
    {
        ClearKVPs(true, true);
        ClearValueSliders();
        ClearModuleList();
    }
    public void ClearKVPs(bool Module, bool Total)
    {
        if (Module)
        {
            foreach (GameObject Panel in ModuleStatKVPInfo.gameObjects) Destroy(Panel);
            ModuleStatKVPInfo.Clear();
        }

        if (Total)
        {
            foreach (GameObject Panel in TotalStatKVPInfo.gameObjects) Destroy(Panel);
            TotalStatKVPInfo.Clear();
        }
    }
    public void ClearValueSliders()
    {
        foreach (GameObject ValueSlider in ModificationUI) Destroy(ValueSlider);
        ModificationUI.Clear();
    }
    public void ClearModuleList()
    {
        foreach (GameObject ModuleDisplay in ModuleDisplays) Destroy(ModuleDisplay);
        ModuleDisplays.Clear();
    }
    public void NewItem()
    {
        IEnumerator<Toggle> ActiveToggleEnum = ItemToggles.ActiveToggles().GetEnumerator();
        ActiveToggleEnum.MoveNext();
        Toggle ActiveToggle = ActiveToggleEnum.Current;

        ItemTypes.Types Type = ActiveToggle.GetComponent<ItemSelectToggle>().Type;

        ClearAll();
        CurrentItem = new ItemData(Type);

        ModuleDropdown.SetOptions();
        InitialiseTotalStatDisplay();
    }

    public void NewModule()
    {
        switch (ModuleDropdown.SelectedModule)
        {
            case ModuleListEnum.Plating:
                CurrentModule = new Modules.Plating();
                break;
            case ModuleListEnum.Reactor:
                CurrentModule = new Modules.Reactor();
                break;
            case ModuleListEnum.Shielding:
                CurrentModule = new Modules.Shield();
                break;
            case ModuleListEnum.WeaponPlaceHolderModule:
                CurrentModule = new WeaponPlaceHolderModule();
                break;
            default:
                throw new ArgumentException(string.Format("Selected module {0} is unhandled by CraftingUI.NewModule", ModuleDropdown.SelectedModule));
        }

        InitialiseModuleStatDisplay();
        InitialiseModificationUI();
    }

    public void AddModule()
    {
        if (!CurrentModule) return;
        ClearKVPs(true, false);
        ModuleStatKVPInfo.Group.ClearGroup();
        ClearValueSliders();
        CurrentItem.Modules.Add(CurrentModule);
        CurrentItem.SetStats();
        ModuleDisplays.Add(UIController.InstantiateModulePanel(CurrentModule, UI_ModuleDisplayList.transform));
        CurrentModule = null;
        UpdateKVPs(false, true);
    }

    public static void RemoveModule(ItemModule.AdditionalModule Module)
    {
        CurrentItem.Modules.Remove(Module);
        CurrentItem.SetStats();
        UpdateKVPs(false, true);
    }


    public void CreateItem()
    {
        if (!CurrentItem) return;
        GameObject ItemInstance = Instantiate(ItemTypes.ItemBasePrefab, new Vector3(5, 0.5f, 5), new Quaternion(0, 0, 0, 0));
        Item Script = ItemInstance.GetComponent<Item>();
        Script.Name = string.Format("{0}", CurrentItem.Type);
        Script.Data = CurrentItem;

        ClearAll();
        CurrentModule = null;
        CurrentItem = null;
    }
    void InitialiseModificationUI()
    {
        ClearValueSliders();
        foreach (ModifiableStatsEnum Stat in Utility.GetFlags(CurrentModule.ModifiableStats))
        {
            ModificationUI.Add(UIController.InstantiateValueSlider(Stat.ToString(), (ItemTypes.StatFlagsEnum)Stat, UI_ModificationUIPanel.transform, 0, 10));
        }
    }

    void InitialiseModuleStatDisplay()
    {
        ClearKVPs(true, false);

        GameObject[] KVPListArray;
        GameObject[] KVPArray;
        KVPArray = CurrentModule.InstantiateStatKVPs(UI_ModuleStatsPanel.transform, ModuleStatKVPInfo.Group, out KVPListArray);

        ModuleStatKVPInfo.AddKVP(KVPArray);
        ModuleStatKVPInfo.AddKVPList(KVPListArray);
    }

    
    void InitialiseTotalStatDisplay()
    {
        ClearKVPs(false, true);

        //General Stats
        List<GameObject> KVPs = CurrentItem.InstantiateStatKVPs(true, out List<GameObject> KVPLists, UI_TotalStatsPanel.transform, TotalStatKVPInfo.Group);
        TotalStatKVPInfo.AddKVP(KVPs);
        TotalStatKVPInfo.AddKVPList(KVPLists);
    }

    public static void UpdateKVPs(bool Module, bool Total)
    {
        if(Module)
        {
            foreach(KeyValuePanel KVP in ModuleStatKVPInfo.Scripts)
            {
                CurrentModule.CalculateStats();
                KVP.UpdateValue();
            }
        }
        if (Total)
        {
            foreach(KeyValuePanel KVP in TotalStatKVPInfo.Scripts)
            {
                KVP.UpdateValue();
            }
        }
    }
}
