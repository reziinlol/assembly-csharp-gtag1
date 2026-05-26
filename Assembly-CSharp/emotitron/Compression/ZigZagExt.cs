using System;

namespace emotitron.Compression
{
	// Token: 0x02001313 RID: 4883
	public static class ZigZagExt
	{
		// Token: 0x06007AEA RID: 31466 RVA: 0x00282F91 File Offset: 0x00281191
		public static ulong ZigZag(this long s)
		{
			return (ulong)(s << 1 ^ s >> 63);
		}

		// Token: 0x06007AEB RID: 31467 RVA: 0x00282F9B File Offset: 0x0028119B
		public static long UnZigZag(this ulong u)
		{
			return (long)(u >> 1 ^ -(long)(u & 1UL));
		}

		// Token: 0x06007AEC RID: 31468 RVA: 0x00282FA6 File Offset: 0x002811A6
		public static uint ZigZag(this int s)
		{
			return (uint)(s << 1 ^ s >> 31);
		}

		// Token: 0x06007AED RID: 31469 RVA: 0x00282FB0 File Offset: 0x002811B0
		public static int UnZigZag(this uint u)
		{
			return (int)((ulong)(u >> 1) ^ (ulong)((long)(-(long)(u & 1U))));
		}

		// Token: 0x06007AEE RID: 31470 RVA: 0x00282FBD File Offset: 0x002811BD
		public static ushort ZigZag(this short s)
		{
			return (ushort)((int)s << 1 ^ s >> 15);
		}

		// Token: 0x06007AEF RID: 31471 RVA: 0x00282FC8 File Offset: 0x002811C8
		public static short UnZigZag(this ushort u)
		{
			return (short)(u >> 1 ^ (int)(-(int)((short)(u & 1))));
		}

		// Token: 0x06007AF0 RID: 31472 RVA: 0x00282FD4 File Offset: 0x002811D4
		public static byte ZigZag(this sbyte s)
		{
			return (byte)((int)s << 1 ^ s >> 7);
		}

		// Token: 0x06007AF1 RID: 31473 RVA: 0x00282FDE File Offset: 0x002811DE
		public static sbyte UnZigZag(this byte u)
		{
			return (sbyte)(u >> 1 ^ (int)(-(int)((sbyte)(u & 1))));
		}
	}
}
