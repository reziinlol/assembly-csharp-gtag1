using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class SIGadgetGrenadeGravity : SIGadgetGrenade
{
	// Token: 0x06000631 RID: 1585 RVA: 0x00022ED7 File Offset: 0x000210D7
	protected override void OnEnable()
	{
		base.OnEnable();
		this.gravityField.SetActive(false);
		this.state = SIGadgetGrenadeGravity.State.Idle;
		this.stateRemainingDuration = -1f;
		this.isLocalPlayerInEffect = false;
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x00022F04 File Offset: 0x00021104
	protected override void HandleActivated()
	{
		if (this.state == SIGadgetGrenadeGravity.State.Idle)
		{
			this.activatedLocally = true;
			this.SetStateAuthority(SIGadgetGrenadeGravity.State.Activated);
			return;
		}
		this.SetStateAuthority(SIGadgetGrenadeGravity.State.Idle);
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void HandleThrown()
	{
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void HandleHitSurface()
	{
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x00022F24 File Offset: 0x00021124
	protected override void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case SIGadgetGrenadeGravity.State.Idle:
			break;
		case SIGadgetGrenadeGravity.State.Activated:
			this.stateRemainingDuration -= dt;
			if (this.stateRemainingDuration <= 0f)
			{
				this.SetStateAuthority(SIGadgetGrenadeGravity.State.Triggered);
				return;
			}
			break;
		case SIGadgetGrenadeGravity.State.Triggered:
			this.stateRemainingDuration -= dt;
			if (this.stateRemainingDuration <= 0f)
			{
				this.SetStateAuthority(SIGadgetGrenadeGravity.State.Idle);
				return;
			}
			if (this.freezePositionOnTrigger)
			{
				this.CheckReenabledFreezePosition();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x00022FA0 File Offset: 0x000211A0
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetGrenadeGravity.State state = (SIGadgetGrenadeGravity.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
		if (this.freezePositionOnTrigger)
		{
			this.CheckReenabledFreezePosition();
		}
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x00022FD8 File Offset: 0x000211D8
	private void SetStateAuthority(SIGadgetGrenadeGravity.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x00022FFC File Offset: 0x000211FC
	private void SetState(SIGadgetGrenadeGravity.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case SIGadgetGrenadeGravity.State.Idle:
			this.activatedLocally = false;
			this.stateRemainingDuration = -1f;
			this.mesh.material = this.idleMat;
			this.DeactivateGravityEffect();
			return;
		case SIGadgetGrenadeGravity.State.Activated:
			this.stateRemainingDuration = this.counterDuration;
			this.mesh.material = this.activatedMat;
			this.DeactivateGravityEffect();
			return;
		case SIGadgetGrenadeGravity.State.Triggered:
			this.stateRemainingDuration = this.triggerDuration;
			this.mesh.material = this.triggeredMat;
			this.ActivateGravityEffect();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x000230AF File Offset: 0x000212AF
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 3L;
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x000230BE File Offset: 0x000212BE
	private void ActivateGravityEffect()
	{
		this.gravityField.SetActive(true);
		if (this.freezePositionOnTrigger)
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
		}
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x000230F0 File Offset: 0x000212F0
	private void DeactivateGravityEffect()
	{
		this.gravityField.SetActive(false);
		if (this.isLocalPlayerInEffect)
		{
			this.isLocalPlayerInEffect = false;
			GTPlayer instance = GTPlayer.Instance;
			if (instance != null)
			{
				instance.UnsetGravityOverride(this);
			}
		}
		if (this.freezePositionOnTrigger && !this.thrownGadget.IsHeld())
		{
			this.rb.isKinematic = false;
		}
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x00023150 File Offset: 0x00021350
	private void CheckReenabledFreezePosition()
	{
		if (this.state == SIGadgetGrenadeGravity.State.Triggered && !this.thrownGadget.IsHeld() && !this.rb.isKinematic)
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
		}
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0002319C File Offset: 0x0002139C
	private void OnTriggerEnter(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			this.isLocalPlayerInEffect = true;
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x0600063E RID: 1598 RVA: 0x000231E0 File Offset: 0x000213E0
	private void OnTriggerExit(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			this.isLocalPlayerInEffect = false;
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x0600063F RID: 1599 RVA: 0x00023218 File Offset: 0x00021418
	public void GravityOverrideFunction(GTPlayer player)
	{
		Vector3 a = Physics.gravity * this.standardGravityMultiplier;
		Vector3 b = Vector3.zero;
		if (!this.thrownGadget.IsHeldLocal())
		{
			b = (base.transform.position - player.headCollider.transform.position).normalized * this.attractorStrength;
		}
		player.AddForce((a + b) * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x04000798 RID: 1944
	[Header("Activation")]
	[SerializeField]
	private float counterDuration = 1f;

	// Token: 0x04000799 RID: 1945
	[Header("Gravity Effect")]
	[SerializeField]
	private GameObject gravityField;

	// Token: 0x0400079A RID: 1946
	[SerializeField]
	private bool freezePositionOnTrigger;

	// Token: 0x0400079B RID: 1947
	[SerializeField]
	private float triggerDuration = 5f;

	// Token: 0x0400079C RID: 1948
	[SerializeField]
	private float standardGravityMultiplier = 1f;

	// Token: 0x0400079D RID: 1949
	[SerializeField]
	private float attractorStrength;

	// Token: 0x0400079E RID: 1950
	[Header("FX")]
	[SerializeField]
	private MeshRenderer mesh;

	// Token: 0x0400079F RID: 1951
	[SerializeField]
	private Material idleMat;

	// Token: 0x040007A0 RID: 1952
	[SerializeField]
	private Material activatedMat;

	// Token: 0x040007A1 RID: 1953
	[SerializeField]
	private Material triggeredMat;

	// Token: 0x040007A2 RID: 1954
	private SIGadgetGrenadeGravity.State state;

	// Token: 0x040007A3 RID: 1955
	private float stateRemainingDuration;

	// Token: 0x040007A4 RID: 1956
	private bool isLocalPlayerInEffect;

	// Token: 0x02000106 RID: 262
	private enum State
	{
		// Token: 0x040007A6 RID: 1958
		Idle,
		// Token: 0x040007A7 RID: 1959
		Activated,
		// Token: 0x040007A8 RID: 1960
		Triggered,
		// Token: 0x040007A9 RID: 1961
		Count
	}
}
