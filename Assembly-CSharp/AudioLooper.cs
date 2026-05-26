using System;
using UnityEngine;

// Token: 0x02000D23 RID: 3363
[RequireComponent(typeof(AudioSource))]
public class AudioLooper : MonoBehaviour
{
	// Token: 0x06005307 RID: 21255 RVA: 0x001B2FB0 File Offset: 0x001B11B0
	protected virtual void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06005308 RID: 21256 RVA: 0x001B2FC0 File Offset: 0x001B11C0
	private void Update()
	{
		if (!this.audioSource.isPlaying)
		{
			if (this.audioSource.clip == this.loopClip && this.interjectionClips.Length != 0 && Random.value < this.interjectionLikelyhood)
			{
				this.audioSource.clip = this.interjectionClips[Random.Range(0, this.interjectionClips.Length)];
			}
			else
			{
				this.audioSource.clip = this.loopClip;
			}
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x04006456 RID: 25686
	private AudioSource audioSource;

	// Token: 0x04006457 RID: 25687
	[SerializeField]
	private AudioClip loopClip;

	// Token: 0x04006458 RID: 25688
	[SerializeField]
	private AudioClip[] interjectionClips;

	// Token: 0x04006459 RID: 25689
	[SerializeField]
	private float interjectionLikelyhood = 0.5f;
}
