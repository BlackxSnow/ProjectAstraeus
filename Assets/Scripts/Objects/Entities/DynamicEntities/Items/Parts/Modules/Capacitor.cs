using Items.Parts;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Modules
{
    public class Capacitor : PartModule
    {
        public override List<ModuleGroups> ValidGroups { get; protected set; } = new List<ModuleGroups>()
        {
            ModuleGroups.Offensive
        };

        public override ItemPart.ModularStats BaseModuleStats { get; } = new ItemPart.ModularStats()
        {
            Damage = new ItemPart.StatData<List<Weapon.DamageInfo>>(true, ItemPart.StatTypeEnum.Additive, new List<Weapon.DamageInfo>() { new Weapon.DamageInfo(1.0f, 0.5f, Weapon.DamageTypesEnum.Electrical) }),
            Sizef = new ItemPart.StatData<Vector2>(new Vector2(1.0f,1.0f)),
            Cost = new ItemPart.StatData<Resources>(true, ItemPart.StatTypeEnum.Additive, new Resources(0.4f, 0.5f, 0.2f)),
            Mass = new ItemPart.StatData<float>(true, ItemPart.StatTypeEnum.Additive, 0.3f),
        };

        public override Dictionary<string, ModifiableStat> ModifiableStats { get; protected set; } = new Dictionary<string, ModifiableStat>()
        {
            { "Size", new ModifiableStat() { StatName = "Size", IsEnabled = true, TargetType = typeof(float), Value = 5f, Bounds = new Vector2(0.5f, 15f) } },
            { "Tech Level", new ModifiableStat() { StatName = "Tech Level", IsEnabled = true, TargetType = typeof(int), Value = 5, Bounds = new Vector2(1, 10) } },
            { "Energy", new ModifiableStat() { StatName = "Energy", IsEnabled = true, TargetType = typeof(float), Value = 1f, Bounds = new Vector2(0.25f, 5f) } },
        };

        public override void CalculateStats()
        {
            ModuleStats = BaseModuleStats;

            Weapon.DamageInfo modifiedDamageInfo = ModuleStats.Damage.Value[0];
            Weapon.DamageInfo baseDamageInfo = modifiedDamageInfo;
            modifiedDamageInfo.Damage = baseDamageInfo.Damage * Mathf.Pow((float)ModifiableStats["Size"].Value, 2) * (float)ModifiableStats["Energy"].Value;
            modifiedDamageInfo.ArmourPiercing = 1.0f - Mathf.Pow(1.0f - baseDamageInfo.ArmourPiercing, (float)ModifiableStats["Energy"].Value / 2.0f);

            ModuleStats.Sizef.Value *= (float)ModifiableStats["Size"].Value;
            ModuleStats.Cost.Value *= Mathf.Pow((float)ModifiableStats["Size"].Value, 2.0f) * Mathf.Pow((float)ModifiableStats["Energy"].Value, 2);
            ModuleStats.Mass.Value *= Mathf.Pow((float)ModifiableStats["Size"].Value, 2.0f) * Mathf.Pow((float)ModifiableStats["Energy"].Value, 2);
        }
    }
}
