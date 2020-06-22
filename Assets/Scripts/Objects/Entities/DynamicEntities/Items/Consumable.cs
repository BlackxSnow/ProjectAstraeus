using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public abstract class Consumable : Item, IUsable
{
    public override void Init()
    {
        base.Init();
        Stats.AddStat(ItemTypes.StatsEnum.Quantity, 5);
        Stats.AddStat(ItemTypes.StatsEnum.MaxQuantity, 5);
    }
    public abstract void Use(Actor user);

    public abstract bool Act(Entity user, Entity target, object iteratedOn);
    public abstract bool GetNextIteration(Entity target, out object iterateOn, out float time);
}
