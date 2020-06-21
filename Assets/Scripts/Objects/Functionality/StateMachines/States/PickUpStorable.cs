using AI.States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace AI.States
{
	public class PickUpStorable : AIState
	{
        public static float PickupDistance = 1.5f;
        public Item Target;

		public PickUpStorable(Entity ai,  Del callback, Item target) : base(ai, callback)
		{
            Target = target;
		}

        public override void StartState()
        {
            base.StartState();
            StateBehaviour(BehaviourTokenSource.Token);
        }

        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            SubState = new MoveWithin(EntitySelf, null, (Target as MonoBehaviour).gameObject, PickupDistance, null)
            {
                Token = behaviourToken
            };
            SubState.StartState();
            try
            {
                await SubState.StateCompleted.WaitAsync(behaviourToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            if(SubState.Succeeded)
            {
                EntitySelf.EntityComponents.Inventory.AddItem(Target);
                Succeeded = true;
            }

            EndState();
        }

        public override void EndState()
        {
            base.EndState();
        }

        public override object Clone()
        {
            return new PickUpStorable(EntitySelf, Callback, Target) { Token = Token };
        }
    }
}
