using Items.Parts;
using Medical;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Control;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static ItemTypes;

namespace Items
{
    public class Weapon : EquippableItem
    {
        [Serializable]
        public struct DamageInfo
        {
            public float Damage;
            public float ArmourPiercing;
            public DamageTypesEnum DamageType;

            public DamageInfo(float damage, float armourPiercing, DamageTypesEnum damageType)
            {
                Damage = damage;
                ArmourPiercing = armourPiercing;
                DamageType = damageType;
            }

        }
        public Transform RightAnchor;
        public Transform LeftAnchor;

        public enum DamageTypesEnum
        {
            Blunt,
            Sharp,
            Explosive,
            Psionic,
            Electrical,
            Other
        }
        [Serializable]
        public class WeaponStats : ItemStats
        {
            [Serializable]
            public class AttackStats : ICloneable
            {
                public List<DamageInfo> Damages;
                public bool Ranged;
                public float Range;
                //Attacks per second
                public float AttackSpeed;
                public float Accuracy;

                public SubTypes AttackSkill;
                //Part Functions required for attack speed, hit chance, and damage calculations
                //Based off of the primary damage part only
                public Dictionary<Health.PartFunctions, float> SpeedFunctions;
                public Dictionary<Health.PartFunctions, float> HitFunctions;
                public Dictionary<Health.PartFunctions, float> DamageFunctions;

                public object Clone()
                {
                    AttackStats result = (AttackStats)MemberwiseClone();
                    result.Damages = new List<DamageInfo>(Damages);
                    return result;
                }
            }

            public List<AttackStats> Attacks;
            public float Block;
        }

        public new WeaponStats Stats = new WeaponStats();

        public override void Init()
        {
            base.Init();

            ValidSlots = new Equipment.Slots[] { Equipment.Slots.Weapon, Equipment.Slots.SecondaryWeapon };
        }

        #region Calculate Stats
        public override void CalculateStats()
        {
            EvaluatePart(CorePart);
        }

        /// <summary>
        /// Evaluate the attacks for a single primary part
        /// </summary>
        /// <param name="part"></param>
        private void EvaluatePart(ItemPart part)
        {
            Stats.Attacks = new List<WeaponStats.AttackStats>();
            WeaponStats.AttackStats baseAttack = new WeaponStats.AttackStats()
            {
                Accuracy = part.PartStats.Accuracy,
                AttackSkill = part.PartStats.AttackSkill,
                AttackSpeed = part.PartStats.AttackSpeed,
                DamageFunctions = part.PartStats.DamageFunctions,
                Damages = part.PartStats.Damage.Value,
                HitFunctions = part.PartStats.HitFunctions,
                Range = part.PartStats.Range + CorePart.PartStats.Range,
                Ranged = part.PartStats.Ranged,
                SpeedFunctions = part.PartStats.SpeedFunctions
            };
            bool givenAttack = false;
            foreach (ItemPart.AttachmentPoint aPoint in part.AttachmentPoints)
            {
                if(aPoint.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Output))
                {
                    if(aPoint.AttachedPart != null)
                    {
                        if(aPoint.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Primary))
                        {
                            EvaluatePart(aPoint.AttachedPart);
                        }
                        else if (aPoint.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Secondary))
                        {
                            givenAttack = true;
                            Stats.Attacks.Add(EvaluateSecondaryPart(baseAttack, aPoint.AttachedPart));
                        }
                    }
                    else if (aPoint.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.HasAttack))
                    {
                        givenAttack = true;
                        Stats.Attacks.Add((WeaponStats.AttackStats)baseAttack.Clone());
                    }
                }
            }
            if (!givenAttack)
            {
                Stats.Attacks.Add((WeaponStats.AttackStats)baseAttack.Clone());
            }
        }

        /// <summary>
        /// Evaluate and return the attack modifed by a secondary part
        /// </summary>
        /// <param name="primaryAttack">The base attack from the primary part</param>
        /// <param name="secondaryPart"></param>
        /// <returns></returns>
        private WeaponStats.AttackStats EvaluateSecondaryPart(WeaponStats.AttackStats primaryAttack, ItemPart secondaryPart)
        {
            WeaponStats.AttackStats resultant = (WeaponStats.AttackStats)primaryAttack.Clone();

            foreach(DamageInfo damage in secondaryPart.PartStats.Damage.Value)
            {
                resultant.Damages.Add(damage);
            }

            for(int i = 0; i < resultant.Damages.Count; i++)
            {
                DamageInfo modifiedDamage = resultant.Damages[i];
                modifiedDamage.Damage *= 0.6f;
                resultant.Damages[i] = modifiedDamage;
            }
            return resultant;
        }
        #endregion

        //TODO Update statKVPs
        public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
        {
            List<GameObject> KVPs = new List<GameObject>();
            List<GameObject> KVPLists = new List<GameObject>();
            List<CreateUI.KVPData> KVPDatas = new List<CreateUI.KVPData>
        {
            new CreateUI.KVPData(StatsEnum.Damage.ToString(), Utility.Misc.RoundedString(Stats.Attacks[0].Damages.Sum(d => d.Damage)), Parent, 1),
            new CreateUI.KVPData(StatsEnum.ArmourPiercing.ToString(), Utility.Misc.RoundedString(Stats.Attacks[0].Damages.Sum(d => d.ArmourPiercing)), Parent, 1),
            new CreateUI.KVPData(StatsEnum.AttackSpeed.ToString(), Utility.Misc.RoundedString(Stats.Attacks[0].AttackSpeed), Parent, 1),
            new CreateUI.KVPData(StatsEnum.Range.ToString(), Utility.Misc.RoundedString(Stats.Attacks[0].Range), Parent, 1),
        };

            KVPDatas[0].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Attacks[0].Damages.Sum(d => d.Damage));
            KVPDatas[1].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Attacks[0].Damages.Sum(d => d.ArmourPiercing));
            KVPDatas[2].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Attacks[0].AttackSpeed);
            KVPDatas[3].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Attacks[0].Range);

            foreach (CreateUI.KVPData Data in KVPDatas)
            {
                Data.Group = Group;
                KVPs.Add(CreateUI.Info.KeyValuePanel(Data));
            }

            List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

            List<GameObject> CombinedKVPs = Utility.Collections.CombineLists(KVPs, BaseKVPs);
            CombinedKVPLists = Utility.Collections.CombineLists(KVPLists, BaseKVPLists);

            return CombinedKVPs;
        }
    } 
}
