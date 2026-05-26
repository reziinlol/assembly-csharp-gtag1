using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000925 RID: 2341
public class RankedProgressionManager : MonoBehaviour
{
	// Token: 0x17000582 RID: 1410
	// (get) Token: 0x06003D32 RID: 15666 RVA: 0x0014C7F6 File Offset: 0x0014A9F6
	// (set) Token: 0x06003D33 RID: 15667 RVA: 0x0014C7FE File Offset: 0x0014A9FE
	public int MaxRank { get; private set; }

	// Token: 0x17000583 RID: 1411
	// (get) Token: 0x06003D34 RID: 15668 RVA: 0x0014C807 File Offset: 0x0014AA07
	// (set) Token: 0x06003D35 RID: 15669 RVA: 0x0014C80F File Offset: 0x0014AA0F
	public float LowTierThreshold { get; set; }

	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x06003D36 RID: 15670 RVA: 0x0014C818 File Offset: 0x0014AA18
	// (set) Token: 0x06003D37 RID: 15671 RVA: 0x0014C820 File Offset: 0x0014AA20
	public float HighTierThreshold { get; set; }

	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x06003D38 RID: 15672 RVA: 0x0014C829 File Offset: 0x0014AA29
	// (set) Token: 0x06003D39 RID: 15673 RVA: 0x000028C5 File Offset: 0x00000AC5
	public List<RankedProgressionManager.RankedProgressionTier> MajorTiers
	{
		get
		{
			return this.majorTiers;
		}
		private set
		{
		}
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void DebugSetELO()
	{
	}

	// Token: 0x06003D3B RID: 15675 RVA: 0x000028C5 File Offset: 0x00000AC5
	[ContextMenu("Reset ELO")]
	private void DebugResetELO()
	{
	}

	// Token: 0x06003D3C RID: 15676 RVA: 0x0014C831 File Offset: 0x0014AA31
	private void Awake()
	{
		if (RankedProgressionManager.Instance)
		{
			GTDev.LogError<string>("Duplicate RankedProgressionManager detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		RankedProgressionManager.Instance = this;
	}

	// Token: 0x06003D3D RID: 15677 RVA: 0x0014C860 File Offset: 0x0014AA60
	private void Start()
	{
		if (this.majorTiers.Count < 3)
		{
			GTDev.LogWarning<string>("At least 3 MMR tiers must be defined.", null);
			return;
		}
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
		float minThreshold = 100f;
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			this.majorTiers[i].SetMinThreshold((i == 0) ? 100f : this.majorTiers[i - 1].thresholdMax);
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				num++;
				this.majorTiers[i].subTiers[j].SetMinThreshold(minThreshold);
				minThreshold = this.majorTiers[i].subTiers[j].thresholdMax;
			}
		}
		this.MaxRank = num - 1;
		this.LowTierThreshold = this.majorTiers[0].thresholdMax;
		List<RankedProgressionManager.RankedProgressionTier> list = this.majorTiers;
		this.HighTierThreshold = list[list.Count - 1].GetMinThreshold();
		this.EloScorePC = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_PC_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.EloScoreQuest = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxPC = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxQuest = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
	}

	// Token: 0x06003D3E RID: 15678 RVA: 0x0014CA12 File Offset: 0x0014AC12
	private void OnDestroy()
	{
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoined);
	}

	// Token: 0x06003D3F RID: 15679 RVA: 0x0014CA40 File Offset: 0x0014AC40
	public void RequestUnlockCompetitiveQueue(bool unlock)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestUnlockCompetitiveQueue(unlock, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x06003D40 RID: 15680 RVA: 0x0014CA59 File Offset: 0x0014AC59
	public IEnumerator LoadStatsWhenReady()
	{
		yield return new WaitUntil(() => NetworkSystem.Instance.LocalPlayer.UserId != null);
		if (this.HasUnlockedCompetitiveQueue())
		{
			this.RequestUnlockCompetitiveQueue(true);
		}
		else
		{
			this.AcquireLocalPlayerRankInformation();
		}
		yield break;
	}

	// Token: 0x06003D41 RID: 15681 RVA: 0x0014CA68 File Offset: 0x0014AC68
	private void OnJoinedRoom(GameModeType newGameModeType)
	{
		if (newGameModeType == GameModeType.InfectionCompetitive)
		{
			this.AcquireRoomRankInformation(false);
		}
	}

	// Token: 0x06003D42 RID: 15682 RVA: 0x0014CA76 File Offset: 0x0014AC76
	private void OnPlayerJoined(NetPlayer player)
	{
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.InfectionCompetitive)
		{
			this.AcquireSinglePlayerRankInformation(player);
		}
	}

