using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Modules.AdditionalModule;


public class Head : Armour
{
    public override void Init()
    {
        base.Init();

        BaseStats = new BaseItemStats()
        {
            Stats = new Dictionary<StatsEnum, object>()
            {
                { StatsEnum.Armour, 1f },
                { StatsEnum.Shield, 0f },
                { StatsEnum.Power, 0f },
                { StatsEnum.PowerUse, 0f },
                { StatsEnum.Size, new Vector2Int(3,3) },
                { StatsEnum.Mass, 0.5f },
                { StatsEnum.Cost, new Resources(1,0,0) }
            },
            CompatibleModules = new List<ModulesEnum>()
            {
                ModulesEnum.Plating,
                ModulesEnum.Reactor,
                ModulesEnum.Shield
            }
        };
        //Stats.Stats = new Dictionary<StatsEnum, object>(BaseStats.Stats);
        Slot = Equipment.Slots.Head;
    }

    public override void FindSubtype()
    {
        SubTypes[] ThresholdTypes = new SubTypes[3]
        {
            SubTypes.LightArmour,
            SubTypes.MediumArmour,
            SubTypes.HeavyArmour
        };
        float[] MassThreshold = new float[3]
        {
            0f, //Light
            2.5f, //Medium
            5f //Heavy
        };
        float Mass = Stats.GetStat<float>(StatsEnum.Mass);

        Subtype = ThresholdTypes[Utility.GetThreshold(Mass, MassThreshold)];
    }
}
