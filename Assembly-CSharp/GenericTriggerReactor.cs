using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000AE RID: 174
public class GenericTriggerReactor : MonoBehaviour, IBuildValidation
{
	// Token: 0x06000430 RID: 1072 RVA: 0x000188BA File Offset: 0x00016ABA
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.ComponentName.Length == 0)
		{
			return true;
		}
		if (Type.GetType(this.ComponentName) == null)
		{
			Debug.LogError("GenericTriggerReactor :: ComponentName must specify a valid Component or be empty.");
			return false;
		}
		return true;
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x000188EB File Offset: 0x00016AEB
	private void Awake()
	{
		this.componentType = Type.GetType(this.ComponentName);
		base.TryGetComponent<GorillaVelocityEstimator>(out this.gorillaVelocityEstimator);
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x0001890B File Offset: 0x00016B0B
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeEnter, this.GTOnTriggerEnter, this.idealMotionPlayRangeEnter);
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00018926 File Offset: 0x00016B26
	private void OnTriggerExit(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeExit, this.GTOnTriggerExit, this.idealMotionPlayRangeExit);
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00018944 File Offset: 0x00016B44
	private void OnTriggerTest(Collider other, Vector2 speedRange, UnityEvent unityEvent, Vector2 idealMotionPlay)
	{
		Component component;
		if (unityEvent != null && (this.componentType == null || other.TryGetComponent(this.componentType, out component)))
		{
			if (this.gorillaVelocityEstimator != null)
			{
				float magnitude = this.gorillaVelocityEstimator.linearVelocity.magnitude;
				if (magnitude < speedRange.x || magnitude > speedRange.y)
				{
					return;
				}
				if (this.idealMotion != null)
				{
					float num = Vector3.Dot(this.gorillaVelocityEstimator.linearVelocity.normalized, this.idealMotion.forward);
					if (num < idealMotionPlay.x || num > idealMotionPlay.y)
					{
						return;
					}
				}
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x04000491 RID: 1169
	[SerializeField]
	private string ComponentName = string.Empty;

	// Token: 0x04000492 RID: 1170
	[Space]
	[SerializeField]
	private Vector2 speedRangeEnter;

	// Token: 0x04000493 RID: 1171
	[SerializeField]
	private Vector2 speedRangeExit;

	// Token: 0x04000494 RID: 1172
	[Space]
	[SerializeField]
	private Transform idealMotion;

	// Token: 0x04000495 RID: 1173
	[SerializeField]
	private Vector2 idealMotionPlayRangeEnter;

	// Token: 0x04000496 RID: 1174
	[SerializeField]
	private Vector2 idealMotionPlayRangeExit;

	// Token: 0x04000497 RID: 1175
	[Space]
	[SerializeField]
	private UnityEvent GTOnTriggerEnter;

	// Token: 0x04000498 RID: 1176
	[SerializeField]
	private UnityEvent GTOnTriggerExit;

	// Token: 0x04000499 RID: 1177
	private Type componentType;

	// Token: 0x0400049A RID: 1178
	private GorillaVelocityEstimator gorillaVelocityEstimator;
}
