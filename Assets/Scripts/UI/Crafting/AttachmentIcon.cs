using Items.Parts;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using UI.Crafting;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class AttachmentIcon : MonoBehaviour
{
    public Vector2 Normal => CalculateNormal();
    [HideInInspector]
    public ItemPart.AttachmentPoint Point;
    [HideInInspector]
    public PartIcon PartIconScript;
    private Collider2D SelfCollider;
    public ItemPart.AttachmentTypeFlags TargetFlag { get; private set; }
#if DEBUG
    public Vector2 Position;
    public ItemPart.AttachmentPoint.DirectionEnum Direction;
    public ItemPart.AttachmentTypeFlags Flags;
#endif

    private Image image;

    private Vector2 CalculateNormal()
    {
        Vector3 v3Normal = transform.position - PartIconScript.transform.position;
        Vector2 normal = new Vector2(v3Normal.x, v3Normal.y);
        normal /= normal.magnitude;
        return normal;
    }

    private void Start()
    {
#if DEBUG
        Point = new ItemPart.AttachmentPoint(Position, Direction, Flags);
#endif
        SelfCollider = GetComponent<Collider2D>();
        PartIconScript = GetComponentInParent<PartIcon>();
        image = GetComponent<Image>();
        TargetFlag = Point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Input) ? ItemPart.AttachmentTypeFlags.Output : ItemPart.AttachmentTypeFlags.Input;

        RectTransform selfRect = GetComponent<RectTransform>();
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        selfRect.anchorMin = new Vector2(0f, 0f);
        selfRect.anchorMax = new Vector2(0f, 0f);
        Vector3 offset = Point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Input) ? -transform.up * selfRect.rect.width / 2.0f : Vector3.zero;
        selfRect.anchoredPosition = new Vector3(parentRect.rect.width * Point.Position.x, parentRect.rect.height * Point.Position.y) + offset;
        selfRect.rotation = Quaternion.Euler(new Vector3(0, 0, (int)Point.Direction * ItemPart.AttachmentPoint.RotationStep));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.IsChildOf(transform.parent) && other.TryGetComponent(out AttachmentIcon icon) && icon.Point.AttachmentFlags.HasFlag(TargetFlag) && icon.Point.AttachedPoint == null)
        {
            //image.color = Color.green;
            PartIconScript.ValidAttachments.Add((SelfCollider, other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out AttachmentIcon icon) && icon.Point.AttachmentFlags.HasFlag(TargetFlag))
        {
            PartIconScript.ValidAttachments.Remove((SelfCollider, other));
            if (PartIconScript.ValidAttachments.Count == 0)
            {
                if (TargetFlag == ItemPart.AttachmentTypeFlags.Input)
                {
                    //image.color = new Color(1, 0.61f, 0.11f);
                }
                else
                {
                    //image.color = Color.blue;
                } 
            }
            
        }
    }
}
