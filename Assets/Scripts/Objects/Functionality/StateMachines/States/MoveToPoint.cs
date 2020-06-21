using AI.States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace AI.States
{
    /// <summary>
    /// A movement order to a specific Vector3 location.
    /// </summary>
    public class MoveToPoint : AIState, IFlockable
    {
        
        //Input
        protected Vector3 Target;
        protected FlockController Flock;
        //Derivative
        protected NavMeshPath TargetPath;
        public bool Arrived { get; protected set; } = false;

        public override void StartState()
        {
            base.StartState();
            StateBehaviour(BehaviourTokenSource.Token);
        }

        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            Succeeded = await SetDestination(true);
            if (Succeeded) EndState();
        }

        public override void EndState()
        {
            StateCompleted.Set();
            Callback?.Invoke();
        }

        public void StopMove()
        {
            Arrived = true;
            Succeeded = true;
            EntitySelf.animator.SetBool("Moving", false);
            EntitySelf.Agent.velocity = Vector3.zero;
            EntitySelf.Agent.ResetPath();

            if (Flock != null)
            {
                Flock.FlockMembers.Remove(this);
                Flock = null;
            }
        }

        public async Task<bool> SetDestination(bool loop)
        {
            float PathingDelay = SetDestination_Setup();

            bool Completed = false;
            while (!Completed)
            {
                Completed = AdjustPath();
                if (Completed)
                {
                    StopMove();
                    break;
                }

                try
                {
                    await Task.Delay(Mathf.RoundToInt(PathingDelay * 1000f), StateTokenSource.Token);
                }
                catch (OperationCanceledException) 
                { 
                    //StopMove(); 
                    return false; 
                }

                if (!loop) return Completed;
                else if (Token.IsCancellationRequested)
                {
                    //StopMove();
                    return false;
                }
            }

            return Completed;
        }

        private float SetDestination_Setup()
        {
            float PathingDelay = Mathf.Clamp(1f - 0.9f * BoidPathing.PathingResolution, 0.1f, 1f);

            if (!EntitySelf.Agent.SetDestination(Target)) throw new System.ArgumentException($"Unable to path to {Target}");
            Arrived = false;
            EntitySelf.animator.SetBool("Moving", true);
            return PathingDelay;
        }

        //Adjust waypoints according to flocking rules
        private bool AdjustPath()
        {
            if (Target != null && Flock != null && Flock.FlockMembers.Count > 1)
            {
                //Arrived?, Flock Arrived?, Adjusted Target
                Tuple<bool, bool, Vector3> adjustment = BoidPathing.AdjustPath_Flock(Target, Flock, this);
                Arrived = adjustment.Item1;

                if (adjustment.Item2) return true;
                else return false;
            }
            else if (Target != null)
            {
                if (Vector3.Distance(EntitySelf.transform.position, Target) < 0.25) return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a new movement order to the position <c>target</c>.
        /// </summary>
        /// <param name="ai">Ref to the parent Entity.</param>
        /// <param name="callback">Parameterless delegate for callback - may be null.</param>
        /// <param name="token">CancellationToken to interrupt behaviour.</param>
        /// <param name="target">Vector3 position to move to.</param>
        /// <param name="flock">The FlockController to calculate flocking from, if any.</param>
        public MoveToPoint(Entity ai, Del callback, Vector3 target, FlockController flock) : base(ai, callback)
        {
            Flock = flock;
            Target = target;
            Flock?.FlockMembers.Add(this);
        }

        /// <summary>
        /// Should only be called by base() from child classes.
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <param name="flock"></param>
        protected MoveToPoint(Entity ai, Del callback, FlockController flock) : base(ai, callback)
        {
            Flock = flock;
            Flock?.FlockMembers.Add(this);
        }

        public override object Clone()
        {
            return new MoveToPoint(EntitySelf, Callback, Target, Flock) { Token = Token };
        }
    }

}