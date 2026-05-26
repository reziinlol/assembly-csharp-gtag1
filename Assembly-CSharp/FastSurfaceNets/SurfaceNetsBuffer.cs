using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace FastSurfaceNets
{
	// Token: 0x020012BE RID: 4798
	public class SurfaceNetsBuffer
	{
		// Token: 0x06007805 RID: 30725 RVA: 0x00275C00 File Offset: 0x00273E00
		internal void Reset(int arraySize)
		{
			this.Positions.Clear();
			this.Normals.Clear();
			this.Indices.Clear();
			this.SurfacePoints.Clear();
			this.SurfaceStrides.Clear();
			if (this.StrideToIndex.Length < arraySize)
			{
				Array.Resize<int>(ref this.StrideToIndex, arraySize);
			}
			for (int i = 0; i < arraySize; i++)
			{
				this.StrideToIndex[i] = int.MaxValue;
			}
		}

		// Token: 0x04008AEA RID: 35562
		public readonly List<float3> Positions = new List<float3>();

		// Token: 0x04008AEB RID: 35563
		public readonly List<float3> Normals = new List<float3>();

		// Token: 0x04008AEC RID: 35564
		public readonly List<int> Indices = new List<int>();

		// Token: 0x04008AED RID: 35565
		internal readonly List<int3> SurfacePoints = new List<int3>();

		// Token: 0x04008AEE RID: 35566
		internal readonly List<int> SurfaceStrides = new List<int>();

		// Token: 0x04008AEF RID: 35567
		internal int[] StrideToIndex = Array.Empty<int>();

		// Token: 0x04008AF0 RID: 35568
		public const int NullVertex = 2147483647;
	}
}
