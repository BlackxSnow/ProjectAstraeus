using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static GameObject KeyValuePanel;

    private void Awake()
    {
        KeyValuePanel = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValuePanel");
    }

    //Instantiates a prefab panel with two texts designed to show a key and value
    public static GameObject InstantiateKVP<T>(string Key, T Value, Transform Parent, Gradient gradient = null, float min = 0, float max = 0)
    {
        GameObject Panel;
        Text KeyText;
        Text ValueText;

        Panel = Instantiate(KeyValuePanel, Parent);
        KeyText = Panel.transform.GetChild(0).GetComponent<Text>();
        ValueText = Panel.transform.GetChild(1).GetComponent<Text>();

        dynamic Result = Value;
        Color color;

        if (gradient != null)
        {
            if (min != max)
            {
                color = gradient.Evaluate(Utility.FindValueMinMax(min, max, Value));
            }
            else
            {
                color = gradient.Evaluate(1.0f);
            }
            ValueText.color = color;
        }

        if (Value is float)
        {
            Result = Mathf.Round(Result);
        }

        KeyText.text = string.Format("{0}", Key);
        ValueText.text = string.Format("{0}", Result);
        return Panel;
    }
}
