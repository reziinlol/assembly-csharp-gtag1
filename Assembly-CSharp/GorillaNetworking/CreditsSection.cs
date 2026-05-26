using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02001044 RID: 4164
	[Serializable]
	internal class CreditsSection
	{
		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06006824 RID: 26660 RVA: 0x00219C7C File Offset: 0x00217E7C
		// (set) Token: 0x06006825 RID: 26661 RVA: 0x00219C84 File Offset: 0x00217E84
		public string Title { get; set; }

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06006826 RID: 26662 RVA: 0x00219C8D File Offset: 0x00217E8D
		// (set) Token: 0x06006827 RID: 26663 RVA: 0x00219C95 File Offset: 0x00217E95
		public List<string> Entries { get; set; }
	}
}
