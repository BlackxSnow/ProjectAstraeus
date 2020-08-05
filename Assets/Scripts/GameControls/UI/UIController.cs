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
            PanelPrefab,
            ItemToolTipPrefab,
            ToolTipPrefab,
            ToolTippedIconPrefab,
            ProgressBarPrefab,
            OrderButtonPrefab,
            ContextMenuPrefab,
            SelectionCirclePrefab,
            ItemIconPrefab,
            TogglePrefab,
            ButtonDescription,

            InventoryPrefab,
            EquipmentPrefab,
            StatsPrefab,

            MedicalDetailsPrefab,
            InjuryIconPrefab,
            ConditionIconPrefab,

            Crafting_AttachmentPoint,
            Crafting_PartIcon,
            Slider,
            Dropdown
        }
        public static string[] AssetKeys = new string[]
        {
            "KeyValuePanel",
            "KeyValueList",
            "TextObject",
            "LayoutPanel",
            "ItemToolTip",
            "ToolTip",
            "ToolTippedIcon",
            "ProgressBar",
            "OrderButton",
            "ContextMenu",
            "SelectionCircle",
            "ItemIcon",
            "Toggle",
            "Button_Description",

            "InventoryUI",
            "EquipmentUI",
            "StatsUI",

            "MedicalInfo",
            "InjuryIcon",
            "ConditionIcon",

            "Crafting_AttachmentPoint",
            "Crafting_PartIcon",
            "Slider",
            "Dropdown"
        };
        public enum SpritesEnum
        {
            Condition_Bleeding,

            Crafting_Attachment_Primary,
            Crafting_Attachment_Secondary,
            Crafting_Attachment_Input
        }
        public static string[] SpriteKeys = new string[]
        {
            "Condition_Bleeding",

            "Crafting_Attachment_Primary",
            "Crafting_Attachment_Secondary",
            "Crafting_Attachment_Input",
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
