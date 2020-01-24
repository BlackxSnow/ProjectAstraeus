using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

namespace Medical
{
    public partial class Health
    {
        public class BodyPart
        {
            public string Name;
            public float InjuryChance;
            //This may be changed to allow for > 100% totals:
            //Float represents the relative impact on that function, 0-1 representing percentage
            //eg. 100% on four manipulators means each contributes 25% split. 50% on one, 100% on three results in: 12.5%, 29.16%*3
            public Dictionary<PartFunctions, float> Functions;
            public Dictionary<string, float> S_Functions;

            public List<Injury> Injuries;

            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                Functions = new Dictionary<PartFunctions, float>();
                Injuries = new List<Injury>();
                foreach(KeyValuePair<string, float> Function in S_Functions)
                {
                    PartFunctions FunctionEnum = (PartFunctions)Enum.Parse(typeof(PartFunctions), Function.Key);
                    Functions.Add(FunctionEnum, Function.Value);
                }
            }
            public BodyPart Clone()
            {
                BodyPart Result = new BodyPart();
                Result.Name = Name;
                Result.InjuryChance = InjuryChance;
                Result.Functions = new Dictionary<PartFunctions, float>(Functions);
                Result.Injuries = new List<Injury>(Injuries);
                return Result;
            }
        }
    }

}