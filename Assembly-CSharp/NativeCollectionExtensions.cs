using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

// Token: 0x020001E3 RID: 483
public static class NativeCollectionExtensions
{
	// Token: 0x06000CAE RID: 3246 RVA: 0x00045E14 File Offset: 0x00044014
	public static T[] ToArray<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
	{
		return list.AsArray().ToArray();
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00045E30 File Offset: 0x00044030
	public static List<T> ToList<[IsUnmanaged] T>(this NativeList<T> list) where T : struct, ValueType
	{
		List<T> list2 = new List<T>(list.Length);
		for (int i = 0; i < list.Length; i++)
		{
			list2.Add(list[i]);
		}
		return list2;
	}
}
