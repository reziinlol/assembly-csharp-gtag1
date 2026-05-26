using System;
using UnityEngine;

// Token: 0x02000670 RID: 1648
public abstract class CosmeticCritterSpawner : CosmeticCritterHoldable
{
	// Token: 0x06002946 RID: 10566 RVA: 0x000DF130 File Offset: 0x000DD330
	public GameObject GetCritterPrefab()
	{
		return this.critterPrefab;
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000DF138 File Offset: 0x000DD338
	public CosmeticCritter GetCritter()
	{
		return this.cachedCritter;
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000DF140 File Offset: 0x000DD340
	public Type GetCritterType()
	{
		return this.cachedType;
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void SetRandomVariables(CosmeticCritter critter)
	{
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000DF148 File Offset: 0x000DD348
	public virtual void OnSpawn(CosmeticCritter critter)
	{
		this.numCritters++;
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x000DF158 File Offset: 0x000DD358
	public virtual void OnDespawn(CosmeticCritter critter)
	{
		this.numCritters = Math.Max(this.numCritters - 1, 0);
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x000DF16E File Offset: 0x000DD36E
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.cachedCritter == null)
		{
			this.cachedCritter = this.critterPrefab.GetComponent<CosmeticCritter>();
			this.cachedType = this.cachedCritter.GetType();
		}
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000DF1A6 File Offset: 0x000DD3A6
	protected override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x040035B5 RID: 13749
	[Tooltip("The critter prefab to spawn.")]
	[SerializeField]
	protected GameObject critterPrefab;

	// Token: 0x040035B6 RID: 13750
	[Tooltip("The maximum number of critters that this spawner can have active at once.")]
	[SerializeField]
	protected int maxCritters;

	// Token: 0x040035B7 RID: 13751
	protected CosmeticCritter cachedCritter;

	// Token: 0x040035B8 RID: 13752
	protected Type cachedType;

	// Token: 0x040035B9 RID: 13753
	protected int numCritters;

	// Token: 0x040035BA RID: 13754
	protected float nextLocalSpawnTime;
}
