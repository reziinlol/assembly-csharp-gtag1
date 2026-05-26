using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020012C8 RID: 4808
	public struct ChunkDTO
	{
		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x0600783F RID: 30783 RVA: 0x00277885 File Offset: 0x00275A85
		public bool IsValid
		{
			get
			{
				return !this.Size.Equals(int3.zero) && this.Density.IsCreated && this.Material.IsCreated;
			}
		}

		// Token: 0x06007840 RID: 30784 RVA: 0x002778B4 File Offset: 0x00275AB4
		public ChunkDTO(Chunk chunk)
		{
			this.WorldId = chunk.World.Id;
			this.Id = chunk.Id;
			this.Size = chunk.Size;
			this.Dimensions = chunk.Dimensions;
			this.Density = chunk.Density;
			this.Material = chunk.Material;
		}

		// Token: 0x04008B44 RID: 35652
		public int WorldId;

		// Token: 0x04008B45 RID: 35653
		public int3 Id;

		// Token: 0x04008B46 RID: 35654
		public int3 Size;

		// Token: 0x04008B47 RID: 35655
		public int3 Dimensions;

		// Token: 0x04008B48 RID: 35656
		public NativeArray<byte> Density;

		// Token: 0x04008B49 RID: 35657
		public NativeArray<byte> Material;
	}
}
