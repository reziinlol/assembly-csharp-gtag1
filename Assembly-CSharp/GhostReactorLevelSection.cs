using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006FF RID: 1791
public class GhostReactorLevelSection : MonoBehaviour
{
	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x06002D0F RID: 11535 RVA: 0x000F526F File Offset: 0x000F346F
	public Transform Anchor
	{
		get
		{
			return this.anchorTransform;
		}
	}

	// Token: 0x17000459 RID: 1113
	// (get) Token: 0x06002D10 RID: 11536 RVA: 0x000F5277 File Offset: 0x000F3477
	public List<Transform> Anchors
	{
		get
		{
			return this.anchors;
		}
	}

	// Token: 0x1700045A RID: 1114
	// (get) Token: 0x06002D11 RID: 11537 RVA: 0x000F527F File Offset: 0x000F347F
	public GhostReactorLevelSection.SectionType Type
	{
		get
		{
			return this.sectionType;
		}
	}

	// Token: 0x1700045B RID: 1115
	// (get) Token: 0x06002D12 RID: 11538 RVA: 0x000F5287 File Offset: 0x000F3487
	public BoxCollider BoundingCollider
	{
		get
		{
			return this.boundingCollider;
		}
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x000F5290 File Offset: 0x000F3490
	private void Awake()
	{
		this.spawnPointGroupLookup = new GhostReactorLevelSection.SpawnPointGroup[11];
		for (int i = 0; i < this.spawnPointGroups.Count; i++)
		{
			this.spawnPointGroups[i].SpawnPointIndexes = new List<int>();
			int type = (int)this.spawnPointGroups[i].type;
			if (type < this.spawnPointGroupLookup.Length)
			{
				this.spawnPointGroupLookup[type] = this.spawnPointGroups[i];
			}
		}
		this.hazardousMaterials = new List<GRHazardousMaterial>(32);
		base.GetComponentsInChildren<GRHazardousMaterial>(this.hazardousMaterials);
		for (int j = 0; j < this.patrolPaths.Count; j++)
		{
			if (this.patrolPaths[j] == null)
			{
				Debug.LogErrorFormat("Why does {0} have a null patrol path at index {1}", new object[]
				{
					base.gameObject.name,
					j
				});
			}
			else
			{
				this.patrolPaths[j].index = j;
			}
		}
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int k = 0; k < this.prePlacedGameEntities.Count; k++)
		{
			this.prePlacedGameEntities[k].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(false, this.renderers);
		for (int l = this.renderers.Count - 1; l >= 0; l--)
		{
			if (this.renderers[l] == null || !this.renderers[l].enabled)
			{
				this.renderers.RemoveAt(l);
			}
		}
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[]
			{
				base.gameObject.name
			});
		}
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x000F5470 File Offset: 0x000F3670
	public static void RandomizeIndices(List<int> list, int count, ref SRand randomGenerator)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x000F54A0 File Offset: 0x000F36A0
	public void InitLevelSection(int sectionIndex, GhostReactor reactor)
	{
		this.index = sectionIndex;
		for (int i = 0; i < this.hazardousMaterials.Count; i++)
		{
			this.hazardousMaterials[i].Init(reactor);
		}
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x000F54DC File Offset: 0x000F36DC
	public void SpawnSectionEntities(ref SRand randomGenerator, GameEntityManager gameEntityManager, GhostReactor reactor, List<GhostReactorSpawnConfig> spawnConfigs, float respawnCount)
	{
		if (spawnConfigs == null)
		{
			spawnConfigs = this.spawnConfigs;
		}
		if (spawnConfigs != null && spawnConfigs.Count > 0)
		{
			GhostReactorSpawnConfig ghostReactorSpawnConfig = spawnConfigs[randomGenerator.NextInt(spawnConfigs.Count)];
			Debug.LogFormat("Spawn Ghost Reactor Level Section {0} {1}", new object[]
			{
				base.gameObject.name,
				ghostReactorSpawnConfig.name
			});
			for (int i = 0; i < this.spawnPointGroups.Count; i++)
			{
				this.spawnPointGroups[i].CurrentIndex = 0;
				this.spawnPointGroups[i].NeedsRandomization = true;
			}
			for (int j = 0; j < ghostReactorSpawnConfig.entitySpawnGroups.Count; j++)
			{
				int num = ghostReactorSpawnConfig.entitySpawnGroups[j].spawnCount;
				if (num > 0)
				{
					int spawnPointType = (int)ghostReactorSpawnConfig.entitySpawnGroups[j].spawnPointType;
					if (spawnPointType < this.spawnPointGroupLookup.Length)
					{
						GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[spawnPointType];
						if (spawnPointGroup != null)
						{
							if (spawnPointGroup.NeedsRandomization)
							{
								spawnPointGroup.NeedsRandomization = false;
								GhostReactorLevelSection.RandomizeIndices(spawnPointGroup.SpawnPointIndexes, spawnPointGroup.spawnPoints.Count, ref randomGenerator);
							}
							num = Mathf.Min(num, spawnPointGroup.spawnPoints.Count);
							for (int k = 0; k < num; k++)
							{
								int currentIndex = spawnPointGroup.CurrentIndex;
								GREntitySpawnPoint nextSpawnPoint = spawnPointGroup.GetNextSpawnPoint();
								nextSpawnPoint == null;
								GameEntity entity = ghostReactorSpawnConfig.entitySpawnGroups[j].entity;
								if (ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity != null)
								{
									ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity.TryForRandomItem(reactor, ref randomGenerator, out entity, 0);
								}
								if (!(entity == null))
								{
									int staticHash = entity.name.GetStaticHash();
									long createData = -1L;
									if (nextSpawnPoint.applyScale)
									{
										createData = BitPackUtils.PackWorldPosForNetwork(nextSpawnPoint.transform.localScale);
									}
									else if (spawnPointGroup.type == GhostReactorSpawnConfig.SpawnPointType.Enemy || spawnPointGroup.type == GhostReactorSpawnConfig.SpawnPointType.Pest || nextSpawnPoint.patrolPath != null)
									{
										int patrolIndex = 255;
										if (nextSpawnPoint.patrolPath != null)
										{
											patrolIndex = nextSpawnPoint.patrolPath.index;
										}
										int num2 = (int)respawnCount;
										if (randomGenerator.NextFloat() < respawnCount - (float)num2)
										{
											num2++;
										}
										GhostReactor.EnemyEntityCreateData enemyEntityCreateData;
										enemyEntityCreateData.respawnCount = num2;
										enemyEntityCreateData.sectionIndex = this.index;
										enemyEntityCreateData.patrolIndex = patrolIndex;
										createData = enemyEntityCreateData.Pack();
									}
									GameEntityCreateData item = new GameEntityCreateData
									{
										entityTypeId = staticHash,
										position = nextSpawnPoint.transform.position,
										rotation = nextSpawnPoint.transform.rotation,
										createData = createData,
										createdByEntityId = -1,
										slotIndex = -1
									};
									GhostReactorLevelSection.tempCreateEntitiesList.Add(item);
									if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 25)
									{
										gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
										GhostReactorLevelSection.tempCreateEntitiesList.Clear();
									}
								}
							}
						}
					}
				}
			}
			for (int l = 0; l < this.prePlacedGameEntities.Count; l++)
			{
				if (!this.prePlacedGameEntities[l].isBuiltIn)
				{
					int staticHash2 = this.prePlacedGameEntities[l].gameObject.name.GetStaticHash();
					if (!gameEntityManager.FactoryHasEntity(staticHash2))
					{
						Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1} Trying to spawn in {2}", new object[]
						{
							this.prePlacedGameEntities[l].gameObject.name,
							staticHash2,
							base.gameObject.name
						});
					}
					else
					{
						GameEntityCreateData item2 = new GameEntityCreateData
						{
							entityTypeId = staticHash2,
							position = this.prePlacedGameEntities[l].transform.position,
							rotation = this.prePlacedGameEntities[l].transform.rotation,
							createData = 0L,
							createdByEntityId = -1,
							slotIndex = -1
						};
						GhostReactorLevelSection.tempCreateEntitiesList.Add(item2);
						if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 25)
						{
							gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
							GhostReactorLevelSection.tempCreateEntitiesList.Clear();
						}
					}
				}
			}
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000F5928 File Offset: 0x000F3B28
	public void RespawnEntity(ref SRand randomGenerator, GameEntityManager gameEntityManager, int entityId, long entityCreateData, GameEntityId createdByEntityId)
	{
		if (0 > this.spawnPointGroupLookup.Length)
		{
			return;
		}
		GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[0];
		int count = spawnPointGroup.spawnPoints.Count;
		if (count > 3)
		{
			this.rotatingIndexForRespawn = (this.rotatingIndexForRespawn + randomGenerator.NextInt(1, 1 + spawnPointGroup.spawnPoints.Count / 2)) % spawnPointGroup.spawnPoints.Count;
		}
		else if (count > 1)
		{
			this.rotatingIndexForRespawn = (this.rotatingIndexForRespawn + 1) % count;
		}
		else
		{
			this.rotatingIndexForRespawn = 0;
		}
		GREntitySpawnPoint grentitySpawnPoint = spawnPointGroup.spawnPoints[this.rotatingIndexForRespawn];
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(entityCreateData);
		enemyEntityCreateData.patrolIndex = ((grentitySpawnPoint.patrolPath != null) ? grentitySpawnPoint.patrolPath.index : 255);
		long createData = enemyEntityCreateData.Pack();
		gameEntityManager.RequestCreateItem(entityId, grentitySpawnPoint.transform.position, grentitySpawnPoint.transform.rotation, createData, createdByEntityId);
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000F5A14 File Offset: 0x000F3C14
	public GRPatrolPath GetPatrolPath(int patrolPathIndex)
	{
		if (patrolPathIndex >= 0 && patrolPathIndex < this.patrolPaths.Count)
		{
			return this.patrolPaths[patrolPathIndex];
		}
		return null;
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x000F5A38 File Offset: 0x000F3C38
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000F5A84 File Offset: 0x000F3C84
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float distSq = this.GetDistSq(playerPos);
		float num = 1024f;
		float num2 = 1296f;
		if (this.hidden && distSq < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && distSq > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000F5AEC File Offset: 0x000F3CEC
	public float GetDistSq(Vector3 pos)
	{
		return (this.boundingCollider.ClosestPoint(pos) - pos).sqrMagnitude;
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x000F5B13 File Offset: 0x000F3D13
	public Transform GetAnchor(int anchorIndex)
	{
		return this.anchors[anchorIndex];
	}

	// Token: 0x040039CC RID: 14796
	private const float SHOW_DIST = 32f;

	// Token: 0x040039CD RID: 14797
	private const float HIDE_DIST = 36f;

	// Token: 0x040039CE RID: 14798
	private const int MAX_CREATE_PER_RPC = 25;

	// Token: 0x040039CF RID: 14799
	[SerializeField]
	private GhostReactorLevelSection.SectionType sectionType;

	// Token: 0x040039D0 RID: 14800
	[SerializeField]
	[Tooltip("Single Anchor Transform used for End Caps and Blockers")]
	private Transform anchorTransform;

	// Token: 0x040039D1 RID: 14801
	[SerializeField]
	[Tooltip("A List of Anchors used as in and out connections for Hubs")]
	private List<Transform> anchors = new List<Transform>();

	// Token: 0x040039D2 RID: 14802
	[SerializeField]
	private List<GhostReactorLevelSection.SpawnPointGroup> spawnPointGroups;

	// Token: 0x040039D3 RID: 14803
	[SerializeField]
	private List<GhostReactorSpawnConfig> spawnConfigs;

	// Token: 0x040039D4 RID: 14804
	[SerializeField]
	private List<GRPatrolPath> patrolPaths;

	// Token: 0x040039D5 RID: 14805
	[SerializeField]
	private BoxCollider boundingCollider;

	// Token: 0x040039D6 RID: 14806
	private List<Renderer> renderers;

	// Token: 0x040039D7 RID: 14807
	private bool hidden;

	// Token: 0x040039D8 RID: 14808
	private List<GRHazardousMaterial> hazardousMaterials;

	// Token: 0x040039D9 RID: 14809
	[HideInInspector]
	public GhostReactorLevelSectionConnector sectionConnector;

	// Token: 0x040039DA RID: 14810
	[HideInInspector]
	public int hubAnchorIndex;

	// Token: 0x040039DB RID: 14811
	private int index;

	// Token: 0x040039DC RID: 14812
	private GhostReactorLevelSection.SpawnPointGroup[] spawnPointGroupLookup;

	// Token: 0x040039DD RID: 14813
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x040039DE RID: 14814
	public static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(32);

	// Token: 0x040039DF RID: 14815
	private int rotatingIndexForRespawn;

	// Token: 0x02000700 RID: 1792
	public enum SectionType
	{
		// Token: 0x040039E1 RID: 14817
		Hub,
		// Token: 0x040039E2 RID: 14818
		EndCap,
		// Token: 0x040039E3 RID: 14819
		Blocker
	}

	// Token: 0x02000701 RID: 1793
	[Serializable]
	public class SpawnPointGroup
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06002D1F RID: 11551 RVA: 0x000F5B42 File Offset: 0x000F3D42
		// (set) Token: 0x06002D20 RID: 11552 RVA: 0x000F5B4A File Offset: 0x000F3D4A
		public bool NeedsRandomization
		{
			get
			{
				return this.needsRandomization;
			}
			set
			{
				this.needsRandomization = value;
			}
		}

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06002D21 RID: 11553 RVA: 0x000F5B53 File Offset: 0x000F3D53
		// (set) Token: 0x06002D22 RID: 11554 RVA: 0x000F5B5B File Offset: 0x000F3D5B
		public int CurrentIndex
		{
			get
			{
				return this.currentIndex;
			}
			set
			{
				this.currentIndex = value;
			}
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06002D23 RID: 11555 RVA: 0x000F5B64 File Offset: 0x000F3D64
		// (set) Token: 0x06002D24 RID: 11556 RVA: 0x000F5B6C File Offset: 0x000F3D6C
		public List<int> SpawnPointIndexes
		{
			get
			{
				return this.spawnPointIndexes;
			}
			set
			{
				this.spawnPointIndexes = value;
			}
		}

		// Token: 0x06002D25 RID: 11557 RVA: 0x000F5B75 File Offset: 0x000F3D75
		public GREntitySpawnPoint GetNextSpawnPoint()
		{
			GREntitySpawnPoint result = this.spawnPoints[this.spawnPointIndexes[this.currentIndex]];
			this.currentIndex = (this.currentIndex + 1) % this.spawnPointIndexes.Count;
			return result;
		}

		// Token: 0x040039E4 RID: 14820
		public GhostReactorSpawnConfig.SpawnPointType type;

		// Token: 0x040039E5 RID: 14821
		public List<GREntitySpawnPoint> spawnPoints;

		// Token: 0x040039E6 RID: 14822
		private List<int> spawnPointIndexes;

		// Token: 0x040039E7 RID: 14823
		private bool needsRandomization;

		// Token: 0x040039E8 RID: 14824
		private int currentIndex;
	}
}
