using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ItemTypes;

namespace UI
{
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

        Vector2 LastSize = new Vector2(0, 0);
        private void Update()
        {
            if (RTransform.rect.size != LastSize)
            {
                //Debug.Log(string.Format("KVP '{0}' is calling SetDirty", Key.TextMesh.text));
                if (!Group)
                    SetSize();
                else
                    Group.SetDirty();
                LastSize = RTransform.rect.size;
            }
            if (Group && (Key.TextMesh.fontSize != GroupTargetSize || Value.TextMesh.fontSize != GroupTargetSize))
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
        public Func<string> GetValue;
        public bool DoNotUpdate = false;

        public void UpdateValue()
        {
            if (DoNotUpdate) return;

            Value.TextMesh.text = GetValue();
        }

    } 
}
