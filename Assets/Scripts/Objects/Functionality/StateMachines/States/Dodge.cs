using Animation.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI.States
{
    class Dodge : AIState
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
            EntitySelf.animator.SetTrigger("Dodge");
            await EntitySelf.animator.GetBehaviour<DodgeBehaviour>().Completed.WaitAsync();
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

        public Dodge(Entity ai, Del callback, Entity attacker) : base(ai, callback)
        {
            Attacker = attacker;
        }
    }
}
