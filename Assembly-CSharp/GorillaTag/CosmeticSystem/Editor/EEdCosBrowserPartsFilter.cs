using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x020011DB RID: 4571
	[Flags]
	public enum EEdCosBrowserPartsFilter
	{
		// Token: 0x0400834B RID: 33611
		None = 0,
		// Token: 0x0400834C RID: 33612
		NoParts = 1,
		// Token: 0x0400834D RID: 33613
		Holdable = 2,
		// Token: 0x0400834E RID: 33614
		Functional = 4,
		// Token: 0x0400834F RID: 33615
		Wardrobe = 8,
		// Token: 0x04008350 RID: 33616
		Store = 16,
		// Token: 0x04008351 RID: 33617
		FirstPerson = 32,
		// Token: 0x04008352 RID: 33618
		LocalRig = 64,
		// Token: 0x04008353 RID: 33619
		All = 127
	}
}
