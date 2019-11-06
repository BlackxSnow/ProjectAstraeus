using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ItemModule.AdditionalModule;
using static ItemTypes;

public class KeyValuePanel : TextKVGroup, IGroupableUI
{
    public float GroupTargetSize;

    //Storing useful components rather than calling GetComponent<> multiple times
    public struct ChildData
    {
        public GameObject @Object;
        public TextMeshProUGUI TextMesh;
        public RectTransform RTransform;

        public ChildData(GameObject gameObject)
        {
            this.Object = gameObject;
            this.TextMesh = gameObject.GetComponent<TextMeshProUGUI>();
            this.RTransform = gameObject.GetComponent<RectTransform>();
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

    //Relating to Value
    public delegate string GetValueDelegate(ItemModule.AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum ValueEnum);
    public GetValueDelegate GetValue;
    public Enum GetValueEnum;
    public bool DoNotUpdate = false;
    public struct RefStruct
    {
        public ItemModule.AdditionalModule RefModule;
        public Item RefItem;
        public StatsAndSkills RefStats;
    }
    public RefStruct Refs = new RefStruct();

    public void UpdateValue()
    {
        if (DoNotUpdate) return;

        Value.TextMesh.text = GetValue(Refs.RefModule, Refs.RefItem, Refs.RefStats, GetValueEnum);
    }

    public static string GetModuleStat(ItemModule.AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum ValueEnum)
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
        return string.Format("{0}", Utility.RoundToNDecimals(RefModule.GetStat<float>((StatsEnum)ValueEnum), 1));
    }
    public static string GetItemStat(ItemModule.AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum ValueEnum)
    {
        if (ValueEnum is Resources.ResourceList ResourceEnum)
        {
            return string.Format("{0}", RefItem.Stats.GetStat<Resources>(StatsEnum.Cost)[ResourceEnum]);
        }
        if ((StatsEnum)ValueEnum == StatsEnum.Size)
        {
            return string.Format("{0}", RefItem.Stats.GetStat<Vector2Int>(StatsEnum.Size));
        }
        return string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.GetStat<float>((StatsEnum)ValueEnum), 1));
    }
    public static string GetStat(ItemModule.AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum StatEnum) => string.Format("{0}", RefStats.Stats[(StatsAndSkills.StatsEnum)StatEnum]);
    public static string GetSkill(ItemModule.AdditionalModule RefModule, Item RefItem, StatsAndSkills RefStats, Enum SkillEnum) => string.Format("{0}", RefStats.Skills[(StatsAndSkills.SkillsEnum)SkillEnum]);
}
