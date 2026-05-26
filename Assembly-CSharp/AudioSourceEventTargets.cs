using System;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class AudioSourceEventTargets : MonoBehaviour
{
	// Token: 0x0600004B RID: 75 RVA: 0x00002BB9 File Offset: 0x00000DB9
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.fadeVolume = this.audioSource.volume;
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00002BD8 File Offset: 0x00000DD8
	public void SetFadeSpeed(float arg)
	{
		this.fadeSpeed = Mathf.Max(arg, 0.01f);
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00002BEB File Offset: 0x00000DEB
	public void StartFade(float arg)
	{
		this.fadeVolume = Mathf.Clamp01(arg);
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00002BFC File Offset: 0x00000DFC
	public void Update()
	{
		if (this.audioSource.volume != this.fadeVolume)
		{
			this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, this.fadeVolume, this.fadeSpeed * Time.deltaTime);
		}
		if (this.lastValueWhenPlayed != this.ExternalTriggerPlay)
		{
			if (!this.lastExternalTriggerPlayMatched)
			{
				this.audioSource.Play();
				this.lastValueWhenPlayed = this.ExternalTriggerPlay;
				this.lastExternalTriggerPlayMatched = true;
			}
			else
			{
				this.ExternalTriggerPlay = this.lastValueWhenPlayed;
				this.lastExternalTriggerPlayMatched = false;
			}
		}
		else
		{
			this.lastExternalTriggerPlayMatched = true;
		}
		if (this.lastValueWhenStopped == this.ExternalTriggerStop)
		{
			this.lastExternalTriggerStopMatched = true;
			return;
		}
		if (!this.lastExternalTriggerStopMatched)
		{
			this.audioSource.Stop();
			this.lastValueWhenStopped = this.ExternalTriggerStop;
			this.lastExternalTriggerStopMatched = true;
			return;
		}
		this.ExternalTriggerStop = this.lastValueWhenStopped;
		this.lastExternalTriggerStopMatched = false;
	}

	// Token: 0x0400002B RID: 43
	private AudioSource audioSource;

	// Token: 0x0400002C RID: 44
	private float fadeVolume;

	// Token: 0x0400002D RID: 45
	private float fadeSpeed;

	// Token: 0x0400002E RID: 46
	[Header("Change Value To Trigger Play (false to true and true to false both work, but value must change the frame you want it played)")]
	public bool ExternalTriggerPlay;

	// Token: 0x0400002F RID: 47
	private bool lastExternalTriggerPlayMatched = true;

	// Token: 0x04000030 RID: 48
	private bool lastValueWhenPlayed;

	// Token: 0x04000031 RID: 49
	[Header("Change Value To Trigger Stop (false to true and true to false both work, but value must change the frame you want it stopped)")]
	public bool ExternalTriggerStop;

	// Token: 0x04000032 RID: 50
	private bool lastExternalTriggerStopMatched = true;

	// Token: 0x04000033 RID: 51
	private bool lastValueWhenStopped;
}
