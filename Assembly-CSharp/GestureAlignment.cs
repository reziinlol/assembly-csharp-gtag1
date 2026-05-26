using System;

// Token: 0x020002CB RID: 715
[Flags]
public enum GestureAlignment : uint
{
	// Token: 0x04001647 RID: 5703
	None = 0U,
	// Token: 0x04001648 RID: 5704
	TowardFace = 128U,
	// Token: 0x04001649 RID: 5705
	AwayFromFace = 256U,
	// Token: 0x0400164A RID: 5706
	WorldUp = 512U,
	// Token: 0x0400164B RID: 5707
	WorldDown = 1024U
}
