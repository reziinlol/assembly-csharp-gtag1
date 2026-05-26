using System;
using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B5 RID: 4789
	[RequireComponent(typeof(LoudSpeakerActivator))]
	public class VoiceBroadcastCosmetic : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060077CA RID: 30666 RVA: 0x00274AAE File Offset: 0x00272CAE
		private void Awake()
		{
			this.loudSpeaker = base.GetComponent<LoudSpeakerActivator>();
			this.animator = base.GetComponent<Animator>();
			this.talkAnimationTrigger = Animator.StringToHash(this.talkAnimationTriggerName);
			this.gsl = base.GetComponentInParent<GorillaSpeakerLoudness>();
		}

		// Token: 0x060077CB RID: 30667 RVA: 0x00274AE5 File Offset: 0x00272CE5
		public void SetWearable(VoiceBroadcastCosmeticWearable wearable)
		{
			this.wearable = wearable;
		}

		// Token: 0x060077CC RID: 30668 RVA: 0x00274AF0 File Offset: 0x00272CF0
		private void StartBroadcast()
		{
			this.loudSpeaker.StartLocalBroadcast();
			UnityEvent unityEvent = this.onStartListening;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent unityEvent2 = this.onStartListening;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke();
			}
			this.wearable.OnCosmeticStartListening();
			this.lastSliceUpdateTime = Time.time;
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060077CD RID: 30669 RVA: 0x00274B47 File Offset: 0x00272D47
		private void StopBroadcast()
		{
			this.loudSpeaker.StopLocalBroadcast();
			UnityEvent unityEvent = this.onStopListening;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this.wearable.OnCosmeticStopListening();
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060077CE RID: 30670 RVA: 0x00274B78 File Offset: 0x00272D78
		public void OnEnable()
		{
			this.isListening = false;
			this.speakingTime = 0f;
		}

		// Token: 0x060077CF RID: 30671 RVA: 0x00274B8C File Offset: 0x00272D8C
		public void OnDisable()
		{
			this.isListening = false;
			this.speakingTime = 0f;
			this.StopBroadcast();
		}

		// Token: 0x060077D0 RID: 30672 RVA: 0x00274BA8 File Offset: 0x00272DA8
		public void SetListenState(bool listening)
		{
			if (this.isListening == listening || !base.enabled || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.isListening = listening;
			this.speakingTime = 0f;
			if (listening)
			{
				this.StartBroadcast();
				return;
			}
			this.StopBroadcast();
		}

		// Token: 0x060077D1 RID: 30673 RVA: 0x00274BF8 File Offset: 0x00272DF8
		public void SliceUpdate()
		{
			float num = Time.time - this.lastSliceUpdateTime;
			this.lastSliceUpdateTime = Time.time;
			if (this.gsl != null && this.gsl.IsSpeaking && this.gsl.LoudnessNormalized >= this.minVolume)
			{
				this.speakingTime += num;
				if (this.speakingTime >= this.minSpeakingTime)
				{
					if (this.animator != null)
					{
						this.animator.SetTrigger(this.talkAnimationTrigger);
					}
					if (this.simpleAnimation != null && !this.simpleAnimation.isPlaying)
					{
						this.simpleAnimation.Play();
					}
					if (!this.isSpeaking)
					{
						UnityEvent unityEvent = this.onStartSpeaking;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
						this.isSpeaking = true;
						return;
					}
				}
			}
			else
			{
				this.speakingTime = 0f;
				if (this.isSpeaking)
				{
					UnityEvent unityEvent2 = this.onStopSpeaking;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke();
					}
					this.isSpeaking = false;
				}
			}
		}

		// Token: 0x060077D2 RID: 30674 RVA: 0x00274D05 File Offset: 0x00272F05
		private void ResetToFirstFrame()
		{
			this.simpleAnimation.Rewind();
			this.simpleAnimation.Play();
			this.simpleAnimation.Sample();
			this.simpleAnimation.Stop();
		}

		// Token: 0x04008A86 RID: 35462
		public TalkingCosmeticType talkingCosmeticType;

		// Token: 0x04008A87 RID: 35463
		[Tooltip("How loud the Gorilla voice should be before detecting as talking.")]
		[SerializeField]
		public float minVolume = 0.1f;

		// Token: 0x04008A88 RID: 35464
		[Tooltip("How long the initial speaking section needs to last to trigger the talking animation.")]
		[SerializeField]
		public float minSpeakingTime = 0.15f;

		// Token: 0x04008A89 RID: 35465
		[SerializeField]
		private Animation simpleAnimation;

		// Token: 0x04008A8A RID: 35466
		[SerializeField]
		private string talkAnimationTriggerName;

		// Token: 0x04008A8B RID: 35467
		private int talkAnimationTrigger;

		// Token: 0x04008A8C RID: 35468
		private const string EVENTS = "Events";

		// Token: 0x04008A8D RID: 35469
		[SerializeField]
		private UnityEvent onStartListening;

		// Token: 0x04008A8E RID: 35470
		[SerializeField]
		private UnityEvent onStartSpeaking;

		// Token: 0x04008A8F RID: 35471
		[SerializeField]
		private UnityEvent onStopSpeaking;

		// Token: 0x04008A90 RID: 35472
		[SerializeField]
		private UnityEvent onStopListening;

		// Token: 0x04008A91 RID: 35473
		private float speakingTime;

		// Token: 0x04008A92 RID: 35474
		private bool isListening;

		// Token: 0x04008A93 RID: 35475
		private bool isSpeaking;

		// Token: 0x04008A94 RID: 35476
		private VoiceBroadcastCosmeticWearable wearable;

		// Token: 0x04008A95 RID: 35477
		private LoudSpeakerActivator loudSpeaker;

		// Token: 0x04008A96 RID: 35478
		private GorillaSpeakerLoudness gsl;

		// Token: 0x04008A97 RID: 35479
		private Animator animator;

		// Token: 0x04008A98 RID: 35480
		private float lastSliceUpdateTime;
	}
}
