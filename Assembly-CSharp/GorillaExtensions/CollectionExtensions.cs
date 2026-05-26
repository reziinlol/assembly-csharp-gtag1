using System;
using System.Collections.Generic;

namespace GorillaExtensions
{
	// Token: 0x02001114 RID: 4372
	public static class CollectionExtensions
	{
		// Token: 0x06006E20 RID: 28192 RVA: 0x002408B8 File Offset: 0x0023EAB8
		public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
		{
			foreach (T item in ts)
			{
				collection.Add(item);
			}
		}

		// Token: 0x06006E21 RID: 28193 RVA: 0x00240900 File Offset: 0x0023EB00
		public static void CopyStringKeepDelimiterAtEnd(this HashSet<string> hash, string str, char delimiter)
		{
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			int i = 0;
			int num = 0;
			int length = str.Length;
			while (i < length)
			{
				if (str[i] == delimiter)
				{
					hash.Add(str.Substring(num, i - num));
					num = i + 1;
				}
				i++;
			}
		}

		// Token: 0x06006E22 RID: 28194 RVA: 0x0024094C File Offset: 0x0023EB4C
		public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
		{
			foreach (T item in ts)
			{
				if (!collection.Contains(item))
				{
					return false;
				}
			}
			return true;
		}
	}
}
