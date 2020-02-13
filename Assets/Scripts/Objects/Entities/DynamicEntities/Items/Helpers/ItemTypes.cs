using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modules;
using Items;
using static StatsAndSkills;

public class ItemTypes : MonoBehaviour
{
    public static GameObject ItemBasePrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Objects/Generic/ItemBase");

    public struct BonusInfoStruct
    {
        public StatsAndSkills.StatsEnum PrimaryStat;
        public StatsAndSkills.StatsEnum SecondaryStat;
        public BonusStruct[] Bonuses;

        public BonusInfoStruct(StatsAndSkills.StatsEnum PrimaryStat, StatsAndSkills.StatsEnum SecondaryStat, BonusStruct[] Bonuses)
        {
            this.PrimaryStat = PrimaryStat;
            this.SecondaryStat = SecondaryStat;
            this.Bonuses = Bonuses;
        }
    }

    public struct BonusStruct
    {
        public StatsEnum Stat; //The relevant item stat to modify
        public SkillsEnum Skill; //The skill to modify the stat with
        public float Coefficient; //How much to multiply the skill value with

        public BonusStruct(StatsEnum Stat, SkillsEnum Skill, float Coefficient)
        {
            this.Stat = Stat;
            this.Skill = Skill;
            this.Coefficient = Coefficient;
        }
    }

    [Flags]
    public enum Types
    {
        Resource,
        [Type(typeof(Head))]
        Head,
        [Type(typeof(Torso))]
        Torso,
        [Type(typeof(Legs))]
        Legs,
        [Type(typeof(Feet))]
        Feet,
        [Type(typeof(Arms))]
        Arms,
        //TODO Implement back and accessory slots
        //[Type(typeof(Back))]
        Back,
        //[Type(typeof(Accessory))]
        Accessory,
        [Type(typeof(Melee))]
        Melee,
        [Type(typeof(Ranged))]
        Bow,
        [Type(typeof(Firearm))]
        Firearm
    }

    public enum SubTypes
    {
        Invalid,

        Sword,
        Polearm,
        Quarterstaff,
        Dagger,
        Hammer,

        Handgun,
        PrecisionRifle,
        AssaultRifle,
        SubmachineGun,
        HeavyWeapon,
        ExplosiveWeapon,
        Bow,

        LightArmour,
        MediumArmour,
        HeavyArmour
    }

    public enum StatsEnum
    {
        //Weapon Stats
        Damage,
        AttackSpeed,
        Accuracy,
        Block,
        ArmourPiercing,
        Range,

        //Defense and Utility Stats
        Armour,
        Power,
        PowerUse,
        Shield,

        //Consumable Stats
        Quantity,
        MaxQuantity,
        Quality,

        //General Stats
        Size,
        Mass,
        Cost,

        //Module Specific
        SizeMod,
        MassMod,
        //Modifiable
        Thickness,
        Material,
        FireMode,
        Length
    }

    [Flags]
    public enum ItemFlagsEnum
    {
        Armour = 1 << 0,
        Weapon = 1 << 1,
        Melee = 1 << 2,
        Ranged = 1 << 3,
        Consumable = 1 << 4
    }
    
