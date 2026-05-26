using System;

// Token: 0x020006E8 RID: 1768
[Flags]
public enum SnapJointType
{
	// Token: 0x040038FC RID: 14588
	None = 0,
	// Token: 0x040038FD RID: 14589
	HandL = 1,
	// Token: 0x040038FE RID: 14590
	HandR = 4,
	// Token: 0x040038FF RID: 14591
	Chest = 8,
	// Token: 0x04003900 RID: 14592
	Back = 16,
	// Token: 0x04003901 RID: 14593
	Head = 32,
	// Token: 0x04003902 RID: 14594
	Holster = 64,
	// Token: 0x04003903 RID: 14595
	ForearmL = 128,
	// Token: 0x04003904 RID: 14596
	ForearmR = 256,
	// Token: 0x04003905 RID: 14597
	AuxHead = 512,
	// Token: 0x04003906 RID: 14598
	AuxBody1 = 1024,
	// Token: 0x04003907 RID: 14599
	AuxBody2 = 2048,
	// Token: 0x04003908 RID: 14600
	AuxShoulderL = 4096,
	// Token: 0x04003909 RID: 14601
	AuxShoulderR = 8192,
	// Token: 0x0400390A RID: 14602
	Max = 16384
}
