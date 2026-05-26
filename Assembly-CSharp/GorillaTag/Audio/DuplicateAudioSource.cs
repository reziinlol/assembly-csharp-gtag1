using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011F1 RID: 4593
	public class DuplicateAudioSource : MonoBehaviour
	{
		// Token: 0x0600733A RID: 29498 RVA: 0x002570FE File Offset: 0x002552FE
		public void SetTargetAudioSource(AudioSource target)
		{
			this.TargetAudioSource = target;
			this.StartDuplicating();
		}

		// Token: 0x0600733B RID: 29499 RVA: 0x00257110 File Offset: 0x00255310
		[ContextMenu("Start Duplicating")]
		public void StartDuplicating()
		{
			this._isDuplicating = true;
			this._audioSource.loop = this.TargetAudioSource.loop;
			this._audioSource.clip = this.TargetAudioSource.clip;
			if (this.TargetAudioSource.isPlaying)
			{
				this._audioSource.Play();
			}
		}

		// Token: 0x0600733C RID: 29500 RVA: 0x00257168 File Offset: 0x00255368
		[ContextMenu("Stop Duplicating")]
		public void StopDuplicating()
		{
			this._isDuplicating = false;
			this._audioSource.Stop();
		}

		// Token: 0x0600733D RID: 29501 RVA: 0x0025717C File Offset: 0x0025537C
		public void LateUpdate()
		{
			if (this._isDuplicating)
			{
				if (this.TargetAudioSource.isPlaying && !this._audioSource.isPlaying)
				{
					this._audioSource.Play();
					return;
				}
				if (!this.TargetAudioSource.isPlaying && this._audioSource.isPlaying)
				{
					this._audioSource.Stop();
				}
			}
		}

		// Token: 0x0400839F RID: 33695
		public AudioSource TargetAudioSource;

		// Token: 0x040083A0 RID: 33696
		[SerializeField]
		private AudioSource _audioSource;

		// Token: 0x040083A1 RID: 33697
		[SerializeField]
		private bool _isDuplicating;
	}
}
