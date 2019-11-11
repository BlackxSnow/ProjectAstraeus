using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Modules;
using static ItemTypes;
using static Modules.AdditionalModule;


namespace Items
{
    public class Melee : Weapon
    {
        public override void Init()
        {
            base.Init();
            Stats.AddStat(StatsEnum.Block, 0f);
            BaseStats.Stats.Add(StatsEnum.Block, 0f);

            BaseStats.CompatibleModules = new List<ModulesEnum>()
            {
                ModulesEnum.Handle,
                ModulesEnum.Blade,
                ModulesEnum.Head
            };
                BaseStats.RequiredModules = new List<ModulesEnum>()
            {
                ModulesEnum.Handle
            };
        }

        public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null)
        {
            List<GameObject> KVPs = new List<GameObject>();
            List<GameObject> KVPLists = new List<GameObject>();
            List<UIController.KVPData> KVPDatas = new List<UIController.KVPData>();

            KVPDatas.Add(new UIController.KVPData(StatsEnum.Block.ToString(), Stats.GetStat<float>(StatsEnum.Block), Parent, 1));

            KVPDatas[0].ValueEnum = StatsEnum.Block;

            foreach (UIController.KVPData Data in KVPDatas)
            {
                Data.RefItem = this;
                Data.ValueDelegate = KeyValuePanel.GetItemStat;
                Data.Group = Group;
                KVPs.Add(UIController.InstantiateKVP(Data));
            }

            List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

            List<GameObject> CombinedKVPs = Utility.CombineLists(KVPs, BaseKVPs);
            CombinedKVPLists = Utility.CombineLists(KVPLists, BaseKVPLists);

            return CombinedKVPs;
        }

        public override void SetStats()
        {
            Vector2 SizeMod = new Vector2(1, 1);
            float MassMod = 1;
            Resources ResourceMod = new Resources(0, 0, 0);
            Stats.Stats = new Dictionary<StatsEnum, object>(BaseStats.Stats);

            Dictionary<StatsEnum, float> StatMods = new Dictionary<StatsEnum, float>()
        {
            { StatsEnum.Damage, 1f },
            { StatsEnum.Block, 1f },
            { StatsEnum.ArmourPiercing, 1f },
            { StatsEnum.AttackSpeed, 1f }
        };

            foreach (AdditionalModule Module in Modules)
            {
                SizeMod += Module.GetStat<Vector2>(StatsEnum.SizeMod) - new Vector2(1f, 1f);
                MassMod += Module.GetStat<float>(StatsEnum.MassMod) - 1f;
                ResourceMod += Module.GetStat<Resources>(StatsEnum.Cost);

                foreach (KeyValuePair<StatsEnum, object> Stat in Module.Stats)
                {
                    if (!Stats.Stats.TryGetValue(Stat.Key, out object Value) || Stat.Key == StatsEnum.Cost)
                    {
                        continue;
                    }
                    if (StatMods[Stat.Key] == 1f)
                    {
                        StatMods[Stat.Key] = 0f;
                    }
                    StatMods[Stat.Key] += (float)Stat.Value;
                }
            }
            foreach (KeyValuePair<StatsEnum, float> StatMod in StatMods)
            {
                Stats.SetStat(StatMod.Key, StatMod.Value, ItemStats.OperationEnum.Multiply);
            }

            Stats.SetStat(StatsEnum.Size, new Vector2Int(Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).x * SizeMod.x), Mathf.RoundToInt(BaseStats.GetStat<Vector2Int>(StatsEnum.Size).y * SizeMod.y)));
            Stats.SetStat(StatsEnum.Mass, BaseStats.GetStat<float>(StatsEnum.Mass) * MassMod);
            Stats.SetStat(StatsEnum.Cost, BaseStats.GetStat<Resources>(StatsEnum.Cost) + ResourceMod);
            FindSubtype();
        }

        public override void FindSubtype()
        {
            Handle handle = (Handle)Modules.Where(M => M is Handle).ElementAtOrDefault(0);
            Blade blade = (Blade)Modules.Where(M => M is Blade).ElementAtOrDefault(0);
            MaulHead head = (MaulHead)Modules.Where(M => M is MaulHead).ElementAtOrDefault(0);

            if (handle != null)
            {
                float handleLength = handle.GetStat<float>(StatsEnum.Length);
                if (head != null) //Handle, head. Done before blade, as if it has a head it is always a hammer
                {
                    Subtype = SubTypes.Hammer;
                }
                else if (blade != null) //Handle, blade
                {
                    float LengthLeniency = 0.1f;
                    float bladeLength = blade.GetStat<float>(StatsEnum.Length);
                    if (handleLength > bladeLength + LengthLeniency) //Polearm
                    {
                        Subtype = SubTypes.Polearm;
                    }
                    else if (bladeLength > handleLength + LengthLeniency) //Sword 
                    {
                        Subtype = SubTypes.Sword;
                    }
                    else
                    {
                        Subtype = SubTypes.Dagger;
                    }
                }
                else //Handle, no blade
                {
                    Subtype = SubTypes.Quarterstaff;
                }
            }
            else
            {
                Subtype = SubTypes.Invalid;
            }
        }
    } 
}
