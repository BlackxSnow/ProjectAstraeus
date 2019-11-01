using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;

//Class containing serializable data for items
[System.Serializable]
public class ItemData : ScriptableObject
{
    public class ItemStats
    {
        private Dictionary<StatFlags, object> Stats = new Dictionary<StatFlags, object>()
        {
            { StatFlags.Damage, 0f },
            { StatFlags.AttackSpeed, 0f },
            { StatFlags.Accuracy, 0f },
            { StatFlags.Block, 0f },
            { StatFlags.ArmourPiercing, 0f },
            { StatFlags.Range, 0f },
            { StatFlags.Armour, 0f },
            { StatFlags.Power, 0f },
            { StatFlags.PowerUse, 0f },
            { StatFlags.Shield, 0f },
            { StatFlags.Size, new Vector2Int(1,1) },
            { StatFlags.Mass, 0f },
            { StatFlags.Cost, new Resources(0,0,0) }
        };

        public T GetStat<T>(StatFlags Stat) where T : struct
        {
            if(!Stats[Stat].GetType().IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} was requested, member type was {1}", typeof(T), Stats[Stat].GetType()));
            }
            return (T)Stats[Stat];
        }

        public void SetStat<T>(StatFlags Stat, T Value)
        {
            if(Value.GetType() != Stats[Stat].GetType())
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }
            Stats[Stat] = Value;
        }

        public void AddStat(StatFlags Stat, float Value)
        {
            if (Value.GetType() != Stats[Stat].GetType())
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }
            if (Stats[Stat] is float stat)
            {
                Stats[Stat] = stat + Value;
            }
        }
        public void SubtractStat(StatFlags Stat, float Value)
        {
            if (Value.GetType() != Stats[Stat].GetType())
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }
            if (Stats[Stat] is float stat)
            {
                Stats[Stat] = stat - Value;
            }
        }
    }

    

    public string ItemName;
    public ItemStats Stats;

    //Modular Parts
    public ItemModule.CoreModule Core;
    public List<ItemModule.AdditionalModule> Modules = new List<ItemModule.AdditionalModule>();

    //Item 'definitions'
    public ItemTypes.Types Type;
    public SubTypes SubType;
    
    public ItemData(ItemTypes.Types _Type)
    {
        this.Type = _Type;
        TypeCores.TryGetValue(_Type, out Core);
        SetStats();
    }

    public void SetStats()
    {
        Vector2 SizeMod = new Vector2(1, 1);
        float MassMod = 1;
        Resources ResourceMod = new Resources(0, 0, 0);
        Stats = new ItemStats(/*new Vector2Int(1, 1), new Resources(0, 0, 0)*/);

        foreach (ItemModule.AdditionalModule Module in Modules)
        {
            SizeMod += Module.SizeMultiplier;
            MassMod += Module.MassMultiplier;
            ResourceMod += Module.Cost;
            
            //Refactor by changing module stat storage to be a Dict<StatFlags, object> where object is the value. Iterate over the dict and add to the appropriate ItemStats.Stats[]
            switch (Module)
            {
                case ItemModule.AdditionalModule.Plating _plating:
                    Stats.AddStat(StatFlags.Armour, _plating.Armour);
                    break;
                case ItemModule.AdditionalModule.Reactor _Reactor:
                    Stats.AddStat(StatFlags.Power, _Reactor.Power);
                    break;
                case ItemModule.AdditionalModule.Shielding _Shielding:
                    Stats.AddStat(StatFlags.Shield, _Shielding.Shield);
                    break;
                default:
                    break;
            }
        }
        Stats.SetStat(StatFlags.Size, new Vector2Int(Mathf.RoundToInt(Core.Size.x * SizeMod.x), Mathf.RoundToInt(Core.Size.y * SizeMod.y)));
        Stats.SetStat(StatFlags.Mass, Core.Mass * MassMod);
        Stats.SetStat(StatFlags.Cost, Core.Cost + ResourceMod);
    }

    public List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> KVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        KVPLists = new List<GameObject>();

        KVPs.Add(UIController.InstantiateKVP("Item Type", Type, Parent, Group: Group));

        if (Core.StatFlags.HasFlag(StatFlags.Armour))         KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.Armour), Stats.GetStat<float>(StatFlags.Armour), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Armour, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.Shield))         KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.Shield), Stats.GetStat<float>(StatFlags.Shield), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Shield, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.Power))          KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.Power), Stats.GetStat<float>(StatFlags.Power), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Power, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.PowerUse))       KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.PowerUse), Stats.GetStat<float>(StatFlags.PowerUse), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.PowerUse, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.Damage))         KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.Damage), Stats.GetStat<float>(StatFlags.Damage), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Damage, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.ArmourPiercing)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.ArmourPiercing), Stats.GetStat<float>(StatFlags.ArmourPiercing), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.ArmourPiercing, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.AttackSpeed))    KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.AttackSpeed), Stats.GetStat<float>(StatFlags.AttackSpeed), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.AttackSpeed, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlags.Range))          KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlags.Range), Stats.GetStat<float>(StatFlags.Range), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Range, RefItem: this));

        KVPs.Add(UIController.InstantiateKVP("Size", Stats.GetStat<Vector2Int>(StatFlags.Size), Parent, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Size, RefItem: this));
        KVPs.Add(UIController.InstantiateKVP("Mass", Stats.GetStat<float>(StatFlags.Mass), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlags.Mass, RefItem: this));
        if (Cost)
        {
            UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData<float>("Iron", Stats.GetStat<Resources>(StatFlags.Cost)[Resources.ResourceList.Iron], null, Group: Group, ValueDelegate: KeyValuePanel.GetItemCost, ValueEnum: Resources.ResourceList.Iron, RefItem: this);
            CostData[1] = new UIController.KVPData<float>("Copper", Stats.GetStat<Resources>(StatFlags.Cost)[Resources.ResourceList.Copper], null, Group: Group, ValueDelegate: KeyValuePanel.GetItemCost, ValueEnum: Resources.ResourceList.Copper, RefItem: this);
            CostData[2] = new UIController.KVPData<float>("Alloy", Stats.GetStat<Resources>(StatFlags.Cost)[Resources.ResourceList.Alloy], null, Group: Group, ValueDelegate: KeyValuePanel.GetItemCost, ValueEnum: Resources.ResourceList.Alloy, RefItem: this);
            KVPLists.Add(UIController.InstantiateKVPList("Cost", CostData, Parent, Group));
        }

        return KVPs;
    }

}
