using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Entity Owner;
    public List<Item> InventoryList = new List<Item>();
    public Item[,] InventoryArray;
    public Vector2Int InventorySize;

    private void Start()
    {
        Owner = GetComponent<Entity>();
        InventoryArray = new Item[(int)InventorySize.x, (int)InventorySize.y];
    }

    public bool AddItem(Item item, Vector2Int location)
    {
        if (!ItemCheck(item, location))
            return false;

        for (int y = 0; y < item.Data.Stats.Size.y; y++)
        {
            for (int x = 0; x < item.Data.Stats.Size.x; x++)
            {
                InventoryArray[x + (int)location.x, y + (int)location.y] = item;
            }
        }
        InventoryList.Add(item);
        item.SetFollow(this.gameObject);
        item.Pack();
        return true;
    }
    public bool AddItem(Item item) //Add to first available space instead of specified location
    {
        for (int y = 0; y < InventorySize.y; y++)
        {
            for (int x = 0; x < InventorySize.x; x++)
            {
                if (ItemCheck(item, new Vector2Int(x, y)))
                    return AddItem(item, new Vector2Int(x, y));
            }
        }
        return false;
    }

    public bool RemoveItem(Item item, bool Drop)
    {
        if (!InventoryList.Contains(item))
            return false;

        bool Nulled = false;
        for (int y = 0; y < InventorySize.y; y++)
        {
            for (int x = 0; x < InventorySize.x; x++)
            {
                if (InventoryArray[x, y] == item)
                {
                    InventoryArray[x, y] = null;
                    Nulled = true;
                }
            }
        }
        if (!Nulled)
            return false;

        InventoryList.Remove(item);

        if (Drop)
        {
            item.SetFollow(null);
            item.Unpack();
        }

        return true;
    }

    public bool ItemCheck(Item item, Vector2Int location)
    {
        if (item.Data.Stats.Size.x + location.x > InventorySize.x || item.Data.Stats.Size.y + location.y > InventorySize.y || location.x < 0 || location.y < 0)
        {
            Debug.Log(string.Format("Item Size too large for Inventory or invalid location... Item Size: {0}, location: {1}, Inventory Size: {2}", item.Data.Stats.Size, location, InventorySize));
            return false;
        }

        for (int y = 0; y < item.Data.Stats.Size.y; y++)
        {
            for (int x = 0; x < item.Data.Stats.Size.x; x++)
            {
                if (InventoryArray[x + (int)location.x, y + (int)location.y])
                {
                    if (InventoryArray[x + (int)location.x, y + (int)location.y] == item) continue;
                    Debug.Log(string.Format("Grid at [{0},{1}] returned false. Contains {2}",x+location.x, y+location.y, InventoryArray[x + (int)location.x, y + (int)location.y].name));
                    return false;
                }
            }
        }
        return true;
    }
}
