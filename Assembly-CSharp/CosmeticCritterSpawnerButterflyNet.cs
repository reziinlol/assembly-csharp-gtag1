using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class CosmeticCritterSpawnerButterflyNet : CosmeticCritterSpawnerTimed
{
	// Token: 0x060004BA RID: 1210 RVA: 0x0001A674 File Offset: 0x00018874
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		Vector3 startPos = base.transform.position + Random.onUnitSphere * this.spawnRadius;
		(critter as CosmeticCritterButterfly).SetStartPos(startPos);
	}

	// Token: 0x04000537 RID: 1335
	[Tooltip("Spawn a butterfly on the surface of a sphere with this radius, and with a center on this object.")]
	[SerializeField]
	private float spawnRadius = 1f;
}
