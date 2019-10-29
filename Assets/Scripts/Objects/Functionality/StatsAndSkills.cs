using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsAndSkills : MonoBehaviour
{
    public struct StatSkill
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
        public float[] LevelThresholds;


        public StatSkill(SkillTypes SkillType = SkillTypes.None, bool Enabled = true)
        {
            this.SkillType = SkillType;
            this.Enabled = Enabled;
            Level = 1;
            Experience = 0;
            LevelThresholds = new float[100];
            for (int i = 0; i < LevelThresholds.Length; i++)
            {
                LevelThresholds[i] = Mathf.Pow(i, 2f);
            }
        }
    }

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
        { StatsEnum.Strength, new StatSkill(StatSkill.SkillTypes.None) },
        { StatsEnum.Dexterity, new StatSkill(StatSkill.SkillTypes.None) },
        { StatsEnum.Endurance, new StatSkill(StatSkill.SkillTypes.None) },
        { StatsEnum.Intelligence, new StatSkill(StatSkill.SkillTypes.None) },
        { StatsEnum.Perception, new StatSkill(StatSkill.SkillTypes.None) },
        { StatsEnum.Charisma, new StatSkill(StatSkill.SkillTypes.None) }
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
        { SkillsEnum.Swords,            new StatSkill(StatSkill.SkillTypes.Melee) },
        { SkillsEnum.Polearms,          new StatSkill(StatSkill.SkillTypes.Melee) },
        { SkillsEnum.Quarterstaffs,     new StatSkill(StatSkill.SkillTypes.Melee) },
        { SkillsEnum.Daggers,           new StatSkill(StatSkill.SkillTypes.Melee) },
        { SkillsEnum.Hammers,           new StatSkill(StatSkill.SkillTypes.Melee) },

        { SkillsEnum.Handguns,          new StatSkill(StatSkill.SkillTypes.Ranged) },
        { SkillsEnum.PrecisionRifles,   new StatSkill(StatSkill.SkillTypes.Ranged) },
        { SkillsEnum.AssaultRifles,     new StatSkill(StatSkill.SkillTypes.Ranged) },
        { SkillsEnum.SubmachineGuns,    new StatSkill(StatSkill.SkillTypes.Ranged) },
        { SkillsEnum.HeavyWeapons,      new StatSkill(StatSkill.SkillTypes.Ranged) },
        { SkillsEnum.ExplosiveWeaponry, new StatSkill(StatSkill.SkillTypes.Ranged) },
        { SkillsEnum.Bows,              new StatSkill(StatSkill.SkillTypes.Ranged) },

        { SkillsEnum.Athletics,         new StatSkill(StatSkill.SkillTypes.Movement) },
        { SkillsEnum.Acrobatics,        new StatSkill(StatSkill.SkillTypes.Movement) },

        { SkillsEnum.LightArmour,       new StatSkill(StatSkill.SkillTypes.Defensive) },
        { SkillsEnum.MediumArmour,      new StatSkill(StatSkill.SkillTypes.Defensive) },
        { SkillsEnum.HeavyArmour,       new StatSkill(StatSkill.SkillTypes.Defensive) },
        { SkillsEnum.Dodge,             new StatSkill(StatSkill.SkillTypes.Defensive) },
        { SkillsEnum.Block,             new StatSkill(StatSkill.SkillTypes.Defensive) },

        { SkillsEnum.ArmourSmithing,    new StatSkill(StatSkill.SkillTypes.Engineering) },
        { SkillsEnum.WeaponSmithing,    new StatSkill(StatSkill.SkillTypes.Engineering) },
        { SkillsEnum.Construction,      new StatSkill(StatSkill.SkillTypes.Engineering) },

        { SkillsEnum.Research,          new StatSkill(StatSkill.SkillTypes.Technology) },
        { SkillsEnum.Hacking,           new StatSkill(StatSkill.SkillTypes.Technology) },
        { SkillsEnum.Medical,           new StatSkill(StatSkill.SkillTypes.Technology) },

        { SkillsEnum.Stealth,           new StatSkill(StatSkill.SkillTypes.Covert) },
        { SkillsEnum.Thievery,          new StatSkill(StatSkill.SkillTypes.Covert) },

        { SkillsEnum.Persuasion,        new StatSkill(StatSkill.SkillTypes.Social) },
        { SkillsEnum.Intimidation,      new StatSkill(StatSkill.SkillTypes.Social) },

        { SkillsEnum.Psionics,          new StatSkill(StatSkill.SkillTypes.Special) }
    };
}
