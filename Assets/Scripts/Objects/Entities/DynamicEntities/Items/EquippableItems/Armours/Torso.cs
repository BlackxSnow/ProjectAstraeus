using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static ItemTypes;
using static Modules.AdditionalModule;

public class Torso : Armour
{
    public override void Init()
    {
        base.Init();

        BaseStats = new BaseItemStats()
        {
            Stats = new Dictionary<StatsEnum, object>()
            {
                { StatsEnum.Armour, 2f },
                { StatsEnum.Shield, 0f },
                { StatsEnum.Power, 0f },
                { StatsEnum.PowerUse, 0f },
                { StatsEnum.Size, new Vector2Int(6,9) },
                { StatsEnum.Mass, 1f },
                { StatsEnum.Cost, new Resources(2,0,0) }
            },
            CompatibleModules = new List<ModulesEnum>()
            {
                ModulesEnum.Plating,
                ModulesEnum.Reactor,
                ModulesEnum.Shield
            }
        };
        ValidSlots = new Equipment.Slots[] { Equipment.Slots.Torso };
    }

    public override bool FindSubtype()
    {
        SubTypes OldType = Subtype;
        SubTypes[] ThresholdTypes = new SubTypes[3]
        {
            SubTypes.LightArmour,
            SubTypes.MediumArmour,
            SubTypes.HeavyArmour
        };
        float[] MassThreshold = new float[3]
        {
            0f, //Light
            5f, //Medium
            10f //Heavy
        };
        float Mass = Stats.GetStat<float>(StatsEnum.Mass);

        Subtype = ThresholdTypes[Utility.GetThreshold(Mass, MassThreshold)];
        if (Subtype != OldType) return true; else return false;
    }
}
