using System;
using System.Collections.Generic;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x020010BD RID: 4285
	public static class ListMethodsExtensions
	{
		// Token: 0x06006B7E RID: 27518 RVA: 0x0022BF1C File Offset: 0x0022A11C
		public static void RemoveAllNullItems<T>(this List<T> list)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null)
				{
					list.RemoveAt(i);
				}
			}
		}
	}
}
