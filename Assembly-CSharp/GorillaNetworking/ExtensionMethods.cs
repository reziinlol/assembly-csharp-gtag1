using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200108A RID: 4234
	public static class ExtensionMethods
	{
		// Token: 0x06006A2E RID: 27182 RVA: 0x00225730 File Offset: 0x00223930
		public static void SafeInvoke<T>(this Action<T> action, T data)
		{
			try
			{
				if (action != null)
				{
					action(data);
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::SafeInvoke] Failure invoking action: {0}", arg));
			}
		}

		// Token: 0x06006A2F RID: 27183 RVA: 0x0022576C File Offset: 0x0022396C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			dict[key] = value;
		}
	}
}
