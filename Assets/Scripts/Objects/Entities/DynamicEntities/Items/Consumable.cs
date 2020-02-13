using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item, IUsable
{
    public override void Init()
    {
        base.Init();
        Stats.AddStat(ItemTypes.StatsEnum.Quantity, 5);
        Stats.AddStat(ItemTypes.StatsEnum.MaxQuantity, 5);
    }
    public virtual void Use(Actor UsingActor)
    {

    }
}
