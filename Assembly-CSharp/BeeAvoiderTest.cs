using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class BeeAvoiderTest : MonoBehaviour
{
	// Token: 0x06000E38 RID: 3640 RVA: 0x0004E678 File Offset: 0x0004C878
	public void Update()
	{
		Vector3 position = this.patrolPoints[this.nextPatrolPoint].transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 target = (position - position2).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * this.drag, target, this.acceleration);
		if ((position2 - position).IsLongerThan(this.instabilityOffRadius))
		{
			this.velocity += Random.insideUnitSphere * this.instability * Time.deltaTime;
		}
		Vector3 vector = position2 + this.velocity * Time.deltaTime;
		GameObject[] array = this.avoidancePoints;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position3 = array[i].transform.position;
			if ((vector - position3).IsShorterThan(this.avoidRadius))
			{
				Vector3 normalized = Vector3.Cross(position3 - vector, position - vector).normalized;
				Vector3 normalized2 = (position - position3).normalized;
				float num = Vector3.Dot(vector - position3, normalized);
				Vector3 b = (this.avoidRadius - num) * normalized;
				vector += b;
				this.velocity += b;
			}
		}
		base.transform.position = vector;
		base.transform.rotation = Quaternion.LookRotation(position - vector);
		if ((vector - position).IsShorterThan(this.patrolArrivedRadius))
		{
			this.nextPatrolPoint = (this.nextPatrolPoint + 1) % this.patrolPoints.Length;
		}
	}

	// Token: 0x04001132 RID: 4402
	public GameObject[] patrolPoints;

	// Token: 0x04001133 RID: 4403
	public GameObject[] avoidancePoints;

	// Token: 0x04001134 RID: 4404
	public float speed;

	// Token: 0x04001135 RID: 4405
	public float acceleration;

	// Token: 0x04001136 RID: 4406
	public float instability;

	// Token: 0x04001137 RID: 4407
	public float instabilityOffRadius;

	// Token: 0x04001138 RID: 4408
	public float drag;

	// Token: 0x04001139 RID: 4409
	public float avoidRadius;

	// Token: 0x0400113A RID: 4410
	public float patrolArrivedRadius;

	// Token: 0x0400113B RID: 4411
	private int nextPatrolPoint;

	// Token: 0x0400113C RID: 4412
	private Vector3 velocity;
}
