using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class SIGadgetGrenadeKnockBack : SIGadgetGrenade
{
	// Token: 0x06000641 RID: 1601 RVA: 0x000232BF File Offset: 0x000214BF
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeKnockBack.State.Idle;
	}

	// Token: 0x06000642 RID: 1602 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void HandleActivated()
	{
	}

	// Token: 0x06000643 RID: 1603 RVA: 0x000232CE File Offset: 0x000214CE
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeKnockBack.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Triggered);
		}
	}

	// Token: 0x06000644 RID: 1604 RVA: 0x000232E0 File Offset: 0x000214E0
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeKnockBack.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Thrown);
		}
	}

	// Token: 0x06000645 RID: 1605 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x000232F4 File Offset: 0x000214F4
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeKnockBack.State state = (SIGadgetGrenadeKnockBack.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0002331E File Offset: 0x0002151E
	private void SetStateAuthority(SIGadgetGrenadeKnockBack.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00023340 File Offset: 0x00021540
	private void SetState(SIGadgetGrenadeKnockBack.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeKnockBack.State.Idle:
		case SIGadgetGrenadeKnockBack.State.Thrown:
			break;
		case SIGadgetGrenadeKnockBack.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00023380 File Offset: 0x00021580
	private void TriggerExplosion()
	{
		Vector3 vector = GTPlayer.Instance.transform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		if (this.explosionRadius * this.explosionRadius > sqrMagnitude)
		{
			float num = Mathf.Sqrt(sqrMagnitude);
			float num2 = 1f - num / this.explosionRadius;
			float speed = this.knockbackStrength * num2;
			GTPlayer.Instance.ApplyKnockback(vector.normalized, speed, false);
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeKnockBack.State.Idle);
		}
		Action grenadeFinished = this.GrenadeFinished;
		if (grenadeFinished == null)
		{
			return;
		}
		grenadeFinished();
	}

	// Token: 0x040007AA RID: 1962
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x040007AB RID: 1963
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040007AC RID: 1964
	private SIGadgetGrenadeKnockBack.State state;

	// Token: 0x02000108 RID: 264
	private enum State
	{
		// Token: 0x040007AE RID: 1966
		Idle,
		// Token: 0x040007AF RID: 1967
		Thrown,
		// Token: 0x040007B0 RID: 1968
		Triggered
	}
}
