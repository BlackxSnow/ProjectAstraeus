using System.Collections;
using System.Collections.Generic;
using UI.Control;
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

            public override void EndEffect()
            {
                base.EndEffect();
            }

            public override Sprite GetIcon()
            {
                if(!IconBleeding)   IconBleeding = UIController.LoadedSprites[UIController.SpritesEnum.Condition_Bleeding];

                return IconBleeding;
            }

            public override void Init(ConditionData Data, Health CharacterHealth, Injury injury = null)
            {
                base.Init(Data, CharacterHealth, injury);
                Icon = GetIcon();
            }

            public Bleeding() { }
        } 
    }
}