using System;

namespace Voxels
{
	// Token: 0x020012C5 RID: 4805
	public enum ChunkState
	{
		// Token: 0x04008B1F RID: 35615
		UNINITIALIZED,
		// Token: 0x04008B20 RID: 35616
		Created,
		// Token: 0x04008B21 RID: 35617
		VoxelDataGenerated,
		// Token: 0x04008B22 RID: 35618
		MeshDataGenerated,
		// Token: 0x04008B23 RID: 35619
		MeshCreated,
		// Token: 0x04008B24 RID: 35620
		CollisionBaked,
		// Token: 0x04008B25 RID: 35621
		MeshAssigned
	}
}
