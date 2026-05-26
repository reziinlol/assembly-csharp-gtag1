using System;
using System.Runtime.InteropServices;

// Token: 0x0200044D RID: 1101
public struct RPCArgBuffer<T> where T : struct
{
	// Token: 0x06001A56 RID: 6742 RVA: 0x0009361A File Offset: 0x0009181A
	public RPCArgBuffer(T argStruct)
	{
		this.DataLength = Marshal.SizeOf(typeof(T));
		this.Data = new byte[this.DataLength];
		this.Args = argStruct;
	}

	// Token: 0x04002504 RID: 9476
	public T Args;

	// Token: 0x04002505 RID: 9477
	public byte[] Data;

	// Token: 0x04002506 RID: 9478
	public int DataLength;
}
