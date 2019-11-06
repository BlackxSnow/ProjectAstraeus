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
        { MaterialTypes.Iron, new Material(MaterialTypes.Iron, 1,0.25f,0.25f) },
        { MaterialTypes.Steel, new Material(MaterialTypes.Steel, 1.1f, 0.5f, 0.5f) }
    };

    public class Material
    {
        public MaterialTypes Name;
        public float MassModifier;
        public float DamageModifier;
        public float ArmourModifier;

        public Material(MaterialTypes Name, float Mass, float Damage, float Armour)
        {
            this.Name = Name;
            MassModifier = Mass;
            DamageModifier = Damage;
            ArmourModifier = Armour;
        }
    }
}
