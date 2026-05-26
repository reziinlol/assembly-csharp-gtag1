using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// Token: 0x020006BF RID: 1727
[NetworkBehaviourWeaved(0)]
public class GameEntityManager : NetworkComponent, IRequestableOwnershipGuardCallbacks, ITickSystemTick, IGorillaSliceableSimple
{
	// Token: 0x14000057 RID: 87
	// (add) Token: 0x06002B15 RID: 11029 RVA: 0x000E6074 File Offset: 0x000E4274
	// (remove) Token: 0x06002B16 RID: 11030 RVA: 0x000E60AC File Offset: 0x000E42AC
	public event GameEntityManager.ZoneStartEvent onZoneStart;

	// Token: 0x14000058 RID: 88
	// (add) Token: 0x06002B17 RID: 11031 RVA: 0x000E60E4 File Offset: 0x000E42E4
	// (remove) Token: 0x06002B18 RID: 11032 RVA: 0x000E611C File Offset: 0x000E431C
	public event GameEntityManager.ZoneClearEvent onZoneClear;

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002B19 RID: 11033 RVA: 0x000E6151 File Offset: 0x000E4351
	// (set) Token: 0x06002B1A RID: 11034 RVA: 0x000E6158 File Offset: 0x000E4358
	public static GameEntityManager activeManager { get; private set; }

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06002B1B RID: 11035 RVA: 0x000E6160 File Offset: 0x000E4360
	// (set) Token: 0x06002B1C RID: 11036 RVA: 0x000E6168 File Offset: 0x000E4368
	public bool TickRunning { get; set; }

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x06002B1D RID: 11037 RVA: 0x000E6171 File Offset: 0x000E4371
	// (set) Token: 0x06002B1E RID: 11038 RVA: 0x000E6179 File Offset: 0x000E4379
	public bool PendingTableData { get; private set; }

