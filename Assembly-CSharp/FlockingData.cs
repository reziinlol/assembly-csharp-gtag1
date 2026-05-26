using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x02000690 RID: 1680
[NetworkStructWeaved(337)]
[StructLayout(LayoutKind.Explicit, Size = 1348)]
public struct FlockingData : INetworkStruct
{
	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x060029E5 RID: 10725 RVA: 0x000E23F0 File Offset: 0x000E05F0
	// (set) Token: 0x060029E6 RID: 10726 RVA: 0x000E23F8 File Offset: 0x000E05F8
	public int count { readonly get; set; }

	// Token: 0x1700042E RID: 1070
	// (get) Token: 0x060029E7 RID: 10727 RVA: 0x000E2404 File Offset: 0x000E0604
	[Networked]
	[Capacity(30)]
	[NetworkedWeavedLinkedList(30, 3, typeof(ElementReaderWriterVector3))]
	[NetworkedWeaved(1, 153)]
	public NetworkLinkedList<Vector3> Positions
	{
		get
		{
			return new NetworkLinkedList<Vector3>(Native.ReferenceToPointer<FixedStorage@153>(ref this._Positions), 30, ElementReaderWriterVector3.GetInstance());
		}
	}

	// Token: 0x1700042F RID: 1071
	// (get) Token: 0x060029E8 RID: 10728 RVA: 0x000E242C File Offset: 0x000E062C
	[Networked]
	[Capacity(30)]
	[NetworkedWeavedLinkedList(30, 4, typeof(ReaderWriter@UnityEngine_Quaternion))]
	[NetworkedWeaved(154, 183)]
	public NetworkLinkedList<Quaternion> Rotations
	{
		get
		{
			return new NetworkLinkedList<Quaternion>(Native.ReferenceToPointer<FixedStorage@183>(ref this._Rotations), 30, ReaderWriter@UnityEngine_Quaternion.GetInstance());
		}
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000E2454 File Offset: 0x000E0654
	public FlockingData(List<Flocking> items)
	{
		this.count = items.Count;
		foreach (Flocking flocking in items)
		{
			this.Positions.Add(flocking.pos);
			this.Rotations.Add(flocking.rot);
		}
	}

	// Token: 0x0400369D RID: 13981
	[FixedBufferProperty(typeof(NetworkLinkedList<Vector3>), typeof(UnityLinkedListSurrogate@ElementReaderWriterVector3), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@153 _Positions;

	// Token: 0x0400369E RID: 13982
	[FixedBufferProperty(typeof(NetworkLinkedList<Quaternion>), typeof(UnityLinkedListSurrogate@ReaderWriter@UnityEngine_Quaternion), 30, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(616)]
	private FixedStorage@183 _Rotations;
}
