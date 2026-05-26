using System;

namespace emotitron.Compression
{
	// Token: 0x0200130A RID: 4874
	public static class PrimitivePackBitsExt
	{
		// Token: 0x06007A51 RID: 31313 RVA: 0x00281DE4 File Offset: 0x0027FFE4
		public static ulong WritePackedBits(this ulong buffer, uint value, ref int bitposition, int bits)
		{
			int bits2 = ((uint)bits).UsedBitCount();
			int num = value.UsedBitCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num);
			return buffer;
		}

		// Token: 0x06007A52 RID: 31314 RVA: 0x00281E18 File Offset: 0x00280018
		public static uint WritePackedBits(this uint buffer, ushort value, ref int bitposition, int bits)
		{
			int bits2 = ((uint)bits).UsedBitCount();
			int num = value.UsedBitCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num);
			return buffer;
		}

		// Token: 0x06007A53 RID: 31315 RVA: 0x00281E4C File Offset: 0x0028004C
		public static ushort WritePackedBits(this ushort buffer, byte value, ref int bitposition, int bits)
		{
			int bits2 = ((uint)bits).UsedBitCount();
			int num = value.UsedBitCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num);
			return buffer;
		}

		// Token: 0x06007A54 RID: 31316 RVA: 0x00281E80 File Offset: 0x00280080
		public static ulong ReadPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06007A55 RID: 31317 RVA: 0x00281EA8 File Offset: 0x002800A8
		public static ulong ReadPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return (ulong)buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06007A56 RID: 31318 RVA: 0x00281ED0 File Offset: 0x002800D0
		public static ulong ReadPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			int bits2 = bits.UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2);
			return (ulong)buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x06007A57 RID: 31319 RVA: 0x00281EF8 File Offset: 0x002800F8
		public static ulong WriteSignedPackedBits(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint value2 = (uint)(value << 1 ^ value >> 31);
			buffer = buffer.WritePackedBits(value2, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06007A58 RID: 31320 RVA: 0x00281F1C File Offset: 0x0028011C
		public static uint WriteSignedPackedBits(this uint buffer, short value, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			buffer = buffer.WritePackedBits((ushort)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06007A59 RID: 31321 RVA: 0x00281F40 File Offset: 0x00280140
		public static ushort WriteSignedPackedBits(this ushort buffer, sbyte value, ref int bitposition, int bits)
		{
			uint num = (uint)((int)value << 1 ^ value >> 31);
			buffer = buffer.WritePackedBits((byte)num, ref bitposition, bits);
			return buffer;
		}

		// Token: 0x06007A5A RID: 31322 RVA: 0x00281F64 File Offset: 0x00280164
		public static int ReadSignedPackedBits(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x06007A5B RID: 31323 RVA: 0x00281F88 File Offset: 0x00280188
		public static short ReadSignedPackedBits(this uint buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (short)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}

		// Token: 0x06007A5C RID: 31324 RVA: 0x00281FAC File Offset: 0x002801AC
		public static sbyte ReadSignedPackedBits(this ushort buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBits(ref bitposition, bits);
			return (sbyte)((int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U)))));
		}
	}
}
