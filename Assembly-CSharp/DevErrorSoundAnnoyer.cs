using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200031C RID: 796
public class DevErrorSoundAnnoyer : MonoBehaviour
{
	// Token: 0x040018BA RID: 6330
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x040018BB RID: 6331
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040018BC RID: 6332
	[SerializeField]
	private Text errorUIText;

	// Token: 0x040018BD RID: 6333
	[SerializeField]
	private Font errorFont;

	// Token: 0x040018BE RID: 6334
	public string displayedText;
}
