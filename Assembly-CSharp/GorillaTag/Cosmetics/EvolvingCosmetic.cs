using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001278 RID: 4728
	[Obsolete]
	public class EvolvingCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06007686 RID: 30342 RVA: 0x0026E329 File Offset: 0x0026C529
		private int LoopMaxValue
		{
			get
			{
				return this.stages.Length;
			}
		}

		// Token: 0x06007687 RID: 30343 RVA: 0x0026E334 File Offset: 0x0026C534
		private void Awake()
		{
			base.gameObject.GetOrAddComponent(ref this.networkEvents);
			this.myRig = base.GetComponentInParent<VRRig>();
			for (int i = 0; i < this.stages.Length; i++)
			{
				this.totalDuration += this.stages[i].Duration;
				if (this.enableLooping)
				{
					if (i < this.loopToStageOnComplete - 1)
					{
						this.timeAtLoopStart += this.stages[i].Duration;
					}
					else
					{
						this.loopDuration += this.stages[i].Duration;
					}
				}
			}
		}

		// Token: 0x06007688 RID: 30344 RVA: 0x0026E3D8 File Offset: 0x0026C5D8
		private void OnEnable()
		{
			if (this.stages.Length == 0)
			{
				return;
			}
			NetPlayer netPlayer = this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer;
			if (netPlayer != null)
			{
				this.networkEvents.Init(netPlayer);
				TickSystem<object>.AddTickCallback(this);
				NetworkSystem.Instance.OnPlayerJoined += this.SendElapsedTime;
				this.networkEvents.Activate += this.ReceiveElapsedTime;
				this.FirstStage();
				return;
			}
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}

		// Token: 0x06007689 RID: 30345 RVA: 0x0026E474 File Offset: 0x0026C674
		private void OnDisable()
		{
			if (this.networkEvents != null)
			{
				TickSystem<object>.RemoveTickCallback(this);
				NetworkSystem.Instance.OnPlayerJoined -= this.SendElapsedTime;
				this.networkEvents.Activate -= this.ReceiveElapsedTime;
				this.FirstStage();
			}
			CallLimiter callLimiter = this.callLimiter;
			if (callLimiter == null)
			{
				return;
			}
			callLimiter.Reset();
		}

		// Token: 0x0600768A RID: 30346 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void Log(bool isComplete, bool isEvent)
		{
		}

		// Token: 0x0600768B RID: 30347 RVA: 0x0026E4F0 File Offset: 0x0026C6F0
		private void FirstStage()
		{
			this.activeStageIndex = 0;
			this.activeStage = this.stages[0];
			this.nextEventIndex = 0;
			this.nextEvent = this.activeStage.GetEventOrNull(0);
			this.totalElapsedTime = 0f;
			this.totalTimeOfPreviousStages = 0f;
			this.HandleStages();
		}

		// Token: 0x0600768C RID: 30348 RVA: 0x0026E548 File Offset: 0x0026C748
		private void HandleStages()
		{
			for (;;)
			{
				float num = this.totalElapsedTime - this.totalTimeOfPreviousStages;
				float f = Mathf.Min(num / this.activeStage.Duration, 1f);
				this.activeStage.continuousProperties.ApplyAll(f);
				while (this.nextEvent != null && num >= this.nextEvent.absoluteTime)
				{
					UnityEvent onTimeReached = this.nextEvent.onTimeReached;
					if (onTimeReached != null)
					{
						onTimeReached.Invoke();
					}
					this.Log(false, true);
					EvolvingCosmetic.EvolutionStage evolutionStage = this.activeStage;
					int index = this.nextEventIndex + 1;
					this.nextEventIndex = index;
					this.nextEvent = evolutionStage.GetEventOrNull(index);
				}
				if (num < this.activeStage.Duration)
				{
					break;
				}
				this.activeStageIndex++;
				if (this.activeStageIndex >= this.stages.Length && !this.enableLooping)
				{
					goto Block_4;
				}
				if (this.activeStageIndex >= this.stages.Length)
				{
					this.activeStageIndex = this.loopToStageOnComplete - 1;
					this.totalTimeOfPreviousStages = this.timeAtLoopStart;
					this.totalElapsedTime -= this.loopDuration;
				}
				else
				{
					this.totalTimeOfPreviousStages += this.activeStage.Duration;
				}
				this.activeStage = this.stages[this.activeStageIndex];
				this.nextEventIndex = 0;
				this.nextEvent = this.activeStage.GetEventOrNull(0);
				if (!this.activeStage.HasDuration)
				{
					this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration * 0.5f;
					TickSystem<object>.RemoveTickCallback(this);
				}
				else
				{
					TickSystem<object>.AddTickCallback(this);
				}
				this.Log(false, false);
			}
			return;
			Block_4:
			this.totalElapsedTime = this.totalDuration;
			TickSystem<object>.RemoveTickCallback(this);
			this.Log(true, false);
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x0600768D RID: 30349 RVA: 0x0026E6FC File Offset: 0x0026C8FC
		// (set) Token: 0x0600768E RID: 30350 RVA: 0x0026E704 File Offset: 0x0026C904
		public bool TickRunning { get; set; }

		// Token: 0x0600768F RID: 30351 RVA: 0x0026E710 File Offset: 0x0026C910
		public void Tick()
		{
			this.totalElapsedTime = Mathf.Clamp(this.totalElapsedTime + Mathf.Max(this.activeStage.DeltaTime(Time.deltaTime), 0f), 0f, this.totalDuration * 1.01f);
			this.HandleStages();
		}

		// Token: 0x06007690 RID: 30352 RVA: 0x0026E760 File Offset: 0x0026C960
		public void CompleteManualStage()
		{
			if (!this.activeStage.HasDuration)
			{
				this.ForceNextStage();
			}
		}

		// Token: 0x06007691 RID: 30353 RVA: 0x0026E775 File Offset: 0x0026C975
		public void ForceNextStage()
		{
			this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration;
			this.HandleStages();
		}

		// Token: 0x06007692 RID: 30354 RVA: 0x0026E795 File Offset: 0x0026C995
		private void SendElapsedTime(NetPlayer player)
		{
			if (this.sendProgressDelayCoroutine != null)
			{
				base.StopCoroutine(this.sendProgressDelayCoroutine);
			}
			this.sendProgressDelayCoroutine = base.StartCoroutine(this.SendElapsedTimeDelayed());
		}

		// Token: 0x06007693 RID: 30355 RVA: 0x0026E7BD File Offset: 0x0026C9BD
		private IEnumerator SendElapsedTimeDelayed()
		{
			yield return new WaitForSeconds(1f);
			this.sendProgressDelayCoroutine = null;
			this.networkEvents.Activate.RaiseOthers(new object[]
			{
				this.totalElapsedTime
			});
			yield break;
		}

		// Token: 0x06007694 RID: 30356 RVA: 0x0026E7CC File Offset: 0x0026C9CC
		private void ReceiveElapsedTime(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "ReceiveElapsedTime");
			if (info.senderID == this.myRig.creator.ActorNumber && this.callLimiter.CheckCallServerTime((double)Time.unscaledTime) && args.Length == 1)
			{
				object obj = args[0];
				if (obj is float)
				{
					float num = (float)obj;
					if (float.IsFinite(num) && num <= this.totalDuration && num >= 0f)
					{
						this.totalElapsedTime = num;
						this.HandleStages();
						return;
					}
				}
			}
		}

		// Token: 0x06007695 RID: 30357 RVA: 0x0026E858 File Offset: 0x0026CA58
		private void SetStage(int targetIndex)
		{
			if (this.stages == null || this.stages.Length == 0)
			{
				return;
			}
			if (this.enableLooping)
			{
				if (targetIndex < 0)
				{
					targetIndex = this.stages.Length - 1;
				}
				else if (targetIndex >= this.stages.Length)
				{
					targetIndex = 0;
				}
			}
			else
			{
				targetIndex = Mathf.Clamp(targetIndex, 0, this.stages.Length - 1);
			}
			this.activeStageIndex = targetIndex;
			this.activeStage = this.stages[targetIndex];
			float num = 0f;
			for (int i = 0; i < targetIndex; i++)
			{
				num += this.stages[i].Duration;
			}
			this.totalTimeOfPreviousStages = num;
			this.totalElapsedTime = num + Mathf.Epsilon;
			this.nextEventIndex = 0;
			this.nextEvent = this.activeStage.GetEventOrNull(0);
			if (this.activeStage.HasDuration)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			else
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			int num2 = 0;
			for (EvolvingCosmetic.EvolutionStage.EventAtTime eventOrNull = this.activeStage.GetEventOrNull(num2); eventOrNull != null; eventOrNull = this.activeStage.GetEventOrNull(num2))
			{
				UnityEvent onTimeReached = eventOrNull.onTimeReached;
				if (onTimeReached != null)
				{
					onTimeReached.Invoke();
				}
				num2++;
			}
			this.HandleStages();
		}

		// Token: 0x06007696 RID: 30358 RVA: 0x0026E96F File Offset: 0x0026CB6F
		private void RestartStageInternal()
		{
			this.SetStage(this.activeStageIndex);
		}

		// Token: 0x06007697 RID: 30359 RVA: 0x0026E97D File Offset: 0x0026CB7D
		public void IncrementStage()
		{
			this.SetStage(this.activeStageIndex + 1);
		}

		// Token: 0x06007698 RID: 30360 RVA: 0x0026E98D File Offset: 0x0026CB8D
		public void DecrementStage()
		{
			this.SetStage(this.activeStageIndex - 1);
		}

		// Token: 0x06007699 RID: 30361 RVA: 0x0026E99D File Offset: 0x0026CB9D
		public void JumpToFirstStage()
		{
			this.SetStage(0);
		}

		// Token: 0x0600769A RID: 30362 RVA: 0x0026E9A6 File Offset: 0x0026CBA6
		public void JumpToLastStage()
		{
			if (this.stages == null || this.stages.Length == 0)
			{
				return;
			}
			this.SetStage(this.stages.Length - 1);
		}

		// Token: 0x0600769B RID: 30363 RVA: 0x0026E9CA File Offset: 0x0026CBCA
		public void RestartCurrentStage()
		{
			this.RestartStageInternal();
		}

		// Token: 0x0600769C RID: 30364 RVA: 0x0026E9D2 File Offset: 0x0026CBD2
		public void JumpToStageIndex(int index)
		{
			this.SetStage(index);
		}

		// Token: 0x04008891 RID: 34961
		[SerializeField]
		private bool enableLooping;

		// Token: 0x04008892 RID: 34962
		[SerializeField]
		private int loopToStageOnComplete = 1;

		// Token: 0x04008893 RID: 34963
		[SerializeField]
		private EvolvingCosmetic.EvolutionStage[] stages;

		// Token: 0x04008894 RID: 34964
		private RubberDuckEvents networkEvents;

		// Token: 0x04008895 RID: 34965
		private VRRig myRig;

		// Token: 0x04008896 RID: 34966
		private CallLimiter callLimiter = new CallLimiter(5, 10f, 0.5f);

		// Token: 0x04008897 RID: 34967
		private int activeStageIndex;

		// Token: 0x04008898 RID: 34968
		private EvolvingCosmetic.EvolutionStage activeStage;

		// Token: 0x04008899 RID: 34969
		private int nextEventIndex;

		// Token: 0x0400889A RID: 34970
		private EvolvingCosmetic.EvolutionStage.EventAtTime nextEvent;

		// Token: 0x0400889B RID: 34971
		private float totalElapsedTime;

		// Token: 0x0400889C RID: 34972
		private float totalTimeOfPreviousStages;

		// Token: 0x0400889D RID: 34973
		private float totalDuration;

		// Token: 0x0400889E RID: 34974
		private float timeAtLoopStart;

		// Token: 0x0400889F RID: 34975
		private float loopDuration;

		// Token: 0x040088A0 RID: 34976
		private Coroutine sendProgressDelayCoroutine;

		// Token: 0x02001279 RID: 4729
		[Serializable]
		private class EvolutionStage
		{
			// Token: 0x0600769E RID: 30366 RVA: 0x0026EA00 File Offset: 0x0026CC00
			private bool HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags flag)
			{
				return (this.progressionFlags & flag) > EvolvingCosmetic.EvolutionStage.ProgressionFlags.None;
			}

			// Token: 0x17000B6E RID: 2926
			// (get) Token: 0x0600769F RID: 30367 RVA: 0x0026EA0D File Offset: 0x0026CC0D
			public bool HasDuration
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time | EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
				}
			}

			// Token: 0x17000B6F RID: 2927
			// (get) Token: 0x060076A0 RID: 30368 RVA: 0x0026EA16 File Offset: 0x0026CC16
			public bool HasTime
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time);
				}
			}

			// Token: 0x17000B70 RID: 2928
			// (get) Token: 0x060076A1 RID: 30369 RVA: 0x0026EA1F File Offset: 0x0026CC1F
			public bool HasTemperature
			{
				get
				{
					return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
				}
			}

			// Token: 0x17000B71 RID: 2929
			// (get) Token: 0x060076A2 RID: 30370 RVA: 0x0026EA28 File Offset: 0x0026CC28
			public float Duration
			{
				get
				{
					if (!this.HasDuration)
					{
						return 1f;
					}
					return this.durationSeconds;
				}
			}

			// Token: 0x060076A3 RID: 30371 RVA: 0x0026EA3E File Offset: 0x0026CC3E
			public float DeltaTime(float deltaTime)
			{
				return (this.HasTime ? deltaTime : 0f) + (this.HasTemperature ? (deltaTime * this.celsiusSpeedupMult.Evaluate(this.thermalReceiver.celsius)) : 0f);
			}

			// Token: 0x060076A4 RID: 30372 RVA: 0x0026EA78 File Offset: 0x0026CC78
			public EvolvingCosmetic.EvolutionStage.EventAtTime GetEventOrNull(int index)
			{
				if (this.events == null || index < 0 || index >= this.events.Length)
				{
					return null;
				}
				return this.events[index];
			}

			// Token: 0x040088A2 RID: 34978
			private const float MIN_STAGE_TIME = 0.01f;

			// Token: 0x040088A3 RID: 34979
			public string debugName;

			// Token: 0x040088A4 RID: 34980
			public EvolvingCosmetic.EvolutionStage.ProgressionFlags progressionFlags = EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time;

			// Token: 0x040088A5 RID: 34981
			[SerializeField]
			private float durationSeconds = float.NaN;

			// Token: 0x040088A6 RID: 34982
			public ThermalReceiver thermalReceiver;

			// Token: 0x040088A7 RID: 34983
			public AnimationCurve celsiusSpeedupMult = AnimationCurve.Linear(0f, 0f, 100f, 2f);

			// Token: 0x040088A8 RID: 34984
			public ContinuousPropertyArray continuousProperties;

			// Token: 0x040088A9 RID: 34985
			[SerializeField]
			private EvolvingCosmetic.EvolutionStage.EventAtTime[] events;

			// Token: 0x0200127A RID: 4730
			[Flags]
			public enum ProgressionFlags
			{
				// Token: 0x040088AB RID: 34987
				None = 0,
				// Token: 0x040088AC RID: 34988
				Time = 1,
				// Token: 0x040088AD RID: 34989
				Temperature = 2
			}

			// Token: 0x0200127B RID: 4731
			[Serializable]
			public class EventAtTime : IComparable<EvolvingCosmetic.EvolutionStage.EventAtTime>
			{
				// Token: 0x17000B72 RID: 2930
				// (get) Token: 0x060076A6 RID: 30374 RVA: 0x0026EAD4 File Offset: 0x0026CCD4
				private string DynamicTimeLabel
				{
					get
					{
						if (this.type != EvolvingCosmetic.EvolutionStage.EventAtTime.Type.DurationFraction)
						{
							return "Time";
						}
						return "Fraction";
					}
				}

				// Token: 0x060076A7 RID: 30375 RVA: 0x0026EAEA File Offset: 0x0026CCEA
				public int CompareTo(EvolvingCosmetic.EvolutionStage.EventAtTime other)
				{
					return this.absoluteTime.CompareTo(other.absoluteTime);
				}

				// Token: 0x040088AE RID: 34990
				public string debugName;

				// Token: 0x040088AF RID: 34991
				public float time;

				// Token: 0x040088B0 RID: 34992
				public EvolvingCosmetic.EvolutionStage.EventAtTime.Type type;

				// Token: 0x040088B1 RID: 34993
				public float absoluteTime;

				// Token: 0x040088B2 RID: 34994
				public UnityEvent onTimeReached;

				// Token: 0x0200127C RID: 4732
				public enum Type
				{
					// Token: 0x040088B4 RID: 34996
					SecondsFromBeginning,
					// Token: 0x040088B5 RID: 34997
					SecondsBeforeEnd,
					// Token: 0x040088B6 RID: 34998
					DurationFraction
				}
			}
		}
	}
}
