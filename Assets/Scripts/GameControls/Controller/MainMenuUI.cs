using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public GameObject Buttons;
    public GameObject MenuPanel;

    public GameObject NewGameMenu;
    public GameObject LoadGameMenu;
    public GameObject SettingsMenu;

    public Controller Control;

    void Awake()
    {
        Control = Controller.Control;
    }

    public void EnableMenu(string Menu)
    {
        MenuPanel.SetActive(true);
        switch (Menu)
        {
            case ("New Game"):
                NewGameMenu.SetActive(true);
                break;
            case ("Load Game"):
                LoadGameMenu.SetActive(true);
                break;
            case ("Settings"):
                SettingsMenu.SetActive(true);
                break;
        }
    }
    public void DisableMenus()
    {
        MenuPanel.SetActive(false);
        NewGameMenu.SetActive(false);
        LoadGameMenu.SetActive(false);
        SettingsMenu.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
