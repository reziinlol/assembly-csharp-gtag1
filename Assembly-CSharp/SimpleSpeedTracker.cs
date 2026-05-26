using System;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002F2 RID: 754
public class SimpleSpeedTracker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170001E5 RID: 485
	// (get) Token: 0x06001337 RID: 4919 RVA: 0x000658A8 File Offset: 0x00063AA8
	private bool HasAxisFilter
	{
		get
		{
			return this.trackAxis > SimpleSpeedTracker.AxisFilter.None;
		}
	}

	// Token: 0x06001338 RID: 4920 RVA: 0x000658B4 File Offset: 0x00063AB4
	public void OnEnable()
	{
		if (this.target == null)
		{
			this.target = base.transform;
		}
		this.lastPos = this.target.position;
		this.lastSliceTime = Time.time;
		this.lastVelocity = Vector3.zero;
		this.lastRawSpeed = 0f;
		this.lastSpeed = 0f;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001339 RID: 4921 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600133A RID: 4922 RVA: 0x00065920 File Offset: 0x00063B20
	public void SliceUpdate()
	{
		float num = Mathf.Max(1E-06f, Time.time - this.lastSliceTime);
		Vector3 position = this.target.position;
		Vector3 lhs = (position - this.lastPos) / num;
		float num2;
		switch (this.trackAxis)
		{
		case SimpleSpeedTracker.AxisFilter.X:
			num2 = Vector3.Dot(lhs, this.ResolveAxisRight());
			break;
		case SimpleSpeedTracker.AxisFilter.Y:
			num2 = Vector3.Dot(lhs, this.ResolveAxisUp());
			break;
		case SimpleSpeedTracker.AxisFilter.Z:
			num2 = Vector3.Dot(lhs, this.ResolveAxisForward());
			break;
		default:
			num2 = lhs.magnitude;
			break;
		}
		this.lastSpeed = (this.useRawSpeed ? num2 : Mathf.Lerp(this.lastSpeed, num2, 1f - Mathf.Exp(-this.responsiveness * num)));
		float time = Mathf.Abs(this.lastSpeed);
		float f = this.postprocessCurve.Evaluate(time);
		this.continuousProperties.ApplyAll(f);
		float num3 = this.useRawSpeed ? num2 : this.lastSpeed;
		UnityEvent<float> unityEvent = this.onSpeedUpdated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num3);
		}
		this.debugCurrentSpeed = num3;
		bool flag = Mathf.Abs(num3) >= this.eventThreshold;
		if (flag && !this.wasAboveThreshold)
		{
			UnityEvent unityEvent2 = this.onSpeedAboveThreshold;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
		}
		else if (!flag && this.wasAboveThreshold)
		{
			UnityEvent unityEvent3 = this.onSpeedBelowThreshold;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke();
			}
		}
		this.wasAboveThreshold = flag;
		if (this.HasAxisFilter)
		{
			bool flag2 = num3 >= this.positiveThreshold;
			if (flag2 && !this.wasMovingPositive)
			{
				UnityEvent unityEvent4 = this.onAbovePositiveThreshold;
				if (unityEvent4 != null)
				{
					unityEvent4.Invoke();
				}
			}
			else if (!flag2 && this.wasMovingPositive)
			{
				UnityEvent unityEvent5 = this.onBelowPositiveThreshold;
				if (unityEvent5 != null)
				{
					unityEvent5.Invoke();
				}
			}
			this.wasMovingPositive = flag2;
			bool flag3 = num3 <= this.negativeThreshold;
			if (flag3 && !this.wasMovingNegative)
			{
				UnityEvent unityEvent6 = this.onAboveNegativeThreshold;
				if (unityEvent6 != null)
				{
					unityEvent6.Invoke();
				}
			}
			else if (!flag3 && this.wasMovingNegative)
			{
				UnityEvent unityEvent7 = this.onBelowNegativeThreshold;
				if (unityEvent7 != null)
				{
					unityEvent7.Invoke();
				}
			}
			this.wasMovingNegative = flag3;
		}
		this.lastVelocity = lhs;
		this.lastRawSpeed = num2;
		this.lastPos = position;
		this.lastSliceTime = Time.time;
	}

	// Token: 0x0600133B RID: 4923 RVA: 0x00065B68 File Offset: 0x00063D68
	public float GetPostProcessSpeed()
	{
		return this.postprocessCurve.Evaluate(Mathf.Abs(this.lastSpeed));
	}

	// Token: 0x0600133C RID: 4924 RVA: 0x00065B80 File Offset: 0x00063D80
	public float GetRawSpeed()
	{
		return this.lastRawSpeed;
	}

	// Token: 0x0600133D RID: 4925 RVA: 0x00065B88 File Offset: 0x00063D88
	public Vector3 GetWorldVelocity()
	{
		return this.lastVelocity;
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x00065B90 File Offset: 0x00063D90
	public Vector3 GetLocalVelocity()
	{
		if (this.useWorldAxes)
		{
			return this.lastVelocity;
		}
		if (this.target != null)
		{
			return this.target.InverseTransformDirection(this.lastVelocity);
		}
		return base.transform.InverseTransformDirection(this.lastVelocity);
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x00065BDD File Offset: 0x00063DDD
	public float GetSignedSpeedAlongForward(Transform reference)
	{
		if (reference == null)
		{
			return 0f;
		}
		return Vector3.Dot(this.lastVelocity, reference.forward);
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x00065BFF File Offset: 0x00063DFF
	public float GetSignedSpeedX()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisRight());
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x00065C12 File Offset: 0x00063E12
	public float GetSignedSpeedY()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisUp());
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x00065C25 File Offset: 0x00063E25
	public float GetSignedSpeedZ()
	{
		return Vector3.Dot(this.lastVelocity, this.ResolveAxisForward());
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x00065C38 File Offset: 0x00063E38
	public Vector3 GetVelocityInAxisSpace()
	{
		Vector3 rhs = this.ResolveAxisRight();
		Vector3 rhs2 = this.ResolveAxisUp();
		Vector3 rhs3 = this.ResolveAxisForward();
		return new Vector3(Vector3.Dot(this.lastVelocity, rhs), Vector3.Dot(this.lastVelocity, rhs2), Vector3.Dot(this.lastVelocity, rhs3));
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x00065C84 File Offset: 0x00063E84
	private Vector3 ResolveAxisRight()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.right;
			}
			return Vector3.right;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.right;
			}
			return this.target.right;
		}
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x00065CE0 File Offset: 0x00063EE0
	private Vector3 ResolveAxisUp()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.up;
			}
			return Vector3.up;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.up;
			}
			return this.target.up;
		}
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x00065D3C File Offset: 0x00063F3C
	private Vector3 ResolveAxisForward()
	{
		if (this.useWorldAxes)
		{
			if (this.worldSpace != null)
			{
				return this.worldSpace.forward;
			}
			return Vector3.forward;
		}
		else
		{
			if (!(this.target != null))
			{
				return base.transform.forward;
			}
			return this.target.forward;
		}
	}

	// Token: 0x04001774 RID: 6004
	[Header("Settings")]
	[Tooltip("Transform whose movement speed is tracked. If left empty, uses this object’s transform.")]
	[SerializeField]
	private Transform target;

	// Token: 0x04001775 RID: 6005
	[Tooltip("If enabled, speed and direction calculations use world (global) space, otherwise local space.\nUse Local Space when you want speed relative to the object’s facing direction (e.g., how fast a sword swings forward)")]
	[SerializeField]
	private bool useWorldAxes;

	// Token: 0x04001776 RID: 6006
	[Tooltip("Optional transform defining a custom world reference.\nIf set, that transform’s Right/Up/Forward axes are treated as world axes.\nIf left empty, Unity’s global world axes are used.")]
	[SerializeField]
	private Transform worldSpace;

	// Token: 0x04001777 RID: 6007
	[Tooltip("If true, uses raw instantaneous speed without smoothing.\nIf false, smooths speed using the Responsiveness setting below.")]
	[SerializeField]
	private bool useRawSpeed;

	// Token: 0x04001778 RID: 6008
	[SerializeField]
	private float responsiveness = 10f;

	// Token: 0x04001779 RID: 6009
	[SerializeField]
	private AnimationCurve postprocessCurve = AnimationCurve.Linear(0f, 0f, 10f, 10f);

	// Token: 0x0400177A RID: 6010
	[Header("Axis Filter")]
	[Tooltip("Optionally restrict speed tracking to a single axis.\nWhen set, speed is signed: positive = moving along the axis, negative = moving against it.\nAxes are resolved using the Space settings above (Local vs World).")]
	[SerializeField]
	private SimpleSpeedTracker.AxisFilter trackAxis;

	// Token: 0x0400177B RID: 6011
	[Header("Property Output")]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x0400177C RID: 6012
	[Header("Events")]
	[Tooltip("Speed threshold used to trigger the Above/Below events.\nWhen an axis filter is set, this compares against absolute speed on that axis.")]
	[SerializeField]
	private float eventThreshold = 1f;

	// Token: 0x0400177D RID: 6013
	public UnityEvent<float> onSpeedUpdated;

	// Token: 0x0400177E RID: 6014
	public UnityEvent onSpeedAboveThreshold;

	// Token: 0x0400177F RID: 6015
	public UnityEvent onSpeedBelowThreshold;

	// Token: 0x04001780 RID: 6016
	[Tooltip("Signed speed along the positive axis direction required to fire onAbovePositiveThreshold / onBelowPositiveThreshold.")]
	[SerializeField]
	private float positiveThreshold = 2f;

	// Token: 0x04001781 RID: 6017
	public UnityEvent onAbovePositiveThreshold;

	// Token: 0x04001782 RID: 6018
	public UnityEvent onBelowPositiveThreshold;

	// Token: 0x04001783 RID: 6019
	[Tooltip("Signed speed threshold for the negative axis direction. Enter as a negative number.\nFires onAboveNegativeThreshold / onBelowNegativeThreshold when signed speed crosses this value.")]
	[SerializeField]
	private float negativeThreshold = -2f;

	// Token: 0x04001784 RID: 6020
	public UnityEvent onAboveNegativeThreshold;

	// Token: 0x04001785 RID: 6021
	public UnityEvent onBelowNegativeThreshold;

	// Token: 0x04001786 RID: 6022
	[Header("Debug")]
	[Tooltip("Current displayed speed value (raw or smoothed). Signed when Axis Filter is set.")]
	public float debugCurrentSpeed;

	// Token: 0x04001787 RID: 6023
	private float lastSpeed;

	// Token: 0x04001788 RID: 6024
	private float lastRawSpeed;

	// Token: 0x04001789 RID: 6025
	private Vector3 lastVelocity;

	// Token: 0x0400178A RID: 6026
	private Vector3 lastPos;

	// Token: 0x0400178B RID: 6027
	private float lastSliceTime;

	// Token: 0x0400178C RID: 6028
	private bool wasAboveThreshold;

	// Token: 0x0400178D RID: 6029
	private bool wasMovingPositive;

	// Token: 0x0400178E RID: 6030
	private bool wasMovingNegative;

	// Token: 0x020002F3 RID: 755
	private enum AxisFilter
	{
		// Token: 0x04001790 RID: 6032
		None,
		// Token: 0x04001791 RID: 6033
		X,
		// Token: 0x04001792 RID: 6034
		Y,
		// Token: 0x04001793 RID: 6035
		Z
	}
}
