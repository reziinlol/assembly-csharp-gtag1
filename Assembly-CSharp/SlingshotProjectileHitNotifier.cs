using System;
using GorillaTag.GuidedRefs;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020004BD RID: 1213
public class SlingshotProjectileHitNotifier : BaseGuidedRefTargetMono
{
	// Token: 0x1400003D RID: 61
	// (add) Token: 0x06001D9B RID: 7579 RVA: 0x000A0290 File Offset: 0x0009E490
	// (remove) Token: 0x06001D9C RID: 7580 RVA: 0x000A02C8 File Offset: 0x0009E4C8
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileHit;

	// Token: 0x1400003E RID: 62
	// (add) Token: 0x06001D9D RID: 7581 RVA: 0x000A0300 File Offset: 0x0009E500
	// (remove) Token: 0x06001D9E RID: 7582 RVA: 0x000A0338 File Offset: 0x0009E538
	public event SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent OnPaperPlaneHit;

	// Token: 0x1400003F RID: 63
	// (add) Token: 0x06001D9F RID: 7583 RVA: 0x000A0370 File Offset: 0x0009E570
	// (remove) Token: 0x06001DA0 RID: 7584 RVA: 0x000A03A8 File Offset: 0x0009E5A8
	public event SlingshotProjectileHitNotifier.ProjectileHitEvent OnProjectileCollisionStay;

	// Token: 0x14000040 RID: 64
	// (add) Token: 0x06001DA1 RID: 7585 RVA: 0x000A03E0 File Offset: 0x0009E5E0
	// (remove) Token: 0x06001DA2 RID: 7586 RVA: 0x000A0418 File Offset: 0x0009E618
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerEnter;

	// Token: 0x14000041 RID: 65
	// (add) Token: 0x06001DA3 RID: 7587 RVA: 0x000A0450 File Offset: 0x0009E650
	// (remove) Token: 0x06001DA4 RID: 7588 RVA: 0x000A0488 File Offset: 0x0009E688
	public event SlingshotProjectileHitNotifier.ProjectileTriggerEvent OnProjectileTriggerExit;

	// Token: 0x06001DA5 RID: 7589 RVA: 0x000A04BD File Offset: 0x0009E6BD
	public void InvokeHit(SlingshotProjectile projectile, Collision collision)
	{
		if (this.projectileType != "" && projectile.tag != this.projectileType)
		{
			return;
		}
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileHit = this.OnProjectileHit;
		if (onProjectileHit == null)
		{
			return;
		}
		onProjectileHit(projectile, collision);
	}

	// Token: 0x06001DA6 RID: 7590 RVA: 0x000A04F7 File Offset: 0x0009E6F7
	public void InvokeHit(PaperPlaneProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.PaperPlaneProjectileHitEvent onPaperPlaneHit = this.OnPaperPlaneHit;
		if (onPaperPlaneHit == null)
		{
			return;
		}
		onPaperPlaneHit(projectile, collider);
	}

	// Token: 0x06001DA7 RID: 7591 RVA: 0x000A050B File Offset: 0x0009E70B
	public void InvokeCollisionStay(SlingshotProjectile projectile, Collision collision)
	{
		SlingshotProjectileHitNotifier.ProjectileHitEvent onProjectileCollisionStay = this.OnProjectileCollisionStay;
		if (onProjectileCollisionStay == null)
		{
			return;
		}
		onProjectileCollisionStay(projectile, collision);
	}

	// Token: 0x06001DA8 RID: 7592 RVA: 0x000A051F File Offset: 0x0009E71F
	public void InvokeTriggerEnter(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerEnter = this.OnProjectileTriggerEnter;
		if (onProjectileTriggerEnter == null)
		{
			return;
		}
		onProjectileTriggerEnter(projectile, collider);
	}

	// Token: 0x06001DA9 RID: 7593 RVA: 0x000A0533 File Offset: 0x0009E733
	public void InvokeTriggerExit(SlingshotProjectile projectile, Collider collider)
	{
		SlingshotProjectileHitNotifier.ProjectileTriggerEvent onProjectileTriggerExit = this.OnProjectileTriggerExit;
		if (onProjectileTriggerExit == null)
		{
			return;
		}
		onProjectileTriggerExit(projectile, collider);
	}

	// Token: 0x06001DAA RID: 7594 RVA: 0x000A0547 File Offset: 0x0009E747
	private new void OnDestroy()
	{
		this.OnProjectileHit = null;
		this.OnProjectileCollisionStay = null;
		this.OnProjectileTriggerEnter = null;
		this.OnProjectileTriggerExit = null;
	}

	// Token: 0x04002802 RID: 10242
	[TagField]
	[SerializeField]
	private string projectileType;

	// Token: 0x020004BE RID: 1214
	// (Invoke) Token: 0x06001DAD RID: 7597
	public delegate void ProjectileHitEvent(SlingshotProjectile projectile, Collision collision);

	// Token: 0x020004BF RID: 1215
	// (Invoke) Token: 0x06001DB1 RID: 7601
	public delegate void PaperPlaneProjectileHitEvent(PaperPlaneProjectile projectile, Collider collider);

	// Token: 0x020004C0 RID: 1216
	// (Invoke) Token: 0x06001DB5 RID: 7605
	public delegate void ProjectileTriggerEvent(SlingshotProjectile projectile, Collider collider);
}
