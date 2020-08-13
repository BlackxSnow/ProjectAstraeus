using Items;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UI.Control.UIController;
using static UnityEngine.Object;

namespace UI.Control
{
    public static class CreateUI
    {
        public enum LayoutTypes
        {
            None,
            Horizontal,
            Vertical,
            Grid
        }

        public class ColourRange
        {
            public Gradient Colours;
            public float Min;
            public float Max;

            public ColourRange(Gradient gradient, float min, float max)
            {
                Colours = gradient;
                Min = min;
                Max = max;
            }
        }

        public class KVPData
        {
            public string Key;
            public dynamic Value;
            public Transform Parent;
            public int Rounding;
            public ColourRange GradientRange;
            public KeyValueGroup Group;
            public Func<string> ValueDelegate;
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

        public static class Window
        {
            public static GameObject Inventory(Inventory TargetInventory)
            {
                GameObject InventoryObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.InventoryPrefab], UnpinnedPanel);
                InventoryUI Inventory = InventoryObject.GetComponent<InventoryUI>();

                Inventory.OpenInventory(TargetInventory);
                Inventory.BringToFront();
                return InventoryObject;
            }

            public static GameObject Equipment(Entity TargetEntity)
            {
                GameObject EquipmentObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.EquipmentPrefab], UnpinnedPanel);
                EquipmentUI Script = EquipmentObject.GetComponent<EquipmentUI>();

                Script.Init(TargetEntity);
                Script.BringToFront();

                return EquipmentObject;
            }

            public static GameObject Stats(StatsAndSkills TargetStats)
            {
                GameObject StatsObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.StatsPrefab], UnpinnedPanel);
                StatsUI Script = StatsObject.GetComponent<StatsUI>();

                Script.Init(TargetStats);
                Script.BringToFront();

                return StatsObject;
            }
        }

        public static class Info
        {

            public static GameObject TextPanel<T>(T Value, Transform Parent, KeyValueGroup Group = null, bool AllowWrapping = false, bool InList = false)
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
            public static List<GameObject> KeyValuePanel(List<KVPData> KVPDatas)
            {
                List<GameObject> KVPs = new List<GameObject>();
                foreach (KVPData Data in KVPDatas)
                {
                    KVPs.Add(KeyValuePanel(Data));
                }
                return KVPs;
            }
            public static GameObject KeyValuePanel(KVPData Data, bool InList = false)
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

                if (Data.GradientRange != null)
                {
                    if (Data.GradientRange.Min != Data.GradientRange.Max)
                    {
                        color = Data.GradientRange.Colours.Evaluate(Utility.Math.FindValueMinMax(Data.GradientRange.Min, Data.GradientRange.Max, Data.Value));
                    }
                    else
                    {
                        color = Data.GradientRange.Colours.Evaluate(1.0f);
                    }
                    ValueText.color = color;
                }

                if (Data.Value is float || Data.Value is double)
                {
                    Result = Utility.Math.RoundToNDecimals(Data.Value, Data.Rounding);
                }

                KeyText.text = string.Format("{0}", Data.Key);
                ValueText.text = string.Format("{0}", Result);
                KVPScript.InList = InList;
                KVPScript.Init();

                if (Data.ValueDelegate != null)
                {
                    KVPScript.GetValue = Data.ValueDelegate;
                }
                else
                {
                    KVPScript.DoNotUpdate = true;
                }

                return Panel;
            }

            //Instantiates a prefab for a list of Key Value Panels
            public static GameObject KVPList(string ListName, KVPData[] KeyValuePanels, Transform Parent, KeyValueGroup Group = null)
            {
                GameObject ListPanel = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.KeyValueListObjectPrefab], Parent);
                GameObject ListNameObject = TextPanel(ListName, ListPanel.transform, Group: Group, InList: true);
                ListNameObject.transform.SetAsFirstSibling();

                KeyValueList ListScript = ListPanel.GetComponent<KeyValueList>();
                ListScript.KVPs = new Transform[KeyValuePanels.Length];

                //Iterate over KVPs and instantiate
                for (int i = 0; i < KeyValuePanels.Length; i++)
                {
                    KeyValuePanels[i].Parent = ListScript.ContentPanel.transform;
                    ListScript.KVPs[i] = KeyValuePanel(KeyValuePanels[i], true).transform;
                }

                return ListPanel;
            }

            public static GameObject ItemToolTip(Item item)
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

            public static GameObject ToolTip(string Title, string Description, GameObject TargetObj)
            {
                GameObject ToolTipObject;
                ToolTipObject = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ToolTipPrefab], CanvasObject.transform);
                ToolTipObject.GetComponent<RectTransform>().position = Input.mousePosition;

                ToolTip Script = ToolTipObject.GetComponent<ToolTip>();
                Script.SetInfo(Title, Description, TargetObj);

                return ToolTipObject;
            }

            public static ProgressBar ProgressBar(GameObject parent, float current, float max, Color background, Color foreground)
            {
                GameObject Bar;
                Bar = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ProgressBarPrefab], CanvasObject.transform);
                ProgressBar BarScript = Bar.GetComponent<ProgressBar>();
                BarScript.Init(current, max, background, foreground, parent);

                return BarScript;
            }
        }

        public static class Crafting
        {

        }

        public static class Layout
        {

            public static GameObject LayoutPanel(Transform Parent, LayoutTypes Layout = LayoutTypes.None, bool ExpandHorizontal = true, bool ExpandVertical = true, float spacing = 0, string PanelName = "")
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
        }

        public static class Interactable
        {

            public static GameObject ContextMenu(Vector2 position)
            {
                GameObject Instance = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.ContextMenuPrefab], position, Quaternion.identity, CanvasObject.transform);
                return Instance;
            }

            public static GameObject Button(Transform parent, out TextMeshProUGUI ButtonText, out Button UIButton)
            {
                GameObject ButtonInstance = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.OrderButtonPrefab], parent);
                ButtonText = ButtonInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                UIButton = ButtonInstance.GetComponent<Button>();
                return ButtonInstance;
            }

            public static (GameObject, DropdownHandler) Dropdown(Transform parent, string title, DropdownHandler.GetDataDelegate getData, DropdownHandler.SetDataDelegate setData)
            {
                GameObject obj = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.Dropdown], parent);
                DropdownHandler handler = obj.GetComponent<DropdownHandler>();

                handler.Initialise(title, getData, setData);
                return (obj, handler);
            }

            public static (GameObject, SliderHandler) Slider(Transform parent, string title, SliderHandler.GetDataDelegate getData, SliderHandler.SetDataDelegate setData, Vector2 bounds)
            {
                GameObject obj = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.Slider], parent);
                SliderHandler handler = obj.GetComponent<SliderHandler>();

                handler.Initialise(title, getData, setData);
                handler.SetSliderBounds(bounds.x, bounds.y);
                return (obj, handler);
            }

            public static GameObject ModuleButton(Transform parent, string title, Items.Modules.PartModule module)
            {
                GameObject obj = Instantiate(ObjectPrefabs[ObjectPrefabsEnum.Crafting_ModuleButton], parent);
                ModuleButton mbutton = obj.GetComponent<ModuleButton>();

                mbutton.RefModule = module;
                mbutton.SetTitle(title);
                return obj;
            }
        }
    }
}
