using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Plating : AdditionalModule
    {
        public override void CalculateStats()
        {
            float Thickness = GetStat<float>(StatsEnum.Thickness);
            Materials.Material _Material = GetStat<Materials.Material>(StatsEnum.Material);
            base.CalculateStats();
            SetStat<float>(StatsEnum.MassMod, (1 + Thickness) * _Material.MassModifier);
            SetStat(StatsEnum.SizeMod, new Vector2(1 + (Thickness / 10), 1 + (Thickness / 10)));

            SetStat(StatsEnum.Armour, Thickness * _Material.ArmourModifier);
            SetStat(StatsEnum.Cost, new Resources((int)Mathf.Round(Thickness), 0, 0));
        }
        public Plating() : base()
        {
            Materials.Material _Material;
            MaterialDict.TryGetValue(MaterialTypes.Iron, out _Material);
            ModifiableStats.Add(StatsEnum.Thickness, 1f);
            ModifiableStats.Add(StatsEnum.Material, _Material);
            Stats.Add(StatsEnum.Armour, 0f);

            ModuleName = "Plating";
            Init();
            CalculateStats();
        }
    }
}