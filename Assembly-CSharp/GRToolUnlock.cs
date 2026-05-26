using System;
using UnityEngine;

// Token: 0x0200080D RID: 2061
public class GRToolUnlock : ScriptableObject
{
	// Token: 0x0400451A RID: 17690
	public string toolName;

	// Token: 0x0400451B RID: 17691
	public string toolId;

	// Token: 0x0400451C RID: 17692
	public int unlockLevel;

	// Token: 0x0400451D RID: 17693
	public int unlockCost;

	// Token: 0x0400451E RID: 17694
	public GRToolUpgrade[] toolUpgrades;
}
