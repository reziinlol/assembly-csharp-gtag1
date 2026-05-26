using System;
using System.Collections.Generic;

namespace PerformanceSystems
{
	// Token: 0x02000EA3 RID: 3747
	public static class FastRemoveExtensions
	{
		// Token: 0x06005C08 RID: 23560 RVA: 0x001D3F68 File Offset: 0x001D2168
		public static bool FastRemove<T>(this List<T> list, T itemToRemove)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			int count = list.Count;
			if (count == 0)
			{
				return false;
			}
			int index = count - 1;
			for (int i = 0; i < count; i++)
			{
				if (@default.Equals(list[i], itemToRemove))
				{
					list[i] = list[index];
					list.RemoveAt(index);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005C09 RID: 23561 RVA: 0x001D3FC0 File Offset: 0x001D21C0
		public static bool FastRemove<T>(this List<T> list, HashSet<T> setToRemove)
		{
			if (setToRemove == null || setToRemove.Count == 0 || list.Count == 0)
			{
				return false;
			}
			bool result = false;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T item = list[i];
				if (setToRemove.Contains(item))
				{
					int index = list.Count - 1;
					list[i] = list[index];
					list.RemoveAt(index);
					result = true;
				}
			}
			return result;
		}
	}
}
