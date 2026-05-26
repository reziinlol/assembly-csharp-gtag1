using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class SIGadgetGrenadeDisrupt : SIGadgetGrenade
{
	// Token: 0x06000628 RID: 1576 RVA: 0x00022D9F File Offset: 0x00020F9F
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeDisrupt.State.Idle;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00022DB0 File Offset: 0x00020FB0
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeDisrupt.State state = (SIGadgetGrenadeDisrupt.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00022DDA File Offset: 0x00020FDA
	private void SetStateAuthority(SIGadgetGrenadeDisrupt.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00022DFC File Offset: 0x00020FFC
	private void SetState(SIGadgetGrenadeDisrupt.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeDisrupt.State.Idle:
		case SIGadgetGrenadeDisrupt.State.Thrown:
			break;
		case SIGadgetGrenadeDisrupt.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00022E3C File Offset: 0x0002103C
	private void TriggerExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius);
		for (int i = 0; i < array.Length; i++)
		{
			I_SIDisruptable componentInParent = array[i].GetComponentInParent<I_SIDisruptable>();
			if (componentInParent != null)
			{
				componentInParent.Disrupt(this.disruptTime);
			}
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Idle);
		}
		Action grenadeFinished = this.GrenadeFinished;
		if (grenadeFinished == null)
		{
			return;
		}
		grenadeFinished();
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void HandleActivated()
	{
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x00022EB4 File Offset: 0x000210B4
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeDisrupt.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Triggered);
		}
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00022EC6 File Offset: 0x000210C6
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeDisrupt.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeDisrupt.State.Thrown);
		}
	}

	// Token: 0x04000791 RID: 1937
	public float disruptTime;

	// Token: 0x04000792 RID: 1938
	[SerializeField]
	private float explosionRadius;

	// Token: 0x04000793 RID: 1939
	private SIGadgetGrenadeDisrupt.State state;

	// Token: 0x02000104 RID: 260
	private enum State
	{
		// Token: 0x04000795 RID: 1941
		Idle,
		// Token: 0x04000796 RID: 1942
		Thrown,
		// Token: 0x04000797 RID: 1943
		Triggered
	}
}
