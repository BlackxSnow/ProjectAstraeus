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
        TotalStatKVPInfo = new KVPStruct(UI_TotalStatsPanel.GetComponent<KeyValueGroup>());
        ModuleStatKVPInfo = new KVPStruct(UI_ModuleStatsPanel.GetComponent<KeyValueGroup>());
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
            case ModuleList.Plating:
                CurrentModule = new Plating();
                break;
            case ModuleList.Reactor:
                CurrentModule = new Reactor();
                break;
            case ModuleList.Shielding:
                CurrentModule = new Shielding();
                break;
            case ModuleList.WeaponPlaceHolderModule:
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
        switch (CurrentModule)
        {
            case Plating plating:
                ModificationUI.Add(UIController.InstantiateValueSlider("Thickness", ModifiableStats.Thickness, UI_ModificationUIPanel.transform, 0, 10));
                break;
            case Reactor reactor:
                ModificationUI.Add(UIController.InstantiateValueSlider("Power Output", ModifiableStats.Power, UI_ModificationUIPanel.transform, 0, 10));
                break;
            case Shielding shielding:
                ModificationUI.Add(UIController.InstantiateValueSlider("Shield", ModifiableStats.Shield, UI_ModificationUIPanel.transform, 0, 10));
                break;
            case WeaponPlaceHolderModule weaponPlaceHolderModule:

                break;
            default:
                throw new ArgumentException(string.Format("Selected module {0} is unhandled by CraftingUI.InitialiseModuleStatDisplay", ModuleDropdown.SelectedModule));
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
        UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
        CostData[0] = new UIController.KVPData<float>("Iron", CurrentItem.Stats.Cost.Iron, null, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Iron);
        CostData[1] = new UIController.KVPData<float>("Copper", CurrentItem.Stats.Cost.Copper, null, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Copper);
        CostData[2] = new UIController.KVPData<float>("Alloy", CurrentItem.Stats.Cost.Alloy, null, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Alloy);

        TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP("Item Type", CurrentItem.Type, UI_TotalStatsPanel.transform));

        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Armour))         TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Armour),         CurrentItem.Stats.Armour,           UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Armour));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Shield))         TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Shield),         CurrentItem.Stats.Shield,           UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Shield));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Power))          TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Power),          CurrentItem.Stats.Power,            UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Power));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.PowerUse))       TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.PowerUse),       CurrentItem.Stats.PowerUse,         UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.PowerUse));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Damage))         TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Damage),         CurrentItem.Stats.Damage,           UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Damage));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.ArmourPiercing)) TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.ArmourPiercing), CurrentItem.Stats.ArmourPiercing,   UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.ArmourPiercing));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.AttackSpeed))    TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.AttackSpeed),    CurrentItem.Stats.AttackSpeed,      UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.AttackSpeed));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Range))          TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Range),          CurrentItem.Stats.Range,            UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Range));


        TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP("Size", CurrentItem.Stats.Size, UI_TotalStatsPanel.transform, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Size));
        TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP("Mass", CurrentItem.Stats.Mass, UI_TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Mass));
        TotalStatKVPInfo.AddKVPList(UIController.InstantiateKVPList("Cost", CostData, UI_TotalStatsPanel.transform));
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
