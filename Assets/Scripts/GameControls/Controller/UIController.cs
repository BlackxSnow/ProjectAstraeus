using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static GameObject KeyValuePanel;
    public static GameObject KeyValueList;
    public static GameObject TextObject;

    private void Awake()
    {
        KeyValuePanel = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValuePanel");
        KeyValueList = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValueList");
        TextObject = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/TextObject");
    }

    public struct KVPData<T>
    {
        public string Key;
        public T Value;
        public Transform Parent;
        public Gradient gradient;
        public float Min;
        public float Max;

        public KVPData(string Key, T Value, Transform Parent, Gradient gradient = null, float Min = 0, float Max = 0)
        {
            this.Key = Key;
            this.Value = Value;
            this.Parent = Parent;
            this.gradient = gradient;
            this.Min = Min;
            this.Max = Max;
        }
    }

    public static GameObject InstantiateText<T>(T Value, Transform Parent)
    {
        GameObject TextInstance = Instantiate(TextObject, Parent);
        TextInstance.GetComponent<TextMeshProUGUI>().text = string.Format("{0}", Value);

        return TextInstance;
    }

    //Instantiates a prefab panel with two texts designed to show a key and value
    public static GameObject InstantiateKVP<T>(KVPData<T> Data)
    {
        return InstantiateKVP(Data.Key, Data.Value, Data.Parent, Data.gradient, Data.Min, Data.Max);
    }
    public static GameObject InstantiateKVP<T>(string Key, T Value, Transform Parent, Gradient gradient = null, float Min = 0, float Max = 0)
    {
        GameObject Panel;
        TextMeshProUGUI KeyText;
        TextMeshProUGUI ValueText;

        Panel = Instantiate(KeyValuePanel, Parent);
        KeyText = Panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        ValueText = Panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        dynamic Result = Value;
        Color color;

        if (gradient != null)
        {
            if (Min != Max)
            {
                color = gradient.Evaluate(Utility.FindValueMinMax(Min, Max, Value));
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

    //Instantiates a prefab for a list of Key Value Panels
    public static GameObject InstantiateKVPList<T>(string ListName, KVPData<T>[] KeyValuePanels, Transform Parent)
    {
        GameObject ListPanel = Instantiate(KeyValueList, Parent);
        Transform ListContent = ListPanel.transform.GetChild(1);
        TextMeshProUGUI ListNameText = ListPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        ListNameText.text = ListName;

        //Iterate over KVPs and instantiate
        for(int i = 0; i < KeyValuePanels.Length; i++)
        {
            KeyValuePanels[i].Parent = ListContent;
            InstantiateKVP(KeyValuePanels[i]);
        }

        return ListPanel;
    }
}
