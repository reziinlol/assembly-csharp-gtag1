using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class CritterSpawnCriteria : ScriptableObject
{
	// Token: 0x060002CB RID: 715 RVA: 0x00011030 File Offset: 0x0000F230
	public bool CanSpawn()
	{
		if (this.spawnTimings.Length == 0)
		{
			return true;
		}
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		string[] array = this.spawnTimings;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == currentTimeOfDay)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400033B RID: 827
	public string[] spawnTimings;
}
