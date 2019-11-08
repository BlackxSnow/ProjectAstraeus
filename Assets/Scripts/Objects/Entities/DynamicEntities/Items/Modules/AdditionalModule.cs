using System;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;

namespace Modules
{

    public class AdditionalModule : ItemModule
    {
        public AdditionalModule Copy()
        {
            AdditionalModule ModuleCopy = (AdditionalModule)MemberwiseClone();
            ModuleCopy.Stats = new Dictionary<StatsEnum, object>(Stats);
            ModuleCopy.ModifiableStats = new Dictionary<StatsEnum, object>(ModifiableStats);
            ModuleCopy.ModuleName = string.Copy(ModuleName);
            return ModuleCopy;
        }

        public Dictionary<StatsEnum, object> Stats = new Dictionary<StatsEnum, object>()
        {
            { StatsEnum.SizeMod, new Vector2(1,1) },
            { StatsEnum.MassMod, 1f },
            { StatsEnum.Cost, new Resources(0,0,0) }
        };
        public Dictionary<StatsEnum, object> ModifiableStats = new Dictionary<StatsEnum, object>();

        public List<Materials.MaterialTypes> CompatibleMaterials = new List<Materials.MaterialTypes>()
        {
            Materials.MaterialTypes.Iron,
            Materials.MaterialTypes.Steel
        };
        public virtual string ModuleName { get; protected set; }

        public enum ModulesEnum
        {
            //Armour
            [Type(typeof(Plating))]
            Plating,
            [Type(typeof(Reactor))]
            Reactor,
            [Type(typeof(Shield))]
            Shield,
            //Ranged
            [Type(typeof(Barrel))]
            Barrel,
            [Type(typeof(Bolt))]
            Bolt,
            [Type(typeof(Calibre))]
            Calibre
        }
        public enum ModifiableStatsEnum
        {
            Power = StatsEnum.Power,
            Thickness = StatsEnum.Thickness,
            Shield = StatsEnum.Shield,
            Material = StatsEnum.Material
        }


        public enum OperationEnum
        {
            None,
            Add,
            Subtract,
            Multiply,
            Divide
        }
        public T GetStat<T>(StatsEnum Stat)
        {
            if (Stats.TryGetValue(Stat, out object StatValue))
            {
                if (!Stats[Stat].GetType().IsAssignableFrom(typeof(T)))
                {
                    throw new ArgumentException(string.Format("Type {0} was requested, member type was {1}", typeof(T), Stats[Stat].GetType()));
                }
                return (T)StatValue;
            }
            else if (ModifiableStats.TryGetValue(Stat, out StatValue))
            {
                if (StatValue is Materials.Material Mat && typeof(T) == typeof(string))
                {
                    return (T)(object)Mat.Name.ToString();
                }
                return (T)StatValue;
            }
            throw new ArgumentException(string.Format("StatsEnum {0} could not be found in module dictionaries", Stat));
        }

        public void SetStat<T>(StatsEnum Stat, T Value, OperationEnum Operation = OperationEnum.None)
        {
            ref Dictionary<StatsEnum, object> RefDict = ref CheckDicts(Stat);

            if (Operation != OperationEnum.None && Value is float FValue)
            {
                CheckOperation(Operation, ref RefDict, Stat, FValue);
                return;
            }
            else if (Operation != OperationEnum.None && !(Value is float))
            {
                throw new ArgumentException(string.Format("Attempted arithmatic with type {0}", Value.GetType()));
            }

            if (!RefDict[Stat].GetType().IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), Stats[Stat].GetType()));
            }

            RefDict[Stat] = Value;
        }

        protected ref Dictionary<StatsEnum, object> CheckDicts(StatsEnum Stat)
        {
            if (Stats.ContainsKey(Stat))
            {
                return ref Stats;
            }
            else if (ModifiableStats.ContainsKey(Stat))
            {
                return ref ModifiableStats;
            }
            throw new ArgumentException(string.Format("No dict found with StatsEnum '{0}'", Stat));
        }

        protected void CheckOperation(OperationEnum Operation, ref Dictionary<StatsEnum, object> RefDict, StatsEnum Stat, float Value)
        {
            switch (Operation)
            {
                case OperationEnum.Add:
                    RefDict[Stat] = (float)RefDict[Stat] + Value;
                    break;
                case OperationEnum.Subtract:
                    RefDict[Stat] = (float)RefDict[Stat] - Value;
                    break;
                case OperationEnum.Multiply:
                    RefDict[Stat] = (float)RefDict[Stat] * Value;
                    break;
                case OperationEnum.Divide:
                    RefDict[Stat] = (float)RefDict[Stat] / Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return;
        }

        public object this[StatsEnum Stat]
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
            GameObject[] KVPArray = new GameObject[Stats.Count - 1];
            KVPListArray = new GameObject[1];

            int i = 0;
            foreach (KeyValuePair<StatsEnum, object> Stat in Stats)
            {
                UIController.KVPData Data;
                if (Stat.Value is float)
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<float>(Stat.Key), Parent, 2);
                }
                else if (Stat.Value is Vector2)
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<Vector2>(Stat.Key), Parent);
                }
                else if (Stat.Value is Materials.Material)
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<string>(Stat.Key), Parent);
                }
                else continue;

                Data.RefModule = this;
                Data.ValueDelegate = KeyValuePanel.GetModuleStat;
                Data.Group = KVPGroup;
                Data.ValueEnum = Stat.Key;
                KVPArray[i] = UIController.InstantiateKVP(Data);
                i++;
            }

            UIController.KVPData[] CostData = new UIController.KVPData[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData("Iron", GetStat<Resources>(StatsEnum.Cost)[Resources.ResourceList.Iron], null);
            CostData[1] = new UIController.KVPData("Copper", GetStat<Resources>(StatsEnum.Cost)[Resources.ResourceList.Copper], null);
            CostData[2] = new UIController.KVPData("Alloy", GetStat<Resources>(StatsEnum.Cost)[Resources.ResourceList.Alloy], null);

            CostData[0].ValueEnum = Resources.ResourceList.Iron;
            CostData[1].ValueEnum = Resources.ResourceList.Copper;
            CostData[2].ValueEnum = Resources.ResourceList.Alloy;

            foreach (UIController.KVPData Data in CostData)
            {
                Data.RefModule = this;
                Data.ValueDelegate = KeyValuePanel.GetModuleStat;
            }
            KVPListArray[0] = (UIController.InstantiateKVPList("Cost", CostData, Parent, KVPGroup));

            return KVPArray;
        }
        public virtual void CalculateStats() { }
        protected virtual void Init()
        {

        }
        public AdditionalModule()
        {
            Init();
        }
    }
}