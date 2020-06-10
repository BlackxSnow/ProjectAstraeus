using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.States;

namespace StateMachine
{
    public class AIStateMachine
    {
        public AIState State { get; protected set; }

        public void SetState(AIState state)
        {
            State = state;
        }
    } 
}
