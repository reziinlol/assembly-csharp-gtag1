using System;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public class MonkeyeProjectileTarget : MonoBehaviour
{
	// Token: 0x06001931 RID: 6449 RVA: 0x0008DBFD File Offset: 0x0008BDFD
	private void Awake()
	{
		this.monkeyeAI = base.GetComponent<MonkeyeAI>();
		this.notifier = base.GetComponentInChildren<SlingshotProjectileHitNotifier>();
	}

	// Token: 0x06001932 RID: 6450 RVA: 0x0008DC17 File Offset: 0x0008BE17
	private void OnEnable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit += this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit += this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x06001933 RID: 6451 RVA: 0x0008DC55 File Offset: 0x0008BE55
	private void OnDisable()
	{
		if (this.notifier != null)
		{
			this.notifier.OnProjectileHit -= this.Notifier_OnProjectileHit;
			this.notifier.OnPaperPlaneHit -= this.Notifier_OnPaperPlaneHit;
		}
	}

	// Token: 0x06001934 RID: 6452 RVA: 0x0008DC93 File Offset: 0x0008BE93
	private void Notifier_OnProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x06001935 RID: 6453 RVA: 0x0008DC93 File Offset: 0x0008BE93
	private void Notifier_OnPaperPlaneHit(PaperPlaneProjectile projectile, Collider collider)
	{
		this.monkeyeAI.SetSleep();
	}

	// Token: 0x04002434 RID: 9268
	private MonkeyeAI monkeyeAI;

	// Token: 0x04002435 RID: 9269
	private SlingshotProjectileHitNotifier notifier;
}
