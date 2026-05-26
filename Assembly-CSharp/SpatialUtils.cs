using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000D7D RID: 3453
public static class SpatialUtils
{
	// Token: 0x060054A3 RID: 21667 RVA: 0x001B9698 File Offset: 0x001B7898
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int XYZToFlatIndex(int x, int y, int z, int xMax, int yMax)
	{
		return z * xMax * yMax + y * xMax + x;
	}

	// Token: 0x060054A4 RID: 21668 RVA: 0x001B96A6 File Offset: 0x001B78A6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int XYZToFlatIndex(Vector3Int xyz, int xMax, int yMax)
	{
		return xyz.z * xMax * yMax + xyz.y * xMax + xyz.x;
	}

	// Token: 0x060054A5 RID: 21669 RVA: 0x001B96C5 File Offset: 0x001B78C5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void FlatIndexToXYZ(int idx, int xMax, int yMax, out int x, out int y, out int z)
	{
		z = idx / (xMax * yMax);
		idx -= z * xMax * yMax;
		y = idx / xMax;
		x = idx % xMax;
	}

	// Token: 0x060054A6 RID: 21670 RVA: 0x001B96E8 File Offset: 0x001B78E8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3Int FlatIndexToXYZ(int idx, int xMax, int yMax)
	{
		int num = idx / (xMax * yMax);
		idx -= num * xMax * yMax;
		int y = idx / xMax;
		return new Vector3Int(idx % xMax, y, num);
	}

	// Token: 0x060054A7 RID: 21671 RVA: 0x001B9714 File Offset: 0x001B7914
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int CompareByZOrder(Vector3Int a, Vector3Int b)
	{
		ulong num;
		SpatialUtils.ZOrderEncode64((uint)a.x, (uint)a.y, (uint)a.z, out num);
		ulong value;
		SpatialUtils.ZOrderEncode64((uint)b.x, (uint)b.y, (uint)b.z, out value);
		return num.CompareTo(value);
	}

	// Token: 0x060054A8 RID: 21672 RVA: 0x001B9761 File Offset: 0x001B7961
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ZOrderEncode64(uint x, uint y, uint z, out ulong code)
	{
		code = (SpatialUtils.Encode64((ulong)x) | SpatialUtils.Encode64((ulong)y) << 1 | SpatialUtils.Encode64((ulong)z) << 2);
	}

	// Token: 0x060054A9 RID: 21673 RVA: 0x001B9780 File Offset: 0x001B7980
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ZOrderDecode64(ulong code, out uint x, out uint y, out uint z)
	{
		x = SpatialUtils.Decode64(code);
		y = SpatialUtils.Decode64(code >> 1);
		z = SpatialUtils.Decode64(code >> 2);
	}

	// Token: 0x060054AA RID: 21674 RVA: 0x001B97A0 File Offset: 0x001B79A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Encode64(ulong w)
	{
		w &= 2097151UL;
		w = ((w | w << 32) & 8725724278095871UL);
		w = ((w | w << 16) & 8725728556220671UL);
		w = ((w | w << 8) & 76280749732458511UL);
		w = ((w | w << 4) & 1207822528635744451UL);
		w = ((w | w << 2) & 1317624576693539401UL);
		return w;
	}

	// Token: 0x060054AB RID: 21675 RVA: 0x001B9810 File Offset: 0x001B7A10
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Decode64(ulong w)
	{
		w &= 1317624576693539401UL;
		w = ((w ^ w >> 2) & 3513665537849438403UL);
		w = ((w ^ w >> 4) & 17298045724797235215UL);
		w = ((w ^ w >> 8) & 71776123339407615UL);
		w = ((w ^ w >> 16) & 71776119061282815UL);
		w = ((w ^ w >> 32) & 2097151UL);
		return (uint)w;
	}

