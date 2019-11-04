using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Materials;

namespace Modules
{
    public class Plating : ItemModule.AdditionalModule
    {

        public override GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
        {
            GameObject[] ThisKVPArray = new GameObject[1];

            ThisKVPArray[0] = UIController.InstantiateKVP("Armour", GetStat<float>(StatFlagsEnum.Armour), Parent, 0, Group: KVPGroup, ValueDelegate: KeyValuePanel.GetModuleStat, RefModule: this, ValueEnum: StatFlagsEnum.Armour);

            GameObject[] BaseKVPArray = base.InstantiateStatKVPs(Parent, KVPGroup, out KVPListArray);
            GameObject[] ReturnKVPArray = Utility.CombineArrays(BaseKVPArray, ThisKVPArray);
            return ReturnKVPArray;
        }

        public override void CalculateStats()
        {
            float Thickness = GetStat<float>(StatFlagsEnum.Thickness);
            Materials.Material _Material = GetStat<Materials.Material>(StatFlagsEnum.Material);
            base.CalculateStats();
            SetStat<float>(StatFlagsEnum.MassMod, (1 + Thickness) * _Material.MassModifier);
            SetStat(StatFlagsEnum.SizeMod, new Vector2(1 + (Thickness / 10), 1 + (Thickness / 10)));

            SetStat(StatFlagsEnum.Armour, Thickness * _Material.ArmourModifier);
            SetStat(StatFlagsEnum.Cost, new Resources((int)Mathf.Round(Thickness), 0, 0));
        }
        public Plating(float _Thickness = 1) : base()
        {
            Materials.Material _Material;
            MaterialDict.TryGetValue(MaterialTypes.Iron, out _Material);
            Stats.Add(StatFlagsEnum.Thickness, _Thickness);
            Stats.Add(StatFlagsEnum.Material, _Material);
            Stats.Add(StatFlagsEnum.Armour, 0f);

            ModuleName = "Plating";
            Init();
            CalculateStats();
        }
    }
}