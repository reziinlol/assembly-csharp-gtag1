using System;
using System.Collections.Generic;
using JetBrains.Annotations;

// Token: 0x020006FA RID: 1786
public class GhostReactorLevelGeneratorV2
{
	// Token: 0x020006FB RID: 1787
	[Serializable]
	public struct TreeLevelConfig
	{
		// Token: 0x06002CFB RID: 11515 RVA: 0x000F397C File Offset: 0x000F1B7C
		public bool ValidateDatetime([CanBeNull] string timestamp)
		{
			DateTime dateTime;
			return string.IsNullOrEmpty(timestamp) || DateTime.TryParse(timestamp, out dateTime);
		}

		// Token: 0x0400399C RID: 14748
		[CanBeNull]
		public string EnableAfterDatetime;

		// Token: 0x0400399D RID: 14749
		[CanBeNull]
		public string DisableAfterDatetime;

		// Token: 0x0400399E RID: 14750
		public int minHubs;

		// Token: 0x0400399F RID: 14751
		public int maxHubs;

		// Token: 0x040039A0 RID: 14752
		public int minCaps;

		// Token: 0x040039A1 RID: 14753
		public int maxCaps;

		// Token: 0x040039A2 RID: 14754
		public List<GhostReactorSpawnConfig> sectionSpawnConfigs;

		// Token: 0x040039A3 RID: 14755
		public List<GhostReactorSpawnConfig> endCapSpawnConfigs;

		// Token: 0x040039A4 RID: 14756
		public List<GhostReactorLevelSection> hubs;

		// Token: 0x040039A5 RID: 14757
		public List<GhostReactorLevelSection> endCaps;

		// Token: 0x040039A6 RID: 14758
		public List<GhostReactorLevelSection> blockers;

		// Token: 0x040039A7 RID: 14759
		public List<GhostReactorLevelSectionConnector> connectors;
	}
}
