using System;
using System.Collections.Generic;

// Token: 0x02000D46 RID: 3398
public static class DictValueTypeUtils
{
	// Token: 0x060053B2 RID: 21426 RVA: 0x001B63E4 File Offset: 0x001B45E4
	public static void TryGetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, out TValue value) where TValue : struct
	{
		if (dict.TryGetValue(key, out value))
		{
			return;
		}
		value = default(TValue);
		dict.Add(key, value);
	}
}
