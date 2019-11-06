using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Barrel : ItemModule.AdditionalModule
    {

        public override void CalculateStats()
        {
            float Shield = GetStat<float>(StatsEnum.Shield);
            base.CalculateStats();
            SetStat(StatsEnum.MassMod, 1 + (Shield / 10));
            SetStat(StatsEnum.SizeMod, new Vector2(1 + (Shield / 10), 1 + (Shield / 10)));
            SetStat(StatsEnum.PowerUse, Shield / 10);
        }
        public Barrel() : base()
        {
            Stats.Add(StatsEnum.Accuracy, 0f);
            Stats.Add(StatsEnum.Damage, 0f);
            //Stats.Add(StatFlagsEnum)
            ModuleName = "Shield";
            Init();
            CalculateStats();
        }
    }
}
