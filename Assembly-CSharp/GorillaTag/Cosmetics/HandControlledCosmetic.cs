using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200128A RID: 4746
	public class HandControlledCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060076E5 RID: 30437 RVA: 0x0027008C File Offset: 0x0026E28C
		public void Awake()
		{
			this.myRig = base.GetComponentInParent<VRRig>();
			this.initialRotation = base.transform.localRotation;
			base.enabled = false;
			if (this.debugRelativePositionTransform1 != null)
			{
				Object.Destroy(this.debugRelativePositionTransform1.gameObject);
			}
			if (this.debugRelativePositionTransform2 != null)
			{
				Object.Destroy(this.debugRelativePositionTransform2.gameObject);
			}
		}

		// Token: 0x060076E6 RID: 30438 RVA: 0x002700FC File Offset: 0x0026E2FC
		private void SetControlIndicatorPoints()
		{
			if (this.myRig.isOfflineVRRig && this.controllingHand != null && this.controlIndicatorCurve != null && this.controlIndicatorCurve.points != null)
			{
				this.controlIndicatorCurve.points[0] = this.controllingHand.position;
				this.controlIndicatorCurve.points[1] = this.controlIndicatorCurve.points[0] + this.myRig.scaleFactor * this.controllingHand.up;
				this.controlIndicatorCurve.points[2] = base.transform.position;
			}
		}

		// Token: 0x060076E7 RID: 30439 RVA: 0x002701C2 File Offset: 0x0026E3C2
		private Vector3 GetRelativeHandPosition()
		{
			return this.controllingHand.TransformPoint(this.handPositionOffset) - this.myRig.bodyTransform.position;
		}

		// Token: 0x060076E8 RID: 30440 RVA: 0x002701EC File Offset: 0x0026E3EC
		public void StartControl(bool leftHand, float flexValue)
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.lowAngleLimits = this.activeSettings.angleLimits;
			this.highAngleLimits = 360f * Vector3.one - this.lowAngleLimits;
			this.handRotationOffset = (leftHand ? this.leftHandRotation : this.rightHandRotation);
			this.controllingHand = (leftHand ? this.myRig.leftHand.rigTarget.transform : this.myRig.rightHand.rigTarget.transform);
			this.startHandRelativePosition = this.GetRelativeHandPosition();
			this.startHandInverseRotation = Quaternion.Inverse(this.controllingHand.rotation * this.handRotationOffset);
			this.isActive = true;
			this.SetControlIndicatorPoints();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060076E9 RID: 30441 RVA: 0x002702CC File Offset: 0x0026E4CC
		public void StopControl()
		{
			this.localEuler = base.transform.localRotation.eulerAngles;
			this.isActive = false;
			this.SetControlIndicatorPoints();
		}

		// Token: 0x060076EA RID: 30442 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnEnable()
		{
		}

		// Token: 0x060076EB RID: 30443 RVA: 0x002702FF File Offset: 0x0026E4FF
		public void OnDisable()
		{
			base.transform.localRotation = this.initialRotation;
			this.StopControl();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060076EC RID: 30444 RVA: 0x0027031E File Offset: 0x0026E51E
		private float ReverseClampDegrees(float value, float low, float high)
		{
			value = Mathf.Repeat(value, 360f);
			if (value <= low || value >= high)
			{
				return value;
			}
			if (value >= 180f)
			{
				return high;
			}
			return low;
		}

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x060076ED RID: 30445 RVA: 0x00270342 File Offset: 0x0026E542
		// (set) Token: 0x060076EE RID: 30446 RVA: 0x0027034A File Offset: 0x0026E54A
		public bool TickRunning { get; set; }

		// Token: 0x060076EF RID: 30447 RVA: 0x00270354 File Offset: 0x0026E554
		public void Tick()
		{
			if (this.isActive)
			{
				HandControlledCosmetic.RotationControl rotationControl = this.activeSettings.rotationControl;
				if (rotationControl != HandControlledCosmetic.RotationControl.Angle)
				{
					if (rotationControl == HandControlledCosmetic.RotationControl.Translation)
					{
						Vector3 relativeHandPosition = this.GetRelativeHandPosition();
						Vector3 to = new Vector3(relativeHandPosition.x, 0f, relativeHandPosition.z);
						float num = Vector3.SignedAngle(new Vector3(this.startHandRelativePosition.x, 0f, this.startHandRelativePosition.z), to, Vector3.up);
						float num2 = 50f * (this.startHandRelativePosition.y - relativeHandPosition.y) / this.myRig.scaleFactor;
						float time = Vector3.Distance(this.startHandRelativePosition, relativeHandPosition) / this.myRig.scaleFactor;
						this.localEuler += Time.deltaTime * new Vector3(this.activeSettings.verticalSensitivity.Evaluate(time) * num2, this.activeSettings.horizontalSensitivity.Evaluate(time) * num, 0f);
						this.startHandRelativePosition = Vector3.MoveTowards(this.startHandRelativePosition, relativeHandPosition, Time.deltaTime * this.activeSettings.inputDecayCurve.Evaluate(time));
					}
				}
				else
				{
					Quaternion quaternion = this.controllingHand.rotation * this.handRotationOffset;
					Quaternion quaternion2 = this.startHandInverseRotation * quaternion;
					this.localEuler += this.activeSettings.inputSensitivity * quaternion2.eulerAngles;
					float t = 1f - Mathf.Exp(-this.activeSettings.inputDecaySpeed * Time.deltaTime);
					this.startHandInverseRotation = Quaternion.Slerp(this.startHandInverseRotation, Quaternion.Inverse(quaternion), t);
				}
				for (int i = 0; i < 3; i++)
				{
					this.localEuler[i] = this.ReverseClampDegrees(this.localEuler[i], this.lowAngleLimits[i], this.highAngleLimits[i]);
				}
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(this.localEuler), 1f - Mathf.Exp(-this.activeSettings.rotationSpeed * Time.deltaTime));
				return;
			}
			Quaternion localRotation = Quaternion.Slerp(base.transform.localRotation, this.initialRotation, 1f - Mathf.Exp(-this.inactiveSettings.rotationSpeed * Time.deltaTime));
			base.transform.localRotation = localRotation;
			this.localEuler = localRotation.eulerAngles;
		}

		// Token: 0x04008919 RID: 35097
		[SerializeField]
		private HandControlledSettingsSO activeSettings;

		// Token: 0x0400891A RID: 35098
		[SerializeField]
		private HandControlledSettingsSO inactiveSettings;

		// Token: 0x0400891B RID: 35099
		[SerializeField]
		private Vector3 handPositionOffset;

		// Token: 0x0400891C RID: 35100
		[SerializeField]
		private Quaternion rightHandRotation;

		// Token: 0x0400891D RID: 35101
		[SerializeField]
		private Quaternion leftHandRotation;

		// Token: 0x0400891E RID: 35102
		private Quaternion handRotationOffset;

		// Token: 0x0400891F RID: 35103
		[SerializeField]
		private BezierCurve controlIndicatorCurve;

		// Token: 0x04008920 RID: 35104
		[SerializeField]
		private Transform debugRelativePositionTransform1;

		// Token: 0x04008921 RID: 35105
		[SerializeField]
		private Transform debugRelativePositionTransform2;

		// Token: 0x04008922 RID: 35106
		private VRRig myRig;

		// Token: 0x04008923 RID: 35107
		private Transform controllingHand;

		// Token: 0x04008924 RID: 35108
		private Vector3 startHandRelativePosition;

		// Token: 0x04008925 RID: 35109
		private Vector3 lowAngleLimits;

		// Token: 0x04008926 RID: 35110
		private Vector3 highAngleLimits;

		// Token: 0x04008927 RID: 35111
		private Vector3 localEuler;

		// Token: 0x04008928 RID: 35112
		private Quaternion startHandInverseRotation;

		// Token: 0x04008929 RID: 35113
		private Quaternion initialRotation;

		// Token: 0x0400892A RID: 35114
		private bool isActive;

		// Token: 0x0200128B RID: 4747
		public enum RotationControl
		{
			// Token: 0x0400892D RID: 35117
			Angle,
			// Token: 0x0400892E RID: 35118
			Translation
		}
	}
}
