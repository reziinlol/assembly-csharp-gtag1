using System;

// Token: 0x02000D70 RID: 3440
internal struct PlayIDWrappedData<T>
{
	// Token: 0x0600547C RID: 21628 RVA: 0x001B8EA1 File Offset: 0x001B70A1
	public PlayIDWrappedData(T initialValue)
	{
		this.currentValue = initialValue;
		this.initialValue = initialValue;
		this.id = EnterPlayID.GetCurrent();
	}

	// Token: 0x170007F7 RID: 2039
	// (get) Token: 0x0600547D RID: 21629 RVA: 0x001B8EBC File Offset: 0x001B70BC
	// (set) Token: 0x0600547E RID: 21630 RVA: 0x001B8ED8 File Offset: 0x001B70D8
	public T Value
	{
		get
		{
			if (!this.id.IsCurrent)
			{
				return this.initialValue;
			}
			return this.currentValue;
		}
		set
		{
			this.currentValue = value;
			this.id = EnterPlayID.GetCurrent();
		}
	}

	// Token: 0x04006527 RID: 25895
	private T currentValue;

	// Token: 0x04006528 RID: 25896
	private T initialValue;

	// Token: 0x04006529 RID: 25897
	private EnterPlayID id;
}
