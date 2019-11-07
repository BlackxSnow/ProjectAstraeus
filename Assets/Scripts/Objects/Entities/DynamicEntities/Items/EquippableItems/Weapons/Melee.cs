using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;

public class Melee : Weapon
{
    public override void Init()
    {
        base.Init();
        Stats.AddStat(StatsEnum.Block, 0f);
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>();

        KVPDatas.Add(new UIController.KVPData(StatsEnum.Block.ToString(), Stats.GetStat<float>(StatsEnum.Block), Parent, 1));

        KVPDatas[0].ValueEnum = StatsEnum.Block;

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
