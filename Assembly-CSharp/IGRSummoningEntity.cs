using System;

// Token: 0x020007E7 RID: 2023
public interface IGRSummoningEntity
{
	// Token: 0x060033B5 RID: 13237
	void OnSummonedEntityInit(GameEntity entity);

	// Token: 0x060033B6 RID: 13238
	void OnSummonedEntityDestroy(GameEntity entity);
}
