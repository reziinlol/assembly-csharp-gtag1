using System;

// Token: 0x0200066E RID: 1646
[Flags]
public enum CosmeticCritterAction
{
	// Token: 0x040035A3 RID: 13731
	None = 0,
	// Token: 0x040035A4 RID: 13732
	RPC = 1,
	// Token: 0x040035A5 RID: 13733
	Spawn = 2,
	// Token: 0x040035A6 RID: 13734
	Despawn = 4,
	// Token: 0x040035A7 RID: 13735
	SpawnLinked = 8,
	// Token: 0x040035A8 RID: 13736
	ShadeHeartbeat = 16
}
