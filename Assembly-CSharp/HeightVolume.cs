using System;
using UnityEngine;

// Token: 0x020008E3 RID: 2275
public class HeightVolume : MonoBehaviour
{
	// Token: 0x06003B85 RID: 15237 RVA: 0x00145FDD File Offset: 0x001441DD
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
		this.musicSource = this.audioSource.gameObject.GetComponent<MusicSource>();
	}

	// Token: 0x06003B86 RID: 15238 RVA: 0x00146014 File Offset: 0x00144214
	private void Update()
	{
		if (this.audioSource.gameObject.activeSelf && (!(this.musicSource != null) || !this.musicSource.VolumeOverridden))
		{
			if (this.targetTransform.position.y > this.heightTop.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.baseVolume : this.minVolume);
				return;
			}
			if (this.targetTransform.position.y < this.heightBottom.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.minVolume : this.baseVolume);
				return;
			}
			this.audioSource.volume = ((!this.invertHeightVol) ? ((this.targetTransform.position.y - this.heightBottom.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume) : ((this.heightTop.position.y - this.targetTransform.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume));
		}
	}

	// Token: 0x04004C0E RID: 19470
	public Transform heightTop;

	// Token: 0x04004C0F RID: 19471
	public Transform heightBottom;

	// Token: 0x04004C10 RID: 19472
	public AudioSource audioSource;

	// Token: 0x04004C11 RID: 19473
	public float baseVolume;

	// Token: 0x04004C12 RID: 19474
	public float minVolume;

	// Token: 0x04004C13 RID: 19475
	public Transform targetTransform;

	// Token: 0x04004C14 RID: 19476
	public bool invertHeightVol;

	// Token: 0x04004C15 RID: 19477
	private MusicSource musicSource;
}
