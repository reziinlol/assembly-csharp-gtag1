using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020005B6 RID: 1462
[NetworkStructWeaved(61)]
[StructLayout(LayoutKind.Explicit, Size = 244)]
public struct PaintbrawlData : INetworkStruct
{
	// Token: 0x170003D7 RID: 983
	// (get) Token: 0x060024CB RID: 9419 RVA: 0x000C506C File Offset: 0x000C326C
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(1, 20)]
	public NetworkArray<int> playerLivesArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerLivesArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x060024CC RID: 9420 RVA: 0x000C5094 File Offset: 0x000C3294
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterInt32))]
	[NetworkedWeaved(21, 20)]
	public NetworkArray<int> playerActorNumberArray
	{
		get
		{
			return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerActorNumberArray), 20, ElementReaderWriterInt32.GetInstance());
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x060024CD RID: 9421 RVA: 0x000C50BC File Offset: 0x000C32BC
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus))]
	[NetworkedWeaved(41, 20)]
	public NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusArray
	{
		get
		{
			return new NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>(Native.ReferenceToPointer<FixedStorage@20>(ref this._playerStatusArray), 20, ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.GetInstance());
		}
	}

	// Token: 0x04003045 RID: 12357
	[FieldOffset(0)]
	public GorillaPaintbrawlManager.PaintbrawlState currentPaintbrawlState;

	// Token: 0x04003046 RID: 12358
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@20 _playerLivesArray;

	// Token: 0x04003047 RID: 12359
	[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(84)]
	private FixedStorage@20 _playerActorNumberArray;

	// Token: 0x04003048 RID: 12360
	[FixedBufferProperty(typeof(NetworkArray<GorillaPaintbrawlManager.PaintbrawlStatus>), typeof(UnityArraySurrogate@ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(164)]
	private FixedStorage@20 _playerStatusArray;
}
