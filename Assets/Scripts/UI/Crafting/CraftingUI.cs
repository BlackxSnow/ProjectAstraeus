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
    public GameObject ModuleStatsPanel;
    public GameObject TotalStatsPanel;
    public GameObject ModuleListContent;
    public GameObject ModificationUIPanel;
    [Space(10)]
    [Header("Interactive UI Elements")]
    public GameObject ItemListPanel;
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
        public void AddKVPList(GameObject KVPList)
        {
            gameObjects.Add(KVPList);
            ListScripts.Add(KVPList.GetComponent<KeyValueList>());
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

    private List<GameObject> ModificationUI = new List<GameObject>();

    private void Awake()
    {
        ItemToggles = ItemListPanel.GetComponent<ToggleGroup>();
        TotalStatKVPInfo = new KVPStruct(TotalStatsPanel.GetComponent<KeyValueGroup>());
        ModuleStatKVPInfo = new KVPStruct(ModuleStatsPanel.GetComponent<KeyValueGroup>());
    }

    public void NewItem()
    {
        IEnumerator<Toggle> ActiveToggleEnum = ItemToggles.ActiveToggles().GetEnumerator();
        ActiveToggleEnum.MoveNext();
        Toggle ActiveToggle = ActiveToggleEnum.Current;

        ItemTypes.Types Type = ActiveToggle.GetComponent<ItemSelectToggle>().Type;

        ClearKVPs(true, true);
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
        ClearKVPs(true, false);
        ClearValueSliders();
        CurrentItem.Modules.Add(CurrentModule);
        CurrentItem.SetStats();
        UpdateKVPs(false, true);
    }

    public void ClearKVPs(bool Module, bool Total)
    {
        if(Module)
        {
            foreach(GameObject Panel in ModuleStatKVPInfo.gameObjects) Destroy(Panel);
            ModuleStatKVPInfo.Clear();
        }

        if(Total)
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

    void InitialiseModificationUI()
    {
        ClearValueSliders();
        switch (CurrentModule)
        {
            case Plating plating:
                ModificationUI.Add(UIController.InstantiateValueSlider("Thickness", ModifiableStats.Thickness, ModificationUIPanel.transform, 0, 10));
                break;
            case Reactor reactor:
                ModificationUI.Add(UIController.InstantiateValueSlider("Power Output", ModifiableStats.Power, ModificationUIPanel.transform, 0, 10));
                break;
            case Shielding shielding:
                ModificationUI.Add(UIController.InstantiateValueSlider("Shield", ModifiableStats.Shield, ModificationUIPanel.transform, 0, 10));
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

        //Stat Switch-case
        switch (CurrentModule)
        {
            case Plating plating:
                ModuleStatKVPInfo.AddKVP(UIController.InstantiateKVP("Armour", plating.Armour, ModuleStatsPanel.transform, 0, ValueDelegate: KeyValuePanel.ModuleGetValue.Armour));
                break;
            case Reactor reactor:
                ModuleStatKVPInfo.AddKVP(UIController.InstantiateKVP("Power", reactor.Power, ModuleStatsPanel.transform, 0, ValueDelegate: KeyValuePanel.ModuleGetValue.Power));
                break;
            case Shielding shielding:
                ModuleStatKVPInfo.AddKVP(UIController.InstantiateKVP("Shield", shielding.Shield, ModuleStatsPanel.transform, 0, ValueDelegate: KeyValuePanel.ModuleGetValue.Shield));
                break;
            case WeaponPlaceHolderModule weaponPlaceHolderModule:

                break;
            default:
                throw new ArgumentException(string.Format("Selected module {0} is unhandled by CraftingUI.InitialiseModuleStatDisplay", ModuleDropdown.SelectedModule));
        }

                //Generic Stats
                UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
        CostData[0] = new UIController.KVPData<float>("Iron", CurrentModule.Cost.Iron, null, Group: ModuleStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Iron);
        CostData[1] = new UIController.KVPData<float>("Copper", CurrentModule.Cost.Copper, null, Group: ModuleStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Copper);
        CostData[2] = new UIController.KVPData<float>("Alloy", CurrentModule.Cost.Alloy, null, Group: ModuleStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Alloy);

        ModuleStatKVPInfo.AddKVP(UIController.InstantiateKVP("Size Modifier", CurrentModule.SizeMultiplier, ModuleStatsPanel.transform, 2, ValueDelegate: KeyValuePanel.ModuleGetValue.SizeMod));
        ModuleStatKVPInfo.AddKVP(UIController.InstantiateKVP("Mass Modifier", CurrentModule.MassMultiplier, ModuleStatsPanel.transform, 2, ValueDelegate: KeyValuePanel.ModuleGetValue.MassMod));
        ModuleStatKVPInfo.AddKVPList(UIController.InstantiateKVPList("Cost", CostData, ModuleStatsPanel.transform));
    }

    void InitialiseTotalStatDisplay()
    {
        ClearKVPs(false, true);

        //General Stats
        UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
        CostData[0] = new UIController.KVPData<float>("Iron", CurrentItem.Stats.Cost.Iron, null, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Iron);
        CostData[1] = new UIController.KVPData<float>("Copper", CurrentItem.Stats.Cost.Copper, null, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Copper);
        CostData[2] = new UIController.KVPData<float>("Alloy", CurrentItem.Stats.Cost.Alloy, null, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Alloy);

        TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP("Item Type", CurrentItem.Type, TotalStatsPanel.transform));

        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Armour))         TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Armour),         CurrentItem.Stats.Armour,           TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Armour));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Shield))         TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Shield),         CurrentItem.Stats.Shield,           TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Shield));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Power))          TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Power),          CurrentItem.Stats.Power,            TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Power));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.PowerUse))       TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.PowerUse),       CurrentItem.Stats.PowerUse,         TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.PowerUse));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Damage))         TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Damage),         CurrentItem.Stats.Damage,           TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Damage));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.ArmourPiercing)) TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.ArmourPiercing), CurrentItem.Stats.ArmourPiercing,   TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.ArmourPiercing));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.AttackSpeed))    TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.AttackSpeed),    CurrentItem.Stats.AttackSpeed,      TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.AttackSpeed));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Range))          TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Range),          CurrentItem.Stats.Range,            TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Range));


        TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP("Size", CurrentItem.Stats.Size, TotalStatsPanel.transform, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Size));
        TotalStatKVPInfo.AddKVP(UIController.InstantiateKVP("Mass", CurrentItem.Stats.Mass, TotalStatsPanel.transform, 1, Group: TotalStatKVPInfo.Group, ValueDelegate: KeyValuePanel.ItemGetValue.Mass));
        TotalStatKVPInfo.AddKVPList(UIController.InstantiateKVPList("Cost", CostData, TotalStatsPanel.transform));
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
