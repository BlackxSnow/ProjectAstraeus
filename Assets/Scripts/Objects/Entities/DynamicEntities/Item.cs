using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using static ItemTypes;


public class Item : DynamicEntity
{
    public BaseItemStats BaseStats = new BaseItemStats()
    {
        Stats = new Dictionary<StatsEnum, object>()
        {
            { StatsEnum.Size, new Vector2Int(1, 1) },
            { StatsEnum.Mass, 1f },
            { StatsEnum.Cost, new Resources(1, 0, 0) }
        },
        CompatibleModules = new List<AdditionalModule.ModulesEnum>()
    };

    public class BaseItemStats : ItemStats
    {
        public List<AdditionalModule.ModulesEnum> CompatibleModules;
    }

    public class ItemStats
    {
        //Move these dictionary additions to relevant derived classes
        public Dictionary<StatsEnum, object> Stats = new Dictionary<StatsEnum, object>()
        {
            { StatsEnum.Size, new Vector2Int(1,1) },
            { StatsEnum.Mass, 0f },
            { StatsEnum.Cost, new Resources(0,0,0) }
        };

        public object this[StatsEnum Stat]
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

        public void AddStat(StatsEnum Stat, object Value)
        {
            Stats.Add(Stat, Value);
        }

        public T GetStat<T>(StatsEnum Stat) where T : struct
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

        public void SetStat<T>(StatsEnum Stat, T Value, OperationEnum Operation = OperationEnum.None)
        {
            if (Value.GetType() != Stats[Stat].GetType())
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }
            if (Operation != OperationEnum.None && !(Value is float))
            {
                throw new ArgumentException(string.Format("Numeric type was passing to SetStat() while Operation was not None"));
            }
            if (Operation != OperationEnum.None && Value is float FVal)
            {
                switch (Operation)
                {
                    case OperationEnum.Add:
                        Stats[Stat] = (float)Stats[Stat] + FVal;
                        return;
                    case OperationEnum.Subtract:
                        Stats[Stat] = (float)Stats[Stat] - FVal;
                        return;
                    case OperationEnum.Multiply:
                        Stats[Stat] = (float)Stats[Stat] * FVal;
                        return;
                    case OperationEnum.Divide:
                        Stats[Stat] = (float)Stats[Stat] / FVal;
                        return;
            }
        }
            Stats[Stat] = Value;
        }
    }

    public void SetStats()
    {
        Vector2 SizeMod = new Vector2(1, 1);
        float MassMod = 1;
        Resources ResourceMod = new Resources(0, 0, 0);
        Stats = new ItemStats(/*new Vector2Int(1, 1), new Resources(0, 0, 0)*/);

        foreach (AdditionalModule Module in Modules)
        {
            SizeMod += Module.GetStat<Vector2>(StatsEnum.SizeMod);
            MassMod += Module.GetStat<float>(StatsEnum.MassMod);
            ResourceMod += Module.GetStat<Resources>(StatsEnum.Cost);

            foreach (KeyValuePair<StatsEnum, object> Stat in Module.Stats)
            {
                if (!Stats.Stats.TryGetValue(Stat.Key, out object Value) || Stat.Key == StatsEnum.Cost)
                {
                    continue;
                }

                Stats.SetStat(Stat.Key, (float)Value, ItemStats.OperationEnum.Add);
            }
        }
        Stats.SetStat(StatsEnum.Size, new Vector2Int(Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).x * SizeMod.x), Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).y * SizeMod.y)));
        Stats.SetStat(StatsEnum.Mass, BaseStats.GetStat<float>(StatsEnum.Mass) * MassMod); //Is this multiplying by too much with > 1 module?
        Stats.SetStat(StatsEnum.Cost, BaseStats.GetStat<Resources>(StatsEnum.Cost) + ResourceMod);
    }


    public virtual List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> KVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        KVPLists = new List<GameObject>();
        List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>();

        UIController.KVPData TypeData = new UIController.KVPData("Item Type", Type, Parent)
        {
            Group = Group
        };

        KVPDatas.Add(new UIController.KVPData("Size", Stats.GetStat<Vector2Int>(StatsEnum.Size), Parent));
        KVPDatas.Add(new UIController.KVPData("Mass", Stats.GetStat<float>(StatsEnum.Mass), Parent));

        KVPDatas[0].ValueEnum = StatsEnum.Size;
        KVPDatas[1].ValueEnum = StatsEnum.Mass;

        foreach (UIController.KVPData Data in KVPDatas)
        {
            Data.RefItem = this;
            Data.ValueDelegate = KeyValuePanel.GetItemStat;
            Data.Group = Group;
            KVPs.Add(UIController.InstantiateKVP(Data));
        }
        KVPs.Add(UIController.InstantiateKVP(TypeData));


        if (Cost)
        {
            UIController.KVPData[] CostData = new UIController.KVPData[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData("Iron", Stats.GetStat<Resources>(StatsEnum.Cost)[Resources.ResourceList.Iron], null);
            CostData[1] = new UIController.KVPData("Copper", Stats.GetStat<Resources>(StatsEnum.Cost)[Resources.ResourceList.Copper], null);
            CostData[2] = new UIController.KVPData("Alloy", Stats.GetStat<Resources>(StatsEnum.Cost)[Resources.ResourceList.Alloy], null);

            CostData[0].ValueEnum = Resources.ResourceList.Iron;
            CostData[1].ValueEnum = Resources.ResourceList.Copper;
            CostData[2].ValueEnum = Resources.ResourceList.Alloy;

            foreach (UIController.KVPData Data in CostData)
            {
                Data.RefItem = this;
                Data.ValueDelegate = KeyValuePanel.GetItemStat;
                Data.Group = Group;
            }
            KVPLists.Add(UIController.InstantiateKVPList("Cost", CostData, Parent, Group));
        }

        return KVPs;
    }

    public string ItemName;
    public ItemStats Stats;

    //Modular Parts
    public List<AdditionalModule> Modules = new List<AdditionalModule>();

    //Item 'definitions'
    public ItemTypes.Types Type;
    public SubTypes SubType;

    //----------------------------------------------------

    public override void Init()
    {
        base.Init();
        EntityType = EntityTypes.Item;
        SetStats();
    }

    protected override void Start()
    {
        base.Start();
        EntityType = EntityTypes.Item;
        EntityManager.RegisterItem(this);
    }

    //Three functions for moving items from world to inventory and vice versa
    public void Pack()
    {
        rendererComponent.enabled = false;
        colliderComponent.enabled = false;
    }
    public void Unpack()
    {
        rendererComponent.enabled = true;
        colliderComponent.enabled = true;
    }
    public void SetFollow(GameObject Target)
    {
        if(Target)  transform.position = Target.transform.position;

        transform.SetParent(Target.transform);
    }

    public override Enum GetEntityType()
    {
        return Type;
    }

    public void DestroyEntity()
    {
        EntityManager.UnregisterItem(this);
        Destroy(this.gameObject);
    }
}
