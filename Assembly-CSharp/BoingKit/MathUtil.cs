using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200138F RID: 5007
	public class MathUtil
	{
		// Token: 0x06007E09 RID: 32265 RVA: 0x0028BF58 File Offset: 0x0028A158
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007E0A RID: 32266 RVA: 0x0028BF6F File Offset: 0x0028A16F
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007E0B RID: 32267 RVA: 0x00297060 File Offset: 0x00295260
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x06007E0C RID: 32268 RVA: 0x00297074 File Offset: 0x00295274
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x06007E0D RID: 32269 RVA: 0x002970A4 File Offset: 0x002952A4
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 a = segmentPosB - segmentPosA;
			float num = 1f / a.magnitude;
			Vector2 rhs = a * num;
			float value = Vector2.Dot(point - segmentPosA, rhs) * num;
			return (segmentPosA + Mathf.Clamp(value, 0f, 1f) * a - point).magnitude;
		}

		// Token: 0x06007E0E RID: 32270 RVA: 0x0029710C File Offset: 0x0029530C
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x06007E0F RID: 32271 RVA: 0x00297134 File Offset: 0x00295334
		public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
		{
			Vector2 b = target - current;
			float magnitude = b.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return target;
			}
			b = Mathf.Min(maxDelta, magnitude) * b.normalized;
			return current + b;
		}

		// Token: 0x06007E10 RID: 32272 RVA: 0x00297176 File Offset: 0x00295376
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x06007E11 RID: 32273 RVA: 0x00297176 File Offset: 0x00295376
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x06007E12 RID: 32274 RVA: 0x0029717F File Offset: 0x0029537F
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06007E13 RID: 32275 RVA: 0x00297188 File Offset: 0x00295388
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x04008F66 RID: 36710
		public static readonly float Pi = 3.1415927f;

		// Token: 0x04008F67 RID: 36711
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x04008F68 RID: 36712
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x04008F69 RID: 36713
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x04008F6A RID: 36714
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x04008F6B RID: 36715
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x04008F6C RID: 36716
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x04008F6D RID: 36717
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x04008F6E RID: 36718
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x04008F6F RID: 36719
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x04008F70 RID: 36720
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x04008F71 RID: 36721
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
