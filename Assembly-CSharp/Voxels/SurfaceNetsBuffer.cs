using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020012E6 RID: 4838
	public struct SurfaceNetsBuffer : IDisposable
	{
		// Token: 0x060078A2 RID: 30882 RVA: 0x0027A68C File Offset: 0x0027888C
		public SurfaceNetsBuffer(int vertexCap, int indexCap, int strideCount, Allocator alloc = Allocator.TempJob)
		{
			this.Vertices = new NativeList<float3>(vertexCap, alloc);
			this.Normals = new NativeList<float3>(vertexCap, alloc);
			this.Materials = new NativeList<byte>(vertexCap, alloc);
			this.Triangles = new NativeList<int>(indexCap, alloc);
			this.SurfacePoints = new NativeList<int3>(vertexCap, alloc);
			this.SurfaceStrides = new NativeList<int>(vertexCap, alloc);
			this.StrideToIndex = new NativeArray<int>(strideCount, alloc, NativeArrayOptions.UninitializedMemory);
			this.Reset(strideCount);
		}

		// Token: 0x060078A3 RID: 30883 RVA: 0x0027A724 File Offset: 0x00278924
		public void Reset(int strideCount)
		{
			this.Vertices.Clear();
			this.Normals.Clear();
			this.Triangles.Clear();
			this.SurfacePoints.Clear();
			this.SurfaceStrides.Clear();
			if (this.StrideToIndex.Length < strideCount)
			{
				this.StrideToIndex.Dispose();
			}
			if (!this.StrideToIndex.IsCreated || this.StrideToIndex.Length != strideCount)
			{
				this.StrideToIndex = new NativeArray<int>(strideCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			}
			for (int i = 0; i < strideCount; i++)
			{
				this.StrideToIndex[i] = int.MaxValue;
			}
		}

		// Token: 0x060078A4 RID: 30884 RVA: 0x0027A7C8 File Offset: 0x002789C8
		public void Dispose()
		{
			this.Vertices.Dispose();
			this.Normals.Dispose();
			this.Materials.Dispose();
			this.Triangles.Dispose();
			this.SurfacePoints.Dispose();
			this.SurfaceStrides.Dispose();
			this.StrideToIndex.Dispose();
		}

		// Token: 0x04008BB8 RID: 35768
		public NativeList<float3> Vertices;

		// Token: 0x04008BB9 RID: 35769
		public NativeList<float3> Normals;

		// Token: 0x04008BBA RID: 35770
		public NativeList<byte> Materials;

		// Token: 0x04008BBB RID: 35771
		public NativeList<int> Triangles;

		// Token: 0x04008BBC RID: 35772
		public NativeList<int3> SurfacePoints;

		// Token: 0x04008BBD RID: 35773
		public NativeList<int> SurfaceStrides;

		// Token: 0x04008BBE RID: 35774
		public NativeArray<int> StrideToIndex;

		// Token: 0x04008BBF RID: 35775
		public const int NullVertex = 2147483647;
	}
}
