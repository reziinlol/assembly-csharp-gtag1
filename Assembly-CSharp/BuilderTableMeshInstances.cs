using System;
using Unity.Collections;
using UnityEngine.Jobs;

// Token: 0x0200063C RID: 1596
public struct BuilderTableMeshInstances
{
	// Token: 0x040033BB RID: 13243
	public TransformAccessArray transforms;

	// Token: 0x040033BC RID: 13244
	public NativeList<int> texIndex;

	// Token: 0x040033BD RID: 13245
	public NativeList<float> tint;
}
