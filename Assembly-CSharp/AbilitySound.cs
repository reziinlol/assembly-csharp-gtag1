using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000719 RID: 1817
[Serializable]
public class AbilitySound
{
	// Token: 0x06002E1F RID: 11807 RVA: 0x000FCB9F File Offset: 0x000FAD9F
	public bool IsValid()
	{
		return this.sounds != null && this.sounds.Count > 0;
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x000FCBBC File Offset: 0x000FADBC
	public void UpdateNextSound()
	{
		AbilitySound.SoundSelectMode soundSelectMode = this.soundSelectMode;
		if (soundSelectMode == AbilitySound.SoundSelectMode.Sequential)
		{
			this.nextSound = (this.nextSound + 1) % this.sounds.Count;
			return;
		}
		if (soundSelectMode != AbilitySound.SoundSelectMode.Random)
		{
			return;
		}
		this.nextSound = Random.Range(0, this.sounds.Count);
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x000FCC0C File Offset: 0x000FAE0C
	public void Play(AudioSource audioSourceIn)
	{
		this.usedAudioSource = ((audioSourceIn != null) ? audioSourceIn : this.audioSource);
		if (this.sounds != null && this.sounds.Count > 0 && this.usedAudioSource != null)
		{
			if (this.nextSound < 0)
			{
				this.UpdateNextSound();
			}
			AudioClip audioClip = this.sounds[this.nextSound];
			this.UpdateNextSound();
			if (audioClip != null)
			{
				this.usedAudioSource.clip = audioClip;
				this.usedAudioSource.volume = this.volume;
				this.usedAudioSource.pitch = this.pitch;
				this.usedAudioSource.loop = this.loop;
				if (this.delay <= 0f)
				{
					this.usedAudioSource.Play();
				}
				else
				{
					this.usedAudioSource.PlayDelayed(this.delay);
				}
				this.currentSound = audioClip;
			}
		}
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x000FCD00 File Offset: 0x000FAF00
	public void Stop()
	{
		if (this.usedAudioSource != null && this.usedAudioSource.clip == this.currentSound)
		{
			this.usedAudioSource.Stop();
			this.currentSound = null;
			this.usedAudioSource = null;
		}
	}

	// Token: 0x04003B33 RID: 15155
	public float volume = 1f;

	// Token: 0x04003B34 RID: 15156
	public float pitch = 1f;

	// Token: 0x04003B35 RID: 15157
	public bool loop;

	// Token: 0x04003B36 RID: 15158
	public float delay;

	// Token: 0x04003B37 RID: 15159
	public List<AudioClip> sounds;

	// Token: 0x04003B38 RID: 15160
	private AudioClip currentSound;

	// Token: 0x04003B39 RID: 15161
	public AudioSource audioSource;

	// Token: 0x04003B3A RID: 15162
	private AudioSource usedAudioSource;

	// Token: 0x04003B3B RID: 15163
	private int nextSound = -1;

	// Token: 0x04003B3C RID: 15164
	public AbilitySound.SoundSelectMode soundSelectMode;

	// Token: 0x0200071A RID: 1818
	public enum SoundSelectMode
	{
		// Token: 0x04003B3E RID: 15166
		Sequential,
		// Token: 0x04003B3F RID: 15167
		Random
	}
}
