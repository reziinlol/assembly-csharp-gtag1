using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Voxels;

namespace FastSurfaceNets
{
	// Token: 0x020012C2 RID: 4802
	internal struct FillChunkJob : IJobParallelFor
	{
		// Token: 0x06007817 RID: 30743 RVA: 0x00276EAC File Offset: 0x002750AC
		public void Execute(int index)
		{
			int z = index / this.strideZ;
			int y = index % this.strideZ / this.strideY;
			int x = index % this.strideY;
			float3 @float = (this.chunkPosition + new int3(x, y, z)).ToFloat3();
			float value = noise.snoise(@float * this.noiseScale) - @float.y / this.heightScale;
			this.sdf[index] = value.ToByte();
		}

		// Token: 0x04008B05 RID: 35589
		[WriteOnly]
		public NativeArray<byte> sdf;

		// Token: 0x04008B06 RID: 35590
		[ReadOnly]
		public int3 shape;

		// Token: 0x04008B07 RID: 35591
		[ReadOnly]
		public int3 chunkPosition;

		// Token: 0x04008B08 RID: 35592
		[ReadOnly]
		public int3 shapeMin;

		// Token: 0x04008B09 RID: 35593
		[ReadOnly]
		public int3 shapeMax;

		// Token: 0x04008B0A RID: 35594
		[ReadOnly]
		public float noiseScale;

		// Token: 0x04008B0B RID: 35595
		[ReadOnly]
		public float heightScale;

		// Token: 0x04008B0C RID: 35596
		[ReadOnly]
		public int3 min;

		// Token: 0x04008B0D RID: 35597
		[ReadOnly]
		public int3 max;

		// Token: 0x04008B0E RID: 35598
		[ReadOnly]
		public int strideY;

		// Token: 0x04008B0F RID: 35599
		[ReadOnly]
		public int strideZ;
	}
}
