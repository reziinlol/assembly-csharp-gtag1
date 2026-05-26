using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class GorillaVelocityEstimator : MonoBehaviour
{
	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000D56 RID: 3414 RVA: 0x00048E59 File Offset: 0x00047059
	// (set) Token: 0x06000D57 RID: 3415 RVA: 0x00048E61 File Offset: 0x00047061
	public Vector3 linearVelocity { get; private set; }

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000D58 RID: 3416 RVA: 0x00048E6A File Offset: 0x0004706A
	// (set) Token: 0x06000D59 RID: 3417 RVA: 0x00048E72 File Offset: 0x00047072
	public Vector3 angularVelocity { get; private set; }

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000D5A RID: 3418 RVA: 0x00048E7B File Offset: 0x0004707B
	// (set) Token: 0x06000D5B RID: 3419 RVA: 0x00048E83 File Offset: 0x00047083
	public Vector3 handPos { get; private set; }

	// Token: 0x06000D5C RID: 3420 RVA: 0x00048E8C File Offset: 0x0004708C
	private void Awake()
	{
		this.history = new GorillaVelocityEstimator.VelocityHistorySample[this.numFrames];
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x00048EA0 File Offset: 0x000470A0
	private void OnEnable()
	{
		this.currentFrame = 0;
		for (int i = 0; i < this.history.Length; i++)
		{
			this.history[i] = default(GorillaVelocityEstimator.VelocityHistorySample);
		}
		this.lastPos = base.transform.position;
		this.lastRotation = base.transform.rotation;
		GorillaVelocityEstimatorManager.Register(this);
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00048F01 File Offset: 0x00047101
	private void OnDisable()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00048F01 File Offset: 0x00047101
	private void OnDestroy()
	{
		GorillaVelocityEstimatorManager.Unregister(this);
	}

	// Token: 0x06000D60 RID: 3424 RVA: 0x00048F0C File Offset: 0x0004710C
	public void TriggeredLateUpdate()
	{
		Vector3 vector;
		Quaternion lhs;
		base.transform.GetPositionAndRotation(out vector, out lhs);
		Vector3 b = Vector3.zero;
		if (!this.useGlobalSpace)
		{
			b = GTPlayer.Instance.InstantaneousVelocity;
		}
		Vector3 vector2 = (vector - this.lastPos) / Time.deltaTime - b;
		Vector3 vector3 = (lhs * Quaternion.Inverse(this.lastRotation)).eulerAngles;
		if (vector3.x > 180f)
		{
			vector3.x -= 360f;
		}
		if (vector3.y > 180f)
		{
			vector3.y -= 360f;
		}
		if (vector3.z > 180f)
		{
			vector3.z -= 360f;
		}
		vector3 *= 0.017453292f / Time.fixedDeltaTime;
		this.linearVelocity += (vector2 - this.history[this.currentFrame].linear) / (float)this.numFrames;
		this.angularVelocity += (vector3 - this.history[this.currentFrame].angular) / (float)this.numFrames;
		this.history[this.currentFrame] = new GorillaVelocityEstimator.VelocityHistorySample
		{
			linear = vector2,
			angular = vector3
		};
		this.handPos = vector;
		this.currentFrame = (this.currentFrame + 1) % this.numFrames;
		this.lastPos = vector;
		this.lastRotation = lhs;
	}

	// Token: 0x04000FFF RID: 4095
	[Min(1f)]
	[SerializeField]
	private int numFrames = 8;

	// Token: 0x04001003 RID: 4099
	private GorillaVelocityEstimator.VelocityHistorySample[] history;

	// Token: 0x04001004 RID: 4100
	private int currentFrame;

	// Token: 0x04001005 RID: 4101
	private Vector3 lastPos;

	// Token: 0x04001006 RID: 4102
	private Quaternion lastRotation;

	// Token: 0x04001007 RID: 4103
	private Vector3 lastRotationVec;

	// Token: 0x04001008 RID: 4104
	public bool useGlobalSpace;

	// Token: 0x020001FE RID: 510
	public struct VelocityHistorySample
	{
		// Token: 0x04001009 RID: 4105
		public Vector3 linear;

		// Token: 0x0400100A RID: 4106
		public Vector3 angular;
	}
}
