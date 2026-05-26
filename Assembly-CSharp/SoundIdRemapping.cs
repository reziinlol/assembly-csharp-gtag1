using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
[Serializable]
internal class SoundIdRemapping
{
	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000106 RID: 262 RVA: 0x000062D1 File Offset: 0x000044D1
	public int SoundIn
	{
		get
		{
			return this.soundIn;
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000107 RID: 263 RVA: 0x000062D9 File Offset: 0x000044D9
	public int SoundOut
	{
		get
		{
			return this.soundOut;
		}
	}

	// Token: 0x04000113 RID: 275
	[GorillaSoundLookup]
	[SerializeField]
	private int soundIn = 1;

	// Token: 0x04000114 RID: 276
	[GorillaSoundLookup]
	[SerializeField]
	private int soundOut = 2;
}
