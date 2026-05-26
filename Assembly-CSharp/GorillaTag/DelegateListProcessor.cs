using System;

namespace GorillaTag
{
	// Token: 0x02001178 RID: 4472
	public class DelegateListProcessor : DelegateListProcessorPlusMinus<DelegateListProcessor, Action>
	{
		// Token: 0x06007138 RID: 28984 RVA: 0x0024FC4C File Offset: 0x0024DE4C
		public DelegateListProcessor()
		{
		}

		// Token: 0x06007139 RID: 28985 RVA: 0x0024FC54 File Offset: 0x0024DE54
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600713A RID: 28986 RVA: 0x0024FC5D File Offset: 0x0024DE5D
		public void Invoke()
		{
			this.ProcessList();
		}

		// Token: 0x0600713B RID: 28987 RVA: 0x0024FC65 File Offset: 0x0024DE65
		public void InvokeSafe()
		{
			this.ProcessListSafe();
		}

		// Token: 0x0600713C RID: 28988 RVA: 0x0024FC6D File Offset: 0x0024DE6D
		protected override void ProcessItem(in Action del)
		{
			del();
		}
	}
}
