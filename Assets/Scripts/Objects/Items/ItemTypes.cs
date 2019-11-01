using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemModule.AdditionalModule;
using static StatsAndSkills;

public class ItemTypes : MonoBehaviour
{
    public static GameObject ItemBasePrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/Objects/Generic/ItemBase");

    public struct BonusInfoStruct
    {
        public StatsEnum PrimaryStat;
        public StatsEnum SecondaryStat;
        public BonusStruct[] Bonuses;

        public BonusInfoStruct(StatsEnum PrimaryStat, StatsEnum SecondaryStat, BonusStruct[] Bonuses)
        {
            this.PrimaryStat = PrimaryStat;
            this.SecondaryStat = SecondaryStat;
            this.Bonuses = Bonuses;
        }
    }

    public struct BonusStruct
    {
        public StatFlags Stat; //The relevant item stat to modify
        public SkillsEnum Skill; //The skill to modify the stat with
        public float Coefficient; //How much to multiply the skill value with

        public BonusStruct(StatFlags Stat, SkillsEnum Skill, float Coefficient)
        {
            this.Stat = Stat;
            this.Skill = Skill;
            this.Coefficient = Coefficient;
        }
    }

    public enum Types
    {
        Resource,
        Head,
        Torso,
        Legs,
        Feet,
        Arms,
        Back,
        Accessory,
        MeleeSharp,
        MeleeBlunt,
        RangedBow,
        RangedFirearm
    }

    public enum SubTypes
    {
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

    [Flags]
    public enum StatFlags
    {
        Damage = 1 << 0,
        AttackSpeed = 1 << 1,
        Accuracy = 1 << 2,
        Block = 1 << 3,
        ArmourPiercing = 1 << 4,
        Range = 1 << 5,

        Armour = 1 << 6,
        Power = 1 << 7,
        PowerUse = 1 << 8,
        Shield = 1 << 9,

        Size = 1 << 10,
        Mass = 1 << 11,
        Cost = 1 << 12
    }

    [Flags]
    public enum ItemFlags
    {
        Armour = 1 << 0,
        Weapon = 1 << 1,
        Melee = 1 << 2,
        Ranged = 1 << 3,
        Consumable = 1 << 4
    }

    public static Dictionary<ItemFlags, StatFlags> FlagStats = new Dictionary<ItemFlags, StatFlags>()
    {
        { ItemFlags.Armour,     StatFlags.Size | StatFlags.Cost | StatFlags.Mass | StatFlags.Armour         | StatFlags.Power       | StatFlags.Shield | StatFlags.PowerUse                   },
        { ItemFlags.Consumable, StatFlags.Size | StatFlags.Cost | StatFlags.Mass                                                                                                              },
        { ItemFlags.Melee,      StatFlags.Size | StatFlags.Cost | StatFlags.Mass | StatFlags.ArmourPiercing | StatFlags.AttackSpeed | StatFlags.Damage | StatFlags.PowerUse                   },
        { ItemFlags.Ranged,     StatFlags.Size | StatFlags.Cost | StatFlags.Mass | StatFlags.ArmourPiercing | StatFlags.AttackSpeed | StatFlags.Damage | StatFlags.PowerUse | StatFlags.Range }
    };

