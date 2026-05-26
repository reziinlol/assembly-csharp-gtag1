using System;

namespace Voxels
{
	// Token: 0x020012EF RID: 4847
	[Serializable]
	public struct GenerationParameters
	{
		// Token: 0x04008BDF RID: 35807
		public MeshGenerationMode MeshGenerationMode;

		// Token: 0x04008BE0 RID: 35808
		public float NoiseScale;

		// Token: 0x04008BE1 RID: 35809
		public float GroundLevel;

		// Token: 0x04008BE2 RID: 35810
		public float HeightScale;

		// Token: 0x04008BE3 RID: 35811
		public float HeightCompensation;

		// Token: 0x04008BE4 RID: 35812
		public int Octaves;

		// Token: 0x04008BE5 RID: 35813
		public float Persistence;

		// Token: 0x04008BE6 RID: 35814
		public float IsoLevel;

		// Token: 0x04008BE7 RID: 35815
		public int Seed;

		// Token: 0x04008BE8 RID: 35816
		public float NormalThreshold;

		// Token: 0x04008BE9 RID: 35817
		public bool AreaWeightedNormals;
	}
}
