using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002EE RID: 750
public class ProximityEffect : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06001314 RID: 4884 RVA: 0x000650C9 File Offset: 0x000632C9
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.enableVisualization = false;
		if (this.visualizer)
		{
			Object.Destroy(this.visualizer);
		}
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x000650F6 File Offset: 0x000632F6
	public void AddReceiver(IProximityEffectReceiver receiver)
	{
		if (this.receivers == null)
		{
			this.receivers = new List<IProximityEffectReceiver>
			{
				receiver
			};
			return;
		}
		if (!this.receivers.Contains(receiver))
		{
			this.receivers.Add(receiver);
		}
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x0006512D File Offset: 0x0006332D
	public void RemoveReceiver(IProximityEffectReceiver receiver)
	{
		this.receivers.Remove(receiver);
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x0006513C File Offset: 0x0006333C
	private void StartCalculating()
	{
		this.centerTransform.position = (this.leftTransform.position + this.rightTransform.position) / 2f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00065174 File Offset: 0x00063374
	private void StopCalculating()
	{
		ProximityEffect.ProximityEvent[] array = this.events;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ResetAllEvents();
		}
		ContinuousPropertyArray continuousPropertyArray = this.continuousProperties;
		if (continuousPropertyArray != null)
		{
			continuousPropertyArray.ApplyAll(0f);
		}
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(0f);
		}
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x000651D0 File Offset: 0x000633D0
	private void OnEnable()
	{
		if (this.triggersToActivate == 0)
		{
			this.StartCalculating();
		}
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x000651E0 File Offset: 0x000633E0
	private void OnDisable()
	{
		if (this.triggersToActivate == 0)
		{
			this.StopCalculating();
		}
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000651F0 File Offset: 0x000633F0
	public void AddTrigger()
	{
		if (this.numTriggers < this.triggersToActivate)
		{
			this.numTriggers++;
			if (this.numTriggers == this.triggersToActivate)
			{
				this.StartCalculating();
			}
		}
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x00065222 File Offset: 0x00063422
	public void RemoveTrigger()
	{
		if (this.numTriggers > 0)
		{
			if (this.numTriggers == this.triggersToActivate)
			{
				this.StopCalculating();
			}
			this.numTriggers--;
		}
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00065250 File Offset: 0x00063450
	private void CalculateProximityScores()
	{
		float num;
		float num2;
		float num3;
		Vector3 vector;
		this.CalculateProximityScores(true, out num, out num2, out num3, out vector);
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x0006526C File Offset: 0x0006346C
	private void CalculateProximityScores(out float distance, out float alignment, out float parallel, out Vector3 midpoint)
	{
		this.CalculateProximityScores(false, out distance, out alignment, out parallel, out midpoint);
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x0006527C File Offset: 0x0006347C
	private void CalculateProximityScores(bool drawGizmos, out float distance, out float alignment, out float parallel, out Vector3 midpoint)
	{
		float d = (this.rig != null) ? this.rig.scaleFactor : 1f;
		Vector3 position = this.leftTransform.position;
		Vector3 position2 = this.rightTransform.position;
		Vector3 forward = this.leftTransform.forward;
		Vector3 forward2 = this.rightTransform.forward;
		Vector3 a = (position2 - position) / d;
		float magnitude = a.magnitude;
		Vector3 vector = a / magnitude;
		distance = this.scoreCurves.distanceModifierCurve.Evaluate(magnitude);
		alignment = this.scoreCurves.alignmentModifierCurve.Evaluate(-Vector3.Dot(forward, forward2));
		parallel = this.scoreCurves.parallelModifierCurve.Evaluate((Vector3.Dot(forward, vector) + Vector3.Dot(forward2, -vector)) / 2f);
		midpoint = position + 0.5f * a;
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x00065374 File Offset: 0x00063574
	private void MoveTransform(Transform target, float score, Vector3 midpoint)
	{
		Vector3 vector;
		Quaternion a;
		target.GetPositionAndRotation(out vector, out a);
		Vector3 vector2 = Vector3.Lerp(vector, midpoint, ProximityEffect.<MoveTransform>g__ExpT|40_0(this.positionCTLerpSpeed));
		if (this.rotateCT)
		{
			Vector3 vector3 = (vector2 - vector) / Time.deltaTime;
			if (vector3 != Vector3.zero)
			{
				Quaternion b = Quaternion.LookRotation(vector3);
				Quaternion a2 = Quaternion.LookRotation(vector2 - this.rig.syncPos);
				Quaternion rotation = Quaternion.Slerp(a, Quaternion.Slerp(a2, b, vector3.magnitude), ProximityEffect.<MoveTransform>g__ExpT|40_0(this.rotationCTLerpSpeed));
				target.SetPositionAndRotation(vector2, rotation);
			}
		}
		else
		{
			target.position = vector2;
		}
		if (this.scaleCT)
		{
			target.localScale = Vector3.Lerp(target.localScale, score * this.scaleCTMult * Vector3.one, ProximityEffect.<MoveTransform>g__ExpT|40_0(this.scaleCTLerpSpeed));
		}
	}

	// Token: 0x170001E2 RID: 482
	// (get) Token: 0x06001321 RID: 4897 RVA: 0x00065450 File Offset: 0x00063650
	// (set) Token: 0x06001322 RID: 4898 RVA: 0x00065458 File Offset: 0x00063658
	public bool TickRunning { get; set; }

	// Token: 0x06001323 RID: 4899 RVA: 0x00065464 File Offset: 0x00063664
	public void Tick()
	{
		float num;
		float num2;
		float num3;
		Vector3 midpoint;
		this.CalculateProximityScores(out num, out num2, out num3, out midpoint);
		if (this.receivers != null)
		{
			for (int i = 0; i < this.receivers.Count; i++)
			{
				this.receivers[i].OnProximityCalculated(num, num2, num3);
			}
		}
		float num4 = num * num2 * num3;
		ContinuousPropertyArray continuousPropertyArray = this.continuousProperties;
		if (continuousPropertyArray != null)
		{
			continuousPropertyArray.ApplyAll(num4);
		}
		UnityEvent<float> unityEvent = this.onScoreCalculated;
		if (unityEvent != null)
		{
			unityEvent.Invoke(num4);
		}
		if (this.centerTransform != null)
		{
			this.MoveTransform(this.centerTransform, num4, midpoint);
		}
		this.anyAboveThreshold = false;
		foreach (ProximityEffect.ProximityEvent proximityEvent in this.events)
		{
			this.anyAboveThreshold = (proximityEvent.Evaluate(num4) || this.anyAboveThreshold);
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x000655C9 File Offset: 0x000637C9
	[CompilerGenerated]
	internal static float <MoveTransform>g__ExpT|40_0(float speed)
	{
		return 1f - Mathf.Exp(-speed * Time.deltaTime);
	}

	// Token: 0x04001743 RID: 5955
	[SerializeField]
	private Transform leftTransform;

	// Token: 0x04001744 RID: 5956
	[SerializeField]
	private Transform rightTransform;

	// Token: 0x04001745 RID: 5957
	[SerializeField]
	[Tooltip("How many times AddTrigger() needs to be called before the events are allowed to be invoked. Used for pausing events until certain actions are performed (like squeezing the triggers of both controllers).")]
	private int triggersToActivate;

	// Token: 0x04001746 RID: 5958
	[Space]
	[SerializeField]
	[Tooltip("The transform that moves to follow the midpoint of the left and right transforms.")]
	private Transform centerTransform;

	// Token: 0x04001747 RID: 5959
	private const string SHOW_CONDITION = "@centerTransform != null";

	// Token: 0x04001748 RID: 5960
	[SerializeField]
	private float positionCTLerpSpeed = 10f;

	// Token: 0x04001749 RID: 5961
	[SerializeField]
	private bool rotateCT;

	// Token: 0x0400174A RID: 5962
	private const string SHOW_ROTATE_CONDITION = "@centerTransform != null && rotateCT";

	// Token: 0x0400174B RID: 5963
	[SerializeField]
	private float rotationCTLerpSpeed = 10f;

	// Token: 0x0400174C RID: 5964
	[SerializeField]
	private bool scaleCT;

	// Token: 0x0400174D RID: 5965
	private const string SHOW_SCALE_CONDITION = "@centerTransform != null && scaleCT";

	// Token: 0x0400174E RID: 5966
	[SerializeField]
	private float scaleCTLerpSpeed = 10f;

	// Token: 0x0400174F RID: 5967
	[SerializeField]
	private float scaleCTMult = 1f;

	// Token: 0x04001750 RID: 5968
	[Space]
	[SerializeField]
	[Tooltip("The curves that get evaluated to determine the alignment score. They get multiplied together, so their Y values should all range from 0-1. The result is compared against the thresholds of the ProximityEvents.")]
	private ProximityEffectScoreCurvesSO scoreCurves;

	// Token: 0x04001751 RID: 5969
	[Space]
	[SerializeField]
	private ContinuousPropertyArray continuousProperties;

	// Token: 0x04001752 RID: 5970
	[SerializeField]
	private UnityEvent<float> onScoreCalculated;

	// Token: 0x04001753 RID: 5971
	[SerializeField]
	private ProximityEffect.ProximityEvent[] events;

	// Token: 0x04001754 RID: 5972
	[Header("Editor Only")]
	[SerializeField]
	private Vector3 defaultLeftHandLocalPosition = new Vector3(-0.0568f, 0.04311f, 0.00249f);

	// Token: 0x04001755 RID: 5973
	[SerializeField]
	private Vector3 defaultLeftHandLocalEuler = new Vector3(173.176f, 80.201f, 3.615f);

	// Token: 0x04001756 RID: 5974
	[Header("Visualization is currently NOT WORKING IN PLAY MODE due to tick optimization")]
	[SerializeField]
	private bool enableVisualization = true;

	// Token: 0x04001757 RID: 5975
	[SerializeField]
	private Material visualizationMaterial;

	// Token: 0x04001758 RID: 5976
	[SerializeField]
	[Range(0f, 1f)]
	private float visualizationLineThickness = 0.01f;

	// Token: 0x04001759 RID: 5977
	[SerializeField]
	[HideInInspector]
	private LineRenderer visualizer;

	// Token: 0x0400175A RID: 5978
	private List<IProximityEffectReceiver> receivers;

	// Token: 0x0400175B RID: 5979
	private VRRig rig;

	// Token: 0x0400175C RID: 5980
	private bool anyAboveThreshold;

	// Token: 0x0400175D RID: 5981
	private int numTriggers;

	// Token: 0x020002EF RID: 751
	[Serializable]
	private class ProximityEvent
	{
		// Token: 0x06001326 RID: 4902 RVA: 0x000655E0 File Offset: 0x000637E0
		public bool Evaluate(float score)
		{
			if (score >= this.highThreshold)
			{
				if (!this.wasAboveThreshold && Time.time - this.lastThresholdTime >= this.highThresholdBufferTime)
				{
					UnityEvent unityEvent = this.onThresholdHigh;
					if (unityEvent != null)
					{
						unityEvent.Invoke();
					}
					this.wasAboveThreshold = true;
					this.wasBelowThreshold = false;
				}
				if (this.wasAboveThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
				return true;
			}
			if (score < this.lowThreshold)
			{
				if (!this.wasBelowThreshold && Time.time - this.lastThresholdTime >= this.lowThresholdBufferTime)
				{
					UnityEvent unityEvent2 = this.onThresholdLow;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.wasAboveThreshold = false;
					this.wasBelowThreshold = true;
				}
				if (this.wasBelowThreshold)
				{
					this.lastThresholdTime = Time.time;
				}
			}
			return false;
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0006569E File Offset: 0x0006389E
		public void ResetAllEvents()
		{
			this.wasAboveThreshold = false;
			this.wasBelowThreshold = true;
		}

		// Token: 0x0400175F RID: 5983
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("High-threshold events will only fire if the alignment score is above this value.")]
		private float highThreshold = 0.5f;

		// Token: 0x04001760 RID: 5984
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the high-threshold events.")]
		private float highThresholdBufferTime;

		// Token: 0x04001761 RID: 5985
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Low-threshold events will only fire if the alignment score is below this value.")]
		private float lowThreshold = 0.3f;

		// Token: 0x04001762 RID: 5986
		[SerializeField]
		[Tooltip("Wait this many seconds before activating the low-threshold events.")]
		private float lowThresholdBufferTime;

		// Token: 0x04001763 RID: 5987
		public UnityEvent onThresholdHigh;

		// Token: 0x04001764 RID: 5988
		public UnityEvent onThresholdLow;

		// Token: 0x04001765 RID: 5989
		private bool wasAboveThreshold;

		// Token: 0x04001766 RID: 5990
		private bool wasBelowThreshold = true;

		// Token: 0x04001767 RID: 5991
		private float lastThresholdTime = -100f;
	}
}
