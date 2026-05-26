using System;

// Token: 0x02000D37 RID: 3383
internal class CallbackContainerUnique<T> : CallbackContainer<T> where T : class, ICallbackUnique
{
	// Token: 0x06005350 RID: 21328 RVA: 0x001B42C1 File Offset: 0x001B24C1
	public CallbackContainerUnique() : base(10)
	{
	}

	// Token: 0x06005351 RID: 21329 RVA: 0x001B42CB File Offset: 0x001B24CB
	public CallbackContainerUnique(int capacity) : base(capacity)
	{
	}

	// Token: 0x06005352 RID: 21330 RVA: 0x001B42D4 File Offset: 0x001B24D4
	public override void Add(in T item)
	{
		T t = item;
		if (t.Registered)
		{
			return;
		}
		base.Add(item);
		t = item;
		t.Registered = true;
	}

	// Token: 0x06005353 RID: 21331 RVA: 0x001B4314 File Offset: 0x001B2514
	public override bool Remove(in T item)
	{
		T t = item;
		if (!t.Registered)
		{
			return false;
		}
		base.Remove(item);
		t = item;
		t.Registered = false;
		return true;
	}
}
