using System;
using UnityEngine;

// Token: 0x0200042C RID: 1068
[Serializable]
public struct SerializableVector3
{
	// Token: 0x06001959 RID: 6489 RVA: 0x0008E41F File Offset: 0x0008C61F
	public SerializableVector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	// Token: 0x0600195A RID: 6490 RVA: 0x0008E436 File Offset: 0x0008C636
	public static implicit operator SerializableVector3(Vector3 v)
	{
		return new SerializableVector3(v.x, v.y, v.z);
	}

	// Token: 0x0600195B RID: 6491 RVA: 0x0008E44F File Offset: 0x0008C64F
	public static implicit operator Vector3(SerializableVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	// Token: 0x0400244F RID: 9295
	public float x;

	// Token: 0x04002450 RID: 9296
	public float y;

	// Token: 0x04002451 RID: 9297
	public float z;
}
