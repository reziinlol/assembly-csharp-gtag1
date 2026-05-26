using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class SIGadgetGrenadeStun : SIGadgetGrenade
{
	// Token: 0x0600064B RID: 1611 RVA: 0x00023427 File Offset: 0x00021627
	protected override void OnEnable()
	{
		base.OnEnable();
		this.state = SIGadgetGrenadeStun.State.Idle;
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void HandleActivated()
	{
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00023436 File Offset: 0x00021636
	protected override void HandleHitSurface()
	{
		if (this.state == SIGadgetGrenadeStun.State.Thrown)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Triggered);
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00023448 File Offset: 0x00021648
	protected override void HandleThrown()
	{
		if (this.state == SIGadgetGrenadeStun.State.Idle)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Thrown);
		}
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0002345C File Offset: 0x0002165C
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeStun.State state = (SIGadgetGrenadeStun.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x00023486 File Offset: 0x00021686
	private void SetStateAuthority(SIGadgetGrenadeStun.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x000234A8 File Offset: 0x000216A8
	private void SetState(SIGadgetGrenadeStun.State newState)
	{
		if (newState == this.state)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeStun.State.Idle:
		case SIGadgetGrenadeStun.State.Thrown:
			break;
		case SIGadgetGrenadeStun.State.Triggered:
			this.TriggerExplosion();
			break;
		default:
			return;
		}
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x000234E8 File Offset: 0x000216E8
	private void TriggerExplosion()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.explosionRadius, UnityLayer.GorillaTagCollider.ToLayerMask());
		for (int i = 0; i < array.Length; i++)
		{
			VRRig componentInParent = array[i].GetComponentInParent<VRRig>();
			if (componentInParent != null)
			{
				Vector3 a = componentInParent.transform.position - base.transform.position;
				float magnitude = a.magnitude;
				float num = 1f - magnitude / this.explosionRadius;
				float d = this.knockbackStrength * num;
				RoomSystem.LaunchPlayer(componentInParent.OwningNetPlayer, d * a / magnitude);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, componentInParent.OwningNetPlayer);
			}
		}
		if (this.gameEntity.lastHeldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.SetStateAuthority(SIGadgetGrenadeStun.State.Idle);
		}
	}

	// Token: 0x040007B1 RID: 1969
	[SerializeField]
	private float knockbackStrength;

	// Token: 0x040007B2 RID: 1970
	[SerializeField]
	private float explosionRadius;

	// Token: 0x040007B3 RID: 1971
	private SIGadgetGrenadeStun.State state;

	// Token: 0x0200010A RID: 266
	private enum State
	{
		// Token: 0x040007B5 RID: 1973
		Idle,
		// Token: 0x040007B6 RID: 1974
		Thrown,
		// Token: 0x040007B7 RID: 1975
		Triggered
	}
}
