using System;
using System.Runtime.CompilerServices;

// Token: 0x0200033F RID: 831
public static class GTBitOps
{
	// Token: 0x06001463 RID: 5219 RVA: 0x0006D1E1 File Offset: 0x0006B3E1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetValueMask(int count)
	{
		return (1 << count) - 1;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x0006D1EB File Offset: 0x0006B3EB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMask(int index, int valueMask)
	{
		return ~(valueMask << index);
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x0006D1F4 File Offset: 0x0006B3F4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetClearMaskByCount(int index, int count)
	{
		return ~((1 << count) - 1 << index);
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0006D204 File Offset: 0x0006B404
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, int index, int valueMask)
	{
		return bits >> index & valueMask;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x0006D20E File Offset: 0x0006B40E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBits(int bits, GTBitOps.BitWriteInfo info)
	{
		return bits >> info.index & info.valueMask;
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0006D222 File Offset: 0x0006B422
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ReadBitsByCount(int bits, int index, int count)
	{
		return bits >> index & (1 << count) - 1;
	}

	// Token: 0x06001469 RID: 5225 RVA: 0x0006D233 File Offset: 0x0006B433
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ReadBit(int bits, int index)
	{
		return (bits >> index & 1) == 1;
	}

	// Token: 0x0600146A RID: 5226 RVA: 0x0006D240 File Offset: 0x0006B440
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, GTBitOps.BitWriteInfo info, int value)
	{
		bits = ((bits & info.clearMask) | (value & info.valueMask) << info.index);
	}

	// Token: 0x0600146B RID: 5227 RVA: 0x0006D260 File Offset: 0x0006B460
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, GTBitOps.BitWriteInfo info, int value)
	{
		GTBitOps.WriteBits(ref bits, info, value);
		return bits;
	}

	// Token: 0x0600146C RID: 5228 RVA: 0x0006D26C File Offset: 0x0006B46C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBits(ref int bits, int index, int valueMask, int clearMask, int value)
	{
		bits = ((bits & clearMask) | (value & valueMask) << index);
	}

	// Token: 0x0600146D RID: 5229 RVA: 0x0006D27E File Offset: 0x0006B47E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBits(int bits, int index, int valueMask, int clearMask, int value)
	{
		GTBitOps.WriteBits(ref bits, index, valueMask, clearMask, value);
		return bits;
	}

	// Token: 0x0600146E RID: 5230 RVA: 0x0006D28D File Offset: 0x0006B48D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBitsByCount(ref int bits, int index, int count, int value)
	{
		bits = ((bits & ~((1 << count) - 1 << index)) | (value & (1 << count) - 1) << index);
	}

	// Token: 0x0600146F RID: 5231 RVA: 0x0006D2B2 File Offset: 0x0006B4B2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBitsByCount(int bits, int index, int count, int value)
	{
		GTBitOps.WriteBitsByCount(ref bits, index, count, value);
		return bits;
	}

	// Token: 0x06001470 RID: 5232 RVA: 0x0006D2BF File Offset: 0x0006B4BF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(ref int bits, int index, bool value)
	{
		bits = ((bits & ~(1 << index)) | (value ? 1 : 0) << index);
	}

	// Token: 0x06001471 RID: 5233 RVA: 0x0006D2DA File Offset: 0x0006B4DA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteBit(int bits, int index, bool value)
	{
		GTBitOps.WriteBit(ref bits, index, value);
		return bits;
	}

	// Token: 0x06001472 RID: 5234 RVA: 0x0006D2E6 File Offset: 0x0006B4E6
	public static string ToBinaryString(int number)
	{
		return Convert.ToString(number, 2).PadLeft(32, '0');
	}

	// Token: 0x02000340 RID: 832
	public readonly struct BitWriteInfo
	{
		// Token: 0x06001473 RID: 5235 RVA: 0x0006D2F8 File Offset: 0x0006B4F8
		public BitWriteInfo(int index, int count)
		{
			this.index = index;
			this.valueMask = GTBitOps.GetValueMask(count);
			this.clearMask = GTBitOps.GetClearMask(index, this.valueMask);
		}

		// Token: 0x04001930 RID: 6448
		public readonly int index;

		// Token: 0x04001931 RID: 6449
		public readonly int valueMask;

		// Token: 0x04001932 RID: 6450
		public readonly int clearMask;
	}
}
