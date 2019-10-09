using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class containing serializable data for items
[System.Serializable]
public class ItemData : ScriptableObject
{
    public enum Types
    {
        Resource,
        Armour,
        MeleeSharp,
        MeleeBlunt,
        RangedBow,
        RangedFirearm
    }

    [Flags]
    public enum ItemFlags
    {
        Equipable = 1,
        Consumable = 2,
        Head = 4,
        Torso = 8,
        Legs = 16,
        Arms = 32,
        Back = 64,
        Accessory = 128,
        PrimaryWep = 256,
        SecondaryWep = 512,
    }

    [System.Serializable]
    public struct ItemStats
    {
        //Weapon Stats
        public float Damage;
        public float AttackSpeed; //Attacks per second
        public float ArmourPiercing;

        //Equipment Stats
        public float Armour;
        public float Power;
        public float Shield;

        //General Stats
        public Vector2Int Size;
        public float Mass;
        public Resources Cost;

        public ItemStats(Vector2Int Size, Resources Cost, float Damage = 0, float AttackSpeed = 0, float ArmourPiercing = 0, float Armour = 0, float Power = 0, float Shield = 0, float Mass = 0)
        {
            this.Damage = Damage;
            this.AttackSpeed = AttackSpeed;
            this.ArmourPiercing = ArmourPiercing;
            this.Armour = Armour;
            this.Power = Power;
            this.Shield = Shield;
            this.Size = Size;
            this.Mass = Mass;
            this.Cost = Cost;
        }
    }

    public ItemStats Stats;

    //Modular Parts
    public ItemModule.CoreModule Core;
    public List<ItemModule.AdditionalModule> Modules;

    //Item 'definitions'
    public Types Type;
    public ItemFlags Flags;
    
    public ItemData(ItemData data)
    {
        Stats = data.Stats;
        Core = data.Core;
        Modules = data.Modules;
        Type = data.Type;
        Flags = data.Flags;
    }
    public ItemData(ItemStats Stats, ItemModule.CoreModule Core, List<ItemModule.AdditionalModule> Modules, Types Type, ItemFlags Flags)
    {
        this.Stats = Stats;
        this.Core = Core;
        this.Modules = Modules;
        this.Type = Type;
        this.Flags = Flags;
    }

    public void SetStats()
    {
        Vector2 SizeMod = new Vector2(1, 1);
        float MassMod = 1;
        Resources ResourceMod = new Resources(0, 0, 0);
        Stats = new ItemStats(new Vector2Int(1, 1), new Resources(0, 0, 0));

        foreach (ItemModule.AdditionalModule Module in Modules)
        {
            SizeMod += Module.SizeMultiplier;
            MassMod += Module.MassMultiplier;
            ResourceMod += Module.Cost;

            switch (Module)
            {
                case ItemModule.AdditionalModule.Plating _plating:
                    Stats.Armour += _plating.Armour;
                    break;
                case ItemModule.AdditionalModule.Reactor _Reactor:
                    Stats.Power += _Reactor.Power;
                    break;
                case ItemModule.AdditionalModule.Shielding _Shielding:
                    Stats.Shield += _Shielding.Shield;
                    break;
                default:
                    break;
            }
        }
        Stats.Size = new Vector2Int(Mathf.RoundToInt(Core.Size.x * SizeMod.x), Mathf.RoundToInt(Core.Size.y * SizeMod.y));
        Stats.Mass = Core.Mass * MassMod;
        Stats.Cost = Core.Cost + ResourceMod;
    }
}
