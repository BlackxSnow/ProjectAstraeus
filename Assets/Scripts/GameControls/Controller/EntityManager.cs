using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static List<Character> Characters = new List<Character>(); //Collection of all units
    public static List<Character> PlayerCharacters = new List<Character>();
    public static List<ISelectable> Selectables = new List<ISelectable>();
    public static List<Item> Items = new List<Item>();


    public static bool RegisterCharacter(Character Character, int FactionID)
    { 
        if (!PlayerCharacters.Contains(Character) && FactionID == 0)
            PlayerCharacters.Add(Character);

        if(Characters.Contains(Character))
            return false;
        else
        {
            Characters.Add(Character);
            return true;
        }
    }

    public static bool RegisterItem(Item item)
    {
        if (Items.Contains(item))
            return false;
        else
        {
            Items.Add(item);
            return true;
        }
    }

    public static bool RegisterSelectable(ISelectable Selectable)
    {
        if(Selectables.Contains(Selectable))
            return false;
        else
        {
            Selectables.Add(Selectable);
            return true;
        }
    }
}
