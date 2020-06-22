using AI.States;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DevTools
{
    public class CombatLog : MonoBehaviour
    {
        public TextMeshProUGUI LogText;

        private void Start()
        {
            LogText = transform.Find("Scroll View/Viewport/Content/Log").GetComponent<TextMeshProUGUI>();
            Attack.AttackEvent += WriteAttackLog;
        }

        public void WriteAttackLog(Entity attacker, Entity defender, Weapon weapon, bool blocked, bool critical, params KeyValuePair<Weapon.DamageTypesEnum, float>[] damages)
        {
            string weaponName = weapon ? weapon.Name : "fist";
            //string attackerColor = $"#{ColorUtility.ToHtmlStringRGB(FactionManager.Factions[attacker.FactionID].FactionColour)}";
            //string defenderColor = $"#{ColorUtility.ToHtmlStringRGB(FactionManager.Factions[defender.FactionID].FactionColour)}";
            string attackerColor = attacker.FactionID == 0 ? "#008000" : "#800000";
            string defenderColor = defender.FactionID == 0 ? "#008000" : "#800000";
            string log;

            if (!blocked)
            {
                string damageText = "";
                int i = 0;
                foreach (KeyValuePair<Weapon.DamageTypesEnum, float> damage in damages)
                {
                    string comma = i > 0 ? "," : "";
                    damageText += $"{comma} {damage.Value} {damage.Key}";
                    i++;
                }
                if (critical) 
                    damageText += $" {ColorTag("#FFAD00", "critical")}";
                damageText += " damage";

                log = $"{ColorTag(attackerColor, attacker.Name)} attacked {ColorTag(defenderColor, defender.Name)} with {ColorTag("#FFAD00", weaponName)} for {ColorTag("#FF0000", damageText)}."; 
            }
            else
            {
                string blockColor;
                if (defender.FactionID == 0)
                    blockColor = "#00FF00";
                else
                    blockColor = "#FF0000";

                log = $"{ColorTag(attackerColor, attacker.Name)} attacked {ColorTag(defenderColor, defender.Name)} with {ColorTag("#FFAD00", weaponName)} but it was {ColorTag(blockColor, "blocked")}!";
            }
            string timeOfDay = string.Format("{0:hh:mm:ss:ff}", DateTime.Now);
            LogText.text += $"\n[{ColorTag("#808080", timeOfDay)}] {log}";
        }

        public string ColorTag(string hex, string input)
        {
            return $"<color={hex}>{input}</color>";
        }
    } 
}
