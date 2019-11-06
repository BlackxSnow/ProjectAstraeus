using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;

public class Weapon : EquippableItem
{
    public override void InitItem(ItemTypes.Types _Type)
    {
        base.InitItem(_Type);
        Stats.AddStat(StatsEnum.Damage, 0f);
        Stats.AddStat(StatsEnum.ArmourPiercing, 0f);
        Stats.AddStat(StatsEnum.AttackSpeed, 0f);
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();
        List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>
        {
            new UIController.KVPData(StatsEnum.Damage.ToString(), Stats.GetStat<float>(StatsEnum.Damage), Parent, 1),
            new UIController.KVPData(StatsEnum.ArmourPiercing.ToString(), Stats.GetStat<float>(StatsEnum.ArmourPiercing), Parent, 1),
            new UIController.KVPData(StatsEnum.AttackSpeed.ToString(), Stats.GetStat<float>(StatsEnum.AttackSpeed), Parent, 1)
        };

        KVPDatas[0].ValueEnum = StatsEnum.Damage;
        KVPDatas[1].ValueEnum = StatsEnum.ArmourPiercing;
        KVPDatas[2].ValueEnum = StatsEnum.AttackSpeed;

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
