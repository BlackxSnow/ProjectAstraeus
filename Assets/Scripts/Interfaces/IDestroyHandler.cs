using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyHandler
{
    bool DestructionMarked { get; set; }
    void Destroy();

}
