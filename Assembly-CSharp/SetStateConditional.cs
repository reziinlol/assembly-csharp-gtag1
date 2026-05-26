using System;
using UnityEngine;

// Token: 0x020005DE RID: 1502
public class SetStateConditional : StateMachineBehaviour
{
	// Token: 0x0600255F RID: 9567 RVA: 0x000C655F File Offset: 0x000C475F
	private void OnValidate()
	{
		this._setToID = this.setToState;
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x000C6572 File Offset: 0x000C4772
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!this._didSetup)
		{
			this.parentAnimator = animator;
			this.Setup(animator, stateInfo, layerIndex);
			this._didSetup = true;
		}
		this._sinceEnter = TimeSince.Now();
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x000C65A0 File Offset: 0x000C47A0
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.delay > 0f && !this._sinceEnter.HasElapsed(this.delay, true))
		{
			return;
		}
		if (!this.CanSetState(animator, stateInfo, layerIndex))
		{
			return;
		}
		animator.Play(this._setToID);
	}

	// Token: 0x06002562 RID: 9570 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x00023994 File Offset: 0x00021B94
	protected virtual bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		return true;
	}

	// Token: 0x040030E1 RID: 12513
	public Animator parentAnimator;

	// Token: 0x040030E2 RID: 12514
	public string setToState;

	// Token: 0x040030E3 RID: 12515
	[SerializeField]
	private AnimStateHash _setToID;

	// Token: 0x040030E4 RID: 12516
	public float delay = 1f;

	// Token: 0x040030E5 RID: 12517
	protected TimeSince _sinceEnter;

	// Token: 0x040030E6 RID: 12518
	[NonSerialized]
	private bool _didSetup;
}
