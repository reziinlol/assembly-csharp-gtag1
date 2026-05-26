using System;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x02001301 RID: 4865
	[RequireComponent(typeof(SoundBankPlayer))]
	public class SoundBankPlayerCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x06007991 RID: 31121 RVA: 0x0027FD8E File Offset: 0x0027DF8E
		// (set) Token: 0x06007992 RID: 31122 RVA: 0x0027FD96 File Offset: 0x0027DF96
		public bool TickRunning { get; set; }

		// Token: 0x06007993 RID: 31123 RVA: 0x0027FD9F File Offset: 0x0027DF9F
		private void Awake()
		{
			this.playAudioLoop = false;
		}

		// Token: 0x06007994 RID: 31124 RVA: 0x00019E3F File Offset: 0x0001803F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007995 RID: 31125 RVA: 0x00019E47 File Offset: 0x00018047
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007996 RID: 31126 RVA: 0x0027FDA8 File Offset: 0x0027DFA8
		public void Tick()
		{
			if (!this.playAudioLoop)
			{
				return;
			}
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null && !this.soundBankPlayer.audioSource.isPlaying)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06007997 RID: 31127 RVA: 0x0027FE10 File Offset: 0x0027E010
		public void PlayAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x06007998 RID: 31128 RVA: 0x0027FE5C File Offset: 0x0027E05C
		public void PlayAudioLoop()
		{
			this.playAudioLoop = true;
		}

		// Token: 0x06007999 RID: 31129 RVA: 0x0027FE68 File Offset: 0x0027E068
		public void PlayAudioNonInterrupting()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				if (this.soundBankPlayer.audioSource.isPlaying)
				{
					return;
				}
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x0600799A RID: 31130 RVA: 0x0027FEC8 File Offset: 0x0027E0C8
		public void PlayAudioWithTunableVolume(bool leftHand, float fingerValue)
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				float volume = Mathf.Clamp01(fingerValue);
				this.soundBankPlayer.audioSource.volume = volume;
				this.soundBankPlayer.Play();
			}
		}

		// Token: 0x0600799B RID: 31131 RVA: 0x0027FF2C File Offset: 0x0027E12C
		public void StopAudio()
		{
			if (this.soundBankPlayer != null && this.soundBankPlayer.audioSource != null && this.soundBankPlayer.soundBank != null)
			{
				this.soundBankPlayer.audioSource.Stop();
			}
			this.playAudioLoop = false;
		}

		// Token: 0x04008C51 RID: 35921
		[SerializeField]
		private SoundBankPlayer soundBankPlayer;

		// Token: 0x04008C52 RID: 35922
		private bool playAudioLoop;
	}
}
