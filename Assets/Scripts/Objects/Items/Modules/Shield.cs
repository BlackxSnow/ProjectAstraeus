using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Shield : ItemModule.AdditionalModule
    {

        public override void CalculateStats()
        {
            float Shield = GetStat<float>(StatFlagsEnum.Shield);
            base.CalculateStats();
            SetStat(StatFlagsEnum.MassMod, 1 + (Shield / 10));
            SetStat(StatFlagsEnum.SizeMod, new Vector2(1 + (Shield / 10), 1 + (Shield / 10)));
            SetStat(StatFlagsEnum.PowerUse, Shield / 10);
        }

        public override GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
        {
            GameObject[] ThisKVPArray = new GameObject[2];

            ThisKVPArray[0] = UIController.InstantiateKVP("Shield", GetStat<float>(StatFlagsEnum.Shield), Parent, 0, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: StatFlagsEnum.Shield);
            ThisKVPArray[1] = UIController.InstantiateKVP("Power Use", GetStat<float>(StatFlagsEnum.PowerUse), Parent, 0, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: StatFlagsEnum.PowerUse);

            GameObject[] BaseKVPArray = base.InstantiateStatKVPs(Parent, KVPGroup, out KVPListArray);
            GameObject[] ReturnKVPArray = Utility.CombineArrays(BaseKVPArray, ThisKVPArray);
            return ReturnKVPArray;
        }
        public Shield(float _Shield = 0) : base()
        {
            Stats.Add(StatFlagsEnum.PowerUse, 0f);
            Stats.Add(StatFlagsEnum.Shield, _Shield);
            ModuleName = "Shield";
            Init();
            CalculateStats();
        }
    }
}
