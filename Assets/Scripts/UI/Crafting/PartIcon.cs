using Items.Parts;
using System.Collections.Generic;
using UnityAsync;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Items.Parts.ItemPart.AttachmentPoint;

namespace UI.Crafting
{
    public class PartIcon : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public static float PartScaling = 2f;
        public List<(Collider2D self, Collider2D other)> ValidAttachments = new List<(Collider2D self, Collider2D other)>();
        public DesignUI DesignScript;

        private RectTransform rTransform;
        private (AttachmentIcon self, AttachmentIcon other) AttachedInput;
        private Transform ItemContainer;
        public ItemPart Part { get; set; }

        private bool IsDragging = false;


        private void Start()
        {
            rTransform = GetComponent<RectTransform>();
            ItemContainer = transform.parent;
            UpdateSize();
        }

        private async void OnEnable()
        {
            await Await.Until(() => Controller.InputControls != null);
            Controller.InputControls.Generic.Rotate.performed += OnRotatePerformed;
        }

        private void Update()
        {
            UpdateSize();
            if(AttachedInput.other)
            {
                UpdatePosition();
            }
        }

        public void UpdateSize()
        {
            //TODO subscribe this to an event on stat change
            float xSize = (int)Part.ModifiableStats["SizeX"].Value * PartScaling;
            float ySize = (int)Part.ModifiableStats["SizeY"].Value * PartScaling;
            rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xSize);
            rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ySize);
        }
        public void UpdatePosition()
        {
            Vector3 offset = AttachedInput.other.transform.position - AttachedInput.self.transform.position;
            offset = new Vector3(offset.x + AttachedInput.self.Normal.x * 4, offset.y + AttachedInput.self.Normal.y * 4, offset.z);
            transform.position += offset;
        }

        private void OnRotatePerformed(InputAction.CallbackContext context)
        {
            if (IsDragging)
            {
                if (context.ReadValue<float>() == -1)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z + RotationStep));
                }
                else if (context.ReadValue<float>() == 1)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.rotation.eulerAngles.z - RotationStep));
                } 
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            IsDragging = true;
            if (AttachedInput.other)
            {
                Detatch();
            }
            Vector2 newPosition = rTransform.anchoredPosition;
            newPosition += eventData.delta;
            newPosition.x = Mathf.Clamp(newPosition.x, -DesignUI.WorkAreaRadius, DesignUI.WorkAreaRadius);
            newPosition.y = Mathf.Clamp(newPosition.y, -DesignUI.WorkAreaRadius, DesignUI.WorkAreaRadius);
            rTransform.anchoredPosition = newPosition;
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            IsDragging = false;
            (Collider2D self, Collider2D collider, float distance) closestAttachment = (null, null, 0);
            foreach((Collider2D self, Collider2D other) in ValidAttachments)
            {
                float distance = self.Distance(other).distance;
                if (closestAttachment.collider == null || distance < closestAttachment.distance)
                {
                    closestAttachment = (self, other, distance);
                }
            }

            if(closestAttachment.collider != null)
            {
                AttachmentIcon selfIcon = closestAttachment.self.GetComponent<AttachmentIcon>();
                AttachmentIcon otherIcon = closestAttachment.collider.GetComponent<AttachmentIcon>();

                int selfIconDirection = ((int)selfIcon.Point.Direction - Mathf.RoundToInt(transform.rotation.eulerAngles.z % 360 / RotationStep));
                int otherIconDirection = ((int)otherIcon.Point.Direction - Mathf.RoundToInt(otherIcon.PartIconScript.transform.rotation.eulerAngles.z % 360 / RotationStep));
                int directionDiff = otherIconDirection - selfIconDirection;
                
                int zRotOffset = (2 - directionDiff) * RotationStep;

                Vector3 targetRotation = new Vector3(0, 0, Mathf.Round(transform.rotation.eulerAngles.z / RotationStep) * RotationStep + zRotOffset);
                transform.rotation = Quaternion.Euler(targetRotation);

                Vector2 offset = closestAttachment.collider.transform.position - closestAttachment.self.transform.position;

                if (selfIcon.TargetFlag == ItemPart.AttachmentTypeFlags.Output)
                {
                    Attach(selfIcon, otherIcon, offset);
                }
                else
                {
                    otherIcon.PartIconScript.Attach(otherIcon, selfIcon, -offset);
                }
                otherIcon.PartIconScript.ValidAttachments.Clear();
            }
            
            ValidAttachments.Clear();
        }

        private void Attach(AttachmentIcon selfIcon, AttachmentIcon targetIcon, Vector2 offset)
        {
            rTransform.pivot = selfIcon.Point.Position;

            selfIcon.Point.Attach(targetIcon.Point);
            transform.SetParent(targetIcon.PartIconScript.transform);

            offset += (rTransform.pivot - new Vector2(0.5f,0.5f)) * rTransform.rect.size;
            offset += targetIcon.Normal * 4f;
            transform.position += new Vector3(offset.x, offset.y);
            transform.SetSiblingIndex(0);
            AttachedInput = (selfIcon, targetIcon);

        }

        private void Detatch()
        {
            AttachedInput.other.Point.Detach();
            transform.SetParent(ItemContainer);
            AttachedInput = (null, null);
            rTransform.pivot = new Vector2(0.5f, 0.5f);
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            DesignScript.CraftingScript.SelectedPart = this;
        }
    } 
}
