using System;
using GorillaNetworking.Store;
using GT_CustomMapSupportRuntime;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F3F RID: 3903
	[Serializable]
	public struct CustomMapCosmeticItem
	{
		// Token: 0x04006FFD RID: 28669
		public GTObjectPlaceholder.ECustomMapCosmeticItem customMapItemSlot;

		// Token: 0x04006FFE RID: 28670
		public HeadModel_CosmeticStand.BustType bustType;

		// Token: 0x04006FFF RID: 28671
		public string playFabID;
	}
}
