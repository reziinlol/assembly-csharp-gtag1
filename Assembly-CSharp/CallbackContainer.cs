using System;
using GorillaTag;

// Token: 0x02000D35 RID: 3381
internal class CallbackContainer<T> : ListProcessorAbstract<T> where T : ICallBack
{
	// Token: 0x06005349 RID: 21321 RVA: 0x001B427C File Offset: 0x001B247C
	public CallbackContainer() : base(100)
	{
	}

	// Token: 0x0600534A RID: 21322 RVA: 0x001B4286 File Offset: 0x001B2486
	public CallbackContainer(int capacity) : base(capacity)
	{
	}

	// Token: 0x0600534B RID: 21323 RVA: 0x001B428F File Offset: 0x001B248F
	public void TryRunCallbacks()
	{
		this.ProcessListSafe();
	}

	// Token: 0x0600534C RID: 21324 RVA: 0x001B4297 File Offset: 0x001B2497
	public void RunCallbacks()
	{
		this.ProcessList();
	}

	// Token: 0x0600534D RID: 21325 RVA: 0x001B42A0 File Offset: 0x001B24A0
	protected override void ProcessItem(in T item)
	{
		T t = item;
		t.CallBack();
	}
}
