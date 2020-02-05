using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Medical
{
    namespace Conditions
    {
        public class Bleeding : Condition
        {
            private static Sprite IconBleeding;
            
            public override void RunEffect()
            {
                CharacterHealth.Damage(Severity * Time.deltaTime, false, Weapon.DamageTypesEnum.Other);
            }

            public override void EndEffect(bool CreateChildren = true)
            {
                base.EndEffect(CreateChildren);
            }

            protected override Sprite GetIcon()
            {
                if(!IconBleeding)
                {

                    IconBleeding = UIController.LoadedSprites[UIController.SpritesEnum.Condition_Bleeding];
                }
                return IconBleeding;
            }

            public override void Init(Health.ConditionStruct Data, Health CharacterHealth)
            {
                base.Init(Data, CharacterHealth);
                Icon = GetIcon();
            }

            public Bleeding() { }
        } 
    }
}