using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderEvents : MonoBehaviour
{
    public delegate void MoveOrder(Vector3 destination, FlockController flockController, bool StopAll = true);
    public static event MoveOrder OnMove;

    public delegate void TradeOrder(Character character);
    public static event TradeOrder OnTrade;

    public delegate void TalkOrder(Character character);
    public static event TalkOrder OnTalk;

    public delegate void UseOrder(StaticEntity staticEntity);
    public static event UseOrder OnUse;

    public delegate void AttackOrder(Actor actor);
    public static event AttackOrder OnAttack;

    public delegate void MeleeAttackOrder(Actor actor);
    public static event MeleeAttackOrder OnMelee;

    public delegate void EnterOrder(ABiotic aBiotic);
    public static event EnterOrder OnEnter;

    public delegate void PickupOrder(DynamicEntity dynamicEntity);
    public static event PickupOrder OnPickup;

    public delegate void FollowOrder(Actor actor);
    public static event FollowOrder OnFollow;

    public static void Move(Vector3 destination)
    {
        FlockController flockController = ScriptableObject.CreateInstance<FlockController>();
        OnMove?.Invoke(destination, flockController);
    }
    public static void Trade(Character character) => OnTrade?.Invoke(character);
    public static void Talk(Character character) => OnTalk?.Invoke(character);
    public static void Use(StaticEntity staticEntity) => OnUse?.Invoke(staticEntity);
    public static void Attack(Actor actor) => OnAttack?.Invoke(actor);
    public static void MeleeAttack(Actor actor) => OnMelee?.Invoke(actor);
    public static void Enter(ABiotic aBiotic) => OnEnter?.Invoke(aBiotic);
    public static void PickUp(DynamicEntity dynamicEntity) => OnPickup?.Invoke(dynamicEntity);
    public static void Follow(Actor actor) => OnFollow?.Invoke(actor);
}
