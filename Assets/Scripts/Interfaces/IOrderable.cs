using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrderable
{
    void Move(Vector3 Destination, FlockController flockController, bool StopAll);
}
