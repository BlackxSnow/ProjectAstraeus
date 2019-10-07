using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockController : ScriptableObject
{
    public List<Movement> FlockMembers = new List<Movement>();

    public FlockController() { }

    // Start is called before the first frame update
    public void Awake()
    {
        foreach(ISelectable _Selectable in Selection.SelectedObjs)
        {
            Movement movement = _Selectable.ISelectablegameObject.GetComponent<Movement>();
            if (movement != null)
            {
                FlockMembers.Add(movement);
            }
        }
    }
}
