using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000E32 RID: 3634
	public class SubscriptionStatus
	{
		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06005875 RID: 22645 RVA: 0x001CA8A2 File Offset: 0x001C8AA2
		// (set) Token: 0x06005876 RID: 22646 RVA: 0x001CA8AA File Offset: 0x001C8AAA
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x06005877 RID: 22647 RVA: 0x001CA8B3 File Offset: 0x001C8AB3
		// (set) Token: 0x06005878 RID: 22648 RVA: 0x001CA8BB File Offset: 0x001C8ABB
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06005879 RID: 22649 RVA: 0x001CA8C4 File Offset: 0x001C8AC4
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000E33 RID: 3635
		public enum Platform
		{
			// Token: 0x040068F8 RID: 26872
			Windows,
			// Token: 0x040068F9 RID: 26873
			Android
		}

		// Token: 0x02000E34 RID: 3636
		public enum TransactionType
		{
			// Token: 0x040068FB RID: 26875
			Unknown,
			// Token: 0x040068FC RID: 26876
			Paid,
			// Token: 0x040068FD RID: 26877
			Redeem,
			// Token: 0x040068FE RID: 26878
			FreeTrial
		}
	}
}
