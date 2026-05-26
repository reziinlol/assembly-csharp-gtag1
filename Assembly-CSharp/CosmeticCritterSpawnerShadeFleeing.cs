using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class CosmeticCritterSpawnerShadeFleeing : CosmeticCritterSpawner
{
	// Token: 0x060004D2 RID: 1234 RVA: 0x0001ADDF File Offset: 0x00018FDF
	public void SetSpawnPosition(Vector3 pos)
	{
		this.spawnPosition = pos;
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x0001ADE8 File Offset: 0x00018FE8
	public override void OnSpawn(CosmeticCritter critter)
	{
		base.OnSpawn(critter);
		(critter as CosmeticCritterShadeFleeing).SetFleePosition(this.spawnPosition, base.transform.position);
	}

	// Token: 0x04000555 RID: 1365
	private Vector3 spawnPosition;
}
