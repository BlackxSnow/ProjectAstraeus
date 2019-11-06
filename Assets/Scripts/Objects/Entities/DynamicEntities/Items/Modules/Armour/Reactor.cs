using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Reactor : ItemModule.AdditionalModule
    {
        public override void CalculateStats()
        {
            float Power = GetStat<float>(StatsEnum.Power);
            base.CalculateStats();
            SetStat(StatsEnum.MassMod, 1 + (Power / 10));
            SetStat(StatsEnum.SizeMod, new Vector2(1 + (Power / 10), 1 + (Power / 10)));
        }
        public Reactor(float _Power = 0) : base()
        {
            ModifiableStats.Add(StatsEnum.Power, _Power);
            ModuleName = "Reactor";
            Init();
            CalculateStats();
        }
    }
}
