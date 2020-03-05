using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAsync;

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
    readonly List<IGroupableUI> GroupMembers = new List<IGroupableUI>();
    public float UniformSize;

    public bool Dirty;
    bool Running;

    public void AddMember(IGroupableUI Member)
    {
        GroupMembers.Add(Member);
    }

    public void RemoveMember(IGroupableUI Member)
    {
        GroupMembers.Remove(Member);
    }

    public void ClearGroup()
    {
        GroupMembers.Clear();
    }

    async void LoopRoutine()
    {
        while (Running)
        {
            await Await.Until(() => Dirty);
            UniformSize = Recalculate();
            //if (Dirty) UniformSize = Recalculate();
            //await Await.NextUpdate();
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
    private float Recalculate()
    {
        float MinSize = 100;
        float[] SizeArray = new float[GroupMembers.Count];
        int index = 0;
        foreach (IGroupableUI KVP in GroupMembers)
        {
            if (!(KVP as MonoBehaviour))
            {
                throw new NullReferenceException("IGroupableUI has been destroyed or otherwise no longer exists.");
            }
            SizeArray[index] = KVP.CalculateSize();
            index++;
        }

        if (SizeArray.Length == 0) return 0f;

        int MinSizeIndex = Array.FindIndex(SizeArray, S => S == Mathf.Min(SizeArray));
        MinSize = SizeArray[MinSizeIndex];
        MinSize = Mathf.Clamp(MinSize, Font.Min, Font.Max);
        foreach (IGroupableUI KVP in GroupMembers)
        {
            KVP.SetSize(MinSize);
        }
        Dirty = false;
        return MinSize;
    }

    public void Init(float MinFont = 8f, float MaxFont = 72f)
    {
        if (!Running)
        {
            Font.Min = MinFont;
            Font.Max = MaxFont;
            Running = true;
            LoopRoutine();
        }
    }
}
