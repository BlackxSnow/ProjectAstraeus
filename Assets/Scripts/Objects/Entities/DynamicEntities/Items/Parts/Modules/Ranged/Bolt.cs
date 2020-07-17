//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static ItemTypes;
//using static Materials;

//namespace Modules
//{
//    public class Bolt : AdditionalModule
//    {
//        public override void CalculateStats()
//        {
//            Firearm.FireModes FireMode = GetStat<Firearm.FireModes>(StatsEnum.FireMode);
//            float FireRate = FloatValuesAttribute.GetStoredData(typeof(Firearm.FireModes), FireMode).FloatArray[0];

//            base.CalculateStats();
//            SetStat(StatsEnum.AttackSpeed, FireRate);
//            SetStat(StatsEnum.Accuracy, 1f / FireRate);
//            SetStat(StatsEnum.MassMod, 1f);
//            SetStat(StatsEnum.SizeMod, new Vector2(1, 1));
//        }
//        public Bolt() : base()
//        {
//            AddModifiableStats();
//            AddStats();
            
//            ModuleName = "Bolt";
//            Init();
//            CalculateStats();
//        }

//        private void AddModifiableStats()
//        {
//            ModifiableStats.Add(StatsEnum.FireMode, new StatInfoObject(Firearm.FireModes.SemiAutomatic));
//        }
//        private void AddStats()
//        {
//            Stats.Add(StatsEnum.AttackSpeed, new StatInfoObject(0f));
//            Stats.Add(StatsEnum.Accuracy, new StatInfoObject(0f));
//        }
//    }
//}
