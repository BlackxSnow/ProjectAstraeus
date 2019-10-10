using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    [Header("Display UI Elements")]
    public GameObject ModuleStatsPanel;
    public GameObject TotalStatsPanel;
    public GameObject ModuleListContent;
    [Space(10)]
    [Header("Interactive UI Elements")]
    public GameObject ItemListPanel;
    private ToggleGroup ItemToggles;
    [Space(10)]

    [HideInInspector]
    public ItemData CurrentItem;

    private List<GameObject> TotalStatKVPs = new List<GameObject>();
    private List<GameObject> ModuleStatKVPs = new List<GameObject>();

    private void Awake()
    {
        ItemToggles = ItemListPanel.GetComponent<ToggleGroup>();
    }

    public void NewItem()
    {
        IEnumerator<Toggle> ActiveToggleEnum = ItemToggles.ActiveToggles().GetEnumerator();
        ActiveToggleEnum.MoveNext();
        Toggle ActiveToggle = ActiveToggleEnum.Current;

        ItemTypes.Types Type = ActiveToggle.GetComponent<ItemSelectToggle>().Type;

        ClearKVPs(true, true);
        CurrentItem = new ItemData(Type);
        InitialiseStatDisplays();
    }

    public void ClearKVPs(bool Module, bool Total)
    {
        if(Module)
        {
            foreach(GameObject Panel in ModuleStatKVPs) Destroy(Panel);
            ModuleStatKVPs.Clear();
        }

        if(Total)
        {
            foreach (GameObject Panel in TotalStatKVPs) Destroy(Panel);
            TotalStatKVPs.Clear();
        }
    }

    void InitialiseStatDisplays()
    {
        ClearKVPs(true, true);
        Debug.Log(string.Format("{0}", ItemTypes.StatFlags.Armour));

        //General Stats
        UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
        CostData[0] = new UIController.KVPData<float>("Iron", CurrentItem.Stats.Cost.Iron, null);
        CostData[1] = new UIController.KVPData<float>("Copper", CurrentItem.Stats.Cost.Copper, null);
        CostData[2] = new UIController.KVPData<float>("Alloy", CurrentItem.Stats.Cost.Alloy, null);

        TotalStatKVPs.Add(UIController.InstantiateKVP("Item Type", CurrentItem.Type, TotalStatsPanel.transform));

        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Armour))         TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Armour),         CurrentItem.Stats.Armour,           TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Shield))         TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Shield),         CurrentItem.Stats.Shield,           TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Power))          TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Power),          CurrentItem.Stats.Power,            TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.PowerUse))       TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.PowerUse),       CurrentItem.Stats.PowerUse,         TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Damage))         TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Damage),         CurrentItem.Stats.Damage,           TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.ArmourPiercing)) TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.ArmourPiercing), CurrentItem.Stats.ArmourPiercing,   TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.AttackSpeed))    TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.AttackSpeed),    CurrentItem.Stats.AttackSpeed,      TotalStatsPanel.transform));
        if (CurrentItem.Core.StatFlags.HasFlag(ItemTypes.StatFlags.Range))          TotalStatKVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Range),          CurrentItem.Stats.Range,            TotalStatsPanel.transform));


        TotalStatKVPs.Add(UIController.InstantiateKVP("Size", CurrentItem.Stats.Size, TotalStatsPanel.transform));
        TotalStatKVPs.Add(UIController.InstantiateKVP("Mass", CurrentItem.Stats.Mass, TotalStatsPanel.transform));
        TotalStatKVPs.Add(UIController.InstantiateKVPList("Cost", CostData, TotalStatsPanel.transform));
    }
}
