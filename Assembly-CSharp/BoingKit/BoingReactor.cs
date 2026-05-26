using System;

namespace BoingKit
{
	// Token: 0x02001370 RID: 4976
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x06007D4E RID: 32078 RVA: 0x00290C62 File Offset: 0x0028EE62
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x06007D4F RID: 32079 RVA: 0x00290C6A File Offset: 0x0028EE6A
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007D50 RID: 32080 RVA: 0x00290C72 File Offset: 0x0028EE72
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
