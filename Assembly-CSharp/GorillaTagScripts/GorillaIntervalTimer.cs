using System;
using GorillaTag.Cosmetics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000EB8 RID: 3768
	public class GorillaIntervalTimer : MonoBehaviourPun
	{
		// Token: 0x06005CB4 RID: 23732 RVA: 0x001D67AF File Offset: 0x001D49AF
		private void Awake()
		{
			if (this.networkProvider == null)
			{
				this.networkProvider = base.GetComponentInParent<NetworkedRandomProvider>();
			}
			this.ResetElapsed();
			this.ResetRun();
		}

		// Token: 0x06005CB5 RID: 23733 RVA: 0x001D67D7 File Offset: 0x001D49D7
		private void OnEnable()
		{
			if (this.runOnEnable)
			{
				if (!this.isRegistered)
				{
					GorillaIntervalTimerManager.RegisterGorillaTimer(this);
					this.isRegistered = true;
				}
				this.StartTimer();
			}
		}

		// Token: 0x06005CB6 RID: 23734 RVA: 0x001D67FC File Offset: 0x001D49FC
		private void OnDisable()
		{
			if (this.isRegistered)
			{
				GorillaIntervalTimerManager.UnregisterGorillaTimer(this);
				this.isRegistered = false;
			}
			this.StopTimer();
		}

		// Token: 0x06005CB7 RID: 23735 RVA: 0x001D681C File Offset: 0x001D4A1C
		public void StartTimer()
		{
			if (!this.isRegistered)
			{
				GorillaIntervalTimerManager.RegisterGorillaTimer(this);
				this.isRegistered = true;
			}
			this.ResetRun();
			this.elapsed = 0f;
			this.isInPostFireDelay = false;
			if (this.useInitialDelay && this.initialDelay > 0f)
			{
				this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.initialDelay));
			}
			else
			{
				this.RollNextInterval();
			}
			this.isRunning = true;
			this.isPaused = false;
			UnityEvent unityEvent = this.onTimerStarted;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06005CB8 RID: 23736 RVA: 0x001D68B0 File Offset: 0x001D4AB0
		public void StopTimer()
		{
			this.isRunning = false;
			this.isPaused = false;
			this.elapsed = 0f;
			this.isInPostFireDelay = false;
			UnityEvent unityEvent = this.onTimerStopped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			if (this.isRegistered)
			{
				GorillaIntervalTimerManager.UnregisterGorillaTimer(this);
				this.isRegistered = false;
			}
		}

		// Token: 0x06005CB9 RID: 23737 RVA: 0x001D6903 File Offset: 0x001D4B03
		public void Pause()
		{
			this.isPaused = true;
		}

		// Token: 0x06005CBA RID: 23738 RVA: 0x001D690C File Offset: 0x001D4B0C
		public void Resume()
		{
			this.isPaused = false;
		}

		// Token: 0x06005CBB RID: 23739 RVA: 0x001D6918 File Offset: 0x001D4B18
		public void SetFixedIntervalSeconds(float seconds)
		{
			this.useRandomDuration = false;
			this.fixedInterval = Mathf.Max(0f, seconds);
			this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.fixedInterval));
			this.elapsed = 0f;
		}

		// Token: 0x06005CBC RID: 23740 RVA: 0x001D6964 File Offset: 0x001D4B64
		public void OverrideNextIntervalSeconds(float seconds)
		{
			this.currentIntervalSeconds = Mathf.Max(0.001f, seconds);
			this.elapsed = 0f;
		}

		// Token: 0x06005CBD RID: 23741 RVA: 0x001D6982 File Offset: 0x001D4B82
		public void ResetRun()
		{
			this.runFiredSoFar = 0;
		}

		// Token: 0x06005CBE RID: 23742 RVA: 0x001D698C File Offset: 0x001D4B8C
		public void InvokeUpdate()
		{
			if (!this.isRunning || this.isPaused)
			{
				return;
			}
			this.elapsed += Time.deltaTime;
			if (this.elapsed >= this.currentIntervalSeconds)
			{
				if (this.isInPostFireDelay)
				{
					this.isInPostFireDelay = false;
					this.elapsed = 0f;
					this.RollNextInterval();
					return;
				}
				UnityEvent unityEvent = this.onIntervalFired;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				this.runFiredSoFar++;
				if (this.runLength == GorillaIntervalTimer.RunLength.Finite && this.runFiredSoFar >= Mathf.Max(1, this.maxFiresPerRun))
				{
					if (this.requireManualReset)
					{
						this.StopTimer();
						return;
					}
					this.runFiredSoFar = 0;
				}
				if (this.usePostIntervalDelay && this.postIntervalDelay > 0f)
				{
					this.isInPostFireDelay = true;
					this.elapsed = 0f;
					this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.postIntervalDelay));
					return;
				}
				this.elapsed = 0f;
				this.RollNextInterval();
			}
		}

		// Token: 0x06005CBF RID: 23743 RVA: 0x001D6A95 File Offset: 0x001D4C95
		private void ResetElapsed()
		{
			this.elapsed = 0f;
		}

		// Token: 0x06005CC0 RID: 23744 RVA: 0x001D6AA4 File Offset: 0x001D4CA4
		private void RollNextInterval()
		{
			if (!this.useRandomDuration)
			{
				this.currentIntervalSeconds = Mathf.Max(0.001f, this.ToSeconds(this.fixedInterval));
				return;
			}
			float num = Mathf.Max(0f, this.ToSeconds(this.randTimeMin));
			float num2 = Mathf.Max(num, this.ToSeconds(this.randTimeMax));
			float b;
			if (this.intervalSource == GorillaIntervalTimer.IntervalSource.NetworkedRandom && this.networkProvider != null)
			{
				switch (this.distribution)
				{
				default:
					b = this.networkProvider.NextFloat(num, num2);
					break;
				case GorillaIntervalTimer.RandomDistribution.Normal:
				{
					double d = Math.Max(double.Epsilon, 1.0 - this.networkProvider.NextDouble(0.0, 1.0));
					double num3 = Math.Max(double.Epsilon, 1.0 - (double)this.networkProvider.NextFloat01());
					double num4 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Sin(6.283185307179586 * num3);
					float num5 = 0.5f * (num + num2);
					float num6 = (num2 - num) / 6f;
					b = Mathf.Clamp(num5 + (float)(num4 * (double)num6), num, num2);
					break;
				}
				case GorillaIntervalTimer.RandomDistribution.Exponential:
				{
					double d2 = Math.Max(double.Epsilon, 1.0 - this.networkProvider.NextDouble(0.0, 1.0));
					double num7 = 0.5 * (double)(num + num2);
					double num8 = (num7 > 0.0) ? (1.0 / num7) : 1.0;
					b = Mathf.Clamp((float)(-(float)Math.Log(d2) / num8), num, num2);
					break;
				}
				}
				this.currentIntervalSeconds = Mathf.Max(0.001f, b);
				return;
			}
			switch (this.distribution)
			{
			default:
				b = Random.Range(num, num2);
				break;
			case GorillaIntervalTimer.RandomDistribution.Normal:
			{
				float f = Mathf.Max(float.Epsilon, 1f - Random.value);
				float num9 = 1f - Random.value;
				float num10 = Mathf.Sqrt(-2f * Mathf.Log(f)) * Mathf.Sin(6.2831855f * num9);
				float num11 = 0.5f * (num + num2);
				float num12 = (num2 - num) / 6f;
				b = Mathf.Clamp(num11 + num10 * num12, num, num2);
				break;
			}
			case GorillaIntervalTimer.RandomDistribution.Exponential:
			{
				float num13 = 0.5f * (num + num2);
				float num14 = (num13 > 0f) ? (1f / num13) : 1f;
				b = Mathf.Clamp(-Mathf.Log(Mathf.Max(float.Epsilon, 1f - Random.value)) / num14, num, num2);
				break;
			}
			}
			this.currentIntervalSeconds = Mathf.Max(0.001f, b);
		}

		// Token: 0x06005CC1 RID: 23745 RVA: 0x001D6D74 File Offset: 0x001D4F74
		private float ToSeconds(float value)
		{
			switch (this.unit)
			{
			default:
				return value;
			case GorillaIntervalTimer.TimeUnit.Minutes:
				return value * 60f;
			case GorillaIntervalTimer.TimeUnit.Hours:
				return value * 3600f;
			}
		}

		// Token: 0x06005CC2 RID: 23746 RVA: 0x001D6DAB File Offset: 0x001D4FAB
		public void RestartTimer()
		{
			this.ResetElapsed();
			this.RollNextInterval();
			this.StartTimer();
		}

		// Token: 0x06005CC3 RID: 23747 RVA: 0x001D6DBF File Offset: 0x001D4FBF
		public float GetPassedTime()
		{
			return this.elapsed;
		}

		// Token: 0x06005CC4 RID: 23748 RVA: 0x001D6DC7 File Offset: 0x001D4FC7
		public float GetRemainingTime()
		{
			return Mathf.Max(0f, this.currentIntervalSeconds - this.elapsed);
		}

		// Token: 0x04006B06 RID: 27398
		[Header("Scheduling")]
		[Tooltip("If true, the timer will automatically start when this component is enabled.")]
		[SerializeField]
		private bool runOnEnable = true;

		// Token: 0x04006B07 RID: 27399
		[Tooltip("If true, apply an initial delay before the first interval is fired.")]
		[SerializeField]
		private bool useInitialDelay;

		// Token: 0x04006B08 RID: 27400
		[Tooltip("Delay (in seconds or minutes depending on Unit) before the first fire if 'Use Initial Delay' is enabled.")]
		[SerializeField]
		private float initialDelay;

		// Token: 0x04006B09 RID: 27401
		[Header("Interval")]
		[Tooltip("Unit of time for Fixed Interval, Min and Max values.")]
		[SerializeField]
		private GorillaIntervalTimer.TimeUnit unit;

		// Token: 0x04006B0A RID: 27402
		[Tooltip("Distribution type used for generating random intervals when Interval Source = LocalRandom.")]
		[SerializeField]
		private GorillaIntervalTimer.RandomDistribution distribution;

		// Token: 0x04006B0B RID: 27403
		[Tooltip("Fixed interval duration (interpreted by Unit) when Use Random Duration = false.")]
		[SerializeField]
		private float fixedInterval = 1f;

		// Token: 0x04006B0C RID: 27404
		[Space]
		[Tooltip("If false, 'Fixed Interval' is used. If true, a random interval is sampled each cycle.")]
		[SerializeField]
		private bool useRandomDuration;

		// Token: 0x04006B0D RID: 27405
		[Tooltip("Minimum interval time (in selected Unit).")]
		[SerializeField]
		private float randTimeMin = 0.5f;

		// Token: 0x04006B0E RID: 27406
		[Tooltip("Maximum interval time (in selected Unit).")]
		[SerializeField]
		private float randTimeMax = 2f;

		// Token: 0x04006B0F RID: 27407
		[Tooltip("Determines whether to use a local random generator or a networked random source.")]
		[SerializeField]
		private GorillaIntervalTimer.IntervalSource intervalSource;

		// Token: 0x04006B10 RID: 27408
		[Header("Networked Interval (optional)")]
		[Tooltip("If Interval Source = NetworkedRandom, the timer queries this component for the next interval")]
		[SerializeField]
		private NetworkedRandomProvider networkProvider;

		// Token: 0x04006B11 RID: 27409
		[Space]
		[Tooltip("If true, wait this additional delay after onIntervalFired() before starting the next interval.")]
		[SerializeField]
		private bool usePostIntervalDelay;

		// Token: 0x04006B12 RID: 27410
		[Tooltip("Additional delay (in selected Unit) to wait after onIntervalFired(), before the next interval begins.")]
		[SerializeField]
		private float postIntervalDelay;

		// Token: 0x04006B13 RID: 27411
		[Header("Run Length")]
		[Tooltip("Infinite runs forever. Finite stops after Max Fires Per Run.")]
		[SerializeField]
		private GorillaIntervalTimer.RunLength runLength;

		// Token: 0x04006B14 RID: 27412
		[Tooltip("Number of times the timer fires before the run completes (when Run Length = Finite).")]
		[SerializeField]
		private int maxFiresPerRun = 3;

		// Token: 0x04006B15 RID: 27413
		[Tooltip("If true, the timer stops at the end of a finite run and requires ResetRun() / StartTimer() to continue. If false, the run counter auto-resets and continues.")]
		[SerializeField]
		private bool requireManualReset = true;

		// Token: 0x04006B16 RID: 27414
		[Header("Events")]
		public UnityEvent onIntervalFired;

		// Token: 0x04006B17 RID: 27415
		public UnityEvent onTimerStarted;

		// Token: 0x04006B18 RID: 27416
		public UnityEvent onTimerStopped;

		// Token: 0x04006B19 RID: 27417
		private const float minIntervalEpsilon = 0.001f;

		// Token: 0x04006B1A RID: 27418
		private float currentIntervalSeconds = 1f;

		// Token: 0x04006B1B RID: 27419
		private float elapsed;

		// Token: 0x04006B1C RID: 27420
		private bool isRunning;

		// Token: 0x04006B1D RID: 27421
		private bool isPaused;

		// Token: 0x04006B1E RID: 27422
		private bool isRegistered;

		// Token: 0x04006B1F RID: 27423
		private int runFiredSoFar;

		// Token: 0x04006B20 RID: 27424
		private bool isInPostFireDelay;

		// Token: 0x02000EB9 RID: 3769
		private enum TimeUnit
		{
			// Token: 0x04006B22 RID: 27426
			Seconds,
			// Token: 0x04006B23 RID: 27427
			Minutes,
			// Token: 0x04006B24 RID: 27428
			Hours
		}

		// Token: 0x02000EBA RID: 3770
		private enum RandomDistribution
		{
			// Token: 0x04006B26 RID: 27430
			Uniform,
			// Token: 0x04006B27 RID: 27431
			Normal,
			// Token: 0x04006B28 RID: 27432
			Exponential
		}

		// Token: 0x02000EBB RID: 3771
		private enum IntervalSource
		{
			// Token: 0x04006B2A RID: 27434
			LocalRandom,
			// Token: 0x04006B2B RID: 27435
			NetworkedRandom
		}

		// Token: 0x02000EBC RID: 3772
		private enum RunLength
		{
			// Token: 0x04006B2D RID: 27437
			Infinite,
			// Token: 0x04006B2E RID: 27438
			Finite
		}
	}
}
