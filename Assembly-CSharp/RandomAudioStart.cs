using System;
using UnityEngine;

// Token: 0x0200055E RID: 1374
public class RandomAudioStart : MonoBehaviour, IBuildValidation
{
	// Token: 0x060022E8 RID: 8936 RVA: 0x000BB669 File Offset: 0x000B9869
	public bool BuildValidationCheck()
	{
		if (this.audioSource == null)
		{
			Debug.LogError("audio source is missing for RandomAudioStart, it won't work correctly", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000BB68C File Offset: 0x000B988C
	private void OnEnable()
	{
		this.audioSource.time = Random.value * this.audioSource.clip.length;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000BB6AF File Offset: 0x000B98AF
	[ContextMenu("Assign Audio Source")]
	public void AssignAudioSource()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x04002DF7 RID: 11767
	public AudioSource audioSource;
}
