using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

// Token: 0x02000DA5 RID: 3493
public static class XXHash32
{
	// Token: 0x060055B1 RID: 21937 RVA: 0x001BE912 File Offset: 0x001BCB12
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Compute(string s, uint seed = 0U)
	{
		return (int)XXHash32.Compute(Encoding.Unicode.GetBytes(s), seed);
	}

	// Token: 0x060055B2 RID: 21938 RVA: 0x001BE92C File Offset: 0x001BCB2C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static uint Compute(ReadOnlySpan<byte> input, uint seed = 0U)
	{
		int length = input.Length;
		uint num9;
		if (length >= 16)
		{
			uint num = seed + 606290984U;
			uint num2 = seed + 2246822519U;
			uint num3 = seed;
			uint num4 = seed - 2654435761U;
			do
			{
				uint num5 = BinaryPrimitives.ReadUInt32LittleEndian(input);
				uint num6 = BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(4));
				uint num7 = BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(8));
				uint num8 = BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(12));
				num += num5 * 2246822519U;
				num = BitOperations.RotateLeft(num, 13);
				num *= 2654435761U;
				num2 += num6 * 2246822519U;
				num2 = BitOperations.RotateLeft(num2, 13);
				num2 *= 2654435761U;
				num3 += num7 * 2246822519U;
				num3 = BitOperations.RotateLeft(num3, 13);
				num3 *= 2654435761U;
				num4 += num8 * 2246822519U;
				num4 = BitOperations.RotateLeft(num4, 13);
				num4 *= 2654435761U;
				input = input.Slice(16);
			}
			while (input.Length >= 16);
			num9 = BitOperations.RotateLeft(num, 1) + BitOperations.RotateLeft(num2, 7) + BitOperations.RotateLeft(num3, 12) + BitOperations.RotateLeft(num4, 18);
		}
		else
		{
			num9 = seed + 374761393U;
		}
		num9 += (uint)length;
		while (input.Length >= 4)
		{
			num9 += BinaryPrimitives.ReadUInt32LittleEndian(input) * 3266489917U;
			num9 = BitOperations.RotateLeft(num9, 17) * 668265263U;
			input = input.Slice(4);
		}
		for (int i = 0; i < input.Length; i++)
		{
			num9 += (uint)(*input[i]) * 374761393U;
			num9 = BitOperations.RotateLeft(num9, 11) * 2654435761U;
		}
		num9 ^= num9 >> 15;
		num9 *= 2246822519U;
		num9 ^= num9 >> 13;
		num9 *= 3266489917U;
		return num9 ^ num9 >> 16;
	}

	// Token: 0x040065C6 RID: 26054
	private const uint Prime32_1 = 2654435761U;

	// Token: 0x040065C7 RID: 26055
	private const uint Prime32_2 = 2246822519U;

	// Token: 0x040065C8 RID: 26056
	private const uint Prime32_3 = 3266489917U;

	// Token: 0x040065C9 RID: 26057
	private const uint Prime32_4 = 668265263U;

	// Token: 0x040065CA RID: 26058
	private const uint Prime32_5 = 374761393U;

	// Token: 0x040065CB RID: 26059
	private const int StripeSize = 16;
}
