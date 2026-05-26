using System;
using UnityEngine;

// Token: 0x020005E0 RID: 1504
public class AnimationEventListener : MonoBehaviour
{
	// Token: 0x06002568 RID: 9576 RVA: 0x000C6638 File Offset: 0x000C4838
	public void PlaySoundAtIndex(int index)
	{
		if (this.audioClips.Length <= index || index < 0)
		{
			return;
		}
		if (this.audioSource == null)
		{
			return;
		}
		if (this.audioClips[index] == null)
		{
			return;
		}
		this.audioSource.GTPlayOneShot(this.audioClips[index], 1f);
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x000C668D File Offset: 0x000C488D
	public void StopAudio()
	{
		if (this.audioSource == null)
		{
			return;
		}
		if (this.audioSource.isPlaying)
		{
			this.audioSource.Stop();
		}
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x000C66B6 File Offset: 0x000C48B6
	public void ActivateObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(true);
		}
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x000C66D2 File Offset: 0x000C48D2
	public void DeactivateObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(false);
		}
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x000C66EE File Offset: 0x000C48EE
	public void ToggleObject()
	{
		if (this.targetObject != null)
		{
			this.targetObject.SetActive(!this.targetObject.activeSelf);
		}
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x000C6717 File Offset: 0x000C4917
	public void PlayParticles()
	{
		if (this.particles != null && !this.particles.isPlaying)
		{
			this.particles.Play();
		}
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x000C673F File Offset: 0x000C493F
	public void StopParticles()
	{
		if (this.particles != null && this.particles.isPlaying)
		{
			this.particles.Stop();
		}
	}

	// Token: 0x040030E8 RID: 12520
	[Tooltip("Set this if calling ActivateObject, DeactivateObject, or ToggleObject")]
	[SerializeField]
	private GameObject targetObject;

	// Token: 0x040030E9 RID: 12521
	[Tooltip("Set this if calling PlayParticles or StopParticles")]
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x040030EA RID: 12522
	[Tooltip("Set this if calling PlaySoundAtIndex or StopAudio")]
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040030EB RID: 12523
	[Tooltip("Set this if calling PlaySoundAtIndex")]
	[SerializeField]
	private AudioClip[] audioClips;
}
