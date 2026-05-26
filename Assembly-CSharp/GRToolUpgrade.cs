using System;
using UnityEngine;

// Token: 0x0200080E RID: 2062
public class GRToolUpgrade : ScriptableObject
{
	// Token: 0x0400451F RID: 17695
	public string upgradeName;

	// Token: 0x04004520 RID: 17696
	public string description;

	// Token: 0x04004521 RID: 17697
	public string upgradeId;

	// Token: 0x04004522 RID: 17698
	[SerializeField]
	public GRToolUpgrade.ToolUpgradeLevel[] upgradeLevels;

	// Token: 0x0200080F RID: 2063
	[Serializable]
	public struct ToolUpgradeLevel
	{
		// Token: 0x04004523 RID: 17699
		[SerializeField]
		public int Cost;

		// Token: 0x04004524 RID: 17700
		[SerializeField]
		public float upgradeAmount;
	}
}
