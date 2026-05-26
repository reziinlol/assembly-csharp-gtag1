using System;
using UnityEngine;

// Token: 0x0200042B RID: 1067
[Serializable]
public struct SerializableVector2
{
	// Token: 0x06001956 RID: 6486 RVA: 0x0008E3E9 File Offset: 0x0008C5E9
	public SerializableVector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06001957 RID: 6487 RVA: 0x0008E3F9 File Offset: 0x0008C5F9
	public static implicit operator SerializableVector2(Vector2 v)
	{
		return new SerializableVector2(v.x, v.y);
	}

	// Token: 0x06001958 RID: 6488 RVA: 0x0008E40C File Offset: 0x0008C60C
	public static implicit operator Vector2(SerializableVector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	// Token: 0x0400244D RID: 9293
	public float x;

	// Token: 0x0400244E RID: 9294
	public float y;
}
