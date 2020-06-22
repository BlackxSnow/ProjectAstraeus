using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour, IDestroyHandler
{
    GameObject TargetObject;
    float MaxAmount;
    bool HoverOnTarget;

    [Serializable]
    public struct BarStruct
    {
        public RectTransform Background;
        public RectTransform Foreground;
        public TextMeshProUGUI Text;
    }
    public BarStruct UIBar;

    public bool DestructionMarked { get; set; }

    public void UpdateBar(float current, bool percentage)
    {
        float currentAdjusted = percentage ? current : current / MaxAmount;
        UIBar.Text.text = $"{Utility.Math.RoundToNDecimals(currentAdjusted * MaxAmount, 1)} / {Utility.Math.RoundToNDecimals(MaxAmount, 1)}";
        UIBar.Foreground.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UIBar.Background.rect.width * currentAdjusted);
    }

    public void Init(float current, float max, Color background, Color foreground, GameObject parent = null)
    {
        if (parent)
        {
            TargetObject = parent;
            HoverOnTarget = true;
        }
        else
        {
            HoverOnTarget = false;
        }
        MaxAmount = max;
        UIBar.Foreground.GetComponent<Image>().color = foreground;
        UIBar.Background.GetComponent<Image>().color = background;
        UpdateBar(current, false);
    }

    private void Update()
    {
        if(HoverOnTarget)   transform.position = Camera.main.WorldToScreenPoint(TargetObject.transform.position);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
