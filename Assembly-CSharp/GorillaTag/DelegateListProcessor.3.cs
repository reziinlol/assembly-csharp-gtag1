using System;

namespace GorillaTag
{
	// Token: 0x0200117A RID: 4474
	public class DelegateListProcessor<T1, T2> : DelegateListProcessorPlusMinus<DelegateListProcessor<T1, T2>, Action<T1, T2>>
	{
		// Token: 0x06007142 RID: 28994 RVA: 0x0024FCD6 File Offset: 0x0024DED6
		public DelegateListProcessor()
		{
		}

		// Token: 0x06007143 RID: 28995 RVA: 0x0024FCDE File Offset: 0x0024DEDE
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06007144 RID: 28996 RVA: 0x0024FCE7 File Offset: 0x0024DEE7
		public void InvokeSafe(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessListSafe();
			this.ResetData();
		}

		// Token: 0x06007145 RID: 28997 RVA: 0x0024FCFD File Offset: 0x0024DEFD
		public void Invoke(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessList();
			this.ResetData();
		}

		// Token: 0x06007146 RID: 28998 RVA: 0x0024FD13 File Offset: 0x0024DF13
		protected override void ProcessItem(in Action<T1, T2> item)
		{
			item(this.m_data1, this.m_data2);
		}

		// Token: 0x06007147 RID: 28999 RVA: 0x0024FD28 File Offset: 0x0024DF28
		private void SetData(in T1 data1, in T2 data2)
		{
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		// Token: 0x06007148 RID: 29000 RVA: 0x0024FD42 File Offset: 0x0024DF42
		private void ResetData()
		{
			this.m_data1 = default(T1);
			this.m_data2 = default(T2);
		}

		// Token: 0x04008139 RID: 33081
		private T1 m_data1;

		// Token: 0x0400813A RID: 33082
		private T2 m_data2;
	}
}
