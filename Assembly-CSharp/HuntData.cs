using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005BC RID: 1468
[NetworkStructWeaved(43)]
[StructLayout(LayoutKind.Explicit, Size = 172)]
public struct HuntData : INetworkStruct
{
	// Token: 0x170003DF RID: 991
	// (get) Token: 0x060024EE RID: 9454 RVA: 0x000C54F0 File Offset: 0x000C36F0
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(3, 20)]
	public NetworkArray<int> currentHuntedArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._currentHuntedArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003E0 RID: 992
	// (get) Token: 0x060024EF RID: 9455 RVA: 0x000C5518 File Offset: 0x000C3718
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(23, 20)]
	public NetworkArray<int> currentTargetArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._currentTargetArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x0400304E RID: 12366
	[FieldOffset(0)]
	public NetworkBool huntStarted;

	// Token: 0x0400304F RID: 12367
	[FieldOffset(4)]
	public NetworkBool waitingToStartNextHuntGame;

	// Token: 0x04003050 RID: 12368
	[FieldOffset(8)]
	public int countDownTime;

	// Token: 0x04003051 RID: 12369
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(12)]
	private FixedStorage@20 _currentHuntedArray;

	// Token: 0x04003052 RID: 12370
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(92)]
	private FixedStorage@20 _currentTargetArray;
}
