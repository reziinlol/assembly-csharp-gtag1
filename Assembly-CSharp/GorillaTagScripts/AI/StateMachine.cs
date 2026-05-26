using System;
using System.Collections.Generic;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000FEC RID: 4076
	public class StateMachine
	{
		// Token: 0x060065EC RID: 26092 RVA: 0x0020DDE0 File Offset: 0x0020BFE0
		public void Tick()
		{
			StateMachine.Transition transition = this.GetTransition();
			if (transition != null)
			{
				this.SetState(transition.To);
			}
			IState currentState = this._currentState;
			if (currentState == null)
			{
				return;
			}
			currentState.Tick();
		}

		// Token: 0x060065ED RID: 26093 RVA: 0x0020DE14 File Offset: 0x0020C014
		public void SetState(IState state)
		{
			if (state == this._currentState)
			{
				return;
			}
			IState currentState = this._currentState;
			if (currentState != null)
			{
				currentState.OnExit();
			}
			this._currentState = state;
			this._transitions.TryGetValue(this._currentState.GetType(), out this._currentTransitions);
			if (this._currentTransitions == null)
			{
				this._currentTransitions = StateMachine.EmptyTransitions;
			}
			this._currentState.OnEnter();
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x0020DE7E File Offset: 0x0020C07E
		public IState GetState()
		{
			return this._currentState;
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x0020DE88 File Offset: 0x0020C088
		public void AddTransition(IState from, IState to, Func<bool> predicate)
		{
			List<StateMachine.Transition> list;
			if (!this._transitions.TryGetValue(from.GetType(), out list))
			{
				list = new List<StateMachine.Transition>();
				this._transitions[from.GetType()] = list;
			}
			list.Add(new StateMachine.Transition(to, predicate));
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x0020DECF File Offset: 0x0020C0CF
		public void AddAnyTransition(IState state, Func<bool> predicate)
		{
			this._anyTransitions.Add(new StateMachine.Transition(state, predicate));
		}

		// Token: 0x060065F1 RID: 26097 RVA: 0x0020DEE4 File Offset: 0x0020C0E4
		private StateMachine.Transition GetTransition()
		{
			foreach (StateMachine.Transition transition in this._anyTransitions)
			{
				if (transition.Condition())
				{
					return transition;
				}
			}
			foreach (StateMachine.Transition transition2 in this._currentTransitions)
			{
				if (transition2.Condition())
				{
					return transition2;
				}
			}
			return null;
		}

		// Token: 0x04007557 RID: 30039
		private IState _currentState;

		// Token: 0x04007558 RID: 30040
		private Dictionary<Type, List<StateMachine.Transition>> _transitions = new Dictionary<Type, List<StateMachine.Transition>>();

		// Token: 0x04007559 RID: 30041
		private List<StateMachine.Transition> _currentTransitions = new List<StateMachine.Transition>();

		// Token: 0x0400755A RID: 30042
		private List<StateMachine.Transition> _anyTransitions = new List<StateMachine.Transition>();

		// Token: 0x0400755B RID: 30043
		private static List<StateMachine.Transition> EmptyTransitions = new List<StateMachine.Transition>(0);

		// Token: 0x02000FED RID: 4077
		private class Transition
		{
			// Token: 0x17000991 RID: 2449
			// (get) Token: 0x060065F4 RID: 26100 RVA: 0x0020DFC6 File Offset: 0x0020C1C6
			public Func<bool> Condition { get; }

			// Token: 0x17000992 RID: 2450
			// (get) Token: 0x060065F5 RID: 26101 RVA: 0x0020DFCE File Offset: 0x0020C1CE
			public IState To { get; }

			// Token: 0x060065F6 RID: 26102 RVA: 0x0020DFD6 File Offset: 0x0020C1D6
			public Transition(IState to, Func<bool> condition)
			{
				this.To = to;
				this.Condition = condition;
			}
		}
	}
}
