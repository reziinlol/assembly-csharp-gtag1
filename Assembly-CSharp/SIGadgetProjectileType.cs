using System;

// Token: 0x020000E0 RID: 224
public interface SIGadgetProjectileType
{
	// Token: 0x06000546 RID: 1350
	void LocalProjectileHit(SIPlayer player = null);

	// Token: 0x06000547 RID: 1351
	void NetworkedProjectileHit(object[] data);
}
