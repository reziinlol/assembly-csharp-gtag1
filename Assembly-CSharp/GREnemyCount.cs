using System;
using GorillaTagScripts.GhostReactor;

// Token: 0x02000787 RID: 1927
[Serializable]
public struct GREnemyCount
{
	// Token: 0x06003106 RID: 12550 RVA: 0x0010B478 File Offset: 0x00109678
	public GREnemyType GetEnemyType()
	{
		if (this.EnemyType == GREnemyType.MoonBoss_Phase1 || this.EnemyType == GREnemyType.MoonBoss_Phase2)
		{
			return GREnemyType.MoonBoss;
		}
		return this.EnemyType;
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x0010B498 File Offset: 0x00109698
	public string GetEnemyName()
	{
		if (this.GetEnemyType() == GREnemyType.MoonBoss)
		{
			return "Meteor Monster";
		}
		return this.GetEnemyType().ToString();
	}

	// Token: 0x04003F23 RID: 16163
	public GREnemyType EnemyType;

	// Token: 0x04003F24 RID: 16164
	public int Count;
}
