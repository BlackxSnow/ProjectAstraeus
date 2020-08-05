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
        private ToggleGroup TGroup;
#pragma warning restore 0649

        private List<GameObject> UI_PartProperties = new List<GameObject>();

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
                UI_PropertiesPanel != null
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

        private void DisplayParts(string partGroup)
        {
            List<ItemPart> validParts = GetPartsList(partGroup);
            foreach(ItemPart part in validParts)
            {
                GameObject obj = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.ButtonDescription], UI_PartsList.transform);
                DescriptionButton button = obj.GetComponent<DescriptionButton>();
                button.Title.text = part.PartName;
                button.Description.text = part.Description;
                button.ActivateAction = delegate { DesignBench.CreatePart(part); };
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

        private void DisplayPartProperties()
        {
            ClearPartProperties();

            foreach(KeyValuePair<string, ItemPart.ModifiableStat> stat in SelectedPart.Part.ModifiableStats)
            {
                if(!stat.Value.IsEnabled)
                {
                    continue;
                }
                try
                {
                    UI_PartProperties.Add(ModifiableStatsHandler.TypeHandlers[stat.Value.TargetType](UI_PropertiesPanel.transform, stat.Key, stat.Value));
                }
                catch (KeyNotFoundException)
                {
                    Debug.LogWarning($"Type \"{stat.Value.TargetType}\" was not found in StatsHandler dictionary!");
                }
            }
        }
    }
}