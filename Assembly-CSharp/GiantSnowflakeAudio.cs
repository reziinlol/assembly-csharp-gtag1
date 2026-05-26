using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200082D RID: 2093
public class GiantSnowflakeAudio : MonoBehaviour
{
	// Token: 0x060035BC RID: 13756 RVA: 0x00129AF0 File Offset: 0x00127CF0
	private void Start()
	{
		foreach (GiantSnowflakeAudio.SnowflakeScaleOverride snowflakeScaleOverride in this.audioOverrides)
		{
			if (base.transform.lossyScale.x < snowflakeScaleOverride.scaleMax)
			{
				base.GetComponent<GorillaSurfaceOverride>().overrideIndex = snowflakeScaleOverride.newOverrideIndex;
			}
		}
	}

	// Token: 0x0400466F RID: 18031
	public List<GiantSnowflakeAudio.SnowflakeScaleOverride> audioOverrides;

	// Token: 0x0200082E RID: 2094
	[Serializable]
	public struct SnowflakeScaleOverride
	{
		// Token: 0x04004670 RID: 18032
		public float scaleMax;

		// Token: 0x04004671 RID: 18033
		[GorillaSoundLookup]
		public int newOverrideIndex;
	}
}
