using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x02000181 RID: 385
[DefaultExecutionOrder(0)]
public class SuperInfectionManager : MonoBehaviour, IGameEntityZoneComponent, IFactoryItemProvider
{
	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000A48 RID: 2632 RVA: 0x00037152 File Offset: 0x00035352
	public bool HasSIZonePlatform
	{
		get
		{
			return this.zoneSuperInfectionRef.TargetID != 0;
		}
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00037162 File Offset: 0x00035362
	public bool HasActiveTryOnDispenser
	{
		get
		{
			return this.tryOnDispenserCount > 0;
		}
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x0003716D File Offset: 0x0003536D
	internal void RegisterTryOnDispenser()
	{
		this.tryOnDispenserCount++;
	}

	// Token: 0x06000A4B RID: 2635 RVA: 0x0003717D File Offset: 0x0003537D
	internal void UnregisterTryOnDispenser()
	{
		this.tryOnDispenserCount = Mathf.Max(this.tryOnDispenserCount - 1, 0);
	}

	// Token: 0x06000A4C RID: 2636 RVA: 0x00037193 File Offset: 0x00035393
	private void Awake()
	{
		GameEntityManager gameEntityManager = this.gameEntityManager;
		gameEntityManager.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(gameEntityManager.OnEntityRemoved, new Action<GameEntity>(this.OnEntityRemoved));
	}

	// Token: 0x06000A4D RID: 2637 RVA: 0x000371BC File Offset: 0x000353BC
	public void OnEnableZoneSuperInfection(SuperInfection zone)
	{
		this.zoneSuperInfection = zone;
		if (this.PendingZoneInit)
		{
			this.PendingZoneInit = false;
			this.OnZoneInit();
		}
		if (this.gameEntityManager.PendingTableData)
		{
			this.gameEntityManager.ResolveTableData();
		}
	}

	// Token: 0x06000A4E RID: 2638 RVA: 0x000371F4 File Offset: 0x000353F4
	private void OnEnable()
	{
		if (!SuperInfectionManager.siManagerByZone.TryAdd(this.gameEntityManager.zone, this))
		{
			Debug.LogError("[GT/SuperInfectionManager]  ERROR!!!  " + string.Format("Tried to add a duplicate Manager for zone `{0}`. Did you forget to change the ", this.gameEntityManager.zone) + "zone on the GameEntityManager on GameObject at path: " + base.transform.GetPathQ(), this);
			return;
		}
		GameMode.OnStartGameMode += this._OnStartGameMode;
	}

	// Token: 0x06000A4F RID: 2639 RVA: 0x00037265 File Offset: 0x00035465
	private void OnDisable()
	{
		SuperInfectionManager.siManagerByZone.Remove(this.gameEntityManager.zone);
		GameMode.OnStartGameMode -= this._OnStartGameMode;
	}

	// Token: 0x06000A50 RID: 2640 RVA: 0x00037290 File Offset: 0x00035490
	private void _OnStartGameMode(GameModeType newGameModeType)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		List<GameEntityId> list;
		using (CollectionPool<List<GameEntityId>, GameEntityId>.Get(out list))
		{
			ESuperGameModes esuperGameModes = (ESuperGameModes)(1 << (int)newGameModeType);
			foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
			{
				SIGadget sigadget;
				SITechTreePage sitechTreePage;
				if (!(gameEntity == null) && gameEntity.TryGetComponent<SIGadget>(out sigadget) && this.techTreeSO.TryGetTreePage(sigadget.PageId, out sitechTreePage) && (sitechTreePage.excludedGameModes & esuperGameModes) != (ESuperGameModes)0)
				{
					list.Add(gameEntity.id);
				}
			}
			if (list.Count > 0)
			{
				this.gameEntityManager.RequestDestroyItems(list);
			}
		}
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00037378 File Offset: 0x00035578
	public static SuperInfectionManager GetSIManagerForZone(GTZone targetZone)
	{
		SuperInfectionManager result;
		if (SuperInfectionManager.siManagerByZone.TryGetValue(targetZone, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000A52 RID: 2642 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsSupercharged
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnZoneCreate()
	{
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00037398 File Offset: 0x00035598
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthority())
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].WriteDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].WriteDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.WriteDataPUN(stream, info);
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0003742C File Offset: 0x0003562C
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		if (!this.gameEntityManager.IsAuthorityPlayer(info.Sender))
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].ReadDataPUN(stream, info);
		}
		for (int j = 0; j < this.zoneSuperInfection.siDeposits.Length; j++)
		{
			this.zoneSuperInfection.siDeposits[j].ReadDataPUN(stream, info);
		}
		this.zoneSuperInfection.questBoard.ReadDataPUN(stream, info);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x000374C8 File Offset: 0x000356C8
	void IGameEntityZoneComponent.SerializeZoneData(BinaryWriter writer)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].SerializeZoneData(writer);
		}
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x00037510 File Offset: 0x00035710
	void IGameEntityZoneComponent.DeserializeZoneData(BinaryReader reader)
	{
		if (this.zoneSuperInfection == null)
		{
			return;
		}
		for (int i = 0; i < this.zoneSuperInfection.siTerminals.Length; i++)
		{
			this.zoneSuperInfection.siTerminals[i].DeserializeZoneData(reader);
		}
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00037558 File Offset: 0x00035758
	void IGameEntityZoneComponent.SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
		SIPlayer siplayer = SIPlayer.Get(actorNumber);
		siplayer.SerializeNetworkState(writer, siplayer.gamePlayer.rig.OwningNetPlayer);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00037584 File Offset: 0x00035784
	void IGameEntityZoneComponent.DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
		SIPlayer player = SIPlayer.Get(actorNumber);
		SIPlayer.DeserializeNetworkStateAndBurn(reader, player, this);
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x000375A0 File Offset: 0x000357A0
	public bool IsZoneReady()
	{
		if (!this.HasSIZonePlatform)
		{
			return NetworkSystem.Instance.InRoom && VRRig.LocalRig.zoneEntity.currentZone == this.gameEntityManager.zone;
		}
		return NetworkSystem.Instance.InRoom && SuperInfectionManager.IsSuperGameMode() && this.zoneSuperInfection.IsNotNull() && VRRig.LocalRig.zoneEntity.currentZone == this.gameEntityManager.zone && SIProgression.Instance != null && SIProgression.Instance._treeReady;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00037638 File Offset: 0x00035838
	public bool ShouldClearZone()
	{
		if (!this.HasSIZonePlatform)
		{
			return false;
		}
		if (GameMode.ActiveGameMode != null)
		{
			GameModeType gameModeType = GameMode.ActiveGameMode.GameType();
			return gameModeType != GameModeType.SuperInfect && gameModeType != GameModeType.SuperCasual;
		}
		return false;
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00037678 File Offset: 0x00035878
	public static bool IsSuperGameMode()
	{
		GameModeType currentGameModeType = GameMode.CurrentGameModeType;
		return currentGameModeType == GameModeType.SuperInfect || currentGameModeType == GameModeType.SuperCasual;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x000376A0 File Offset: 0x000358A0
	public void OnCreateGameEntity(GameEntity entity)
	{
		SIGadget component = entity.GetComponent<SIGadget>();
		bool flag = (entity.createData & long.MinValue) != 0L;
		if (component != null)
		{
			SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)-1)));
			if (siplayer != null)
			{
				int num = 0;
				for (int i = siplayer.activePlayerGadgets.Count - 1; i >= 0; i--)
				{
					GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
					if (gameEntityFromNetId == null)
					{
						siplayer.activePlayerGadgets.RemoveAt(i);
					}
					else
					{
						num++;
						if (num >= siplayer.TotalGadgetLimit)
						{
							this.gameEntityManager.DestroyItemLocal(gameEntityFromNetId.id);
							break;
						}
					}
				}
				if (!siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
				{
					siplayer.activePlayerGadgets.Add(entity.GetNetId());
				}
			}
			SIUpgradeSet siupgradeSet = new SIUpgradeSet((int)((entity.createData & 9223372032559808512L) >> 32));
			siupgradeSet = component.FilterUpgradeNodes(siupgradeSet);
			component.ApplyUpgradeNodes(siupgradeSet);
			component.RefreshUpgradeVisuals(siupgradeSet);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.AddGadget(component);
			}
			if (flag)
			{
				entity.shouldDestroyOnZoneExit = true;
				GameEntityDelayedDestroy gameEntityDelayedDestroy = entity.gameObject.AddComponent<GameEntityDelayedDestroy>();
				gameEntityDelayedDestroy.Configure(SIGadgetDispenser.g_tryOnOptions);
				gameEntityDelayedDestroy.ResetTimer();
			}
		}
		List<SuperInfectionSnapPoint> list;
		using (CollectionPool<List<SuperInfectionSnapPoint>, SuperInfectionSnapPoint>.Get(out list))
		{
			entity.GetComponentsInChildren<SuperInfectionSnapPoint>(true, list);
			foreach (SuperInfectionSnapPoint snapPoint in list)
			{
				this.RegisterSnapPoint(snapPoint);
			}
		}
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00037878 File Offset: 0x00035A78
	public void OnZoneClear(ZoneClearReason reason)
	{
		SuperInfection superInfection = this.zoneSuperInfection;
		if (superInfection != null)
		{
			superInfection.OnZoneClear(reason);
		}
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		if (localPlayer != null)
		{
			localPlayer.Reset();
		}
		SIPlayer.ClearPlayerCache();
		this.allSnapPoints.Clear();
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x000378AC File Offset: 0x00035AAC
	public void OnZoneInit()
	{
		if (this.zoneSuperInfection == null && this.HasSIZonePlatform)
		{
			this.PendingZoneInit = true;
			return;
		}
		SuperInfectionManager.activeSuperInfectionManager = this;
		if (this.gameEntityManager.IsAuthority() && this.zoneSuperInfection != null)
		{
			this.TestSpawnGadget();
		}
		if (this.zoneSuperInfection != null)
		{
			this.zoneSuperInfection.OnZoneInit();
		}
		if (SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber) != null)
		{
			this.progression.Init();
			if (this.progression.ClientReady)
			{
				SIPlayer.SetAndBroadcastProgression();
			}
			else
			{
				this.progression.OnClientReady += this.<OnZoneInit>g__WhenReady|50_0;
			}
		}
		this.allSnapPoints.Clear();
		foreach (GameEntity gameEntity in this.gameEntityManager.GetGameEntities())
		{
			if (!(gameEntity == null))
			{
				List<SuperInfectionSnapPoint> list;
				using (CollectionPool<List<SuperInfectionSnapPoint>, SuperInfectionSnapPoint>.Get(out list))
				{
					gameEntity.GetComponentsInChildren<SuperInfectionSnapPoint>(true, list);
					foreach (SuperInfectionSnapPoint snapPoint in list)
					{
						this.RegisterSnapPoint(snapPoint);
					}
				}
			}
		}
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00037A28 File Offset: 0x00035C28
	public void RegisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		List<SuperInfectionSnapPoint> list;
		if (!this.allSnapPoints.TryGetValue(snapPoint.jointType, out list))
		{
			list = (this.allSnapPoints[snapPoint.jointType] = new List<SuperInfectionSnapPoint>());
		}
		list.Add(snapPoint);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00037A6C File Offset: 0x00035C6C
	public void UnregisterSnapPoint(SuperInfectionSnapPoint snapPoint)
	{
		if (this.allSnapPoints.ContainsKey(snapPoint.jointType))
		{
			this.allSnapPoints[snapPoint.jointType].Remove(snapPoint);
			if (this.allSnapPoints[snapPoint.jointType].Count == 0)
			{
				this.allSnapPoints.Remove(snapPoint.jointType);
			}
		}
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00037ACE File Offset: 0x00035CCE
	public IEnumerable<SuperInfectionSnapPoint> GetPoints(SnapJointType jointType)
	{
		foreach (KeyValuePair<SnapJointType, List<SuperInfectionSnapPoint>> keyValuePair in this.allSnapPoints)
		{
			if ((keyValuePair.Key & jointType) != SnapJointType.None)
			{
				foreach (SuperInfectionSnapPoint superInfectionSnapPoint in keyValuePair.Value)
				{
					yield return superInfectionSnapPoint;
				}
				List<SuperInfectionSnapPoint>.Enumerator enumerator2 = default(List<SuperInfectionSnapPoint>.Enumerator);
			}
		}
		Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator enumerator = default(Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00037AE8 File Offset: 0x00035CE8
	public SuperInfectionSnapPoint FindNearestSnapPoint(SnapJointType jointType, Vector3 origin, float maxDist, bool includeOccupied = false)
	{
		SuperInfectionSnapPoint result = null;
		float num = maxDist * maxDist;
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.GetPoints(jointType))
		{
			if (!(superInfectionSnapPoint == null) && superInfectionSnapPoint.isActiveAndEnabled && (includeOccupied || !superInfectionSnapPoint.HasSnapped()))
			{
				float sqrMagnitude = (superInfectionSnapPoint.transform.position - origin).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = superInfectionSnapPoint;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00037B7C File Offset: 0x00035D7C
	public void CallRPC(SuperInfectionManager.ClientToAuthorityRPC clientToAuthorityRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToAuthorityRPC", this.gameEntityManager.GetAuthorityPlayer(), new object[]
			{
				(int)clientToAuthorityRPC,
				data
			});
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00037BB8 File Offset: 0x00035DB8
	public void CallRPC(SuperInfectionManager.ClientToClientRPC clientToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIClientToClientRPC", RpcTarget.Others, new object[]
			{
				(int)clientToClientRPC,
				data
			});
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x00037BEA File Offset: 0x00035DEA
	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", RpcTarget.Others, new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x00037C1C File Offset: 0x00035E1C
	public void CallRPC(SuperInfectionManager.AuthorityToClientRPC authorityToClientRPC, int actorNr, object[] data)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.photonView.RPC("SIAuthorityToClientRPC", NetworkSystem.Instance.GetNetPlayerByID(actorNr).GetPlayerRef(), new object[]
			{
				(int)authorityToClientRPC,
				data
			});
		}
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00037C68 File Offset: 0x00035E68
	[PunRPC]
	public void SIClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidAuthorityRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToAuthorityRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToAuthorityRPC(clientToAuthorityRPCEnum, data, info);
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x00037CC4 File Offset: 0x00035EC4
	public void ProcessClientToAuthorityRPC(int clientToAuthorityRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToAuthorityRPCEnum)
		{
		case 0:
		{
			if (this.zoneSuperInfection == null)
			{
				return;
			}
			if (data.Length != 4)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			int data2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out data2))
			{
				return;
			}
			int num2;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num2))
			{
				return;
			}
			int num3;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out num3))
			{
				return;
			}
			if (num3 < 0 || num3 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SITouchscreenButton.SITouchscreenButtonType), (SITouchscreenButton.SITouchscreenButtonType)num))
			{
				return;
			}
			if (!Enum.IsDefined(typeof(SICombinedTerminal.TerminalSubFunction), (SICombinedTerminal.TerminalSubFunction)num2))
			{
				return;
			}
			this.zoneSuperInfection.siTerminals[num3].TouchscreenButtonPressed((SITouchscreenButton.SITouchscreenButtonType)num, data2, info.Sender.ActorNumber, (SICombinedTerminal.TerminalSubFunction)num2);
			return;
		}
		case 1:
		{
			if (this.zoneSuperInfection == null)
			{
				return;
			}
			if (data.Length != 1)
			{
				return;
			}
			int num4;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num4))
			{
				return;
			}
			if (num4 < 0 || num4 >= this.zoneSuperInfection.siTerminals.Length)
			{
				return;
			}
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			SICombinedTerminal sicombinedTerminal = this.zoneSuperInfection.siTerminals[num4];
			if (!siplayer.gamePlayer.rig.IsPositionInRange(sicombinedTerminal.transform.position, 3f))
			{
				return;
			}
			sicombinedTerminal.PlayerHandScanned(info.Sender.ActorNumber);
			return;
		}
		case 2:
		{
			if (this.zoneSuperInfection == null)
			{
				return;
			}
			if (data.Length != 2)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			int num5;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out num5))
			{
				return;
			}
			if (num5 < 0 || num5 >= this.zoneSuperInfection.siDeposits.Length)
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (gameEntityFromNetId == null)
			{
				return;
			}
			SIResourceDeposit siresourceDeposit = this.zoneSuperInfection.siDeposits[num5];
			if ((gameEntityFromNetId.transform.position - siresourceDeposit.transform.position).IsLongerThan(3f))
			{
				return;
			}
			SIResource component = gameEntityFromNetId.GetComponent<SIResource>();
			if (component != null)
			{
				siresourceDeposit.ResourceDeposited(component);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToAuthorityRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId3))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data3;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data3))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(netId3);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToAuthorityRPC(info, rpcID2, data3);
			}
			break;
		}
		case 5:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00037FBC File Offset: 0x000361BC
	[PunRPC]
	public void SIAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (!this.gameEntityManager.IsValidClientRPC(info.Sender))
		{
			return;
		}
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.authorityToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessAuthorityToClientRPC(authorityToClientRPCEnum, data, info);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00038018 File Offset: 0x00036218
	public void ProcessAuthorityToClientRPC(int authorityToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (authorityToClientRPCEnum)
		{
		case 3:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
			if (!gameEntityFromNetId)
			{
				return;
			}
			SIGadget component = gameEntityFromNetId.GetComponent<SIGadget>();
			if (component)
			{
				component.ProcessAuthorityToClientRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 4:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data2;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data2))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessAuthorityToClientRPC(info, rpcID2, data2);
				return;
			}
			break;
		}
		case 5:
		{
			if (data.Length != 1)
			{
				return;
			}
			Vector3 position;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out position))
			{
				return;
			}
			if (SIPlayer.LocalPlayer)
			{
				SIPlayer.LocalPlayer.TriggerIdolDepositedCelebration(position);
				return;
			}
			break;
		}
		case 6:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x0003812C File Offset: 0x0003632C
	[PunRPC]
	public void SIClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		if (data == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
		if (siplayer.IsNull() || !siplayer.clientToClientRPCLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		this.ProcessClientToClientRPC(clientToClientRPCEnum, data, info);
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00038174 File Offset: 0x00036374
	public void ProcessClientToClientRPC(int clientToClientRPCEnum, object[] data, PhotonMessageInfo info)
	{
		switch (clientToClientRPCEnum)
		{
		case 0:
		{
			SIPlayer siplayer = SIPlayer.Get(info.Sender.ActorNumber);
			if (siplayer == null)
			{
				return;
			}
			if (data.Length != 8)
			{
				return;
			}
			int[] resourceArray;
			if (!GameEntityManager.ValidateDataType<int[]>(data[0], out resourceArray))
			{
				return;
			}
			int[] limitedDepositTimeArray;
			if (!GameEntityManager.ValidateDataType<int[]>(data[1], out limitedDepositTimeArray))
			{
				return;
			}
			bool[][] techTreeData;
			if (!GameEntityManager.ValidateDataType<bool[][]>(data[2], out techTreeData))
			{
				return;
			}
			int stashedQuests;
			if (!GameEntityManager.ValidateDataType<int>(data[3], out stashedQuests))
			{
				return;
			}
			int stashedBonusPoints;
			if (!GameEntityManager.ValidateDataType<int>(data[4], out stashedBonusPoints))
			{
				return;
			}
			int bonusProgress;
			if (!GameEntityManager.ValidateDataType<int>(data[5], out bonusProgress))
			{
				return;
			}
			int[] currentQuestIds;
			if (!GameEntityManager.ValidateDataType<int[]>(data[6], out currentQuestIds))
			{
				return;
			}
			int[] currentQuestProgresses;
			if (!GameEntityManager.ValidateDataType<int[]>(data[7], out currentQuestProgresses))
			{
				return;
			}
			siplayer.UpdateProgression(resourceArray, limitedDepositTimeArray, techTreeData, stashedQuests, stashedBonusPoints, bonusProgress, currentQuestIds, currentQuestProgresses);
			if (this.zoneSuperInfection != null)
			{
				this.zoneSuperInfection.RefreshStations(info.Sender.ActorNumber);
				return;
			}
			break;
		}
		case 1:
		{
			if (data.Length != 5)
			{
				return;
			}
			if (SIPlayer.Get(info.Sender.ActorNumber) == null)
			{
				return;
			}
			int netId;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId))
			{
				return;
			}
			Vector3 velocity;
			if (GameEntityManager.ValidateDataType<Vector3>(data[1], out velocity))
			{
				float num = 10000f;
				if (velocity.IsValid(num))
				{
					Vector3 angVelocity;
					if (GameEntityManager.ValidateDataType<Vector3>(data[2], out angVelocity))
					{
						num = 10000f;
						if (angVelocity.IsValid(num))
						{
							Vector3 targetPosition;
							if (GameEntityManager.ValidateDataType<Vector3>(data[3], out targetPosition))
							{
								num = 10000f;
								if (targetPosition.IsValid(num))
								{
									Quaternion targetRotation;
									if (!GameEntityManager.ValidateDataType<Quaternion>(data[4], out targetRotation) || !targetRotation.IsValid())
									{
										return;
									}
									GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(netId);
									if (gameEntityFromNetId == null)
									{
										return;
									}
									if (gameEntityFromNetId.heldByActorNumber != info.Sender.ActorNumber && gameEntityFromNetId.snappedByActorNumber != info.Sender.ActorNumber)
									{
										return;
									}
									SIGadgetDashYoyo component = gameEntityFromNetId.GetComponent<SIGadgetDashYoyo>();
									if (component == null)
									{
										return;
									}
									component.RemoteThrowYoYoTarget(velocity, angVelocity, targetPosition, targetRotation);
									return;
								}
							}
							return;
						}
					}
					return;
				}
			}
			return;
		}
		case 2:
		{
			if (data.Length != 2)
			{
				return;
			}
			int netId2;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId2))
			{
				return;
			}
			int rpcID;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID))
			{
				return;
			}
			GameEntity gameEntityFromNetId2 = this.gameEntityManager.GetGameEntityFromNetId(netId2);
			if (!gameEntityFromNetId2)
			{
				return;
			}
			SIGadget component2 = gameEntityFromNetId2.GetComponent<SIGadget>();
			if (component2)
			{
				component2.ProcessClientToClientRPC(info, rpcID, null);
				return;
			}
			break;
		}
		case 3:
		{
			if (data.Length != 3)
			{
				return;
			}
			int netId3;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out netId3))
			{
				return;
			}
			int rpcID2;
			if (!GameEntityManager.ValidateDataType<int>(data[1], out rpcID2))
			{
				return;
			}
			object[] data2;
			if (!GameEntityManager.ValidateDataType<object[]>(data[2], out data2))
			{
				return;
			}
			GameEntity gameEntityFromNetId3 = this.gameEntityManager.GetGameEntityFromNetId(netId3);
			if (!gameEntityFromNetId3)
			{
				return;
			}
			SIGadget component3 = gameEntityFromNetId3.GetComponent<SIGadget>();
			if (component3)
			{
				component3.ProcessClientToClientRPC(info, rpcID2, data2);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00038424 File Offset: 0x00036624
	[ContextMenu("Spawn Debug Object")]
	private void TestSpawnGadget()
	{
		this.testSpawner.Spawn(this.gameEntityManager);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00038437 File Offset: 0x00036637
	public IEnumerable<GameEntity> GetFactoryItems()
	{
		return this.techTreeSO.SpawnableEntities;
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00038444 File Offset: 0x00036644
	private void OnEntityRemoved(GameEntity entity)
	{
		SIGadget sigadget;
		entity.TryGetComponent<SIGadget>(out sigadget);
		if (this.zoneSuperInfection != null && sigadget != null)
		{
			this.zoneSuperInfection.RemoveGadget(sigadget);
		}
		if (sigadget == null)
		{
			return;
		}
		SIPlayer siplayer = SIPlayer.Get((int)(entity.createData & (long)((ulong)-1)));
		if (siplayer != null && siplayer.activePlayerGadgets.Contains(entity.GetNetId()))
		{
			siplayer.activePlayerGadgets.Remove(entity.GetNetId());
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x000384C5 File Offset: 0x000366C5
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		if (entity.GetComponent<SIGadget>() == null)
		{
			return createData;
		}
		return (createData & -4294967296L) | ((long)SIPlayer.LocalPlayer.ActorNr & (long)((ulong)-1));
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x000384F4 File Offset: 0x000366F4
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		if (this.techTreeSO.IsSpawnableEntityTypeId(entityTypeId) && !SuperInfectionManager.IsSuperGameMode())
		{
			return false;
		}
		SIPlayer.Get(actorNr);
		if ((createData & -9223372036854775808L) != 0L)
		{
			return false;
		}
		GameObject gameObject = this.gameEntityManager.FactoryPrefabById(entityTypeId);
		if (gameObject == null)
		{
			return false;
		}
		if (gameObject.GetComponent<SIGadget>() == null)
		{
			return false;
		}
		SIPlayer siplayer = SIPlayer.Get(actorNr);
		if (siplayer == null)
		{
			return false;
		}
		SIPlayer y = SIPlayer.Get((int)(createData & (long)((ulong)-1)));
		if (siplayer != y)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < siplayer.activePlayerGadgets.Count; i++)
		{
			GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]);
			if (((gameEntityFromNetId != null) ? gameEntityFromNetId.GetComponent<SIGadget>() : null) != null)
			{
				num++;
			}
		}
		if (num > siplayer.TotalGadgetLimit)
		{
			return false;
		}
		SIUpgradeType siupgradeType;
		if (this.techTreeSO.TryGetUpgradeTypeByEntityTypeId(entityTypeId, out siupgradeType))
		{
			bool flag = siplayer.CurrentProgression.IsUnlocked(siupgradeType);
			bool flag2 = SuperInfectionManager._ValidatePlayerHasGadgetUpgrades(createData, siplayer, siupgradeType);
			new SIUpgradeSet((int)((createData & 9223372032559808512L) >> 32));
			siplayer.GetUpgrades((SITechTreePageId)siupgradeType.GetPageId());
			if (!flag || !flag2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount)
	{
		return false;
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00038634 File Offset: 0x00036834
	public bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		this.gameEntityManager.IsAuthority();
		if (this.techTreeSO.IsSpawnableEntityTypeId(entityTypeId) && !SuperInfectionManager.IsSuperGameMode())
		{
			return false;
		}
		SIUpgradeType siupgradeType;
		if (!this.techTreeSO.TryGetUpgradeTypeByEntityTypeId(entityTypeId, out siupgradeType))
		{
			return true;
		}
		if ((createData & -9223372036854775808L) != 0L)
		{
			return this.HasActiveTryOnDispenser;
		}
		SIPlayer siplayer = SIPlayer.Get((int)(createData & (long)((ulong)-1)));
		if (siplayer == null)
		{
			return false;
		}
		bool flag = siplayer.CurrentProgression.IsUnlocked(siupgradeType);
		bool flag2 = SuperInfectionManager._ValidatePlayerHasGadgetUpgrades(createData, siplayer, siupgradeType);
		if (!flag || !flag2)
		{
			new SIUpgradeSet((int)((createData & 9223372032559808512L) >> 32));
			siplayer.GetUpgrades((SITechTreePageId)siupgradeType.GetPageId());
		}
		return flag && flag2;
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x000386E8 File Offset: 0x000368E8
	private static bool _ValidatePlayerHasGadgetUpgrades(long createData, SIPlayer siPlayer, SIUpgradeType upgradeType)
	{
		SIUpgradeSet siupgradeSet = new SIUpgradeSet((int)((createData & 9223372032559808512L) >> 32));
		SIUpgradeSet upgrades = siPlayer.GetUpgrades((SITechTreePageId)upgradeType.GetPageId());
		return (siupgradeSet.GetBits() & ~upgrades.GetBits()) == 0;
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ValidateCreateItemBatchSize(int size)
	{
		return true;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0003872C File Offset: 0x0003692C
	public void ClearPlayerGadgets(SIPlayer siPlayer)
	{
		for (int i = siPlayer.activePlayerGadgets.Count - 1; i >= 0; i--)
		{
			if (i < siPlayer.activePlayerGadgets.Count && siPlayer.activePlayerGadgets[i] >= 0)
			{
				GameEntity gameEntityFromNetId = this.gameEntityManager.GetGameEntityFromNetId(siPlayer.activePlayerGadgets[i]);
				if (!(gameEntityFromNetId == null) && !(gameEntityFromNetId.id == GameEntityId.Invalid))
				{
					this.gameEntityManager.RequestDestroyItem(gameEntityFromNetId.id);
				}
			}
		}
		siPlayer.activePlayerGadgets.Clear();
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x000387F4 File Offset: 0x000369F4
	[CompilerGenerated]
	private void <OnZoneInit>g__WhenReady|50_0()
	{
		this.progression.OnClientReady -= this.<OnZoneInit>g__WhenReady|50_0;
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x04000C70 RID: 3184
	private const string preLog = "[GT/SuperInfectionManager]  ";

	// Token: 0x04000C71 RID: 3185
	private const string preErr = "[GT/SuperInfectionManager]  ERROR!!!  ";

	// Token: 0x04000C72 RID: 3186
	public GameEntityManager gameEntityManager;

	// Token: 0x04000C73 RID: 3187
	public TestSpawnGadget testSpawner;

	// Token: 0x04000C74 RID: 3188
	public PhotonView photonView;

	// Token: 0x04000C75 RID: 3189
	public XSceneRef zoneSuperInfectionRef;

	// Token: 0x04000C76 RID: 3190
	[NonSerialized]
	public SuperInfection zoneSuperInfection;

	// Token: 0x04000C77 RID: 3191
	[SerializeField]
	private SITechTreeSO techTreeSO;

	// Token: 0x04000C78 RID: 3192
	[SerializeField]
	private SIProgression progression;

	// Token: 0x04000C79 RID: 3193
	[DebugReadout]
	public static SuperInfectionManager activeSuperInfectionManager;

	// Token: 0x04000C7A RID: 3194
	public static Dictionary<GTZone, SuperInfectionManager> siManagerByZone = new Dictionary<GTZone, SuperInfectionManager>();

	// Token: 0x04000C7B RID: 3195
	private static List<VRRig> tempRigs = new List<VRRig>(20);

	// Token: 0x04000C7C RID: 3196
	private static List<VRRig> tempRigs2 = new List<VRRig>(20);

	// Token: 0x04000C7D RID: 3197
	private readonly Dictionary<SnapJointType, List<SuperInfectionSnapPoint>> allSnapPoints = new Dictionary<SnapJointType, List<SuperInfectionSnapPoint>>();

	// Token: 0x04000C7E RID: 3198
	private const float rpcProximityCheckRange = 3f;

	// Token: 0x04000C7F RID: 3199
	private bool PendingZoneInit;

	// Token: 0x04000C80 RID: 3200
	private int tryOnDispenserCount;

	// Token: 0x04000C81 RID: 3201
	private const int roomFXTypeCount = 5;

	// Token: 0x02000182 RID: 386
	public enum ClientToAuthorityRPC
	{
		// Token: 0x04000C83 RID: 3203
		CombinedTerminalButtonPress,
		// Token: 0x04000C84 RID: 3204
		CombinedTerminalHandScan,
		// Token: 0x04000C85 RID: 3205
		ResourceDepositDeposited,
		// Token: 0x04000C86 RID: 3206
		CallEntityRPC,
		// Token: 0x04000C87 RID: 3207
		CallEntityRPCData,
		// Token: 0x04000C88 RID: 3208
		RequestStartRoomFX
	}

	// Token: 0x02000183 RID: 387
	public enum RoomFXType
	{
		// Token: 0x04000C8A RID: 3210
		Underwater,
		// Token: 0x04000C8B RID: 3211
		LunarMode,
		// Token: 0x04000C8C RID: 3212
		ConstLowG,
		// Token: 0x04000C8D RID: 3213
		Bouncy,
		// Token: 0x04000C8E RID: 3214
		Supercharge
	}

	// Token: 0x02000184 RID: 388
	public enum AuthorityToClientRPC
	{
		// Token: 0x04000C90 RID: 3216
		TechPointGranted,
		// Token: 0x04000C91 RID: 3217
		ResourceDepositTechPointGranted,
		// Token: 0x04000C92 RID: 3218
		ResourceDepositTechPointRejected,
		// Token: 0x04000C93 RID: 3219
		CallEntityRPC,
		// Token: 0x04000C94 RID: 3220
		CallEntityRPCData,
		// Token: 0x04000C95 RID: 3221
		TriggerMonkeIdolDepositCelebration,
		// Token: 0x04000C96 RID: 3222
		StartRoomFX
	}

	// Token: 0x02000185 RID: 389
	public enum ClientToClientRPC
	{
		// Token: 0x04000C98 RID: 3224
		BroadcastProgression,
		// Token: 0x04000C99 RID: 3225
		LaunchDashYoyo,
		// Token: 0x04000C9A RID: 3226
		CallEntityRPC,
		// Token: 0x04000C9B RID: 3227
		CallEntityRPCData
	}
}
