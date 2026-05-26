using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007C4 RID: 1988
public class GRProgressionScriptableObject : ScriptableObject
{
	// Token: 0x040041E4 RID: 16868
	[SerializeField]
	[Header("Progression Tiers")]
	public List<GRPlayer.ProgressionLevels> progressionData;
}
