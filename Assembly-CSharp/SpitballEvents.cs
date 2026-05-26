using System;
using UnityEngine;

// Token: 0x02000591 RID: 1425
public class SpitballEvents : SubEmitterListener
{
	// Token: 0x06002416 RID: 9238 RVA: 0x000C1BD8 File Offset: 0x000BFDD8
	protected override void OnSubEmit()
	{
		base.OnSubEmit();
		if (this._audioSource && this._sfxHit)
		{
			this._audioSource.GTPlayOneShot(this._sfxHit, 1f);
		}
	}

	// Token: 0x04002F5A RID: 12122
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x04002F5B RID: 12123
	[SerializeField]
	private AudioClip _sfxHit;
}
