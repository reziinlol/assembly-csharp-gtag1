using System;
using UnityEngine;

// Token: 0x02000D7C RID: 3452
[CreateAssetMenu(menuName = "Gorilla Tag/SoundBankSO")]
public class SoundBankSO : ScriptableObject
{
	// Token: 0x0400654B RID: 25931
	public AudioClip[] sounds;

	// Token: 0x0400654C RID: 25932
	public Vector2 volumeRange = new Vector2(0.5f, 0.5f);

	// Token: 0x0400654D RID: 25933
	public Vector2 pitchRange = new Vector2(1f, 1f);
}
