using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011AD RID: 4525
	[Serializable]
	public struct GuidedRefBasicTargetInfo
	{
		// Token: 0x04008238 RID: 33336
		[SerializeField]
		public GuidedRefTargetIdSO targetId;

		// Token: 0x04008239 RID: 33337
		[Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
		[SerializeField]
		public GuidedRefHubIdSO[] hubIds;

		// Token: 0x0400823A RID: 33338
		[DebugOption]
		[SerializeField]
		public bool hackIgnoreDuplicateRegistration;
	}
}
