using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000D7A RID: 3450
public class SoundBankPlayer : MonoBehaviour
{
	// Token: 0x170007FA RID: 2042
	// (get) Token: 0x06005499 RID: 21657 RVA: 0x001B911C File Offset: 0x001B731C
	public bool isPlaying
	{
		get
		{
			return Time.realtimeSinceStartup < this.playEndTime;
		}
	}

	// Token: 0x170007FB RID: 2043
	// (get) Token: 0x0600549A RID: 21658 RVA: 0x001B912B File Offset: 0x001B732B
	public float NormalizedTime
	{
		get
		{
			if (this.clipDuration != 0f)
			{
				return Mathf.Clamp01(this.CurrentTime / this.clipDuration);
			}
			return 1f;
		}
	}

	// Token: 0x170007FC RID: 2044
	// (get) Token: 0x0600549B RID: 21659 RVA: 0x001B9152 File Offset: 0x001B7352
	public float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup - this.playStartTime;
		}
	}

	// Token: 0x0600549C RID: 21660 RVA: 0x001B9160 File Offset: 0x001B7360
	protected void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.outputAudioMixerGroup = this.outputAudioMixerGroup;
			this.audioSource.spatialize = this.spatialize;
			this.audioSource.spatializePostEffects = this.spatializePostEffects;
			this.audioSource.bypassEffects = this.bypassEffects;
			this.audioSource.bypassListenerEffects = this.bypassListenerEffects;
			this.audioSource.bypassReverbZones = this.bypassReverbZones;
			this.audioSource.priority = this.priority;
			this.audioSource.spatialBlend = this.spatialBlend;
			this.audioSource.dopplerLevel = this.dopplerLevel;
			this.audioSource.spread = this.spread;
			this.audioSource.rolloffMode = this.rolloffMode;
			this.audioSource.minDistance = this.minDistance;
			this.audioSource.maxDistance = this.maxDistance;
			this.audioSource.reverbZoneMix = this.reverbZoneMix;
		}
		this.audioSource.volume = 1f;
		this.audioSource.playOnAwake = false;
		if (this.shuffleOrder)
		{
			int[] array = new int[this.soundBank.sounds.Length / 2];
			this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
			for (int i = 0; i < this.playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = Random.Range(0, this.soundBank.sounds.Length);
					if (Array.IndexOf<int>(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				this.playlist[i] = new SoundBankPlayer.PlaylistEntry
				{
					index = num,
					volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
					pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
				};
			}
			return;
		}
		this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
		for (int k = 0; k < this.playlist.Length; k++)
		{
			this.playlist[k] = new SoundBankPlayer.PlaylistEntry
			{
				index = k % this.soundBank.sounds.Length,
				volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
				pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
			};
		}
	}

	// Token: 0x0600549D RID: 21661 RVA: 0x001B9459 File Offset: 0x001B7659
	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x0600549E RID: 21662 RVA: 0x001B946C File Offset: 0x001B766C
	public void Play()
	{
		this.Play(null, null);
	}

	// Token: 0x0600549F RID: 21663 RVA: 0x001B9494 File Offset: 0x001B7694
	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (!base.enabled || this.soundBank.sounds.Length == 0 || this.playlist == null)
		{
			return;
		}
		SoundBankPlayer.PlaylistEntry playlistEntry = this.playlist[this.nextIndex];
		this.audioSource.pitch = ((pitchOverride != null) ? pitchOverride.Value : playlistEntry.pitch);
		AudioClip audioClip = this.soundBank.sounds[playlistEntry.index];
		if (audioClip != null)
		{
			this.audioSource.GTPlayOneShot(audioClip, (volumeOverride != null) ? volumeOverride.Value : playlistEntry.volume);
			this.clipDuration = audioClip.length;
			this.playStartTime = Time.realtimeSinceStartup;
			this.playEndTime = Mathf.Max(this.playEndTime, this.playStartTime + audioClip.length);
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		if (this.missingSoundsAreOk)
		{
			this.clipDuration = 0f;
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		Debug.LogErrorFormat("Sounds bank {0} is missing a clip at {1}", new object[]
		{
			base.gameObject.name,
			playlistEntry.index
		});
	}

	// Token: 0x060054A0 RID: 21664 RVA: 0x001B95D9 File Offset: 0x001B77D9
	public void RestartSequence()
	{
		this.nextIndex = 0;
	}

	// Token: 0x0400652F RID: 25903
	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	// Token: 0x04006530 RID: 25904
	public bool playOnEnable = true;

	// Token: 0x04006531 RID: 25905
	public bool shuffleOrder = true;

	// Token: 0x04006532 RID: 25906
	public bool missingSoundsAreOk;

	// Token: 0x04006533 RID: 25907
	public SoundBankSO soundBank;

	// Token: 0x04006534 RID: 25908
	public AudioMixerGroup outputAudioMixerGroup;

	// Token: 0x04006535 RID: 25909
	public bool spatialize;

	// Token: 0x04006536 RID: 25910
	public bool spatializePostEffects;

	// Token: 0x04006537 RID: 25911
	public bool bypassEffects;

	// Token: 0x04006538 RID: 25912
	public bool bypassListenerEffects;

	// Token: 0x04006539 RID: 25913
	public bool bypassReverbZones;

	// Token: 0x0400653A RID: 25914
	public int priority = 128;

	// Token: 0x0400653B RID: 25915
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x0400653C RID: 25916
	public float reverbZoneMix = 1f;

	// Token: 0x0400653D RID: 25917
	public float dopplerLevel = 1f;

	// Token: 0x0400653E RID: 25918
	public float spread;

	// Token: 0x0400653F RID: 25919
	public AudioRolloffMode rolloffMode;

	// Token: 0x04006540 RID: 25920
	public float minDistance = 1f;

	// Token: 0x04006541 RID: 25921
	public float maxDistance = 100f;

	// Token: 0x04006542 RID: 25922
	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04006543 RID: 25923
	private int nextIndex;

	// Token: 0x04006544 RID: 25924
	private float playStartTime;

	// Token: 0x04006545 RID: 25925
	private float playEndTime;

	// Token: 0x04006546 RID: 25926
	private float clipDuration;

	// Token: 0x04006547 RID: 25927
	private SoundBankPlayer.PlaylistEntry[] playlist;

	// Token: 0x02000D7B RID: 3451
	private struct PlaylistEntry
	{
		// Token: 0x04006548 RID: 25928
		public int index;

		// Token: 0x04006549 RID: 25929
		public float volume;

		// Token: 0x0400654A RID: 25930
		public float pitch;
	}
}
