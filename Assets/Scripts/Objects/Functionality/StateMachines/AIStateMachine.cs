using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.States;
using System.Threading;

namespace AI
{
    /// <summary>
    /// Handler for storing and executing AIState behaviours.
    /// </summary>
    public class AIStateMachine
    {
        public AIState Current { get; protected set; } = null;
        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        public LinkedList<AIState> StateQueue = new LinkedList<AIState>();
        Entity AI;

        /// <summary>
        /// Clears the queue and adds <c>state</c> to the new queue.
        /// </summary>
        /// <param name="state">The new target AIState.</param>
        public void SetState(AIState state)
        {
            StateQueue.Clear();
            StateQueue.AddLast(state);
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            Current = null;
            RunNextState();
        }

        /// <summary>
        /// Adds <c>state</c> to the end of the current queue.
        /// </summary>
        /// <param name="state">The AIState to add.</param>
        public void AddState(AIState state)
        {
            StateQueue.AddLast(state);
            RunNextState();
        }

        /// <summary>
        /// Adds the current <c>AIState</c> to the front of the queue, and immediately runs <c>state</c>.
        /// </summary>
        /// <param name="state"></param>
        public void AddStateImmediate(AIState state)
        {
            if(Current != null && !Current.StateCompleted.IsSet)
            {
                StateQueue.AddFirst(Current.Clone() as AIState);
                Current.StateTokenSource.Cancel();
            }
            Current = state;
            Current.Token = TokenSource.Token;
            Current.StartState();
        }

        public void ClearStates()
        {
            TokenSource.Cancel();
            StateQueue.Clear();
            Current = null;
            TokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Evaluates the current and next state in the queue and runs if applicable.
        /// </summary>
        void RunNextState()
        {
            if (Current != null && Current.StateCompleted.IsSet)
            {
                Current = null;
            }
            if(Current == null && StateQueue.Count > 0)
            {
                Current = StateQueue.First.Value;
                StateQueue.RemoveFirst();
                Current.StartState();
                Current.Callback = RunNextState;
                Current.Token = TokenSource.Token;
            }
        }

        public AIStateMachine(Entity ai)
        {
            AI = ai;
        }
    } 
}
