using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200059B RID: 1435
public class SoundEffects : MonoBehaviour
{
	// Token: 0x170003CD RID: 973
	// (get) Token: 0x0600245B RID: 9307 RVA: 0x000C3862 File Offset: 0x000C1A62
	public bool isPlaying
	{
		get
		{
			return this._lastClipIndex >= 0 && this._lastClipLength >= 0.0 && this._lastClipElapsedTime < this._lastClipLength;
		}
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000C3895 File Offset: 0x000C1A95
	public void Clear()
	{
		this.audioClips.Clear();
		this._lastClipIndex = -1;
		this._lastClipLength = -1.0;
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000C38B8 File Offset: 0x000C1AB8
	public void Stop()
	{
		if (this.source)
		{
			this.source.GTStop();
		}
		this._lastClipLength = -1.0;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000C38E4 File Offset: 0x000C1AE4
	public void PlayNext(float delayMin, float delayMax, float volMin, float volMax)
	{
		float delay = this._rnd.NextFloat(delayMin, delayMax);
		float volume = this._rnd.NextFloat(volMin, volMax);
		this.PlayNext(delay, volume);
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000C3918 File Offset: 0x000C1B18
	public void PlayNext(float delay = 0f, float volume = 1f)
	{
		if (!this.source)
		{
			return;
		}
		if (this.audioClips == null || this.audioClips.Count == 0)
		{
			return;
		}
		if (this.source.isPlaying)
		{
			this.source.GTStop();
		}
		int num = this._rnd.NextInt(this.audioClips.Count);
		while (this.distinct && this._lastClipIndex == num)
		{
			num = this._rnd.NextInt(this.audioClips.Count);
		}
		AudioClip audioClip = this.audioClips[num];
		this._lastClipIndex = num;
		this._lastClipLength = (double)audioClip.length;
		float num2 = delay;
		if (num2 < this._minDelay)
		{
			num2 = this._minDelay;
		}
		if (num2 < 0.0001f)
		{
			this.source.GTPlayOneShot(audioClip, volume);
			this._lastClipElapsedTime = 0f;
			return;
		}
		this.source.clip = audioClip;
		this.source.volume = volume;
		this.source.GTPlayDelayed(num2);
		this._lastClipElapsedTime = -num2;
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000C3A2C File Offset: 0x000C1C2C
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		if (string.IsNullOrEmpty(this.seed))
		{
			this.seed = "0x1337C0D3";
		}
		this._rnd = new SRand(this.seed);
		if (this.audioClips == null)
		{
			this.audioClips = new List<AudioClip>();
		}
	}

	// Token: 0x04002FC5 RID: 12229
	public AudioSource source;

	// Token: 0x04002FC6 RID: 12230
	[Space]
	public List<AudioClip> audioClips = new List<AudioClip>();

	// Token: 0x04002FC7 RID: 12231
	public string seed = "0x1337C0D3";

	// Token: 0x04002FC8 RID: 12232
	[Space]
	public bool distinct = true;

	// Token: 0x04002FC9 RID: 12233
	[SerializeField]
	private float _minDelay;

	// Token: 0x04002FCA RID: 12234
	[Space]
	[SerializeField]
	private SRand _rnd;

	// Token: 0x04002FCB RID: 12235
	[NonSerialized]
	private int _lastClipIndex = -1;

	// Token: 0x04002FCC RID: 12236
	[NonSerialized]
	private double _lastClipLength = -1.0;

	// Token: 0x04002FCD RID: 12237
	[NonSerialized]
	private TimeSince _lastClipElapsedTime;
}
