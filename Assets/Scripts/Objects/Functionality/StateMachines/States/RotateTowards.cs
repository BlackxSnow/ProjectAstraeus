using AI.States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    /// <summary>
    /// A movement order to a specific Vector3 location.
    /// </summary>
    public class RotateTowards : AIState
    {
        
        //Input
        protected Transform Target;

        public override void StartState()
        {
            base.StartState();
            StateBehaviour(BehaviourTokenSource.Token);
        }

        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            try
            {
                Vector3 startDirection = EntitySelf.transform.rotation.eulerAngles;
                Quaternion startRotation = EntitySelf.transform.rotation;
                startDirection.y = 0;
                const float rotSpeed = 5.0f;
                float time = 0;
                bool completed = false;
                while (!completed)
                {
                    Vector3 targetDirection = (Target.position - EntitySelf.transform.position).normalized;
                    //targetDirection.y = 0;
                    Quaternion targetRotation = new Quaternion();
                    //targetRotation.SetFromToRotation(startDirection, targetDirection);
                    targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                    EntitySelf.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);

                    if (time >= 1f)
                        completed = true;

                    time += Time.deltaTime * rotSpeed;
                    await Await.NextUpdate().ConfigureAwait(behaviourToken);
                }
                EndState();
            }
            catch (OperationCanceledException)
            {
                EndState();
            }
        }

        public override void EndState()
        {
            StateCompleted.Set();
            Callback?.Invoke();
        }

        public RotateTowards(Entity ai, Del callback, Transform target) : base(ai, callback)
        {
            Target = target;
        }

        public override object Clone()
        {
            return new RotateTowards(EntitySelf, Callback, Target) { Token = Token };
        }
    }

}