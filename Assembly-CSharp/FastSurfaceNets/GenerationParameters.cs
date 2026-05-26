using System;
using Unity.Mathematics;

namespace FastSurfaceNets
{
	// Token: 0x020012C3 RID: 4803
	[Serializable]
	public class GenerationParameters
	{
		// Token: 0x04008B10 RID: 35600
		public bool recalculateNormals;

		// Token: 0x04008B11 RID: 35601
		public bool customNormals = true;

		// Token: 0x04008B12 RID: 35602
		public bool useBurst = true;

		// Token: 0x04008B13 RID: 35603
		public float normalThreshold = 60f;

		// Token: 0x04008B14 RID: 35604
		public bool areaWeightedNormals = true;

		// Token: 0x04008B15 RID: 35605
		public bool generateShape = true;

		// Token: 0x04008B16 RID: 35606
		public int3 shapeMin = new int3(1);

		// Token: 0x04008B17 RID: 35607
		public int3 shapeMax = new int3(15);

		// Token: 0x04008B18 RID: 35608
		public float noiseScale = 0.01f;

		// Token: 0x04008B19 RID: 35609
		public float baseHeight = 10f;

		// Token: 0x04008B1A RID: 35610
		public float heightScale = 5f;
	}
}
