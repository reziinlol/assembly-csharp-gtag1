using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000CF4 RID: 3316
[Serializable]
internal class PrivateRoomCount
{
	// Token: 0x06005252 RID: 21074 RVA: 0x001B19CA File Offset: 0x001AFBCA
	public int GetRoomCount()
	{
		return this.count;
	}

	// Token: 0x06005253 RID: 21075 RVA: 0x001B19D4 File Offset: 0x001AFBD4
	public int GetRoomCount(GameModeType mode)
	{
		for (int i = 0; i < this.modeCountOverrides.Length; i++)
		{
			if (this.modeCountOverrides[i].Mode == mode)
			{
				return this.modeCountOverrides[i].Count;
			}
		}
		return this.count;
	}

	// Token: 0x06005254 RID: 21076 RVA: 0x001B1A18 File Offset: 0x001AFC18
	public virtual int GetRoomCount(GTZone zone, GameModeType mode)
	{
		return this.GetRoomCount(mode);
	}

	// Token: 0x0400639F RID: 25503
	[SerializeField]
	protected int count;

	// Token: 0x040063A0 RID: 25504
	[SerializeField]
	protected RoomCountForMode[] modeCountOverrides;
}
