using System;
using System.Collections.Generic;
using System.IO;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000143 RID: 323
public class SIPlayer : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000810 RID: 2064 RVA: 0x0002C1F1 File Offset: 0x0002A3F1
	public static SIPlayer LocalPlayer
	{
		get
		{
			return SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000811 RID: 2065 RVA: 0x0002C207 File Offset: 0x0002A407
	public int TotalGadgetLimit
	{
		get
		{
			if (!this.gamePlayer.IsSubscribed)
			{
				return 3;
			}
			return 6;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000812 RID: 2066 RVA: 0x0002C219 File Offset: 0x0002A419
	// (set) Token: 0x06000813 RID: 2067 RVA: 0x0002C221 File Offset: 0x0002A421
	public bool TickRunning { get; set; }

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000814 RID: 2068 RVA: 0x0002C22A File Offset: 0x0002A42A
	public int ActorNr
	{
		get
		{
			if (!this.gamePlayer.rig.isOfflineVRRig)
			{
				return this.gamePlayer.rig.OwningNetPlayer.ActorNumber;
			}
			return NetworkSystem.Instance.LocalPlayerID;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x06000815 RID: 2069 RVA: 0x0002C25E File Offset: 0x0002A45E
	public SIPlayer.ProgressionData CurrentProgression
	{
		get
		{
			return this.currentProgression;
		}
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x0002C268 File Offset: 0x0002A468
	private void Awake()
	{
		this.activePlayerGadgets = new List<int>();
		SIPlayer.progressionSO = this.progressionSORef;
		this.clientToAuthorityRPCLimiter = new CallLimiter(25, 1f, 0.5f);
		this.clientToClientRPCLimiter = new CallLimiter(25, 1f, 0.5f);
		this.authorityToClientRPCLimiter = new CallLimiter(25, 1f, 0.5f);
		this.currentProgression = new SIPlayer.ProgressionData(true);
		GamePlayer gamePlayer = this.gamePlayer;
		gamePlayer.OnPlayerLeftZone = (Action)Delegate.Combine(gamePlayer.OnPlayerLeftZone, new Action(this.ClearGadgetsOnLeaveZone));
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0002C303 File Offset: 0x0002A503
	private void OnEnable()
	{
		if (this == SIPlayer.LocalPlayer)
		{
			TickSystem<object>.AddTickCallback(this);
		}
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0002C318 File Offset: 0x0002A518
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<object>.RemoveTickCallback(this);
		this.Reset();
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x0002C32E File Offset: 0x0002A52E
	public void Reset()
	{
		if (SIPlayer.LocalPlayer == this)
		{
			SIProgression.StaticSaveQuestProgress();
			SIProgression.StaticClearAllQuestEventListeners();
		}
		this.lastQuestsAvailableToClaim = 999;
		this.tpParticleSystem.Stop();
		this.netInitialized = false;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0002C364 File Offset: 0x0002A564
	public static SIPlayer Get(int actorNumber)
	{
		SIPlayer result;
		if (SIPlayer.siPlayerByActorNr.TryGetValue(actorNumber, out result))
		{
			return result;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer))
		{
			return null;
		}
		SIPlayer.siPlayerByActorNr.Add(actorNumber, gamePlayer.GetComponent<SIPlayer>());
		return SIPlayer.siPlayerByActorNr[actorNumber];
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0002C3AA File Offset: 0x0002A5AA
	public static void ClearPlayerCache()
	{
		SIPlayer.siPlayerByActorNr.Clear();
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002C3B6 File Offset: 0x0002A5B6
	public static SIPlayer Get(VRRig vrRig)
	{
		if (!vrRig)
		{
			return null;
		}
		return vrRig.GetComponent<SIPlayer>();
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0002C3C8 File Offset: 0x0002A5C8
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		for (int i = 0; i < 6; i++)
		{
			writer.Write(this.CurrentProgression.resourceArray[i]);
		}
		for (int j = 0; j < 2; j++)
		{
			writer.Write(this.CurrentProgression.limitedDepositTimeArray[j]);
		}
		for (int k = 0; k < SIPlayer.progressionSO.TreePageCount; k++)
		{
			for (int l = 0; l < SIPlayer.progressionSO.TreeNodeCounts[k]; l++)
			{
				writer.Write(this.CurrentProgression.techTreeData[k][l]);
			}
		}
		writer.Write((byte)this.CurrentProgression.stashedQuests);
		writer.Write((byte)this.CurrentProgression.stashedBonusPoints);
		writer.Write((byte)this.CurrentProgression.bonusProgress);
		for (int m = 0; m < this.CurrentProgression.currentQuestIds.Length; m++)
		{
			writer.Write(this.CurrentProgression.currentQuestIds[m]);
			writer.Write(this.CurrentProgression.currentQuestProgresses[m]);
		}
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0002C4D0 File Offset: 0x0002A6D0
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, SIPlayer player, SuperInfectionManager siManager)
	{
		if (player == null || player == SIPlayer.LocalPlayer)
		{
			for (int i = 0; i < 6; i++)
			{
				reader.ReadInt32();
			}
			for (int j = 0; j < 2; j++)
			{
				reader.ReadInt32();
			}
			for (int k = 0; k < SIPlayer.progressionSO.TreePageCount; k++)
			{
				for (int l = 0; l < SIPlayer.progressionSO.TreeNodeCounts[k]; l++)
				{
					reader.ReadBoolean();
				}
			}
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			return;
		}
		int[] array = new int[6];
		int[] array2 = new int[2];
		bool[][] array3 = new bool[SIPlayer.progressionSO.TreePageCount][];
		for (int m = 0; m < 6; m++)
		{
			array[m] = reader.ReadInt32();
		}
		for (int n = 0; n < 2; n++)
		{
			array2[n] = reader.ReadInt32();
		}
		for (int num = 0; num < SIPlayer.progressionSO.TreePageCount; num++)
		{
			array3[num] = new bool[SIPlayer.progressionSO.TreeNodeCounts[num]];
			for (int num2 = 0; num2 < SIPlayer.progressionSO.TreeNodeCounts[num]; num2++)
			{
				array3[num][num2] = reader.ReadBoolean();
			}
		}
		int stashedQuests = (int)reader.ReadByte();
		int stashedBonusPoints = (int)reader.ReadByte();
		int bonusProgress = (int)reader.ReadByte();
		int[] array4 = new int[3];
		int[] array5 = new int[3];
		for (int num3 = 0; num3 < 3; num3++)
		{
			array4[num3] = reader.ReadInt32();
			array5[num3] = reader.ReadInt32();
		}
		player.UpdateProgression(array, array2, array3, stashedQuests, stashedBonusPoints, bonusProgress, array4, array5);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002C6A9 File Offset: 0x0002A8A9
	public bool HasLimitedResourceBeenDeposited(SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			return false;
		}
		if (this == SIPlayer.LocalPlayer)
		{
			return SIProgression.Instance.DailyLimitedTurnedIn;
		}
		return this.CurrentProgression.limitedDepositTimeArray[(int)limitedDepositType] > 0;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002C6D8 File Offset: 0x0002A8D8
	public bool CanLimitedResourceBeDeposited(SIResource.LimitedDepositType limitedDepositType)
	{
		return limitedDepositType == SIResource.LimitedDepositType.None || (SIProgression.Instance && SIProgression.Instance.IsLimitedDepositAvailable(limitedDepositType));
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
	public void GatherResource(SIResource.ResourceType type, SIResource.LimitedDepositType limitedDepositType, int count)
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		Dictionary<SIResource.ResourceType, int> resourceDict = SIProgression.Instance.resourceDict;
		resourceDict[type] += count;
		SIProgression.Instance.ApplyLimitedDepositTime(limitedDepositType);
		bool flag = type == SIResource.ResourceType.TechPoint || (limitedDepositType == SIResource.LimitedDepositType.MonkeIdol && SIProgression.Instance.limitedDepositTimeArray[(int)limitedDepositType] != 1) || SIProgression.Instance.TryDepositResources(type, count);
		if (type == SIResource.ResourceType.StrangeWood)
		{
			PlayerGameEvents.MiscEvent("SIStrangeWoodCollect", 1);
		}
		else if (type == SIResource.ResourceType.WeirdGear)
		{
			PlayerGameEvents.MiscEvent("SISWeirdGearCollect", 1);
		}
		else if (type == SIResource.ResourceType.FloppyMetal)
		{
			PlayerGameEvents.MiscEvent("SIFloppyMetalCollect", 1);
		}
		else if (type == SIResource.ResourceType.BouncySand)
		{
			PlayerGameEvents.MiscEvent("SIBouncySandCollect", 1);
		}
		else if (type == SIResource.ResourceType.VibratingSpring)
		{
			PlayerGameEvents.MiscEvent("SIVibratingSpringCollect", 1);
		}
		if (activeSuperInfectionManager != null && activeSuperInfectionManager.zoneSuperInfection != null && flag)
		{
			SIProgression.Instance.CollectResourceTelemetry(type, count);
		}
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0002C7D4 File Offset: 0x0002A9D4
	public void GetBonusProgress(SuperInfectionManager manager)
	{
		if (SIProgression.Instance.stashedBonusPoints <= 0)
		{
			return;
		}
		SIProgression.Instance.GetBonusProgress();
		this.BonusProgressCelebrate();
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0002C7F9 File Offset: 0x0002A9F9
	public int GetResourceAmount(SIResource.ResourceType type)
	{
		return this.CurrentProgression.resourceArray[(int)type];
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002C808 File Offset: 0x0002AA08
	public void SetProgressionLocal()
	{
		this.currentProgression = new SIPlayer.ProgressionData(SIProgression.Instance);
		int num = 0;
		if (this.currentProgression.techTreeData != null)
		{
			for (int i = 0; i < this.currentProgression.techTreeData.Length; i++)
			{
				if (this.currentProgression.techTreeData[i] != null)
				{
					for (int j = 0; j < this.currentProgression.techTreeData[i].Length; j++)
					{
						if (this.currentProgression.techTreeData[i][j])
						{
							num++;
						}
					}
				}
			}
		}
		this.gamePlayer.SetInitializePlayer(true);
		this.UpdateVisualsForAvailableQuestRedemption();
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0002C89C File Offset: 0x0002AA9C
	public void UpdateProgression(int[] resourceArray, int[] limitedDepositTimeArray, bool[][] techTreeData, int _stashedQuests, int _stashedBonusPoints, int _bonusProgress, int[] _currentQuestIds, int[] _currentQuestProgresses)
	{
		SIPlayer.ProgressionData newProgression = new SIPlayer.ProgressionData(resourceArray, limitedDepositTimeArray, techTreeData, _stashedQuests, _stashedBonusPoints, _bonusProgress, _currentQuestIds, _currentQuestProgresses);
		if (this.netInitialized)
		{
			this.CelebrateIfQuestProgressMade(newProgression);
		}
		else
		{
			this.netInitialized = true;
			this.currentProgression = newProgression;
			this.gamePlayer.SetInitializePlayer(true);
		}
		this.currentProgression = newProgression;
		this.UpdateVisualsForAvailableQuestRedemption();
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0002C8F8 File Offset: 0x0002AAF8
	public void CelebrateIfQuestProgressMade(SIPlayer.ProgressionData newProgression)
	{
		int num = this.QuestsAvailableToClaim();
		if (this.currentProgression.bonusProgress < newProgression.bonusProgress && this.currentProgression.stashedBonusPoints == newProgression.stashedBonusPoints && this.currentProgression.stashedBonusPoints > 0)
		{
			this.BonusProgressCelebrate();
		}
		bool flag = num > 0 && this.currentProgression.stashedQuests > newProgression.stashedQuests;
		bool flag2 = this.currentProgression.bonusProgress >= 4 && this.currentProgression.stashedBonusPoints > newProgression.stashedBonusPoints;
		bool flag3 = this.currentProgression.limitedDepositTimeArray[1] == 0 && newProgression.limitedDepositTimeArray[1] == 1;
		if ((flag || flag2 || flag3) && this.currentProgression.resourceArray[0] < newProgression.resourceArray[0])
		{
			this.TechPointGrantedCelebrate();
		}
		if (num > this.lastQuestsAvailableToClaim)
		{
			this.questCompleteCelebrate.SetActive(true);
		}
		this.lastQuestsAvailableToClaim = num;
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0002C9E4 File Offset: 0x0002ABE4
	public void TechPointGrantedCelebrate()
	{
		SIQuestBoard questBoard = SuperInfectionManager.activeSuperInfectionManager.zoneSuperInfection.questBoard;
		if (this != SIPlayer.LocalPlayer)
		{
			questBoard.GrantBonusPointProgress();
		}
		if (this.techPointGainedCelebrate != null)
		{
			this.techPointGainedCelebrate.SetActive(false);
			this.techPointGainedCelebrate.SetActive(true);
		}
		else
		{
			Debug.LogError("[SIPlayer]  ERROR!!!  Null reference: `techPointGainedCelebrate`.");
		}
		if (!questBoard.celebrateParticle)
		{
			Debug.LogError("[SIPlayer]  ERROR!!!  SuperInfectionManager.zoneSuperInfection.questBoard.celebrateParticle != null");
		}
		questBoard.celebrateParticle.Play();
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0002CA68 File Offset: 0x0002AC68
	public void BonusProgressCelebrate()
	{
		this.bonusProgressionCelebrate.SetActive(false);
		this.bonusProgressionCelebrate.SetActive(true);
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x0002CA84 File Offset: 0x0002AC84
	public bool AttemptUnlockNode(SIUpgradeType upgrade, SuperInfectionManager manager)
	{
		if (this.CurrentProgression.techTreeData[upgrade.GetPageId()][upgrade.GetNodeId()])
		{
			return false;
		}
		SITechTreeNode treeNode = SIPlayer.progressionSO.GetTreeNode(upgrade);
		if (!this.PlayerCanAffordNode(treeNode))
		{
			return false;
		}
		this.PurchaseNode(treeNode);
		return true;
	}

	// Token: 0x0600082A RID: 2090 RVA: 0x0002CAD0 File Offset: 0x0002ACD0
	public bool PlayerCanAffordNode(SITechTreeNode node)
	{
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					if (this.CurrentProgression.resourceArray[(int)keyValuePair.Key] < keyValuePair.Value)
					{
						return false;
					}
				}
				return true;
			}
		}
		foreach (SIResource.ResourceCost resourceCost in node.nodeCost)
		{
			if (this.CurrentProgression.resourceArray[(int)resourceCost.type] < resourceCost.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600082B RID: 2091 RVA: 0x0002CB98 File Offset: 0x0002AD98
	public void PurchaseNode(SITechTreeNode node)
	{
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					SIProgression.Instance.GetResourceArray()[(int)keyValuePair.Key] -= keyValuePair.Value;
				}
				goto IL_A8;
			}
		}
		foreach (SIResource.ResourceCost resourceCost in node.nodeCost)
		{
			SIProgression.Instance.GetResourceArray()[(int)resourceCost.type] -= resourceCost.amount;
		}
		IL_A8:
		SIProgression.Instance.unlockedTechTreeData[node.upgradeType.GetPageId()][node.upgradeType.GetNodeId()] = true;
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0002CC88 File Offset: 0x0002AE88
	public bool NodeResearched(SIUpgradeType upgrade)
	{
		return this.CurrentProgression.techTreeData[upgrade.GetPageId()][upgrade.GetNodeId()];
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0002CCA4 File Offset: 0x0002AEA4
	public SIUpgradeSet GetUpgrades(SITechTreePageId pageId)
	{
		SIUpgradeSet result = default(SIUpgradeSet);
		bool[] array = this.CurrentProgression.techTreeData[(int)pageId];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i])
			{
				result.Add(i);
			}
		}
		return result;
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002CCE4 File Offset: 0x0002AEE4
	public bool NodeParentsUnlocked(SIUpgradeType upgrade)
	{
		SITechTreeNode treeNode = SIPlayer.progressionSO.GetTreeNode(upgrade);
		for (int i = 0; i < treeNode.parentUpgrades.Length; i++)
		{
			if (!this.NodeResearched(treeNode.parentUpgrades[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002CD24 File Offset: 0x0002AF24
	public void ResetTechTree()
	{
		SIProgression.Instance.unlockedTechTreeData = new bool[SIPlayer.progressionSO.TreePageCount][];
		for (int i = 0; i < SIPlayer.progressionSO.TreePageCount; i++)
		{
			SIProgression.Instance.unlockedTechTreeData[i] = new bool[SIPlayer.progressionSO.TreeNodeCounts[i]];
		}
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002CD81 File Offset: 0x0002AF81
	public void ResetResources()
	{
		SIProgression.Instance.resourceDict = null;
		SIProgression.Instance.EnsureInitialized();
		SIProgression.Instance.limitedDepositTimeArray = new int[2];
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002CDAD File Offset: 0x0002AFAD
	public static void SetAndBroadcastProgression()
	{
		SIPlayer.LocalPlayer.SetAndBroadcastProgressionLocal();
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0002CDBC File Offset: 0x0002AFBC
	public void SetAndBroadcastProgressionLocal()
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		this.SetProgressionLocal();
		if (!NetworkSystem.Instance.InRoom || activeSuperInfectionManager == null)
		{
			return;
		}
		activeSuperInfectionManager.CallRPC(SuperInfectionManager.ClientToClientRPC.BroadcastProgression, new object[]
		{
			SIPlayer.LocalPlayer.CurrentProgression.resourceArray,
			SIPlayer.LocalPlayer.currentProgression.limitedDepositTimeArray,
			SIPlayer.LocalPlayer.currentProgression.techTreeData,
			SIPlayer.LocalPlayer.currentProgression.stashedQuests,
			SIPlayer.LocalPlayer.currentProgression.stashedBonusPoints,
			SIPlayer.LocalPlayer.currentProgression.bonusProgress,
			SIPlayer.LocalPlayer.currentProgression.currentQuestIds,
			SIPlayer.LocalPlayer.currentProgression.currentQuestProgresses
		});
		if (activeSuperInfectionManager.zoneSuperInfection != null)
		{
			activeSuperInfectionManager.zoneSuperInfection.RefreshStations(SIPlayer.LocalPlayer.ActorNr);
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0002CEBC File Offset: 0x0002B0BC
	public void UpdateVisualsForAvailableQuestRedemption()
	{
		bool flag = SuperInfectionManager.activeSuperInfectionManager != null && SuperInfectionManager.activeSuperInfectionManager.IsZoneReady() && (this.QuestsAvailableToClaim() > 0 || (this.currentProgression.bonusProgress >= 4 && this.currentProgression.stashedBonusPoints > 0));
		if (this.tpParticleSystem.isPlaying && !flag)
		{
			this.tpParticleSystem.Stop();
			return;
		}
		if (!this.tpParticleSystem.isPlaying && flag)
		{
			this.tpParticleSystem.Play();
		}
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0002CF4C File Offset: 0x0002B14C
	public int QuestsAvailableToClaim()
	{
		int num = 0;
		for (int i = 0; i < this.currentProgression.currentQuestIds.Length; i++)
		{
			if (SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[i]) != null && this.currentProgression.currentQuestProgresses[i] >= SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[i]).requiredOccurenceCount)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x0002CFC8 File Offset: 0x0002B1C8
	public bool QuestAvailableToClaim(int questIndex)
	{
		return SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[questIndex]) != null && this.currentProgression.currentQuestProgresses[questIndex] >= SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[questIndex]).requiredOccurenceCount;
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0002D028 File Offset: 0x0002B228
	public void TriggerIdolDepositedCelebration(Vector3 position)
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (activeSuperInfectionManager.gameEntityManager.IsAuthority())
		{
			activeSuperInfectionManager.CallRPC(SuperInfectionManager.AuthorityToClientRPC.TriggerMonkeIdolDepositCelebration, new object[]
			{
				position
			});
		}
		this.monkeIdolDepositCelebrate.transform.position = position;
		this.monkeIdolDepositCelebrate.SetActive(false);
		this.monkeIdolDepositCelebrate.SetActive(true);
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0002D087 File Offset: 0x0002B287
	public void ClearGadgetsOnLeaveZone()
	{
		if (!SuperInfectionManager.activeSuperInfectionManager.gameEntityManager.IsAuthority())
		{
			return;
		}
		SuperInfectionManager.activeSuperInfectionManager.ClearPlayerGadgets(this);
	}

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000838 RID: 2104 RVA: 0x0002D0A8 File Offset: 0x0002B2A8
	// (remove) Token: 0x06000839 RID: 2105 RVA: 0x0002D0E0 File Offset: 0x0002B2E0
	public event Action<Vector3> OnKnockback;

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x0600083A RID: 2106 RVA: 0x0002D118 File Offset: 0x0002B318
	// (remove) Token: 0x0600083B RID: 2107 RVA: 0x0002D150 File Offset: 0x0002B350
	public event Action OnBlasterHit;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x0600083C RID: 2108 RVA: 0x0002D188 File Offset: 0x0002B388
	// (remove) Token: 0x0600083D RID: 2109 RVA: 0x0002D1C0 File Offset: 0x0002B3C0
	public event Action OnBlasterSplashHit;

	// Token: 0x0600083E RID: 2110 RVA: 0x0002D1F5 File Offset: 0x0002B3F5
	public void NotifyBlasterHit()
	{
		Action onBlasterHit = this.OnBlasterHit;
		if (onBlasterHit == null)
		{
			return;
		}
		onBlasterHit();
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0002D207 File Offset: 0x0002B407
	public void NotifyBlasterSplashHit()
	{
		Action onBlasterSplashHit = this.OnBlasterSplashHit;
		if (onBlasterSplashHit == null)
		{
			return;
		}
		onBlasterSplashHit();
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0002D219 File Offset: 0x0002B419
	public void PlayerKnockback(Vector3 directionAndMagnitude, bool forceOffGround = true, bool applyExclusionZone = true)
	{
		if (applyExclusionZone && this.exclusionZoneCount > 0)
		{
			return;
		}
		Action<Vector3> onKnockback = this.OnKnockback;
		if (onKnockback != null)
		{
			onKnockback(directionAndMagnitude);
		}
		GTPlayer.Instance.ApplyClampedKnockback(directionAndMagnitude.normalized, directionAndMagnitude.magnitude, 1.5f, forceOffGround);
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0002D258 File Offset: 0x0002B458
	public void PlayerHandHaptic(bool isLeft, float hapticStrength, float hapticDuration, bool applyExclusionZone = true)
	{
		if (applyExclusionZone && this.exclusionZoneCount > 0)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(isLeft, hapticStrength, hapticDuration);
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0002D278 File Offset: 0x0002B478
	public void Tick()
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		bool isSupercharged = activeSuperInfectionManager != null && activeSuperInfectionManager.IsSupercharged;
		if (!SIPlayer._TryUpdateSlotEntityCharge(this.gamePlayer, 0, isSupercharged))
		{
			SIPlayer._TryUpdateSlotEntityCharge(this.gamePlayer, 2, isSupercharged);
		}
		if (!SIPlayer._TryUpdateSlotEntityCharge(this.gamePlayer, 1, isSupercharged))
		{
			SIPlayer._TryUpdateSlotEntityCharge(this.gamePlayer, 3, isSupercharged);
		}
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0002D2D4 File Offset: 0x0002B4D4
	private static bool _TryUpdateSlotEntityCharge(GamePlayer gamePlayer, int slotIndex, bool isSupercharged)
	{
		GameEntity gameEntity;
		if (!gamePlayer.TryGetSlotEntity(slotIndex, out gameEntity))
		{
			GamePlayer.SlotData slotData;
			if (gamePlayer.TryGetSlotData(slotIndex, out slotData) && Time.time - SIPlayer._debug_lastStaleSlotLogTime > 5f)
			{
				SIPlayer._debug_lastStaleSlotLogTime = Time.time;
			}
			return false;
		}
		IEnergyGadget component = gameEntity.GetComponent<IEnergyGadget>();
		if (component == null || !component.UsesEnergy || component.IsFull)
		{
			return false;
		}
		float dt = Time.deltaTime * (isSupercharged ? 5f : 1f);
		component.UpdateRecharge(dt);
		return true;
	}

	// Token: 0x04000A2E RID: 2606
	private const string preLog = "[SIPlayer]  ";

	// Token: 0x04000A2F RID: 2607
	private const string preErr = "[SIPlayer]  ERROR!!!  ";

	// Token: 0x04000A30 RID: 2608
	public GamePlayer gamePlayer;

	// Token: 0x04000A31 RID: 2609
	private static Dictionary<int, SIPlayer> siPlayerByActorNr = new Dictionary<int, SIPlayer>();

	// Token: 0x04000A32 RID: 2610
	public CallLimiter clientToAuthorityRPCLimiter;

	// Token: 0x04000A33 RID: 2611
	public CallLimiter clientToClientRPCLimiter;

	// Token: 0x04000A34 RID: 2612
	public CallLimiter authorityToClientRPCLimiter;

	// Token: 0x04000A35 RID: 2613
	public static SITechTreeSO progressionSO;

	// Token: 0x04000A36 RID: 2614
	public SITechTreeSO progressionSORef;

	// Token: 0x04000A37 RID: 2615
	public ParticleSystem tpParticleSystem;

	// Token: 0x04000A38 RID: 2616
	public GameObject bonusProgressionCelebrate;

	// Token: 0x04000A39 RID: 2617
	[FormerlySerializedAs("testPointGainedCelebrate")]
	public GameObject techPointGainedCelebrate;

	// Token: 0x04000A3A RID: 2618
	public GameObject monkeIdolDepositCelebrate;

	// Token: 0x04000A3B RID: 2619
	public GameObject questCompleteCelebrate;

	// Token: 0x04000A3C RID: 2620
	private int lastQuestsAvailableToClaim;

	// Token: 0x04000A3D RID: 2621
	private const int STANDARD_GADGET_LIMIT = 3;

	// Token: 0x04000A3E RID: 2622
	private const int SUBSCRIBER_GADGET_LIMIT = 6;

	// Token: 0x04000A3F RID: 2623
	[NonSerialized]
	public int exclusionZoneCount;

	// Token: 0x04000A41 RID: 2625
	public bool netInitialized;

	// Token: 0x04000A42 RID: 2626
	private SIPlayer.ProgressionData currentProgression;

	// Token: 0x04000A43 RID: 2627
	public List<int> activePlayerGadgets = new List<int>();

	// Token: 0x04000A47 RID: 2631
	private static float _debug_lastStaleSlotLogTime;

	// Token: 0x02000144 RID: 324
	[Serializable]
	public struct ProgressionData
	{
		// Token: 0x06000846 RID: 2118 RVA: 0x0002D370 File Offset: 0x0002B570
		public ProgressionData(bool itsNullLol)
		{
			this.resourceArray = new int[6];
			this.limitedDepositTimeArray = new int[2];
			this.techTreeData = new bool[SIPlayer.progressionSO.TreePageCount][];
			for (int i = 0; i < SIPlayer.progressionSO.TreePageCount; i++)
			{
				this.techTreeData[i] = new bool[SIPlayer.progressionSO.TreeNodeCounts[i]];
			}
			this.stashedQuests = 0;
			this.stashedBonusPoints = 0;
			this.bonusProgress = 0;
			this.currentQuestIds = new int[3];
			this.currentQuestProgresses = new int[3];
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0002D405 File Offset: 0x0002B605
		public ProgressionData(int[] _resourceArray, int[] _limitedDepositTimeArray, bool[][] _techTreeData, int _stashedQuests, int _stashedBonusPoints, int _bonusProgress, int[] _currentQuestIds, int[] _currentQuestProgresses)
		{
			this.resourceArray = _resourceArray;
			this.limitedDepositTimeArray = _limitedDepositTimeArray;
			this.techTreeData = _techTreeData;
			this.stashedQuests = _stashedQuests;
			this.stashedBonusPoints = _stashedBonusPoints;
			this.bonusProgress = _bonusProgress;
			this.currentQuestIds = _currentQuestIds;
			this.currentQuestProgresses = _currentQuestProgresses;
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x0002D444 File Offset: 0x0002B644
		public ProgressionData(SIProgression siProgression)
		{
			this.resourceArray = siProgression.GetResourceArray();
			this.limitedDepositTimeArray = siProgression.limitedDepositTimeArray;
			this.techTreeData = siProgression.unlockedTechTreeData;
			this.stashedQuests = siProgression.stashedQuests;
			this.stashedBonusPoints = siProgression.stashedBonusPoints;
			this.bonusProgress = siProgression.bonusProgress;
			this.currentQuestIds = siProgression.ActiveQuestIds;
			this.currentQuestProgresses = siProgression.ActiveQuestProgresses;
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x0002D4B1 File Offset: 0x0002B6B1
		public bool IsUnlocked(SIUpgradeType upgradeType)
		{
			return this.techTreeData[upgradeType.GetPageId()][upgradeType.GetNodeId()];
		}

		// Token: 0x04000A48 RID: 2632
		public int[] resourceArray;

		// Token: 0x04000A49 RID: 2633
		public int[] limitedDepositTimeArray;

		// Token: 0x04000A4A RID: 2634
		public bool[][] techTreeData;

		// Token: 0x04000A4B RID: 2635
		public int stashedQuests;

		// Token: 0x04000A4C RID: 2636
		public int stashedBonusPoints;

		// Token: 0x04000A4D RID: 2637
		public int bonusProgress;

		// Token: 0x04000A4E RID: 2638
		public int[] currentQuestIds;

		// Token: 0x04000A4F RID: 2639
		public int[] currentQuestProgresses;
	}
}
