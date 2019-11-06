using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController HUDControl;
    Transform CharacterPanel;
    public GameObject CharacterUI;

    [System.Serializable]
    public struct SelectInfo
    {
        public GameObject InfoPanel;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Type;
        public Image Icon;
        public GameObject DetailsPanel;

        [System.Serializable]
        public struct ButtonsStruct
        {
            public Button Inventory;
            public Button Equipment;
            public Button Stats;
        }
        public ButtonsStruct Buttons;

        
    }
    public SelectInfo SelectHUD;

    List<Character> PortraitedChracters = new List<Character>();

    // Start is called before the first frame update
    void Awake()
    {
        HUDControl = this;
        CharacterPanel = transform.Find("Character Panel");
    }

    //Instantiate icons for all player characters on HUD
    void CreateCharacterIcons()
    {
        foreach (Character Character in EntityManager.PlayerCharacters)
        {
            if (PortraitedChracters.Contains(Character)) continue;
            PortraitedChracters.Add(Character);
            GameObject _CharUI = Instantiate(CharacterUI, CharacterPanel);
            Text _Text = _CharUI.transform.GetChild(1).GetComponent<Text>();
            _Text.text = Character.Name;
            _CharUI.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CreateCharacterIcons();
    }

    public void ActivateWindow (GameObject Window)
    {
        Window.SetActive(true);
    }

    List<GameObject> SelectionUIKVPs;
    public void DisplaySelectionUI(ISelectable Selection)
    {
        DynamicEntity SelectedEntity = Selection as DynamicEntity;
        SelectHUD.InfoPanel.SetActive(true);
        SelectHUD.Name.text = SelectedEntity.Name;
        SelectHUD.Type.text = string.Format("{0}", SelectedEntity.GetEntityType());

        if(Selection is Item)
        {
            SelectionUIKVPs = (Selection as Item).InstantiateStatKVPs(false, out List<GameObject> KVPLists, SelectHUD.DetailsPanel.transform);
            Utility.CombineLists(SelectionUIKVPs, KVPLists);
        } else
        {
            SelectionUIKVPs = Selection.InstantiateStatDisplay();
        }
        DisplaySelectionButtons(Selection);
        
    }

    void ResetButton(Button TargetButton)
    {
        TargetButton.onClick.RemoveAllListeners();
        TargetButton.gameObject.SetActive(false);
    }

    void DisplaySelectionButtons(ISelectable Selection)
    {
        DynamicEntity SelectionEntity = Selection as DynamicEntity;

        ResetButton(SelectHUD.Buttons.Inventory);
        ResetButton(SelectHUD.Buttons.Equipment);
        ResetButton(SelectHUD.Buttons.Stats);

        if (SelectionEntity.EntityFlags.HasFlag(Entity.EntityFlagsEnum.HasInventory))
        {
            SelectHUD.Buttons.Inventory.onClick.AddListener(delegate { UIController.OpenInventory(SelectionEntity.EntityComponents.Inventory); });
            SelectHUD.Buttons.Inventory.gameObject.SetActive(true);
        }
        if (SelectionEntity.EntityFlags.HasFlag(Entity.EntityFlagsEnum.CanEquip))
        {
            SelectHUD.Buttons.Equipment.onClick.AddListener(delegate { UIController.OpenEquipmentWindow(SelectionEntity); });
            SelectHUD.Buttons.Equipment.gameObject.SetActive(true);
        }
        if (SelectionEntity.EntityFlags.HasFlag(Entity.EntityFlagsEnum.HasStats))
        {
            SelectHUD.Buttons.Stats.onClick.AddListener(delegate { UIController.OpenStatsWindow(SelectionEntity.EntityComponents.Stats); });
            SelectHUD.Buttons.Stats.gameObject.SetActive(true);
        }
    }
    public void ClearSelectionUI()
    {
        SelectHUD.InfoPanel.SetActive(false);
        if (SelectionUIKVPs == null) return;
        foreach (GameObject UIObject in SelectionUIKVPs)
        {
            Destroy(UIObject);
        }
        SelectionUIKVPs = null;
        
    }
}
