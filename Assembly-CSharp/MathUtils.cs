using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000D5C RID: 3420
public static class MathUtils
{
	// Token: 0x06005405 RID: 21509 RVA: 0x001B6FCF File Offset: 0x001B51CF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Xlerp(float a, float b, float dt, float decay = 16f)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	// Token: 0x06005406 RID: 21510 RVA: 0x001B6FE0 File Offset: 0x001B51E0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Xlerp(Vector3 a, Vector3 b, float dt, float decay = 16f)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	// Token: 0x06005407 RID: 21511 RVA: 0x001B6FFD File Offset: 0x001B51FD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float SafeDivide(this float f, float d, float eps = 1E-06f)
	{
		if (Math.Abs(d) < eps)
		{
			return 0f;
		}
		if (float.IsNaN(f))
		{
			return 0f;
		}
		return f / d;
	}

	// Token: 0x06005408 RID: 21512 RVA: 0x001B7020 File Offset: 0x001B5220
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 SafeDivide(this Vector3 v, float d)
	{
		v.x = v.x.SafeDivide(d, 1E-05f);
		v.y = v.y.SafeDivide(d, 1E-05f);
		v.z = v.z.SafeDivide(d, 1E-05f);
		return v;
	}

	// Token: 0x06005409 RID: 21513 RVA: 0x001B7078 File Offset: 0x001B5278
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 SafeDivide(this Vector3 v, Vector3 d)
	{
		v.x = v.x.SafeDivide(d.x, 1E-05f);
		v.y = v.y.SafeDivide(d.y, 1E-05f);
		v.z = v.z.SafeDivide(d.z, 1E-05f);
		return v;
	}

	// Token: 0x0600540A RID: 21514 RVA: 0x001B70DD File Offset: 0x001B52DD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Saturate(this float f, float eps = 1E-06f)
	{
		return Math.Min(Math.Max(f, 0f), 1f - eps);
	}

	// Token: 0x0600540B RID: 21515 RVA: 0x001B70F6 File Offset: 0x001B52F6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Sin(this Vector3 v)
	{
		v.x = Mathf.Sin(v.x);
		v.y = Mathf.Sin(v.y);
		v.z = Mathf.Sin(v.z);
		return v;
	}

	// Token: 0x0600540C RID: 21516 RVA: 0x001B712F File Offset: 0x001B532F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Quantize(this float f, float step)
	{
		return MathF.Round(f / step) * step;
	}

	// Token: 0x0600540D RID: 21517 RVA: 0x001B713B File Offset: 0x001B533B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this Quaternion a, Quaternion b, float epsilon = 1E-06f)
	{
		return Math.Abs(Quaternion.Dot(a, b)) > 1f - epsilon;
	}

