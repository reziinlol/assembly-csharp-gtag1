using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010A2 RID: 4258
	public class DynamicCosmeticStand_Link : MonoBehaviour
	{
		// Token: 0x06006AD6 RID: 27350 RVA: 0x00228B22 File Offset: 0x00226D22
		public void SetStandType(HeadModel_CosmeticStand.BustType type)
		{
			this.stand.SetStandType(type);
		}

		// Token: 0x06006AD7 RID: 27351 RVA: 0x00228B30 File Offset: 0x00226D30
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.stand.SpawnItemOntoStand(PlayFabID);
		}

		// Token: 0x06006AD8 RID: 27352 RVA: 0x00228B3E File Offset: 0x00226D3E
		public void SaveCosmeticMountPosition()
		{
			this.stand.UpdateCosmeticsMountPositions();
		}

		// Token: 0x06006AD9 RID: 27353 RVA: 0x00228B4B File Offset: 0x00226D4B
		public void ClearCosmeticItems()
		{
			this.stand.ClearCosmetics();
		}

		// Token: 0x04007B04 RID: 31492
		public DynamicCosmeticStand stand;
	}
}
