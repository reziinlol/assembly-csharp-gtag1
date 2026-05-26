using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000F7E RID: 3966
	[NetworkStructWeaved(9)]
	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct ObstacleCourseData : INetworkStruct
	{
		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06006310 RID: 25360 RVA: 0x001FE64D File Offset: 0x001FC84D
		// (set) Token: 0x06006311 RID: 25361 RVA: 0x001FE655 File Offset: 0x001FC855
		public int ObstacleCourseCount { readonly get; set; }

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x06006312 RID: 25362 RVA: 0x001FE660 File Offset: 0x001FC860
		[Networked]
		[Capacity(4)]
		[NetworkedWeavedArray(4, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(1, 4)]
		public NetworkArray<int> WinnerActorNumber
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._WinnerActorNumber), 4, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x06006313 RID: 25363 RVA: 0x001FE684 File Offset: 0x001FC884
		[Networked]
		[Capacity(4)]
		[NetworkedWeavedArray(4, 1, typeof(ElementReaderWriterInt32))]
		[NetworkedWeaved(5, 4)]
		public NetworkArray<int> CurrentRaceState
		{
			get
			{
				return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@4>(ref this._CurrentRaceState), 4, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x001FE6A8 File Offset: 0x001FC8A8
		public ObstacleCourseData(List<ObstacleCourse> courses)
		{
			this.ObstacleCourseCount = courses.Count;
			int[] array = new int[this.ObstacleCourseCount];
			int[] array2 = new int[this.ObstacleCourseCount];
			for (int i = 0; i < courses.Count; i++)
			{
				array[i] = courses[i].winnerActorNumber;
				array2[i] = (int)courses[i].currentState;
			}
			this.WinnerActorNumber.CopyFrom(array, 0, this.ObstacleCourseCount);
			this.CurrentRaceState.CopyFrom(array2, 0, this.ObstacleCourseCount);
		}

		// Token: 0x040071C1 RID: 29121
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@4 _WinnerActorNumber;

		// Token: 0x040071C2 RID: 29122
		[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 4, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(20)]
		private FixedStorage@4 _CurrentRaceState;
	}
}
