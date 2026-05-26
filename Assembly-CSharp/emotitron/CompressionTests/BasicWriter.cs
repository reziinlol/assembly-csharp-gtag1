using System;

namespace emotitron.CompressionTests
{
	// Token: 0x02001319 RID: 4889
	public class BasicWriter
	{
		// Token: 0x06007B44 RID: 31556 RVA: 0x00284286 File Offset: 0x00282486
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06007B45 RID: 31557 RVA: 0x0028428E File Offset: 0x0028248E
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06007B46 RID: 31558 RVA: 0x002842A5 File Offset: 0x002824A5
		public static byte BasicRead(byte[] buffer)
		{
			byte result = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return result;
		}

		// Token: 0x04008CB1 RID: 36017
		public static int pos;
	}
}
