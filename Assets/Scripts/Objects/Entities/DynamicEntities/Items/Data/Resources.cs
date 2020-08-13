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
    Dictionary<ResourceList, float> ResourceDict { get; set; }

    public Resources(float Iron, float Copper, float Alloy)
    {
        ResourceDict = new Dictionary<ResourceList, float>()
        {
            { ResourceList.Iron, Iron },
            { ResourceList.Copper, Copper },
            { ResourceList.Alloy, Alloy }
        };
    }

    public float this[ResourceList resource]
    {
        get
        {
            return ResourceDict[resource];
        }
        set
        {
            ResourceDict[resource] = value;
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
        foreach (KeyValuePair<ResourceList, float> Res in A.ResourceDict)
        {
            Result[Res.Key] = A[Res.Key] * B[Res.Key];
        };
        return Result;
    }

    public static Resources operator *(Resources A, float B)
    {
        Resources Result = new Resources(0, 0, 0);
        foreach (KeyValuePair<ResourceList, float> Res in A.ResourceDict)
        {
            Result[Res.Key] = A[Res.Key] * B;
        };
        return Result;
    }
}
