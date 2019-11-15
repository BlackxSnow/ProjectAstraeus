using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Barrel : AdditionalModule
    {
        public override void CalculateStats()
        {
            float Length = GetStat<float>(StatsEnum.Length);
            Materials.Material _Material = GetStat<Materials.Material>(StatsEnum.Material);

            base.CalculateStats();
            SetStat(StatsEnum.Damage, 1f + Length * Mathf.Pow(_Material.DamageModifier, 0.5f));
            SetStat(StatsEnum.ArmourPiercing, 1f + Length);
            SetStat(StatsEnum.Range, 1f + Length * 50f);
            SetStat(StatsEnum.Accuracy, 1f + 1f / (10f / (Length * 10f)));
            SetStat(StatsEnum.MassMod, 1f + Length);
            SetStat(StatsEnum.SizeMod, new Vector2(1f + (Length * 10), 1f));
        }
        public Barrel() : base()
        {
            SetCompatibleMaterials();
            AddModifiableStats();
            AddStats();

            ModuleName = "Barrel";
            Init();
            CalculateStats();
        }


        private void SetCompatibleMaterials()
        {
            CompatibleMaterials = new List<MaterialTypes>()
            {
                MaterialTypes.Iron,
                MaterialTypes.Steel
            };
        }
        private void AddModifiableStats()
        {
            ModifiableStats.Add(StatsEnum.Length, new StatInfoObject(0f) { MaxValue = 1.00f });
            ModifiableStats.Add(StatsEnum.Material, new StatInfoObject(MaterialDict[MaterialTypes.Iron]));
        }
        private void AddStats()
        {
            Stats.Add(StatsEnum.Damage, new StatInfoObject(0f));
            Stats.Add(StatsEnum.ArmourPiercing, new StatInfoObject(0f));
            Stats.Add(StatsEnum.Range, new StatInfoObject(0f));
            Stats.Add(StatsEnum.Accuracy, new StatInfoObject(0f));
        }
    }
}
