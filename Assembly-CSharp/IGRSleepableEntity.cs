using System;
using UnityEngine;

// Token: 0x0200082A RID: 2090
public interface IGRSleepableEntity
{
	// Token: 0x170004AD RID: 1197
	// (get) Token: 0x060035AF RID: 13743
	Vector3 Position { get; }

	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x060035B0 RID: 13744
	float WakeUpRadius { get; }

	// Token: 0x060035B1 RID: 13745
	bool IsSleeping();

	// Token: 0x060035B2 RID: 13746
	void WakeUp();

	// Token: 0x060035B3 RID: 13747
	void Sleep();
}
