using Interfaces;
using Items;
using Items.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Control;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Crafting
{
    public class CraftingUI : Window
    {
#pragma warning disable 0649
        [Header("Crafting UI")]
        [SerializeField]
        private GameObject UI_ItemSelection;
        [SerializeField]
        private DesignUI DesignBench;
        [SerializeField]
        private GameObject UI_PartsList;
        [SerializeField]
        private GameObject UI_PropertiesPanel;
        [SerializeField]
        private GameObject UI_ModulesList;
        [SerializeField]
        private GameObject UI_ModulePropertiesPanel;
        [SerializeField]
        private ToggleGroup TGroup;
#pragma warning restore 0649

        private List<GameObject> UI_PartButtons = new List<GameObject>();
        private List<GameObject> UI_PartProperties = new List<GameObject>();

        private List<GameObject> UI_ModuleButtons = new List<GameObject>();
        private List<GameObject> UI_ModuleProperties = new List<GameObject>();

        public ICraftable CurrentDesign { get; private set; }
        private PartIcon selectedPart;
        public PartIcon SelectedPart 
        { 
            get => selectedPart; 
            set 
            { 
                selectedPart = value;
                DisplayPartProperties();
            }
        }
        private ModuleButton selectedModule;
        public ModuleButton SelectedModule
        {
            get => selectedModule;
            set
            {
                selectedModule = value;
                DisplayModuleProperties();
            }
        }

        protected override void Start()
        {
            base.Start();
            CheckReferences();
        }

        private void CheckReferences()
        {
            List<bool> checks = new List<bool>
            {
                UI_ItemSelection != null,
                DesignBench != null,
                UI_PartsList != null,
                TGroup != null,
                UI_PropertiesPanel != null,
                UI_ModulePropertiesPanel != null
            };

            foreach (bool check in checks)
            {
                if (!check)
                {
                    Debug.LogWarning("One or more serialized fields is null");
                    return;
                }
            }
        }

        public void SelectItem()
        {
            IEnumerator<UnityEngine.UI.Toggle> toggles = TGroup.ActiveToggles().GetEnumerator();
            toggles.MoveNext();
            toggles.Current.GetComponent<Control.Toggle>().Activate();
        }

        //TODO: Cleanup excess temp GameObjects from ItemSelection
        public void StartDesign(ICraftable craftable)
        {
            DisplayParts(craftable.PartGroup);
            CurrentDesign = craftable;
            UI_ItemSelection.SetActive(false);
        }

        private void ClearPartsList()
        {
            foreach(GameObject obj in UI_PartButtons)
            {
                Destroy(obj);
            }
            UI_PartButtons.Clear();
        }

        private void DisplayParts(string partGroup)
        {
            ClearPartsList();
            List<ItemPart> validParts = GetPartsList(partGroup);
            foreach(ItemPart part in validParts)
            {
                GameObject obj = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ButtonDescription], UI_PartsList.transform);
                DescriptionButton button = obj.GetComponent<DescriptionButton>();
                button.Title.text = part.PartName;
                button.Description.text = part.Description;
                button.ActivateAction = delegate { DesignBench.CreatePart(part); };
                UI_PartButtons.Add(obj);
            }
        }

        private List<ItemPart> GetPartsList(string partGroup)
        {
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ItemPart).IsAssignableFrom(p) && p != typeof(ItemPart) ).ToList();

            List<ItemPart> validParts = new List<ItemPart>();

            foreach(Type type in types)
            {
                ItemPart part = Activator.CreateInstance(type) as ItemPart;
                if (part.PartGroup == partGroup)
                {
                    validParts.Add(part);
                }
            }
            return validParts;
        }

        private void ClearPartProperties()
        {
            foreach (GameObject obj in UI_PartProperties)
            {
                Destroy(obj);
            }
            UI_PartProperties.Clear();
        }

        private GameObject[] DisplayProperties(Dictionary<string, ModifiableStat> modifiableStats, Transform parent)
        {
            GameObject[] result = new GameObject[modifiableStats.Count];

            int i = 0;
            foreach(KeyValuePair<string, ModifiableStat> stat in modifiableStats)
            {
                if (!stat.Value.IsEnabled)
                {
                    continue;
                }
                try
                {
                    result[i] = ModifiableStatsHandler.TypeHandlers[stat.Value.TargetType](parent, stat.Key, stat.Value);
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogWarning($"Type \"{stat.Value.TargetType}\" was not found in StatsHandler dictionary!");
                }
                i++;
            }
            return result;
        }

        private void DisplayPartProperties()
        {
            ClearPartProperties();
            UI_PartProperties.AddRange(DisplayProperties(SelectedPart.Part.ModifiableStats, UI_PropertiesPanel.transform));
        }

        private void ClearModuleProperties()
        {
            foreach(GameObject obj in UI_ModuleProperties)
            {
                Destroy(obj);
            }
            UI_ModuleProperties.Clear();
        }

        private void DisplayModuleProperties()
        {
            ClearModuleProperties();
            UI_ModuleProperties.AddRange(DisplayProperties(SelectedModule.RefModule.ModifiableStats, UI_ModulePropertiesPanel.transform));
        }
    }
}