using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000704 RID: 1796
[NetworkBehaviourWeaved(0)]
public class GhostReactorManager : NetworkComponent, IGameEntityZoneComponent
{
	// Token: 0x06002D2C RID: 11564 RVA: 0x000F5E90 File Offset: 0x000F4090
	protected override void Awake()
	{
		base.Awake();
		this.noiseEventManager = base.GetComponent<GRNoiseEventManager>();
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x000F5EA4 File Offset: 0x000F40A4
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
	}

	// Token: 0x06002D2E RID: 11566 RVA: 0x0008D8AF File Offset: 0x0008BAAF
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x000F5EB2 File Offset: 0x000F40B2
	public bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x000F5EBF File Offset: 0x000F40BF
	private bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x000F5ECD File Offset: 0x000F40CD
	private bool IsAuthorityPlayer(Player player)
	{
		return this.gameEntityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x000F5EDB File Offset: 0x000F40DB
	private Player GetAuthorityPlayer()
	{
		return this.gameEntityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000F5EE8 File Offset: 0x000F40E8
	public bool IsZoneActive()
	{
		return this.gameEntityManager.IsZoneActive();
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000F5EF5 File Offset: 0x000F40F5
	public bool IsPositionInZone(Vector3 pos)
	{
		return this.gameEntityManager.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000F5F03 File Offset: 0x000F4103
	public bool IsValidClientRPC(Player sender)
	{
		return this.gameEntityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x000F5F11 File Offset: 0x000F4111
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000F5F20 File Offset: 0x000F4120
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000F5F30 File Offset: 0x000F4130
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000F5F3F File Offset: 0x000F413F
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender);
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000F5F4D File Offset: 0x000F414D
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, entityNetId);
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000F5F5C File Offset: 0x000F415C
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000F5F6C File Offset: 0x000F416C
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.gameEntityManager.IsValidAuthorityRPC(sender, pos);
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000F5F7B File Offset: 0x000F417B
	public static GhostReactorManager Get(GameEntity gameEntity)
	{
		if (gameEntity == null || gameEntity.manager == null)
		{
			return null;
		}
		return gameEntity.manager.ghostReactorManager;
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void RefreshShiftCredit()
	{
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000F5FA4 File Offset: 0x000F41A4
	[PunRPC]
	public void RefreshShiftCreditRPC(PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RefreshShiftCredit))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (grplayer.mothershipId.IsNullOrEmpty())
		{
			return;
		}
		ProgressionManager.Instance.GetShiftCredit(grplayer.mothershipId);
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000F6008 File Offset: 0x000F4208
	public void SendMothershipId()
	{
		string mothershipId = MothershipClientContext.MothershipId;
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000F6010 File Offset: 0x000F4210
	[PunRPC]
	public void SendMothershipIdRPC(string mothershipId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SendMothershipId))
		{
			return;
		}
		if (mothershipId.Length > 40)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (!grplayer.mothershipId.IsNullOrEmpty())
		{
			return;
		}
		if (grplayer.mothershipId.IsNullOrEmpty())
		{
			grplayer.mothershipId = mothershipId.Trim();
			ProgressionManager.Instance.GetShiftCredit(grplayer.mothershipId);
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000F6098 File Offset: 0x000F4298
	public void RequestCollectItem(GameEntityId collectibleEntityId, GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestCollectItemRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000F60EC File Offset: 0x000F42EC
	public void RequestDepositCollectible(GameEntityId collectibleEntityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				-1,
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000F615C File Offset: 0x000F435C
	[PunRPC]
	public void RequestCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, collectibleEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestCollectItemLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsEntityNearEntity(collectibleEntityNetId, collectorEntityNetId, 16f))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyCollectItemRPC", RpcTarget.All, new object[]
			{
				collectibleEntityNetId,
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000F6208 File Offset: 0x000F4408
	[PunRPC]
	public void ApplyCollectItemRPC(int collectibleEntityNetId, int collectorEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyCollectItem))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(collectingPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity2 != null)
			{
				GRToolCollector component2 = gameEntity2.GetComponent<GRToolCollector>();
				if (component2 != null && component2.tool != null)
				{
					component2.PerformCollection(component);
				}
			}
			else
			{
				ProgressionManager.Instance.DepositCore(component.type);
				this.ReportCoreCollection(grplayer, component.type);
				int count = this.reactor.vrRigs.Count;
				int coreValue = component.energyValue / 4;
				for (int i = 0; i < count; i++)
				{
					GRPlayer.Get(this.reactor.vrRigs[i]).IncrementCoresCollectedGroup(coreValue);
				}
				grplayer.IncrementCoresCollectedPlayer(coreValue);
			}
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000F6378 File Offset: 0x000F4578
	public void RequestApplySeedExtractorState(int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply)
	{
		this.photonView.RPC("RequestApplySeedExtractorStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			coreCount,
			coresProcessedByOverdrive,
			researchPoints,
			coreProcessingPercentage,
			overdriveSupply
		});
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000F63D0 File Offset: 0x000F45D0
	[PunRPC]
	public void RequestApplySeedExtractorStateRPC(int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SeedExtractorAction) || coreCount < 0 || coresProcessedByOverdrive < 0 || researchPoints < 0 || !float.IsFinite(coreProcessingPercentage) || !float.IsFinite(overdriveSupply))
		{
			return;
		}
		if (info.Sender.ActorNumber != this.reactor.seedExtractor.CurrentPlayerActorNumber)
		{
			return;
		}
		this.photonView.RPC("ApplySeedExtractorStateRPC", RpcTarget.All, new object[]
		{
			info.Sender.ActorNumber,
			coreCount,
			coresProcessedByOverdrive,
			researchPoints,
			coreProcessingPercentage,
			overdriveSupply
		});
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000F6494 File Offset: 0x000F4694
	[PunRPC]
	public void ApplySeedExtractorStateRPC(int playerActorNumber, int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.SeedExtractorAction) || coreCount < 0 || coresProcessedByOverdrive < 0 || researchPoints < 0 || !float.IsFinite(coreProcessingPercentage) || !float.IsFinite(overdriveSupply))
		{
			return;
		}
		if (this.reactor != null && this.reactor.seedExtractor != null)
		{
			this.reactor.seedExtractor.ApplyState(playerActorNumber, coreCount, coresProcessedByOverdrive, researchPoints, coreProcessingPercentage, overdriveSupply);
		}
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000F651C File Offset: 0x000F471C
	public void RequestDistillCollectible(GameEntityId collectibleEntityId, Player sender)
	{
		if (!this.IsValidAuthorityRPC(sender))
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(collectibleEntityId);
		if (gameEntity != null)
		{
			this.photonView.RPC("DistillItemRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(collectibleEntityId),
				gameEntity.lastHeldByActorNumber
			});
		}
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x000F6584 File Offset: 0x000F4784
	[PunRPC]
	public void DistillItemRPC(int collectibleEntityNetId, int collectingPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectibleEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.DistillItem))
		{
			return;
		}
		if (GRPlayer.Get(collectingPlayerActorNumber) == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectibleEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity == null)
			{
				return;
			}
			GRCollectible component = gameEntity.GetComponent<GRCollectible>();
			if (component == null)
			{
				return;
			}
			Debug.LogWarning("Warning - NOT IMPLEMENTED - Return validating inserting core for distillery.");
			if (gameEntity != null && component != null)
			{
				component.InvokeOnCollected();
			}
			this.gameEntityManager.DestroyItemLocal(entityIdFromNetId);
		}
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x000F6634 File Offset: 0x000F4834
	public void RequestChargeTool(GameEntityId collectorEntityId, GameEntityId targetToolId, int targetEnergyDelta = 0, bool useCollectorEnergy = true)
	{
		this.photonView.RPC("RequestChargeToolRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId),
			this.gameEntityManager.GetNetIdFromEntityId(targetToolId),
			targetEnergyDelta,
			useCollectorEnergy
		});
	}

	// Token: 0x06002D4C RID: 11596 RVA: 0x000F6698 File Offset: 0x000F4898
	[PunRPC]
	public void RequestChargeToolRPC(int collectorEntityNetId, int targetToolNetId, int targetEnergyDelta, bool useCollectorEnergy, PhotonMessageInfo info)
	{
		GamePlayer player;
		if (!this.IsValidAuthorityRPC(info.Sender) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId) || !this.gameEntityManager.IsEntityNearEntity(collectorEntityNetId, targetToolNetId, 16f) || !GamePlayer.TryGetGamePlayer(info.Sender.ActorNumber, out player) || !this.gameEntityManager.IsPlayerHandNearEntity(player, collectorEntityNetId, false, true, 16f) || !this.gameEntityManager.IsPlayerHandNearEntity(player, targetToolNetId, false, true, 16f))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestChargeToolLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (true)
		{
			this.photonView.RPC("ApplyChargeToolRPC", RpcTarget.All, new object[]
			{
				collectorEntityNetId,
				targetToolNetId,
				targetEnergyDelta,
				useCollectorEnergy,
				info.Sender
			});
		}
	}

