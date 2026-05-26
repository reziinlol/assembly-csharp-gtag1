using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002AF RID: 687
public class ClackerCosmetic : MonoBehaviour
{
	// Token: 0x060011D1 RID: 4561 RVA: 0x0005F5C0 File Offset: 0x0005D7C0
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.arm1.parent = this;
		this.arm2.parent = this;
		this.arm1.transform = this.clackerArm1;
		this.arm2.transform = this.clackerArm2;
		this.arm1.lastWorldPosition = this.clackerArm1.transform.TransformPoint(this.LocalCenterOfMass);
		this.arm2.lastWorldPosition = this.clackerArm2.transform.TransformPoint(this.LocalCenterOfMass);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0005F67C File Offset: 0x0005D87C
	private void Update()
	{
		Vector3 lastWorldPosition = this.arm1.lastWorldPosition;
		this.arm1.UpdateArm();
		this.arm2.UpdateArm();
		ref Vector3 eulerAngles = this.clackerArm1.transform.eulerAngles;
		Vector3 eulerAngles2 = this.clackerArm2.transform.eulerAngles;
		Mathf.DeltaAngle(eulerAngles.y, eulerAngles2.y);
		if ((this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).IsShorterThan(this.collisionDistance))
		{
			float sqrMagnitude = (this.arm1.velocity - this.arm2.velocity).sqrMagnitude;
			if (this.parentHoldable.InHand())
			{
				if (sqrMagnitude > this.heavyClackSpeed * this.heavyClackSpeed)
				{
					this.heavyClackAudio.Play();
				}
				else if (sqrMagnitude > this.mediumClackSpeed * this.mediumClackSpeed)
				{
					this.mediumClackAudio.Play();
				}
				else if (sqrMagnitude > this.minimumClackSpeed * this.minimumClackSpeed)
				{
					this.lightClackAudio.Play();
				}
			}
			Vector3 a = (this.arm1.lastWorldPosition + this.arm2.lastWorldPosition) / 2f;
			Vector3 vector = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * (this.collisionDistance + 0.001f) / 2f;
			Vector3 b = a + vector;
			Vector3 b2 = a - vector;
			if ((lastWorldPosition - b).IsLongerThan(lastWorldPosition - b2))
			{
				vector = -vector;
			}
			this.arm1.SetPosition(a + vector);
			this.arm2.SetPosition(a - vector);
			ref Vector3 ptr = ref this.arm1.velocity;
			Vector3 velocity = this.arm2.velocity;
			Vector3 velocity2 = this.arm1.velocity;
			ptr = velocity;
			this.arm2.velocity = velocity2;
			Vector3 b3 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * this.pushApartStrength * Mathf.Sqrt(sqrMagnitude);
			this.arm1.velocity = this.arm1.velocity + b3;
			this.arm2.velocity = this.arm2.velocity - b3;
		}
	}

	// Token: 0x04001561 RID: 5473
	[SerializeField]
	private TransferrableObject parentHoldable;

	// Token: 0x04001562 RID: 5474
	[SerializeField]
	private Transform clackerArm1;

	// Token: 0x04001563 RID: 5475
	[SerializeField]
	private Transform clackerArm2;

	// Token: 0x04001564 RID: 5476
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x04001565 RID: 5477
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x04001566 RID: 5478
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x04001567 RID: 5479
	[SerializeField]
	private float drag;

	// Token: 0x04001568 RID: 5480
	[SerializeField]
	private float gravity;

	// Token: 0x04001569 RID: 5481
	[SerializeField]
	private float localFriction;

	// Token: 0x0400156A RID: 5482
	[SerializeField]
	private float minimumClackSpeed;

	// Token: 0x0400156B RID: 5483
	[SerializeField]
	private SoundBankPlayer lightClackAudio;

	// Token: 0x0400156C RID: 5484
	[SerializeField]
	private float mediumClackSpeed;

	// Token: 0x0400156D RID: 5485
	[SerializeField]
	private SoundBankPlayer mediumClackAudio;

	// Token: 0x0400156E RID: 5486
	[SerializeField]
	private float heavyClackSpeed;

	// Token: 0x0400156F RID: 5487
	[SerializeField]
	private SoundBankPlayer heavyClackAudio;

	// Token: 0x04001570 RID: 5488
	[SerializeField]
	private float collisionDistance;

	// Token: 0x04001571 RID: 5489
	private float centerOfMassRadius;

	// Token: 0x04001572 RID: 5490
	[SerializeField]
	private float pushApartStrength;

	// Token: 0x04001573 RID: 5491
	private ClackerCosmetic.PerArmData arm1;

	// Token: 0x04001574 RID: 5492
	private ClackerCosmetic.PerArmData arm2;

	// Token: 0x04001575 RID: 5493
	private Quaternion RotationCorrection;

	// Token: 0x020002B0 RID: 688
	private struct PerArmData
	{
		// Token: 0x060011D4 RID: 4564 RVA: 0x0005F908 File Offset: 0x0005DB08
		public void UpdateArm()
		{
			Vector3 target = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
			Vector3 a = this.lastWorldPosition + this.velocity * Time.deltaTime * this.parent.drag;
			Vector3 vector = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			Vector3 vector2 = this.transform.position + (a - this.transform.position).ProjectOntoPlane(vector).normalized * this.parent.centerOfMassRadius;
			vector2 = Vector3.MoveTowards(vector2, target, this.parent.localFriction * Time.deltaTime);
			this.velocity = (vector2 - this.lastWorldPosition) / Time.deltaTime;
			this.velocity += Vector3.down * this.parent.gravity * Time.deltaTime;
			this.lastWorldPosition = vector2;
			this.transform.rotation = Quaternion.LookRotation(vector, vector2 - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0005FA70 File Offset: 0x0005DC70
		public void SetPosition(Vector3 newPosition)
		{
			Vector3 forward = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			this.transform.rotation = Quaternion.LookRotation(forward, newPosition - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		// Token: 0x04001576 RID: 5494
		public ClackerCosmetic parent;

		// Token: 0x04001577 RID: 5495
		public Transform transform;

		// Token: 0x04001578 RID: 5496
		public Vector3 velocity;

		// Token: 0x04001579 RID: 5497
		public Vector3 lastWorldPosition;
	}
}
