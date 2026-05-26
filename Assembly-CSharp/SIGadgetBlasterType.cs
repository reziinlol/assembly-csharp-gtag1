using System;

// Token: 0x020000DE RID: 222
public interface SIGadgetBlasterType
{
	// Token: 0x06000537 RID: 1335
	void OnUpdateAuthority(float dt);

	// Token: 0x06000538 RID: 1336
	void OnUpdateRemote(float dt);

	// Token: 0x06000539 RID: 1337
	void SetStateShared();

	// Token: 0x0600053A RID: 1338
	void NetworkFireProjectile(object[] data);

	// Token: 0x0600053B RID: 1339
	void ApplyUpgradeNodes(SIUpgradeSet withUpgrades);
}
