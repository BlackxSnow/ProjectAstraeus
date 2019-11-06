using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Faction = FactionManager.Faction;

public class Diplomacy : Window
{
    public Text InfoText;
    public GameObject KeyValuePanel;
    public Transform InfoLeft;
    public Transform RelationsPanel;

    List<GameObject> Panels = new List<GameObject>();

    public void ShowFactionInfo(int ID)
    {

        //Creating generic faction information
        Panels.Add(UIController.InstantiateKVP(new UIController.KVPData("Name", FactionManager.Factions[ID].Name, InfoLeft)));
        Panels.Add(UIController.InstantiateKVP(new UIController.KVPData("ID", FactionManager.Factions[ID].ID, InfoLeft)));

        //Creating text for faction relations
        foreach (KeyValuePair<int, Faction> _KVP in FactionManager.Factions)
        {
            if (_KVP.Key == ID)
            {
                Gradient gradient = Utility.CreateGradient(Color.green);
                UIController.KVPData Data = new UIController.KVPData(FactionManager.Factions[_KVP.Key].Name, "SELF", RelationsPanel)
                {
                    gradient = gradient
                };
                Panels.Add(UIController.InstantiateKVP(Data));
            }
            else
            {
                Gradient gradient = Utility.CreateGradient(Color.red, Color.white, Color.green);
                UIController.KVPData Data = new UIController.KVPData(FactionManager.Factions[_KVP.Key].Name, FactionManager.Factions[ID].GetRelations(_KVP.Key), RelationsPanel)
                {
                    gradient = gradient,
                    Min = -100,
                    Max = 100
                };
                Panels.Add(UIController.InstantiateKVP(Data));
            }
        }
    }

    //Clears all shown info from above functions
    public void ClearFactionInfo()
    {
        List<GameObject> RemovePanels = new List<GameObject>();
        for (int i = 0; i < Panels.Count; i++)
        {
            GameObject Panel = Panels[i];
            GameObject.Destroy(Panel);
            RemovePanels.Add(Panel);
        }
        foreach (GameObject Panel in RemovePanels)
        {
            GameObject.Destroy(Panel);
            Panels.Remove(Panel);
        }
        RemovePanels.Clear();
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //Generate faction buttons
        foreach (KeyValuePair<int, Faction> _KVP in FactionManager.Factions)
        {
            Text _Text = Instantiate(InfoText, transform.GetChild(0).GetChild(0).GetChild(1));
            _Text.text = _KVP.Value.Name;
            _Text.gameObject.GetComponent<Button>().onClick.AddListener(delegate { ClearFactionInfo();  ShowFactionInfo(_KVP.Key); }); //Add onclick event to show info
            _Text.gameObject.SetActive(true);
        }
        ShowFactionInfo(0); //Show player faction information
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Show faction info via name. Unused
    public void ShowFactionInfo(string Name)
    {
        int _ID = FactionManager.FactionIDs[Name];
        ShowFactionInfo(_ID);
    }
}
