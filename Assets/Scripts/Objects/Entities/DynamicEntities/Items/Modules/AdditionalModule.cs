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
            ModuleCopy.Stats = new Dictionary<StatsEnum, StatInfoObject>(Stats);
            ModuleCopy.ModifiableStats = new Dictionary<StatsEnum, StatInfoObject>(ModifiableStats);
            ModuleCopy.ModuleName = string.Copy(ModuleName);
            return ModuleCopy;
        }

        public struct StatInfoObject
        {
            public object Value;
            public float MinValue;
            public float MaxValue;

            public StatInfoObject(object Value)
            {
                this.Value = Value;
                MinValue = 0;
                MaxValue = 0;
            }
        }

        public Dictionary<StatsEnum, StatInfoObject> Stats = new Dictionary<StatsEnum, StatInfoObject>()
        {
            { StatsEnum.SizeMod, new StatInfoObject(new Vector2(1,1)) },
            { StatsEnum.MassMod, new StatInfoObject(1f) },
            { StatsEnum.Cost, new StatInfoObject(new Resources(0,0,0)) }
        };
        public Dictionary<StatsEnum, StatInfoObject> ModifiableStats = new Dictionary<StatsEnum, StatInfoObject>();

        public List<Firearm.FireModes> CompatibleFireModes = new List<Firearm.FireModes>()
        {
            Firearm.FireModes.BoltAction,
            Firearm.FireModes.SemiAutomatic,
            Firearm.FireModes.Automatic
        };

        public List<Materials.MaterialTypes> CompatibleMaterials = new List<Materials.MaterialTypes>()
        {
            Materials.MaterialTypes.Wood,
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
            //Melee
            [Type(typeof(Handle))]
            Handle,
            [Type(typeof(Blade))]
            Blade,
            [Type(typeof(MaulHead))]
            Head,
            //Ranged
            [Type(typeof(Barrel))]
            Barrel,
            [Type(typeof(Bolt))]
            Bolt
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
            object StatValue;
            if (Stats.TryGetValue(Stat, out StatInfoObject StatInfo))
            {
                StatValue = StatInfo.Value;
                if (!Stats[Stat].Value.GetType().IsAssignableFrom(typeof(T)))
                {
                    throw new ArgumentException(string.Format("Type {0} was requested, member type was {1}", typeof(T), Stats[Stat].Value.GetType()));
                }
                return (T)StatValue;
            }
            else if (ModifiableStats.TryGetValue(Stat, out StatInfo))
            {
                StatValue = StatInfo.Value;
                if (StatValue is Materials.Material Mat && typeof(T) == typeof(string))
                {
                    return (T)(object)Mat.Name.ToString();
                } else if (typeof(T) == typeof(string))
                {
                    return (T)(object)StatValue.ToString();
                }
                return (T)StatValue;
            }
            throw new ArgumentException(string.Format("StatsEnum {0} could not be found in module dictionaries", Stat));
        }

        public void SetStat<T>(StatsEnum Stat, T Value, OperationEnum Operation = OperationEnum.None)
        {
            ref Dictionary<StatsEnum, StatInfoObject> RefDict = ref CheckDicts(Stat);
            if (!RefDict.TryGetValue(Stat, out StatInfoObject DictValue))
            {
                throw new ArgumentException(string.Format($"{Stat} does not exist in {RefDict}"));
            }
            if (!RefDict[Stat].Value.GetType().IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} was passed to member of type {1}", Value.GetType(), RefDict[Stat].Value.GetType()));
            }
            if (Operation != OperationEnum.None && Value is float FValue)
            {
                CheckOperation(Operation, ref RefDict, Stat, FValue);
                return;
            }
            else if (Operation != OperationEnum.None && !(Value is float))
            {
                throw new ArgumentException(string.Format("Attempted arithmatic with type {0}", Value.GetType()));
            }


            StatInfoObject NewInfo = new StatInfoObject(Value)
            {
                MinValue = RefDict[Stat].MinValue,
                MaxValue = RefDict[Stat].MaxValue
            };
            RefDict[Stat] = NewInfo;
        }

        protected ref Dictionary<StatsEnum, StatInfoObject> CheckDicts(StatsEnum Stat)
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

        protected void CheckOperation(OperationEnum Operation, ref Dictionary<StatsEnum, StatInfoObject> RefDict, StatsEnum Stat, float Value)
        {
            StatInfoObject NewInfo;
            switch (Operation)
            {
                case OperationEnum.Add:
                    NewInfo = RefDict[Stat];
                    NewInfo.Value = (float)RefDict[Stat].Value + Value;
                    Debug.Log(RefDict[Stat]);
                    break;
                case OperationEnum.Subtract:
                    NewInfo = RefDict[Stat];
                    NewInfo.Value = (float)RefDict[Stat].Value - Value;
                    Debug.Log(RefDict[Stat]);
                    //RefDict[Stat] = (float)RefDict[Stat] - Value;
                    break;
                case OperationEnum.Multiply:
                    NewInfo = RefDict[Stat];
                    NewInfo.Value = (float)RefDict[Stat].Value * Value;
                    Debug.Log(RefDict[Stat]);
                    //RefDict[Stat] = (float)RefDict[Stat] * Value;
                    break;
                case OperationEnum.Divide:
                    NewInfo = RefDict[Stat];
                    NewInfo.Value = (float)RefDict[Stat].Value / Value;
                    Debug.Log(RefDict[Stat]);
                    //RefDict[Stat] = (float)RefDict[Stat] / Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return;
        }
        public virtual GameObject[] InstantiateModifiableStatKVPs(Transform Parent, KeyValueGroup KVPGroup)
        {
            GameObject[] KVPArray = new GameObject[ModifiableStats.Count];

            int i = 0;
            foreach (KeyValuePair<StatsEnum, StatInfoObject> Stat in ModifiableStats)
            {
                UIController.KVPData Data;
                if (Stat.Value.Value is float)
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<float>(Stat.Key), Parent, 2);
                }
                else
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<string>(Stat.Key), Parent);
                }

                Data.RefModule = this;
                Data.ValueDelegate = KeyValuePanel.GetModuleStat;
                Data.Group = KVPGroup;
                Data.ValueEnum = Stat.Key;
                KVPArray[i] = UIController.InstantiateKVP(Data);
                i++;
            }

            return KVPArray;
        }
        public virtual GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
        {
            GameObject[] KVPArray = new GameObject[Stats.Count - 1];
            KVPListArray = new GameObject[1];

            int i = 0;
            foreach (KeyValuePair<StatsEnum, StatInfoObject> Stat in Stats)
            {
                UIController.KVPData Data;
                if (Stat.Value.Value is float)
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<float>(Stat.Key), Parent, 2);
                }
                else if (Stat.Value.Value is Vector2)
                {
                    Data = new UIController.KVPData(Stat.Key.ToString(), GetStat<Vector2>(Stat.Key), Parent);
                }
                else if (Stat.Value.Value is Materials.Material)
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
                Data.Group = KVPGroup;
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