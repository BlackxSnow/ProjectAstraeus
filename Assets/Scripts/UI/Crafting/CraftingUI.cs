using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Modules;
using TMPro;

using System.Reflection;
using System.Linq;
using static Modules.AdditionalModule;

namespace UI
{
    namespace Crafting
    {
        public class CraftingUI : Window
        {
            [Header("Display UI Elements")]
            public GameObject UI_ModuleStatsPanel;
            public GameObject UI_TotalStatsPanel;
            public GameObject UI_ModuleDisplayList;
            public GameObject UI_ModificationUIPanel;
            [Space(10)]
            [Header("Interactive UI Elements")]
            public GameObject UI_ItemListPanel;
            private ToggleGroup ItemToggles;
            public ModuleSelect ModuleDropdown;
            [Space(10)]

            [HideInInspector]
            public static Item CurrentItem;
            public static AdditionalModule CurrentModule;
            public static CraftingUI ActiveUI;

            public struct KVPStruct
            {
                public List<GameObject> gameObjects;
                public List<KeyValuePanel> Scripts;
                public List<KeyValueList> ListScripts;
                public KeyValueGroup Group;

                public KVPStruct(KeyValueGroup Group)
                {
                    Group.Init();
                    gameObjects = new List<GameObject>();
                    Scripts = new List<KeyValuePanel>();
                    ListScripts = new List<KeyValueList>();
                    this.Group = Group;
                }

                public void AddKVP(GameObject KVP)
                {
                    gameObjects.Add(KVP);
                    Scripts.Add(KVP.GetComponent<KeyValuePanel>());
                }
                public void AddKVP(GameObject[] KVPs)
                {
                    for (int i = 0; i < KVPs.Length; i++)
                    {
                        AddKVP(KVPs[i]);
                    }
                }
                public void AddKVP(List<GameObject> KVPs)
                {
                    foreach (GameObject KVP in KVPs)
                    {
                        AddKVP(KVP);
                    }
                }
                public void AddKVPList(GameObject KVPList)
                {
                    gameObjects.Add(KVPList);
                    KeyValueList Script = KVPList.GetComponent<KeyValueList>();
                    ListScripts.Add(Script);
                    for (int i = 0; i < Script.KVPs.Length; i++)
                    {
                        Scripts.Add(Script.KVPs[i].GetComponent<KeyValuePanel>());
                    }
                }
                public void AddKVPList(GameObject[] KVPLists)
                {
                    for (int i = 0; i < KVPLists.Length; i++)
                    {
                        AddKVPList(KVPLists[i]);
                    }
                }
                public void AddKVPList(List<GameObject> KVPLists)
                {
                    foreach (GameObject KVPList in KVPLists)
                    {
                        AddKVPList(KVPList);
                    }
                }
                public void Clear()
                {
                    foreach(KeyValuePanel KVP in Scripts)
                    {
                        Group.RemoveMember(KVP);
                    };
                    gameObjects.Clear();
                    Scripts.Clear();
                }
            }

            public static KeyValueGroup CraftingUIGroup;
            private static KVPStruct TotalStatKVPInfo;
            private static KVPStruct ModuleStatKVPInfo;
            private static KVPStruct ModulePanelKVPInfo;

            private readonly Dictionary<AdditionalModule, ModuleDisplayPanel> ModuleDisplays = new Dictionary<AdditionalModule, ModuleDisplayPanel>();
            private readonly List<GameObject> ModificationUI = new List<GameObject>();

            private void Awake()
            {
                CraftingUIGroup = ScriptableObject.CreateInstance<KeyValueGroup>();
                CraftingUIGroup.Font.Max = 18;

                ItemToggles = UI_ItemListPanel.GetComponent<ToggleGroup>();
                TotalStatKVPInfo = new KVPStruct(CraftingUIGroup);
                ModuleStatKVPInfo = new KVPStruct(CraftingUIGroup);
                ModulePanelKVPInfo = new KVPStruct(CraftingUIGroup);
                ActiveUI = this;
            }
            public void ClearAll()
            {
                ClearKVPs(true, true);
                ClearModificationUI();
                ClearModuleList();
            }
            public void ClearKVPs(bool Module, bool Total)
            {
                if (Module)
                {
                    foreach (GameObject Panel in ModuleStatKVPInfo.gameObjects) Destroy(Panel);
                    ModuleStatKVPInfo.Clear();
                }

                if (Total)
                {
                    foreach (GameObject Panel in TotalStatKVPInfo.gameObjects) Destroy(Panel);
                    TotalStatKVPInfo.Clear();
                }
            }
            public void ClearModificationUI()
            {
                foreach (GameObject ModificationElement in ModificationUI) Destroy(ModificationElement);
                ModificationUI.Clear();
            }
            public void ClearModuleList()
            {
                foreach (KeyValuePair<AdditionalModule, ModuleDisplayPanel> ModuleDisplay in ModuleDisplays)
                {
                    if (ModuleDisplay.Value == null) continue;
                    Destroy(ModuleDisplay.Value.gameObject);
                }
                ModuleDisplays.Clear();
            }
            public void DeselectModules()
            {
                foreach (KeyValuePair<AdditionalModule, ModuleDisplayPanel> ModulePanel in ModuleDisplays)
                {
                    ModulePanel.Value.Deselect();
                }
                ClearModificationUI();
                ClearKVPs(true, false);
                CurrentModule = null;
            }
            public void SelectModule(AdditionalModule Module)
            {
                DeselectModules();

                ModuleDisplays[Module].Select();
                CurrentModule = Module;

                InitialiseModuleStatDisplay();
                InitialiseModificationUI();
            }
            public void SelectModule(AdditionalModule Module, ModuleDisplayPanel Panel)
            {
                DeselectModules();

                Panel.Select();
                CurrentModule = Module;

                InitialiseModuleStatDisplay();
                InitialiseModificationUI();
            }

