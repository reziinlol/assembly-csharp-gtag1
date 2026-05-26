using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000CF5 RID: 3317
[Serializable]
internal class RoomCount : PrivateRoomCount
{
	// Token: 0x06005256 RID: 21078 RVA: 0x001B1A24 File Offset: 0x001AFC24
	public int GetRoomCount(GTZone zone)
	{
		for (int i = 0; i < this.zoneCountOverrides.Length; i++)
		{
			if (this.zoneCountOverrides[i].Zone == zone)
			{
				return this.zoneCountOverrides[i].Count;
			}
		}
		return this.count;
	}

	// Token: 0x06005257 RID: 21079 RVA: 0x001B1A68 File Offset: 0x001AFC68
	public override int GetRoomCount(GTZone zone, GameModeType mode)
	{
		return Mathf.Min(this.GetRoomCount(zone), base.GetRoomCount(mode));
	}

	// Token: 0x040063A1 RID: 25505
	[SerializeField]
	private RoomCountForZone[] zoneCountOverrides;
}
