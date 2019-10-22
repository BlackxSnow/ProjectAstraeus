﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : DynamicEntity
{
    public ItemData Data;

    bool Following;
    GameObject FollowTarget;

    void InitialiseInheritedProperties()
    {
        EntityType = EntityTypes.Item;
    }

    protected override void Start()
    {
        base.Start();
        InitialiseInheritedProperties();
        EntityType = EntityTypes.Item;
        EntityManager.RegisterItem(this);
        if(Data) Data.SetStats();
        else if(!Data && Controller.Dev)
        {
            Data = new ItemData(ItemTypes.Types.Torso);

        }
    }

    //Three functions for moving items from world to inventory and vice versa
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
        if(Target)  transform.position = Target.transform.position;

        transform.SetParent(Target.transform);
    }

    public override Enum GetEntityType()
    {
        return Data.Type;
    }
}