    public static Dictionary<SubTypes, BonusInfoStruct> EquipmentBonusInfo = new Dictionary<SubTypes, BonusInfoStruct>()
    {
        //Melee
        { SubTypes.Sword, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Swords, 0.5f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Swords, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Swords, 2.0f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Swords, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Daggers, 0.05f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Daggers, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Daggers, 0.2f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Daggers, 0.2f)
        })},
        { SubTypes.Polearm, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Polearms, 0.5f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Polearms, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Polearms, 2.0f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Polearms, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Quarterstaffs, 0.05f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Quarterstaffs, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Quarterstaffs, 0.2f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Quarterstaffs, 0.2f)
        })},
        { SubTypes.Quarterstaff, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Quarterstaffs, 0.5f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Quarterstaffs, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Quarterstaffs, 2.0f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Quarterstaffs, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Polearms, 0.05f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Polearms, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Polearms, 0.2f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Polearms, 0.2f)
        })},
        { SubTypes.Dagger, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Daggers, 0.5f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Daggers, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Daggers, 2.0f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Daggers, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Swords, 0.05f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Swords, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Swords, 0.2f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Swords, 0.2f)
        })},
        { SubTypes.Hammer, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Hammers, 0.5f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Hammers, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Hammers, 2.0f),
            new BonusStruct(StatsEnum.Block, SkillsEnum.Hammers, 2.0f)
        })},

        //Ranged
        { SubTypes.Handgun, new BonusInfoStruct(StatsAndSkills.StatsEnum.Perception, StatsAndSkills.StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Handguns, 0.5f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.Handguns, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Handguns, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.SubmachineGuns, 0.05f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.SubmachineGuns, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.SubmachineGuns, 0.2f),
        })},
        { SubTypes.PrecisionRifle, new BonusInfoStruct(StatsAndSkills.StatsEnum.Perception, StatsAndSkills.StatsEnum.Endurance,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.PrecisionRifles, 0.5f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.PrecisionRifles, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.PrecisionRifles, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.AssaultRifles, 0.05f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.AssaultRifles, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.AssaultRifles, 0.2f),
        })},
        { SubTypes.AssaultRifle, new BonusInfoStruct(StatsAndSkills.StatsEnum.Perception, StatsAndSkills.StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.AssaultRifles, 0.5f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.AssaultRifles, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.AssaultRifles, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.PrecisionRifles, 0.05f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.PrecisionRifles, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.PrecisionRifles, 0.2f),
        })},
        { SubTypes.SubmachineGun, new BonusInfoStruct(StatsAndSkills.StatsEnum.Perception, StatsAndSkills.StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.SubmachineGuns, 0.5f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.SubmachineGuns, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.SubmachineGuns, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Handguns, 0.05f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.Handguns, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Handguns, 0.2f),
        })},
        { SubTypes.HeavyWeapon, new BonusInfoStruct(StatsAndSkills.StatsEnum.Strength, StatsAndSkills.StatsEnum.Perception,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.HeavyWeapons, 0.5f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.HeavyWeapons, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.HeavyWeapons, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.ExplosiveWeaponry, 0.05f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.ExplosiveWeaponry, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.ExplosiveWeaponry, 0.2f),
        })},
        { SubTypes.ExplosiveWeapon, new BonusInfoStruct(StatsAndSkills.StatsEnum.Perception, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.ExplosiveWeaponry, 0.5f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.ExplosiveWeaponry, 0.5f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.ExplosiveWeaponry, 2.0f),
            new BonusStruct(StatsEnum.Damage, SkillsEnum.HeavyWeapons, 0.05f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.HeavyWeapons, 0.05f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.HeavyWeapons, 0.2f),
        })},
        { SubTypes.Bow, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Perception,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Damage, SkillsEnum.Bows, 1.0f),
            new BonusStruct(StatsEnum.AttackSpeed, SkillsEnum.Bows, 2.0f),
            new BonusStruct(StatsEnum.Range, SkillsEnum.Bows, 2.0f),
            new BonusStruct(StatsEnum.Accuracy, SkillsEnum.Bows, 4.0f),
        })},

        //Armours
        { SubTypes.LightArmour, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Armour, SkillsEnum.LightArmour, 0.1f),
            new BonusStruct(StatsEnum.Mass, SkillsEnum.LightArmour, -0.2f),
        })},
        { SubTypes.MediumArmour, new BonusInfoStruct(StatsAndSkills.StatsEnum.Dexterity, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Armour, SkillsEnum.MediumArmour, 0.1f),
            new BonusStruct(StatsEnum.Mass, SkillsEnum.MediumArmour, -0.2f),
        })},
        { SubTypes.HeavyArmour, new BonusInfoStruct(StatsAndSkills.StatsEnum.Strength, StatsAndSkills.StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatsEnum.Armour, SkillsEnum.HeavyArmour, 0.1f),
            new BonusStruct(StatsEnum.Mass, SkillsEnum.HeavyArmour, -0.2f),
        })}
    };
}
