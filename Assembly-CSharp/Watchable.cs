using System;
using System.Collections.Generic;

// Token: 0x020003AC RID: 940
public class Watchable<T>
{
	// Token: 0x1700022F RID: 559
	// (get) Token: 0x060016A4 RID: 5796 RVA: 0x00083ED8 File Offset: 0x000820D8
	// (set) Token: 0x060016A5 RID: 5797 RVA: 0x00083EE0 File Offset: 0x000820E0
	public T value
	{
		get
		{
			return this._value;
		}
		set
		{
			T value2 = this._value;
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016A6 RID: 5798 RVA: 0x00083F40 File Offset: 0x00082140
	public Watchable()
	{
	}

	// Token: 0x060016A7 RID: 5799 RVA: 0x00083F53 File Offset: 0x00082153
	public Watchable(T initial)
	{
		this._value = initial;
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x00083F70 File Offset: 0x00082170
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			foreach (Action<T> action in this.callbacks)
			{
				action(this._value);
			}
		}
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x00083FD8 File Offset: 0x000821D8
	public void RemoveCallback(Action<T> callback)
	{
		this.callbacks.Remove(callback);
	}

	// Token: 0x040021AC RID: 8620
	private T _value;

	// Token: 0x040021AD RID: 8621
	private List<Action<T>> callbacks = new List<Action<T>>();
}