            public void NewItem()
            {
                IEnumerator<Toggle> ActiveToggleEnum = ItemToggles.ActiveToggles().GetEnumerator();
                ActiveToggleEnum.MoveNext();
                Toggle ActiveToggle = ActiveToggleEnum.Current;

                ItemTypes.Types TypeEnum = ActiveToggle.GetComponent<ItemSelectToggle>().Type;
                Type ItemType = TypeAttribute.GetStoredData(typeof(ItemTypes.Types), TypeEnum).Type;

                if (CurrentItem) CurrentItem.DestroyEntity();
                ClearAll();

                GameObject ItemInstance = Instantiate(ItemTypes.ItemBasePrefab, new Vector3(0, 0.5f, 0), new Quaternion(0, 0, 0, 0));
                CurrentItem = (Item)ItemInstance.AddComponent(ItemType);
                CurrentItem.Type = TypeEnum;
                CurrentItem.Init();
                CurrentItem.Pack();

                ModuleDropdown.SetOptions();
                InitialiseTotalStatDisplay();
            }

            public void AddModule()
            {
                Type ModuleType = TypeAttribute.GetStoredData(typeof(ModulesEnum), ModuleDropdown.SelectedModule).Type;
                CurrentModule = (AdditionalModule)ScriptableObject.CreateInstance(ModuleType);
                
                CurrentItem.Modules.Add(CurrentModule);
                CurrentItem.CalculateStats();
                ModuleDisplays.Add(CurrentModule, UIController.InstantiateModulePanel(CurrentModule, UI_ModuleDisplayList.transform, out GameObject[] ModuleKVPs, CraftingUIGroup).GetComponent<ModuleDisplayPanel>());
                SelectModule(CurrentModule);
                ModulePanelKVPInfo.AddKVP(ModuleKVPs);
                UpdateKVPs(false, true);
            }

            public void RemoveModule(AdditionalModule Module)
            {
                if (CurrentModule == Module)
                {
                    ClearKVPs(true, false);
                }
                CurrentItem.Modules.Remove(Module);
                CurrentItem.CalculateStats();
                UpdateKVPs(false, true);
            }


            public void CreateItem()
            {
                if (!CurrentItem) return;
                CurrentItem.Name = string.Format("{0}", CurrentItem.Type);
                CurrentItem.Unpack();

                ClearAll();
                CurrentModule = null;
                CurrentItem = null;
            }

            void InitialiseModificationUI()
            {
                List<UIController.DropdownData> dropdownDatas = new List<UIController.DropdownData>();
                ClearModificationUI();
                foreach (KeyValuePair<ItemTypes.StatsEnum, StatInfoObject> ModifiableStat in CurrentModule.ModifiableStats)
                {
                    float Min = 0f;
                    float Max = 10f;
                    if (ModifiableStat.Value.MinValue != 0 || ModifiableStat.Value.MaxValue != 0)
                    {
                        Min = ModifiableStat.Value.MinValue;
                        Max = ModifiableStat.Value.MaxValue;
                    }

                    switch (ModifiableStat.Key)
                    {
                        case ItemTypes.StatsEnum.Material:
                            dropdownDatas.Add(new UIController.DropdownData() { Name = "Material", Parent = UI_ModificationUIPanel.transform, Option = Crafting.ModuleDropdown.DropdownOptions.Material });
                            break;
                        case ItemTypes.StatsEnum.FireMode:
                            dropdownDatas.Add(new UIController.DropdownData() { Name = "Fire Mode", Parent = UI_ModificationUIPanel.transform, Option = Crafting.ModuleDropdown.DropdownOptions.FireMode });
                            break;
                        default:
                            ModificationUI.Add(UIController.InstantiateValueSlider(ModifiableStat.Key.ToString(), ModifiableStat.Key, UI_ModificationUIPanel.transform, Min, Max));
                            break;
                    }
                }
                foreach(UIController.DropdownData dropdownData in dropdownDatas)
                {
                    ModificationUI.Add(UIController.InstantiateDropdown(dropdownData));
                }
            }

            void InitialiseModuleStatDisplay()
            {
                ClearKVPs(true, false);

                GameObject[] KVPArray;
                KVPArray = CurrentModule.InstantiateStatKVPs(UI_ModuleStatsPanel.transform, ModuleStatKVPInfo.Group, out GameObject[] KVPListArray);

                ModuleStatKVPInfo.AddKVP(KVPArray);
                ModuleStatKVPInfo.AddKVPList(KVPListArray);
            }


            void InitialiseTotalStatDisplay()
            {
                ClearKVPs(false, true);

                //General Stats
                List<GameObject> KVPs = CurrentItem.InstantiateStatKVPs(true, out List<GameObject> KVPLists, UI_TotalStatsPanel.transform, TotalStatKVPInfo.Group);
                TotalStatKVPInfo.AddKVP(KVPs);
                TotalStatKVPInfo.AddKVPList(KVPLists);
            }

            public static void UpdateKVPs(bool Module, bool Total)
            {
                if (Module)
                {
                    CurrentModule.CalculateStats();
                    foreach (KeyValuePanel KVP in ModuleStatKVPInfo.Scripts)
                    {
                        KVP.UpdateValue();
                    }
                    foreach (KeyValuePanel KVP in ModulePanelKVPInfo.Scripts)
                    {
                        KVP.UpdateValue();
                    }
                }
                if (Total)
                {
                    CurrentItem.CalculateStats();
                    foreach (KeyValuePanel KVP in TotalStatKVPInfo.Scripts)
                    {
                        KVP.UpdateValue();
                    }
                }
            }
        } 
    }
}