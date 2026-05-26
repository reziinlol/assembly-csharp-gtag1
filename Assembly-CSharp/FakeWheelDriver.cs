using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002C3 RID: 707
public class FakeWheelDriver : MonoBehaviour
{
	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x0600123C RID: 4668 RVA: 0x000618F8 File Offset: 0x0005FAF8
	// (set) Token: 0x0600123D RID: 4669 RVA: 0x00061900 File Offset: 0x0005FB00
	public bool hasCollision { get; private set; }

	// Token: 0x0600123E RID: 4670 RVA: 0x00061909 File Offset: 0x0005FB09
	public void SetThrust(Vector3 thrust)
	{
		this.thrust = thrust;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00061914 File Offset: 0x0005FB14
	private void OnCollisionStay(Collision collision)
	{
		int num = 0;
		Vector3 a = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			if (contactPoint.thisCollider == this.wheelCollider)
			{
				a += contactPoint.point;
				num++;
			}
		}
		if (num > 0)
		{
			this.collisionNormal = collision.contacts[0].normal;
			this.collisionPoint = a / (float)num;
			this.hasCollision = true;
		}
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x000619A0 File Offset: 0x0005FBA0
	private void FixedUpdate()
	{
		if (this.hasCollision)
		{
			Vector3 vector = base.transform.rotation * this.thrust;
			if (this.myRigidBody.linearVelocity.IsShorterThan(this.maxSpeed))
			{
				vector = vector.ProjectOntoPlane(this.collisionNormal).normalized * this.thrust.magnitude;
				this.myRigidBody.AddForceAtPosition(vector, this.collisionPoint);
			}
			Vector3 vector2 = this.myRigidBody.linearVelocity.ProjectOntoPlane(this.collisionNormal).ProjectOntoPlane(vector.normalized);
			if (vector2.IsLongerThan(this.lateralFrictionForce))
			{
				this.myRigidBody.AddForceAtPosition(-vector2.normalized * this.lateralFrictionForce, this.collisionPoint);
			}
			else
			{
				this.myRigidBody.AddForceAtPosition(-vector2, this.collisionPoint);
			}
		}
		this.hasCollision = false;
	}

	// Token: 0x0400161C RID: 5660
	[SerializeField]
	private Rigidbody myRigidBody;

	// Token: 0x0400161D RID: 5661
	[SerializeField]
	private Vector3 thrust;

	// Token: 0x0400161E RID: 5662
	[SerializeField]
	private Collider wheelCollider;

	// Token: 0x0400161F RID: 5663
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04001620 RID: 5664
	[SerializeField]
	private float lateralFrictionForce;

	// Token: 0x04001622 RID: 5666
	private Vector3 collisionPoint;

	// Token: 0x04001623 RID: 5667
	private Vector3 collisionNormal;
}
