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
        Toggle ActiveToggle = ActiveToggleEnum.Current;


    }

    void InitialiseStatDisplays()
    {
        TotalStatKVPs.Clear();
        ModuleStatKVPs.Clear();

        //General Stats
        UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[3];
        CostData[0] = new UIController.KVPData<float>("Iron", CurrentItem.Stats.Cost.Iron, null);
        CostData[1] = new UIController.KVPData<float>("Copper", CurrentItem.Stats.Cost.Copper, null);
        CostData[2] = new UIController.KVPData<float>("Alloy", CurrentItem.Stats.Cost.Alloy, null);

        TotalStatKVPs.Add(UIController.InstantiateKVP("Item Type", CurrentItem.Type, TotalStatsPanel.transform));
        TotalStatKVPs.Add(UIController.InstantiateKVP("Size", CurrentItem.Stats.Size, TotalStatsPanel.transform));
        TotalStatKVPs.Add(UIController.InstantiateKVP("Mass", CurrentItem.Stats.Mass, TotalStatsPanel.transform));
        TotalStatKVPs.Add(UIController.InstantiateKVPList("Cost", CostData, TotalStatsPanel.transform));
    }
}
