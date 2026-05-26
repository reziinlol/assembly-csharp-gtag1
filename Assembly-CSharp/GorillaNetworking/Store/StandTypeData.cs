using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x020010A8 RID: 4264
	public class StandTypeData
	{
		// Token: 0x06006B0C RID: 27404 RVA: 0x0022A398 File Offset: 0x00228598
		public StandTypeData(string[] spawnData)
		{
			this.departmentID = spawnData[0];
			this.displayID = spawnData[1];
			this.standID = spawnData[2];
			this.bustType = spawnData[3];
			if (spawnData.Length == 5)
			{
				this.playFabID = spawnData[4];
			}
			Debug.Log(string.Concat(new string[]
			{
				"StoreStuff: StandTypeData: ",
				this.departmentID,
				"\n",
				this.displayID,
				"\n",
				this.standID,
				"\n",
				this.bustType,
				"\n",
				this.playFabID
			}));
		}

		// Token: 0x06006B0D RID: 27405 RVA: 0x0022A47C File Offset: 0x0022867C
		public StandTypeData(string departmentID, string displayID, string standID, HeadModel_CosmeticStand.BustType bustType, string playFabID)
		{
			this.departmentID = departmentID;
			this.displayID = displayID;
			this.standID = standID;
			this.bustType = bustType.ToString();
			this.playFabID = playFabID;
		}

		// Token: 0x04007B24 RID: 31524
		public string departmentID = "";

		// Token: 0x04007B25 RID: 31525
		public string displayID = "";

		// Token: 0x04007B26 RID: 31526
		public string standID = "";

		// Token: 0x04007B27 RID: 31527
		public string bustType = "";

		// Token: 0x04007B28 RID: 31528
		public string playFabID = "";

		// Token: 0x020010A9 RID: 4265
		public enum EStandDataID
		{
			// Token: 0x04007B2A RID: 31530
			departmentID,
			// Token: 0x04007B2B RID: 31531
			displayID,
			// Token: 0x04007B2C RID: 31532
			standID,
			// Token: 0x04007B2D RID: 31533
			bustType,
			// Token: 0x04007B2E RID: 31534
			playFabID,
			// Token: 0x04007B2F RID: 31535
			Count
		}
	}
}
