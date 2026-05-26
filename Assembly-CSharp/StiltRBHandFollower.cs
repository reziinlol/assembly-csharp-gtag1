using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class StiltRBHandFollower : MonoBehaviour
{
	// Token: 0x06000A00 RID: 2560 RVA: 0x00035BF1 File Offset: 0x00033DF1
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.maxAngularVelocity = this.angularSpeedLimit;
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00035C10 File Offset: 0x00033E10
	private void FixedUpdate()
	{
		Vector3 a = this.targetHand.TransformPoint(this.handOffset);
		float d;
		Vector3 a2;
		(this.targetHand.TransformRotation(this.handRotOffset) * Quaternion.Inverse(this.rb.transform.rotation)).ToAngleAxis(out d, out a2);
		this.rb.linearVelocity = (a - this.rb.transform.position) / Time.fixedDeltaTime;
		this.rb.angularVelocity = a2 * d * 0.017453292f / Time.fixedDeltaTime;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00035CB7 File Offset: 0x00033EB7
	private void OnCollisionEnter(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00035CB7 File Offset: 0x00033EB7
	private void OnCollisionStay(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00035CDB File Offset: 0x00033EDB
	private void OnCollisionExit(Collision collision)
	{
		this.collisions.Remove(collision.collider);
	}

	// Token: 0x04000C4C RID: 3148
	private Rigidbody rb;

	// Token: 0x04000C4D RID: 3149
	[SerializeField]
	private Transform targetHand;

	// Token: 0x04000C4E RID: 3150
	[SerializeField]
	private Vector3 handOffset;

	// Token: 0x04000C4F RID: 3151
	[SerializeField]
	private Quaternion handRotOffset = Quaternion.identity;

	// Token: 0x04000C50 RID: 3152
	[SerializeField]
	private float angularSpeedLimit;

	// Token: 0x04000C51 RID: 3153
	private Dictionary<Collider, Vector3> collisions = new Dictionary<Collider, Vector3>();
}
