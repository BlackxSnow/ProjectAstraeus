using Items.Parts;
using System;
using System.Collections.Generic;
using UI.Control;
using UnityEngine;

namespace Items
{
    public abstract class Item : DynamicEntity
    {
        [Serializable]
        public class DebugClass
        {
            public Item item;
            public bool DEBUG = false;
            public Vector2Int DEBUG_SIZE = new Vector2Int(1, 1);
            public float DEBUG_MASS = 1f;
            public Resources DEBUG_COST = new Resources(1, 1, 1);

            public void SetValues()
            {
                item.Stats.Size = DEBUG_SIZE;
                item.Stats.Mass = DEBUG_MASS;
                item.Stats.Cost = DEBUG_COST;
            }
        }
        public DebugClass DebugValues;


        public class ItemStats
        {
            public float Range;

            public Vector2 Sizef;
            public Vector2Int Size;
            public float Mass;
            public Resources Cost;
        }

        public abstract void CalculateStats();


        public virtual List<GameObject> InstantiateStatKVPs(bool Cost, out List<GameObject> KVPLists, Transform Parent, KeyValueGroup Group = null)
        {
            List<GameObject> KVPs = new List<GameObject>();
            KVPLists = new List<GameObject>();
            List<CreateUI.KVPData> KVPDatas = new List<CreateUI.KVPData>();

            CreateUI.KVPData TypeData = new CreateUI.KVPData("Item Type", Type, Parent)
            {
                Group = Group
            };

            KVPDatas.Add(new CreateUI.KVPData("Size", Stats.Size, Parent));
            KVPDatas.Add(new CreateUI.KVPData("Mass", Stats.Mass, Parent));

            KVPDatas[0].ValueDelegate = () => Stats.Size.ToString();
            KVPDatas[1].ValueDelegate = () => Utility.Misc.RoundedString(Stats.Mass);

            foreach (CreateUI.KVPData Data in KVPDatas)
            {
                Data.Group = Group;
                KVPs.Add(CreateUI.Info.KeyValuePanel(Data));
            }
            KVPs.Add(CreateUI.Info.KeyValuePanel(TypeData));


            if (Cost)
            {
                CreateUI.KVPData[] CostData = new CreateUI.KVPData[Resources.ResourceCount];
                CostData[0] = new CreateUI.KVPData("Iron", Stats.Cost[Resources.ResourceList.Iron], null);
                CostData[1] = new CreateUI.KVPData("Copper", Stats.Cost[Resources.ResourceList.Copper], null);
                CostData[2] = new CreateUI.KVPData("Alloy", Stats.Cost[Resources.ResourceList.Alloy], null);

                CostData[0].ValueDelegate = () => Stats.Cost[Resources.ResourceList.Iron].ToString();
                CostData[1].ValueDelegate = () => Stats.Cost[Resources.ResourceList.Copper].ToString();
                CostData[2].ValueDelegate = () => Stats.Cost[Resources.ResourceList.Alloy].ToString();

                foreach (CreateUI.KVPData Data in CostData)
                {
                    Data.Group = Group;
                }
                KVPLists.Add(CreateUI.Info.KVPList("Cost", CostData, Parent, Group));
            }

            return KVPs;
        }

        public string ItemName;
        public ItemStats Stats = new ItemStats();
        public Inventory Container;

        //Modular Parts
        public ItemPart CorePart;

        //Item 'definitions'
        public ItemTypes.Types Type;

        //----------------------------------------------------

        public override void Init()
        {
            base.Init();
            EntityType = EntityTypes.Item;
            if (DebugValues != null && DebugValues.DEBUG) DebugValues.SetValues();
        }

        protected override void Start()
        {
            base.Start();
            EntityType = EntityTypes.Item;
            EntityManager.RegisterItem(this);
        }

        //Functions for moving items from world to inventory and vice versa
        #region Item handling
        public void Pack()
        {
            rendererComponent.enabled = false;
            colliderComponent.enabled = false;
        }
        public void Unpack()
        {
            rendererComponent.enabled = true;
            colliderComponent.enabled = true;
        }
        public void SetFollow(GameObject Target)
        {
            if (Target) transform.position = Target.transform.position;

            transform.parent = Target?.transform;
        }
        public void AddToInventory(Inventory Target)
        {
            Pack();
            Container = Target;
            SetFollow(Target.gameObject);
        }
        public void RemoveFromInventory()
        {
            Unpack();
            Container = null;
            SetFollow(null);
        }
        public override Enum GetEntityType()
        {
            return Type;
        }
        #endregion
        /// <summary>
        /// Destroys the entity, deregisters it, and removes it from any storing inventory.
        /// </summary>
        public void DestroyEntity()
        {
            if (Container) Container.RemoveItem(this, true);
            EntityManager.UnregisterItem(this);
            Destroy(gameObject);
        }
    }
}
