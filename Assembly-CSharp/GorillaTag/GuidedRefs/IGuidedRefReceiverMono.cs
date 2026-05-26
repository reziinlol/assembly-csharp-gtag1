using System;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011B9 RID: 4537
	public interface IGuidedRefReceiverMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x0600729D RID: 29341
		bool GuidedRefTryResolveReference(GuidedRefTryResolveInfo target);

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x0600729E RID: 29342
		// (set) Token: 0x0600729F RID: 29343
		int GuidedRefsWaitingToResolveCount { get; set; }

		// Token: 0x060072A0 RID: 29344
		void OnAllGuidedRefsResolved();

		// Token: 0x060072A1 RID: 29345
		void OnGuidedRefTargetDestroyed(int fieldId);
	}
}
