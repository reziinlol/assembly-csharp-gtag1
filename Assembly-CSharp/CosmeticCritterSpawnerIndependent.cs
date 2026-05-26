using System;

// Token: 0x02000671 RID: 1649
public class CosmeticCritterSpawnerIndependent : CosmeticCritterSpawner
{
	// Token: 0x0600294F RID: 10575 RVA: 0x000DF1AE File Offset: 0x000DD3AE
	public virtual bool CanSpawnLocal()
	{
		return this.numCritters < this.maxCritters;
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000DF1BE File Offset: 0x000DD3BE
	public virtual bool CanSpawnRemote(double serverTime)
	{
		return this.numCritters < this.maxCritters && this.callLimiter.CheckCallServerTime(serverTime);
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000DF1DC File Offset: 0x000DD3DC
	protected override void OnEnable()
	{
		base.OnEnable();
		CosmeticCritterManager.Instance.RegisterIndependentSpawner(this);
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000DF1EF File Offset: 0x000DD3EF
	protected override void OnDisable()
	{
		base.OnDisable();
		CosmeticCritterManager.Instance.UnregisterIndependentSpawner(this);
	}
}
