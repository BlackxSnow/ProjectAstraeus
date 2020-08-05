using Boo.Lang;
using Items;
using Items.Parts;
using UI.Control;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Crafting
{
    public class DesignUI : MonoBehaviour, IDragHandler
    {
#pragma warning disable 0649
        public CraftingUI CraftingScript;
        [SerializeField]
        private Image UI_GridImage;
        [SerializeField]
        private RectTransform UI_ItemContainer;
#pragma warning restore 0649

        private Vector2 Offset = new Vector2();
        private Vector2 TexelOffset = new Vector2();
        private Vector2 TextureSize;

        // Start is called before the first frame update
        void Start()
        {
            CheckReferences();
            UI_GridImage.material.SetTextureOffset("_MainTex", new Vector2(0, 0));
            Material gridMaterial = new Material(UI_GridImage.material);
            UI_GridImage.material = gridMaterial;
            TextureSize = UI_GridImage.sprite.texture.texelSize;
        }

        private void CheckReferences()
        {
            List<bool> checks = new List<bool>
            {
                CraftingScript != null,
                UI_GridImage != null,
                UI_ItemContainer != null
            };

            foreach (bool check in checks)
            {
                if (!check)
                {
                    Debug.LogWarning("One or more serialized fields is null");
                    return;
                }
            }
        }


        public const float WorkAreaRadius = 500;
        private const float MinReturnSpeed = 1;
        private const float ReturnPercentage = 0.05f;
        public void OnDrag(PointerEventData eventData)
        {
            OffsetGrid(-eventData.delta);
        }

        private void Update()
        {
            if (!Utility.Math.WithinBounds(Offset, 500))
            {
                GridElasticReturn();
            }
        }

        private void GridElasticReturn()
        {
            Vector2 offset = new Vector2();
            if (Mathf.Abs(Offset.x) > WorkAreaRadius)
            {
                offset.x += Mathf.Max(MinReturnSpeed, (Mathf.Abs(Offset.x) - 500f) * ReturnPercentage) * -(Offset.x / Mathf.Abs(Offset.x));
            }
            if (Mathf.Abs(Offset.y) > WorkAreaRadius)
            {
                offset.y += Mathf.Max(MinReturnSpeed, (Mathf.Abs(Offset.y) - 500f) * ReturnPercentage) * -(Offset.y / Mathf.Abs(Offset.y));
            }
            OffsetGrid(offset);
        }

        private void OffsetGrid(Vector2 offset)
        {
            Offset += offset;
            TexelOffset.x += offset.x * TextureSize.x;
            TexelOffset.y += offset.y * TextureSize.y;
            UI_GridImage.material.mainTextureOffset = TexelOffset;
            UI_ItemContainer.anchoredPosition = -Offset;
        }

        public void CreatePart(ItemPart part)
        {
            GameObject obj = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.Crafting_PartIcon], UI_ItemContainer.transform);
            PartIcon partScript = obj.GetComponent<PartIcon>();
            partScript.DesignScript = this;
            partScript.Part = part;

            foreach(ItemPart.AttachmentPoint point in part.AttachmentPoints)
            {
                GameObject pointObj = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.Crafting_AttachmentPoint], obj.transform);
                AttachmentIcon pointScript = pointObj.GetComponent<AttachmentIcon>();
                pointScript.Point = point;
                Image pointImage = pointObj.GetComponent<Image>();

                if(point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Input))
                {
                    pointImage.sprite = UIController.LoadedSprites[UIController.SpritesEnum.Crafting_Attachment_Input];
                }
                else if (point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Secondary))
                {
                    pointImage.sprite = UIController.LoadedSprites[UIController.SpritesEnum.Crafting_Attachment_Secondary];
                }
                else if (point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Primary))
                {
                    pointImage.sprite = UIController.LoadedSprites[UIController.SpritesEnum.Crafting_Attachment_Primary];
                }
            }

            if(part.IsCore && CraftingScript.CurrentDesign.CorePart == null)
            {
                CraftingScript.CurrentDesign.CorePart = part;
            }
        }
    } 
}
