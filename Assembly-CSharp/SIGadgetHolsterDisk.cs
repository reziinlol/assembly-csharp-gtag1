using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class SIGadgetHolsterDisk : SIGadget, I_SIDisruptable
{
	// Token: 0x06000658 RID: 1624 RVA: 0x000235C8 File Offset: 0x000217C8
	private void Awake()
	{
		this.SetState(SIGadgetHolsterDisk.State.Unequipped);
		this.referenceGadget.gameObject.SetActive(false);
		this.referenceTransform = this.referenceGadget.transform;
		this.cooldownTimer = 0f;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x000235FE File Offset: 0x000217FE
	private void Start()
	{
		this.CreateGadget();
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x00023608 File Offset: 0x00021808
	private void CreateGadget()
	{
		this.gameEntity.manager.RequestCreateItem(this.referenceGadget.gameObject.name.GetStaticHash(), this.referenceGadget.transform.position, this.referenceGadget.transform.rotation, (long)this.gameEntity.GetNetId());
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00023668 File Offset: 0x00021868
	public void RegisterGadget(SIGadget gadget)
	{
		this.cachedGadget = gadget;
		this.grenadeGadget = this.cachedGadget.GetComponent<SIGadgetGrenade>();
		this.gadgetRB = this.cachedGadget.GetComponent<Rigidbody>();
		SIGadgetGrenade sigadgetGrenade = this.grenadeGadget;
		sigadgetGrenade.GrenadeFinished = (Action)Delegate.Combine(sigadgetGrenade.GrenadeFinished, new Action(this.GadgetRespawn));
		this.cachedGadget.gameObject.SetActive(false);
		this.GadgetRespawn();
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x000236DC File Offset: 0x000218DC
	private new void OnDisable()
	{
		if (this.grenadeGadget != null)
		{
			SIGadgetGrenade sigadgetGrenade = this.grenadeGadget;
			sigadgetGrenade.GrenadeFinished = (Action)Delegate.Remove(sigadgetGrenade.GrenadeFinished, new Action(this.GadgetRespawn));
		}
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x00023714 File Offset: 0x00021914
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		switch (this.state)
		{
		case SIGadgetHolsterDisk.State.Unequipped:
		case SIGadgetHolsterDisk.State.Ready:
			break;
		case SIGadgetHolsterDisk.State.OnCooldown:
			this.cooldownTimer += dt;
			this.grenadeGadget.grenadeRenderer.material.SetFloat("_RespawnAmount", this.cooldownTimer / this.cooldownTime);
			if (this.cooldownTimer > this.cooldownTime)
			{
				this.SetState(SIGadgetHolsterDisk.State.Ready);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0002378C File Offset: 0x0002198C
	private void SetState(SIGadgetHolsterDisk.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetHolsterDisk.State.Unequipped:
			this.cooldownTimer = 0f;
			return;
		case SIGadgetHolsterDisk.State.OnCooldown:
			break;
		case SIGadgetHolsterDisk.State.Ready:
			this.cachedGadget.gameEntity.pickupable = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x000237E1 File Offset: 0x000219E1
	public void DiskSnappedToHolster()
	{
		this.cachedGadget.gameObject.SetActive(true);
		this.gameEntity.pickupable = false;
		this.GadgetRespawn();
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00023806 File Offset: 0x00021A06
	public void DiskRemovedFromHolster()
	{
		this.SetState(SIGadgetHolsterDisk.State.Unequipped);
		this.gameEntity.pickupable = true;
		this.cachedGadget.gameObject.SetActive(false);
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0002382C File Offset: 0x00021A2C
	public void GadgetRespawn()
	{
		this.cachedGadget.transform.parent = base.transform;
		this.cachedGadget.transform.localPosition = this.referenceTransform.localPosition;
		this.cachedGadget.transform.localRotation = this.referenceTransform.localRotation;
		this.cachedGadget.gameEntity.pickupable = false;
		this.gadgetRB.isKinematic = true;
		this.SetState(SIGadgetHolsterDisk.State.OnCooldown);
		this.cooldownTimer = 0f;
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000238B4 File Offset: 0x00021AB4
	public void Disrupt(float disruptTime)
	{
		this.SetState(SIGadgetHolsterDisk.State.OnCooldown);
		this.cooldownTimer = -disruptTime;
	}

	// Token: 0x040007BF RID: 1983
	public SIGadget referenceGadget;

	// Token: 0x040007C0 RID: 1984
	public float cooldownTime;

	// Token: 0x040007C1 RID: 1985
	private SIGadgetHolsterDisk.State state;

	// Token: 0x040007C2 RID: 1986
	private float cooldownTimer;

	// Token: 0x040007C3 RID: 1987
	private SIGadgetGrenade grenadeGadget;

	// Token: 0x040007C4 RID: 1988
	private Rigidbody gadgetRB;

	// Token: 0x040007C5 RID: 1989
	private SIGadget cachedGadget;

	// Token: 0x040007C6 RID: 1990
	private Transform referenceTransform;

	// Token: 0x0200010E RID: 270
	private enum State
	{
		// Token: 0x040007C8 RID: 1992
		Unequipped,
		// Token: 0x040007C9 RID: 1993
		OnCooldown,
		// Token: 0x040007CA RID: 1994
		Ready
	}
}
