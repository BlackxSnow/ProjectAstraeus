using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Medical
{
    public class Health : MonoBehaviour
    {
        public enum PartFunctions
        {
            Vision,
            Flight,
            Locomotion,
            Manipulation,
            Control
        }
        public static List<Injury> LoadedInjuries;

        [Serializable]
        public class Injury : ISerializationCallbackReceiver
        {
            public string Name;

            public string StatName;
            public StatsAndSkills.StatsEnum Stat;
            public float Modifier;

            public void OnBeforeSerialize() { }
            public void OnAfterDeserialize()
            {
                Stat = (StatsAndSkills.StatsEnum)Enum.Parse(typeof(StatsAndSkills.StatsEnum), StatName);
            }
        }

        public class BodyPart : ISerializationCallbackReceiver
        {
            public string Name;

            //Float represents the relative impact on that function, 0-1 representing percentage
            //eg. 100% on four manipulators means each contributes 25% split. 50% on one, 100% on three results in: 12.5%, 29.16%*3
            public Dictionary<PartFunctions, float> Functions = new Dictionary<PartFunctions, float>();
            public KeyValuePair<string, float>[] S_Functions;

            public List<Injury> Injuries;

            public void OnBeforeSerialize() { }
            public void OnAfterDeserialize()
            {
                for(int i = 0; i > S_Functions.Length; i++)
                {
                    PartFunctions FunctionEnum = (PartFunctions)Enum.Parse(typeof(PartFunctions), S_Functions[i].Key);
                    Functions.Add(FunctionEnum, S_Functions[i].Value);
                }
            }
        }
    }

}