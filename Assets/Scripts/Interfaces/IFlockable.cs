using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlockable
{
    bool Arrived { get; }
    Entity EntitySelf { get; }
}
