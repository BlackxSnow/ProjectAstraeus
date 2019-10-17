using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {

    public GameObject Music;
    public static Controller Control;
    public bool SplashStart;

    public static bool Dev = true; //Development / debug mode

	void Awake () {
        if (Control == null)
        {
            Control = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(Music);
        } else if (Control != this)
        {
            Destroy(gameObject);
        }
        if (SplashStart)
        {
            SceneManager.LoadScene(2);
        }
	}

    public static void LoadScene(int Index)
    {
        SceneManager.LoadScene(1); //Loading Screen
        SceneManager.LoadScene(Index);
    }
}
