using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x020011BA RID: 4538
	public interface IGuidedRefTargetMono : IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x060072A2 RID: 29346
		// (set) Token: 0x060072A3 RID: 29347
		GuidedRefBasicTargetInfo GRefTargetInfo { get; set; }

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x060072A4 RID: 29348
		Object GuidedRefTargetObject { get; }
	}
}
