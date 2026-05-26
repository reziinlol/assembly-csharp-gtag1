using System;

namespace GorillaTag
{
	// Token: 0x0200117C RID: 4476
	public class InDelegateListProcessor<T1, T2> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T1, T2>, InAction<T1, T2>>
	{
		// Token: 0x0600714E RID: 29006 RVA: 0x0024FDBC File Offset: 0x0024DFBC
		public InDelegateListProcessor()
		{
		}

		// Token: 0x0600714F RID: 29007 RVA: 0x0024FDC4 File Offset: 0x0024DFC4
		public InDelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06007150 RID: 29008 RVA: 0x0024FDCD File Offset: 0x0024DFCD
		public void InvokeSafe(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessListSafe();
			this.ResetData();
		}

		// Token: 0x06007151 RID: 29009 RVA: 0x0024FDE3 File Offset: 0x0024DFE3
		public void Invoke(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessList();
			this.ResetData();
		}

		// Token: 0x06007152 RID: 29010 RVA: 0x0024FDF9 File Offset: 0x0024DFF9
		protected override void ProcessItem(in InAction<T1, T2> item)
		{
			item(this.m_data1, this.m_data2);
		}

		// Token: 0x06007153 RID: 29011 RVA: 0x0024FE0E File Offset: 0x0024E00E
		private void SetData(in T1 data1, in T2 data2)
		{
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		// Token: 0x06007154 RID: 29012 RVA: 0x0024FE28 File Offset: 0x0024E028
		private void ResetData()
		{
			this.m_data1 = default(T1);
			this.m_data2 = default(T2);
		}

		// Token: 0x0400813C RID: 33084
		private T1 m_data1;

		// Token: 0x0400813D RID: 33085
		private T2 m_data2;
	}
}
