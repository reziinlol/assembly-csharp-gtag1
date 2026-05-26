using System;
using UnityEngine;

// Token: 0x020005D1 RID: 1489
public class GorillaSurfaceOverride : MonoBehaviour
{
	// Token: 0x040030BB RID: 12475
	[GorillaSoundLookup]
	public int overrideIndex;

	// Token: 0x040030BC RID: 12476
	public float extraVelMultiplier = 1f;

	// Token: 0x040030BD RID: 12477
	public float extraVelMaxMultiplier = 1f;

	// Token: 0x040030BE RID: 12478
	[HideInInspector]
	[NonSerialized]
	public float slidePercentageOverride = -1f;

	// Token: 0x040030BF RID: 12479
	public bool sendOnTapEvent;

	// Token: 0x040030C0 RID: 12480
	public bool disablePushBackEffect;
}
