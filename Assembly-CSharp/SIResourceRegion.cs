using System;

// Token: 0x02000160 RID: 352
public class SIResourceRegion : SpawnRegion<GameEntity, SIResourceRegion>
{
	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000942 RID: 2370 RVA: 0x00031FE4 File Offset: 0x000301E4
	// (set) Token: 0x06000943 RID: 2371 RVA: 0x00031FEC File Offset: 0x000301EC
	public float LastSpawnTime { get; set; }

	// Token: 0x04000B59 RID: 2905
	public SIResource resourcePrefab;
}
