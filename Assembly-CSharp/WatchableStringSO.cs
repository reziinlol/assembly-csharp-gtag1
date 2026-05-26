using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003AE RID: 942
[CreateAssetMenu(fileName = "WatchableStringSO", menuName = "ScriptableObjects/WatchableStringSO")]
public class WatchableStringSO : ScriptableObject
{
	// Token: 0x17000232 RID: 562
	// (get) Token: 0x060016B2 RID: 5810 RVA: 0x00084121 File Offset: 0x00082321
	// (set) Token: 0x060016B3 RID: 5811 RVA: 0x00084129 File Offset: 0x00082329
	private string _value { get; set; }

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x060016B4 RID: 5812 RVA: 0x00084132 File Offset: 0x00082332
	// (set) Token: 0x060016B5 RID: 5813 RVA: 0x00084140 File Offset: 0x00082340
	public string Value
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
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x000841A0 File Offset: 0x000823A0
	private void EnsureInitialized()
	{
		if (!this.enterPlayID.IsCurrent)
		{
			this._value = this.InitialValue;
			this.callbacks = new List<Action<string>>();
			this.enterPlayID = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x000841D4 File Offset: 0x000823D4
	public void AddCallback(Action<string> callback, bool shouldCallbackNow = false)
	{
		this.EnsureInitialized();
		this.callbacks.Add(callback);
		if (shouldCallbackNow)
		{
			string value = this._value;
			foreach (Action<string> action in this.callbacks)
			{
				action(value);
			}
		}
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x00084244 File Offset: 0x00082444
	public void RemoveCallback(Action<string> callback)
	{
		this.EnsureInitialized();
		this.callbacks.Remove(callback);
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x00084259 File Offset: 0x00082459
	public override string ToString()
	{
		return this.Value;
	}

	// Token: 0x040021B2 RID: 8626
	[TextArea]
	public string InitialValue;

	// Token: 0x040021B4 RID: 8628
	private EnterPlayID enterPlayID;

	// Token: 0x040021B5 RID: 8629
	private List<Action<string>> callbacks;
}
