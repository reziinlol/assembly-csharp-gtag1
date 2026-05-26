using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001142 RID: 4418
	public static class GTColor
	{
		// Token: 0x06007023 RID: 28707 RVA: 0x002493C4 File Offset: 0x002475C4
		public static Color RandomHSV(GTColor.HSVRanges ranges)
		{
			return Color.HSVToRGB(Random.Range(ranges.h.x, ranges.h.y), Random.Range(ranges.s.x, ranges.s.y), Random.Range(ranges.v.x, ranges.v.y));
		}

		// Token: 0x02001143 RID: 4419
		[Serializable]
		public struct HSVRanges
		{
			// Token: 0x06007024 RID: 28708 RVA: 0x00249427 File Offset: 0x00247627
			public HSVRanges(float hMin = 0f, float hMax = 1f, float sMin = 0f, float sMax = 1f, float vMin = 0f, float vMax = 1f)
			{
				this.h = new Vector2(hMin, hMax);
				this.s = new Vector2(sMin, sMax);
				this.v = new Vector2(vMin, vMax);
			}

			// Token: 0x04008008 RID: 32776
			public Vector2 h;

			// Token: 0x04008009 RID: 32777
			public Vector2 s;

			// Token: 0x0400800A RID: 32778
			public Vector2 v;
		}
	}
}
