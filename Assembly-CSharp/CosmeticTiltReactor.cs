using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000674 RID: 1652
public class CosmeticTiltReactor : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600295B RID: 10587 RVA: 0x000DF2C8 File Offset: 0x000DD4C8
	private void Awake()
	{
		this.referenceDirection.Normalize();
		if (!this.useTransform && this.referenceDirection == Vector3.zero)
		{
			GTDev.LogError<string>("CosmeticTiltReactor " + base.gameObject.name + " referenceDirection cannot be 0 vector", null);
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			GTDev.LogError<string>("CosmeticTiltReactor " + base.gameObject.name + " referenceTransform cannot be null", null);
		}
		this.hasContinuousProperties = (this.continuousProperties != null && this.continuousProperties.Count > 0);
		this.calculateDot = this.hasContinuousProperties;
		using (List<CosmeticTiltReactor.TiltEvent>.Enumerator enumerator = this.events.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.comparisonMethod == CosmeticTiltReactor.TiltEvent.ComparisonMethod.DotProduct)
				{
					this.calculateDot = true;
				}
				else
				{
					this.calculateAngle = true;
				}
				if (this.calculateDot && this.calculateAngle)
				{
					break;
				}
			}
		}
		this._rig = base.GetComponentInParent<VRRig>();
		this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
		if (this._rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
		{
			this._rig = GorillaTagger.Instance.offlineVRRig;
		}
		if (this._rig == null && !this.syncForAllPlayers)
		{
			GTDev.LogError<string>("CosmeticTiltReactor on " + base.gameObject.name + " set to not syncForAllPlayers and has no VR Rig parent. Events will not fire", null);
		}
		else if (this._rig != null)
		{
			this.isLocallyOwned = this._rig.isLocal;
		}
		if (this.parentTransferable == null && this.onlyWhileHeld)
		{
			GTDev.LogError<string>("CosmeticTiltReactor on " + base.gameObject.name + " set to OnlyWhileHeld but has no TransferrableObject parent. Events will not fire", null);
		}
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000DF4B8 File Offset: 0x000DD6B8
	public void OnEnable()
	{
		if (!this.syncForAllPlayers && !this.isLocallyOwned)
		{
			return;
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			return;
		}
		Vector3 vector = this.useTransform ? this.referenceTransform.up : this.referenceDirection;
		if (this.calculateAngle)
		{
			this.angle = Vector3.Angle(base.transform.up, vector);
		}
		if (this.calculateDot)
		{
			this.dotProduct = Vector3.Dot(base.transform.up, vector);
		}
		this.ResetEvents();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000DF554 File Offset: 0x000DD754
	public void OnDisable()
	{
		if (!this.syncForAllPlayers && !this.isLocallyOwned)
		{
			return;
		}
		if (this.useTransform && this.referenceTransform == null)
		{
			return;
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000DF588 File Offset: 0x000DD788
	public void SliceUpdate()
	{
		if (this.onlyWhileHeld)
		{
			bool flag = this.parentTransferable != null && this.parentTransferable.InHand();
			if (!flag && this.wasInHand)
			{
				this.ResetEvents();
			}
			this.wasInHand = flag;
			if (!flag)
			{
				return;
			}
		}
		Vector3 vector = this.useTransform ? this.referenceTransform.up : this.referenceDirection;
		if (this.calculateAngle)
		{
			this.angle = Vector3.Angle(base.transform.up, vector);
		}
		if (this.calculateDot)
		{
			this.dotProduct = Vector3.Dot(base.transform.up, vector);
		}
		this.FireEvents();
		if (this.hasContinuousProperties)
		{
			this.continuousProperties.ApplyAll(this.dotProduct);
		}
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000DF650 File Offset: 0x000DD850
	private void ResetEvents()
	{
		if (this.events == null || this.events.Count <= 0)
		{
			return;
		}
		foreach (CosmeticTiltReactor.TiltEvent tiltEvent in this.events)
		{
			switch (tiltEvent.tiltEventType)
			{
			case CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold:
				tiltEvent.wasGreater = true;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold:
				tiltEvent.wasGreater = false;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThresholdForDuration:
				tiltEvent.wasGreater = true;
				tiltEvent.hasFired = false;
				break;
			case CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThresholdForDuration:
				tiltEvent.wasGreater = false;
				tiltEvent.hasFired = false;
				break;
			}
			tiltEvent.thresholdCrossTime = double.MinValue;
		}
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000DF714 File Offset: 0x000DD914
	private void FireEvents()
	{
		if (this.events == null || this.events.Count <= 0)
		{
			return;
		}
		foreach (CosmeticTiltReactor.TiltEvent tiltEvent in this.events)
		{
			bool flag = (tiltEvent.comparisonMethod == CosmeticTiltReactor.TiltEvent.ComparisonMethod.Angle) ? (this.angle > tiltEvent.angleThreshold) : (this.dotProduct > tiltEvent.dotThreshold);
			CosmeticTiltReactor.TiltEvent.TiltEventType tiltEventType = tiltEvent.tiltEventType;
			if (tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold || tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold)
			{
				if (flag != tiltEvent.wasGreater)
				{
					if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThreshold && flag)
					{
						if (tiltEvent.thresholdCrossTime + (double)tiltEvent.retriggerDelay <= Time.timeAsDouble)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
							tiltEvent.wasGreater = true;
							UnityEvent onTiltEvent = tiltEvent.OnTiltEvent;
							if (onTiltEvent != null)
							{
								onTiltEvent.Invoke();
							}
						}
					}
					else if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold && !flag)
					{
						if (tiltEvent.thresholdCrossTime + (double)tiltEvent.retriggerDelay <= Time.timeAsDouble)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
							tiltEvent.wasGreater = false;
							UnityEvent onTiltEvent2 = tiltEvent.OnTiltEvent;
							if (onTiltEvent2 != null)
							{
								onTiltEvent2.Invoke();
							}
						}
					}
					else
					{
						tiltEvent.wasGreater = flag;
					}
				}
			}
			else
			{
				if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.GreaterThanThresholdForDuration)
				{
					if (flag)
					{
						if (!tiltEvent.wasGreater)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
						}
						else if (!tiltEvent.hasFired && tiltEvent.thresholdCrossTime + (double)tiltEvent.duration <= Time.timeAsDouble)
						{
							UnityEvent onTiltEvent3 = tiltEvent.OnTiltEvent;
							if (onTiltEvent3 != null)
							{
								onTiltEvent3.Invoke();
							}
							tiltEvent.hasFired = true;
						}
					}
					else
					{
						tiltEvent.hasFired = false;
					}
				}
				if (tiltEvent.tiltEventType == CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThresholdForDuration)
				{
					if (!flag)
					{
						if (tiltEvent.wasGreater)
						{
							tiltEvent.thresholdCrossTime = Time.timeAsDouble;
						}
						else if (!tiltEvent.hasFired && tiltEvent.thresholdCrossTime + (double)tiltEvent.duration <= Time.timeAsDouble)
						{
							UnityEvent onTiltEvent4 = tiltEvent.OnTiltEvent;
							if (onTiltEvent4 != null)
							{
								onTiltEvent4.Invoke();
							}
							tiltEvent.hasFired = true;
						}
					}
					else
					{
						tiltEvent.hasFired = false;
					}
				}
				tiltEvent.wasGreater = flag;
			}
		}
	}

	// Token: 0x040035BD RID: 13757
	[SerializeField]
	private bool useTransform;

	// Token: 0x040035BE RID: 13758
	[Tooltip("Direction to which this transform's y is compared in world space")]
	[SerializeField]
	private Vector3 referenceDirection = Vector3.up;

	// Token: 0x040035BF RID: 13759
	[Tooltip("compare referenceTransform's y to this transform's y")]
	[SerializeField]
	private Transform referenceTransform;

	// Token: 0x040035C0 RID: 13760
	[SerializeField]
	private List<CosmeticTiltReactor.TiltEvent> events;

	// Token: 0x040035C1 RID: 13761
	[Tooltip("input for continuous properties is the dot product of this transform's y and the reference direction")]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x040035C2 RID: 13762
	[Tooltip("Should this script be run for all clients or just the owner")]
	[SerializeField]
	private bool syncForAllPlayers = true;

	// Token: 0x040035C3 RID: 13763
	[Tooltip("option to run only if this transferrable object is in the hand")]
	[SerializeField]
	private bool onlyWhileHeld;

	// Token: 0x040035C4 RID: 13764
	private VRRig _rig;

	// Token: 0x040035C5 RID: 13765
	private TransferrableObject parentTransferable;

	// Token: 0x040035C6 RID: 13766
	private bool isLocallyOwned;

	// Token: 0x040035C7 RID: 13767
	private bool hasContinuousProperties;

	// Token: 0x040035C8 RID: 13768
	private float angle;

	// Token: 0x040035C9 RID: 13769
	private float dotProduct;

	// Token: 0x040035CA RID: 13770
	private bool calculateAngle;

	// Token: 0x040035CB RID: 13771
	private bool calculateDot;

	// Token: 0x040035CC RID: 13772
	private bool wasInHand;

	// Token: 0x02000675 RID: 1653
	[Serializable]
	public class TiltEvent
	{
		// Token: 0x06002962 RID: 10594 RVA: 0x000DF95C File Offset: 0x000DDB5C
		public TiltEvent()
		{
			this.tiltEventType = CosmeticTiltReactor.TiltEvent.TiltEventType.LessThanThreshold;
			this.comparisonMethod = CosmeticTiltReactor.TiltEvent.ComparisonMethod.DotProduct;
			this.angleThreshold = 15f;
			this.retriggerDelay = 0f;
			this.duration = 0.5f;
		}

		// Token: 0x040035CD RID: 13773
		public CosmeticTiltReactor.TiltEvent.ComparisonMethod comparisonMethod;

		// Token: 0x040035CE RID: 13774
		public CosmeticTiltReactor.TiltEvent.TiltEventType tiltEventType;

		// Token: 0x040035CF RID: 13775
		[Range(0f, 180f)]
		[Tooltip("Angle in degrees from the reference direction")]
		public float angleThreshold;

		// Token: 0x040035D0 RID: 13776
		[Range(-1f, 1f)]
		[Tooltip("Dot product compared to the reference direction")]
		public float dotThreshold;

		// Token: 0x040035D1 RID: 13777
		[Tooltip("Minimum time between events firing")]
		public float retriggerDelay;

		// Token: 0x040035D2 RID: 13778
		[Tooltip("Amount of time the angle or dot product should be less/greater than the threshold before firing an event")]
		public float duration;

		// Token: 0x040035D3 RID: 13779
		public UnityEvent OnTiltEvent;

		// Token: 0x040035D4 RID: 13780
		[NonSerialized]
		public bool wasGreater;

		// Token: 0x040035D5 RID: 13781
		[NonSerialized]
		public bool hasFired;

		// Token: 0x040035D6 RID: 13782
		[NonSerialized]
		public double thresholdCrossTime = double.MinValue;

		// Token: 0x02000676 RID: 1654
		public enum ComparisonMethod
		{
			// Token: 0x040035D8 RID: 13784
			DotProduct,
			// Token: 0x040035D9 RID: 13785
			Angle
		}

		// Token: 0x02000677 RID: 1655
		public enum TiltEventType
		{
			// Token: 0x040035DB RID: 13787
			LessThanThreshold,
			// Token: 0x040035DC RID: 13788
			GreaterThanThreshold,
			// Token: 0x040035DD RID: 13789
			LessThanThresholdForDuration,
			// Token: 0x040035DE RID: 13790
			GreaterThanThresholdForDuration
		}
	}
}
