using System;

namespace emotitron.Compression
{
	// Token: 0x0200130B RID: 4875
	public static class PrimitivePackBytesExt
	{
		// Token: 0x06007A5D RID: 31325 RVA: 0x00281FD0 File Offset: 0x002801D0
		public static ulong WritePackedBytes(this ulong buffer, ulong value, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write(value, ref bitposition, num << 3);
			return buffer;
		}

		// Token: 0x06007A5E RID: 31326 RVA: 0x0028200C File Offset: 0x0028020C
		public static uint WritePackedBytes(this uint buffer, uint value, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num << 3);
			return buffer;
		}

		// Token: 0x06007A5F RID: 31327 RVA: 0x00282048 File Offset: 0x00280248
		public static void InjectPackedBytes(this ulong value, ref ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write(value, ref bitposition, num << 3);
		}

		// Token: 0x06007A60 RID: 31328 RVA: 0x00282084 File Offset: 0x00280284
		public static void InjectPackedBytes(this uint value, ref uint buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = value.UsedByteCount();
			buffer = buffer.Write((ulong)num, ref bitposition, bits2);
			buffer = buffer.Write((ulong)value, ref bitposition, num << 3);
		}

		// Token: 0x06007A61 RID: 31329 RVA: 0x002820C0 File Offset: 0x002802C0
		public static ulong ReadPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, num << 3);
		}

		// Token: 0x06007A62 RID: 31330 RVA: 0x002820EC File Offset: 0x002802EC
		public static uint ReadPackedBytes(this uint buffer, ref int bitposition, int bits)
		{
			int bits2 = (bits + 7 >> 3).UsedBitCount();
			int num = (int)buffer.Read(ref bitposition, bits2);
			return buffer.Read(ref bitposition, num << 3);
		}

		// Token: 0x06007A63 RID: 31331 RVA: 0x00282118 File Offset: 0x00280318
		public static ulong WriteSignedPackedBytes(this ulong buffer, int value, ref int bitposition, int bits)
		{
			uint num = (uint)(value << 1 ^ value >> 31);
			return buffer.WritePackedBytes((ulong)num, ref bitposition, bits);
		}

		// Token: 0x06007A64 RID: 31332 RVA: 0x00282138 File Offset: 0x00280338
		public static int ReadSignedPackedBytes(this ulong buffer, ref int bitposition, int bits)
		{
			uint num = (uint)buffer.ReadPackedBytes(ref bitposition, bits);
			return (int)((ulong)(num >> 1) ^ (ulong)((long)(-(long)(num & 1U))));
		}
	}
}
