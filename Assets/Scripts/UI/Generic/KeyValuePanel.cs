using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static ItemModule.AdditionalModule;

public class KeyValuePanel : MonoBehaviour
{
    public KeyValueGroup Group;
    public float GroupTargetSize;

    struct FontSizes
    {
        public float Min;
        public float Max;

        public FontSizes(float Min, float Max)
        {
            this.Min = Min;
            this.Max = Max;
        }
    }

    FontSizes Font = new FontSizes(8, 72);

    //Storing useful components rather than calling GetComponent<> multiple times
    public struct ChildData
    {
        public GameObject @Object;
        public TextMeshProUGUI TextMesh;

        public ChildData(GameObject gameObject)
        {
            this.Object = gameObject;
            this.TextMesh = gameObject.GetComponent<TextMeshProUGUI>();
        }
    }
    
    public ChildData Key;
    public ChildData Value;

    RectTransform RectComponent;

    // Start is called before the first frame update
    void Awake()
    {
        RectComponent = GetComponent<RectTransform>();
        Key = new ChildData(gameObject.transform.GetChild(0).gameObject);
        Value = new ChildData(gameObject.transform.GetChild(1).gameObject);
    }

    Vector2 LastSize = new Vector2(0,0);
    private void Update()
    {
        if(RectComponent.rect.size != LastSize)
        {
            if (!Group)
                SetSize();
            else
                Group.SetDirty();
            LastSize = RectComponent.rect.size;
        }
        if(Group && (Key.TextMesh.fontSize != GroupTargetSize || Value.TextMesh.fontSize != GroupTargetSize))
        {
            SetSize(GroupTargetSize);
        }
    }

    public void SetSize(float TargetSize)
    {
        Key.TextMesh.fontSize = TargetSize;
        Value.TextMesh.fontSize = TargetSize;

        if (Group) GroupTargetSize = TargetSize;

        Key.TextMesh.ForceMeshUpdate();
        Value.TextMesh.ForceMeshUpdate();
    }
    public void SetSize()
    {
        float TargetSize = CalculateSize();
        
        Key.TextMesh.fontSize = TargetSize;
        Value.TextMesh.fontSize = TargetSize;
        Key.TextMesh.ForceMeshUpdate();
        Value.TextMesh.ForceMeshUpdate();
    }

    public float CalculateSize()
    {
        Key.TextMesh.enableAutoSizing = true;
        Value.TextMesh.enableAutoSizing = true;
        Key.TextMesh.ForceMeshUpdate();
        Value.TextMesh.ForceMeshUpdate();

        float MinFont = Mathf.Min(Key.TextMesh.fontSize, Value.TextMesh.fontSize);
        MinFont = Mathf.Clamp(MinFont, Font.Min, Font.Max);

        Key.TextMesh.enableAutoSizing = false;
        Value.TextMesh.enableAutoSizing = false;
        return MinFont;
    }

    public delegate string GetValueDelegate();
    public GetValueDelegate GetValue;
    public bool DoNotUpdate = false;

    public void UpdateValue()
    {
        if(!DoNotUpdate)    Value.TextMesh.text = GetValue();
    }

    public static class ModuleGetValue
    {
        public static string Armour() =>        string.Format("{0}", Utility.RoundToNDecimals(( CraftingUI.CurrentModule as Plating).Armour,        1));
        public static string Cost_Iron() =>     string.Format("{0}",                            CraftingUI.CurrentModule.Cost.Iron                   );
        public static string Cost_Copper() =>   string.Format("{0}",                            CraftingUI.CurrentModule.Cost.Copper                 );
        public static string Cost_Alloy() =>    string.Format("{0}",                            CraftingUI.CurrentModule.Cost.Alloy                  );
        public static string MassMod() =>       string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentModule.MassMultiplier,            1));
        public static string Power() =>         string.Format("{0}", Utility.RoundToNDecimals(( CraftingUI.CurrentModule as Reactor).Power,         1));
        public static string PowerUsage() =>    string.Format("{0}", Utility.RoundToNDecimals(( CraftingUI.CurrentModule as Shielding).PowerUsage,  1));
        public static string Shield() =>        string.Format("{0}", Utility.RoundToNDecimals(( CraftingUI.CurrentModule as Shielding).Shield,      1));
        public static string SizeMod() =>       string.Format("{0}",                            CraftingUI.CurrentModule.SizeMultiplier,            1);
    }

    public static class ItemGetValue
    {
        public static string Armour() =>            string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Armour,            1));
        public static string ArmourPiercing() =>    string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.ArmourPiercing,    1));
        public static string AttackSpeed() =>       string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.AttackSpeed,       1));
        public static string Cost_Iron() =>         string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Cost.Iron,         1));
        public static string Cost_Copper() =>       string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Cost.Copper,       1));
        public static string Cost_Alloy() =>        string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Cost.Alloy,        1));
        public static string Damage() =>            string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Damage,            1));
        public static string Mass() =>              string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Mass,              1));
        public static string Power() =>             string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Power,             1));
        public static string PowerUse() =>          string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.PowerUse,          1));
        public static string Range() =>             string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Range,             1));
        public static string Shield() =>            string.Format("{0}", Utility.RoundToNDecimals(  CraftingUI.CurrentItem.Stats.Shield,            1));
        public static string Size() =>              string.Format("{0}",                            CraftingUI.CurrentItem.Stats.Size,              1);
    }
}
