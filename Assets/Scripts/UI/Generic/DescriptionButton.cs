using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DescriptionButton : MonoBehaviour
{
    [HideInInspector]
    public TextMeshProUGUI Title;
    [HideInInspector]
    public TextMeshProUGUI Description;
    [HideInInspector]
    public Button button;

    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Description = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
}