	// Token: 0x0600540E RID: 21518 RVA: 0x001B7154 File Offset: 0x001B5354
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] BoxCorners(Vector3 center, Vector3 size)
	{
		Vector3 b = new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 b2 = new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 b3 = new Vector3(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + b + b2 + b3,
			center + b + b2 - b3,
			center - b + b2 - b3,
			center - b + b2 + b3,
			center + b - b2 + b3,
			center + b - b2 - b3,
			center - b - b2 - b3,
			center - b - b2 + b3
		};
	}

	// Token: 0x0600540F RID: 21519 RVA: 0x001B7290 File Offset: 0x001B5490
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void BoxCornersNonAlloc(Vector3 center, Vector3 size, Vector3[] array, int index = 0)
	{
		Vector3 b = new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 b2 = new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 b3 = new Vector3(0f, 0f, size.z * 0.5f);
		array[index] = center + b + b2 + b3;
		array[index + 1] = center + b + b2 - b3;
		array[index + 2] = center - b + b2 - b3;
		array[index + 3] = center - b + b2 + b3;
		array[index + 4] = center + b - b2 + b3;
		array[index + 5] = center + b - b2 - b3;
		array[index + 6] = center - b - b2 - b3;
		array[index + 7] = center - b - b2 + b3;
	}

	// Token: 0x06005410 RID: 21520 RVA: 0x001B73D4 File Offset: 0x001B55D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] OrientedBoxCorners(Vector3 center, Vector3 size, Quaternion angles)
	{
		Vector3 b = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 b2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 b3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + b + b2 + b3,
			center + b + b2 - b3,
			center - b + b2 - b3,
			center - b + b2 + b3,
			center + b - b2 + b3,
			center + b - b2 - b3,
			center - b - b2 - b3,
			center - b - b2 + b3
		};
	}

	// Token: 0x06005411 RID: 21521 RVA: 0x001B7520 File Offset: 0x001B5720
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void OrientedBoxCornersNonAlloc(Vector3 center, Vector3 size, Quaternion angles, Vector3[] array, int index = 0)
	{
		Vector3 b = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 b2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 b3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		array[index] = center + b + b2 + b3;
		array[index + 1] = center + b + b2 - b3;
		array[index + 2] = center - b + b2 - b3;
		array[index + 3] = center - b + b2 + b3;
		array[index + 4] = center + b - b2 + b3;
		array[index + 5] = center + b - b2 - b3;
		array[index + 6] = center - b - b2 - b3;
		array[index + 7] = center - b - b2 + b3;
	}

	// Token: 0x06005412 RID: 21522 RVA: 0x001B7674 File Offset: 0x001B5874
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool OrientedBoxContains(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Vector3 vector = Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one).inverse.MultiplyPoint3x4(point);
		Vector3 vector2 = boxSize * 0.5f;
		vector.x = Mathf.Abs(vector.x);
		vector.y = Mathf.Abs(vector.y);
		vector.z = Mathf.Abs(vector.z);
		return (Mathf.Approximately(vector.x, vector2.x) && Mathf.Approximately(vector.y, vector2.y) && Mathf.Approximately(vector.z, vector2.z)) || (vector.x < vector2.x && vector.y < vector2.y && vector.z < vector2.z);
	}

	// Token: 0x06005413 RID: 21523 RVA: 0x001B774C File Offset: 0x001B594C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int OrientedBoxSphereOverlap(Vector3 center, float radius, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Matrix4x4 matrix4x = Matrix4x4.Inverse(Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one));
		Vector3 vector = boxSize * 0.5f;
		Vector3 vector2 = matrix4x.MultiplyPoint3x4(center);
		Vector3 vector3 = Vector3.right * radius;
		float magnitude = matrix4x.MultiplyVector(vector3).magnitude;
		Vector3 vector4 = -vector;
		Vector3 b = vector2.Clamped(vector4, vector);
		if ((vector2 - b).sqrMagnitude > magnitude * magnitude)
		{
			return -1;
		}
		if (vector4.x + magnitude <= vector2.x && vector2.x <= vector.x - magnitude && vector.x - vector4.x > magnitude && vector4.y + magnitude <= vector2.y && vector2.y <= vector.y - magnitude && vector.y - vector4.y > magnitude && vector4.z + magnitude <= vector2.z && vector2.z <= vector.z - magnitude && vector.z - vector4.z > magnitude)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06005414 RID: 21524 RVA: 0x001B7874 File Offset: 0x001B5A74
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 Clamp(ref Vector3 v, ref Vector3 min, ref Vector3 max)
	{
		float num = v.x;
		num = ((num > max.x) ? max.x : num);
		num = ((num < min.x) ? min.x : num);
		float num2 = v.y;
		num2 = ((num2 > max.y) ? max.y : num2);
		num2 = ((num2 < min.y) ? min.y : num2);
		float num3 = v.z;
		num3 = ((num3 > max.z) ? max.z : num3);
		num3 = ((num3 < min.z) ? min.z : num3);
		return new Vector3(num, num2, num3);
	}

	// Token: 0x06005415 RID: 21525 RVA: 0x001B7910 File Offset: 0x001B5B10
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Bounds[] Subdivide(Bounds b, int x = 1, int y = 1, int z = 1)
	{
		if (x < 1)
		{
			x = 1;
		}
		if (y < 1)
		{
			y = 1;
		}
		if (z < 1)
		{
			z = 1;
		}
		int num = x * y * z;
		if (num == 1)
		{
			return new Bounds[]
			{
				b
			};
		}
		Vector3 size = b.size;
		float num2 = size.x * 0.5f;
		float num3 = size.y * 0.5f;
		float num4 = size.z * 0.5f;
		float num5 = size.x / (float)x;
		float num6 = size.y / (float)y;
		float num7 = size.z / (float)z;
		Vector3 size2 = new Vector3(num5, num6, num7);
		Bounds[] array = new Bounds[num];
		for (int i = 0; i < num; i++)
		{
			int num8;
			int num9;
			int num10;
			SpatialUtils.FlatIndexToXYZ(i, x, y, out num8, out num9, out num10);
			float num11 = num5 * (float)num8;
			float num12 = num5 * (float)(num8 + 1);
			float x2 = (num11 + num12) * 0.5f - num2;
			float num13 = num6 * (float)num9;
			float num14 = num6 * (float)(num9 + 1);
			float y2 = (num13 + num14) * 0.5f - num3;
			float num15 = num7 * (float)num10;
			float num16 = num7 * (float)(num10 + 1);
			float z2 = (num15 + num16) * 0.5f - num4;
			array[i].center = new Vector3(x2, y2, z2);
			array[i].size = size2;
		}
		return array;
	}

	// Token: 0x06005416 RID: 21526 RVA: 0x001B7A55 File Offset: 0x001B5C55
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ClampToReal(this float f, float min, float max, float epsilon = 1E-06f)
	{
		if (float.IsNaN(f))
		{
			f = 0f;
		}
		if (float.IsNegativeInfinity(min))
		{
			min = float.MinValue;
		}
		if (float.IsPositiveInfinity(max))
		{
			max = float.MaxValue;
		}
		return f.ClampApprox(min, max, epsilon);
	}

	// Token: 0x06005417 RID: 21527 RVA: 0x001B7A8D File Offset: 0x001B5C8D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ClampApprox(this float f, float min, float max, float epsilon = 1E-06f)
	{
		if (f < min || f.Approx(min, epsilon))
		{
			return min;
		}
		if (f > max || f.Approx(max, epsilon))
		{
			return max;
		}
		return f;
	}

	// Token: 0x06005418 RID: 21528 RVA: 0x001B7AB0 File Offset: 0x001B5CB0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx(this float a, float b, float epsilon = 1E-06f)
	{
		return Math.Abs(a - b) < epsilon;
	}

	// Token: 0x06005419 RID: 21529 RVA: 0x001B7ABD File Offset: 0x001B5CBD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx1(this float a, float epsilon = 1E-06f)
	{
		return Math.Abs(a - 1f) < epsilon;
	}

	// Token: 0x0600541A RID: 21530 RVA: 0x001B7ACE File Offset: 0x001B5CCE
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Approx0(this float a, float epsilon = 1E-06f)
	{
		return Math.Abs(a) < epsilon;
	}

	// Token: 0x0600541B RID: 21531 RVA: 0x001B7ADC File Offset: 0x001B5CDC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetScaledRadius(float radius, Vector3 scale)
	{
		float val = Math.Abs(scale.x);
		float val2 = Math.Abs(scale.y);
		float val3 = Math.Abs(scale.z);
		return Math.Max(Math.Abs(Math.Max(val, Math.Max(val2, val3)) * radius), 0f);
	}

	// Token: 0x0600541C RID: 21532 RVA: 0x001B7B2C File Offset: 0x001B5D2C
	public static float Linear(float value, float min, float max, float newMin, float newMax)
	{
		float num = (value - min) / (max - min) * (newMax - newMin) + newMin;
		if (num < newMin)
		{
			return newMin;
		}
		if (num > newMax)
		{
			return newMax;
		}
		return num;
	}

	// Token: 0x0600541D RID: 21533 RVA: 0x001B7B57 File Offset: 0x001B5D57
	public static float LinearUnclamped(float value, float min, float max, float newMin, float newMax)
	{
		return (value - min) / (max - min) * (newMax - newMin) + newMin;
	}

	// Token: 0x0600541E RID: 21534 RVA: 0x001B7B68 File Offset: 0x001B5D68
	public static float GetCircleValue(float degrees)
	{
		if (degrees > 90f)
		{
			degrees -= 180f;
		}
		else if (degrees < -90f)
		{
			degrees += 180f;
		}
		if (degrees > 180f)
		{
			degrees -= 270f;
		}
		else if (degrees < -180f)
		{
			degrees += 270f;
		}
		return degrees / 90f;
	}

	// Token: 0x0600541F RID: 21535 RVA: 0x001B7BC4 File Offset: 0x001B5DC4
	public static Vector3 WeightedMaxVector(Vector3 a, Vector3 b, float eps = 0.0001f)
	{
		float magnitude = a.magnitude;
		float magnitude2 = b.magnitude;
		if (magnitude < eps || magnitude2 < eps)
		{
			return Vector3.zero;
		}
		a / magnitude;
		b / magnitude2;
		Vector3 a2 = a * (magnitude / (magnitude + magnitude2)) + b * (magnitude2 / (magnitude + magnitude2));
		float d = Mathf.Max(magnitude, magnitude2);
		return a2 * d;
	}

	// Token: 0x06005420 RID: 21536 RVA: 0x001B7C28 File Offset: 0x001B5E28
	public static Vector3 MatchMagnitudeInDirection(Vector3 input, Vector3 target, float eps = 0.0001f)
	{
		Vector3 result = input;
		float magnitude = target.magnitude;
		if (magnitude > eps)
		{
			Vector3 vector = target / magnitude;
			float num = Vector3.Dot(input, vector);
			float num2 = magnitude - num;
			if (num2 > 0f)
			{
				result = input + num2 * vector;
			}
		}
		return result;
	}

	// Token: 0x06005421 RID: 21537 RVA: 0x001B7C74 File Offset: 0x001B5E74
	public static int CalculateAgeFromDateTime(DateTime Dob)
	{
		return new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
	}

	// Token: 0x06005422 RID: 21538 RVA: 0x001B7CA8 File Offset: 0x001B5EA8
	public static int PositiveModulo(this int x, int m)
	{
		int num = x % m;
		if (num >= 0)
		{
			return num;
		}
		return num + m;
	}

	// Token: 0x06005423 RID: 21539 RVA: 0x001B7CC4 File Offset: 0x001B5EC4
	public static float PositiveModulo(this float x, float m)
	{
		float num = x % m;
		if ((num < 0f && m > 0f) || (num > 0f && m < 0f))
		{
			num += m;
		}
		return num;
	}

	// Token: 0x04006502 RID: 25858
	private const float kDecay = 16f;

	// Token: 0x04006503 RID: 25859
	public const float kFloatEpsilon = 1E-06f;
}
