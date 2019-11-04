using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static ItemModule.AdditionalModule;

//Class containing serializable data for items
[System.Serializable]
public class ItemData : ScriptableObject
{
    public class ItemStats
    {
        public Dictionary<StatFlagsEnum, object> Stats = new Dictionary<StatFlagsEnum, object>()
        {
            { StatFlagsEnum.Damage, 0f },
            { StatFlagsEnum.AttackSpeed, 0f },
            { StatFlagsEnum.Accuracy, 0f },
            { StatFlagsEnum.Block, 0f },
            { StatFlagsEnum.ArmourPiercing, 0f },
            { StatFlagsEnum.Range, 0f },
            { StatFlagsEnum.Armour, 0f },
            { StatFlagsEnum.Power, 0f },
            { StatFlagsEnum.PowerUse, 0f },
            { StatFlagsEnum.Shield, 0f },
            { StatFlagsEnum.Size, new Vector2Int(1,1) },
            { StatFlagsEnum.Mass, 0f },
            { StatFlagsEnum.Cost, new Resources(0,0,0) }
        };

        public object this[StatFlagsEnum Stat]
        {
            get
            {
                return Stats[Stat];
            }
            set
            {
                if (Stats[Stat].GetType() != value.GetType())
                {
                    throw new ArgumentException(string.Format("Type {0} was requested, member type was {1}", value.GetType(), Stats[Stat].GetType()));
                }
                Stats[Stat] = value;
            }
        }

        public T GetStat<T>(StatFlagsEnum Stat) where T : struct
        {
            if (!Stats[Stat].GetType().IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} was requested, member type was {1}", typeof(T), Stats[Stat].GetType()));
            }
            return (T)Stats[Stat];
        }

        public enum OperationEnum
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide
        }

        public void SetStat<T>(StatFlagsEnum Stat, T Value, OperationEnum Operation = OperationEnum.None)
        {
            if (Value.GetType() != Stats[Stat].GetType())
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }
            Stats[Stat] = Value;
        }

        public void AddStat(StatFlagsEnum Stat, float Value)
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
        public void SubtractStat(StatFlagsEnum Stat, float Value)
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
            SizeMod += Module.GetStat<Vector2>(StatFlagsEnum.SizeMod);
            MassMod += Module.GetStat<float>(StatFlagsEnum.MassMod);
            ResourceMod += Module.GetStat<Resources>(StatFlagsEnum.Cost);

            foreach (StatFlagsEnum StatFlag in Utility.GetFlags(Module.StatFlags))
            {
                if (!Stats.Stats.TryGetValue(StatFlag, out object Value) || StatFlag == StatFlagsEnum.Cost)
                {
                    continue;
                }

                Stats.SetStat(StatFlag, (float)Value, ItemStats.OperationEnum.Add);
            }
        }
        Stats.SetStat(StatFlagsEnum.Size, new Vector2Int(Mathf.RoundToInt(Core.Size.x * SizeMod.x), Mathf.RoundToInt(Core.Size.y * SizeMod.y)));
        Stats.SetStat(StatFlagsEnum.Mass, Core.Mass * MassMod); //Is this multiplying by too much with > 1 module?
        Stats.SetStat(StatFlagsEnum.Cost, Core.Cost + ResourceMod);
    }

    public List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> KVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        KVPLists = new List<GameObject>();

        KVPs.Add(UIController.InstantiateKVP("Item Type", Type, Parent, Group: Group));

        if (Core.StatFlags.HasFlag(StatFlagsEnum.Armour))         KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.Armour), Stats.GetStat<float>(StatFlagsEnum.Armour), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Armour, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.Shield))         KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.Shield), Stats.GetStat<float>(StatFlagsEnum.Shield), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Shield, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.Power))          KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.Power), Stats.GetStat<float>(StatFlagsEnum.Power), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Power, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.PowerUse))       KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.PowerUse), Stats.GetStat<float>(StatFlagsEnum.PowerUse), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.PowerUse, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.Damage))         KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.Damage), Stats.GetStat<float>(StatFlagsEnum.Damage), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Damage, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.ArmourPiercing)) KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.ArmourPiercing), Stats.GetStat<float>(StatFlagsEnum.ArmourPiercing), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.ArmourPiercing, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.AttackSpeed))    KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.AttackSpeed), Stats.GetStat<float>(StatFlagsEnum.AttackSpeed), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.AttackSpeed, RefItem: this));
        if (Core.StatFlags.HasFlag(StatFlagsEnum.Range))          KVPs.Add(UIController.InstantiateKVP(string.Format("{0}", StatFlagsEnum.Range), Stats.GetStat<float>(StatFlagsEnum.Range), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Range, RefItem: this));

        KVPs.Add(UIController.InstantiateKVP("Size", Stats.GetStat<Vector2Int>(StatFlagsEnum.Size), Parent, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Size, RefItem: this));
        KVPs.Add(UIController.InstantiateKVP("Mass", Stats.GetStat<float>(StatFlagsEnum.Mass), Parent, 1, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: StatFlagsEnum.Mass, RefItem: this));
        if (Cost)
        {
            UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData<float>("Iron", Stats.GetStat<Resources>(StatFlagsEnum.Cost)[Resources.ResourceList.Iron], null, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: Resources.ResourceList.Iron, RefItem: this);
            CostData[1] = new UIController.KVPData<float>("Copper", Stats.GetStat<Resources>(StatFlagsEnum.Cost)[Resources.ResourceList.Copper], null, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: Resources.ResourceList.Copper, RefItem: this);
            CostData[2] = new UIController.KVPData<float>("Alloy", Stats.GetStat<Resources>(StatFlagsEnum.Cost)[Resources.ResourceList.Alloy], null, Group: Group, ValueDelegate: KeyValuePanel.GetItemStat, ValueEnum: Resources.ResourceList.Alloy, RefItem: this);
            KVPLists.Add(UIController.InstantiateKVPList("Cost", CostData, Parent, Group));
        }

        return KVPs;
    }

}
