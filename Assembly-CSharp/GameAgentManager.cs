using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020006A3 RID: 1699
[NetworkBehaviourWeaved(0)]
public class GameAgentManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x06002A59 RID: 10841 RVA: 0x000E3A51 File Offset: 0x000E1C51
	// (set) Token: 0x06002A5A RID: 10842 RVA: 0x000E3A59 File Offset: 0x000E1C59
	public bool TickRunning { get; set; }

	// Token: 0x06002A5B RID: 10843 RVA: 0x000E3A64 File Offset: 0x000E1C64
	protected override void Awake()
	{
		this.agents = new List<GameAgent>(128);
		this.netIdsForDestination = new List<int>();
		this.destinationsForDestination = new List<Vector3>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<byte>();
		this.netIdsForBehavior = new List<int>();
		this.behaviorsForBehavior = new List<byte>();
		this.nextAgentIndexUpdate = 0;
		this.nextAgentIndexThink = 0;
	}

	// Token: 0x06002A5C RID: 10844 RVA: 0x000E3AD1 File Offset: 0x000E1CD1
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x000E3ADF File Offset: 0x000E1CDF
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x000E3AED File Offset: 0x000E1CED
	public static GameAgentManager Get(GameEntity gameEntity)
	{
		if (!(gameEntity == null) && !(gameEntity.manager == null))
		{
			return gameEntity.manager.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x000E3B13 File Offset: 0x000E1D13
	public List<GameAgent> GetAgents()
	{
		return this.agents;
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x000E3B1B File Offset: 0x000E1D1B
	public int GetGameAgentCount()
	{
		return this.agents.Count;
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x000E3B28 File Offset: 0x000E1D28
	public void AddGameAgent(GameAgent gameAgent)
	{
		this.agents.Add(gameAgent);
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000E3B36 File Offset: 0x000E1D36
	public void RemoveGameAgent(GameAgent gameAgent)
	{
		this.agents.Remove(gameAgent);
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x000E3B45 File Offset: 0x000E1D45
	public GameAgent GetGameAgent(GameEntityId id)
	{
		return this.entityManager.GetGameEntity(id).GetComponent<GameAgent>();
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x000E3B58 File Offset: 0x000E1D58
	public void Tick()
	{
		if (this.IsAuthority())
		{
			int num = Mathf.Min(1, this.agents.Count);
			for (int i = 0; i < num; i++)
			{
				if (this.nextAgentIndexThink >= this.agents.Count)
				{
					this.nextAgentIndexThink = 0;
				}
				this.agents[this.nextAgentIndexThink].OnThink(Time.deltaTime);
				this.nextAgentIndexThink++;
			}
		}
		for (int j = 0; j < this.agents.Count; j++)
		{
			if (this.agents[j] != null)
			{
				this.agents[j].OnUpdate();
			}
		}
		if (this.IsAuthority())
		{
			if (this.netIdsForDestination.Count > 0 && Time.time > this.lastDestinationSentTime + this.destinationCooldown)
			{
				this.lastDestinationSentTime = Time.time;
				base.SendRPC("ApplyDestinationRPC", RpcTarget.All, new object[]
				{
					this.netIdsForDestination.ToArray(),
					this.destinationsForDestination.ToArray()
				});
				this.netIdsForDestination.Clear();
				this.destinationsForDestination.Clear();
			}
			if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSentTime + this.stateCooldown)
			{
				this.lastStateSentTime = Time.time;
				base.SendRPC("ApplyStateRPC", RpcTarget.All, new object[]
				{
					this.netIdsForState.ToArray(),
					this.statesForState.ToArray()
				});
				this.netIdsForState.Clear();
				this.statesForState.Clear();
			}
			if (this.netIdsForBehavior.Count > 0 && Time.time > this.lastBehaviorSentTime + this.behaviorCooldown)
			{
				this.lastBehaviorSentTime = Time.time;
				base.SendRPC("ApplyBehaviorRPC", RpcTarget.All, new object[]
				{
					this.netIdsForBehavior.ToArray(),
					this.behaviorsForBehavior.ToArray()
				});
				this.netIdsForBehavior.Clear();
				this.behaviorsForBehavior.Clear();
			}
		}
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000E3D67 File Offset: 0x000E1F67
	public bool IsAuthority()
	{
		return this.entityManager.IsAuthority();
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x000E3D74 File Offset: 0x000E1F74
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x000E3D82 File Offset: 0x000E1F82
	public bool IsAuthorityPlayer(Player player)
	{
		return this.entityManager.IsAuthorityPlayer(player);
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000E3D90 File Offset: 0x000E1F90
	public Player GetAuthorityPlayer()
	{
		return this.entityManager.GetAuthorityPlayer();
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x000E3D9D File Offset: 0x000E1F9D
	public bool IsZoneActive()
	{
		return this.entityManager.IsZoneActive();
	}

	// Token: 0x06002A6A RID: 10858 RVA: 0x000E3DAA File Offset: 0x000E1FAA
	public bool IsPositionInManagerBounds(Vector3 pos)
	{
		return this.entityManager.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x000E3DB8 File Offset: 0x000E1FB8
	public bool IsValidClientRPC(Player sender)
	{
		return this.entityManager.IsValidClientRPC(sender);
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x000E3DC6 File Offset: 0x000E1FC6
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId);
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000E3DD5 File Offset: 0x000E1FD5
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x000E3DE5 File Offset: 0x000E1FE5
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidClientRPC(sender, pos);
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000E3DF4 File Offset: 0x000E1FF4
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.entityManager.IsValidAuthorityRPC(sender);
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000E3E02 File Offset: 0x000E2002
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, entityNetId);
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x000E3E11 File Offset: 0x000E2011
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, entityNetId, pos);
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x000E3E21 File Offset: 0x000E2021
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.entityManager.IsValidAuthorityRPC(sender, pos);
	}

	// Token: 0x06002A73 RID: 10867 RVA: 0x000E3E30 File Offset: 0x000E2030
	public void RequestDestination(GameAgent agent, Vector3 dest)
	{
		if (!this.IsAuthority())
		{
			Debug.LogError("RequestDestination should only be called from the master client");
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForDestination.Contains(netIdFromEntityId))
		{
			this.destinationsForDestination[this.netIdsForDestination.IndexOf(netIdFromEntityId)] = dest;
			return;
		}
		this.netIdsForDestination.Add(netIdFromEntityId);
		this.destinationsForDestination.Add(dest);
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x000E3EA8 File Offset: 0x000E20A8
	[PunRPC]
	public void ApplyDestinationRPC(int[] netEntityId, Vector3[] dest, PhotonMessageInfo info)
	{
		if (!this.IsZoneActive() || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyDestination))
		{
			return;
		}
		if (netEntityId == null || dest == null || netEntityId.Length != dest.Length)
		{
			return;
		}
		int i = 0;
		while (i < netEntityId.Length)
		{
			if (this.IsValidClientRPC(info.Sender, netEntityId[i], dest[i]))
			{
				int num = i;
				float num2 = 10000f;
				if (dest[num].IsValid(num2))
				{
					i++;
					continue;
				}
			}
			return;
		}
		for (int j = 0; j < netEntityId.Length; j++)
		{
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[j]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.ApplyDestination(dest[j]);
		}
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x000E3F6C File Offset: 0x000E216C
	public void RequestState(GameAgent agent, byte state)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = state;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(state);
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x000E3FD8 File Offset: 0x000E21D8
	[PunRPC]
	public void ApplyStateRPC(int[] netEntityId, byte[] state, PhotonMessageInfo info)
	{
		if (netEntityId == null || state == null || netEntityId.Length != state.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component == null)
			{
				return;
			}
			component.OnBodyStateChanged(state[i]);
		}
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x000E4060 File Offset: 0x000E2260
	public void RequestBehavior(GameAgent agent, byte behavior)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.entityManager.GetNetIdFromEntityId(agent.entity.id);
		if (this.netIdsForBehavior.Contains(netIdFromEntityId))
		{
			this.behaviorsForBehavior[this.netIdsForBehavior.IndexOf(netIdFromEntityId)] = behavior;
			return;
		}
		this.netIdsForBehavior.Add(netIdFromEntityId);
		this.behaviorsForBehavior.Add(behavior);
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x000E40CC File Offset: 0x000E22CC
	[PunRPC]
	public void ApplyBehaviorRPC(int[] netEntityId, byte[] behavior, PhotonMessageInfo info)
	{
		if (netEntityId == null || behavior == null || netEntityId.Length != behavior.Length || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyBehaviour))
		{
			return;
		}
		for (int i = 0; i < netEntityId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netEntityId[i]))
			{
				return;
			}
			GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(netEntityId[i]));
			if (gameEntity == null)
			{
				return;
			}
			GameAgent component = gameEntity.GetComponent<GameAgent>();
			if (component != null)
			{
				component.OnBehaviorStateChanged(behavior[i]);
			}
		}
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x000E4154 File Offset: 0x000E2354
	public void RequestTarget(GameAgent agent, NetPlayer player)
	{
		if (player == agent.targetPlayer)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.targetPlayer = player;
		base.SendRPC("ApplyTargetRPC", RpcTarget.Others, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			(player == null) ? null : player.GetPlayerRef()
		});
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x000E41C4 File Offset: 0x000E23C4
	[PunRPC]
	public void ApplyTargetRPC(int agentNetId, Player player, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, agentNetId) || this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget) || player == null)
		{
			return;
		}
		GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
		if (gameEntity == null)
		{
			return;
		}
		GameAgent component = gameEntity.GetComponent<GameAgent>();
		if (component == null)
		{
			return;
		}
		component.targetPlayer = NetPlayer.Get(player);
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x000E4234 File Offset: 0x000E2434
	public void RequestJump(GameAgent agent, Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		if (agent == null)
		{
			return;
		}
		agent.OnJumpRequested(start, end, heightScale, speedScale);
		base.SendRPC("ApplyJumpRPC", RpcTarget.Others, new object[]
		{
			this.entityManager.GetNetIdFromEntityId(agent.entity.id),
			start,
			end,
			heightScale,
			speedScale
		});
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x000E42B8 File Offset: 0x000E24B8
	[PunRPC]
	public void ApplyJumpRPC(int agentNetId, Vector3 start, Vector3 end, float heightScale, float speedScale, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, agentNetId) && !this.m_RpcSpamChecks.IsSpamming(GameAgentManager.RPC.ApplyTarget))
		{
			float num = 10000f;
			if (start.IsValid(num))
			{
				float num2 = 10000f;
				if (end.IsValid(num2) && this.entityManager.IsPositionInManagerBounds(start) && this.entityManager.IsPositionInManagerBounds(end) && this.entityManager.IsEntityNearPosition(agentNetId, start, 16f) && heightScale <= 5f && speedScale <= 5f)
				{
					if ((end - start).sqrMagnitude > 625f)
					{
						return;
					}
					GameEntity gameEntity = this.entityManager.GetGameEntity(this.entityManager.GetEntityIdFromNetId(agentNetId));
					if (gameEntity == null)
					{
						return;
					}
					GameAgent component = gameEntity.GetComponent<GameAgent>();
					if (component == null)
					{
						return;
					}
					component.OnJumpRequested(start, end, heightScale, speedScale);
					return;
				}
			}
		}
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x000E43A0 File Offset: 0x000E25A0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		int num = Mathf.Min(4, this.agents.Count);
		stream.SendNext(num);
		for (int i = 0; i < num; i++)
		{
			if (this.nextAgentIndexUpdate >= this.agents.Count)
			{
				this.nextAgentIndexUpdate = 0;
			}
			stream.SendNext(this.entityManager.GetNetIdFromEntityId(this.agents[this.nextAgentIndexUpdate].entity.id));
			long num2 = BitPackUtils.PackWorldPosForNetwork(this.agents[this.nextAgentIndexUpdate].transform.position);
			stream.SendNext(num2);
			int num3 = BitPackUtils.PackQuaternionForNetwork(this.agents[this.nextAgentIndexUpdate].transform.rotation);
			stream.SendNext(num3);
			this.nextAgentIndexUpdate++;
		}
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x000E4490 File Offset: 0x000E2690
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender))
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		for (int i = 0; i < num; i++)
		{
			int netId = (int)stream.ReceiveNext();
			Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork((long)stream.ReceiveNext());
			Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork((int)stream.ReceiveNext());
			if (this.IsPositionInManagerBounds(vector) && this.entityManager.IsValidNetId(netId))
			{
				GameEntityId entityIdFromNetId = this.entityManager.GetEntityIdFromNetId(netId);
				GameAgent gameAgent = this.GetGameAgent(entityIdFromNetId);
				if (gameAgent != null)
				{
					gameAgent.ApplyNetworkUpdate(vector, rotation);
				}
			}
		}
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04003716 RID: 14102
	public const float MAX_JUMP_DISTANCE = 25f;

	// Token: 0x04003717 RID: 14103
	public GameEntityManager entityManager;

	// Token: 0x04003718 RID: 14104
	public PhotonView photonView;

	// Token: 0x04003719 RID: 14105
	private List<GameAgent> agents;

	// Token: 0x0400371A RID: 14106
	private float lastDestinationSentTime;

	// Token: 0x0400371B RID: 14107
	private float destinationCooldown;

	// Token: 0x0400371C RID: 14108
	private List<int> netIdsForDestination;

	// Token: 0x0400371D RID: 14109
	private List<Vector3> destinationsForDestination;

	// Token: 0x0400371E RID: 14110
	private List<int> netIdsForState;

	// Token: 0x0400371F RID: 14111
	private List<byte> statesForState;

	// Token: 0x04003720 RID: 14112
	private float lastStateSentTime;

	// Token: 0x04003721 RID: 14113
	private float stateCooldown;

	// Token: 0x04003722 RID: 14114
	private List<int> netIdsForBehavior;

	// Token: 0x04003723 RID: 14115
	private List<byte> behaviorsForBehavior;

	// Token: 0x04003724 RID: 14116
	private float lastBehaviorSentTime;

	// Token: 0x04003725 RID: 14117
	private float behaviorCooldown = 0.25f;

	// Token: 0x04003726 RID: 14118
	private const int MAX_UPDATES_PER_FRAME = 4;

	// Token: 0x04003727 RID: 14119
	private int nextAgentIndexUpdate;

	// Token: 0x04003728 RID: 14120
	private const int MAX_THINK_PER_FRAME = 1;

	// Token: 0x04003729 RID: 14121
	private int nextAgentIndexThink;

	// Token: 0x0400372B RID: 14123
	public CallLimitersList<CallLimiter, GameAgentManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameAgentManager.RPC>();

	// Token: 0x020006A4 RID: 1700
	public enum RPC
	{
		// Token: 0x0400372D RID: 14125
		ApplyDestination,
		// Token: 0x0400372E RID: 14126
		ApplyState,
		// Token: 0x0400372F RID: 14127
		ApplyBehaviour,
		// Token: 0x04003730 RID: 14128
		ApplyImpact,
		// Token: 0x04003731 RID: 14129
		ApplyTarget
	}
}
