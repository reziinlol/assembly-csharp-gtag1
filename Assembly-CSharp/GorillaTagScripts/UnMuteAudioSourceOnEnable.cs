using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F29 RID: 3881
	public class UnMuteAudioSourceOnEnable : MonoBehaviour
	{
		// Token: 0x060060EC RID: 24812 RVA: 0x001F36C2 File Offset: 0x001F18C2
		public void Awake()
		{
			this.originalVolume = this.audioSource.volume;
		}

		// Token: 0x060060ED RID: 24813 RVA: 0x001F36D5 File Offset: 0x001F18D5
		public void OnEnable()
		{
			this.audioSource.volume = this.originalVolume;
		}

		// Token: 0x060060EE RID: 24814 RVA: 0x001F36E8 File Offset: 0x001F18E8
		public void OnDisable()
		{
			this.audioSource.volume = 0f;
		}

		// Token: 0x04006F81 RID: 28545
		public AudioSource audioSource;

		// Token: 0x04006F82 RID: 28546
		public float originalVolume;
	}
}
