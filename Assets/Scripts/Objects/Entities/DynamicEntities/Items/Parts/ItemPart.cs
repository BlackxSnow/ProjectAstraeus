using Items.Modules;
using JetBrains.Annotations;
using Medical;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using UnityEditorInternal;
using UnityEngine;

namespace Items.Parts
{
    public abstract class ItemPart
    {
        [Flags]
        public enum AttachmentTypeFlags
        {
            Input = 1 << 0,
            Output = 1 << 1,
            Primary = 1 << 2,
            Secondary = 1 << 3,
            HasAttack = 1 << 4
        }
        public class AttachmentPoint
        {
            public enum DirectionEnum 
            {
                Up,
                Right,
                Down,
                Left
            }
            public const int RotationStep = 90;
            //Normalized location data relative to part
            public Vector2 Position { get; private set; }
            public DirectionEnum Direction { get; private set; }
            public AttachmentTypeFlags AttachmentFlags { get; private set; }
            public ItemPart AttachedPart { get; private set; }
            public AttachmentPoint AttachedPoint { get; private set; }
            public ItemPart ParentPart { get; set; }



            public bool Attach(AttachmentPoint targetPoint)
            {
                if(!AttachmentFlags.HasFlag(AttachmentTypeFlags.Input) && !targetPoint.AttachmentFlags.HasFlag(AttachmentTypeFlags.Input))
                {
                    Debug.LogWarning("Neither AttachmentPoint is input");
                    return false;
                }
                else if (!AttachmentFlags.HasFlag(AttachmentTypeFlags.Output) && !targetPoint.AttachmentFlags.HasFlag(AttachmentTypeFlags.Output))
                {
                    Debug.LogWarning("Neither AttachmentPoint is output");
                    return false;
                }

                AttachedPart = targetPoint.ParentPart;
                AttachedPoint = targetPoint;

                targetPoint.AttachedPart = ParentPart;
                targetPoint.AttachedPoint = this;
                return true;
            }

            public bool Detach()
            {
                if (AttachedPoint == null)
                {
                    Debug.LogWarning("AttachedPoint is null");
                    return false;
                }

                AttachedPoint.AttachedPoint = null;
                AttachedPoint.AttachedPart = null;
                AttachedPart = null;
                AttachedPoint = null;
                return true;
            }

            /// <summary>
            /// Creates an attachment point with Position pos and AttachmentFlags flags
            /// </summary>
            /// <param name="pos">Normalised position of point</param>
            /// <param name="direction">Facing direction of point</param>
            /// <param name="flags">Attachment flags associated with point</param>
            public AttachmentPoint(Vector2 pos, DirectionEnum direction, AttachmentTypeFlags flags)
            {
                if(!Utility.Math.WithinBounds(pos.x, 0,1) || !Utility.Math.WithinBounds(pos.y, 0, 1))
                {
                    throw new ArgumentException($"{pos} is out of bounds");
                }
                Direction = direction;
                Position = pos;
                AttachmentFlags = flags;
            }
        }
        public struct PartModifiableStats
        {
            public (bool enabled, float value) SizeX;
            public (bool enabled, float value) SizeY;
            public (bool enabled, Materials.Material value) Material;
        }
        public enum StatTypeEnum
        {
            Additive,
            Multiplicative
        }
        public struct StatData<T>
        {
            public bool Enabled;
            public StatTypeEnum StatType;
            public T Value;

            public static implicit operator T(StatData<T> data)
            {
                return data.Value;
            }

            public static implicit operator StatData<T>(T value)
            {
                return new StatData<T>(value);
            }

            public StatData(bool enabled, StatTypeEnum statType, T value)
            {
                Enabled = enabled;
                StatType = statType;
                Value = value;
            }
            public StatData(T value)
            {
                Enabled = true;
                StatType = StatTypeEnum.Additive;
                Value = value;
            }
        }

        public class ModularStats : Item.ItemStats
        {
            public new StatData<float> Range;
            public StatData<bool> Ranged;

            public new StatData<Vector2> Sizef;
            public new StatData<Vector2Int> Size;
            public new StatData<float> Mass;
            public new StatData<Resources> Cost;

            public StatData<List<Weapon.DamageInfo>> Damage;
            public StatData<float> Accuracy;
            public StatData<float> AttackSpeed;
            public StatData<ItemTypes.SubTypes> AttackSkill;
            public StatData<float> Block;

            public StatData<Dictionary<Health.PartFunctions, float>> SpeedFunctions;
            public StatData<Dictionary<Health.PartFunctions, float>> HitFunctions;
            public StatData<Dictionary<Health.PartFunctions, float>> DamageFunctions;

            public ModularStats Clone()
            {
                return (ModularStats)MemberwiseClone();
            }
        }

        #region Object data
        public abstract List<PartModule.ModuleGroups> ValidGroups { get; protected set; }
        public abstract AttachmentPoint[] AttachmentPoints { get; protected set; }
        public abstract ModularStats BasePartStats { get; }
        #endregion

        #region Runtime data
        public PartModule[] InstalledModules;
        public ModularStats PartStats;
        public PartModifiableStats ModifiableStats;
        #endregion

        /// <summary>
        /// Sets PartStats based on ModifiableStats and BasePartStats
        /// </summary>
        public abstract void CalculateStats();

        public virtual void Init()
        {
            foreach(AttachmentPoint point in AttachmentPoints)
            {
                point.ParentPart = this;
            }
        }

        public ItemPart()
        {
            Init();
            Debug.Log("ItemPart Initialised");
        }
    }
}
