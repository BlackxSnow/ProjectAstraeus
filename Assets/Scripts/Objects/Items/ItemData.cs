using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class containing serializable data for items
[System.Serializable]
public class ItemData : ScriptableObject
{
    [System.Serializable]
    public struct ItemStats
    {
        //Weapon Stats
        public float Damage;
        public float AttackSpeed; //Attacks per second
        public float ArmourPiercing;
        public float Range;

        //Equipment Stats
        public float Armour;
        public float Power;
        public float PowerUse;
        public float Shield;

        //General Stats
        public Vector2Int Size;
        public float Mass;
        public Resources Cost;

        public ItemStats(Vector2Int Size, Resources Cost, float Damage = 0, float AttackSpeed = 0, float ArmourPiercing = 0, float Range = 0, float Armour = 0, float Power = 0, float PowerUse = 0, float Shield = 0, float Mass = 0)
        {
            this.Damage = Damage;
            this.AttackSpeed = AttackSpeed;
            this.ArmourPiercing = ArmourPiercing;
            this.Range = Range;
            this.Armour = Armour;
            this.Power = Power;
            this.PowerUse = PowerUse;
            this.Shield = Shield;
            this.Size = Size;
            this.Mass = Mass;
            this.Cost = Cost;
        }
    }

    public string ItemName;
    public ItemStats Stats;

    //Modular Parts
    public ItemModule.CoreModule Core;
    public List<ItemModule.AdditionalModule> Modules = new List<ItemModule.AdditionalModule>();

    //Item 'definitions'
    public ItemTypes.Types Type;
    
    public ItemData(ItemTypes.Types _Type)
    {
        this.Type = _Type;
        ItemTypes.TypeCores.TryGetValue(_Type, out Core);
        SetStats();
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

    public List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> KVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        KVPLists = new List<GameObject>();

        KVPs.Add(UIController.InstantiateKVP("Item Type", Type, Parent));

        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.Armour)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Armour), Stats.Armour, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Armour, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.Shield)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Shield), Stats.Shield, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Shield, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.Power)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Power), Stats.Power, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Power, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.PowerUse)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.PowerUse), Stats.PowerUse, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.PowerUse, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.Damage)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Damage), Stats.Damage, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Damage, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.ArmourPiercing)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.ArmourPiercing), Stats.ArmourPiercing, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.ArmourPiercing, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.AttackSpeed)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.AttackSpeed), Stats.AttackSpeed, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.AttackSpeed, RefItem: this));
        if (Core.StatFlags.HasFlag(ItemTypes.StatFlags.Range)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", ItemTypes.StatFlags.Range), Stats.Range, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Range, RefItem: this));

        KVPs.Add(UIController.InstantiateKVP("Size", Stats.Size, Parent, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Size, RefItem: this));
        KVPs.Add(UIController.InstantiateKVP("Mass", Stats.Mass, Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Mass, RefItem: this));
        if (Cost)
        {
            UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData<float>("Iron", Stats.Cost.Iron, null, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Iron, RefItem: this);
            CostData[1] = new UIController.KVPData<float>("Copper", Stats.Cost.Copper, null, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Copper, RefItem: this);
            CostData[2] = new UIController.KVPData<float>("Alloy", Stats.Cost.Alloy, null, Group: Group, ValueDelegate: KeyValuePanel.ItemGetValue.Cost_Alloy, RefItem: this);
            KVPLists.Add(UIController.InstantiateKVPList("Cost", CostData, Parent));
        }

        return KVPs;
    }

}
