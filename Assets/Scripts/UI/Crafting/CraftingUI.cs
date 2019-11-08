using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Modules;
using TMPro;

using System.Reflection;
using System.Linq;

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
                    gameObjects.Clear();
                    Scripts.Clear();
                    Group.ClearGroup();
                }
            }

            private static KVPStruct TotalStatKVPInfo;
            private static KVPStruct ModuleStatKVPInfo;

            private readonly List<GameObject> ModuleDisplays = new List<GameObject>();
            private readonly List<GameObject> ModificationUI = new List<GameObject>();

            private void Awake()
            {
                ItemToggles = UI_ItemListPanel.GetComponent<ToggleGroup>();
                TotalStatKVPInfo = new KVPStruct(ScriptableObject.CreateInstance<KeyValueGroup>());
                ModuleStatKVPInfo = new KVPStruct(ScriptableObject.CreateInstance<KeyValueGroup>());
            }
            public void ClearAll()
            {
                ClearKVPs(true, true);
                ClearValueSliders();
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
            public void ClearValueSliders()
            {
                foreach (GameObject ValueSlider in ModificationUI) Destroy(ValueSlider);
                ModificationUI.Clear();
            }
            public void ClearModuleList()
            {
                foreach (GameObject ModuleDisplay in ModuleDisplays) Destroy(ModuleDisplay);
                ModuleDisplays.Clear();
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

            public void NewModule()
            {
                Type ModuleType = TypeAttribute.GetStoredData(typeof(AdditionalModule.ModulesEnum), ModuleDropdown.SelectedModule).Type;
                CurrentModule = (AdditionalModule)ScriptableObject.CreateInstance(ModuleType);

                InitialiseModuleStatDisplay();
                InitialiseModificationUI();
            }

            public void AddModule()
            {
                if (!CurrentModule) return;
                ClearKVPs(true, false);
                ModuleStatKVPInfo.Group.ClearGroup();
                ClearValueSliders();
                CurrentItem.Modules.Add(CurrentModule);
                CurrentItem.SetStats();
                ModuleDisplays.Add(UIController.InstantiateModulePanel(CurrentModule, UI_ModuleDisplayList.transform));
                CurrentModule = null;
                UpdateKVPs(false, true);
            }

            public static void RemoveModule(AdditionalModule Module)
            {
                CurrentItem.Modules.Remove(Module);
                CurrentItem.SetStats();
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
                ClearValueSliders();
                foreach (KeyValuePair<ItemTypes.StatsEnum, object> ModifiableStat in CurrentModule.ModifiableStats)
                {
                    if (ModifiableStat.Key == ItemTypes.StatsEnum.Material)
                    {
                        ModificationUI.Add(UIController.InstantiateDropdown("Material", UI_ModificationUIPanel.transform, Crafting.ModuleDropdown.DropdownOptions.Material));
                        continue;
                    }
                    ModificationUI.Add(UIController.InstantiateValueSlider(ModifiableStat.Key.ToString(), ModifiableStat.Key, UI_ModificationUIPanel.transform, 0, 10));
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
                    foreach (KeyValuePanel KVP in ModuleStatKVPInfo.Scripts)
                    {
                        CurrentModule.CalculateStats();
                        KVP.UpdateValue();
                    }
                }
                if (Total)
                {
                    foreach (KeyValuePanel KVP in TotalStatKVPInfo.Scripts)
                    {
                        KVP.UpdateValue();
                    }
                }
            }
        } 
    }
}