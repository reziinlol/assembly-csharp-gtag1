using System;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200091A RID: 2330
public class RankedMultiplayerScore : MonoBehaviourTick
{
	// Token: 0x1700057D RID: 1405
	// (get) Token: 0x06003CE4 RID: 15588 RVA: 0x0014B5FB File Offset: 0x001497FB
	// (set) Token: 0x06003CE5 RID: 15589 RVA: 0x0014B603 File Offset: 0x00149803
	public RankedProgressionManager Progression { get; private set; }

	// Token: 0x06003CE6 RID: 15590 RVA: 0x0014B60C File Offset: 0x0014980C
	public void Initialize()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.OnStateChanged;
		GorillaTagCompetitiveManager.onRoundStart += this.OnGameStarted;
		GorillaTagCompetitiveManager.onRoundEnd += this.OnGameEnded;
		GorillaTagCompetitiveManager.onPlayerJoined += this.OnPlayerJoined;
		GorillaTagCompetitiveManager.onPlayerLeft += this.OnPlayerLeft;
		GorillaTagCompetitiveManager.onTagOccurred += this.OnTagReported;
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null)
		{
			this.CompetitiveManager = (instance as GorillaTagCompetitiveManager);
		}
		this.Progression = RankedProgressionManager.Instance;
		RankedProgressionManager progression = this.Progression;
		progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Combine(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
	}

	// Token: 0x06003CE7 RID: 15591 RVA: 0x0014B6CC File Offset: 0x001498CC
	private void HandlePlayerEloAcquired(int playerId, float elo, int tier)
	{
		this.CachePlayerRankedProgressionData(playerId, tier, elo);
	}

	// Token: 0x06003CE8 RID: 15592 RVA: 0x0014B6D7 File Offset: 0x001498D7
	private void OnDestroy()
	{
		this.Unsubscribe();
	}

	// Token: 0x06003CE9 RID: 15593 RVA: 0x0014B6E0 File Offset: 0x001498E0
	public void Unsubscribe()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.OnStateChanged;
		GorillaTagCompetitiveManager.onRoundStart -= this.OnGameStarted;
		GorillaTagCompetitiveManager.onRoundEnd -= this.OnGameEnded;
		GorillaTagCompetitiveManager.onPlayerJoined -= this.OnPlayerJoined;
		GorillaTagCompetitiveManager.onPlayerLeft -= this.OnPlayerLeft;
		GorillaTagCompetitiveManager.onTagOccurred -= this.OnTagReported;
		if (this.Progression != null)
		{
			RankedProgressionManager progression = this.Progression;
			progression.OnPlayerEloAcquired = (Action<int, float, int>)Delegate.Remove(progression.OnPlayerEloAcquired, new Action<int, float, int>(this.HandlePlayerEloAcquired));
		}
	}

	// Token: 0x06003CEA RID: 15594 RVA: 0x0014B788 File Offset: 0x00149988
	public override void Tick()
	{
		if (this.PerSecondTimer > 0f && Time.time >= this.PerSecondTimer + 1f)
		{
			if (this.CompetitiveManager == null)
			{
				return;
			}
			this.OnPerSecondTimerElapsed(NetworkSystem.Instance.AllNetPlayers.Length, this.CompetitiveManager.currentInfected.Count);
			this.PerSecondTimer = Time.time;
		}
	}

	// Token: 0x06003CEB RID: 15595 RVA: 0x0014B7F4 File Offset: 0x001499F4
	private void OnPerSecondTimerElapsed(int playersInGame, int infectedPlayers)
	{
		foreach (int num in this.AllPlayerInRoundScores.Keys.ToList<int>())
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[num];
			playerScoreInRound.Infected = this.CompetitiveManager.IsInfected(NetworkSystem.Instance.GetPlayer(num));
			if (!playerScoreInRound.Infected)
			{
				float t = (float)infectedPlayers / (float)playersInGame;
				playerScoreInRound.PointsOnDefense += Mathf.Lerp(this.PointsPerUninfectedSecMin, this.PointsPerUninfectedSecMax, t);
			}
			this.AllPlayerInRoundScores[num] = playerScoreInRound;
		}
	}

	// Token: 0x06003CEC RID: 15596 RVA: 0x0014B8AC File Offset: 0x00149AAC
	public void ResetMatch()
	{
		this.AllFinalPlayerScores.Clear();
		this.AllPlayerInRoundScores.Clear();
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x0014B8C4 File Offset: 0x00149AC4
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState state)
	{
		if (state == GorillaTagCompetitiveManager.GameState.StartingCountdown)
		{
			this.OnGameStarted();
			this.Progression.AcquireRoomRankInformation(true);
		}
	}

	// Token: 0x06003CEE RID: 15598 RVA: 0x0014B8DC File Offset: 0x00149ADC
	public void OnGameStarted()
	{
		this.PerSecondTimer = Time.time;
		if (!this.IsLateJoiner)
		{
			this.ResetMatch();
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				this.StartTrackingPlayer(NetworkSystem.Instance.AllNetPlayers[i], false);
			}
		}
	}

	// Token: 0x06003CEF RID: 15599 RVA: 0x0014B92C File Offset: 0x00149B2C
	public void OnGameEnded()
	{
		foreach (int key in this.AllPlayerInRoundScores.Keys.ToList<int>())
		{
			RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = this.AllPlayerInRoundScores[key];
			if (!playerScoreInRound.Infected)
			{
				playerScoreInRound.TaggedTime = Time.time;
			}
			this.AllPlayerInRoundScores[key] = playerScoreInRound;
		}
		this.PerSecondTimer = -1f;
		this.ReportScore();
		this.WasInfectedInitially = false;
		this.IsLateJoiner = false;
	}

	// Token: 0x06003CF0 RID: 15600 RVA: 0x0014B9D0 File Offset: 0x00149BD0
	private void OnPlayerJoined(NetPlayer player)
	{
		if (NetworkSystem.Instance.IsMasterClient && this.CompetitiveManager.IsMatchActive())
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<float> list3 = new List<float>();
			List<float> list4 = new List<float>();
			List<bool> list5 = new List<bool>();
			List<float> list6 = new List<float>();
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				list.Add(keyValuePair.Value.PlayerId);
				list2.Add(keyValuePair.Value.NumTags);
				list3.Add(keyValuePair.Value.PointsOnDefense);
				list4.Add(Time.time - keyValuePair.Value.JoinTime);
				list5.Add(keyValuePair.Value.Infected);
				if (!keyValuePair.Value.Infected)
				{
					list6.Add(0f);
				}
				else
				{
					list6.Add(Time.time - keyValuePair.Value.TaggedTime);
				}
			}
			GameMode.ActiveNetworkHandler.SendRPC("SendScoresToLateJoinerRPC", player, new object[]
			{
				list.ToArray(),
				list2.ToArray(),
				list3.ToArray(),
				list4.ToArray(),
				list5.ToArray(),
				list6.ToArray()
			});
		}
		this.StartTrackingPlayer(player, true);
	}

	// Token: 0x06003CF1 RID: 15601 RVA: 0x0014BB58 File Offset: 0x00149D58
	public void ReceivedScoresForLateJoiner(int[] playerIds, int[] numTags, float[] pointsOnDefense, float[] joinTime, bool[] infected, float[] taggedTime)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			this.IsLateJoiner = true;
			for (int i = 0; i < playerIds.Length; i++)
			{
				int num = playerIds[i];
				RankedMultiplayerScore.PlayerScoreInRound value = new RankedMultiplayerScore.PlayerScoreInRound(num, infected[i]);
				value.NumTags = numTags[i];
				value.PointsOnDefense = pointsOnDefense[i];
				value.JoinTime = Time.time - joinTime[i];
				if (!infected[i])
				{
					value.TaggedTime = 0f;
				}
				else
				{
					value.TaggedTime = Time.time - taggedTime[i];
				}
				this.AllPlayerInRoundScores.TryAdd(num, value);
			}
		}
	}

	// Token: 0x06003CF2 RID: 15602 RVA: 0x0014BBEE File Offset: 0x00149DEE
	private void OnPlayerLeft(NetPlayer player)
	{
		this.AllPlayerInRoundScores.Remove(player.ActorNumber);
	}

	// Token: 0x06003CF3 RID: 15603 RVA: 0x0014BC04 File Offset: 0x00149E04
	private void StartTrackingPlayer(NetPlayer player, bool lateJoin)
	{
		bool initInfected = lateJoin;
		if (!lateJoin && this.CompetitiveManager != null)
		{
			initInfected = this.CompetitiveManager.IsInfected(player);
			if (player.ActorNumber == NetworkSystem.Instance.LocalPlayerID)
			{
				this.WasInfectedInitially = true;
			}
		}
		if (player == NetworkSystem.Instance.LocalPlayer)
		{
			this.CachePlayerRankedProgressionData(player.ActorNumber, this.Progression.GetProgressionRankIndex(), this.Progression.GetEloScore());
		}
		this.AllPlayerInRoundScores.TryAdd(player.ActorNumber, new RankedMultiplayerScore.PlayerScoreInRound(player.ActorNumber, initInfected));
	}

	// Token: 0x06003CF4 RID: 15604 RVA: 0x0014BC98 File Offset: 0x00149E98
	public RankedMultiplayerScore.PlayerScoreInRound GetInGameScoreForSelf()
	{
		RankedMultiplayerScore.PlayerScoreInRound result;
		if (this.AllPlayerInRoundScores.TryGetValue(NetworkSystem.Instance.LocalPlayerID, out result))
		{
			return result;
		}
		return default(RankedMultiplayerScore.PlayerScoreInRound);
	}

	// Token: 0x06003CF5 RID: 15605 RVA: 0x0014BCCC File Offset: 0x00149ECC
	public void OnTagReported(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		RankedMultiplayerScore.PlayerScoreInRound value;
		if (this.AllPlayerInRoundScores.TryGetValue(taggingPlayer.ActorNumber, out value))
		{
			value.NumTags++;
			this.AllPlayerInRoundScores[taggingPlayer.ActorNumber] = value;
		}
		RankedMultiplayerScore.PlayerScoreInRound value2;
		if (this.AllPlayerInRoundScores.TryGetValue(taggedPlayer.ActorNumber, out value2))
		{
			value2.Infected = true;
			value2.TaggedTime = Time.time;
			this.AllPlayerInRoundScores[taggedPlayer.ActorNumber] = value2;
		}
	}

	// Token: 0x06003CF6 RID: 15606 RVA: 0x0014BD48 File Offset: 0x00149F48
	private void ReportScore()
	{
		object obj;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("matchId", out obj))
		{
			foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
			{
				this.AllFinalPlayerScores.Add(new RankedMultiplayerScore.PlayerScore
				{
					PlayerId = keyValuePair.Key,
					GameScore = this.ComputeGameScore(keyValuePair.Value.NumTags, keyValuePair.Value.PointsOnDefense),
					EloScore = (this.PlayerRankedElos.ContainsKey(keyValuePair.Key) ? this.PlayerRankedElos[keyValuePair.Key] : 0f),
					NumTags = keyValuePair.Value.NumTags,
					TimeUntagged = keyValuePair.Value.TaggedTime - keyValuePair.Value.JoinTime,
					PointsOnDefense = keyValuePair.Value.PointsOnDefense
				});
			}
			GorillaTagCompetitiveServerApi.Instance.RequestSubmitMatchScores((string)obj, this.AllFinalPlayerScores);
		}
		this.PredictPlayerEloChanges();
	}

	// Token: 0x06003CF7 RID: 15607 RVA: 0x0014BE94 File Offset: 0x0014A094
	public float ComputeGameScore(int tags, float pointsOnDefense)
	{
		return (float)(tags * this.PointsPerTag) + pointsOnDefense;
	}

	// Token: 0x06003CF8 RID: 15608 RVA: 0x0014BEA4 File Offset: 0x0014A0A4
	private void PredictPlayerEloChanges()
	{
		this.VisitedScoreCombintations.Clear();
		this.AllFinalPlayerScores = (from s in this.AllFinalPlayerScores
		orderby s.GameScore descending
		select s).ToList<RankedMultiplayerScore.PlayerScore>();
		float k = this.Progression.MaxEloConstant / (float)(this.AllFinalPlayerScores.Count - 1);
		this.InProgressEloDeltaPerPlayer.Clear();
		for (int i = 0; i < this.AllFinalPlayerScores.Count; i++)
		{
			this.InProgressEloDeltaPerPlayer.Add(this.AllFinalPlayerScores[i].PlayerId, 0f);
		}
		for (int j = 0; j < this.AllFinalPlayerScores.Count; j++)
		{
			for (int l = 0; l < this.AllFinalPlayerScores.Count; l++)
			{
				if (j != l)
				{
					bool flag = this.AllFinalPlayerScores[j].GameScore.Approx(this.AllFinalPlayerScores[l].GameScore, 1E-06f);
					float eloWinProbability = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[l].EloScore, this.AllFinalPlayerScores[j].EloScore);
					float eloWinProbability2 = RankedProgressionManager.GetEloWinProbability(this.AllFinalPlayerScores[j].EloScore, this.AllFinalPlayerScores[l].EloScore);
					int key = j * this.AllFinalPlayerScores.Count + l;
					if (!this.VisitedScoreCombintations.ContainsKey(key))
					{
						RankedMultiplayerScore.PlayerScore playerScore = this.AllFinalPlayerScores[j];
						float actualResult;
						if (flag)
						{
							actualResult = 0.5f;
						}
						else
						{
							actualResult = (float)((j < l) ? 1 : 0);
						}
						float eloScore = playerScore.EloScore;
						float num = RankedProgressionManager.UpdateEloScore(eloScore, eloWinProbability, actualResult, k);
						Dictionary<int, float> inProgressEloDeltaPerPlayer = this.InProgressEloDeltaPerPlayer;
						int playerId = playerScore.PlayerId;
						inProgressEloDeltaPerPlayer[playerId] += num - eloScore;
						this.VisitedScoreCombintations.Add(key, true);
					}
					int key2 = l * this.AllFinalPlayerScores.Count + j;
					if (!this.VisitedScoreCombintations.ContainsKey(key2))
					{
						RankedMultiplayerScore.PlayerScore playerScore2 = this.AllFinalPlayerScores[l];
						float actualResult;
						if (flag)
						{
							actualResult = 0.5f;
						}
						else
						{
							actualResult = (float)((l < j) ? 1 : 0);
						}
						float eloScore2 = playerScore2.EloScore;
						float num2 = RankedProgressionManager.UpdateEloScore(eloScore2, eloWinProbability2, actualResult, k);
						Dictionary<int, float> inProgressEloDeltaPerPlayer = this.InProgressEloDeltaPerPlayer;
						int playerId = playerScore2.PlayerId;
						inProgressEloDeltaPerPlayer[playerId] += num2 - eloScore2;
						this.VisitedScoreCombintations.Add(key2, true);
					}
				}
			}
		}
	}

	// Token: 0x06003CF9 RID: 15609 RVA: 0x0014C134 File Offset: 0x0014A334
	public void CachePlayerRankedProgressionData(int playerId, int tierIdx, float elo)
	{
		if (this.PlayerRankedTierIndices.ContainsKey(playerId))
		{
			this.PlayerRankedTierIndices[playerId] = tierIdx;
		}
		else
		{
			this.PlayerRankedTierIndices.Add(playerId, tierIdx);
		}
		if (this.PlayerRankedElos.ContainsKey(playerId))
		{
			this.PlayerRankedElos[playerId] = elo;
			return;
		}
		this.PlayerRankedElos.Add(playerId, elo);
	}

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x06003CFA RID: 15610 RVA: 0x0014C194 File Offset: 0x0014A394
	// (set) Token: 0x06003CFB RID: 15611 RVA: 0x0014C19C File Offset: 0x0014A39C
	public Dictionary<int, int> PlayerRankedTiers
	{
		get
		{
			return this.PlayerRankedTierIndices;
		}
		set
		{
			this.PlayerRankedTierIndices = value;
		}
	}

	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x06003CFC RID: 15612 RVA: 0x0014C1A5 File Offset: 0x0014A3A5
	// (set) Token: 0x06003CFD RID: 15613 RVA: 0x0014C1AD File Offset: 0x0014A3AD
	public Dictionary<int, float> PlayerRankedEloScores
	{
		get
		{
			return this.PlayerRankedElos;
		}
		set
		{
			this.PlayerRankedElos = value;
		}
	}

	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x06003CFE RID: 15614 RVA: 0x0014C1B6 File Offset: 0x0014A3B6
	// (set) Token: 0x06003CFF RID: 15615 RVA: 0x0014C1BE File Offset: 0x0014A3BE
	public Dictionary<int, float> ProjectedEloDeltas
	{
		get
		{
			return this.InProgressEloDeltaPerPlayer;
		}
		set
		{
			this.InProgressEloDeltaPerPlayer = value;
		}
	}

	// Token: 0x06003D00 RID: 15616 RVA: 0x0014C1C8 File Offset: 0x0014A3C8
	public List<RankedMultiplayerScore.PlayerScoreInRound> GetSortedScores()
	{
		List<RankedMultiplayerScore.PlayerScoreInRound> list = new List<RankedMultiplayerScore.PlayerScoreInRound>();
		foreach (KeyValuePair<int, RankedMultiplayerScore.PlayerScoreInRound> keyValuePair in this.AllPlayerInRoundScores)
		{
			list.Add(keyValuePair.Value);
		}
		list.Sort((RankedMultiplayerScore.PlayerScoreInRound s1, RankedMultiplayerScore.PlayerScoreInRound s2) => this.ComputeGameScore(s2.NumTags, s2.PointsOnDefense).CompareTo(this.ComputeGameScore(s1.NumTags, s1.PointsOnDefense)));
		return list;
	}

	// Token: 0x04004D7B RID: 19835
	public static float LongestUntaggedTieEpsilon = 0.2f;

	// Token: 0x04004D7C RID: 19836
	public static int RESULT_TIE = -1;

	// Token: 0x04004D7D RID: 19837
	[SerializeField]
	private int PointsPerTag = 30;

	// Token: 0x04004D7E RID: 19838
	[SerializeField]
	private float PointsPerUninfectedSecMin = 0.5f;

	// Token: 0x04004D7F RID: 19839
	[SerializeField]
	private float PointsPerUninfectedSecMax = 2f;

	// Token: 0x04004D80 RID: 19840
	private float PerSecondTimer = -1f;

	// Token: 0x04004D81 RID: 19841
	private bool WasInfectedInitially;

	// Token: 0x04004D82 RID: 19842
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04004D83 RID: 19843
	protected Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound> AllPlayerInRoundScores = new Dictionary<int, RankedMultiplayerScore.PlayerScoreInRound>();

	// Token: 0x04004D84 RID: 19844
	protected List<RankedMultiplayerScore.PlayerScore> AllFinalPlayerScores = new List<RankedMultiplayerScore.PlayerScore>();

	// Token: 0x04004D85 RID: 19845
	protected Dictionary<int, bool> VisitedScoreCombintations = new Dictionary<int, bool>();

	// Token: 0x04004D86 RID: 19846
	protected Dictionary<int, float> InProgressEloDeltaPerPlayer = new Dictionary<int, float>();

	// Token: 0x04004D87 RID: 19847
	protected Dictionary<int, int> PlayerRankedTierIndices = new Dictionary<int, int>();

	// Token: 0x04004D88 RID: 19848
	protected Dictionary<int, float> PlayerRankedElos = new Dictionary<int, float>();

	// Token: 0x04004D89 RID: 19849
	private RankedMultiplayerScore.ResultData PendingResults;

	// Token: 0x04004D8A RID: 19850
	private RankedMultiplayerScore.RecordHolder<int> ResultsMostTags;

	// Token: 0x04004D8B RID: 19851
	private RankedMultiplayerScore.RecordHolder<float> ResultsLongestUntagged;

	// Token: 0x04004D8C RID: 19852
	private bool IsLateJoiner;

	// Token: 0x0200091B RID: 2331
	public struct PlayerScore
	{
		// Token: 0x04004D8E RID: 19854
		public int PlayerId;

		// Token: 0x04004D8F RID: 19855
		public float GameScore;

		// Token: 0x04004D90 RID: 19856
		public float EloScore;

		// Token: 0x04004D91 RID: 19857
		public int NumTags;

		// Token: 0x04004D92 RID: 19858
		public float TimeUntagged;

		// Token: 0x04004D93 RID: 19859
		public float PointsOnDefense;
	}

	// Token: 0x0200091C RID: 2332
	public struct PlayerScoreInRound
	{
		// Token: 0x06003D04 RID: 15620 RVA: 0x0014C308 File Offset: 0x0014A508
		public PlayerScoreInRound(int id, bool initInfected = false)
		{
			this.PlayerId = id;
			this.NumTags = 0;
			this.PointsOnDefense = 0f;
			this.JoinTime = Time.time;
			this.Infected = initInfected;
			this.TaggedTime = (initInfected ? Time.time : 0f);
		}

		// Token: 0x04004D94 RID: 19860
		public int PlayerId;

		// Token: 0x04004D95 RID: 19861
		public int NumTags;

		// Token: 0x04004D96 RID: 19862
		public float PointsOnDefense;

		// Token: 0x04004D97 RID: 19863
		public float JoinTime;

		// Token: 0x04004D98 RID: 19864
		public float TaggedTime;

		// Token: 0x04004D99 RID: 19865
		public bool Infected;
	}

	// Token: 0x0200091D RID: 2333
	public struct ResultData
	{
		// Token: 0x06003D05 RID: 15621 RVA: 0x0014C355 File Offset: 0x0014A555
		public bool IsMostTagsTied()
		{
			return this.MostTagsPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x06003D06 RID: 15622 RVA: 0x0014C364 File Offset: 0x0014A564
		public bool IsLongestUntaggedTied()
		{
			return this.LongestUntaggedPlayerId == RankedMultiplayerScore.RESULT_TIE;
		}

		// Token: 0x04004D9A RID: 19866
		public float Elo;

		// Token: 0x04004D9B RID: 19867
		public int Rank;

		// Token: 0x04004D9C RID: 19868
		public int MostTags;

		// Token: 0x04004D9D RID: 19869
		public float LongestUntagged;

		// Token: 0x04004D9E RID: 19870
		public int MostTagsPlayerId;

		// Token: 0x04004D9F RID: 19871
		public int LongestUntaggedPlayerId;
	}

	// Token: 0x0200091E RID: 2334
	public struct RecordHolder<T>
	{
		// Token: 0x04004DA0 RID: 19872
		public int PlayerId;

		// Token: 0x04004DA1 RID: 19873
		public T Value;
	}
}
