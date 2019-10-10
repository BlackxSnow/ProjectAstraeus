using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTypes : MonoBehaviour
{
    public enum Types
    {
        Resource,
        Head,
        Torso,
        Legs,
        Feet,
        Arms,
        Back,
        Accessory,
        MeleeSharp,
        MeleeBlunt,
        RangedBow,
        RangedFirearm
    }

    [Flags]
    public enum StatFlags
    {
        Damage = 1 << 0,
        AttackSpeed = 1 << 1,
        ArmourPiercing = 1 << 2,
        Range = 1 << 3,
        Armour = 1 << 4,
        Power = 1 << 5,
        PowerUse = 1 << 6,
        Shield = 1 << 7,
        Size = 1 << 8,
        Mass = 1 << 9,
        Cost = 1 << 10
    }

    [Flags]
    public enum ItemFlags
    {
        Armour = 1 << 0,
        Weapon = 1 << 1,
        Melee = 1 << 2,
        Ranged = 1 << 3,
        Consumable = 1 << 4
    }

    public static Dictionary<ItemFlags, StatFlags> FlagStats = new Dictionary<ItemFlags, StatFlags>()
    {
        { ItemFlags.Armour,     StatFlags.Size | StatFlags.Cost | StatFlags.Mass | StatFlags.Armour         | StatFlags.Power       | StatFlags.Shield | StatFlags.PowerUse                   },
        { ItemFlags.Consumable, StatFlags.Size | StatFlags.Cost | StatFlags.Mass                                                                                                              },
        { ItemFlags.Melee,      StatFlags.Size | StatFlags.Cost | StatFlags.Mass | StatFlags.ArmourPiercing | StatFlags.AttackSpeed | StatFlags.Damage | StatFlags.PowerUse                   },
        { ItemFlags.Ranged,     StatFlags.Size | StatFlags.Cost | StatFlags.Mass | StatFlags.ArmourPiercing | StatFlags.AttackSpeed | StatFlags.Damage | StatFlags.PowerUse | StatFlags.Range }
    };

    public static Dictionary<Types, ItemModule.CoreModule> TypeCores = new Dictionary<Types, ItemModule.CoreModule>()
    {
        { Types.Head,          new ItemModule.CoreModule(   new Vector2Int(3, 5),   0.5f,   new Resources(1,0,0),   ItemFlags.Armour                       ) },
        { Types.Torso,         new ItemModule.CoreModule(   new Vector2Int(7,10),   1.0f,   new Resources(2,0,0),   ItemFlags.Armour                       ) },
        { Types.Legs,          new ItemModule.CoreModule(   new Vector2Int(6,15),   1.0f,   new Resources(2,0,0),   ItemFlags.Armour                       ) },
        { Types.Feet,          new ItemModule.CoreModule(   new Vector2Int(3, 5),   0.5f,   new Resources(1,0,0),   ItemFlags.Armour                       ) },
        { Types.Arms,          new ItemModule.CoreModule(   new Vector2Int(2, 5),   0.5f,   new Resources(1,0,0),   ItemFlags.Armour                       ) },
        { Types.Back,          new ItemModule.CoreModule(   new Vector2Int(4,10),   1.0f,   new Resources(2,0,0),   ItemFlags.Armour                       ) },
        { Types.Accessory,     new ItemModule.CoreModule(   new Vector2Int(1, 1),   0.1f,   new Resources(1,0,0),   ItemFlags.Armour                       ) },
        { Types.MeleeSharp,    new ItemModule.CoreModule(   new Vector2Int(4, 2),   0.5f,   new Resources(1,0,0),   ItemFlags.Melee  | ItemFlags.Weapon    ) },
        { Types.MeleeBlunt,    new ItemModule.CoreModule(   new Vector2Int(4, 2),   1.0f,   new Resources(1,0,0),   ItemFlags.Melee  | ItemFlags.Weapon    ) },
        { Types.RangedBow,     new ItemModule.CoreModule(   new Vector2Int(4, 5),   0.5f,   new Resources(1,0,0),   ItemFlags.Ranged | ItemFlags.Weapon    ) },
        { Types.RangedFirearm, new ItemModule.CoreModule(   new Vector2Int(5, 4),   1.0f,   new Resources(1,0,0),   ItemFlags.Ranged | ItemFlags.Weapon    ) }
    };
}
