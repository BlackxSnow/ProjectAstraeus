using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ItemModule.AdditionalModule;

public class UIController : MonoBehaviour
{
    public static GameObject CanvasObject;

    public static GameObject KeyValuePanelObjectPrefab;
    public static GameObject KeyValueListObjectPrefab;
    public static GameObject TextObjectPrefab;
    public static GameObject ValueSliderPrefab;
    public static GameObject ModuleDisplayPrefab;
    public static GameObject ItemToolTipPrefab;
    public static GameObject ToolTipPrefab;

    private void Awake()
    {
        KeyValuePanelObjectPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValuePanel");
        KeyValueListObjectPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValueList");
        TextObjectPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/TextObject");
        ValueSliderPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/ValueSlider");
        ModuleDisplayPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/ModuleDisplayPanel");
        ItemToolTipPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/ItemToolTip");

        CanvasObject = FindObjectOfType<Canvas>().gameObject;
    }

    public struct KVPData<T>
    {
        public string Key;
        public T Value;
        public Transform Parent;
        public int Rounding;
        public Gradient gradient;
        public float Min;
        public float Max;
        public KeyValueGroup Group;
        public KeyValuePanel.GetValueDelegate ValueDelegate;
        public ItemModule.AdditionalModule RefModule;
        public ItemData RefItem;

        public KVPData(string Key, T Value, Transform Parent, int Rounding = 0, Gradient gradient = null, float Min = 0, float Max = 0, KeyValueGroup Group = null, KeyValuePanel.GetValueDelegate ValueDelegate = null, ItemModule.AdditionalModule RefModule = null, ItemData RefItem = null)
        {
            this.Key = Key;
            this.Value = Value;
            this.Parent = Parent;
            this.Rounding = Rounding;
            this.gradient = gradient;
            this.Min = Min;
            this.Max = Max;
            this.Group = Group;
            this.ValueDelegate = ValueDelegate;
            this.RefModule = RefModule;
            this.RefItem = RefItem;
        }
    }

    public static GameObject InstantiateText<T>(T Value, Transform Parent)
    {
        GameObject TextInstance = Instantiate(TextObjectPrefab, Parent);
        TextInstance.GetComponent<TextMeshProUGUI>().text = string.Format("{0}", Value);

        return TextInstance;
    }

    //Instantiates a prefab panel with two texts designed to show a key and value
    public static GameObject InstantiateKVP<T>(KVPData<T> Data)
    {
        return InstantiateKVP(Data.Key, Data.Value, Data.Parent, Data.Rounding, Data.gradient, Data.Min, Data.Max, Data.Group, Data.ValueDelegate, Data.RefModule, Data.RefItem);
    }
    public static GameObject InstantiateKVP<T>(string Key, T Value, Transform Parent, int Rounding = 0, Gradient gradient = null, float Min = 0, float Max = 0, KeyValueGroup Group = null, KeyValuePanel.GetValueDelegate ValueDelegate = null, ItemModule.AdditionalModule RefModule = null, ItemData RefItem = null)
    {
        GameObject Panel;
        TextMeshProUGUI KeyText;
        TextMeshProUGUI ValueText;
        KeyValuePanel KVPScript;

        Panel = Instantiate(KeyValuePanelObjectPrefab, Parent);
        KVPScript = Panel.GetComponent<KeyValuePanel>();
        KeyText = KVPScript.Key.TextMesh;
        ValueText = KVPScript.Value.TextMesh;

        if (Group)
        {
            KVPScript.Group = Group;
            Group.AddMember(KVPScript);
        }

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

        if (Value is float || Value is double)
        {
            Result = Utility.RoundToNDecimals(Value, Rounding);
        }

        KeyText.text = string.Format("{0}", Key);
        ValueText.text = string.Format("{0}", Result);

        if (ValueDelegate != null) KVPScript.GetValue = ValueDelegate;
        else
        {
            KVPScript.DoNotUpdate = true;
        }
        if (ValueDelegate != null && !RefItem && !RefModule) throw new ArgumentException("KVP with ValueDelegate require either a RefItem or RefModule");

        KVPScript.Refs.RefItem = RefItem;
        KVPScript.Refs.RefModule = RefModule;

        return Panel;
    }

    //Instantiates a prefab for a list of Key Value Panels
    public static GameObject InstantiateKVPList<T>(string ListName, KVPData<T>[] KeyValuePanels, Transform Parent)
    {
        GameObject ListPanel = Instantiate(KeyValueListObjectPrefab, Parent);
        Transform ListContent = ListPanel.transform.GetChild(1);
        TextMeshProUGUI ListNameText = ListPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        ListNameText.text = ListName;

        KeyValueList ListScript = ListPanel.GetComponent<KeyValueList>();
        ListScript.KVPs = new Transform[KeyValuePanels.Length];

        //Iterate over KVPs and instantiate
        for (int i = 0; i < KeyValuePanels.Length; i++)
        {
            KeyValuePanels[i].Parent = ListContent;
            ListScript.KVPs[i] = InstantiateKVP(KeyValuePanels[i]).transform;
        }

        return ListPanel;
    }

    public static GameObject InstantiateValueSlider(string Name, ModifiableStats StatEnum, Transform Parent, float Min = 0, float Max = 10)
    {
        if (StatEnum == ModifiableStats.Material) throw new ArgumentException(string.Format("ValueSlider cannot be instantiated with non float target: 'Material'") );
        GameObject ValueSlider;

        ValueSlider = Instantiate(ValueSliderPrefab, Parent);

        ValueModSlider ValueSliderScript = ValueSlider.GetComponent<ValueModSlider>();
        TextMeshProUGUI LabelText = ValueSliderScript.LabelText;

        LabelText.text = Name;
        ValueSliderScript.TargetStat = StatEnum;
        ValueSliderScript.ValueBounds = new ValueModSlider.MinMax(Min, Max);

        ValueSliderScript.SetSliderValues();

        return ValueSlider;

    }

    public static GameObject InstantiateModulePanel(ItemModule.AdditionalModule Module, Transform Parent)
    {
        GameObject DisplayPanel = Instantiate(ModuleDisplayPrefab, Parent);
        ModuleDisplayPanel Script = DisplayPanel.GetComponent<ModuleDisplayPanel>();
        Script.Module = Module;

        KeyValueGroup Group = new KeyValueGroup();
        Module.InstantiateStatKVPs(Script.StatPanel.transform, Group, out GameObject[] KVPListArray);
        Script.Name.text = Module.ModuleName;

        return DisplayPanel;
    }
    public static GameObject InstantiateToolTip(Item item)
    {
        GameObject ToolTip;
        ToolTip = Instantiate(ItemToolTipPrefab, CanvasObject.transform);
        ToolTip.GetComponent<RectTransform>().position = Input.mousePosition;

        ItemTooltip Script = ToolTip.GetComponent<ItemTooltip>();
        ItemData Data = item.Data;

        Script.SetInfo(Data.ItemName, string.Format("{0}", Data.Type));
        KeyValueGroup Group = new KeyValueGroup();
        
        Data.InstantiateStatKVPs(false, out List<GameObject> _, Script.StatsPanel.transform, Group);

        return ToolTip;
    }
}
