using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000899 RID: 2201
public class GorillaTagCompetitiveScoreboard : MonoBehaviour
{
	// Token: 0x060039AD RID: 14765 RVA: 0x0013A394 File Offset: 0x00138594
	private void Awake()
	{
		GorillaTagCompetitiveManager.RegisterScoreboard(this);
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060039AE RID: 14766 RVA: 0x0013A3CD File Offset: 0x001385CD
	private void OnDestroy()
	{
		GorillaTagCompetitiveManager.DeregisterScoreboard(this);
	}

	// Token: 0x060039AF RID: 14767 RVA: 0x0013A3D8 File Offset: 0x001385D8
	public void UpdateScores(GorillaTagCompetitiveManager.GameState gameState, float activeRoundTime, List<RankedMultiplayerScore.PlayerScoreInRound> scores, Dictionary<int, int> PlayerRankedTiers, Dictionary<int, float> PlayerPredictedEloDeltas, List<NetPlayer> infectedPlayers, RankedProgressionManager progressionManager)
	{
		this.waitingForPlayers.SetActive(gameState == GorillaTagCompetitiveManager.GameState.WaitingForPlayers);
		for (int i = 0; i < this.lines.Length; i++)
		{
			if (gameState != GorillaTagCompetitiveManager.GameState.WaitingForPlayers && scores != null && scores.Count > i)
			{
				RankedMultiplayerScore.PlayerScoreInRound playerScoreInRound = scores[i];
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerScoreInRound.PlayerId);
				if (netPlayerByID != null)
				{
					this.lines[i].gameObject.SetActive(true);
					if (PlayerRankedTiers == null || !PlayerRankedTiers.ContainsKey(playerScoreInRound.PlayerId))
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, null);
					}
					else
					{
						this.lines[i].SetPlayer(netPlayerByID.SanitizedNickName, progressionManager.GetProgressionRankIcon(PlayerRankedTiers[playerScoreInRound.PlayerId]));
					}
					if (playerScoreInRound.TaggedTime.Approx(0f, 1E-06f))
					{
						this.lines[i].SetScore(Mathf.Max(activeRoundTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					else
					{
						this.lines[i].SetScore(Mathf.Max(playerScoreInRound.TaggedTime - playerScoreInRound.JoinTime, 0f), playerScoreInRound.NumTags);
					}
					if (PlayerPredictedEloDeltas.ContainsKey(playerScoreInRound.PlayerId))
					{
						float num = PlayerPredictedEloDeltas[playerScoreInRound.PlayerId];
						GorillaTagCompetitiveScoreboard.PredictedResult predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Even;
						if (num > this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Great;
						}
						else if (num > this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Good;
						}
						else if (num < -this.largeEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Poor;
						}
						else if (num < -this.smallEloDelta)
						{
							predictedResult = GorillaTagCompetitiveScoreboard.PredictedResult.Bad;
						}
						this.lines[i].SetPredictedResult(predictedResult);
					}
					this.lines[i].SetInfected(gameState == GorillaTagCompetitiveManager.GameState.Playing && infectedPlayers.Contains(netPlayerByID));
				}
			}
			else
			{
				this.lines[i].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060039B0 RID: 14768 RVA: 0x0013A5A8 File Offset: 0x001387A8
	public void DisplayPredictedResults(bool bShow)
	{
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].DisplayPredictedResults(bShow);
		}
	}

	// Token: 0x0400498E RID: 18830
	public GorillaTagCompetitiveScoreboardLine[] lines;

	// Token: 0x0400498F RID: 18831
	public GameObject waitingForPlayers;

	// Token: 0x04004990 RID: 18832
	public float smallEloDelta = 10f;

	// Token: 0x04004991 RID: 18833
	public float largeEloDelta = 25f;

	// Token: 0x0200089A RID: 2202
	public enum PredictedResult
	{
		// Token: 0x04004993 RID: 18835
		Great,
		// Token: 0x04004994 RID: 18836
		Good,
		// Token: 0x04004995 RID: 18837
		Even,
		// Token: 0x04004996 RID: 18838
		Bad,
		// Token: 0x04004997 RID: 18839
		Poor
	}
}
