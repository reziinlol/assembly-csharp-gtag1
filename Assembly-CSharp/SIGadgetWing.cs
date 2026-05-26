using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000ED RID: 237
public class SIGadgetWing : SIGadget
{
	// Token: 0x0600059B RID: 1435 RVA: 0x000202E8 File Offset: 0x0001E4E8
	private void Awake()
	{
		if (this.m_buttonActivatable == null)
		{
			this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x000203AB File Offset: 0x0001E5AB
	private void OnGrabbed()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x000203AB File Offset: 0x0001E5AB
	private void OnSnapped()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnReleased()
	{
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnUnsnapped()
	{
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000203C4 File Offset: 0x0001E5C4
	protected override void OnUpdateAuthority(float dt)
	{
		Vector3 position = this.m_wingCenter.transform.position;
		SIGadgetWing_EState state = this._state;
		this._state = (this.m_buttonActivatable.CheckInput(0.25f) ? SIGadgetWing_EState.TriggerPressed : SIGadgetWing_EState.Idle);
		if (state != this._state)
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)this._state);
			this._lastWingPos = position;
		}
		if (this._state != SIGadgetWing_EState.TriggerPressed)
		{
			return;
		}
		Vector3 lhs = this._lastWingPos - position;
		Vector3 up = this.m_wingCenter.transform.up;
		float num = Mathf.Max(Vector3.Dot(lhs, up), 0f);
		double num2 = PhotonNetwork.Time - (double)GTPlayer.Instance.LastTouchedGroundAtNetworkTime;
		float num3 = Mathf.Lerp(this.m_flapStrength, this.m_flapDecayedStrength, (float)num2 / this.m_decayDuration);
		if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			return;
		}
		Vector3 force = up * (num * num3);
		GTPlayer.Instance.AddForce(force, ForceMode.Impulse);
		this._lastWingPos = position;
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnUpdateRemote(float dt)
	{
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x000204C0 File Offset: 0x0001E6C0
	public override void OnEntityStateChange(long prevState, long newState)
	{
		if (newState == prevState || newState < 0L || newState >= 2L)
		{
			return;
		}
		this.m_gtAnimator.SetState(newState);
	}

	// Token: 0x040006C7 RID: 1735
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x040006C8 RID: 1736
	[SerializeField]
	private float m_flapStrength;

	// Token: 0x040006C9 RID: 1737
	[SerializeField]
	private float m_flapDecayedStrength;

	// Token: 0x040006CA RID: 1738
	[SerializeField]
	private float m_decayDuration;

	// Token: 0x040006CB RID: 1739
	[SerializeField]
	private float m_liftStrength;

	// Token: 0x040006CC RID: 1740
	[SerializeField]
	private float m_liftCap;

	// Token: 0x040006CD RID: 1741
	[SerializeField]
	private Transform m_wingCenter;

	// Token: 0x040006CE RID: 1742
	[SerializeField]
	private GTAnimator m_gtAnimator;

	// Token: 0x040006CF RID: 1743
	private Vector3 _lastWingPos;

	// Token: 0x040006D0 RID: 1744
	private SIGadgetWing_EState _state;
}
