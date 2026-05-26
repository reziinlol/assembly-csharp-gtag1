using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001341 RID: 4929
	public class MathUtil
	{
		// Token: 0x06007C22 RID: 31778 RVA: 0x0028BF58 File Offset: 0x0028A158
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007C23 RID: 31779 RVA: 0x0028BF6F File Offset: 0x0028A16F
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007C24 RID: 31780 RVA: 0x0028BF88 File Offset: 0x0028A188
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x04008D54 RID: 36180
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04008D55 RID: 36181
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04008D56 RID: 36182
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04008D57 RID: 36183
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x04008D58 RID: 36184
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x04008D59 RID: 36185
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x04008D5A RID: 36186
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04008D5B RID: 36187
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04008D5C RID: 36188
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04008D5D RID: 36189
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04008D5E RID: 36190
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04008D5F RID: 36191
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x04008D60 RID: 36192
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x04008D61 RID: 36193
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04008D62 RID: 36194
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
