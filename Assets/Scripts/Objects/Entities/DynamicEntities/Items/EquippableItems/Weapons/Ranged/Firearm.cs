//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Linq;
//using Modules;
//using static ItemTypes;
//using static Modules.AdditionalModule;
//using System.Threading.Tasks;

//public class Firearm : Ranged
//{
//    public enum FireModes
//    {
//        [FloatValues(1f)]
//        BoltAction,
//        [FloatValues(10f)]
//        SemiAutomatic,
//        [FloatValues(20f)]
//        Automatic
//    }
//    public enum Calibers
//    {
//        BMG_50,

//    }

//    public override void Init()
//    {
//        base.Init();

//        BaseStats.CompatibleModules = new List<ModulesEnum>()
//        {
//            ModulesEnum.Barrel,
//            ModulesEnum.Bolt,
//        };

//        BaseStats.RequiredModules = BaseStats.CompatibleModules;
//    }

//    public override void CalculateStats()
//    {
//        Stats.Stats = new Dictionary<StatsEnum, object>(BaseStats.Stats);

//        Dictionary<StatsEnum, float> StatMods = new Dictionary<StatsEnum, float>()
//        {
//            { StatsEnum.Damage, 1f },
//            { StatsEnum.Accuracy, 1f },
//            { StatsEnum.ArmourPiercing, 1f },
//            { StatsEnum.AttackSpeed, 1f },
//            { StatsEnum.Range, 1f }
//        };

//        SetStats(StatMods);
//    }

//    public struct ThresholdStruct
//    {
//        public float PrecisionRifleBarrel;
//        public float AssaultRifleBarrel;
//        public float SMGBarrel;
//        public float PistolBarrel;
//    }
//    public static ThresholdStruct MaxLengths = new ThresholdStruct()
//    {
//        PrecisionRifleBarrel = 1f,
//        AssaultRifleBarrel = 0.50f,
//        SMGBarrel = 0.25f,
//        PistolBarrel = 0.13f
//    };

//#warning Unfinished
//    public override bool FindSubtype()
//    {
//        SubTypes OldType = Subtype;
//        Bolt bolt = (Bolt)Modules.Where(M => M is Bolt).FirstOrDefault();
//        Barrel barrel = (Barrel)Modules.Where(M => M is Barrel).FirstOrDefault();
//        if (bolt && barrel)
//        {
//            FireModes FireMode = bolt.GetStat<FireModes>(StatsEnum.FireMode);
//            float barrelLength = barrel.GetStat<float>(StatsEnum.Length);
//            if (FireMode == FireModes.BoltAction)
//            {
//                Subtype = SubTypes.PrecisionRifle;
//            } else if (barrelLength <=  MaxLengths.PistolBarrel)
//            {
//                Subtype = SubTypes.Handgun;
//            } else if (barrelLength <= MaxLengths.SMGBarrel)
//            {
//                if (FireMode == FireModes.Automatic)
//                {
//                    Subtype = SubTypes.SubmachineGun;
//                } else
//                {
//                    Subtype = SubTypes.AssaultRifle;
//                }
//            } else if (barrelLength <= MaxLengths.AssaultRifleBarrel)
//            {
//                Subtype = SubTypes.AssaultRifle;
//            } else if (barrelLength <= MaxLengths.PrecisionRifleBarrel)
//            {
//                if (FireMode != FireModes.Automatic)
//                {
//                    Subtype = SubTypes.PrecisionRifle;
//                } else
//                {
//                    Subtype = SubTypes.HeavyWeapon;
//                }
//            }
//        } else
//        {
//            Subtype = SubTypes.Invalid;
//        }

//        if (Subtype != OldType) return true; else return false;
        
//    }
//}
