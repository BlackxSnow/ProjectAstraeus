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
    public static Transform PinnedPanel;
    public static Transform UnpinnedPanel;

    public static GameObject KeyValuePanelObjectPrefab;
    public static GameObject KeyValueListObjectPrefab;
    public static GameObject TextObjectPrefab;
    public static GameObject ValueSliderPrefab;
    public static GameObject ModuleDisplayPrefab;
    public static GameObject PanelPrefab;
    public static GameObject ItemToolTipPrefab;
    //public static GameObject ToolTipPrefab;

    //Window Prefabs
    public static GameObject InventoryPrefab;
    public static GameObject EquipmentPrefab;
    public static GameObject StatsPrefab;

    private void Awake()
    {
        KeyValuePanelObjectPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValuePanel");
        KeyValueListObjectPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/KeyValueList");
        TextObjectPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/TextObject");
        ValueSliderPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/ValueSlider");
        ModuleDisplayPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/ModuleDisplayPanel");
        PanelPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/LayoutPanel");
        ItemToolTipPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/ItemToolTip");

        InventoryPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/InventoryUI");
        EquipmentPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Equipment/EquipmentUI");
        StatsPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Stats/StatsUI");

        CanvasObject = FindObjectOfType<Canvas>().gameObject;
        UnpinnedPanel = CanvasObject.transform.GetChild(0);
        PinnedPanel = CanvasObject.transform.GetChild(1);
    }

    public enum LayoutTypes
    {
        None,
        Horizontal,
        Vertical,
        Grid
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
        public StatsAndSkills RefStats;

        public KVPData(string Key, T Value, Transform Parent, int Rounding = 0, Gradient gradient = null, float Min = 0, float Max = 0, KeyValueGroup Group = null, KeyValuePanel.GetValueDelegate ValueDelegate = null, ItemModule.AdditionalModule RefModule = null, ItemData RefItem = null, StatsAndSkills RefStats = null)
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
            this.RefStats = RefStats;
        }
    }

    public static GameObject OpenInventory(Inventory TargetInventory)
    {
        GameObject InventoryObject = Instantiate(InventoryPrefab, UnpinnedPanel);
        InventoryUI Inventory = InventoryObject.GetComponent<InventoryUI>();

        Inventory.OpenInventory(TargetInventory);
        Inventory.BringToFront();
        return InventoryObject;
    }

    public static GameObject OpenEquipmentWindow(Entity TargetEntity)
    {
        GameObject EquipmentObject = Instantiate(EquipmentPrefab, UnpinnedPanel);
        EquipmentUI Script = EquipmentObject.GetComponent<EquipmentUI>();

        Script.Init(TargetEntity);
        Script.BringToFront();

        return EquipmentObject;
    }

    public static GameObject OpenStatsWindow(StatsAndSkills TargetStats)
    {
        GameObject StatsObject = Instantiate(StatsPrefab, UnpinnedPanel);
        StatsUI Script = StatsObject.GetComponent<StatsUI>();

        Script.Init(TargetStats);
        Script.BringToFront();

        return StatsObject;
    }

    public static GameObject InstantiateText<T>(T Value, Transform Parent, KeyValueGroup Group = null)
    {
        GameObject TextInstance = Instantiate(TextObjectPrefab, Parent);
        TextInstance.GetComponent<TextMeshProUGUI>().text = string.Format("{0}", Value);
        TextKVGroup Script = TextInstance.GetComponent<TextKVGroup>();
        
        if (Group)
        {
            Script.Group = Group;
            Group.AddMember(Script);
        }

        return TextInstance;
    }

    //Instantiates a prefab panel with two texts designed to show a key and value
    public static GameObject InstantiateKVP<T>(KVPData<T> Data)
    {
        return InstantiateKVP(Data.Key, Data.Value, Data.Parent, Data.Rounding, Data.gradient, Data.Min, Data.Max, Data.Group, Data.ValueDelegate, Data.RefModule, Data.RefItem, Data.RefStats);
    }
    public static GameObject InstantiateKVP<T>(string Key, T Value, Transform Parent, int Rounding = 0, Gradient gradient = null, float Min = 0, float Max = 0, KeyValueGroup Group = null, KeyValuePanel.GetValueDelegate ValueDelegate = null, ItemModule.AdditionalModule RefModule = null, ItemData RefItem = null, StatsAndSkills RefStats = null, Enum ValueEnum = null, float PreferredHeight = 0, float KeyRatio = .5f, bool LeftAligned = false)
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
        if (PreferredHeight != 0)
        {
            Panel.GetComponent<LayoutElement>().preferredHeight = PreferredHeight;
        }
        if (LeftAligned)
        {
            KeyText.alignment = TextAlignmentOptions.Left;
            ValueText.alignment = TextAlignmentOptions.Left;
        }
        KeyText.GetComponent<RectTransform>().anchorMax = new Vector2(KeyRatio - .05f, 1f);
        ValueText.GetComponent<RectTransform>().anchorMin = new Vector2(KeyRatio + .05f, 0f);

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

        if (ValueDelegate != null)
        {
            KVPScript.GetValue = ValueDelegate;
        } else
        {
            KVPScript.DoNotUpdate = true;
        }
        if (ValueDelegate != null && !RefItem && !RefModule && !RefStats) throw new ArgumentException("KVP with ValueDelegate requires one of: RefItem, RefModule, or RefStats");

        KVPScript.GetValueEnum = ValueEnum;
        KVPScript.Refs.RefItem = RefItem;
        KVPScript.Refs.RefModule = RefModule;
        KVPScript.Refs.RefStats = RefStats;

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

    public static GameObject InstantiateLayoutPanel(Transform Parent, LayoutTypes Layout = LayoutTypes.None, bool ExpandHorizontal = true, bool ExpandVertical = true, float spacing = 0, string PanelName = "")
    {
        GameObject PanelInstance = Instantiate(PanelPrefab, Parent);
        switch (Layout)
        {
            case LayoutTypes.None:
                break;
            case LayoutTypes.Horizontal:
                HorizontalLayoutGroup HLayout = PanelInstance.AddComponent<HorizontalLayoutGroup>();
                HLayout.childForceExpandWidth = ExpandHorizontal;
                HLayout.childForceExpandHeight = ExpandVertical;
                HLayout.spacing = spacing;
                break;
            case LayoutTypes.Vertical:
                VerticalLayoutGroup VLayout = PanelInstance.AddComponent<VerticalLayoutGroup>();
                VLayout.childForceExpandWidth = ExpandHorizontal;
                VLayout.childForceExpandHeight = ExpandVertical;
                VLayout.spacing = spacing;
                break;
            case LayoutTypes.Grid:
                PanelInstance.AddComponent<GridLayoutGroup>();
                break;
        }
        if (PanelName != "")
        {
            PanelInstance.name = PanelName;
        }
        return PanelInstance;
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
        Group.ForceRecalculate();

        return ToolTip;
    }
}
