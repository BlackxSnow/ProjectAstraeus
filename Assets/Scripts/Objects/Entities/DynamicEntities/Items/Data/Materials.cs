using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class Materials
    {
        public enum MaterialTypes
        {
            Wood,
            Iron,
            Steel
            //Additional materials here
        }

        public static Dictionary<MaterialTypes, Material> MaterialDict = new Dictionary<MaterialTypes, Material>()
    {
        { MaterialTypes.Wood, new Material()
        {
            Name = MaterialTypes.Wood,
            MassModifier = 0.5f,
            DamageModifier = 0.1f,
            ArmourModifier = 0.1f,
            BaseCost = new Resources(0, 0, 0)
        } },
        { MaterialTypes.Iron, new Material()
        {
            Name = MaterialTypes.Iron,
            MassModifier = 1f,
            DamageModifier = 0.25f,
            ArmourModifier = 0.25f,
            BaseCost = new Resources(1, 0, 0)
        } },
        { MaterialTypes.Steel, new Material()
        {
            Name = MaterialTypes.Steel,
            MassModifier = 1.1f,
            DamageModifier = 0.5f,
            ArmourModifier = 0.5f,
            BaseCost = new Resources(0, 0, 1)
        } }
    };

        public class Material
        {
            public MaterialTypes Name;
            public float MassModifier;
            public float DamageModifier;
            public float ArmourModifier;
            public Resources BaseCost;
        }
    }

}