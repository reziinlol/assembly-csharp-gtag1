using System;
using UnityEngine;

// Token: 0x02000AE6 RID: 2790
[Serializable]
public class OptionalRef<T> where T : Object
{
	// Token: 0x1700069D RID: 1693
	// (get) Token: 0x0600472C RID: 18220 RVA: 0x0017FFA5 File Offset: 0x0017E1A5
	// (set) Token: 0x0600472D RID: 18221 RVA: 0x0017FFAD File Offset: 0x0017E1AD
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
		}
	}

	// Token: 0x1700069E RID: 1694
	// (get) Token: 0x0600472E RID: 18222 RVA: 0x0017FFB8 File Offset: 0x0017E1B8
	// (set) Token: 0x0600472F RID: 18223 RVA: 0x0017FFE0 File Offset: 0x0017E1E0
	public T Value
	{
		get
		{
			if (this)
			{
				return this._target;
			}
			return default(T);
		}
		set
		{
			this._target = (value ? value : default(T));
		}
	}

	// Token: 0x06004730 RID: 18224 RVA: 0x0018000C File Offset: 0x0017E20C
	public static implicit operator bool(OptionalRef<T> r)
	{
		if (r == null)
		{
			return false;
		}
		if (!r._enabled)
		{
			return false;
		}
		Object @object = r._target;
		return @object != null && @object;
	}

	// Token: 0x06004731 RID: 18225 RVA: 0x00180040 File Offset: 0x0017E240
	public static implicit operator T(OptionalRef<T> r)
	{
		if (r == null)
		{
			return default(T);
		}
		if (!r._enabled)
		{
			return default(T);
		}
		Object @object = r._target;
		if (@object == null)
		{
			return default(T);
		}
		if (!@object)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06004732 RID: 18226 RVA: 0x001800A4 File Offset: 0x0017E2A4
	public static implicit operator Object(OptionalRef<T> r)
	{
		if (r == null)
		{
			return null;
		}
		if (!r._enabled)
		{
			return null;
		}
		Object @object = r._target;
		if (@object == null)
		{
			return null;
		}
		if (!@object)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x040059AA RID: 22954
	[SerializeField]
	private bool _enabled;

	// Token: 0x040059AB RID: 22955
	[SerializeField]
	private T _target;
}
