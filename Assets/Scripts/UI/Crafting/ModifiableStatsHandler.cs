using Items;
using Items.Parts;
using System;
using System.Collections.Generic;
using UI.Control;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Crafting
{
    public static class ModifiableStatsHandler
    {
        public delegate GameObject StatsTypeHandler(Transform parent, string name, ItemPart.ModifiableStat statData);
        public static Dictionary<Type, StatsTypeHandler> TypeHandlers = new Dictionary<Type, StatsTypeHandler>()
        {
            {typeof(float), new StatsTypeHandler(FloatHandler) },
            {typeof(int), new StatsTypeHandler(IntHandler) },
            {typeof(string), new StatsTypeHandler(StringHandler) },
            {typeof(Materials.Material), new StatsTypeHandler(MaterialHandler) }
        };

        #region Handlers
        public static GameObject FloatHandler(Transform parent, string name, ItemPart.ModifiableStat statData)
        {
            if (statData.TargetType != typeof(float))
            {
                throw new ArgumentException($"Type of '{statData.TargetType}' was passed to incorrect handler.");
            }

            SliderHandler.GetDataDelegate getData = delegate { return (float)statData.Value; };
            SliderHandler.SetDataDelegate setData = delegate (object data) { statData.Value = (float)data; };

            (GameObject, SliderHandler) result = CreateUI.Interactable.Slider(parent, name, getData, setData, statData.Bounds);
            result.Item2.UpdateValues();
            return result.Item1;
        }

        public static GameObject IntHandler(Transform parent, string name, ItemPart.ModifiableStat statData)
        {
            if (!statData.TargetType.IsAssignableFrom(typeof(int)))
            {
                throw new ArgumentException($"Type of '{statData.TargetType}' was passed to incorrect handler.");
            }

            SliderHandler.GetDataDelegate getData = delegate { return (int)statData.Value; };
            SliderHandler.SetDataDelegate setData = delegate (object data) { statData.Value = (int)data; };

            (GameObject, SliderHandler) result = CreateUI.Interactable.Slider(parent, name, getData, setData, statData.Bounds);
            result.Item2.SetSliderMode(true);
            result.Item2.IsIntegerSlider = true;
            result.Item2.UpdateValues();
            return result.Item1;
        }

        public static GameObject StringHandler(Transform parent, string name, ItemPart.ModifiableStat statData)
        {
            if (statData.TargetType != typeof(string))
            {
                throw new ArgumentException($"Type of '{statData.TargetType}' was passed to incorrect handler.");
            }
            throw new NotImplementedException();
        }

        public static GameObject MaterialHandler(Transform parent, string name, ItemPart.ModifiableStat statData)
        {
            if (statData.TargetType != typeof(Materials.Material))
            {
                throw new ArgumentException($"Type of '{statData.TargetType}' was passed to incorrect handler.");
            }

            DropdownHandler.GetDataDelegate getData = delegate { return statData.Value; };
            DropdownHandler.SetDataDelegate setData = delegate (object data) { statData.Value = (Materials.Material)data; };

            (GameObject, DropdownHandler) result = CreateUI.Interactable.Dropdown(parent, name, getData, setData);

            DropdownHandler.OptionData[] optionData = new DropdownHandler.OptionData[Materials.MaterialDict.Count];

            int index = 0;
            foreach(KeyValuePair<Materials.MaterialTypes, Materials.Material> material in Materials.MaterialDict)
            {
                optionData[index] = new DropdownHandler.OptionData(material.Key.ToString(), material.Value);
                index++;
            }
            result.Item2.SetOptions(optionData);
            result.Item2.UpdateValues();
            return result.Item1;
        }

        #endregion
    }
}
