using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class OwlLook : MonoBehaviour
{
	// Token: 0x060021AB RID: 8619 RVA: 0x000B395F File Offset: 0x000B1B5F
	private void Awake()
	{
		this.overlapRigs = new VRRig[20];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000B3988 File Offset: 0x000B1B88
	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			if (!(this.rigs[i] == this.myRig))
			{
				Vector3 rhs = this.rigs[i].tagSound.transform.position - base.transform.position;
				if (rhs.magnitude <= this.lookRadius)
				{
					float num4 = Vector3.Dot(-base.transform.up, rhs.normalized);
					if (num4 > num2)
					{
						this.overlapRigs[num3++] = this.rigs[i];
					}
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 rhs = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, rhs);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 vector = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector2 = this.neck.InverseTransformDirection(vector);
		vector2.y = Mathf.Clamp(vector2.y, this.minNeckY, this.maxNeckY);
		vector = this.neck.TransformDirection(vector2.normalized);
		Vector3 forward = Vector3.RotateTowards(this.head.forward, vector, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(forward, this.neck.up);
	}

	// Token: 0x04002C7C RID: 11388
	public Transform head;

	// Token: 0x04002C7D RID: 11389
	public Transform lookTarget;

	// Token: 0x04002C7E RID: 11390
	public Transform neck;

	// Token: 0x04002C7F RID: 11391
	public float lookRadius = 0.5f;

	// Token: 0x04002C80 RID: 11392
	public Collider[] overlapColliders;

	// Token: 0x04002C81 RID: 11393
	public VRRig[] rigs = new VRRig[20];

	// Token: 0x04002C82 RID: 11394
	public VRRig[] overlapRigs;

	// Token: 0x04002C83 RID: 11395
	public float rotSpeed = 1f;

	// Token: 0x04002C84 RID: 11396
	public float lookAtAngleDegrees = 60f;

	// Token: 0x04002C85 RID: 11397
	public float maxNeckY;

	// Token: 0x04002C86 RID: 11398
	public float minNeckY;

	// Token: 0x04002C87 RID: 11399
	public VRRig myRig;
}