	// Token: 0x06003D43 RID: 15683 RVA: 0x0014CA9C File Offset: 0x0014AC9C
	private void AcquireLocalPlayerRankInformation()
	{
		List<string> list = new List<string>();
		list.Add(NetworkSystem.Instance.LocalPlayer.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnLocalPlayerRankedInformationAcquired));
	}

	// Token: 0x06003D44 RID: 15684 RVA: 0x0014CADC File Offset: 0x0014ACDC
	private void AcquireSinglePlayerRankInformation(NetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add(player.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
	}

	// Token: 0x06003D45 RID: 15685 RVA: 0x0014CB18 File Offset: 0x0014AD18
	public void AcquireRoomRankInformation(bool includeLocalPlayer = true)
	{
		List<string> list = new List<string>();
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (includeLocalPlayer || !netPlayer.IsLocal)
			{
				list.Add(netPlayer.UserId);
			}
		}
		if (list.Count > 0)
		{
			GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
		}
	}

	// Token: 0x06003D46 RID: 15686 RVA: 0x0014CBA0 File Offset: 0x0014ADA0
	private void OnPlayersRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		foreach (GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData rankedModePlayerProgressionData in rankedModeProgressionData.playerData)
		{
			if (rankedModePlayerProgressionData != null && rankedModePlayerProgressionData.platformData != null && rankedModePlayerProgressionData.platformData.Length >= 2)
			{
				int num = -1;
				foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
				{
					if (netPlayer.UserId == rankedModePlayerProgressionData.playfabID)
					{
						num = netPlayer.ActorNumber;
						break;
					}
				}
				if (num >= 0)
				{
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = rankedModePlayerProgressionData.platformData[1];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData2 = rankedModePlayerProgressionData.platformData[0];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData3 = rankedModeProgressionPlatformData2;
					int rankFromTiers = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData3.majorTier, rankedModeProgressionPlatformData3.minorTier);
					Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
					if (onPlayerEloAcquired != null)
					{
						onPlayerEloAcquired(num, rankedModeProgressionPlatformData3.elo, rankFromTiers);
					}
					if (num == NetworkSystem.Instance.LocalPlayerID)
					{
						this.SetLocalProgressionData(rankedModePlayerProgressionData);
					}
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
					{
						VRRig rig = rigContainer.Rig;
						if (rig != null)
						{
							int rankFromTiers2 = this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
							int rankFromTiers3 = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData2.majorTier, rankedModeProgressionPlatformData2.minorTier);
							rig.SetRankedInfo(rankedModeProgressionPlatformData3.elo, rankFromTiers2, rankFromTiers3, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x06003D47 RID: 15687 RVA: 0x0014CD30 File Offset: 0x0014AF30
	private void OnLocalPlayerRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		if (rankedModeProgressionData.playerData.Count > 0)
		{
			this.SetLocalProgressionData(rankedModeProgressionData.playerData[0]);
			float eloScore = this.GetEloScore();
			int progressionRankIndexQuest = this.GetProgressionRankIndexQuest();
			int progressionRankIndexPC = this.GetProgressionRankIndexPC();
			int tier = progressionRankIndexPC;
			this.HandlePlayerRankedInfoReceived(NetworkSystem.Instance.LocalPlayer.ActorNumber, eloScore, tier);
			VRRig.LocalRig.SetRankedInfo(eloScore, progressionRankIndexQuest, progressionRankIndexPC, true);
		}
	}

	// Token: 0x06003D48 RID: 15688 RVA: 0x0014CD9B File Offset: 0x0014AF9B
	public bool AreValuesValid(float elo, int questTier, int pcTier)
	{
		return elo >= 100f && elo <= 4000f && questTier >= 0 && questTier <= this.MaxRank && pcTier >= 0 && pcTier <= this.MaxRank;
	}

	// Token: 0x06003D49 RID: 15689 RVA: 0x0014CDCA File Offset: 0x0014AFCA
	public void HandlePlayerRankedInfoReceived(int actorNum, float elo, int tier)
	{
		Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
		if (onPlayerEloAcquired == null)
		{
			return;
		}
		onPlayerEloAcquired(actorNum, elo, tier);
	}

	// Token: 0x06003D4A RID: 15690 RVA: 0x0014CDDF File Offset: 0x0014AFDF
	public void SetLocalProgressionData(GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData data)
	{
		this.ProgressionData = data;
	}

	// Token: 0x06003D4B RID: 15691 RVA: 0x0014CDE8 File Offset: 0x0014AFE8
	public void LoadStats()
	{
		base.StartCoroutine(this.LoadStatsWhenReady());
	}

	// Token: 0x06003D4C RID: 15692 RVA: 0x0014CDF7 File Offset: 0x0014AFF7
	public float GetEloScore()
	{
		return this.GetEloScorePC();
	}

	// Token: 0x06003D4D RID: 15693 RVA: 0x0014CDFF File Offset: 0x0014AFFF
	public void SetEloScore(float val)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestSetEloValue(val, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x06003D4E RID: 15694 RVA: 0x0014CE18 File Offset: 0x0014B018
	public float GetEloScorePC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[0].elo;
	}

	// Token: 0x06003D4F RID: 15695 RVA: 0x0014CE57 File Offset: 0x0014B057
	public float GetEloScoreQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[1].elo;
	}

	// Token: 0x06003D50 RID: 15696 RVA: 0x0014CE96 File Offset: 0x0014B096
	private int GetNewTierGracePeriodIdx()
	{
		return this.NewTierGracePeriodIdxPC;
	}

	// Token: 0x06003D51 RID: 15697 RVA: 0x0014CEA3 File Offset: 0x0014B0A3
	private void SetNewTierGracePeriodIdx(int val)
	{
		this.NewTierGracePeriodIdxPC.Set(val);
	}

	// Token: 0x06003D52 RID: 15698 RVA: 0x0014CEB1 File Offset: 0x0014B0B1
	private void IncrementNewTierGracePeriodIdx()
	{
		this.NewTierGracePeriodIdxPC.Increment();
	}

	// Token: 0x06003D53 RID: 15699 RVA: 0x0014CEBE File Offset: 0x0014B0BE
	public bool TryGetProgressionSubTier(out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		subTier = null;
		index = -1;
		return this.TryGetProgressionSubTier(this.GetEloScore(), out subTier, out index);
	}

	// Token: 0x06003D54 RID: 15700 RVA: 0x0014CED4 File Offset: 0x0014B0D4
	public bool TryGetProgressionSubTier(float elo, out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		int num = 0;
		subTier = null;
		index = -1;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			float num2 = (i < this.majorTiers.Count - 1) ? this.majorTiers[i].thresholdMax : 4000.1f;
			if (elo < num2)
			{
				int j = 0;
				while (j < this.majorTiers[i].subTiers.Count)
				{
					float num3 = (j < this.majorTiers[i].subTiers.Count - 1) ? this.majorTiers[i].subTiers[j].thresholdMax : num2;
					if (elo < num3)
					{
						subTier = this.majorTiers[i].subTiers[j];
						index = num;
						return true;
					}
					j++;
					num++;
				}
			}
			else
			{
				num += this.majorTiers[i].subTiers.Count;
			}
		}
		return false;
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x0014CFD8 File Offset: 0x0014B1D8
	private RankedProgressionManager.RankedProgressionTier GetProgressionMajorTierBySubTierIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x0014D034 File Offset: 0x0014B234
	private RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTierByIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i].subTiers[j];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x0014D09C File Offset: 0x0014B29C
	private RankedProgressionManager.RankedProgressionSubTier GetNextProgressionSubTierByIndex(int idx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx + 1);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex;
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x0014D0C0 File Offset: 0x0014B2C0
	private RankedProgressionManager.RankedProgressionSubTier GetPrevProgressionSubTierByIndex(int idx)
	{
		if (idx > 0)
		{
			RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx - 1);
			if (progressionSubTierByIndex != null)
			{
				return progressionSubTierByIndex;
			}
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x0014D0E7 File Offset: 0x0014B2E7
	public string GetProgressionRankName()
	{
		return this.GetProgressionRankName(this.GetEloScore());
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x0014D0F8 File Offset: 0x0014B2F8
	public string GetProgressionRankName(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.name;
		}
		return string.Empty;
	}

	// Token: 0x06003D5B RID: 15707 RVA: 0x0014D120 File Offset: 0x0014B320
	public string GetNextProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x0014D140 File Offset: 0x0014B340
	public string GetPrevProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x0014D160 File Offset: 0x0014B360
	public int GetProgressionRankIndex()
	{
		return this.GetProgressionRankIndexPC();
	}

	// Token: 0x06003D5E RID: 15710 RVA: 0x0014D168 File Offset: 0x0014B368
	public RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTier()
	{
		return this.GetProgressionSubTierByIndex(this.GetProgressionRankIndex());
	}

	// Token: 0x06003D5F RID: 15711 RVA: 0x0014D178 File Offset: 0x0014B378
	public int GetProgressionRankIndexQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[1];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x06003D60 RID: 15712 RVA: 0x0014D1CC File Offset: 0x0014B3CC
	public int GetProgressionRankIndexPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[0];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x06003D61 RID: 15713 RVA: 0x0014D220 File Offset: 0x0014B420
	public int GetRankFromTiers(int majorTier, int minorTier)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				if (i == majorTier && j == minorTier)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x06003D62 RID: 15714 RVA: 0x0014D278 File Offset: 0x0014B478
	public int GetProgressionRankIndex(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int result;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x06003D63 RID: 15715 RVA: 0x0014D295 File Offset: 0x0014B495
	public float GetProgressionRankProgress()
	{
		return this.GetProgressionRankProgressPC();
	}

	// Token: 0x06003D64 RID: 15716 RVA: 0x0014D29D File Offset: 0x0014B49D
	public float GetProgressionRankProgressQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[1].rankProgress;
	}

	// Token: 0x06003D65 RID: 15717 RVA: 0x0014D2DC File Offset: 0x0014B4DC
	public float GetProgressionRankProgressPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[0].rankProgress;
	}

	// Token: 0x06003D66 RID: 15718 RVA: 0x0014D31C File Offset: 0x0014B51C
	public int ClampProgressionRankIndex(int subTierIdx)
	{
		if (subTierIdx < 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == subTierIdx)
				{
					return subTierIdx;
				}
				j++;
				num++;
			}
		}
		return num - 1;
	}

	// Token: 0x06003D67 RID: 15719 RVA: 0x0014D378 File Offset: 0x0014B578
	public Sprite GetProgressionRankIcon()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return null;
		}
		int index = (this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].minorTier;
		int index2 = (this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[index2].subTiers[index];
		if (rankedProgressionSubTier == null)
		{
			return null;
		}
		return rankedProgressionSubTier.icon;
	}

	// Token: 0x06003D68 RID: 15720 RVA: 0x0014D410 File Offset: 0x0014B610
	public string GetRankedProgressionTierName()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return "None";
		}
		int minorTier = this.ProgressionData.platformData[0].minorTier;
		int majorTier = this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[majorTier].subTiers[minorTier];
		if (rankedProgressionSubTier != null)
		{
			return rankedProgressionSubTier.name;
		}
		return "None";
	}

	// Token: 0x06003D69 RID: 15721 RVA: 0x0014D49C File Offset: 0x0014B69C
	public Sprite GetProgressionRankIcon(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.icon;
		}
		return null;
	}

	// Token: 0x06003D6A RID: 15722 RVA: 0x0014D4C0 File Offset: 0x0014B6C0
	public Sprite GetProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x0014D4E0 File Offset: 0x0014B6E0
	public Sprite GetNextProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x0014D500 File Offset: 0x0014B700
	public Sprite GetPrevProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x06003D6D RID: 15725 RVA: 0x0014D520 File Offset: 0x0014B720
	public float GetCurrentELO()
	{
		return this.GetEloScore();
	}

	// Token: 0x06003D6E RID: 15726 RVA: 0x0014D528 File Offset: 0x0014B728
	public void GetSubtierRankThresholds(int subTierIdx, out float minThreshold, out float maxThreshold)
	{
		minThreshold = 0f;
		maxThreshold = 1f;
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			maxThreshold = progressionSubTierByIndex.thresholdMax;
			if (maxThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex != null)
				{
					maxThreshold = progressionMajorTierBySubTierIndex.thresholdMax;
					if (maxThreshold <= 0f)
					{
						maxThreshold = 4000f;
					}
				}
			}
			minThreshold = progressionSubTierByIndex.GetMinThreshold();
			if (minThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex2 = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex2 != null)
				{
					minThreshold = progressionMajorTierBySubTierIndex2.GetMinThreshold();
					if (minThreshold <= 0f)
					{
						minThreshold = 100f;
					}
				}
			}
		}
	}

	// Token: 0x06003D6F RID: 15727 RVA: 0x0014D5B6 File Offset: 0x0014B7B6
	public static float GetEloWinProbability(float ratingPlayer1, float ratingPlayer2)
	{
		return 1f / (1f + Mathf.Pow(10f, (ratingPlayer1 - ratingPlayer2) / 400f));
	}

	// Token: 0x06003D70 RID: 15728 RVA: 0x0014D5D7 File Offset: 0x0014B7D7
	public static float UpdateEloScore(float eloScore, float expectedResult, float actualResult, float k)
	{
		return Mathf.Clamp(eloScore + k * (actualResult - expectedResult), 100f, 4000f);
	}

	// Token: 0x06003D71 RID: 15729 RVA: 0x0014D5EF File Offset: 0x0014B7EF
	public RankedProgressionManager.ERankedMatchmakingTier GetRankedMatchmakingTier()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return RankedProgressionManager.ERankedMatchmakingTier.Low;
		}
		return (RankedProgressionManager.ERankedMatchmakingTier)this.ProgressionData.platformData[0].majorTier;
	}

	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x06003D72 RID: 15730 RVA: 0x0014D62A File Offset: 0x0014B82A
	public float CompetitiveQueueEloFloor
	{
		get
		{
			return this.LowTierThreshold;
		}
	}

	// Token: 0x06003D73 RID: 15731 RVA: 0x0014D632 File Offset: 0x0014B832
	private bool HasUnlockedCompetitiveQueue()
	{
		return GorillaComputer.instance.allowedInCompetitive;
	}

	// Token: 0x04004DB2 RID: 19890
	public static RankedProgressionManager Instance;

	// Token: 0x04004DB3 RID: 19891
	public const float DEFAULT_ELO = 100f;

	// Token: 0x04004DB4 RID: 19892
	public const float MIN_ELO = 100f;

	// Token: 0x04004DB5 RID: 19893
	public const float MAX_ELO = 4000f;

	// Token: 0x04004DB6 RID: 19894
	public const float MAJOR_TIER_MIN_RANGE = 200f;

	// Token: 0x04004DB7 RID: 19895
	public const float SUB_TIER_MIN_RANGE = 20f;

	// Token: 0x04004DB8 RID: 19896
	public static string RANKED_ELO_KEY = "RankedElo";

	// Token: 0x04004DB9 RID: 19897
	public static string RANKED_PROGRESSION_GRACE_PERIOD_KEY = "RankedProgGracePeriod";

	// Token: 0x04004DBA RID: 19898
	public static string RANKED_ELO_PC_KEY = "RankedEloPC";

	// Token: 0x04004DBB RID: 19899
	public static string RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY = "RankedProgGracePeriodPC";

	// Token: 0x04004DBC RID: 19900
	private RankedMultiplayerStatisticFloat EloScorePC;

	// Token: 0x04004DBD RID: 19901
	private RankedMultiplayerStatisticFloat EloScoreQuest;

	// Token: 0x04004DBE RID: 19902
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxPC;

	// Token: 0x04004DBF RID: 19903
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxQuest;

	// Token: 0x04004DC0 RID: 19904
	private GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData ProgressionData;

	// Token: 0x04004DC1 RID: 19905
	[SerializeField]
	private List<RankedProgressionManager.RankedProgressionTier> majorTiers = new List<RankedProgressionManager.RankedProgressionTier>();

	// Token: 0x04004DC2 RID: 19906
	[SerializeField]
	private int newTierGracePeriod = 3;

	// Token: 0x04004DC3 RID: 19907
	public float MaxEloConstant = 90f;

	// Token: 0x04004DC5 RID: 19909
	private RankedProgressionManager.RankedProgressionEvent ProgressionEvent;

	// Token: 0x04004DC6 RID: 19910
	public Action<int, float, int> OnPlayerEloAcquired;

	// Token: 0x04004DC9 RID: 19913
	[Space]
	[ContextMenuItem("Set ELO", "DebugSetELO")]
	public int debugEloPoints = 100;

	// Token: 0x02000926 RID: 2342
	public enum ERankedMatchmakingTier
	{
		// Token: 0x04004DCB RID: 19915
		Low,
		// Token: 0x04004DCC RID: 19916
		Medium,
		// Token: 0x04004DCD RID: 19917
		High
	}

	// Token: 0x02000927 RID: 2343
	public enum ERankedProgressionEventType
	{
		// Token: 0x04004DCF RID: 19919
		None,
		// Token: 0x04004DD0 RID: 19920
		Progress,
		// Token: 0x04004DD1 RID: 19921
		Promotion,
		// Token: 0x04004DD2 RID: 19922
		Relegation
	}

	// Token: 0x02000928 RID: 2344
	public class RankedProgressionEvent
	{
		// Token: 0x06003D78 RID: 15736 RVA: 0x0014D6A0 File Offset: 0x0014B8A0
		public override string ToString()
		{
			string text = "Progression Info\n";
			text += string.Format("Event Type: {0}\n", this.evtType.ToString());
			text += string.Format("Left Tier: {0}\n", this.leftName);
			text += string.Format("Right Tier: {0}\n", this.rightName);
			text += string.Format("Left Value: {0}\n", this.minVal.ToString("N0"));
			text += string.Format("Right Value: {0}\n", this.maxVal.ToString("N0"));
			text += string.Format("Elo Delta: {0}\n", this.delta.ToString("N0"));
			if (this.evtType == RankedProgressionManager.ERankedProgressionEventType.Promotion || this.evtType == RankedProgressionManager.ERankedProgressionEventType.Relegation)
			{
				text += string.Format("Fanfare Tier: {0}\n", this.newTierName);
			}
			return text;
		}

		// Token: 0x04004DD3 RID: 19923
		public RankedProgressionManager.ERankedProgressionEventType evtType;

		// Token: 0x04004DD4 RID: 19924
		public Sprite progressIconLeft;

		// Token: 0x04004DD5 RID: 19925
		public Sprite progressIconRight;

		// Token: 0x04004DD6 RID: 19926
		public Sprite newTierIcon;

		// Token: 0x04004DD7 RID: 19927
		public string leftName;

		// Token: 0x04004DD8 RID: 19928
		public string rightName;

		// Token: 0x04004DD9 RID: 19929
		public string newTierName;

		// Token: 0x04004DDA RID: 19930
		public float minVal;

		// Token: 0x04004DDB RID: 19931
		public float maxVal;

		// Token: 0x04004DDC RID: 19932
		public float delta;
	}

	// Token: 0x02000929 RID: 2345
	public abstract class RankedProgressionTierBase
	{
		// Token: 0x06003D7A RID: 15738 RVA: 0x0014D790 File Offset: 0x0014B990
		public void SetMinThreshold(float val)
		{
			this.thresholdMin = val;
		}

		// Token: 0x06003D7B RID: 15739 RVA: 0x0014D799 File Offset: 0x0014B999
		public float GetMinThreshold()
		{
			if (this.thresholdMin < 0f)
			{
				GTDev.LogError<string>("Tier min threshold not initialized. Can only be used at runtime.", null);
			}
			return this.thresholdMin;
		}

		// Token: 0x04004DDD RID: 19933
		public string name;

		// Token: 0x04004DDE RID: 19934
		public Color color = Color.white;

		// Token: 0x04004DDF RID: 19935
		public float thresholdMax;

		// Token: 0x04004DE0 RID: 19936
		private float thresholdMin = -1f;
	}

	// Token: 0x0200092A RID: 2346
	[Serializable]
	public class RankedProgressionSubTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x04004DE1 RID: 19937
		public Sprite icon;
	}

	// Token: 0x0200092B RID: 2347
	[Serializable]
	public class RankedProgressionTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x06003D7E RID: 15742 RVA: 0x0014D7E0 File Offset: 0x0014B9E0
		public void InsertSubTierAt(int idx, float tierMin)
		{
			RankedProgressionManager.RankedProgressionSubTier item = new RankedProgressionManager.RankedProgressionSubTier
			{
				name = "NewTier"
			};
			this.subTiers.Insert(idx, item);
			this.EnforceSubTierValidity(tierMin);
		}

		// Token: 0x06003D7F RID: 15743 RVA: 0x0014D814 File Offset: 0x0014BA14
		public void EnforceSubTierValidity(float thresholdMin)
		{
			float num = (((this.thresholdMax == 0f) ? 4000f : this.thresholdMax) - thresholdMin) / (float)this.subTiers.Count;
			for (int i = 0; i < this.subTiers.Count - 1; i++)
			{
				float num2 = thresholdMin + (float)(i + 1) * num;
				num2 = Mathf.Round(num2 / 10f);
				this.subTiers[i].thresholdMax = num2 * 10f;
			}
		}

		// Token: 0x04004DE2 RID: 19938
		public List<RankedProgressionManager.RankedProgressionSubTier> subTiers = new List<RankedProgressionManager.RankedProgressionSubTier>();
	}
}
