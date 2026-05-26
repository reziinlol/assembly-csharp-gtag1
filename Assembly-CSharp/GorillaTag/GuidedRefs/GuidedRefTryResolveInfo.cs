using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011BD RID: 4541
	public struct GuidedRefTryResolveInfo
	{
		// Token: 0x04008259 RID: 33369
		public int fieldId;

		// Token: 0x0400825A RID: 33370
		public int index;

		// Token: 0x0400825B RID: 33371
		[FormerlySerializedAs("target")]
		public IGuidedRefTargetMono targetMono;
	}
}
