using AI.States;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DevTools
{
    public class StateMachineInfo : MonoBehaviour
    {
        private TextMeshProUGUI QueueText;
        Utility.Async.Timer UpdateTimer;
        void Start()
        {
            UpdateTimer = new Utility.Async.Timer(0.05f, delegate { UpdateInfo(); }, true);
            UpdateTimer.Start();
            QueueText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void UpdateInfo()
        {
            ISelectable selected = UI.Control.Selection.SelectedObjs.Count > 0 ? UI.Control.Selection.SelectedObjs?[0] : null;
            if (selected is Actor actor)
            {
                
                QueueText.text = $"Current:\n\t{actor.StateMachine?.Current?.GetType()}";
                depth = 2;
                CheckSubState(actor.StateMachine.Current);
                QueueText.text += $"\nQueued:";
                foreach(AIState state in actor.StateMachine.StateQueue)
                {
                    QueueText.text += $"\n\t{state.GetType()}";
                }
            }
        }
        int depth = 2;
        private void CheckSubState(AIState state)
        {
            if(state?.SubState != null)
            {
                QueueText.text += "\n";
                for (int i = 0; i < depth; i++)
                {
                    QueueText.text += "\t";
                }
                QueueText.text += $"{state.SubState.GetType()}";
                depth++;
                CheckSubState(state?.SubState);
            }
        }
    } 
}
