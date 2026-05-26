using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTag.Rendering;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

// Token: 0x020006EF RID: 1775
public class GhostReactor : MonoBehaviourTick, IBuildValidation
{
	// Token: 0x06002CC2 RID: 11458 RVA: 0x000F1FBC File Offset: 0x000F01BC
	public static GhostReactor Get(GameEntity gameEntity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(gameEntity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return ghostReactorManager.reactor;
	}

	// Token: 0x06002CC3 RID: 11459 RVA: 0x000F1FE4 File Offset: 0x000F01E4
	private void Awake()
	{
		GhostReactor.instance = this;
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
		this.vrRigs = new List<VRRig>();
		for (int j = 0; j < this.itemPurchaseStands.Count; j++)
		{
			if (this.itemPurchaseStands[j] == null)
			{
				Debug.LogErrorFormat("Null Item Purchase Stand {0}", new object[]
				{
					j
				});
			}
			else
			{
				this.itemPurchaseStands[j].Setup(j);
			}
		}
		for (int k = 0; k < this.toolPurchasingStations.Count; k++)
		{
			if (this.toolPurchasingStations[k] == null)
			{
				Debug.LogErrorFormat("Null Tool Purchasing Station {0}", new object[]
				{
					k
				});
			}
			else
			{
				this.toolPurchasingStations[k].PurchaseStationId = k;
			}
		}
		if (this.promotionBot != null)
		{
			this.promotionBot.Init(this);
		}
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
		this.handPrintMPB = new MaterialPropertyBlock();
		this.handPrintMPB.SetFloatArray("_HandPrintData", new float[1024]);
		this.bays = new List<GRBay>(32);
		base.GetComponentsInChildren<GRBay>(false, this.bays);
		this.storeDisplays = new List<GRUIStoreDisplay>();
		base.GetComponentsInChildren<GRUIStoreDisplay>(false, this.storeDisplays);
	}

	// Token: 0x06002CC4 RID: 11460 RVA: 0x000F2178 File Offset: 0x000F0378
	private new void OnEnable()
	{
		base.OnEnable();
		if (this.zone == GTZone.customMaps)
		{
			return;
		}
		GTDev.Log<string>(string.Format("GhostReactor::OnEnable getting manager for zone {0}", this.zone), null);
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (managerForZone == null)
		{
			Debug.LogErrorFormat("No GameEntityManager found for zone {0}", new object[]
			{
				this.zone
			});
			return;
		}
		this.grManager = managerForZone.ghostReactorManager;
		if (this.grManager == null)
		{
			Debug.LogErrorFormat("No GhostReactorManager found for zone {0}", new object[]
			{
				this.zone
			});
			return;
		}
		this.grManager.reactor = this;
		this.grManager.gameEntityManager.boundsBoxCollider = this.boundsBoxCollider;
		if (GameLightingManager.instance != null && this.zone != GTZone.customMaps)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(true);
		}
		VRRigCache.OnRigActivated += this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated += this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged += this.OnVRRigsChanged;
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnLocalPlayerConnectedToRoom;
		}
		for (int i = 0; i < this.toolPurchasingStations.Count; i++)
		{
			this.toolPurchasingStations[i].Init(this.grManager, this);
		}
		if (this.debugUpgradeKiosk != null)
		{
			this.debugUpgradeKiosk.Init(this.grManager, this);
		}
		if (this.currencyDepositor != null)
		{
			this.currencyDepositor.Init(this);
		}
		if (this.distillery != null)
		{
			this.distillery.Init(this);
		}
		if (this.seedExtractor != null)
		{
			this.seedExtractor.Init(this.toolProgression, this);
		}
		if (this.levelGenerator != null)
		{
			this.levelGenerator.Init(this);
		}
		if (this.employeeBadges != null)
		{
			this.employeeBadges.Init(this);
		}
		if (this.toolProgression != null)
		{
			this.toolProgression.Init(this);
			this.toolProgression.OnProgressionUpdated += this.OnProgressionUpdated;
		}
		if (this.shiftManager != null)
		{
			this.shiftManager.Init(this.grManager);
		}
		for (int j = 0; j < this.toolUpgradePurchaseStationsFull.Count; j++)
		{
			this.toolUpgradePurchaseStationsFull[j].Init(this.toolProgression, this);
		}
		GRElevatorManager._instance.InitShuttles(this);
		if (this.recycler != null)
		{
			this.recycler.Init(this);
		}
		if (this.zoneShaderSettings != null)
		{
			this.zoneShaderSettings.BecomeActiveInstance(true);
		}
		for (int k = 0; k < this.bays.Count; k++)
		{
			this.bays[k].Setup(this);
		}
		for (int l = 0; l < this.storeDisplays.Count; l++)
		{
			this.storeDisplays[l].Setup(-1, this);
		}
		this.RefreshDepth();
	}

	// Token: 0x06002CC5 RID: 11461 RVA: 0x000F24B6 File Offset: 0x000F06B6
	public void EnableGhostReactorForVirtualStump()
	{
		GhostReactor.instance = this;
		this.RefreshReviveStations(false);
		this.OnEnable();
	}

	// Token: 0x06002CC6 RID: 11462 RVA: 0x000F24CC File Offset: 0x000F06CC
	public void RefreshReviveStations(bool searchScene = false)
	{
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		if (searchScene)
		{
			this.reviveStations.AddRange(Object.FindObjectsByType<GRReviveStation>(FindObjectsInactive.Include, FindObjectsSortMode.None));
		}
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
	}

	// Token: 0x06002CC7 RID: 11463 RVA: 0x000F2530 File Offset: 0x000F0730
	private new void OnDisable()
	{
		base.OnDisable();
		if (this.zone == GTZone.customMaps)
		{
			return;
		}
		GameLightingManager.instance.ZoneEnableCustomDynamicLighting(false);
		VRRigCache.OnRigActivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigDeactivated -= this.OnVRRigsChanged;
		VRRigCache.OnRigNameChanged -= this.OnVRRigsChanged;
		if (this.toolProgression != null)
		{
			this.toolProgression.OnProgressionUpdated -= this.OnProgressionUpdated;
		}
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnLocalPlayerConnectedToRoom;
		}
	}

	// Token: 0x06002CC8 RID: 11464 RVA: 0x000F25E1 File Offset: 0x000F07E1
	private void OnProgressionUpdated()
	{
		if (this.toolProgression != null)
		{
			this.UpdateLocalPlayerFromProgression();
		}
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x000F25F8 File Offset: 0x000F07F8
	public void UpdateLocalPlayerFromProgression()
	{
		GRPlayer local = GRPlayer.GetLocal();
		if (local != null)
		{
			int dropPodLevel = this.toolProgression.GetDropPodLevel();
			if (local.dropPodLevel != dropPodLevel)
			{
				local.dropPodLevel = dropPodLevel;
				Debug.LogFormat("Drop Pod UpdateLocalPlayerFromProgression Level {0} {1} {2}", new object[]
				{
					this.grManager.IsZoneActive(),
					local.dropPodLevel,
					local.dropPodChasisLevel
				});
				if (this.grManager.IsZoneActive())
				{
					this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodLevel, dropPodLevel);
				}
			}
			int dropPodChasisLevel = this.toolProgression.GetDropPodChasisLevel();
			if (local.dropPodChasisLevel != dropPodChasisLevel)
			{
				local.dropPodChasisLevel = dropPodChasisLevel;
				Debug.LogFormat("Drop Pod UpdateLocalPlayerFromProgression Level {0} {1} {2}", new object[]
				{
					this.grManager.IsZoneActive(),
					local.dropPodLevel,
					local.dropPodChasisLevel
				});
				if (this.grManager.IsZoneActive())
				{
					this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodChassisLevel, dropPodChasisLevel);
				}
			}
			if (local.badge)
			{
				local.badge.RefreshText(PhotonNetwork.LocalPlayer);
			}
			this.RefreshStore();
		}
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x000F272B File Offset: 0x000F092B
	public GRPatrolPath GetPatrolPath(long createData)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		return this.levelGenerator.GetPatrolPath(createData);
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x000F274C File Offset: 0x000F094C
	public override void Tick()
	{
		if (this.grManager == null)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.grManager.gameEntityManager.IsAuthority())
		{
			if (Time.timeAsDouble - this.lastCollectibleDispenserUpdateTime > (double)this.collectibleDispenserUpdateFrequency)
			{
				this.lastCollectibleDispenserUpdateTime = Time.timeAsDouble;
				for (int i = 0; i < this.collectibleDispensers.Count; i++)
				{
					if (this.collectibleDispensers[i] != null && this.collectibleDispensers[i].ReadyToDispenseNewCollectible)
					{
						this.collectibleDispensers[i].RequestDispenseCollectible();
					}
				}
			}
			if (this.sleepableEntities.Count > 0)
			{
				this.sentientCoreUpdateIndex = Mathf.Max(0, this.sentientCoreUpdateIndex % this.sleepableEntities.Count);
				if (this.sentientCoreUpdateIndex < this.sleepableEntities.Count)
				{
					IGRSleepableEntity igrsleepableEntity = this.sleepableEntities[this.sentientCoreUpdateIndex];
					float num = igrsleepableEntity.WakeUpRadius * igrsleepableEntity.WakeUpRadius;
					float num2 = (igrsleepableEntity.WakeUpRadius + 0.5f) * (igrsleepableEntity.WakeUpRadius + 0.5f);
					bool flag = false;
					bool flag2 = false;
					for (int j = 0; j < this.vrRigs.Count; j++)
					{
						GRPlayer component = this.vrRigs[j].GetComponent<GRPlayer>();
						if (!(component == null) && component.State != GRPlayer.GRPlayerState.Ghost)
						{
							float sqrMagnitude = (igrsleepableEntity.Position - this.vrRigs[j].bodyTransform.position).sqrMagnitude;
							if (sqrMagnitude < num2)
							{
								flag = true;
							}
							if (sqrMagnitude < num)
							{
								flag2 = true;
								break;
							}
						}
					}
					bool flag3 = igrsleepableEntity.IsSleeping();
					if (flag3 && flag2)
					{
						igrsleepableEntity.WakeUp();
					}
					else if (!flag3 && !flag)
					{
						igrsleepableEntity.Sleep();
					}
					this.sentientCoreUpdateIndex++;
				}
			}
		}
		bool flag4 = false;
		foreach (GhostReactor.EntityTypeRespawnTracker entityTypeRespawnTracker in this.respawnQueue)
		{
			entityTypeRespawnTracker.entityNextRespawnTime -= Time.deltaTime;
			if (entityTypeRespawnTracker.entityNextRespawnTime < 0f)
			{
				entityTypeRespawnTracker.entityNextRespawnTime = 0f;
				flag4 = true;
				if (this.grManager.gameEntityManager.IsAuthority())
				{
					this.levelGenerator.RespawnEntity(entityTypeRespawnTracker.entityTypeID, entityTypeRespawnTracker.entityCreateData, GameEntityId.Invalid);
				}
			}
		}
		if (flag4)
		{
			this.respawnQueue.RemoveAll((GhostReactor.EntityTypeRespawnTracker e) => e.entityNextRespawnTime <= 0f);
		}
		this.UpdateHandprints(Time.deltaTime);
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x000F2A04 File Offset: 0x000F0C04
	private void OnLocalPlayerConnectedToRoom()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			grplayer.Reset();
		}
		if (this.shiftManager != null)
		{
			this.shiftManager.shiftStats.ResetShiftStats();
			this.shiftManager.RefreshShiftStatsDisplay();
		}
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x000F2A54 File Offset: 0x000F0C54
	private void OnVRRigsChanged(RigContainer container)
	{
		this.VRRigRefresh();
	}

	// Token: 0x06002CCE RID: 11470 RVA: 0x000F2A5C File Offset: 0x000F0C5C
	public void VRRigRefresh()
	{
		if (this.isRefreshing)
		{
			return;
		}
		this.isRefreshing = true;
		this.vrRigs.Clear();
		this.vrRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.vrRigs.Sort(delegate(VRRig a, VRRig b)
		{
			if (a == null || a.OwningNetPlayer == null)
			{
				return 1;
			}
			if (b == null || b.OwningNetPlayer == null)
			{
				return -1;
			}
			return a.OwningNetPlayer.ActorNumber.CompareTo(b.OwningNetPlayer.ActorNumber);
		});
		if (this.promotionBot != null)
		{
			this.promotionBot.Refresh();
		}
		this.RefreshScoreboards();
		this.RefreshDepth();
		this.RefreshStore();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null && this.vrRigs.Count > grplayer.maxNumberOfPlayersInShift)
		{
			grplayer.maxNumberOfPlayersInShift = this.vrRigs.Count;
		}
		this.isRefreshing = false;
	}

	// Token: 0x06002CCF RID: 11471 RVA: 0x000F2B38 File Offset: 0x000F0D38
	public void UpdateScoreboardScreen(GRUIScoreboard.ScoreboardScreen newScreen)
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].SwitchToScreen(newScreen);
		}
		this.RefreshScoreboards();
	}

	// Token: 0x06002CD0 RID: 11472 RVA: 0x000F2B74 File Offset: 0x000F0D74
	public void RefreshScoreboards()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			if (!(this.scoreboards[i] == null))
			{
				this.scoreboards[i].Refresh(this.vrRigs);
				if (this.shiftManager != null)
				{
					if (this.shiftManager.ShiftActive)
					{
						this.scoreboards[i].total.text = "-AWAITING SHIFT END-";
					}
					else if (this.shiftManager.ShiftTotalEarned < 0)
					{
						this.scoreboards[i].total.text = "-SHIFT NOT ACTIVE-";
					}
					else
					{
						this.scoreboards[i].total.text = this.shiftManager.ShiftTotalEarned.ToString();
					}
				}
			}
		}
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x000F2C58 File Offset: 0x000F0E58
	public int GetItemCost(int entityTypeId)
	{
		int result;
		if (!this.grManager.gameEntityManager.PriceLookup(entityTypeId, out result))
		{
			return 100;
		}
		return result;
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x000F2C80 File Offset: 0x000F0E80
	public void UpdateRemoteScoreboardScreen(GRUIScoreboard.ScoreboardScreen scoreboardPage)
	{
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (managerForZone != null && managerForZone.ghostReactorManager != null)
		{
			managerForZone.ghostReactorManager.photonView.RPC("BroadcastScoreboardPage", RpcTarget.Others, new object[]
			{
				scoreboardPage
			});
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x000F2CD8 File Offset: 0x000F0ED8
	public void SetNextDelveDepth(int newLevel, int newDepthConfigIndex)
	{
		this.depthLevel = newLevel;
		this.depthLevel = Mathf.Clamp(this.depthLevel, 0, this.levelGenerator.depthConfigs.Count);
		if (this.depthLevel >= 0 && this.zone == GTZone.ghostReactorDrill && PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			int joinDepthSectionFromLevel = GhostReactor.GetJoinDepthSectionFromLevel(this.depthLevel);
			Hashtable hashtable = new Hashtable
			{
				{
					"ghostReactorDepth",
					joinDepthSectionFromLevel.ToString()
				}
			};
			Debug.LogFormat("GR Room Param Set {0} {1}", new object[]
			{
				"ghostReactorDepth",
				hashtable["ghostReactorDepth"]
			});
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable, null, null);
		}
		this.depthConfigIndex = newDepthConfigIndex;
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000F2DA3 File Offset: 0x000F0FA3
	public static int GetJoinDepthSectionFromLevel(int depthLevel)
	{
		if (depthLevel < 4)
		{
			return 0;
		}
		if (depthLevel < 10)
		{
			return 1;
		}
		if (depthLevel < 15)
		{
			return 2;
		}
		if (depthLevel < 20)
		{
			return 3;
		}
		if (depthLevel < 25)
		{
			return 5;
		}
		return 6;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x000F2DC8 File Offset: 0x000F0FC8
	public void DelveToNextDepth()
	{
		if (this.shiftManager != null)
		{
			this.shiftManager.authorizedToDelveDeeper = false;
		}
		this.RefreshDepth();
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x000F2DEC File Offset: 0x000F0FEC
	public int PickLevelConfigForDepth(int depthLevel)
	{
		if (this.zone == GTZone.customMaps)
		{
			return 0;
		}
		GhostReactorLevelDepthConfig depthLevelConfig = this.GetDepthLevelConfig(depthLevel);
		int num = 0;
		for (int i = 0; i < depthLevelConfig.options.Count; i++)
		{
			num += depthLevelConfig.options[i].weight;
		}
		int num2 = Random.Range(0, num + 1);
		for (int j = 0; j < depthLevelConfig.options.Count; j++)
		{
			if (depthLevelConfig.options[j].weight >= num2)
			{
				return j;
			}
			num2 -= depthLevelConfig.options[j].weight;
		}
		return 0;
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x000F2E8B File Offset: 0x000F108B
	public void RefreshDepth()
	{
		if (this.shiftManager != null)
		{
			this.shiftManager.RefreshDepthDisplay();
		}
		this.RefreshBays();
	}

	// Token: 0x06002CD8 RID: 11480 RVA: 0x000F2EAC File Offset: 0x000F10AC
	public int GetDepthLevel()
	{
		return this.depthLevel;
	}

	// Token: 0x06002CD9 RID: 11481 RVA: 0x000F2EB4 File Offset: 0x000F10B4
	public int GetDepthConfigIndex()
	{
		return this.depthConfigIndex;
	}

	// Token: 0x06002CDA RID: 11482 RVA: 0x000F2EBC File Offset: 0x000F10BC
	public GhostReactorLevelDepthConfig GetDepthLevelConfig(int level)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		level = Mathf.Clamp(level, 0, this.levelGenerator.depthConfigs.Count - 1);
		return this.levelGenerator.depthConfigs[level];
	}

	// Token: 0x06002CDB RID: 11483 RVA: 0x000F2EFC File Offset: 0x000F10FC
	public GhostReactorLevelGenConfig GetCurrLevelGenConfig()
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		int num = this.GetDepthLevel();
		num = Mathf.Clamp(num, 0, this.levelGenerator.depthConfigs.Count - 1);
		this.depthConfigIndex = Mathf.Clamp(this.depthConfigIndex, 0, this.levelGenerator.depthConfigs[num].options.Count - 1);
		return this.levelGenerator.depthConfigs[num].options[this.depthConfigIndex].levelConfig;
	}

	// Token: 0x06002CDC RID: 11484 RVA: 0x000F2F90 File Offset: 0x000F1190
	public void RefreshStore()
	{
		for (int i = 0; i < this.storeDisplays.Count; i++)
		{
			this.storeDisplays[i].Setup(PhotonNetwork.LocalPlayer.ActorNumber, this);
		}
	}

	// Token: 0x06002CDD RID: 11485 RVA: 0x000F2FD0 File Offset: 0x000F11D0
	public void RefreshBays()
	{
		for (int i = 0; i < this.bays.Count; i++)
		{
			this.bays[i].Refresh();
		}
	}

	// Token: 0x06002CDE RID: 11486 RVA: 0x000F3004 File Offset: 0x000F1204
	public void UpdateHandprints(float deltaTime)
	{
		int num = this.handPrintData.Count - 1000;
		if (num > 0)
		{
			this.handPrintData.RemoveRange(0, num);
			this.handPrintLocations.RemoveRange(0, num);
		}
		float time = Time.time;
		int i = this.handPrintData.Count - 1;
		while (i >= 0)
		{
			this.handPrintData[i] = this.handPrintData[i] - deltaTime;
			if (i + this.handPrintCombineTestDelta >= this.handPrintData.Count)
			{
				goto IL_13E;
			}
			if (this.handPrintData[i + this.handPrintCombineTestDelta] <= this.handPrintFadeTime - 3f)
			{
				Matrix4x4 matrix4x = this.handPrintLocations[i];
				Matrix4x4 matrix4x2 = this.handPrintLocations[i + this.handPrintCombineTestDelta];
				Vector3 vector = new Vector3(matrix4x.m03 - matrix4x2.m03, matrix4x.m13 - matrix4x2.m13, matrix4x.m23 - matrix4x2.m23);
				if (vector.sqrMagnitude < this.handPrintScale * this.handPrintScale)
				{
					List<float> list = this.handPrintData;
					int index = i;
					list[index] -= deltaTime * (float)this.handPrintData.Count * 50f;
					goto IL_13E;
				}
				goto IL_13E;
			}
			IL_169:
			i--;
			continue;
			IL_13E:
			if (this.handPrintData[i] < 0f)
			{
				this.handPrintData.RemoveAt(i);
				this.handPrintLocations.RemoveAt(i);
				goto IL_169;
			}
			goto IL_169;
		}
		if (this.handPrintData.Count > 0)
		{
			this.handPrintCombineTestDelta = (this.handPrintCombineTestDelta + 1) % this.handPrintData.Count;
			if (this.handPrintCombineTestDelta == 0)
			{
				this.handPrintCombineTestDelta = 1;
			}
		}
		else
		{
			this.handPrintCombineTestDelta = 1;
		}
		if (this.handPrintMaterial != null)
		{
			this.handPrintMaterial.SetFloat("_FadeDuration", this.handPrintFadeTime);
			this.handPrintMaterial.enableInstancing = true;
		}
		int num2 = Mathf.Min(Math.Min(1000, 1023), this.handPrintLocations.Count);
		if (num2 > 0)
		{
			this.handPrintMPB.Clear();
			this.handPrintMPB.SetFloatArray("_HandPrintData", this.handPrintData.GetRange(0, num2));
			this.handPrintMPB.SetFloat("_FadeDuration", this.handPrintFadeTime);
			RenderParams renderParams = new RenderParams(this.handPrintMaterial)
			{
				shadowCastingMode = ShadowCastingMode.Off,
				receiveShadows = false,
				layer = base.gameObject.layer,
				matProps = this.handPrintMPB,
				worldBounds = new Bounds(Vector3.zero, Vector3.one * 2000f)
			};
			Graphics.RenderMeshInstanced<Matrix4x4>(renderParams, this.handPrintMesh, 0, this.handPrintLocations.GetRange(0, num2), -1, 0);
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			if (Time.time - this.handPrintTimeLeft >= this.handPrintInkTime)
			{
				grplayer.SetGooParticleSystemEnabled(true, false);
			}
			if (Time.time - this.handPrintTimeRight >= this.handPrintInkTime)
			{
				grplayer.SetGooParticleSystemEnabled(false, false);
			}
		}
	}

	// Token: 0x06002CDF RID: 11487 RVA: 0x000F3324 File Offset: 0x000F1524
	public void OnTapLocal(bool isLeftHand, Vector3 pos, Quaternion orient, GorillaSurfaceOverride surfaceOverride)
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer == null)
		{
			return;
		}
		if (!(surfaceOverride != null) || surfaceOverride.overrideIndex != 79)
		{
			float num = isLeftHand ? this.handPrintTimeLeft : this.handPrintTimeRight;
			if (Time.time - num < this.handPrintInkTime && (Time.time < this.lastBroadcastHandTapTime || Time.time > this.lastBroadcastHandTapTime + this.broadcastHandTapDelay))
			{
				GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
				if (managerForZone != null && managerForZone.ghostReactorManager != null)
				{
					managerForZone.ghostReactorManager.photonView.RPC("BroadcastHandprint", RpcTarget.All, new object[]
					{
						pos,
						orient
					});
				}
				this.lastBroadcastHandTapTime = Time.time;
			}
			return;
		}
		grplayer.SetGooParticleSystemEnabled(isLeftHand, true);
		if (isLeftHand)
		{
			this.handPrintTimeLeft = Time.time;
			return;
		}
		this.handPrintTimeRight = Time.time;
	}

	// Token: 0x06002CE0 RID: 11488 RVA: 0x000F341C File Offset: 0x000F161C
	public void AddHandprint(Vector3 pos, Quaternion orient)
	{
		Matrix4x4 item = default(Matrix4x4);
		item.SetTRS(pos, orient * Quaternion.Euler(90f, 0f, 180f), Vector3.one * this.handPrintScale);
		this.handPrintLocations.Add(item);
		this.handPrintData.Add(this.handPrintFadeTime);
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x000F3480 File Offset: 0x000F1680
	public void ClearAllHandprints()
	{
		this.handPrintData.Clear();
		this.handPrintLocations.Clear();
	}

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x06002CE2 RID: 11490 RVA: 0x000F3498 File Offset: 0x000F1698
	public int NumActivePlayers
	{
		get
		{
			return this.vrRigs.Count;
		}
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000F34A8 File Offset: 0x000F16A8
	public void OnAbilityDie(GameEntity entity, float forcedRespawn = -1f)
	{
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(entity.createData);
		if (enemyEntityCreateData.respawnCount == 0)
		{
			return;
		}
		if (this.grManager.GetBossEntity() != null)
		{
			GREnemyBossMoon component = this.grManager.GetBossEntity().GetComponent<GREnemyBossMoon>();
			if (component != null && component.BossHasRevealed)
			{
				return;
			}
		}
		GhostReactor.EntityTypeRespawnTracker entityTypeRespawnTracker = new GhostReactor.EntityTypeRespawnTracker();
		entityTypeRespawnTracker.entityTypeID = entity.typeId;
		entityTypeRespawnTracker.entityCreateData = enemyEntityCreateData.Pack();
		entityTypeRespawnTracker.entityNextRespawnTime = ((forcedRespawn < 0f) ? this.respawnTime : forcedRespawn);
		this.respawnQueue.Add(entityTypeRespawnTracker);
	}

	// Token: 0x06002CE4 RID: 11492 RVA: 0x000F3542 File Offset: 0x000F1742
	public void ClearAllRespawns()
	{
		this.respawnQueue.Clear();
	}

	// Token: 0x06002CE5 RID: 11493 RVA: 0x00023994 File Offset: 0x00021B94
	bool IBuildValidation.BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x0400392D RID: 14637
	public static GhostReactor instance;

	// Token: 0x0400392E RID: 14638
	public GTZone zone;

	// Token: 0x0400392F RID: 14639
	public Transform restartMarker;

	// Token: 0x04003930 RID: 14640
	public PhotonView photonView;

	// Token: 0x04003931 RID: 14641
	public AudioSource entryRoomAudio;

	// Token: 0x04003932 RID: 14642
	public AudioClip entryRoomDeathSound;

	// Token: 0x04003933 RID: 14643
	[FormerlySerializedAs("zoneLimit")]
	public BoxCollider boundsBoxCollider;

	// Token: 0x04003934 RID: 14644
	public BoxCollider safeZoneLimit;

	// Token: 0x04003935 RID: 14645
	public List<GhostReactor.TempEnemySpawnInfo> tempSpawnEnemies;

	// Token: 0x04003936 RID: 14646
	public GameEntity overrideEnemySpawn;

	// Token: 0x04003937 RID: 14647
	public List<GameEntity> tempSpawnItems;

	// Token: 0x04003938 RID: 14648
	public Transform tempSpawnItemsMarker;

	// Token: 0x04003939 RID: 14649
	public List<GRUIBuyItem> itemPurchaseStands;

	// Token: 0x0400393A RID: 14650
	public List<GRToolPurchaseStation> toolPurchasingStations;

	// Token: 0x0400393B RID: 14651
	public GRDebugUpgradeKiosk debugUpgradeKiosk;

	// Token: 0x0400393C RID: 14652
	public List<GRUIScoreboard> scoreboards;

	// Token: 0x0400393D RID: 14653
	public List<GRCollectibleDispenser> collectibleDispensers = new List<GRCollectibleDispenser>();

	// Token: 0x0400393E RID: 14654
	public List<IGRSleepableEntity> sleepableEntities = new List<IGRSleepableEntity>();

	// Token: 0x0400393F RID: 14655
	private List<GRBay> bays;

	// Token: 0x04003940 RID: 14656
	private List<GRUIStoreDisplay> storeDisplays;

	// Token: 0x04003941 RID: 14657
	public GRUIStationEmployeeBadges employeeBadges;

	// Token: 0x04003942 RID: 14658
	public GRUIEmployeeTerminal employeeTerminal;

	// Token: 0x04003943 RID: 14659
	public GhostReactorShiftManager shiftManager;

	// Token: 0x04003944 RID: 14660
	public GhostReactorLevelGenerator levelGenerator;

	// Token: 0x04003945 RID: 14661
	public GRCurrencyDepositor currencyDepositor;

	// Token: 0x04003946 RID: 14662
	public GRSeedExtractor seedExtractor;

	// Token: 0x04003947 RID: 14663
	public GRDistillery distillery;

	// Token: 0x04003948 RID: 14664
	public GRToolProgressionManager toolProgression;

	// Token: 0x04003949 RID: 14665
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x0400394A RID: 14666
	public List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull;

	// Token: 0x0400394B RID: 14667
	public GRRecycler recycler;

	// Token: 0x0400394C RID: 14668
	public List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = new List<GhostReactor.EntityTypeRespawnTracker>();

	// Token: 0x0400394D RID: 14669
	public List<float> difficultyScalingPerPlayer = new List<float>(10);

	// Token: 0x0400394E RID: 14670
	public float respawnTime = 10f;

	// Token: 0x0400394F RID: 14671
	public float respawnMinDistToPlayer = 8f;

	// Token: 0x04003950 RID: 14672
	public float difficultyScalingForCurrentFloor = 1f;

	// Token: 0x04003951 RID: 14673
	public LayerMask envLayerMask;

	// Token: 0x04003952 RID: 14674
	public Material handPrintMaterial;

	// Token: 0x04003953 RID: 14675
	public Mesh handPrintMesh;

	// Token: 0x04003954 RID: 14676
	public float handPrintScale;

	// Token: 0x04003955 RID: 14677
	public float handPrintInkTime = 30f;

	// Token: 0x04003956 RID: 14678
	public float handPrintFadeTime = 600f;

	// Token: 0x04003957 RID: 14679
	private const int handPrintMaxCount = 1000;

	// Token: 0x04003958 RID: 14680
	private List<Matrix4x4> handPrintLocations = new List<Matrix4x4>(1000);

	// Token: 0x04003959 RID: 14681
	private List<float> handPrintData = new List<float>(1000);

	// Token: 0x0400395A RID: 14682
	private MaterialPropertyBlock handPrintMPB;

	// Token: 0x0400395B RID: 14683
	[ReadOnly]
	public List<GRReviveStation> reviveStations;

	// Token: 0x0400395C RID: 14684
	public List<GRVendingMachine> vendingMachines;

	// Token: 0x0400395D RID: 14685
	public List<VRRig> vrRigs;

	// Token: 0x0400395E RID: 14686
	private float collectibleDispenserUpdateFrequency = 3f;

	// Token: 0x0400395F RID: 14687
	private double lastCollectibleDispenserUpdateTime = -10.0;

	// Token: 0x04003960 RID: 14688
	private int sentientCoreUpdateIndex;

	// Token: 0x04003961 RID: 14689
	private SRand randomGenerator;

	// Token: 0x04003962 RID: 14690
	[ReadOnly]
	public int depthLevel;

	// Token: 0x04003963 RID: 14691
	[ReadOnly]
	public int depthConfigIndex;

	// Token: 0x04003964 RID: 14692
	public Dictionary<int, double> playerProgressionData;

	// Token: 0x04003965 RID: 14693
	public GRDropZone dropZone;

	// Token: 0x04003966 RID: 14694
	public static float DROP_ZONE_REPEL = 2.25f;

	// Token: 0x04003967 RID: 14695
	public ZoneShaderSettings zoneShaderSettings;

	// Token: 0x04003968 RID: 14696
	public GRUIPromotionBot promotionBot;

	// Token: 0x04003969 RID: 14697
	private bool isRefreshing;

	// Token: 0x0400396A RID: 14698
	public GhostReactorManager grManager;

	// Token: 0x0400396B RID: 14699
	private float handPrintTimeLeft = -1000f;

	// Token: 0x0400396C RID: 14700
	private float handPrintTimeRight = -1000f;

	// Token: 0x0400396D RID: 14701
	private int handPrintCombineTestDelta = 1;

	// Token: 0x0400396E RID: 14702
	private float lastBroadcastHandTapTime;

	// Token: 0x0400396F RID: 14703
	private float broadcastHandTapDelay = 0.3f;

	// Token: 0x020006F0 RID: 1776
	[Serializable]
	public class TempEnemySpawnInfo
	{
		// Token: 0x04003970 RID: 14704
		public GameEntity prefab;

		// Token: 0x04003971 RID: 14705
		public Transform spawnMarker;

		// Token: 0x04003972 RID: 14706
		public int patrolPath;
	}

	// Token: 0x020006F1 RID: 1777
	public class EntityTypeRespawnTracker
	{
		// Token: 0x04003973 RID: 14707
		public int entityTypeID;

		// Token: 0x04003974 RID: 14708
		public long entityCreateData;

		// Token: 0x04003975 RID: 14709
		public float entityNextRespawnTime;
	}

	// Token: 0x020006F2 RID: 1778
	public enum EntityGroupTypes
	{
		// Token: 0x04003977 RID: 14711
		EnemyChaser,
		// Token: 0x04003978 RID: 14712
		EnemyChaserArmored,
		// Token: 0x04003979 RID: 14713
		EnemyRanged,
		// Token: 0x0400397A RID: 14714
		EnemyRangedArmored,
		// Token: 0x0400397B RID: 14715
		CollectibleFlower,
		// Token: 0x0400397C RID: 14716
		BarrierEnergyCostGate,
		// Token: 0x0400397D RID: 14717
		BarrierSpectralWall,
		// Token: 0x0400397E RID: 14718
		HazardSpectralLiquid
	}

	// Token: 0x020006F3 RID: 1779
	public enum EnemyType
	{
		// Token: 0x04003980 RID: 14720
		Chaser,
		// Token: 0x04003981 RID: 14721
		Ranged,
		// Token: 0x04003982 RID: 14722
		Phantom,
		// Token: 0x04003983 RID: 14723
		Environment,
		// Token: 0x04003984 RID: 14724
		CustomMapsEnemy
	}

	// Token: 0x020006F4 RID: 1780
	public struct EnemyEntityCreateData
	{
		// Token: 0x06002CEA RID: 11498 RVA: 0x000F3636 File Offset: 0x000F1836
		private static long PackData(int value, int nbits, int shift)
		{
			return ((long)value & (long)((1 << nbits) - 1)) << shift;
		}

		// Token: 0x06002CEB RID: 11499 RVA: 0x000F3649 File Offset: 0x000F1849
		private static int UnpackData(long createData, int nbits, int shift)
		{
			return (int)(createData >> shift & (long)((1 << nbits) - 1));
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x000F365C File Offset: 0x000F185C
		public static GhostReactor.EnemyEntityCreateData Unpack(long bits)
		{
			return new GhostReactor.EnemyEntityCreateData
			{
				respawnCount = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 16),
				sectionIndex = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 8),
				patrolIndex = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 0)
			};
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x000F36A0 File Offset: 0x000F18A0
		public long Pack()
		{
			return GhostReactor.EnemyEntityCreateData.PackData(this.respawnCount, 8, 16) | GhostReactor.EnemyEntityCreateData.PackData(this.sectionIndex, 8, 8) | GhostReactor.EnemyEntityCreateData.PackData(this.patrolIndex, 8, 0);
		}

		// Token: 0x04003985 RID: 14725
		public int respawnCount;

		// Token: 0x04003986 RID: 14726
		public int sectionIndex;

		// Token: 0x04003987 RID: 14727
		public int patrolIndex;
	}

	// Token: 0x020006F5 RID: 1781
	public struct ToolEntityCreateData
	{
		// Token: 0x06002CEE RID: 11502 RVA: 0x000F3636 File Offset: 0x000F1836
		private static long PackData(int value, int nbits, int shift)
		{
			return ((long)value & (long)((1 << nbits) - 1)) << shift;
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x000F3649 File Offset: 0x000F1849
		private static int UnpackData(long createData, int nbits, int shift)
		{
			return (int)(createData >> shift & (long)((1 << nbits) - 1));
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000F36CC File Offset: 0x000F18CC
		public static GhostReactor.ToolEntityCreateData Unpack(long bits)
		{
			GhostReactor.ToolEntityCreateData result = default(GhostReactor.ToolEntityCreateData);
			result.stationIndex = GhostReactor.ToolEntityCreateData.UnpackData(bits, 8, 0) - 1;
			int num = GhostReactor.ToolEntityCreateData.UnpackData(bits, 8, 8);
			result.decayTime = 5f * (float)num;
			return result;
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000F370B File Offset: 0x000F190B
		public long Pack()
		{
			long result = GhostReactor.ToolEntityCreateData.PackData(this.stationIndex + 1, 8, 0);
			GhostReactor.ToolEntityCreateData.PackData((int)(this.decayTime / 5f), 8, 8);
			return result;
		}

		// Token: 0x04003988 RID: 14728
		public int stationIndex;

		// Token: 0x04003989 RID: 14729
		public float decayTime;
	}
}
