using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Modules;

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
    public static GameObject DropdownPrefab;
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
        DropdownPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/UI/Windows/Generic/Dropdown");

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

    public class KVPData
    {
        public string Key;
        public dynamic Value;
        public Transform Parent;
        public int Rounding;
        public Gradient gradient;
        public float Min;
        public float Max;
        public KeyValueGroup Group;
        public KeyValuePanel.GetValueDelegate ValueDelegate;
        public AdditionalModule RefModule;
        public Item RefItem;
        public StatsAndSkills RefStats;
        public Enum ValueEnum;
        public float KeyRatio;

        public KVPData(string Key, dynamic Value, Transform Parent, int Rounding = 0, float KeyRatio = 0.5f)
        {
            this.Key = Key;
            this.Value = Value;
            this.Parent = Parent;
            this.Rounding = Rounding;
            this.KeyRatio = KeyRatio;
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

    public static GameObject InstantiateText<T>(T Value, Transform Parent, KeyValueGroup Group = null, bool AllowWrapping = false)
    {
        GameObject TextInstanceObject = Instantiate(TextObjectPrefab, Parent);
        TextInstanceObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0}", Value);
        TextKVGroup Script = TextInstanceObject.GetComponent<TextKVGroup>();
        Script.TextComponent.enableWordWrapping = AllowWrapping;
        if (Group)
        {
            Script.Group = Group;
            Group.AddMember(Script);
        }

        return TextInstanceObject;
    }

    //Instantiates a prefab panel with two texts designed to show a key and value
    public static List<GameObject> InstantiateKVP(List<KVPData> KVPDatas)
    {
        List<GameObject> KVPs = new List<GameObject>();
        foreach (KVPData Data in KVPDatas)
        {
            KVPs.Add(InstantiateKVP(Data));
        }
        return KVPs;
    }
    public static GameObject InstantiateKVP(KVPData Data)//string Key, T Value, Transform Parent, int Rounding = 0, Gradient gradient = null, float Min = 0, float Max = 0, KeyValueGroup Group = null, KeyValuePanel.GetValueDelegate ValueDelegate = null, AdditionalModule RefModule = null, Item RefItem = null, StatsAndSkills RefStats = null, Enum ValueEnum = null, float KeyRatio = .5f)
    {
        GameObject Panel;
        TextMeshProUGUI KeyText;
        TextMeshProUGUI ValueText;
        KeyValuePanel KVPScript;

        Panel = Instantiate(KeyValuePanelObjectPrefab, Data.Parent);
        KVPScript = Panel.GetComponent<KeyValuePanel>();
        KeyText = KVPScript.Key.TextMesh;
        ValueText = KVPScript.Value.TextMesh;

        if (Data.Group)
        {
            KVPScript.Group = Data.Group;
            Data.Group.AddMember(KVPScript);
        }

        KeyText.GetComponent<RectTransform>().anchorMax = new Vector2(Data.KeyRatio - .05f, 1f);
        ValueText.GetComponent<RectTransform>().anchorMin = new Vector2(Data.KeyRatio + .05f, 0f);

        dynamic Result = Data.Value;
        Color color;

        if (Data.gradient != null)
        {
            if (Data.Min != Data.Max)
            {
                color = Data.gradient.Evaluate(Utility.FindValueMinMax(Data.Min, Data.Max, Data.Value));
            }
            else
            {
                color = Data.gradient.Evaluate(1.0f);
            }
            ValueText.color = color;
        }

        if (Data.Value is float || Data.Value is double)
        {
            Result = Utility.RoundToNDecimals(Data.Value, Data.Rounding);
        }

        KeyText.text = string.Format("{0}", Data.Key);
        ValueText.text = string.Format("{0}", Result);

        if (Data.ValueDelegate != null)
        {
            KVPScript.GetValue = Data.ValueDelegate;
        } else
        {
            KVPScript.DoNotUpdate = true;
        }
        if (Data.ValueDelegate != null && !Data.RefItem && !Data.RefModule && !Data.RefStats) throw new ArgumentException("KVP with ValueDelegate requires one of: RefItem, RefModule, or RefStats");

        KVPScript.GetValueEnum = Data.ValueEnum;
        KVPScript.Refs.RefItem = Data.RefItem;
        KVPScript.Refs.RefModule = Data.RefModule;
        KVPScript.Refs.RefStats = Data.RefStats;

        return Panel;
    }

    //Instantiates a prefab for a list of Key Value Panels
    public static GameObject InstantiateKVPList(string ListName, KVPData[] KeyValuePanels, Transform Parent, KeyValueGroup Group = null)
    {
        GameObject ListPanel = Instantiate(KeyValueListObjectPrefab, Parent);
        GameObject ListNameObject = InstantiateText(ListName, ListPanel.transform, Group: Group);
        ListNameObject.transform.SetAsFirstSibling();

        KeyValueList ListScript = ListPanel.GetComponent<KeyValueList>();
        ListScript.KVPs = new Transform[KeyValuePanels.Length];

        //Iterate over KVPs and instantiate
        for (int i = 0; i < KeyValuePanels.Length; i++)
        {
            KeyValuePanels[i].Parent = ListScript.ContentPanel.transform;
            ListScript.KVPs[i] = InstantiateKVP(KeyValuePanels[i]).transform;
        }

        return ListPanel;
    }

    public static GameObject InstantiateValueSlider(string Name, ItemTypes.StatsEnum StatEnum, Transform Parent, float Min = 0, float Max = 10)
    {
        if (StatEnum == ItemTypes.StatsEnum.Material) throw new ArgumentException(string.Format("ValueSlider cannot be instantiated with non float target: 'Material'") );
        GameObject ValueSlider;

        ValueSlider = Instantiate(ValueSliderPrefab, Parent);

        UI.Crafting.ModuleSlider ValueSliderScript = ValueSlider.GetComponent<UI.Crafting.ModuleSlider>();
        TextMeshProUGUI LabelText = ValueSliderScript.LabelText;

        LabelText.text = Name;
        ValueSliderScript.TargetStat = StatEnum;
        ValueSliderScript.ValueBounds = new UI.Crafting.ModuleSlider.MinMax(Min, Max);

        ValueSliderScript.SetSliderValues();

        return ValueSlider;

    }

    public static GameObject InstantiateDropdown(string Name, ItemTypes.StatsEnum ModStat, Transform Parent)
    {
#warning This needs to be finished
        GameObject DropdownObject;

        DropdownObject = Instantiate(DropdownPrefab, Parent);

        UI.Crafting.ModuleDropdown DropdownScript = DropdownObject.GetComponent<UI.Crafting.ModuleDropdown>();


        throw new NotImplementedException();
    } 

    public static GameObject InstantiateModulePanel(AdditionalModule Module, Transform Parent)
    {
        GameObject DisplayPanel = Instantiate(ModuleDisplayPrefab, Parent);
        ModuleDisplayPanel Script = DisplayPanel.GetComponent<ModuleDisplayPanel>();
        Script.Module = Module;

        KeyValueGroup Group = ScriptableObject.CreateInstance<KeyValueGroup>();
        Group.Init();
        Module.InstantiateStatKVPs(Script.StatPanel.transform, Group, out _);
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
        Item Data = item;

        Script.SetInfo(Data.ItemName, string.Format("{0}", Data.Type));
        KeyValueGroup Group = ScriptableObject.CreateInstance<KeyValueGroup>();
        Group.Init();

        Data.InstantiateStatKVPs(false, out List<GameObject> _, Script.StatsPanel.transform, Group);
        Group.ForceRecalculate();

        return ToolTip;
    }
}
