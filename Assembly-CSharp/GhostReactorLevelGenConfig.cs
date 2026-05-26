using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F9 RID: 1785
[CreateAssetMenu(fileName = "GhostReactorLevelGenConfig", menuName = "ScriptableObjects/GhostReactorLevelGenConfig")]
public class GhostReactorLevelGenConfig : ScriptableObject
{
	// Token: 0x06002CF8 RID: 11512 RVA: 0x000F37D0 File Offset: 0x000F19D0
	private void OnValidate()
	{
		for (int i = 0; i < this.treeLevels.Count; i++)
		{
			GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig = this.treeLevels[i];
			treeLevelConfig.minHubs = Mathf.Abs(treeLevelConfig.minHubs);
			treeLevelConfig.maxHubs = Mathf.Abs(treeLevelConfig.maxHubs);
			treeLevelConfig.minCaps = Mathf.Abs(treeLevelConfig.minCaps);
			treeLevelConfig.maxCaps = Mathf.Abs(treeLevelConfig.maxCaps);
			if (treeLevelConfig.minHubs > treeLevelConfig.maxHubs)
			{
				treeLevelConfig.maxHubs = treeLevelConfig.minHubs;
			}
			if (treeLevelConfig.minCaps > treeLevelConfig.maxCaps)
			{
				treeLevelConfig.maxCaps = treeLevelConfig.minCaps;
			}
			this.treeLevels[i] = treeLevelConfig;
		}
		GhostReactorLevelGeneratorV2.TreeLevelConfig treeLevelConfig2 = this.treeLevels[this.treeLevels.Count - 1];
		if (treeLevelConfig2.minHubs > 0 || treeLevelConfig2.maxHubs > 0)
		{
			Debug.LogError("Ghost Reactor Level Gen Setup Error: The last tree level can only spawn end caps around the furthest level of hubs. Otherwise it would spawn hubs without a further level to spawn end caps around them");
			treeLevelConfig2.minHubs = 0;
			treeLevelConfig2.maxHubs = 0;
			this.treeLevels[this.treeLevels.Count - 1] = treeLevelConfig2;
		}
		using (List<GREnemyCount>.Enumerator enumerator = this.minEnemyKills.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Count < 0)
				{
					Debug.LogError("Ghost Reactor Level Gen Setup Error: cannot have negative required enemy kills");
				}
			}
		}
	}

	// Token: 0x04003992 RID: 14738
	public int shiftDuration;

	// Token: 0x04003993 RID: 14739
	public int coresRequired;

	// Token: 0x04003994 RID: 14740
	public int shiftBonus;

	// Token: 0x04003995 RID: 14741
	public int sentientCoresRequired;

	// Token: 0x04003996 RID: 14742
	public int maxPlayerDeaths = -1;

	// Token: 0x04003997 RID: 14743
	public List<GREnemyCount> minEnemyKills = new List<GREnemyCount>();

	// Token: 0x04003998 RID: 14744
	[ColorUsage(true, true)]
	public Color ambientLight = Color.black;

	// Token: 0x04003999 RID: 14745
	public List<GhostReactorLevelGeneratorV2.TreeLevelConfig> treeLevels = new List<GhostReactorLevelGeneratorV2.TreeLevelConfig>();

	// Token: 0x0400399A RID: 14746
	public List<GRBonusEntry> enemyGlobalBonuses = new List<GRBonusEntry>();

	// Token: 0x0400399B RID: 14747
	public GRDropTableOverrides dropTableOverrides;
}
