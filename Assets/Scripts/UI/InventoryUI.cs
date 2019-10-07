using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUI : Window
{
    Inventory TargetInventory;
    public Inventory OpenInventory;

    public GameObject InventoryGrid;
    public int GridSize = 25;

    GridLayoutGroup LayoutComponent;
    GameObject GridPanel;
    RectTransform GridRTransform;

    GameObject ItemPanel;
    Text InventoryName;

    public GameObject ItemIcon;

    List<GameObject> ActiveGrids = new List<GameObject>();
    List<GameObject> ActiveItems = new List<GameObject>();

    List<Item> RenderedItems = new List<Item>();

    // Start is called before the first frame update
    void Start()
    {
        GridPanel = transform.GetChild(0).gameObject;
        LayoutComponent = GridPanel.GetComponent<GridLayoutGroup>();
        GridRTransform = this.gameObject.GetComponent<RectTransform>();

        ItemPanel = transform.GetChild(1).gameObject;
        InventoryName = transform.GetChild(2).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        BuildAndRenderCheck();
    }

    private void BuildAndRenderCheck()
    {
        if (Selection.SelectedObjs.Count == 1)
        {
            Inventory _Inventory = (Selection.SelectedObjs[0] as MonoBehaviour).gameObject.GetComponent<Inventory>();
            if (_Inventory)
            {
                TargetInventory = _Inventory;
                if (OpenInventory != TargetInventory)
                {
                    BuildGrid();
                    InventoryName.text = TargetInventory.gameObject.name;
                }
            }
        }
        if (OpenInventory && (OpenInventory.InventoryList.Except(RenderedItems).Any() || RenderedItems.Except(OpenInventory.InventoryList).Any()))
            RenderItems();
    }

    //Build the inventory grid to size specification
    private void BuildGrid()
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
            OpenInventory = TargetInventory;
        }
    }

    //Render contained items above the inventory grid
    public void RenderItems()
    {
        foreach (GameObject item in ActiveItems)
            Destroy(item);
        ActiveItems.Clear();
        RenderedItems.Clear();

        for (int y = 0; y < TargetInventory.InventorySize.y; y++)
        {
            for (int x = 0; x < TargetInventory.InventorySize.x; x++)
            {
                Item TargetItem = TargetInventory.InventoryArray[x, y];
                if (TargetItem && !RenderedItems.Contains(TargetItem))
                {
                    RenderedItems.Add(TargetItem);
                    GameObject _ = Instantiate(ItemIcon, ItemPanel.transform);
                    ActiveItems.Add(_);

                    ItemIcon _II = _.GetComponent<ItemIcon>();
                    _II.UIContainer = this;
                    _II.Container = TargetInventory;
                    _II.RefItem = TargetItem;

                    RectTransform _RT = _.GetComponent<RectTransform>();
                    _RT.anchoredPosition = new Vector3(x * GridSize, -y * GridSize, 0);
                    _RT.sizeDelta = TargetItem.Data.Stats.Size * GridSize;

                }
            }
        }
        //Debug.Log(string.Format("Finished Rendering... Active: {0}, Rendered: {1}, Target: {2}", ActiveItems.Count, RenderedItems.Count, TargetInventory.InventoryList.Count));
    }
}
