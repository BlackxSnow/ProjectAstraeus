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

    public int Iron;
    public int Copper;
    public int Alloy;

    public Resources(int _Iron, int _Copper, int _Alloy)
    {
        Iron = _Iron;
        Copper = _Copper;
        Alloy = _Alloy;
    }
    public static Resources operator+ (Resources A, Resources B)
    {
        Resources Result = new Resources(0, 0, 0);
        Result.Iron = A.Iron + B.Iron;
        Result.Copper = A.Copper + B.Copper;
        Result.Alloy = A.Alloy + B.Alloy;

        return Result;
    }

    public void SetCosts(int _Iron, int _Copper, int _Alloy)
    {
        Iron = _Iron;
        Copper = _Copper;
        Alloy = _Alloy;
    }
}
