using System;
using UnityEngine;

// Token: 0x0200076E RID: 1902
public class SimpleStateMachine<State> where State : Enum
{
	// Token: 0x06003034 RID: 12340 RVA: 0x00106397 File Offset: 0x00104597
	public void Setup(State initialState, Action<State> onStateStart, Action<State> onStateEnd, Action<State> onStateUpdate)
	{
		this.onStateStart = onStateStart;
		this.onStateEnd = onStateEnd;
		this.onStateUpdate = onStateUpdate;
		this.stateStartTime = Time.timeAsDouble;
		this.currState = initialState;
		if (onStateStart != null)
		{
			onStateStart(this.currState);
		}
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x001063D0 File Offset: 0x001045D0
	public void Update()
	{
		Action<State> action = this.onStateUpdate;
		if (action == null)
		{
			return;
		}
		action(this.currState);
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x001063E8 File Offset: 0x001045E8
	public void SetState(State state, bool force = false)
	{
		if (!force && state.Equals(this.currState))
		{
			return;
		}
		Action<State> action = this.onStateEnd;
		if (action != null)
		{
			action(this.currState);
		}
		this.currState = state;
		this.stateStartTime = Time.timeAsDouble;
		Action<State> action2 = this.onStateStart;
		if (action2 == null)
		{
			return;
		}
		action2(this.currState);
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x00106452 File Offset: 0x00104652
	public State GetState()
	{
		return this.currState;
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x0010645A File Offset: 0x0010465A
	public double GetStateStartTime()
	{
		return this.stateStartTime;
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x00106462 File Offset: 0x00104662
	public bool IsStateFinished(double currTime, float stateDuration)
	{
		return currTime >= this.stateStartTime + (double)stateDuration;
	}

	// Token: 0x04003DC7 RID: 15815
	private State currState;

	// Token: 0x04003DC8 RID: 15816
	private double stateStartTime;

	// Token: 0x04003DC9 RID: 15817
	private Action<State> onStateStart;

	// Token: 0x04003DCA RID: 15818
	private Action<State> onStateEnd;

	// Token: 0x04003DCB RID: 15819
	private Action<State> onStateUpdate;
}
