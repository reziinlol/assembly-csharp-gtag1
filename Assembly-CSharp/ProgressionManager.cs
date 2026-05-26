using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000940 RID: 2368
public class ProgressionManager : MonoBehaviour
{
	// Token: 0x1700059F RID: 1439
	// (get) Token: 0x06003E0F RID: 15887 RVA: 0x0014F278 File Offset: 0x0014D478
	// (set) Token: 0x06003E10 RID: 15888 RVA: 0x0014F27F File Offset: 0x0014D47F
	public static ProgressionManager Instance { get; private set; }

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06003E11 RID: 15889 RVA: 0x0014F288 File Offset: 0x0014D488
	// (remove) Token: 0x06003E12 RID: 15890 RVA: 0x0014F2C0 File Offset: 0x0014D4C0
	public event Action OnTreeUpdated;

	// Token: 0x14000072 RID: 114
	// (add) Token: 0x06003E13 RID: 15891 RVA: 0x0014F2F8 File Offset: 0x0014D4F8
	// (remove) Token: 0x06003E14 RID: 15892 RVA: 0x0014F330 File Offset: 0x0014D530
	public event Action OnInventoryUpdated;

	// Token: 0x14000073 RID: 115
	// (add) Token: 0x06003E15 RID: 15893 RVA: 0x0014F368 File Offset: 0x0014D568
	// (remove) Token: 0x06003E16 RID: 15894 RVA: 0x0014F3A0 File Offset: 0x0014D5A0
	public event Action<string, int> OnTrackRead;

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x06003E17 RID: 15895 RVA: 0x0014F3D8 File Offset: 0x0014D5D8
	// (remove) Token: 0x06003E18 RID: 15896 RVA: 0x0014F410 File Offset: 0x0014D610
	public event Action<string, int> OnTrackSet;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x06003E19 RID: 15897 RVA: 0x0014F448 File Offset: 0x0014D648
	// (remove) Token: 0x06003E1A RID: 15898 RVA: 0x0014F480 File Offset: 0x0014D680
	public event Action<string, string> OnNodeUnlocked;

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x06003E1B RID: 15899 RVA: 0x0014F4B8 File Offset: 0x0014D6B8
	// (remove) Token: 0x06003E1C RID: 15900 RVA: 0x0014F4F0 File Offset: 0x0014D6F0
	public event Action<string, int> OnGetShiftCredit;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x06003E1D RID: 15901 RVA: 0x0014F528 File Offset: 0x0014D728
	// (remove) Token: 0x06003E1E RID: 15902 RVA: 0x0014F560 File Offset: 0x0014D760
	public event Action<string, int, int> OnGetShiftCreditCapData;

	// Token: 0x14000078 RID: 120
	// (add) Token: 0x06003E1F RID: 15903 RVA: 0x0014F598 File Offset: 0x0014D798
	// (remove) Token: 0x06003E20 RID: 15904 RVA: 0x0014F5D0 File Offset: 0x0014D7D0
	public event Action<bool> OnPurchaseShiftCreditCapIncrease;

	// Token: 0x14000079 RID: 121
	// (add) Token: 0x06003E21 RID: 15905 RVA: 0x0014F608 File Offset: 0x0014D808
	// (remove) Token: 0x06003E22 RID: 15906 RVA: 0x0014F640 File Offset: 0x0014D840
	public event Action<bool> OnPurchaseShiftCredit;

	// Token: 0x1400007A RID: 122
	// (add) Token: 0x06003E23 RID: 15907 RVA: 0x0014F678 File Offset: 0x0014D878
	// (remove) Token: 0x06003E24 RID: 15908 RVA: 0x0014F6B0 File Offset: 0x0014D8B0
	public event Action<bool> OnChaosDepositSuccess;

	// Token: 0x1400007B RID: 123
	// (add) Token: 0x06003E25 RID: 15909 RVA: 0x0014F6E8 File Offset: 0x0014D8E8
	// (remove) Token: 0x06003E26 RID: 15910 RVA: 0x0014F720 File Offset: 0x0014D920
	public event Action<ProgressionManager.JuicerStatusResponse> OnJucierStatusUpdated;

	// Token: 0x1400007C RID: 124
	// (add) Token: 0x06003E27 RID: 15911 RVA: 0x0014F758 File Offset: 0x0014D958
	// (remove) Token: 0x06003E28 RID: 15912 RVA: 0x0014F790 File Offset: 0x0014D990
	public event Action<bool> OnPurchaseOverdrive;

	// Token: 0x1400007D RID: 125
	// (add) Token: 0x06003E29 RID: 15913 RVA: 0x0014F7C8 File Offset: 0x0014D9C8
	// (remove) Token: 0x06003E2A RID: 15914 RVA: 0x0014F800 File Offset: 0x0014DA00
	public event Action<ProgressionManager.DockWristStatusResponse> OnDockWristStatusUpdated;

	// Token: 0x1400007E RID: 126
	// (add) Token: 0x06003E2B RID: 15915 RVA: 0x0014F838 File Offset: 0x0014DA38
	// (remove) Token: 0x06003E2C RID: 15916 RVA: 0x0014F870 File Offset: 0x0014DA70
	public event Action<ProgressionManager.GhostReactorStatsResponse> OnGhostReactorStatsUpdated;

	// Token: 0x1400007F RID: 127
	// (add) Token: 0x06003E2D RID: 15917 RVA: 0x0014F8A8 File Offset: 0x0014DAA8
	// (remove) Token: 0x06003E2E RID: 15918 RVA: 0x0014F8E0 File Offset: 0x0014DAE0
	public event Action<ProgressionManager.GhostReactorInventoryResponse> OnGhostReactorInventoryUpdated;

