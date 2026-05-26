using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008CD RID: 2253
public class GRAdaptiveMusicController : MonoBehaviour
{
	// Token: 0x06003AE0 RID: 15072 RVA: 0x0014343E File Offset: 0x0014163E
	private void Start()
	{
		this.cachedSourcePosition = this.AudioSources[0].transform.position;
	}

	// Token: 0x06003AE1 RID: 15073 RVA: 0x0014345C File Offset: 0x0014165C
	private void PlayCurrentTrack()
	{
		if (this.trackIndex < 0 || this.trackIndex >= this.Tracks.Count)
		{
			return;
		}
		GRAdaptiveMusicController.SingleTrack singleTrack = this.Tracks[this.trackIndex];
		AudioSource currentAudioSource = this.GetCurrentAudioSource();
		currentAudioSource.clip = singleTrack.IntroClip;
		currentAudioSource.Play();
		AudioSource nextAudioSource = this.GetNextAudioSource();
		nextAudioSource.clip = singleTrack.LoopedClip;
		nextAudioSource.loop = true;
		double num = AudioSettings.dspTime + (double)singleTrack.IntroClip.length;
		currentAudioSource.SetScheduledEndTime(num);
		nextAudioSource.PlayScheduled(num);
		this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		this.CurrentTrack = singleTrack;
	}

	// Token: 0x06003AE2 RID: 15074 RVA: 0x001434FD File Offset: 0x001416FD
	[ContextMenu("Transition Next Track")]
	public void TransitionToNextTrack()
	{
		this.GoToTrack(this.trackIndex + 1, false);
	}

	// Token: 0x06003AE3 RID: 15075 RVA: 0x0014350E File Offset: 0x0014170E
	public void TransitionToLastTrack()
	{
		this.GoToTrack(this.Tracks.Count - 1, false);
	}

	// Token: 0x06003AE4 RID: 15076 RVA: 0x00143524 File Offset: 0x00141724
	public void GoToTrack(int nextIndex, bool force = false)
	{
		if (!force && (nextIndex < 0 || nextIndex >= this.Tracks.Count || this.trackIndex == nextIndex))
		{
			return;
		}
		Debug.Log(string.Format("GRAdaptiveMusicController - Going to track {0}.", nextIndex));
		GRAdaptiveMusicController.SingleTrack singleTrack = this.Tracks[nextIndex];
		AudioSource audioSource = this.GetCurrentAudioSource();
		AudioSource nextAudioSource = this.GetNextAudioSource();
		double num = (double)audioSource.timeSamples / (double)audioSource.clip.frequency % GRAdaptiveMusicController.BAR_DURATION;
		double num2 = AudioSettings.dspTime + (GRAdaptiveMusicController.BAR_DURATION - num);
		nextAudioSource.Stop();
		nextAudioSource.clip = singleTrack.IntroClip;
		nextAudioSource.loop = false;
		audioSource.SetScheduledEndTime(num2);
		nextAudioSource.PlayScheduled(num2);
		this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		if (singleTrack.LoopedClip != null)
		{
			audioSource = nextAudioSource;
			nextAudioSource = this.GetNextAudioSource();
			nextAudioSource.clip = singleTrack.LoopedClip;
			nextAudioSource.loop = true;
			double num3 = num2 + (double)singleTrack.IntroClip.length;
			audioSource.SetScheduledEndTime(num3);
			nextAudioSource.PlayScheduled(num3);
			this.currentAudioSourceIndex = this.NextAudioSourceIndex;
		}
		else
		{
			this.Finish(singleTrack.IntroClip.length + 1f);
		}
		this.trackIndex = nextIndex;
		this.CurrentTrack = singleTrack;
	}

