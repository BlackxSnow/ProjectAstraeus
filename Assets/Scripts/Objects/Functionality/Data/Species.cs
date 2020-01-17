using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Medical;

//Data class for species, including animals and humanoid races
[Serializable]
public class Species
{
    public static List<Species> LoadedSpecies = new List<Species>();
    public string Name;
    public List<Health.BodyPart> Body = new List<Health.BodyPart>();
    
}
