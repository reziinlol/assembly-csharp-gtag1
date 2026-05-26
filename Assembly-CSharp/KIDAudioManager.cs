using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000B35 RID: 2869
[DefaultExecutionOrder(0)]
public class KIDAudioManager : MonoBehaviour
{
	// Token: 0x170006D6 RID: 1750
	// (get) Token: 0x060048A5 RID: 18597 RVA: 0x0018432D File Offset: 0x0018252D
	public static KIDAudioManager Instance
	{
		get
		{
			if (!KIDAudioManager._instance)
			{
				if (!ApplicationQuittingState.IsQuitting)
				{
					Debug.LogError("No KIDAudioManager instance found in scene!");
				}
				return null;
			}
			return KIDAudioManager._instance;
		}
	}

	// Token: 0x060048A6 RID: 18598 RVA: 0x00184354 File Offset: 0x00182554
	private void Awake()
	{
		if (KIDAudioManager._instance == null)
		{
			KIDAudioManager._instance = this;
			base.transform.parent = null;
			Object.DontDestroyOnLoad(base.gameObject);
			this.ConfigureAudioSource();
			this.InitializeSoundClips();
			this.mainMixer.GetFloat("Game_Volume", out this.cachedGameVolume);
			return;
		}
		if (KIDAudioManager._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060048A7 RID: 18599 RVA: 0x001843C8 File Offset: 0x001825C8
	private void ConfigureAudioSource()
	{
		if (this.audioSource != null)
		{
			this.audioSource.outputAudioMixerGroup = this.kidUIGroup;
			this.audioSource.playOnAwake = false;
			this.audioSource.spatialBlend = 0f;
			this.audioSource.volume = 1f;
			this.audioSource.enabled = true;
		}
		if (this.loopingAudioSource != null)
		{
			this.loopingAudioSource.outputAudioMixerGroup = this.kidUIGroup;
			this.loopingAudioSource.playOnAwake = false;
			this.loopingAudioSource.spatialBlend = 0f;
			this.loopingAudioSource.volume = 1f;
			this.loopingAudioSource.loop = true;
			this.loopingAudioSource.enabled = true;
		}
	}

	// Token: 0x060048A8 RID: 18600 RVA: 0x00184490 File Offset: 0x00182690
	private void InitializeSoundClips()
	{
		this.soundClips = new Dictionary<KIDAudioManager.KIDSoundType, AudioClip>
		{
			{
				KIDAudioManager.KIDSoundType.ButtonClick,
				this.buttonClickSound
			},
			{
				KIDAudioManager.KIDSoundType.Denied,
				this.deniedSound
			},
			{
				KIDAudioManager.KIDSoundType.Success,
				this.successSound
			},
			{
				KIDAudioManager.KIDSoundType.Hover,
				this.buttonHoverSound
			},
			{
				KIDAudioManager.KIDSoundType.ButtonHeld,
				this.buttonHeldSound
			},
			{
				KIDAudioManager.KIDSoundType.PageTransition,
				this.pageTransitionSound
			},
			{
				KIDAudioManager.KIDSoundType.InputBack,
				this.inputBackSound
			},
			{
				KIDAudioManager.KIDSoundType.TurnOffPermission,
				this.turnOffPermissionSound
			}
		};
	}

	// Token: 0x060048A9 RID: 18601 RVA: 0x00184510 File Offset: 0x00182710
	public void SetKIDUIAudioActive(bool active)
	{
		if (!this.IsInstanceValid() || this.isKIDUIActive == active)
		{
			return;
		}
		this.isKIDUIActive = active;
		if (!active)
		{
			this.StopButtonHeldSound();
		}
		if (active)
		{
			this.KIDSnapshot.TransitionTo(0f);
			return;
		}
		this.normalSnapshot.TransitionTo(0f);
	}

	// Token: 0x060048AA RID: 18602 RVA: 0x00184564 File Offset: 0x00182764
	public void PlaySound(KIDAudioManager.KIDSoundType soundType)
	{
		if (!this.IsInstanceValid())
		{
			return;
		}
		if (soundType == KIDAudioManager.KIDSoundType.ButtonHeld)
		{
			Debug.LogWarning("[KIDAudioManager] Button held sound is already playing, skipping delayed sound.");
			return;
		}
		AudioClip audioClip;
		if (this.soundClips.TryGetValue(soundType, out audioClip) && audioClip != null)
		{
			this.audioSource.PlayOneShot(audioClip);
			return;
		}
		Debug.LogWarning(string.Format("[KIDAudioManager] Sound clip for {0} is null or not found!", soundType));
	}

	// Token: 0x060048AB RID: 18603 RVA: 0x001845C4 File Offset: 0x001827C4
	public void StartButtonHeldSound()
	{
		if (!this.IsInstanceValid() || this.buttonHeldSound == null || this.isHoldSoundPlaying)
		{
			return;
		}
		this.loopingAudioSource.clip = this.buttonHeldSound;
		this.loopingAudioSource.Play();
		this.isHoldSoundPlaying = true;
	}

	// Token: 0x060048AC RID: 18604 RVA: 0x00184613 File Offset: 0x00182813
	public void StopButtonHeldSound()
	{
		if (!this.IsInstanceValid() || !this.isHoldSoundPlaying)
		{
			return;
		}
		if (this.loopingAudioSource.clip == this.buttonHeldSound)
		{
			this.loopingAudioSource.Stop();
		}
		this.isHoldSoundPlaying = false;
	}

	// Token: 0x060048AD RID: 18605 RVA: 0x00184650 File Offset: 0x00182850
	private bool IsInstanceValid()
	{
		return !(KIDAudioManager._instance == null) && !(KIDAudioManager._instance != this) && !(this.audioSource == null) && !(this.loopingAudioSource == null);
	}

	// Token: 0x060048AE RID: 18606 RVA: 0x0018468B File Offset: 0x0018288B
	public bool IsKIDUIActive()
	{
		return this.isKIDUIActive;
	}

	// Token: 0x060048AF RID: 18607 RVA: 0x00184693 File Offset: 0x00182893
	public void PlaySoundWithDelay(KIDAudioManager.KIDSoundType soundType)
	{
		base.StartCoroutine(this.PlayDelayedSound(soundType, 0.05f));
	}

	// Token: 0x060048B0 RID: 18608 RVA: 0x001846A8 File Offset: 0x001828A8
	private IEnumerator PlayDelayedSound(KIDAudioManager.KIDSoundType soundType, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlaySound(soundType);
		yield break;
	}

	// Token: 0x04005B05 RID: 23301
	private static KIDAudioManager _instance;

	// Token: 0x04005B06 RID: 23302
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04005B07 RID: 23303
	[SerializeField]
	private AudioSource loopingAudioSource;

	// Token: 0x04005B08 RID: 23304
	[SerializeField]
	private AudioMixer mainMixer;

	// Token: 0x04005B09 RID: 23305
	[SerializeField]
	private AudioMixerSnapshot KIDSnapshot;

	// Token: 0x04005B0A RID: 23306
	[SerializeField]
	private AudioMixerSnapshot normalSnapshot;

	// Token: 0x04005B0B RID: 23307
	[SerializeField]
	private AudioMixerGroup kidUIGroup;

	// Token: 0x04005B0C RID: 23308
	[SerializeField]
	private AudioClip buttonClickSound;

	// Token: 0x04005B0D RID: 23309
	[SerializeField]
	private AudioClip deniedSound;

	// Token: 0x04005B0E RID: 23310
	[SerializeField]
	private AudioClip successSound;

	// Token: 0x04005B0F RID: 23311
	[SerializeField]
	private AudioClip buttonHoverSound;

	// Token: 0x04005B10 RID: 23312
	[SerializeField]
	private AudioClip buttonHeldSound;

	// Token: 0x04005B11 RID: 23313
	[SerializeField]
	private AudioClip pageTransitionSound;

	// Token: 0x04005B12 RID: 23314
	[SerializeField]
	private AudioClip inputBackSound;

	// Token: 0x04005B13 RID: 23315
	[SerializeField]
	private AudioClip turnOffPermissionSound;

	// Token: 0x04005B14 RID: 23316
	private const string GAME_VOLUME = "Game_Volume";

	// Token: 0x04005B15 RID: 23317
	private const string KID_VOLUME = "KID_UI_Volume";

	// Token: 0x04005B16 RID: 23318
	private const float MUTED_VALUE = -80f;

	// Token: 0x04005B17 RID: 23319
	private const float UNMUTED_VALUE = 0f;

	// Token: 0x04005B18 RID: 23320
	private bool isKIDUIActive;

	// Token: 0x04005B19 RID: 23321
	private float cachedGameVolume;

	// Token: 0x04005B1A RID: 23322
	private bool isHoldSoundPlaying;

	// Token: 0x04005B1B RID: 23323
	private Dictionary<KIDAudioManager.KIDSoundType, AudioClip> soundClips;

	// Token: 0x02000B36 RID: 2870
	public enum KIDSoundType
	{
		// Token: 0x04005B1D RID: 23325
		ButtonClick,
		// Token: 0x04005B1E RID: 23326
		Hover,
		// Token: 0x04005B1F RID: 23327
		Success,
		// Token: 0x04005B20 RID: 23328
		Denied,
		// Token: 0x04005B21 RID: 23329
		InputBack,
		// Token: 0x04005B22 RID: 23330
		TurnOffPermission,
		// Token: 0x04005B23 RID: 23331
		PageTransition,
		// Token: 0x04005B24 RID: 23332
		ButtonHeld
	}
}
