using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AEC RID: 2796
[Serializable]
public struct SRand
{
	// Token: 0x06004756 RID: 18262 RVA: 0x001805E3 File Offset: 0x0017E7E3
	public SRand(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06004757 RID: 18263 RVA: 0x001805E3 File Offset: 0x0017E7E3
	public SRand(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06004758 RID: 18264 RVA: 0x001805F8 File Offset: 0x0017E7F8
	public SRand(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06004759 RID: 18265 RVA: 0x00180612 File Offset: 0x0017E812
	public SRand(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600475A RID: 18266 RVA: 0x0018062C File Offset: 0x0017E82C
	public SRand(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600475B RID: 18267 RVA: 0x0018065E File Offset: 0x0017E85E
	public SRand(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x0600475C RID: 18268 RVA: 0x0018068F File Offset: 0x0017E88F
	public double NextDouble()
	{
		return this.NextState() % 268435457U * 3.725290298461914E-09;
	}

	// Token: 0x0600475D RID: 18269 RVA: 0x001806A9 File Offset: 0x0017E8A9
	public double NextDouble(double max)
	{
		if (max < 0.0)
		{
			return 0.0;
		}
		return this.NextDouble() * max;
	}

	// Token: 0x0600475E RID: 18270 RVA: 0x001806CC File Offset: 0x0017E8CC
	public double NextDouble(double min, double max)
	{
		double num = max - min;
		if (num <= 0.0)
		{
			return min;
		}
		double num2 = this.NextDouble() * num;
		return min + num2;
	}

	// Token: 0x0600475F RID: 18271 RVA: 0x001806F7 File Offset: 0x0017E8F7
	public float NextFloat()
	{
		return (float)this.NextDouble();
	}

	// Token: 0x06004760 RID: 18272 RVA: 0x00180700 File Offset: 0x0017E900
	public float NextFloat(float max)
	{
		return (float)this.NextDouble((double)max);
	}

	// Token: 0x06004761 RID: 18273 RVA: 0x0018070B File Offset: 0x0017E90B
	public float NextFloat(float min, float max)
	{
		return (float)this.NextDouble((double)min, (double)max);
	}

	// Token: 0x06004762 RID: 18274 RVA: 0x00180718 File Offset: 0x0017E918
	public bool NextBool()
	{
		return this.NextState() % 2U == 1U;
	}

	// Token: 0x06004763 RID: 18275 RVA: 0x00180725 File Offset: 0x0017E925
	public uint NextUInt()
	{
		return this.NextState();
	}

	// Token: 0x06004764 RID: 18276 RVA: 0x00180725 File Offset: 0x0017E925
	public int NextInt()
	{
		return (int)this.NextState();
	}

	// Token: 0x06004765 RID: 18277 RVA: 0x0018072D File Offset: 0x0017E92D
	public int NextInt(int max)
	{
		if (max <= 0)
		{
			return 0;
		}
		return (int)((ulong)this.NextState() % (ulong)((long)max));
	}

	// Token: 0x06004766 RID: 18278 RVA: 0x00180740 File Offset: 0x0017E940
	public int NextInt(int min, int max)
	{
		int num = max - min;
		if (num <= 0)
		{
			return min;
		}
		return min + this.NextInt(num);
	}

	// Token: 0x06004767 RID: 18279 RVA: 0x00180760 File Offset: 0x0017E960
	public int NextIntWithExclusion(int min, int max, int exclude)
	{
		int num = max - min - 1;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 1 + this.NextInt(num);
		if (num2 > exclude)
		{
			return num2;
		}
		return num2 - 1;
	}

	// Token: 0x06004768 RID: 18280 RVA: 0x00180790 File Offset: 0x0017E990
	public int NextIntWithExclusion2(int min, int max, int exclude, int exclude2)
	{
		if (exclude == exclude2)
		{
			return this.NextIntWithExclusion(min, max, exclude);
		}
		int num = max - min - 2;
		if (num <= 0)
		{
			return min;
		}
		int num2 = min + 2 + this.NextInt(num);
		int num3;
		int num4;
		if (exclude >= exclude2)
		{
			num3 = exclude2 + 1;
			num4 = exclude;
		}
		else
		{
			num3 = exclude + 1;
			num4 = exclude2;
		}
		if (num2 <= num3)
		{
			return num2 - 2;
		}
		if (num2 <= num4)
		{
			return num2 - 1;
		}
		return num2;
	}

	// Token: 0x06004769 RID: 18281 RVA: 0x001807F6 File Offset: 0x0017E9F6
	public byte NextByte()
	{
		return (byte)(this.NextState() & 255U);
	}

	// Token: 0x0600476A RID: 18282 RVA: 0x00180808 File Offset: 0x0017EA08
	public Color32 NextColor32()
	{
		byte r = this.NextByte();
		byte g = this.NextByte();
		byte b = this.NextByte();
		return new Color32(r, g, b, byte.MaxValue);
	}

	// Token: 0x0600476B RID: 18283 RVA: 0x00180838 File Offset: 0x0017EA38
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointInsideSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = MathF.Pow(this.NextFloat(), 0.33333334f);
		float num5 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num5 * num4 * radius, num2 * num5 * num4 * radius, num3 * num5 * num4 * radius);
	}

	// Token: 0x0600476C RID: 18284 RVA: 0x001808C4 File Offset: 0x0017EAC4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointOnSphere(float radius)
	{
		float num = this.NextFloat() * 2f - 1f;
		float num2 = this.NextFloat() * 2f - 1f;
		float num3 = this.NextFloat() * 2f - 1f;
		float num4 = 1f / MathF.Sqrt(num * num + num2 * num2 + num3 * num3);
		return new Vector3(num * num4 * radius, num2 * num4 * radius, num3 * num4 * radius);
	}

	// Token: 0x0600476D RID: 18285 RVA: 0x00180938 File Offset: 0x0017EB38
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 NextPointInsideBox(Vector3 extents)
	{
		float num = this.NextFloat() - 0.5f;
		float num2 = this.NextFloat() - 0.5f;
		float num3 = this.NextFloat() - 0.5f;
		return new Vector3(num * extents.x, num2 * extents.y, num3 * extents.z);
	}

	// Token: 0x0600476E RID: 18286 RVA: 0x00180988 File Offset: 0x0017EB88
	public Color NextColor()
	{
		float r = this.NextFloat();
		float g = this.NextFloat();
		float b = this.NextFloat();
		return new Color(r, g, b, 1f);
	}

	// Token: 0x0600476F RID: 18287 RVA: 0x001809B8 File Offset: 0x0017EBB8
	public void Shuffle<T>(T[] array)
	{
		int i = array.Length;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int num2 = i;
			int num3 = num;
			T t = array[num];
			T t2 = array[i];
			array[num2] = t;
			array[num3] = t2;
		}
	}

	// Token: 0x06004770 RID: 18288 RVA: 0x00180A08 File Offset: 0x0017EC08
	public void Shuffle<T>(List<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			int num = this.NextInt(i--);
			int index = i;
			int index2 = num;
			T value = list[num];
			T value2 = list[i];
			list[index] = value;
			list[index2] = value2;
		}
	}

	// Token: 0x06004771 RID: 18289 RVA: 0x00180A60 File Offset: 0x0017EC60
	public void Reset()
	{
		this._state = this._seed;
	}

	// Token: 0x06004772 RID: 18290 RVA: 0x001805E3 File Offset: 0x0017E7E3
	public void Reset(int seed)
	{
		this._seed = (uint)seed;
		this._state = this._seed;
	}

	// Token: 0x06004773 RID: 18291 RVA: 0x001805E3 File Offset: 0x0017E7E3
	public void Reset(uint seed)
	{
		this._seed = seed;
		this._state = this._seed;
	}

	// Token: 0x06004774 RID: 18292 RVA: 0x001805F8 File Offset: 0x0017E7F8
	public void Reset(long seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06004775 RID: 18293 RVA: 0x00180612 File Offset: 0x0017E812
	public void Reset(DateTime seed)
	{
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06004776 RID: 18294 RVA: 0x0018062C File Offset: 0x0017E82C
	public void Reset(string seed)
	{
		if (string.IsNullOrEmpty(seed))
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06004777 RID: 18295 RVA: 0x0018065E File Offset: 0x0017E85E
	public void Reset(byte[] seed)
	{
		if (seed == null || seed.Length == 0)
		{
			throw new ArgumentException("Seed cannot be null or empty", "seed");
		}
		this._seed = (uint)StaticHash.Compute(seed);
		this._state = this._seed;
	}

	// Token: 0x06004778 RID: 18296 RVA: 0x00180A70 File Offset: 0x0017EC70
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint NextState()
	{
		return this._state = this.Mix(this._state + 184402071U);
	}

	// Token: 0x06004779 RID: 18297 RVA: 0x00180A98 File Offset: 0x0017EC98
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint Mix(uint x)
	{
		x = (x >> 17 ^ x) * 3982152891U;
		x = (x >> 11 ^ x) * 2890668881U;
		x = (x >> 15 ^ x) * 830770091U;
		x = (x >> 14 ^ x);
		return x;
	}

	// Token: 0x0600477A RID: 18298 RVA: 0x00180ACD File Offset: 0x0017ECCD
	public override int GetHashCode()
	{
		return StaticHash.Compute((int)this._seed, (int)this._state);
	}

	// Token: 0x0600477B RID: 18299 RVA: 0x00180AE0 File Offset: 0x0017ECE0
	public override string ToString()
	{
		return string.Format("{0} {{ {1}: {2:X8} {3}: {4:X8} }}", new object[]
		{
			"SRand",
			"_seed",
			this._seed,
			"_state",
			this._state
		});
	}

	// Token: 0x0600477C RID: 18300 RVA: 0x00180B31 File Offset: 0x0017ED31
	public static SRand New()
	{
		return new SRand(DateTime.UtcNow);
	}

	// Token: 0x0600477D RID: 18301 RVA: 0x00180B3D File Offset: 0x0017ED3D
	public static explicit operator SRand(int seed)
	{
		return new SRand(seed);
	}

	// Token: 0x0600477E RID: 18302 RVA: 0x00180B45 File Offset: 0x0017ED45
	public static explicit operator SRand(uint seed)
	{
		return new SRand(seed);
	}

	// Token: 0x0600477F RID: 18303 RVA: 0x00180B4D File Offset: 0x0017ED4D
	public static explicit operator SRand(long seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004780 RID: 18304 RVA: 0x00180B55 File Offset: 0x0017ED55
	public static explicit operator SRand(string seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004781 RID: 18305 RVA: 0x00180B5D File Offset: 0x0017ED5D
	public static explicit operator SRand(byte[] seed)
	{
		return new SRand(seed);
	}

	// Token: 0x06004782 RID: 18306 RVA: 0x00180B65 File Offset: 0x0017ED65
	public static explicit operator SRand(DateTime seed)
	{
		return new SRand(seed);
	}

	// Token: 0x040059C2 RID: 22978
	[SerializeField]
	private uint _seed;

	// Token: 0x040059C3 RID: 22979
	[SerializeField]
	private uint _state;

	// Token: 0x040059C4 RID: 22980
	private const double MAX_AS_DOUBLE = 268435456.0;

	// Token: 0x040059C5 RID: 22981
	private const uint MAX_PLUS_ONE = 268435457U;

	// Token: 0x040059C6 RID: 22982
	private const double STEP_SIZE = 3.725290298461914E-09;

	// Token: 0x040059C7 RID: 22983
	private const float ONE_THIRD = 0.33333334f;
}
