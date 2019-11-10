using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Shield : AdditionalModule
    {

        public override void CalculateStats()
        {
            float Shield = GetStat<float>(StatsEnum.Shield);
            base.CalculateStats();
            SetStat(StatsEnum.MassMod, 1 + (Shield / 10));
            SetStat(StatsEnum.SizeMod, new Vector2(1 + (Shield / 10), 1 + (Shield / 10)));
            SetStat(StatsEnum.PowerUse, Shield / 10);
        }
        public Shield() : base()
        {
            AddModifiableStats();
            AddStats();

            ModuleName = "Shield";
            Init();
            CalculateStats();
        }

        private void AddModifiableStats()
        {
            ModifiableStats.Add(StatsEnum.Shield, 1f);
        }
        private void AddStats()
        {
            Stats.Add(StatsEnum.PowerUse, 0f);
        }
    }
}