using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    public Slider Zoom;
    public Text ZoomT;

    public Slider RotX;
    public Text RotXT;

    public Slider RotY;
    public Text RotYT;

    public Slider Traverse;
    public Text TraverseT;

    public GameObject ConfirmationBox;

    public void UpdateSettings()
    {
        Zoom.value = PlayerPrefs.GetFloat("Zoom", 10);
        ZoomT.text = Zoom.value.ToString();

        RotX.value = PlayerPrefs.GetFloat("RotX", 40);
        RotXT.text = RotX.value.ToString();

        RotY.value = PlayerPrefs.GetFloat("RotY", 20);
        RotYT.text = RotY.value.ToString();

        Traverse.value = PlayerPrefs.GetFloat("Traverse", 10);
        TraverseT.text = Traverse.value.ToString();
    }

    public void ResetSettingsDialogue(bool State)
    {
        ConfirmationBox.SetActive(State);
    }
    
    public void ResetSettings()
    {
        PlayerPrefs.DeleteAll();
    }

    void OnGUI()
    {
        ZoomT.text = (Zoom.value / 10).ToString();
        RotXT.text = (RotX.value / 10).ToString();
        RotYT.text = (RotY.value / 10).ToString();
        TraverseT.text = (Traverse.value / 10).ToString();
    }

    public void SetSettings()
    {
        PlayerPrefs.SetFloat("Zoom", Zoom.value);
        PlayerPrefs.SetFloat("RotX", RotX.value);
        PlayerPrefs.SetFloat("RotY", RotY.value);
        PlayerPrefs.SetFloat("Traverse", Traverse.value);
    }
}
