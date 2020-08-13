using System;
using UnityEngine;

namespace Items
{
    public class ModifiableStat
    {
        public string StatName;
        public bool IsEnabled;
        public Type TargetType;
        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                if (TargetType.IsAssignableFrom(value.GetType()))
                {
                    _value = value;
                }
                else
                {
                    Debug.LogError($"Incorrect type '{value.GetType()} was given to stat \"{StatName}\"; '{TargetType}' was expected.");
                    return;
                }

            }
        }
        public Vector2 Bounds;
    }
}
