using System;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class PlayAudioOnEnable : MonoBehaviour
{
	// Token: 0x060007BA RID: 1978 RVA: 0x0002A5A3 File Offset: 0x000287A3
	private void OnEnable()
	{
		this.audioSource.clip = this.audioClips[Random.Range(0, this.audioClips.Length)];
		this.audioSource.GTPlay();
	}

	// Token: 0x040009C8 RID: 2504
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040009C9 RID: 2505
	[SerializeField]
	private AudioClip[] audioClips;
}
