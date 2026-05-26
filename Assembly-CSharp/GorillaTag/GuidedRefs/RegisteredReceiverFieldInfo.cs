using System;
using UnityEngine.Serialization;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011BC RID: 4540
	public struct RegisteredReceiverFieldInfo
	{
		// Token: 0x04008256 RID: 33366
		[FormerlySerializedAs("receiver")]
		public IGuidedRefReceiverMono receiverMono;

		// Token: 0x04008257 RID: 33367
		public int fieldId;

		// Token: 0x04008258 RID: 33368
		public int index;
	}
}
