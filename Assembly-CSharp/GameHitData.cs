using System;
using UnityEngine;

// Token: 0x020006CD RID: 1741
public struct GameHitData
{
	// Token: 0x04003842 RID: 14402
	public GameEntityId hitEntityId;

	// Token: 0x04003843 RID: 14403
	public GameEntityId hitByEntityId;

	// Token: 0x04003844 RID: 14404
	public int hitTypeId;

	// Token: 0x04003845 RID: 14405
	public Vector3 hitEntityPosition;

	// Token: 0x04003846 RID: 14406
	public Vector3 hitPosition;

	// Token: 0x04003847 RID: 14407
	public Vector3 hitImpulse;

	// Token: 0x04003848 RID: 14408
	public int hitAmount;

	// Token: 0x04003849 RID: 14409
	public int hittablePoint;
}
