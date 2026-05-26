using System;
using System.Runtime.CompilerServices;
using PlayFab.Json;

namespace GorillaExtensions
{
	// Token: 0x0200111B RID: 4379
	[NullableContext(1)]
	[Nullable(0)]
	public static class JsonObjectExtensions
	{
		// Token: 0x06006E3E RID: 28222 RVA: 0x00240F4C File Offset: 0x0023F14C
		[return: Nullable(2)]
		public static T GetValue<[Nullable(2)] T>(this JsonObject obj, string key)
		{
			object obj2;
			if (!obj.TryGetValue(key, out obj2))
			{
				return default(T);
			}
			if (obj2 is T)
			{
				return (T)((object)obj2);
			}
			return default(T);
		}

		// Token: 0x06006E3F RID: 28223 RVA: 0x00240F8A File Offset: 0x0023F18A
		public static bool TryGetValue<[Nullable(2)] T>(this JsonObject obj, string key, [Nullable(2)] out T t)
		{
			t = obj.GetValue(key);
			return t != null;
		}
	}
}
