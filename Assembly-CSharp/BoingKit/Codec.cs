using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001385 RID: 4997
	public class Codec
	{
		// Token: 0x06007DAD RID: 32173 RVA: 0x00295FA4 File Offset: 0x002941A4
		public static float PackSaturated(float a, float b)
		{
			a = Mathf.Floor(a * 4095f);
			b = Mathf.Floor(b * 4095f);
			return a * 4096f + b;
		}

		// Token: 0x06007DAE RID: 32174 RVA: 0x00295FCB File Offset: 0x002941CB
		public static float PackSaturated(Vector2 v)
		{
			return Codec.PackSaturated(v.x, v.y);
		}

		// Token: 0x06007DAF RID: 32175 RVA: 0x00295FDE File Offset: 0x002941DE
		public static Vector2 UnpackSaturated(float f)
		{
			return new Vector2(Mathf.Floor(f / 4096f), Mathf.Repeat(f, 4096f)) / 4095f;
		}

		// Token: 0x06007DB0 RID: 32176 RVA: 0x00296008 File Offset: 0x00294208
		public static Vector2 OctWrap(Vector2 v)
		{
			return (Vector2.one - new Vector2(Mathf.Abs(v.y), Mathf.Abs(v.x))) * new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		// Token: 0x06007DB1 RID: 32177 RVA: 0x0029605C File Offset: 0x0029425C
		public static float PackNormal(Vector3 n)
		{
			n /= Mathf.Abs(n.x) + Mathf.Abs(n.y) + Mathf.Abs(n.z);
			return Codec.PackSaturated(((n.z >= 0f) ? new Vector2(n.x, n.y) : Codec.OctWrap(new Vector2(n.x, n.y))) * 0.5f + 0.5f * Vector2.one);
		}

		// Token: 0x06007DB2 RID: 32178 RVA: 0x002960F0 File Offset: 0x002942F0
		public static Vector3 UnpackNormal(float f)
		{
			Vector2 vector = Codec.UnpackSaturated(f);
			vector = vector * 2f - Vector2.one;
			Vector3 vector2 = new Vector3(vector.x, vector.y, 1f - Mathf.Abs(vector.x) - Mathf.Abs(vector.y));
			float num = Mathf.Clamp01(-vector2.z);
			vector2.x += ((vector2.x >= 0f) ? (-num) : num);
			vector2.y += ((vector2.y >= 0f) ? (-num) : num);
			return vector2.normalized;
		}

		// Token: 0x06007DB3 RID: 32179 RVA: 0x00296198 File Offset: 0x00294398
		public static uint PackRgb(Color color)
		{
			return (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x06007DB4 RID: 32180 RVA: 0x002961C8 File Offset: 0x002943C8
		public static Color UnpackRgb(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f);
		}

		// Token: 0x06007DB5 RID: 32181 RVA: 0x00296204 File Offset: 0x00294404
		public static uint PackRgba(Color color)
		{
			return (uint)(color.a * 255f) << 24 | (uint)(color.b * 255f) << 16 | (uint)(color.g * 255f) << 8 | (uint)(color.r * 255f);
		}

		// Token: 0x06007DB6 RID: 32182 RVA: 0x00296250 File Offset: 0x00294450
		public static Color UnpackRgba(uint i)
		{
			return new Color((i & 255U) / 255f, ((i & 65280U) >> 8) / 255f, ((i & 16711680U) >> 16) / 255f, ((i & 4278190080U) >> 24) / 255f);
		}

		// Token: 0x06007DB7 RID: 32183 RVA: 0x002962A6 File Offset: 0x002944A6
		public static uint Pack8888(uint x, uint y, uint z, uint w)
		{
			return (x & 255U) << 24 | (y & 255U) << 16 | (z & 255U) << 8 | (w & 255U);
		}

		// Token: 0x06007DB8 RID: 32184 RVA: 0x002962CF File Offset: 0x002944CF
		public static void Unpack8888(uint i, out uint x, out uint y, out uint z, out uint w)
		{
			x = (i >> 24 & 255U);
			y = (i >> 16 & 255U);
			z = (i >> 8 & 255U);
			w = (i & 255U);
		}

		// Token: 0x06007DB9 RID: 32185 RVA: 0x00296300 File Offset: 0x00294500
		private static int IntReinterpret(float f)
		{
			return new Codec.IntFloat
			{
				FloatValue = f
			}.IntValue;
		}

		// Token: 0x06007DBA RID: 32186 RVA: 0x00296323 File Offset: 0x00294523
		public static int HashConcat(int hash, int i)
		{
			return (hash ^ i) * Codec.FnvPrime;
		}

		// Token: 0x06007DBB RID: 32187 RVA: 0x0029632E File Offset: 0x0029452E
		public static int HashConcat(int hash, long i)
		{
			hash = Codec.HashConcat(hash, (int)(i & (long)((ulong)-1)));
			hash = Codec.HashConcat(hash, (int)(i >> 32));
			return hash;
		}

		// Token: 0x06007DBC RID: 32188 RVA: 0x0029634B File Offset: 0x0029454B
		public static int HashConcat(int hash, float f)
		{
			return Codec.HashConcat(hash, Codec.IntReinterpret(f));
		}

		// Token: 0x06007DBD RID: 32189 RVA: 0x00296359 File Offset: 0x00294559
		public static int HashConcat(int hash, bool b)
		{
			return Codec.HashConcat(hash, b ? 1 : 0);
		}

		// Token: 0x06007DBE RID: 32190 RVA: 0x00296368 File Offset: 0x00294568
		public static int HashConcat(int hash, params int[] ints)
		{
			foreach (int i2 in ints)
			{
				hash = Codec.HashConcat(hash, i2);
			}
			return hash;
		}

		// Token: 0x06007DBF RID: 32191 RVA: 0x00296394 File Offset: 0x00294594
		public static int HashConcat(int hash, params float[] floats)
		{
			foreach (float f in floats)
			{
				hash = Codec.HashConcat(hash, f);
			}
			return hash;
		}

		// Token: 0x06007DC0 RID: 32192 RVA: 0x002963BF File Offset: 0x002945BF
		public static int HashConcat(int hash, Vector2 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y
			});
		}

		// Token: 0x06007DC1 RID: 32193 RVA: 0x002963DF File Offset: 0x002945DF
		public static int HashConcat(int hash, Vector3 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z
			});
		}

		// Token: 0x06007DC2 RID: 32194 RVA: 0x00296408 File Offset: 0x00294608
		public static int HashConcat(int hash, Vector4 v)
		{
			return Codec.HashConcat(hash, new float[]
			{
				v.x,
				v.y,
				v.z,
				v.w
			});
		}

		// Token: 0x06007DC3 RID: 32195 RVA: 0x0029643A File Offset: 0x0029463A
		public static int HashConcat(int hash, Quaternion q)
		{
			return Codec.HashConcat(hash, new float[]
			{
				q.x,
				q.y,
				q.z,
				q.w
			});
		}

		// Token: 0x06007DC4 RID: 32196 RVA: 0x0029646C File Offset: 0x0029466C
		public static int HashConcat(int hash, Color c)
		{
			return Codec.HashConcat(hash, new float[]
			{
				c.r,
				c.g,
				c.b,
				c.a
			});
		}

		// Token: 0x06007DC5 RID: 32197 RVA: 0x0029649E File Offset: 0x0029469E
		public static int HashConcat(int hash, Transform t)
		{
			return Codec.HashConcat(hash, t.GetHashCode());
		}

		// Token: 0x06007DC6 RID: 32198 RVA: 0x002964AC File Offset: 0x002946AC
		public static int Hash(int i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06007DC7 RID: 32199 RVA: 0x002964B9 File Offset: 0x002946B9
		public static int Hash(long i)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, i);
		}

		// Token: 0x06007DC8 RID: 32200 RVA: 0x002964C6 File Offset: 0x002946C6
		public static int Hash(float f)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, f);
		}

		// Token: 0x06007DC9 RID: 32201 RVA: 0x002964D3 File Offset: 0x002946D3
		public static int Hash(bool b)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, b);
		}

		// Token: 0x06007DCA RID: 32202 RVA: 0x002964E0 File Offset: 0x002946E0
		public static int Hash(params int[] ints)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, ints);
		}

		// Token: 0x06007DCB RID: 32203 RVA: 0x002964ED File Offset: 0x002946ED
		public static int Hash(params float[] floats)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, floats);
		}

		// Token: 0x06007DCC RID: 32204 RVA: 0x002964FA File Offset: 0x002946FA
		public static int Hash(Vector2 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06007DCD RID: 32205 RVA: 0x00296507 File Offset: 0x00294707
		public static int Hash(Vector3 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06007DCE RID: 32206 RVA: 0x00296514 File Offset: 0x00294714
		public static int Hash(Vector4 v)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, v);
		}

		// Token: 0x06007DCF RID: 32207 RVA: 0x00296521 File Offset: 0x00294721
		public static int Hash(Quaternion q)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, q);
		}

		// Token: 0x06007DD0 RID: 32208 RVA: 0x0029652E File Offset: 0x0029472E
		public static int Hash(Color c)
		{
			return Codec.HashConcat(Codec.FnvDefaultBasis, c);
		}

		// Token: 0x06007DD1 RID: 32209 RVA: 0x0029653C File Offset: 0x0029473C
		private static int HashTransformHierarchyRecurvsive(int hash, Transform t)
		{
			hash = Codec.HashConcat(hash, t);
			hash = Codec.HashConcat(hash, t.childCount);
			for (int i = 0; i < t.childCount; i++)
			{
				hash = Codec.HashTransformHierarchyRecurvsive(hash, t.GetChild(i));
			}
			return hash;
		}

		// Token: 0x06007DD2 RID: 32210 RVA: 0x00296581 File Offset: 0x00294781
		public static int HashTransformHierarchy(Transform t)
		{
			return Codec.HashTransformHierarchyRecurvsive(Codec.FnvDefaultBasis, t);
		}

		// Token: 0x04008F4A RID: 36682
		public static readonly int FnvDefaultBasis = -2128831035;

		// Token: 0x04008F4B RID: 36683
		public static readonly int FnvPrime = 16777619;

		// Token: 0x02001386 RID: 4998
		[StructLayout(LayoutKind.Explicit)]
		private struct IntFloat
		{
			// Token: 0x04008F4C RID: 36684
			[FieldOffset(0)]
			public int IntValue;

			// Token: 0x04008F4D RID: 36685
			[FieldOffset(0)]
			public float FloatValue;
		}
	}
}
