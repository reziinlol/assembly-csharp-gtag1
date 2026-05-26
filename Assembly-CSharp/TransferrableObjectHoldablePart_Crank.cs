using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004CC RID: 1228
public class TransferrableObjectHoldablePart_Crank : TransferrableObjectHoldablePart
{
	// Token: 0x06001DD8 RID: 7640 RVA: 0x000A0AD7 File Offset: 0x0009ECD7
	public void SetOnCrankedCallback(Action<float> onCrankedCallback)
	{
		this.onCrankedCallback = onCrankedCallback;
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x000A0AE0 File Offset: 0x0009ECE0
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x);
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x000A0BC8 File Offset: 0x0009EDC8
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		Vector3 a;
		if (rig.isOfflineVRRig)
		{
			Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
			Vector3 vector = this.rotatingPart.InverseTransformPoint(controllerTransform.position);
			Vector3 position = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
			Vector3 vector2 = this.rotatingPart.TransformPoint(position);
			if (this.maxHandSnapDistance > 0f && (controllerTransform.position - vector2).IsLongerThan(this.maxHandSnapDistance))
			{
				this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
				return;
			}
			controllerTransform.position = vector2;
			a = controllerTransform.position;
		}
		else
		{
			VRMap vrmap = isHeldLeftHand ? rig.leftHand : rig.rightHand;
			a = vrmap.GetExtrapolatedControllerPosition();
			a -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(isHeldLeftHand) * rig.scaleFactor;
		}
		Vector3 vector3 = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (a - this.rotatingPart.position);
		float num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		if (num2 != 0f)
		{
			if (this.onCrankedCallback != null)
			{
				this.onCrankedCallback(num2);
			}
			for (int i = 0; i < this.thresholds.Length; i++)
			{
				this.thresholds[i].OnCranked(num2);
			}
		}
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(num - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x000A0DC8 File Offset: 0x0009EFC8
	private void OnDrawGizmosSelected()
	{
		Transform transform = (this.rotatingPart != null) ? this.rotatingPart : base.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x04002822 RID: 10274
	[SerializeField]
	private float crankHandleX;

	// Token: 0x04002823 RID: 10275
	[SerializeField]
	private float crankHandleY;

	// Token: 0x04002824 RID: 10276
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x04002825 RID: 10277
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x04002826 RID: 10278
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04002827 RID: 10279
	private float crankAngleOffset;

	// Token: 0x04002828 RID: 10280
	private float crankRadius;

	// Token: 0x04002829 RID: 10281
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x0400282A RID: 10282
	private float lastAngle;

	// Token: 0x0400282B RID: 10283
	private Quaternion baseLocalAngle;

	// Token: 0x0400282C RID: 10284
	private Quaternion baseLocalAngleInverse;

	// Token: 0x0400282D RID: 10285
	private Action<float> onCrankedCallback;

	// Token: 0x0400282E RID: 10286
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank.CrankThreshold[] thresholds;

	// Token: 0x020004CD RID: 1229
	[Serializable]
	private struct CrankThreshold
	{
		// Token: 0x06001DDD RID: 7645 RVA: 0x000A0E43 File Offset: 0x0009F043
		public void OnCranked(float deltaAngle)
		{
			this.currentAngle += deltaAngle;
			if (Mathf.Abs(this.currentAngle) > this.angleThreshold)
			{
				this.currentAngle = 0f;
				this.onReached.Invoke();
			}
		}

		// Token: 0x0400282F RID: 10287
		public float angleThreshold;

		// Token: 0x04002830 RID: 10288
		public UnityEvent onReached;

		// Token: 0x04002831 RID: 10289
		[HideInInspector]
		public float currentAngle;
	}
}
