using Nito.AsyncEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AI
{
	namespace States
	{
		public abstract class AIState : ICloneable
		{
			public delegate void Del();
			public Del Callback;
			public AIState SubState = null;
			public Entity EntitySelf { get; protected set; }
			public CancellationToken Token;
			public CancellationTokenSource StateTokenSource;
			protected CancellationTokenSource BehaviourTokenSource;
			public AsyncAutoResetEvent ContinueState = new AsyncAutoResetEvent();
			public AsyncManualResetEvent StateCompleted = new AsyncManualResetEvent();
			public bool Succeeded = false;
			public bool IsCommitted = false;

			public virtual void StartState()
			{
				StateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(Token);
				BehaviourTokenSource = CancellationTokenSource.CreateLinkedTokenSource(StateTokenSource.Token);
			}

			public abstract void StateBehaviour(CancellationToken behaviourToken);

			public virtual void EndState()
			{
				StateCompleted.Set();
				Callback?.Invoke();
			}

			public AIState(Entity ai, Del callback)
			{
				EntitySelf = ai;
				Callback = callback;
			}

			public abstract object Clone();

		}  
	}
}
