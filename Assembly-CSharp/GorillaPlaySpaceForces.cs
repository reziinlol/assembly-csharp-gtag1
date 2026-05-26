using System;
using UnityEngine;

// Token: 0x020005CD RID: 1485
public class GorillaPlaySpaceForces : MonoBehaviour
{
	// Token: 0x06002532 RID: 9522 RVA: 0x000C5DE0 File Offset: 0x000C3FE0
	private void Start()
	{
		this.playspaceRigidbody = base.GetComponent<Rigidbody>();
		this.leftHandRigidbody = this.leftHand.GetComponent<Rigidbody>();
		this.leftHandCollider = this.leftHand.GetComponent<Collider>();
		this.rightHandRigidbody = this.rightHand.GetComponent<Rigidbody>();
		this.rightHandCollider = this.rightHand.GetComponent<Collider>();
	}

	// Token: 0x06002533 RID: 9523 RVA: 0x000C5E3D File Offset: 0x000C403D
	private void FixedUpdate()
	{
		if (Time.time >= 0.1f)
		{
			this.bodyCollider.transform.position = this.headsetTransform.position + this.bodyColliderOffset;
		}
	}

	// Token: 0x040030A8 RID: 12456
	public GameObject rightHand;

	// Token: 0x040030A9 RID: 12457
	public GameObject leftHand;

	// Token: 0x040030AA RID: 12458
	public Collider bodyCollider;

	// Token: 0x040030AB RID: 12459
	private Collider leftHandCollider;

	// Token: 0x040030AC RID: 12460
	private Collider rightHandCollider;

	// Token: 0x040030AD RID: 12461
	public Transform rightHandTransform;

	// Token: 0x040030AE RID: 12462
	public Transform leftHandTransform;

	// Token: 0x040030AF RID: 12463
	private Rigidbody leftHandRigidbody;

	// Token: 0x040030B0 RID: 12464
	private Rigidbody rightHandRigidbody;

	// Token: 0x040030B1 RID: 12465
	public Vector3 bodyColliderOffset;

	// Token: 0x040030B2 RID: 12466
	public float forceConstant;

	// Token: 0x040030B3 RID: 12467
	private Vector3 lastLeftHandPosition;

	// Token: 0x040030B4 RID: 12468
	private Vector3 lastRightHandPosition;

	// Token: 0x040030B5 RID: 12469
	private Rigidbody playspaceRigidbody;

	// Token: 0x040030B6 RID: 12470
	public Transform headsetTransform;
}
