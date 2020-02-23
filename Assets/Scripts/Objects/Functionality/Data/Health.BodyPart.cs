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
            public string BoneName;
            public float InjuryChance;
            public float SeverityCost;

            
            public Dictionary<PartFunctions, float> Functions;
            public Dictionary<string, float> Functions_S;

            public List<Injury> Injuries;

            [OnDeserialized]
            public void OnDeserialised(StreamingContext context)
            {
                Functions = Utility.DeserializeEnumCollection<PartFunctions, float>(Functions_S);
                Injuries = new List<Injury>();
            }
            public BodyPart Clone()
            {
                BodyPart Result = (BodyPart)MemberwiseClone();
                Result.Injuries = new List<Injury>(Injuries);
                return Result;
            }
        }
    }

}