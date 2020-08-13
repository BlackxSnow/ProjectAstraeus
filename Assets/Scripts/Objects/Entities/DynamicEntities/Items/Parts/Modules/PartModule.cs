using Items.Parts;
using System.Collections.Generic;


namespace Items.Modules
{
    public abstract class PartModule
    {
        public enum ModuleGroups
        {
            Offensive,
            Defensive,
            Utility
        }

        #region Object data
        public abstract List<ModuleGroups> ValidGroups { get; protected set; }
        public abstract ItemPart.ModularStats BaseModuleStats { get; }
        #endregion

        #region Runtime data
        public ItemPart ParentPart { get; set; }
        public ItemPart.ModularStats ModuleStats { get; protected set; }
        public abstract Dictionary<string, ModifiableStat> ModifiableStats { get; protected set; }
        #endregion

        
        /// <summary>
        /// Sets ModuleStats based on ModifiableStats and BaseModuleStats values
        /// </summary>
        public abstract void CalculateStats(); 
    }


}
