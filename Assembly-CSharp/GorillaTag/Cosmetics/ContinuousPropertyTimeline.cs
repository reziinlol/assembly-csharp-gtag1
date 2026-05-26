using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200125D RID: 4701
	public class ContinuousPropertyTimeline : MonoBehaviour, ITickSystemTick, ISpawnable
	{
		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x060075C0 RID: 30144 RVA: 0x00269512 File Offset: 0x00267712
		// (set) Token: 0x060075C1 RID: 30145 RVA: 0x0026951D File Offset: 0x0026771D
		private bool IsBackward
		{
			get
			{
				return !this.IsForward;
			}
			set
			{
				this.IsForward = !value;
			}
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x060075C2 RID: 30146 RVA: 0x00269529 File Offset: 0x00267729
		// (set) Token: 0x060075C3 RID: 30147 RVA: 0x00269534 File Offset: 0x00267734
		private bool IsPaused
		{
			get
			{
				return !this.IsPlaying;
			}
			set
			{
				this.IsPlaying = !value;
			}
		}

		// Token: 0x060075C4 RID: 30148 RVA: 0x00269540 File Offset: 0x00267740
		public void TimelinePlay()
		{
			this.IsPlaying = true;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060075C5 RID: 30149 RVA: 0x0026954F File Offset: 0x0026774F
		public void TimelinePause()
		{
			this.IsPaused = true;
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060075C6 RID: 30150 RVA: 0x0026955E File Offset: 0x0026775E
		public void TimelineToggleDirection()
		{
			this.IsForward = !this.IsForward;
		}

		// Token: 0x060075C7 RID: 30151 RVA: 0x0026956F File Offset: 0x0026776F
		public void TimelineTogglePlay()
		{
			if (this.IsPlaying)
			{
				this.TimelinePause();
				return;
			}
			this.TimelinePlay();
		}

		// Token: 0x060075C8 RID: 30152 RVA: 0x00269586 File Offset: 0x00267786
		public void TimelinePlayForward()
		{
			this.IsForward = true;
			this.TimelinePlay();
		}

		// Token: 0x060075C9 RID: 30153 RVA: 0x00269595 File Offset: 0x00267795
		public void TimelinePlayBackward()
		{
			this.IsBackward = true;
			this.TimelinePlay();
		}

		// Token: 0x060075CA RID: 30154 RVA: 0x002695A4 File Offset: 0x002677A4
		public void TimelinePlayFromBeginning()
		{
			this.time = 0f;
			this.TimelinePlayForward();
			this.OnReachedBeginning();
		}

		// Token: 0x060075CB RID: 30155 RVA: 0x002695BD File Offset: 0x002677BD
		public void TimelinePlayFromEnd()
		{
			this.time = this.durationSeconds;
			this.TimelinePlayBackward();
			this.OnReachedEnd();
		}

		// Token: 0x060075CC RID: 30156 RVA: 0x002695D7 File Offset: 0x002677D7
		public void TimelineScrubToTime(float t)
		{
			if (t <= 0f)
			{
				this.time = 0f;
				this.OnReachedBeginning();
				return;
			}
			if (t >= this.durationSeconds)
			{
				this.time = this.durationSeconds;
				this.OnReachedEnd();
				return;
			}
			this.time = t;
		}

		// Token: 0x060075CD RID: 30157 RVA: 0x00269616 File Offset: 0x00267816
		public void TimelineScrubToFraction(float f)
		{
			this.TimelineScrubToTime(f * this.durationSeconds);
		}

		// Token: 0x060075CE RID: 30158 RVA: 0x00269626 File Offset: 0x00267826
		public void TimelineSetDuration(float d)
		{
			this.durationSeconds = d;
			this.inverseDuration = 1f / this.durationSeconds;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
		}

		// Token: 0x060075CF RID: 30159 RVA: 0x00269654 File Offset: 0x00267854
		public void TimelineSetBackwardDuration(float d)
		{
			this.separateBackwardDuration = true;
			this.backwardDuration = d;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
		}

		// Token: 0x060075D0 RID: 30160 RVA: 0x00269677 File Offset: 0x00267877
		private void Awake()
		{
			this.IsPlaying = this.startPlaying;
		}

		// Token: 0x060075D1 RID: 30161 RVA: 0x00269688 File Offset: 0x00267888
		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			this.inverseDuration = 1f / this.durationSeconds;
			this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnEnable, this.myRig != null && this.myRig.isLocal);
			if (this.IsPlaying)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x060075D2 RID: 30162 RVA: 0x0026970A File Offset: 0x0026790A
		private void OnDisable()
		{
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnDisable, this.myRig != null && this.myRig.isLocal);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060075D3 RID: 30163 RVA: 0x0026973C File Offset: 0x0026793C
		private void OnReachedEnd()
		{
			if (this.IsForward)
			{
				switch (this.endBehavior)
				{
				case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
					this.TimelinePause();
					this.time = this.durationSeconds;
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
					this.TimelinePlayFromBeginning();
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
					this.IsBackward = true;
					this.time = this.durationSeconds;
					break;
				}
			}
			this.continuousProperties.cachedRigIsLocal = (this.myRig != null && this.myRig.isLocal);
			this.continuousProperties.ApplyAll(1f);
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedEnd, this.myRig != null && this.myRig.isLocal);
		}

		// Token: 0x060075D4 RID: 30164 RVA: 0x002697FC File Offset: 0x002679FC
		private void OnReachedBeginning()
		{
			if (this.IsBackward)
			{
				switch (this.endBehavior)
				{
				case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
					this.TimelinePause();
					this.time = 0f;
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
					this.TimelinePlayFromEnd();
					break;
				case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
					this.IsForward = true;
					this.time = 0f;
					break;
				}
			}
			this.continuousProperties.cachedRigIsLocal = (this.myRig != null && this.myRig.isLocal);
			this.continuousProperties.ApplyAll(0f);
			this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedBeginning, this.myRig != null && this.myRig.isLocal);
		}

		// Token: 0x060075D5 RID: 30165 RVA: 0x002698B8 File Offset: 0x00267AB8
		private void InBetween()
		{
			float f = this.time * this.inverseDuration;
			this.continuousProperties.cachedRigIsLocal = (this.myRig != null && this.myRig.isLocal);
			this.continuousProperties.ApplyAll(f);
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x060075D6 RID: 30166 RVA: 0x00269906 File Offset: 0x00267B06
		// (set) Token: 0x060075D7 RID: 30167 RVA: 0x0026990E File Offset: 0x00267B0E
		public bool TickRunning { get; set; }

		// Token: 0x060075D8 RID: 30168 RVA: 0x00269918 File Offset: 0x00267B18
		public void Tick()
		{
			if (this.IsForward)
			{
				this.time += Time.deltaTime;
				if (this.time >= this.durationSeconds)
				{
					this.OnReachedEnd();
					return;
				}
				this.InBetween();
				return;
			}
			else
			{
				this.time -= Time.deltaTime * this.backwardDeltaMult;
				if (this.time <= 0f)
				{
					this.OnReachedBeginning();
					return;
				}
				this.InBetween();
				return;
			}
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x060075D9 RID: 30169 RVA: 0x0026998E File Offset: 0x00267B8E
		// (set) Token: 0x060075DA RID: 30170 RVA: 0x00269996 File Offset: 0x00267B96
		public bool IsSpawned { get; set; }

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x060075DB RID: 30171 RVA: 0x0026999F File Offset: 0x00267B9F
		// (set) Token: 0x060075DC RID: 30172 RVA: 0x002699A7 File Offset: 0x00267BA7
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060075DD RID: 30173 RVA: 0x002699B0 File Offset: 0x00267BB0
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x060075DE RID: 30174 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x04008788 RID: 34696
		[SerializeField]
		private float durationSeconds = 1f;

		// Token: 0x04008789 RID: 34697
		[SerializeField]
		private float backwardDuration = 1f;

		// Token: 0x0400878A RID: 34698
		[Tooltip("If true, the the timeline can move at a different speed when playing backwards.")]
		[SerializeField]
		private bool separateBackwardDuration;

		// Token: 0x0400878B RID: 34699
		[Tooltip("When this object is enabled for the first time, should it immediately start playing from the beginning?")]
		[SerializeField]
		private bool startPlaying;

		// Token: 0x0400878C RID: 34700
		[Tooltip("Determine what happens when the timeline reaches the end (or beginning while playing backwards).")]
		[SerializeField]
		private ContinuousPropertyTimeline.TimelineEndBehavior endBehavior;

		// Token: 0x0400878D RID: 34701
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x0400878E RID: 34702
		[SerializeField]
		private FlagEvents<ContinuousPropertyTimeline.TimelineEvent> events;

		// Token: 0x0400878F RID: 34703
		private float time;

		// Token: 0x04008790 RID: 34704
		private float inverseDuration;

		// Token: 0x04008791 RID: 34705
		private float backwardDeltaMult;

		// Token: 0x04008792 RID: 34706
		private bool IsForward = true;

		// Token: 0x04008793 RID: 34707
		private bool IsPlaying;

		// Token: 0x04008795 RID: 34709
		private VRRig myRig;

		// Token: 0x0200125E RID: 4702
		private enum TimelineEndBehavior
		{
			// Token: 0x04008799 RID: 34713
			Stop,
			// Token: 0x0400879A RID: 34714
			Loop,
			// Token: 0x0400879B RID: 34715
			PingPong
		}

		// Token: 0x0200125F RID: 4703
		[Flags]
		private enum TimelineEvent
		{
			// Token: 0x0400879D RID: 34717
			OnReachedEnd = 1,
			// Token: 0x0400879E RID: 34718
			OnReachedBeginning = 2,
			// Token: 0x0400879F RID: 34719
			OnEnable = 4,
			// Token: 0x040087A0 RID: 34720
			OnDisable = 8
		}
	}
}
