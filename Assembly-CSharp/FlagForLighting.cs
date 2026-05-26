using System;
using UnityEngine;

// Token: 0x02000D4C RID: 3404
public class FlagForLighting : MonoBehaviour
{
	// Token: 0x040064D6 RID: 25814
	public FlagForLighting.TimeOfDay myTimeOfDay;

	// Token: 0x02000D4D RID: 3405
	public enum TimeOfDay
	{
		// Token: 0x040064D8 RID: 25816
		Sunrise,
		// Token: 0x040064D9 RID: 25817
		TenAM,
		// Token: 0x040064DA RID: 25818
		Noon,
		// Token: 0x040064DB RID: 25819
		ThreePM,
		// Token: 0x040064DC RID: 25820
		Sunset,
		// Token: 0x040064DD RID: 25821
		Night,
		// Token: 0x040064DE RID: 25822
		RainingDay,
		// Token: 0x040064DF RID: 25823
		RainingNight,
		// Token: 0x040064E0 RID: 25824
		None
	}
}
