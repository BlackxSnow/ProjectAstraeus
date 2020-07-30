using Items.Parts;
using System.Collections.Generic;
using UnityAsync;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Items.Parts.ItemPart.AttachmentPoint;

namespace UI.Crafting
{
    public class PartIcon : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public List<(Collider2D self, Collider2D other)> ValidAttachments = new List<(Collider2D self, Collider2D other)>();
        private RectTransform rTransform;
        private AttachmentIcon AttachedInput;
        private Transform ItemContainer;
        


        private void Start()
        {
            rTransform = GetComponent<RectTransform>();
            ItemContainer = transform.parent;
        }

        private void Update()
        {
            Debug.DrawRay(transform.position, transform.up * 50f, Color.red);
        }

        private async void OnEnable()
        {
            await Await.Until(() => Controller.InputControls != null);
            Controller.InputControls.Generic.Rotate.performed += OnRotatePerformed;
        }

        private bool IsDragging;
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
            if (AttachedInput)
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

                int selfIconDirection = ((int)selfIcon.Direction - Mathf.RoundToInt(transform.rotation.eulerAngles.z % 360 / RotationStep));
                int otherIconDirection = ((int)otherIcon.Direction - Mathf.RoundToInt(otherIcon.PartIconScript.transform.rotation.eulerAngles.z % 360 / RotationStep));
                int directionDiff = otherIconDirection - selfIconDirection;
                //int rotDir = selfIconDirection - otherIconDirection > 0 ? -1 : 1;
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
            selfIcon.Point.Attach(targetIcon.Point);
            transform.SetParent(targetIcon.PartIconScript.transform);
            offset += targetIcon.Normal * 4f;
            transform.position += new Vector3(offset.x, offset.y);
            transform.SetSiblingIndex(0);
            AttachedInput = targetIcon;
        }

        private void Detatch()
        {
            AttachedInput.Point.Detach();
            transform.SetParent(ItemContainer);
            AttachedInput = null;
        }
    } 
}
