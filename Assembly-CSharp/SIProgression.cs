using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000145 RID: 325
[DefaultExecutionOrder(-100)]
public class SIProgression : MonoBehaviour, IGorillaSliceableSimple, GorillaQuestManager
{
	// Token: 0x170000AB RID: 171
	// (get) Token: 0x0600084A RID: 2122 RVA: 0x0002D4C7 File Offset: 0x0002B6C7
	// (set) Token: 0x0600084B RID: 2123 RVA: 0x0002D4CE File Offset: 0x0002B6CE
	public static SIProgression Instance { get; private set; }

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x0600084C RID: 2124 RVA: 0x0002D4D8 File Offset: 0x0002B6D8
	// (remove) Token: 0x0600084D RID: 2125 RVA: 0x0002D510 File Offset: 0x0002B710
	public event Action OnClientReady;

	// Token: 0x0600084E RID: 2126 RVA: 0x0002D548 File Offset: 0x0002B748
	private void Awake()
	{
		if (SIProgression.Instance == null)
		{
			SIProgression.Instance = this;
		}
		this.emptyNode = default(SIProgression.SINode);
		SIProgression.InitResourceToStringDictionary();
		this.resourceCapsArray = Enumerable.Repeat<int>(int.MaxValue, 6).ToArray<int>();
		for (int i = 0; i < this.resourceCaps.Length; i++)
		{
			this.resourceCapsArray[(int)this.resourceCaps[i].resourceType] = this.resourceCaps[i].resourceMax;
		}
		foreach (object obj in Enum.GetValues(typeof(SITechTreePageId)))
		{
			SITechTreePageId key = (SITechTreePageId)obj;
			this.heldOrSnappedByGadgetPageType.Add(key, 0);
		}
		this.EnsureInitialized();
		SIProgression.InitializeQuests();
		this.ResetTelemetryIntervalData();
		this.LoadSavedTelemetryData();
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002D640 File Offset: 0x0002B840
	public void OnEnable()
	{
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated += this.HandleTreeUpdated;
			ProgressionManager.Instance.OnInventoryUpdated += this.HandleInventoryUpdated;
			ProgressionManager.Instance.OnNodeUnlocked += this.HandleNodeUnlocked;
			ProgressionManager.Instance.RefreshProgressionTree();
			ProgressionManager.Instance.RefreshUserInventory();
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002D6B8 File Offset: 0x0002B8B8
	public void OnDisable()
	{
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated -= this.HandleTreeUpdated;
			ProgressionManager.Instance.OnInventoryUpdated -= this.HandleInventoryUpdated;
			ProgressionManager.Instance.OnNodeUnlocked -= this.HandleNodeUnlocked;
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0002D71C File Offset: 0x0002B91C
	public static string GetResourceString(SIResource.ResourceType resourceType)
	{
		if (SIProgression._resourceToString == null)
		{
			SIProgression.InitResourceToStringDictionary();
		}
		return SIProgression._resourceToString[resourceType];
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0002D738 File Offset: 0x0002B938
	private static void InitResourceToStringDictionary()
	{
		SIProgression._resourceToString = new Dictionary<SIResource.ResourceType, string>();
		SIProgression._resourceToString[SIResource.ResourceType.TechPoint] = "SI_TechPoints";
		SIProgression._resourceToString[SIResource.ResourceType.StrangeWood] = "SI_StrangeWood";
		SIProgression._resourceToString[SIResource.ResourceType.WeirdGear] = "SI_WeirdGear";
		SIProgression._resourceToString[SIResource.ResourceType.VibratingSpring] = "SI_VibratingSpring";
		SIProgression._resourceToString[SIResource.ResourceType.BouncySand] = "SI_BouncySand";
		SIProgression._resourceToString[SIResource.ResourceType.FloppyMetal] = "SI_FloppyMetal";
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002D7AF File Offset: 0x0002B9AF
	public void Init()
	{
		SIPlayer.LocalPlayer.SetProgressionLocal();
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.RefreshProgressionTree();
			ProgressionManager.Instance.RefreshUserInventory();
		}
		this.ClearAllQuestEventListeners();
		this.SetupAllQuestEventListeners();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002D7E8 File Offset: 0x0002B9E8
	public void EnsureInitialized()
	{
		if (this.techTreeSO != null)
		{
			this.techTreeSO.EnsureInitialized();
		}
		if (SIPlayer.progressionSO == null)
		{
			SIPlayer.progressionSO = this.techTreeSO;
		}
		int num = 6;
		if (this.resourceDict == null || this.resourceDict.Count != num)
		{
			this.resourceDict = new Dictionary<SIResource.ResourceType, int>();
			this.resourceDict[SIResource.ResourceType.TechPoint] = 0;
			this.resourceDict[SIResource.ResourceType.StrangeWood] = 0;
			this.resourceDict[SIResource.ResourceType.VibratingSpring] = 0;
			this.resourceDict[SIResource.ResourceType.BouncySand] = 0;
			this.resourceDict[SIResource.ResourceType.FloppyMetal] = 0;
			this.resourceDict[SIResource.ResourceType.WeirdGear] = 0;
		}
		int num2 = 2;
		if (this.limitedDepositTimeArray == null || this.limitedDepositTimeArray.Length != num2)
		{
			this.limitedDepositTimeArray = new int[num2];
		}
		int treePageCount = SIPlayer.progressionSO.TreePageCount;
		if (this.unlockedTechTreeData == null || this.unlockedTechTreeData.Length != treePageCount)
		{
			this.unlockedTechTreeData = new bool[treePageCount][];
		}
		for (int i = 0; i < treePageCount; i++)
		{
			int num3 = SIPlayer.progressionSO.TreeNodeCounts[i];
			if (this.unlockedTechTreeData[i] == null || this.unlockedTechTreeData[i].Length != num3)
			{
				this.unlockedTechTreeData[i] = new bool[num3];
			}
		}
		if (this.activeQuestIds == null || this.activeQuestIds.Length != 3)
		{
			this.activeQuestIds = new int[3];
		}
		if (this.activeQuestProgresses == null || this.activeQuestProgresses.Length != 3)
		{
			this.activeQuestProgresses = new int[3];
		}
		this.CopySaveDataToDiff();
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0002D968 File Offset: 0x0002BB68
	private void ApplyServerQuestsStatus(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		if (userQuestsStatus == null)
		{
			return;
		}
		this.stashedQuests = userQuestsStatus.TodayClaimableQuests;
		this.stashedBonusPoints = userQuestsStatus.TodayClaimableBonus;
		this.dailyLimitedTurnedIn = (userQuestsStatus.TodayClaimableIdol <= 0);
		this.lastQuestGrantTime = DateTime.UtcNow;
		this.RefreshActiveQuests();
		SIPlayer.SetAndBroadcastProgression();
		if (!this.questsInitialized)
		{
			this.questsInitialized = true;
			this.ClientReady = true;
			Action onClientReady = this.OnClientReady;
			if (onClientReady == null)
			{
				return;
			}
			onClientReady();
		}
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0002D9E0 File Offset: 0x0002BBE0
	public int GetCurrencyAmount(SIResource.ResourceType currencyType)
	{
		ProgressionManager.MothershipItemSummary mothershipItemSummary;
		if (!ProgressionManager.Instance.GetInventoryItem(SIProgression._resourceToString[currencyType], out mothershipItemSummary))
		{
			return 0;
		}
		return mothershipItemSummary.Quantity;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0002DA10 File Offset: 0x0002BC10
	public bool IsNodeUnlocked(SIUpgradeType upgradeType)
	{
		if (this.siNodes != null)
		{
			SIProgression.SINode sinode;
			return this.siNodes.TryGetValue(upgradeType, out sinode) && sinode.unlocked;
		}
		ProgressionManager instance = ProgressionManager.Instance;
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse = (instance != null) ? instance.GetTree("SI_Gadgets") : null;
		if (userHydratedProgressionTreeResponse != null)
		{
			foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
			{
				if (userHydratedNodeDefinition.name == upgradeType.ToString())
				{
					return userHydratedNodeDefinition.unlocked;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x0002DABC File Offset: 0x0002BCBC
	public void UnlockNode(SIUpgradeType upgradeType)
	{
		if (this._treeReady && this._inventoryReady)
		{
			ProgressionManager instance = ProgressionManager.Instance;
			UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse = (instance != null) ? instance.GetTree("SI_Gadgets") : null;
			SIProgression.SINode sinode;
			if (this.siNodes == null || !this.siNodes.TryGetValue(upgradeType, out sinode) || sinode.unlocked)
			{
				return;
			}
			ProgressionManager.Instance.UnlockNode(userHydratedProgressionTreeResponse.Tree.id, sinode.id);
		}
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0002DB2C File Offset: 0x0002BD2C
	private void HandleTreeUpdated()
	{
		this._treeReady = true;
		this.UpdateTree();
		this.UpdateUnlockOnPlayer();
		if (!this._startingPackageGranted)
		{
			if (this.IsNodeUnlocked(SIUpgradeType.Initialize))
			{
				this._startingPackageGranted = true;
			}
			else
			{
				this.startingPackageBackupAttempts++;
				if (this.startingPackageBackupAttempts > 10)
				{
					this._startingPackageGranted = true;
				}
				else
				{
					base.StartCoroutine(this.TryClaimNewPlayerPackage());
				}
			}
		}
		GTDev.Log<string>("[SIProgression] Updating local tech tree costs from remote tree", null);
		foreach (GraphNode<SITechTreeNode> graphNode in this.techTreeSO.AllNodes)
		{
			SITechTreeNode value = graphNode.Value;
			SIProgression.SINode sinode;
			if (this.siNodes.TryGetValue(value.upgradeType, out sinode))
			{
				SIResource.ResourceCost[] array = SIResource.GenerateCostsFrom(sinode.costs);
				if (array.IsValid_AllowZero() && !SIResource.CostsAreEqual(value.nodeCost, array, true))
				{
					GTDev.Log<string>(string.Format("[SIProgression] Changing {0} costs from {1} to {2}", value.upgradeType, SIResource.PrintCost(value.nodeCost), SIResource.PrintCost(array)), null);
					value.nodeCost = array;
				}
			}
		}
		Action onTreeReady = this.OnTreeReady;
		if (onTreeReady == null)
		{
			return;
		}
		onTreeReady();
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0002DC68 File Offset: 0x0002BE68
	private void HandleInventoryUpdated()
	{
		this._inventoryReady = true;
		this.UpdateCurrencyOnPlayer();
		Action onInventoryReady = this.OnInventoryReady;
		if (onInventoryReady == null)
		{
			return;
		}
		onInventoryReady();
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0002DC87 File Offset: 0x0002BE87
	private IEnumerator TryClaimNewPlayerPackage()
	{
		yield return new WaitForSecondsRealtime(Mathf.Pow((float)this.startingPackageBackupAttempts, 2f));
		if (!this._startingPackageGranted)
		{
			this.TryUnlock(SIUpgradeType.Initialize);
		}
		yield break;
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0002DC98 File Offset: 0x0002BE98
	private void HandleNodeUnlocked(string treeId, string nodeId)
	{
		this.UpdateTree();
		this.UpdateUnlockOnPlayer();
		SIProgression.SINode nodeFromID = this.GetNodeFromID(nodeId);
		if (!string.IsNullOrEmpty(nodeFromID.id))
		{
			Action<SIUpgradeType> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked == null)
			{
				return;
			}
			onNodeUnlocked(nodeFromID.upgradeType);
		}
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0002DCDC File Offset: 0x0002BEDC
	private void UpdateTree()
	{
		ProgressionManager instance = ProgressionManager.Instance;
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse = (instance != null) ? instance.GetTree("SI_Gadgets") : null;
		this.siNodes = new Dictionary<SIUpgradeType, SIProgression.SINode>();
		foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
		{
			SIUpgradeType siupgradeType;
			if (!Enum.TryParse<SIUpgradeType>(userHydratedNodeDefinition.name, out siupgradeType))
			{
				siupgradeType = SIUpgradeType.InvalidNode;
			}
			Dictionary<SIResource.ResourceType, int> dictionary = new Dictionary<SIResource.ResourceType, int>();
			HydratedProgressionNodeCost cost = userHydratedNodeDefinition.cost;
			if (((cost != null) ? cost.items : null) != null)
			{
				foreach (KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair in userHydratedNodeDefinition.cost.items)
				{
					foreach (KeyValuePair<SIResource.ResourceType, string> keyValuePair2 in SIProgression._resourceToString)
					{
						if (keyValuePair2.Value == keyValuePair.Key)
						{
							dictionary[keyValuePair2.Key] = keyValuePair.Value.Delta;
							break;
						}
					}
				}
			}
			SIProgression.SINode value = new SIProgression.SINode
			{
				id = userHydratedNodeDefinition.id,
				unlocked = userHydratedNodeDefinition.unlocked,
				costs = dictionary,
				parents = new List<SIProgression.SINode>(),
				upgradeType = siupgradeType
			};
			this.siNodes[siupgradeType] = value;
		}
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0002DE98 File Offset: 0x0002C098
	public bool TryUnlock(SIUpgradeType upgrade)
	{
		if (upgrade == SIUpgradeType.Initialize)
		{
			if (!this._startingPackageGranted)
			{
				this.UnlockNode(upgrade);
				return true;
			}
			return false;
		}
		else
		{
			this.techTreeSO.EnsureInitialized();
			GraphNode<SITechTreeNode> graphNode;
			if (!this.techTreeSO.TryGetNode(upgrade, out graphNode))
			{
				return false;
			}
			SIPlayer localPlayer = SIPlayer.LocalPlayer;
			SITechTreeNode value = graphNode.Value;
			if (localPlayer.NodeResearched(upgrade))
			{
				return false;
			}
			if (!this._treeReady)
			{
				ProgressionManager.Instance.RefreshProgressionTree();
			}
			if (!this._inventoryReady)
			{
				ProgressionManager.Instance.RefreshUserInventory();
			}
			if (!localPlayer.NodeParentsUnlocked(upgrade))
			{
				return false;
			}
			foreach (SIResource.ResourceCost resourceCost in value.nodeCost)
			{
				if (resourceCost.amount > this.GetCurrencyAmount(resourceCost.type))
				{
					return false;
				}
			}
			this.UnlockNode(upgrade);
			return true;
		}
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0002DF64 File Offset: 0x0002C164
	private SIProgression.SINode GetNodeFromID(string id)
	{
		foreach (KeyValuePair<SIUpgradeType, SIProgression.SINode> keyValuePair in this.siNodes)
		{
			if (keyValuePair.Value.id == id)
			{
				return keyValuePair.Value;
			}
		}
		return default(SIProgression.SINode);
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002DFDC File Offset: 0x0002C1DC
	private void UpdateCurrencyOnPlayer()
	{
		foreach (SIResource.ResourceType resourceType in this.resourceDict.Keys.ToList<SIResource.ResourceType>())
		{
			int value = 0;
			try
			{
				value = this.GetCurrencyAmount(resourceType);
			}
			catch
			{
			}
			this.resourceDict[resourceType] = value;
		}
		SIPlayer.SetAndBroadcastProgression();
		if (!this.ClientReady && this.questSourceList != null)
		{
			this.ClientReady = true;
			Action onClientReady = this.OnClientReady;
			if (onClientReady == null)
			{
				return;
			}
			onClientReady();
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0002E088 File Offset: 0x0002C288
	private void UpdateUnlockOnPlayer()
	{
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		this.techTreeSO.EnsureInitialized();
		int num = 0;
		foreach (KeyValuePair<SIUpgradeType, SIProgression.SINode> keyValuePair in this.siNodes)
		{
			SIUpgradeType key = keyValuePair.Key;
			if (key >= SIUpgradeType.Thruster_Unlock)
			{
				this.unlockedTechTreeData[key.GetPageId()][key.GetNodeId()] = keyValuePair.Value.unlocked;
				if (keyValuePair.Value.unlocked)
				{
					num++;
				}
			}
		}
		SIPlayer.SetAndBroadcastProgression();
		if (!this.ClientReady && this.questSourceList != null)
		{
			this.ClientReady = true;
			Action onClientReady = this.OnClientReady;
			if (onClientReady == null)
			{
				return;
			}
			onClientReady();
		}
	}

	// Token: 0x170000AC RID: 172
	// (get) Token: 0x06000862 RID: 2146 RVA: 0x0002E154 File Offset: 0x0002C354
	public int[] ActiveQuestIds
	{
		get
		{
			return this.activeQuestIds;
		}
	}

	// Token: 0x170000AD RID: 173
	// (get) Token: 0x06000863 RID: 2147 RVA: 0x0002E15C File Offset: 0x0002C35C
	public int[] ActiveQuestProgresses
	{
		get
		{
			return this.activeQuestProgresses;
		}
	}

	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06000864 RID: 2148 RVA: 0x0002E164 File Offset: 0x0002C364
	public bool DailyLimitedTurnedIn
	{
		get
		{
			return this.dailyLimitedTurnedIn;
		}
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0002E16C File Offset: 0x0002C36C
	public static void InitializeQuests()
	{
		SIProgression.Instance._InitializeQuests();
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0002E178 File Offset: 0x0002C378
	private void ProcessAllQuests(Action<RotatingQuest> action)
	{
		foreach (RotatingQuest obj in this.questSourceList.quests)
		{
			action(obj);
		}
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0002E1D0 File Offset: 0x0002C3D0
	private void QuestLoadPostProcess(RotatingQuest quest)
	{
		quest.SetRequiredZone();
		if (quest.requiredZones.Count == 1 && quest.requiredZones[0] == GTZone.none)
		{
			quest.requiredZones.Clear();
		}
		quest.isQuestActive = true;
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0002E208 File Offset: 0x0002C408
	private void QuestSavePreProcess(RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 0)
		{
			quest.requiredZones.Add(GTZone.none);
		}
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x0002E224 File Offset: 0x0002C424
	private void _InitializeQuests()
	{
		ProgressionManager.Instance.GetActiveSIQuests(new Action<List<RotatingQuest>>(this.LoadQuestsFromServer), null);
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0002E240 File Offset: 0x0002C440
	public void LoadQuestsFromServer(List<RotatingQuest> serverQuests)
	{
		if (serverQuests == null || serverQuests.Count == 0)
		{
			Debug.LogError("[SIProgression] Server returned no quests");
			this.LoadQuestsFromLocalJson();
		}
		else
		{
			this.questSourceList = new SIProgression.SIQuestsList
			{
				quests = serverQuests
			};
			this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
		}
		this.LoadQuestProgress();
		if (!this.questsInitialized)
		{
			ProgressionManager.Instance.GetSIQuestStatus(new Action<ProgressionManager.UserQuestsStatusResponse>(this.ApplyServerQuestsStatus), null);
		}
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0002E2B4 File Offset: 0x0002C4B4
	private void LoadQuestsFromLocalJson()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("TestingSuperInfectionQuests");
		this.LoadQuestsFromJson(textAsset.text);
		this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x0002E2EA File Offset: 0x0002C4EA
	public void SliceUpdate()
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (activeSuperInfectionManager == null || !activeSuperInfectionManager.IsZoneReady())
		{
			return;
		}
		if (!this.questsInitialized)
		{
			return;
		}
		this.CheckTimeCrossover();
		this.SaveQuestProgress();
		this.CheckTelemetry();
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0002E31E File Offset: 0x0002C51E
	private void CheckTimeCrossover()
	{
		this.CheckTimeCrossoverServer();
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0002E328 File Offset: 0x0002C528
	private void CheckTimeCrossoverServer()
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = utcNow.Date + this.CROSSOVER_TIME_OF_DAY;
		if (dateTime > utcNow)
		{
			dateTime = dateTime.AddDays(-1.0);
		}
		if ((dateTime - this.lastQuestGrantTime).Ticks <= 0L)
		{
			return;
		}
		this.lastQuestGrantTime = utcNow.Date + this.CROSSOVER_TIME_OF_DAY;
		ProgressionManager.Instance.GetSIQuestStatus(new Action<ProgressionManager.UserQuestsStatusResponse>(this.ApplyServerQuestsStatus), null);
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0002E3B0 File Offset: 0x0002C5B0
	public static void StaticSaveQuestProgress()
	{
		SIProgression.Instance.SaveQuestProgress();
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x0002E3BC File Offset: 0x0002C5BC
	public void LoadQuestProgress()
	{
		this.LoadQuestProgressServer();
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0002E3C4 File Offset: 0x0002C5C4
	public void SaveQuestProgress()
	{
		this.SaveQuestProgressServer();
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0002E3CC File Offset: 0x0002C5CC
	public void LoadQuestProgressServer()
	{
		int num = 0;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			int @int = PlayerPrefs.GetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_ID_Key", i), -1);
			int int2 = PlayerPrefs.GetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_Progress_Key", i), -1);
			this.activeQuestIds[i] = @int;
			this.activeQuestProgresses[i] = int2;
			if (@int != -1)
			{
				RotatingQuest questById = this.questSourceList.GetQuestById(@int);
				if (questById == null || !questById.isQuestActive)
				{
					this.activeQuestIds[i] = -1;
					this.activeQuestProgresses[i] = -1;
				}
				else
				{
					num++;
					questById.ApplySavedProgress(int2);
				}
			}
		}
		this.bonusProgress = PlayerPrefs.GetInt("v1_SIProgression:bonusProgress", 0);
		this.CopySaveDataToDiff();
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x0002E494 File Offset: 0x0002C694
	public void SaveQuestProgressServer()
	{
		int num = 0;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			if (num >= this.stashedQuests)
			{
				this.activeQuestIds[i] = -1;
				this.activeQuestProgresses[i] = 0;
			}
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById == null || !questById.isQuestActive)
			{
				this.activeQuestIds[i] = -1;
				this.activeQuestProgresses[i] = 0;
			}
			else
			{
				num++;
			}
			int num2 = -1;
			int num3 = 0;
			if (questById != null)
			{
				num2 = questById.questID;
				num3 = questById.GetProgress();
			}
			this.activeQuestProgresses[i] = num3;
			if (num2 != this.activeQuestIdsDiff[i])
			{
				PlayerPrefs.SetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_ID_Key", i), num2);
			}
			if (num3 != this.activeQuestProgressesDiff[i])
			{
				PlayerPrefs.SetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_Progress_Key", i), num3);
			}
		}
		if (this.bonusProgress != this.bonusProgressDiff)
		{
			PlayerPrefs.SetInt("v1_SIProgression:bonusProgress", this.bonusProgress);
		}
		PlayerPrefs.Save();
		this.CopySaveDataToDiff();
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x0002E5A8 File Offset: 0x0002C7A8
	public void CopySaveDataToDiff()
	{
		this.lastQuestGrantTimeDiff = this.lastQuestGrantTime;
		this.stashedQuestsDiff = this.stashedQuests;
		this.stashedBonusPointsDiff = this.stashedBonusPoints;
		this.bonusProgressDiff = this.bonusProgress;
		int[] array = new int[6];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.resourceDict[(SIResource.ResourceType)i];
		}
		SIProgression._SafeShallowCopyArray<int>(array, ref this.resourceArrayDiff);
		SIProgression._SafeShallowCopyArray<int>(this.limitedDepositTimeArray, ref this.limitedDepositTimeDiff);
		SIProgression._SafeShallowCopyArray<int>(this.activeQuestIds, ref this.activeQuestIdsDiff);
		SIProgression._SafeShallowCopyArray<int>(this.activeQuestProgresses, ref this.activeQuestProgressesDiff);
		if (this.unlockedTechTreeDataDiff == null || this.unlockedTechTreeDataDiff.Length != this.unlockedTechTreeData.Length)
		{
			this.unlockedTechTreeDataDiff = new bool[this.unlockedTechTreeData.Length][];
		}
		for (int j = 0; j < this.unlockedTechTreeData.Length; j++)
		{
			SIProgression._SafeShallowCopyArray<bool>(this.unlockedTechTreeData[j], ref this.unlockedTechTreeDataDiff[j]);
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002E6A1 File Offset: 0x0002C8A1
	private static void _SafeShallowCopyArray<T>(T[] sourceArray, ref T[] ref_destinationArray)
	{
		if (ref_destinationArray == null || ref_destinationArray.Length != sourceArray.Length)
		{
			ref_destinationArray = new T[sourceArray.Length];
		}
		Array.Copy(sourceArray, ref_destinationArray, sourceArray.Length);
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x0002E6C8 File Offset: 0x0002C8C8
	public int[] GetResourceArray()
	{
		int[] array = new int[this.resourceDict.Count];
		for (int i = 0; i < this.resourceDict.Count; i++)
		{
			array[i] = this.resourceDict[(SIResource.ResourceType)i];
		}
		return array;
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x0002E70C File Offset: 0x0002C90C
	public void SetResourceArray(int[] resourceArray)
	{
		for (int i = 0; i < resourceArray.Length; i++)
		{
			this.resourceDict[(SIResource.ResourceType)i] = resourceArray[i];
		}
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x0002E736 File Offset: 0x0002C936
	public void HandleQuestCompleted(int questID)
	{
		this.UpdateQuestProgresses();
		SIPlayer.SetAndBroadcastProgression();
		SIPlayer.LocalPlayer.questCompleteCelebrate.SetActive(true);
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0002E754 File Offset: 0x0002C954
	public void HandleQuestProgressChanged(bool initialLoad)
	{
		if (this.UpdateQuestProgresses())
		{
			SIPlayer.SetAndBroadcastProgression();
		}
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0002E764 File Offset: 0x0002C964
	private bool UpdateQuestProgresses()
	{
		bool result = false;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			int num = 0;
			if (questById != null)
			{
				num = questById.GetProgress();
				if (questById.questType != QuestType.moveDistance || this.activeQuestProgresses[i] / 100 != num / 100)
				{
					result = true;
				}
			}
			this.activeQuestProgresses[i] = num;
		}
		this.SaveQuestProgress();
		return result;
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0002E7D4 File Offset: 0x0002C9D4
	public void AttemptIncrementResource(SIResource.ResourceType resource)
	{
		ProgressionManager.Instance.IncrementSIResource(resource.ToString(), new Action<string>(this.OnSuccessfulIncrementResource), delegate(string err)
		{
			Debug.LogError(err);
		});
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0002E823 File Offset: 0x0002CA23
	private void OnSuccessfulIncrementResource(string resourceStr)
	{
		if (Enum.Parse<SIResource.ResourceType>(resourceStr) == SIResource.ResourceType.TechPoint)
		{
			SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		}
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0002E844 File Offset: 0x0002CA44
	public void AttemptRedeemCompletedQuest(int questIndex)
	{
		RotatingQuest quest = this.questSourceList.GetQuestById(this.activeQuestIds[questIndex]);
		if (quest == null || this.activeQuestIds[questIndex] == -1)
		{
			return;
		}
		if (!quest.isQuestComplete)
		{
			return;
		}
		if (this.redeemingQuestInProgress[questIndex])
		{
			return;
		}
		this.redeemingQuestInProgress[questIndex] = true;
		ProgressionManager.Instance.CompleteSIQuest(quest.questID, delegate(ProgressionManager.UserQuestsStatusResponse status)
		{
			this.OnSuccessfulQuestRedeem(questIndex, quest, status);
		}, delegate(string err)
		{
			if (err.Contains("409") || err.Contains("404"))
			{
				this.OnInvalidQuestRedeemAttempt(questIndex, quest);
			}
			this.redeemingQuestInProgress[questIndex] = false;
			Debug.LogError(err);
		});
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0002E8F8 File Offset: 0x0002CAF8
	private void OnSuccessfulQuestRedeem(int questIndex, RotatingQuest quest, ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		this.activeQuestIds[questIndex] = -1;
		this.activeQuestProgresses[questIndex] = 0;
		quest.ApplySavedProgress(0);
		this.redeemingQuestInProgress[questIndex] = false;
		Dictionary<SIResource.ResourceType, int> dictionary = this.resourceDict;
		int num = dictionary[SIResource.ResourceType.TechPoint];
		dictionary[SIResource.ResourceType.TechPoint] = num + 1;
		this.ApplyServerQuestsStatus(userQuestsStatus);
		SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0002E959 File Offset: 0x0002CB59
	private void OnInvalidQuestRedeemAttempt(int questIndex, RotatingQuest quest)
	{
		this.activeQuestIds[questIndex] = -1;
		this.activeQuestProgresses[questIndex] = 0;
		quest.ApplySavedProgress(0);
		ProgressionManager.Instance.GetSIQuestStatus(new Action<ProgressionManager.UserQuestsStatusResponse>(this.ApplyServerQuestsStatus), null);
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0002E98B File Offset: 0x0002CB8B
	public void AttemptRedeemBonusPoint()
	{
		ProgressionManager.Instance.CompleteSIBonus(delegate(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
		{
			this.OnSuccessfulBonusRedeem(userQuestsStatus);
		}, delegate(string err)
		{
			Debug.LogError(err);
		});
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0002E9C2 File Offset: 0x0002CBC2
	private void OnSuccessfulBonusRedeem(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		this.bonusProgress = 0;
		this.ApplyServerQuestsStatus(userQuestsStatus);
		SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0002E9E6 File Offset: 0x0002CBE6
	public void AttemptCollectMonkeIdol()
	{
		ProgressionManager.Instance.CollectSIIdol(new Action<ProgressionManager.UserQuestsStatusResponse>(this.OnSuccessfulMonkeIdolRedeem), delegate(string err)
		{
			Debug.LogError(err);
		});
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0002EA1D File Offset: 0x0002CC1D
	private void OnSuccessfulMonkeIdolRedeem(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		this.ApplyServerQuestsStatus(userQuestsStatus);
		this.limitedDepositTimeArray[1] = 1;
		SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002EA43 File Offset: 0x0002CC43
	public void GetBonusProgress()
	{
		this.bonusProgress++;
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0002EA54 File Offset: 0x0002CC54
	public void SetupAllQuestEventListeners()
	{
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById != null && this.activeQuestIds[i] != -1)
			{
				questById.questManager = this;
				if (!questById.isQuestComplete)
				{
					questById.AddEventListener();
				}
			}
		}
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0002EAAB File Offset: 0x0002CCAB
	public static void StaticClearAllQuestEventListeners()
	{
		SIProgression.Instance.ClearAllQuestEventListeners();
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0002EAB8 File Offset: 0x0002CCB8
	public void ClearAllQuestEventListeners()
	{
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById != null)
			{
				questById.RemoveEventListener();
			}
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0002EAF6 File Offset: 0x0002CCF6
	public void LoadQuestsFromJson(string jsonString)
	{
		this.questSourceList = JsonConvert.DeserializeObject<SIProgression.SIQuestsList>(jsonString);
		this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x0002EB16 File Offset: 0x0002CD16
	public void RefreshActiveQuests()
	{
		this.ClearAllQuestEventListeners();
		this.SelectActiveQuests();
		this.HandleQuestProgressChanged(true);
		this.SetupAllQuestEventListeners();
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0002EB34 File Offset: 0x0002CD34
	private void SelectActiveQuests()
	{
		int num = 0;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById != null && questById.isQuestActive && num < this.stashedQuests)
			{
				this.activeQuestCategories[i] = questById.category;
				num++;
			}
			else
			{
				this.activeQuestIds[i] = -1;
				this.activeQuestProgresses[i] = 0;
				this.activeQuestCategories[i] = QuestCategory.NONE;
				if (questById != null)
				{
					questById.ApplySavedProgress(0);
				}
			}
		}
		int num2 = Mathf.Max(0, this.stashedQuests);
		int num3 = 0;
		while (num3 < this.activeQuestIds.Length && num < num2)
		{
			RotatingQuest questById2 = this.questSourceList.GetQuestById(this.activeQuestIds[num3]);
			if (questById2 == null || !questById2.isQuestActive)
			{
				int num4 = Random.Range(0, this.questSourceList.quests.Count);
				for (int j = 0; j < this.questSourceList.quests.Count; j++)
				{
					int num5 = (num4 + j) % this.questSourceList.quests.Count;
					RotatingQuest questById3 = this.questSourceList.GetQuestById(num5);
					if (questById3 != null && questById3.isQuestActive && this.<SelectActiveQuests>g__GetMatchingCategoryCount|171_0(questById3) < this.perCategoryQuestLimit)
					{
						bool flag = false;
						for (int k = 0; k < this.activeQuestIds.Length; k++)
						{
							if (num5 == this.activeQuestIds[k])
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.activeQuestIds[num3] = num5;
							this.activeQuestCategories[num3] = questById3.category;
							questById3.ApplySavedProgress(0);
							this.activeQuestProgresses[num3] = 0;
							num++;
							break;
						}
					}
				}
			}
			num3++;
		}
		this.SaveQuestProgress();
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x0002ECF8 File Offset: 0x0002CEF8
	private void SelectCurrentTurnInDate()
	{
		DateTime dateTime = new DateTime(2025, 1, 10, 18, 0, 0, DateTimeKind.Utc);
		TimeSpan timeSpan = TimeSpan.FromHours(-8.0);
		DateTime dateStart = new DateTime(1, 1, 1, 0, 0, 0);
		DateTime dateEnd = new DateTime(2006, 12, 31, 0, 0, 0);
		TimeSpan daylightDelta = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime daylightTransitionStart = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 4, 1, DayOfWeek.Sunday);
		TimeZoneInfo.TransitionTime daylightTransitionEnd = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, DayOfWeek.Sunday);
		DateTime dateStart2 = new DateTime(2007, 1, 1, 0, 0, 0);
		DateTime dateEnd2 = new DateTime(9999, 12, 31, 0, 0, 0);
		TimeSpan daylightDelta2 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime daylightTransitionStart2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, DayOfWeek.Sunday);
		TimeZoneInfo.TransitionTime daylightTransitionEnd2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, DayOfWeek.Sunday);
		TimeZoneInfo timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Pacific Standard Time", timeSpan, "Pacific Standard Time", "Pacific Standard Time", "Pacific Standard Time", new TimeZoneInfo.AdjustmentRule[]
		{
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd),
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateStart2, dateEnd2, daylightDelta2, daylightTransitionStart2, daylightTransitionEnd2)
		});
		if (timeZoneInfo != null && timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow - timeSpan))
		{
			dateTime -= TimeSpan.FromHours(1.0);
		}
		int days = (DateTime.UtcNow - dateTime).Days;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x0002EE70 File Offset: 0x0002D070
	public bool TryDepositResources(SIResource.ResourceType type, int count)
	{
		int resourceMaxCap = this.GetResourceMaxCap(type);
		int num = this.resourceDict[type];
		if (resourceMaxCap == num)
		{
			return false;
		}
		count = Math.Min(count, resourceMaxCap - num);
		Dictionary<SIResource.ResourceType, int> dictionary = this.resourceDict;
		dictionary[type] += count;
		this.AttemptIncrementResource(type);
		return true;
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x0002EEC4 File Offset: 0x0002D0C4
	public int GetResourceMaxCap(SIResource.ResourceType type)
	{
		return this.resourceCapsArray[(int)type];
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0002EECE File Offset: 0x0002D0CE
	public bool IsLimitedDepositAvailable(SIResource.LimitedDepositType limitedDepositType)
	{
		return !this.dailyLimitedTurnedIn;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x0002EED9 File Offset: 0x0002D0D9
	public void ApplyLimitedDepositTime(SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			return;
		}
		this.AttemptCollectMonkeIdol();
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x0002EEE5 File Offset: 0x0002D0E5
	private void OnDestroy()
	{
		this.SaveQuestProgress();
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x0002EEED File Offset: 0x0002D0ED
	public bool GetOnlineNode(SIUpgradeType type, out SIProgression.SINode node)
	{
		if (!this._treeReady)
		{
			node = this.emptyNode;
			return false;
		}
		return this.siNodes.TryGetValue(type, out node);
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x0002EF12 File Offset: 0x0002D112
	public static bool ResourcesMaxed()
	{
		return SIProgression.Instance._ResourcesMaxed();
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002EF20 File Offset: 0x0002D120
	public bool _ResourcesMaxed()
	{
		foreach (KeyValuePair<SIResource.ResourceType, int> keyValuePair in this.resourceDict)
		{
			if (keyValuePair.Key != SIResource.ResourceType.TechPoint && keyValuePair.Value < this.GetResourceMaxCap(keyValuePair.Key))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002EF94 File Offset: 0x0002D194
	public void CheckTelemetry()
	{
		GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
		if (!(activeGameMode == null))
		{
			GameModeType gameModeType = activeGameMode.GameType();
			if (gameModeType == GameModeType.SuperInfect || gameModeType == GameModeType.SuperCasual)
			{
				if (!activeGameMode.ValidGameMode())
				{
					this.timeTelemetryLastChecked = Time.realtimeSinceStartup;
					return;
				}
				float num = Time.realtimeSinceStartup - this.timeTelemetryLastChecked;
				this.timeTelemetryLastChecked = Time.realtimeSinceStartup;
				this.totalPlayTime += num;
				if (NetworkSystem.Instance.InRoom)
				{
					this.roomPlayTime += num;
				}
				this.intervalPlayTime += num;
				for (int i = 0; i < 11; i++)
				{
					SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
					if (SIProgression.Instance.heldOrSnappedByGadgetPageType[sitechTreePageId] > 0)
					{
						Dictionary<SITechTreePageId, float> dictionary = this.timeUsingGadgetTypeInterval;
						SITechTreePageId key = sitechTreePageId;
						dictionary[key] += num;
						dictionary = this.timeUsingGadgetTypeTotal;
						key = sitechTreePageId;
						dictionary[key] += num;
					}
				}
				if (SIProgression.Instance.heldOrSnappedOwnGadgets > 0)
				{
					this.timeUsingOwnGadgetsInterval += num;
					this.timeUsingOwnGadgetsTotal += num;
				}
				if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
				{
					this.timeUsingOthersGadgetsInterval += num;
					this.timeUsingOthersGadgetsTotal += num;
				}
				if (this.lastTelemetrySent + this.telemetryCooldown < Time.realtimeSinceStartup)
				{
					this.lastTelemetrySent = Time.realtimeSinceStartup;
					this.SaveTelemetryData();
					GorillaTelemetry.SuperInfectionEvent(false, this.totalPlayTime, this.roomPlayTime, Time.realtimeSinceStartup, this.intervalPlayTime, this.activeTerminalTimeTotal, this.activeTerminalTimeInterval, this.timeUsingGadgetTypeTotal, this.timeUsingGadgetTypeInterval, this.timeUsingOwnGadgetsTotal, this.timeUsingOwnGadgetsInterval, this.timeUsingOthersGadgetsTotal, this.timeUsingOthersGadgetsInterval, this.tagsUsingGadgetTypeTotal, this.tagsUsingGadgetTypeInterval, this.tagsHoldingOwnGadgetTotal, this.tagsHoldingOwnGadgetInterval, this.tagsHoldingOthersGadgetTotal, this.tagsHoldingOthersGadgetInterval, this.resourcesCollectedTotal, this.resourcesCollectedInterval, this.roundsPlayedTotal, this.roundsPlayedInterval, SIProgression.Instance.unlockedTechTreeData, NetworkSystem.Instance.RoomPlayerCount);
					this.ResetTelemetryIntervalData();
				}
				return;
			}
		}
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0002F1A8 File Offset: 0x0002D3A8
	public void SendTelemetryData()
	{
		if (Time.realtimeSinceStartup < this.lastDisconnectTelemetrySent + this.minDisconnectTelemetryCooldown)
		{
			return;
		}
		this.lastDisconnectTelemetrySent = Time.realtimeSinceStartup;
		this.SaveTelemetryData();
		GorillaTelemetry.SuperInfectionEvent(true, this.totalPlayTime, this.roomPlayTime, Time.realtimeSinceStartup, this.intervalPlayTime, this.activeTerminalTimeTotal, this.activeTerminalTimeInterval, this.timeUsingGadgetTypeTotal, this.timeUsingGadgetTypeInterval, this.timeUsingOwnGadgetsTotal, this.timeUsingOwnGadgetsInterval, this.timeUsingOthersGadgetsTotal, this.timeUsingOthersGadgetsInterval, this.tagsUsingGadgetTypeTotal, this.tagsUsingGadgetTypeInterval, this.tagsHoldingOwnGadgetTotal, this.tagsHoldingOwnGadgetInterval, this.tagsHoldingOthersGadgetTotal, this.tagsHoldingOthersGadgetInterval, this.resourcesCollectedTotal, this.resourcesCollectedInterval, this.roundsPlayedTotal, this.roundsPlayedInterval, SIProgression.Instance.unlockedTechTreeData, NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.RoomPlayerCount : -1);
		this.ResetTelemetryIntervalData();
		this.roomPlayTime = 0f;
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0002F298 File Offset: 0x0002D498
	public void SendPurchaseResourcesData()
	{
		this.SaveTelemetryData();
		GorillaTelemetry.SuperInfectionEvent("si_fill_resources", 500, -1, this.totalPlayTime, this.roomPlayTime, Time.realtimeSinceStartup);
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x0002F2C1 File Offset: 0x0002D4C1
	public void SendPurchaseTechPointsData(int techPointsPurchased)
	{
		this.SaveTelemetryData();
		GorillaTelemetry.SuperInfectionEvent("si_purchase_tech_points", techPointsPurchased * 100, techPointsPurchased, this.totalPlayTime, this.roomPlayTime, Time.realtimeSinceStartup);
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x0002F2EC File Offset: 0x0002D4EC
	public void LoadSavedTelemetryData()
	{
		this.totalPlayTime = PlayerPrefs.GetFloat("super_infection_total_play_time", 0f);
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			this.timeUsingGadgetTypeTotal[sitechTreePageId] = PlayerPrefs.GetFloat("super_infection_time_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), 0f);
			this.tagsUsingGadgetTypeTotal[sitechTreePageId] = PlayerPrefs.GetInt("super_infection_tags_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), 0);
		}
		this.activeTerminalTimeTotal = PlayerPrefs.GetFloat("super_infection_terminal_total_time", 0f);
		this.tagsHoldingOthersGadgetTotal = PlayerPrefs.GetInt("super_infection_tags_holding_others_gadgets_total", 0);
		this.tagsHoldingOwnGadgetTotal = PlayerPrefs.GetInt("super_infection_tags_holding_own_gadgets_total", 0);
		for (int j = 0; j < 6; j++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)j;
			this.resourcesCollectedTotal[resourceType] = PlayerPrefs.GetInt("super_infection_resource_type_collected_total" + resourceType.GetName<SIResource.ResourceType>(), 0);
		}
		this.roundsPlayedTotal = PlayerPrefs.GetInt("super_infection_rounds_played_total", 0);
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0002F3E0 File Offset: 0x0002D5E0
	private void SaveTelemetryData()
	{
		PlayerPrefs.SetFloat("super_infection_total_play_time", this.totalPlayTime);
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			PlayerPrefs.SetFloat("super_infection_time_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), this.timeUsingGadgetTypeTotal[sitechTreePageId]);
			PlayerPrefs.SetInt("super_infection_tags_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), this.tagsUsingGadgetTypeTotal[sitechTreePageId]);
		}
		PlayerPrefs.SetFloat("super_infection_terminal_total_time", this.activeTerminalTimeTotal);
		PlayerPrefs.SetInt("super_infection_tags_holding_others_gadgets_total", this.tagsHoldingOthersGadgetTotal);
		PlayerPrefs.SetInt("super_infection_tags_holding_own_gadgets_total", this.tagsHoldingOwnGadgetTotal);
		for (int j = 0; j < 6; j++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)j;
			PlayerPrefs.SetInt("super_infection_resource_type_collected_total" + resourceType.GetName<SIResource.ResourceType>(), this.resourcesCollectedTotal[resourceType]);
		}
		PlayerPrefs.SetInt("super_infection_rounds_played_total", this.roundsPlayedTotal);
		PlayerPrefs.Save();
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0002F4C4 File Offset: 0x0002D6C4
	public void ResetTelemetryIntervalData()
	{
		this.lastTelemetrySent = Time.realtimeSinceStartup;
		this.intervalPlayTime = 0f;
		this.activeTerminalTimeInterval = 0f;
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId key = (SITechTreePageId)i;
			this.timeUsingGadgetTypeInterval[key] = 0f;
			this.tagsUsingGadgetTypeInterval[key] = 0;
		}
		this.timeUsingOwnGadgetsInterval = 0f;
		this.timeUsingOthersGadgetsInterval = 0f;
		this.tagsHoldingOthersGadgetInterval = 0;
		this.tagsHoldingOwnGadgetInterval = 0;
		for (int j = 0; j < 6; j++)
		{
			SIResource.ResourceType key2 = (SIResource.ResourceType)j;
			this.resourcesCollectedInterval[key2] = 0;
		}
		this.roundsPlayedInterval = 0;
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0002F568 File Offset: 0x0002D768
	public void HandleTagTelemetry(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (taggingPlayer.ActorNumber != SIPlayer.LocalPlayer.ActorNr)
		{
			return;
		}
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			if (SIProgression.Instance.heldOrSnappedByGadgetPageType[sitechTreePageId] > 0)
			{
				Dictionary<SITechTreePageId, int> dictionary = this.tagsUsingGadgetTypeTotal;
				SITechTreePageId key = sitechTreePageId;
				int num = dictionary[key];
				dictionary[key] = num + 1;
				Dictionary<SITechTreePageId, int> dictionary2 = this.tagsUsingGadgetTypeInterval;
				key = sitechTreePageId;
				num = dictionary2[key];
				dictionary2[key] = num + 1;
			}
		}
		if (SIProgression.Instance.heldOrSnappedOwnGadgets > 0)
		{
			this.tagsHoldingOwnGadgetInterval++;
			this.tagsHoldingOwnGadgetTotal++;
		}
		if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
		{
			this.tagsHoldingOthersGadgetInterval++;
			this.tagsHoldingOthersGadgetTotal++;
		}
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x0002F630 File Offset: 0x0002D830
	public void UpdateHeldGadgetsTelemetry(SITechTreePageId id, bool isMine, int changeAmount)
	{
		Dictionary<SITechTreePageId, int> dictionary = SIProgression.Instance.heldOrSnappedByGadgetPageType;
		dictionary[id] += changeAmount;
		if (isMine)
		{
			SIProgression.Instance.heldOrSnappedOwnGadgets += changeAmount;
			return;
		}
		SIProgression.Instance.heldOrSnappedOthersGadgets += changeAmount;
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x0002F684 File Offset: 0x0002D884
	public void CollectResourceTelemetry(SIResource.ResourceType type, int count)
	{
		Dictionary<SIResource.ResourceType, int> dictionary = this.resourcesCollectedTotal;
		dictionary[type] += count;
		dictionary = this.resourcesCollectedInterval;
		dictionary[type] += count;
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0002F6C3 File Offset: 0x0002D8C3
	public void AddRoundTelemetry()
	{
		this.roundsPlayedInterval++;
		this.roundsPlayedTotal++;
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0002F7B4 File Offset: 0x0002D9B4
	[CompilerGenerated]
	private int <SelectActiveQuests>g__GetMatchingCategoryCount|171_0(RotatingQuest quest)
	{
		if (quest.category == QuestCategory.NONE)
		{
			return 0;
		}
		int num = 0;
		QuestCategory[] array = this.activeQuestCategories;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == quest.category)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x04000A51 RID: 2641
	[SerializeField]
	private SITechTreeSO techTreeSO;

	// Token: 0x04000A52 RID: 2642
	[SerializeField]
	private int perCategoryQuestLimit = 1;

	// Token: 0x04000A53 RID: 2643
	public Action OnTreeReady;

	// Token: 0x04000A54 RID: 2644
	public Action OnInventoryReady;

	// Token: 0x04000A55 RID: 2645
	public Action<SIUpgradeType> OnNodeUnlocked;

	// Token: 0x04000A57 RID: 2647
	public bool ClientReady;

	// Token: 0x04000A58 RID: 2648
	private static Dictionary<SIResource.ResourceType, string> _resourceToString;

	// Token: 0x04000A59 RID: 2649
	private const string TREE_NAME = "SI_Gadgets";

	// Token: 0x04000A5A RID: 2650
	private Dictionary<SIUpgradeType, SIProgression.SINode> siNodes;

	// Token: 0x04000A5B RID: 2651
	internal bool _treeReady;

	// Token: 0x04000A5C RID: 2652
	private bool _inventoryReady;

	// Token: 0x04000A5D RID: 2653
	public Dictionary<SITechTreePageId, int> heldOrSnappedByGadgetPageType = new Dictionary<SITechTreePageId, int>();

	// Token: 0x04000A5E RID: 2654
	public int heldOrSnappedOwnGadgets;

	// Token: 0x04000A5F RID: 2655
	public int heldOrSnappedOthersGadgets;

	// Token: 0x04000A60 RID: 2656
	public float timeTelemetryLastChecked;

	// Token: 0x04000A61 RID: 2657
	public float lastTelemetrySent;

	// Token: 0x04000A62 RID: 2658
	private float telemetryCooldown = 600f;

	// Token: 0x04000A63 RID: 2659
	private float totalPlayTime;

	// Token: 0x04000A64 RID: 2660
	private float roomPlayTime;

	// Token: 0x04000A65 RID: 2661
	private float intervalPlayTime;

	// Token: 0x04000A66 RID: 2662
	[NonSerialized]
	public float activeTerminalTimeTotal;

	// Token: 0x04000A67 RID: 2663
	[NonSerialized]
	public float activeTerminalTimeInterval;

	// Token: 0x04000A68 RID: 2664
	private Dictionary<SITechTreePageId, float> timeUsingGadgetTypeTotal = new Dictionary<SITechTreePageId, float>();

	// Token: 0x04000A69 RID: 2665
	private Dictionary<SITechTreePageId, float> timeUsingGadgetTypeInterval = new Dictionary<SITechTreePageId, float>();

	// Token: 0x04000A6A RID: 2666
	private float timeUsingOthersGadgetsTotal;

	// Token: 0x04000A6B RID: 2667
	private float timeUsingOthersGadgetsInterval;

	// Token: 0x04000A6C RID: 2668
	private float timeUsingOwnGadgetsTotal;

	// Token: 0x04000A6D RID: 2669
	private float timeUsingOwnGadgetsInterval;

	// Token: 0x04000A6E RID: 2670
	private Dictionary<SITechTreePageId, int> tagsUsingGadgetTypeTotal = new Dictionary<SITechTreePageId, int>();

	// Token: 0x04000A6F RID: 2671
	private Dictionary<SITechTreePageId, int> tagsUsingGadgetTypeInterval = new Dictionary<SITechTreePageId, int>();

	// Token: 0x04000A70 RID: 2672
	private int tagsHoldingOthersGadgetTotal;

	// Token: 0x04000A71 RID: 2673
	private int tagsHoldingOthersGadgetInterval;

	// Token: 0x04000A72 RID: 2674
	private int tagsHoldingOwnGadgetTotal;

	// Token: 0x04000A73 RID: 2675
	private int tagsHoldingOwnGadgetInterval;

	// Token: 0x04000A74 RID: 2676
	private Dictionary<SIResource.ResourceType, int> resourcesCollectedTotal = new Dictionary<SIResource.ResourceType, int>();

	// Token: 0x04000A75 RID: 2677
	private Dictionary<SIResource.ResourceType, int> resourcesCollectedInterval = new Dictionary<SIResource.ResourceType, int>();

	// Token: 0x04000A76 RID: 2678
	private int roundsPlayedTotal;

	// Token: 0x04000A77 RID: 2679
	private int roundsPlayedInterval;

	// Token: 0x04000A78 RID: 2680
	private SIProgression.SINode emptyNode;

	// Token: 0x04000A79 RID: 2681
	public SIProgression.SIQuestsList questSourceList;

	// Token: 0x04000A7A RID: 2682
	private const int STARTING_STASHED_QUESTS = 0;

	// Token: 0x04000A7B RID: 2683
	private const int STARTING_STASHED_BONUS_POINTS = 0;

	// Token: 0x04000A7C RID: 2684
	public const int SHARED_QUEST_TURNINS_FOR_POINT = 4;

	// Token: 0x04000A7D RID: 2685
	public const int NEW_QUESTS_PER_DAY = 3;

	// Token: 0x04000A7E RID: 2686
	public const int NEW_BONUS_POINTS_PER_DAY = 1;

	// Token: 0x04000A7F RID: 2687
	public const int MAX_STASHED_QUESTS = 6;

	// Token: 0x04000A80 RID: 2688
	public const int MAX_STASHED_BONUS_POINTS = 2;

	// Token: 0x04000A81 RID: 2689
	public const int MAX_RESOURCE_COUNT = 30;

	// Token: 0x04000A82 RID: 2690
	private const int ACTIVE_QUEST_COUNT = 3;

	// Token: 0x04000A83 RID: 2691
	private const string kLocalQuestPath = "TestingSuperInfectionQuests";

	// Token: 0x04000A84 RID: 2692
	private const string kVersion = "v1_";

	// Token: 0x04000A85 RID: 2693
	private const string kLastQuestGrantTime = "v1_SIProgression:lastSharedGrantTime";

	// Token: 0x04000A86 RID: 2694
	private const string kBonusProgress = "v1_SIProgression:bonusProgress";

	// Token: 0x04000A87 RID: 2695
	private const string kDailyQuestId = "v1_Rotating_Quest_Daily_ID_Key";

	// Token: 0x04000A88 RID: 2696
	private const string kDailyQuestProgress = "v1_Rotating_Quest_Daily_Progress_Key";

	// Token: 0x04000A89 RID: 2697
	private const string kStashedQuests = "v1_SIProgression:stashedQuests";

	// Token: 0x04000A8A RID: 2698
	private const string kStashedBonusPoints = "v1_SIProgression:stashedBonusPoints";

	// Token: 0x04000A8B RID: 2699
	private const string kTechTree = "v1_SITechTree:";

	// Token: 0x04000A8C RID: 2700
	private const string kLimitedDeposit = "v1_SIResource:LimitedDeposit:";

	// Token: 0x04000A8D RID: 2701
	private const string kTechPoints = "v1_SIResource:techPoints";

	// Token: 0x04000A8E RID: 2702
	private const string kStrangeWood = "v1_SIResource:strangeWood";

	// Token: 0x04000A8F RID: 2703
	private const string kWeirdGear = "v1_SIResource:weirdGear";

	// Token: 0x04000A90 RID: 2704
	private const string kVibratingSpring = "v1_SIResource:vibratingSpring";

	// Token: 0x04000A91 RID: 2705
	private const string kBouncySand = "v1_SIResource:bouncySand";

	// Token: 0x04000A92 RID: 2706
	private const string kFloppyMetal = "v1_SIResource:floppyMetal";

	// Token: 0x04000A93 RID: 2707
	private const string kStartingPackageGranted = "v1_SIProgression:startingPackageGranted";

	// Token: 0x04000A94 RID: 2708
	public TimeSpan CROSSOVER_TIME_OF_DAY = new TimeSpan(1, 0, 0);

	// Token: 0x04000A95 RID: 2709
	public DateTime lastQuestGrantTime;

	// Token: 0x04000A96 RID: 2710
	public int stashedQuests;

	// Token: 0x04000A97 RID: 2711
	public int completedQuests;

	// Token: 0x04000A98 RID: 2712
	public int stashedBonusPoints;

	// Token: 0x04000A99 RID: 2713
	public int completedBonusPoints;

	// Token: 0x04000A9A RID: 2714
	public int bonusProgress;

	// Token: 0x04000A9B RID: 2715
	public int questGrantRefreshCooldown = 28800;

	// Token: 0x04000A9C RID: 2716
	public Dictionary<SIResource.ResourceType, int> resourceDict;

	// Token: 0x04000A9D RID: 2717
	public int[] limitedDepositTimeArray;

	// Token: 0x04000A9E RID: 2718
	public bool[][] unlockedTechTreeData;

	// Token: 0x04000A9F RID: 2719
	[SerializeField]
	private int[] activeQuestIds = new int[3];

	// Token: 0x04000AA0 RID: 2720
	[SerializeField]
	private int[] activeQuestProgresses = new int[3];

	// Token: 0x04000AA1 RID: 2721
	[SerializeField]
	private QuestCategory[] activeQuestCategories = new QuestCategory[3];

	// Token: 0x04000AA2 RID: 2722
	private bool dailyLimitedTurnedIn;

	// Token: 0x04000AA3 RID: 2723
	public SIProgression.SIProgressionResourceCap[] resourceCaps;

	// Token: 0x04000AA4 RID: 2724
	public int[] resourceCapsArray;

	// Token: 0x04000AA5 RID: 2725
	private DateTime lastQuestGrantTimeDiff;

	// Token: 0x04000AA6 RID: 2726
	private int stashedQuestsDiff;

	// Token: 0x04000AA7 RID: 2727
	private int stashedBonusPointsDiff;

	// Token: 0x04000AA8 RID: 2728
	private int bonusProgressDiff;

	// Token: 0x04000AA9 RID: 2729
	private int[] resourceArrayDiff;

	// Token: 0x04000AAA RID: 2730
	private int[] limitedDepositTimeDiff;

	// Token: 0x04000AAB RID: 2731
	private bool[][] unlockedTechTreeDataDiff;

	// Token: 0x04000AAC RID: 2732
	private int[] activeQuestIdsDiff;

	// Token: 0x04000AAD RID: 2733
	private int[] activeQuestProgressesDiff;

	// Token: 0x04000AAE RID: 2734
	private bool questsInitialized;

	// Token: 0x04000AAF RID: 2735
	private bool _startingPackageGranted;

	// Token: 0x04000AB0 RID: 2736
	private float lastStartingPackageAttemptStarted;

	// Token: 0x04000AB1 RID: 2737
	private int startingPackageBackupAttempts;

	// Token: 0x04000AB2 RID: 2738
	private const int STARTING_PACKAGE_MAX_ATTEMPTS = 10;

	// Token: 0x04000AB3 RID: 2739
	private bool[] redeemingQuestInProgress = new bool[3];

	// Token: 0x04000AB4 RID: 2740
	private float lastDisconnectTelemetrySent;

	// Token: 0x04000AB5 RID: 2741
	private float minDisconnectTelemetryCooldown = 60f;

	// Token: 0x02000146 RID: 326
	public struct SINode
	{
		// Token: 0x04000AB6 RID: 2742
		public string id;

		// Token: 0x04000AB7 RID: 2743
		public bool unlocked;

		// Token: 0x04000AB8 RID: 2744
		public Dictionary<SIResource.ResourceType, int> costs;

		// Token: 0x04000AB9 RID: 2745
		public List<SIProgression.SINode> parents;

		// Token: 0x04000ABA RID: 2746
		public SIUpgradeType upgradeType;
	}

	// Token: 0x02000147 RID: 327
	[Serializable]
	public class SIQuestsList
	{
		// Token: 0x060008A2 RID: 2210 RVA: 0x0002F7F4 File Offset: 0x0002D9F4
		public RotatingQuest GetQuestById(int questID)
		{
			foreach (RotatingQuest rotatingQuest in this.quests)
			{
				if (rotatingQuest.questID == questID)
				{
					return rotatingQuest.disable ? null : rotatingQuest;
				}
			}
			return null;
		}

		// Token: 0x04000ABB RID: 2747
		public List<RotatingQuest> quests;
	}

	// Token: 0x02000148 RID: 328
	[Serializable]
	public struct SIProgressionResourceCap
	{
		// Token: 0x04000ABC RID: 2748
		public SIResource.ResourceType resourceType;

		// Token: 0x04000ABD RID: 2749
		public int resourceMax;
	}
}
