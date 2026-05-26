using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FB8 RID: 4024
	public class BuilderScaleAudioRadius : MonoBehaviour
	{
		// Token: 0x06006490 RID: 25744 RVA: 0x0020672D File Offset: 0x0020492D
		private void OnEnable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x00206749 File Offset: 0x00204949
		private void OnDisable()
		{
			if (this.useLossyScaleOnEnable)
			{
				this.RevertScale();
			}
		}

		// Token: 0x06006492 RID: 25746 RVA: 0x00206759 File Offset: 0x00204959
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScaleOnEnable)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x06006493 RID: 25747 RVA: 0x00206795 File Offset: 0x00204995
		private void PlaySound()
		{
			if (this.autoPlaySoundBank != null)
			{
				this.autoPlaySoundBank.Play();
				return;
			}
			if (this.audioSource.clip != null)
			{
				this.audioSource.Play();
			}
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x002067D0 File Offset: 0x002049D0
		public void SetScale(float inScale)
		{
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay)
				{
					this.PlaySound();
				}
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.maxDist = this.audioSource.maxDistance;
					this.audioSource.maxDistance *= this.scale;
				}
			}
			else
			{
				this.minDist = this.audioSource.minDistance;
				this.maxDist = this.audioSource.maxDistance;
				this.audioSource.maxDistance *= this.scale;
				this.audioSource.minDistance *= this.scale;
			}
			if (this.autoPlay)
			{
				this.PlaySound();
			}
			this.shouldRevert = true;
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x002068D0 File Offset: 0x00204AD0
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			AudioRolloffMode rolloffMode = this.audioSource.rolloffMode;
			if (rolloffMode > AudioRolloffMode.Linear)
			{
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					this.audioSource.maxDistance = this.maxDist;
				}
			}
			else
			{
				this.audioSource.minDistance = this.minDist;
				this.audioSource.maxDistance = this.maxDist;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x0400735E RID: 29534
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScaleOnEnable;

		// Token: 0x0400735F RID: 29535
		[Tooltip("Play sound after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04007360 RID: 29536
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007361 RID: 29537
		[FormerlySerializedAs("soundBankToPlay")]
		[SerializeField]
		private SoundBankPlayer autoPlaySoundBank;

		// Token: 0x04007362 RID: 29538
		private float minDist;

		// Token: 0x04007363 RID: 29539
		private float maxDist = 1f;

		// Token: 0x04007364 RID: 29540
		private AnimationCurve customCurve;

		// Token: 0x04007365 RID: 29541
		private AnimationCurve scaledCurve = new AnimationCurve();

		// Token: 0x04007366 RID: 29542
		private float scale = 1f;

		// Token: 0x04007367 RID: 29543
		private bool shouldRevert;

		// Token: 0x04007368 RID: 29544
		private bool setScaleNextFrame;

		// Token: 0x04007369 RID: 29545
		private int enableFrame;
	}
}
