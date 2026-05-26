using System;
using UnityEngine;

// Token: 0x020002AE RID: 686
public class AudioFader : MonoBehaviour
{
	// Token: 0x060011CC RID: 4556 RVA: 0x0005F3EF File Offset: 0x0005D5EF
	private void Start()
	{
		this.fadeInSpeed = this.maxVolume / this.fadeInDuration;
		this.fadeOutSpeed = this.maxVolume / this.fadeOutDuration;
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x0005F418 File Offset: 0x0005D618
	public void FadeIn()
	{
		this.targetVolume = this.maxVolume;
		if (this.fadeInDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeInSpeed;
		}
		else
		{
			this.currentVolume = this.maxVolume;
		}
		this.audioToFade.volume = this.currentVolume;
		if (!this.audioToFade.isPlaying)
		{
			this.audioToFade.GTPlay();
		}
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0005F488 File Offset: 0x0005D688
	public void FadeOut()
	{
		this.targetVolume = 0f;
		if (this.fadeOutDuration > 0f)
		{
			base.enabled = true;
			this.currentFadeSpeed = this.fadeOutSpeed;
		}
		else
		{
			this.currentVolume = 0f;
			if (this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
		if (this.outro != null && this.currentVolume > 0f)
		{
			this.outro.volume = this.currentVolume;
			this.outro.GTPlay();
		}
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x0005F51C File Offset: 0x0005D71C
	private void Update()
	{
		this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.targetVolume, this.currentFadeSpeed * Time.deltaTime);
		this.audioToFade.volume = this.currentVolume;
		if (this.currentVolume == this.targetVolume)
		{
			base.enabled = false;
			if (this.currentVolume == 0f && this.audioToFade.isPlaying)
			{
				this.audioToFade.Stop();
			}
		}
	}

	// Token: 0x04001557 RID: 5463
	[SerializeField]
	private AudioSource audioToFade;

	// Token: 0x04001558 RID: 5464
	[SerializeField]
	private AudioSource outro;

	// Token: 0x04001559 RID: 5465
	[SerializeField]
	private float fadeInDuration = 0.3f;

	// Token: 0x0400155A RID: 5466
	[SerializeField]
	private float fadeOutDuration = 0.3f;

	// Token: 0x0400155B RID: 5467
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x0400155C RID: 5468
	private float currentVolume;

	// Token: 0x0400155D RID: 5469
	private float targetVolume;

	// Token: 0x0400155E RID: 5470
	private float currentFadeSpeed;

	// Token: 0x0400155F RID: 5471
	private float fadeInSpeed;

	// Token: 0x04001560 RID: 5472
	private float fadeOutSpeed;
}
