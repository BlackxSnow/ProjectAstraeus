using Items.Parts;
using UI.Crafting;
using UnityEngine;
using UnityEngine.UI;

public class AttachmentIcon : MonoBehaviour
{

    public Vector2 Normal => CalculateNormal();
    [HideInInspector]
    public ItemPart.AttachmentPoint Point;
    [HideInInspector]
    public PartIcon PartIconScript { get; protected set; }
    private Collider2D SelfCollider;
    public ItemPart.AttachmentTypeFlags TargetFlag { get; private set; }

    private Image image;

    private RectTransform selfRect;
    private RectTransform parentRect;

    private Vector2 CalculateNormal()
    {
        Vector3 v3Normal = transform.position - PartIconScript.transform.position;
        Vector2 normal = new Vector2(v3Normal.x, v3Normal.y);
        normal /= normal.magnitude;
        return normal;
    }

    private void Start()
    {
        SelfCollider = GetComponent<Collider2D>();
        PartIconScript = GetComponentInParent<PartIcon>();
        image = GetComponent<Image>();
        TargetFlag = Point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Input) ? ItemPart.AttachmentTypeFlags.Output : ItemPart.AttachmentTypeFlags.Input;

        selfRect = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();

        selfRect.anchorMin = new Vector2(0f, 0f);
        selfRect.anchorMax = new Vector2(0f, 0f);
        selfRect.rotation = Quaternion.Euler(new Vector3(0, 0, (int)Point.Direction * ItemPart.AttachmentPoint.RotationStep));
        UpdatePosition();
       
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        //TODO fix offset calculation
        Vector2 normal = Normal;
        Vector3 offset = Point.AttachmentFlags.HasFlag(ItemPart.AttachmentTypeFlags.Input) ? new Vector3(-normal.x, -normal.y) * selfRect.rect.width / 2.0f : Vector3.zero;
        selfRect.anchoredPosition = new Vector3(parentRect.rect.width * Point.Position.x, parentRect.rect.height * Point.Position.y) + offset;
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
