using Items.Modules;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Items.Parts
{
    public class Hammer : ItemPart
    {
        public override List<PartModule.ModuleGroups> ValidGroups { get; protected set; } = new List<PartModule.ModuleGroups>()
        {
            PartModule.ModuleGroups.Offensive
        };
        public override AttachmentPoint[] AttachmentPoints { get; protected set; } = new AttachmentPoint[]
        {
            new AttachmentPoint(new Vector2(0, 0.5f), AttachmentPoint.DirectionEnum.Left, AttachmentTypeFlags.Output | AttachmentTypeFlags.Secondary | AttachmentTypeFlags.HasAttack),
            new AttachmentPoint(new Vector2(0.5f, 1), AttachmentPoint.DirectionEnum.Up, AttachmentTypeFlags.Output | AttachmentTypeFlags.Primary),
            new AttachmentPoint(new Vector2(1, 0.5f), AttachmentPoint.DirectionEnum.Right, AttachmentTypeFlags.Output | AttachmentTypeFlags.Secondary | AttachmentTypeFlags.HasAttack),
            new AttachmentPoint(new Vector2(0.5f, 0), AttachmentPoint.DirectionEnum.Down, AttachmentTypeFlags.Input)
        };
        public override ModularStats BasePartStats { get; } = new ModularStats()
        {
            Damage = new StatData<List<Weapon.DamageInfo>>(new List<Weapon.DamageInfo>() { new Weapon.DamageInfo(0.5f, 0.5f, Weapon.DamageTypesEnum.Blunt) }),
            Accuracy = new StatData<float>(1.0f),
            AttackSkill = new StatData<ItemTypes.SubTypes>(ItemTypes.SubTypes.Hammer),
            AttackSpeed = new StatData<float>(1.0f),
            Block = new StatData<float>(0.01f),
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
        public new PartModifiableStats ModifiableStats = new PartModifiableStats()
        {
            SizeX = (true, 2.0f),
            SizeY = (true, 1.0f),
            Material = (true, Materials.MaterialDict[Materials.MaterialTypes.Iron])
        };

        //TODO Rewrite hammer stats
        public override void CalculateStats()
        {
            PartStats = BasePartStats.Clone();
            Weapon.DamageInfo modifiedDamageInfo = PartStats.Damage.Value[0];
            modifiedDamageInfo.Damage *= ModifiableStats.SizeX.value * ModifiableStats.SizeY.value;
            PartStats.AttackSpeed.Value /= ModifiableStats.SizeY.value / 2.0f;
            PartStats.Block.Value *= ModifiableStats.SizeY.value;
            PartStats.Cost.Value = ModifiableStats.Material.value.BaseCost * (ModifiableStats.SizeY.value / 10f);
            PartStats.Mass.Value *= ModifiableStats.SizeY.value / 10.0f;
            PartStats.Range.Value = ModifiableStats.SizeY.value;
            PartStats.Sizef.Value.y *= ModifiableStats.SizeY.value;
            PartStats.Size.Value = new Vector2Int(Mathf.RoundToInt(PartStats.Sizef.Value.x), Mathf.RoundToInt(PartStats.Sizef.Value.y));
        }
    }
}
