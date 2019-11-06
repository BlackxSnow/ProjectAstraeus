using UnityEngine;
using static ItemTypes;


namespace Modules
{
    public class ItemModule : ScriptableObject
    {
        
    }
    //Can we scrap the idea of cores entirely in favour of data on Item.cs?
    public class CoreModule : ItemModule
    {
        public Vector2Int Size = new Vector2Int(1, 1);
        public float Mass = 1f;
        public Resources Cost = new Resources(0, 0, 0);
        public Equipment.Slots Slot;

        public CoreModule() { }
        public CoreModule(Vector2Int Size, float Mass, Resources Cost, ItemFlagsEnum ItemFlags, Equipment.Slots Slot)
        {
            this.Size = Size;
            this.Mass = Mass;
            this.Cost = Cost;
            this.Slot = Slot;
        }
    }


}
