using System;
using UnityEngine;

// Token: 0x020007E8 RID: 2024
public class GRSummonedEntity : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060033B7 RID: 13239 RVA: 0x0011CCCD File Offset: 0x0011AECD
	private void Awake()
	{
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x060033B8 RID: 13240 RVA: 0x0011CCDC File Offset: 0x0011AEDC
	public void OnEntityInit()
	{
		this.summonerEntityId = this.entity.createdByEntityId;
		if (this.summonerEntityId.IsValid())
		{
			this.summoner = this.FindSummoner();
			if (this.summoner != null)
			{
				this.summoner.OnSummonedEntityInit(this.entity);
			}
		}
	}

	// Token: 0x060033B9 RID: 13241 RVA: 0x0011CD2C File Offset: 0x0011AF2C
	public GameEntityId GetSummonerID()
	{
		return this.summonerEntityId;
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x0011CD34 File Offset: 0x0011AF34
	public void OnEntityDestroy()
	{
		if (this.summoner != null)
		{
			this.summoner.OnSummonedEntityDestroy(this.entity);
		}
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x0011CD50 File Offset: 0x0011AF50
	private IGRSummoningEntity FindSummoner()
	{
		if (this.summonerEntityId.IsValid())
		{
			GameEntity gameEntity = GhostReactorManager.Get(this.entity).gameEntityManager.GetGameEntity(this.summonerEntityId);
			if (gameEntity != null)
			{
				return gameEntity.GetComponent<IGRSummoningEntity>();
			}
		}
		return null;
	}

	// Token: 0x0400436E RID: 17262
	private GameEntityId summonerEntityId = GameEntityId.Invalid;

	// Token: 0x0400436F RID: 17263
	private GameEntity entity;

	// Token: 0x04004370 RID: 17264
	private IGRSummoningEntity summoner;
}
