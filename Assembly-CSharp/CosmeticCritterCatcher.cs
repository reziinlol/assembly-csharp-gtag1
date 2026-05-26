using System;
using UnityEngine;

// Token: 0x0200066C RID: 1644
public abstract class CosmeticCritterCatcher : CosmeticCritterHoldable
{
	// Token: 0x0600291D RID: 10525 RVA: 0x000DE723 File Offset: 0x000DC923
	public CosmeticCritterSpawner GetLinkedSpawner()
	{
		return this.optionalLinkedSpawner;
	}

	// Token: 0x0600291E RID: 10526
	public abstract CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter);

	// Token: 0x0600291F RID: 10527 RVA: 0x000DE72B File Offset: 0x000DC92B
	public virtual bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		return this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x06002920 RID: 10528
	public abstract void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime);

	// Token: 0x06002921 RID: 10529 RVA: 0x000DE739 File Offset: 0x000DC939
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterCatcher(this);
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000DE74C File Offset: 0x000DC94C
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterCatcher(this);
	}

	// Token: 0x0400359E RID: 13726
	[SerializeField]
	[Tooltip("If this catcher is capable of spawning immediately after catching, the linked spawner must be assigned here.")]
	protected CosmeticCritterSpawner optionalLinkedSpawner;
}
