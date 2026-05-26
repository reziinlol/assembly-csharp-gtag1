using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
public class SIBlasterPumpableProjectile : MonoBehaviour, SIGadgetProjectileModifier
{
	// Token: 0x06000512 RID: 1298 RVA: 0x0001C51C File Offset: 0x0001A71C
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		SIGadgetPumpBlaster component = projectile.parentBlaster.GetComponent<SIGadgetPumpBlaster>();
		if (component == null)
		{
			return;
		}
		this.pumpChargedAmount = Mathf.Min(this.maxPump, component.currentPumpChargeAmount);
		projectile.startingVelocity += this.pumpChargedAmount;
		if (this.strengthPerPumpCharge > 0f)
		{
			SIBlasterDirectHitProjectile component2 = projectile.GetComponent<SIBlasterDirectHitProjectile>();
			if (component2 != null)
			{
				component2.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSplashProjectile component3 = projectile.GetComponent<SIBlasterSplashProjectile>();
			if (component3 != null)
			{
				component3.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSprayProjectile component4 = projectile.GetComponent<SIBlasterSprayProjectile>();
			if (component4 != null)
			{
				component4.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
		}
	}

	// Token: 0x040005D4 RID: 1492
	public float maxPump;

	// Token: 0x040005D5 RID: 1493
	public float pumpChargedAmount;

	// Token: 0x040005D6 RID: 1494
	public float velocityPerPumpCharge;

	// Token: 0x040005D7 RID: 1495
	public float strengthPerPumpCharge;
}
