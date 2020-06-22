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
    public static class Colour
    {
        //Create a gradient with colours. Evenly spaced.
        public static Gradient CreateGradient(params Color[] colours)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] CKey = new GradientColorKey[colours.Length];
            GradientAlphaKey[] AKey = new GradientAlphaKey[colours.Length];
            for (int i = 0; i < colours.Length; i++)
            {
                CKey[i].color = colours[i];
                CKey[i].time = 1f / (colours.Length - 1) * i;

                AKey[i].alpha = 1;
                AKey[i].time = (1 / colours.Length) * i;
            }

            gradient.SetKeys(CKey, AKey);
            return gradient;
        }
    } 
}
