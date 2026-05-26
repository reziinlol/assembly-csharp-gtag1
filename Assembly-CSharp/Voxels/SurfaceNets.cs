using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020012EA RID: 4842
	public static class SurfaceNets
	{
		// Token: 0x060078B2 RID: 30898 RVA: 0x0027B5C4 File Offset: 0x002797C4
		public static JobHandle Generate(NativeArray<byte> sdf, int3 shape, int3 min, int3 max, SurfaceNetsBuffer buffer, JobHandle dependency = default(JobHandle))
		{
			if (sdf.Length != shape.x * shape.y * shape.z)
			{
				throw new ArgumentException("SDF size does not match shape dimensions.");
			}
			return new SurfaceNetsJob
			{
				sdf = sdf,
				shape = shape,
				min = min,
				max = max,
				buffer = buffer,
				isoLevel = 0f.ToByte()
			}.Schedule(dependency);
		}
	}
}
