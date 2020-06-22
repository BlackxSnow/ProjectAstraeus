using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.States
{
    /// <summary>
    /// A movement order to within a distance of a Vector3 or GameObject target
    /// </summary>
    public class MoveWithin : MoveToPoint
    {
        protected GameObject TargetObject;
        protected float Distance;

        public override void StartState()
        {
            base.StartState();
        }

        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            while (true)
            {
                if (TargetObject) Target = TargetObject.transform.position;

                Vector3 positionDiff = EntitySelf.transform.position - Target;
                positionDiff.y = Mathf.Pow(positionDiff.y, 2);
                float ModifiedDistance = positionDiff.magnitude;
                if (ModifiedDistance < Distance)
                {
                    StopMove();
                    EndState();
                    return;
                }
                else if (Token.IsCancellationRequested)
                {
                    StopMove();
                    return;
                }

                await SetDestination(false);
            }
        }

        public override void EndState()
        {
            StateCompleted.Set();
            Callback?.Invoke();
        }
        /// <summary>
        /// Creates a new movement order to within <c>distance</c> of <c>target</c>.
        /// </summary>
        /// <param name="ai">Ref to the parent Entity.</param>
        /// <param name="callback">Parameterless delegate for callback - may be null.</param>
        /// <param name="token">CancellationToken to interrupt behaviour.</param>
        /// <param name="target">>GameObject to move towards.</param>
        /// <param name="distance">Distance from target to path to.</param>
        /// <param name="flock">The FlockController to calculate flocking from, if any.</param>
        public MoveWithin(Entity ai, Del callback, GameObject target, float distance, FlockController flock) : base(ai, callback, flock)
        {
            TargetObject = target;
            Distance = distance;
        }
        /// <summary>
        /// Creates a new movement order to within <c>distance</c> of <c>target</c>.
        /// </summary>
        /// <param name="ai">Ref to the parent Entity.</param>
        /// <param name="callback">Parameterless delegate for callback - may be null.</param>
        /// <param name="token">CancellationToken to interrupt behaviour.</param>
        /// <param name="target">>Vector3 to move towards.</param>
        /// <param name="distance">Distance from target to path to.</param>
        /// <param name="flock">The FlockController to calculate flocking from, if any.</param>
        public MoveWithin(Entity ai, Del callback, Vector3 target, float distance, FlockController flock) : base(ai, callback, target, flock)
        {
            Distance = distance;
        }

        public static MoveWithin MoveState(Entity self, GameObject target, float range, CancellationToken token)
        {
            MoveWithin moveWithin = new MoveWithin(self, null, target, range, null)
            {
                Token = token
            };
            moveWithin.StartState();
            return moveWithin;
        }

        public override object Clone()
        {
            return new MoveWithin(EntitySelf, Callback, Target, Distance, Flock) { Token = Token };
        }
    } 

}