	// Token: 0x060054AC RID: 21676 RVA: 0x001B9880 File Offset: 0x001B7A80
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ZOrderEncode(uint x, uint y)
	{
		x = ((x | x << 16) & 65535U);
		x = ((x | x << 8) & 16711935U);
		x = ((x | x << 4) & 252645135U);
		x = ((x | x << 2) & 858993459U);
		x = ((x | x << 1) & 1431655765U);
		y = ((y | y << 16) & 65535U);
		y = ((y | y << 8) & 16711935U);
		y = ((y | y << 4) & 252645135U);
		y = ((y | y << 2) & 858993459U);
		y = ((y | y << 1) & 1431655765U);
		return x | y << 1;
	}

	// Token: 0x060054AD RID: 21677 RVA: 0x001B9918 File Offset: 0x001B7B18
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ZOrderDecode(uint code, out uint x, out uint y)
	{
		x = (code & 1431655765U);
		x = ((x ^ x >> 1) & 858993459U);
		x = ((x ^ x >> 2) & 252645135U);
		x = ((x ^ x >> 4) & 16711935U);
		x = ((x ^ x >> 8) & 65535U);
		y = (code >> 1 & 1431655765U);
		y = ((y ^ y >> 1) & 858993459U);
		y = ((y ^ y >> 2) & 252645135U);
		y = ((y ^ y >> 4) & 16711935U);
		y = ((y ^ y >> 8) & 65535U);
	}

	// Token: 0x060054AE RID: 21678 RVA: 0x001B99B4 File Offset: 0x001B7BB4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint ZOrderEncode(uint x, uint y, uint z)
	{
		x = ((x | x << 16) & 50331903U);
		x = ((x | x << 8) & 50393103U);
		x = ((x | x << 4) & 51130563U);
		x = ((x | x << 2) & 153391689U);
		y = ((y | y << 16) & 50331903U);
		y = ((y | y << 8) & 50393103U);
		y = ((y | y << 4) & 51130563U);
		y = ((y | y << 2) & 153391689U);
		z = ((z | z << 16) & 50331903U);
		z = ((z | z << 8) & 50393103U);
		z = ((z | z << 4) & 51130563U);
		z = ((z | z << 2) & 153391689U);
		return x | y << 1 | z << 2;
	}

	// Token: 0x060054AF RID: 21679 RVA: 0x001B9A6C File Offset: 0x001B7C6C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ZOrderDecode(uint code, out uint x, out uint y, out uint z)
	{
		x = (code & 153391689U);
		x = ((x ^ x >> 2) & 51130563U);
		x = ((x ^ x >> 4) & 50393103U);
		x = ((x ^ x >> 8) & 50331903U);
		x = ((x ^ x >> 16) & 1023U);
		y = (code >> 1 & 153391689U);
		y = ((y ^ y >> 2) & 51130563U);
		y = ((y ^ y >> 4) & 50393103U);
		y = ((y ^ y >> 8) & 50331903U);
		y = ((y ^ y >> 16) & 1023U);
		z = (code >> 2 & 153391689U);
		z = ((z ^ z >> 2) & 51130563U);
		z = ((z ^ z >> 4) & 50393103U);
		z = ((z ^ z >> 8) & 50331903U);
		z = ((z ^ z >> 16) & 1023U);
	}

