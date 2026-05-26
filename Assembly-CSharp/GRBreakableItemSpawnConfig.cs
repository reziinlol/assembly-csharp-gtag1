using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000750 RID: 1872
[CreateAssetMenu(fileName = "GhostReactorBreakableItemSpawnConfig", menuName = "ScriptableObjects/GhostReactorBreakableItemSpawnConfig")]
public class GRBreakableItemSpawnConfig : ScriptableObject
{
	// Token: 0x06002F67 RID: 12135 RVA: 0x00102400 File Offset: 0x00100600
	public bool TryForRandomItem(GameEntity spawnFromEntity, out GameEntity entity, int sanity = 0)
	{
		GRBreakableItemSpawnConfig @override = this.GetOverride(spawnFromEntity);
		if (sanity <= 5 && @override != null)
		{
			return @override.TryForRandomItem(spawnFromEntity, out entity, sanity + 1);
		}
		if (sanity > 5)
		{
			Debug.LogError("Circular override loop");
		}
		if (Random.Range(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = Random.Range(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06002F68 RID: 12136 RVA: 0x001024C4 File Offset: 0x001006C4
	public bool TryForRandomItem(GhostReactor reactor, ref SRand srand, out GameEntity entity, int sanity = 0)
	{
		GRBreakableItemSpawnConfig @override = this.GetOverride(reactor);
		if (sanity <= 5 && @override != null)
		{
			return @override.TryForRandomItem(reactor, ref srand, out entity, sanity + 1);
		}
		if (sanity > 5)
		{
			Debug.LogError("Circular override loop");
		}
		if (srand.NextFloat(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = srand.NextFloat(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x0010258C File Offset: 0x0010078C
	private GRBreakableItemSpawnConfig GetOverride(GameEntity entity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(entity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return this.GetOverride(ghostReactorManager.reactor);
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x001025B8 File Offset: 0x001007B8
	private GRBreakableItemSpawnConfig GetOverride(GhostReactor reactor)
	{
		if (reactor == null)
		{
			return null;
		}
		GhostReactorLevelGenConfig currLevelGenConfig = reactor.GetCurrLevelGenConfig();
		if (currLevelGenConfig == null || currLevelGenConfig.dropTableOverrides == null)
		{
			return null;
		}
		return currLevelGenConfig.dropTableOverrides.GetOverride(this);
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x001025FC File Offset: 0x001007FC
	private void OnValidate()
	{
		this.precomputedItemTotalWeight = 0f;
		for (int i = 0; i < this.perItemProbabilities.Count; i++)
		{
			this.precomputedItemTotalWeight += this.perItemProbabilities[i].probability;
		}
	}

	// Token: 0x04003CDE RID: 15582
	[SerializeField]
	[Range(0f, 1f)]
	public float spawnAnythingProbability = 0.2f;

	// Token: 0x04003CDF RID: 15583
	public List<GRBreakableItemSpawnConfig.ItemProbability> perItemProbabilities = new List<GRBreakableItemSpawnConfig.ItemProbability>();

	// Token: 0x04003CE0 RID: 15584
	[SerializeField]
	[ReadOnly]
	private float precomputedItemTotalWeight;

	// Token: 0x02000751 RID: 1873
	[Serializable]
	public struct ItemProbability
	{
		// Token: 0x04003CE1 RID: 15585
		public GameEntity entity;

		// Token: 0x04003CE2 RID: 15586
		public float probability;
	}
}
