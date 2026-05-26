using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class SIGadgetGrenadeBlackHole : SIGadgetGrenade
{
	// Token: 0x0600061E RID: 1566 RVA: 0x00022C3F File Offset: 0x00020E3F
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeBlackHole.State.Idle;
	}

	// Token: 0x0600061F RID: 1567 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void HandleActivated()
	{
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x00022C4E File Offset: 0x00020E4E
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeBlackHole.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Triggered);
		}
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x00022C60 File Offset: 0x00020E60
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeBlackHole.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Thrown);
		}
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x00022C74 File Offset: 0x00020E74
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeBlackHole.State state = (SIGadgetGrenadeBlackHole.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x00022C9E File Offset: 0x00020E9E
	private void SetStateAuthority(SIGadgetGrenadeBlackHole.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x00022CC0 File Offset: 0x00020EC0
	private void SetState(SIGadgetGrenadeBlackHole.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeBlackHole.State.Idle:
		case SIGadgetGrenadeBlackHole.State.Thrown:
			break;
		case SIGadgetGrenadeBlackHole.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x00022D00 File Offset: 0x00020F00
	private void TriggerExplosion()
	{
		Vector3 vector = base.transform.position - GTPlayer.Instance.transform.position;
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
			this.SetStateAuthority(SIGadgetGrenadeBlackHole.State.Idle);
		}
	}

	// Token: 0x0400078A RID: 1930
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x0400078B RID: 1931
	[SerializeField]
	private float explosionRadius;

	// Token: 0x0400078C RID: 1932
	private SIGadgetGrenadeBlackHole.State state;

	// Token: 0x02000102 RID: 258
	private enum State
	{
		// Token: 0x0400078E RID: 1934
		Idle,
		// Token: 0x0400078F RID: 1935
		Thrown,
		// Token: 0x04000790 RID: 1936
		Triggered
	}
}
