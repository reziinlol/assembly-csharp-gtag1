using System;

namespace GorillaTag
{
	// Token: 0x0200117E RID: 4478
	public abstract class ListProcessorAbstract<T> : ListProcessor<T>
	{
		// Token: 0x06007163 RID: 29027 RVA: 0x0025005B File Offset: 0x0024E25B
		protected ListProcessorAbstract()
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x06007164 RID: 29028 RVA: 0x00250076 File Offset: 0x0024E276
		protected ListProcessorAbstract(int capacity) : base(capacity, null)
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x06007165 RID: 29029
		protected abstract void ProcessItem(in T item);
	}
}
