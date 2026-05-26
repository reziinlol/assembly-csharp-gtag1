using System;
using UnityEngine;

// Token: 0x020006CF RID: 1743
public interface IGameHitter
{
	// Token: 0x06002BDA RID: 11226
	void OnSuccessfulHit(GameHitData hit);

	// Token: 0x06002BDB RID: 11227 RVA: 0x000028C5 File Offset: 0x00000AC5
	void OnSuccessfulHitPlayer(GRPlayer player, Vector3 hitPosition)
	{
	}
}
