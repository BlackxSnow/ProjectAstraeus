using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour, IDestroyHandler
{
    GameObject TargetObject;
    float MaxAmount;

    [Serializable]
    public struct BarStruct
    {
        public RectTransform Background;
        public RectTransform Foreground;
        public TextMeshProUGUI Text;
    }
    public BarStruct UIBar;

    public bool DestructionMarked { get; set; }

    public void UpdateBar(float current)
    {
        UIBar.Text.text = $"{Utility.RoundToNDecimals(current, 1)} / {Utility.RoundToNDecimals(MaxAmount, 1)}";
        UIBar.Foreground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UIBar.Background.rect.width * current / MaxAmount);
    }

    public void Init(GameObject parent, float current, float max, Color background, Color foreground)
    {
        MaxAmount = max;
        TargetObject = parent;
        UIBar.Foreground.GetComponent<Image>().color = foreground;
        UIBar.Background.GetComponent<Image>().color = background;
        UpdateBar(current);
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(TargetObject.transform.position);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
