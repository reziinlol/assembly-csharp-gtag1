using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000010 RID: 16
[RequireComponent(typeof(AudioSource))]
public class AudioSourceClipRandomizer : MonoBehaviour
{
	// Token: 0x06000047 RID: 71 RVA: 0x00002B1B File Offset: 0x00000D1B
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.playOnAwake = this.source.playOnAwake;
		this.source.playOnAwake = false;
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00002B48 File Offset: 0x00000D48
	public void Play()
	{
		int num = Random.Range(0, 60);
		if (GorillaComputer.instance != null)
		{
			num = GorillaComputer.instance.GetServerTime().Second;
		}
		this.source.clip = this.clips[num % this.clips.Length];
		this.source.GTPlay();
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00002BA9 File Offset: 0x00000DA9
	private void OnEnable()
	{
		if (this.playOnAwake)
		{
			this.Play();
		}
	}

	// Token: 0x04000028 RID: 40
	[SerializeField]
	private AudioClip[] clips;

	// Token: 0x04000029 RID: 41
	private AudioSource source;

	// Token: 0x0400002A RID: 42
	private bool playOnAwake;
}
