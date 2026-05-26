using System;
using UnityEngine;

// Token: 0x02000AE7 RID: 2791
[Serializable]
public struct OrientedBounds
{
	// Token: 0x1700069F RID: 1695
	// (get) Token: 0x06004734 RID: 18228 RVA: 0x001800DC File Offset: 0x0017E2DC
	public static OrientedBounds Empty { get; } = new OrientedBounds
	{
		size = Vector3.zero,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x170006A0 RID: 1696
	// (get) Token: 0x06004735 RID: 18229 RVA: 0x001800E3 File Offset: 0x0017E2E3
	public static OrientedBounds Identity { get; } = new OrientedBounds
	{
		size = Vector3.one,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x06004736 RID: 18230 RVA: 0x001800EA File Offset: 0x0017E2EA
	public Matrix4x4 TRS()
	{
		return Matrix4x4.TRS(this.center, this.rotation, this.size);
	}

	// Token: 0x040059AC RID: 22956
	public Vector3 size;

	// Token: 0x040059AD RID: 22957
	public Vector3 center;

	// Token: 0x040059AE RID: 22958
	public Quaternion rotation;
}
