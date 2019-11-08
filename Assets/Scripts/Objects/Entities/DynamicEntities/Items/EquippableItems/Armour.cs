using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using static Modules.AdditionalModule;
using static ItemTypes;



public class Armour : EquippableItem
{


    public override void Init()
    {
        base.Init();
        Stats.AddStat(StatsEnum.Armour, 0f);
        Stats.AddStat(StatsEnum.Shield, 0f);
        Stats.AddStat(StatsEnum.Power, 0f);
        Stats.AddStat(StatsEnum.PowerUse, 0f);

        BaseStats = new BaseItemStats()
        {
            Stats = new Dictionary<StatsEnum, object>()
            {
                { StatsEnum.Armour, 1f },
                { StatsEnum.Shield, 0f },
                { StatsEnum.Power, 0f },
                { StatsEnum.PowerUse, 0f },
                { StatsEnum.Size, new Vector2Int(1,1) },
                { StatsEnum.Mass, 1f },
                { StatsEnum.Cost, new Resources(1,0,0) }
            },
            CompatibleModules = new List<ModulesEnum>()
            {
                ModulesEnum.Plating,
                ModulesEnum.Reactor,
                ModulesEnum.Shield
            }
        };
        SetStats();
    }
    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null) 
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>();

        KVPDatas.Add(new UIController.KVPData(StatsEnum.Armour.ToString(), Stats.GetStat<float>(StatsEnum.Armour), Parent, 1));
        KVPDatas.Add(new UIController.KVPData(StatsEnum.Shield.ToString(), Stats.GetStat<float>(StatsEnum.Shield), Parent, 1));
        KVPDatas.Add(new UIController.KVPData(StatsEnum.Power.ToString(), Stats.GetStat<float>(StatsEnum.Power), Parent, 1));
        KVPDatas.Add(new UIController.KVPData(StatsEnum.PowerUse.ToString(), Stats.GetStat<float>(StatsEnum.PowerUse), Parent, 1));

        KVPDatas[0].ValueEnum = StatsEnum.Armour;
        KVPDatas[1].ValueEnum = StatsEnum.Shield;
        KVPDatas[2].ValueEnum = StatsEnum.Power;
        KVPDatas[3].ValueEnum = StatsEnum.PowerUse;

        foreach (UIController.KVPData Data in KVPDatas)
        {
            Data.RefItem = this;
            Data.ValueDelegate = KeyValuePanel.GetItemStat;
            Data.Group = Group;
            KVPs.Add(UIController.InstantiateKVP(Data));
        }

        List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

        List<GameObject> CombinedKVPs = Utility.CombineLists(KVPs, BaseKVPs);
        CombinedKVPLists = Utility.CombineLists(KVPLists, BaseKVPLists);

        return CombinedKVPs;
    }

    public override void SetStats()
    {
        Vector2 SizeMod = new Vector2(1, 1);
        float MassMod = 1;
        Resources ResourceMod = new Resources(0, 0, 0);
        Stats.Stats = new Dictionary<StatsEnum, object>(BaseStats.Stats);

        Dictionary<StatsEnum, float> StatMods = new Dictionary<StatsEnum, float>()
        {
                { StatsEnum.Armour, 1f },
                { StatsEnum.Shield, 1f },
                { StatsEnum.Power, 1f },
                { StatsEnum.PowerUse, 1f },
        };

        foreach (AdditionalModule Module in Modules)
        {
            SizeMod += Module.GetStat<Vector2>(StatsEnum.SizeMod) - new Vector2(1f, 1f);
            MassMod += Module.GetStat<float>(StatsEnum.MassMod) - 1f;
            ResourceMod += Module.GetStat<Resources>(StatsEnum.Cost);

            foreach (KeyValuePair<StatsEnum, object> Stat in Module.Stats)
            {
                if (!Stats.Stats.TryGetValue(Stat.Key, out object Value) || Stat.Key == StatsEnum.Cost)
                {
                    continue;
                }
                if (StatMods[Stat.Key] == 1f)
                {
                    StatMods[Stat.Key] = 0f;
                }
                StatMods[Stat.Key] += (float)Stat.Value;
            }
        }
        foreach (KeyValuePair<StatsEnum, float> StatMod in StatMods)
        {
            Stats.SetStat(StatMod.Key, StatMod.Value, ItemStats.OperationEnum.Multiply);
        }

        Stats.SetStat(StatsEnum.Size, new Vector2Int(Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).x * SizeMod.x), Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).y * SizeMod.y)));
        Stats.SetStat(StatsEnum.Mass, BaseStats.GetStat<float>(StatsEnum.Mass) * MassMod);
        Stats.SetStat(StatsEnum.Cost, BaseStats.GetStat<Resources>(StatsEnum.Cost) + ResourceMod);
    }
}
