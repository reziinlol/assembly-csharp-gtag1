using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
[NetworkStructWeaved(21)]
[Serializable]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct ReliableStateData : INetworkStruct
{
	// Token: 0x1700036D RID: 877
	// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x000AB3AA File Offset: 0x000A95AA
	// (set) Token: 0x06001FD2 RID: 8146 RVA: 0x000AB3B2 File Offset: 0x000A95B2
	public long Header { readonly get; set; }

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06001FD3 RID: 8147 RVA: 0x000AB3BC File Offset: 0x000A95BC
	[Networked]
	[Capacity(5)]
	[NetworkedWeavedArray(5, 2, typeof(ElementReaderWriterInt64))]
	[NetworkedWeaved(11, 10)]
	public NetworkArray<long> TransferrableStates
	{
		get
		{
			return new NetworkArray<long>(Native.ReferenceToPointer<FixedStorage@10>(ref this._TransferrableStates), 5, ElementReaderWriterInt64.GetInstance());
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06001FD4 RID: 8148 RVA: 0x000AB3DF File Offset: 0x000A95DF
	// (set) Token: 0x06001FD5 RID: 8149 RVA: 0x000AB3E7 File Offset: 0x000A95E7
	public int WearablesPackedState { readonly get; set; }

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06001FD6 RID: 8150 RVA: 0x000AB3F0 File Offset: 0x000A95F0
	// (set) Token: 0x06001FD7 RID: 8151 RVA: 0x000AB3F8 File Offset: 0x000A95F8
	public int LThrowableProjectileIndex { readonly get; set; }

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x06001FD8 RID: 8152 RVA: 0x000AB401 File Offset: 0x000A9601
	// (set) Token: 0x06001FD9 RID: 8153 RVA: 0x000AB409 File Offset: 0x000A9609
	public int RThrowableProjectileIndex { readonly get; set; }

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06001FDA RID: 8154 RVA: 0x000AB412 File Offset: 0x000A9612
	// (set) Token: 0x06001FDB RID: 8155 RVA: 0x000AB41A File Offset: 0x000A961A
	public int SizeLayerMask { readonly get; set; }

	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06001FDC RID: 8156 RVA: 0x000AB423 File Offset: 0x000A9623
	// (set) Token: 0x06001FDD RID: 8157 RVA: 0x000AB42B File Offset: 0x000A962B
	public int RandomThrowableIndex { readonly get; set; }

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x06001FDE RID: 8158 RVA: 0x000AB434 File Offset: 0x000A9634
	// (set) Token: 0x06001FDF RID: 8159 RVA: 0x000AB43C File Offset: 0x000A963C
	public long PackedBeads { readonly get; set; }

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x06001FE0 RID: 8160 RVA: 0x000AB445 File Offset: 0x000A9645
	// (set) Token: 0x06001FE1 RID: 8161 RVA: 0x000AB44D File Offset: 0x000A964D
	public long PackedBeadsMoreThan6 { readonly get; set; }

	// Token: 0x04002A95 RID: 10901
	[FixedBufferProperty(typeof(NetworkArray<long>), typeof(UnityArraySurrogate@ElementReaderWriterInt64), 5, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(44)]
	private FixedStorage@10 _TransferrableStates;
}
