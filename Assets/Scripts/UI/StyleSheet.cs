using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class StyleSheet : MonoBehaviour
    {
        public class ColourClass
        {
            public readonly Color Base = new Color(0f, 0.5f, 0.8f, 1f);
            public readonly Color Pressed = new Color(0f, 0.3f, 0.8f, 1f);
            public readonly Color Selected = new Color(0f, 0.75f, 0.8f, 1f);
            public readonly Color Background = new Color(0.2f, 0.25f, 0.5f, 1f);
            public readonly Color Panel = new Color(0.2f, 0.4f, 0.6f, 1f);
        }

        public static ColourClass Colours = new ColourClass();
    } 
}
