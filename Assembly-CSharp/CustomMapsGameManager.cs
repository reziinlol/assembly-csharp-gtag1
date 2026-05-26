using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A54 RID: 2644
public class CustomMapsGameManager : MonoBehaviour, IGameEntityZoneComponent
{
	// Token: 0x060043C0 RID: 17344 RVA: 0x0016B673 File Offset: 0x00169873
	private void Awake()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			Object.Destroy(this);
			return;
		}
		CustomMapsGameManager.instance = this;
		this.customMapsAgents = new Dictionary<int, AIAgent>(Constants.aiAgentLimit);
		CustomMapsGameManager.tempCreateEntitiesList = new List<GameEntityCreateData>(Constants.aiAgentLimit);
	}

	// Token: 0x060043C1 RID: 17345 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x060043C2 RID: 17346 RVA: 0x0016B6B0 File Offset: 0x001698B0
	public void CreatePlacedEntities(List<MapEntity> entities)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("CustomMapsManager::CreateAIAgents not the authority", null);
			return;
		}
		int gameAgentCount = this.gameAgentManager.GetGameAgentCount();
		if (gameAgentCount >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>("[CustomMapsGameManager::CreateAIAgents] Failed to create agent. Max Agent count " + string.Format("({0}) has been reached!", Constants.aiAgentLimit), null);
			return;
		}
		CustomMapsGameManager.tempCreateEntitiesList.Clear();
		int b = (Constants.aiAgentLimit - gameAgentCount < 0) ? 0 : (Constants.aiAgentLimit - gameAgentCount);
		int num = Mathf.Min(entities.Count, b);
		if (num < entities.Count)
		{
			GTDev.LogWarning<string>(string.Format("[CustomMapsGameManager::CreateAIAgents] Only creating {0} out of the ", num) + string.Format("requested {0} agents. Max Agent count ({1}) has been reached.!", entities.Count, Constants.aiAgentLimit), null);
		}
		for (int i = 0; i < num; i++)
		{
			if (entities[i].IsNull())
			{
				Debug.Log(string.Format("[CustomMapsGameManager::CreateAIAgents] Requested entity to create is null! {0}/{1}", i, entities.Count));
			}
			else
			{
				int num2 = (entities[i] is AIAgent) ? "CustomMapsAIAgent".GetStaticHash() : "CustomMapsGrabbableEntity".GetStaticHash();
				if (!this.gameEntityManager.FactoryHasEntity(num2))
				{
					Debug.LogErrorFormat("[CustomMapsManager::CreateAIAgents] Cannot Find Entity in Factory {0} {1}", new object[]
					{
						entities[i].gameObject.name,
						num2
					});
				}
				else
				{
					GameEntityCreateData item = new GameEntityCreateData
					{
						entityTypeId = num2,
						position = entities[i].transform.position,
						rotation = entities[i].transform.rotation,
						createData = entities[i].GetPackedCreateData(),
						createdByEntityId = -1,
						slotIndex = -1
					};
					CustomMapsGameManager.tempCreateEntitiesList.Add(item);
				}
			}
		}
		if (CustomMapsGameManager.tempCreateEntitiesList.Count > 0)
		{
			this.gameEntityManager.RequestCreateItems(CustomMapsGameManager.tempCreateEntitiesList);
			CustomMapsGameManager.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x060043C3 RID: 17347 RVA: 0x0016B8C3 File Offset: 0x00169AC3
	public void TEST_Spawning()
	{
		GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn starting spawn", null);
		base.StartCoroutine(this.TEST_Spawn());
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x0016B8DD File Offset: 0x00169ADD
	private IEnumerator TEST_Spawn()
	{
		while (this.spawnCount < 10)
		{
			yield return new WaitForSeconds(5f);
			GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn spawning enemy", null);
			this.TEST_index = ((this.TEST_index == 5) ? 3 : 5);
			this.SpawnEnemyFromPoint("79e43963", this.TEST_index);
			this.spawnCount++;
		}
		yield break;
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x0016B8EC File Offset: 0x00169AEC
	public GameEntityId SpawnEnemyFromPoint(string spawnPointId, int enemyTypeId)
	{
		AISpawnPoint aispawnPoint;
		if (!AISpawnManager.instance.GetSpawnPoint(spawnPointId, out aispawnPoint))
		{
			GTDev.LogError<string>("CustomMapsGameManager::SpawnEnemyFromPoint cannot find spawn point", null);
			return GameEntityId.Invalid;
		}
		return this.SpawnEnemyAtLocation(enemyTypeId, aispawnPoint.transform.position, aispawnPoint.transform.rotation);
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x0016B938 File Offset: 0x00169B38
	public GameEntityId SpawnEnemyAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameEntityManager.GetGameEntities().Count >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Max Agents ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsAIAgent".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x060043C7 RID: 17351 RVA: 0x0016B9D4 File Offset: 0x00169BD4
	public void SpawnEnemyClient(int enemyTypeId, int agentId)
	{
		if (this.gameEntityManager.IsAuthority())
		{
			return;
		}
		if (enemyTypeId == -1)
		{
			return;
		}
		AIAgent aiagent;
		if (AISpawnManager.HasInstance && AISpawnManager.instance.SpawnEnemy(enemyTypeId, out aiagent))
		{
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
			return;
		}
		MapEntity mapEntity;
		if (MapSpawnManager.instance.SpawnEntity(enemyTypeId, out mapEntity))
		{
			aiagent = (AIAgent)mapEntity;
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
			return;
		}
	}

	// Token: 0x060043C8 RID: 17352 RVA: 0x0016BA68 File Offset: 0x00169C68
	public GameEntityId SpawnGrabbableAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameEntityManager.GetGameEntities().Count >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed: Max Entities ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsGrabbableEntity".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x060043C9 RID: 17353 RVA: 0x000F9EC7 File Offset: 0x000F80C7
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		return createData;
	}

	// Token: 0x060043CA RID: 17354 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		return false;
	}

	// Token: 0x060043CB RID: 17355 RVA: 0x0016BB04 File Offset: 0x00169D04
	public bool ValidateCreateMultipleItems(int zoneId, byte[] compressedStateData, int EntityCount)
	{
		return EntityCount <= Constants.aiAgentLimit;
	}

	// Token: 0x060043CC RID: 17356 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ValidateCreateItemBatchSize(int size)
	{
		return true;
	}

	// Token: 0x060043CD RID: 17357 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ValidateCreateItem(int nedId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int createdByEntityNetId)
	{
		return true;
	}

	// Token: 0x060043CE RID: 17358 RVA: 0x0016BB11 File Offset: 0x00169D11
	private bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x060043CF RID: 17359 RVA: 0x0016BB1E File Offset: 0x00169D1E
	private bool IsDriver()
	{
		return CustomMapsTerminal.IsDriver;
	}

	// Token: 0x060043D0 RID: 17360 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnZoneCreate()
	{
	}

	// Token: 0x060043D1 RID: 17361 RVA: 0x0016BB25 File Offset: 0x00169D25
	public void OnZoneInit()
	{
		if (CustomMapsGameManager.agentsToCreateOnZoneInit.IsNullOrEmpty<MapEntity>())
		{
			return;
		}
		this.CreatePlacedEntities(CustomMapsGameManager.agentsToCreateOnZoneInit);
		CustomMapsGameManager.agentsToCreateOnZoneInit.Clear();
	}

	// Token: 0x060043D2 RID: 17362 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnZoneClear(ZoneClearReason reason)
	{
	}

	// Token: 0x060043D3 RID: 17363 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ShouldClearZone()
	{
		return true;
	}

	// Token: 0x060043D4 RID: 17364 RVA: 0x0016BB49 File Offset: 0x00169D49
	public bool IsZoneReady()
	{
		return CustomMapLoader.CanLoadEntities && NetworkSystem.Instance.InRoom;
	}

	// Token: 0x060043D5 RID: 17365 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x060043D6 RID: 17366 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void SetupCollisions(GameObject go)
	{
	}

	// Token: 0x060043D7 RID: 17367 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SerializeZoneData(BinaryWriter writer)
	{
	}

	// Token: 0x060043D8 RID: 17368 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DeserializeZoneData(BinaryReader reader)
	{
	}

	// Token: 0x060043D9 RID: 17369 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x060043DA RID: 17370 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x060043DB RID: 17371 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
	}

	// Token: 0x060043DC RID: 17372 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
	}

	// Token: 0x060043DD RID: 17373 RVA: 0x0016BB5E File Offset: 0x00169D5E
	public static GameEntityManager GetEntityManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameEntityManager;
		}
		return null;
	}

	// Token: 0x060043DE RID: 17374 RVA: 0x0016BB78 File Offset: 0x00169D78
	public static GameAgentManager GetAgentManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameAgentManager;
		}
		return null;
	}

	// Token: 0x060043DF RID: 17375 RVA: 0x0016BB94 File Offset: 0x00169D94
	public static CustomMapsAIBehaviourController GetBehaviorControllerForEntity(GameEntityId entityId)
	{
		GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
		if (entityManager.IsNull())
		{
			return null;
		}
		GameEntity gameEntity = entityManager.GetGameEntity(entityId);
		if (gameEntity.IsNull())
		{
			return null;
		}
		return gameEntity.gameObject.GetComponent<CustomMapsAIBehaviourController>();
	}

	// Token: 0x060043E0 RID: 17376 RVA: 0x0016BBCE File Offset: 0x00169DCE
	public static void AddAgentsToCreate(List<MapEntity> entitiesToCreate)
	{
		if (CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		if (entitiesToCreate.IsNullOrEmpty<MapEntity>())
		{
			return;
		}
		CustomMapsGameManager.agentsToCreateOnZoneInit.AddRange(entitiesToCreate);
	}

	// Token: 0x060043E1 RID: 17377 RVA: 0x0016BBF1 File Offset: 0x00169DF1
	public void OnPlayerHit(GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.ghostReactorManager.RequestEnemyHitPlayer(GhostReactor.EnemyType.CustomMapsEnemy, hitByEntityId, player, hitPosition);
	}

	// Token: 0x040055BC RID: 21948
	public GameEntityManager gameEntityManager;

	// Token: 0x040055BD RID: 21949
	public GameAgentManager gameAgentManager;

	// Token: 0x040055BE RID: 21950
	public GhostReactorManager ghostReactorManager;

	// Token: 0x040055BF RID: 21951
	public static CustomMapsGameManager instance;

	// Token: 0x040055C0 RID: 21952
	private const string AGENT_PREFAB_NAME = "CustomMapsAIAgent";

	// Token: 0x040055C1 RID: 21953
	private const string GRABBABLE_PREFAB_NAME = "CustomMapsGrabbableEntity";

	// Token: 0x040055C2 RID: 21954
	private Dictionary<int, AIAgent> customMapsAgents;

	// Token: 0x040055C3 RID: 21955
	private static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(128);

	// Token: 0x040055C4 RID: 21956
	private static List<MapEntity> agentsToCreateOnZoneInit = new List<MapEntity>(128);

	// Token: 0x040055C5 RID: 21957
	private int TEST_index;

	// Token: 0x040055C6 RID: 21958
	private int spawnCount;
}
