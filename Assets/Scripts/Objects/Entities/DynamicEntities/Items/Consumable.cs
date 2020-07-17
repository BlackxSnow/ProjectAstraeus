using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Items;

public abstract class Consumable : Item, IUsable
{
    public class ConsumableStats : ItemStats
    {
        public float Quality;
        public float Quantity;
        public float MaxQuantity;
    }
    public override void Init()
    {
        base.Init();
    }

    public new ConsumableStats Stats;

    public abstract void Use(Actor user);

    public abstract bool Act(Entity user, Entity target, object iteratedOn);
    public abstract bool GetNextIteration(Entity target, out object iterateOn, out float time);
}
