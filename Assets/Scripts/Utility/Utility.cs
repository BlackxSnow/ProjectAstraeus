using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Nito.AsyncEx;
using UnityAsync;

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
    public enum RoundType
    {
        Floor,
        Ceil,
        Round
    }
    public static float RoundToNDecimals<T>(T Value, int Decimals, RoundType roundType = RoundType.Round)
    {
        dynamic ValueF;
        if (Value is float || Value is int || Value is double)
        {
            ValueF = Value;
        } else throw new System.ArgumentException("Parameter must be numerical type", "Value");

        float MultipliedValue = ValueF * Mathf.Pow(10, Decimals);
        switch (roundType)
        {
            case RoundType.Floor:
                MultipliedValue = Mathf.Floor(MultipliedValue);
                break;
            case RoundType.Ceil:
                MultipliedValue = Mathf.Ceil(MultipliedValue);
                break;
            case RoundType.Round:
                MultipliedValue = Mathf.Round(MultipliedValue);
                break;
            default:
                break;
        }
        
        return MultipliedValue / Mathf.Pow(10, Decimals);
    }

    public static int GetThreshold(float Value, params float[] Thresholds)
    {
        for (int i = 0; i < Thresholds.Length; i++)
        {
            if (Value > Thresholds[i] && (Thresholds.Length <= i+1 || Value < Thresholds[i+1]))
            {
                return i;
            }
        }
        throw new ArithmeticException($"Unable to find threshold for {Value}");
    }

    public static bool WithinBounds (float Value, float MinBound, float MaxBound)
    {
        if (Mathf.Clamp(Value, MinBound, MaxBound) == Value)
        {
            return true;
        } else
        {
            return false;
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

    public static Vector3 ScreenToCanvasSpace(Vector3 ScreenInput, RectTransform Canvas)
    {
        Vector3 CanvasSize = Canvas.rect.size;
        Vector3 ModifiedMousePosition = new Vector3(
            (ScreenInput.x / Screen.width) * CanvasSize.x,
            (ScreenInput.y / Screen.height) * -CanvasSize.y,
            ScreenInput.z
        );
        return ModifiedMousePosition;
    }
    public static Vector2 ScreenToCanvasSpace(Vector2 ScreenInput, RectTransform Canvas)
    {
        Vector3 CanvasSize = Canvas.rect.size;
        Vector3 ModifiedPosition = new Vector3(
            (ScreenInput.x / Screen.width) * CanvasSize.x,
            (ScreenInput.y / Screen.height) * -CanvasSize.y
        );
        return ModifiedPosition;
    }

    public static Vector3 SubtractVector2FromVector3(Vector3 BaseVector, Vector2 SubractVector)
    {
        Vector3 Result = new Vector3(BaseVector.x - SubractVector.x, BaseVector.y - SubractVector.y, BaseVector.z);
        return Result;
    }
    public static Vector3 AddVector2ToVector3(Vector3 BaseVector, Vector2 AddVector)
    {
        Vector3 Result = new Vector3(BaseVector.x + AddVector.x, BaseVector.y + AddVector.y, BaseVector.z);
        return Result;
    }
    public static Equipment.VectorResults RearrangeVector(Vector2 InputVector, out Vector2 ResultVector)
    {
        ResultVector = new Vector2
        {
            x = Mathf.Max(InputVector.x, InputVector.y),
            y = Mathf.Min(InputVector.x, InputVector.y)
        };

        if (ResultVector != InputVector)
        {
            return Equipment.VectorResults.y;
        } else
        {
            return Equipment.VectorResults.x;
        }
    }

    public static IEnumerable<Enum> GetFlags(Enum input)
    {
        foreach (Enum value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }

    public static T[] CombineArrays<T>(params T[][] Arrays)
    {
        int ArrayLength = 0;
        for (int i = 0; i < Arrays.Length; i++)
        {
            ArrayLength += Arrays[i].Length;
        }
        T[] ReturnArray = new T[ArrayLength];
        int CurrentIndex = 0;
        for (int i = 0; i < Arrays.Length; i++)
        {
            Array.Copy(Arrays[i], 0, ReturnArray, CurrentIndex, Arrays[i].Length);
            CurrentIndex += Arrays[i].Length;
        }
        return ReturnArray;
    }

    public static List<T> CombineLists<T>(List<T> BaseList, params List<T>[] AdditionalLists)
    {
        for(int i = 0; i < AdditionalLists.Length; i++)
        {
            foreach(T item in AdditionalLists[i])
            {
                BaseList.Add(item);
            }
        }
        return BaseList;
    }

    public static Transform[] GetChildren(Transform Parent)
    {
        Transform[] Children = new Transform[Parent.childCount];
        for (int i = 0; i < Parent.childCount; i++)
        {
            Children[i] = Parent.GetChild(i);
        }
        return Children;
    }

    public static Dictionary<T,V> DeserializeEnumCollection<T,V>(Dictionary<string,V> Input)
    {

        Dictionary<T,V> Result = new Dictionary<T, V>();
        if (Input == null) return null;
        foreach (KeyValuePair<string, V> kvp in Input)
        {
           T ParsedValue = (T)Enum.Parse(typeof(T), kvp.Key);
            Result.Add(ParsedValue, kvp.Value);
        }
        return Result;
    }
    public static List<T> DeserializeEnumCollection<T>(List<string> Input)
    {
        List<T> Result = new List<T>();
        if (Input == null) return null;
        foreach (string TypeString in Input)
        {
            T ParsedValue = (T)Enum.Parse(typeof(T), TypeString);
            Result.Add(ParsedValue);
        }
        return Result;
    }

    
    public async static Task QueueRemovalFromList<T>(List<T> TargetList, T ToRemove, AsyncAutoResetEvent RunQueue, object LockObject)
    {
        await RunQueue.WaitAsync();
        lock (LockObject)
        {
            TargetList.Remove(ToRemove);
        }
    }

    public static List<T> CloneList<T>(IEnumerable<ICloneable> Original)
    {
        List<T> Result = new List<T>();
        foreach(ICloneable obj in Original)
        {
            Result.Add((T)obj.Clone());
        }
        return Result;
    }

    public struct TimeSpan
    {
        private float days;
        private float totalDays;
        private float hours;
        private float totalHours;
        private float minutes;
        private float totalMinutes;
        private float seconds;
        private float totalSeconds;
        private float miliseconds;
        private float totalMiliseconds;
        public float Days { get => days; }

        public float Hours { get => hours; }
        public float Minutes { get => minutes; }
        public float Seconds { get => seconds; }
        public float Miliseconds { get => miliseconds; }

        public static TimeSpan FromDays(float val) => FromSeconds(val * 24 * 60 * 60);
        public static TimeSpan FromHours(float val) => FromSeconds(val * 60 * 60);
        public static TimeSpan FromMinutes(float val) => FromSeconds(val * 60);
        public static TimeSpan FromSeconds(float val)
        {
            TimeSpan Result = new TimeSpan();
            float Remaining = val;

            Result.totalDays = val / 86400;
            Result.days = Mathf.Floor(Remaining / 86400);
            Remaining -= Result.days * 86400;

            Result.totalHours = val / 3600;
            Result.hours = Mathf.Floor(Remaining / 3600);
            Remaining -= Result.hours * 3600;

            Result.totalMinutes = val / 60;
            Result.minutes = Mathf.Floor(Remaining / 60);
            Remaining -= Result.minutes * 60;

            Result.totalSeconds = val;
            Result.seconds = Mathf.Floor(Remaining);
            Remaining -= Result.seconds;

            Result.totalMiliseconds = val * 1000;
            Result.miliseconds = Mathf.Floor(Remaining * 1000);

            return Result;
        }

        public KeyValuePair<string, float> GetLargestUnit()
        {
            if (totalDays >= 1) return new KeyValuePair<string, float>("d", totalDays);
            else if (totalHours >= 1) return new KeyValuePair<string, float>("h", totalHours);
            else if (totalMinutes >= 1) return new KeyValuePair<string, float>("m", totalMinutes);
            else if (totalSeconds >= 1) return new KeyValuePair<string, float>("s", totalSeconds);
            else return new KeyValuePair<string, float>("ms", totalMiliseconds);
        }
    }

    public class Timer
    {
        //Limit in seconds
        public float Limit = 1f;
        public float Current = 0f;
        private float Interval;
        private float LastTime;
        public bool Repeat;

        public delegate void ElapsedDelegate(float ActualTime);
        public ElapsedDelegate Elapsed;

        public bool Enabled = false;

        private float StartTime;
        public void Start()
        {
            
            Current = 0f;
            Interval = Mathf.Min(Limit / 5f, 0.01f);
            Enabled = true;
            LastTime = Time.time;
            StartTime = Time.time;
            Counter();
        }
        public void Stop()
        {
            Enabled = false;
            Current = 0f;
        }

        async void Counter()
        {
            while (Enabled)
            {
                await new UnityEngine.WaitForSeconds(Limit);
                Elapsed(Time.time - StartTime);
                StartTime = Time.time;
                if (!Repeat) break;
            }
        }

        public Timer(float Seconds, ElapsedDelegate RunMethod, bool Repeat = false)
        {
            this.Repeat = Repeat;
            Limit = Seconds;
            Elapsed = RunMethod;
        }
    }
}