	// Token: 0x06002D4D RID: 11597 RVA: 0x000F679C File Offset: 0x000F499C
	[PunRPC]
	public void ApplyChargeToolRPC(int collectorEntityNetId, int targetToolNetId, int targetEnergyDelta, bool useCollectorEnergy, Player collectingPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyChargeTool) || !this.gameEntityManager.IsValidNetId(collectorEntityNetId) || !this.gameEntityManager.IsValidNetId(targetToolNetId))
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			GameEntityId entityIdFromNetId2 = this.gameEntityManager.GetEntityIdFromNetId(targetToolNetId);
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(entityIdFromNetId2);
			if (gameEntity != null && gameEntity2 != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				GRTool component2 = gameEntity2.GetComponent<GRTool>();
				if (component != null && component.tool != null && component2 != null)
				{
					int num = (targetEnergyDelta != 0) ? targetEnergyDelta : 100;
					int b = Mathf.Max(component2.GetEnergyMax() - component2.energy, 0);
					int num2;
					if (!useCollectorEnergy)
					{
						num2 = Mathf.Min(num, b);
						Debug.Log(string.Format("Apply SelfCharge {0}", num2));
					}
					else
					{
						num2 = Mathf.Min(Mathf.Min(component.tool.energy, num), b);
					}
					if (num2 > 0)
					{
						if (useCollectorEnergy)
						{
							component.tool.SetEnergy(component.tool.energy - num2);
						}
						component2.RefillEnergy(num2, entityIdFromNetId);
						component.PlayChargeEffect(component2);
					}
				}
			}
		}
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x000F690F File Offset: 0x000F4B0F
	public void RequestDepositCurrency(GameEntityId collectorEntityId)
	{
		this.photonView.RPC("RequestDepositCurrencyRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(collectorEntityId)
		});
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x000F6944 File Offset: 0x000F4B44
	[PunRPC]
	public void RequestDepositCurrencyRPC(int collectorEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, collectorEntityNetId))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestDepositCurrencyLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
		this.gameEntityManager.GetGameEntity(entityIdFromNetId);
		GamePlayer player;
		if (GamePlayer.TryGetGamePlayer(info.Sender.ActorNumber, out player) && this.gameEntityManager.IsPlayerHandNearEntity(player, collectorEntityNetId, false, true, 16f) && (grplayer.transform.position - this.reactor.currencyDepositor.transform.position).magnitude < 16f)
		{
			this.photonView.RPC("ApplyDepositCurrencyRPC", RpcTarget.All, new object[]
			{
				collectorEntityNetId,
				info.Sender.ActorNumber
			});
		}
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x000F6A40 File Offset: 0x000F4C40
	[PunRPC]
	public void ApplyDepositCurrencyRPC(int collectorEntityNetId, int targetPlayerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, collectorEntityNetId) || this.reactor == null || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyDepositCurrency))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(targetPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (true)
		{
			GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(collectorEntityNetId);
			GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity != null)
			{
				GRToolCollector component = gameEntity.GetComponent<GRToolCollector>();
				if (component != null && component.tool != null)
				{
					int energy = component.tool.energy;
					int energyDepositPerUse = component.energyDepositPerUse;
					if (energy >= energyDepositPerUse)
					{
						this.ReportCoreCollection(grplayer, ProgressionManager.CoreType.Core);
						int count = this.reactor.vrRigs.Count;
						int coreValue = energyDepositPerUse / 4;
						for (int i = 0; i < count; i++)
						{
							GRPlayer.Get(this.reactor.vrRigs[i]).IncrementCoresCollectedGroup(coreValue);
						}
						grplayer.IncrementCoresCollectedPlayer(coreValue);
						int energy2 = energy - energyDepositPerUse;
						component.tool.SetEnergy(energy2);
						this.reactor.RefreshScoreboards();
						ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.Core);
						component.PlayChargeEffect(this.reactor.currencyDepositor);
					}
				}
			}
		}
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x000F6B8C File Offset: 0x000F4D8C
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", RpcTarget.All, new object[]
		{
			type,
			this.gameEntityManager.GetNetIdFromEntityId(hitByEntityId),
			hitPosition,
			Vector3.zero
		});
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x000F6BE4 File Offset: 0x000F4DE4
	public void RequestEnemyHitPlayer(GhostReactor.EnemyType type, GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition, Vector3 hitImpulse)
	{
		this.photonView.RPC("ApplyEnemyHitPlayerRPC", RpcTarget.All, new object[]
		{
			type,
			this.gameEntityManager.GetNetIdFromEntityId(hitByEntityId),
			hitPosition,
			hitImpulse
		});
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x000F6C3C File Offset: 0x000F4E3C
	[PunRPC]
	private void ApplyEnemyHitPlayerRPC(GhostReactor.EnemyType type, int entityNetId, Vector3 hitPosition, Vector3 hitImpulse, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidNetId(entityNetId))
		{
			return;
		}
		float num = 10000f;
		if (hitPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitImpulse.IsValid(num2))
			{
				if (hitImpulse.magnitude > 50f)
				{
					return;
				}
				GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
				GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
				if (grplayer == null || !grplayer.applyEnemyHitLimiter.CheckCallTime(Time.unscaledTime))
				{
					return;
				}
				this.OnEnemyHitPlayerInternal(type, entityIdFromNetId, grplayer, hitPosition, hitImpulse);
				return;
			}
		}
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x000F6CD4 File Offset: 0x000F4ED4
	private void OnEnemyHitPlayerInternal(GhostReactor.EnemyType type, GameEntityId entityId, GRPlayer player, Vector3 hitPosition, Vector3 hitImpulse)
	{
		if (type == GhostReactor.EnemyType.Chaser || type == GhostReactor.EnemyType.Phantom || type == GhostReactor.EnemyType.Ranged || type == GhostReactor.EnemyType.CustomMapsEnemy)
		{
			player.OnPlayerHit(hitPosition, hitImpulse, this, entityId);
			GameHitter component = this.gameEntityManager.GetGameEntity(entityId).GetComponent<GameHitter>();
			if (component != null)
			{
				component.ApplyHitToPlayer(player, hitPosition);
			}
		}
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x000F6D20 File Offset: 0x000F4F20
	public void ReportLocalPlayerHit()
	{
		base.GetView.RPC("ReportLocalPlayerHitRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x000F6D38 File Offset: 0x000F4F38
	[PunRPC]
	private void ReportLocalPlayerHitRPC(PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.reportLocalHitLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, this);
	}

	// Token: 0x06002D57 RID: 11607 RVA: 0x000F6D7C File Offset: 0x000F4F7C
	public void RequestPlayerRevive(GRReviveStation reviveStation, GRPlayer player)
	{
		if ((NetworkSystem.Instance.InRoom && this.IsAuthority()) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("ApplyPlayerRevivedRPC", RpcTarget.All, new object[]
			{
				reviveStation.Index,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber
			});
		}
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x000F6DEC File Offset: 0x000F4FEC
	[PunRPC]
	private void ApplyPlayerRevivedRPC(int reviveStationIndex, int playerActorNumber, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyPlayerRevived))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (reviveStationIndex < 0 || reviveStationIndex >= this.reactor.reviveStations.Count)
		{
			return;
		}
		GRReviveStation grreviveStation = this.reactor.reviveStations[reviveStationIndex];
		if (grreviveStation == null)
		{
			return;
		}
		grreviveStation.RevivePlayer(grplayer);
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x000F6E64 File Offset: 0x000F5064
	public void RequestPlayerStateChange(GRPlayer player, GRPlayer.GRPlayerState newState)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("PlayerStateChangeRPC", RpcTarget.All, new object[]
			{
				PhotonNetwork.LocalPlayer.ActorNumber,
				player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
				(int)newState
			});
			return;
		}
		player.ChangePlayerState(newState, this);
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x000F6ED8 File Offset: 0x000F50D8
	[PunRPC]
	private void PlayerStateChangeRPC(int playerResponsibleNumber, int playerActorNumber, int newState, PhotonMessageInfo info)
	{
		bool flag = this.IsValidClientRPC(info.Sender);
		bool flag2 = newState == 1 && info.Sender.ActorNumber == playerActorNumber;
		bool flag3 = newState == 0 && flag;
		if (!flag2 && !flag3)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || grplayer2.IsNull() || !grplayer2.playerStateChangeLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (newState == 0 && playerResponsibleNumber != playerActorNumber)
		{
			GRPlayer grplayer3 = GRPlayer.Get(playerResponsibleNumber);
			if (grplayer3 != null && grplayer3 != grplayer)
			{
				grplayer3.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Assists, 1f);
			}
		}
		grplayer.ChangePlayerState((GRPlayer.GRPlayerState)newState, this);
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x000F6F90 File Offset: 0x000F5190
	public void RequestGrantPlayerShield(GRPlayer player, int shieldHp, int shieldFlags)
	{
		base.GetView.RPC("RequestGrantPlayerShieldRPC", this.GetAuthorityPlayer(), new object[]
		{
			PhotonNetwork.LocalPlayer.ActorNumber,
			player.gamePlayer.rig.OwningNetPlayer.ActorNumber,
			shieldHp,
			shieldFlags
		});
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000F6FFC File Offset: 0x000F51FC
	[PunRPC]
	private void RequestGrantPlayerShieldRPC(int shieldingPlayer, int playerToGrantShieldActorNumber, int shieldHp, int shieldFlags, PhotonMessageInfo info)
	{
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (!this.IsValidAuthorityRPC(info.Sender) || grplayer.IsNull() || !grplayer.fireShieldLimiter.CheckCallTime(Time.unscaledTime) || grplayer2.IsNull() || !grplayer2.CanActivateShield(shieldHp))
		{
			return;
		}
		base.GetView.RPC("ApplyGrantPlayerShieldRPC", RpcTarget.All, new object[]
		{
			shieldingPlayer,
			playerToGrantShieldActorNumber,
			shieldHp,
			shieldFlags
		});
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000F709C File Offset: 0x000F529C
	[PunRPC]
	private void ApplyGrantPlayerShieldRPC(int shieldingPlayer, int playerToGrantShieldActorNumber, int shieldHp, int shieldFlags, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.GrantPlayerShield))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(playerToGrantShieldActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (grplayer.TryActivateShield(shieldHp, shieldFlags))
		{
			GRPlayer grplayer2 = GRPlayer.Get(shieldingPlayer);
			if (grplayer2 != null)
			{
				grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Assists, 1f);
			}
		}
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x000F7100 File Offset: 0x000F5300
	public void RequestFireProjectile(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if ((NetworkSystem.Instance.InRoom && base.IsMine) || !NetworkSystem.Instance.InRoom)
		{
			base.GetView.RPC("RequestFireProjectileRPC", RpcTarget.All, new object[]
			{
				this.gameEntityManager.GetNetIdFromEntityId(entityId),
				firingPosition,
				targetPosition,
				networkTime
			});
		}
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x000F7180 File Offset: 0x000F5380
	[PunRPC]
	private void RequestFireProjectileRPC(int entityNetId, Vector3 firingPosition, Vector3 targetPosition, double networkTime, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId, targetPosition) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.RequestFireProjectile) || !this.gameEntityManager.IsEntityNearPosition(entityNetId, firingPosition, 16f))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(entityNetId);
		this.OnRequestFireProjectileInternal(entityIdFromNetId, firingPosition, targetPosition, networkTime);
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x000F71DC File Offset: 0x000F53DC
	private void OnRequestFireProjectileInternal(GameEntityId entityId, Vector3 firingPosition, Vector3 targetPosition, double networkTime)
	{
		GREnemyRanged gameComponent = this.gameEntityManager.GetGameComponent<GREnemyRanged>(entityId);
		if (gameComponent != null)
		{
			gameComponent.RequestRangedAttack(firingPosition, targetPosition, networkTime);
		}
		GRHazardTower gameComponent2 = this.gameEntityManager.GetGameComponent<GRHazardTower>(entityId);
		if (gameComponent2 != null)
		{
			gameComponent2.OnFire(firingPosition, targetPosition, networkTime);
		}
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x000F722C File Offset: 0x000F542C
	[PunRPC]
	public void BroadcastHandprint(Vector3 pos, Quaternion orient, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		float num = 10000f;
		if (!pos.IsValid(num) || !orient.IsValid())
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender);
		if (grplayer == null)
		{
			return;
		}
		if (!GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, pos, false, true, 3f))
		{
			return;
		}
		if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber && Time.time - this.LastHandprintTime <= 0.25f)
		{
			return;
		}
		this.LastHandprintTime = Time.time;
		this.reactor.AddHandprint(pos, orient);
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x000F72D7 File Offset: 0x000F54D7
	public void OnAbilityDie(GameEntity entity, float forcedRespawn = -1f)
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.OnAbilityDie(entity, forcedRespawn);
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x000F72F8 File Offset: 0x000F54F8
	public void RequestShiftStartAuthority(bool isFirstShift)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			double time = PhotonNetwork.Time;
			SRand srand = new SRand(Mathf.FloorToInt(Time.time * 100f));
			int num = srand.NextInt(0, int.MaxValue);
			string text = Guid.NewGuid().ToString();
			this.photonView.RPC("ApplyShiftStartRPC", RpcTarget.All, new object[]
			{
				time,
				num,
				text,
				isFirstShift
			});
			shiftManager.RequestState(GhostReactorShiftManager.State.ShiftActive);
			ProgressionManager.Instance.StartOfShift(text, shiftManager.shiftRewardCoresForMothership, this.reactor.vrRigs.Count, this.reactor.GetDepthLevel());
		}
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x000F73E0 File Offset: 0x000F55E0
	[PunRPC]
	public void ApplyShiftStartRPC(double shiftStartTime, int randomSeed, string gameIdGuid, bool isFirstShift, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftStartTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		int index = Math.Clamp(this.reactor.NumActivePlayers, 0, this.reactor.difficultyScalingPerPlayer.Count - 1);
		this.reactor.difficultyScalingForCurrentFloor = 1f;
		if (this.reactor.difficultyScalingPerPlayer.Count > 0)
		{
			this.reactor.difficultyScalingForCurrentFloor = this.reactor.difficultyScalingPerPlayer[index];
		}
		double num = PhotonNetwork.Time - shiftStartTime;
		if (num < 0.0 || num > 10.0)
		{
			return;
		}
		levelGenerator.Generate(randomSeed);
		if (this.gameEntityManager.IsAuthority())
		{
			if (this.activeSpawnSectionEntitiesCoroutine != null)
			{
				base.StopCoroutine(this.activeSpawnSectionEntitiesCoroutine);
			}
			this.activeSpawnSectionEntitiesCoroutine = base.StartCoroutine(this.SpawnSectionEntitiesCoroutine(this.reactor.difficultyScalingForCurrentFloor));
		}
		shiftManager.shiftStats.ResetShiftStats();
		shiftManager.ResetJudgment();
		shiftManager.RefreshShiftStatsDisplay();
		shiftManager.OnShiftStarted(gameIdGuid, shiftStartTime, true, isFirstShift);
		this.reactor.ClearAllHandprints();
		this.reactor.ClearAllRespawns();
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x000F753B File Offset: 0x000F573B
	private IEnumerator SpawnSectionEntitiesCoroutine(float respawnCount)
	{
		int initialFrameCount = Time.frameCount;
		while (initialFrameCount == Time.frameCount)
		{
			yield return this.spawnSectionEntitiesWait;
		}
		if (this.gameEntityManager.IsAuthority())
		{
			this.reactor.levelGenerator.SpawnEntitiesInEachSection(respawnCount);
		}
		yield break;
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x000F7554 File Offset: 0x000F5754
	[PunRPC]
	public void RequestShiftEnd()
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (shiftManager == null || !shiftManager.ShiftActive)
		{
			return;
		}
		GhostReactorManager.tempEntitiesToDestroy.Clear();
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			GameEntity gameEntity = gameEntities[i];
			if (gameEntity != null && !this.ShouldEntitySurviveShift(gameEntity))
			{
				GhostReactorManager.tempEntitiesToDestroy.Add(gameEntity.id);
			}
		}
		this.gameEntityManager.RequestDestroyItems(GhostReactorManager.tempEntitiesToDestroy);
		this.photonView.RPC("ApplyShiftEndRPC", RpcTarget.Others, new object[]
		{
			PhotonNetwork.Time
		});
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(PhotonNetwork.Time, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
		shiftManager.RequestState(GhostReactorShiftManager.State.PostShift);
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x000F766A File Offset: 0x000F586A
	public void SendRequestShiftEndRPC()
	{
		this.photonView.RPC("RequestShiftEnd", this.gameEntityManager.GetAuthorityPlayer(), Array.Empty<object>());
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x000F768C File Offset: 0x000F588C
	[PunRPC]
	public void ApplyShiftEndRPC(double networkedTime, PhotonMessageInfo info)
	{
		if (!double.IsFinite(networkedTime) || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftEnd))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		if (!shiftManager.ShiftActive)
		{
			return;
		}
		this.reactor.ClearAllRespawns();
		levelGenerator.ClearLevelSections();
		shiftManager.OnShiftEnded(networkedTime, true, ZoneClearReason.JoinZone);
		shiftManager.CalculateShiftTotal();
		shiftManager.RevealJudgment(Mathf.FloorToInt((float)shiftManager.shiftStats.GetShiftStat(GRShiftStatType.EnemyDeaths) / 5f));
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x000F772C File Offset: 0x000F592C
	private bool ShouldEntitySurviveShift(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return true;
		}
		if (this.reactor == null)
		{
			return false;
		}
		if (this.IsEnemy(gameEntity))
		{
			return false;
		}
		if (gameEntity.GetComponent<GRBreakable>() != null || gameEntity.GetComponent<GRCollectibleDispenser>() != null || gameEntity.GetComponent<GRMetalEnergyGate>() != null || gameEntity.GetComponent<GRBarrierSpectral>() != null || gameEntity.GetComponent<GRSconce>() != null)
		{
			return false;
		}
		Collider safeZoneLimit = this.reactor.safeZoneLimit;
		Vector3 position = gameEntity.gameObject.transform.position;
		return safeZoneLimit.bounds.Contains(position) || gameEntity.GetComponent<GRBadge>() != null;
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x000F77EC File Offset: 0x000F59EC
	private bool IsEnemy(GameEntity gameEntity)
	{
		return gameEntity.GetComponent<GREnemyChaser>() != null || gameEntity.GetComponent<GREnemyRanged>() != null || gameEntity.GetComponent<GREnemyPhantom>() != null || gameEntity.GetComponent<GREnemyPest>() != null || gameEntity.GetComponent<GREnemySummoner>() != null || gameEntity.GetComponent<GREnemyMonkeye>() != null || gameEntity.GetComponent<GREnemyBossMoon>() != null;
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x000F785C File Offset: 0x000F5A5C
	public void InstantDeathForCurrentEnemies()
	{
		int num = 0;
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			if (!(gameEntities[i] == null))
			{
				GameEntity gameEntity = gameEntities[i];
				if (!(gameEntity.GetComponent<GREnemyBossMoon>() != null))
				{
					GREnemyChaser component = gameEntity.GetComponent<GREnemyChaser>();
					if (component != null)
					{
						component.InstantDeath();
						num++;
					}
					else
					{
						GREnemyRanged component2 = gameEntity.GetComponent<GREnemyRanged>();
						if (component2 != null)
						{
							component2.InstantDeath();
							num++;
						}
						else
						{
							GREnemyPest component3 = gameEntity.GetComponent<GREnemyPest>();
							if (component3 != null)
							{
								component3.InstantDeath();
								num++;
							}
							else
							{
								GREnemySummoner component4 = gameEntity.GetComponent<GREnemySummoner>();
								if (component4 != null)
								{
									component4.InstantDeath();
									num++;
								}
								else
								{
									GREnemyMonkeye component5 = gameEntity.GetComponent<GREnemyMonkeye>();
									if (component5 != null)
									{
										component5.InstantDeath();
										num++;
									}
								}
							}
						}
					}
				}
			}
		}
		Debug.Log(string.Format("Instant death for {0} enemies.", num));
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestRestoreBossHP()
	{
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestHurtBossHP()
	{
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestKillBossEyes()
	{
	}

	// Token: 0x06002D6F RID: 11631 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestKillBossSummoned()
	{
	}

	// Token: 0x06002D70 RID: 11632 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestGoBackBossPhase()
	{
	}

	// Token: 0x06002D71 RID: 11633 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestAdvanceBossPhase()
	{
	}

	// Token: 0x06002D72 RID: 11634 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RequestBossBehavior(GREnemyBossMoon.Behavior bossBehavior)
	{
	}

	// Token: 0x06002D73 RID: 11635 RVA: 0x000F7968 File Offset: 0x000F5B68
	public GameEntity GetBossEntity()
	{
		if (this.cachedBossEntity != null && this.cachedBossEntity.IsNotNull())
		{
			return this.cachedBossEntity;
		}
		if (this.gameEntityManager == null)
		{
			return null;
		}
		GameEntity result = null;
		List<GameEntity> gameEntities = this.gameEntityManager.GetGameEntities();
		for (int i = 0; i < gameEntities.Count; i++)
		{
			if (!(gameEntities[i] == null) && !(gameEntities[i].GetComponent<GREnemyBossMoon>() == null))
			{
				result = gameEntities[i];
				break;
			}
		}
		this.cachedBossEntity = result;
		return result;
	}

	// Token: 0x06002D74 RID: 11636 RVA: 0x000F79FB File Offset: 0x000F5BFB
	public void ClearCachedBossEntity()
	{
		this.cachedBossEntity = null;
	}

	// Token: 0x06002D75 RID: 11637 RVA: 0x000F7A04 File Offset: 0x000F5C04
	public void ReportEnemyDeath()
	{
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.EnemyDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRKillEnemy", 1);
	}

	// Token: 0x06002D76 RID: 11638 RVA: 0x000F7A3C File Offset: 0x000F5C3C
	public void ReportCoreCollection(GRPlayer player, ProgressionManager.CoreType type)
	{
		Debug.Log("GhostReactorManager ReportCoreCollection");
		if (player == null)
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		if (type == ProgressionManager.CoreType.ChaosSeed)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.SentientCoresCollected);
		}
		else if (type == ProgressionManager.CoreType.SuperCore)
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.CoresDeposited, 3f);
			int count = this.reactor.vrRigs.Count;
			for (int i = 0; i < count; i++)
			{
				GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
				if (grplayer != null)
				{
					grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, 15f);
				}
			}
		}
		else
		{
			shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.CoresCollected);
			player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.CoresDeposited, 1f);
			int count2 = this.reactor.vrRigs.Count;
			for (int j = 0; j < count2; j++)
			{
				GRPlayer grplayer2 = GRPlayer.Get(this.reactor.vrRigs[j]);
				if (grplayer2 != null)
				{
					grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, 5f);
				}
			}
		}
		shiftManager.RefreshShiftStatsDisplay();
		PlayerGameEvents.MiscEvent("GRCollectCore", 1);
	}

	// Token: 0x06002D77 RID: 11639 RVA: 0x000F7B88 File Offset: 0x000F5D88
	public void ReportPlayerDeath(GRPlayer player)
	{
		if (this.reactor == null || player == null || this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		shiftManager.shiftStats.IncrementShiftStat(GRShiftStatType.PlayerDeaths);
		shiftManager.RefreshShiftStatsDisplay();
		player.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Deaths, 1f);
	}

	// Token: 0x06002D78 RID: 11640 RVA: 0x000F7BE4 File Offset: 0x000F5DE4
	public void PromotionBotActivePlayerRequest(int state)
	{
		this.photonView.RPC("PromotionBotActivePlayerRequestRPC", this.GetAuthorityPlayer(), new object[]
		{
			state
		});
	}

	// Token: 0x06002D79 RID: 11641 RVA: 0x000F7C0C File Offset: 0x000F5E0C
	[PunRPC]
	public void PromotionBotActivePlayerRequestRPC(int state, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.promotionBotLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (promotionBot == null)
		{
			return;
		}
		if (state == 6)
		{
			if (promotionBot.currentPlayerActorNumber != -1)
			{
				return;
			}
			state = 1;
		}
		int actorNumber = info.Sender.ActorNumber;
		this.photonView.RPC("PromotionBotActivePlayerResponseRPC", RpcTarget.Others, new object[]
		{
			actorNumber,
			state
		});
		promotionBot.SetActivePlayerStateChange(actorNumber, state);
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x000F7CC8 File Offset: 0x000F5EC8
	[PunRPC]
	public void PromotionBotActivePlayerResponseRPC(int actorNumber, int state, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		if (GRPlayer.Get(info.Sender.ActorNumber) == null || promotionBot == null || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.PromotionBotResponse))
		{
			return;
		}
		promotionBot.SetActivePlayerStateChange(actorNumber, state);
	}

	// Token: 0x06002D7B RID: 11643 RVA: 0x000F7D38 File Offset: 0x000F5F38
	[PunRPC]
	public void BroadcastScoreboardPage(int scoreboardPage, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.scoreboardPageLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (GRUIScoreboard.ValidPage((GRUIScoreboard.ScoreboardScreen)scoreboardPage))
		{
			GhostReactor.instance.UpdateScoreboardScreen((GRUIScoreboard.ScoreboardScreen)scoreboardPage);
		}
	}

	// Token: 0x06002D7C RID: 11644 RVA: 0x000F7D94 File Offset: 0x000F5F94
	[PunRPC]
	public void BroadcastStartingProgression(int points, int redeemedPoints, double shiftJoinedTime, PhotonMessageInfo info)
	{
		if (double.IsNaN(shiftJoinedTime) || double.IsInfinity(shiftJoinedTime))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null || !grplayer.progressionBroadcastLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		grplayer.SetProgressionData(points, redeemedPoints, false);
		grplayer.shiftJoinTime = Math.Clamp(shiftJoinedTime, PhotonNetwork.Time - 10.0, PhotonNetwork.Time);
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x000F7E18 File Offset: 0x000F6018
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			0,
			0
		});
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x000F7E51 File Offset: 0x000F6051
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction, int param0)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			param0,
			0
		});
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x000F7E8A File Offset: 0x000F608A
	public void RequestPlayerAction(GhostReactorManager.GRPlayerAction playerAction, int param0, int param1)
	{
		this.photonView.RPC("RequestPlayerActionRPC", this.GetAuthorityPlayer(), new object[]
		{
			(int)playerAction,
			param0,
			param1
		});
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x000F7EC4 File Offset: 0x000F60C4
	public bool VerifyShuttleInteractability(GRPlayer player, int shuttleIdx, bool ignoreOwnership = false)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		GRShuttle shuttleById = GRElevatorManager._instance.GetShuttleById(shuttleIdx);
		return !(shuttleById == null) && shuttleById.IsShuttleInteractableByPlayer(player, ignoreOwnership);
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x000F7F00 File Offset: 0x000F6100
	[PunRPC]
	public void RequestPlayerActionRPC(int playerAction, int param0, int param1, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		bool flag = false;
		switch (playerAction)
		{
		case 1:
			flag = (!shiftManager.ShiftActive && shiftManager.authorizedToDelveDeeper);
			if (flag)
			{
				int num = this.reactor.GetDepthLevel() + 1;
				this.reactor.depthConfigIndex = this.reactor.PickLevelConfigForDepth(num);
				param0 = num;
				param1 = this.reactor.depthConfigIndex;
			}
			break;
		case 2:
			flag = true;
			break;
		case 3:
			flag = this.VerifyShuttleInteractability(grplayer, param0, true);
			param1 = info.Sender.ActorNumber;
			break;
		case 4:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 5:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 6:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 7:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 8:
			flag = this.VerifyShuttleInteractability(grplayer, param0, false);
			param1 = info.Sender.ActorNumber;
			break;
		case 9:
			flag = true;
			param0 = Mathf.Clamp(param0, 0, 1);
			param1 = info.Sender.ActorNumber;
			break;
		case 10:
			flag = true;
			param0 = Mathf.Clamp(param0, 0, 3);
			param1 = info.Sender.ActorNumber;
			break;
		case 11:
			flag = (param0 == info.Sender.ActorNumber || this.IsAuthorityPlayer(info.Sender));
			if (this.reactor.seedExtractor.StationOpen && this.reactor.seedExtractor.CurrentPlayerActorNumber != info.Sender.ActorNumber)
			{
				playerAction = 13;
			}
			break;
		case 12:
			flag = this.IsAuthorityPlayer(info.Sender);
			break;
		case 13:
			flag = this.IsAuthorityPlayer(info.Sender);
			break;
		case 14:
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(param1);
			if (this.IsAuthorityPlayer(info.Sender) && gameEntityFromNetId != null && gameEntityFromNetId.lastHeldByActorNumber == param0)
			{
				flag = true;
			}
			break;
		}
		case 15:
		{
			int netId = param1;
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (gameEntityFromNetId2 != null && this.reactor.seedExtractor.ValidateSeedDepositSucceeded(param0, param1))
			{
				this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId2.id);
				flag = true;
			}
			break;
		}
		case 16:
			flag = (info.Sender.ActorNumber == param0);
			break;
		}
		if (flag)
		{
			this.photonView.RPC("ApplyPlayerActionRPC", RpcTarget.All, new object[]
			{
				playerAction,
				param0,
				param1
			});
		}
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x000F8250 File Offset: 0x000F6450
	[PunRPC]
	public void ApplyPlayerActionRPC(int playerAction, int param0, int param1, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyShiftStart))
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		this.gameEntityManager.IsAuthorityPlayer(info.Sender);
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestShiftStartLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		switch (playerAction)
		{
		case 1:
			this.reactor.SetNextDelveDepth(param0, param1);
			return;
		case 2:
			this.reactor.shiftManager.SetState((GhostReactorShiftManager.State)param0, false);
			return;
		case 3:
		{
			GRPlayer grplayer2 = GRPlayer.Get(param1);
			if (grplayer2 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer2, param0, true))
			{
				return;
			}
			GRShuttle shuttleById = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById != null)
			{
				shuttleById.OnOpenDoor();
				return;
			}
			break;
		}
		case 4:
		{
			GRPlayer grplayer3 = GRPlayer.Get(param1);
			if (grplayer3 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer3, param0, false))
			{
				return;
			}
			GRShuttle shuttleById2 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById2 != null)
			{
				shuttleById2.OnCloseDoor();
				return;
			}
			break;
		}
		case 5:
		{
			GRPlayer grplayer4 = GRPlayer.Get(param1);
			if (grplayer4 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer4, param0, false))
			{
				return;
			}
			GRShuttle shuttleById3 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById3 != null)
			{
				shuttleById3.OnLaunch();
				return;
			}
			break;
		}
		case 6:
		{
			GRPlayer grplayer5 = GRPlayer.Get(param1);
			if (grplayer5 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer5, param0, false))
			{
				return;
			}
			GRShuttle shuttleById4 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById4 != null)
			{
				shuttleById4.OnArrive();
				return;
			}
			break;
		}
		case 7:
		{
			GRPlayer grplayer6 = GRPlayer.Get(param1);
			if (grplayer6 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer6, param0, false))
			{
				return;
			}
			GRShuttle shuttleById5 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById5 != null)
			{
				shuttleById5.OnTargetLevelUp();
				return;
			}
			break;
		}
		case 8:
		{
			GRPlayer grplayer7 = GRPlayer.Get(param1);
			if (grplayer7 == null)
			{
				return;
			}
			if (!this.VerifyShuttleInteractability(grplayer7, param0, false))
			{
				return;
			}
			GRShuttle shuttleById6 = GRElevatorManager._instance.GetShuttleById(param0);
			if (shuttleById6 != null)
			{
				shuttleById6.OnTargetLevelDown();
				return;
			}
			break;
		}
		case 9:
		{
			GRPlayer grplayer8 = GRPlayer.Get(param1);
			if (grplayer8 != null)
			{
				param0 = Mathf.Clamp(param0, 0, 1);
				grplayer8.dropPodLevel = param0;
				this.reactor.RefreshBays();
				grplayer8.RefreshShuttles();
				return;
			}
			break;
		}
		case 10:
		{
			GRPlayer grplayer9 = GRPlayer.Get(param1);
			if (grplayer9 != null)
			{
				param0 = Mathf.Clamp(param0, 0, 3);
				grplayer9.dropPodChasisLevel = param0;
				this.reactor.RefreshBays();
				grplayer9.RefreshShuttles();
				return;
			}
			break;
		}
		case 11:
			this.reactor.seedExtractor.CardSwipeSuccess();
			this.reactor.seedExtractor.OpenStation(param0);
			return;
		case 12:
			this.reactor.seedExtractor.CloseStation();
			return;
		case 13:
			this.reactor.seedExtractor.CardSwipeFail();
			return;
		case 14:
			this.reactor.seedExtractor.TryDepositSeed(param0, param1);
			return;
		case 15:
			this.reactor.seedExtractor.SeedDepositSucceeded(param0, param1);
			return;
		case 16:
			this.reactor.seedExtractor.SeedDepositFailed(param0, param1);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x000F85C4 File Offset: 0x000F67C4
	public GRToolUpgradePurchaseStationFull GetToolUpgradeStationFullForIndex(int idx)
	{
		if (this.reactor == null || this.reactor.toolUpgradePurchaseStationsFull == null || idx < 0 || idx >= this.reactor.toolUpgradePurchaseStationsFull.Count)
		{
			return null;
		}
		return this.reactor.toolUpgradePurchaseStationsFull[idx];
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x000F8616 File Offset: 0x000F6816
	public int GetIndexForToolUpgradeStationFull(GRToolUpgradePurchaseStationFull station)
	{
		if (this.reactor == null || this.reactor.toolUpgradePurchaseStationsFull == null)
		{
			return -1;
		}
		return this.reactor.toolUpgradePurchaseStationsFull.IndexOf(station);
	}

	// Token: 0x06002D85 RID: 11653 RVA: 0x000F8648 File Offset: 0x000F6848
	public void RequestNetworkShelfAndItemChange(GRToolUpgradePurchaseStationFull station, int shelf, int item)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SelectShelfAndItem,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			shelf,
			item
		});
	}

	// Token: 0x06002D86 RID: 11654 RVA: 0x000F86B0 File Offset: 0x000F68B0
	private void SelectToolShelfAndItemRPCRouted(int stationIndex, int shelf, int item, PhotonMessageInfo info)
	{
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber == info.Sender.ActorNumber)
		{
			toolUpgradeStationFullForIndex.SetSelectedShelfAndItem(shelf, item, true);
		}
	}

	// Token: 0x06002D87 RID: 11655 RVA: 0x000F86EC File Offset: 0x000F68EC
	public void RequestPurchaseToolOrUpgrade(GRToolUpgradePurchaseStationFull station, int shelf, int item)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", this.GetAuthorityPlayer(), new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.RequestPurchaseAuthority,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			shelf,
			item
		});
	}

	// Token: 0x06002D88 RID: 11656 RVA: 0x000F8758 File Offset: 0x000F6958
	public void RequestPurchaseRPCRoutedAuthority(int stationIndex, int shelf, int item, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull())
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber != info.Sender.ActorNumber)
		{
			return;
		}
		ValueTuple<bool, bool> valueTuple = toolUpgradeStationFullForIndex.TryPurchaseAuthority(grplayer, shelf, item);
		bool item2 = valueTuple.Item1;
		if (!valueTuple.Item2)
		{
			return;
		}
		if (item2)
		{
			this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
			{
				GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseSuccess,
				info.Sender.ActorNumber,
				stationIndex,
				shelf,
				item
			});
		}
		else
		{
			this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
			{
				GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseFail,
				info.Sender.ActorNumber,
				stationIndex,
				shelf,
				item
			});
		}
		toolUpgradeStationFullForIndex.ToolPurchaseResponseLocal(grplayer, shelf, item, item2);
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x000F887C File Offset: 0x000F6A7C
	public void NotifyPurchaseToolOrUpgradeRPCRouted(int actorNumber, int stationIndex, int shelf, int item, bool success, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		if (grplayer != null)
		{
			toolUpgradeStationFullForIndex.ToolPurchaseResponseLocal(grplayer, shelf, item, success);
		}
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x000F88C8 File Offset: 0x000F6AC8
	public void RequestStationExclusivity(GRToolUpgradePurchaseStationFull station)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", this.GetAuthorityPlayer(), new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.RequestStationExclusivityAuthority,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			0,
			0
		});
	}

	// Token: 0x06002D8B RID: 11659 RVA: 0x000F8934 File Offset: 0x000F6B34
	public void SetActivePlayerAuthority(GRToolUpgradePurchaseStationFull station, int actorNumber)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		station.SetActivePlayer(actorNumber);
		this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SetToolStationActivePlayer,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			station.currentActivePlayerActorNumber,
			0
		});
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x000F89B0 File Offset: 0x000F6BB0
	public void RequestStationExclusivityRPCRoutedAuthority(int stationIndex, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (toolUpgradeStationFullForIndex.currentActivePlayerActorNumber != -1)
		{
			return;
		}
		this.SetActivePlayerAuthority(toolUpgradeStationFullForIndex, info.Sender.ActorNumber);
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x000F89FC File Offset: 0x000F6BFC
	public void SetToolStationActivePlayerRPCRouted(int stationIndex, int activeOwner, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		toolUpgradeStationFullForIndex.SetActivePlayer(activeOwner);
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x000F8A34 File Offset: 0x000F6C34
	public void BroadcastHandleAndSelectionWheelPosition(GRToolUpgradePurchaseStationFull station, int handlePos, int wheelPos)
	{
		int indexForToolUpgradeStationFull = this.GetIndexForToolUpgradeStationFull(station);
		if (indexForToolUpgradeStationFull == -1)
		{
			return;
		}
		if (NetworkSystem.Instance.LocalPlayer.ActorNumber != station.currentActivePlayerActorNumber)
		{
			return;
		}
		this.photonView.RPC("ToolPurchaseV2_RPC", RpcTarget.Others, new object[]
		{
			GhostReactorManager.ToolPurchaseActionV2.SetHandleAndSelectionWheelPosition,
			PhotonNetwork.LocalPlayer.ActorNumber,
			indexForToolUpgradeStationFull,
			handlePos,
			wheelPos
		});
	}

	// Token: 0x06002D8F RID: 11663 RVA: 0x000F8AB4 File Offset: 0x000F6CB4
	public void SetHandleAndSelectionWheelPositionRPCRouted(int stationIndex, int handlePos, int wheelPos, PhotonMessageInfo info)
	{
		GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = this.GetToolUpgradeStationFullForIndex(stationIndex);
		if (toolUpgradeStationFullForIndex == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != toolUpgradeStationFullForIndex.currentActivePlayerActorNumber)
		{
			return;
		}
		toolUpgradeStationFullForIndex.SetHandleAndSelectionWheelPositionRemote(handlePos, wheelPos);
	}

	// Token: 0x06002D90 RID: 11664 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void RequestHackToolStation()
	{
	}

	// Token: 0x06002D91 RID: 11665 RVA: 0x000F8AF0 File Offset: 0x000F6CF0
	[PunRPC]
	public void ToolPurchaseV2_RPC(GhostReactorManager.ToolPurchaseActionV2 command, int initiatorID, int stationIndex, int param1, int param2, PhotonMessageInfo info)
	{
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolUpgradeStationAction))
		{
			return;
		}
		switch (command)
		{
		case GhostReactorManager.ToolPurchaseActionV2.RequestPurchaseAuthority:
			this.RequestPurchaseRPCRoutedAuthority(stationIndex, param1, param2, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SelectShelfAndItem:
			this.SelectToolShelfAndItemRPCRouted(stationIndex, param1, param2, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseFail:
			this.NotifyPurchaseToolOrUpgradeRPCRouted(initiatorID, stationIndex, param1, param2, false, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.NotifyPurchaseSuccess:
			this.NotifyPurchaseToolOrUpgradeRPCRouted(initiatorID, stationIndex, param1, param2, true, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.RequestStationExclusivityAuthority:
			this.RequestStationExclusivityRPCRoutedAuthority(stationIndex, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SetToolStationActivePlayer:
			this.SetToolStationActivePlayerRPCRouted(stationIndex, param1, info);
			return;
		case GhostReactorManager.ToolPurchaseActionV2.SetHandleAndSelectionWheelPosition:
			this.SetHandleAndSelectionWheelPositionRPCRouted(stationIndex, param1, param2, info);
			break;
		case GhostReactorManager.ToolPurchaseActionV2.SetToolStationHackedDebug:
			break;
		default:
			return;
		}
	}

	// Token: 0x06002D92 RID: 11666 RVA: 0x000F8B93 File Offset: 0x000F6D93
	public void ToolPurchaseStationRequest(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action)
	{
		this.photonView.RPC("ToolPurchaseStationRequestRPC", this.GetAuthorityPlayer(), new object[]
		{
			stationIndex,
			action
		});
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x000F8BC4 File Offset: 0x000F6DC4
	[PunRPC]
	public void ToolPurchaseStationRequestRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationAction action, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidAuthorityRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer.IsNull() || !grplayer.requestToolPurchaseStationLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (action)
		{
		case GhostReactorManager.ToolPurchaseStationAction.ShiftLeft:
			grtoolPurchaseStation.ShiftLeftAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.ShiftRight:
			grtoolPurchaseStation.ShiftRightAuthority();
			this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
			{
				stationIndex,
				GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate,
				grtoolPurchaseStation.ActiveEntryIndex,
				0
			});
			this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate, grtoolPurchaseStation.ActiveEntryIndex, 0);
			return;
		case GhostReactorManager.ToolPurchaseStationAction.TryPurchase:
		{
			bool flag = false;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetNetPlayerByID(info.Sender.ActorNumber), out rigContainer))
			{
				GRPlayer component = rigContainer.Rig.GetComponent<GRPlayer>();
				int num;
				if (component != null && grtoolPurchaseStation.TryPurchaseAuthority(component, out num))
				{
					this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
					{
						stationIndex,
						GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded,
						info.Sender.ActorNumber,
						num
					});
					this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded, info.Sender.ActorNumber, num);
					flag = true;
				}
			}
			if (!flag)
			{
				this.photonView.RPC("ToolPurchaseStationResponseRPC", RpcTarget.Others, new object[]
				{
					stationIndex,
					GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed,
					info.Sender.ActorNumber,
					0
				});
				this.ToolPurchaseResponseLocal(stationIndex, GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed, info.Sender.ActorNumber, 0);
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x000F8E0C File Offset: 0x000F700C
	[PunRPC]
	public void ToolPurchaseStationResponseRPC(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (!this.IsValidClientRPC(info.Sender) || stationIndex < 0 || stationIndex >= toolPurchasingStations.Count || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolPurchaseResponse))
		{
			return;
		}
		this.ToolPurchaseResponseLocal(stationIndex, responseType, dataA, dataB);
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x000F8E6C File Offset: 0x000F706C
	private void ToolPurchaseResponseLocal(int stationIndex, GhostReactorManager.ToolPurchaseStationResponse responseType, int dataA, int dataB)
	{
		if (this.reactor == null)
		{
			return;
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		if (stationIndex < 0 || stationIndex >= toolPurchasingStations.Count)
		{
			return;
		}
		GRToolPurchaseStation grtoolPurchaseStation = toolPurchasingStations[stationIndex];
		if (grtoolPurchaseStation == null)
		{
			return;
		}
		switch (responseType)
		{
		case GhostReactorManager.ToolPurchaseStationResponse.SelectionUpdate:
			grtoolPurchaseStation.OnSelectionUpdate(dataA);
			return;
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseSucceeded:
		{
			grtoolPurchaseStation.OnPurchaseSucceeded();
			GRPlayer grplayer = GRPlayer.Get(dataA);
			if (grplayer != null)
			{
				grplayer.IncrementCoresSpentPlayer(dataB);
				grplayer.AddItemPurchased(grtoolPurchaseStation.GetCurrentToolName());
				grplayer.SubtractShiftCredit(dataB);
				return;
			}
			break;
		}
		case GhostReactorManager.ToolPurchaseStationResponse.PurchaseFailed:
			grtoolPurchaseStation.OnPurchaseFailed();
			break;
		default:
			return;
		}
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x000F8F08 File Offset: 0x000F7108
	public void ToolUpgradeStationRequestUpgrade(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId)
	{
		this.photonView.RPC("ToolUpgradeStationRequestUpgradeRPC", this.GetAuthorityPlayer(), new object[]
		{
			UpgradeID,
			entityNetId
		});
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x000F8F38 File Offset: 0x000F7138
	public void ToolSnapRequestUpgrade(int upgradeNetID, GRToolProgressionManager.ToolParts UpgradeID, int entityNetId)
	{
		this.photonView.RPC("ToolSnapRequestUpgradeRPC", this.GetAuthorityPlayer(), new object[]
		{
			upgradeNetID,
			UpgradeID,
			entityNetId
		});
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x000F8F74 File Offset: 0x000F7174
	[PunRPC]
	public void ToolSnapRequestUpgradeRPC(int upgradeNetID, GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, PhotonMessageInfo info)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
		if (grplayer == null)
		{
			return;
		}
		if (this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ToolUpgradeStationAction))
		{
			return;
		}
		if (!this.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
		if (gameEntity != null)
		{
			Object component = gameEntity.GetComponent<GRTool>();
			GameEntity gameEntity2 = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(upgradeNetID));
			if (component != null && gameEntity2 != null && GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, gameEntity2.transform.position, false, true, 16f) && GameEntityManager.IsPlayerHandNearPosition(grplayer.gamePlayer, gameEntity2.transform.position, false, true, 16f))
			{
				this.photonView.RPC("UpgradeToolRemoteRPC", RpcTarget.All, new object[]
				{
					UpgradeID,
					entityNetId,
					false,
					info.Sender.ActorNumber
				});
				this.gameEntityManager.RequestDestroyItem(gameEntity2.id);
			}
		}
	}

	// Token: 0x06002D99 RID: 11673 RVA: 0x000F90BC File Offset: 0x000F72BC
	public void ToolUpgradeStationRequestUpgradeRPC(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002D9A RID: 11674 RVA: 0x000F90CC File Offset: 0x000F72CC
	[PunRPC]
	public void UpgradeToolRemoteRPC(GRToolProgressionManager.ToolParts UpgradeID, int entityNetId, bool applyCost, int playerNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (applyCost)
		{
			GRPlayer grplayer = GRPlayer.Get(info.Sender.ActorNumber);
			int shiftCreditDelta;
			if (grplayer != null && this.reactor.toolProgression.GetShiftCreditCost(UpgradeID, out shiftCreditDelta))
			{
				grplayer.SubtractShiftCredit(shiftCreditDelta);
			}
		}
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
		if (gameEntity != null)
		{
			GRTool component = gameEntity.GetComponent<GRTool>();
			if (component != null)
			{
				component.UpgradeTool(UpgradeID);
			}
		}
	}

	// Token: 0x06002D9B RID: 11675 RVA: 0x00023994 File Offset: 0x00021B94
	private bool DoesUserHaveResearchUnlocked(int UserID, string ResearchID)
	{
		return true;
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x000F915B File Offset: 0x000F735B
	public void ToolPlacedInUpgradeStation(GameEntity entity)
	{
		this.photonView.RPC("PlacedToolInUpgradeStationRPC", RpcTarget.All, new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(entity.id)
		});
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void PlacedToolInUpgradeStationRPC(int entityNetId, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x000F918D File Offset: 0x000F738D
	public void UpgradeToolAtToolStation()
	{
		this.photonView.RPC("UpgradeToolAtToolStationRPC", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void UpgradeToolAtToolStationRPC(PhotonMessageInfo info)
	{
	}

	// Token: 0x06002DA0 RID: 11680 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void LocalEjectToolInUpgradeStation()
	{
	}

	// Token: 0x06002DA1 RID: 11681 RVA: 0x000F91A8 File Offset: 0x000F73A8
	public void EntityEnteredDropZone(GameEntity entity)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.reactor == null)
		{
			return;
		}
		GRUIStationEmployeeBadges employeeBadges = this.reactor.employeeBadges;
		long num = BitPackUtils.PackWorldPosForNetwork(entity.transform.position);
		int num2 = BitPackUtils.PackQuaternionForNetwork(entity.transform.rotation);
		if (entity.gameObject.GetComponent<GRBadge>() != null)
		{
			GRUIEmployeeBadgeDispenser gruiemployeeBadgeDispenser = employeeBadges.badgeDispensers[entity.gameObject.GetComponent<GRBadge>().dispenserIndex];
			if (gruiemployeeBadgeDispenser != null)
			{
				num = BitPackUtils.PackWorldPosForNetwork(gruiemployeeBadgeDispenser.GetSpawnPosition());
				num2 = BitPackUtils.PackQuaternionForNetwork(gruiemployeeBadgeDispenser.GetSpawnRotation());
			}
		}
		this.photonView.RPC("EntityEnteredDropZoneRPC", RpcTarget.All, new object[]
		{
			this.gameEntityManager.GetNetIdFromEntityId(entity.id),
			num,
			num2
		});
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x000F9290 File Offset: 0x000F7490
	[PunRPC]
	public void EntityEnteredDropZoneRPC(int entityNetId, long position, int rotation, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.EntityEnteredDropZone))
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "EntityEnteredDropZoneRPC");
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(position);
		float num = 10000f;
		if (!vector.IsValid(num))
		{
			return;
		}
		Quaternion rotation2 = BitPackUtils.UnpackQuaternionFromNetwork(rotation);
		if (!rotation2.IsValid())
		{
			return;
		}
		if (!this.IsPositionInZone(vector))
		{
			return;
		}
		if ((vector - this.reactor.dropZone.transform.position).magnitude > 5f)
		{
			return;
		}
		this.LocalEntityEnteredDropZone(this.gameEntityManager.GetEntityIdFromNetId(entityNetId), vector, rotation2);
	}

	// Token: 0x06002DA3 RID: 11683 RVA: 0x000F933C File Offset: 0x000F753C
	private void LocalEntityEnteredDropZone(GameEntityId entityId, Vector3 position, Quaternion rotation)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRDropZone dropZone = this.reactor.dropZone;
		Vector3 linearVelocity = dropZone.GetRepelDirectionWorld() * GhostReactor.DROP_ZONE_REPEL;
		GameEntity gameEntity = this.gameEntityManager.GetGameEntity(entityId);
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber >= 0 && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			int handIndex = gamePlayer.FindHandIndex(entityId);
			gamePlayer.ClearGrabbedIfHeld(entityId, this.gameEntityManager);
			if (gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
				GamePlayerLocal.instance.ClearGrabbed(handIndex);
			}
			gameEntity.heldByActorNumber = -1;
			gameEntity.heldByHandIndex = -1;
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		gameEntity.transform.SetParent(null);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		if (!(gameEntity.gameObject.GetComponent<GRBadge>() != null))
		{
			Rigidbody component = gameEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.position = position;
				component.rotation = rotation;
				component.linearVelocity = linearVelocity;
				component.angularVelocity = Vector3.zero;
			}
		}
		dropZone.PlayEffect();
	}

	// Token: 0x06002DA4 RID: 11684 RVA: 0x000F9470 File Offset: 0x000F7670
	public void RequestRecycleScanItem(GameEntityId gameEntityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(gameEntityId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleScanItemRPC", RpcTarget.All, new object[]
		{
			netIdFromEntityId
		});
	}

	// Token: 0x06002DA5 RID: 11685 RVA: 0x000F94B4 File Offset: 0x000F76B4
	[PunRPC]
	public void ApplyRecycleScanItemRPC(int netId, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplRecycleScanItem))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.gameEntityManager.GetEntityIdFromNetId(netId);
		this.reactor.recycler.ScanItem(entityIdFromNetId);
	}

	// Token: 0x06002DA6 RID: 11686 RVA: 0x000F9508 File Offset: 0x000F7708
	public void RequestRecycleItem(int lastHeldActorNumber, GameEntityId toolId, GRTool.GRToolType toolType)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.gameEntityManager == null)
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(toolId);
		if (netIdFromEntityId == -1)
		{
			return;
		}
		base.SendRPC("ApplyRecycleItemRPC", RpcTarget.All, new object[]
		{
			lastHeldActorNumber,
			netIdFromEntityId,
			toolType
		});
	}

	// Token: 0x06002DA7 RID: 11687 RVA: 0x000F956C File Offset: 0x000F776C
	[PunRPC]
	public void ApplyRecycleItemRPC(int lastHeldActorNumber, int toolNetId, GRTool.GRToolType toolType, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || !this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplyRecycleItem) || !this.gameEntityManager.IsEntityNearPosition(toolNetId, this.reactor.recycler.transform.position, 16f))
		{
			return;
		}
		int count = this.reactor.vrRigs.Count;
		Mathf.FloorToInt((float)this.reactor.recycler.GetRecycleValue(toolType) / (float)count);
		ProgressionManager.Instance.RecycleTool(toolType, this.reactor.vrRigs.Count);
		this.reactor.RefreshScoreboards();
		this.reactor.recycler.RecycleItem();
		this.gameEntityManager.DestroyItemLocal(this.gameEntityManager.GetEntityIdFromNetId(toolNetId));
	}

	// Token: 0x06002DA8 RID: 11688 RVA: 0x000F9644 File Offset: 0x000F7844
	public void RequestSentientCorePerformJump(GameEntity entity, Vector3 startPos, Vector3 normal, Vector3 direction, float waitTime)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.gameEntityManager.GetNetIdFromEntityId(entity.id);
		double num = PhotonNetwork.Time + (double)waitTime;
		base.SendRPC("SentientCorePerformJumpRPC", RpcTarget.All, new object[]
		{
			netIdFromEntityId,
			startPos,
			normal,
			direction,
			num
		});
	}

	// Token: 0x06002DA9 RID: 11689 RVA: 0x000F96B8 File Offset: 0x000F78B8
	[PunRPC]
	public void SentientCorePerformJumpRPC(int entityNetId, Vector3 startPosition, Vector3 surfaceNormal, Vector3 jumpDirection, double jumpStartTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, startPosition) && !this.m_RpcSpamChecks.IsSpamming(GhostReactorManager.RPC.ApplySentientCoreDestination))
		{
			float num = 10000f;
			if (startPosition.IsValid(num))
			{
				float num2 = 10000f;
				if (surfaceNormal.IsValid(num2))
				{
					float num3 = 10000f;
					if (jumpDirection.IsValid(num3) && double.IsFinite(jumpStartTime) && PhotonNetwork.Time - jumpStartTime <= 5.0 && this.gameEntityManager.IsEntityNearPosition(entityNetId, startPosition, 16f))
					{
						GameEntity gameEntity = this.gameEntityManager.GetGameEntity(this.gameEntityManager.GetEntityIdFromNetId(entityNetId));
						if (gameEntity == null)
						{
							return;
						}
						GRSentientCore component = gameEntity.GetComponent<GRSentientCore>();
						if (component == null)
						{
							return;
						}
						component.PerformJump(startPosition, surfaceNormal, jumpDirection, jumpStartTime);
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002DAA RID: 11690 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002DAB RID: 11691 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002DAC RID: 11692 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002DAD RID: 11693 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002DAE RID: 11694 RVA: 0x000F9789 File Offset: 0x000F7989
	protected void OnNewPlayerEnteredGhostReactor()
	{
		if (this.reactor == null)
		{
			return;
		}
		this.reactor.VRRigRefresh();
	}

	// Token: 0x06002DAF RID: 11695 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityZoneClear(GTZone zoneId)
	{
	}

	// Token: 0x06002DB0 RID: 11696 RVA: 0x000F97A8 File Offset: 0x000F79A8
	public void OnZoneCreate()
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		int newDepthConfigIndex = this.reactor.PickLevelConfigForDepth(grplayer.shuttleData.targetLevel);
		this.reactor.SetNextDelveDepth(grplayer.shuttleData.targetLevel, newDepthConfigIndex);
		this.reactor.DelveToNextDepth();
		if (this.reactor.shiftManager != null)
		{
			this.reactor.shiftManager.SetState(GhostReactorShiftManager.State.WaitingForConnect, true);
		}
	}

	// Token: 0x06002DB1 RID: 11697 RVA: 0x000F9840 File Offset: 0x000F7A40
	public void OnZoneInit()
	{
		if (this.reactor == null)
		{
			return;
		}
		if (this.reactor.zone == GTZone.customMaps)
		{
			return;
		}
		this.reactor.VRRigRefresh();
		if (this.reactor.employeeTerminal != null)
		{
			this.reactor.employeeTerminal.Setup();
		}
		if (GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodLevel, this.reactor.toolProgression.GetDropPodLevel());
			this.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodChassisLevel, this.reactor.toolProgression.GetDropPodChasisLevel());
		}
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x000F98E8 File Offset: 0x000F7AE8
	public void OnZoneClear(ZoneClearReason reason)
	{
		if (this.reactor == null)
		{
			return;
		}
		GRPlayer component = GamePlayerLocal.instance.gamePlayer.GetComponent<GRPlayer>();
		if (component != null)
		{
			GRBadge badge = component.badge;
			if (badge != null && badge.IsAttachedToPlayer())
			{
				component.lastLeftWithBadgeAttachedTime = Time.timeAsDouble;
			}
			component.SendGameEndedTelemetry(false, reason);
		}
		if (this.reactor.levelGenerator != null)
		{
			this.reactor.levelGenerator.ClearLevelSections();
		}
		if (this.reactor.shiftManager != null)
		{
			this.reactor.shiftManager.OnShiftEnded(0.0, false, reason);
		}
		GRPlayer grplayer = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			grplayer.SetGooParticleSystemEnabled(false, false);
			grplayer.SetGooParticleSystemEnabled(true, false);
		}
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x000F99C9 File Offset: 0x000F7BC9
	public bool IsZoneReady()
	{
		return this.reactor != null;
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ShouldClearZone()
	{
		return true;
	}

	// Token: 0x06002DB5 RID: 11701 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x06002DB6 RID: 11702 RVA: 0x000F99D8 File Offset: 0x000F7BD8
	public void SerializeZoneData(BinaryWriter writer)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		writer.Write(this.reactor.depthLevel);
		writer.Write(this.reactor.depthConfigIndex);
		writer.Write(this.reactor.difficultyScalingForCurrentFloor);
		if (shiftManager != null)
		{
			writer.Write(shiftManager.ShiftActive);
			writer.Write(shiftManager.ShiftStartNetworkTime);
			shiftManager.shiftStats.Serialize(writer);
			writer.Write(shiftManager.ShiftId);
			writer.Write(shiftManager.stateStartTime);
			writer.Write((byte)shiftManager.GetState());
			writer.Write(levelGenerator.seed);
		}
		if (promotionBot != null)
		{
			writer.Write(promotionBot.GetCurrentPlayerActorNumber());
			writer.Write((int)promotionBot.currentState);
		}
		for (int i = 0; i < array.Length; i++)
		{
			writer.Write((int)array[i].currentScreen);
		}
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		writer.Write(toolPurchasingStations.Count);
		for (int j = 0; j < toolPurchasingStations.Count; j++)
		{
			writer.Write(toolPurchasingStations[j].ActiveEntryIndex);
		}
		List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull = this.reactor.toolUpgradePurchaseStationsFull;
		writer.Write(toolUpgradePurchaseStationsFull.Count);
		for (int k = 0; k < toolUpgradePurchaseStationsFull.Count; k++)
		{
			writer.Write(toolUpgradePurchaseStationsFull[k].SelectedShelf);
			writer.Write(toolUpgradePurchaseStationsFull[k].SelectedItem);
			writer.Write(toolUpgradePurchaseStationsFull[k].currentActivePlayerActorNumber);
		}
		List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = this.reactor.respawnQueue;
		writer.Write(this.reactor.respawnQueue.Count);
		for (int l = 0; l < respawnQueue.Count; l++)
		{
			writer.Write(respawnQueue[l].entityTypeID);
			writer.Write(respawnQueue[l].entityCreateData);
			writer.Write(respawnQueue[l].entityNextRespawnTime);
		}
		bool value = false;
		writer.Write(value);
	}

	// Token: 0x06002DB7 RID: 11703 RVA: 0x000F9C20 File Offset: 0x000F7E20
	public void DeserializeZoneData(BinaryReader reader)
	{
		GhostReactorShiftManager shiftManager = this.reactor.shiftManager;
		GhostReactorLevelGenerator levelGenerator = this.reactor.levelGenerator;
		GRUIPromotionBot promotionBot = this.reactor.promotionBot;
		GRUIScoreboard[] array = this.reactor.scoreboards.ToArray();
		int depthLevel = reader.ReadInt32();
		this.reactor.depthLevel = depthLevel;
		int depthConfigIndex = reader.ReadInt32();
		this.reactor.depthConfigIndex = depthConfigIndex;
		float difficultyScalingForCurrentFloor = reader.ReadSingle();
		this.reactor.difficultyScalingForCurrentFloor = difficultyScalingForCurrentFloor;
		if (shiftManager != null)
		{
			bool flag = reader.ReadBoolean();
			double shiftStartTime = reader.ReadDouble();
			shiftManager.shiftStats.Deserialize(reader);
			shiftManager.RefreshShiftStatsDisplay();
			string text = reader.ReadString();
			shiftManager.SetShiftId(text);
			shiftManager.stateStartTime = reader.ReadDouble();
			GhostReactorShiftManager.State newState = (GhostReactorShiftManager.State)reader.ReadByte();
			shiftManager.SetState(newState, true);
			int inputSeed = reader.ReadInt32();
			if (flag)
			{
				levelGenerator.Generate(inputSeed);
				shiftManager.OnShiftStarted(text, shiftStartTime, false, true);
				this.reactor.ClearAllHandprints();
			}
		}
		if (promotionBot != null)
		{
			int actorNumber = reader.ReadInt32();
			int state = reader.ReadInt32();
			promotionBot.SetActivePlayerStateChange(actorNumber, state);
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i].currentScreen = (GRUIScoreboard.ScoreboardScreen)reader.ReadInt32();
		}
		this.reactor.RefreshScoreboards();
		this.reactor.RefreshDepth();
		List<GRToolPurchaseStation> toolPurchasingStations = this.reactor.toolPurchasingStations;
		int num = reader.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			int newSelectedIndex = reader.ReadInt32();
			if (j < toolPurchasingStations.Count && toolPurchasingStations[j] != null)
			{
				toolPurchasingStations[j].OnSelectionUpdate(newSelectedIndex);
			}
		}
		List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull = this.reactor.toolUpgradePurchaseStationsFull;
		int num2 = reader.ReadInt32();
		for (int k = 0; k < num2; k++)
		{
			int shelf = reader.ReadInt32();
			int item = reader.ReadInt32();
			int activePlayer = reader.ReadInt32();
			if (k < toolUpgradePurchaseStationsFull.Count && toolUpgradePurchaseStationsFull[k] != null)
			{
				toolUpgradePurchaseStationsFull[k].SetSelectedShelfAndItem(shelf, item, true);
				toolUpgradePurchaseStationsFull[k].SetActivePlayer(activePlayer);
			}
		}
		List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = this.reactor.respawnQueue;
		respawnQueue.Clear();
		int num3 = reader.ReadInt32();
		for (int l = 0; l < num3; l++)
		{
			respawnQueue.Add(new GhostReactor.EntityTypeRespawnTracker
			{
				entityTypeID = reader.ReadInt32(),
				entityCreateData = reader.ReadInt64(),
				entityNextRespawnTime = reader.ReadSingle()
			});
		}
		reader.ReadBoolean();
		this.reactor.VRRigRefresh();
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x000F9EC7 File Offset: 0x000F80C7
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		return createData;
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		return false;
	}

	// Token: 0x06002DBA RID: 11706 RVA: 0x000F9ECA File Offset: 0x000F80CA
	public bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount)
	{
		return EntityCount <= 128;
	}

	// Token: 0x06002DBB RID: 11707 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		return true;
	}

	// Token: 0x06002DBC RID: 11708 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ValidateCreateItemBatchSize(int size)
	{
		return true;
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06002DBE RID: 11710 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06002DBF RID: 11711 RVA: 0x000F9ED8 File Offset: 0x000F80D8
	public void OnTapLocal(bool isLeftHand, Vector3 pos, Quaternion rot, GorillaSurfaceOverride surfaceOverride, Vector3 handVelocity)
	{
		if (this.reactor != null)
		{
			this.reactor.OnTapLocal(isLeftHand, pos, rot, surfaceOverride);
		}
		if (this.IsAuthority())
		{
			float num = Math.Clamp(handVelocity.magnitude / 8f, 0f, 1f);
			if (num > 0.25f)
			{
				GRNoiseEventManager.instance.AddNoiseEvent(pos, num, 1f);
			}
		}
	}

	// Token: 0x06002DC0 RID: 11712 RVA: 0x000F9F44 File Offset: 0x000F8144
	public void OnSharedTap(VRRig rig, Vector3 tapPos, float handTapSpeed)
	{
		if (this.IsAuthority())
		{
			float num = Math.Clamp(handTapSpeed / 8f, 0f, 1f);
			if (num > 0.25f)
			{
				GRNoiseEventManager.instance.AddNoiseEvent(tapPos, num, 1f);
			}
		}
	}

	// Token: 0x06002DC1 RID: 11713 RVA: 0x000F9F8C File Offset: 0x000F818C
	public void SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		GRPlayer grplayer = GRPlayer.Get(actorNumber);
		grplayer.SerializeNetworkState(writer, grplayer.gamePlayer.rig.OwningNetPlayer);
	}

	// Token: 0x06002DC2 RID: 11714 RVA: 0x000F9FB8 File Offset: 0x000F81B8
	public void DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		GRPlayer player = GRPlayer.Get(actorNumber);
		GRPlayer.DeserializeNetworkStateAndBurn(reader, player, this);
	}

	// Token: 0x06002DC3 RID: 11715 RVA: 0x00002076 File Offset: 0x00000276
	public bool DebugIsToolStationHacked()
	{
		return false;
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06002DC4 RID: 11716 RVA: 0x00002076 File Offset: 0x00000276
	public static bool AggroDisabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002DC7 RID: 11719 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002DC8 RID: 11720 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040039F9 RID: 14841
	private const string EVENT_CORE_COLLECTED = "GRCollectCore";

	// Token: 0x040039FA RID: 14842
	private const string EVENT_ENEMY_KILLED = "GRKillEnemy";

	// Token: 0x040039FB RID: 14843
	public const string EVENT_BREAKABLE_BROKEN = "GRSmashBreakable";

	// Token: 0x040039FC RID: 14844
	public const string EVENT_ENEMY_ARMOR_BREAK = "GRArmorBreak";

	// Token: 0x040039FD RID: 14845
	public const string NETWORK_ROOM_GR_DEPTH = "ghostReactorDepth";

	// Token: 0x040039FE RID: 14846
	public const int GHOSTREACTOR_ZONE_ID = 5;

	// Token: 0x040039FF RID: 14847
	public const GTZone GT_ZONE_GHOSTREACTOR = GTZone.ghostReactor;

	// Token: 0x04003A00 RID: 14848
	public GameEntityManager gameEntityManager;

	// Token: 0x04003A01 RID: 14849
	public GameAgentManager gameAgentManager;

	// Token: 0x04003A02 RID: 14850
	public GRNoiseEventManager noiseEventManager;

	// Token: 0x04003A03 RID: 14851
	public PhotonView photonView;

	// Token: 0x04003A04 RID: 14852
	public GhostReactor reactor;

	// Token: 0x04003A05 RID: 14853
	public CallLimitersList<CallLimiter, GhostReactorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GhostReactorManager.RPC>();

	// Token: 0x04003A06 RID: 14854
	private const float HandprintThrottleTime = 0.25f;

	// Token: 0x04003A07 RID: 14855
	private float LastHandprintTime;

	// Token: 0x04003A08 RID: 14856
	private Coroutine activeSpawnSectionEntitiesCoroutine;

	// Token: 0x04003A09 RID: 14857
	private WaitForSeconds spawnSectionEntitiesWait = new WaitForSeconds(0.1f);

	// Token: 0x04003A0A RID: 14858
	private static List<GameEntityId> tempEntitiesToDestroy = new List<GameEntityId>();

	// Token: 0x04003A0B RID: 14859
	private GameEntity cachedBossEntity;

	// Token: 0x04003A0C RID: 14860
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x04003A0D RID: 14861
	public static bool entityDebugEnabled = false;

	// Token: 0x04003A0E RID: 14862
	public static bool noiseDebugEnabled = false;

	// Token: 0x04003A0F RID: 14863
	public static bool bayUnlockEnabled = false;

	// Token: 0x02000705 RID: 1797
	public enum RPC
	{
		// Token: 0x04003A11 RID: 14865
		ApplyCollectItem,
		// Token: 0x04003A12 RID: 14866
		ApplyChargeTool,
		// Token: 0x04003A13 RID: 14867
		ApplyDepositCurrency,
		// Token: 0x04003A14 RID: 14868
		ApplyPlayerRevived,
		// Token: 0x04003A15 RID: 14869
		GrantPlayerShield,
		// Token: 0x04003A16 RID: 14870
		RequestFireProjectile,
		// Token: 0x04003A17 RID: 14871
		ApplyShiftStart,
		// Token: 0x04003A18 RID: 14872
		ApplyShiftEnd,
		// Token: 0x04003A19 RID: 14873
		ToolPurchaseResponse,
		// Token: 0x04003A1A RID: 14874
		ApplyBreakableBroken,
		// Token: 0x04003A1B RID: 14875
		EntityEnteredDropZone,
		// Token: 0x04003A1C RID: 14876
		PromotionBotResponse,
		// Token: 0x04003A1D RID: 14877
		DistillItem,
		// Token: 0x04003A1E RID: 14878
		ApplySentientCoreDestination,
		// Token: 0x04003A1F RID: 14879
		Handprint,
		// Token: 0x04003A20 RID: 14880
		ApplyRecycleItem,
		// Token: 0x04003A21 RID: 14881
		ApplRecycleScanItem,
		// Token: 0x04003A22 RID: 14882
		SeedExtractorAction,
		// Token: 0x04003A23 RID: 14883
		ToolUpgradeStationAction,
		// Token: 0x04003A24 RID: 14884
		SendMothershipId,
		// Token: 0x04003A25 RID: 14885
		RefreshShiftCredit
	}

	// Token: 0x02000706 RID: 1798
	public enum GRPlayerAction
	{
		// Token: 0x04003A27 RID: 14887
		ButtonShiftStart,
		// Token: 0x04003A28 RID: 14888
		DelveDeeper,
		// Token: 0x04003A29 RID: 14889
		DelveState,
		// Token: 0x04003A2A RID: 14890
		ShuttleOpen,
		// Token: 0x04003A2B RID: 14891
		ShuttleClose,
		// Token: 0x04003A2C RID: 14892
		ShuttleLaunch,
		// Token: 0x04003A2D RID: 14893
		ShuttleArrive,
		// Token: 0x04003A2E RID: 14894
		ShuttleTargetLevelUp,
		// Token: 0x04003A2F RID: 14895
		ShuttleTargetLevelDown,
		// Token: 0x04003A30 RID: 14896
		SetPodLevel,
		// Token: 0x04003A31 RID: 14897
		SetPodChassisLevel,
		// Token: 0x04003A32 RID: 14898
		SeedExtractorOpenStation,
		// Token: 0x04003A33 RID: 14899
		SeedExtractorCloseStation,
		// Token: 0x04003A34 RID: 14900
		SeedExtractorCardSwipeFail,
		// Token: 0x04003A35 RID: 14901
		SeedExtractorTryDepositSeed,
		// Token: 0x04003A36 RID: 14902
		SeedExtractorDepositSeedSucceeded,
		// Token: 0x04003A37 RID: 14903
		SeedExtractorDepositSeedFailed,
		// Token: 0x04003A38 RID: 14904
		DEBUG_ResetDepth,
		// Token: 0x04003A39 RID: 14905
		DEBUG_DelveDeeper,
		// Token: 0x04003A3A RID: 14906
		DEBUG_DelveShallower
	}

	// Token: 0x02000707 RID: 1799
	public enum ToolPurchaseActionV2
	{
		// Token: 0x04003A3C RID: 14908
		RequestPurchaseAuthority,
		// Token: 0x04003A3D RID: 14909
		SelectShelfAndItem,
		// Token: 0x04003A3E RID: 14910
		NotifyPurchaseFail,
		// Token: 0x04003A3F RID: 14911
		NotifyPurchaseSuccess,
		// Token: 0x04003A40 RID: 14912
		RequestStationExclusivityAuthority,
		// Token: 0x04003A41 RID: 14913
		SetToolStationActivePlayer,
		// Token: 0x04003A42 RID: 14914
		SetHandleAndSelectionWheelPosition,
		// Token: 0x04003A43 RID: 14915
		SetToolStationHackedDebug
	}

	// Token: 0x02000708 RID: 1800
	public enum ToolPurchaseStationAction
	{
		// Token: 0x04003A45 RID: 14917
		ShiftLeft,
		// Token: 0x04003A46 RID: 14918
		ShiftRight,
		// Token: 0x04003A47 RID: 14919
		TryPurchase
	}

	// Token: 0x02000709 RID: 1801
	public enum ToolPurchaseStationResponse
	{
		// Token: 0x04003A49 RID: 14921
		SelectionUpdate,
		// Token: 0x04003A4A RID: 14922
		PurchaseSucceeded,
		// Token: 0x04003A4B RID: 14923
		PurchaseFailed
	}
}
