using System;

namespace emotitron.Compression
{
	// Token: 0x0200130E RID: 4878
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x06007AD2 RID: 31442
		public abstract ulong Encode(T val);

		// Token: 0x06007AD3 RID: 31443
		public abstract T Decode(uint val);

		// Token: 0x06007AD4 RID: 31444
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x06007AD5 RID: 31445
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x06007AD6 RID: 31446
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
