﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Biotic
{
    void InitialiseInheritedProperties()
    {
        EntityType = EntityTypes.Character;
    }
    protected override void Start()
    {
        base.Start();
        InitialiseInheritedProperties();
        EntityManager.RegisterCharacter(this, FactionID);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        
        base.Update();
    }


}
