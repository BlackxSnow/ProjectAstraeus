using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.States
{
    class Block : AIState
    {
        Entity Attacker;
        public override void StartState()
        {
            IsCommitted = true;
            base.StartState();
            StateBehaviour(BehaviourTokenSource.Token);
        }
        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            await EntitySelf.EntityComponents.IKController.BlockIK(Attacker);
            EndState();
        }
        public override void EndState()
        {
            base.EndState();
        }
        
        public override object Clone()
        {
            return new Block(EntitySelf, Callback, Attacker) { Token = Token };
        }

        public Block(Entity ai, Del callback, Entity attacker) : base(ai, callback)
        {
            Attacker = attacker;
        }
    }
}
