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
            SetStat(StatsEnum.Cost, (new Resources(0,0,0) + _Material.BaseCost) * (int)Mathf.Ceil(Length));
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
            ModifiableStats.Add(StatsEnum.Length, 1f);
            ModifiableStats.Add(StatsEnum.Material, MaterialDict[MaterialTypes.Wood]);
        }
        private void AddStats()
        {
            Stats.Add(StatsEnum.Range, 0f);
            Stats.Add(StatsEnum.Damage, 0f);
        }
    } 
}
