using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005BE RID: 1470
[NetworkStructWeaved(22)]
[StructLayout(LayoutKind.Explicit, Size = 88)]
public struct TagData : INetworkStruct
{
	// Token: 0x170003E3 RID: 995
	// (get) Token: 0x060024F7 RID: 9463 RVA: 0x000C55DC File Offset: 0x000C37DC
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(2, 20)]
	public NetworkArray<int> infectedPlayerList
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._infectedPlayerList), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003E4 RID: 996
	// (get) Token: 0x060024F8 RID: 9464 RVA: 0x000C5603 File Offset: 0x000C3803
	// (set) Token: 0x060024F9 RID: 9465 RVA: 0x000C560B File Offset: 0x000C380B
	public int currentItID { readonly get; set; }

	// Token: 0x04003055 RID: 12373
	[FieldOffset(4)]
	public NetworkBool isCurrentlyTag;

	// Token: 0x04003056 RID: 12374
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(8)]
	private FixedStorage@20 _infectedPlayerList;
}
