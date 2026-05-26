using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000938 RID: 2360
public class MusicManager : MonoBehaviour
{
	// Token: 0x06003DDD RID: 15837 RVA: 0x0014EAE7 File Offset: 0x0014CCE7
	private void Awake()
	{
		if (MusicManager.Instance == null)
		{
			MusicManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003DDE RID: 15838 RVA: 0x0014EB07 File Offset: 0x0014CD07
	public void RegisterMusicSource(MusicSource musicSource)
	{
		if (!this.activeSources.Contains(musicSource))
		{
			this.activeSources.Add(musicSource);
		}
	}

	// Token: 0x06003DDF RID: 15839 RVA: 0x0014EB24 File Offset: 0x0014CD24
	public void UnregisterMusicSource(MusicSource musicSource)
	{
		if (this.activeSources.Contains(musicSource))
		{
			this.activeSources.Remove(musicSource);
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06003DE0 RID: 15840 RVA: 0x0014EB48 File Offset: 0x0014CD48
	public void FadeOutMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeOutVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.SetVolumeOverride(0f);
		}
	}

	// Token: 0x06003DE1 RID: 15841 RVA: 0x0014EBBC File Offset: 0x0014CDBC
	public void FadeInMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeInVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06003DE2 RID: 15842 RVA: 0x0014EC2C File Offset: 0x0014CE2C
	private IEnumerator FadeInVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float volumeOverride = Mathf.MoveTowards(musicSource.AudioSource.volume, musicSource.DefaultVolume, num * deltaTime);
				musicSource.SetVolumeOverride(volumeOverride);
				if (musicSource.AudioSource.volume != musicSource.DefaultVolume)
				{
					complete = false;
				}
			}
			yield return null;
		}
		using (HashSet<MusicSource>.Enumerator enumerator = this.activeSources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MusicSource musicSource2 = enumerator.Current;
				musicSource2.UnsetVolumeOverride();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06003DE3 RID: 15843 RVA: 0x0014EC42 File Offset: 0x0014CE42
	private IEnumerator FadeOutVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float volumeOverride = Mathf.MoveTowards(musicSource.AudioSource.volume, 0f, num * deltaTime);
				musicSource.SetVolumeOverride(volumeOverride);
				if (musicSource.AudioSource.volume != 0f)
				{
					complete = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003DE4 RID: 15844 RVA: 0x0014EC58 File Offset: 0x0014CE58
	public static void StopAllMusic()
	{
		MusicManager.StopAllMusic(null);
	}

	// Token: 0x06003DE5 RID: 15845 RVA: 0x0014EC60 File Offset: 0x0014CE60
	public static void StopAllMusic(AudioClip clip)
	{
		if (MusicManager.Instance == null)
		{
			return;
		}
		MusicManager.Instance.StopAllCoroutines();
		foreach (MusicSource musicSource in MusicManager.Instance.activeSources)
		{
			musicSource.UnsetVolumeOverride();
			musicSource.AudioSource.Stop();
			if (clip != null)
			{
				musicSource.AudioSource.PlayOneShot(clip);
			}
		}
	}

	// Token: 0x04004E2A RID: 20010
	[OnEnterPlay_SetNull]
	public static volatile MusicManager Instance;

	// Token: 0x04004E2B RID: 20011
	private HashSet<MusicSource> activeSources = new HashSet<MusicSource>();
}
