using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyValuePanel : MonoBehaviour
{
    //Storing useful components rather than calling GetComponent<> multiple times
    public struct ChildData
    {
        public GameObject @Object;
        public TextMeshProUGUI TextMesh;

        public ChildData(GameObject gameObject)
        {
            this.Object = gameObject;
            this.TextMesh = gameObject.GetComponent<TextMeshProUGUI>();
        }
    }
    
    public ChildData Key;
    public ChildData Value;

    // Start is called before the first frame update
    void Awake()
    {
        Key = new ChildData(gameObject.transform.GetChild(0).gameObject);
        Value = new ChildData(gameObject.transform.GetChild(1).gameObject);
    }
}
