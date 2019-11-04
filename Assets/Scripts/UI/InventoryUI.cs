using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : Window
{
    public Inventory CurrentInventory;

    public GameObject InventoryGrid;
    public static int GridSize = 25;

    GridLayoutGroup LayoutComponent;
    public GameObject GridPanel;
    RectTransform GridRTransform;

    public GameObject ItemPanel;
    public TextMeshProUGUI InventoryName;

    public GameObject ItemIcon;

    List<GameObject> ActiveGrids = new List<GameObject>();
    List<GameObject> ActiveItems = new List<GameObject>();

    List<Item> RenderedItems = new List<Item>();

    //Manually called Initiatisation
    bool Initialised = false;
    void Init()
    {
        Initialised = true;

        LayoutComponent = GridPanel.GetComponent<GridLayoutGroup>();
        GridRTransform = this.gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        RenderCheck();
    }

    public void OpenInventory(Inventory TargetInventory)
    {
        if (!Initialised) Init();
        BuildGrid(TargetInventory);
        InventoryName.text = TargetInventory.Owner.Name;
    }

    private void RenderCheck()
    {
        //Check if either list is out of sync
        if (CurrentInventory && (CurrentInventory.InventoryList.Except(RenderedItems).Any() || RenderedItems.Except(CurrentInventory.InventoryList).Any()))
            RenderItems();
    }

    //Build the inventory grid to size specification
    private void BuildGrid(Inventory TargetInventory)
    {
        if (TargetInventory)
        {
            foreach (GameObject grid in ActiveGrids)
                Destroy(grid);
            ActiveGrids.Clear();

            LayoutComponent.constraintCount = TargetInventory.InventorySize.x;
            for (int y = 0; y < TargetInventory.InventorySize.y; y++)
            {
                for (int x = 0; x < TargetInventory.InventorySize.x; x++)
                {
                    ActiveGrids.Add(Instantiate(InventoryGrid, GridPanel.transform));
                }
            }
            GridRTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, TargetInventory.InventorySize.x * GridSize + 50);
            GridRTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TargetInventory.InventorySize.y * GridSize + 70);
            CurrentInventory = TargetInventory;
        }
    }

    //Render contained items above the inventory grid
    public void RenderItems()
    {
        foreach (GameObject item in ActiveItems)
            Destroy(item);
        ActiveItems.Clear();
        RenderedItems.Clear();

        for (int y = 0; y < CurrentInventory.InventorySize.y; y++)
        {
            for (int x = 0; x < CurrentInventory.InventorySize.x; x++)
            {
                Item TargetItem = CurrentInventory.InventoryArray[x, y];
                if (TargetItem && !RenderedItems.Contains(TargetItem))
                {
                    RenderedItems.Add(TargetItem);
                    GameObject _ = Instantiate(ItemIcon, ItemPanel.transform);
                    ActiveItems.Add(_);

                    ItemIcon _II = _.GetComponent<ItemIcon>();
                    _II.InventoryInfo.UIContainer = this;
                    _II.InventoryInfo.Container = CurrentInventory;
                    _II.RefItem = TargetItem;

                    RectTransform _RT = _.GetComponent<RectTransform>();
                    _RT.anchoredPosition = new Vector3(x * GridSize, -y * GridSize, 0);
                    _RT.sizeDelta = TargetItem.Data.Stats.GetStat<Vector2Int>(ItemTypes.StatFlagsEnum.Size) * GridSize;

                }
            }
        }
    }
}
