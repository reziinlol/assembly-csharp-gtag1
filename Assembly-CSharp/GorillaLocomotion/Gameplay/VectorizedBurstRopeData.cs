using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x0200110B RID: 4363
	public struct VectorizedBurstRopeData
	{
		// Token: 0x04007EFF RID: 32511
		public NativeArray<float4> posX;

		// Token: 0x04007F00 RID: 32512
		public NativeArray<float4> posY;

		// Token: 0x04007F01 RID: 32513
		public NativeArray<float4> posZ;

		// Token: 0x04007F02 RID: 32514
		public NativeArray<int4> validNodes;

		// Token: 0x04007F03 RID: 32515
		public NativeArray<float4> lastPosX;

		// Token: 0x04007F04 RID: 32516
		public NativeArray<float4> lastPosY;

		// Token: 0x04007F05 RID: 32517
		public NativeArray<float4> lastPosZ;

		// Token: 0x04007F06 RID: 32518
		public NativeArray<float3> ropeRoots;

		// Token: 0x04007F07 RID: 32519
		public NativeArray<float4> nodeMass;
	}
}
