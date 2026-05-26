using System;
using System.Collections.Generic;

// Token: 0x02000D89 RID: 3465
public class StaticArrayBag<T>
{
	// Token: 0x060054F8 RID: 21752 RVA: 0x001BC4C4 File Offset: 0x001BA6C4
	public T[] GetStaticArray(int size)
	{
		T[] array;
		if (!this.m_bag.ContainsKey(size))
		{
			array = new T[size];
			this.m_bag[size] = array;
		}
		else
		{
			array = this.m_bag[size];
		}
		return array;
	}

	// Token: 0x04006583 RID: 25987
	private Dictionary<int, T[]> m_bag = new Dictionary<int, T[]>(1);
}
