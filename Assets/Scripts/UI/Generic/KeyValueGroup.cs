using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyValueGroup : MonoBehaviour
{
    List<KeyValuePanel> GroupMembers = new List<KeyValuePanel>();
    public float UniformSize;

    public bool Dirty;

    public void AddMember(KeyValuePanel Member)
    {
        GroupMembers.Add(Member);
    }

    public void ClearGroup()
    {
        GroupMembers.Clear();
    }

    private void Update()
    {
        if (Dirty) UniformSize = Recalculate();
    }

    public void SetDirty()
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
}
