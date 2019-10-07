using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SysRandom = System.Random;

public class FactionManager : MonoBehaviour
{
    //Dictionaries storing ID/Faction and Name/ID respectively
    public static Dictionary<int, Faction> Factions = new Dictionary<int, Faction>()
    {
        { 0, new Faction("Player", Color.cyan) }
    };
    public static Dictionary<string, int> FactionIDs = new Dictionary<string, int>()
    {
        {"Player", 0 }
    };

    public static SysRandom SRand = new SysRandom();

    //Class containing faction information
    public class Faction
    {
        public static List<Color> Colours = new List<Color>()
        {
            Color.black,
            Color.blue,
            //Color.cyan, (Used by player in testing)
            Color.gray,
            Color.green,
            Color.magenta,
            Color.red,
            Color.white,
            Color.yellow
        };
        static int CurrentID = 0;

        //Instance related information:
        public int ID;
        public string Name;

        private Dictionary<int, float> FactionRelations = new Dictionary<int, float>();

        public Color FactionColour;

        public void SetRelations(int TargetID, float Opinion)
        {
            if (FactionRelations.ContainsKey(TargetID))
            {
                FactionRelations[TargetID] = Opinion;
            } else
            {
                FactionRelations.Add(TargetID, Opinion);
            }
        }
        public float GetRelations(int TargetID)
        {
            if (FactionRelations.ContainsKey(TargetID))
            {
                return FactionRelations[TargetID];
            } else
            {
                throw new System.ArgumentOutOfRangeException("TargetID", "Target relation does not exist");
            }
        }

        public Faction(string _Name, Color _FactionColour)
        {
            ID = CurrentID++;
            Name = _Name;
            FactionColour = _FactionColour;
            Colours.Remove(FactionColour);
        }
    }

    void GenerateFactions()
    {
        //generate factions
        for (int i = 0; i < 4; i++)
        {
            Color colour = Faction.Colours[Mathf.RoundToInt(Random.value * (Faction.Colours.Count - 1))];
            Faction _NewFaction = new Faction(string.Format("Faction {0}", i+1), colour);
            Faction.Colours.Remove(colour);
            Factions.Add(_NewFaction.ID, _NewFaction);
            FactionIDs.Add(_NewFaction.Name, _NewFaction.ID);
        }
        //Generate faction opinions
        for (int i = 0; i < Factions.Count; i++)
        {
            Faction faction = Factions[i];
            for (int n = 0; n < Factions.Count; n++)
            {
                if (n == i) continue;
                faction.SetRelations(n, (Random.value - 0.5f) * 200);
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        GenerateFactions();
    }
}