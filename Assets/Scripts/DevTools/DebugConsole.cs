using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using Items;
using Items.Parts;

namespace DevTools
{
    public class DebugConsole : MonoBehaviour
    {
        private TMP_InputField InputField;
        private ScrollRect ScrollPanel;
        private TextMeshProUGUI HistoryText;

        private void Start()
        {
            InputField = GetComponentInChildren<TMP_InputField>();
            ScrollPanel = GetComponentInChildren<ScrollRect>();
            HistoryText = transform.Find("Scroll View/Viewport/Content/History").GetComponent<TextMeshProUGUI>();
            InputField.onSubmit.AddListener(EnterCommand);
            InputField.onDeselect.AddListener(EnableCamera);
            InputField.onSelect.AddListener(DisableCamera);
        }

        private void EnableCamera(string _)
        {
            CameraControl.CamControl.ControlEnabled = true;
        }
        private void DisableCamera(string _)
        {
            CameraControl.CamControl.ControlEnabled = false;
        }

        private void EnterCommand(string input)
        {
            InputField.text = "";
            if (input == "") return;
            HistoryText.text += $"\n<color=#ababab>{input}</color>";
            ParseCommand(input);
        }

        public void ParseCommand(string InputCommand)
        {
            string CommandString;
            //if (InputCommand.Contains(' '))
            //    CommandString = InputCommand.Remove(InputCommand.Length - 1);
            //else
            //    CommandString = InputCommand;
            CommandString = InputCommand;
            string[] CommandWords = CommandString.Split(' ');

            //try
        //{
            switch (CommandWords[0])
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
                case "Clear":
                    HistoryText.text = "";
                    break;
                case "Help":
                    Help(CommandWords.Length > 1 ? CommandWords?[1] : "");
                    break;
                case "CreateWeapon":
                    CreateWeapon();
                    break;
                case "SC":
                    SetRace("John", "Human");
                    SetRace("Nick", "Human");
                    SetFaction("John", 1);
                    break;
                default:
                    HistoryText.text += $"\n'{InputCommand}' is not recognised as a valid command.";
                    break;
            }
            //}
            //catch (Exception e)
            //{
            //    Help(CommandWords[0]);
            //    throw e;
            //}
        }
        public void ParseCommand(TextMeshProUGUI CommandText)
        {
            ParseCommand(CommandText.text);
        }



        #region commands
        public void Help(string command = "")
        {
            if (command == "")
            {
                HistoryText.text += $"\nCommands:";
                foreach (KeyValuePair<string, string> commandpair in HelpInfo)
                {
                    HistoryText.text += $"\n\t{commandpair.Key}";
                }
            }
            else
            {
                HistoryText.text += $"\nUsage: {HelpInfo[command]}";
            }
        }
        #region HelpInfo
        private Dictionary<string, string> HelpInfo = new Dictionary<string, string>()
    {
        { "SetRace", "SetRace [CharacterName] [RaceName]" },
        { "Damage", "Damage [CharacterName] [Value] [Critical?] [DamageType]" },
        { "SetFaction", "SetFaction [CharacterName] [FactionID]" },
        { "Clear", "Clear" }
    };
        #endregion
        public void SetRace(string Target, string Race)
        {
            Character TargetChar = EntityManager.Characters.Find(c => c.Name == Target);
            Species TargetRace = Species.LoadedSpecies.Find(s => s.Name.Equals(Race));
            if (TargetRace != null && TargetChar != null)
            {
                TargetChar.Race = TargetRace;
                TargetChar.EntityComponents.Health.Body = TargetRace.Body.ConvertAll(p => p.Clone());
                TargetChar.EntityComponents.Health.Init();
            }
            else
            {
                if (TargetRace == null) HistoryText.text += $"/n '{Race}' is not valid.";
                if (TargetChar == null) HistoryText.text += $"/n '{Target}' is not valid.";
            }
        }

        public void Damage(string Target, float Amount, bool Critical = false, string DamageType = "Blunt")
        {
            Character TargetChar = EntityManager.Characters.FirstOrDefault(c => c.Name == Target);
            bool ValidType = Enum.TryParse(DamageType, out Weapon.DamageTypesEnum DamageTypeParsed);
            if (!TargetChar && !ValidType)
            {
                if (TargetChar == null) HistoryText.text += $"/n '{Target}' is not valid.";
                if (!ValidType) HistoryText.text += $"/n '{DamageType}' is not valid.";
            }


            TargetChar.EntityComponents.Health.Damage(Amount, Critical, DamageTypeParsed);
            Debug.Log($"Dealt {Amount} damage to {Target} as {DamageType} damage. Critical?: {Critical}");
        }

        public void SetFaction(string Target, int ID)
        {
            Character TargetChar = EntityManager.Characters.Find(c => c.Name == Target);
            TargetChar.ChangeFaction(ID);
        }

        public void CreateWeapon()
        {
            GameObject itemBase = Instantiate(ItemTypes.ItemBasePrefab, new Vector3(0, 1, 0), Quaternion.identity);
            Weapon weapon = itemBase.AddComponent<Weapon>();
            Handle handle = new Handle();
            handle.ModifiableStats.SizeY.value = 1.0f;
            handle.CalculateStats();

            Hammer hammer = new Hammer();
            hammer.ModifiableStats.SizeX.value = 2.0f;
            hammer.ModifiableStats.SizeY.value = 1.0f;
            hammer.CalculateStats();

            Blade blade = new Blade();
            blade.ModifiableStats.SizeY.value = 1.0f;
            blade.CalculateStats();

            hammer.AttachmentPoints.First(p => p.Position == new Vector2(0.5f, 0)).Attach(handle.AttachmentPoints.First(p => p.Position == new Vector2(0.5f, 1)));
            blade.AttachmentPoints[0].Attach(hammer.AttachmentPoints.First(p => p.Position == new Vector2(0, 0.5f)));

            weapon.CorePart = handle;
            weapon.CalculateStats();
            
        }
        #endregion
    }

}