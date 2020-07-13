﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using static ItemTypes;
using System.Threading;
using Medical;
using System.Threading.Tasks;
using System.Linq;
using UI.Control;

public class Weapon : EquippableItem
{

    public Transform RightAnchor;
    public Transform LeftAnchor;

    public enum DamageTypesEnum
    {
        Blunt,
        Sharp,
        Explosive,
        Psionic,
        Other
    }

    //Data for an attack call
    public struct AttackData
    {
        bool Ranged;
        //Functions to check for attack speed, hit chance, and damage respectively
        public Dictionary<Health.PartFunctions, float> SpeedFunctions;
        public Dictionary<Health.PartFunctions, float> HitFunctions;
        public Dictionary<Health.PartFunctions, float> DamageFunctions;
    }

    public AttackData AttackFunctions = new AttackData();

    public override void Init()
    {
        base.Init();
        Stats.AddStat(StatsEnum.Damage, 0f);
        Stats.AddStat(StatsEnum.ArmourPiercing, 0f);
        Stats.AddStat(StatsEnum.AttackSpeed, 0f);

        BaseStats = new BaseItemStats()
        {
            Stats = new Dictionary<StatsEnum, object>()
            {
                { StatsEnum.Damage, 1f },
                { StatsEnum.ArmourPiercing, 1f },
                { StatsEnum.AttackSpeed, 1f },
                { StatsEnum.Size, new Vector2Int(1, 1) },
                { StatsEnum.Mass, 1f },
                { StatsEnum.Cost, new Resources(1, 0, 0) }
            },

        };
        ValidSlots = new Equipment.Slots[] { Equipment.Slots.Weapon, Equipment.Slots.SecondaryWeapon };
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<CreateUI.KVPData> KVPDatas = new List<CreateUI.KVPData>
        {
            new CreateUI.KVPData(StatsEnum.Damage.ToString(), Stats.GetStat<float>(StatsEnum.Damage), Parent, 1),
            new CreateUI.KVPData(StatsEnum.ArmourPiercing.ToString(), Stats.GetStat<float>(StatsEnum.ArmourPiercing), Parent, 1),
            new CreateUI.KVPData(StatsEnum.AttackSpeed.ToString(), Stats.GetStat<float>(StatsEnum.AttackSpeed), Parent, 1)
        };

        KVPDatas[0].ValueEnum = StatsEnum.Damage;
        KVPDatas[1].ValueEnum = StatsEnum.ArmourPiercing;
        KVPDatas[2].ValueEnum = StatsEnum.AttackSpeed;

        foreach (CreateUI.KVPData Data in KVPDatas)
        {
            Data.RefItem = this;
            Data.ValueDelegate = KeyValuePanel.GetItemStat;
            Data.Group = Group;
            KVPs.Add(CreateUI.Info.KeyValuePanel(Data));
        }

        List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

        List<GameObject> CombinedKVPs = Utility.Collections.CombineLists(KVPs, BaseKVPs);
        CombinedKVPLists = Utility.Collections.CombineLists(KVPLists, BaseKVPLists);

        return CombinedKVPs;
    }
}
