using Nito.AsyncEx;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.Control
{
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
            ProgressBarPrefab,
            OrderButtonPrefab,
            ContextMenuPrefab,
            SelectionCirclePrefab,
            ItemIconPrefab,

            InventoryPrefab,
            EquipmentPrefab,
            StatsPrefab,

            MedicalDetailsPrefab,
            InjuryIconPrefab,
            ConditionIconPrefab
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
            "ProgressBar",
            "OrderButton",
            "ContextMenu",
            "SelectionCircle",
            "ItemIcon",

            "InventoryUI",
            "EquipmentUI",
            "StatsUI",

            "MedicalInfo",
            "InjuryIcon",
            "ConditionIcon"
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

        public static AsyncManualResetEvent DataLoadedEvent = new AsyncManualResetEvent();
        async void LoadAssets()
        {
            Task<IList<GameObject>> AssetTasks = Addressables.LoadAssetsAsync<GameObject>(AssetKeys, null, Addressables.MergeMode.Union).Task;
            Task<IList<Sprite>> SpriteTasks = Addressables.LoadAssetsAsync<Sprite>(SpriteKeys, null, Addressables.MergeMode.Union).Task;
            await Task.WhenAll(AssetTasks, SpriteTasks);

            if (AssetTasks.Result.Count != AssetKeys.Length) { Debug.LogWarning($"{AssetKeys.Length} asset keys exist but {AssetTasks.Result.Count} were loaded"); }

            int i = 0;
            foreach (GameObject Asset in AssetTasks.Result)
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

            DataLoadedEvent.Set();
        }
    } 
}
