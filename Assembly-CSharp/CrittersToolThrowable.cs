using System;
using System.Diagnostics;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200007B RID: 123
public class CrittersToolThrowable : CrittersActor
{
	// Token: 0x06000301 RID: 769 RVA: 0x00011B86 File Offset: 0x0000FD86
	public override void Initialize()
	{
		base.Initialize();
		this.hasBeenGrabbedByPlayer = false;
		this.shouldDisable = false;
		this.hasTriggeredSinceLastGrab = false;
		this._sqrActivationSpeed = this.requiredActivationSpeed * this.requiredActivationSpeed;
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00011BB6 File Offset: 0x0000FDB6
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.hasBeenGrabbedByPlayer = true;
		this.hasTriggeredSinceLastGrab = false;
		this.OnPickedUp();
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00011BDC File Offset: 0x0000FDDC
	public void OnCollisionEnter(Collision collision)
	{
		if (CrittersManager.instance.containerLayer.Contains(collision.gameObject.layer))
		{
			return;
		}
		if (this.requiresPlayerGrabBeforeActivate && !this.hasBeenGrabbedByPlayer)
		{
			return;
		}
		if (this._sqrActivationSpeed > 0f && collision.relativeVelocity.sqrMagnitude < this._sqrActivationSpeed)
		{
			return;
		}
		if (this.onlyTriggerOncePerGrab && this.hasTriggeredSinceLastGrab)
		{
			return;
		}
		if (this.onlyTriggerOnDirectCritterHit)
		{
			CrittersPawn component = collision.gameObject.GetComponent<CrittersPawn>();
			if (component != null && component.isActiveAndEnabled)
			{
				this.hasTriggeredSinceLastGrab = true;
				this.OnImpactCritter(component);
			}
		}
		else
		{
			Vector3 point = collision.contacts[0].point;
			Vector3 normal = collision.contacts[0].normal;
			this.hasTriggeredSinceLastGrab = true;
			this.OnImpact(point, normal);
		}
		if (this.destroyOnImpact)
		{
			this.shouldDisable = true;
		}
	}

	// Token: 0x06000304 RID: 772 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
	}

	// Token: 0x06000305 RID: 773 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnImpactCritter(CrittersPawn impactedCritter)
	{
	}

	// Token: 0x06000306 RID: 774 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnPickedUp()
	{
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00011CC8 File Offset: 0x0000FEC8
	[Conditional("DRAW_DEBUG")]
	protected void ShowDebugVisualization(Vector3 position, float scale, float duration = 0f)
	{
		if (!this.debugImpactPrefab)
		{
			return;
		}
		DelayedDestroyObject delayedDestroyObject = Object.Instantiate<DelayedDestroyObject>(this.debugImpactPrefab, position, Quaternion.identity);
		delayedDestroyObject.transform.localScale *= scale;
		if (duration != 0f)
		{
			delayedDestroyObject.lifetime = duration;
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x00011D1C File Offset: 0x0000FF1C
	public override bool ProcessLocal()
	{
		bool result = base.ProcessLocal();
		if (this.shouldDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return result;
	}

	// Token: 0x06000309 RID: 777 RVA: 0x00011D48 File Offset: 0x0000FF48
	public override void TogglePhysics(bool enable)
	{
		if (enable)
		{
			this.rb.isKinematic = false;
			this.rb.interpolation = RigidbodyInterpolation.Interpolate;
			this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			return;
		}
		this.rb.isKinematic = true;
		this.rb.interpolation = RigidbodyInterpolation.None;
		this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

	// Token: 0x04000361 RID: 865
	[Header("Throwable")]
	public bool requiresPlayerGrabBeforeActivate = true;

	// Token: 0x04000362 RID: 866
	public float requiredActivationSpeed = 2f;

	// Token: 0x04000363 RID: 867
	public bool onlyTriggerOnDirectCritterHit;

	// Token: 0x04000364 RID: 868
	public bool destroyOnImpact = true;

	// Token: 0x04000365 RID: 869
	public bool onlyTriggerOncePerGrab = true;

	// Token: 0x04000366 RID: 870
	[Header("Debug")]
	[SerializeField]
	private DelayedDestroyObject debugImpactPrefab;

	// Token: 0x04000367 RID: 871
	private bool hasBeenGrabbedByPlayer;

	// Token: 0x04000368 RID: 872
	protected bool shouldDisable;

	// Token: 0x04000369 RID: 873
	private bool hasTriggeredSinceLastGrab;

	// Token: 0x0400036A RID: 874
	private float _sqrActivationSpeed;
}
