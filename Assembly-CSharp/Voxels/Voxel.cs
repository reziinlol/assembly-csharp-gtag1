using System;

namespace Voxels
{
	// Token: 0x020012FA RID: 4858
	[Serializable]
	public struct Voxel
	{
		// Token: 0x06007943 RID: 31043 RVA: 0x0027E4F2 File Offset: 0x0027C6F2
		public Voxel(byte material, byte density)
		{
			this.Material = material;
			this.Density = density;
		}

		// Token: 0x04008C28 RID: 35880
		public byte Material;

		// Token: 0x04008C29 RID: 35881
		public byte Density;
	}
}