	// Token: 0x060054B0 RID: 21680 RVA: 0x001B9B50 File Offset: 0x001B7D50
	public static bool TryGetBounds(IList<Renderer> renderers, out Bounds result)
	{
		result = default(Bounds);
		if (renderers == null)
		{
			return false;
		}
		int count = renderers.Count;
		if (count == 0)
		{
			return false;
		}
		Renderer renderer = null;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			Renderer renderer2 = renderers[i];
			if (renderer == null)
			{
				renderer = renderer2;
				if (renderer != null)
				{
					result = renderer.bounds;
					num++;
				}
			}
			else if (!(renderer2 == null))
			{
				Bounds bounds = renderer2.bounds;
				if (!(bounds.size == Vector3.zero))
				{
					result.Encapsulate(bounds);
					num++;
				}
			}
		}
		return num > 0;
	}

	// Token: 0x060054B1 RID: 21681 RVA: 0x001B9BEC File Offset: 0x001B7DEC
	public static bool TryGetBounds(IList<Collider> colliders, out Bounds result)
	{
		result = default(Bounds);
		if (colliders == null)
		{
			return false;
		}
		int count = colliders.Count;
		if (count == 0)
		{
			return false;
		}
		Collider collider = null;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			Collider collider2 = colliders[i];
			if (collider == null)
			{
				collider = collider2;
				if (collider != null)
				{
					result = collider.bounds;
					num++;
				}
			}
			else if (!(collider2 == null))
			{
				Bounds bounds = collider2.bounds;
				if (!(bounds.size == Vector3.zero))
				{
					result.Encapsulate(bounds);
					num++;
				}
			}
		}
		return num > 0;
	}

	// Token: 0x060054B2 RID: 21682 RVA: 0x001B9C88 File Offset: 0x001B7E88
	public static bool TryGetBounds(Transform x, out Bounds result, bool includeRenderers = true, bool includeColliders = true, bool fallbackToXforms = false)
	{
		result = default(Bounds);
		if (x == null)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		if (includeRenderers)
		{
			Bounds bounds;
			flag = SpatialUtils.TryGetBounds(x.GetComponentsInChildren<Renderer>(), out bounds);
			if (flag)
			{
				result = bounds;
			}
		}
		if (includeColliders)
		{
			Bounds bounds2;
			flag2 = SpatialUtils.TryGetBounds(x.GetComponentsInChildren<Collider>(), out bounds2);
			if (flag2)
			{
				if (flag)
				{
					result.Encapsulate(bounds2);
				}
				else
				{
					result = bounds2;
				}
			}
		}
		bool flag3 = flag || flag2;
		if (flag3 || !fallbackToXforms)
		{
			return flag3;
		}
		Transform[] componentsInChildren = x.GetComponentsInChildren<Transform>();
		result.center = componentsInChildren[0].position;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			result.Encapsulate(componentsInChildren[i].position);
		}
		return true;
	}

	// Token: 0x060054B3 RID: 21683 RVA: 0x001B9D34 File Offset: 0x001B7F34
	public static BoundingSphere GetRadialBounds(ref Bounds bounds, ref Matrix4x4 xform)
	{
		Vector3 center = bounds.center;
		Vector3 extents = bounds.extents;
		Vector3 b = new Vector3(extents.x, 0f, 0f);
		Vector3 b2 = new Vector3(0f, extents.y, 0f);
		Vector3 b3 = new Vector3(0f, 0f, extents.z);
		Vector3 a = xform.MultiplyPoint(center + b + b2 + b3);
		Vector3 vector = xform.MultiplyPoint(center + b + b2 - b3);
		Vector3 vector2 = xform.MultiplyPoint(center - b + b2 - b3);
		Vector3 vector3 = xform.MultiplyPoint(center - b + b2 + b3);
		Vector3 vector4 = xform.MultiplyPoint(center + b - b2 + b3);
		Vector3 vector5 = xform.MultiplyPoint(center + b - b2 - b3);
		Vector3 vector6 = xform.MultiplyPoint(center - b - b2 - b3);
		Vector3 vector7 = xform.MultiplyPoint(center - b - b2 + b3);
		Vector3 vector8 = (a + vector + vector2 + vector3 + vector4 + vector5 + vector6 + vector7) * 0.125f;
		float num = 0f;
		float num2 = SpatialUtils.DistSq(a, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector2, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector3, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector4, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector5, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector6, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		num2 = SpatialUtils.DistSq(vector7, vector8);
		if (num2 > num)
		{
			num = num2;
		}
		return new BoundingSphere(vector8, Mathf.Sqrt(num));
	}

	// Token: 0x060054B4 RID: 21684 RVA: 0x001B9F74 File Offset: 0x001B8174
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float DistSq(Vector3 a, Vector3 b)
	{
		float num = b.x - a.x;
		float num2 = b.y - a.y;
		float num3 = b.z - a.z;
		return num * num + num2 * num2 + num3 * num3;
	}

	// Token: 0x060054B5 RID: 21685 RVA: 0x001B9FB4 File Offset: 0x001B81B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] GetCorners(this Bounds b)
	{
		return SpatialUtils.GetCorners(b.min, b.max);
	}

	// Token: 0x060054B6 RID: 21686 RVA: 0x001B9FCC File Offset: 0x001B81CC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] GetCorners(Vector3 min, Vector3 max)
	{
		return new Vector3[]
		{
			new Vector3(min.x, max.y, max.z),
			new Vector3(max.x, max.y, max.z),
			new Vector3(max.x, min.y, max.z),
			new Vector3(min.x, min.y, max.z),
			new Vector3(min.x, max.y, min.z),
			new Vector3(max.x, max.y, min.z),
			new Vector3(max.x, min.y, min.z),
			new Vector3(min.x, min.y, min.z)
		};
	}

	// Token: 0x060054B7 RID: 21687 RVA: 0x001BA0D0 File Offset: 0x001B82D0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3[] GetCorners(this Bounds b, Matrix4x4 transform)
	{
		Vector3[] corners = b.GetCorners();
		for (int i = 0; i < corners.Length; i++)
		{
			corners[i] = transform.MultiplyPoint(corners[i]);
		}
		return corners;
	}

	// Token: 0x060054B8 RID: 21688 RVA: 0x001BA108 File Offset: 0x001B8308
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Bounds TransformedBy(this Bounds b, Matrix4x4 transform)
	{
		Vector3 position = transform.GetPosition();
		Vector3 a = transform.MultiplyVector(Vector3.right);
		Vector3 a2 = transform.MultiplyVector(Vector3.up);
		Vector3 a3 = transform.MultiplyVector(Vector3.forward);
		Vector3 min = b.min;
		Vector3 max = b.max;
		Vector3 lhs = a * min.x;
		Vector3 rhs = a * max.x;
		Vector3 lhs2 = a2 * min.y;
		Vector3 rhs2 = a2 * max.y;
		Vector3 lhs3 = a3 * min.z;
		Vector3 rhs3 = a3 * max.z;
		b.SetMinMax(Vector3.Min(lhs, rhs) + Vector3.Min(lhs2, rhs2) + Vector3.Min(lhs3, rhs3) + position, Vector3.Max(lhs, rhs) + Vector3.Max(lhs2, rhs2) + Vector3.Max(lhs3, rhs3) + position);
		return b;
	}

	// Token: 0x060054B9 RID: 21689 RVA: 0x001BA208 File Offset: 0x001B8408
	public static bool BoxIntersectsBox(ref Bounds a, ref Bounds b)
	{
		Vector3 min = a.min;
		Vector3 max = a.max;
		Vector3 min2 = b.min;
		Vector3 max2 = b.max;
		return min.x <= max2.x && min2.x <= max.x && min.y <= max2.y && min2.y <= max.y && min.z <= max2.z && min2.z <= max.z;
	}

	// Token: 0x060054BA RID: 21690 RVA: 0x001BA28C File Offset: 0x001B848C
	public static void ComputeBoundingSphere2Pass(Vector3[] points, out Vector3 center, out float radius)
	{
		center = default(Vector3);
		radius = 0f;
		if (points.IsNullOrEmpty<Vector3>())
		{
			return;
		}
		Bounds bounds = GeometryUtility.CalculateBounds(points, Matrix4x4.identity);
		Vector3 center2 = bounds.center;
		float num = (bounds.max - bounds.min).magnitude * 0.5f;
		if (num.Approx0(1E-06f))
		{
			num = 0f;
		}
		Vector3 vector;
		float num2;
		SpatialUtils.ComputeBoundingSphereRitter(points, out vector, out num2);
		bool flag = num < num2;
		center = (flag ? center2 : vector);
		radius = (flag ? num : num2);
	}

	// Token: 0x060054BB RID: 21691 RVA: 0x001BA324 File Offset: 0x001B8524
	public static void ComputeBoundingSphereRitter(Vector3[] points, out Vector3 center, out float radius)
	{
		center = default(Vector3);
		radius = 0f;
		if (points.IsNullOrEmpty<Vector3>())
		{
			return;
		}
		Vector3 vector = SpatialUtils.kMinVector;
		Vector3 vector2 = SpatialUtils.kMinVector;
		Vector3 vector3 = SpatialUtils.kMinVector;
		Vector3 vector4 = SpatialUtils.kMaxVector;
		Vector3 vector5 = SpatialUtils.kMaxVector;
		Vector3 vector6 = SpatialUtils.kMaxVector;
		foreach (Vector3 vector7 in points)
		{
			if (vector7.x < vector.x)
			{
				vector = vector7;
			}
			if (vector7.x > vector4.x)
			{
				vector4 = vector7;
			}
			if (vector7.y < vector2.y)
			{
				vector2 = vector7;
			}
			if (vector7.y > vector5.y)
			{
				vector5 = vector7;
			}
			if (vector7.z < vector3.z)
			{
				vector3 = vector7;
			}
			if (vector7.z > vector6.z)
			{
				vector6 = vector7;
			}
		}
		float num = vector4.x - vector.x;
		float num2 = vector4.y - vector.y;
		float num3 = vector4.z - vector.z;
		float num4 = num * num + num2 * num2 + num3 * num3;
		float num5 = vector5.x - vector2.x;
		num2 = vector5.y - vector2.y;
		num3 = vector5.z - vector2.z;
		float num6 = num5 * num5 + num2 * num2 + num3 * num3;
		float num7 = vector6.x - vector3.x;
		num2 = vector6.y - vector3.y;
		num3 = vector6.z - vector3.z;
		float num8 = num7 * num7 + num2 * num2 + num3 * num3;
		Vector3 vector8 = vector;
		Vector3 vector9 = vector4;
		float num9 = num4;
		if (num6 > num9)
		{
			num9 = num6;
			vector8 = vector2;
			vector9 = vector5;
		}
		if (num8 > num9)
		{
			vector8 = vector3;
			vector9 = vector6;
		}
		center = new Vector3((vector8.x + vector9.x) * 0.5f, (vector8.y + vector9.y) * 0.5f, (vector8.z + vector9.z) * 0.5f);
		float num10 = vector9.x - center.x;
		num2 = vector9.y - center.y;
		num3 = vector9.z - center.z;
		float num11 = num10 * num10 + num2 * num2 + num3 * num3;
		radius = Mathf.Sqrt(num11);
		foreach (Vector3 vector10 in points)
		{
			float num12 = vector10.x - center.x;
			num2 = vector10.y - center.y;
			num3 = vector10.z - center.z;
			float num13 = num12 * num12 + num2 * num2 + num3 * num3;
			if (num13 > num11)
			{
				float num14 = Mathf.Sqrt(num13);
				radius = (radius + num14) * 0.5f;
				num11 = radius * radius;
				float num15 = num14 - radius;
				center.x = (radius * center.x + num15 * vector10.x) / num14;
				center.y = (radius * center.y + num15 * vector10.y) / num14;
				center.z = (radius * center.z + num15 * vector10.z) / num14;
			}
		}
	}

	// Token: 0x0400654E RID: 25934
	private static readonly Vector3 kMinVector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

	// Token: 0x0400654F RID: 25935
	private static readonly Vector3 kMaxVector = new Vector3(float.MinValue, float.MinValue, float.MinValue);
}
