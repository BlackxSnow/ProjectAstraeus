using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyValueGroup : ScriptableObject
{
    List<KeyValuePanel> GroupMembers = new List<KeyValuePanel>();
    public float UniformSize;

    public bool Dirty;
    bool Running;

    public void AddMember(KeyValuePanel Member)
    {
        GroupMembers.Add(Member);
    }

    public void ClearGroup()
    {
        GroupMembers.Clear();
    }

    IEnumerator LoopRoutine()
    {
        while (Running)
        {
            if (Dirty) UniformSize = Recalculate();
            yield return new WaitForFixedUpdate();
        }
    }

    public new void SetDirty()
    {
        Dirty = true;
    }

    float Recalculate()
    {
        float MinSize = 100;
        float[] SizeArray = new float[GroupMembers.Count];
        int index = 0;
        foreach(KeyValuePanel KVP in GroupMembers)
        {
            SizeArray[index] = KVP.CalculateSize();
            index++;
        }
        MinSize = Mathf.Min(SizeArray);
        foreach (KeyValuePanel KVP in GroupMembers)
        {
            KVP.SetSize(MinSize);
        }
        Dirty = false;
        return MinSize;
    }

    public KeyValueGroup()
    {
        Running = true;
        Controller.Control.StartCoroutineWrapper(LoopRoutine());
    }
}
