using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B1 RID: 4785
	public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
	{
		// Token: 0x060077BB RID: 30651 RVA: 0x00274310 File Offset: 0x00272510
		private void Awake()
		{
			if (this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
			}
			if (this.soundBank != null && this.audioSource != null)
			{
				this.soundBank.audioSource = this.audioSource;
			}
		}

		// Token: 0x060077BC RID: 30652 RVA: 0x0027436C File Offset: 0x0027256C
		private void Update()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (averageVelocity.magnitude < this.minVelocityThreshold)
			{
				return;
			}
			float t = Mathf.InverseLerp(this.minVelocityThreshold, this.maxVelocity, averageVelocity.magnitude);
			float num = Mathf.Lerp(this.minOutputVolume, this.maxOutputVolume, t);
			this.audioSource.volume = num;
			if (this.audioSource != null && !this.audioSource.isPlaying && this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (this.soundBank != null && this.soundBank.soundBank != null && !this.soundBank.isPlaying)
			{
				this.soundBank.Play(new float?(num), null);
			}
		}

		// Token: 0x04008A5B RID: 35419
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04008A5C RID: 35420
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008A5D RID: 35421
		[SerializeField]
		private AudioClip audioClip;

		// Token: 0x04008A5E RID: 35422
		[SerializeField]
		private SoundBankPlayer soundBank;

		// Token: 0x04008A5F RID: 35423
		[Tooltip(" Minimum velocity to trigger audio")]
		[SerializeField]
		private float minVelocityThreshold = 0.5f;

		// Token: 0x04008A60 RID: 35424
		[SerializeField]
		private float maxVelocity = 2f;

		// Token: 0x04008A61 RID: 35425
		[SerializeField]
		private float minOutputVolume;

		// Token: 0x04008A62 RID: 35426
		[SerializeField]
		private float maxOutputVolume = 1f;
	}
}
