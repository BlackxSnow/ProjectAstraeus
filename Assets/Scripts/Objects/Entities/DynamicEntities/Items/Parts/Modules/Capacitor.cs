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

        public override ModuleModifiableStats ModifiableStats { get; protected set; } = new ModuleModifiableStats()
        {
            Size = (true, 5f),
            TechLevel = (true, 1),
            Strength = (true, 1.0f)
        };

        public override void CalculateStats()
        {
            ModuleStats = BaseModuleStats;

            Weapon.DamageInfo modifiedDamageInfo = ModuleStats.Damage.Value[0];
            Weapon.DamageInfo baseDamageInfo = modifiedDamageInfo;
            modifiedDamageInfo.Damage = baseDamageInfo.Damage * Mathf.Pow(ModifiableStats.Size.value, 2) * ModifiableStats.Strength.value;
            modifiedDamageInfo.ArmourPiercing = 1.0f - Mathf.Pow(1.0f - baseDamageInfo.ArmourPiercing, ModifiableStats.Strength.value / 2.0f);

            ModuleStats.Sizef.Value *= ModifiableStats.Size.value;
            ModuleStats.Cost.Value *= Mathf.Pow(ModifiableStats.Size.value, 2.0f) * Mathf.Pow(ModifiableStats.Strength.value, ModifiableStats.Strength.value);
            ModuleStats.Mass.Value *= Mathf.Pow(ModifiableStats.Size.value, 2.0f) * Mathf.Pow(ModifiableStats.Strength.value, ModifiableStats.Strength.value);
        }
    }
}
