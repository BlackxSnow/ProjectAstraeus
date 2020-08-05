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
    public static class Math
    {

        //Find the percentage of a value between the minimum and maximum. Returns 0-1
        public static float FindValueMinMax<T>(float Min, float Max, T Value)
        {
            if (Value is float || Value is int || Value is double)
            {
                dynamic ValueF = Value;
                float Result = (ValueF - Min) / (Max - Min);
                return Result;
            }
            else
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
            }
            else throw new System.ArgumentException("Parameter must be numerical type", "Value");

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
                if (Value > Thresholds[i] && (Thresholds.Length <= i + 1 || Value < Thresholds[i + 1]))
                {
                    return i;
                }
            }
            throw new ArithmeticException($"Unable to find threshold for {Value}");
        }

        public static bool WithinBounds(float Value, float MinBound, float MaxBound)
        {
            if (Mathf.Clamp(Value, MinBound, MaxBound) == Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool WithinBounds(Vector2 value, float boundSize)
        {
            if(WithinBounds(value.x, -boundSize, boundSize) && WithinBounds(value.y, -boundSize, boundSize))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    } 
}
