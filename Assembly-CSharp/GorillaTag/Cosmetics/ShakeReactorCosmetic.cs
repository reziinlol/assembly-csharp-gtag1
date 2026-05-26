using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012A4 RID: 4772
	public class ShakeReactorCosmetic : MonoBehaviour, ISpawnable
	{
		// Token: 0x0600776E RID: 30574 RVA: 0x002729EC File Offset: 0x00270BEC
		private void OnEnable()
		{
			this.lastReversalTime = Time.time;
			this.pathSinceLastReversal = 0f;
			this.recentHalfCycleDurations.Clear();
			this.hasLastDir = false;
			this.lastPosition = ((this.speedTracker != null) ? this.speedTracker.transform.position : base.transform.position);
			this.isShaking = false;
			this.debugCurrentHalfCycleDistance = 0f;
			this.debugCurrentRateHz = 0f;
			this.lastAmplitudeMeters = 0f;
			this.nextAllowedShakeStartTime = Time.time;
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = (this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (!this.subscribed && this._events.Activate != null)
			{
				this._events.Activate.reliable = true;
				this._events.Activate += this.OnShake;
				this.subscribed = true;
			}
		}

		// Token: 0x0600776F RID: 30575 RVA: 0x00272B58 File Offset: 0x00270D58
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShake;
				this.subscribed = false;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007770 RID: 30576 RVA: 0x00272BB0 File Offset: 0x00270DB0
		private void Update()
		{
			if (this.myRig != null && !this.myRig.isLocal)
			{
				return;
			}
			if (this.speedTracker == null)
			{
				if (this.isShaking)
				{
					this.isShaking = false;
					if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[]
						{
							this.isShaking
						});
					}
					UnityEvent shakeEndShared = this.ShakeEndShared;
					if (shakeEndShared != null)
					{
						shakeEndShared.Invoke();
					}
					UnityEvent shakeEndLocal = this.ShakeEndLocal;
					if (shakeEndLocal != null)
					{
						shakeEndLocal.Invoke();
					}
					this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0f, this.startCooldownSeconds);
				}
				return;
			}
			Vector3 position = this.speedTracker.transform.position;
			float magnitude = (position - this.lastPosition).magnitude;
			if (magnitude > 0f)
			{
				this.pathSinceLastReversal += magnitude;
				this.debugCurrentHalfCycleDistance = this.pathSinceLastReversal;
			}
			Vector3 worldVelocity = this.speedTracker.GetWorldVelocity();
			float magnitude2 = worldVelocity.magnitude;
			Vector3 to = (worldVelocity.sqrMagnitude > 1E-06f) ? worldVelocity.normalized : this.lastVelocityDir;
			bool flag = false;
			if (this.hasLastDir)
			{
				if (Vector3.Angle(this.lastVelocityDir, to) >= this.angleToleranceDeg && magnitude2 >= this.minSpeedForReversal)
				{
					float num = Time.time - this.lastReversalTime;
					if (num > 0.0005f)
					{
						this.EnqueueHalfCycle(num);
						this.lastAmplitudeMeters = this.pathSinceLastReversal;
						this.lastReversalTime = Time.time;
						this.pathSinceLastReversal = 0f;
						flag = true;
					}
				}
			}
			else
			{
				this.hasLastDir = true;
				this.lastVelocityDir = to;
				this.lastReversalTime = Time.time;
			}
			this.lastVelocityDir = to;
			this.lastPosition = position;
			float averageHalfCycleDuration = this.GetAverageHalfCycleDuration();
			float b = Time.time - this.lastReversalTime;
			float num2 = Mathf.Max((averageHalfCycleDuration > 1E-05f) ? averageHalfCycleDuration : float.PositiveInfinity, b);
			float num3 = (num2 < float.PositiveInfinity) ? (0.5f / num2) : 0f;
			this.debugCurrentRateHz = num3;
			bool flag2 = num3 >= this.shakeRateThreshold;
			bool flag3 = this.lastAmplitudeMeters >= this.shakeAmplitudeThreshold;
			if (!this.isShaking)
			{
				if (Time.time >= this.nextAllowedShakeStartTime && flag2 && flag3)
				{
					this.isShaking = true;
					if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[]
						{
							this.isShaking
						});
					}
					UnityEvent shakeStartLocal = this.ShakeStartLocal;
					if (shakeStartLocal != null)
					{
						shakeStartLocal.Invoke();
					}
					UnityEvent shakeStartShared = this.ShakeStartShared;
					if (shakeStartShared != null)
					{
						shakeStartShared.Invoke();
					}
				}
			}
			else
			{
				float num4 = (this.shakeRateThreshold > 1E-05f) ? (0.5f / this.shakeRateThreshold) : float.PositiveInfinity;
				float num5 = 1f * num4;
				bool flag4 = Time.time - this.lastReversalTime > num5;
				if ((!flag2 && !flag) || flag4)
				{
					this.isShaking = false;
					if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
					{
						this._events.Activate.RaiseOthers(new object[]
						{
							this.isShaking
						});
					}
					UnityEvent shakeEndLocal2 = this.ShakeEndLocal;
					if (shakeEndLocal2 != null)
					{
						shakeEndLocal2.Invoke();
					}
					UnityEvent shakeEndShared2 = this.ShakeEndShared;
					if (shakeEndShared2 != null)
					{
						shakeEndShared2.Invoke();
					}
					this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0f, this.startCooldownSeconds);
				}
			}
			if (this.useMaxes && this.isShaking)
			{
				bool flag5 = num3 >= this.maxShakeRate;
				bool flag6 = this.lastAmplitudeMeters >= this.maxShakeAmplitude;
				if (flag5 || flag6)
				{
					UnityEvent maxShake = this.MaxShake;
					if (maxShake != null)
					{
						maxShake.Invoke();
					}
				}
			}
			float strength = 0f;
			if (this.isShaking)
			{
				float num6 = Mathf.Max(1E-05f, this.shakeAmplitudeThreshold);
				if (this.useMaxes && this.maxShakeAmplitude > num6)
				{
					strength = Mathf.InverseLerp(num6, this.maxShakeAmplitude, this.lastAmplitudeMeters);
				}
				else
				{
					float b2 = Mathf.Max(num6, this.shakeAmplitudeThreshold * Mathf.Max(1f, this.softMaxMultiplier));
					strength = Mathf.InverseLerp(num6, b2, this.lastAmplitudeMeters);
				}
			}
			this.ApplyStrength(strength);
		}

		// Token: 0x06007771 RID: 30577 RVA: 0x00273060 File Offset: 0x00271260
		private void EnqueueHalfCycle(float duration)
		{
			this.recentHalfCycleDurations.Enqueue(duration);
			while (this.recentHalfCycleDurations.Count > Mathf.Max(1, 1))
			{
				this.recentHalfCycleDurations.Dequeue();
			}
		}

		// Token: 0x06007772 RID: 30578 RVA: 0x00273090 File Offset: 0x00271290
		private float GetAverageHalfCycleDuration()
		{
			if (this.recentHalfCycleDurations.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in this.recentHalfCycleDurations)
			{
				num += num2;
			}
			return num / (float)this.recentHalfCycleDurations.Count;
		}

		// Token: 0x06007773 RID: 30579 RVA: 0x00273108 File Offset: 0x00271308
		private void ApplyStrength(float strength01)
		{
			if (this.continuousProperties != null)
			{
				this.continuousProperties.ApplyAll(strength01);
			}
		}

		// Token: 0x06007774 RID: 30580 RVA: 0x00273120 File Offset: 0x00271320
		private void OnShake(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnShake");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args.Length != 1)
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			if (flag)
			{
				UnityEvent shakeStartShared = this.ShakeStartShared;
				if (shakeStartShared == null)
				{
					return;
				}
				shakeStartShared.Invoke();
				return;
			}
			else
			{
				UnityEvent shakeEndShared = this.ShakeEndShared;
				if (shakeEndShared == null)
				{
					return;
				}
				shakeEndShared.Invoke();
				return;
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x06007775 RID: 30581 RVA: 0x002731AB File Offset: 0x002713AB
		// (set) Token: 0x06007776 RID: 30582 RVA: 0x002731B3 File Offset: 0x002713B3
		public bool IsSpawned { get; set; }

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x06007777 RID: 30583 RVA: 0x002731BC File Offset: 0x002713BC
		// (set) Token: 0x06007778 RID: 30584 RVA: 0x002731C4 File Offset: 0x002713C4
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007779 RID: 30585 RVA: 0x002731CD File Offset: 0x002713CD
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x0600777A RID: 30586 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x040089D8 RID: 35288
		[Header("Speed Source")]
		[Tooltip("Speed component provider")]
		[SerializeField]
		private SimpleSpeedTracker speedTracker;

		// Token: 0x040089D9 RID: 35289
		[Header("Settings")]
		[Tooltip("Minimum reversals-per-second required to consider motion a shake - Hz.")]
		[SerializeField]
		private float shakeRateThreshold = 1f;

		// Token: 0x040089DA RID: 35290
		[Tooltip("Minimum distance traveled between direction reversals to count as a valid half-cycle.")]
		[SerializeField]
		private float shakeAmplitudeThreshold = 0.1f;

		// Token: 0x040089DB RID: 35291
		[Tooltip("Minimum angle change (degrees) between consecutive lobes to register a reversal. Higher = stricter.")]
		[SerializeField]
		[Range(10f, 170f)]
		private float angleToleranceDeg = 120f;

		// Token: 0x040089DC RID: 35292
		[Tooltip("Minimum speed required to accept a direction reversal, ignores tiny jitter near stop.")]
		[SerializeField]
		private float minSpeedForReversal = 0.2f;

		// Token: 0x040089DD RID: 35293
		[Tooltip("After a shake ends, how long to wait before ShakeStartLocal can fire again")]
		[SerializeField]
		private float startCooldownSeconds = 0.2f;

		// Token: 0x040089DE RID: 35294
		[SerializeField]
		private bool useMaxes;

		// Token: 0x040089DF RID: 35295
		[Tooltip("If enabled, exceeding this rate is considered a max shake.")]
		[SerializeField]
		private float maxShakeRate = 6f;

		// Token: 0x040089E0 RID: 35296
		[Tooltip("If enabled, exceeding this amplitude per half cycle is considered a max shake.")]
		[SerializeField]
		private float maxShakeAmplitude = 0.3f;

		// Token: 0x040089E1 RID: 35297
		[Header("Continuous Output")]
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x040089E2 RID: 35298
		[Header("Advanced")]
		[Tooltip("When no hard max amplitude is defined, strength is mapped to Threshold × this multiplier.")]
		[SerializeField]
		private float softMaxMultiplier = 3f;

		// Token: 0x040089E3 RID: 35299
		[FormerlySerializedAs("ShakeStart")]
		[Header("Events")]
		public UnityEvent ShakeStartLocal;

		// Token: 0x040089E4 RID: 35300
		public UnityEvent ShakeStartShared;

		// Token: 0x040089E5 RID: 35301
		[FormerlySerializedAs("ShakeEnd")]
		public UnityEvent ShakeEndLocal;

		// Token: 0x040089E6 RID: 35302
		public UnityEvent ShakeEndShared;

		// Token: 0x040089E7 RID: 35303
		public UnityEvent MaxShake;

		// Token: 0x040089E8 RID: 35304
		[Header("Debug")]
		public bool isShaking;

		// Token: 0x040089E9 RID: 35305
		public float lastAmplitudeMeters;

		// Token: 0x040089EA RID: 35306
		public float debugCurrentHalfCycleDistance;

		// Token: 0x040089EB RID: 35307
		public float debugCurrentRateHz;

		// Token: 0x040089EC RID: 35308
		private const int kFrequencyHistoryCount = 1;

		// Token: 0x040089ED RID: 35309
		private const float kNoReversalGraceMultiplier = 1f;

		// Token: 0x040089EE RID: 35310
		private readonly Queue<float> recentHalfCycleDurations = new Queue<float>();

		// Token: 0x040089EF RID: 35311
		private Vector3 lastVelocityDir;

		// Token: 0x040089F0 RID: 35312
		private bool hasLastDir;

		// Token: 0x040089F1 RID: 35313
		private float lastReversalTime;

		// Token: 0x040089F2 RID: 35314
		private Vector3 lastPosition;

		// Token: 0x040089F3 RID: 35315
		private float pathSinceLastReversal;

		// Token: 0x040089F4 RID: 35316
		private float nextAllowedShakeStartTime;

		// Token: 0x040089F5 RID: 35317
		private const float kEpsilon = 1E-05f;

		// Token: 0x040089F6 RID: 35318
		private const float kTinyVelocitySqr = 1E-06f;

		// Token: 0x040089F7 RID: 35319
		private const float kMinHalfCycleDuration = 0.0005f;

		// Token: 0x040089F8 RID: 35320
		private const float kHalfPerCycle = 0.5f;

		// Token: 0x040089F9 RID: 35321
		private RubberDuckEvents _events;

		// Token: 0x040089FA RID: 35322
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x040089FB RID: 35323
		private VRRig myRig;

		// Token: 0x040089FC RID: 35324
		private bool subscribed;
	}
}
