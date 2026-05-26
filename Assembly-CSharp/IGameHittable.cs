using System;

// Token: 0x020006CE RID: 1742
public interface IGameHittable
{
	// Token: 0x06002BD8 RID: 11224
	bool IsHitValid(GameHitData hit);

	// Token: 0x06002BD9 RID: 11225
	void OnHit(GameHitData hit);
}
