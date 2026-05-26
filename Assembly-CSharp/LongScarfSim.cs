using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020002E2 RID: 738
public class LongScarfSim : MonoBehaviour
{
	// Token: 0x060012C8 RID: 4808 RVA: 0x00063DB4 File Offset: 0x00061FB4
	private void Start()
	{
		this.clampToPlane.Normalize();
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.baseLocalRotations = new Quaternion[this.gameObjects.Length];
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.baseLocalRotations[i] = this.gameObjects[i].transform.localRotation;
		}
	}

	// Token: 0x060012C9 RID: 4809 RVA: 0x00063E1C File Offset: 0x0006201C
	private void LateUpdate()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 a = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector = position + (a - position).normalized * this.centerOfMassLength;
		Vector3 vector2 = base.transform.InverseTransformPoint(vector);
		float num = Vector3.Dot(vector2, this.clampToPlane);
		if (num < 0f)
		{
			vector2 -= this.clampToPlane * num;
			vector = base.transform.TransformPoint(vector2);
		}
		Vector3 a2 = vector;
		this.velocity = (a2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = a2;
		float target = (float)(this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold) ? 1 : 0);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, target, this.blendAmountPerSecond * Time.deltaTime);
		Quaternion b = Quaternion.LookRotation(a2 - position);
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			Quaternion a3 = this.gameObjects[i].transform.parent.rotation * this.baseLocalRotations[i];
			this.gameObjects[i].transform.rotation = Quaternion.Lerp(a3, b, this.currentBlend);
		}
	}

	// Token: 0x040016F6 RID: 5878
	[SerializeField]
	private GameObject[] gameObjects;

	// Token: 0x040016F7 RID: 5879
	[SerializeField]
	private float speedThreshold = 1f;

	// Token: 0x040016F8 RID: 5880
	[SerializeField]
	private float blendAmountPerSecond = 1f;

	// Token: 0x040016F9 RID: 5881
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040016FA RID: 5882
	private Quaternion[] baseLocalRotations;

	// Token: 0x040016FB RID: 5883
	private float currentBlend;

	// Token: 0x040016FC RID: 5884
	[SerializeField]
	private float centerOfMassLength;

	// Token: 0x040016FD RID: 5885
	[SerializeField]
	private float gravityStrength;

	// Token: 0x040016FE RID: 5886
	[SerializeField]
	private float drag;

	// Token: 0x040016FF RID: 5887
	[SerializeField]
	private Vector3 clampToPlane;

	// Token: 0x04001700 RID: 5888
	private Vector3 lastCenterPos;

	// Token: 0x04001701 RID: 5889
	private Vector3 velocity;
}
