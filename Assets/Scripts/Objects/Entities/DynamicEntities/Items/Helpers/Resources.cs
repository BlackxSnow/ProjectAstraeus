using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Resources
{
    public static int ResourceCount = 3;
    public enum ResourceList
    {
        Iron,
        Copper,
        Alloy
    }

    Dictionary<ResourceList, int> ResourceDict;

    public Resources(int Iron, int Copper, int Alloy)
    {
        ResourceDict = new Dictionary<ResourceList, int>()
        {
            { ResourceList.Iron, Iron },
            { ResourceList.Copper, Copper },
            { ResourceList.Alloy, Alloy }
        };
    }

    public int this[ResourceList Resource]
    {
        get
        {
            return ResourceDict[Resource];
        }
        set
        {
            ResourceDict[Resource] = value;
        }
    }

    public static Resources operator+ (Resources A, Resources B)
    {
        Resources Result = new Resources(0, 0, 0);
        Result[ResourceList.Iron] = A[ResourceList.Iron] + B[ResourceList.Iron];
        Result[ResourceList.Copper] = A[ResourceList.Copper] + B[ResourceList.Copper];
        Result[ResourceList.Alloy] = A[ResourceList.Alloy] + B[ResourceList.Alloy];

        return Result;
    }

    public static Resources operator* (Resources A, Resources B)
    {
        Resources Result = new Resources(0, 0, 0);
        foreach (KeyValuePair<ResourceList, int> Res in A.ResourceDict)
        {
            Result[Res.Key] = A[Res.Key] * B[Res.Key];
        };
        return Result;
    }

    public static Resources operator *(Resources A, int B)
    {
        Resources Result = new Resources(0, 0, 0);
        foreach (KeyValuePair<ResourceList, int> Res in A.ResourceDict)
        {
            Result[Res.Key] = A[Res.Key] * B;
        };
        return Result;
    }
}
