using System;
using UnityEngine;

// Token: 0x02000E1C RID: 3612
[Serializable]
public struct MatrixZonePair
{
	// Token: 0x040068AC RID: 26796
	[SerializeField]
	public Matrix4x4 matrix;

	// Token: 0x040068AD RID: 26797
	[SerializeField]
	public int zoneIndex;
}
