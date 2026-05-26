using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003D RID: 61
public class CosmeticPlaySoundOnColision : MonoBehaviour
{
	// Token: 0x060000FA RID: 250 RVA: 0x00006034 File Offset: 0x00004234
	private void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.soundLookup = new Dictionary<int, int>();
		this.audioSource = base.GetComponent<AudioSource>();
		for (int i = 0; i < this.soundIdRemappings.Length; i++)
		{
			this.soundLookup.Add(this.soundIdRemappings[i].SoundIn, this.soundIdRemappings[i].SoundOut);
		}
	}

	// Token: 0x060000FB RID: 251 RVA: 0x0000609C File Offset: 0x0000429C
	private void OnTriggerEnter(Collider other)
	{
		GorillaSurfaceOverride gorillaSurfaceOverride;
		if (this.speed >= this.minSpeed && other.TryGetComponent<GorillaSurfaceOverride>(out gorillaSurfaceOverride))
		{
			int soundIndex;
			if (this.soundLookup.TryGetValue(gorillaSurfaceOverride.overrideIndex, out soundIndex))
			{
				this.playSound(soundIndex, this.invokeEventOnOverideSound);
				return;
			}
			this.playSound(this.defaultSound, this.invokeEventOnDefaultSound);
		}
	}

	// Token: 0x060000FC RID: 252 RVA: 0x000060F8 File Offset: 0x000042F8
	private void playSound(int soundIndex, bool invokeEvent)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			if (this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
				{
					this.OnStopPlayback.Invoke();
				}
				if (this.crWaitForStopPlayback != null)
				{
					base.StopCoroutine(this.crWaitForStopPlayback);
					this.crWaitForStopPlayback = null;
				}
			}
			this.audioSource.clip = GTPlayer.Instance.materialData[soundIndex].audio;
			this.audioSource.GTPlay();
			if (invokeEvent && (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem()))
			{
				this.OnStartPlayback.Invoke();
				this.crWaitForStopPlayback = base.StartCoroutine(this.waitForStopPlayback());
			}
		}
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000061D4 File Offset: 0x000043D4
	private IEnumerator waitForStopPlayback()
	{
		while (this.audioSource.isPlaying)
		{
			yield return null;
		}
		if (this.invokeEventsOnAllClients || this.transferrableObject.IsMyItem())
		{
			this.OnStopPlayback.Invoke();
		}
		this.crWaitForStopPlayback = null;
		yield break;
	}

	// Token: 0x060000FE RID: 254 RVA: 0x000061E3 File Offset: 0x000043E3
	private void FixedUpdate()
	{
		this.speed = Vector3.Distance(base.transform.position, this.previousFramePosition) * Time.fixedDeltaTime * 100f;
		this.previousFramePosition = base.transform.position;
	}

	// Token: 0x04000102 RID: 258
	[GorillaSoundLookup]
	[SerializeField]
	private int defaultSound = 1;

	// Token: 0x04000103 RID: 259
	[SerializeField]
	private SoundIdRemapping[] soundIdRemappings;

	// Token: 0x04000104 RID: 260
	[SerializeField]
	private UnityEvent OnStartPlayback;

	// Token: 0x04000105 RID: 261
	[SerializeField]
	private UnityEvent OnStopPlayback;

	// Token: 0x04000106 RID: 262
	[SerializeField]
	private float minSpeed = 0.1f;

	// Token: 0x04000107 RID: 263
	private TransferrableObject transferrableObject;

	// Token: 0x04000108 RID: 264
	private Dictionary<int, int> soundLookup;

	// Token: 0x04000109 RID: 265
	private AudioSource audioSource;

	// Token: 0x0400010A RID: 266
	private Coroutine crWaitForStopPlayback;

	// Token: 0x0400010B RID: 267
	private float speed;

	// Token: 0x0400010C RID: 268
	private Vector3 previousFramePosition;

	// Token: 0x0400010D RID: 269
	[SerializeField]
	private bool invokeEventsOnAllClients;

	// Token: 0x0400010E RID: 270
	[SerializeField]
	private bool invokeEventOnOverideSound = true;

	// Token: 0x0400010F RID: 271
	[SerializeField]
	private bool invokeEventOnDefaultSound;
}
