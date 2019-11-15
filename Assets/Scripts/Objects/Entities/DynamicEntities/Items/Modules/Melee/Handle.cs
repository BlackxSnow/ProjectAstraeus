using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Handle : AdditionalModule
    {
        public override void CalculateStats()
        {
            float Length = GetStat<float>(StatsEnum.Length);
            Materials.Material _Material = GetStat<Materials.Material>(StatsEnum.Material);
            base.CalculateStats();
            SetStat(StatsEnum.MassMod, (1f + (Length * 5f)) * _Material.MassModifier);
            SetStat(StatsEnum.SizeMod, new Vector2(1f + (Length / 10f), 1f));

            SetStat(StatsEnum.Damage,  (Length * 5f) * _Material.DamageModifier);
            SetStat(StatsEnum.Range, Length);
            SetStat(StatsEnum.Cost, (new Resources(0,0,0) + _Material.BaseCost) * (int)Mathf.Ceil(Length*10));
        }
        public Handle() : base()
        {
            AddModifiableStats();
            AddStats();

            ModuleName = "Handle";
            Init();
            CalculateStats();
        }
        private void AddModifiableStats()
        {
            ModifiableStats.Add(StatsEnum.Length, new StatInfoObject(1f) { MaxValue = 2.00f });
            ModifiableStats.Add(StatsEnum.Material, new StatInfoObject(MaterialDict[MaterialTypes.Wood]));
        }
        private void AddStats()
        {
            Stats.Add(StatsEnum.Range, new StatInfoObject(0f));
            Stats.Add(StatsEnum.Damage, new StatInfoObject(0f));
        }
    } 
}
