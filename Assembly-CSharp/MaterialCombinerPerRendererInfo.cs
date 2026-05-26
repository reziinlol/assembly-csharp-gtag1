using System;
using UnityEngine;

// Token: 0x02000380 RID: 896
[Serializable]
public struct MaterialCombinerPerRendererInfo
{
	// Token: 0x04001AA3 RID: 6819
	public Renderer renderer;

	// Token: 0x04001AA4 RID: 6820
	public int slotIndex;

	// Token: 0x04001AA5 RID: 6821
	public int sliceIndex;

	// Token: 0x04001AA6 RID: 6822
	public Color baseColor;

	// Token: 0x04001AA7 RID: 6823
	public Material oldMat;

	// Token: 0x04001AA8 RID: 6824
	public bool wasMeshCombined;
}
