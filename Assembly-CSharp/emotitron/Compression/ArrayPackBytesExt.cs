using System;

namespace emotitron.Compression
{
	// Token: 0x02001303 RID: 4867
	public static class ArrayPackBytesExt
	{
		// Token: 0x060079AF RID: 31151 RVA: 0x00280250 File Offset: 0x0027E450
		public unsafe static void WritePackedBytes(ulong* uPtr, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			ArraySerializeUnsafe.Write(uPtr, (ulong)num, ref bitposition, bits2);
			ArraySerializeUnsafe.Write(uPtr, value, ref bitposition, num << 3);
		}

		// Token: 0x060079B0 RID: 31152 RVA: 0x00280288 File Offset: 0x0027E488
		public static void WritePackedBytes(this ulong[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x060079B1 RID: 31153 RVA: 0x002802C0 File Offset: 0x0027E4C0
		public static void WritePackedBytes(this uint[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x060079B2 RID: 31154 RVA: 0x002802F8 File Offset: 0x0027E4F8
		public static void WritePackedBytes(this byte[] buffer, ulong value, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer.Write((ulong)num, ref bitposition, bits2);
			buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x060079B3 RID: 31155 RVA: 0x00280330 File Offset: 0x0027E530
		public unsafe static ulong ReadPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits2) << 3;
			return ArraySerializeUnsafe.Read(uPtr, ref bitposition, bits3);
		}

		// Token: 0x060079B4 RID: 31156 RVA: 0x00280364 File Offset: 0x0027E564
		public static ulong ReadPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060079B5 RID: 31157 RVA: 0x00280398 File Offset: 0x0027E598
		public static ulong ReadPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060079B6 RID: 31158 RVA: 0x002803CC File Offset: 0x0027E5CC
		public static ulong ReadPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			if (bits == 0)
			{
				return 0UL;
			}
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int bits3 = (int)buffer.Read(ref bitposition, bits2) << 3;
			return buffer.Read(ref bitposition, bits3);
		}

		// Token: 0x060079B7 RID: 31159 RVA: 0x00280400 File Offset: 0x0027E600
		public unsafe static void WriteSignedPackedBytes(ulong* uPtr, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			ArrayPackBytesExt.WritePackedBytes(uPtr, (ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079B8 RID: 31160 RVA: 0x00280420 File Offset: 0x0027E620
		public unsafe static int ReadSignedPackedBytes(ulong* uPtr, ref int bitposition, int bits)
		{
			uint num = (uint)ArrayPackBytesExt.ReadPackedBytes(uPtr, ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079B9 RID: 31161 RVA: 0x00280444 File Offset: 0x0027E644
		public static void WriteSignedPackedBytes(this ulong[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079BA RID: 31162 RVA: 0x00280464 File Offset: 0x0027E664
		public static int ReadSignedPackedBytes(this ulong[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079BB RID: 31163 RVA: 0x00280488 File Offset: 0x0027E688
		public static void WriteSignedPackedBytes(this uint[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079BC RID: 31164 RVA: 0x002804A8 File Offset: 0x0027E6A8
		public static int ReadSignedPackedBytes(this uint[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079BD RID: 31165 RVA: 0x002804CC File Offset: 0x0027E6CC
		public static void WriteSignedPackedBytes(this byte[] buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x060079BE RID: 31166 RVA: 0x002804EC File Offset: 0x0027E6EC
		public static int ReadSignedPackedBytes(this byte[] buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}

		// Token: 0x060079BF RID: 31167 RVA: 0x00280510 File Offset: 0x0027E710
		public static void WriteSignedPackedBytes64(this byte[] buffer, long value, ref int bitposition, int bits)
		{
			ulong value2 = (ulong)(value << 1 ^ value >> 63);
			buffer.WritePackedBytes(value2, ref bitposition, bits);
		}

		// Token: 0x060079C0 RID: 31168 RVA: 0x00280530 File Offset: 0x0027E730
		public static long ReadSignedPackedBytes64(this byte[] buffer, ref int bitposition, int bits)
		{
			ulong num = buffer.ReadPackedBytes(ref bitposition, bits);
			return (long)(num >> 1 ^ -(long)(num & 1UL));
		}
	}
}
