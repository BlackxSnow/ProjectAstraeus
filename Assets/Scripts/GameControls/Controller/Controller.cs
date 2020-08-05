using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour 
{
    public static PlayerControls InputControls;

    public GameObject Music;
    public static Controller Control;
    public bool SplashStart;

    public static bool Dev = true; //Development / debug mode

    private bool IsInitialised = false;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        if(!IsInitialised)
        {
            if (Control == null)
            {
                Control = this;
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(Music);
            }
            else if (Control != this)
            {
                Destroy(gameObject);
            }

            if (SplashStart)
            {
                SceneManager.LoadScene(2);
            }
            InputControls = new PlayerControls();
            InputControls.Enable();
            IsInitialised = true;
        }
    }

    public static void LoadScene(int Index)
    {
        SceneManager.LoadScene(1); //Loading Screen
        SceneManager.LoadScene(Index);
    }
}
