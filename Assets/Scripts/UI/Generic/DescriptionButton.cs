using UnityEngine;
using TMPro;
public class DescriptionButton : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;

    private void Awake()
    {
        Title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Description = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
}
