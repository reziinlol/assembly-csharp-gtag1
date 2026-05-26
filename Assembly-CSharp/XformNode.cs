using System;
using UnityEngine;

// Token: 0x02000AF8 RID: 2808
[Serializable]
public class XformNode
{
	// Token: 0x170006B0 RID: 1712
	// (get) Token: 0x060047E8 RID: 18408 RVA: 0x00181CE0 File Offset: 0x0017FEE0
	public Vector4 worldPosition
	{
		get
		{
			if (!this.parent)
			{
				return this.localPosition;
			}
			Matrix4x4 localToWorldMatrix = this.parent.localToWorldMatrix;
			Vector4 result = this.localPosition;
			MatrixUtils.MultiplyXYZ3x4(ref localToWorldMatrix, ref result);
			return result;
		}
	}

	// Token: 0x170006B1 RID: 1713
	// (get) Token: 0x060047E9 RID: 18409 RVA: 0x00181D1E File Offset: 0x0017FF1E
	// (set) Token: 0x060047EA RID: 18410 RVA: 0x00181D2B File Offset: 0x0017FF2B
	public float radius
	{
		get
		{
			return this.localPosition.w;
		}
		set
		{
			this.localPosition.w = value;
		}
	}

	// Token: 0x060047EB RID: 18411 RVA: 0x00181D39 File Offset: 0x0017FF39
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	// Token: 0x04005A09 RID: 23049
	public Vector4 localPosition;

	// Token: 0x04005A0A RID: 23050
	public Transform parent;
}
