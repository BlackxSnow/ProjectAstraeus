﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        None
    }
    public EquippableItem[] Equipped = new EquippableItem[6];

    public bool EquipItem(EquippableItem RefItem, EquipmentSlot UISlot)
    {
        Slots Slot = UISlot.Slot;
        if (RefItem.Slot != Slot || Slot == Slots.None)
        {
            throw new ArgumentException(string.Format("Item's slot and parameter slot differ or slot is none (item slot: '{0}', slot parameter: '{1}'", RefItem.Slot, Slot));
        }
        if (Equipped[(int)Slot] == RefItem)
        {
            UISlot.RenderItem();
            return false;
        }
        if (Equipped[(int)Slot] != null)
        {
            Debug.Log(string.Format("Slot is already occupied by '{0}'", Equipped[(int)Slot]));
            return false;
        }
        VectorResults SizeComparison = CompareVectorSizes(UISlot.MaxSize, RefItem.Stats.GetStat<Vector2Int>(ItemTypes.StatsEnum.Size));
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
         Utility.RearrangeVector(Max, out Vector2 MaxRearranged);
         VectorResults ComparedResult = Utility.RearrangeVector(Compared, out Vector2 ComparedRearranged);
        
        if(ComparedRearranged.x > MaxRearranged.x || ComparedRearranged.y > MaxRearranged.y)
        {
            return VectorResults.None;
        }
        return ComparedResult;
    }
}
