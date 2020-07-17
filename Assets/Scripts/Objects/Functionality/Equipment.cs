using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Items;

public class Equipment : MonoBehaviour
{
    public enum VectorResults
    {
        x,
        y,
        None
    }
    public enum Slots
    {
        Head,
        Torso,
        Arms,
        Legs,
        Feet,
        Back,
        Weapon,
        SecondaryWeapon,
        None
    }
    public EquippableItem[] Equipped;

    private void Awake()
    {
        Equipped = new EquippableItem[8];
    }

    public bool EquipItem(EquippableItem RefItem, EquipmentSlot UISlot)
    {
        Slots Slot = UISlot.Slot;
        if (!RefItem.ValidSlots.Contains(Slot) || Slot == Slots.None)
        {
            throw new ArgumentException(string.Format("Item's slot and parameter slot differ or slot is none (item slot: '{0}', slot parameter: '{1}'", RefItem.ValidSlots[0], Slot));
        }
        if (Equipped[(int)Slot] != null && Equipped[(int)Slot] == RefItem)
        {
            UISlot.RenderItem();
            return false;
        }
        if (Equipped[(int)Slot] != null)
        {
            Debug.Log(string.Format("Slot is already occupied by '{0}'", Equipped[(int)Slot]));
            return false;
        }
        VectorResults SizeComparison = CompareVectorSizes(UISlot.MaxSize, RefItem.Stats.Size);
        if (SizeComparison == VectorResults.None)
        {
            Debug.Log(string.Format("Item does not fit"));
            return false;
        }

        Equipped[(int)Slot] = RefItem;
        UISlot.RenderItem(SizeComparison);
        return true;
    }

    public void UnequipItem(EquippableItem RefItem, EquipmentSlot UISlot)
    {
        if (Equipped[(int)UISlot.Slot] != RefItem)
        {
            throw new ArgumentException(string.Format("Item '{0}' does not exist in slot '{1}'", RefItem, UISlot.Slot));
        }
        Equipped[(int)UISlot.Slot] = null;
    }

    static VectorResults CompareVectorSizes(Vector2 Max, Vector2 Compared)
    {
         Utility.Vector.RearrangeVector(Max, out Vector2 MaxRearranged);
         VectorResults ComparedResult = Utility.Vector.RearrangeVector(Compared, out Vector2 ComparedRearranged);
        
        if(ComparedRearranged.x > MaxRearranged.x || ComparedRearranged.y > MaxRearranged.y)
        {
            return VectorResults.None;
        }
        return ComparedResult;
    }
}
