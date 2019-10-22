using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEntity : Entity, ISelectable
{
    public bool Selected { get; set; }
    public bool ViewableOnly { get; set; }
    public bool Circled { get; set; }
    public bool FinalisedSelection { get; set; }
    public GameObject ISelectablegameObject { get; set; }

    public GameObject SelectObj;
    GameObject SelectObjInstance;
    Projector SelectObjProjector;

    protected Collider colliderComponent;

    protected override void Start()
    {
        ISelectablegameObject = gameObject;
        SelectObjInstance = Instantiate(SelectObj, transform);
        SelectObjProjector = SelectObjInstance.GetComponent<Projector>();
        colliderComponent = GetComponent<Collider>();
        SelectObjUpdate();
        base.Start();
        EntityManager.RegisterSelectable(this);
        if (FactionID != 0)
        {
            ViewableOnly = true;
        }
        else
        {
            ViewableOnly = false;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        SelectControl();
        base.Update();
    }

    public void SelectControl()
    {
        SelectObjUpdate();
        if (Selected && !Circled)
        {
            SelectObjInstance.SetActive(true);
            Circled = true;
        }
        else if (!Selected && Circled)
        {
            SelectObjInstance.SetActive(false);
            Circled = false;
        }
    }

    void SelectObjUpdate() //Updates the size of the select projection according to the bounding box.
    {
        float _BoundsSize = Mathf.Max(colliderComponent.bounds.size.x, colliderComponent.bounds.size.z);
        SelectObjProjector.orthographicSize = _BoundsSize + 0.25f;
    }

    public virtual GameObject[] InstantiateStatDisplay() { return null; }
}
