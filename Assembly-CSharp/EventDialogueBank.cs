using System;
using UnityEngine;

// Token: 0x0200009A RID: 154
public class EventDialogueBank : MonoBehaviour
{
	// Token: 0x060003DE RID: 990 RVA: 0x000172E0 File Offset: 0x000154E0
	private void Awake()
	{
		for (int i = 0; i < this.bank.Length; i++)
		{
			if (this.bank[i].audioSource == null)
			{
				this.bank[i].audioSource = this.defaultAudioSource;
			}
			this.bank[i].audioSource.playOnAwake = false;
			this.bank[i].audioSource.gameObject.SetActive(true);
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00017364 File Offset: 0x00015564
	private void LateUpdate()
	{
		if (this._index == Mathf.FloorToInt(this.index) - 1)
		{
			return;
		}
		this._index = Mathf.FloorToInt(this.index) - 1;
		if (this._index < 0 || this._index >= this.bank.Length || this.bank[this._index].audioClip == null || this.bank[this._index].audioSource == null)
		{
			return;
		}
		if (this.bank[this._index].audioSource.isPlaying)
		{
			this.bank[this._index].audioSource.Stop();
		}
		this.bank[this._index].audioSource.clip = this.bank[this._index].audioClip;
		this.bank[this._index].audioSource.Play();
	}

	// Token: 0x0400044A RID: 1098
	[SerializeField]
	private EventDialogueBank.EventDialogueBankEntry[] bank;

	// Token: 0x0400044B RID: 1099
	[SerializeField]
	private AudioSource defaultAudioSource;

	// Token: 0x0400044C RID: 1100
	[SerializeField]
	private float index;

	// Token: 0x0400044D RID: 1101
	private int _index = -1;

	// Token: 0x0200009B RID: 155
	[Serializable]
	public struct EventDialogueBankEntry
	{
		// Token: 0x0400044E RID: 1102
		public AudioClip audioClip;

		// Token: 0x0400044F RID: 1103
		public AudioSource audioSource;
	}
}
