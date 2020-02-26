using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using System.Threading.Tasks;

//TODO Rewrite using async instead of coroutines
public class Actor : DynamicEntity, IOrderable, IDamageable, IInterruptible
{
    public CancellationTokenSource TokenSource { get; private set; } = new CancellationTokenSource();
    bool Subscribed;

    Inventory InventoryScript;
    Movement MovementScript;
    bool Movable;

    float PickupDistance = 1.5f;

    public Species Race;


    protected override void Start()
    {
        base.Start();
        MovementScript = GetComponent<Movement>();
        InventoryScript = GetComponent<Inventory>();

        if (MovementScript != null) Movable = true; else Movable = false;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Interrupt()
    {
        TokenSource.Cancel();
        TokenSource = new CancellationTokenSource();
    }

    public void Move(Vector3 Destination, FlockController flockController, bool interrupt = true)
    {
        if (interrupt) Interrupt();

        _ = MovementScript.SetDestination(Destination, flockController, TokenSource.Token);
    }

    public void Pickup(DynamicEntity Target)
    {
        Interrupt();
        PickupObject(Target);
    }

    //Behaviour to retrieve items
    private async void PickupObject(DynamicEntity Target)
    {
        Task<bool> MoveTask = EntityComponents.Movement.MoveWithin(Target.gameObject, PickupDistance, null, TokenSource.Token);
        await MoveTask;
        if (!MoveTask.Result) return;

        if (Target is Item) InventoryScript.AddItem(Target as Item);

    }

    private bool ProximityCheck(GameObject Target)
    {
        if (Vector3.Distance(transform.position, Target.transform.position) <= PickupDistance)
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
        UI.MedicalDetailsHUD MedicalPanelScript = MedicalPanel.GetComponent<UI.MedicalDetailsHUD>();
        MedicalPanelScript.SelectedActor = this;
        MedicalPanelScript.Init();

        return new List<GameObject>() { MedicalPanel };
    }
}
