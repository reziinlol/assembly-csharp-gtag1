using System;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x020011A3 RID: 4515
	public class ShakeReaction : MonoBehaviour, ITickSystemPost
	{
		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x0600722E RID: 29230 RVA: 0x00252E12 File Offset: 0x00251012
		private float loopSoundTotalDuration
		{
			get
			{
				return this.loopSoundFadeInDuration + this.loopSoundSustainDuration + this.loopSoundFadeOutDuration;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x0600722F RID: 29231 RVA: 0x00252E28 File Offset: 0x00251028
		// (set) Token: 0x06007230 RID: 29232 RVA: 0x00252E30 File Offset: 0x00251030
		bool ITickSystemPost.PostTickRunning { get; set; }

		// Token: 0x06007231 RID: 29233 RVA: 0x00252E3C File Offset: 0x0025103C
		protected void Awake()
		{
			this.sampleHistoryPos = new Vector3[256];
			this.sampleHistoryTime = new float[256];
			this.sampleHistoryVel = new Vector3[256];
			if (this.particles != null)
			{
				this.maxEmissionRate = this.particles.emission.rateOverTime.constant;
			}
			Application.quitting += this.HandleApplicationQuitting;
		}

		// Token: 0x06007232 RID: 29234 RVA: 0x00252EBC File Offset: 0x002510BC
		protected void OnEnable()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			for (int i = 0; i < 256; i++)
			{
				this.sampleHistoryTime[i] = unscaledTime;
				this.sampleHistoryPos[i] = position;
				this.sampleHistoryVel[i] = Vector3.zero;
			}
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.loop = true;
				this.loopSoundAudioSource.GTPlay();
			}
			this.hasLoopSound = (this.loopSoundAudioSource != null);
			this.hasShakeSound = (this.shakeSoundBankPlayer != null);
			this.hasParticleSystem = (this.particles != null);
			TickSystem<object>.AddPostTickCallback(this);
		}

		// Token: 0x06007233 RID: 29235 RVA: 0x00252F73 File Offset: 0x00251173
		protected void OnDisable()
		{
			if (this.loopSoundAudioSource != null)
			{
				this.loopSoundAudioSource.GTStop();
			}
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06007234 RID: 29236 RVA: 0x00156E8B File Offset: 0x0015508B
		private void HandleApplicationQuitting()
		{
			TickSystem<object>.RemovePostTickCallback(this);
		}

		// Token: 0x06007235 RID: 29237 RVA: 0x00252F94 File Offset: 0x00251194
		void ITickSystemPost.PostTick()
		{
			float unscaledTime = Time.unscaledTime;
			Vector3 position = this.shakeXform.position;
			int num = (this.currentIndex - 1 + 256) % 256;
			this.currentIndex = (this.currentIndex + 1) % 256;
			this.sampleHistoryTime[this.currentIndex] = unscaledTime;
			float num2 = unscaledTime - this.sampleHistoryTime[num];
			this.sampleHistoryPos[this.currentIndex] = position;
			if (num2 > 0f)
			{
				Vector3 a = position - this.sampleHistoryPos[num];
				this.sampleHistoryVel[this.currentIndex] = a / num2;
			}
			else
			{
				this.sampleHistoryVel[this.currentIndex] = Vector3.zero;
			}
			float sqrMagnitude = (this.sampleHistoryVel[num] - this.sampleHistoryVel[this.currentIndex]).sqrMagnitude;
			this.poopVelocity = Mathf.Round(Mathf.Sqrt(sqrMagnitude) * 1000f) / 1000f;
			float num3 = this.shakeXform.lossyScale.x * this.velocityThreshold * this.velocityThreshold;
			if (sqrMagnitude >= num3)
			{
				this.lastShakeTime = unscaledTime;
			}
			float num4 = unscaledTime - this.lastShakeTime;
			float time = Mathf.Clamp01(num4 / this.particleDuration);
			if (this.hasParticleSystem)
			{
				this.particles.emission.rateOverTime = this.emissionCurve.Evaluate(time) * this.maxEmissionRate;
			}
			if (this.hasShakeSound && this.lastShakeTime - this.lastShakeSoundTime > this.shakeSoundCooldown)
			{
				this.shakeSoundBankPlayer.Play();
				this.lastShakeSoundTime = unscaledTime;
			}
			if (this.hasLoopSound)
			{
				if (num4 < this.loopSoundFadeInDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeInCurve.Evaluate(Mathf.Clamp01(num4 / this.loopSoundFadeInDuration));
					return;
				}
				if (num4 < this.loopSoundFadeInDuration + this.loopSoundSustainDuration)
				{
					this.loopSoundAudioSource.volume = this.loopSoundBaseVolume;
					return;
				}
				this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeOutCurve.Evaluate(Mathf.Clamp01((num4 - this.loopSoundFadeInDuration - this.loopSoundSustainDuration) / this.loopSoundFadeOutDuration));
			}
		}

		// Token: 0x040081EF RID: 33263
		[SerializeField]
		private Transform shakeXform;

		// Token: 0x040081F0 RID: 33264
		[SerializeField]
		private float velocityThreshold = 5f;

		// Token: 0x040081F1 RID: 33265
		[SerializeField]
		private SoundBankPlayer shakeSoundBankPlayer;

		// Token: 0x040081F2 RID: 33266
		[SerializeField]
		private float shakeSoundCooldown = 1f;

		// Token: 0x040081F3 RID: 33267
		[SerializeField]
		private AudioSource loopSoundAudioSource;

		// Token: 0x040081F4 RID: 33268
		[SerializeField]
		private float loopSoundBaseVolume = 1f;

		// Token: 0x040081F5 RID: 33269
		[SerializeField]
		private float loopSoundSustainDuration = 1f;

		// Token: 0x040081F6 RID: 33270
		[SerializeField]
		private float loopSoundFadeInDuration = 1f;

		// Token: 0x040081F7 RID: 33271
		[SerializeField]
		private AnimationCurve loopSoundFadeInCurve;

		// Token: 0x040081F8 RID: 33272
		[SerializeField]
		private float loopSoundFadeOutDuration = 1f;

		// Token: 0x040081F9 RID: 33273
		[SerializeField]
		private AnimationCurve loopSoundFadeOutCurve;

		// Token: 0x040081FA RID: 33274
		[SerializeField]
		private ParticleSystem particles;

		// Token: 0x040081FB RID: 33275
		[SerializeField]
		private AnimationCurve emissionCurve;

		// Token: 0x040081FC RID: 33276
		[SerializeField]
		private float particleDuration = 5f;

		// Token: 0x040081FE RID: 33278
		private const int sampleHistorySize = 256;

		// Token: 0x040081FF RID: 33279
		private float[] sampleHistoryTime;

		// Token: 0x04008200 RID: 33280
		private Vector3[] sampleHistoryPos;

		// Token: 0x04008201 RID: 33281
		private Vector3[] sampleHistoryVel;

		// Token: 0x04008202 RID: 33282
		private int currentIndex;

		// Token: 0x04008203 RID: 33283
		private float lastShakeSoundTime = float.MinValue;

		// Token: 0x04008204 RID: 33284
		private float lastShakeTime = float.MinValue;

		// Token: 0x04008205 RID: 33285
		private float maxEmissionRate;

		// Token: 0x04008206 RID: 33286
		private bool hasLoopSound;

		// Token: 0x04008207 RID: 33287
		private bool hasShakeSound;

		// Token: 0x04008208 RID: 33288
		private bool hasParticleSystem;

		// Token: 0x04008209 RID: 33289
		[DebugReadout]
		private float poopVelocity;
	}
}
