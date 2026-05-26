using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001397 RID: 5015
	public class VectorUtil
	{
		// Token: 0x06007E53 RID: 32339 RVA: 0x0029807C File Offset: 0x0029627C
		public static Vector3 Rotate2D(Vector3 v, float angle)
		{
			Vector3 result = v;
			float num = Mathf.Cos(angle);
			float num2 = Mathf.Sin(angle);
			result.x = num * v.x - num2 * v.y;
			result.y = num2 * v.x + num * v.y;
			return result;
		}

		// Token: 0x06007E54 RID: 32340 RVA: 0x002980CA File Offset: 0x002962CA
		public static Vector4 NormalizeSafe(Vector4 v, Vector4 fallback)
		{
			if (v.sqrMagnitude <= MathUtil.Epsilon)
			{
				return fallback;
			}
			return v.normalized;
		}

		// Token: 0x06007E55 RID: 32341 RVA: 0x002980E3 File Offset: 0x002962E3
		public static Vector3 FindOrthogonal(Vector3 v)
		{
			if (v.x >= MathUtil.Sqrt3Inv)
			{
				return new Vector3(v.y, -v.x, 0f);
			}
			return new Vector3(0f, v.z, -v.y);
		}

		// Token: 0x06007E56 RID: 32342 RVA: 0x00298121 File Offset: 0x00296321
		public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
		{
			a = VectorUtil.FindOrthogonal(v);
			b = Vector3.Cross(a, v);
		}

		// Token: 0x06007E57 RID: 32343 RVA: 0x00298144 File Offset: 0x00296344
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float num = Vector3.Dot(a, b);
			if (num > 0.99999f)
			{
				return Vector3.Lerp(a, b, t);
			}
			if (num < -0.99999f)
			{
				Vector3 axis = VectorUtil.FindOrthogonal(a);
				return Quaternion.AngleAxis(180f * t, axis) * a;
			}
			float num2 = MathUtil.AcosSafe(num);
			return (Mathf.Sin((1f - t) * num2) * a + Mathf.Sin(t * num2) * b) / Mathf.Sin(num2);
		}

		// Token: 0x06007E58 RID: 32344 RVA: 0x002981C8 File Offset: 0x002963C8
		public static Vector3 GetClosestPointOnSegment(Vector3 p, Vector3 segA, Vector3 segB)
		{
			Vector3 a = segB - segA;
			if (a.sqrMagnitude < MathUtil.Epsilon)
			{
				return 0.5f * (segA + segB);
			}
			float d = Mathf.Clamp01(Vector3.Dot(p - segA, a.normalized) / a.magnitude);
			return segA + d * a;
		}

		// Token: 0x06007E59 RID: 32345 RVA: 0x0029822C File Offset: 0x0029642C
		public static Vector3 TriLerp(ref Vector3 v000, ref Vector3 v001, ref Vector3 v010, ref Vector3 v011, ref Vector3 v100, ref Vector3 v101, ref Vector3 v110, ref Vector3 v111, float tx, float ty, float tz)
		{
			Vector3 a = Vector3.Lerp(v000, v001, tx);
			Vector3 b = Vector3.Lerp(v010, v011, tx);
			Vector3 a2 = Vector3.Lerp(v100, v101, tx);
			Vector3 b2 = Vector3.Lerp(v110, v111, tx);
			Vector3 a3 = Vector3.Lerp(a, b, ty);
			Vector3 b3 = Vector3.Lerp(a2, b2, ty);
			return Vector3.Lerp(a3, b3, tz);
		}

		// Token: 0x06007E5A RID: 32346 RVA: 0x002982A8 File Offset: 0x002964A8
		public static Vector3 TriLerp(ref Vector3 v000, ref Vector3 v001, ref Vector3 v010, ref Vector3 v011, ref Vector3 v100, ref Vector3 v101, ref Vector3 v110, ref Vector3 v111, bool lerpX, bool lerpY, bool lerpZ, float tx, float ty, float tz)
		{
			Vector3 vector = lerpX ? Vector3.Lerp(v000, v001, tx) : v000;
			Vector3 b = lerpX ? Vector3.Lerp(v010, v011, tx) : v010;
			Vector3 vector2 = lerpX ? Vector3.Lerp(v100, v101, tx) : v100;
			Vector3 b2 = lerpX ? Vector3.Lerp(v110, v111, tx) : v110;
			Vector3 vector3 = lerpY ? Vector3.Lerp(vector, b, ty) : vector;
			Vector3 b3 = lerpY ? Vector3.Lerp(vector2, b2, ty) : vector2;
			if (!lerpZ)
			{
				return vector3;
			}
			return Vector3.Lerp(vector3, b3, tz);
		}

		// Token: 0x06007E5B RID: 32347 RVA: 0x00298374 File Offset: 0x00296574
		public static Vector3 TriLerp(ref Vector3 min, ref Vector3 max, bool lerpX, bool lerpY, bool lerpZ, float tx, float ty, float tz)
		{
			Vector3 vector = lerpX ? Vector3.Lerp(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), tx) : new Vector3(min.x, min.y, min.z);
			Vector3 b = lerpX ? Vector3.Lerp(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), tx) : new Vector3(min.x, max.y, min.z);
			Vector3 vector2 = lerpX ? Vector3.Lerp(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z), tx) : new Vector3(min.x, min.y, max.z);
			Vector3 b2 = lerpX ? Vector3.Lerp(new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z), tx) : new Vector3(min.x, max.y, max.z);
			Vector3 vector3 = lerpY ? Vector3.Lerp(vector, b, ty) : vector;
			Vector3 b3 = lerpY ? Vector3.Lerp(vector2, b2, ty) : vector2;
			if (!lerpZ)
			{
				return vector3;
			}
			return Vector3.Lerp(vector3, b3, tz);
		}

		// Token: 0x06007E5C RID: 32348 RVA: 0x00298500 File Offset: 0x00296700
		public static Vector4 TriLerp(ref Vector4 v000, ref Vector4 v001, ref Vector4 v010, ref Vector4 v011, ref Vector4 v100, ref Vector4 v101, ref Vector4 v110, ref Vector4 v111, bool lerpX, bool lerpY, bool lerpZ, float tx, float ty, float tz)
		{
			Vector4 vector = lerpX ? Vector4.Lerp(v000, v001, tx) : v000;
			Vector4 b = lerpX ? Vector4.Lerp(v010, v011, tx) : v010;
			Vector4 vector2 = lerpX ? Vector4.Lerp(v100, v101, tx) : v100;
			Vector4 b2 = lerpX ? Vector4.Lerp(v110, v111, tx) : v110;
			Vector4 vector3 = lerpY ? Vector4.Lerp(vector, b, ty) : vector;
			Vector4 b3 = lerpY ? Vector4.Lerp(vector2, b2, ty) : vector2;
			if (!lerpZ)
			{
				return vector3;
			}
			return Vector4.Lerp(vector3, b3, tz);
		}

		// Token: 0x06007E5D RID: 32349 RVA: 0x002985CC File Offset: 0x002967CC
		public static Vector4 TriLerp(ref Vector4 min, ref Vector4 max, bool lerpX, bool lerpY, bool lerpZ, float tx, float ty, float tz)
		{
			Vector4 vector = lerpX ? Vector4.Lerp(new Vector4(min.x, min.y, min.z), new Vector4(max.x, min.y, min.z), tx) : new Vector4(min.x, min.y, min.z);
			Vector4 b = lerpX ? Vector4.Lerp(new Vector4(min.x, max.y, min.z), new Vector4(max.x, max.y, min.z), tx) : new Vector4(min.x, max.y, min.z);
			Vector4 vector2 = lerpX ? Vector4.Lerp(new Vector4(min.x, min.y, max.z), new Vector4(max.x, min.y, max.z), tx) : new Vector4(min.x, min.y, max.z);
			Vector4 b2 = lerpX ? Vector4.Lerp(new Vector4(min.x, max.y, max.z), new Vector4(max.x, max.y, max.z), tx) : new Vector4(min.x, max.y, max.z);
			Vector4 vector3 = lerpY ? Vector4.Lerp(vector, b, ty) : vector;
			Vector4 b3 = lerpY ? Vector4.Lerp(vector2, b2, ty) : vector2;
			if (!lerpZ)
			{
				return vector3;
			}
			return Vector4.Lerp(vector3, b3, tz);
		}

		// Token: 0x06007E5E RID: 32350 RVA: 0x00298758 File Offset: 0x00296958
		public static Vector3 ClampLength(Vector3 v, float minLen, float maxLen)
		{
			float sqrMagnitude = v.sqrMagnitude;
			if (sqrMagnitude < MathUtil.Epsilon)
			{
				return v;
			}
			float num = Mathf.Sqrt(sqrMagnitude);
			return v * (Mathf.Clamp(num, minLen, maxLen) / num);
		}

		// Token: 0x06007E5F RID: 32351 RVA: 0x0029878E File Offset: 0x0029698E
		public static float MinComponent(Vector3 v)
		{
			return Mathf.Min(v.x, Mathf.Min(v.y, v.z));
		}

		// Token: 0x06007E60 RID: 32352 RVA: 0x002987AC File Offset: 0x002969AC
		public static float MaxComponent(Vector3 v)
		{
			return Mathf.Max(v.x, Mathf.Max(v.y, v.z));
		}

		// Token: 0x06007E61 RID: 32353 RVA: 0x002987CA File Offset: 0x002969CA
		public static Vector3 ComponentWiseAbs(Vector3 v)
		{
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}

		// Token: 0x06007E62 RID: 32354 RVA: 0x002987F2 File Offset: 0x002969F2
		public static Vector3 ComponentWiseMult(Vector3 a, Vector3 b)
		{
			return Vector3.Scale(a, b);
		}

		// Token: 0x06007E63 RID: 32355 RVA: 0x002987FB File Offset: 0x002969FB
		public static Vector3 ComponentWiseDiv(Vector3 num, Vector3 den)
		{
			return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);
		}

		// Token: 0x06007E64 RID: 32356 RVA: 0x00298829 File Offset: 0x00296A29
		public static Vector3 ComponentWiseDivSafe(Vector3 num, Vector3 den)
		{
			return new Vector3(num.x * MathUtil.InvSafe(den.x), num.y * MathUtil.InvSafe(den.y), num.z * MathUtil.InvSafe(den.z));
		}

		// Token: 0x06007E65 RID: 32357 RVA: 0x00298868 File Offset: 0x00296A68
		public static Vector3 ClampBend(Vector3 vector, Vector3 reference, float maxBendAngle)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < MathUtil.Epsilon)
			{
				return vector;
			}
			float sqrMagnitude2 = reference.sqrMagnitude;
			if (sqrMagnitude2 < MathUtil.Epsilon)
			{
				return vector;
			}
			Vector3 rhs = vector / Mathf.Sqrt(sqrMagnitude);
			Vector3 vector2 = reference / Mathf.Sqrt(sqrMagnitude2);
			Vector3 vector3 = Vector3.Cross(vector2, rhs);
			float value = Vector3.Dot(vector2, rhs);
			Vector3 axis = (vector3.sqrMagnitude > MathUtil.Epsilon) ? vector3.normalized : VectorUtil.FindOrthogonal(vector2);
			if (Mathf.Acos(Mathf.Clamp01(value)) <= maxBendAngle)
			{
				return vector;
			}
			return QuaternionUtil.AxisAngle(axis, maxBendAngle) * reference * (Mathf.Sqrt(sqrMagnitude) / Mathf.Sqrt(sqrMagnitude2));
		}

		// Token: 0x04008F86 RID: 36742
		public static readonly Vector3 Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);

		// Token: 0x04008F87 RID: 36743
		public static readonly Vector3 Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
	}
}
