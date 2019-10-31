using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface IGroupableUI
{
    RectTransform RTransform { get; set; }
    float CalculateSize();
    void SetSize(float Size);
    Bounds GetBounds();
}
