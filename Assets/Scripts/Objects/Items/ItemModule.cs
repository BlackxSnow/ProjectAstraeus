using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModule : ScriptableObject
{
    public class CoreModule : ItemModule
    {
        public Vector2Int Size;
        public float Mass;
        public Resources Cost = new Resources(0,0,0);
    }

    //Additional Modules
    public class AdditionalModule : ItemModule
    {
        public Vector2 SizeMultiplier;
        public float MassMultiplier;
        public Resources Cost = new Resources(0,0,0);

        public AdditionalModule (Vector2 _SizeMultiplier, float _MassMultiplier, Resources _Cost)
        {
            SizeMultiplier = _SizeMultiplier;
            MassMultiplier = _MassMultiplier;
            Cost = _Cost;
        }

        public class Plating : AdditionalModule
        {
            public float Thickness;
            Material material;
            public float Armour;

            public Plating(Vector2 _Size, float _Mass, Resources _Cost, float _Thickness, Material _material, float _Armour) : base(_Size, _Mass, _Cost)
            {
                Thickness = _Thickness;
                material = _material;
                Armour = _Armour;
                material = _material;
            }
        }

        public class Reactor : AdditionalModule
        {
            public float Power;

            public Reactor(Vector2 _Size, float _Mass, Resources _Cost, float _Power) : base(_Size, _Mass, _Cost)
            {
                Power = _Power;
            }
        }

        public class Shielding : AdditionalModule
        {
            public float Shield;
            public float PowerUsage;
            public Shielding(Vector2 _Size, float _Mass, Resources _Cost, float _Shield, float _PowerUsage) : base(_Size, _Mass, _Cost)
            {
                Shield = _Shield;
                PowerUsage = _PowerUsage;
            }
        }
    }
}
