using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

// Token: 0x020001C8 RID: 456
[NetworkStructWeaved(21)]
[StructLayout(LayoutKind.Explicit, Size = 84)]
public struct GhostLabData : INetworkStruct
{
	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000C0E RID: 3086 RVA: 0x000417A5 File Offset: 0x0003F9A5
	// (set) Token: 0x06000C0F RID: 3087 RVA: 0x000417AD File Offset: 0x0003F9AD
	public int DoorState { readonly get; set; }

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000C10 RID: 3088 RVA: 0x000417B8 File Offset: 0x0003F9B8
	[Networked]
	[Capacity(20)]
	[NetworkedWeavedArray(20, 1, typeof(ElementReaderWriterNetworkBool))]
	[NetworkedWeaved(1, 20)]
	public NetworkArray<NetworkBool> OpenDoors
	{
		get
		{
			return new NetworkArray<NetworkBool>(Native.ReferenceToPointer<FixedStorage@20>(ref this._OpenDoors), 20, ElementReaderWriterNetworkBool.GetInstance());
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x000417E0 File Offset: 0x0003F9E0
	public GhostLabData(int state, bool[] openDoors)
	{
		this.DoorState = state;
		for (int i = 0; i < openDoors.Length; i++)
		{
			bool val = openDoors[i];
			this.OpenDoors.Set(i, val);
		}
	}

	// Token: 0x04000EAF RID: 3759
	[FixedBufferProperty(typeof(NetworkArray<NetworkBool>), typeof(UnityArraySurrogate@ElementReaderWriterNetworkBool), 20, order = -2147483647)]
	[WeaverGenerated]
	[SerializeField]
	[FieldOffset(4)]
	private FixedStorage@20 _OpenDoors;
}
