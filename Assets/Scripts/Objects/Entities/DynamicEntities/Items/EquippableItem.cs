using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippableItem : Item
{
    public override void InitItem(ItemTypes.Types _Type)
    {
        base.InitItem(_Type);
    }

    public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
    {
        List<GameObject> KVPs = new List<GameObject>();
        List<GameObject> KVPLists = new List<GameObject>();

        UIController.KVPData SubTypeData = new UIController.KVPData("Sub Type", SubType, Parent);
        SubTypeData.Group = Group;

        KVPs.Add(UIController.InstantiateKVP(SubTypeData));

        List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

        List<GameObject> CombinedKVPs = Utility.CombineLists(KVPs, BaseKVPs);
        CombinedKVPLists = Utility.CombineLists(KVPLists, BaseKVPLists);

        return CombinedKVPs;
    }
}
