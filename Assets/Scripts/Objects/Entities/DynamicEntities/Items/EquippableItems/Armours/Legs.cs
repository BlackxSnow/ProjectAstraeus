using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemTypes;
using static Modules.AdditionalModule;

public class Legs : Armour
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
    }
}
