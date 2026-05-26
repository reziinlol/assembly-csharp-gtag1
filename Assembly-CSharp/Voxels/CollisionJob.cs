using System;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012D2 RID: 4818
	[BurstCompile]
	public struct CollisionJob : IJob
	{
		// Token: 0x06007879 RID: 30841 RVA: 0x00278B0A File Offset: 0x00276D0A
		public void Execute()
		{
			Physics.BakeMesh(this.MeshId, false, MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase);
		}

		// Token: 0x04008B5F RID: 35679
		public const MeshColliderCookingOptions CookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;

		// Token: 0x04008B60 RID: 35680
		public EntityId MeshId;
	}
}
