using System;
using UnityEngine;

// Token: 0x0200015E RID: 350
public class SIResourceMonkeIdol : SIResource
{
	// Token: 0x0600093E RID: 2366 RVA: 0x00031FA2 File Offset: 0x000301A2
	protected override void OnEnable()
	{
		base.OnEnable();
		this.depositEnabledParticle.SetActive(SIPlayer.LocalPlayer.CanLimitedResourceBeDeposited(this.limitedDepositType));
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00031FC5 File Offset: 0x000301C5
	public override void HandleDepositAuth(SIPlayer depositingPlayer)
	{
		SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(base.transform.position);
	}

	// Token: 0x04000B56 RID: 2902
	[SerializeField]
	private GameObject depositEnabledParticle;
}
