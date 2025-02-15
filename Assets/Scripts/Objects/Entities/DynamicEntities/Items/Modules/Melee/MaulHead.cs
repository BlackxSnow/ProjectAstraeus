﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class MaulHead : AdditionalModule
    {
        public override void CalculateStats()
        {
            float Length = GetStat<float>(StatsEnum.Length);
            float Thickness = GetStat<float>(StatsEnum.Thickness);
            Materials.Material _Material = GetStat<Materials.Material>(StatsEnum.Material);

            base.CalculateStats();
            SetStat(StatsEnum.MassMod, (1f + Length * Thickness * 50f) * _Material.MassModifier);
            SetStat(StatsEnum.SizeMod, new Vector2(1f + (Thickness), 1f + Length));

            SetStat(StatsEnum.Damage, (2f + ((Length + Thickness) * 100f)) * _Material.DamageModifier);
            SetStat(StatsEnum.Range, Thickness);
            SetStat(StatsEnum.Cost, (new Resources(0,0,0) + _Material.BaseCost) * (int)Mathf.Ceil(Length));
        }
        public MaulHead() : base()
        {
            AddModifiableStats();
            AddStats();

            ModuleName = "Head";
            Init();
            CalculateStats();
        }

        private void AddModifiableStats()
        {
            ModifiableStats.Add(StatsEnum.Length, new StatInfoObject(0f) { MaxValue = 0.30f });
            ModifiableStats.Add(StatsEnum.Thickness, new StatInfoObject(0f) { MaxValue = 0.15f });
            ModifiableStats.Add(StatsEnum.Material, new StatInfoObject(MaterialDict[MaterialTypes.Wood]));
        }
        private void AddStats()
        {
            Stats.Add(StatsEnum.Range, new StatInfoObject(0f));
            Stats.Add(StatsEnum.Damage, new StatInfoObject(0f));
        }
    } 
}
