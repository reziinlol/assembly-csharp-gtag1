using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x02000450 RID: 1104
[NetworkStructWeaved(1)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct HitTargetStruct : INetworkStruct
{
	// Token: 0x06001A5E RID: 6750 RVA: 0x000939A0 File Offset: 0x00091BA0
	public HitTargetStruct(int v)
	{
		this.Score = v;
	}

	// Token: 0x04002529 RID: 9513
	[FieldOffset(0)]
	public int Score;
}
