using Items.Modules;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Parts
{
    public class Handle : ItemPart
    {
        public override string PartName { get; protected set; } = "Handle";
        public override string Description { get; protected set; } = "I can't think of a rhyme for this one.";
        public override bool IsCore { get; protected set; } = true;
        public override List<PartModule.ModuleGroups> ValidGroups { get; protected set; } = new List<PartModule.ModuleGroups>()
        {
            PartModule.ModuleGroups.Offensive
        };
        public override string PartGroup { get; protected set; } = "Weapon";
        public override AttachmentPoint[] AttachmentPoints { get; protected set; } = new AttachmentPoint[]
        {
            new AttachmentPoint(new Vector2(0.5f, 1.0f), AttachmentPoint.DirectionEnum.Up, AttachmentTypeFlags.Output | AttachmentTypeFlags.Primary | AttachmentTypeFlags.HasAttack),
            new AttachmentPoint(new Vector2(0.5f, 0f), AttachmentPoint.DirectionEnum.Down, AttachmentTypeFlags.Output | AttachmentTypeFlags.Primary | AttachmentTypeFlags.HasAttack),
        };
        public override ModularStats BasePartStats { get; } = new ModularStats()
        {
            Damage = new StatData<List<Weapon.DamageInfo>>(new List<Weapon.DamageInfo>() { new Weapon.DamageInfo(0.5f, 0.5f, Weapon.DamageTypesEnum.Blunt) }),
            Accuracy = new StatData<float>(1.0f),
            AttackSkill = new StatData<ItemTypes.SubTypes>(ItemTypes.SubTypes.Quarterstaff),
            AttackSpeed = new StatData<float>(1.0f),
            Block = new StatData<float>(0.1f),
            Cost = new StatData<Resources>(new Resources(0.5f, 0, 0)),
            Mass = new StatData<float>(0.3f),
            Range = new StatData<float>(0.1f),
            Ranged = new StatData<bool>(false),
            Sizef = new StatData<Vector2>(new Vector2(0.1f, 0.1f)),
            SpeedFunctions = new StatData<Dictionary<Medical.Health.PartFunctions, float>>(new Dictionary<Medical.Health.PartFunctions, float>()
            {
                { Medical.Health.PartFunctions.Manipulation, 1.0f },
                { Medical.Health.PartFunctions.Control, 1.0f }
            }),
            HitFunctions = new StatData<Dictionary<Medical.Health.PartFunctions, float>>(new Dictionary<Medical.Health.PartFunctions, float>()
            {
                { Medical.Health.PartFunctions.Manipulation, 1.0f },
                { Medical.Health.PartFunctions.Control, 1.0f },
                { Medical.Health.PartFunctions.Vision, 0.25f }
            }),
            DamageFunctions = new StatData<Dictionary<Medical.Health.PartFunctions, float>>(new Dictionary<Medical.Health.PartFunctions, float>() 
            {
                { Medical.Health.PartFunctions.Manipulation, 2.0f },
                { Medical.Health.PartFunctions.Control, 1.0f }
            })
        };
        public override Dictionary<string, ModifiableStat> ModifiableStats { get; set; } = new Dictionary<string, ModifiableStat>()
        {
            { "SizeX", new ModifiableStat() { StatName = "Size X", IsEnabled = false, TargetType = typeof(int), Value = 5 } },
            { "SizeY", new ModifiableStat() { StatName = "Size Y", IsEnabled = true, TargetType = typeof(int), Value = 20, Bounds = new Vector2(10, 150)} },
            { "Material", new ModifiableStat() { StatName = "Material", IsEnabled = true, TargetType = typeof(Materials.Material), Value = Materials.MaterialDict[Materials.MaterialTypes.Iron]} }
        };

        public override void CalculateStats()
        {
            PartStats = BasePartStats.Clone();
            Weapon.DamageInfo modifiedDamageInfo = PartStats.Damage.Value[0];
            modifiedDamageInfo.Damage *= (int)ModifiableStats["SizeY"].Value * (int)ModifiableStats["SizeY"].Value;
            PartStats.AttackSpeed.Value /= (int)ModifiableStats["SizeY"].Value / 2.0f;
            PartStats.Block.Value *= (int)ModifiableStats["SizeY"].Value;
            PartStats.Cost.Value = ((Materials.Material)ModifiableStats["Material"].Value).BaseCost * ((int)ModifiableStats["SizeY"].Value / 10f);
            PartStats.Mass.Value *= (int)ModifiableStats["SizeY"].Value / 10.0f;
            PartStats.Range.Value = (int)ModifiableStats["SizeY"].Value;
            PartStats.Sizef.Value.y *= (int)ModifiableStats["SizeY"].Value;
            PartStats.Size.Value = new Vector2Int(Mathf.RoundToInt(PartStats.Sizef.Value.x), Mathf.RoundToInt(PartStats.Sizef.Value.y));
        }

        public Handle() : base()
        {

        }
    }
}
