using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Modules;
using static ItemTypes;

public class KeyValuePanel : TextKVGroup, IGroupableUI
{
    
    public float GroupTargetSize;
    public Image Icon;
    public Sprite WarningIcons;

    public struct ChildData
    {
        public GameObject @Object;
        public TextMeshProUGUI TextMesh;
        public RectTransform RTransform;

        public ChildData(GameObject gameObject)
        {
            Object = gameObject;
            TextMesh = gameObject.GetComponent<TextMeshProUGUI>();
            RTransform = gameObject.GetComponent<RectTransform>();
        }
    }
    
    public ChildData Key;
    public ChildData Value;

    protected override void Awake()
    {
        base.Awake();
        Key = new ChildData(gameObject.transform.GetChild(0).gameObject);
        Value = new ChildData(gameObject.transform.GetChild(1).gameObject);
    }

    Vector2 LastSize = new Vector2(0,0);
    private void Update()
    {
        if(RTransform.rect.size != LastSize)
        {
            //Debug.Log(string.Format("KVP '{0}' is calling SetDirty", Key.TextMesh.text));
            if (!Group)
                SetSize();
            else
                Group.SetDirty();
            LastSize = RTransform.rect.size;
        }
        if(Group && (Key.TextMesh.fontSize != GroupTargetSize || Value.TextMesh.fontSize != GroupTargetSize))
        {
            SetSize(GroupTargetSize);
        }
    }

    public override Bounds GetBounds()
    {
        return Key.TextMesh.bounds.size.y > Value.TextMesh.bounds.size.y ? Key.TextMesh.bounds : Value.TextMesh.bounds;
    }

    public override void SetSize(float TargetSize)
    {
        float VerticalSize = (TargetSize / 90) * 100;

        Key.TextMesh.fontSize = TargetSize;
        Value.TextMesh.fontSize = TargetSize;
        RTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, VerticalSize);

        if (Group) GroupTargetSize = TargetSize;

        Key.TextMesh.ForceMeshUpdate();
        Value.TextMesh.ForceMeshUpdate();
    }
    public void SetSize()
    {
        float TargetSize = CalculateSize(Key.TextMesh, Value.TextMesh);
        
        Key.TextMesh.fontSize = TargetSize;
        Value.TextMesh.fontSize = TargetSize;

        Key.TextMesh.ForceMeshUpdate();
        Value.TextMesh.ForceMeshUpdate();
    }
    public override float CalculateSize()
    {
        return CalculateSize(Key.TextMesh, Value.TextMesh);
    }

    void EnableWarningIcon()
    {
        Icon.color = Color.white;
        Icon.sprite = WarningIcons;
    }

    void DisableWarningIcon()
    {
        Icon.color = new Color(0, 0, 0, 0);
    }

    //Relating to Value
    public delegate string GetValueDelegate(AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum ValueEnum);
    public GetValueDelegate GetValue;
    public Enum GetValueEnum;
    public bool DoNotUpdate = false;
    public struct RefStruct
    {
        public AdditionalModule RefModule;
        public Item RefItem;
        public StatsAndSkills RefStats;
    }
    public RefStruct Refs = new RefStruct();

    public void UpdateValue()
    {
        if (DoNotUpdate) return;
        Value.TextMesh.text = GetValue(Refs.RefModule, Refs.RefItem, Refs.RefStats, GetValueEnum);
        if (GetValueEnum is SubTypes Subtype)
        {
            if (Value.TextMesh.text == SubTypes.Invalid.ToString())
            { EnableWarningIcon(); }
            else { DisableWarningIcon(); }
        }
    }

    public static string GetModuleStat(AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum ValueEnum)
    {
        if (ValueEnum is Resources.ResourceList ResourceEnum)
        {
            return string.Format("{0}", RefModule.GetStat<Resources>(StatsEnum.Cost)[ResourceEnum]);
        }
        if ((StatsEnum)ValueEnum == StatsEnum.SizeMod)
        {
            return string.Format("{0}", RefModule.GetStat<Vector2>(StatsEnum.SizeMod));
        }
        if ((StatsEnum)ValueEnum == StatsEnum.Material)
        {
            return string.Format("{0}", RefModule.GetStat<string>(StatsEnum.Material));
        }
        else if ((StatsEnum)ValueEnum == StatsEnum.FireMode)
        {
            return string.Format("{0}", RefModule.GetStat<Firearm.FireModes>(StatsEnum.FireMode));
        }
        return string.Format("{0}", Utility.Math.RoundToNDecimals(RefModule.GetStat<float>((StatsEnum)ValueEnum), 1));
    }
    public static string GetItemStat(AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum ValueEnum)
    {
        if (ValueEnum is Resources.ResourceList ResourceEnum)
        {
            return string.Format("{0}", RefItem.Stats.GetStat<Resources>(StatsEnum.Cost)[ResourceEnum]);
        }
        if ((StatsEnum)ValueEnum == StatsEnum.Size)
        {
            return string.Format("{0}", RefItem.Stats.GetStat<Vector2Int>(StatsEnum.Size));
        }
        if (ValueEnum is SubTypes)
        {
            SubTypes Subtype = ((EquippableItem)RefItem).Subtype;
            return Subtype.ToString();
        }
        return string.Format("{0}", Utility.Math.RoundToNDecimals(RefItem.Stats.GetStat<float>((StatsEnum)ValueEnum), 1));
    }
    public static string GetStat(AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum StatEnum) => string.Format("{0}", RefStats.Stats[(StatsAndSkills.StatsEnum)StatEnum]);
    public static string GetSkill(AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum SkillEnum) => string.Format("{0}", RefStats.Skills[(StatsAndSkills.SkillsEnum)SkillEnum]);
}
