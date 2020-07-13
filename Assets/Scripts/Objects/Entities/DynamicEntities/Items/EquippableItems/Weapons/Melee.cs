using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Modules;
using static ItemTypes;
using static Modules.AdditionalModule;
using Medical;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;
using AI.States;
using UI.Control;

namespace Items
{
    public class Melee : Weapon
    {
        public override void Init()
        {
            base.Init();
            Stats.AddStat(StatsEnum.Block, 0f);
            BaseStats.Stats.Add(StatsEnum.Block, 0f);
            Stats.AddStat(StatsEnum.Range, 1f);
            BaseStats.Stats.Add(StatsEnum.Range, 1f);

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

            AttackFunctions = new AttackData
            {
                SpeedFunctions = new Dictionary<Health.PartFunctions, float>
                {
                    { Health.PartFunctions.Manipulation, 1.0f }
                },
                HitFunctions = new Dictionary<Health.PartFunctions, float>
                {
                    { Health.PartFunctions.Manipulation, 1.5f },
                    { Health.PartFunctions.Vision, 1.0f }
                },

                DamageFunctions = new Dictionary<Health.PartFunctions, float>
                {
                    { Health.PartFunctions.Manipulation, 2.0f }
                }
            };
        }




        public override List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> CombinedKVPLists, Transform Parent, KeyValueGroup Group = null) 
        {
            List<GameObject> KVPs = new List<GameObject>();
            List<GameObject> KVPLists = new List<GameObject>();
            List<CreateUI.KVPData> KVPDatas = new List<CreateUI.KVPData>();

            KVPDatas.Add(new CreateUI.KVPData(StatsEnum.Block.ToString(), Stats.GetStat<float>(StatsEnum.Block), Parent, 1));

            KVPDatas[0].ValueEnum = StatsEnum.Block;

            foreach (CreateUI.KVPData Data in KVPDatas)
            {
                Data.RefItem = this;
                Data.ValueDelegate = KeyValuePanel.GetItemStat;
                Data.Group = Group;
                KVPs.Add(CreateUI.Info.KeyValuePanel(Data));
            }

            List<GameObject> BaseKVPs = base.InstantiateStatKVPs(Cost, out List<GameObject> BaseKVPLists, Parent, Group);

            List<GameObject> CombinedKVPs = Utility.Collections.CombineLists(KVPs, BaseKVPs);
            CombinedKVPLists = Utility.Collections.CombineLists(KVPLists, BaseKVPLists);

            return CombinedKVPs;
        }

        public override void CalculateStats()
        {
            Stats.Stats = new Dictionary<StatsEnum, object>(BaseStats.Stats);

            Dictionary<StatsEnum, float> StatMods = new Dictionary<StatsEnum, float>()
            {
                { StatsEnum.Damage, 1f },
                { StatsEnum.Block, 1f },
                { StatsEnum.ArmourPiercing, 1f },
                { StatsEnum.AttackSpeed, 1f },
                { StatsEnum.Range, 1f },
            };

            SetStats(StatMods);
        }

        public struct ThresholdStruct
        {
            public float Leniency;
            public float Dagger;
            public float Sword;
        }
        public static ThresholdStruct MaxLengths = new ThresholdStruct()
        {
            Leniency = 0.1f,
            Dagger = 0.35f,
            Sword = 0
        };

        public override bool FindSubtype()
        {
            SubTypes OldType = Subtype;
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
                    float bladeLength = blade.GetStat<float>(StatsEnum.Length);
                    float TotalLength = bladeLength + handleLength;
                    if (TotalLength <= MaxLengths.Dagger) //Short weapon
                    {
                        Subtype = SubTypes.Dagger;
                    }
                    else if (handleLength > bladeLength + MaxLengths.Leniency) //long handle, shorter blade
                    {
                        Subtype = SubTypes.Polearm;
                    }
                    else 
                    {
                        Subtype = SubTypes.Sword;
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
            if (Subtype != OldType)
            {
                return true;
            } else
            {
                return false;
            }
        }
    } 
}
