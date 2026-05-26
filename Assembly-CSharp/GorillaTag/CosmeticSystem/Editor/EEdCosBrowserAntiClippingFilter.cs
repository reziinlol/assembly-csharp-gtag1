using System;

namespace GorillaTag.CosmeticSystem.Editor
{
	// Token: 0x020011D9 RID: 4569
	[Flags]
	public enum EEdCosBrowserAntiClippingFilter
	{
		// Token: 0x04008330 RID: 33584
		None = 0,
		// Token: 0x04008331 RID: 33585
		NameTag = 1,
		// Token: 0x04008332 RID: 33586
		LeftArm = 2,
		// Token: 0x04008333 RID: 33587
		RightArm = 4,
		// Token: 0x04008334 RID: 33588
		Chest = 8,
		// Token: 0x04008335 RID: 33589
		HuntComputer = 16,
		// Token: 0x04008336 RID: 33590
		Badge = 32,
		// Token: 0x04008337 RID: 33591
		BuilderWatch = 64,
		// Token: 0x04008338 RID: 33592
		FriendshipBraceletLeft = 128,
		// Token: 0x04008339 RID: 33593
		FriendshipBraceletRight = 256,
		// Token: 0x0400833A RID: 33594
		All = 511
	}
}
