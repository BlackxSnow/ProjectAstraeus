using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class Actor : DynamicEntity, IOrderable, IDamageable
{
    NavMeshAgent Agent;
    bool Subscribed;

    Inventory InventoryScript;
    Movement MovementScript;
    bool Movable;

    float PickupDistance = 1.5f;

    public Species Race;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Agent = GetComponent<NavMeshAgent>();
        MovementScript = GetComponent<Movement>();
        InventoryScript = GetComponent<Inventory>();

        if (MovementScript != null) Movable = true; else Movable = false;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Move(Vector3 Destination, FlockController flockController, bool StopAll = true)
    {
        if (StopAll)
            StopAllCoroutines();

        MovementScript.SetDestination(Destination, flockController);
    }

    public void Pickup(DynamicEntity Target)
    {
        StopAllCoroutines();
        StartCoroutine(PickupRoutine(Target));
    }

    //Behaviour to retrieve items
    IEnumerator PickupRoutine(DynamicEntity Target)
    {
        bool Moved = false;
        bool Finished = false;
        while(!Finished)
        {
            if (ProximityCheck(Target.gameObject))
            {
                if (Target is Item)
                    InventoryScript.AddItem(Target as Item);
                Finished = true;
            }
            else if (!Moved)
            {
                Move(Target.transform.position, null, false);
                Moved = true;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool ProximityCheck(GameObject Target)
    {
        if (Vector3.Distance(this.transform.position, Target.transform.position) <= PickupDistance)
            return true;
        else
            return false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Selected && FactionID == 0 && !Subscribed && Movable)
        {
            OrderEvents.OnMove += Move;
            OrderEvents.OnPickup += Pickup;
            Subscribed = true;
        } else if (!Selected && Subscribed)
        {
            OrderEvents.OnMove -= Move;
            OrderEvents.OnPickup -= Pickup;
            Subscribed = false;
        }
    }

    //Create medical info display for HUD
    public override List<GameObject> InstantiateStatDisplay()
    {
        GameObject MedicalPanel = Instantiate(UIController.ObjectPrefabs[UIController.ObjectPrefabsEnum.MedicalDetailsPrefab], HUDController.HUDControl.SelectHUD.DetailsPanel.transform);
        MedicalDetailsHUD MedicalPanelScript = MedicalPanel.GetComponent<MedicalDetailsHUD>();
        MedicalPanelScript.SelectedActor = this;
        MedicalPanelScript.Init();

        return new List<GameObject>() { MedicalPanel };
    }
}
