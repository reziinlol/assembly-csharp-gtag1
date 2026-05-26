using System;
using System.Collections.Generic;

namespace GorillaTag.GuidedRefs.Internal
{
	// Token: 0x020011BE RID: 4542
	public class RelayInfo
	{
		// Token: 0x0400825C RID: 33372
		[NonSerialized]
		public IGuidedRefTargetMono targetMono;

		// Token: 0x0400825D RID: 33373
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> registeredFields;

		// Token: 0x0400825E RID: 33374
		[NonSerialized]
		public List<RegisteredReceiverFieldInfo> resolvedFields;
	}
}
