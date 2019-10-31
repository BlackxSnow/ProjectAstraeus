using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextKVGroup : MonoBehaviour, IGroupableUI
{
    public TextMeshProUGUI TextComponent;
    public KeyValueGroup Group;
    public RectTransform RTransform { get; set; }

    public KeyValueGroup.FontSizes Font = new KeyValueGroup.FontSizes(8, 72);

    protected virtual void Awake()
    {
        RTransform = GetComponent<RectTransform>();
    }

    public virtual Bounds GetBounds()
    {
        return TextComponent.bounds;
    }

    public virtual void SetSize(float TargetSize)
    {
        TextComponent.fontSize = TargetSize;
        float VerticalSize = (TargetSize / 90) * 100;
        RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, VerticalSize);
        TextComponent.ForceMeshUpdate();
    }

    protected virtual float CalculateSize(params TextMeshProUGUI[] Meshes)
    {
        float[] SizeArray = new float[Meshes.Length];
        for(int i = 0; i < Meshes.Length; i++)
        {
            AutoSize(Meshes[i], false);
            Meshes[i].ForceMeshUpdate();
            SizeArray[i] = Meshes[i].fontSize;
        }

        float MinFont = Mathf.Min(SizeArray);
        if (!Group) MinFont = Mathf.Clamp(MinFont, Font.Min, Font.Max);
        float VerticalSize = (MinFont / 90) * 100;
        RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, VerticalSize);

        return MinFont;
    }
    public virtual float CalculateSize()
    {
        return CalculateSize(TextComponent);
    }
    protected virtual void AutoSize(TextMeshProUGUI TargetText, bool Resize = true)
    {
        if (!TargetText) return;
        float FontSizePercentage = 90f;
        RectTransform TextRTransform = TargetText.GetComponent<RectTransform>();
        float ExpandedSize = (transform.parent.GetComponent<RectTransform>().rect.height - GetComponentInParent<VerticalLayoutGroup>().spacing * (transform.parent.childCount - 1)) / transform.parent.childCount ;
        RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ExpandedSize);

        TargetText.fontSize = RTransform.rect.height * (FontSizePercentage / 100);
        TargetText.ForceMeshUpdate();

        float SizeRatio = TextRTransform.rect.width / TargetText.bounds.size.x;
        if (SizeRatio < 1)
        {
            TargetText.fontSize *= SizeRatio;
            TargetText.ForceMeshUpdate();
            float VerticalSize = (TargetText.fontSize / 90) * 100;
            if (Resize) RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, VerticalSize);
        }
    }
}
