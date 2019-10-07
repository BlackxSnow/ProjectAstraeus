using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Actor
{
    protected override void Start()
    {
        base.Start();
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
