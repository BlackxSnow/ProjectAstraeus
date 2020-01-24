using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextKVGroup : MonoBehaviour, IGroupableUI
{
    public TextMeshProUGUI TextComponent;
    private KeyValueGroup group;
    public KeyValueGroup Group
    {
        get { return group; }
        set
        {
            group = value;
            if(value != null)
                group.AddMember(this);
        }
    }
    public RectTransform RTransform { get; set; }
    public KeyValueGroup.FontSizes Font = new KeyValueGroup.FontSizes(8, 72);
    VerticalLayoutGroup VLayout;
    public bool InList { get; set; }

    public float SelfFontSize;
    protected virtual void Awake()
    {
        
    }

    void OnDestroy()
    {
        if (Group)
        {
            Group.RemoveMember(this);
        }
    }

    public virtual void Init()
    {
        RTransform = GetComponent<RectTransform>();
        if (InList)
        {
            if (this is KeyValuePanel)
            {
                VLayout = transform.parent.parent.parent.GetComponent<VerticalLayoutGroup>();
            } else
            {
                VLayout = transform.parent.parent.GetComponent<VerticalLayoutGroup>();
            }
        } else
        {
            VLayout = GetComponentInParent<VerticalLayoutGroup>();
        }
    }

    public virtual Bounds GetBounds()
    {
        return TextComponent.bounds;
    }

    public virtual void SetSize(float TargetSize)
    {
        TextComponent.fontSize = TargetSize;
        float VerticalSize = (TargetSize / 90) * 100;
        if (!TextComponent || TextComponent.enableWordWrapping == false)
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

        if(!TextComponent || TextComponent.enableWordWrapping == false)
        {
            RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, VerticalSize);
        }


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

        float ExpandedSize = (VLayout.transform.GetComponent<RectTransform>().rect.height - (VLayout.spacing * VLayout.transform.childCount + VLayout.padding.vertical)) / GetEffectiveChildCount() ;
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
        SelfFontSize = TargetText.fontSize;
    }

    int LastChildCount;
    List<KeyValueList> ChildLists;
    List<TextKVGroup> ChildTexts;

    public int GetEffectiveChildCount()
    {
        Transform Target = InList ? transform.parent.parent.parent : transform.parent;
        int EffectiveCount = 0;

        if (Target.childCount != LastChildCount)
        {
            ChildTexts = GetChildrenComponents(Target, out ChildLists);
            LastChildCount = Target.childCount;
        }
        EffectiveCount += ChildTexts.Count;
        foreach (KeyValueList KVL in ChildLists)
        {
            EffectiveCount += KVL.ContentPanel.transform.childCount + 1; //+1 for Title

        }
        return EffectiveCount;
    }

    private List<TextKVGroup> GetChildrenComponents(Transform Target, out List<KeyValueList> KeyValueLists)
    {
        List<TextKVGroup> TextKVs = new List<TextKVGroup>();
        KeyValueLists = new List<KeyValueList>();

        for (int i = 0; i < Target.childCount; i++)
        {
            Transform Child = Target.GetChild(i);
            TextKVGroup TextKV = Child.GetComponent<TextKVGroup>();
            KeyValueList KVList = Child.GetComponent<KeyValueList>();
            if (TextKV)
            {
                TextKVs.Add(TextKV);
            } else if (KVList)
            {
                KeyValueLists.Add(KVList);
            }
        }

        return TextKVs;
    }
}
