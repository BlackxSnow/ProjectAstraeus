using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerProxy : MonoBehaviour
{
    public void LoadScene(int Index)
    {
        Controller.LoadScene(Index);
    }
}
