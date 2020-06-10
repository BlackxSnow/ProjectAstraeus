using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class IKBehaviour : MonoBehaviour
{
    Animator animator;
    private Actor ThisActor;
    private float BlockWeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ThisActor = GetComponent<Actor>();
    }

    private Actor Attacker;
    public async Task BlockIK(Actor attacker)
    {
        Attacker = attacker;
        BlockWeight = 1f;
        await Task.Delay(2000);
        BlockWeight = 0f;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (Attacker != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, BlockWeight);
            animator.SetIKPosition(AvatarIKGoal.RightHand, Attacker.animator.GetBoneTransform(HumanBodyBones.RightHand).position/*Attacker.EntityComponents.Equipment.Equipped[(int)Equipment.Slots.Weapon].gameObject.transform.position*/);
        }
    }
}
