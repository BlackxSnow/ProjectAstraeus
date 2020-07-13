using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UI.Control;
using UnityEngine;
using static ItemTypes;

public class Ranged : Weapon
{
    public override void Init()
    {
        base.Init();
        BaseStats.Stats.Add(StatsEnum.Range, 1f);
        BaseStats.Stats.Add(StatsEnum.Accuracy, 1f);

        Stats.AddStat(StatsEnum.Range, 0f);
        Stats.AddStat(StatsEnum.Accuracy, 0f);
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<CreateUI.KVPData> KVPDatas = new List<CreateUI.KVPData>();

        KVPDatas.Add(new CreateUI.KVPData(StatsEnum.Range.ToString(), Stats.GetStat<float>(StatsEnum.Range), Parent, 1));
        KVPDatas.Add(new CreateUI.KVPData(StatsEnum.Accuracy.ToString(), Stats.GetStat<float>(StatsEnum.Accuracy), Parent, 1));

        KVPDatas[0].ValueEnum = StatsEnum.Range;
        KVPDatas[1].ValueEnum = StatsEnum.Accuracy;

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
