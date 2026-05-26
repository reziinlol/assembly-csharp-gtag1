using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000D9A RID: 3482
public static class UnityObjectUtils
{
	// Token: 0x06005579 RID: 21881 RVA: 0x001BDDE8 File Offset: 0x001BBFE8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T AsNull<T>(this T obj) where T : Object
	{
		if (obj == null)
		{
			return default(T);
		}
		if (!(obj == null))
		{
			return obj;
		}
		return default(T);
	}

	// Token: 0x0600557A RID: 21882 RVA: 0x00045E72 File Offset: 0x00044072
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SafeDestroy(this Object obj)
	{
		Object.Destroy(obj);
	}
}
