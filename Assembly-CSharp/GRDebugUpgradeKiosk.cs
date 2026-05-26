using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000759 RID: 1881
public class GRDebugUpgradeKiosk : MonoBehaviour
{
	// Token: 0x06002F91 RID: 12177 RVA: 0x00102CE7 File Offset: 0x00100EE7
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x06002F93 RID: 12179 RVA: 0x00102CF7 File Offset: 0x00100EF7
	public void OnButtonSpawnClub()
	{
		this.OnButtonSpawnEntity("GhostReactorToolClub", this.toolSpawnNode);
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x00102D0A File Offset: 0x00100F0A
	public void OnButtonSpawnCollector()
	{
		this.OnButtonSpawnEntity("GhostReactorToolCollector", this.toolSpawnNode);
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x00102D1D File Offset: 0x00100F1D
	public void OnButtonSpawnLantern()
	{
		this.OnButtonSpawnEntity("GhostReactorToolLantern", this.toolSpawnNode);
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x00102D30 File Offset: 0x00100F30
	public void OnButtonSpawnFlash()
	{
		this.OnButtonSpawnEntity("GhostReactorToolFlash", this.toolSpawnNode);
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x00102D43 File Offset: 0x00100F43
	public void OnButtonSpawnShieldGun()
	{
		this.OnButtonSpawnEntity("GhostReactorToolShieldGun", this.toolSpawnNode);
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x00102D56 File Offset: 0x00100F56
	public void OnButtonSpawnRevive()
	{
		this.OnButtonSpawnEntity("GhostReactorToolRevive", this.toolSpawnNode);
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x00102D69 File Offset: 0x00100F69
	public void OnButtonSpawnDirectionalShield()
	{
		this.OnButtonSpawnEntity("GhostReactorToolDirectionalShield", this.toolSpawnNode);
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x00102D7C File Offset: 0x00100F7C
	public void OnButtonSpawnStatusWatch()
	{
		this.OnButtonSpawnEntity("GhostReactorToolStatusWatch", this.toolSpawnNode);
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x00102D8F File Offset: 0x00100F8F
	public void OnButtonSpawnDockWrist()
	{
		this.OnButtonSpawnEntity("GhostReactorToolDockWrist", this.toolSpawnNode);
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x00102DA2 File Offset: 0x00100FA2
	public void OnButtonSpawnSmallBackpack()
	{
		this.OnButtonSpawnEntity("GhostReactorToolSmallBackpack", this.toolSpawnNode);
	}

	// Token: 0x06002F9D RID: 12189 RVA: 0x00102DB5 File Offset: 0x00100FB5
	public void OnButtonKillAllEnemies()
	{
		this.KillAllEnemies();
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x00102DBD File Offset: 0x00100FBD
	public void OnButtonSpawnPest()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyPest", this.enemySpawnNode);
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x00102DD0 File Offset: 0x00100FD0
	public void OnButtonSpawnChaser()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyChaser", this.enemySpawnNode);
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x00102DE3 File Offset: 0x00100FE3
	public void OnButtonSpawnPhantom()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyPhantom", this.enemySpawnNode);
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x00102DF6 File Offset: 0x00100FF6
	public void OnButtonSpawnRanged()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyRanged", this.enemySpawnNode);
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x00102E09 File Offset: 0x00101009
	public void OnButtonSpawnSummoner()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemySummoner", this.enemySpawnNode);
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x00102E1C File Offset: 0x0010101C
	public void OnButtonSpawnIceRanged()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyRangedIce", this.enemySpawnNode);
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x00102E2F File Offset: 0x0010102F
	public void OnButtonSpawnUpgEff1()
	{
		this.OnButtonSpawnEntity("GRUPowerEff1", this.upgradeSpawnNode);
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x00102E42 File Offset: 0x00101042
	public void OnButtonSpawnUpgEff2()
	{
		this.OnButtonSpawnEntity("GRUPowerEff2", this.upgradeSpawnNode);
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x00102E55 File Offset: 0x00101055
	public void OnButtonSpawnUpgEff3()
	{
		this.OnButtonSpawnEntity("GRUPowerEff3", this.upgradeSpawnNode);
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x00102E68 File Offset: 0x00101068
	public void OnButtonSpawnUpgBatonDmg1()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage1", this.upgradeSpawnNode);
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x00102E7B File Offset: 0x0010107B
	public void OnButtonSpawnUpgBatonDmg2()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage2", this.upgradeSpawnNode);
	}

	// Token: 0x06002FA9 RID: 12201 RVA: 0x00102E8E File Offset: 0x0010108E
	public void OnButtonSpawnUpgBatonDmg3()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage3", this.upgradeSpawnNode);
	}

	// Token: 0x06002FAA RID: 12202 RVA: 0x00102E2F File Offset: 0x0010102F
	public void OnButtonSpawnUpgEfficiency1()
	{
		this.OnButtonSpawnEntity("GRUPowerEff1", this.upgradeSpawnNode);
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x00102E42 File Offset: 0x00101042
	public void OnButtonSpawnUpgEfficiency2()
	{
		this.OnButtonSpawnEntity("GRUPowerEff2", this.upgradeSpawnNode);
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x00102E55 File Offset: 0x00101055
	public void OnButtonSpawnUpgEfficiency3()
	{
		this.OnButtonSpawnEntity("GRUPowerEff3", this.upgradeSpawnNode);
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x00102EA1 File Offset: 0x001010A1
	public void OnButtonSpawnChaosSeed()
	{
		this.OnButtonSpawnEntity("GhostReactorCollectibleSentientCore", this.enemySpawnNode);
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x00102EB4 File Offset: 0x001010B4
	public void OnButtonSpawnEntity(string entityName, Transform location)
	{
		if (location == null)
		{
			return;
		}
		Debug.Log("GRDebugUpgradeKiosk attempting to spawn " + entityName);
		int staticHash = entityName.GetStaticHash();
		GameEntityId gameEntityId = this.grManager.gameEntityManager.RequestCreateItem(staticHash, location.position, Quaternion.identity, 0L);
		GameAgent component = this.grManager.gameEntityManager.GetGameEntity(gameEntityId).gameObject.GetComponent<GameAgent>();
		if (component != null)
		{
			if (entityName.Contains("enemy", StringComparison.OrdinalIgnoreCase))
			{
				GhostReactorManager.entityDebugEnabled = true;
			}
			this.spawnedEntities.Add(gameEntityId);
			component.ApplyDestination(location.position);
			return;
		}
		Debug.Log("GRDebugUpgradeKiosk failed to spawn " + entityName);
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x00102F64 File Offset: 0x00101164
	public void KillAllEnemies()
	{
		foreach (GameEntityId entityId in this.spawnedEntities)
		{
			this.grManager.gameEntityManager.RequestDestroyItem(entityId);
		}
		this.spawnedEntities.Clear();
	}

	// Token: 0x04003D08 RID: 15624
	public Transform upgradeSpawnNode;

	// Token: 0x04003D09 RID: 15625
	public Transform toolSpawnNode;

	// Token: 0x04003D0A RID: 15626
	public Transform enemySpawnNode;

	// Token: 0x04003D0B RID: 15627
	private GhostReactorManager grManager;

	// Token: 0x04003D0C RID: 15628
	private GhostReactor reactor;

	// Token: 0x04003D0D RID: 15629
	private List<GameEntityId> spawnedEntities = new List<GameEntityId>();
}
