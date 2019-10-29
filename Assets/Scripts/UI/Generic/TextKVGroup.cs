using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextKVGroup : MonoBehaviour, IGroupableUI
{
    public TextMeshProUGUI TextComponent;
    public KeyValueGroup Group;

    public KeyValueGroup.FontSizes Font = new KeyValueGroup.FontSizes(8, 72);

    public void SetSize(float TargetSize)
    {
        TextComponent.fontSize = TargetSize;
        TextComponent.ForceMeshUpdate();
    }
    public float CalculateSize()
    {
        TextComponent.enableAutoSizing = true;
        TextComponent.ForceMeshUpdate();
        float MinFont = TextComponent.fontSize;
        if (!Group) MinFont = Mathf.Clamp(MinFont, Font.Min, Font.Max);

        TextComponent.enableAutoSizing = false;

        return MinFont;
    }
}
