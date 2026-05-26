using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class TestSpawnGadget : MonoBehaviour
{
	// Token: 0x06000A8A RID: 2698 RVA: 0x00038A40 File Offset: 0x00036C40
	public void Spawn(GameEntityManager gameEntityManager)
	{
		SIUpgradeSet upgrades = default(SIUpgradeSet);
		foreach (TestSpawnGadget.SpawnTypeWithUpgrades spawnTypeWithUpgrades in this.testSpawnList)
		{
			if (!(spawnTypeWithUpgrades.prefab == null))
			{
				upgrades.Clear();
				foreach (SIUpgradeType upgrade in spawnTypeWithUpgrades.upgrades)
				{
					upgrades.Add(upgrade);
				}
				this.SpawnGadgetBatch(gameEntityManager, spawnTypeWithUpgrades.prefab, upgrades);
			}
		}
		if (!this.spawnAllGadgets)
		{
			return;
		}
		upgrades.Clear();
		foreach (GameEntity gameEntity in gameEntityManager.tempFactoryItems)
		{
			if (!this.skipEntityList.Contains(gameEntity))
			{
				this.SpawnGadgetBatch(gameEntityManager, gameEntity, upgrades);
			}
		}
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00038B48 File Offset: 0x00036D48
	private void SpawnGadgetBatch(GameEntityManager gameEntityManager, GameEntity entityToSpawn, SIUpgradeSet upgrades)
	{
		for (int i = 0; i < this.spawnBatchSize; i++)
		{
			gameEntityManager.RequestCreateItem(entityToSpawn.gameObject.name.GetStaticHash(), base.transform.position + Random.insideUnitSphere, base.transform.rotation, (long)upgrades.GetBits() << 32);
		}
	}

	// Token: 0x04000CCA RID: 3274
	public int spawnBatchSize = 4;

	// Token: 0x04000CCB RID: 3275
	public List<TestSpawnGadget.SpawnTypeWithUpgrades> testSpawnList = new List<TestSpawnGadget.SpawnTypeWithUpgrades>();

	// Token: 0x04000CCC RID: 3276
	public bool spawnAllGadgets;

	// Token: 0x04000CCD RID: 3277
	public List<GameEntity> skipEntityList = new List<GameEntity>();

	// Token: 0x0200018A RID: 394
	[Serializable]
	public struct SpawnTypeWithUpgrades
	{
		// Token: 0x04000CCE RID: 3278
		public GameEntity prefab;

		// Token: 0x04000CCF RID: 3279
		public SIUpgradeType[] upgrades;
	}
}