	// Token: 0x06003AE5 RID: 15077 RVA: 0x00143660 File Offset: 0x00141860
	[ContextMenu("Restart")]
	public void Restart()
	{
		Debug.Log("Restarting AdaptiveMusicController.");
		this.cachedSourceVolume = this.AudioSources[0].volume;
		this.synchedMusicController.enabled = false;
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.AdjustedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.RepositionAudioSourcePoint.position);
		}
		this.trackIndex = 0;
		this.currentAudioSourceIndex = 0;
		this.PlayCurrentTrack();
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x001436E0 File Offset: 0x001418E0
	public void RestartAt(int index)
	{
		Debug.Log(string.Format("Restarting AdaptiveMusicController at index {0}.", index));
		this.cachedSourceVolume = this.AudioSources[0].volume;
		this.synchedMusicController.enabled = false;
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.AdjustedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.RepositionAudioSourcePoint.position);
		}
		this.trackIndex = index;
		this.currentAudioSourceIndex = 0;
		this.GoToTrack(this.trackIndex, true);
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x00143771 File Offset: 0x00141971
	private AudioSource GetCurrentAudioSource()
	{
		return this.AudioSources[this.currentAudioSourceIndex];
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x00143784 File Offset: 0x00141984
	private AudioSource GetNextAudioSource()
	{
		return this.AudioSources[this.NextAudioSourceIndex];
	}

	// Token: 0x17000541 RID: 1345
	// (get) Token: 0x06003AE9 RID: 15081 RVA: 0x00143797 File Offset: 0x00141997
	private int NextAudioSourceIndex
	{
		get
		{
			return (this.currentAudioSourceIndex + 1) % this.AudioSources.Count;
		}
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x001437B0 File Offset: 0x001419B0
	private void StopAllAudioSources()
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].Stop();
		}
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x001437E4 File Offset: 0x001419E4
	private void UpdateAudioSourcesVolume(float volume)
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].mute = false;
			this.AudioSources[i].volume = volume;
		}
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x0014382C File Offset: 0x00141A2C
	private void UpdateAudioSourcesPosition(Vector3 position)
	{
		for (int i = 0; i < this.AudioSources.Count; i++)
		{
			this.AudioSources[i].transform.position = position;
		}
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x00143866 File Offset: 0x00141A66
	private void Finish(float delay)
	{
		if (this.finishCoroutine != null)
		{
			return;
		}
		this.finishCoroutine = base.StartCoroutine(this.TryFinish(delay));
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x00143884 File Offset: 0x00141A84
	private IEnumerator TryFinish(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.StopAllAudioSources();
		this.UpdateAudioSourcesVolume(this.cachedSourceVolume);
		if (this.RepositionAudioSourcePoint != null)
		{
			this.UpdateAudioSourcesPosition(this.cachedSourcePosition);
		}
		this.synchedMusicController.enabled = true;
		yield break;
	}

	// Token: 0x04004B51 RID: 19281
	private static double BAR_DURATION = 1.6551724137931034;

	// Token: 0x04004B52 RID: 19282
	public List<GRAdaptiveMusicController.SingleTrack> Tracks;

	// Token: 0x04004B53 RID: 19283
	public GRAdaptiveMusicController.SingleTrack CurrentTrack;

	// Token: 0x04004B54 RID: 19284
	[SerializeField]
	private int trackIndex;

	// Token: 0x04004B55 RID: 19285
	public List<AudioSource> AudioSources;

	// Token: 0x04004B56 RID: 19286
	public Transform RepositionAudioSourcePoint;

	// Token: 0x04004B57 RID: 19287
	public float AdjustedSourceVolume = 0.035f;

	// Token: 0x04004B58 RID: 19288
	private int currentAudioSourceIndex;

	// Token: 0x04004B59 RID: 19289
	private float cachedSourceVolume = 0.1f;

	// Token: 0x04004B5A RID: 19290
	private Vector3 cachedSourcePosition = Vector3.zero;

	// Token: 0x04004B5B RID: 19291
	[SerializeField]
	private SynchedMusicController synchedMusicController;

	// Token: 0x04004B5C RID: 19292
	private Coroutine finishCoroutine;

	// Token: 0x020008CE RID: 2254
	[Serializable]
	public class SingleTrack
	{
		// Token: 0x04004B5D RID: 19293
		public AudioClip IntroClip;

		// Token: 0x04004B5E RID: 19294
		public AudioClip LoopedClip;
	}
}
