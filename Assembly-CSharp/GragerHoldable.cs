using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002D6 RID: 726
public class GragerHoldable : MonoBehaviour
{
	// Token: 0x06001295 RID: 4757 RVA: 0x00062E04 File Offset: 0x00061004
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.lastWorldPosition = base.transform.TransformPoint(this.LocalCenterOfMass);
		this.lastClackParentLocalPosition = base.transform.parent.InverseTransformPoint(this.lastWorldPosition);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x00062E78 File Offset: 0x00061078
	private void Update()
	{
		Vector3 target = base.transform.TransformPoint(this.LocalCenterOfMass);
		Vector3 a = this.lastWorldPosition + this.velocity * Time.deltaTime * this.drag;
		Vector3 vector = base.transform.parent.TransformDirection(this.LocalRotationAxis);
		Vector3 vector2 = base.transform.position + (a - base.transform.position).ProjectOntoPlane(vector).normalized * this.centerOfMassRadius;
		vector2 = Vector3.MoveTowards(vector2, target, this.localFriction * Time.deltaTime);
		this.velocity = (vector2 - this.lastWorldPosition) / Time.deltaTime;
		this.velocity += Vector3.down * this.gravity * Time.deltaTime;
		this.lastWorldPosition = vector2;
		base.transform.rotation = Quaternion.LookRotation(vector2 - base.transform.position, vector) * this.RotationCorrection;
		Vector3 a2 = base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(this.LocalCenterOfMass));
		if ((a2 - this.lastClackParentLocalPosition).IsLongerThan(this.distancePerClack))
		{
			this.clackAudio.GTPlayOneShot(this.allClacks[Random.Range(0, this.allClacks.Length)], 1f);
			this.lastClackParentLocalPosition = a2;
		}
	}

	// Token: 0x0400169B RID: 5787
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x0400169C RID: 5788
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x0400169D RID: 5789
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x0400169E RID: 5790
	[SerializeField]
	private float drag;

	// Token: 0x0400169F RID: 5791
	[SerializeField]
	private float gravity;

	// Token: 0x040016A0 RID: 5792
	[SerializeField]
	private float localFriction;

	// Token: 0x040016A1 RID: 5793
	[SerializeField]
	private float distancePerClack;

	// Token: 0x040016A2 RID: 5794
	[SerializeField]
	private AudioSource clackAudio;

	// Token: 0x040016A3 RID: 5795
	[SerializeField]
	private AudioClip[] allClacks;

	// Token: 0x040016A4 RID: 5796
	private float centerOfMassRadius;

	// Token: 0x040016A5 RID: 5797
	private Vector3 velocity;

	// Token: 0x040016A6 RID: 5798
	private Vector3 lastWorldPosition;

	// Token: 0x040016A7 RID: 5799
	private Vector3 lastClackParentLocalPosition;

	// Token: 0x040016A8 RID: 5800
	private Quaternion RotationCorrection;
}
