using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Nito.AsyncEx;
using UnityAsync;
using System.Threading;

namespace Utility
{
    public static class Misc
    {
        public static Transform[] GetChildren(Transform Parent)
        {
            Transform[] Children = new Transform[Parent.childCount];
            for (int i = 0; i < Parent.childCount; i++)
            {
                Children[i] = Parent.GetChild(i);
            }
            return Children;
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

        public static string RoundedString(float input, int decimals = 1)
        {
            return Math.RoundToNDecimals(input, decimals).ToString();
        }
    } 
}
