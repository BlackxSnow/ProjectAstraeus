using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Reactor : ItemModule.AdditionalModule
    {
        public override void CalculateStats()
        {
            float Power = GetStat<float>(StatFlagsEnum.Power);
            base.CalculateStats();
            SetStat(StatFlagsEnum.MassMod, 1 + (Power / 10));
            SetStat(StatFlagsEnum.SizeMod, new Vector2(1 + (Power / 10), 1 + (Power / 10)));
        }

        public override GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
        {
            GameObject[] ThisKVPArray = new GameObject[1];

            ThisKVPArray[0] = UIController.InstantiateKVP("Power", GetStat<float>(StatFlagsEnum.Power), Parent, 0, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: StatFlagsEnum.Power);

            GameObject[] BaseKVPArray = base.InstantiateStatKVPs(Parent, KVPGroup, out KVPListArray);
            GameObject[] ReturnKVPArray = Utility.CombineArrays(BaseKVPArray, ThisKVPArray);
            return ReturnKVPArray;
        }
        public Reactor(float _Power = 0) : base()
        {
            Stats.Add(StatFlagsEnum.Power, _Power);
            ModuleName = "Reactor";
            Init();
            CalculateStats();
        }
    }
}
