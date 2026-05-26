using System;
using Unity.Mathematics;

// Token: 0x0200037E RID: 894
[Serializable]
public struct GTSimpleNameID
{
	// Token: 0x060015C7 RID: 5575 RVA: 0x000734B8 File Offset: 0x000716B8
	static GTSimpleNameID()
	{
		if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".Length != 64 || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[0] != '0' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[9] != '9' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[10] != 'A' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[36] != 'a' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[62] != '_' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[63] != '-')
		{
			throw new Exception("GTSimpleNameID: The constant string `_k_possibleChars` does not match the expected format. Did you change something without updating the logic?");
		}
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x00073540 File Offset: 0x00071740
	public unsafe static GTSimpleNameID FromString(string input)
	{
		if (input == null)
		{
			input = string.Empty;
		}
		GTSimpleNameID result = default(GTSimpleNameID);
		int num = math.min(input.Length, 41);
		result.U0 = (ulong)((long)num & 63L);
		int num2 = 6;
		int i = 0;
		while (i < num)
		{
			char c = input[i];
			byte b;
			if (c >= 'A')
			{
				if (c >= 'a')
				{
					if (c > 'z')
					{
						goto IL_A7;
					}
					b = (byte)(c - 'a' + '$');
				}
				else if (c > 'Z')
				{
					if (c != '_')
					{
						goto IL_A7;
					}
					b = 62;
				}
				else
				{
					b = (byte)(c - 'A' + '\n');
				}
			}
			else if (c >= '0')
			{
				if (c > '9')
				{
					goto IL_A7;
				}
				b = (byte)(c - '0');
			}
			else
			{
				if (c != '-')
				{
					goto IL_A7;
				}
				b = 63;
			}
			ulong num3 = (ulong)b;
			int num4 = num2 + i * 6;
			ulong* ptr = &result.U0;
			int num5 = num4 / 64;
			int num6 = num4 % 64;
			ulong num7 = 63UL;
			ulong num8 = num3 & num7;
			ulong num9 = ~(num7 << num6);
			ptr[num5] &= num9;
			ptr[num5] |= num8 << num6;
			int num10 = 64 - num6;
			if (num10 < 6 && num5 < 3)
			{
				int num11 = 6 - num10;
				ulong num12 = (1UL << num11) - 1UL;
				ulong num13 = num8 >> num10;
				ptr[num5 + 1] &= ~num12;
				ptr[num5 + 1] |= num13;
			}
			i++;
			continue;
			IL_A7:
			throw new ArgumentException(string.Format("Invalid character '{0}' in input string.", c), "input");
		}
		return result;
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x000736C0 File Offset: 0x000718C0
	public override string ToString()
	{
		int num = math.min((int)(this.U0 & 63UL), 41);
		char[] array = new char[num];
		int num2 = 6;
		for (int i = 0; i < num; i++)
		{
			int bitOffset = num2 + i * 6;
			ulong num3 = GTSimpleNameID._Read6Bits(this, bitOffset);
			array[i] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[(int)num3];
		}
		return new string(array);
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x0007371C File Offset: 0x0007191C
	private unsafe static ulong _Read6Bits(in GTSimpleNameID cv, int bitOffset)
	{
		fixed (ulong* ptr = &cv.U0)
		{
			ulong* ptr2 = ptr;
			int num = bitOffset / 64;
			int num2 = bitOffset % 64;
			ulong num3 = ptr2[num] >> num2;
			int num4 = 64 - num2;
			if (num4 < 6 && num < 3)
			{
				int num5 = 6 - num4;
				ulong num6 = (1UL << num5) - 1UL;
				ulong num7 = ptr2[num + 1] & num6;
				num7 <<= num4;
				num3 |= num7;
			}
			return num3 & 63UL;
		}
	}

	// Token: 0x04001A97 RID: 6807
	public ulong U0;

	// Token: 0x04001A98 RID: 6808
	public ulong U1;

	// Token: 0x04001A99 RID: 6809
	public ulong U2;

	// Token: 0x04001A9A RID: 6810
	public ulong U3;

	// Token: 0x04001A9B RID: 6811
	private const string _k_possibleChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";

	// Token: 0x04001A9C RID: 6812
	private const int _k_maxLength = 41;

	// Token: 0x04001A9D RID: 6813
	private const ulong _k_bitmask6Bits = 63UL;

	// Token: 0x04001A9E RID: 6814
	private const ushort _k_indexOf_A = 10;

	// Token: 0x04001A9F RID: 6815
	private const ushort _k_indexOf_a = 36;

	// Token: 0x04001AA0 RID: 6816
	private const ushort _k_indexOf_underscore = 62;

	// Token: 0x04001AA1 RID: 6817
	private const ushort _k_indexOf_hyphen = 63;
}
