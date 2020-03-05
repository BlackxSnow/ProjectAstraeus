using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;

public class DebugConsole : MonoBehaviour
{
    public void ParseCommand(string InputCommand)
    {
        string CommandString = InputCommand.Remove(InputCommand.Length - 1); 
        string[] CommandWords = CommandString.Split(' ');
        
        switch(CommandWords[0])
        {
            case "SetRace":
                SetRace(CommandWords[1], CommandWords[2]);
                break;
            case "Damage":
                Damage(CommandWords[1], float.Parse(CommandWords[2]), bool.Parse(CommandWords[3]));
                break;
            case "SetFaction":
                SetFaction(CommandWords[1], int.Parse(CommandWords[2]));
                break;
        }
    }
    public void ParseCommand(TextMeshProUGUI CommandText)
    {
        ParseCommand(CommandText.text);
    }
    public void SetRace(string Target, string Race)
    {
        Character TargetChar = EntityManager.Characters.Find(c => c.Name == Target);
        Species TargetRace = Species.LoadedSpecies.Find(s => s.Name.Equals(Race));
        if (TargetRace != null && TargetChar != null)
        {
            TargetChar.Race = TargetRace;
            TargetChar.EntityComponents.Health.Body = TargetRace.Body.ConvertAll(p => p.Clone());
            TargetChar.EntityComponents.Health.Init();
            Debug.Log("Race set");
        } else
        {
            Debug.Log("Failed to set race");
        }
    }

    public void Damage(string Target, float Amount, bool Critical = false, string DamageType = "Blunt")
    {
        Character TargetChar = EntityManager.Characters.Find(c => c.Name == Target);
        Weapon.DamageTypesEnum DamageTypeParsed = (Weapon.DamageTypesEnum)Enum.Parse(typeof(Weapon.DamageTypesEnum), DamageType);
        TargetChar.EntityComponents.Health.Damage(Amount, Critical, DamageTypeParsed);
        Debug.Log($"Dealt {Amount} damage to {Target} as {DamageType} damage. Critical?: {Critical}");
    }
    
    public void SetFaction(string Target, int ID)
    {
        Character TargetChar = EntityManager.Characters.Find(c => c.Name == Target);
        TargetChar.ChangeFaction(ID);
    }
}
