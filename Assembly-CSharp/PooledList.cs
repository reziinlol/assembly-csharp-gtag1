using System;
using System.Collections.Generic;
using GorillaTag;

// Token: 0x02000DA0 RID: 3488
public class PooledList<T> : ObjectPoolEvents
{
	// Token: 0x06005582 RID: 21890 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ObjectPoolEvents.OnTaken()
	{
	}

	// Token: 0x06005583 RID: 21891 RVA: 0x001BDE7B File Offset: 0x001BC07B
	void ObjectPoolEvents.OnReturned()
	{
		this.List.Clear();
	}

	// Token: 0x040065BC RID: 26044
	public List<T> List = new List<T>();
}
