using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020001D0 RID: 464
[NetworkStructWeaved(11)]
[StructLayout(LayoutKind.Explicit, Size = 44)]
public struct SkeletonNetData : INetworkStruct
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000C52 RID: 3154 RVA: 0x000436F1 File Offset: 0x000418F1
	// (set) Token: 0x06000C53 RID: 3155 RVA: 0x000436F9 File Offset: 0x000418F9
	public int CurrentState { readonly get; set; }

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000C54 RID: 3156 RVA: 0x00043702 File Offset: 0x00041902
	// (set) Token: 0x06000C55 RID: 3157 RVA: 0x00043714 File Offset: 0x00041914
	[Networked]
	[NetworkedWeaved(1, 3)]
	public unsafe Vector3 Position
	{
		readonly get
		{
			return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position);
		}
		set
		{
			*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._Position) = value;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000C56 RID: 3158 RVA: 0x00043727 File Offset: 0x00041927
	// (set) Token: 0x06000C57 RID: 3159 RVA: 0x00043739 File Offset: 0x00041939
	[Networked]
	[NetworkedWeaved(4, 4)]
	public unsafe Quaternion Rotation
	{
		readonly get
		{
			return *(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation);
		}
		set
		{
			*(Quaternion*)Native.ReferenceToPointer<FixedStorage@4>(ref this._Rotation) = value;
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000C58 RID: 3160 RVA: 0x0004374C File Offset: 0x0004194C
	// (set) Token: 0x06000C59 RID: 3161 RVA: 0x00043754 File Offset: 0x00041954
	public int CurrentNode { readonly get; set; }

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000C5A RID: 3162 RVA: 0x0004375D File Offset: 0x0004195D
	// (set) Token: 0x06000C5B RID: 3163 RVA: 0x00043765 File Offset: 0x00041965
	public int NextNode { readonly get; set; }

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000C5C RID: 3164 RVA: 0x0004376E File Offset: 0x0004196E
	// (set) Token: 0x06000C5D RID: 3165 RVA: 0x00043776 File Offset: 0x00041976
	public int AngerPoint { readonly get; set; }

	// Token: 0x06000C5E RID: 3166 RVA: 0x0004377F File Offset: 0x0004197F
	public SkeletonNetData(int state, Vector3 pos, Quaternion rot, int cNode, int nNode, int angerPoint)
	{
		this.CurrentState = state;
		this.Position = pos;
		this.Rotation = rot;
		this.CurrentNode = cNode;
		this.NextNode = nNode;
		this.AngerPoint = angerPoint;
	}

	// Token: 0x04000F02 RID: 3842
	[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@3 _Position;

	// Token: 0x04000F03 RID: 3843
	[FixedBufferProperty(typeof(Quaternion), typeof(UnityValueSurrogate@ReaderWriter@UnityEngine_Quaternion), 0, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(16)]
	private FixedStorage@4 _Rotation;
}
