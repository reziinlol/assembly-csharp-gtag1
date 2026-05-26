using System;

namespace GorillaTag
{
	// Token: 0x0200117B RID: 4475
	public class InDelegateListProcessor<T> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T>, InAction<T>>
	{
		// Token: 0x06007149 RID: 29001 RVA: 0x0024FD5C File Offset: 0x0024DF5C
		public InDelegateListProcessor()
		{
		}

		// Token: 0x0600714A RID: 29002 RVA: 0x0024FD64 File Offset: 0x0024DF64
		public InDelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600714B RID: 29003 RVA: 0x0024FD6D File Offset: 0x0024DF6D
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x0600714C RID: 29004 RVA: 0x0024FD8D File Offset: 0x0024DF8D
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x0600714D RID: 29005 RVA: 0x0024FDAD File Offset: 0x0024DFAD
		protected override void ProcessItem(in InAction<T> item)
		{
			item(this.m_data);
		}

		// Token: 0x0400813B RID: 33083
		private T m_data;
	}
}
