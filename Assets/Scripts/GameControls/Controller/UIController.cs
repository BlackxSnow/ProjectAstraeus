using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using TMPro;
using Modules;
using System.Threading.Tasks;

public class UIController : MonoBehaviour
{
    public enum ObjectPrefabsEnum
    {
        KeyValuePanelObjectPrefab,
        KeyValueListObjectPrefab,
        TextObjectPrefab,
        ValueSliderPrefab,
        ModuleDisplayPrefab,
        PanelPrefab,
        ItemToolTipPrefab,
        ToolTipPrefab,
        ToolTippedIconPrefab,
        DropdownPrefab,

        InventoryPrefab,
        EquipmentPrefab,
        StatsPrefab,

        MedicalDetailsPrefab
    }
    public static string[] AssetKeys = new string[]
    {
        "KeyValuePanel",
        "KeyValueList",
        "TextObject",
        "ValueSlider",
        "ModuleDisplayPanel",
        "LayoutPanel",
        "ItemToolTip",
        "ToolTip",
        "ToolTippedIcon",
        "Dropdown",

        "InventoryUI",
        "EquipmentUI",
        "StatsUI",

        "MedicalInfo"
    };
    public enum SpritesEnum
    {
        Condition_Bleeding
    }
    public static string[] SpriteKeys = new string[]
    {
        "Condition_Bleeding"
    };

    public static Dictionary<ObjectPrefabsEnum, GameObject> ObjectPrefabs = new Dictionary<ObjectPrefabsEnum, GameObject>();
    public static Dictionary<SpritesEnum, Sprite> LoadedSprites = new Dictionary<SpritesEnum, Sprite>();



    public static GameObject CanvasObject;
    public static Transform PinnedPanel;
    public static Transform UnpinnedPanel;

    private void Awake()
    {
        LoadAssets();

        CanvasObject = FindObjectOfType<Canvas>().gameObject;
        UnpinnedPanel = CanvasObject.transform.GetChild(0);
        PinnedPanel = CanvasObject.transform.GetChild(1);
    }