	// Token: 0x06002B1F RID: 11039 RVA: 0x000E6184 File Offset: 0x000E4384
	protected override void Awake()
	{
		base.Awake();
		this.entities = new List<GameEntity>(64);
		this.entitiesActiveCount = 0;
		this.gameEntityData = new List<GameEntityData>(64);
		this.netIdToIndex = new Dictionary<int, int>(16384);
		this.netIds = new NativeArray<int>(16384, Unity.Collections.Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.createdItemTypeCount = new Dictionary<int, int>();
		this.OnEntityRemoved = (Action<GameEntity>)Delegate.Combine(this.OnEntityRemoved, new Action<GameEntity>(CustomGameMode.OnGameEntityRemoved));
		this.zoneStateData = new GameEntityManager.ZoneStateData
		{
			zoneStateRequests = new List<GameEntityManager.ZoneStateRequest>(),
			zonePlayers = new List<Player>(),
			recievedStateBytes = new byte[15360],
			numRecievedStateBytes = 0
		};
		this.guard.AddCallbackTarget(this);
		this.netIdsForCreate = new List<int>();
		this.entityTypeIdsForCreate = new List<int>();
		this.packedPositionsForCreate = new List<long>();
		this.packedRotationsForCreate = new List<int>();
		this.createDataForCreate = new List<long>();
		this.createdByEntityNetIdForCreate = new List<int>();
		this.netIdsForDelete = new List<int>();
		this.netIdsForState = new List<int>();
		this.statesForState = new List<long>();
		this.zoneComponents = new List<IGameEntityZoneComponent>(8);
		if (this.ghostReactorManager != null)
		{
			this.zoneComponents.Add(this.ghostReactorManager);
		}
		if (this.customMapsManager != null)
		{
			this.zoneComponents.Add(this.customMapsManager);
		}
		if (this.superInfectionManager != null)
		{
			this.zoneComponents.Add(this.superInfectionManager);
		}
		this.BuildFactory();
		GameEntityManager.allManagers.Add(this);
		GameEntityManager.managersByZone[(int)this.zone] = this;
		if (base.transform.parent != null)
		{
			base.transform.SetParent(null, true);
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x000E6364 File Offset: 0x000E4564
	internal void RegisterScenePlacedEntities()
	{
		if (this.scenePlacedEntitiesRegistered)
		{
			return;
		}
		this.scenePlacedEntitiesRegistered = true;
		string zoneSceneName = this.GetZoneSceneName();
		List<GameEntity> list;
		GameEntityManager.s_scenePlacedEntities.TryGetValue(zoneSceneName, out list);
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.RegisterSingleScenePlacedEntity(list[i]);
		}
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x000E63B8 File Offset: 0x000E45B8
	private void RegisterSingleScenePlacedEntity(GameEntity entity)
	{
		if (entity == null)
		{
			return;
		}
		XSceneRefTarget xsceneRefTarget;
		int num;
		if (entity.TryGetComponent<XSceneRefTarget>(out xsceneRefTarget) && xsceneRefTarget.UniqueID > 0)
		{
			num = GameEntityManager.NetIdFromXSceneRefId(xsceneRefTarget.UniqueID);
		}
		else
		{
			num = GameEntityManager.ComputeNetIdFromHierarchyForCustomMaps(entity.transform);
		}
		int num2;
		if (this.netIdToIndex.TryGetValue(num, out num2))
		{
			GameEntity gameEntity = (num2 >= 0 && num2 < this.entities.Count) ? this.entities[num2] : null;
			if (gameEntity == entity)
			{
				this.EnsureScenePlacedRecord(entity);
				return;
			}
			if (!(gameEntity == null))
			{
				Debug.LogError("[GT/GameEntityManager]  ERROR!!!  RegisterSingleScenePlacedEntity" + string.Format(": NetId collision for scene-placed entity '{0}' (netId={1})", entity.name, num) + string.Format(" with live entity '{0}' at index {1}. Skipping.", gameEntity.name, num2));
				return;
			}
			this.netIdToIndex.Remove(num);
		}
		if (!entity.scenePlacedInitialized)
		{
			entity.scenePlacedHomePosition = entity.transform.position;
			entity.scenePlacedHomeRotation = entity.transform.rotation;
			entity.scenePlacedHomeScale = entity.transform.lossyScale.x;
			if (!entity.gameObject.activeSelf)
			{
				entity.gameObject.SetActive(true);
			}
			entity.IsScenePlaced = true;
			entity.Create(this, num, -2147483647);
			entity.Init(0L, -1);
			this.AddGameEntity(num, entity);
			entity.scenePlacedInitialized = true;
		}
		else
		{
			GameEntityManager manager = entity.manager;
			if (manager != null && manager != this)
			{
				manager.RemoveGameEntity(entity);
				if (entity.builtInEntities != null)
				{
					for (int i = 0; i < entity.builtInEntities.Count; i++)
					{
						manager.RemoveGameEntity(entity.builtInEntities[i]);
					}
				}
			}
			entity.manager = this;
			this.AddGameEntity(num, entity);
			if (entity.builtInEntities != null)
			{
				bool flag = num < -1 && num != int.MinValue;
				for (int j = 0; j < entity.builtInEntities.Count; j++)
				{
					int netId = flag ? (num - 1 - j) : (num + 1 + j);
					entity.builtInEntities[j].manager = this;
					this.AddGameEntity(netId, entity.builtInEntities[j]);
				}
			}
			if (!entity.gameObject.activeSelf)
			{
				entity.gameObject.SetActive(true);
			}
		}
		GameEntityManager.s_scenePlacedHomeScenes[num] = entity.gameObject.scene.name;
		this.EnsureScenePlacedRecord(entity);
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x000E6640 File Offset: 0x000E4840
	private void EnsureScenePlacedRecord(GameEntity entity)
	{
		for (int i = 0; i < this.scenePlacedEntities.Count; i++)
		{
			if (this.scenePlacedEntities[i].entity == entity)
			{
				return;
			}
		}
		this.scenePlacedEntities.Add(new GameEntityManager.ScenePlacedRecord
		{
			entity = entity,
			position = entity.scenePlacedHomePosition,
			rotation = entity.scenePlacedHomeRotation,
			uniformScale = entity.scenePlacedHomeScale
		});
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x000E66C0 File Offset: 0x000E48C0
	private static void ResetScenePlacedTransform(GameEntity entity, in GameEntityManager.ScenePlacedRecord record)
	{
		bool flag = entity.transform.parent != null;
		bool flag2 = entity.IsHeld() || entity.snappedByActorNumber != -1;
		if (flag || flag2)
		{
			if (entity.manager != null)
			{
				entity.manager.ReleaseScenePlacedHold(entity);
			}
			else
			{
				GameEntityManager.DetachScenePlacedFromRig(entity);
			}
		}
		entity.transform.SetPositionAndRotation(record.position, record.rotation);
		entity.transform.localScale = Vector3.one * record.uniformScale;
		Rigidbody component = entity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.linearVelocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000E6774 File Offset: 0x000E4974
	internal void ReleaseScenePlacedHold(GameEntity entity)
	{
		if (entity == null || !entity.IsScenePlaced)
		{
			return;
		}
		int heldByActorNumber = entity.heldByActorNumber;
		int snappedByActorNumber = entity.snappedByActorNumber;
		GamePlayer gamePlayer;
		if (heldByActorNumber != -1 && GamePlayer.TryGetGamePlayer(heldByActorNumber, out gamePlayer))
		{
			gamePlayer.ClearGrabbedIfHeld(entity.id, this);
			if (gamePlayer.IsLocal())
			{
				GamePlayerLocal instance = GamePlayerLocal.instance;
				if (instance != null)
				{
					instance.ClearGrabbedIfHeld(entity.id, this);
				}
			}
		}
		GamePlayer gamePlayer2;
		if (snappedByActorNumber != -1 && GamePlayer.TryGetGamePlayer(snappedByActorNumber, out gamePlayer2))
		{
			gamePlayer2.ClearSnappedIfSnapped(entity.id, this);
		}
		GameEntityManager.DetachScenePlacedFromRig(entity);
		GameEntityManager.MoveScenePlacedToHomeScene(entity);
		bool flag = entity.heldByActorNumber != -1 || entity.snappedByActorNumber != -1 || entity.attachedToEntityId != GameEntityId.Invalid;
		entity.heldByActorNumber = -1;
		entity.heldByHandIndex = -1;
		entity.snappedByActorNumber = -1;
		entity.snappedJoint = SnapJointType.None;
		entity.attachedToEntityId = GameEntityId.Invalid;
		if (flag)
		{
			Action onReleased = entity.OnReleased;
			if (onReleased == null)
			{
				return;
			}
			onReleased();
		}
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x000E6864 File Offset: 0x000E4A64
	private static void DetachScenePlacedFromRig(GameEntity entity)
	{
		Transform parent = entity.transform.parent;
		if (parent == null)
		{
			return;
		}
		if (parent.GetComponentInParent<VRRig>() != null)
		{
			entity.transform.SetParent(null, true);
		}
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x000E68A4 File Offset: 0x000E4AA4
	private static void MoveScenePlacedToHomeScene(GameEntity entity)
	{
		if (entity == null || !entity.IsScenePlaced || entity.manager == null)
		{
			return;
		}
		string name;
		if (!GameEntityManager.s_scenePlacedHomeScenes.TryGetValue(entity.GetNetId(), out name))
		{
			return;
		}
		Scene sceneByName = SceneManager.GetSceneByName(name);
		if (!sceneByName.IsValid() || !sceneByName.isLoaded)
		{
			return;
		}
		if (entity.gameObject.scene == sceneByName)
		{
			return;
		}
		SceneManager.MoveGameObjectToScene(entity.gameObject, sceneByName);
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x000E6920 File Offset: 0x000E4B20
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
		VRRigCache.OnRigDeactivated += this.OnRigDeactivated;
		VRRigCache.OnActiveRigsChanged += this.RefreshRigList;
		RoomSystem.JoinedRoomEvent += new Action(this.OnNetworkJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnNetworkLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnNetworkPlayerLeft);
		this.RefreshRigList();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x000E69C0 File Offset: 0x000E4BC0
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
		VRRigCache.OnRigDeactivated -= this.OnRigDeactivated;
		VRRigCache.OnActiveRigsChanged -= this.RefreshRigList;
		RoomSystem.JoinedRoomEvent -= new Action(this.OnNetworkJoinedRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnNetworkLeftRoom);
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnNetworkPlayerLeft);
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x000E6A5C File Offset: 0x000E4C5C
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.netIds.Dispose();
		GameEntityManager.allManagers.Remove(this);
		GameEntityManager x;
		if (GameEntityManager.managersByZone.TryGetValue((int)this.zone, out x) && x == this)
		{
			GameEntityManager.managersByZone.Remove((int)this.zone);
		}
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x000E6AB4 File Offset: 0x000E4CB4
	public static GameEntityManager GetManagerForZone(GTZone zone)
	{
		for (int i = 0; i < GameEntityManager.allManagers.Count; i++)
		{
			if (GameEntityManager.allManagers[i].zone == zone)
			{
				return GameEntityManager.allManagers[i];
			}
		}
		return null;
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SliceUpdate()
	{
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x000E6AF8 File Offset: 0x000E4CF8
	public void Tick()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.UpdateZoneState();
		if (!this.IsZoneActive())
		{
			if (this.netIdsForCreate.Count > 0 || this.netIdsForDelete.Count > 0 || this.netIdsForState.Count > 0)
			{
				this.ClearPendingRPCBatches();
			}
			if (this.PendingTableData)
			{
				if (Time.frameCount - this.pendingTableDataSetFrame > 90)
				{
					this.ResolveTableData();
					return;
				}
				int num = Time.frameCount % 300;
			}
			return;
		}
		float time = Time.time;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (gameEntity != null && gameEntity.LastTickTime + gameEntity.MinTimeBetweenTicks < time && gameEntity.isActiveAndEnabled)
			{
				Action onTick = gameEntity.OnTick;
				if (onTick != null)
				{
					onTick();
				}
				gameEntity.LastTickTime = time;
			}
		}
		if (!this.IsAuthority())
		{
			return;
		}
		if (this.scenePlacedEntities.Count > 0)
		{
			this.scenePlacedBoundsCheckTimer -= Time.deltaTime;
			if (this.scenePlacedBoundsCheckTimer <= 0f)
			{
				this.scenePlacedBoundsCheckTimer = 1f;
				for (int j = 0; j < this.scenePlacedEntities.Count; j++)
				{
					GameEntityManager.ScenePlacedRecord scenePlacedRecord = this.scenePlacedEntities[j];
					if (!(scenePlacedRecord.entity == null))
					{
						Vector3 position = scenePlacedRecord.entity.transform.position;
						if ((position - scenePlacedRecord.position).sqrMagnitude >= 0.0625f && !this.IsPositionInManagerBounds(position))
						{
							GameEntityManager.ResetScenePlacedTransform(scenePlacedRecord.entity, scenePlacedRecord);
						}
					}
				}
			}
		}
		if (this.netIdsForCreate.Count > 0 && Time.time > this.lastCreateSent + this.createCooldown)
		{
			this.lastCreateSent = Time.time;
			this.photonView.RPC("CreateItemRPC", RpcTarget.Others, new object[]
			{
				this.netIdsForCreate.ToArray(),
				this.entityTypeIdsForCreate.ToArray(),
				this.packedPositionsForCreate.ToArray(),
				this.packedRotationsForCreate.ToArray(),
				this.createDataForCreate.ToArray(),
				this.createdByEntityNetIdForCreate.ToArray()
			});
			this.netIdsForCreate.Clear();
			this.entityTypeIdsForCreate.Clear();
			this.packedPositionsForCreate.Clear();
			this.packedRotationsForCreate.Clear();
			this.createDataForCreate.Clear();
			this.createdByEntityNetIdForCreate.Clear();
		}
		if (this.netIdsForDelete.Count > 0 && Time.time > this.lastDestroySent + this.destroyCooldown)
		{
			this.lastDestroySent = Time.time;
			this.photonView.RPC("DestroyItemRPC", RpcTarget.Others, new object[]
			{
				this.netIdsForDelete.ToArray()
			});
			this.netIdsForDelete.Clear();
		}
		if (this.netIdsForState.Count > 0 && Time.time > this.lastStateSent + this.stateCooldown)
		{
			this.lastStateSent = Time.time;
			this.photonView.RPC("ApplyStateRPC", RpcTarget.All, new object[]
			{
				this.netIdsForState.ToArray(),
				this.statesForState.ToArray()
			});
			this.netIdsForState.Clear();
			this.statesForState.Clear();
		}
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x000E6E4F File Offset: 0x000E504F
	public GameEntityId AddGameEntity(GameEntity gameEntity)
	{
		return this.AddGameEntity(this.CreateNetId(1 + gameEntity.builtInEntities.Count), gameEntity);
	}

	// Token: 0x06002B2E RID: 11054 RVA: 0x000E6E6C File Offset: 0x000E506C
	public GameEntityId AddGameEntity(int netId, GameEntity gameEntity)
	{
		if (netId == -1)
		{
			Debug.LogError("[GT/GameEntityManager]  ERROR!!!  AddGameEntity: Aborting. Invalid netId for GameEntity '" + ((gameEntity != null) ? gameEntity.name : null) + "'.", gameEntity);
			return GameEntityId.Invalid;
		}
		int num;
		if (this.netIdToIndex.TryGetValue(netId, out num) && num != -1)
		{
			GameEntity gameEntity2 = this.GetGameEntity(num);
			if (gameEntity2 != null)
			{
				if (gameEntity2 == gameEntity)
				{
					return gameEntity.id;
				}
				Debug.LogError(string.Concat(new string[]
				{
					"[GT/GameEntityManager]  ERROR!!!  AddGameEntity",
					string.Format(": NetId {0} collision: ", netId),
					"'",
					gameEntity2.name,
					"' replaced by '",
					gameEntity.name,
					"'. Destroying old entity to prevent zombie."
				}));
				this.DestroyItemLocal(gameEntity2.id);
			}
		}
		int num2 = this.FindNewEntityIndex();
		this.entities[num2] = gameEntity;
		this.entitiesActiveCount++;
		GameEntityData item = default(GameEntityData);
		this.gameEntityData.Add(item);
		gameEntity.id = new GameEntityId
		{
			index = num2
		};
		this.netIdToIndex[netId] = num2;
		this.netIds[num2] = netId;
		Action<GameEntity> onEntityAdded = this.OnEntityAdded;
		if (onEntityAdded != null)
		{
			onEntityAdded(gameEntity);
		}
		return gameEntity.id;
	}

	// Token: 0x06002B2F RID: 11055 RVA: 0x000E6FC0 File Offset: 0x000E51C0
	private int FindNewEntityIndex()
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] == null)
			{
				return i;
			}
		}
		this.entities.Add(null);
		return this.entities.Count - 1;
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x000E7014 File Offset: 0x000E5214
	public void RemoveGameEntity(GameEntity entity)
	{
		this.netIdToIndex.Remove(entity.GetNetId());
		int index = entity.id.index;
		if (index < 0 || index >= this.entities.Count)
		{
			return;
		}
		if (this.entities[index] == entity)
		{
			this.entities[index] = null;
			this.entitiesActiveCount--;
		}
		else
		{
			for (int i = 0; i < this.entities.Count; i++)
			{
				if (this.entities[i] == entity)
				{
					this.entities[i] = null;
					this.entitiesActiveCount--;
					break;
				}
			}
		}
		Action<GameEntity> onEntityRemoved = this.OnEntityRemoved;
		if (onEntityRemoved == null)
		{
			return;
		}
		onEntityRemoved(entity);
	}

	// Token: 0x06002B31 RID: 11057 RVA: 0x000E70DB File Offset: 0x000E52DB
	public List<GameEntity> GetGameEntities()
	{
		return this.entities;
	}

	// Token: 0x06002B32 RID: 11058 RVA: 0x000E70E4 File Offset: 0x000E52E4
	public bool IsValidNetId(int netId)
	{
		int num;
		return this.netIdToIndex.TryGetValue(netId, out num) && num >= 0 && num < this.entities.Count;
	}

	// Token: 0x06002B33 RID: 11059 RVA: 0x000E7118 File Offset: 0x000E5318
	public int FindOpenIndex()
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (this.netIds[i] != -1)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x000E7150 File Offset: 0x000E5350
	public GameEntityId GetEntityIdFromNetId(int netId)
	{
		int index;
		if (this.netIdToIndex.TryGetValue(netId, out index))
		{
			return new GameEntityId
			{
				index = index
			};
		}
		return GameEntityId.Invalid;
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x000E7184 File Offset: 0x000E5384
	public int GetNetIdFromEntityId(GameEntityId id)
	{
		if (id.index < 0 || id.index >= this.netIds.Length)
		{
			return -1;
		}
		return this.netIds[id.index];
	}

	// Token: 0x06002B36 RID: 11062 RVA: 0x000E71B8 File Offset: 0x000E53B8
	private void ClearPendingRPCBatches()
	{
		this.netIdsForCreate.Clear();
		this.entityTypeIdsForCreate.Clear();
		this.packedPositionsForCreate.Clear();
		this.packedRotationsForCreate.Clear();
		this.createDataForCreate.Clear();
		this.createdByEntityNetIdForCreate.Clear();
		this.netIdsForDelete.Clear();
		this.netIdsForState.Clear();
		this.statesForState.Clear();
	}

	// Token: 0x06002B37 RID: 11063 RVA: 0x000E7228 File Offset: 0x000E5428
	public virtual bool IsAuthority()
	{
		return !NetworkSystem.Instance.InRoom || this.guard.isTrulyMine;
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000E7243 File Offset: 0x000E5443
	public bool IsAuthorityPlayer(NetPlayer player)
	{
		return player != null && this.IsAuthorityPlayer(player.GetPlayerRef());
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x000E7258 File Offset: 0x000E5458
	public bool IsAuthorityPlayer(Player player)
	{
		if (player != null && this.guard.actualOwner != null)
		{
			int actorNumber = player.ActorNumber;
			Player playerRef = this.guard.actualOwner.GetPlayerRef();
			int? num = (playerRef != null) ? new int?(playerRef.ActorNumber) : null;
			return actorNumber == num.GetValueOrDefault() & num != null;
		}
		return false;
	}

	// Token: 0x06002B3A RID: 11066 RVA: 0x000E72B8 File Offset: 0x000E54B8
	public bool IsZoneAuthority()
	{
		return this.IsAuthority();
	}

	// Token: 0x06002B3B RID: 11067 RVA: 0x000E72C0 File Offset: 0x000E54C0
	public bool HasAuthority()
	{
		return this.GetAuthorityPlayer() != null;
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x000E72CB File Offset: 0x000E54CB
	public Player GetAuthorityPlayer()
	{
		if (this.guard.actualOwner != null)
		{
			return this.guard.actualOwner.GetPlayerRef();
		}
		return null;
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x000E72EC File Offset: 0x000E54EC
	public virtual bool IsZoneActive()
	{
		if (GorillaComputer.instance != null && GorillaComputer.instance.IsPlayerInVirtualStump() && GameEntityManager.IsSuppressZonesInVStumpEnabled())
		{
			return CustomMapLoader.CanLoadEntities && this.zone == GTZone.customMaps && this.zoneStateData.state == GameEntityManager.ZoneState.Active;
		}
		return this.zoneStateData.state == GameEntityManager.ZoneState.Active;
	}

	// Token: 0x06002B3E RID: 11070 RVA: 0x000E7350 File Offset: 0x000E5550
	private static bool IsSuppressZonesInVStumpEnabled()
	{
		GorillaServer instance = GorillaServer.Instance;
		return instance != null && instance.CheckIsSuppressZonesInVStumpEnabled();
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x000E7378 File Offset: 0x000E5578
	public virtual bool IsPositionInManagerBounds(Vector3 pos)
	{
		bool result;
		if (this.boundsBoxCollider != null)
		{
			result = this.boundsBoxCollider.bounds.Contains(pos);
		}
		else
		{
			ZoneGraphBSP instance = ZoneGraphBSP.Instance;
			if (instance != null && instance.HasCompiledTree())
			{
				ZoneDef zoneDef = instance.FindZoneAtPoint(pos);
				result = (zoneDef != null && zoneDef.zoneId == this.zone);
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x000E73EC File Offset: 0x000E55EC
	public virtual bool IsValidClientRPC(Player sender)
	{
		bool flag = this.IsAuthorityPlayer(sender);
		bool flag2 = this.IsZoneActive();
		bool flag3 = sender.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		return flag && (flag2 || flag3);
	}

	// Token: 0x06002B41 RID: 11073 RVA: 0x000E7421 File Offset: 0x000E5621
	public bool IsValidClientRPC(Player sender, int entityNetId)
	{
		return this.IsValidClientRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x06002B42 RID: 11074 RVA: 0x000E7435 File Offset: 0x000E5635
	public bool IsValidClientRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidClientRPC(sender, entityNetId) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x000E744A File Offset: 0x000E564A
	public bool IsValidClientRPC(Player sender, Vector3 pos)
	{
		return this.IsValidClientRPC(sender) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x000E745E File Offset: 0x000E565E
	public bool IsValidAuthorityRPC(Player sender)
	{
		return this.IsAuthority() && (this.IsZoneActive() || sender.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x000E7486 File Offset: 0x000E5686
	public bool IsValidAuthorityRPC(Player sender, int entityNetId)
	{
		return this.IsValidAuthorityRPC(sender) && this.IsValidNetId(entityNetId);
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x000E749A File Offset: 0x000E569A
	public bool IsValidAuthorityRPC(Player sender, int entityNetId, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(sender, entityNetId) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002B47 RID: 11079 RVA: 0x000E74AF File Offset: 0x000E56AF
	public bool IsValidAuthorityRPC(Player sender, Vector3 pos)
	{
		return this.IsValidAuthorityRPC(sender) && this.IsPositionInManagerBounds(pos);
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000E74C3 File Offset: 0x000E56C3
	public bool IsValidEntity(GameEntityId id)
	{
		return this.GetGameEntity(id) != null;
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x000E74D2 File Offset: 0x000E56D2
	public GameEntity GetGameEntity(GameEntityId id)
	{
		if (!id.IsValid())
		{
			return null;
		}
		return this.GetGameEntity(id.index);
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x000E74EC File Offset: 0x000E56EC
	public GameEntity GetGameEntityFromNetId(int netId)
	{
		int index;
		if (this.netIdToIndex.TryGetValue(netId, out index))
		{
			return this.GetGameEntity(index);
		}
		return null;
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x000E7512 File Offset: 0x000E5712
	private GameEntity GetGameEntity(int index)
	{
		if (index == -1)
		{
			return null;
		}
		if (index < 0 || index >= this.entities.Count)
		{
			return null;
		}
		return this.entities[index];
	}

	// Token: 0x06002B4C RID: 11084 RVA: 0x000E753C File Offset: 0x000E573C
	public T GetGameComponent<T>(GameEntityId id) where T : Component
	{
		GameEntity gameEntity = this.GetGameEntity(id);
		if (gameEntity == null)
		{
			return default(T);
		}
		return gameEntity.GetComponent<T>();
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x000E756C File Offset: 0x000E576C
	public bool LocalValidateMigrationRecoveryItem(int entityTypeId, ref long createData)
	{
		GameObject gameObject = this.FactoryPrefabById(entityTypeId);
		if (gameObject == null)
		{
			return false;
		}
		GameEntity component = gameObject.GetComponent<GameEntity>();
		if (component != null)
		{
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				createData = this.zoneComponents[i].ProcessMigratedGameEntityCreateData(component, createData);
			}
		}
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		for (int j = 0; j < this.zoneComponents.Count; j++)
		{
			if (!this.zoneComponents[j].ValidateMigratedGameEntity(0, entityTypeId, Vector3.zero, Quaternion.identity, createData, actorNumber))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x000E7614 File Offset: 0x000E5814
	public bool IsEntityValidToMigrate(GameEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		Vector3 position = VRRig.LocalRig.transform.position;
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		bool flag = true;
		int num = 0;
		while (num < this.zoneComponents.Count && flag)
		{
			flag &= this.zoneComponents[num].ValidateMigratedGameEntity(this.GetNetIdFromEntityId(entity.id), entity.typeId, position, Quaternion.identity, entity.createData, actorNumber);
			num++;
		}
		return flag;
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x000E7698 File Offset: 0x000E5898
	private void BuildFactory()
	{
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder(true))
		{
			string value = "[GT/GameEntityManager]  BuildFactory: Entity names and typeIds for manager \"" + base.name + "\":";
			utf16ValueStringBuilder.AppendLine(value);
			foreach (IGameEntityZoneComponent gameEntityZoneComponent in this.zoneComponents)
			{
				IFactoryItemProvider factoryItemProvider = gameEntityZoneComponent as IFactoryItemProvider;
				if (factoryItemProvider != null)
				{
					foreach (GameEntity item in factoryItemProvider.GetFactoryItems())
					{
						if (!this.tempFactoryItems.Contains(item))
						{
							this.tempFactoryItems.Add(item);
						}
					}
				}
			}
			this.itemPrefabFactory = new Dictionary<int, GameObject>(1024);
			this.priceLookupByEntityId = new Dictionary<int, int>();
			for (int i = 0; i < this.tempFactoryItems.Count; i++)
			{
				GameObject gameObject = this.tempFactoryItems[i].gameObject;
				int staticHash = gameObject.name.GetStaticHash();
				if (gameObject.GetComponent<GRToolLantern>())
				{
					this.priceLookupByEntityId.Add(staticHash, 50);
				}
				else if (gameObject.GetComponent<GRToolCollector>())
				{
					this.priceLookupByEntityId.Add(staticHash, 50);
				}
				this.itemPrefabFactory.Add(staticHash, gameObject);
				utf16ValueStringBuilder.AppendFormat<string, int>("    - name=\"{0}\", typeId={1}\n", gameObject.name, staticHash);
				if (utf16ValueStringBuilder.Length > 5000)
				{
					utf16ValueStringBuilder.Append("... (continued in next log message) ...");
					utf16ValueStringBuilder.Clear();
					if (i + 1 < this.tempFactoryItems.Count)
					{
						utf16ValueStringBuilder.Append(value);
						utf16ValueStringBuilder.Append(" ... CONTINUED FROM PREVIOUS ...\n");
					}
				}
			}
		}
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x000E78AC File Offset: 0x000E5AAC
	private int CreateNetId(int numToCreate)
	{
		int result = this.nextNetId;
		this.nextNetId += numToCreate;
		return result;
	}

	// Token: 0x06002B51 RID: 11089 RVA: 0x000E78C4 File Offset: 0x000E5AC4
	private void RecalculateNextNetId()
	{
		int num = 0;
		for (int i = 0; i < this.entities.Count; i++)
		{
			if (this.entities[i] != null)
			{
				int num2 = this.netIds[i];
				if (num2 >= 0)
				{
					int num3 = num2 + this.entities[i].builtInEntities.Count;
					if (num3 > num)
					{
						num = num3;
					}
				}
			}
		}
		this.nextNetId = num + 1;
	}

	// Token: 0x06002B52 RID: 11090 RVA: 0x000E7936 File Offset: 0x000E5B36
	public GameEntityId RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData)
	{
		return this.RequestCreateItem(entityTypeId, position, rotation, createData, GameEntityId.Invalid);
	}

	// Token: 0x06002B53 RID: 11091 RVA: 0x000E7948 File Offset: 0x000E5B48
	public GameEntityId RequestCreateItem(int entityTypeId, Vector3 position, Quaternion rotation, long createData, GameEntityId createdByEntityId)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive() || !this.IsPositionInManagerBounds(position))
		{
			return GameEntityId.Invalid;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(createdByEntityId);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			if (!this.zoneComponents[i].ValidateCreateItem(0, entityTypeId, position, rotation, createData, netIdFromEntityId))
			{
				MonoBehaviour monoBehaviour = this.zoneComponents[i] as MonoBehaviour;
				if (monoBehaviour != null)
				{
					string name = monoBehaviour.name;
				}
				return GameEntityId.Invalid;
			}
		}
		long item = BitPackUtils.PackWorldPosForNetwork(position);
		int item2 = BitPackUtils.PackQuaternionForNetwork(rotation);
		int numToCreate = 1 + this.FactoryGetBuiltInEntityCountById(entityTypeId);
		int num = this.CreateNetId(numToCreate);
		this.netIdsForCreate.Add(num);
		this.entityTypeIdsForCreate.Add(entityTypeId);
		this.packedPositionsForCreate.Add(item);
		this.packedRotationsForCreate.Add(item2);
		this.createDataForCreate.Add(createData);
		this.createdByEntityNetIdForCreate.Add(netIdFromEntityId);
		return this.CreateAndInitItemLocal(num, entityTypeId, position, rotation, createData, netIdFromEntityId);
	}

	// Token: 0x06002B54 RID: 11092 RVA: 0x000E7A54 File Offset: 0x000E5C54
	[PunRPC]
	public void CreateItemRPC(int[] netId, int[] entityTypeId, long[] packedPos, int[] packedRot, long[] createData, int[] createdByEntityNetId, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItem))
		{
			return;
		}
		if (netId == null || entityTypeId == null || packedPos == null || createData == null || createdByEntityNetId == null || netId.Length != entityTypeId.Length || netId.Length != packedPos.Length || netId.Length != packedRot.Length || netId.Length != createData.Length || netId.Length != createdByEntityNetId.Length)
		{
			return;
		}
		if (netId.Length > 1)
		{
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				if (!this.zoneComponents[i].ValidateCreateItemBatchSize(netId.Length))
				{
					return;
				}
			}
		}
		for (int j = 0; j < netId.Length; j++)
		{
			if (!GameEntityManager.IsScenePlacedNetId(netId[j]))
			{
				Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos[j]);
				Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(packedRot[j]);
				float num = 10000f;
				if (!vector.IsValid(num) || !rotation.IsValid() || !this.FactoryHasEntity(entityTypeId[j]) || !this.IsPositionInManagerBounds(vector))
				{
					return;
				}
				int num2 = netId[j];
				int entityTypeId2 = entityTypeId[j];
				long createData2 = createData[j];
				int createdByEntityNetId2 = createdByEntityNetId[j];
				bool flag = true;
				for (int k = 0; k < this.zoneComponents.Count; k++)
				{
					if (!this.zoneComponents[k].ValidateCreateItem(num2, entityTypeId2, vector, rotation, createData2, createdByEntityNetId2))
					{
						flag = false;
					}
				}
				if (flag)
				{
					this.CreateAndInitItemLocal(num2, entityTypeId2, vector, rotation, createData2, createdByEntityNetId2);
				}
			}
		}
	}

	// Token: 0x06002B55 RID: 11093 RVA: 0x000E7BBC File Offset: 0x000E5DBC
	public void RequestCreateItems(List<GameEntityCreateData> entityData)
	{
		if (!this.IsZoneAuthority() || !this.IsZoneActive())
		{
			GTDev.LogError<string>(string.Format("[GameEntityManager::RequestCreateItems] Cannot create items. Zone Auth: {0} ", this.IsZoneAuthority()) + string.Format("| Zone Active: {0}", this.IsZoneActive()), null);
			return;
		}
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(this.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(entityData.Count);
		for (int i = 0; i < entityData.Count; i++)
		{
			GameEntityCreateData gameEntityCreateData = entityData[i];
			int numToCreate = 1 + this.FactoryGetBuiltInEntityCountById(gameEntityCreateData.entityTypeId);
			int value = this.CreateNetId(numToCreate);
			long value2 = BitPackUtils.PackWorldPosForNetwork(gameEntityCreateData.position);
			int value3 = BitPackUtils.PackQuaternionForNetwork(gameEntityCreateData.rotation);
			binaryWriter.Write(value);
			binaryWriter.Write(gameEntityCreateData.entityTypeId);
			binaryWriter.Write(value2);
			binaryWriter.Write(value3);
			binaryWriter.Write(gameEntityCreateData.createData);
			binaryWriter.Write(gameEntityCreateData.createdByEntityId);
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		this.photonView.RPC("CreateItemsRPC", RpcTarget.All, new object[]
		{
			(int)this.zone,
			array
		});
	}

	// Token: 0x06002B56 RID: 11094 RVA: 0x000E7D0C File Offset: 0x000E5F0C
	[PunRPC]
	public void CreateItemsRPC(int zoneId, byte[] stateData, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender) || stateData == null || stateData.Length >= 15360 || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.CreateItems))
		{
			return;
		}
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(GZipStream.UncompressBuffer(stateData)))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < this.zoneComponents.Count; i++)
					{
						if (!this.zoneComponents[i].ValidateCreateMultipleItems(zoneId, stateData, num))
						{
							return;
						}
					}
					for (int j = 0; j < num; j++)
					{
						int num2 = binaryReader.ReadInt32();
						int entityTypeId = binaryReader.ReadInt32();
						long data = binaryReader.ReadInt64();
						int data2 = binaryReader.ReadInt32();
						long createData = binaryReader.ReadInt64();
						int createdByEntityNetId = binaryReader.ReadInt32();
						Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
						Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(data2);
						float num3 = 10000f;
						if (vector.IsValid(num3) && rotation.IsValid() && this.FactoryHasEntity(entityTypeId) && this.IsPositionInManagerBounds(vector))
						{
							bool flag = true;
							for (int k = 0; k < this.zoneComponents.Count; k++)
							{
								flag &= this.zoneComponents[k].ValidateCreateItem(num2, entityTypeId, vector, rotation, createData, createdByEntityNetId);
							}
							if (flag)
							{
								this.CreateAndInitItemLocal(num2, entityTypeId, vector, rotation, createData, createdByEntityNetId);
							}
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06002B57 RID: 11095 RVA: 0x000E7ED4 File Offset: 0x000E60D4
	public void RequestMigrationRecovery(List<GameEntityCreateData> entityData)
	{
		if (entityData == null || entityData.Count == 0)
		{
			return;
		}
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		using (MemoryStream memoryStream = new MemoryStream(this.tempSerializeGameState))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(entityData.Count);
				for (int i = 0; i < entityData.Count; i++)
				{
					GameEntityCreateData gameEntityCreateData = entityData[i];
					GameEntityManager._JoinWithItems_WriteOne(binaryWriter, gameEntityCreateData.entityTypeId, gameEntityCreateData.position, gameEntityCreateData.rotation, gameEntityCreateData.createData, gameEntityCreateData.createdByEntityId, gameEntityCreateData.slotIndex);
				}
				byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
				this.photonView.RPC("JoinWithItemsRPC", this.GetAuthorityPlayer(), new object[]
				{
					array,
					Array.Empty<int>(),
					PhotonNetwork.LocalPlayer.ActorNumber
				});
			}
		}
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000E7FDC File Offset: 0x000E61DC
	public void JoinWithItems(List<GameEntity> entities)
	{
		if (entities.Count == 0)
		{
			return;
		}
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		MemoryStream memoryStream = new MemoryStream(this.tempSerializeGameState);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		int num = 0;
		for (int i = 0; i < entities.Count; i++)
		{
			if (entities[i] != null)
			{
				num++;
			}
		}
		binaryWriter.Write(num);
		for (int j = 0; j < entities.Count; j++)
		{
			GameEntity gameEntity = entities[j];
			if (!(gameEntity == null))
			{
				long createData = gameEntity.createData;
				for (int k = 0; k < this.zoneComponents.Count; k++)
				{
					createData = this.zoneComponents[k].ProcessMigratedGameEntityCreateData(gameEntity, createData);
				}
				GameEntityManager._JoinWithItems_WriteOne(binaryWriter, gameEntity.typeId, gameEntity.transform.localPosition, gameEntity.transform.localRotation, createData, this.GetNetIdFromEntityId(gameEntity.createdByEntityId), gameEntity.slotIndex);
			}
		}
		long position = memoryStream.Position;
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		this.photonView.RPC("JoinWithItemsRPC", this.GetAuthorityPlayer(), new object[]
		{
			array,
			Array.Empty<int>(),
			PhotonNetwork.LocalPlayer.ActorNumber
		});
	}

	// Token: 0x06002B59 RID: 11097 RVA: 0x000E8134 File Offset: 0x000E6334
	[PunRPC]
	public void PlayerLeftZoneRPC(PhotonMessageInfo info)
	{
		if (this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.PlayerLeftZone))
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
		if (gamePlayer == null)
		{
			return;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			gamePlayer.DidJoinWithItems = false;
		}
		this._leavingItemScratch.Clear();
		foreach (GameEntityId item in gamePlayer.IterateHeldAndSnappedItems(this))
		{
			this._leavingItemScratch.Add(item);
		}
		for (int i = 0; i < this._leavingItemScratch.Count; i++)
		{
			GameEntityId gameEntityId = this._leavingItemScratch[i];
			GameEntity gameEntity = this.GetGameEntity(gameEntityId);
			if (gameEntity != null && gameEntity.IsScenePlaced)
			{
				this.ReleaseScenePlacedHold(gameEntity);
				GameEntityManager.ScenePlacedRecord scenePlacedRecord;
				if (this.IsAuthority() && this.TryGetScenePlacedRecord(gameEntity, out scenePlacedRecord))
				{
					GameEntityManager.ResetScenePlacedTransform(gameEntity, scenePlacedRecord);
				}
			}
			else
			{
				if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(gameEntityId)))
				{
					this.netIdsForDelete.Add(this.GetNetIdFromEntityId(gameEntityId));
				}
				this.DestroyItemLocal(gameEntityId);
			}
		}
		this._leavingItemScratch.Clear();
		this.playerZoneJoinTimes.Remove(info.Sender.ActorNumber);
		Action onPlayerLeftZone = gamePlayer.OnPlayerLeftZone;
		if (onPlayerLeftZone == null)
		{
			return;
		}
		onPlayerLeftZone();
	}

	// Token: 0x06002B5A RID: 11098 RVA: 0x000E829C File Offset: 0x000E649C
	private bool TryGetScenePlacedRecord(GameEntity entity, out GameEntityManager.ScenePlacedRecord record)
	{
		for (int i = 0; i < this.scenePlacedEntities.Count; i++)
		{
			if (this.scenePlacedEntities[i].entity == entity)
			{
				record = this.scenePlacedEntities[i];
				return true;
			}
		}
		record = default(GameEntityManager.ScenePlacedRecord);
		return false;
	}

	// Token: 0x06002B5B RID: 11099 RVA: 0x000E82F4 File Offset: 0x000E64F4
	[PunRPC]
	public void JoinWithItemsRPC(byte[] stateData, int[] netIds, int joiningActorNum, PhotonMessageInfo info)
	{
		bool isAuthority = this.IsAuthority();
		if (isAuthority)
		{
			if (!this.IsValidAuthorityRPC(info.Sender))
			{
				return;
			}
		}
		else if (!this.IsAuthorityPlayer(info.Sender))
		{
			return;
		}
		float num;
		bool flag = this.playerZoneJoinTimes.TryGetValue(joiningActorNum, out num) && Time.unscaledTime - num < 10f;
		GamePlayer joiningPlayer;
		bool flag2 = GamePlayer.TryGetGamePlayer(joiningActorNum, out joiningPlayer);
		bool flag3;
		if (!isAuthority)
		{
			Player authorityPlayer = this.GetAuthorityPlayer();
			int? num2 = (authorityPlayer != null) ? new int?(authorityPlayer.ActorNumber) : null;
			int actorNumber = info.Sender.ActorNumber;
			flag3 = !(num2.GetValueOrDefault() == actorNumber & num2 != null);
		}
		else
		{
			flag3 = false;
		}
		bool flag4 = flag3;
		bool flag5 = isAuthority && info.Sender.ActorNumber != joiningActorNum;
		bool flag6 = stateData == null || stateData.Length >= 255;
		bool flag7 = flag2 && joiningPlayer.DidJoinWithItems && !flag;
		if (!flag2 || flag4 || flag5 || flag6 || flag7)
		{
			return;
		}
		if (!this.IsInZone())
		{
			return;
		}
		if (isAuthority)
		{
			joiningPlayer.DidJoinWithItems = true;
		}
		Action createItemsCallback = null;
		createItemsCallback = delegate()
		{
			try
			{
				GamePlayer joiningPlayer2 = joiningPlayer;
				joiningPlayer2.OnPlayerInitialized = (Action)Delegate.Remove(joiningPlayer2.OnPlayerInitialized, createItemsCallback);
				using (MemoryStream memoryStream = new MemoryStream(GZipStream.UncompressBuffer(stateData)))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num3 = binaryReader.ReadInt32();
						if (num3 <= 4)
						{
							if (isAuthority || netIds.Length == num3)
							{
								if (isAuthority)
								{
									netIds = new int[num3];
								}
								for (int i = 0; i < num3; i++)
								{
									int entityTypeId;
									Vector3 vector;
									Quaternion quaternion;
									long createData;
									int createdByEntityNetId;
									int num4;
									GameEntityManager._JoinWithItems_ReadOne(binaryReader, out entityTypeId, out vector, out quaternion, out createData, out createdByEntityNetId, out num4);
									Transform transform;
									if (!joiningPlayer.TryGetSlotXform(num4, out transform))
									{
										Debug.LogError("[GT/GameEntityManager]  ERROR!!!  " + string.Format("JoinWithItemsRPC: No slot transform for item's slot, {0}.", num4));
									}
									else
									{
										if (isAuthority)
										{
											int numToCreate = 1 + this.FactoryGetBuiltInEntityCountById(entityTypeId);
											netIds[i] = this.CreateNetId(numToCreate);
										}
										int netId = netIds[i];
										Vector3 pos = transform.TransformPoint(vector);
										float num5 = 10000f;
										if (vector.IsValid(num5) && quaternion.IsValid() && this.FactoryHasEntity(entityTypeId) && this.IsPositionInManagerBounds(pos))
										{
											bool flag8 = true;
											int num6 = 0;
											while (num6 < this.zoneComponents.Count && flag8)
											{
												flag8 &= this.zoneComponents[num6].ValidateMigratedGameEntity(netId, entityTypeId, joiningPlayer.rig.transform.position, Quaternion.identity, createData, joiningActorNum);
												num6++;
											}
											if (flag8)
											{
												GameEntityId gameEntityId = this.CreateAndInitItemLocal(netId, entityTypeId, joiningPlayer.rig.transform.position, Quaternion.identity, createData, createdByEntityNetId);
												bool isLeftHand = num4 == 0;
												SnapJointType snapIndexToJoint = GameSnappable.GetSnapIndexToJoint(num4);
												if (snapIndexToJoint != SnapJointType.None)
												{
													this.SnapEntityLocal(gameEntityId, isLeftHand, vector, quaternion, (int)snapIndexToJoint, joiningPlayer.rig.Creator);
												}
												else
												{
													this.GrabEntityOnCreate(gameEntityId, isLeftHand, vector, quaternion, joiningPlayer.rig.Creator);
												}
											}
										}
									}
								}
								if (isAuthority)
								{
									this.photonView.RPC("JoinWithItemsRPC", RpcTarget.Others, new object[]
									{
										stateData,
										netIds,
										joiningActorNum
									});
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		};
		if (joiningPlayer.AdditionalDataInitialized)
		{
			createItemsCallback();
			return;
		}
		GamePlayer joiningPlayer3 = joiningPlayer;
		joiningPlayer3.OnPlayerInitialized = (Action)Delegate.Combine(joiningPlayer3.OnPlayerInitialized, createItemsCallback);
	}

	// Token: 0x06002B5C RID: 11100 RVA: 0x000E84BE File Offset: 0x000E66BE
	private static void _JoinWithItems_WriteOne(BinaryWriter writer, int typeId, Vector3 localPos, Quaternion localRot, long createData, int createdByEntityId, int slotIndex)
	{
		writer.Write(typeId);
		writer.Write(BitPackUtils.PackWorldPosForNetwork(localPos));
		writer.Write(BitPackUtils.PackQuaternionForNetwork(localRot));
		writer.Write(createData);
		writer.Write(createdByEntityId);
		writer.Write((byte)(slotIndex + 1));
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000E84FC File Offset: 0x000E66FC
	private static void _JoinWithItems_ReadOne(BinaryReader reader, out int entityTypeId, out Vector3 localPos, out Quaternion localRot, out long createData, out int createdByEntityNetId, out int slotIndex)
	{
		entityTypeId = reader.ReadInt32();
		localPos = BitPackUtils.UnpackWorldPosFromNetwork(reader.ReadInt64());
		localRot = BitPackUtils.UnpackQuaternionFromNetwork(reader.ReadInt32());
		createData = reader.ReadInt64();
		createdByEntityNetId = reader.ReadInt32();
		slotIndex = (int)(reader.ReadByte() - 1);
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000E8550 File Offset: 0x000E6750
	public bool FactoryHasEntity(int entityTypeId)
	{
		GameObject gameObject;
		return this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject);
	}

	// Token: 0x06002B5F RID: 11103 RVA: 0x000E856C File Offset: 0x000E676C
	public GameObject FactoryPrefabById(int entityTypeId)
	{
		GameObject result;
		if (this.itemPrefabFactory.TryGetValue(entityTypeId, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x06002B60 RID: 11104 RVA: 0x000E858C File Offset: 0x000E678C
	public GameEntity FactoryEntityById(int entityTypeId)
	{
		GameObject gameObject;
		if (this.itemPrefabFactory.TryGetValue(entityTypeId, out gameObject))
		{
			return gameObject.GetComponent<GameEntity>();
		}
		return null;
	}

	// Token: 0x06002B61 RID: 11105 RVA: 0x000E85B4 File Offset: 0x000E67B4
	public int FactoryGetBuiltInEntityCountById(int entityTypeId)
	{
		GameEntity gameEntity = this.FactoryEntityById(entityTypeId);
		if (gameEntity == null || gameEntity.builtInEntities == null)
		{
			return 0;
		}
		return gameEntity.builtInEntities.Count;
	}

	// Token: 0x06002B62 RID: 11106 RVA: 0x000E85E7 File Offset: 0x000E67E7
	public bool PriceLookup(int entityTypeId, out int price)
	{
		if (this.priceLookupByEntityId.TryGetValue(entityTypeId, out price))
		{
			return true;
		}
		price = -1;
		return false;
	}

	// Token: 0x06002B63 RID: 11107 RVA: 0x000E8600 File Offset: 0x000E6800
	private void ValidateThatNetIdIsNotAlreadyUsed(int netId, int newTypeId)
	{
		for (int i = 0; i < this.netIds.Length; i++)
		{
			if (i < this.entities.Count && this.netIds[i] == netId)
			{
				this.entities[i] == null;
			}
		}
	}

	// Token: 0x06002B64 RID: 11108 RVA: 0x000E8654 File Offset: 0x000E6854
	public GameEntityId CreateAndInitItemLocal(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		GameEntity gameEntity = this.CreateItemLocal(netId, entityTypeId, position, rotation);
		if (gameEntity == null)
		{
			return GameEntityId.Invalid;
		}
		this.InitItemLocal(gameEntity, createData, createdByEntityNetId);
		return gameEntity.id;
	}

	// Token: 0x06002B65 RID: 11109 RVA: 0x000E8690 File Offset: 0x000E6890
	public GameEntity CreateItemLocal(int netId, int entityTypeId, Vector3 position, Quaternion rotation)
	{
		if (entityTypeId == -1)
		{
			return null;
		}
		this.nextNetId = Mathf.Max(netId + 1, this.nextNetId);
		GameObject original;
		if (!this.itemPrefabFactory.TryGetValue(entityTypeId, out original))
		{
			return null;
		}
		if (!this.createdItemTypeCount.ContainsKey(entityTypeId))
		{
			this.createdItemTypeCount[entityTypeId] = 0;
		}
		if (this.createdItemTypeCount[entityTypeId] > 100)
		{
			return null;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int num = dictionary[entityTypeId];
		dictionary[entityTypeId] = num + 1;
		GameEntity componentInChildren = UnityEngine.Object.Instantiate<GameObject>(original, position, rotation).GetComponentInChildren<GameEntity>();
		this.AddGameEntity(netId, componentInChildren);
		componentInChildren.Create(this, netId, entityTypeId);
		return componentInChildren;
	}

	// Token: 0x06002B66 RID: 11110 RVA: 0x000E8734 File Offset: 0x000E6934
	public void InitItemLocal(GameEntity entity, long createData, int createdByEntityNetId)
	{
		entity.Init(createData, createdByEntityNetId);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].OnCreateGameEntity(entity);
		}
	}

	// Token: 0x06002B67 RID: 11111 RVA: 0x000E8774 File Offset: 0x000E6974
	public void RequestDestroyItem(GameEntityId entityId)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity != null && gameEntity.IsScenePlaced)
		{
			return;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(entityId);
		if (!this.netIdsForDelete.Contains(netIdFromEntityId))
		{
			this.netIdsForDelete.Add(netIdFromEntityId);
		}
		int num = this.netIdsForState.IndexOf(netIdFromEntityId);
		if (num >= 0)
		{
			this.netIdsForState.RemoveAt(num);
			this.statesForState.RemoveAt(num);
		}
		this.DestroyItemLocal(entityId);
	}

	// Token: 0x06002B68 RID: 11112 RVA: 0x000E87F8 File Offset: 0x000E69F8
	public void RequestDestroyItems(List<GameEntityId> entityIds)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < entityIds.Count; i++)
		{
			list.Add(this.GetNetIdFromEntityId(entityIds[i]));
		}
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("DestroyItemRPC", RpcTarget.All, new object[]
			{
				list.ToArray()
			});
		}
	}

	// Token: 0x06002B69 RID: 11113 RVA: 0x000E8860 File Offset: 0x000E6A60
	[PunRPC]
	public void DestroyItemRPC(int[] entityNetId, PhotonMessageInfo info)
	{
		if (entityNetId == null || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.DestroyItem))
		{
			return;
		}
		for (int i = 0; i < entityNetId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, entityNetId[i]))
			{
				return;
			}
			if (!GameEntityManager.IsScenePlacedNetId(entityNetId[i]))
			{
				this.DestroyItemLocal(this.GetEntityIdFromNetId(entityNetId[i]));
			}
		}
	}

	// Token: 0x06002B6A RID: 11114 RVA: 0x000E88B8 File Offset: 0x000E6AB8
	public void DestroyItemLocal(GameEntityId entityId)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		if (!this.createdItemTypeCount.ContainsKey(gameEntity.typeId))
		{
			this.createdItemTypeCount[gameEntity.typeId] = 1;
		}
		Dictionary<int, int> dictionary = this.createdItemTypeCount;
		int typeId = gameEntity.typeId;
		int num = dictionary[typeId];
		dictionary[typeId] = num - 1;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			if (gamePlayer.IsLocal())
			{
				GamePlayerLocal.instance.ClearGrabbedIfHeld(gameEntity.id, this);
			}
			gamePlayer.ClearGrabbedIfHeld(gameEntity.id, this);
		}
		GamePlayer gamePlayer2;
		if (GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer2))
		{
			gamePlayer2.ClearSnappedIfSnapped(gameEntity.id, this);
		}
		this.RemoveGameEntity(gameEntity);
		if (gameEntity.isBuiltIn || gameEntity.IsScenePlaced)
		{
			gameEntity.gameObject.SetActive(false);
			return;
		}
		UnityEngine.Object.Destroy(gameEntity.gameObject);
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000E89A0 File Offset: 0x000E6BA0
	public void RequestState(GameEntityId entityId, long newState)
	{
		if (this.IsAuthority())
		{
			this.RequestStateAuthority(entityId, newState);
			return;
		}
		this.photonView.RPC("RequestStateRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			newState
		});
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000E89F4 File Offset: 0x000E6BF4
	private void RequestStateAuthority(GameEntityId entityId, long newState)
	{
		if (!this.IsAuthority())
		{
			return;
		}
		int netIdFromEntityId = this.GetNetIdFromEntityId(entityId);
		if (!this.IsValidNetId(netIdFromEntityId))
		{
			return;
		}
		if (this.netIdsForState.Contains(netIdFromEntityId))
		{
			this.statesForState[this.netIdsForState.IndexOf(netIdFromEntityId)] = newState;
			return;
		}
		this.netIdsForState.Add(netIdFromEntityId);
		this.statesForState.Add(newState);
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000E8A5C File Offset: 0x000E6C5C
	[PunRPC]
	public void RequestStateRPC(int entityNetId, long newState, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !gamePlayer.netStateLimiter.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		if (gameEntity == null || gameEntity.IsNull())
		{
			return;
		}
		bool flag = false;
		GRToolClub component = gameEntity.GetComponent<GRToolClub>();
		GRToolCollector component2 = gameEntity.GetComponent<GRToolCollector>();
		GRToolRevive component3 = gameEntity.GetComponent<GRToolRevive>();
		GRToolLantern component4 = gameEntity.GetComponent<GRToolLantern>();
		GRToolFlash component5 = gameEntity.GetComponent<GRToolFlash>();
		GRToolDirectionalShield component6 = gameEntity.GetComponent<GRToolDirectionalShield>();
		GRToolShieldGun component7 = gameEntity.GetComponent<GRToolShieldGun>();
		if (component == null && component2 == null && component3 == null && component4 == null && component5 == null && component6 == null && component7 == null)
		{
			flag = this.IsAuthorityPlayer(info.Sender);
		}
		bool flag2 = gamePlayer.IsHoldingEntity(entityIdFromNetId, false) || gamePlayer.IsHoldingEntity(entityIdFromNetId, true);
		bool flag3 = gameEntity.lastHeldByActorNumber == info.Sender.ActorNumber;
		if (!flag && (flag2 || flag3))
		{
			if (component4 != null)
			{
				flag = component4.CanChangeState(newState);
			}
			if (component5 != null)
			{
				flag = component5.CanChangeState(newState);
			}
			if (component != null || component2 != null || component3 != null || component6 != null || component7 != null)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			bool flag4 = gameEntity.snappedByActorNumber == gamePlayer.rig.OwningNetPlayer.ActorNumber;
			if (gameEntity.canHoldingPlayerUpdateState && flag2)
			{
				flag = true;
			}
			else if (gameEntity.canLastHoldingPlayerUpdateState && flag3)
			{
				flag = true;
			}
			else if (gameEntity.canSnapPlayerUpdateState && flag4)
			{
				flag = true;
			}
		}
		IGameEntityCustomStateChange component8 = gameEntity.GetComponent<IGameEntityCustomStateChange>();
		if (component8 != null)
		{
			flag = component8.CanChangeState(newState, info.Sender.ActorNumber);
		}
		if (flag)
		{
			if (this.netIdsForState.Contains(entityNetId))
			{
				this.statesForState[this.netIdsForState.IndexOf(entityNetId)] = newState;
				return;
			}
			this.netIdsForState.Add(entityNetId);
			this.statesForState.Add(newState);
		}
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x000E8C84 File Offset: 0x000E6E84
	[PunRPC]
	public void ApplyStateRPC(int[] netId, long[] newState, PhotonMessageInfo info)
	{
		if (netId == null || newState == null || netId.Length != newState.Length || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ApplyState))
		{
			return;
		}
		for (int i = 0; i < netId.Length; i++)
		{
			if (!this.IsValidClientRPC(info.Sender, netId[i]))
			{
				return;
			}
			GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(netId[i]);
			GameEntity gameEntity = this.entities[entityIdFromNetId.index];
			if (gameEntity != null)
			{
				gameEntity.SetState(newState[i]);
			}
		}
	}

	// Token: 0x06002B6F RID: 11119 RVA: 0x000E8CFC File Offset: 0x000E6EFC
	public void RequestGrabEntity(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation)
	{
		bool inRoom = PhotonNetwork.InRoom;
		if (!this.IsAuthority() || !inRoom)
		{
			this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		if (inRoom)
		{
			long num = BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation);
			this.photonView.RPC("RequestGrabEntityRPC", this.GetAuthorityPlayer(), new object[]
			{
				this.GetNetIdFromEntityId(gameEntityId),
				isLeftHand,
				num
			});
		}
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000E8D78 File Offset: 0x000E6F78
	[PunRPC]
	public void RequestGrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, PhotonMessageInfo info)
	{
		if (!this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out vector, out quaternion);
		float num = 10000f;
		if (!vector.IsValid(num) || !quaternion.IsValid() || vector.sqrMagnitude > 6400f)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !this.IsPlayerHandNearEntity(gamePlayer, entityNetId, isLeftHand, false, 16f) || this.IsValidEntity(gamePlayer.GetGameEntityId(isLeftHand)) || !gamePlayer.netGrabLimiter.CheckCallTime(Time.time) || gamePlayer.IsHoldingEntity(this, isLeftHand))
		{
			return;
		}
		GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(entityNetId));
		if (gameEntity == null)
		{
			return;
		}
		if (!this.ValidateGrab(gameEntity, info.Sender.ActorNumber, isLeftHand))
		{
			return;
		}
		this.photonView.RPC("GrabEntityRPC", RpcTarget.All, new object[]
		{
			entityNetId,
			isLeftHand,
			packedPosRot,
			info.Sender
		});
		PhotonNetwork.SendAllOutgoingCommands();
	}

	// Token: 0x06002B71 RID: 11121 RVA: 0x000E8E88 File Offset: 0x000E7088
	[PunRPC]
	public void GrabEntityRPC(int entityNetId, bool isLeftHand, long packedPosRot, Player grabbedByPlayer, PhotonMessageInfo info)
	{
		if (!this.IsValidClientRPC(info.Sender, entityNetId) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.GrabEntity))
		{
			return;
		}
		Vector3 localPosition;
		Quaternion localRotation;
		BitPackUtils.UnpackHandPosRotFromNetwork(packedPosRot, out localPosition, out localRotation);
		float num = 10000f;
		if (!localPosition.IsValid(num) || !localRotation.IsValid() || localPosition.sqrMagnitude > 6400f)
		{
			return;
		}
		this.GrabEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, localPosition, localRotation, NetPlayer.Get(grabbedByPlayer));
	}

	// Token: 0x06002B72 RID: 11122 RVA: 0x000E8F00 File Offset: 0x000E7100
	private void GrabEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(grabbedByPlayer.ActorNumber), out rigContainer))
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		if (gameEntity == null)
		{
			return;
		}
		if (grabbedByPlayer == null)
		{
			return;
		}
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		if (grabbedByPlayer.IsLocal && gameEntity.heldByActorNumber == grabbedByPlayer.ActorNumber && gameEntity.heldByHandIndex == handIndex)
		{
			return;
		}
		GameEntityManager.TryDetachCompletely(gameEntity);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(grabbedByPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		GamePlayer gamePlayer2;
		if (GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer2))
		{
			int num = gamePlayer2.FindHandIndex(gameEntityId);
			bool flag = gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
			gamePlayer2.ClearGrabbedIfHeld(gameEntityId, this);
			if (num != -1 && flag)
			{
				GamePlayerLocal.instance.ClearGrabbed(num);
			}
		}
		Transform handTransform = gamePlayer.GetHandTransform(handIndex);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			if (grabbedByPlayer.IsLocal)
			{
				component.constraints = RigidbodyConstraints.FreezeAll;
				component.isKinematic = false;
			}
			else
			{
				component.constraints = RigidbodyConstraints.None;
				component.isKinematic = true;
			}
		}
		gameEntity.transform.SetParent(handTransform);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameEntity.transform.localScale = Vector3.one;
		gameEntity.heldByActorNumber = grabbedByPlayer.ActorNumber;
		gameEntity.heldByHandIndex = handIndex;
		gameEntity.lastHeldByActorNumber = gameEntity.heldByActorNumber;
		gamePlayer.SetGrabbed(gameEntityId, handIndex, this);
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.SetGrabbed(gameEntityId, GamePlayer.GetHandIndex(isLeftHand));
			GamePlayerLocal.instance.PlayCatchFx(isLeftHand);
		}
		GameEntityManager.TryUnsnapLocal(gameEntity);
		gameEntity.PlayCatchFx();
		Action onGrabbed = gameEntity.OnGrabbed;
		if (onGrabbed != null)
		{
			onGrabbed();
		}
		CustomGameMode.OnEntityGrabbed(gameEntity, true);
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x000E90F0 File Offset: 0x000E72F0
	public void GrabEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
	{
		if (grabbedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GamePlayerLocal.instance.gamePlayer.DeleteGrabbedEntityLocal(GamePlayer.GetHandIndex(isLeftHand));
		}
		this.GrabEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, grabbedByPlayer);
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x000E912C File Offset: 0x000E732C
	public GameEntityId TryGrabLocal(Vector3 handPosition, Vector3 fingerPosition, bool isLeftHand, out Vector3 closestPointOnBoundingBox, out bool fingerPositionUsed)
	{
		float a = 0.03f;
		float maxAdjustedGrabDistance = 0f;
		float num = 0.1f;
		float max = 0.25f;
		fingerPositionUsed = false;
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		Vector3 rigidbodyVelocity = GTPlayer.Instance.RigidbodyVelocity;
		GameEntity gameEntity = null;
		float num2 = float.MaxValue;
		Vector3 vector = handPosition;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity2 = this.entities[i];
			if (this.ValidateGrab(gameEntity2, actorNumber, isLeftHand))
			{
				float num3 = 0.75f;
				float magnitude = (handPosition - gameEntity2.transform.position).magnitude;
				if (magnitude <= num3 && (gameEntity2.snappedByActorNumber == -1 || gameEntity2.snappedByActorNumber != actorNumber || magnitude <= 0.1f))
				{
					Vector3 vector2 = gameEntity2.GetVelocity() - rigidbodyVelocity;
					float magnitude2 = vector2.magnitude;
					float num4 = Mathf.Clamp(magnitude2 * num, 0f, max);
					Vector3 slopProjection = (magnitude2 > 0.2f) ? (vector2.normalized * num4) : Vector3.zero;
					maxAdjustedGrabDistance = Mathf.Max(a, gameEntity2.pickupRangeFromSurface);
					GameEntity.RendererSet grabbableRenderers = gameEntity2.GetGrabbableRenderers();
					foreach (ValueTuple<MeshFilter, MeshRenderer> valueTuple in grabbableRenderers.renderers)
					{
						MeshFilter item = valueTuple.Item1;
						MeshRenderer item2 = valueTuple.Item2;
						if (item2.gameObject.activeInHierarchy && item2.enabled)
						{
							GameEntityManager._TryGrabLocal_TestBounds(handPosition, item2.transform, slopProjection, item.sharedMesh.bounds, num4, maxAdjustedGrabDistance, gameEntity2, false, ref num2, ref gameEntity, ref vector, ref fingerPositionUsed);
							if (GameEntityManager.IsThinAlongDirection(item2.transform, item.sharedMesh.bounds, fingerPosition - handPosition, 0.1f))
							{
								GameEntityManager._TryGrabLocal_TestBounds(fingerPosition, item2.transform, slopProjection, item.sharedMesh.bounds, num4, maxAdjustedGrabDistance, gameEntity2, true, ref num2, ref gameEntity, ref vector, ref fingerPositionUsed);
							}
						}
					}
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in grabbableRenderers.skinnedRenderers)
					{
						if (skinnedMeshRenderer.gameObject.activeInHierarchy && skinnedMeshRenderer.enabled)
						{
							GameEntityManager._TryGrabLocal_TestBounds(handPosition, skinnedMeshRenderer.rootBone, slopProjection, skinnedMeshRenderer.localBounds, num4, maxAdjustedGrabDistance, gameEntity2, false, ref num2, ref gameEntity, ref vector, ref fingerPositionUsed);
							if (GameEntityManager.IsThinAlongDirection(skinnedMeshRenderer.rootBone, skinnedMeshRenderer.bounds, fingerPosition - handPosition, 0.1f))
							{
								GameEntityManager._TryGrabLocal_TestBounds(fingerPosition, skinnedMeshRenderer.rootBone, slopProjection, skinnedMeshRenderer.localBounds, num4, maxAdjustedGrabDistance, gameEntity2, true, ref num2, ref gameEntity, ref vector, ref fingerPositionUsed);
							}
						}
					}
					if (grabbableRenderers.renderers.Count == 0 && grabbableRenderers.skinnedRenderers.Count == 0)
					{
						float num5 = magnitude;
						if (num5 < num2)
						{
							num2 = num5;
							gameEntity = gameEntity2;
							vector = gameEntity2.transform.position;
						}
					}
				}
			}
		}
		closestPointOnBoundingBox = vector;
		if (!(gameEntity != null))
		{
			return GameEntityId.Invalid;
		}
		if (num2 > Mathf.Max(a, gameEntity.pickupRangeFromSurface))
		{
			return GameEntityId.Invalid;
		}
		return gameEntity.id;
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x000E9488 File Offset: 0x000E7688
	private static bool IsThinAlongDirection(Transform transform, Bounds bounds, Vector3 direction, float thinThreshold = 0.1f)
	{
		return GameEntityManager.GetBoundsThicknessAlongDirection(bounds, transform.InverseTransformDirection(direction)) <= thinThreshold;
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x000E94A0 File Offset: 0x000E76A0
	private static float GetBoundsThicknessAlongDirection(Bounds bounds, Vector3 localDirection)
	{
		localDirection.Normalize();
		Vector3 rhs = new Vector3(Mathf.Abs(localDirection.x), Mathf.Abs(localDirection.y), Mathf.Abs(localDirection.z));
		return Vector3.Dot(bounds.extents, rhs);
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000E94EC File Offset: 0x000E76EC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _TryGrabLocal_TestBounds(Vector3 handPosition, Transform t, Vector3 slopProjection, Bounds bounds, float slopForSpeed, float maxAdjustedGrabDistance, GameEntity entity, bool isTestingAltPosition, ref float bestDist, ref GameEntity bestEntity, ref Vector3 closestPoint, ref bool usedAltPosition)
	{
		Vector3 vector = t.InverseTransformPoint(handPosition);
		Vector3 b = t.InverseTransformPoint(handPosition + slopProjection);
		bounds.extents != bounds.extents;
		Vector3 b2;
		float num;
		Vector3 b3;
		float num2;
		if (GameEntityManager.SegmentHitsBounds(bounds, vector, b, out b2, out num))
		{
			b3 = ((num <= 0f) ? Vector3.zero : t.TransformVector(vector - b2));
			num2 = b3.magnitude - slopForSpeed;
		}
		else
		{
			b3 = t.TransformVector(vector - bounds.ClosestPoint(vector));
			num2 = b3.magnitude;
		}
		num2 = Mathf.Max(0f, num2 - maxAdjustedGrabDistance);
		b3 = b3.normalized * num2;
		if (num2 < bestDist)
		{
			bestDist = num2;
			bestEntity = entity;
			closestPoint = handPosition - b3;
			usedAltPosition = isTestingAltPosition;
		}
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x000E95BC File Offset: 0x000E77BC
	private void DrawDebugStar(Vector3 position, float radius)
	{
		for (int i = 0; i < 20; i++)
		{
			Debug.DrawLine(position, position + Random.onUnitSphere * radius, Color.red, 10f);
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x000E95F8 File Offset: 0x000E77F8
	private static bool SegmentHitsBounds(Bounds bounds, Vector3 a, Vector3 b, out Vector3 hitPoint, out float distance)
	{
		hitPoint = default(Vector3);
		distance = float.MaxValue;
		Vector3 a2 = b - a;
		float magnitude = a2.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			if (bounds.Contains(a))
			{
				distance = 0f;
				hitPoint = a;
				return true;
			}
			return false;
		}
		else
		{
			Ray ray = new Ray(a, a2 / magnitude);
			if (bounds.IntersectRay(ray, out distance) && distance <= magnitude)
			{
				hitPoint = a + ray.direction * distance;
				return true;
			}
			return false;
		}
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x000E9688 File Offset: 0x000E7888
	public bool GetEntitiesWithComponentInRadius<T>(Vector3 center, float radius, bool checkRootOnly, List<T> nearbyEntities)
	{
		float num = radius * radius;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (!(gameEntity == null))
			{
				T t;
				if (checkRootOnly)
				{
					t = gameEntity.GetComponent<T>();
				}
				else
				{
					t = gameEntity.GetComponentInChildren<T>();
				}
				if (t != null && (this.entities[i].transform.position - center).sqrMagnitude < num)
				{
					nearbyEntities.Add(t);
				}
			}
		}
		return nearbyEntities.Count > 0;
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x000E971C File Offset: 0x000E791C
	public void LogGrabDiagnostics(Vector3 handPosition, bool isLeftHand, int handIndex)
	{
		int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
		int num = 0;
		for (int i = 0; i < this.entities.Count; i++)
		{
			GameEntity gameEntity = this.entities[i];
			if (!(gameEntity == null) && (handPosition - gameEntity.transform.position).magnitude <= 0.75f)
			{
				num++;
				this.WhyGrabRejected(gameEntity, actorNumber, isLeftHand);
			}
		}
	}

	// Token: 0x06002B7C RID: 11132 RVA: 0x000E9794 File Offset: 0x000E7994
	private string WhyGrabRejected(GameEntity gameEntity, int playerActorNumber, bool isLeftHand)
	{
		if (gameEntity == null)
		{
			return "null";
		}
		if (!gameEntity.pickupable)
		{
			return "not pickupable";
		}
		if (gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != playerActorNumber)
		{
			return string.Format("onlyGrabActor={0} (you={1})", gameEntity.onlyGrabActorNumber, playerActorNumber);
		}
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			return string.Format("heldByActor={0}", gameEntity.heldByActorNumber);
		}
		if (gameEntity.snappedByActorNumber != -1 && gameEntity.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer))
		{
			return string.Format("snappedByActor={0}", gameEntity.snappedByActorNumber);
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && !component.CanGrabWithHand(isLeftHand))
		{
			return "GameSnappable disallows " + (isLeftHand ? "left" : "right") + " hand";
		}
		if (this.IsValidEntity(gameEntity.attachedToEntityId))
		{
			GameEntity gameEntity2 = this.GetGameEntity(gameEntity.attachedToEntityId);
			if (gameEntity2 != null)
			{
				if (gameEntity2.snappedByActorNumber != -1 && gameEntity2.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity2.snappedByActorNumber, out gamePlayer))
				{
					return string.Format("attachedTo '{0}' snappedByActor={1}", gameEntity2.name, gameEntity2.snappedByActorNumber);
				}
				GameSnappable component2 = gameEntity2.GetComponent<GameSnappable>();
				if (component2 != null && !component2.CanGrabWithHand(isLeftHand))
				{
					return string.Concat(new string[]
					{
						"attachedTo '",
						gameEntity2.name,
						"' GameSnappable disallows ",
						isLeftHand ? "left" : "right",
						" hand"
					});
				}
			}
		}
		return null;
	}

	// Token: 0x06002B7D RID: 11133 RVA: 0x000E9954 File Offset: 0x000E7B54
	private bool ValidateGrab(GameEntity gameEntity, int playerActorNumber, bool isLeftHand)
	{
		if (gameEntity == null || !gameEntity.pickupable)
		{
			return false;
		}
		if (gameEntity.onlyGrabActorNumber != -1 && gameEntity.onlyGrabActorNumber != playerActorNumber)
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (gameEntity.heldByActorNumber != -1 && gameEntity.heldByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		if (gameEntity.snappedByActorNumber != -1 && gameEntity.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer))
		{
			return false;
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && !component.CanGrabWithHand(isLeftHand))
		{
			return false;
		}
		if (this.IsValidEntity(gameEntity.attachedToEntityId))
		{
			GameEntity gameEntity2 = this.GetGameEntity(gameEntity.attachedToEntityId);
			if (gameEntity2 != null)
			{
				if (gameEntity2.snappedByActorNumber != -1 && gameEntity2.snappedByActorNumber != playerActorNumber && GamePlayer.TryGetGamePlayer(gameEntity2.snappedByActorNumber, out gamePlayer))
				{
					return false;
				}
				GameSnappable component2 = gameEntity2.GetComponent<GameSnappable>();
				if (component2 != null && !component2.CanGrabWithHand(isLeftHand))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06002B7E RID: 11134 RVA: 0x000E9A64 File Offset: 0x000E7C64
	public T GetParentEntity<T>(Transform transform) where T : MonoBehaviour
	{
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x000E9AA4 File Offset: 0x000E7CA4
	public void RequestThrowEntity(GameEntityId entityId, bool isLeftHand, Vector3 headPosition, Vector3 velocity, Vector3 angVelocity)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 vector = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			Vector3 vector2 = gameEntity.transform.TransformPoint(component.centerOfMass);
			Vector3 vector3 = vector2 - headPosition;
			float magnitude = vector3.magnitude;
			if (magnitude > 0f)
			{
				vector3 /= magnitude;
				RaycastHit raycastHit;
				if (Physics.SphereCast(headPosition, 0.05f, vector3, out raycastHit, magnitude + 0.1f, 513, QueryTriggerInteraction.Ignore))
				{
					component.GetComponentsInChildren<Collider>(this._collidersList);
					Vector3 position = component.position + -raycastHit.normal * 1000f;
					float num = float.MaxValue;
					bool flag = false;
					Plane plane = new Plane(raycastHit.normal, raycastHit.point);
					foreach (Collider collider in this._collidersList)
					{
						if (collider.enabled && !collider.isTrigger)
						{
							Vector3 point = collider.ClosestPoint(position);
							float num2 = Mathf.Abs(plane.GetDistanceToPoint(point));
							if (num2 < num)
							{
								num = num2;
								flag = true;
							}
						}
					}
					if (flag)
					{
						vector += raycastHit.normal * num;
					}
					else
					{
						float d = Mathf.Max(raycastHit.distance - 0.2f, 0f);
						Vector3 a = headPosition + vector3 * d;
						vector += a - vector2;
					}
				}
			}
		}
		bool inRoom = PhotonNetwork.InRoom;
		if (!this.IsAuthority() || !inRoom)
		{
			this.ThrowEntityLocal(entityId, isLeftHand, vector, rotation, velocity, angVelocity, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		if (inRoom)
		{
			this.photonView.RPC("RequestThrowEntityRPC", this.GetAuthorityPlayer(), new object[]
			{
				this.GetNetIdFromEntityId(entityId),
				isLeftHand,
				vector,
				rotation,
				velocity,
				angVelocity
			});
		}
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x000E9CF4 File Offset: 0x000E7EF4
	[PunRPC]
	public void RequestThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3) && velocity.sqrMagnitude <= 1600f && this.IsPositionInManagerBounds(position))
					{
						GamePlayer gamePlayer;
						if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netThrowLimiter.CheckCallTime(Time.time))
						{
							return;
						}
						this.photonView.RPC("ThrowEntityRPC", RpcTarget.All, new object[]
						{
							entityNetId,
							isLeftHand,
							position,
							rotation,
							velocity,
							angVelocity,
							info.Sender,
							info.SentServerTime
						});
						PhotonNetwork.SendAllOutgoingCommands();
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x000E9E18 File Offset: 0x000E8018
	[PunRPC]
	public void ThrowEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player thrownByPlayer, double throwTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3) && velocity.sqrMagnitude <= 1600f)
					{
						NetPlayer netPlayer = NetPlayer.Get(thrownByPlayer);
						if (netPlayer.IsLocal && !this.IsAuthority())
						{
							return;
						}
						this.ThrowEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, velocity, angVelocity, netPlayer);
						return;
					}
				}
			}
		}
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x000E9EC0 File Offset: 0x000E80C0
	private void ThrowEntityLocal(GameEntityId entityId, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer thrownByPlayer)
	{
		if (entityId.index < 0 || entityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[entityId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (thrownByPlayer == null)
		{
			return;
		}
		gameEntity.transform.SetParent(null);
		if (gameEntity.IsScenePlaced)
		{
			GameEntityManager.MoveScenePlacedToHomeScene(gameEntity);
		}
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.constraints = RigidbodyConstraints.None;
			component.position = position;
			component.rotation = rotation;
			component.linearVelocity = velocity;
			component.angularVelocity = angVelocity;
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
		gameEntity.attachedToEntityId = GameEntityId.Invalid;
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(thrownByPlayer);
		if (vrrig != null && gameEntity.gravityController != null)
		{
			gameEntity.gravityController.SetPersonalGravityDirection(-vrrig.transform.up);
		}
		bool flag = thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		int handIndex = GamePlayer.GetHandIndex(isLeftHand);
		RigContainer rigContainer;
		if (flag)
		{
			GamePlayerLocal.instance.gamePlayer.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.ClearGrabbed(handIndex);
			GamePlayerLocal.instance.PlayThrowFx(isLeftHand);
		}
		else if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(thrownByPlayer.ActorNumber), out rigContainer))
		{
			GamePlayer gamePlayerRef = rigContainer.Rig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearGrabbedIfHeld(entityId, this);
				gamePlayerRef.ClearSnappedIfSnapped(entityId, this);
			}
		}
		gameEntity.PlayThrowFx();
		Action onReleased = gameEntity.OnReleased;
		if (onReleased != null)
		{
			onReleased();
		}
		CustomGameMode.OnEntityGrabbed(gameEntity, false);
		GRBadge component2 = gameEntity.GetComponent<GRBadge>();
		if (component2 != null)
		{
			GRPlayer grplayer = GRPlayer.Get(thrownByPlayer.ActorNumber);
			if (grplayer != null)
			{
				grplayer.AttachBadge(component2);
			}
		}
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x000EA0A8 File Offset: 0x000E82A8
	public void RequestSnapEntity(GameEntityId entityId, bool isLeftHand, SnapJointType jointType)
	{
		GameEntity gameEntity = this.GetGameEntity(entityId);
		if (gameEntity == null)
		{
			return;
		}
		Vector3 position = gameEntity.transform.position;
		Quaternion rotation = gameEntity.transform.rotation;
		if (!this.IsAuthority())
		{
			this.SnapEntityLocal(entityId, isLeftHand, position, rotation, (int)jointType, NetPlayer.Get(PhotonNetwork.LocalPlayer));
		}
		this.photonView.RPC("RequestSnapEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			isLeftHand,
			position,
			rotation,
			(int)jointType
		});
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x000EA14C File Offset: 0x000E834C
	[PunRPC]
	public void RequestSnapEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, PhotonMessageInfo info)
	{
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid() && this.IsPositionInManagerBounds(position))
			{
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !GameEntityManager.IsPlayerHandNearPosition(gamePlayer, position, isLeftHand, false, 16f) || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId), isLeftHand) || !gamePlayer.netSnapLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				this.photonView.RPC("SnapEntityRPC", RpcTarget.All, new object[]
				{
					entityNetId,
					isLeftHand,
					position,
					rotation,
					jointType,
					info.Sender,
					info.SentServerTime
				});
				PhotonNetwork.SendAllOutgoingCommands();
				return;
			}
		}
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x000EA23C File Offset: 0x000E843C
	[PunRPC]
	public void SnapEntityRPC(int entityNetId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, Player thrownByPlayer, double snapTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId, position) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				if (!this.IsAuthority() && thrownByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					return;
				}
				this.SnapEntityLocal(this.GetEntityIdFromNetId(entityNetId), isLeftHand, position, rotation, jointType, NetPlayer.Get(thrownByPlayer));
				return;
			}
		}
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x000EA2BC File Offset: 0x000E84BC
	private void SnapEntityLocal(GameEntityId gameEntityId, bool isLeftHand, Vector3 position, Quaternion rotation, int jointType, NetPlayer snappedByPlayer)
	{
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntity == null)
		{
			return;
		}
		if (snappedByPlayer == null)
		{
			return;
		}
		if (snappedByPlayer.IsLocal && gameEntity.heldByActorNumber != snappedByPlayer.ActorNumber && gameEntity.lastHeldByActorNumber == snappedByPlayer.ActorNumber)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(snappedByPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		GameEntityManager.TryDetachCompletely(gameEntity);
		SuperInfectionSnapPoint superInfectionSnapPoint;
		if (jointType == 64)
		{
			gameEntity.GetComponent<GameSnappable>();
			superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, (SnapJointType)jointType);
		}
		else
		{
			superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, (SnapJointType)jointType);
			int num = -1;
			if (jointType == 1)
			{
				num = 2;
			}
			if (jointType == 4)
			{
				num = 3;
			}
			if (num != -1)
			{
				gamePlayer.SetSnapped(gameEntityId, num, this);
			}
		}
		if (superInfectionSnapPoint == null)
		{
			return;
		}
		if (superInfectionSnapPoint.HasSnapped())
		{
			GameEntity snappedEntity = superInfectionSnapPoint.GetSnappedEntity();
			snappedEntity.transform.SetParent(null);
			snappedEntity.transform.SetLocalPositionAndRotation(position, rotation);
			Rigidbody component = snappedEntity.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.isKinematic = false;
				component.constraints = RigidbodyConstraints.None;
				component.position = position;
				component.rotation = rotation;
				component.linearVelocity = Vector3.up * 5f;
			}
			snappedEntity.heldByActorNumber = -1;
			snappedEntity.heldByHandIndex = -1;
			snappedEntity.snappedByActorNumber = -1;
			snappedEntity.snappedJoint = SnapJointType.None;
			snappedEntity.PlayThrowFx();
			Action onReleased = snappedEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		superInfectionSnapPoint.Snapped(gameEntity);
		gameEntity.transform.SetParent(superInfectionSnapPoint.transform);
		gameEntity.transform.SetLocalPositionAndRotation(position, rotation);
		gameEntity.transform.localScale = Vector3.one;
		Rigidbody component2 = gameEntity.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.isKinematic = true;
		}
		Vector3 zero = Vector3.zero;
		Quaternion identity = Quaternion.identity;
		GameSnappable component3 = gameEntity.GetComponent<GameSnappable>();
		if (component3 != null)
		{
			component3.GetSnapOffset((SnapJointType)jointType, out zero, out identity);
		}
		gameEntity.transform.localPosition = zero;
		gameEntity.transform.localRotation = identity;
		gameEntity.snappedByActorNumber = snappedByPlayer.ActorNumber;
		gameEntity.snappedJoint = (SnapJointType)jointType;
		if (component3 != null)
		{
			component3.OnSnap();
		}
		Action onSnapped = gameEntity.OnSnapped;
		if (onSnapped != null)
		{
			onSnapped();
		}
		gameEntity.PlaySnapFx();
	}

	// Token: 0x06002B87 RID: 11143 RVA: 0x000EA50B File Offset: 0x000E870B
	public void SnapEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, int jointType, NetPlayer grabbedByPlayer)
	{
		this.SnapEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, jointType, grabbedByPlayer);
	}

	// Token: 0x06002B88 RID: 11144 RVA: 0x000EA51C File Offset: 0x000E871C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TryUnsnapLocal(GameEntity gameEntity)
	{
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(gameEntity.snappedByActorNumber, out gamePlayer))
		{
			gamePlayer.ClearSnappedIfSnapped(gameEntity.id, gameEntity.manager);
		}
		GameSnappable component = gameEntity.GetComponent<GameSnappable>();
		if (component != null && component.snappedToJoint != null && component.snappedToJoint.jointType != SnapJointType.None)
		{
			SuperInfectionSnapPoint superInfectionSnapPoint = SuperInfectionSnapPointManager.FindSnapPoint(gamePlayer, component.snappedToJoint.jointType);
			if (superInfectionSnapPoint == null)
			{
				superInfectionSnapPoint = component.snappedToJoint;
			}
			component.OnUnsnap();
			superInfectionSnapPoint.Unsnapped();
			Action onUnsnapped = gameEntity.OnUnsnapped;
			if (onUnsnapped != null)
			{
				onUnsnapped();
			}
		}
		gameEntity.snappedByActorNumber = -1;
		gameEntity.snappedJoint = SnapJointType.None;
	}

	// Token: 0x06002B89 RID: 11145 RVA: 0x000EA5C4 File Offset: 0x000E87C4
	public void RequestAttachEntity(GameEntityId entityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (this.GetGameEntity(entityId) == null)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			this.AttachEntityLocal(entityId, attachToEntityId, slotId, localPosition, localRotation);
		}
		this.photonView.RPC("RequestAttachEntityRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			this.GetNetIdFromEntityId(attachToEntityId),
			slotId,
			localPosition,
			localRotation
		});
	}

	// Token: 0x06002B8A RID: 11146 RVA: 0x000EA650 File Offset: 0x000E8850
	public void RequestAttachEntityAuthority(GameEntityId entityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (this.GetGameEntity(entityId) == null)
		{
			return;
		}
		if (!this.IsAuthority())
		{
			return;
		}
		this.photonView.RPC("AttachEntityRPC", RpcTarget.All, new object[]
		{
			this.GetNetIdFromEntityId(entityId),
			this.GetNetIdFromEntityId(attachToEntityId),
			slotId,
			localPosition,
			localRotation,
			null,
			PhotonNetwork.Time
		});
	}

	// Token: 0x06002B8B RID: 11147 RVA: 0x000EA6D8 File Offset: 0x000E88D8
	[PunRPC]
	public void RequestAttachEntityRPC(int entityNetId, int attachToEntityNetId, int slotId, Vector3 localPosition, Quaternion localRotation, PhotonMessageInfo info)
	{
		bool flag = !this.IsValidNetId(attachToEntityNetId);
		if (this.IsValidAuthorityRPC(info.Sender, entityNetId))
		{
			float num = 10000f;
			if (localPosition.IsValid(num) && localRotation.IsValid())
			{
				if (!flag)
				{
					if (localPosition.sqrMagnitude > 4f || !this.IsEntityNearEntity(entityNetId, attachToEntityNetId, 16f))
					{
						return;
					}
				}
				else if (!this.IsPositionInManagerBounds(localPosition))
				{
					return;
				}
				GameEntity gameEntityFromNetId = this.GetGameEntityFromNetId(entityNetId);
				if (gameEntityFromNetId == null)
				{
					return;
				}
				GameDockable component = gameEntityFromNetId.GetComponent<GameDockable>();
				if (component == null)
				{
					return;
				}
				GameEntity gameEntityFromNetId2 = this.GetGameEntityFromNetId(attachToEntityNetId);
				if (gameEntityFromNetId2 != null)
				{
					GameDock component2 = gameEntityFromNetId2.GetComponent<GameDock>();
					if (component2 == null)
					{
						return;
					}
					if (!component2.CanDock(component))
					{
						return;
					}
				}
				GamePlayer gamePlayer = GamePlayer.GetGamePlayer(info.Sender);
				if (gamePlayer == null || !gamePlayer.IsHoldingEntity(this.GetEntityIdFromNetId(entityNetId)) || !gamePlayer.netSnapLimiter.CheckCallTime(Time.time))
				{
					return;
				}
				this.photonView.RPC("AttachEntityRPC", RpcTarget.All, new object[]
				{
					entityNetId,
					attachToEntityNetId,
					slotId,
					localPosition,
					localRotation,
					info.Sender,
					info.SentServerTime
				});
				PhotonNetwork.SendAllOutgoingCommands();
				return;
			}
		}
	}

	// Token: 0x06002B8C RID: 11148 RVA: 0x000EA83C File Offset: 0x000E8A3C
	[PunRPC]
	public void AttachEntityRPC(int entityNetId, int attachToEntityNetId, int slotId, Vector3 localPosition, Quaternion localRotation, Player attachedByPlayer, double snapTime, PhotonMessageInfo info)
	{
		if (this.IsValidClientRPC(info.Sender, entityNetId) && this.IsValidNetId(attachToEntityNetId) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.ThrowEntity))
		{
			float num = 10000f;
			if (localPosition.IsValid(num) && localRotation.IsValid())
			{
				if (!this.IsAuthority() && attachedByPlayer != null && attachedByPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					return;
				}
				this.AttachEntityLocal(this.GetEntityIdFromNetId(entityNetId), this.GetEntityIdFromNetId(attachToEntityNetId), slotId, localPosition, localRotation);
				return;
			}
		}
	}

	// Token: 0x06002B8D RID: 11149 RVA: 0x000EA8C4 File Offset: 0x000E8AC4
	private void AttachEntityLocal(GameEntityId gameEntityId, GameEntityId attachToEntityId, int slotId, Vector3 localPosition, Quaternion localRotation)
	{
		if (gameEntityId.index < 0 || gameEntityId.index >= this.entities.Count)
		{
			return;
		}
		GameEntity gameEntity = this.entities[gameEntityId.index];
		if (gameEntity == null)
		{
			return;
		}
		GameEntity gameEntity2 = this.entities[attachToEntityId.index];
		GameEntityManager.TryDetachCompletely(gameEntity);
		bool flag = gameEntity2 == null;
		Transform parent = (gameEntity2 == null) ? null : gameEntity2.transform;
		gameEntity.transform.SetParent(parent);
		gameEntity.transform.SetLocalPositionAndRotation(localPosition, localRotation);
		gameEntity.attachedToEntityId = (flag ? GameEntityId.Invalid : gameEntity2.id);
		Rigidbody component = gameEntity.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = !flag;
			component.constraints = RigidbodyConstraints.None;
		}
		GameDockable component2 = gameEntity.GetComponent<GameDockable>();
		if (gameEntity2 != null)
		{
			Action onAttached = gameEntity.OnAttached;
			if (onAttached != null)
			{
				onAttached();
			}
			GameDock component3 = gameEntity2.GetComponent<GameDock>();
			if (component3 != null)
			{
				component3.OnDock(gameEntity, gameEntity2);
				if (component2 != null)
				{
					component2.OnDock(gameEntity, gameEntity2);
				}
			}
		}
	}

	// Token: 0x06002B8E RID: 11150 RVA: 0x000EA9E4 File Offset: 0x000E8BE4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TryDetachLocal(GameEntity gameEntity)
	{
		if (gameEntity.attachedToEntityId != GameEntityId.Invalid)
		{
			GameEntity gameEntity2 = gameEntity.manager.entities[gameEntity.attachedToEntityId.index];
			if (gameEntity2 != null)
			{
				GameDock component = gameEntity2.GetComponent<GameDock>();
				if (component != null)
				{
					component.OnUndock(gameEntity, gameEntity2);
					GameDockable component2 = gameEntity.GetComponent<GameDockable>();
					if (component2 != null)
					{
						component2.OnUndock(gameEntity, gameEntity2);
					}
				}
			}
		}
		if (gameEntity.attachedToEntityId != GameEntityId.Invalid)
		{
			Action onDetached = gameEntity.OnDetached;
			if (onDetached != null)
			{
				onDetached();
			}
		}
		gameEntity.attachedToEntityId = GameEntityId.Invalid;
	}

	// Token: 0x06002B8F RID: 11151 RVA: 0x000EAA86 File Offset: 0x000E8C86
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void TryDetachCompletely(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return;
		}
		GameEntityManager.TryRemoveFromHandLocal(gameEntity);
		GameEntityManager.TryUnsnapLocal(gameEntity);
		GameEntityManager.TryDetachLocal(gameEntity);
	}

	// Token: 0x06002B90 RID: 11152 RVA: 0x000EAAA4 File Offset: 0x000E8CA4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void TryRemoveFromHandLocal(GameEntity gameEntity)
	{
		GameEntityId id = gameEntity.id;
		int heldByActorNumber = gameEntity.heldByActorNumber;
		GamePlayer gamePlayer;
		if (GamePlayer.TryGetGamePlayer(heldByActorNumber, out gamePlayer))
		{
			if (heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				GamePlayerLocal.instance.ClearGrabbedIfHeld(id, gameEntity.manager);
			}
			gamePlayer.ClearGrabbedIfHeld(id, gameEntity.manager);
			Action onReleased = gameEntity.OnReleased;
			if (onReleased != null)
			{
				onReleased();
			}
		}
		gameEntity.heldByActorNumber = -1;
		gameEntity.heldByHandIndex = -1;
	}

	// Token: 0x06002B91 RID: 11153 RVA: 0x000EA50B File Offset: 0x000E870B
	public void AttachEntityOnCreate(GameEntityId gameEntityId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, int jointType, NetPlayer grabbedByPlayer)
	{
		this.SnapEntityLocal(gameEntityId, isLeftHand, localPosition, localRotation, jointType, grabbedByPlayer);
	}

	// Token: 0x06002B92 RID: 11154 RVA: 0x000EAB18 File Offset: 0x000E8D18
	public void RequestHit(GameHitData hit)
	{
		GameHittable gameComponent = this.GetGameComponent<GameHittable>(hit.hitEntityId);
		if (gameComponent == null)
		{
			return;
		}
		gameComponent.ApplyHit(hit);
		this.photonView.RPC("RequestHitRPC", this.GetAuthorityPlayer(), new object[]
		{
			this.GetNetIdFromEntityId(hit.hitEntityId),
			this.GetNetIdFromEntityId(hit.hitByEntityId),
			hit.hitTypeId,
			hit.hitEntityPosition,
			hit.hitPosition,
			hit.hitImpulse,
			hit.hittablePoint
		});
	}

	// Token: 0x06002B93 RID: 11155 RVA: 0x000EABD0 File Offset: 0x000E8DD0
	[PunRPC]
	public void RequestHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, int hittablePoint, PhotonMessageInfo info)
	{
		float num = 10000f;
		if (entityPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitPosition.IsValid(num2))
			{
				float num3 = 10000f;
				if (hitImpulse.IsValid(num3) && this.IsValidAuthorityRPC(info.Sender, hittableNetId, entityPosition) && this.IsPositionInManagerBounds(hitPosition))
				{
					GamePlayer gamePlayer;
					if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer) || !gamePlayer.netImpulseLimiter.CheckCallTime(Time.time))
					{
						return;
					}
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
					GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
					if (gameComponent == null)
					{
						return;
					}
					GameHitData hitData = new GameHitData
					{
						hitTypeId = hitTypeId,
						hitEntityId = entityIdFromNetId,
						hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
						hitEntityPosition = entityPosition,
						hitPosition = hitPosition,
						hitImpulse = hitImpulse,
						hittablePoint = hittablePoint
					};
					if (!gameComponent.IsHitValid(hitData))
					{
						return;
					}
					base.SendRPC("ApplyHitRPC", RpcTarget.All, new object[]
					{
						hittableNetId,
						hitByNetId,
						hitTypeId,
						entityPosition,
						hitPosition,
						hitImpulse,
						hittablePoint,
						info.Sender
					});
					return;
				}
			}
		}
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x000EAD24 File Offset: 0x000E8F24
	[PunRPC]
	public void ApplyHitRPC(int hittableNetId, int hitByNetId, int hitTypeId, Vector3 entityPosition, Vector3 hitPosition, Vector3 hitImpulse, int hittablePoint, Player player, PhotonMessageInfo info)
	{
		float num = 10000f;
		if (hitPosition.IsValid(num))
		{
			float num2 = 10000f;
			if (hitImpulse.IsValid(num2) && this.IsValidClientRPC(info.Sender, hittableNetId, entityPosition) && !this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.HitEntity) && player != null)
			{
				if (player.IsLocal)
				{
					return;
				}
				if (this.GetGameEntity(this.GetEntityIdFromNetId(hittableNetId)) == null)
				{
					return;
				}
				hitImpulse = Vector3.ClampMagnitude(hitImpulse, 100f);
				GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(hittableNetId);
				GameHitData hitData = new GameHitData
				{
					hitTypeId = hitTypeId,
					hitEntityId = entityIdFromNetId,
					hitByEntityId = this.GetEntityIdFromNetId(hitByNetId),
					hitEntityPosition = entityPosition,
					hitPosition = hitPosition,
					hitImpulse = hitImpulse,
					hitAmount = 0,
					hittablePoint = hittablePoint
				};
				GameEntity gameEntity = this.GetGameEntity(this.GetEntityIdFromNetId(hitByNetId));
				GameHittable gameComponent = this.GetGameComponent<GameHittable>(entityIdFromNetId);
				if (gameEntity != null)
				{
					GameHitter component = gameEntity.GetComponent<GameHitter>();
					if (component != null)
					{
						hitData.hitAmount = component.CalcHitAmount((GameHitType)hitTypeId, gameComponent, gameEntity);
					}
				}
				if (gameComponent != null)
				{
					gameComponent.ApplyHit(hitData);
				}
				return;
			}
		}
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x000EAE58 File Offset: 0x000E9058
	public bool IsPlayerHandNearEntity(GamePlayer player, int entityNetId, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && GameEntityManager.IsPlayerHandNearPosition(player, gameEntity.transform.position, isLeftHand, checkBothHands, acceptableRadius);
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x000EAE98 File Offset: 0x000E9098
	public static bool IsPlayerHandNearPosition(GamePlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 16f)
	{
		bool flag = true;
		if (player != null && player.rig != null)
		{
			if (isLeftHand || checkBothHands)
			{
				flag = ((worldPosition - player.rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius);
			}
			if (!isLeftHand || checkBothHands)
			{
				float sqrMagnitude = (worldPosition - player.rig.rightHandTransform.position).sqrMagnitude;
				flag = (flag && sqrMagnitude < acceptableRadius * acceptableRadius);
			}
		}
		return flag;
	}

	// Token: 0x06002B97 RID: 11159 RVA: 0x000EAF24 File Offset: 0x000E9124
	public bool IsEntityNearEntity(int entityNetId, int otherEntityNetId, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(otherEntityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && this.IsEntityNearPosition(entityNetId, gameEntity.transform.position, acceptableRadius);
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x000EAF60 File Offset: 0x000E9160
	public bool IsEntityNearPosition(int entityNetId, Vector3 position, float acceptableRadius = 16f)
	{
		GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(entityNetId);
		GameEntity gameEntity = this.GetGameEntity(entityIdFromNetId);
		return !(gameEntity == null) && Vector3.SqrMagnitude(gameEntity.transform.position - position) < acceptableRadius * acceptableRadius;
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x0000DAF2 File Offset: 0x0000BCF2
	public static bool ValidateDataType<T>(object obj, out T dataAsType)
	{
		if (obj is T)
		{
			dataAsType = (T)((object)obj);
			return true;
		}
		dataAsType = default(T);
		return false;
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x000EAFA4 File Offset: 0x000E91A4
	private void ClearZone(bool ignoreHeldGadgets = false)
	{
		GamePlayerLocal.instance.DebugSlotsReport(string.Format("Pre ClearZone zone={0}", this.zone));
		this.ClearPendingRPCBatches();
		if (ignoreHeldGadgets)
		{
			List<GameEntity> list = GamePlayerLocal.instance.gamePlayer.HeldAndSnappedEntities(null);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i] == null || list[i].manager != this)
				{
					list.RemoveAt(i);
				}
				else if (list[i].shouldDestroyOnZoneExit)
				{
					list.RemoveAt(i);
				}
			}
			for (int j = this.entities.Count - 1; j >= 0; j--)
			{
				if (!(this.entities[j] == null) && !this.entities[j].IsScenePlaced && !list.Contains(this.entities[j]))
				{
					this.DestroyItemLocal(this.entities[j].id);
				}
			}
			GamePlayerLocal.instance.joinWithItemsSentForCurrentMigration = false;
			GamePlayerLocal.instance.gamePlayer.DidJoinWithItems = false;
			GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone post-preserve zone={0}", this.zone));
		}
		else
		{
			for (int k = 0; k < this.entities.Count; k++)
			{
				if (!(this.entities[k] == null) && !(this.entities[k].manager != this) && !this.entities[k].IsScenePlaced)
				{
					this.DestroyItemLocal(this.entities[k].id);
				}
			}
			GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone post-destroy zone={0}", this.zone));
			GamePlayer gamePlayerRef = VRRig.LocalRig.GamePlayerRef;
			if (gamePlayerRef != null)
			{
				gamePlayerRef.ClearZone(this);
			}
			GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone post-ClearZone(player) zone={0}", this.zone));
		}
		for (int l = 0; l < this.entities.Count; l++)
		{
			if (this.entities[l] != null && this.entities[l].manager != this)
			{
				int key = this.netIds[l];
				int num;
				if (this.netIdToIndex.TryGetValue(key, out num) && num == l)
				{
					this.netIdToIndex.Remove(key);
				}
				this.entities[l] = null;
			}
		}
		foreach (VRRig vrrig in VRRigCache.ActiveRigs)
		{
			GamePlayer component = vrrig.GetComponent<GamePlayer>();
			if (!component.IsLocal())
			{
				component.ClearZone(this);
			}
		}
		this.gameEntityData.Clear();
		this.entitiesActiveCount = 0;
		this.scenePlacedEntitiesRegistered = false;
		this.scenePlacedEntities.Clear();
		for (int m = 0; m < this.zoneComponents.Count; m++)
		{
			this.zoneComponents[m].OnZoneClear(this.zoneClearReason);
		}
		GamePlayerLocal.instance.DebugSlotsReport(string.Format("ClearZone END zone={0}", this.zone));
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x000EB320 File Offset: 0x000E9520
	public int SerializeGameState(int zoneId, byte[] bytes, int maxBytes)
	{
		MemoryStream memoryStream = new MemoryStream(bytes);
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			this.zoneComponents[i].SerializeZoneData(binaryWriter);
		}
		GameEntityManager.tempEntitiesToSerialize.Clear();
		for (int j = 0; j < this.entities.Count; j++)
		{
			GameEntity gameEntity = this.entities[j];
			if (!(gameEntity == null))
			{
				int attachedPlayerActorNr = gameEntity.AttachedPlayerActorNr;
				if (attachedPlayerActorNr != -1)
				{
					bool flag = false;
					for (int k = 0; k < GameEntityManager.tempRigs.Count; k++)
					{
						if (GameEntityManager.tempRigs[k].Creator.ActorNumber == attachedPlayerActorNr)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						goto IL_B7;
					}
				}
				GameEntityManager.tempEntitiesToSerialize.Add(gameEntity);
			}
			IL_B7:;
		}
		binaryWriter.Write(GameEntityManager.tempEntitiesToSerialize.Count);
		for (int l = 0; l < GameEntityManager.tempEntitiesToSerialize.Count; l++)
		{
			GameEntity gameEntity2 = GameEntityManager.tempEntitiesToSerialize[l];
			if (!(gameEntity2 == null))
			{
				int netIdFromEntityId = this.GetNetIdFromEntityId(gameEntity2.id);
				binaryWriter.Write(netIdFromEntityId);
				binaryWriter.Write(gameEntity2.typeId);
				long value = BitPackUtils.PackWorldPosForNetwork(gameEntity2.transform.position);
				int value2 = BitPackUtils.PackQuaternionForNetwork(gameEntity2.transform.rotation);
				binaryWriter.Write(value);
				binaryWriter.Write(value2);
			}
		}
		for (int m = 0; m < GameEntityManager.tempEntitiesToSerialize.Count; m++)
		{
			GameEntity gameEntity3 = GameEntityManager.tempEntitiesToSerialize[m];
			if (!(gameEntity3 == null))
			{
				int netIdFromEntityId2 = this.GetNetIdFromEntityId(gameEntity3.id);
				binaryWriter.Write(netIdFromEntityId2);
				binaryWriter.Write(gameEntity3.createData);
				binaryWriter.Write(this.GetNetIdFromEntityId(gameEntity3.createdByEntityId));
				binaryWriter.Write(gameEntity3.GetState());
				int num = -1;
				GameEntity gameEntity4 = this.GetGameEntity(gameEntity3.attachedToEntityId);
				if (gameEntity4 != null)
				{
					num = this.GetNetIdFromEntityId(gameEntity4.id);
				}
				binaryWriter.Write(num);
				if (num != -1)
				{
					long value3 = BitPackUtils.PackHandPosRotForNetwork(gameEntity3.transform.localPosition, gameEntity3.transform.localRotation);
					binaryWriter.Write(value3);
				}
				GameAgent component = gameEntity3.GetComponent<GameAgent>();
				bool flag2 = component != null;
				binaryWriter.Write(flag2);
				if (flag2)
				{
					Vector3 worldPos = Vector3.zero;
					if (component.navAgent != null)
					{
						worldPos = component.navAgent.destination;
					}
					long value4 = BitPackUtils.PackWorldPosForNetwork(worldPos);
					binaryWriter.Write(value4);
					NetPlayer targetPlayer = component.targetPlayer;
					int value5 = (targetPlayer != null) ? targetPlayer.ActorNumber : -1;
					binaryWriter.Write(value5);
				}
				byte b = (byte)gameEntity3.entitySerialize.Count;
				binaryWriter.Write(b);
				for (int n = 0; n < (int)b; n++)
				{
					gameEntity3.entitySerialize[n].OnGameEntitySerialize(binaryWriter);
				}
				for (int num2 = 0; num2 < this.zoneComponents.Count; num2++)
				{
					this.zoneComponents[num2].SerializeZoneEntityData(binaryWriter, gameEntity3);
				}
			}
		}
		int count = GameEntityManager.tempRigs.Count;
		binaryWriter.Write(count);
		for (int num3 = 0; num3 < GameEntityManager.tempRigs.Count; num3++)
		{
			VRRig vrrig = GameEntityManager.tempRigs[num3];
			NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
			binaryWriter.Write(owningNetPlayer.ActorNumber);
			GamePlayer gamePlayerRef = vrrig.GamePlayerRef;
			bool flag3 = gamePlayerRef != null;
			binaryWriter.Write(flag3);
			if (flag3)
			{
				gamePlayerRef.SerializeNetworkState(binaryWriter, owningNetPlayer, this);
				for (int num4 = 0; num4 < this.zoneComponents.Count; num4++)
				{
					this.zoneComponents[num4].SerializeZonePlayerData(binaryWriter, owningNetPlayer.ActorNumber);
				}
			}
		}
		return (int)memoryStream.Position;
	}

	// Token: 0x06002B9C RID: 11164 RVA: 0x000EB708 File Offset: 0x000E9908
	public unsafe void DeserializeTableState(byte[] bytes, int numBytes)
	{
		if (numBytes <= 0)
		{
			return;
		}
		GameEntityManager.tempAttachments.Clear();
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				for (int i = 0; i < this.zoneComponents.Count; i++)
				{
					this.zoneComponents[i].DeserializeZoneData(binaryReader);
				}
				int num = binaryReader.ReadInt32();
				int num2 = num;
				Span<bool> span = new Span<bool>(stackalloc byte[(UIntPtr)num2], num2);
				for (int j = 0; j < num; j++)
				{
					int netId = binaryReader.ReadInt32();
					int entityTypeId = binaryReader.ReadInt32();
					long data = binaryReader.ReadInt64();
					int data2 = binaryReader.ReadInt32();
					Vector3 position = BitPackUtils.UnpackWorldPosFromNetwork(data);
					Quaternion rotation = BitPackUtils.UnpackQuaternionFromNetwork(data2);
					GameEntity gameEntityFromNetId = this.GetGameEntityFromNetId(netId);
					if (gameEntityFromNetId != null)
					{
						*span[j] = true;
						if (gameEntityFromNetId.IsScenePlaced)
						{
							gameEntityFromNetId.transform.SetPositionAndRotation(position, rotation);
						}
					}
					else if (GameEntityManager.IsScenePlacedNetId(netId))
					{
						*span[j] = true;
					}
					else
					{
						this.CreateItemLocal(netId, entityTypeId, position, rotation);
					}
				}
				int k = 0;
				while (k < num)
				{
					int num3 = binaryReader.ReadInt32();
					long createData = binaryReader.ReadInt64();
					int createdByEntityNetId = binaryReader.ReadInt32();
					long state = binaryReader.ReadInt64();
					GameEntity gameEntityFromNetId2 = this.GetGameEntityFromNetId(num3);
					if (gameEntityFromNetId2 != null)
					{
						if (!(*span[k]))
						{
							this.InitItemLocal(gameEntityFromNetId2, createData, createdByEntityNetId);
							gameEntityFromNetId2.SetState(state);
						}
						else if (gameEntityFromNetId2.IsScenePlaced)
						{
							gameEntityFromNetId2.SetState(state);
						}
					}
					int num4 = binaryReader.ReadInt32();
					if (num4 == -1)
					{
						goto IL_1DA;
					}
					long data3 = binaryReader.ReadInt64();
					if (!(gameEntityFromNetId2 == null))
					{
						Vector3 localPosition;
						Quaternion localRotation;
						BitPackUtils.UnpackHandPosRotFromNetwork(data3, out localPosition, out localRotation);
						GameEntityManager.tempAttachments.Add(new GameEntityManager.AttachmentData
						{
							entityNetId = num3,
							attachToEntityNetId = num4,
							localPosition = localPosition,
							localRotation = localRotation
						});
						goto IL_1DA;
					}
					IL_290:
					k++;
					continue;
					IL_1DA:
					if (binaryReader.ReadBoolean())
					{
						long data4 = binaryReader.ReadInt64();
						int playerID = binaryReader.ReadInt32();
						Vector3 destination = BitPackUtils.UnpackWorldPosFromNetwork(data4);
						GameAgent component = gameEntityFromNetId2.GetComponent<GameAgent>();
						if (component != null)
						{
							if (component.IsOnNavMesh())
							{
								component.navAgent.destination = destination;
							}
							component.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
						}
					}
					byte b = binaryReader.ReadByte();
					for (int l = 0; l < (int)b; l++)
					{
						gameEntityFromNetId2.entitySerialize[l].OnGameEntityDeserialize(binaryReader);
					}
					for (int m = 0; m < this.zoneComponents.Count; m++)
					{
						this.zoneComponents[m].DeserializeZoneEntityData(binaryReader, gameEntityFromNetId2);
					}
					goto IL_290;
				}
				int num5 = binaryReader.ReadInt32();
				for (int n = 0; n < num5; n++)
				{
					int actorNumber = binaryReader.ReadInt32();
					if (binaryReader.ReadBoolean())
					{
						GamePlayer gamePlayer;
						GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer);
						GamePlayer.DeserializeNetworkState(binaryReader, gamePlayer, this);
						for (int num6 = 0; num6 < this.zoneComponents.Count; num6++)
						{
							this.zoneComponents[num6].DeserializeZonePlayerData(binaryReader, actorNumber);
						}
					}
				}
				for (int num7 = 0; num7 < GameEntityManager.tempAttachments.Count; num7++)
				{
					GameEntityManager.AttachmentData attachmentData = GameEntityManager.tempAttachments[num7];
					GameEntityId entityIdFromNetId = this.GetEntityIdFromNetId(attachmentData.entityNetId);
					GameEntityId entityIdFromNetId2 = this.GetEntityIdFromNetId(attachmentData.attachToEntityNetId);
					if (!(entityIdFromNetId == entityIdFromNetId2))
					{
						this.AttachEntityLocal(entityIdFromNetId, entityIdFromNetId2, 0, attachmentData.localPosition, attachmentData.localRotation);
					}
				}
			}
		}
	}

	// Token: 0x06002B9D RID: 11165 RVA: 0x000EBAD4 File Offset: 0x000E9CD4
	private void UpdateZoneState()
	{
		this.UpdateAuthority(GameEntityManager.tempRigs);
		if (this.IsAuthority())
		{
			this.UpdateClientsFromAuthority(GameEntityManager.tempRigs);
			this.UpdateZoneStateAuthority();
		}
		else
		{
			this.UpdateZoneStateClient();
		}
		for (int i = this.zoneStateData.zonePlayers.Count - 1; i >= 0; i--)
		{
			if (this.zoneStateData.zonePlayers[i] == null)
			{
				this.zoneStateData.zonePlayers.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x000EBB50 File Offset: 0x000E9D50
	private void UpdateAuthority(List<VRRig> allRigs)
	{
		if (!PhotonNetwork.InRoom && base.IsMine)
		{
			if (!this.IsAuthority())
			{
				this.guard.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
		}
		else if (this.IsAuthority() && !this.IsInZone())
		{
			Player player = null;
			GTZone currentZone = VRRig.LocalRig.zoneEntity.currentZone;
			if (this.useRandomCheckForAuthority)
			{
				int num = 0;
				while (player == null)
				{
					if (num >= 10)
					{
						break;
					}
					num++;
					int index = Random.Range(0, allRigs.Count);
					VRRig vrrig = allRigs[index];
					GamePlayer gamePlayer;
					if (GamePlayer.TryGetGamePlayer(vrrig, out gamePlayer) && !(gamePlayer.rig == null) && gamePlayer.rig.Creator != null && !gamePlayer.rig.isLocal)
					{
						GTZone currentZone2 = vrrig.zoneEntity.currentZone;
						if (currentZone2 == this.zone && currentZone2 != currentZone)
						{
							player = gamePlayer.rig.Creator.GetPlayerRef();
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < allRigs.Count; i++)
				{
					VRRig vrrig2 = allRigs[i];
					GamePlayer gamePlayer2;
					if (GamePlayer.TryGetGamePlayer(vrrig2, out gamePlayer2) && !(gamePlayer2.rig == null) && gamePlayer2.rig.Creator != null && !gamePlayer2.rig.isLocal)
					{
						GTZone currentZone3 = vrrig2.zoneEntity.currentZone;
						if (currentZone3 == this.zone && currentZone3 != currentZone)
						{
							player = gamePlayer2.rig.Creator.GetPlayerRef();
						}
					}
				}
			}
			if (player != null)
			{
				this.guard.TransferOwnership(player, "");
			}
		}
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x000EBCF8 File Offset: 0x000E9EF8
	private void UpdateClientsFromAuthority(List<VRRig> allRigs)
	{
		if (!this.IsInZone())
		{
			return;
		}
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			GameEntityManager.ZoneStateRequest zoneStateRequest = this.zoneStateData.zoneStateRequests[i];
			if (zoneStateRequest.player != null && zoneStateRequest.zone == this.zone)
			{
				this.SendZoneStateToPlayerOrTarget(zoneStateRequest.zone, zoneStateRequest.player, RpcTarget.MasterClient);
				zoneStateRequest.completed = true;
				this.zoneStateData.zoneStateRequests[i] = zoneStateRequest;
				this.zoneStateData.zoneStateRequests.RemoveAt(i);
				return;
			}
			this.zoneStateData.zoneStateRequests.RemoveAt(i);
			i--;
		}
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x000EBDB0 File Offset: 0x000E9FB0
	public void TestSerializeTableState()
	{
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		int num = this.SerializeGameState((int)this.zone, this.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		Debug.LogFormat("Test Serialize Game State Buffer Size Uncompressed {0}", new object[]
		{
			num
		});
		Debug.LogFormat("Test Serialize Game State Buffer Size Compressed {0}", new object[]
		{
			array.Length
		});
	}

	// Token: 0x06002BA1 RID: 11169 RVA: 0x000EBE20 File Offset: 0x000EA020
	public static void ClearByteBuffer(byte[] buffer)
	{
		int num = buffer.Length;
		for (int i = 0; i < num; i++)
		{
			buffer[i] = 0;
		}
	}

	// Token: 0x06002BA2 RID: 11170 RVA: 0x000EBE44 File Offset: 0x000EA044
	private void SendZoneStateToPlayerOrTarget(GTZone zone, Player player, RpcTarget target)
	{
		GameEntityManager.ClearByteBuffer(this.tempSerializeGameState);
		this.SerializeGameState((int)zone, this.tempSerializeGameState, 15360);
		byte[] array = GZipStream.CompressBuffer(this.tempSerializeGameState);
		byte[] array2 = new byte[512];
		int i = 0;
		int num = 0;
		int num2 = array.Length;
		while (i < num2)
		{
			int num3 = Mathf.Min(512, num2 - i);
			Array.Copy(array, i, array2, 0, num3);
			if (player != null)
			{
				this.photonView.RPC("SendTableDataRPC", player, new object[]
				{
					num,
					num2,
					array2
				});
			}
			else
			{
				this.photonView.RPC("SendTableDataRPC", target, new object[]
				{
					num,
					num2,
					array2
				});
			}
			i += num3;
			num++;
		}
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x000EBF24 File Offset: 0x000EA124
	[PunRPC]
	public void SendTableDataRPC(int packetNum, int totalBytes, byte[] bytes, PhotonMessageInfo info)
	{
		if (!this.IsAuthorityPlayer(info.Sender) || this.m_RpcSpamChecks.IsSpamming(GameEntityManager.RPC.SendTableData) || bytes == null || bytes.Length >= 15360)
		{
			return;
		}
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		if (packetNum == 0)
		{
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
		}
		Array.Copy(bytes, 0, this.zoneStateData.recievedStateBytes, this.zoneStateData.numRecievedStateBytes, bytes.Length);
		this.zoneStateData.numRecievedStateBytes += bytes.Length;
		if (this.zoneStateData.numRecievedStateBytes >= totalBytes)
		{
			if (this.superInfectionManager != null && this.superInfectionManager.zoneSuperInfection == null && !this.scenePlacedEntitiesRegistered)
			{
				this.PendingTableData = true;
				this.pendingTableDataSetFrame = Time.frameCount;
				return;
			}
			this.ResolveTableData();
		}
	}

	// Token: 0x06002BA4 RID: 11172 RVA: 0x000EC028 File Offset: 0x000EA228
	public void ResolveTableData()
	{
		this.PendingTableData = false;
		if (GameEntityManager.activeManager.IsNotNull() && GameEntityManager.activeManager != this)
		{
			GameEntityManager.activeManager.zoneClearReason = ZoneClearReason.MigrateGameEntityZone;
			GameEntityManager.activeManager.ClearZone(true);
		}
		this.ClearZone(true);
		this.RegisterScenePlacedEntities();
		try
		{
			byte[] array = GZipStream.UncompressBuffer(this.zoneStateData.recievedStateBytes);
			int numBytes = array.Length;
			this.DeserializeTableState(array, numBytes);
			this.RecalculateNextNetId();
			this.SetZoneState(GameEntityManager.ZoneState.Active);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			Debug.LogError("[GT/GameEntityManager]  ERROR!!!  ResolveTableData: See exception in previous message.");
		}
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x000EC0C8 File Offset: 0x000EA2C8
	private void UpdateZoneStateAuthority()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone())
		{
			if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				return;
			}
			if (this.entitiesActiveCount > 0 && this.ShouldClearZone())
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.ClearZone(false);
				return;
			}
		}
		GameEntityManager.ZoneState state = this.zoneStateData.state;
		if (state > GameEntityManager.ZoneState.WaitingForState)
		{
			return;
		}
		bool flag = this.IsInZone();
		bool inRoom = PhotonNetwork.InRoom;
		bool flag2 = GameEntityManager.HasAnyScenePlacedInScene(this.GetZoneSceneName());
		bool flag3 = this.scenePlacedEntitiesRegistered;
		int num = (int)(this.zoneStateData.state | (GameEntityManager.ZoneState)((flag ? 1 : 0) << 4) | (GameEntityManager.ZoneState)((inRoom ? 1 : 0) << 5) | (GameEntityManager.ZoneState)((flag2 ? 1 : 0) << 6) | (GameEntityManager.ZoneState)((flag3 ? 1 : 0) << 7) | (GameEntityManager.ZoneState)(this.entities.Count << 8) | (GameEntityManager.ZoneState)(this.zoneComponents.Count << 20));
		if (num != this._lastUpdateZoneStateAuthLogSig)
		{
			this._lastUpdateZoneStateAuthLogSig = num;
		}
		if (flag && inRoom)
		{
			this.SetZoneState(GameEntityManager.ZoneState.Active);
			for (int i = 0; i < this.zoneComponents.Count; i++)
			{
				this.zoneComponents[i].OnZoneCreate();
			}
		}
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x000EC220 File Offset: 0x000EA420
	private void UpdateZoneStateClient()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		if (gamePlayer == null || gamePlayer.rig == null || gamePlayer.rig.OwningNetPlayer == null)
		{
			return;
		}
		if (!this.IsInZone())
		{
			if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
				return;
			}
			if (this.entities.Count > 0 && this.ShouldClearZone())
			{
				this.zoneClearReason = ZoneClearReason.LeaveZone;
				this.ClearZone(true);
				return;
			}
		}
		GameEntityManager.ZoneState state = this.zoneStateData.state;
		if (state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			if (state != GameEntityManager.ZoneState.WaitingToRequestState)
			{
				return;
			}
			if (Time.timeAsDouble - this.zoneStateData.stateStartTime > 1.0)
			{
				this.RecalculateNextNetId();
				List<GameEntity> list = GamePlayerLocal.instance.gamePlayer.HeldAndSnappedEntities(null);
				this.SetZoneState(GameEntityManager.ZoneState.WaitingForState);
				this.photonView.RPC("RequestZoneStateRPC", this.GetAuthorityPlayer(), new object[]
				{
					(int)this.zone
				});
				this.JoinWithItems(list);
				GamePlayerLocal.instance.joinWithItemsSentForCurrentMigration = true;
			}
		}
		else if (this.HasAuthority() && this.IsInZone() && !this.IsAuthority())
		{
			this.SetZoneState(GameEntityManager.ZoneState.WaitingToRequestState);
			return;
		}
	}

	// Token: 0x06002BA7 RID: 11175 RVA: 0x000EC360 File Offset: 0x000EA560
	protected virtual bool IsInZone()
	{
		if (GorillaComputer.instance.IsPlayerInVirtualStump() && GameEntityManager.IsSuppressZonesInVStumpEnabled())
		{
			return CustomMapLoader.CanLoadEntities && this.zone == GTZone.customMaps;
		}
		bool flag = VRRig.LocalRig.zoneEntity.currentZone == this.zone;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag &= this.zoneComponents[i].IsZoneReady();
		}
		return flag;
	}

	// Token: 0x06002BA8 RID: 11176 RVA: 0x000EC3DC File Offset: 0x000EA5DC
	private bool ShouldClearZone()
	{
		bool flag = false;
		for (int i = 0; i < this.zoneComponents.Count; i++)
		{
			flag |= this.zoneComponents[i].ShouldClearZone();
		}
		return flag;
	}

	// Token: 0x06002BA9 RID: 11177 RVA: 0x000EC418 File Offset: 0x000EA618
	private void SetZoneState(GameEntityManager.ZoneState newState)
	{
		if (newState == this.zoneStateData.state)
		{
			return;
		}
		this.zoneStateData.state = newState;
		this.zoneStateData.stateStartTime = Time.timeAsDouble;
		switch (this.zoneStateData.state)
		{
		case GameEntityManager.ZoneState.WaitingToEnterZone:
		{
			bool flag = this.ShouldClearZone();
			bool flag2 = this.zoneClearReason == ZoneClearReason.MigrateGameEntityZone;
			bool flag3 = this.zoneClearReason == ZoneClearReason.Disconnect;
			bool ignoreHeldGadgets = !flag && !flag2 && !flag3;
			if (flag3 && GameEntityManager.activeManager == this)
			{
				GameEntityManager.activeManager = null;
				GamePlayerLocal.instance.currGameEntityManager = null;
			}
			if (!this.IsAuthority())
			{
				this.photonView.RPC("PlayerLeftZoneRPC", this.GetAuthorityPlayer(), Array.Empty<object>());
			}
			this.ClearZone(ignoreHeldGadgets);
			return;
		}
		case GameEntityManager.ZoneState.WaitingToRequestState:
			break;
		case GameEntityManager.ZoneState.WaitingForState:
			this.zoneStateData.numRecievedStateBytes = 0;
			for (int i = 0; i < this.zoneStateData.recievedStateBytes.Length; i++)
			{
				this.zoneStateData.recievedStateBytes[i] = 0;
			}
			this.RegisterScenePlacedEntities();
			if (this.scenePlacedEntities.Count > 0 && GamePlayerLocal.instance != null && GamePlayerLocal.instance.currGameEntityManager != this)
			{
				GamePlayerLocal.instance.currGameEntityManager = this;
				GamePlayerLocal.instance.pendingFullMigration = true;
				return;
			}
			break;
		case GameEntityManager.ZoneState.Active:
		{
			if (GameEntityManager.activeManager == this)
			{
				for (int j = 0; j < this.zoneComponents.Count; j++)
				{
					try
					{
						this.zoneComponents[j].OnZoneInit();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
				this.RegisterScenePlacedEntities();
				return;
			}
			GameEntityManager activeManager = GameEntityManager.activeManager;
			GameEntityManager.activeManager = this;
			for (int k = 0; k < this.zoneComponents.Count; k++)
			{
				try
				{
					this.zoneComponents[k].OnZoneInit();
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
				}
			}
			this.RegisterScenePlacedEntities();
			GamePlayerLocal.instance.MigrateToEntityManager(this);
			if (activeManager.IsNotNull())
			{
				activeManager.zoneClearReason = ZoneClearReason.MigrateGameEntityZone;
				activeManager.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06002BAA RID: 11178 RVA: 0x000EC650 File Offset: 0x000EA850
	public void DebugSendState()
	{
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToRequestState);
	}

	// Token: 0x06002BAB RID: 11179 RVA: 0x000EC65C File Offset: 0x000EA85C
	[PunRPC]
	public void RequestZoneStateRPC(int zoneId, PhotonMessageInfo info)
	{
		int actorNumber = info.Sender.ActorNumber;
		if (!this.IsAuthority())
		{
			return;
		}
		if (zoneId != (int)this.zone || this.zoneStateData.zoneStateRequests == null)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(info.Sender, out gamePlayer))
		{
			return;
		}
		if (!gamePlayer.newJoinZoneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.playerZoneJoinTimes[actorNumber] = Time.unscaledTime;
		for (int i = 0; i < this.zoneStateData.zoneStateRequests.Count; i++)
		{
			Player player = this.zoneStateData.zoneStateRequests[i].player;
			if (player != null && player.ActorNumber == actorNumber)
			{
				return;
			}
		}
		this.zoneStateData.zoneStateRequests.Add(new GameEntityManager.ZoneStateRequest
		{
			player = info.Sender,
			zone = this.zone,
			completed = false
		});
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x000EC747 File Offset: 0x000EA947
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.superInfectionManager != null)
		{
			this.superInfectionManager.WriteDataPUN(stream, info);
		}
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x000EC764 File Offset: 0x000EA964
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.superInfectionManager != null)
		{
			this.superInfectionManager.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x000EC781 File Offset: 0x000EA981
	private void OnNetworkJoinedRoom()
	{
		GameEntityManager.HasAnyScenePlacedInScene(this.GetZoneSceneName());
		this.zoneClearReason = ZoneClearReason.JoinZone;
		this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x000EC7A0 File Offset: 0x000EA9A0
	private void OnNetworkLeftRoom()
	{
		for (int i = 0; i < this.entities.Count; i++)
		{
		}
		this.zoneClearReason = ZoneClearReason.Disconnect;
		if (this.zoneStateData.state != GameEntityManager.ZoneState.WaitingToEnterZone)
		{
			this.SetZoneState(GameEntityManager.ZoneState.WaitingToEnterZone);
		}
		else
		{
			if (GameEntityManager.activeManager == this)
			{
				GameEntityManager.activeManager = null;
				GamePlayerLocal.instance.currGameEntityManager = null;
			}
			this.ClearZone(false);
		}
		this.playerZoneJoinTimes.Clear();
	}

	// Token: 0x06002BB2 RID: 11186 RVA: 0x000EC814 File Offset: 0x000EAA14
	private void OnNetworkPlayerLeft(NetPlayer leavingPlayer)
	{
		int num = 0;
		foreach (GameEntity gameEntity in this.entities)
		{
			if (gameEntity != null && gameEntity.IsAttachedToPlayer(leavingPlayer))
			{
				num++;
			}
		}
		this.playerZoneJoinTimes.Remove(leavingPlayer.ActorNumber);
	}

	// Token: 0x06002BB3 RID: 11187 RVA: 0x000EC88C File Offset: 0x000EAA8C
	public void OnRigDeactivated(RigContainer container)
	{
		GamePlayer component = container.GetComponent<GamePlayer>();
		int? num;
		if (component == null)
		{
			num = null;
		}
		else
		{
			VRRig rig = component.rig;
			if (rig == null)
			{
				num = null;
			}
			else
			{
				NetPlayer owningNetPlayer = rig.OwningNetPlayer;
				num = ((owningNetPlayer != null) ? new int?(owningNetPlayer.ActorNumber) : null);
			}
		}
		int? num2 = num;
		if (num2 != null)
		{
			num2.GetValueOrDefault();
		}
		if (this != GameEntityManager.activeManager)
		{
			int num3 = 0;
			foreach (GameEntity gameEntity in this.entities)
			{
				if (gameEntity != null)
				{
					GameEntity gameEntity2 = gameEntity;
					NetPlayer player;
					if (component == null)
					{
						player = null;
					}
					else
					{
						VRRig rig2 = component.rig;
						player = ((rig2 != null) ? rig2.OwningNetPlayer : null);
					}
					if (gameEntity2.IsAttachedToPlayer(player))
					{
						num3++;
					}
				}
			}
			return;
		}
		if (component != null)
		{
			List<GameEntityId> list = component.HeldAndSnappedItems(this);
			this._leavingItemScratch.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				GameEntity gameEntity3 = this.GetGameEntity(list[i]);
				if (gameEntity3 != null && gameEntity3.IsScenePlaced)
				{
					this.ReleaseScenePlacedHold(gameEntity3);
					GameEntityManager.ScenePlacedRecord scenePlacedRecord;
					if (this.IsAuthority() && this.TryGetScenePlacedRecord(gameEntity3, out scenePlacedRecord))
					{
						GameEntityManager.ResetScenePlacedTransform(gameEntity3, scenePlacedRecord);
					}
				}
				else if (this.IsAuthority())
				{
					this._leavingItemScratch.Add(list[i]);
				}
			}
			if (this.IsAuthority() && this._leavingItemScratch.Count > 0)
			{
				this.RequestDestroyItems(this._leavingItemScratch);
			}
			this._leavingItemScratch.Clear();
		}
		component.ResetData();
	}

	// Token: 0x06002BB4 RID: 11188 RVA: 0x000ECA44 File Offset: 0x000EAC44
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null || !toPlayer.IsLocal)
		{
			return;
		}
		if (fromPlayer == null || fromPlayer.InRoom)
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(fromPlayer.ActorNumber, out gamePlayer))
		{
			return;
		}
		this._leavingItemScratch.Clear();
		foreach (GameEntityId item in gamePlayer.IterateHeldAndSnappedItems(this))
		{
			this._leavingItemScratch.Add(item);
		}
		for (int i = 0; i < this._leavingItemScratch.Count; i++)
		{
			GameEntityId gameEntityId = this._leavingItemScratch[i];
			GameEntity gameEntity = this.GetGameEntity(gameEntityId);
			if (gameEntity != null && gameEntity.IsScenePlaced)
			{
				this.ReleaseScenePlacedHold(gameEntity);
				GameEntityManager.ScenePlacedRecord scenePlacedRecord;
				if (this.IsAuthority() && this.TryGetScenePlacedRecord(gameEntity, out scenePlacedRecord))
				{
					GameEntityManager.ResetScenePlacedTransform(gameEntity, scenePlacedRecord);
				}
			}
			else
			{
				if (!this.netIdsForDelete.Contains(this.GetNetIdFromEntityId(gameEntityId)))
				{
					this.netIdsForDelete.Add(this.GetNetIdFromEntityId(gameEntityId));
				}
				this.DestroyItemLocal(gameEntityId);
			}
		}
		this._leavingItemScratch.Clear();
		Action onPlayerLeftZone = gamePlayer.OnPlayerLeftZone;
		if (onPlayerLeftZone == null)
		{
			return;
		}
		onPlayerLeftZone();
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return false;
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x00002076 File Offset: 0x00000276
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return false;
	}

	// Token: 0x06002BB8 RID: 11192 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x000ECB84 File Offset: 0x000EAD84
	public void RefreshRigList()
	{
		GameEntityManager.tempRigs.Clear();
		GameEntityManager.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GameEntityManager.tempRigs);
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x000ECBAE File Offset: 0x000EADAE
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitSceneUnloadHandler()
	{
		SceneManager.sceneUnloaded += GameEntityManager.OnZoneSceneUnloaded;
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000ECBC4 File Offset: 0x000EADC4
	private static void OnZoneSceneUnloaded(Scene scene)
	{
		List<GameEntity> list;
		if (!GameEntityManager.s_scenePlacedEntities.TryGetValue(scene.name, out list))
		{
			return;
		}
		for (int i = list.Count - 1; i >= 0; i--)
		{
			GameEntity gameEntity = list[i];
			if (!(gameEntity == null))
			{
				UnityEngine.Object.Destroy(gameEntity.gameObject);
			}
		}
		GameEntityManager.s_scenePlacedEntities.Remove(scene.name);
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000ECC28 File Offset: 0x000EAE28
	private string GetZoneSceneName()
	{
		string text = (ZoneManagement.instance != null) ? ZoneManagement.instance.GetSceneNameForZone(this.zone) : null;
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return base.gameObject.scene.name;
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000ECC74 File Offset: 0x000EAE74
	internal static bool HasAnyScenePlacedInScene(string sceneName)
	{
		List<GameEntity> list;
		return GameEntityManager.s_scenePlacedEntities.TryGetValue(sceneName, out list) && list.Count > 0;
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x000ECC9C File Offset: 0x000EAE9C
	internal static void RegisterScenePlacedEntity(GameEntity entity)
	{
		string name = entity.gameObject.scene.name;
		List<GameEntity> list;
		if (!GameEntityManager.s_scenePlacedEntities.TryGetValue(name, out list))
		{
			list = new List<GameEntity>(8);
			GameEntityManager.s_scenePlacedEntities[name] = list;
		}
		if (!list.Contains(entity))
		{
			list.Add(entity);
			GameEntityManager.NotifyManagersOfLateScenePlacedEntity(entity, name);
		}
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x000ECCF8 File Offset: 0x000EAEF8
	private static void NotifyManagersOfLateScenePlacedEntity(GameEntity entity, string sceneName)
	{
		foreach (KeyValuePair<int, GameEntityManager> keyValuePair in GameEntityManager.managersByZone)
		{
			GameEntityManager value = keyValuePair.Value;
			if (!(value == null) && value.scenePlacedEntitiesRegistered && !(value.GetZoneSceneName() != sceneName))
			{
				value.RegisterSingleScenePlacedEntity(entity);
			}
		}
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x000ECD74 File Offset: 0x000EAF74
	internal static void UnregisterScenePlacedEntity(GameEntity entity)
	{
		int num = (entity.manager != null) ? entity.GetNetId() : 0;
		string text;
		string key;
		if (num != 0 && GameEntityManager.s_scenePlacedHomeScenes.TryGetValue(num, out text))
		{
			key = text;
			GameEntityManager.s_scenePlacedHomeScenes.Remove(num);
		}
		else
		{
			key = entity.gameObject.scene.name;
		}
		List<GameEntity> list;
		if (GameEntityManager.s_scenePlacedEntities.TryGetValue(key, out list))
		{
			list.Remove(entity);
			if (list.Count == 0)
			{
				GameEntityManager.s_scenePlacedEntities.Remove(key);
			}
		}
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x000ECDFB File Offset: 0x000EAFFB
	internal static bool IsScenePlacedNetId(int netId)
	{
		return netId < -1 && netId != int.MinValue;
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x000ECE10 File Offset: 0x000EB010
	public static int NetIdFromXSceneRefId(int uniqueId)
	{
		int num = -uniqueId;
		if (num == -1)
		{
			num = -2;
		}
		return num;
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x000ECE28 File Offset: 0x000EB028
	internal static int ComputeNetIdFromHierarchyForCustomMaps(Transform t)
	{
		int num = t.gameObject.scene.name.GetStaticHash();
		Transform transform = t;
		while (transform != null)
		{
			num = StaticHash.Compute(num, transform.name.GetStaticHash());
			transform = transform.parent;
		}
		if (num > 0)
		{
			num = -num;
		}
		if (num == 0 || num == -1 || num == -2147483648)
		{
			num = -2;
		}
		return num;
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x040037C2 RID: 14274
	private const string preLog = "[GT/GameEntityManager]  ";

	// Token: 0x040037C3 RID: 14275
	private const string preErr = "[GT/GameEntityManager]  ERROR!!!  ";

	// Token: 0x040037C4 RID: 14276
	private const string preErrBeta = "[GT/GameEntityManager]  ERROR!!!  (beta only log) ";

	// Token: 0x040037C5 RID: 14277
	private const int MAX_STATE_BYTES = 15360;

	// Token: 0x040037C6 RID: 14278
	private const int MAX_CHUNK_BYTES = 512;

	// Token: 0x040037C7 RID: 14279
	private const int MAX_JOINWITHITEMS_BYTES = 255;

	// Token: 0x040037C8 RID: 14280
	public const float MAX_LOCAL_MAGNITUDE_SQ = 6400f;

	// Token: 0x040037C9 RID: 14281
	public const float MAX_DISTANCE_FROM_HAND = 16f;

	// Token: 0x040037CA RID: 14282
	public const float MAX_ENTITY_DIST = 16f;

	// Token: 0x040037CB RID: 14283
	public const float MAX_THROW_SPEED_SQ = 1600f;

	// Token: 0x040037CC RID: 14284
	public const int MAX_ENTITY_COUNT_PER_TYPE = 100;

	// Token: 0x040037CD RID: 14285
	public const int INVALID_ID = -1;

	// Token: 0x040037CE RID: 14286
	public const int INVALID_INDEX = -1;

	// Token: 0x040037CF RID: 14287
	private static List<GameEntityManager> allManagers = new List<GameEntityManager>(8);

	// Token: 0x040037D0 RID: 14288
	internal static readonly Dictionary<int, GameEntityManager> managersByZone = new Dictionary<int, GameEntityManager>(8);

	// Token: 0x040037D1 RID: 14289
	public GTZone zone;

	// Token: 0x040037D2 RID: 14290
	public PhotonView photonView;

	// Token: 0x040037D3 RID: 14291
	public RequestableOwnershipGuard guard;

	// Token: 0x040037D4 RID: 14292
	public Player prevAuthorityPlayer;

	// Token: 0x040037D5 RID: 14293
	[FormerlySerializedAs("zoneLimit")]
	public BoxCollider boundsBoxCollider;

	// Token: 0x040037D6 RID: 14294
	public bool useRandomCheckForAuthority;

	// Token: 0x040037D7 RID: 14295
	public GameAgentManager gameAgentManager;

	// Token: 0x040037D8 RID: 14296
	public GhostReactorManager ghostReactorManager;

	// Token: 0x040037D9 RID: 14297
	public CustomMapsGameManager customMapsManager;

	// Token: 0x040037DA RID: 14298
	public SuperInfectionManager superInfectionManager;

	// Token: 0x040037DB RID: 14299
	protected List<IGameEntityZoneComponent> zoneComponents;

	// Token: 0x040037DC RID: 14300
	private List<GameEntity> entities;

	// Token: 0x040037DD RID: 14301
	private int entitiesActiveCount;

	// Token: 0x040037DE RID: 14302
	private List<GameEntityData> gameEntityData;

	// Token: 0x040037DF RID: 14303
	public List<GameEntity> tempFactoryItems;

	// Token: 0x040037E2 RID: 14306
	private Dictionary<int, GameObject> itemPrefabFactory;

	// Token: 0x040037E3 RID: 14307
	private Dictionary<int, int> priceLookupByEntityId;

	// Token: 0x040037E4 RID: 14308
	private List<GameEntity> tempEntities = new List<GameEntity>();

	// Token: 0x040037E5 RID: 14309
	private List<int> netIdsForCreate;

	// Token: 0x040037E6 RID: 14310
	private List<int> entityTypeIdsForCreate;

	// Token: 0x040037E7 RID: 14311
	private List<int> packedRotationsForCreate;

	// Token: 0x040037E8 RID: 14312
	private List<long> packedPositionsForCreate;

	// Token: 0x040037E9 RID: 14313
	private List<long> createDataForCreate;

	// Token: 0x040037EA RID: 14314
	private List<int> createdByEntityNetIdForCreate;

	// Token: 0x040037EB RID: 14315
	private float createCooldown = 0.24f;

	// Token: 0x040037EC RID: 14316
	private float lastCreateSent;

	// Token: 0x040037ED RID: 14317
	private List<int> netIdsForDelete;

	// Token: 0x040037EE RID: 14318
	private float destroyCooldown = 0.25f;

	// Token: 0x040037EF RID: 14319
	private float lastDestroySent;

	// Token: 0x040037F0 RID: 14320
	private List<int> netIdsForState;

	// Token: 0x040037F1 RID: 14321
	private List<long> statesForState;

	// Token: 0x040037F2 RID: 14322
	private float lastStateSent;

	// Token: 0x040037F3 RID: 14323
	private float stateCooldown;

	// Token: 0x040037F4 RID: 14324
	private Dictionary<int, int> netIdToIndex;

	// Token: 0x040037F5 RID: 14325
	private NativeArray<int> netIds;

	// Token: 0x040037F6 RID: 14326
	private Dictionary<int, int> createdItemTypeCount;

	// Token: 0x040037F7 RID: 14327
	private const float ZONE_MIGRATION_RECOVERY_DURATION = 10f;

	// Token: 0x040037F8 RID: 14328
	private readonly Dictionary<int, float> playerZoneJoinTimes = new Dictionary<int, float>(20);

	// Token: 0x040037FA RID: 14330
	private ZoneClearReason zoneClearReason;

	// Token: 0x040037FB RID: 14331
	[NonSerialized]
	public Action<GameEntity> OnEntityRemoved;

	// Token: 0x040037FC RID: 14332
	[NonSerialized]
	public Action<GameEntity> OnEntityAdded;

	// Token: 0x040037FF RID: 14335
	private int pendingTableDataSetFrame;

	// Token: 0x04003800 RID: 14336
	[DebugReadout]
	private GameEntityManager.ZoneStateData zoneStateData;

	// Token: 0x04003801 RID: 14337
	private int nextNetId = 1;

	// Token: 0x04003802 RID: 14338
	public CallLimitersList<CallLimiter, GameEntityManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GameEntityManager.RPC>();

	// Token: 0x04003803 RID: 14339
	private bool scenePlacedEntitiesRegistered;

	// Token: 0x04003804 RID: 14340
	private float scenePlacedBoundsCheckTimer;

	// Token: 0x04003805 RID: 14341
	private int _lastUpdateZoneStateAuthLogSig = int.MinValue;

	// Token: 0x04003806 RID: 14342
	private readonly List<GameEntityManager.ScenePlacedRecord> scenePlacedEntities = new List<GameEntityManager.ScenePlacedRecord>(16);

	// Token: 0x04003807 RID: 14343
	private readonly List<GameEntityId> _leavingItemScratch = new List<GameEntityId>(4);

	// Token: 0x04003808 RID: 14344
	private List<Collider> _collidersList = new List<Collider>(16);

	// Token: 0x04003809 RID: 14345
	private static List<VRRig> tempRigs = new List<VRRig>(32);

	// Token: 0x0400380A RID: 14346
	private static List<GameEntity> tempEntitiesToSerialize = new List<GameEntity>(512);

	// Token: 0x0400380B RID: 14347
	private static List<GameEntityManager.AttachmentData> tempAttachments = new List<GameEntityManager.AttachmentData>(512);

	// Token: 0x0400380C RID: 14348
	private byte[] tempSerializeGameState = new byte[15360];

	// Token: 0x0400380D RID: 14349
	[OnEnterPlay_Clear]
	private static readonly Dictionary<string, List<GameEntity>> s_scenePlacedEntities = new Dictionary<string, List<GameEntity>>();

	// Token: 0x0400380E RID: 14350
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, string> s_scenePlacedHomeScenes = new Dictionary<int, string>();

	// Token: 0x020006C0 RID: 1728
	// (Invoke) Token: 0x06002BC9 RID: 11209
	public delegate void ZoneStartEvent(GTZone zoneId);

	// Token: 0x020006C1 RID: 1729
	// (Invoke) Token: 0x06002BCD RID: 11213
	public delegate void ZoneClearEvent(GTZone zoneId);

	// Token: 0x020006C2 RID: 1730
	private enum ZoneState
	{
		// Token: 0x04003810 RID: 14352
		WaitingToEnterZone,
		// Token: 0x04003811 RID: 14353
		WaitingToRequestState,
		// Token: 0x04003812 RID: 14354
		WaitingForState,
		// Token: 0x04003813 RID: 14355
		Active
	}

	// Token: 0x020006C3 RID: 1731
	private struct ZoneStateRequest
	{
		// Token: 0x04003814 RID: 14356
		public Player player;

		// Token: 0x04003815 RID: 14357
		public GTZone zone;

		// Token: 0x04003816 RID: 14358
		public bool completed;
	}

	// Token: 0x020006C4 RID: 1732
	private class ZoneStateData
	{
		// Token: 0x04003817 RID: 14359
		public GameEntityManager.ZoneState state;

		// Token: 0x04003818 RID: 14360
		public double stateStartTime;

		// Token: 0x04003819 RID: 14361
		public List<GameEntityManager.ZoneStateRequest> zoneStateRequests;

		// Token: 0x0400381A RID: 14362
		public List<Player> zonePlayers;

		// Token: 0x0400381B RID: 14363
		[HideInInspector]
		public byte[] recievedStateBytes;

		// Token: 0x0400381C RID: 14364
		[HideInInspector]
		public int numRecievedStateBytes;
	}

	// Token: 0x020006C5 RID: 1733
	public enum RPC
	{
		// Token: 0x0400381E RID: 14366
		CreateItem,
		// Token: 0x0400381F RID: 14367
		CreateItems,
		// Token: 0x04003820 RID: 14368
		DestroyItem,
		// Token: 0x04003821 RID: 14369
		ApplyState,
		// Token: 0x04003822 RID: 14370
		GrabEntity,
		// Token: 0x04003823 RID: 14371
		ThrowEntity,
		// Token: 0x04003824 RID: 14372
		SendTableData,
		// Token: 0x04003825 RID: 14373
		HitEntity,
		// Token: 0x04003826 RID: 14374
		PlayerLeftZone
	}

	// Token: 0x020006C6 RID: 1734
	private struct ScenePlacedRecord
	{
		// Token: 0x04003827 RID: 14375
		public GameEntity entity;

		// Token: 0x04003828 RID: 14376
		public Vector3 position;

		// Token: 0x04003829 RID: 14377
		public Quaternion rotation;

		// Token: 0x0400382A RID: 14378
		public float uniformScale;
	}

	// Token: 0x020006C7 RID: 1735
	private struct AttachmentData
	{
		// Token: 0x0400382B RID: 14379
		public int entityNetId;

		// Token: 0x0400382C RID: 14380
		public int attachToEntityNetId;

		// Token: 0x0400382D RID: 14381
		public Vector3 localPosition;

		// Token: 0x0400382E RID: 14382
		public Quaternion localRotation;
	}
}
