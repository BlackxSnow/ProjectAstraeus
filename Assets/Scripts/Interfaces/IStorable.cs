using Items;
using UnityEngine;

namespace Interfaces
{
    public interface IStorable
    {
        string name { get; set; }
        Inventory StoredIn { get; set; }
        Item.ItemStats Stats { get; set; }
        void AddToInventory(Inventory target);
        void RemoveFromInventory();
        void SetFollow(GameObject target);
    }
}
