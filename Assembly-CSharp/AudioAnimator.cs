using System;
using UnityEngine;

// Token: 0x020002A8 RID: 680
public class AudioAnimator : MonoBehaviour
{
	// Token: 0x060011B5 RID: 4533 RVA: 0x0005EDAA File Offset: 0x0005CFAA
	private void Start()
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x0005EDBC File Offset: 0x0005CFBC
	private void InitBaseVolume()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			this.targets[i].baseVolume = this.targets[i].audioSource.volume;
		}
		this.didInitBaseVolume = true;
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0005EE0A File Offset: 0x0005D00A
	public void UpdateValue(float value, bool ignoreSmoothing = false)
	{
		this.UpdatePitchAndVolume(value, value, ignoreSmoothing);
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x0005EE18 File Offset: 0x0005D018
	public void UpdatePitchAndVolume(float pitchValue, float volumeValue, bool ignoreSmoothing = false)
	{
		if (!this.didInitBaseVolume)
		{
			this.InitBaseVolume();
		}
		for (int i = 0; i < this.targets.Length; i++)
		{
			AudioAnimator.AudioTarget audioTarget = this.targets[i];
			float p = audioTarget.pitchCurve.Evaluate(pitchValue);
			float pitch = Mathf.Pow(1.05946f, p);
			audioTarget.audioSource.pitch = pitch;
			float num = audioTarget.volumeCurve.Evaluate(volumeValue);
			float volume = audioTarget.audioSource.volume;
			float num2 = audioTarget.baseVolume * num;
			if (ignoreSmoothing)
			{
				audioTarget.audioSource.volume = num2;
			}
			else if (volume > num2)
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num, (1f - audioTarget.lowerSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
			else
			{
				audioTarget.audioSource.volume = Mathf.MoveTowards(audioTarget.audioSource.volume, audioTarget.baseVolume * num, (1f - audioTarget.riseSmoothing) * audioTarget.baseVolume * Time.deltaTime * 90f);
			}
		}
	}

	// Token: 0x0400153C RID: 5436
	private bool didInitBaseVolume;

	// Token: 0x0400153D RID: 5437
	[SerializeField]
	private AudioAnimator.AudioTarget[] targets;

	// Token: 0x020002A9 RID: 681
	[Serializable]
	private struct AudioTarget
	{
		// Token: 0x0400153E RID: 5438
		public AudioSource audioSource;

		// Token: 0x0400153F RID: 5439
		public AnimationCurve pitchCurve;

		// Token: 0x04001540 RID: 5440
		public AnimationCurve volumeCurve;

		// Token: 0x04001541 RID: 5441
		[NonSerialized]
		public float baseVolume;

		// Token: 0x04001542 RID: 5442
		public float riseSmoothing;

		// Token: 0x04001543 RID: 5443
		public float lowerSmoothing;
	}
}
