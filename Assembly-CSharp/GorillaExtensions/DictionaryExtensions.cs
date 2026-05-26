using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x02001115 RID: 4373
	public static class DictionaryExtensions
	{
		// Token: 0x06006E23 RID: 28195 RVA: 0x002409A0 File Offset: 0x0023EBA0
		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
		{
			TValue result;
			if (dict.TryGetValue(key, out result))
			{
				return result;
			}
			dict[key] = Activator.CreateInstance<TValue>();
			return dict[key];
		}
	}
}
