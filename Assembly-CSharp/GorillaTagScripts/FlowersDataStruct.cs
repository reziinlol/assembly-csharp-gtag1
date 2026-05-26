using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F04 RID: 3844
	[NetworkStructWeaved(13)]
	[StructLayout(LayoutKind.Explicit, Size = 52)]
	public struct FlowersDataStruct : INetworkStruct
	{
		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x06005F9A RID: 24474 RVA: 0x001EC45F File Offset: 0x001EA65F
		// (set) Token: 0x06005F9B RID: 24475 RVA: 0x001EC467 File Offset: 0x001EA667
		public int FlowerCount { readonly get; set; }

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x06005F9C RID: 24476 RVA: 0x001EC470 File Offset: 0x001EA670
		[Networked]
		[NetworkedWeavedLinkedList(1, 1, typeof(ElementReaderWriterByte))]
		[NetworkedWeaved(1, 6)]
		public NetworkLinkedList<byte> FlowerWateredData
		{
			get
			{
				return new NetworkLinkedList<byte>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerWateredData), 1, ElementReaderWriterByte.GetInstance());
			}
		}

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x06005F9D RID: 24477 RVA: 0x001EC494 File Offset: 0x001EA694
		[Networked]
		[NetworkedWeavedLinkedList(1, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(7, 6)]
		public NetworkLinkedList<int> FlowerStateData
		{
			get
			{
				return new NetworkLinkedList<int>(Native.ReferenceToPointer<FixedStorage@6>(ref this._FlowerStateData), 1, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x06005F9E RID: 24478 RVA: 0x001EC4B8 File Offset: 0x001EA6B8
		public FlowersDataStruct(List<Flower> allFlowers)
		{
			this.FlowerCount = allFlowers.Count;
			foreach (Flower flower in allFlowers)
			{
				this.FlowerWateredData.Add(flower.IsWatered ? 1 : 0);
				this.FlowerStateData.Add((int)flower.GetCurrentState());
			}
		}

		// Token: 0x04006E50 RID: 28240
		[FixedBufferProperty(typeof(NetworkLinkedList<byte>), typeof(UnityLinkedListSurrogate@ElementReaderWriterByte), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@6 _FlowerWateredData;

		// Token: 0x04006E51 RID: 28241
		[FixedBufferProperty(typeof(NetworkLinkedList<int>), typeof(UnityLinkedListSurrogate@ElementReaderWriterInt32), 1, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(28)]
		private FixedStorage@6 _FlowerStateData;
	}
}
