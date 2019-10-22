using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Materials;

public class ItemModule : ScriptableObject
{


    public class CoreModule : ItemModule
    {
        public Vector2Int Size = new Vector2Int(1, 1);
        public float Mass = 1f;
        public Resources Cost = new Resources(0, 0, 0);
        public ItemTypes.ItemFlags Flags;
        public ItemTypes.StatFlags StatFlags;
        public AdditionalModule.ModuleList AvailableModules;

        public CoreModule() { }
        public CoreModule (Vector2Int Size, float Mass, Resources Cost, ItemTypes.ItemFlags Flags, AdditionalModule.ModuleList AvailableModules)
        {
            this.Size = Size;
            this.Mass = Mass;
            this.Cost = Cost;
            this.Flags = Flags;
            this.AvailableModules = AvailableModules;

            foreach(ItemTypes.ItemFlags Flag in Utility.GetFlags(Flags))
            {
                ItemTypes.FlagStats.TryGetValue(Flag, out ItemTypes.StatFlags _StatFlags );
                StatFlags = StatFlags | _StatFlags;
            }
        }
    }

    //Additional Modules
    public class AdditionalModule : ItemModule
    {
        public virtual string ModuleName { get; protected set; }
        public Vector2 SizeMultiplier;
        public float MassMultiplier;
        public Resources Cost = new Resources(0, 0, 0);

        [Flags]
        public enum ModuleList
        {
            Plating = 1 << 0,
            Reactor = 1 << 1,
            Shielding = 1 << 2,
            WeaponPlaceHolderModule = 1 << 3
        }

        public enum ModifiableStats
        {
            Thickness,
            Material,
            Power,
            Shield
        }

        public enum DisplayStats
        {
            Damage,
            AttackSpeed,
            ArmourPiercing,
            Range,
            Armour,
            Power,
            PowerUse,
            Shield,
            SizeMod,
            MassMod,
            Cost
        }

        public virtual GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
        {
            GameObject[] KVPArray = new GameObject[2];
            KVPListArray = new GameObject[1];

            UIController.KVPData<float>[] CostData = new UIController.KVPData<float>[Resources.ResourceCount];
            CostData[0] = new UIController.KVPData<float>("Iron", Cost.Iron, null, Group: KVPGroup, ValueDelegate: KeyValuePanel.ModuleGetValue.Cost_Iron, RefModule: this);
            CostData[1] = new UIController.KVPData<float>("Copper", Cost.Copper, null, Group: KVPGroup, ValueDelegate: KeyValuePanel.ModuleGetValue.Cost_Copper, RefModule: this);
            CostData[2] = new UIController.KVPData<float>("Alloy", Cost.Alloy, null, Group: KVPGroup, ValueDelegate: KeyValuePanel.ModuleGetValue.Cost_Alloy, RefModule: this);

            KVPArray[0] = UIController.InstantiateKVP("Size Modifier", SizeMultiplier, Parent, 2, Group: KVPGroup, ValueDelegate: KeyValuePanel.ModuleGetValue.SizeMod, RefModule: this);
            KVPArray[1] = UIController.InstantiateKVP("Mass Modifier", MassMultiplier, Parent, 2, Group: KVPGroup, ValueDelegate: KeyValuePanel.ModuleGetValue.MassMod, RefModule: this);
            KVPListArray[0] = UIController.InstantiateKVPList("Cost", CostData, Parent);
            return KVPArray;
        }
        public virtual void CalculateStats() { }

        public class Plating : AdditionalModule
        {
            public float Thickness;
            public Materials.Material @Material;
            public float Armour;

            public override GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
            {
                GameObject[] ThisKVPArray = new GameObject[1];

                ThisKVPArray[0] = UIController.InstantiateKVP("Armour", Armour, Parent, 0, Group: KVPGroup, ValueDelegate: KeyValuePanel.ModuleGetValue.Armour, RefModule: this);

                GameObject[] BaseKVPArray = base.InstantiateStatKVPs(Parent, KVPGroup, out KVPListArray);
                GameObject[] ReturnKVPArray = Utility.CombineArrays(BaseKVPArray, ThisKVPArray);
                return ReturnKVPArray;
            }

            public override void CalculateStats()
            {
                base.CalculateStats();
                MassMultiplier = (1 + Thickness) * Material.MassModifier;
                SizeMultiplier = new Vector2(1 + (Thickness / 10), 1 + (Thickness / 10));

                Armour = Thickness * Material.ArmourModifier;
                Cost.Iron = (int)Mathf.Round(Thickness);
            }
            public Plating(float _Thickness = 1)
            {
                ModuleName = "Plating";
                Thickness = _Thickness;
                MaterialDict.TryGetValue(MaterialTypes.Iron, out Material);
                CalculateStats();
            }
        }

        public class Reactor : AdditionalModule
        {
            public float Power;

            public override void CalculateStats()
            {
                base.CalculateStats();
                MassMultiplier = 1 + (Power / 10);
                SizeMultiplier = new Vector2(1 + (Power / 10), 1 + (Power / 10));
            }

            public override GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
            {
                GameObject[] ThisKVPArray = new GameObject[1];

                ThisKVPArray[0] = UIController.InstantiateKVP("Power", Power, Parent, 0, ValueDelegate: KeyValuePanel.ModuleGetValue.Power, RefModule: this);

                GameObject[] BaseKVPArray = base.InstantiateStatKVPs(Parent, KVPGroup, out KVPListArray);
                GameObject[] ReturnKVPArray = Utility.CombineArrays(BaseKVPArray, ThisKVPArray);
                return ReturnKVPArray;
            }
            public Reactor()
            {
                ModuleName = "Reactor";
            }
            public Reactor(float _Power = 1)
            {
                Power = _Power;
                CalculateStats();
            }
        }

        public class Shielding : AdditionalModule
        {
            public float Shield;
            public float PowerUsage;

            public override void CalculateStats()
            {
                base.CalculateStats();
                MassMultiplier = 1 + (Shield / 10);
                SizeMultiplier = new Vector2(1 + (Shield / 10), 1 + (Shield / 10));
                PowerUsage = Shield / 10;
            }

            public override GameObject[] InstantiateStatKVPs(Transform Parent, KeyValueGroup KVPGroup, out GameObject[] KVPListArray)
            {
                GameObject[] ThisKVPArray = new GameObject[2];

                ThisKVPArray[0] = UIController.InstantiateKVP("Shield", Shield, Parent, 0, ValueDelegate: KeyValuePanel.ModuleGetValue.Shield, RefModule: this);
                ThisKVPArray[1] = UIController.InstantiateKVP("Power Use", PowerUsage, Parent, 0, ValueDelegate: KeyValuePanel.ModuleGetValue.PowerUsage, RefModule: this);

                GameObject[] BaseKVPArray = base.InstantiateStatKVPs(Parent, KVPGroup, out KVPListArray);
                GameObject[] ReturnKVPArray = Utility.CombineArrays(BaseKVPArray, ThisKVPArray);
                return ReturnKVPArray;
            }
            public Shielding()
            {
                ModuleName = "Shield";
            }
            public Shielding(float _Shield = 1)
            {
                Shield = _Shield;
                CalculateStats();
            }
        }

        public class WeaponPlaceHolderModule : AdditionalModule
        {

        }
    }
}
