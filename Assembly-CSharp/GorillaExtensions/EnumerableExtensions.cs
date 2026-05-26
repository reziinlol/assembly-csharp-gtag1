using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x02001116 RID: 4374
	public static class EnumerableExtensions
	{
		// Token: 0x06006E24 RID: 28196 RVA: 0x002409D0 File Offset: 0x0023EBD0
		public static TValue MinBy<TValue, TKey>(this IEnumerable<TValue> ts, Func<TValue, TKey> keyGetter) where TKey : struct, IComparable<TKey>
		{
			TValue result = default(TValue);
			TKey? tkey = null;
			foreach (TValue tvalue in ts)
			{
				TKey value = keyGetter(tvalue);
				if (tkey == null || value.CompareTo(tkey.Value) < 0)
				{
					result = tvalue;
					tkey = new TKey?(value);
				}
			}
			if (tkey == null)
			{
				throw new ArgumentException("Cannot calculate MinBy on an empty IEnumerable.");
			}
			return result;
		}

		// Token: 0x06006E25 RID: 28197 RVA: 0x00240A6C File Offset: 0x0023EC6C
		public static IEnumerable<T> Peek<T>(this IEnumerable<T> ts, Action<T> action)
		{
			foreach (T t in ts)
			{
				action(t);
				yield return t;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
