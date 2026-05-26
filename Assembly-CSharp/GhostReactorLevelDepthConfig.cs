using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006F7 RID: 1783
[CreateAssetMenu(fileName = "GhostReactorLevelDepthConfig", menuName = "ScriptableObjects/GhostReactorLevelDepthConfig")]
public class GhostReactorLevelDepthConfig : ScriptableObject
{
	// Token: 0x0400398D RID: 14733
	public string displayName;

	// Token: 0x0400398E RID: 14734
	public List<GhostReactorLevelGenConfig> configGenOptions = new List<GhostReactorLevelGenConfig>();

	// Token: 0x0400398F RID: 14735
	public List<GhostReactorLevelDepthConfig.LevelOption> options = new List<GhostReactorLevelDepthConfig.LevelOption>();

	// Token: 0x020006F8 RID: 1784
	[Serializable]
	public class LevelOption
	{
		// Token: 0x04003990 RID: 14736
		public int weight = 100;

		// Token: 0x04003991 RID: 14737
		public GhostReactorLevelGenConfig levelConfig;
	}
}
