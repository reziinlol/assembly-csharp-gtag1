using System;
using UnityEngine;

// Token: 0x020008EB RID: 2283
public class HoverboardAudio : MonoBehaviour
{
	// Token: 0x06003BBA RID: 15290 RVA: 0x0014718F File Offset: 0x0014538F
	private void Start()
	{
		this.Stop();
	}

	// Token: 0x06003BBB RID: 15291 RVA: 0x00147197 File Offset: 0x00145397
	public void PlayTurnSound(float angle)
	{
		if (Time.time > this.turnSoundCooldownUntilTimestamp && angle > this.minAngleDeltaForTurnSound)
		{
			this.turnSoundCooldownUntilTimestamp = Time.time + this.turnSoundCooldownDuration;
			this.turnSounds.Play();
		}
	}

	// Token: 0x06003BBC RID: 15292 RVA: 0x001471CC File Offset: 0x001453CC
	public void UpdateAudioLoop(float speed, float airspeed, float strainLevel, float grindLevel)
	{
		this.motorAnimator.UpdateValue(speed, false);
		this.windRushAnimator.UpdateValue(airspeed, false);
		if (grindLevel > 0f)
		{
			this.grindAnimator.UpdatePitchAndVolume(speed, grindLevel + 0.5f, false);
		}
		else
		{
			this.grindAnimator.UpdatePitchAndVolume(0f, 0f, false);
		}
		strainLevel = Mathf.Clamp01(strainLevel * 10f);
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = Mathf.MoveTowards(this.hum1.volume, this.hum1BaseVolume * strainLevel, this.fadeSpeed * Time.deltaTime);
	}

	// Token: 0x06003BBD RID: 15293 RVA: 0x00147288 File Offset: 0x00145488
	public void Stop()
	{
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = 0f;
		this.windRushAnimator.UpdateValue(0f, true);
		this.motorAnimator.UpdateValue(0f, true);
		this.grindAnimator.UpdateValue(0f, true);
	}

	// Token: 0x04004C4D RID: 19533
	[SerializeField]
	private AudioSource hum1;

	// Token: 0x04004C4E RID: 19534
	[SerializeField]
	private SoundBankPlayer turnSounds;

	// Token: 0x04004C4F RID: 19535
	private bool didInitHum1BaseVolume;

	// Token: 0x04004C50 RID: 19536
	private float hum1BaseVolume;

	// Token: 0x04004C51 RID: 19537
	[SerializeField]
	private float fadeSpeed;

	// Token: 0x04004C52 RID: 19538
	[SerializeField]
	private AudioAnimator windRushAnimator;

	// Token: 0x04004C53 RID: 19539
	[SerializeField]
	private AudioAnimator motorAnimator;

	// Token: 0x04004C54 RID: 19540
	[SerializeField]
	private AudioAnimator grindAnimator;

	// Token: 0x04004C55 RID: 19541
	[SerializeField]
	private float turnSoundCooldownDuration;

	// Token: 0x04004C56 RID: 19542
	[SerializeField]
	private float minAngleDeltaForTurnSound;

	// Token: 0x04004C57 RID: 19543
	private float turnSoundCooldownUntilTimestamp;
}
