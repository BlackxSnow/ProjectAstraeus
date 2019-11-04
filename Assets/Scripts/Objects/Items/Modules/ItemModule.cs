using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Materials;
using static ItemTypes;

public class ItemModule : ScriptableObject
{
    public class CoreModule : ItemModule
    {
        public Vector2Int Size = new Vector2Int(1, 1);
        public float Mass = 1f;
        public Resources Cost = new Resources(0, 0, 0);
        public ItemFlagsEnum ItemFlags;
        public StatFlagsEnum StatFlags;
        public AdditionalModule.ModuleListEnum AvailableModules;
        public Equipment.Slots Slot;

        public CoreModule() { }
        public CoreModule (Vector2Int Size, float Mass, Resources Cost, ItemFlagsEnum ItemFlags, AdditionalModule.ModuleListEnum AvailableModules, Equipment.Slots Slot)
        {
            this.Size = Size;
            this.Mass = Mass;
            this.Cost = Cost;
            this.ItemFlags = ItemFlags;
            this.AvailableModules = AvailableModules;
            this.Slot = Slot;

            foreach(ItemFlagsEnum Flag in Utility.GetFlags(ItemFlags))
            {
                FlagStats.TryGetValue(Flag, out StatFlagsEnum _StatFlags );
                StatFlags = StatFlags | _StatFlags;
            }
        }
    }

    //Additional Modules
    public class AdditionalModule : ItemModule
    {
        public ModifiableStatsEnum ModifiableStats;
        public StatFlagsEnum StatFlags;
        protected Dictionary<StatFlagsEnum, object> Stats = new Dictionary<StatFlagsEnum, object>()
        {
            { StatFlagsEnum.SizeMod, new Vector2(1,1) },
            { StatFlagsEnum.MassMod, 1f },
            { StatFlagsEnum.Cost, new Resources(0,0,0) }
        };
        public virtual string ModuleName { get; protected set; }

        [Flags]
        public enum ModuleListEnum
        {
            Plating = 1 << 0,
            Reactor = 1 << 1,
            Shielding = 1 << 2,
            WeaponPlaceHolderModule = 1 << 3
        }
        public enum ModifiableStatsEnum
        {
            Power = StatFlagsEnum.Power,
            Thickness = StatFlagsEnum.Thickness,
            Shield = StatFlagsEnum.Shield
            //Material = StatFlagsEnum.Material
        }
        

        public enum OperationEnum
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide
        }
        public T GetStat<T>(StatFlagsEnum Stat)
        {
            if (!Stats[Stat].GetType().IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} was requested, member type was {1}", typeof(T), Stats[Stat].GetType()));
            }
            return (T)Stats[Stat];
        }

        public void SetStat<T>(StatFlagsEnum Stat, T Value, OperationEnum Operation = OperationEnum.None)
        {
            if (Value.GetType() != Stats[Stat].GetType())
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }
            if (Operation != OperationEnum.None && Value is float FValue)
            {
                switch (Operation)
                {
                    case OperationEnum.Add:
                        Stats[Stat] = (float)Stats[Stat] + FValue;
                        break;
                    case OperationEnum.Subtract:
                        Stats[Stat] = (float)Stats[Stat] - FValue;
                        break;
                    case OperationEnum.Multiply:
                        Stats[Stat] = (float)Stats[Stat] * FValue;
                        break;
                    case OperationEnum.Divide:
                        Stats[Stat] = (float)Stats[Stat] / FValue;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return;
            } else if (Operation != OperationEnum.None && !(Value is float))
            {
                throw new ArgumentException(string.Format("Attempted arithmatic with type {0}", Value.GetType()));
            }
            Stats[Stat] = Value;
        }

        public object this[StatFlagsEnum Stat]
        {
            get
            {
                return Stats[Stat];
            }
            set
            {
                if (value.GetType() != Stats[Stat].GetType())
                {
                    throw new ArgumentException("Input value is of different type than key's value!");
                }
                Stats[Stat] = value;
            }
        }

        public virtual GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
        {
            GameObject[] KVPArray = new GameObject[2];
            KVPListArray = new GameObject[1];

            UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData<float>("Iron", GetStat<Resources>(StatFlagsEnum.Cost)[Resources.ResourceList.Iron], null, Group: KVPGroup, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: Resources.ResourceList.Iron);
            CostData[1] = new UIController.KVPData<float>("Copper", GetStat<Resources>(StatFlagsEnum.Cost)[Resources.ResourceList.Copper], null, Group: KVPGroup, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: Resources.ResourceList.Copper);
            CostData[2] = new UIController.KVPData<float>("Alloy", GetStat<Resources>(StatFlagsEnum.Cost)[Resources.ResourceList.Alloy], null, Group: KVPGroup, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: Resources.ResourceList.Alloy);

            KVPArray[0] = UIController.InstantiateKVP("Size Modifier", GetStat<Vector2>(StatFlagsEnum.SizeMod), Parent, 2, Group: KVPGroup, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: StatFlagsEnum.SizeMod);
            KVPArray[1] = UIController.InstantiateKVP("Mass Modifier", GetStat<float>(StatFlagsEnum.MassMod), Parent, 2, Group: KVPGroup, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: StatFlagsEnum.MassMod);
            KVPListArray[0] = UIController.InstantiateKVPList("Cost", CostData, Parent, KVPGroup);
            return KVPArray;
        }
        public virtual void CalculateStats() { }
        protected virtual void Init()
        {
            StatFlags = 0;
            ModifiableStats = 0;
            foreach (KeyValuePair<StatFlagsEnum, object> Pair in Stats)
            {
                StatFlags |= Pair.Key;
                if (Enum.IsDefined(typeof(ModifiableStatsEnum), Pair.Key.ToString()))
                {
                    ModifiableStats |= (ModifiableStatsEnum)Enum.Parse(typeof(ModifiableStatsEnum), Pair.Key.ToString());
                }
            }
        }
        public AdditionalModule()
        {
            Init();
        }

        public class WeaponPlaceHolderModule : AdditionalModule
        {

        }
    }
}
