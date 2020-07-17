using Medical;
using System.Collections.Generic;
using System.Linq;
using UI.Control;
using UnityEngine;
using static ItemTypes;

namespace Items
{
    public class Weapon : EquippableItem
    {
        public struct DamageInfo
        {
            //Deserialisation data
            public string DamageType_S;

            //Object data
            public float Damage;
            public float ArmourPiercing;
            public DamageTypesEnum DamageType;

        }
        public Transform RightAnchor;
        public Transform LeftAnchor;

        public enum DamageTypesEnum
        {
            Blunt,
            Sharp,
            Explosive,
            Psionic,
            Other
        }

        public class WeaponStats : ItemStats
        {
            public struct AttackStats
            {
                public DamageInfo[] Damages;
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
            }

            public AttackStats[] Attacks;
            public float Block;
        }

        public new WeaponStats Stats = new WeaponStats();

        public override void Init()
        {
            base.Init();

            ValidSlots = new Equipment.Slots[] { Equipment.Slots.Weapon, Equipment.Slots.SecondaryWeapon };
        }

        //TODO Implement - Source stats from parts and their modules.
        public override void CalculateStats()
        {
            throw new System.NotImplementedException();
        }

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
