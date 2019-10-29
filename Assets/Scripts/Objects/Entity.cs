using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour, IOwnable
{
    public enum EntityTypes
    {
        Item,
        Character,
        Animal,
        Vehicle,
        Structure
    }
    [Flags]
    public enum EntityFlagsEnum
    {
        HasInventory = 1 << 0,
        CanEquip = 1 << 1,
        HasStats = 1 << 2,
    }

    public struct EntityComponentsStruct
    {
        public Inventory @Inventory;
        public Equipment @Equipment;
        public StatsAndSkills @Stats;
    }

    public int FactionID { get; set; }

    public string Name;
    public EntityTypes EntityType;
    public EntityFlagsEnum EntityFlags;
    public EntityComponentsStruct EntityComponents;

    public Animator animator;
    protected Renderer rendererComponent;

    public virtual Enum GetEntityType()
    {
        return EntityType;
    }

    public virtual void GetEntityComponents()
    {
        EntityComponents.Inventory = GetComponent<Inventory>();
        EntityComponents.Equipment = GetComponent<Equipment>();
        EntityComponents.Stats = GetComponent<StatsAndSkills>();
        if (EntityComponents.Inventory) EntityFlags |= EntityFlagsEnum.HasInventory;
        if (EntityComponents.Equipment) EntityFlags |= EntityFlagsEnum.CanEquip;
        if (EntityComponents.Stats) EntityFlags |= EntityFlagsEnum.HasStats;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        GetEntityComponents();
        
        animator = GetComponent<Animator>();
        if (Name == "") Name = name;
        FactionID = 0;//Mathf.RoundToInt(Random.value * (FactionManager.Factions.Count - 1));
        rendererComponent = gameObject.GetComponentInChildren<Renderer>();
        if (rendererComponent)
            rendererComponent.material.color = FactionManager.Factions[FactionID].FactionColour; //Debug; Visually shows faction colour
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void Awake()
    {

    }
}
