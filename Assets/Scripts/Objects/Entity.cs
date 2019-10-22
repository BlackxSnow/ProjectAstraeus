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
    public int FactionID { get; set; }

    public string Name;
    public EntityTypes EntityType;

    public Animator animator;
    protected Renderer rendererComponent;

    public virtual Enum GetEntityType()
    {
        return EntityType;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        Name = name;
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
