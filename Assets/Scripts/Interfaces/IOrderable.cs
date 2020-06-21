using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrderable
{
    AI.AIStateMachine StateMachine { get; }
    void Move(Vector3 Destination, FlockController flockController, bool StopAll);
}
