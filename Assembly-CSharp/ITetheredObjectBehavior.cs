using System;
using UnityEngine;

// Token: 0x020004AF RID: 1199
internal interface ITetheredObjectBehavior
{
	// Token: 0x06001D41 RID: 7489
	void DbgClear();

	// Token: 0x06001D42 RID: 7490
	void EnableDistanceConstraints(bool v, float playerScale);

	// Token: 0x06001D43 RID: 7491
	void EnableDynamics(bool enable, bool collider, bool kinematic);

	// Token: 0x06001D44 RID: 7492
	bool IsEnabled();

	// Token: 0x06001D45 RID: 7493
	void ReParent();

	// Token: 0x06001D46 RID: 7494
	bool ReturnStep();

	// Token: 0x06001D47 RID: 7495
	void TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership);
}