    async void LoadAssets()
    {
        Task<IList<GameObject>> AssetTasks = Addressables.LoadAssetsAsync<GameObject>(AssetKeys, null, Addressables.MergeMode.Union).Task;
        Task<IList<Sprite>> SpriteTasks = Addressables.LoadAssetsAsync<Sprite>(SpriteKeys, null, Addressables.MergeMode.Union).Task;
        await Task.WhenAll(AssetTasks, SpriteTasks);

        if (AssetTasks.Result.Count != AssetKeys.Length) { Debug.LogWarning($"{AssetKeys.Length} asset keys exist but {AssetTasks.Result.Count} were loaded"); }

        int i = 0;
        foreach(GameObject Asset in AssetTasks.Result)
        {
            ObjectPrefabs.Add((ObjectPrefabsEnum)i, Asset);
            Debug.Log($"Loaded {(ObjectPrefabsEnum)i} :: {Asset.name}");
            i++;
        }
        i = 0;
        foreach (Sprite Asset in SpriteTasks.Result)
        {
            LoadedSprites.Add((SpritesEnum)i, Asset);
            Debug.Log($"Loaded {(SpritesEnum)i} :: {Asset.name}");
            i++;
        }
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
        GameObject InventoryObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.InventoryPrefab], UnpinnedPanel);
        InventoryUI Inventory = InventoryObject.GetComponent<InventoryUI>();

        Inventory.OpenInventory(TargetInventory);
        Inventory.BringToFront();
        return InventoryObject;
    }

    public static GameObject OpenEquipmentWindow(Entity TargetEntity)
    {
        GameObject EquipmentObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.EquipmentPrefab], UnpinnedPanel);
        EquipmentUI Script = EquipmentObject.GetComponent<EquipmentUI>();

        Script.Init(TargetEntity);
        Script.BringToFront();

        return EquipmentObject;
    }

    public static GameObject OpenStatsWindow(StatsAndSkills TargetStats)
    {
        GameObject StatsObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.StatsPrefab], UnpinnedPanel);
        StatsUI Script = StatsObject.GetComponent<StatsUI>();

        Script.Init(TargetStats);
        Script.BringToFront();

        return StatsObject;
    }

    public static GameObject InstantiateText<T>(T Value, Transform Parent, KeyValueGroup Group = null, bool AllowWrapping = false, bool InList = false)
    {
        GameObject TextInstanceObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.TextObjectPrefab], Parent);
        TextInstanceObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0}", Value);
        TextKVGroup Script = TextInstanceObject.GetComponent<TextKVGroup>();
        Script.TextComponent.enableWordWrapping = AllowWrapping;
        Script.InList = InList;
        Script.Init();
        if (Group)
        {
            Script.Group = Group;
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
    public static GameObject InstantiateKVP(KVPData Data, bool InList = false)
    {
        GameObject Panel;
        TextMeshProUGUI KeyText;
        TextMeshProUGUI ValueText;
        KeyValuePanel KVPScript;

        Panel = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.KeyValuePanelObjectPrefab], Data.Parent);
        KVPScript = Panel.GetComponent<KeyValuePanel>();
        KeyText = KVPScript.Key.TextMesh;
        ValueText = KVPScript.Value.TextMesh;

        if (Data.Group)
        {
            KVPScript.Group = Data.Group;
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
        KVPScript.InList = InList;
        KVPScript.Init();

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
        GameObject ListPanel = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.KeyValueListObjectPrefab], Parent);
        GameObject ListNameObject = InstantiateText(ListName, ListPanel.transform, Group: Group, InList: true);
        ListNameObject.transform.SetAsFirstSibling();

        KeyValueList ListScript = ListPanel.GetComponent<KeyValueList>();
        ListScript.KVPs = new Transform[KeyValuePanels.Length];

        //Iterate over KVPs and instantiate
        for (int i = 0; i < KeyValuePanels.Length; i++)
        {
            KeyValuePanels[i].Parent = ListScript.ContentPanel.transform;
            ListScript.KVPs[i] = InstantiateKVP(KeyValuePanels[i], true).transform;
        }

        return ListPanel;
    }

    public static GameObject InstantiateValueSlider(string Name, ItemTypes.StatsEnum StatEnum, Transform Parent, float Min = 0, float Max = 10)
    {
        if (StatEnum == ItemTypes.StatsEnum.Material) throw new ArgumentException(string.Format("ValueSlider cannot be instantiated with non float target: 'Material'") );
        GameObject ValueSlider;

        ValueSlider = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ValueSliderPrefab], Parent);

        UI.Crafting.ModuleSlider ValueSliderScript = ValueSlider.GetComponent<UI.Crafting.ModuleSlider>();
        TextMeshProUGUI LabelText = ValueSliderScript.LabelText;

        LabelText.text = Name;
        ValueSliderScript.TargetStat = StatEnum;
        ValueSliderScript.ValueBounds = new UI.Crafting.ModuleSlider.MinMax(Min, Max);

        ValueSliderScript.SetSliderValues();

        return ValueSlider;

    }

    public class DropdownData
    {
        public string Name;
        public Transform Parent;
        public UI.Crafting.ModuleDropdown.DropdownOptions Option;
    }
    public static GameObject InstantiateDropdown(DropdownData Data)
    {
        return InstantiateDropdown(Data.Name, Data.Parent, Data.Option);
    }
    public static GameObject InstantiateDropdown(string Name, Transform Parent, UI.Crafting.ModuleDropdown.DropdownOptions DropdownType)
    {
        GameObject DropdownObject;

        DropdownObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.DropdownPrefab], Parent);

        UI.Crafting.ModuleDropdown DropdownScript = DropdownObject.GetComponent<UI.Crafting.ModuleDropdown>();
        DropdownScript.DropdownType = DropdownType;
        DropdownScript.TextMeshComponent.text = Name;
        DropdownScript.Init();

        return DropdownObject;
    } 

    public static GameObject InstantiateModulePanel(AdditionalModule Module, Transform Parent, out GameObject[] KVPs, KeyValueGroup Group = null)
    {
        GameObject DisplayPanel = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ModuleDisplayPrefab], Parent);
        ModuleDisplayPanel Script = DisplayPanel.GetComponent<ModuleDisplayPanel>();
        Script.Module = Module;
        if (!Group)
        {
            Group = ScriptableObject.CreateInstance<KeyValueGroup>();
            Group.Font.Max = 18;
        }
        
        Group.Init();
        KVPs = Module.InstantiateModifiableStatKVPs(Script.StatPanel.transform, Group);
        Script.Name.text = Module.ModuleName;

        return DisplayPanel;
    }

    public static GameObject InstantiateLayoutPanel(Transform Parent, LayoutTypes Layout = LayoutTypes.None, bool ExpandHorizontal = true, bool ExpandVertical = true, float spacing = 0, string PanelName = "")
    {
        GameObject PanelInstance = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.PanelPrefab], Parent);
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
    public static GameObject InstantiateItemToolTip(Item item)
    {
        GameObject ToolTipObject;
        ToolTipObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ItemToolTipPrefab], CanvasObject.transform);
        ToolTipObject.GetComponent<RectTransform>().position = Input.mousePosition;

        ItemTooltip Script = ToolTipObject.GetComponent<ItemTooltip>();
        Item Data = item;

        Script.SetInfo(Data.ItemName, string.Format("{0}", Data.Type));
        KeyValueGroup Group = ScriptableObject.CreateInstance<KeyValueGroup>();
        Group.Init();

        Data.InstantiateStatKVPs(false, out List<GameObject> _, Script.StatsPanel.transform, Group);
        Group.ForceRecalculate();

        return ToolTipObject;
    }

    public static GameObject InstantiateToolTip(string Title, string Description, GameObject TargetObj)
    {
        GameObject ToolTipObject;
        ToolTipObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ToolTipPrefab], CanvasObject.transform);
        ToolTipObject.GetComponent<RectTransform>().position = Input.mousePosition;

        ToolTip Script = ToolTipObject.GetComponent<ToolTip>();
        Script.SetInfo(Title, Description, TargetObj);

        return ToolTipObject;
    }
}
