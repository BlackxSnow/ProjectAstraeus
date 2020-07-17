using Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UI.Control;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AI.States
{
    public class UseItem : AIState
    {
        IUsable Usable;
        Entity Target;

        public override void StartState()
        {
            base.StartState();
            StateBehaviour(BehaviourTokenSource.Token);
        }
        public override async void StateBehaviour(CancellationToken behaviourToken)
        {
            bool completed = false;
            while(!completed)
            {
                float range = (Usable as Item).Stats.Range;
                if (Vector3.Distance(EntitySelf.transform.position, Target.transform.position) > range)
                {
                    SubState = MoveWithin.MoveState(EntitySelf, (Target as MonoBehaviour).gameObject, range, behaviourToken);
                    await SubState.StateCompleted.WaitAsync();
                    SubState = null;
                }
                if(!Usable.GetNextIteration(Target, out object iterateOn, out float time)) 
                    break;
                ProgressBar progressBar = CreateUI.Info.ProgressBar(Target.gameObject, 0, time, Color.grey, Color.green);
                Progress<float> progress = new Progress<float>();
                progress.ProgressChanged += delegate(object _, float arg) { progressBar.UpdateBar(arg, true); };
                await Utility.Async.WaitForSeconds(time, behaviourToken, progress);
                progress.ProgressChanged -= delegate (object _, float arg) { progressBar.UpdateBar(arg, true); };
                progressBar.Destroy();
                Usable.Act(EntitySelf, Target, iterateOn);
            }
            EndState();
        }
        public override void EndState()
        {
            base.EndState();
        }
        public UseItem (Entity ai, Del callback, Entity target, IUsable usable) : base(ai, callback)
        {
            Target = target;
            Usable = usable;
        }
        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
