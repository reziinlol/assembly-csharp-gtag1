using System;
using UnityEngine;

// Token: 0x0200093B RID: 2363
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x06003DF3 RID: 15859 RVA: 0x0014EF68 File Offset: 0x0014D168
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x06003DF4 RID: 15860 RVA: 0x0014EF70 File Offset: 0x0014D170
	public float DefaultVolume
	{
		get
		{
			return this.defaultVolume;
		}
	}

	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x06003DF5 RID: 15861 RVA: 0x0014EF78 File Offset: 0x0014D178
	public bool VolumeOverridden
	{
		get
		{
			return this.volumeOverride != null;
		}
	}

	// Token: 0x06003DF6 RID: 15862 RVA: 0x0014EF85 File Offset: 0x0014D185
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (this.setDefaultVolumeFromAudioSourceOnAwake)
		{
			this.defaultVolume = this.audioSource.volume;
		}
	}

	// Token: 0x06003DF7 RID: 15863 RVA: 0x0014EFBA File Offset: 0x0014D1BA
	private void OnEnable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.RegisterMusicSource(this);
		}
	}

	// Token: 0x06003DF8 RID: 15864 RVA: 0x0014EFD8 File Offset: 0x0014D1D8
	private void OnDisable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.UnregisterMusicSource(this);
		}
	}

	// Token: 0x06003DF9 RID: 15865 RVA: 0x0014EFF6 File Offset: 0x0014D1F6
	public void SetVolumeOverride(float volume)
	{
		this.volumeOverride = new float?(volume);
		this.audioSource.volume = this.volumeOverride.Value;
	}

	// Token: 0x06003DFA RID: 15866 RVA: 0x0014F01A File Offset: 0x0014D21A
	public void UnsetVolumeOverride()
	{
		this.volumeOverride = null;
		this.audioSource.volume = this.defaultVolume;
	}

	// Token: 0x04004E36 RID: 20022
	[SerializeField]
	private float defaultVolume = 1f;

	// Token: 0x04004E37 RID: 20023
	[SerializeField]
	private bool setDefaultVolumeFromAudioSourceOnAwake = true;

	// Token: 0x04004E38 RID: 20024
	private AudioSource audioSource;

	// Token: 0x04004E39 RID: 20025
	private float? volumeOverride;
}
