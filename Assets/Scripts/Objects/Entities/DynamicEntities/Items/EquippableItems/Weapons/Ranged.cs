using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;

public class Ranged : Weapon
{
    public override void InitItem(ItemTypes.Types _Type)
    {
        base.InitItem(_Type);
        Stats.AddStat(StatsEnum.Range, 0f);
        Stats.AddStat(StatsEnum.Accuracy, 0f);
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>();

        KVPDatas.Add(new UIController.KVPData(StatsEnum.Range.ToString(), Stats.GetStat<float>(StatsEnum.Range), Parent, 1));
        KVPDatas.Add(new UIController.KVPData(StatsEnum.Accuracy.ToString(), Stats.GetStat<float>(StatsEnum.Accuracy), Parent, 1));

        KVPDatas[0].ValueEnum = StatsEnum.Range;
        KVPDatas[1].ValueEnum = StatsEnum.Accuracy;

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
}
