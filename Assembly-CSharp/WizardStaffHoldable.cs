using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class WizardStaffHoldable : TransferrableObject
{
	// Token: 0x06000DD2 RID: 3538 RVA: 0x0004BA83 File Offset: 0x00049C83
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.tipTargetLocalPosition = this.tipTransform.localPosition;
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000DD3 RID: 3539 RVA: 0x0004BAB6 File Offset: 0x00049CB6
	internal override void OnEnable()
	{
		base.OnEnable();
		this.InitToDefault();
	}

	// Token: 0x06000DD4 RID: 3540 RVA: 0x0004BAC4 File Offset: 0x00049CC4
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000DD5 RID: 3541 RVA: 0x0004BAD2 File Offset: 0x00049CD2
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		if (this.hasEffectsGameObject && this.effectsHaveBeenPlayed)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x06000DD6 RID: 3542 RVA: 0x0004BB04 File Offset: 0x00049D04
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand() || this.itemState == TransferrableObject.ItemStates.State1 || !GorillaParent.hasInstance || !this.hitLastFrame)
		{
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minSlamVelocity)
		{
			return;
		}
		Vector3 up = this.tipTransform.up;
		Vector3 up2 = Vector3.up;
		if (Vector3.Angle(up, up2) > this.minSlamAngle)
		{
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
	}

	// Token: 0x06000DD7 RID: 3543 RVA: 0x0004BB88 File Offset: 0x00049D88
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.cooldownRemaining -= Time.deltaTime;
		if (this.cooldownRemaining <= 0f)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
			if (this.hasEffectsGameObject)
			{
				this.effectsGameObject.SetActive(false);
			}
			this.effectsHaveBeenPlayed = false;
		}
		if (base.InHand())
		{
			Vector3 position = base.transform.position;
			Vector3 end = base.transform.TransformPoint(this.tipTargetLocalPosition);
			RaycastHit raycastHit;
			if (Physics.Linecast(position, end, out raycastHit, this.tipCollisionLayerMask))
			{
				this.tipTransform.position = raycastHit.point;
				this.hitLastFrame = true;
			}
			else
			{
				this.tipTransform.localPosition = this.tipTargetLocalPosition;
				this.hitLastFrame = false;
			}
			if (this.itemState == TransferrableObject.ItemStates.State1 && this.hasEffectsGameObject && !this.effectsHaveBeenPlayed)
			{
				this.effectsGameObject.SetActive(true);
				this.effectsHaveBeenPlayed = true;
			}
		}
	}

	// Token: 0x06000DD8 RID: 3544 RVA: 0x0004BC78 File Offset: 0x00049E78
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState == TransferrableObject.ItemStates.State1 && !this.effectsHaveBeenPlayed)
		{
			this.cooldownRemaining = this.cooldown;
		}
	}

	// Token: 0x04001084 RID: 4228
	[Tooltip("This GameObject will activate when the staff hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04001085 RID: 4229
	[Tooltip("The Transform of the staff's tip which will be used to determine if the staff is being slammed. Up axis (Y) should point along the length of the staff.")]
	public Transform tipTransform;

	// Token: 0x04001086 RID: 4230
	public float tipCollisionRadius = 0.05f;

	// Token: 0x04001087 RID: 4231
	public LayerMask tipCollisionLayerMask;

	// Token: 0x04001088 RID: 4232
	[Tooltip("Used to calculate velocity of the staff.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001089 RID: 4233
	public float cooldown = 5f;

	// Token: 0x0400108A RID: 4234
	[Tooltip("The velocity of the staff's tip must be greater than this value to activate the effect.")]
	public float minSlamVelocity = 0.5f;

	// Token: 0x0400108B RID: 4235
	[Tooltip("The angle (in degrees) between the staff's tip and the ground must be less than this value to activate the effect.")]
	public float minSlamAngle = 5f;

	// Token: 0x0400108C RID: 4236
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x0400108D RID: 4237
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x0400108E RID: 4238
	private Vector3 tipTargetLocalPosition;

	// Token: 0x0400108F RID: 4239
	private bool hasEffectsGameObject;

	// Token: 0x04001090 RID: 4240
	private bool effectsHaveBeenPlayed;
}