	// Token: 0x06003E2F RID: 15919 RVA: 0x0014F915 File Offset: 0x0014DB15
	private void Awake()
	{
		if (ProgressionManager.Instance == null)
		{
			ProgressionManager.Instance = this;
		}
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x0014F92C File Offset: 0x0014DB2C
	public void RefreshProgressionTree()
	{
		ProgressionManager.<RefreshProgressionTree>d__71 <RefreshProgressionTree>d__;
		<RefreshProgressionTree>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshProgressionTree>d__.<>4__this = this;
		<RefreshProgressionTree>d__.<>1__state = -1;
		<RefreshProgressionTree>d__.<>t__builder.Start<ProgressionManager.<RefreshProgressionTree>d__71>(ref <RefreshProgressionTree>d__);
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x0014F964 File Offset: 0x0014DB64
	public void RefreshUserInventory()
	{
		ProgressionManager.<RefreshUserInventory>d__72 <RefreshUserInventory>d__;
		<RefreshUserInventory>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshUserInventory>d__.<>4__this = this;
		<RefreshUserInventory>d__.<>1__state = -1;
		<RefreshUserInventory>d__.<>t__builder.Start<ProgressionManager.<RefreshUserInventory>d__72>(ref <RefreshUserInventory>d__);
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x0014F99C File Offset: 0x0014DB9C
	public UserHydratedProgressionTreeResponse GetTree(string treeName)
	{
		UserHydratedProgressionTreeResponse result;
		this._trees.TryGetValue(treeName, out result);
		return result;
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x0014F9B9 File Offset: 0x0014DBB9
	public bool GetInventoryItem(string inventoryKey, out ProgressionManager.MothershipItemSummary item)
	{
		return this._inventory.TryGetValue((inventoryKey != null) ? inventoryKey.Trim() : null, out item);
	}

	// Token: 0x06003E34 RID: 15924 RVA: 0x0014F9D4 File Offset: 0x0014DBD4
	public int GetNodeCost(string treeName, string nodeId, string currencyKey)
	{
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse;
		if (!this._trees.TryGetValue(treeName, out userHydratedProgressionTreeResponse) || userHydratedProgressionTreeResponse == null || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(currencyKey))
		{
			return 0;
		}
		foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
		{
			if (userHydratedNodeDefinition.id == nodeId && userHydratedNodeDefinition.cost != null && userHydratedNodeDefinition.cost.items != null)
			{
				using (HydratedInventoryChangeMap.HydratedInventoryChangeMapEnumerator enumerator2 = userHydratedNodeDefinition.cost.items.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair = enumerator2.Current;
						string key = keyValuePair.Key;
						if (string.Equals((key != null) ? key.Trim() : null, currencyKey.Trim(), StringComparison.Ordinal))
						{
							return keyValuePair.Value.Delta;
						}
					}
					break;
				}
			}
		}
		return 0;
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x0014FAD8 File Offset: 0x0014DCD8
	public void GetProgression(string trackId)
	{
		ProgressionManager.<GetProgression>d__76 <GetProgression>d__;
		<GetProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<GetProgression>d__.<>4__this = this;
		<GetProgression>d__.trackId = trackId;
		<GetProgression>d__.<>1__state = -1;
		<GetProgression>d__.<>t__builder.Start<ProgressionManager.<GetProgression>d__76>(ref <GetProgression>d__);
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x0014FB18 File Offset: 0x0014DD18
	public void SetProgression(string trackId, int progress)
	{
		ProgressionManager.<SetProgression>d__77 <SetProgression>d__;
		<SetProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetProgression>d__.<>4__this = this;
		<SetProgression>d__.trackId = trackId;
		<SetProgression>d__.progress = progress;
		<SetProgression>d__.<>1__state = -1;
		<SetProgression>d__.<>t__builder.Start<ProgressionManager.<SetProgression>d__77>(ref <SetProgression>d__);
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x0014FB60 File Offset: 0x0014DD60
	public void UnlockNode(string treeId, string nodeId)
	{
		ProgressionManager.<UnlockNode>d__78 <UnlockNode>d__;
		<UnlockNode>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UnlockNode>d__.<>4__this = this;
		<UnlockNode>d__.treeId = treeId;
		<UnlockNode>d__.nodeId = nodeId;
		<UnlockNode>d__.<>1__state = -1;
		<UnlockNode>d__.<>t__builder.Start<ProgressionManager.<UnlockNode>d__78>(ref <UnlockNode>d__);
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x0014FBA8 File Offset: 0x0014DDA8
	public void IncrementSIResource(string resourceName, Action<string> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<IncrementSIResource>d__79 <IncrementSIResource>d__;
		<IncrementSIResource>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<IncrementSIResource>d__.<>4__this = this;
		<IncrementSIResource>d__.resourceName = resourceName;
		<IncrementSIResource>d__.OnSuccess = OnSuccess;
		<IncrementSIResource>d__.OnFailure = OnFailure;
		<IncrementSIResource>d__.<>1__state = -1;
		<IncrementSIResource>d__.<>t__builder.Start<ProgressionManager.<IncrementSIResource>d__79>(ref <IncrementSIResource>d__);
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x0014FBF8 File Offset: 0x0014DDF8
	public void CompleteSIQuest(int questID, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<CompleteSIQuest>d__80 <CompleteSIQuest>d__;
		<CompleteSIQuest>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<CompleteSIQuest>d__.<>4__this = this;
		<CompleteSIQuest>d__.questID = questID;
		<CompleteSIQuest>d__.OnSuccess = OnSuccess;
		<CompleteSIQuest>d__.OnFailure = OnFailure;
		<CompleteSIQuest>d__.<>1__state = -1;
		<CompleteSIQuest>d__.<>t__builder.Start<ProgressionManager.<CompleteSIQuest>d__80>(ref <CompleteSIQuest>d__);
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x0014FC48 File Offset: 0x0014DE48
	public void CompleteSIBonus(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<CompleteSIBonus>d__81 <CompleteSIBonus>d__;
		<CompleteSIBonus>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<CompleteSIBonus>d__.<>4__this = this;
		<CompleteSIBonus>d__.OnSuccess = OnSuccess;
		<CompleteSIBonus>d__.OnFailure = OnFailure;
		<CompleteSIBonus>d__.<>1__state = -1;
		<CompleteSIBonus>d__.<>t__builder.Start<ProgressionManager.<CompleteSIBonus>d__81>(ref <CompleteSIBonus>d__);
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x0014FC90 File Offset: 0x0014DE90
	public void CollectSIIdol(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<CollectSIIdol>d__82 <CollectSIIdol>d__;
		<CollectSIIdol>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<CollectSIIdol>d__.<>4__this = this;
		<CollectSIIdol>d__.OnSuccess = OnSuccess;
		<CollectSIIdol>d__.OnFailure = OnFailure;
		<CollectSIIdol>d__.<>1__state = -1;
		<CollectSIIdol>d__.<>t__builder.Start<ProgressionManager.<CollectSIIdol>d__82>(ref <CollectSIIdol>d__);
	}

	// Token: 0x06003E3C RID: 15932 RVA: 0x0014FCD8 File Offset: 0x0014DED8
	public void GetActiveSIQuests(Action<List<RotatingQuest>> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<GetActiveSIQuests>d__83 <GetActiveSIQuests>d__;
		<GetActiveSIQuests>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<GetActiveSIQuests>d__.<>4__this = this;
		<GetActiveSIQuests>d__.OnSuccess = OnSuccess;
		<GetActiveSIQuests>d__.OnFailure = OnFailure;
		<GetActiveSIQuests>d__.<>1__state = -1;
		<GetActiveSIQuests>d__.<>t__builder.Start<ProgressionManager.<GetActiveSIQuests>d__83>(ref <GetActiveSIQuests>d__);
	}

	// Token: 0x06003E3D RID: 15933 RVA: 0x0014FD20 File Offset: 0x0014DF20
	public void GetSIQuestStatus(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<GetSIQuestStatus>d__84 <GetSIQuestStatus>d__;
		<GetSIQuestStatus>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<GetSIQuestStatus>d__.<>4__this = this;
		<GetSIQuestStatus>d__.OnSuccess = OnSuccess;
		<GetSIQuestStatus>d__.OnFailure = OnFailure;
		<GetSIQuestStatus>d__.<>1__state = -1;
		<GetSIQuestStatus>d__.<>t__builder.Start<ProgressionManager.<GetSIQuestStatus>d__84>(ref <GetSIQuestStatus>d__);
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x0014FD68 File Offset: 0x0014DF68
	public void PurchaseTechPoints(int amount, Action OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<PurchaseTechPoints>d__85 <PurchaseTechPoints>d__;
		<PurchaseTechPoints>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<PurchaseTechPoints>d__.<>4__this = this;
		<PurchaseTechPoints>d__.amount = amount;
		<PurchaseTechPoints>d__.OnSuccess = OnSuccess;
		<PurchaseTechPoints>d__.OnFailure = OnFailure;
		<PurchaseTechPoints>d__.<>1__state = -1;
		<PurchaseTechPoints>d__.<>t__builder.Start<ProgressionManager.<PurchaseTechPoints>d__85>(ref <PurchaseTechPoints>d__);
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x0014FDB8 File Offset: 0x0014DFB8
	public void PurchaseResources(Action<ProgressionManager.UserInventory> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<PurchaseResources>d__86 <PurchaseResources>d__;
		<PurchaseResources>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<PurchaseResources>d__.<>4__this = this;
		<PurchaseResources>d__.OnSuccess = OnSuccess;
		<PurchaseResources>d__.OnFailure = OnFailure;
		<PurchaseResources>d__.<>1__state = -1;
		<PurchaseResources>d__.<>t__builder.Start<ProgressionManager.<PurchaseResources>d__86>(ref <PurchaseResources>d__);
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x0014FDFF File Offset: 0x0014DFFF
	public void PurchaseShiftCreditCapIncrease()
	{
		this.PurchaseShiftCreditCapIncreaseInternal(false);
	}

	// Token: 0x06003E41 RID: 15937 RVA: 0x0014FE08 File Offset: 0x0014E008
	private void PurchaseShiftCreditCapIncreaseInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseShiftCreditCapIncrease(new ProgressionManager.PurchaseShiftCreditCapIncreaseRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E42 RID: 15938 RVA: 0x0014FE5A File Offset: 0x0014E05A
	public void PurchaseShiftCredit()
	{
		this.PurchaseShiftCreditInternal(false);
	}

	// Token: 0x06003E43 RID: 15939 RVA: 0x0014FE64 File Offset: 0x0014E064
	private void PurchaseShiftCreditInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseShiftCredit(new ProgressionManager.PurchaseShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E44 RID: 15940 RVA: 0x0014FEB8 File Offset: 0x0014E0B8
	public void GetShiftCredit(string mothershipId)
	{
		base.StartCoroutine(this.DoGetShiftCredit(new ProgressionManager.GetShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			TargetMothershipId = mothershipId
		}));
	}

	// Token: 0x06003E45 RID: 15941 RVA: 0x0014FF0A File Offset: 0x0014E10A
	public void GetJuicerStatus()
	{
		this.GetJuicerStatusInternal(false);
	}

	// Token: 0x06003E46 RID: 15942 RVA: 0x0014FF14 File Offset: 0x0014E114
	private void GetJuicerStatusInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoGetJuicerStatus(new ProgressionManager.GetJuicerStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E47 RID: 15943 RVA: 0x0014FF66 File Offset: 0x0014E166
	public void DepositCore(ProgressionManager.CoreType coreType)
	{
		this.DepositCoreInternal(coreType, false);
	}

	// Token: 0x06003E48 RID: 15944 RVA: 0x0014FF70 File Offset: 0x0014E170
	private void DepositCoreInternal(ProgressionManager.CoreType coreType, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoDepositCore(new ProgressionManager.DepositCoreRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			CoreBeingDeposited = coreType,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E49 RID: 15945 RVA: 0x0014FFC9 File Offset: 0x0014E1C9
	public void PurchaseOverdrive()
	{
		this.PurchaseOverdriveInternal(false);
	}

	// Token: 0x06003E4A RID: 15946 RVA: 0x0014FFD4 File Offset: 0x0014E1D4
	private void PurchaseOverdriveInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseOverdrive(new ProgressionManager.PurchaseOverdriveRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E4B RID: 15947 RVA: 0x00150026 File Offset: 0x0014E226
	public void SubtractShiftCredit(int creditsToSubtract)
	{
		this.SubtractShiftCreditInternal(creditsToSubtract, false);
	}

	// Token: 0x06003E4C RID: 15948 RVA: 0x00150030 File Offset: 0x0014E230
	private void SubtractShiftCreditInternal(int creditsToSubtract, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoSubtractShiftCredit(new ProgressionManager.SubtractShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftCreditToRemove = creditsToSubtract,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E4D RID: 15949 RVA: 0x00150089 File Offset: 0x0014E289
	public void AdvanceDockWristUpgradeLevel(ProgressionManager.WristDockUpgradeType upgrade)
	{
		this.AdvanceDockWristUpgradeLevelInternal(upgrade, false);
	}

	// Token: 0x06003E4E RID: 15950 RVA: 0x00150094 File Offset: 0x0014E294
	private void AdvanceDockWristUpgradeLevelInternal(ProgressionManager.WristDockUpgradeType upgrade, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoAdvanceDockWristUpgradeLevel(new ProgressionManager.AdvanceDockWristUpgradeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			Upgrade = upgrade,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E4F RID: 15951 RVA: 0x001500ED File Offset: 0x0014E2ED
	public void GetDockWristUpgradeStatus()
	{
		base.StartCoroutine(this.DoGetDockWristUpgradeStatus(new ProgressionManager.DockWristUpgradeStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x06003E50 RID: 15952 RVA: 0x00150130 File Offset: 0x0014E330
	public void PurchaseDrillUpgrade(ProgressionManager.DrillUpgradeLevel upgrade)
	{
		base.StartCoroutine(this.DoPurchaseDrillUpgrade(new ProgressionManager.PurchaseDrillUpgradeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			Upgrade = upgrade
		}));
	}

	// Token: 0x06003E51 RID: 15953 RVA: 0x00150184 File Offset: 0x0014E384
	public void RecycleTool(GRTool.GRToolType toolBeingRecycled, int numberOfPlayers)
	{
		base.StartCoroutine(this.DoRecycleTool(new ProgressionManager.RecycleToolRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ToolBeingRecycled = toolBeingRecycled,
			NumberOfPlayers = numberOfPlayers
		}));
	}

	// Token: 0x06003E52 RID: 15954 RVA: 0x001501E0 File Offset: 0x0014E3E0
	public void StartOfShift(string shiftId, int coresRequired, int numberOfPlayers, int depth)
	{
		base.StartCoroutine(this.DoStartOfShift(new ProgressionManager.StartOfShiftRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftId = shiftId,
			CoresRequired = coresRequired,
			NumberOfPlayers = numberOfPlayers,
			Depth = depth
		}));
	}

	// Token: 0x06003E53 RID: 15955 RVA: 0x00150248 File Offset: 0x0014E448
	public void EndOfShiftReward(string shiftId)
	{
		this.EndOfShiftRewardInternal(shiftId, false);
	}

	// Token: 0x06003E54 RID: 15956 RVA: 0x00150254 File Offset: 0x0014E454
	private void EndOfShiftRewardInternal(string shiftId, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoEndOfShiftReward(new ProgressionManager.EndOfShiftRewardRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftId = shiftId,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E55 RID: 15957 RVA: 0x001502AD File Offset: 0x0014E4AD
	public void GetGhostReactorStats()
	{
		base.StartCoroutine(this.DoGetGhostReactorStats(new ProgressionManager.GhostReactorStatsRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x06003E56 RID: 15958 RVA: 0x001502ED File Offset: 0x0014E4ED
	public void GetGhostReactorInventory()
	{
		base.StartCoroutine(this.DoGetGhostReactorInventory(new ProgressionManager.GhostReactorInventoryRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x06003E57 RID: 15959 RVA: 0x0015032D File Offset: 0x0014E52D
	public void SetGhostReactorInventory(string jsonInventory)
	{
		this.SetGhostReactorInventoryInternal(jsonInventory, false);
	}

	// Token: 0x06003E58 RID: 15960 RVA: 0x00150338 File Offset: 0x0014E538
	private void SetGhostReactorInventoryInternal(string jsonInventory, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoSetGhostReactorInventory(new ProgressionManager.SetGhostReactorInventoryRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			InventoryJson = jsonInventory,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003E59 RID: 15961 RVA: 0x00150391 File Offset: 0x0014E591
	private IEnumerator HandleWebRequestRetries<T>(ProgressionManager.RequestType requestType, T data, Action<T> actionToTake, Action failureActionToTake = null)
	{
		if (!this.retryCounters.ContainsKey(requestType))
		{
			this.retryCounters[requestType] = 0;
		}
		if (this.retryCounters[requestType] < this.maxRetriesOnFail)
		{
			float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.retryCounters[requestType] + 1)));
			Debug.LogWarning(string.Format("PM: Retrying ... attempt #{0}, waiting {1}s", this.retryCounters[requestType] + 1, num));
			Dictionary<ProgressionManager.RequestType, int> dictionary = this.retryCounters;
			int num2 = dictionary[requestType];
			dictionary[requestType] = num2 + 1;
			yield return new WaitForSecondsRealtime(num);
			actionToTake(data);
		}
		else
		{
			Debug.LogError("PM: Maximum retries attempted.");
			this.retryCounters[requestType] = 0;
			if (failureActionToTake != null)
			{
				failureActionToTake();
			}
		}
		yield break;
	}

	// Token: 0x06003E5A RID: 15962 RVA: 0x001503C0 File Offset: 0x0014E5C0
	private bool HandleWebRequestFailures(UnityWebRequest request, bool retryOnConflict = false)
	{
		bool result = false;
		Debug.LogError(string.Format("PM: HandleWebRequestFailures Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
		if (request.result != UnityWebRequest.Result.ProtocolError)
		{
			result = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_6A;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_6A;
			}
			bool flag = true;
			goto IL_6C;
			IL_6A:
			flag = false;
			IL_6C:
			if (flag || (retryOnConflict && request.responseCode == 409L))
			{
				result = true;
				Debug.LogError(string.Format("PM: HTTP {0} error: {1}", request.responseCode, request.error));
			}
		}
		return result;
	}

	// Token: 0x06003E5B RID: 15963 RVA: 0x00150470 File Offset: 0x0014E670
	private IEnumerator DoGetProgression(ProgressionManager.GetProgressionRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetProgressionRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetProgression);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			int num = int.Parse(request.downloadHandler.text);
			this._tracks[data.TrackId] = num;
			Debug.Log("PM: GetProgression Success: track is " + data.TrackId + " and progress is " + num.ToString());
			this.retryCounters[ProgressionManager.RequestType.GetProgression] = 0;
			Action<string, int> onTrackRead = this.OnTrackRead;
			if (onTrackRead != null)
			{
				onTrackRead(data.TrackId, num);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<string>(ProgressionManager.RequestType.GetProgression, data.TrackId, delegate(string x)
		{
			this.GetProgression(x);
		}, null);
		yield break;
	}

	// Token: 0x06003E5C RID: 15964 RVA: 0x00150486 File Offset: 0x0014E686
	private IEnumerator DoSetProgression(ProgressionManager.SetProgressionRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetProgressionRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SetProgression);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.GetProgressionResponse getProgressionResponse = JsonConvert.DeserializeObject<ProgressionManager.GetProgressionResponse>(request.downloadHandler.text);
			this._tracks[data.TrackId] = getProgressionResponse.Progress;
			this.retryCounters[ProgressionManager.RequestType.SetProgression] = 0;
			Action<string, int> onTrackSet = this.OnTrackSet;
			if (onTrackSet != null)
			{
				onTrackSet(data.TrackId, getProgressionResponse.Progress);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ValueTuple<string, int>>(ProgressionManager.RequestType.SetProgression, new ValueTuple<string, int>(data.TrackId, data.Progress), delegate([TupleElementNames(new string[]
		{
			"TrackId",
			"Progress"
		})] ValueTuple<string, int> x)
		{
			this.SetProgression(x.Item1, x.Item2);
		}, null);
		yield break;
	}

	// Token: 0x06003E5D RID: 15965 RVA: 0x0015049C File Offset: 0x0014E69C
	private IEnumerator DoUnlockNode(ProgressionManager.UnlockNodeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.UnlockNodeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.UnlockProgressionTreeNode);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.UnlockProgressionTreeNode] = 0;
			this.RefreshProgressionTree();
			this.RefreshUserInventory();
			Action<string, string> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked != null)
			{
				onNodeUnlocked(data.TreeId, data.NodeId);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ValueTuple<string, string>>(ProgressionManager.RequestType.UnlockProgressionTreeNode, new ValueTuple<string, string>(data.TreeId, data.NodeId), delegate([TupleElementNames(new string[]
		{
			"TreeId",
			"NodeId"
		})] ValueTuple<string, string> x)
		{
			this.UnlockNode(x.Item1, x.Item2);
		}, null);
		yield break;
	}

	// Token: 0x06003E5E RID: 15966 RVA: 0x001504B2 File Offset: 0x0014E6B2
	private IEnumerator DoIncrementSIResource(ProgressionManager.IncrementSIResourceRequest data, Action<string> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.IncrementSIResourceRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.IncrementSIResource);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.IncrementSIResourceResponse incrementSIResourceResponse = JsonConvert.DeserializeObject<ProgressionManager.IncrementSIResourceResponse>(request.downloadHandler.text);
			Action<string> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(incrementSIResourceResponse.ResourceType);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.IncrementSIResourceRequest>(ProgressionManager.RequestType.IncrementSIResource, data, delegate(ProgressionManager.IncrementSIResourceRequest x)
		{
			this.IncrementSIResource(data.ResourceType, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E5F RID: 15967 RVA: 0x001504D6 File Offset: 0x0014E6D6
	private IEnumerator DoQuestCompleteReward(ProgressionManager.SetSIQuestCompleteRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIQuestCompleteRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CompleteSIQuest);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIQuestCompleteRequest>(ProgressionManager.RequestType.CompleteSIQuest, data, delegate(ProgressionManager.SetSIQuestCompleteRequest x)
		{
			this.CompleteSIQuest(data.QuestID, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E60 RID: 15968 RVA: 0x001504FA File Offset: 0x0014E6FA
	private IEnumerator DoBonusCompleteReward(ProgressionManager.SetSIBonusCompleteRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIBonusCompleteRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CompleteSIBonus);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIBonusCompleteRequest>(ProgressionManager.RequestType.CompleteSIBonus, data, delegate(ProgressionManager.SetSIBonusCompleteRequest x)
		{
			this.CompleteSIBonus(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E61 RID: 15969 RVA: 0x0015051E File Offset: 0x0014E71E
	private IEnumerator DoIdolCollectReward(ProgressionManager.SetSIIdolCollectRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIIdolCollectRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CollectSIIdol);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIIdolCollectRequest>(ProgressionManager.RequestType.CollectSIIdol, data, delegate(ProgressionManager.SetSIIdolCollectRequest x)
		{
			this.CollectSIIdol(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E62 RID: 15970 RVA: 0x00150542 File Offset: 0x0014E742
	private IEnumerator DoGetActiveSIQuests(ProgressionManager.GetActiveSIQuestsRequest data, Action<List<RotatingQuest>> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetActiveSIQuestsRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.GetActiveSIQuests);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetActiveSIQuestsResponse getActiveSIQuestsResponse = JsonConvert.DeserializeObject<ProgressionManager.GetActiveSIQuestsResponse>(request.downloadHandler.text);
			Action<List<RotatingQuest>> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getActiveSIQuestsResponse.Result.Quests);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetActiveSIQuestsRequest>(ProgressionManager.RequestType.GetActiveSIQuests, data, delegate(ProgressionManager.GetActiveSIQuestsRequest x)
		{
			this.GetActiveSIQuests(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E63 RID: 15971 RVA: 0x00150566 File Offset: 0x0014E766
	private IEnumerator DoGetSIQuestsStatus(ProgressionManager.GetSIQuestsStatusRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetSIQuestsStatusRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.GetSIQuestsStatus);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetSIQuestsStatusRequest>(ProgressionManager.RequestType.GetSIQuestsStatus, data, delegate(ProgressionManager.GetSIQuestsStatusRequest x)
		{
			this.GetSIQuestStatus(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E64 RID: 15972 RVA: 0x0015058A File Offset: 0x0014E78A
	private IEnumerator DoPurchaseTechPoints(ProgressionManager.PurchaseTechPointsRequest data, Action OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseTechPointsRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.PurchaseTechPoints);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			Action onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess();
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseTechPointsRequest>(ProgressionManager.RequestType.PurchaseTechPoints, data, delegate(ProgressionManager.PurchaseTechPointsRequest x)
		{
			this.PurchaseTechPoints(data.TechPointsAmount, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E65 RID: 15973 RVA: 0x001505AE File Offset: 0x0014E7AE
	private IEnumerator DoPurchaseResources(ProgressionManager.PurchaseResourcesRequest data, Action<ProgressionManager.UserInventory> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseResourcesRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.PurchaseResources);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.UserInventoryResponse userInventoryResponse = JsonConvert.DeserializeObject<ProgressionManager.UserInventoryResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserInventory> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess(userInventoryResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseResourcesRequest>(ProgressionManager.RequestType.PurchaseResources, data, delegate(ProgressionManager.PurchaseResourcesRequest x)
		{
			this.PurchaseResources(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2(request.error);
		});
		yield break;
	}

	// Token: 0x06003E66 RID: 15974 RVA: 0x001505D2 File Offset: 0x0014E7D2
	private IEnumerator DoPurchaseShiftCreditCapIncrease(ProgressionManager.PurchaseShiftCreditCapIncreaseRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseShiftCreditCapIncreaseRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.PurchaseShiftCreditCapIncreaseResponse purchaseShiftCreditCapIncreaseResponse = JsonConvert.DeserializeObject<ProgressionManager.PurchaseShiftCreditCapIncreaseResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease] = 0;
			this.RefreshShinyRocksTotal();
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(purchaseShiftCreditCapIncreaseResponse.TargetMothershipId, purchaseShiftCreditCapIncreaseResponse.CurrentShiftCreditCapIncreases, purchaseShiftCreditCapIncreaseResponse.CurrentShiftCreditCapIncreasesMax);
			}
			Action<bool> onPurchaseShiftCreditCapIncrease = this.OnPurchaseShiftCreditCapIncrease;
			if (onPurchaseShiftCreditCapIncrease != null)
			{
				onPurchaseShiftCreditCapIncrease(true);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "User Already Has Purchased Max Shift Credit Cap")
		{
			Action<bool> onPurchaseShiftCreditCapIncrease2 = this.OnPurchaseShiftCreditCapIncrease;
			if (onPurchaseShiftCreditCapIncrease2 != null)
			{
				onPurchaseShiftCreditCapIncrease2(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseShiftCreditCapIncreaseRequest>(ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease, data, delegate(ProgressionManager.PurchaseShiftCreditCapIncreaseRequest x)
		{
			this.PurchaseShiftCreditCapIncreaseInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E67 RID: 15975 RVA: 0x001505E8 File Offset: 0x0014E7E8
	private IEnumerator DoPurchaseShiftCredit(ProgressionManager.PurchaseShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.PurchaseShiftCreditResponse purchaseShiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.PurchaseShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.PurchaseShiftCredit] = 0;
			this.RefreshShinyRocksTotal();
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(purchaseShiftCreditResponse.TargetMothershipId, purchaseShiftCreditResponse.CurrentShiftCredits);
			}
			Action<bool> onPurchaseShiftCredit = this.OnPurchaseShiftCredit;
			if (onPurchaseShiftCredit != null)
			{
				onPurchaseShiftCredit(true);
			}
			GRPlayer local = GRPlayer.GetLocal();
			if (local != null)
			{
				local.SendCreditsRefilledTelemetry(100, purchaseShiftCreditResponse.CurrentShiftCredits);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "User Already at Max Shift Credit")
		{
			Action<bool> onPurchaseShiftCredit2 = this.OnPurchaseShiftCredit;
			if (onPurchaseShiftCredit2 != null)
			{
				onPurchaseShiftCredit2(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseShiftCreditRequest>(ProgressionManager.RequestType.PurchaseShiftCredit, data, delegate(ProgressionManager.PurchaseShiftCreditRequest x)
		{
			this.PurchaseShiftCreditInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E68 RID: 15976 RVA: 0x001505FE File Offset: 0x0014E7FE
	private IEnumerator DoGetShiftCredit(ProgressionManager.GetShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetShiftCredit] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetShiftCreditRequest>(ProgressionManager.RequestType.GetShiftCredit, data, delegate(ProgressionManager.GetShiftCreditRequest x)
		{
			this.GetShiftCredit(x.TargetMothershipId);
		}, null);
		yield break;
	}

	// Token: 0x06003E69 RID: 15977 RVA: 0x00150614 File Offset: 0x0014E814
	private IEnumerator DoGetJuicerStatus(ProgressionManager.GetJuicerStatusRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetJuicerStatusRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetJuicerStatus);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.GetJuicerStatus] = 0;
			ProgressionManager.JuicerStatusResponse obj = JsonConvert.DeserializeObject<ProgressionManager.JuicerStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.JuicerStatusResponse> onJucierStatusUpdated = this.OnJucierStatusUpdated;
			if (onJucierStatusUpdated != null)
			{
				onJucierStatusUpdated(obj);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetJuicerStatusRequest>(ProgressionManager.RequestType.GetJuicerStatus, data, delegate(ProgressionManager.GetJuicerStatusRequest x)
		{
			this.GetJuicerStatusInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E6A RID: 15978 RVA: 0x0015062A File Offset: 0x0014E82A
	private IEnumerator DoDepositCore(ProgressionManager.DepositCoreRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.DepositCoreRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.DepositCore);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.DepositCore] = 0;
			if (data.CoreBeingDeposited == ProgressionManager.CoreType.ChaosSeed)
			{
				Action<bool> onChaosDepositSuccess = this.OnChaosDepositSuccess;
				if (onChaosDepositSuccess != null)
				{
					onChaosDepositSuccess(true);
				}
				this.GetJuicerStatus();
			}
			else
			{
				ProgressionManager.DepositCoreResponse depositCoreResponse = JsonConvert.DeserializeObject<ProgressionManager.DepositCoreResponse>(request.downloadHandler.text);
				Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
				if (onGetShiftCredit != null)
				{
					onGetShiftCredit(data.MothershipId, depositCoreResponse.CurrentShiftCredits);
				}
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "DepositGRCore already at seed cap")
		{
			if (data.CoreBeingDeposited == ProgressionManager.CoreType.ChaosSeed)
			{
				Action<bool> onChaosDepositSuccess2 = this.OnChaosDepositSuccess;
				if (onChaosDepositSuccess2 != null)
				{
					onChaosDepositSuccess2(false);
				}
				this.GetJuicerStatus();
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.DepositCoreRequest>(ProgressionManager.RequestType.DepositCore, data, delegate(ProgressionManager.DepositCoreRequest x)
		{
			this.DepositCoreInternal(x.CoreBeingDeposited, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E6B RID: 15979 RVA: 0x00150640 File Offset: 0x0014E840
	private IEnumerator DoPurchaseOverdrive(ProgressionManager.PurchaseOverdriveRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseOverdriveRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseOverdrive);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.PurchaseOverdrive] = 0;
			this.GetJuicerStatus();
			this.RefreshShinyRocksTotal();
			Action<bool> onPurchaseOverdrive = this.OnPurchaseOverdrive;
			if (onPurchaseOverdrive != null)
			{
				onPurchaseOverdrive(true);
			}
			yield break;
		}
		if (request.responseCode == 400L && (request.downloadHandler.text == "User Already At Overdrive Cap" || request.downloadHandler.text == "User would exceed Overdrive Cap"))
		{
			Action<bool> onPurchaseOverdrive2 = this.OnPurchaseOverdrive;
			if (onPurchaseOverdrive2 != null)
			{
				onPurchaseOverdrive2(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseOverdriveRequest>(ProgressionManager.RequestType.PurchaseOverdrive, data, delegate(ProgressionManager.PurchaseOverdriveRequest x)
		{
			this.PurchaseOverdriveInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E6C RID: 15980 RVA: 0x00150656 File Offset: 0x0014E856
	private IEnumerator DoSubtractShiftCredit(ProgressionManager.SubtractShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SubtractShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SubtractShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.SubtractShiftCredit] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SubtractShiftCreditRequest>(ProgressionManager.RequestType.SubtractShiftCredit, data, delegate(ProgressionManager.SubtractShiftCreditRequest x)
		{
			this.SubtractShiftCreditInternal(data.ShiftCreditToRemove, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E6D RID: 15981 RVA: 0x0015066C File Offset: 0x0014E86C
	private IEnumerator DoAdvanceDockWristUpgradeLevel(ProgressionManager.AdvanceDockWristUpgradeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.AdvanceDockWristUpgradeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.AdvanceDockWristUpgrade);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.DockWristStatusResponse obj = JsonConvert.DeserializeObject<ProgressionManager.DockWristStatusResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.AdvanceDockWristUpgrade] = 0;
			Action<ProgressionManager.DockWristStatusResponse> onDockWristStatusUpdated = this.OnDockWristStatusUpdated;
			if (onDockWristStatusUpdated != null)
			{
				onDockWristStatusUpdated(obj);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.AdvanceDockWristUpgradeRequest>(ProgressionManager.RequestType.AdvanceDockWristUpgrade, data, delegate(ProgressionManager.AdvanceDockWristUpgradeRequest x)
		{
			this.AdvanceDockWristUpgradeLevelInternal(data.Upgrade, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E6E RID: 15982 RVA: 0x00150682 File Offset: 0x0014E882
	private IEnumerator DoGetDockWristUpgradeStatus(ProgressionManager.DockWristUpgradeStatusRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.DockWristUpgradeStatusRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetDockWristUpgradeStatus);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.DockWristStatusResponse obj = JsonConvert.DeserializeObject<ProgressionManager.DockWristStatusResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetDockWristUpgradeStatus] = 0;
			Action<ProgressionManager.DockWristStatusResponse> onDockWristStatusUpdated = this.OnDockWristStatusUpdated;
			if (onDockWristStatusUpdated != null)
			{
				onDockWristStatusUpdated(obj);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.DockWristUpgradeStatusRequest>(ProgressionManager.RequestType.GetDockWristUpgradeStatus, data, delegate(ProgressionManager.DockWristUpgradeStatusRequest x)
		{
			this.GetDockWristUpgradeStatus();
		}, null);
		yield break;
	}

	// Token: 0x06003E6F RID: 15983 RVA: 0x00150698 File Offset: 0x0014E898
	private IEnumerator DoPurchaseDrillUpgrade(ProgressionManager.PurchaseDrillUpgradeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseDrillUpgradeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseDrillUpgrade);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.PurchaseDrillUpgrade] = 0;
			this.RefreshUserInventory();
			Action<string, string> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked != null)
			{
				onNodeUnlocked("", "");
			}
			if (data.Upgrade == ProgressionManager.DrillUpgradeLevel.Base)
			{
				GRPlayer local = GRPlayer.GetLocal();
				if (local != null)
				{
					local.SendPodUpgradeTelemetry(ProgressionManager.DrillUpgradeLevel.Base.ToString(), 0, 2500, 0);
				}
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseDrillUpgradeRequest>(ProgressionManager.RequestType.PurchaseDrillUpgrade, data, delegate(ProgressionManager.PurchaseDrillUpgradeRequest x)
		{
			this.PurchaseDrillUpgrade(data.Upgrade);
		}, null);
		yield break;
	}

	// Token: 0x06003E70 RID: 15984 RVA: 0x001506AE File Offset: 0x0014E8AE
	private IEnumerator DoRecycleTool(ProgressionManager.RecycleToolRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.RecycleToolRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.RecycleTool);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.RecycleTool] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.RecycleToolRequest>(ProgressionManager.RequestType.RecycleTool, data, delegate(ProgressionManager.RecycleToolRequest x)
		{
			this.RecycleTool(data.ToolBeingRecycled, data.NumberOfPlayers);
		}, null);
		yield break;
	}

	// Token: 0x06003E71 RID: 15985 RVA: 0x001506C4 File Offset: 0x0014E8C4
	private IEnumerator DoStartOfShift(ProgressionManager.StartOfShiftRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.StartOfShiftRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.StartOfShift);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.StartOfShift] = 0;
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.StartOfShiftRequest>(ProgressionManager.RequestType.StartOfShift, data, delegate(ProgressionManager.StartOfShiftRequest x)
		{
			this.StartOfShift(data.ShiftId, data.CoresRequired, data.NumberOfPlayers, data.Depth);
		}, null);
		yield break;
	}

	// Token: 0x06003E72 RID: 15986 RVA: 0x001506DA File Offset: 0x0014E8DA
	private IEnumerator DoEndOfShiftReward(ProgressionManager.EndOfShiftRewardRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.EndOfShiftRewardRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.EndOfShiftReward);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.EndOfShiftReward] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.error == "EndOfShiftReward Unknown Shift or Mothership Failure.")
		{
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.EndOfShiftRewardRequest>(ProgressionManager.RequestType.EndOfShiftReward, data, delegate(ProgressionManager.EndOfShiftRewardRequest x)
		{
			this.EndOfShiftRewardInternal(data.ShiftId, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E73 RID: 15987 RVA: 0x001506F0 File Offset: 0x0014E8F0
	private IEnumerator DoGetGhostReactorStats(ProgressionManager.GhostReactorStatsRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GhostReactorStatsRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetGhostReactorStats);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.GhostReactorStatsResponse obj = JsonConvert.DeserializeObject<ProgressionManager.GhostReactorStatsResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetGhostReactorStats] = 0;
			Action<ProgressionManager.GhostReactorStatsResponse> onGhostReactorStatsUpdated = this.OnGhostReactorStatsUpdated;
			if (onGhostReactorStatsUpdated != null)
			{
				onGhostReactorStatsUpdated(obj);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GhostReactorStatsRequest>(ProgressionManager.RequestType.GetGhostReactorStats, data, delegate(ProgressionManager.GhostReactorStatsRequest x)
		{
			this.GetGhostReactorStats();
		}, null);
		yield break;
	}

	// Token: 0x06003E74 RID: 15988 RVA: 0x00150706 File Offset: 0x0014E906
	private IEnumerator DoGetGhostReactorInventory(ProgressionManager.GhostReactorInventoryRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GhostReactorInventoryRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetGhostReactorInventory);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			ProgressionManager.GhostReactorInventoryResponse obj = JsonConvert.DeserializeObject<ProgressionManager.GhostReactorInventoryResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetGhostReactorInventory] = 0;
			Action<ProgressionManager.GhostReactorInventoryResponse> onGhostReactorInventoryUpdated = this.OnGhostReactorInventoryUpdated;
			if (onGhostReactorInventoryUpdated != null)
			{
				onGhostReactorInventoryUpdated(obj);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GhostReactorInventoryRequest>(ProgressionManager.RequestType.GetGhostReactorInventory, data, delegate(ProgressionManager.GhostReactorInventoryRequest x)
		{
			this.GetGhostReactorInventory();
		}, null);
		yield break;
	}

	// Token: 0x06003E75 RID: 15989 RVA: 0x0015071C File Offset: 0x0014E91C
	private IEnumerator DoSetGhostReactorInventory(ProgressionManager.SetGhostReactorInventoryRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetGhostReactorInventoryRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SetGhostReactorInventory);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.Success)
		{
			this.retryCounters[ProgressionManager.RequestType.SetGhostReactorInventory] = 0;
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetGhostReactorInventoryRequest>(ProgressionManager.RequestType.SetGhostReactorInventory, data, delegate(ProgressionManager.SetGhostReactorInventoryRequest x)
		{
			this.SetGhostReactorInventoryInternal(data.InventoryJson, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x06003E76 RID: 15990 RVA: 0x00150732 File Offset: 0x0014E932
	private bool IsSuccessResponse(long code)
	{
		return code >= 200L && code < 300L;
	}

	// Token: 0x06003E77 RID: 15991 RVA: 0x00150748 File Offset: 0x0014E948
	private UnityWebRequest FormatWebRequest<T>(string url, T pendingRequest, ProgressionManager.RequestType type)
	{
		string str = "";
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(pendingRequest));
		switch (type)
		{
		case ProgressionManager.RequestType.GetProgression:
			str = "/api/GetProgression";
			break;
		case ProgressionManager.RequestType.SetProgression:
			str = "/api/SetProgression";
			break;
		case ProgressionManager.RequestType.UnlockProgressionTreeNode:
			str = "/api/UnlockProgressionTreeNode";
			break;
		case ProgressionManager.RequestType.IncrementSIResource:
			str = "/api/IncrementSIResource";
			break;
		case ProgressionManager.RequestType.CompleteSIQuest:
			str = "/api/SetSIQuestComplete";
			break;
		case ProgressionManager.RequestType.CompleteSIBonus:
			str = "/api/SetSIBonusComplete";
			break;
		case ProgressionManager.RequestType.CollectSIIdol:
			str = "/api/SetSIIdolCollect";
			break;
		case ProgressionManager.RequestType.GetActiveSIQuests:
			str = "/api/GetActiveSIQuests";
			break;
		case ProgressionManager.RequestType.GetSIQuestsStatus:
			str = "/api/GetSIQuestsStatus";
			break;
		case ProgressionManager.RequestType.ResetSIQuestsStatus:
			str = "/api/ResetSIQuestsStatus";
			break;
		case ProgressionManager.RequestType.PurchaseTechPoints:
			str = "/api/PurchaseTechPoints";
			break;
		case ProgressionManager.RequestType.PurchaseResources:
			str = "/api/PurchaseResources";
			break;
		case ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease:
			str = "/api/PurchaseShiftCreditCapIncrease";
			break;
		case ProgressionManager.RequestType.PurchaseShiftCredit:
			str = "/api/PurchaseShiftCredit";
			break;
		case ProgressionManager.RequestType.GetJuicerStatus:
			str = "/api/GetJuicerStatus";
			break;
		case ProgressionManager.RequestType.DepositCore:
			str = "/api/DepositGRCore";
			break;
		case ProgressionManager.RequestType.PurchaseOverdrive:
			str = "/api/PurchaseOverdrive";
			break;
		case ProgressionManager.RequestType.GetShiftCredit:
			str = "/api/GetShiftCredit";
			break;
		case ProgressionManager.RequestType.SubtractShiftCredit:
			str = "/api/SubtractShiftCredit";
			break;
		case ProgressionManager.RequestType.AdvanceDockWristUpgrade:
			str = "/api/AdvanceDockWristUpgrade";
			break;
		case ProgressionManager.RequestType.GetDockWristUpgradeStatus:
			str = "/api/GetDockWristUpgradeStatus";
			break;
		case ProgressionManager.RequestType.PurchaseDrillUpgrade:
			str = "/api/PurchaseDrillUpgrade";
			break;
		case ProgressionManager.RequestType.RecycleTool:
			str = "/api/RecycleTool";
			break;
		case ProgressionManager.RequestType.StartOfShift:
			str = "/api/StartOfShift";
			break;
		case ProgressionManager.RequestType.EndOfShiftReward:
			str = "/api/EndOfShiftReward";
			break;
		case ProgressionManager.RequestType.GetGhostReactorStats:
			str = "/api/GetGhostReactorStats";
			break;
		case ProgressionManager.RequestType.GetGhostReactorInventory:
			str = "/api/GetGhostReactorInventory";
			break;
		case ProgressionManager.RequestType.SetGhostReactorInventory:
			str = "/api/SetGhostReactorInventory";
			break;
		}
		UnityWebRequest unityWebRequest = new UnityWebRequest(url + str, "POST");
		unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		return unityWebRequest;
	}

	// Token: 0x06003E78 RID: 15992 RVA: 0x00150928 File Offset: 0x0014EB28
	private void OnGetTrees(GetProgressionTreesForPlayerResponse response)
	{
		if (((response != null) ? response.Results : null) == null)
		{
			return;
		}
		this._trees.Clear();
		foreach (UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse in response.Results)
		{
			UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse2 = new UserHydratedProgressionTreeResponse();
			userHydratedProgressionTreeResponse2.Tree = userHydratedProgressionTreeResponse.Tree;
			userHydratedProgressionTreeResponse2.Track = userHydratedProgressionTreeResponse.Track;
			userHydratedProgressionTreeResponse2.Nodes = userHydratedProgressionTreeResponse.Nodes;
			this._trees[userHydratedProgressionTreeResponse.Tree.name] = userHydratedProgressionTreeResponse2;
		}
		Action onTreeUpdated = this.OnTreeUpdated;
		if (onTreeUpdated == null)
		{
			return;
		}
		onTreeUpdated();
	}

	// Token: 0x06003E79 RID: 15993 RVA: 0x001509DC File Offset: 0x0014EBDC
	private void OnGetInventory(MothershipGetInventoryResponse response)
	{
		if (((response != null) ? response.Results : null) == null)
		{
			return;
		}
		this._inventory.Clear();
		foreach (KeyValuePair<string, MothershipPlayerInventorySummary> keyValuePair in response.Results)
		{
			MothershipPlayerInventorySummary value = keyValuePair.Value;
			if (((value != null) ? value.entitlements : null) != null)
			{
				foreach (MothershipInventoryItemSummary mothershipInventoryItemSummary in keyValuePair.Value.entitlements)
				{
					string name = mothershipInventoryItemSummary.name;
					string key = (name != null) ? name.Trim() : null;
					this._inventory[key] = new ProgressionManager.MothershipItemSummary
					{
						EntitlementId = mothershipInventoryItemSummary.entitlement_id,
						InGameId = mothershipInventoryItemSummary.in_game_id,
						Name = mothershipInventoryItemSummary.name,
						Quantity = mothershipInventoryItemSummary.quantity
					};
				}
			}
		}
		Action onInventoryUpdated = this.OnInventoryUpdated;
		if (onInventoryUpdated == null)
		{
			return;
		}
		onInventoryUpdated();
	}

	// Token: 0x06003E7A RID: 15994 RVA: 0x00150B04 File Offset: 0x0014ED04
	public int GetShinyRocksTotal()
	{
		if (CosmeticsController.instance != null)
		{
			return CosmeticsController.instance.CurrencyBalance;
		}
		return 0;
	}

	// Token: 0x06003E7B RID: 15995 RVA: 0x00150B23 File Offset: 0x0014ED23
	public void RefreshShinyRocksTotal()
	{
		if (CosmeticsController.instance != null)
		{
			CosmeticsController.instance.GetCurrencyBalance();
		}
	}

	// Token: 0x06003E7C RID: 15996 RVA: 0x00150B40 File Offset: 0x0014ED40
	public static void GetMothershipFailure(MothershipError callError, int errorCode)
	{
		Debug.LogError("Progression: GetMothershipFailure: " + callError.MothershipErrorCode + ":" + callError.Message);
	}

	// Token: 0x04004E58 RID: 20056
	private readonly Dictionary<string, UserHydratedProgressionTreeResponse> _trees = new Dictionary<string, UserHydratedProgressionTreeResponse>();

	// Token: 0x04004E59 RID: 20057
	private readonly Dictionary<string, ProgressionManager.MothershipItemSummary> _inventory = new Dictionary<string, ProgressionManager.MothershipItemSummary>();

	// Token: 0x04004E5A RID: 20058
	private readonly Dictionary<string, int> _tracks = new Dictionary<string, int>();

	// Token: 0x04004E5B RID: 20059
	private Dictionary<ProgressionManager.RequestType, int> retryCounters = new Dictionary<ProgressionManager.RequestType, int>();

	// Token: 0x04004E5C RID: 20060
	private int maxRetriesOnFail = 4;

	// Token: 0x04004E5D RID: 20061
	private const double k_minRefreshIntervalSeconds = 2.0;

	// Token: 0x04004E5E RID: 20062
	private double _lastTreeRefreshTime = double.NegativeInfinity;

	// Token: 0x04004E5F RID: 20063
	private double _lastInventoryRefreshTime = double.NegativeInfinity;

	// Token: 0x04004E60 RID: 20064
	private bool _treeRefreshInFlight;

	// Token: 0x04004E61 RID: 20065
	private bool _inventoryRefreshInFlight;

	// Token: 0x04004E62 RID: 20066
	public static int debug_refreshTreeCount;

	// Token: 0x04004E63 RID: 20067
	public static int debug_refreshInventoryCount;

	// Token: 0x04004E64 RID: 20068
	public static int debug_refreshTreeDroppedByThrottle;

	// Token: 0x04004E65 RID: 20069
	public static int debug_refreshInventoryDroppedByThrottle;

	// Token: 0x04004E66 RID: 20070
	public static double debug_lastRefreshTreeAttemptTime;

	// Token: 0x04004E67 RID: 20071
	public static double debug_lastRefreshInventoryAttemptTime;

	// Token: 0x02000941 RID: 2369
	public struct MothershipItemSummary
	{
		// Token: 0x04004E68 RID: 20072
		public string Name;

		// Token: 0x04004E69 RID: 20073
		public string EntitlementId;

		// Token: 0x04004E6A RID: 20074
		public string InGameId;

		// Token: 0x04004E6B RID: 20075
		public int Quantity;
	}

	// Token: 0x02000942 RID: 2370
	private enum RequestType
	{
		// Token: 0x04004E6D RID: 20077
		GetProgression,
		// Token: 0x04004E6E RID: 20078
		SetProgression,
		// Token: 0x04004E6F RID: 20079
		UnlockProgressionTreeNode,
		// Token: 0x04004E70 RID: 20080
		IncrementSIResource,
		// Token: 0x04004E71 RID: 20081
		CompleteSIQuest,
		// Token: 0x04004E72 RID: 20082
		CompleteSIBonus,
		// Token: 0x04004E73 RID: 20083
		CollectSIIdol,
		// Token: 0x04004E74 RID: 20084
		GetActiveSIQuests,
		// Token: 0x04004E75 RID: 20085
		GetSIQuestsStatus,
		// Token: 0x04004E76 RID: 20086
		ResetSIQuestsStatus,
		// Token: 0x04004E77 RID: 20087
		PurchaseTechPoints,
		// Token: 0x04004E78 RID: 20088
		PurchaseResources,
		// Token: 0x04004E79 RID: 20089
		PurchaseShiftCreditCapIncrease,
		// Token: 0x04004E7A RID: 20090
		PurchaseShiftCredit,
		// Token: 0x04004E7B RID: 20091
		RegisterToGRShift,
		// Token: 0x04004E7C RID: 20092
		GetJuicerStatus,
		// Token: 0x04004E7D RID: 20093
		DepositCore,
		// Token: 0x04004E7E RID: 20094
		PurchaseOverdrive,
		// Token: 0x04004E7F RID: 20095
		GetShiftCredit,
		// Token: 0x04004E80 RID: 20096
		SubtractShiftCredit,
		// Token: 0x04004E81 RID: 20097
		AdvanceDockWristUpgrade,
		// Token: 0x04004E82 RID: 20098
		GetDockWristUpgradeStatus,
		// Token: 0x04004E83 RID: 20099
		PurchaseDrillUpgrade,
		// Token: 0x04004E84 RID: 20100
		RecycleTool,
		// Token: 0x04004E85 RID: 20101
		StartOfShift,
		// Token: 0x04004E86 RID: 20102
		EndOfShiftReward,
		// Token: 0x04004E87 RID: 20103
		GetGhostReactorStats,
		// Token: 0x04004E88 RID: 20104
		GetGhostReactorInventory,
		// Token: 0x04004E89 RID: 20105
		SetGhostReactorInventory
	}

	// Token: 0x02000943 RID: 2371
	public enum WristDockUpgradeType
	{
		// Token: 0x04004E8B RID: 20107
		None,
		// Token: 0x04004E8C RID: 20108
		Upgrade1,
		// Token: 0x04004E8D RID: 20109
		Upgrade2,
		// Token: 0x04004E8E RID: 20110
		Upgrade3
	}

	// Token: 0x02000944 RID: 2372
	public enum DrillUpgradeLevel
	{
		// Token: 0x04004E90 RID: 20112
		None,
		// Token: 0x04004E91 RID: 20113
		Base,
		// Token: 0x04004E92 RID: 20114
		Upgrade1,
		// Token: 0x04004E93 RID: 20115
		Upgrade2,
		// Token: 0x04004E94 RID: 20116
		Upgrade3
	}

	// Token: 0x02000945 RID: 2373
	public enum CoreType
	{
		// Token: 0x04004E96 RID: 20118
		None,
		// Token: 0x04004E97 RID: 20119
		Core,
		// Token: 0x04004E98 RID: 20120
		SuperCore,
		// Token: 0x04004E99 RID: 20121
		ChaosSeed
	}

	// Token: 0x02000946 RID: 2374
	[Serializable]
	private class GetProgressionRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004E9A RID: 20122
		public string TrackId;
	}

	// Token: 0x02000947 RID: 2375
	[Serializable]
	private class GetProgressionResponse
	{
		// Token: 0x04004E9B RID: 20123
		public string Track;

		// Token: 0x04004E9C RID: 20124
		public int Progress;

		// Token: 0x04004E9D RID: 20125
		public int StatusCode;

		// Token: 0x04004E9E RID: 20126
		public string Error;
	}

	// Token: 0x02000948 RID: 2376
	[Serializable]
	private class SetProgressionRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004E9F RID: 20127
		public string TrackId;

		// Token: 0x04004EA0 RID: 20128
		public int Progress;
	}

	// Token: 0x02000949 RID: 2377
	[Serializable]
	private class SetProgressionResponse
	{
		// Token: 0x04004EA1 RID: 20129
		public string Track;

		// Token: 0x04004EA2 RID: 20130
		public int Progress;

		// Token: 0x04004EA3 RID: 20131
		public int StatusCode;

		// Token: 0x04004EA4 RID: 20132
		public string Error;
	}

	// Token: 0x0200094A RID: 2378
	[Serializable]
	private class UnlockNodeRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EA5 RID: 20133
		public string TreeId;

		// Token: 0x04004EA6 RID: 20134
		public string NodeId;
	}

	// Token: 0x0200094B RID: 2379
	[Serializable]
	private class UnlockNodeResponse
	{
		// Token: 0x04004EA7 RID: 20135
		public UserHydratedProgressionTreeResponse Tree;

		// Token: 0x04004EA8 RID: 20136
		public int StatusCode;

		// Token: 0x04004EA9 RID: 20137
		public string Error;
	}

	// Token: 0x0200094C RID: 2380
	[Serializable]
	private class IncrementSIResourceRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EAA RID: 20138
		public string ResourceType;
	}

	// Token: 0x0200094D RID: 2381
	[Serializable]
	private class IncrementSIResourceResponse : ProgressionManager.UserInventoryResponse
	{
		// Token: 0x04004EAB RID: 20139
		public string ResourceType;
	}

	// Token: 0x0200094E RID: 2382
	[Serializable]
	private class GetActiveSIQuestsRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200094F RID: 2383
	[Serializable]
	private class GetActiveSIQuestsResponse
	{
		// Token: 0x04004EAC RID: 20140
		public ProgressionManager.GetActiveSIQuestsResult Result;

		// Token: 0x04004EAD RID: 20141
		public int StatusCode;

		// Token: 0x04004EAE RID: 20142
		public string Error;
	}

	// Token: 0x02000950 RID: 2384
	[Serializable]
	public class GetActiveSIQuestsResult
	{
		// Token: 0x04004EAF RID: 20143
		public List<RotatingQuest> Quests;
	}

	// Token: 0x02000951 RID: 2385
	[Serializable]
	private class GetSIQuestsStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000952 RID: 2386
	[Serializable]
	private class ResetSIQuestsStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000953 RID: 2387
	[Serializable]
	private class PurchaseTechPointsRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EB0 RID: 20144
		public int TechPointsAmount;
	}

	// Token: 0x02000954 RID: 2388
	private class PurchaseResourcesRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000955 RID: 2389
	[Serializable]
	private class GetSIQuestsStatusResponse
	{
		// Token: 0x04004EB1 RID: 20145
		public ProgressionManager.UserQuestsStatusResponse Result;
	}

	// Token: 0x02000956 RID: 2390
	[Serializable]
	private class UserInventoryResponse
	{
		// Token: 0x04004EB2 RID: 20146
		public ProgressionManager.UserInventory Result;
	}

	// Token: 0x02000957 RID: 2391
	[Serializable]
	public class UserInventory
	{
		// Token: 0x04004EB3 RID: 20147
		public Dictionary<string, int> Inventory;
	}

	// Token: 0x02000958 RID: 2392
	[Serializable]
	private class SetSIQuestCompleteRequest : ProgressionManager.RewardRequest
	{
		// Token: 0x04004EB4 RID: 20148
		public int QuestID;
	}

	// Token: 0x02000959 RID: 2393
	[Serializable]
	private class SetSIBonusCompleteRequest : ProgressionManager.RewardRequest
	{
	}

	// Token: 0x0200095A RID: 2394
	[Serializable]
	private class SetSIIdolCollectRequest : ProgressionManager.RewardRequest
	{
	}

	// Token: 0x0200095B RID: 2395
	[Serializable]
	private class RewardRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200095C RID: 2396
	[Serializable]
	private class MothershipRequest
	{
		// Token: 0x04004EB5 RID: 20149
		public string MothershipId;

		// Token: 0x04004EB6 RID: 20150
		public string MothershipToken;

		// Token: 0x04004EB7 RID: 20151
		public string MothershipEnvId;

		// Token: 0x04004EB8 RID: 20152
		public string MothershipDeploymentId;
	}

	// Token: 0x0200095D RID: 2397
	[Serializable]
	private class MothershipUserDataWriteRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EB9 RID: 20153
		public bool SkipUserDataCache;
	}

	// Token: 0x0200095E RID: 2398
	[Serializable]
	public class UserQuestsStatusResponse
	{
		// Token: 0x04004EBA RID: 20154
		public int TodayClaimableQuests;

		// Token: 0x04004EBB RID: 20155
		public int TodayClaimableBonus;

		// Token: 0x04004EBC RID: 20156
		public int TodayClaimableIdol;
	}

	// Token: 0x0200095F RID: 2399
	[Serializable]
	private class PurchaseShiftCreditCapIncreaseRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000960 RID: 2400
	[Serializable]
	private class PurchaseShiftCreditCapIncreaseResponse
	{
		// Token: 0x04004EBD RID: 20157
		public int StatusCode;

		// Token: 0x04004EBE RID: 20158
		public string Error;

		// Token: 0x04004EBF RID: 20159
		public int CurrentShiftCreditCapIncreases;

		// Token: 0x04004EC0 RID: 20160
		public int CurrentShiftCreditCapIncreasesMax;

		// Token: 0x04004EC1 RID: 20161
		public string TargetMothershipId;
	}

	// Token: 0x02000961 RID: 2401
	[Serializable]
	private class PurchaseShiftCreditRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000962 RID: 2402
	[Serializable]
	private class PurchaseShiftCreditResponse
	{
		// Token: 0x04004EC2 RID: 20162
		public int StatusCode;

		// Token: 0x04004EC3 RID: 20163
		public string Error;

		// Token: 0x04004EC4 RID: 20164
		public int CurrentShiftCredits;

		// Token: 0x04004EC5 RID: 20165
		public string TargetMothershipId;
	}

	// Token: 0x02000963 RID: 2403
	[Serializable]
	private class GetShiftCreditRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EC6 RID: 20166
		public string TargetMothershipId;
	}

	// Token: 0x02000964 RID: 2404
	[Serializable]
	public class ShiftCreditResponse
	{
		// Token: 0x04004EC7 RID: 20167
		public int StatusCode;

		// Token: 0x04004EC8 RID: 20168
		public string Error;

		// Token: 0x04004EC9 RID: 20169
		public int CurrentShiftCredits;

		// Token: 0x04004ECA RID: 20170
		public int CurrentShiftCreditCapIncreases;

		// Token: 0x04004ECB RID: 20171
		public int CurrentShiftCreditCapIncreasesMax;

		// Token: 0x04004ECC RID: 20172
		public string TargetMothershipId;
	}

	// Token: 0x02000965 RID: 2405
	[Serializable]
	private class GetJuicerStatusRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000966 RID: 2406
	[Serializable]
	private class DepositCoreRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004ECD RID: 20173
		public ProgressionManager.CoreType CoreBeingDeposited;
	}

	// Token: 0x02000967 RID: 2407
	[Serializable]
	private class DepositCoreResponse
	{
		// Token: 0x04004ECE RID: 20174
		public int StatusCode;

		// Token: 0x04004ECF RID: 20175
		public string Error;

		// Token: 0x04004ED0 RID: 20176
		public int CurrentShiftCredits;
	}

	// Token: 0x02000968 RID: 2408
	[Serializable]
	private class PurchaseOverdriveRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000969 RID: 2409
	[Serializable]
	public class JuicerStatusResponse
	{
		// Token: 0x04004ED1 RID: 20177
		public string MothershipId;

		// Token: 0x04004ED2 RID: 20178
		public int StatusCode;

		// Token: 0x04004ED3 RID: 20179
		public string Error;

		// Token: 0x04004ED4 RID: 20180
		public int CurrentCoreCount;

		// Token: 0x04004ED5 RID: 20181
		public int CoreProcessingTimeSec;

		// Token: 0x04004ED6 RID: 20182
		public float CoreProcessingPercent;

		// Token: 0x04004ED7 RID: 20183
		public int OverdriveSupply;

		// Token: 0x04004ED8 RID: 20184
		public int OverdriveCap;

		// Token: 0x04004ED9 RID: 20185
		public int CoresProcessedByOverdrive;

		// Token: 0x04004EDA RID: 20186
		public bool RefreshJuice;
	}

	// Token: 0x0200096A RID: 2410
	[Serializable]
	private class SubtractShiftCreditRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004EDB RID: 20187
		public int ShiftCreditToRemove;
	}

	// Token: 0x0200096B RID: 2411
	[Serializable]
	private class AdvanceDockWristUpgradeRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004EDC RID: 20188
		public ProgressionManager.WristDockUpgradeType Upgrade;
	}

	// Token: 0x0200096C RID: 2412
	[Serializable]
	private class DockWristUpgradeStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200096D RID: 2413
	[Serializable]
	public class DockWristStatusResponse
	{
		// Token: 0x04004EDD RID: 20189
		public int CurrentUpgrade1Level;

		// Token: 0x04004EDE RID: 20190
		public int CurrentUpgrade2Level;

		// Token: 0x04004EDF RID: 20191
		public int CurrentUpgrade3Level;

		// Token: 0x04004EE0 RID: 20192
		public int Upgrade1LevelMax;

		// Token: 0x04004EE1 RID: 20193
		public int Upgrade2LevelMax;

		// Token: 0x04004EE2 RID: 20194
		public int Upgrade3LevelMax;
	}

	// Token: 0x0200096E RID: 2414
	[Serializable]
	private class PurchaseDrillUpgradeRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EE3 RID: 20195
		public ProgressionManager.DrillUpgradeLevel Upgrade;
	}

	// Token: 0x0200096F RID: 2415
	[Serializable]
	private class PurchaseDrillUpgradeResponse
	{
		// Token: 0x04004EE4 RID: 20196
		public int StatusCode;

		// Token: 0x04004EE5 RID: 20197
		public string Error;
	}

	// Token: 0x02000970 RID: 2416
	[Serializable]
	private class RecycleToolRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EE6 RID: 20198
		public GRTool.GRToolType ToolBeingRecycled;

		// Token: 0x04004EE7 RID: 20199
		public int NumberOfPlayers;
	}

	// Token: 0x02000971 RID: 2417
	[Serializable]
	private class StartOfShiftRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004EE8 RID: 20200
		public string ShiftId;

		// Token: 0x04004EE9 RID: 20201
		public int CoresRequired;

		// Token: 0x04004EEA RID: 20202
		public int NumberOfPlayers;

		// Token: 0x04004EEB RID: 20203
		public int Depth;
	}

	// Token: 0x02000972 RID: 2418
	[Serializable]
	private class EndOfShiftRewardRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004EEC RID: 20204
		public string ShiftId;
	}

	// Token: 0x02000973 RID: 2419
	[Serializable]
	private class GhostReactorStatsRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000974 RID: 2420
	[Serializable]
	public class GhostReactorStatsResponse
	{
		// Token: 0x04004EED RID: 20205
		public string MothershipId;

		// Token: 0x04004EEE RID: 20206
		public int MaxDepthReached;
	}

	// Token: 0x02000975 RID: 2421
	[Serializable]
	private class GhostReactorInventoryRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000976 RID: 2422
	[Serializable]
	public class GhostReactorInventoryResponse
	{
		// Token: 0x04004EEF RID: 20207
		public string MothershipId;

		// Token: 0x04004EF0 RID: 20208
		public string InventoryJson;
	}

	// Token: 0x02000977 RID: 2423
	[Serializable]
	private class SetGhostReactorInventoryRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004EF1 RID: 20209
		public string InventoryJson;
	}

	// Token: 0x02000978 RID: 2424
	[Serializable]
	public class SetGhostReactorInventoryResponse
	{
		// Token: 0x04004EF2 RID: 20210
		public string MothershipId;
	}
}
