using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x0200111C RID: 4380
	public static class ListExtensions
	{
		// Token: 0x06006E40 RID: 28224 RVA: 0x00240FA8 File Offset: 0x0023F1A8
		public static TCol ShuffleIntoCollection<TCol, TVal>(this List<TVal> list) where TCol : ICollection<TVal>, new()
		{
			List<TVal> list2 = new List<TVal>(list);
			TCol result = Activator.CreateInstance<TCol>();
			int i = list2.Count;
			while (i > 1)
			{
				i--;
				int num = Random.Range(0, i);
				List<TVal> list3 = list2;
				int index = i;
				List<TVal> list4 = list2;
				int index2 = num;
				TVal value = list2[num];
				TVal value2 = list2[i];
				list3[index] = value;
				list4[index2] = value2;
			}
			foreach (TVal item in list2)
			{
				result.Add(item);
			}
			return result;
		}
	}
}
