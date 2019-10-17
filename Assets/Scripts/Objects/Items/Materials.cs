using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials
{
    public enum MaterialTypes
    {
        Iron,
        Steel
        //Additional materials here
    }

    public static Dictionary<MaterialTypes, Material> MaterialDict = new Dictionary<MaterialTypes, Material>()
    {
        { MaterialTypes.Iron, new Material(1,0.25f,0.25f) },
        { MaterialTypes.Steel, new Material(1.1f, 0.5f, 0.5f) }
    };

    public class Material
    {
        public float MassModifier;
        public float DamageModifier;
        public float ArmourModifier;

        public Material(float Mass, float Damage, float Armour)
        {
            MassModifier = Mass;
            DamageModifier = Damage;
            ArmourModifier = Armour;
        }
    }
}
