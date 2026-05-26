using System;
using UnityEngine;

// Token: 0x02000672 RID: 1650
public abstract class CosmeticCritterSpawnerTimed : CosmeticCritterSpawnerIndependent
{
	// Token: 0x06002954 RID: 10580 RVA: 0x000DF202 File Offset: 0x000DD402
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(5, this.spawnIntervalMinMax.x, 0.5f);
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x000DF21A File Offset: 0x000DD41A
	public override bool CanSpawnLocal()
	{
		if (Time.time >= this.nextLocalSpawnTime)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
			return base.CanSpawnLocal();
		}
		return false;
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000DF258 File Offset: 0x000DD458
	public override bool CanSpawnRemote(double serverTime)
	{
		return base.CanSpawnRemote(serverTime);
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000DF261 File Offset: 0x000DD461
	protected override void OnEnable()
	{
		base.OnEnable();
		if (base.IsLocal)
		{
			this.nextLocalSpawnTime = Time.time + Random.Range(this.spawnIntervalMinMax.x, this.spawnIntervalMinMax.y);
		}
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000DF298 File Offset: 0x000DD498
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x040035BB RID: 13755
	[Tooltip("The minimum and maximum time to wait between spawn attempts.")]
	[SerializeField]
	private Vector2 spawnIntervalMinMax = new Vector2(2f, 5f);

	// Token: 0x040035BC RID: 13756
	[Tooltip("Currently does nothing.")]
	[SerializeField]
	[Range(0f, 1f)]
	private float spawnChance = 1f;
}
