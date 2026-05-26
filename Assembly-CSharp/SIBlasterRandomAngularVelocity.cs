using System;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class SIBlasterRandomAngularVelocity : MonoBehaviour, SIGadgetProjectileModifier
{
	// Token: 0x06000514 RID: 1300 RVA: 0x0001C5F4 File Offset: 0x0001A7F4
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.rb.angularVelocity = new Vector3(Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel));
	}

	// Token: 0x040005D8 RID: 1496
	public float maxVel;
}
