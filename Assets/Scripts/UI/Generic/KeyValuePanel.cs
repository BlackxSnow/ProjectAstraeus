using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ItemModule.AdditionalModule;

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
    public delegate string GetValueDelegate(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum);
    public GetValueDelegate GetValue;
    public Enum GetValueEnum;
    public bool DoNotUpdate = false;
    public struct RefStruct
    {
        public ItemModule.AdditionalModule RefModule;
        public ItemData RefItem;
        public StatsAndSkills RefStats;
    }
    public RefStruct Refs = new RefStruct();

    public void UpdateValue()
    {
        if (DoNotUpdate) return;

        Value.TextMesh.text = GetValue(Refs.RefModule, Refs.RefItem, Refs.RefStats, GetValueEnum);
    }

    public static class ModuleGetValue
    {
        public static string Armour(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals((RefModule as Plating).Armour, 1));
        public static string Cost_Iron(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", RefModule.Cost.Iron);
        public static string Cost_Copper(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", RefModule.Cost.Copper);
        public static string Cost_Alloy(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", RefModule.Cost.Alloy);
        public static string MassMod(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefModule.MassMultiplier, 1));
        public static string Power(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals((RefModule as Reactor).Power, 1));
        public static string PowerUsage(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals((RefModule as Shielding).PowerUsage, 1));
        public static string Shield(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals((RefModule as Shielding).Shield, 1));
        public static string SizeMod(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", RefModule.SizeMultiplier, 1);
    }

    public static class ItemGetValue
    {
        public static string Armour(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Armour, 1));
        public static string ArmourPiercing(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.ArmourPiercing, 1));
        public static string AttackSpeed(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.AttackSpeed, 1));
        public static string Cost_Iron(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Cost.Iron, 1));
        public static string Cost_Copper(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Cost.Copper, 1));
        public static string Cost_Alloy(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Cost.Alloy, 1));
        public static string Damage(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Damage, 1));
        public static string Mass(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Mass, 1));
        public static string Power(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Power, 1));
        public static string PowerUse(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.PowerUse, 1));
        public static string Range(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Range, 1));
        public static string Shield(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", Utility.RoundToNDecimals(RefItem.Stats.Shield, 1));
        public static string Size(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum ValueEnum) => string.Format("{0}", RefItem.Stats.Size, 1);
    }

    public static string GetStat(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum StatEnum) => string.Format("{0}", RefStats.Stats[(StatsAndSkills.StatsEnum)StatEnum]);
    public static string GetSkill(ItemModule.AdditionalModule RefModule, ItemData RefItem, StatsAndSkills RefStats, Enum SkillEnum) => string.Format("{0}", RefStats.Skills[(StatsAndSkills.SkillsEnum)SkillEnum]);
}
