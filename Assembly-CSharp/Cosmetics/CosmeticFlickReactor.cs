using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x02001128 RID: 4392
	public class CosmeticFlickReactor : MonoBehaviour
	{
		// Token: 0x06006F90 RID: 28560 RVA: 0x00246DB7 File Offset: 0x00244FB7
		private void Reset()
		{
			if (this.speedTracker == null)
			{
				this.speedTracker = base.GetComponent<SimpleSpeedTracker>();
			}
			if (this.rb == null)
			{
				this.rb = base.GetComponent<Rigidbody>();
			}
		}

		// Token: 0x06006F91 RID: 28561 RVA: 0x00246DF0 File Offset: 0x00244FF0
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = (this.rig != null && this.rig.isLocal);
			this.ResetState();
			this.blockUntilTime = 0f;
			this.hasLastPosition = false;
		}

		// Token: 0x06006F92 RID: 28562 RVA: 0x00246E74 File Offset: 0x00245074
		private void Update()
		{
			Vector3 axis = this.ResolveAxisDirection();
			if (axis.sqrMagnitude < 0.5f)
			{
				return;
			}
			float signedSpeedAlong = this.GetSignedSpeedAlong(axis);
			if (Mathf.Abs(signedSpeedAlong) >= this.minSpeedThreshold)
			{
				int num = (signedSpeedAlong > 0f) ? 1 : -1;
				if (num != this.lastPeakSign || Mathf.Abs(signedSpeedAlong) > Mathf.Abs(this.lastPeakSpeed))
				{
					if (this.lastPeakSign == 0 || num != -this.lastPeakSign)
					{
						this.lastPeakSign = num;
						this.lastPeakSpeed = signedSpeedAlong;
						this.lastPeakTime = Time.time;
						return;
					}
					float num2 = Time.time - this.lastPeakTime;
					float num3 = Mathf.Abs(this.lastPeakSpeed) + Mathf.Abs(signedSpeedAlong);
					bool flag = num2 <= this.flickWindowSeconds;
					bool flag2 = num3 >= this.directionChangeRequired;
					bool flag3 = Time.time >= this.blockUntilTime;
					if (flag && flag2 && flag3)
					{
						this.FireEvents(Mathf.Abs(signedSpeedAlong));
						this.blockUntilTime = Time.time + this.retriggerBufferSeconds;
						this.ResetState();
						return;
					}
					this.lastPeakSign = num;
					this.lastPeakSpeed = signedSpeedAlong;
					this.lastPeakTime = Time.time;
					return;
				}
			}
			else if (Time.time - this.lastPeakTime > this.flickWindowSeconds)
			{
				this.ResetState();
			}
		}

		// Token: 0x06006F93 RID: 28563 RVA: 0x00246FC0 File Offset: 0x002451C0
		private Vector3 ResolveAxisDirection()
		{
			switch (this.axisMode)
			{
			case CosmeticFlickReactor.AxisMode.X:
				if (!this.useWorldAxes)
				{
					return base.transform.right;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.right;
				}
				return this.worldSpace.right;
			case CosmeticFlickReactor.AxisMode.Y:
				if (!this.useWorldAxes)
				{
					return base.transform.up;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.up;
				}
				return this.worldSpace.up;
			case CosmeticFlickReactor.AxisMode.Z:
				if (!this.useWorldAxes)
				{
					return base.transform.forward;
				}
				if (!(this.worldSpace != null))
				{
					return Vector3.forward;
				}
				return this.worldSpace.forward;
			case CosmeticFlickReactor.AxisMode.CustomForward:
				if (!(this.axisReference != null))
				{
					return Vector3.zero;
				}
				return this.axisReference.forward;
			default:
				return Vector3.zero;
			}
		}

		// Token: 0x06006F94 RID: 28564 RVA: 0x002470B0 File Offset: 0x002452B0
		private float GetSignedSpeedAlong(Vector3 axis)
		{
			Vector3 lhs;
			if (this.speedTracker != null)
			{
				lhs = this.speedTracker.GetWorldVelocity();
			}
			else if (this.rb != null)
			{
				lhs = this.rb.linearVelocity;
			}
			else
			{
				if (!this.hasLastPosition)
				{
					this.lastPosition = base.transform.position;
					this.hasLastPosition = true;
					return 0f;
				}
				Vector3 a = base.transform.position - this.lastPosition;
				float d = (Time.deltaTime > Mathf.Epsilon) ? (1f / Time.deltaTime) : 0f;
				lhs = a * d;
				this.lastPosition = base.transform.position;
			}
			return Vector3.Dot(lhs, axis.normalized);
		}

		// Token: 0x06006F95 RID: 28565 RVA: 0x00247178 File Offset: 0x00245378
		private void FireEvents(float currentAbsSpeed)
		{
			if (this.isLocal)
			{
				UnityEvent onFlickLocal = this.OnFlickLocal;
				if (onFlickLocal != null)
				{
					onFlickLocal.Invoke();
				}
			}
			UnityEvent onFlickShared = this.OnFlickShared;
			if (onFlickShared != null)
			{
				onFlickShared.Invoke();
			}
			if (this.maxSpeedThreshold > 0f)
			{
				float value = Mathf.InverseLerp(this.minSpeedThreshold, this.maxSpeedThreshold, currentAbsSpeed);
				UnityEvent<float> unityEvent = this.onFlickStrength;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(Mathf.Clamp01(value));
			}
		}

		// Token: 0x06006F96 RID: 28566 RVA: 0x002471E5 File Offset: 0x002453E5
		private void ResetState()
		{
			this.lastPeakSign = 0;
			this.lastPeakSpeed = 0f;
			this.lastPeakTime = -9999f;
		}

		// Token: 0x04007F76 RID: 32630
		[Header("Axis")]
		[Tooltip("Which single axis/direction to use for flick detection.\n- X/Y/Z use the axes defined by the Space settings below (Local vs World).\n- CustomForward uses axisReference.forward (ignores Space).")]
		[SerializeField]
		private CosmeticFlickReactor.AxisMode axisMode = CosmeticFlickReactor.AxisMode.Z;

		// Token: 0x04007F77 RID: 32631
		[Tooltip("Used only when AxisMode = CustomForward. The forward/back of this transform defines the direction.")]
		[SerializeField]
		private Transform axisReference;

		// Token: 0x04007F78 RID: 32632
		[Header("Space")]
		[Tooltip("If enabled, X/Y/Z use world axes, otherwise local axes.\nUse Local for movement relative to the object’s facing.\nUse World for absolute directions independent of rotation.")]
		[SerializeField]
		private bool useWorldAxes;

		// Token: 0x04007F79 RID: 32633
		[Tooltip("Optional transform to define a custom world frame for X/Y/Z.\nIf assigned and Space is World, this transform’s Right/Up/Forward act as the world axes.\nIf not assigned, Unity’s global axes are used.")]
		[SerializeField]
		private Transform worldSpace;

		// Token: 0x04007F7A RID: 32634
		[Header("Velocity Source")]
		[Tooltip("Primary velocity tracker.")]
		[SerializeField]
		private SimpleSpeedTracker speedTracker;

		// Token: 0x04007F7B RID: 32635
		[Tooltip("Fallback velocity source if speedTracker is missing.")]
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04007F7C RID: 32636
		[Header("Thresholds")]
		[Tooltip("Minimum absolute signed speed along the chosen axis required to consider a object movement (m/s).")]
		[SerializeField]
		private float minSpeedThreshold = 2f;

		// Token: 0x04007F7D RID: 32637
		[Tooltip("Optional upper bound for mapping flick strength to 0–1.\nSet <= 0 to disable onFlickStrength.")]
		[SerializeField]
		private float maxSpeedThreshold;

		// Token: 0x04007F7E RID: 32638
		[Tooltip("How much back-and-forth reversal is required to register a flick.\nExample: 2.5 means => +1.3 then -1.2 within the window (|1.3| + |1.2| = 2.5).")]
		[SerializeField]
		private float directionChangeRequired = 2f;

		// Token: 0x04007F7F RID: 32639
		[Header("Timing")]
		[Tooltip("Max time allowed between the initial peak and its reversal (seconds).")]
		[SerializeField]
		private float flickWindowSeconds = 0.2f;

		// Token: 0x04007F80 RID: 32640
		[Tooltip("Buffer time after a successful flick during which no new flicks are allowed (seconds).")]
		[SerializeField]
		private float retriggerBufferSeconds = 0.15f;

		// Token: 0x04007F81 RID: 32641
		[Header("Events")]
		public UnityEvent OnFlickShared;

		// Token: 0x04007F82 RID: 32642
		public UnityEvent OnFlickLocal;

		// Token: 0x04007F83 RID: 32643
		public UnityEvent<float> onFlickStrength;

		// Token: 0x04007F84 RID: 32644
		private Vector3 lastPosition;

		// Token: 0x04007F85 RID: 32645
		private bool hasLastPosition;

		// Token: 0x04007F86 RID: 32646
		private float lastPeakSpeed;

		// Token: 0x04007F87 RID: 32647
		private float lastPeakTime = -999f;

		// Token: 0x04007F88 RID: 32648
		private int lastPeakSign;

		// Token: 0x04007F89 RID: 32649
		private float blockUntilTime;

		// Token: 0x04007F8A RID: 32650
		private VRRig rig;

		// Token: 0x04007F8B RID: 32651
		private bool isLocal;

		// Token: 0x02001129 RID: 4393
		private enum AxisMode
		{
			// Token: 0x04007F8D RID: 32653
			X,
			// Token: 0x04007F8E RID: 32654
			Y,
			// Token: 0x04007F8F RID: 32655
			Z,
			// Token: 0x04007F90 RID: 32656
			CustomForward
		}
	}
}
