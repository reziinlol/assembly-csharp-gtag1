using System;
using UnityEngine;

// Token: 0x020007C5 RID: 1989
public interface IGameProjectileLauncher
{
	// Token: 0x060032AF RID: 12975 RVA: 0x000028C5 File Offset: 0x00000AC5
	void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x000028C5 File Offset: 0x00000AC5
	void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}
}
