using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public static class StyleSheet
    {
        public static class Colours
        {
            public static readonly Color Base = new Color(0f, 0.5f, 0.8f, 1f);
            public static readonly Color Pressed = new Color(0f, 0.3f, 0.8f, 1f);
            public static readonly Color Selected = new Color(0f, 0.75f, 0.8f, 1f);
            public static readonly Color Background = new Color(0.2f, 0.25f, 0.5f, 1f);
            public static readonly Color Panel = new Color(0.2f, 0.4f, 0.6f, 1f);

            public static readonly Color AlphaError = new Color(1f, 0f, 1f, 0f);
            public static readonly Color Error = new Color(1f, 0f, 1f, 1f);

            public static class Gradients
            {
                public static readonly Gradient Severity = new Gradient()
                {
                    alphaKeys = new GradientAlphaKey[2] 
                    {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(1f, 1f)
                    },
                    colorKeys = new GradientColorKey[2]
                    {
                        //new GradientColorKey(new Color(0.541f, 0.42f,   0),     0f),
                        new GradientColorKey(new Color(0.804f, 0.431f,  0),     0.5f),
                        new GradientColorKey(new Color(0.541f, 0,       0),     1f)
                    }
                };
                public static readonly Gradient Quality = new Gradient()
                {
                    alphaKeys = new GradientAlphaKey[1] { new GradientAlphaKey(1f, 0f) },
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey(new Color(1f,  0,      0),     0f),
                        new GradientColorKey(new Color(1f,  0.94f,  0.17f), 0.5f),
                        new GradientColorKey(new Color(0,   1f,     0),     1f)
                    }
                };
            }
        }
    } 
}
