﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using static ItemTypes;
using static Modules.AdditionalModule;

public class EquippableItem : Item
{
    public Equipment.Slots Slot;
    public ItemTypes.SubTypes Subtype;

    public ItemTypes.BonusInfoStruct BonusInfo;
    public override void Init()
    {
        base.Init();
    }

    public virtual void SetStats(Dictionary<StatsEnum, float> StatMods)
    {
        Vector2 SizeMod = new Vector2(1, 1);
        float MassMod = 1;
        Resources ResourceMod = new Resources(0, 0, 0);

        foreach (AdditionalModule Module in Modules)
        {
            SizeMod += Module.GetStat<Vector2>(StatsEnum.SizeMod) - new Vector2(1f, 1f);
            MassMod += Module.GetStat<float>(StatsEnum.MassMod) - 1f;
            ResourceMod += Module.GetStat<Resources>(StatsEnum.Cost);

            foreach (KeyValuePair<StatsEnum, StatInfoObject> Stat in Module.Stats)
            {
                if (!Stats.Stats.TryGetValue(Stat.Key, out object Value) || Stat.Key == StatsEnum.Cost)
                {
                    continue;
                }
                if (StatMods[Stat.Key] == 1f)
                {
                    StatMods[Stat.Key] = 0f;
                }
                StatMods[Stat.Key] += (float)Stat.Value.Value - 1f;
            }
        }
        foreach (KeyValuePair<StatsEnum, float> StatMod in StatMods)
        {
            Stats.SetStat(StatMod.Key, Mathf.Max(StatMod.Value, 0), ItemStats.OperationEnum.Multiply);
        }

        Stats.SetStat(StatsEnum.Size, new Vector2Int(Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).x * SizeMod.x), Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).y * SizeMod.y)));
        Stats.SetStat(StatsEnum.Mass, BaseStats.GetStat<float>(StatsEnum.Mass) * MassMod);
        Stats.SetStat(StatsEnum.Cost, BaseStats.GetStat<Resources>(StatsEnum.Cost) + ResourceMod);

        bool Changed = FindSubtype();
        if (Subtype != SubTypes.Invalid && Changed)
        {
            BonusInfo = EquipmentBonusInfo[Subtype];
        }
    }

    public virtual bool FindSubtype()
    {
        return false;
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();

        UIController.KVPData SubTypeData = new UIController.KVPData("Sub Type", Subtype, Parent);
        SubTypeData.Group = Group;
        SubTypeData.ValueEnum = ItemTypes.SubTypes.Sword;
        SubTypeData.ValueDelegate = KeyValuePanel.GetItemStat;
        SubTypeData.RefItem = this;

        KVPs.Add(UIController.InstantiateKVP(SubTypeData));

        List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

        List<GameObject> CombinedKVPs = Utility.CombineLists(KVPs, BaseKVPs);
        CombinedKVPLists = Utility.CombineLists(KVPLists, BaseKVPLists);

        return CombinedKVPs;
    }
}

