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
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Type;
        public Image Icon;

        public GameObject DetailsPanel;
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

    List<GameObject> SelectionUI;
    public void DisplaySelectionUI(ISelectable Selection)
    {
        Entity SelectedEntity = Selection as Entity;
        SelectHUD.Name.text = SelectedEntity.name;
        SelectHUD.Type.text = string.Format("{0}", SelectedEntity.GetEntityType());

        if(Selection is Item)
        {
            SelectionUI = (Selection as Item).Data.InstantiateStatKVPs(false, out List<GameObject> KVPLists, SelectHUD.DetailsPanel.transform);
            Utility.CombineLists(SelectionUI, KVPLists);
        } else
        {
            SelectionUI = Selection.InstantiateStatDisplay();
        }
        DisplaySelectionButtons(Selection);
    }
    void DisplaySelectionButtons(ISelectable Selection)
    {

    }
    public void ClearSelectionUI()
    {
        if (SelectionUI == null) return;
        foreach (GameObject UIObject in SelectionUI)
        {
            Destroy(UIObject);
        }
        SelectionUI = null;
    }
}
