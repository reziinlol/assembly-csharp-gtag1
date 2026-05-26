using System;

namespace GorillaTag
{
	// Token: 0x02001179 RID: 4473
	public class DelegateListProcessor<T> : DelegateListProcessorPlusMinus<DelegateListProcessor<T>, Action<T>>
	{
		// Token: 0x0600713D RID: 28989 RVA: 0x0024FC76 File Offset: 0x0024DE76
		public DelegateListProcessor()
		{
		}

		// Token: 0x0600713E RID: 28990 RVA: 0x0024FC7E File Offset: 0x0024DE7E
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600713F RID: 28991 RVA: 0x0024FC87 File Offset: 0x0024DE87
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x06007140 RID: 28992 RVA: 0x0024FCA7 File Offset: 0x0024DEA7
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x06007141 RID: 28993 RVA: 0x0024FCC7 File Offset: 0x0024DEC7
		protected override void ProcessItem(in Action<T> item)
		{
			item(this.m_data);
		}

		// Token: 0x04008138 RID: 33080
		private T m_data;
	}
}
