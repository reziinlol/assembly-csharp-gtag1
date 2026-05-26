using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020012D5 RID: 4821
	[BurstCompile]
	public struct MarchingCubesMeshingJob : IJob
	{
		// Token: 0x0600787C RID: 30844 RVA: 0x00278EA0 File Offset: 0x002770A0
		public void Execute()
		{
			this.dimension = this.chunkSize + 1;
			for (int i = 0; i < this.chunkSize; i++)
			{
				for (int j = 0; j < this.chunkSize; j++)
				{
					for (int k = 0; k < this.chunkSize; k++)
					{
						this.ProcessCube(i, j, k);
					}
				}
			}
		}

		// Token: 0x0600787D RID: 30845 RVA: 0x00278EF8 File Offset: 0x002770F8
		private void ProcessCube(int x, int y, int z)
		{
			int3 @int = new int3(x, y, z);
			this.GetMaterialValue(@int);
			NativeArray<byte> nativeArray = new NativeArray<byte>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < 8; i++)
			{
				int3 pos = new int3(x, y, z) + (int3)MarchingCubesLookup.CornerOffsets[i];
				nativeArray[i] = this.GetVoxelValue(pos);
			}
			int num = 0;
			for (int j = 0; j < 8; j++)
			{
				if (nativeArray[j] < this.isoLevel)
				{
					num |= 1 << j;
				}
			}
			if (num == 0 || num == 255)
			{
				nativeArray.Dispose();
				return;
			}
			NativeArray<float3> nativeArray2 = new NativeArray<float3>(12, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float> nativeArray3 = new NativeArray<float>(12, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int k = 0; k < 12; k++)
			{
				if ((MarchingCubesLookup.EdgeTable[num] & 1 << k) != 0)
				{
					int2 int2 = MarchingCubesLookup.EdgeVertices[k];
					float3 start = new float3((float)x, (float)y, (float)z) + MarchingCubesLookup.CornerOffsets[int2.x];
					float3 end = new float3((float)x, (float)y, (float)z) + MarchingCubesLookup.CornerOffsets[int2.y];
					float t = (float)(this.isoLevel - nativeArray[int2.x]) / (float)(nativeArray[int2.y] - nativeArray[int2.x]);
					nativeArray2[k] = math.lerp(start, end, t);
					int materialValue = (int)this.GetMaterialValue((@int + MarchingCubesLookup.CornerOffsets[int2.x]).ToInt3());
					byte materialValue2 = this.GetMaterialValue((@int + MarchingCubesLookup.CornerOffsets[int2.y]).ToInt3());
					int num2 = math.max(materialValue, (int)materialValue2);
					nativeArray3[k] = (float)num2;
				}
			}
			int num3 = 0;
			while (num3 < 16 && MarchingCubesLookup.TriTable[num * 16 + num3] != -1)
			{
				float3 @float = nativeArray2[MarchingCubesLookup.TriTable[num * 16 + num3]];
				float3 float2 = nativeArray2[MarchingCubesLookup.TriTable[num * 16 + num3 + 1]];
				float3 float3 = nativeArray2[MarchingCubesLookup.TriTable[num * 16 + num3 + 2]];
				float x2 = nativeArray3[MarchingCubesLookup.TriTable[num * 16 + num3]];
				float y2 = nativeArray3[MarchingCubesLookup.TriTable[num * 16 + num3 + 1]];
				float z2 = nativeArray3[MarchingCubesLookup.TriTable[num * 16 + num3 + 2]];
				float4 float4 = new float4(x2, y2, z2, 0f);
				if (!@float.Equals(float2) && !@float.Equals(float3) && !float2.Equals(float3))
				{
					float3 float5 = math.normalize(math.cross(float2 - @float, float3 - @float));
					float3 xyz = math.normalize(math.cross((math.abs(float5.y) < 0.999f) ? new float3(0f, 1f, 0f) : new float3(1f, 0f, 0f), float5));
					float4 tangent = new float4(xyz, 1f);
					int num4 = this.triangleCounter.Increment() * 3;
					this.vertexData[num4] = new MeshVertexData(@float, float5, tangent, float4, new float4(1f, 0f, 0f, 0f));
					this.triangleData[num4] = (ushort)num4;
					this.vertexData[num4 + 1] = new MeshVertexData(float2, float5, tangent, float4, new float4(0f, 1f, 0f, 0f));
					this.triangleData[num4 + 1] = (ushort)(num4 + 1);
					this.vertexData[num4 + 2] = new MeshVertexData(float3, float5, tangent, float4, new float4(0f, 0f, 1f, 0f));
					this.triangleData[num4 + 2] = (ushort)(num4 + 2);
				}
				num3 += 3;
			}
			nativeArray.Dispose();
			nativeArray2.Dispose();
			nativeArray3.Dispose();
		}

		// Token: 0x0600787E RID: 30846 RVA: 0x00279346 File Offset: 0x00277546
		private byte GetMaterialValue(int3 pos)
		{
			return this.materials[pos.x + this.dimension * (pos.y + pos.z * this.dimension)];
		}

		// Token: 0x0600787F RID: 30847 RVA: 0x00279378 File Offset: 0x00277578
		private byte GetVoxelValue(int3 pos)
		{
			if (pos.x < 0 || pos.y < 0 || pos.z < 0 || pos.x > this.chunkSize || pos.y > this.chunkSize || pos.z > this.chunkSize)
			{
				return 0;
			}
			int index = pos.x + this.dimension * (pos.y + pos.z * this.dimension);
			return this.voxels[index];
		}

		// Token: 0x04008B71 RID: 35697
		[ReadOnly]
		public NativeArray<byte> voxels;

		// Token: 0x04008B72 RID: 35698
		[ReadOnly]
		public NativeArray<byte> materials;

		// Token: 0x04008B73 RID: 35699
		[ReadOnly]
		public int chunkSize;

		// Token: 0x04008B74 RID: 35700
		[ReadOnly]
		public byte isoLevel;

		// Token: 0x04008B75 RID: 35701
		public NativeCounter triangleCounter;

		// Token: 0x04008B76 RID: 35702
		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<MeshVertexData> vertexData;

		// Token: 0x04008B77 RID: 35703
		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<ushort> triangleData;

		// Token: 0x04008B78 RID: 35704
		private int dimension;
	}
}
