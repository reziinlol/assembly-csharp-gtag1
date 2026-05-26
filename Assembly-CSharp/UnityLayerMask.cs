using System;

// Token: 0x020003A8 RID: 936
[Flags]
public enum UnityLayerMask
{
	// Token: 0x040020E9 RID: 8425
	Everything = -1,
	// Token: 0x040020EA RID: 8426
	Nothing = 0,
	// Token: 0x040020EB RID: 8427
	Default = 1,
	// Token: 0x040020EC RID: 8428
	TransparentFX = 2,
	// Token: 0x040020ED RID: 8429
	IgnoreRaycast = 4,
	// Token: 0x040020EE RID: 8430
	Water = 16,
	// Token: 0x040020EF RID: 8431
	UI = 32,
	// Token: 0x040020F0 RID: 8432
	MeshBakerAtlas = 64,
	// Token: 0x040020F1 RID: 8433
	GorillaEquipment = 128,
	// Token: 0x040020F2 RID: 8434
	GorillaBodyCollider = 256,
	// Token: 0x040020F3 RID: 8435
	GorillaObject = 512,
	// Token: 0x040020F4 RID: 8436
	GorillaHand = 1024,
	// Token: 0x040020F5 RID: 8437
	GorillaTrigger = 2048,
	// Token: 0x040020F6 RID: 8438
	MetaReportScreen = 4096,
	// Token: 0x040020F7 RID: 8439
	GorillaHead = 8192,
	// Token: 0x040020F8 RID: 8440
	GorillaTagCollider = 16384,
	// Token: 0x040020F9 RID: 8441
	GorillaBoundary = 32768,
	// Token: 0x040020FA RID: 8442
	GorillaEquipmentContainer = 65536,
	// Token: 0x040020FB RID: 8443
	LCKHide = 131072,
	// Token: 0x040020FC RID: 8444
	GorillaInteractable = 262144,
	// Token: 0x040020FD RID: 8445
	FirstPersonOnly = 524288,
	// Token: 0x040020FE RID: 8446
	GorillaParticle = 1048576,
	// Token: 0x040020FF RID: 8447
	GorillaCosmetics = 2097152,
	// Token: 0x04002100 RID: 8448
	MirrorOnly = 4194304,
	// Token: 0x04002101 RID: 8449
	GorillaThrowable = 8388608,
	// Token: 0x04002102 RID: 8450
	GorillaHandSocket = 16777216,
	// Token: 0x04002103 RID: 8451
	GorillaCosmeticParticle = 33554432,
	// Token: 0x04002104 RID: 8452
	BuilderProp = 67108864,
	// Token: 0x04002105 RID: 8453
	NoMirror = 134217728,
	// Token: 0x04002106 RID: 8454
	GorillaSlingshotCollider = 268435456,
	// Token: 0x04002107 RID: 8455
	RopeSwing = 536870912,
	// Token: 0x04002108 RID: 8456
	Prop = 1073741824,
	// Token: 0x04002109 RID: 8457
	Bake = -2147483648
}
