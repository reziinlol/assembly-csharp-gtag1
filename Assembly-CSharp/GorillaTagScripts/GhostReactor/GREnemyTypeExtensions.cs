using System;

namespace GorillaTagScripts.GhostReactor
{
	// Token: 0x02000F89 RID: 3977
	public static class GREnemyTypeExtensions
	{
		// Token: 0x0600633C RID: 25404 RVA: 0x001FEC78 File Offset: 0x001FCE78
		public static GREnemyType GetEnemyType(this GameEntity entity)
		{
			if (entity == null)
			{
				return GREnemyType.None;
			}
			GREnemy component = entity.GetComponent<GREnemy>();
			if (component == null)
			{
				return GREnemyType.None;
			}
			if (component.enemyType == GREnemyType.MoonBoss_Phase1 || component.enemyType == GREnemyType.MoonBoss_Phase2)
			{
				return GREnemyType.MoonBoss;
			}
			return component.enemyType;
		}

		// Token: 0x0600633D RID: 25405 RVA: 0x001FECC0 File Offset: 0x001FCEC0
		public static string Pluralize(this GREnemyType t)
		{
			string result;
			if (t == GREnemyType.MoonBoss)
			{
				result = "Meteor Monsters";
			}
			else
			{
				result = string.Format("{0}s", t);
			}
			return result;
		}
	}
}
