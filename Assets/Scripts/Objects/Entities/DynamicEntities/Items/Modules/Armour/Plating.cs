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
            base.CalculateStats(); //This is not switching from Iron??? Check the Dropdown getting the material value
            SetStat<float>(StatsEnum.MassMod, (1f + Thickness) * _Material.MassModifier);
            SetStat(StatsEnum.SizeMod, new Vector2(1f + (Thickness / 10f), 1f + (Thickness / 10f)));

            SetStat(StatsEnum.Armour, Thickness * _Material.ArmourModifier);
            SetStat(StatsEnum.Cost, new Resources((int)Mathf.Round(Thickness), 0, 0));
        }
        public Plating() : base()
        {
            AddModifiableStats();
            AddStats();

            ModuleName = "Plating";
            Init();
            CalculateStats();
        }
        private void AddModifiableStats()
        {
            ModifiableStats.Add(StatsEnum.Thickness, new StatInfoObject(1f));
            ModifiableStats.Add(StatsEnum.Material, new StatInfoObject(MaterialDict[MaterialTypes.Iron]));
        }
        private void AddStats()
        {
            Stats.Add(StatsEnum.Armour, new StatInfoObject(0f));
        }
    }
}