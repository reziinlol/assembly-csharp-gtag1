using System;
using UnityEngine;

// Token: 0x020009BE RID: 2494
[Serializable]
public class Ref<T> where T : class
{
	// Token: 0x170005E2 RID: 1506
	// (get) Token: 0x06003FD3 RID: 16339 RVA: 0x0015513B File Offset: 0x0015333B
	// (set) Token: 0x06003FD4 RID: 16340 RVA: 0x00155143 File Offset: 0x00153343
	public T AsT
	{
		get
		{
			return this;
		}
		set
		{
			this._target = (value as Object);
		}
	}

	// Token: 0x06003FD5 RID: 16341 RVA: 0x00155158 File Offset: 0x00153358
	public static implicit operator bool(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		return @object != null && @object != null;
	}

	// Token: 0x06003FD6 RID: 16342 RVA: 0x00155180 File Offset: 0x00153380
	public static implicit operator T(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return default(T);
		}
		if (@object == null)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06003FD7 RID: 16343 RVA: 0x001551C8 File Offset: 0x001533C8
	public static implicit operator Object(Ref<T> r)
	{
		Object @object = (r != null) ? r._target : null;
		if (@object == null)
		{
			return null;
		}
		if (@object == null)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x04005043 RID: 20547
	[SerializeField]
	private Object _target;
}
