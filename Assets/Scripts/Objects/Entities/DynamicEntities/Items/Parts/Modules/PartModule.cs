using Items.Parts;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static ItemTypes;


namespace Items.Modules
{
    public class PartModule
    {
        public enum ModuleGroups
        {
            Offensive,
            Defensive,
            Utility
        }

        public struct ModuleModifiableStatsToggle
        {
            public bool SizeX;
            public bool SizeY;

            public bool Strength;
        }
        public struct ModuleModifiableStats
        {
            public float SizeX;
            public float SizeY;

            public float Strength;
        }

        #region Serialisation data
        public List<string> ValidGroups_S;
        #endregion

        #region Object data
        public List<ModuleGroups> ValidGroups;
        public ItemPart.ModularStats BaseModuleStats;
        public ModuleModifiableStatsToggle ModifiableStatsToggle;
        #endregion

        #region Runtime data
        public ItemPart ParentPart;
        public ItemPart.ModularStats ModuleStats;
        public ModuleModifiableStats ModifiableStats;
        #endregion



        [OnDeserialized]
        public void OnDeserialised(StreamingContext context)
        {
            ValidGroups = Utility.Collections.DeserializeEnumCollection<ModuleGroups>(ValidGroups_S);
            ValidGroups_S.Clear();
        }
    }


}
