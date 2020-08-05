using Interfaces;
using System;
using System.Collections.Generic;
using UI.Control;
using UnityEngine;
using static ItemTypes;

namespace Items
{
    public class Armour : EquippableItem, ICraftable
    {

        public class ArmourStats : ItemStats
        {
            public float Armour;
            public float Shield;
            public float PowerUse;
            public float PowerProduction;
        }

        public new ArmourStats Stats = new ArmourStats();
        public string PartGroup { get; protected set; } = "Armour";
        public override void Init()
        {
            base.Init();
            CalculateStats();
        }

        public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
        {
            List<GameObject> KVPs = new List<GameObject>();
            List<GameObject> KVPLists = new List<GameObject>();
            List<CreateUI.KVPData> KVPDatas = new List<CreateUI.KVPData>();

            KVPDatas.Add(new CreateUI.KVPData(StatsEnum.Armour.ToString(), Utility.Misc.RoundedString(Stats.Armour), Parent, 1));
            KVPDatas.Add(new CreateUI.KVPData(StatsEnum.Shield.ToString(), Utility.Misc.RoundedString(Stats.Shield), Parent, 1));
            KVPDatas.Add(new CreateUI.KVPData(StatsEnum.Power.ToString(), Utility.Misc.RoundedString(Stats.PowerProduction), Parent, 1));
            KVPDatas.Add(new CreateUI.KVPData(StatsEnum.PowerUse.ToString(), Utility.Misc.RoundedString(Stats.PowerUse), Parent, 1));

            KVPDatas[0].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Armour);
            KVPDatas[1].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Shield);
            KVPDatas[2].ValueDelegate = () => Utility.Misc.RoundedString(Stats.PowerProduction);
            KVPDatas[3].ValueDelegate = () => Utility.Misc.RoundedString(Stats.PowerUse);

            foreach (CreateUI.KVPData Data in KVPDatas)
            {
                Data.Group = Group;
                KVPs.Add(CreateUI.Info.KeyValuePanel(Data));
            }

            List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

            List<GameObject> CombinedKVPs = Utility.Collections.CombineLists(KVPs, BaseKVPs);
            CombinedKVPLists = Utility.Collections.CombineLists(KVPLists, BaseKVPLists);

            return CombinedKVPs;
        }

        public override void CalculateStats()
        {
            //throw new NotImplementedException();
        }
    } 
}
