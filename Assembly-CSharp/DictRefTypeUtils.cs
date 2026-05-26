using System;
using System.Collections.Generic;

// Token: 0x02000D45 RID: 3397
public static class DictRefTypeUtils
{
	// Token: 0x060053B1 RID: 21425 RVA: 0x001B63B2 File Offset: 0x001B45B2
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : class, new()
	{
		if (dict.TryGetValue(key, out value) && value != null)
		{
			return;
		}
		value = Activator.CreateInstance<TValue>();
		dict.Add(key, value);
	}
}
