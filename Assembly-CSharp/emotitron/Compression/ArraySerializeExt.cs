using System;

namespace emotitron.Compression
{
	// Token: 0x02001305 RID: 4869
	public static class ArraySerializeExt
	{
		// Token: 0x060079D2 RID: 31186 RVA: 0x00280824 File Offset: 0x0027EA24
		public static void Zero(this byte[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060079D3 RID: 31187 RVA: 0x00280844 File Offset: 0x0027EA44
		public static void Zero(this byte[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060079D4 RID: 31188 RVA: 0x00280868 File Offset: 0x0027EA68
		public static void Zero(this byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060079D5 RID: 31189 RVA: 0x0028088C File Offset: 0x0027EA8C
		public static void Zero(this ushort[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060079D6 RID: 31190 RVA: 0x002808AC File Offset: 0x0027EAAC
		public static void Zero(this ushort[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060079D7 RID: 31191 RVA: 0x002808D0 File Offset: 0x0027EAD0
		public static void Zero(this ushort[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0;
			}
		}

		// Token: 0x060079D8 RID: 31192 RVA: 0x002808F4 File Offset: 0x0027EAF4
		public static void Zero(this uint[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060079D9 RID: 31193 RVA: 0x00280914 File Offset: 0x0027EB14
		public static void Zero(this uint[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060079DA RID: 31194 RVA: 0x00280938 File Offset: 0x0027EB38
		public static void Zero(this uint[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0U;
			}
		}

		// Token: 0x060079DB RID: 31195 RVA: 0x0028095C File Offset: 0x0027EB5C
		public static void Zero(this ulong[] buffer, int startByte, int endByte)
		{
			for (int i = startByte; i <= endByte; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060079DC RID: 31196 RVA: 0x0028097C File Offset: 0x0027EB7C
		public static void Zero(this ulong[] buffer, int startByte)
		{
			int num = buffer.Length;
			for (int i = startByte; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060079DD RID: 31197 RVA: 0x002809A0 File Offset: 0x0027EBA0
		public static void Zero(this ulong[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < num; i++)
			{
				buffer[i] = 0UL;
			}
		}

		// Token: 0x060079DE RID: 31198 RVA: 0x002809C4 File Offset: 0x0027EBC4
		public static void WriteSigned(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079DF RID: 31199 RVA: 0x002809E4 File Offset: 0x0027EBE4
		public static void WriteSigned(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079E0 RID: 31200 RVA: 0x00280A04 File Offset: 0x0027EC04
		public static void WriteSigned(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.Write((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079E1 RID: 31201 RVA: 0x00280A24 File Offset: 0x0027EC24
		public static void WriteSigned(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x060079E2 RID: 31202 RVA: 0x00280A44 File Offset: 0x0027EC44
		public static void WriteSigned(this uint[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x060079E3 RID: 31203 RVA: 0x00280A64 File Offset: 0x0027EC64
		public static void WriteSigned(this ulong[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.Write(value2, ref bitposition, bits);
		}

		// Token: 0x060079E4 RID: 31204 RVA: 0x00280A84 File Offset: 0x0027EC84
		public static int ReadSigned(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079E5 RID: 31205 RVA: 0x00280AA8 File Offset: 0x0027ECA8
		public static int ReadSigned(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079E6 RID: 31206 RVA: 0x00280ACC File Offset: 0x0027ECCC
		public static int ReadSigned(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.Read(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079E7 RID: 31207 RVA: 0x00280AF0 File Offset: 0x0027ECF0
		public static long ReadSigned64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x060079E8 RID: 31208 RVA: 0x00280B10 File Offset: 0x0027ED10
		public static long ReadSigned64(this uint[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x060079E9 RID: 31209 RVA: 0x00280B30 File Offset: 0x0027ED30
		public static long ReadSigned64(this ulong[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.Read(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}

		// Token: 0x060079EA RID: 31210 RVA: 0x00280B4F File Offset: 0x0027ED4F
		public static void WriteFloat(this byte[] buffer, float value, ref int bitposition)
		{
			buffer.Write((ulong)value.uint32, ref bitposition, 32);
		}

		// Token: 0x060079EB RID: 31211 RVA: 0x00280B66 File Offset: 0x0027ED66
		public static float ReadFloat(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 32);
		}

		// Token: 0x060079EC RID: 31212 RVA: 0x00280B7C File Offset: 0x0027ED7C
		public static void Append(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | value << i;
			buffer[num] = (byte)num3;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				buffer[num] = (byte)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x060079ED RID: 31213 RVA: 0x00280BD8 File Offset: 0x0027EDD8
		public static void Append(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = (1UL << i) - 1UL;
			ulong num3 = ((ulong)buffer[num] & num2) | value << i;
			buffer[num] = (uint)num3;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				buffer[num] = (uint)(value >> i);
			}
			bitposition += bits;
		}

		// Token: 0x060079EE RID: 31214 RVA: 0x00280C38 File Offset: 0x0027EE38
		public static void Append(this uint[] buffer, uint value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = ((ulong)buffer[num2] & num3) | (ulong)value << num;
			buffer[num2] = (uint)num4;
			buffer[num2 + 1] = (uint)(num4 >> 32);
			bitposition += bits;
		}

		// Token: 0x060079EF RID: 31215 RVA: 0x00280C84 File Offset: 0x0027EE84
		public static void Append(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			ulong num3 = (1UL << num) - 1UL;
			ulong num4 = (buffer[num2] & num3) | value << num;
			buffer[num2] = num4;
			buffer[num2 + 1] = value >> 64 - num;
			bitposition += bits;
		}

		// Token: 0x060079F0 RID: 31216 RVA: 0x00280CD0 File Offset: 0x0027EED0
		public static void Write(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 7;
			int num2 = bitposition >> 3;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 8 - num;
			for (i -= 8; i > 8; i -= 8)
			{
				num2++;
				num5 = value >> num;
				buffer[num2] = (byte)num5;
				num += 8;
			}
			if (i > 0)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (byte)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			}
			bitposition += bits;
		}

		// Token: 0x060079F1 RID: 31217 RVA: 0x00280D74 File Offset: 0x0027EF74
		public static void Write(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 31;
			int num2 = bitposition >> 5;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
			num = 32 - num;
			for (i -= 32; i > 32; i -= 32)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = (uint)(((ulong)buffer[num2] & ~num4) | (num5 & num4));
				num += 32;
			}
			bitposition += bits;
		}

		// Token: 0x060079F2 RID: 31218 RVA: 0x00280E08 File Offset: 0x0027F008
		public static void Write(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = bitposition & 63;
			int num2 = bitposition >> 6;
			int i = num + bits;
			ulong num3 = ulong.MaxValue >> 64 - bits;
			ulong num4 = num3 << num;
			ulong num5 = value << num;
			buffer[num2] = ((buffer[num2] & ~num4) | (num5 & num4));
			num = 64 - num;
			for (i -= 64; i > 64; i -= 64)
			{
				num2++;
				num4 = num3 >> num;
				num5 = value >> num;
				buffer[num2] = ((buffer[num2] & ~num4) | (num5 & num4));
				num += 64;
			}
			bitposition += bits;
		}

		// Token: 0x060079F3 RID: 31219 RVA: 0x00280E98 File Offset: 0x0027F098
		public static void WriteBool(this ulong[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060079F4 RID: 31220 RVA: 0x00280EAA File Offset: 0x0027F0AA
		public static void WriteBool(this uint[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060079F5 RID: 31221 RVA: 0x00280EBC File Offset: 0x0027F0BC
		public static void WriteBool(this byte[] buffer, bool b, ref int bitposition)
		{
			buffer.Write((ulong)(b ? 1L : 0L), ref bitposition, 1);
		}

		// Token: 0x060079F6 RID: 31222 RVA: 0x00280ED0 File Offset: 0x0027F0D0
		public static ulong Read(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 7;
			int num = bitposition >> 3;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 8 - i; i < bits; i += 8)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x060079F7 RID: 31223 RVA: 0x00280F2C File Offset: 0x0027F12C
		public static ulong Read(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 31;
			int num = bitposition >> 5;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = (ulong)buffer[num] >> i;
			for (i = 32 - i; i < bits; i += 32)
			{
				num++;
				num3 |= (ulong)buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x060079F8 RID: 31224 RVA: 0x00280F88 File Offset: 0x0027F188
		public static ulong Read(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int i = bitposition & 63;
			int num = bitposition >> 6;
			ulong num2 = ulong.MaxValue >> 64 - bits;
			ulong num3 = buffer[num] >> i;
			for (i = 64 - i; i < bits; i += 64)
			{
				num++;
				num3 |= buffer[num] << i;
			}
			bitposition += bits;
			return num3 & num2;
		}

		// Token: 0x060079F9 RID: 31225 RVA: 0x00280FE2 File Offset: 0x0027F1E2
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this byte[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x060079FA RID: 31226 RVA: 0x00280FEC File Offset: 0x0027F1EC
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this uint[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x060079FB RID: 31227 RVA: 0x00280FF6 File Offset: 0x0027F1F6
		[Obsolete("Just use Read(), it return a ulong already.")]
		public static ulong ReadUInt64(this ulong[] buffer, ref int bitposition, int bits = 64)
		{
			return buffer.Read(ref bitposition, bits);
		}

		// Token: 0x060079FC RID: 31228 RVA: 0x00281000 File Offset: 0x0027F200
		public static uint ReadUInt32(this byte[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x060079FD RID: 31229 RVA: 0x0028100B File Offset: 0x0027F20B
		public static uint ReadUInt32(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x060079FE RID: 31230 RVA: 0x00281016 File Offset: 0x0027F216
		public static uint ReadUInt32(this ulong[] buffer, ref int bitposition, int bits = 32)
		{
			return (uint)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x060079FF RID: 31231 RVA: 0x00281021 File Offset: 0x0027F221
		public static ushort ReadUInt16(this byte[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007A00 RID: 31232 RVA: 0x0028102C File Offset: 0x0027F22C
		public static ushort ReadUInt16(this uint[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007A01 RID: 31233 RVA: 0x00281037 File Offset: 0x0027F237
		public static ushort ReadUInt16(this ulong[] buffer, ref int bitposition, int bits = 16)
		{
			return (ushort)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007A02 RID: 31234 RVA: 0x00281042 File Offset: 0x0027F242
		public static byte ReadByte(this byte[] buffer, ref int bitposition, int bits = 8)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007A03 RID: 31235 RVA: 0x0028104D File Offset: 0x0027F24D
		public static byte ReadByte(this uint[] buffer, ref int bitposition, int bits = 32)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007A04 RID: 31236 RVA: 0x00281058 File Offset: 0x0027F258
		public static byte ReadByte(this ulong[] buffer, ref int bitposition, int bits)
		{
			return (byte)buffer.Read(ref bitposition, bits);
		}

		// Token: 0x06007A05 RID: 31237 RVA: 0x00281063 File Offset: 0x0027F263
		public static bool ReadBool(this ulong[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06007A06 RID: 31238 RVA: 0x00281074 File Offset: 0x0027F274
		public static bool ReadBool(this uint[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06007A07 RID: 31239 RVA: 0x00281085 File Offset: 0x0027F285
		public static bool ReadBool(this byte[] buffer, ref int bitposition)
		{
			return buffer.Read(ref bitposition, 1) == 1UL;
		}

		// Token: 0x06007A08 RID: 31240 RVA: 0x00281096 File Offset: 0x0027F296
		public static char ReadChar(this ulong[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06007A09 RID: 31241 RVA: 0x002810A2 File Offset: 0x0027F2A2
		public static char ReadChar(this uint[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06007A0A RID: 31242 RVA: 0x002810AE File Offset: 0x0027F2AE
		public static char ReadChar(this byte[] buffer, ref int bitposition)
		{
			return (char)buffer.Read(ref bitposition, 16);
		}

		// Token: 0x06007A0B RID: 31243 RVA: 0x002810BC File Offset: 0x0027F2BC
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
			bitposition += bits;
		}

		// Token: 0x06007A0C RID: 31244 RVA: 0x00281104 File Offset: 0x0027F304
		public static void ReadOutSafe(this ulong[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 64) ? 64 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
		}

		// Token: 0x06007A0D RID: 31245 RVA: 0x00281144 File Offset: 0x0027F344
		public static void ReadOutSafe(this byte[] source, int srcStartPos, ulong[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
		}

		// Token: 0x06007A0E RID: 31246 RVA: 0x00281184 File Offset: 0x0027F384
		public static void ReadOutSafe(this byte[] source, int srcStartPos, byte[] target, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int num = srcStartPos;
			int num2;
			for (int i = bits; i > 0; i -= num2)
			{
				num2 = ((i > 8) ? 8 : i);
				ulong value = source.Read(ref num, num2);
				target.Write(value, ref bitposition, num2);
			}
		}

		// Token: 0x06007A0F RID: 31247 RVA: 0x002811C4 File Offset: 0x0027F3C4
		public static ulong IndexAsUInt64(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (ulong)buffer[num] | (ulong)buffer[num + 1] << 8 | (ulong)buffer[num + 2] << 16 | (ulong)buffer[num + 3] << 24 | (ulong)buffer[num + 4] << 32 | (ulong)buffer[num + 5] << 40 | (ulong)buffer[num + 6] << 48 | (ulong)buffer[num + 7] << 56;
		}

		// Token: 0x06007A10 RID: 31248 RVA: 0x00281220 File Offset: 0x0027F420
		public static ulong IndexAsUInt64(this uint[] buffer, int index)
		{
			int num = index << 1;
			return (ulong)buffer[num] | (ulong)buffer[num + 1] << 32;
		}

		// Token: 0x06007A11 RID: 31249 RVA: 0x00281240 File Offset: 0x0027F440
		public static uint IndexAsUInt32(this byte[] buffer, int index)
		{
			int num = index << 3;
			return (uint)((int)buffer[num] | (int)buffer[num + 1] << 8 | (int)buffer[num + 2] << 16 | (int)buffer[num + 3] << 24);
		}

		// Token: 0x06007A12 RID: 31250 RVA: 0x00281270 File Offset: 0x0027F470
		public static uint IndexAsUInt32(this ulong[] buffer, int index)
		{
			int num = index >> 1;
			int num2 = (index & 1) << 5;
			return (uint)((byte)(buffer[num] >> num2));
		}

		// Token: 0x06007A13 RID: 31251 RVA: 0x00281290 File Offset: 0x0027F490
		public static byte IndexAsUInt8(this ulong[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 7) << 3;
			return (byte)(buffer[num] >> num2);
		}

		// Token: 0x06007A14 RID: 31252 RVA: 0x002812B0 File Offset: 0x0027F4B0
		public static byte IndexAsUInt8(this uint[] buffer, int index)
		{
			int num = index >> 3;
			int num2 = (index & 3) << 3;
			return (byte)((ulong)buffer[num] >> num2);
		}

		// Token: 0x04008C54 RID: 35924
		private const string bufferOverrunMsg = "Byte buffer length exceeded by write or read. Dataloss will occur. Likely due to a Read/Write mismatch.";
	}
}
