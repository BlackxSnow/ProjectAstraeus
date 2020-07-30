using Items.Parts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Crafting
{
    public class DesignUI : MonoBehaviour, IDragHandler
    {
        private const string GridName = "BackingGrid";
        private const string ItemContainerName = "ItemContainer";
        private Image GridImage;
        private RectTransform ItemContainer;

        private Vector2 Offset = new Vector2();
        private Vector2 TexelOffset = new Vector2();
        private Vector2 TextureSize;

        // Start is called before the first frame update
        void Start()
        {
            GridImage = transform.Find(GridName).GetComponent<Image>();
            ItemContainer = transform.Find(ItemContainerName).GetComponent<RectTransform>();
            GridImage.material.SetTextureOffset("_MainTex", new Vector2(0, 0));
            Material gridMaterial = new Material(GridImage.material);
            GridImage.material = gridMaterial;
            TextureSize = GridImage.sprite.texture.texelSize;
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
            GridImage.material.mainTextureOffset = TexelOffset;
            ItemContainer.anchoredPosition = -Offset;
        }

        public void InstantiatePart(ItemPart part)
        {

        }
    } 
}