    public static Dictionary<Types, ItemModule.CoreModule> TypeCores = new Dictionary<Types, ItemModule.CoreModule>()
    {
        { Types.Head,          new ItemModule.CoreModule(   new Vector2Int(3, 5),   0.5f,   new Resources(1,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.Head    ) },
        { Types.Torso,         new ItemModule.CoreModule(   new Vector2Int(7,10),   1.0f,   new Resources(2,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.Torso   ) },
        { Types.Legs,          new ItemModule.CoreModule(   new Vector2Int(6,15),   1.0f,   new Resources(2,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.Legs    ) },
        { Types.Feet,          new ItemModule.CoreModule(   new Vector2Int(3, 5),   0.5f,   new Resources(1,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.Feet    ) },
        { Types.Arms,          new ItemModule.CoreModule(   new Vector2Int(2, 5),   0.5f,   new Resources(1,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.Arms    ) },
        { Types.Back,          new ItemModule.CoreModule(   new Vector2Int(4,10),   1.0f,   new Resources(2,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.Back    ) },
        { Types.Accessory,     new ItemModule.CoreModule(   new Vector2Int(1, 1),   0.1f,   new Resources(1,0,0),   (ItemFlags.Armour                   ),    (ModuleList.Plating | ModuleList.Reactor | ModuleList.Shielding), Equipment.Slots.None    ) },
        { Types.MeleeSharp,    new ItemModule.CoreModule(   new Vector2Int(4, 2),   0.5f,   new Resources(1,0,0),   (ItemFlags.Melee  | ItemFlags.Weapon),    (ModuleList.WeaponPlaceHolderModule                            ), Equipment.Slots.Weapon  ) },
        { Types.MeleeBlunt,    new ItemModule.CoreModule(   new Vector2Int(4, 2),   1.0f,   new Resources(1,0,0),   (ItemFlags.Melee  | ItemFlags.Weapon),    (ModuleList.WeaponPlaceHolderModule                            ), Equipment.Slots.Weapon  ) },
        { Types.RangedBow,     new ItemModule.CoreModule(   new Vector2Int(4, 5),   0.5f,   new Resources(1,0,0),   (ItemFlags.Ranged | ItemFlags.Weapon),    (ModuleList.WeaponPlaceHolderModule                            ), Equipment.Slots.Weapon  ) },
        { Types.RangedFirearm, new ItemModule.CoreModule(   new Vector2Int(5, 4),   1.0f,   new Resources(1,0,0),   (ItemFlags.Ranged | ItemFlags.Weapon),    (ModuleList.WeaponPlaceHolderModule                            ), Equipment.Slots.Weapon  ) }
    };
    
    public static Dictionary<SubTypes, BonusInfoStruct> EquipmentBonusInfo = new Dictionary<SubTypes, BonusInfoStruct>()
    {
        //Melee
        { SubTypes.Sword, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Swords, 0.5f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Swords, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Swords, 2.0f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Swords, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.Daggers, 0.05f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Daggers, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Daggers, 0.2f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Daggers, 0.2f)
        })},
        { SubTypes.Polearm, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Polearms, 0.5f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Polearms, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Polearms, 2.0f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Polearms, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.Quarterstaffs, 0.05f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Quarterstaffs, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Quarterstaffs, 0.2f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Quarterstaffs, 0.2f)
        })},
        { SubTypes.Quarterstaff, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Quarterstaffs, 0.5f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Quarterstaffs, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Quarterstaffs, 2.0f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Quarterstaffs, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.Polearms, 0.05f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Polearms, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Polearms, 0.2f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Polearms, 0.2f)
        })},
        { SubTypes.Dagger, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Daggers, 0.5f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Daggers, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Daggers, 2.0f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Daggers, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.Swords, 0.05f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Swords, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Swords, 0.2f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Swords, 0.2f)
        })},
        { SubTypes.Hammer, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Hammers, 0.5f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Hammers, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Hammers, 2.0f),
            new BonusStruct(StatFlags.Block, SkillsEnum.Hammers, 2.0f)
        })},

        //Ranged
        { SubTypes.Handgun, new BonusInfoStruct(StatsEnum.Perception, StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Handguns, 0.5f),
            new BonusStruct(StatFlags.Range, SkillsEnum.Handguns, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Handguns, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.SubmachineGuns, 0.05f),
            new BonusStruct(StatFlags.Range, SkillsEnum.SubmachineGuns, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.SubmachineGuns, 0.2f),
        })},
        { SubTypes.PrecisionRifle, new BonusInfoStruct(StatsEnum.Perception, StatsEnum.Endurance,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.PrecisionRifles, 0.5f),
            new BonusStruct(StatFlags.Range, SkillsEnum.PrecisionRifles, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.PrecisionRifles, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.AssaultRifles, 0.05f),
            new BonusStruct(StatFlags.Range, SkillsEnum.AssaultRifles, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.AssaultRifles, 0.2f),
        })},
        { SubTypes.AssaultRifle, new BonusInfoStruct(StatsEnum.Perception, StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.AssaultRifles, 0.5f),
            new BonusStruct(StatFlags.Range, SkillsEnum.AssaultRifles, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.AssaultRifles, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.PrecisionRifles, 0.05f),
            new BonusStruct(StatFlags.Range, SkillsEnum.PrecisionRifles, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.PrecisionRifles, 0.2f),
        })},
        { SubTypes.SubmachineGun, new BonusInfoStruct(StatsEnum.Perception, StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.SubmachineGuns, 0.5f),
            new BonusStruct(StatFlags.Range, SkillsEnum.SubmachineGuns, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.SubmachineGuns, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.Handguns, 0.05f),
            new BonusStruct(StatFlags.Range, SkillsEnum.Handguns, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Handguns, 0.2f),
        })},
        { SubTypes.HeavyWeapon, new BonusInfoStruct(StatsEnum.Strength, StatsEnum.Perception,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.HeavyWeapons, 0.5f),
            new BonusStruct(StatFlags.Range, SkillsEnum.HeavyWeapons, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.HeavyWeapons, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.ExplosiveWeaponry, 0.05f),
            new BonusStruct(StatFlags.Range, SkillsEnum.ExplosiveWeaponry, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.ExplosiveWeaponry, 0.2f),
        })},
        { SubTypes.ExplosiveWeapon, new BonusInfoStruct(StatsEnum.Perception, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.ExplosiveWeaponry, 0.5f),
            new BonusStruct(StatFlags.Range, SkillsEnum.ExplosiveWeaponry, 0.5f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.ExplosiveWeaponry, 2.0f),
            new BonusStruct(StatFlags.Damage, SkillsEnum.HeavyWeapons, 0.05f),
            new BonusStruct(StatFlags.Range, SkillsEnum.HeavyWeapons, 0.05f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.HeavyWeapons, 0.2f),
        })},
        { SubTypes.Bow, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Perception,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Damage, SkillsEnum.Bows, 1.0f),
            new BonusStruct(StatFlags.AttackSpeed, SkillsEnum.Bows, 2.0f),
            new BonusStruct(StatFlags.Range, SkillsEnum.Bows, 2.0f),
            new BonusStruct(StatFlags.Accuracy, SkillsEnum.Bows, 4.0f),
        })},

        //Armours
        { SubTypes.LightArmour, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Dexterity,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Armour, SkillsEnum.LightArmour, 0.1f),
            new BonusStruct(StatFlags.Mass, SkillsEnum.LightArmour, -0.2f),
        })},
        { SubTypes.MediumArmour, new BonusInfoStruct(StatsEnum.Dexterity, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Armour, SkillsEnum.MediumArmour, 0.1f),
            new BonusStruct(StatFlags.Mass, SkillsEnum.MediumArmour, -0.2f),
        })},
        { SubTypes.HeavyArmour, new BonusInfoStruct(StatsEnum.Strength, StatsEnum.Strength,
        new BonusStruct[]
        {
            new BonusStruct(StatFlags.Armour, SkillsEnum.HeavyArmour, 0.1f),
            new BonusStruct(StatFlags.Mass, SkillsEnum.HeavyArmour, -0.2f),
        })}
    };
}
