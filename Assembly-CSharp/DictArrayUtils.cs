using System;
using System.Collections.Generic;

// Token: 0x02000D47 RID: 3399
public static class DictArrayUtils
{
	// Token: 0x060053B3 RID: 21427 RVA: 0x001B6405 File Offset: 0x001B4605
	public static void TryGetOrAddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, out List<TValue> list, int capacity)
	{
		if (dict.TryGetValue(key, out list) && list != null)
		{
			return;
		}
		list = new List<TValue>(capacity);
		dict.Add(key, list);
	}

	// Token: 0x060053B4 RID: 21428 RVA: 0x001B6427 File Offset: 0x001B4627
	public static void TryGetOrAddArray<TKey, TValue>(this Dictionary<TKey, TValue[]> dict, TKey key, out TValue[] array, int size)
	{
		if (dict.TryGetValue(key, out array) && array != null)
		{
			return;
		}
		array = new TValue[size];
		dict.Add(key, array);
	}
}
