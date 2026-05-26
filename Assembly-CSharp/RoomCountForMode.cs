using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000CF7 RID: 3319
[Serializable]
internal class RoomCountForMode
{
	// Token: 0x170007C1 RID: 1985
	// (get) Token: 0x0600525C RID: 21084 RVA: 0x001B1A95 File Offset: 0x001AFC95
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x170007C2 RID: 1986
	// (get) Token: 0x0600525D RID: 21085 RVA: 0x001B1A9D File Offset: 0x001AFC9D
	public GameModeType Mode
	{
		get
		{
			return this.mode;
		}
	}

	// Token: 0x040063A4 RID: 25508
	[SerializeField]
	private GameModeType mode;

	// Token: 0x040063A5 RID: 25509
	[SerializeField]
	private int count;
}
