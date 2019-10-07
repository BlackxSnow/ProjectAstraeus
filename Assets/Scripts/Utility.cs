using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    //Create a gradient with colours. Evenly spaced.
    public static Gradient CreateGradient(params Color[] colours)
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] CKey = new GradientColorKey[colours.Length];
        GradientAlphaKey[] AKey = new GradientAlphaKey[colours.Length];
        for(int i = 0; i < colours.Length; i++)
        {
            CKey[i].color = colours[i];
            CKey[i].time = 1f / (colours.Length - 1) * i;

            AKey[i].alpha = 1;
            AKey[i].time = (1 / colours.Length) * i;
        }

        gradient.SetKeys(CKey, AKey);
        return gradient;
    }

    //Find the percentage of a value between the minimum and maximum. Returns 0-1
    public static float FindValueMinMax<T>(float Min, float Max, T Value)
    {
        if (Value is float || Value is int || Value is double)
        {
            dynamic ValueF = Value;
            float Result = (ValueF - Min) / (Max - Min);
            return Result;
        } else
        {
            throw new System.ArgumentException("Parameter must be numerical type", "Value");
        }
    }

    public static Vector3 FlipY(Vector3 vector)
    {
        Vector3 FlippedVector = new Vector3(vector.x, Screen.height - vector.y, vector.z);
        return FlippedVector;
    }
    public static float FlipY(float TargetFloat)
    {
        float FlippedFloat = Screen.height - TargetFloat;
        return FlippedFloat;
    }

    public static Vector3 ScreenToCanvasSpace(Vector3 ScreenInput, Vector2 RefResolution)
    {
        Vector3 ModifiedMousePosition = new Vector3(
            (ScreenInput.x / Screen.width) * RefResolution.x,
            (ScreenInput.y / Screen.height) * -RefResolution.y,
            ScreenInput.z
        );
        return ModifiedMousePosition;
    }
    public static Vector2 ScreenToCanvasSpace(Vector2 ScreenInput, Vector2 RefResolution)
    {
        Vector3 ModifiedPosition = new Vector3(
            (ScreenInput.x / Screen.width) * RefResolution.x,
            (ScreenInput.y / Screen.height) * -RefResolution.y
        );
        return ModifiedPosition;
    }

    public static Vector3 SubtractVector2FromVector3(Vector3 BaseVector, Vector2 SubractVector)
    {
        Vector3 Result = new Vector3(BaseVector.x - SubractVector.x, BaseVector.y - SubractVector.y, BaseVector.z);
        return Result;
    }
    public static Vector3 AddVector2ToVector3(Vector3 BaseVector, Vector2 SubractVector)
    {
        Vector3 Result = new Vector3(BaseVector.x + SubractVector.x, BaseVector.y + SubractVector.y, BaseVector.z);
        return Result;
    }
}
