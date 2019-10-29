using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyValueGroup : ScriptableObject
{
    public struct FontSizes
    {
        public float Min;
        public float Max;

        public FontSizes(float Min, float Max)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    public FontSizes Font = new FontSizes(8, 72);

    List<IGroupableUI> GroupMembers = new List<IGroupableUI>();
    public float UniformSize;

    public bool Dirty;
    bool Running;

    public void AddMember(IGroupableUI Member)
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

    public void ForceRecalculate()
    {
        UniformSize = Recalculate();
        foreach(IGroupableUI Member in GroupMembers)
        {
            Member.SetSize(UniformSize);
        }
    }
    float Recalculate()
    {
        float MinSize = 100;
        float[] SizeArray = new float[GroupMembers.Count];
        int index = 0;
        foreach(IGroupableUI KVP in GroupMembers)
        {
            SizeArray[index] = KVP.CalculateSize();
            index++;
        }
        MinSize = Mathf.Min(SizeArray);
        MinSize = Mathf.Clamp(MinSize, Font.Min, Font.Max);
        foreach (IGroupableUI KVP in GroupMembers)
        {
            KVP.SetSize(MinSize);
        }
        Dirty = false;
        return MinSize;
    }

    public KeyValueGroup(float MinFont = 8, float MaxFont = 72)
    {
        Font.Min = MinFont;
        Font.Max = MaxFont;
        Running = true;
        Controller.Control.StartCoroutineWrapper(LoopRoutine());
    }
}
