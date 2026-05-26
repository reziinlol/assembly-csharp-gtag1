using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003AD RID: 941
public class WatchableGenericSO<T> : ScriptableObject
{
	// Token: 0x17000230 RID: 560
	// (get) Token: 0x060016AA RID: 5802 RVA: 0x00083FE7 File Offset: 0x000821E7
	// (set) Token: 0x060016AB RID: 5803 RVA: 0x00083FEF File Offset: 0x000821EF
	private T _value { get; set; }

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x060016AC RID: 5804 RVA: 0x00083FF8 File Offset: 0x000821F8
	// (set) Token: 0x060016AD RID: 5805 RVA: 0x00084008 File Offset: 0x00082208
	public T Value
	{
		get
		{
			this.EnsureInitialized();
			return this._value;
		}
		set
		{
			this.EnsureInitialized();
			this._value = value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016AE RID: 5806 RVA: 0x00084068 File Offset: 0x00082268
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<T>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x060016AF RID: 5807 RVA: 0x0008409C File Offset: 0x0008229C
	public void AddCallback(Action<T> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			T value = this._value;
			foreach (Action<T> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x0008410C File Offset: 0x0008230C
	public void RemoveCallback(Action<T> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x040021AE RID: 8622
	public T InitialValue;

	// Token: 0x040021B0 RID: 8624
	private EnterPlayID enterPlayID;

	// Token: 0x040021B1 RID: 8625
	private List<Action<T>> callbacks;
}
