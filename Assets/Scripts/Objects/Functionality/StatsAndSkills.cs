using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsAndSkills : MonoBehaviour
{
    public class StatSkill
    {
        public enum SkillTypes
        {
            None,
            Melee,
            Ranged,
            Defensive,
            Movement,
            Engineering,
            Technology,
            Covert,
            Social,
            Special
        }
        public SkillTypes SkillType;
        public bool Enabled;

        public int Level;
        public float Experience;
        float[] LevelThresholds;

        public string Description { get; }
        public string[] XPGainActivities { get; }
        public KeyValuePair<string, string>[] AffectedActivities { get; }

        public void AddXP(float Amount)
        {
            Experience += Amount;
            if (Experience >= LevelThresholds[Level + 1])
            {
                Level++;
                Experience -= LevelThresholds[Level];
                Debug.Log(string.Format("Leveled up! Next threshold is Level {0} at {1} XP", Level + 1, LevelThresholds[Level + 1]));
            }
        }
        public StatSkill(SkillTypes SkillType, string Description, string[] XPGainActivities, KeyValuePair<string,string>[] AffectedActivities, bool Enabled = true)
        {
            this.SkillType = SkillType;
            this.Description = Description;
            this.XPGainActivities = XPGainActivities;
            this.AffectedActivities = AffectedActivities;
            this.Enabled = Enabled;
            Level = 1;
            Experience = 0;
            LevelThresholds = new float[100];
            for (int i = 0; i < LevelThresholds.Length; i++)
            {
                LevelThresholds[i] = Mathf.Pow(i * 5, 2f);
            }
        }
    }

    public StatSkill GetSkillInfo(Enum StatSkillEnum)
    {
        if (StatSkillEnum is SkillsEnum Skill)
        {
            return Skills[Skill];
        } else if (StatSkillEnum is StatsEnum Stat)
        {
            return Stats[Stat];
        }

        throw new ArgumentException(string.Format("Input enum '{0}' is of unhandled type", StatSkillEnum));
            
    }
    public void AddXP(Enum StatSkillEnum, float XP)
    {
        if (StatSkillEnum is SkillsEnum Skill)
        {
            Skills[Skill].AddXP(XP);
            return;
        }
        else if (StatSkillEnum is StatsEnum Stat)
        {
            Stats[Stat].AddXP(XP);
            return;
        }

        throw new ArgumentException(string.Format("Input enum '{0}' is of unhandled type", StatSkillEnum));

    }

    //Skill & stat definitions
    public enum StatsEnum
    {
        Strength,
        Dexterity,
        Endurance,
        Intelligence,
        Perception,
        Charisma
    }
    public Dictionary<StatsEnum, StatSkill> Stats = new Dictionary<StatsEnum, StatSkill>
    {
        { StatsEnum.Strength, new StatSkill(StatSkill.SkillTypes.None, "Raw physical strength. How much you can carry, pull, lift, as well as how hard you can smack things (or people).", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Swords] ) },
        { StatsEnum.Dexterity, new StatSkill(StatSkill.SkillTypes.None, "A measure of how light footed you are. Anything that requires movement is enhanced by this. Synonymous with finesse in some contexts.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Swords] ) },
        { StatsEnum.Endurance, new StatSkill(StatSkill.SkillTypes.None, "How long you can keep going for in tough conditions such as when critically injured, lacking sleep, or when carrying an extreme burden.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Swords] ) },
        { StatsEnum.Intelligence, new StatSkill(StatSkill.SkillTypes.None, "The sharpness of your mind. How quickly and thoroughly you process data (computational and mental).", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Swords] ) },
        { StatsEnum.Perception, new StatSkill(StatSkill.SkillTypes.None, "How quickly and accurately you're able to take in the world around you and make split second decisions based on it.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Swords] ) },
        { StatsEnum.Charisma, new StatSkill(StatSkill.SkillTypes.None, "Your ability to sway and charm people, from casual flirting to formal diplomatic meetings.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Swords] ) }
    };

    public enum SkillsEnum
    {
        Swords,
        Polearms,
        Quarterstaffs,
        Daggers,
        Hammers,

        Handguns,
        PrecisionRifles,
        AssaultRifles,
        SubmachineGuns,
        HeavyWeapons,
        ExplosiveWeaponry,
        Bows,

        Athletics,
        Acrobatics,

        LightArmour,
        MediumArmour,
        HeavyArmour,
        Dodge,
        Block,

        ArmourSmithing,
        WeaponSmithing,
        Construction,

        Research,
        Hacking,
        Medical,

        Stealth,
        Thievery,

        Persuasion,
        Intimidation,

        Psionics
    }
    public Dictionary<SkillsEnum, StatSkill> Skills = new Dictionary<SkillsEnum, StatSkill>
    {
        { SkillsEnum.Swords,            new StatSkill(StatSkill.SkillTypes.Melee, "Proficiency with all swords.", new string[]{"Fighting with swords"}, SkillEffects[SkillsEnum.Swords] )},
        { SkillsEnum.Polearms,          new StatSkill(StatSkill.SkillTypes.Melee, "Proficiency with all polearms.", new string[]{"Fighting with polearms"}, SkillEffects[SkillsEnum.Polearms] )},
        { SkillsEnum.Quarterstaffs,     new StatSkill(StatSkill.SkillTypes.Melee, "Proficiency with quarterstaffs.", new string[]{"Fighting with quarterstaffs"}, SkillEffects[SkillsEnum.Quarterstaffs] )},
        { SkillsEnum.Daggers,           new StatSkill(StatSkill.SkillTypes.Melee, "Proficiency with daggers and knives.", new string[]{"Fighting with daggers"}, SkillEffects[SkillsEnum.Daggers] )},
        { SkillsEnum.Hammers,           new StatSkill(StatSkill.SkillTypes.Melee, "Proficiency with Hammers, mauls, and other weapons which function via the 'cause your organs to cease functioning due to trauma' effect.", new string[]{"Fighting with hammers"}, SkillEffects[SkillsEnum.Hammers] )},

        { SkillsEnum.Handguns,          new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with all handguns", new string[]{"Fighting with handguns"}, SkillEffects[SkillsEnum.Handguns] )},
        { SkillsEnum.PrecisionRifles,   new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with sniper rifles, and hunting rifles.", new string[]{"Fighting with precision rifles"}, SkillEffects[SkillsEnum.PrecisionRifles] )},
        { SkillsEnum.AssaultRifles,     new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with semi or fully automatic rifles", new string[]{"Fighting with assault rifles"}, SkillEffects[SkillsEnum.AssaultRifles] )},
        { SkillsEnum.SubmachineGuns,    new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with submachine guns", new string[]{"Fighting with submachine guns"}, SkillEffects[SkillsEnum.SubmachineGuns] )},
        { SkillsEnum.HeavyWeapons,      new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with large, high calibre, fully automatic weaponry. Usually overkill unless you've got emus to kill.", new string[]{"Fighting with heavy weapons"}, SkillEffects[SkillsEnum.HeavyWeapons] )},
        { SkillsEnum.ExplosiveWeaponry, new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with rocket launchers, grenade launchers, and placed or thrown explosive devices.", new string[]{"Fighting with explosive weaponry"}, SkillEffects[SkillsEnum.ExplosiveWeaponry] )},
        { SkillsEnum.Bows,              new StatSkill(StatSkill.SkillTypes.Ranged, "Proficiency with bows. Only the most confident fighters choose the bow as their weapon of choice in a world with machine guns.", new string[]{"Fighting with bows"}, SkillEffects[SkillsEnum.Bows] )},

        { SkillsEnum.Athletics,         new StatSkill(StatSkill.SkillTypes.Movement, "The ability to run and swim. Great for extending your lifespan a few seconds when the heavy weapons jam.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Athletics] )},
        { SkillsEnum.Acrobatics,        new StatSkill(StatSkill.SkillTypes.Movement, "The ability to jump, climb, and fall. Sadly this won't save you from the emus, as they can jump 2.1m into the air.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Acrobatics] )},

        { SkillsEnum.LightArmour,       new StatSkill(StatSkill.SkillTypes.Defensive, "Proficiency with light armours.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.LightArmour] )},
        { SkillsEnum.MediumArmour,      new StatSkill(StatSkill.SkillTypes.Defensive, "Proficiency with medium armours.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.MediumArmour] )},
        { SkillsEnum.HeavyArmour,       new StatSkill(StatSkill.SkillTypes.Defensive, "Proficiency with heavy armours.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.HeavyArmour] )},
        { SkillsEnum.Dodge,             new StatSkill(StatSkill.SkillTypes.Defensive, "The ability to duck and dodge out of the way of harm. Works against bullets too if you're good enough.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Dodge] )},
        { SkillsEnum.Block,             new StatSkill(StatSkill.SkillTypes.Defensive, "The ability to block and deflect attacks. It is not recommended to try blocking bullets, however.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Block] )},

        { SkillsEnum.ArmourSmithing,    new StatSkill(StatSkill.SkillTypes.Engineering, "Proficiency at crafting armours of all weights and sizes.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.ArmourSmithing] )},
        { SkillsEnum.WeaponSmithing,    new StatSkill(StatSkill.SkillTypes.Engineering, "Proficiency at crafting weapons of all types.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.WeaponSmithing] )},
        { SkillsEnum.Construction,      new StatSkill(StatSkill.SkillTypes.Engineering, "Procifiency at building structures and vehicles from huts and bikes to forts and gunships.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Construction] )},

        { SkillsEnum.Research,          new StatSkill(StatSkill.SkillTypes.Technology, "Proficiency at research and discovery.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Research] )},
        { SkillsEnum.Hacking,           new StatSkill(StatSkill.SkillTypes.Technology, "Proficiency at hacking into computer systems and robotics.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Hacking] )},
        { SkillsEnum.Medical,           new StatSkill(StatSkill.SkillTypes.Technology, "Proficiency at patching all types of wounds, as well as performing surgeries.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Medical] )},

        { SkillsEnum.Stealth,           new StatSkill(StatSkill.SkillTypes.Covert, "The ability to sneak and hide from potentially dangerous or unwanted creatures, sapient or otherwise.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Stealth] )},
        { SkillsEnum.Thievery,          new StatSkill(StatSkill.SkillTypes.Covert, "The ability to steal valuable (or not so valuable) items without their owner or nearby people noticing.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Thievery] )},

        { SkillsEnum.Persuasion,        new StatSkill(StatSkill.SkillTypes.Social, "The ability to charm and persuade people into doing what you want them to, and being happy about it.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Persuasion] )},
        { SkillsEnum.Intimidation,      new StatSkill(StatSkill.SkillTypes.Social, "The ability to scare people into doing what you want them to. They won't like it, though.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Intimidation] )},

        { SkillsEnum.Psionics,          new StatSkill(StatSkill.SkillTypes.Special, "Proficiency with utilising the energy of the aether to improve abilities and cause unnatural, powerful phenomena.", new string[]{"XPGain","Activities"}, SkillEffects[SkillsEnum.Psionics] )}
    };

    public static Dictionary<SkillsEnum, KeyValuePair<string, string>[]> SkillEffects = new Dictionary<SkillsEnum, KeyValuePair<string, string>[]>
    {
        { SkillsEnum.Swords,            new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Sword damage","High"), new KeyValuePair<string, string>("Sword attack speed","High"), new KeyValuePair<string, string>("Sword accuracy","High"), new KeyValuePair<string, string>("Sword blocking","High"), new KeyValuePair<string, string>("Dagger related stats","Low") }},
        { SkillsEnum.Polearms,          new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Polearm damage","High"), new KeyValuePair<string, string>("Polearm attack speed","High"), new KeyValuePair<string, string>("Polearm accuracy","High"), new KeyValuePair<string, string>("Polearm blocking","High"), new KeyValuePair<string, string>("Quarterstaff related stats","Low") }},
        { SkillsEnum.Quarterstaffs,     new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Quarterstaff damage","High"), new KeyValuePair<string, string>("Quarterstaff attack speed","High"), new KeyValuePair<string, string>("Quarterstaff accuracy","High"), new KeyValuePair<string, string>("Quarterstaff blocking","High"), new KeyValuePair<string, string>("Polearm related stats","Low") }},
        { SkillsEnum.Daggers,           new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Dagger damage","High"), new KeyValuePair<string, string>("Dagger attack speed","High"), new KeyValuePair<string, string>("Dagger accuracy","High"), new KeyValuePair<string, string>("Dagger blocking","High"), new KeyValuePair<string, string>("Sword related stats","Low") }},
        { SkillsEnum.Hammers,           new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Hammer damage","High"), new KeyValuePair<string, string>("Hammer attack speed","High"), new KeyValuePair<string, string>("Hammer accuracy","High"), new KeyValuePair<string, string>("Hammer blocking","High") }},

        { SkillsEnum.Handguns,          new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Handgun damage","High"), new KeyValuePair<string, string>("Handgun reload speed","High"), new KeyValuePair<string, string>("Handgun accuracy","High"), new KeyValuePair<string, string>("Submachinegun related stats","Low") }},
        { SkillsEnum.PrecisionRifles,   new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Precision rifle damage","High"), new KeyValuePair<string, string>("Precision rifle reload speed","High"), new KeyValuePair<string, string>("Precision rifle accuracy","High"), new KeyValuePair<string, string>("Assault rifle related stats", "Low") }},
        { SkillsEnum.AssaultRifles,     new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Assault rifle damage","High"), new KeyValuePair<string, string>("Assault rifle reload speed","High"), new KeyValuePair<string, string>("Assault rifle accuracy","High"), new KeyValuePair<string, string>("Precision rifle related stats", "Low") }},
        { SkillsEnum.SubmachineGuns,    new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Submachinegun damage","High"), new KeyValuePair<string, string>("Submachinegun reload speed","High"), new KeyValuePair<string, string>("Submachinegun accuracy","High"), new KeyValuePair<string, string>("Handgun related stats", "Low") }},
        { SkillsEnum.HeavyWeapons,      new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Heavy weapon damage","High"), new KeyValuePair<string, string>("Heavy weapon reload speed","High"), new KeyValuePair<string, string>("Heavy weapon accuracy","High"), new KeyValuePair<string, string>("Explosives related stats", "Low") }},
        { SkillsEnum.ExplosiveWeaponry, new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Explosives damage","High"), new KeyValuePair<string, string>("Explosives reload speed","High"), new KeyValuePair<string, string>("Explosives accuracy","High"), new KeyValuePair<string, string>("Heavy weapon related stats", "Low") }},
        { SkillsEnum.Bows,              new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Bow damage","High"), new KeyValuePair<string, string>("Bow attack speed","High"), new KeyValuePair<string, string>("Bow accuracy","High") }},

        { SkillsEnum.Athletics,         new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Running speed","High"), new KeyValuePair<string, string>("Swimming speed","High") }},
        { SkillsEnum.Acrobatics,        new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Climbing speed","High"), new KeyValuePair<string, string>("Climbing success","High") }},

        { SkillsEnum.LightArmour,       new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Light armour encumbrance","High") }},
        { SkillsEnum.MediumArmour,      new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Medium armour encumbrance","High") }},
        { SkillsEnum.HeavyArmour,       new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Heavy armour encumbrance","High") }},
        { SkillsEnum.Dodge,             new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Dodge chance","High"), new KeyValuePair<string, string>("Dodge recovery speed","High") }},
        { SkillsEnum.Block,             new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Block chance","high"), new KeyValuePair<string, string>("Block recovery speed","High") }},

        { SkillsEnum.ArmourSmithing,    new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Armour crafting quality","High"), new KeyValuePair<string, string>("Armour crafting speed","High"), new KeyValuePair<string, string>("Armour crafting cost","Moderate") }},
        { SkillsEnum.WeaponSmithing,    new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Weapon crafting quality","High"), new KeyValuePair<string, string>("Weapon crafting speed","High"), new KeyValuePair<string, string>("Weapon crafting cost","Moderate") }},
        { SkillsEnum.Construction,      new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Build speed","High"), new KeyValuePair<string, string>("Build quality","High"), new KeyValuePair<string, string>("Build cost","Moderate") }},

        { SkillsEnum.Research,          new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Research speed","High"), new KeyValuePair<string, string>("Research level","High") }},
        { SkillsEnum.Hacking,           new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Hacking speed","High"), new KeyValuePair<string, string>("Hacking success chance","High"), new KeyValuePair<string, string>("Hacking critical fail chance","High") }},
        { SkillsEnum.Medical,           new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Medical care speed","High"), new KeyValuePair<string, string>("Medical care quality","High"), new KeyValuePair<string, string>("Surgery success chance","High"), new KeyValuePair<string, string>("Surgery critical fail chance","High") }},

        { SkillsEnum.Stealth,           new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Max sneak speed","High"), new KeyValuePair<string, string>("Noise created","High"), new KeyValuePair<string, string>("Detection range","High") }},
        { SkillsEnum.Thievery,          new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Thieving speed","High"), new KeyValuePair<string, string>("Thievery sound created","High") }},

        { SkillsEnum.Persuasion,        new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Store prices","Moderate"), new KeyValuePair<string, string>("Unlocks potential actions","") }},
        { SkillsEnum.Intimidation,      new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Store prices","Low"), new KeyValuePair<string, string>("Unlocks potential actions","") }},

        { SkillsEnum.Psionics,          new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("Psionics cooldown","High"), new KeyValuePair<string, string>("Psionic energy generation","High"), new KeyValuePair<string, string>("Psionic ability power","High") }}
    };
}
