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
        public Vector2 SizeMultiplier;
        public float MassMultiplier;
        public Resources Cost = new Resources(0,0,0);

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

        public virtual void CalculateStats() { }

        public class Plating : AdditionalModule
        {
            public float Thickness;
            public Materials.Material @Material;
            public float Armour;



            public override void CalculateStats()
            {
                base.CalculateStats();
                MassMultiplier = (1 + Thickness) * Material.MassModifier;
                SizeMultiplier = new Vector2(1 + (Thickness / 10), 1 + (Thickness / 10));

                Armour = Thickness * Material.ArmourModifier;
            }

            public Plating(float _Thickness = 1)
            {
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
