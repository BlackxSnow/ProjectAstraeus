using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Medical
{
    namespace Conditions
    {
        public class Bleeding : Condition
        {
            private static Sprite IconBleeding;
            
            public override void RunEffect()
            {
                CharacterHealth.Damage(Severity * Time.deltaTime, false);
            }

            public override void EndEffect()
            {
                base.EndEffect();
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