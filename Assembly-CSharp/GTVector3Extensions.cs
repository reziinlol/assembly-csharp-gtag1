using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000330 RID: 816
public static class GTVector3Extensions
{
	// Token: 0x0600142B RID: 5163 RVA: 0x0006C926 File Offset: 0x0006AB26
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 X_Z(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	// Token: 0x0600142C RID: 5164 RVA: 0x0006C940 File Offset: 0x0006AB40
	public static Vector3 Sum(this IList<Vector3> vecs)
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < vecs.Count; i++)
		{
			vector += vecs[i];
		}
		return vector;
	}

	// Token: 0x0600142D RID: 5165 RVA: 0x0006C974 File Offset: 0x0006AB74
	public static Vector3 Average(this IList<Vector3> vecs)
	{
		int count = vecs.Count;
		if (count == 0)
		{
			return Vector3.zero;
		}
		Vector3 a = Vector3.zero;
		for (int i = 0; i < count; i++)
		{
			a += vecs[i];
		}
		return a / (float)count;
	}

	// Token: 0x0600142E RID: 5166 RVA: 0x0006C9BC File Offset: 0x0006ABBC
	public static Vector3 Sum(this IEnumerable<Vector3> vecs)
	{
		Vector3 vector = Vector3.zero;
		foreach (Vector3 b in vecs)
		{
			vector += b;
		}
		return vector;
	}

	// Token: 0x0600142F RID: 5167 RVA: 0x0006CA0C File Offset: 0x0006AC0C
	public static Vector3 Average(this IEnumerable<Vector3> vecs)
	{
		Vector3 a = Vector3.zero;
		int num = 0;
		foreach (Vector3 b in vecs)
		{
			a += b;
			num++;
		}
		if (num == 0)
		{
			return Vector3.zero;
		}
		return a / (float)num;
	}
}
