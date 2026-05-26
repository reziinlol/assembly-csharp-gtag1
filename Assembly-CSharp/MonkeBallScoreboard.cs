using System;
using TMPro;
using UnityEngine;

// Token: 0x020005FF RID: 1535
public class MonkeBallScoreboard : MonoBehaviour
{
	// Token: 0x06002655 RID: 9813 RVA: 0x000CB356 File Offset: 0x000C9556
	public void Setup(MonkeBallGame game)
	{
		this.game = game;
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000CB360 File Offset: 0x000C9560
	public void RefreshScore()
	{
		for (int i = 0; i < this.game.team.Count; i++)
		{
			this.teamDisplays[i].scoreLabel.text = this.game.team[i].score.ToString();
		}
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000CB3B5 File Offset: 0x000C95B5
	public void RefreshTeamPlayers(int teamId, int numPlayers)
	{
		this.teamDisplays[teamId].playersLabel.text = string.Format("PLAYERS: {0}", Mathf.Clamp(numPlayers, 0, 99));
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000CB3E1 File Offset: 0x000C95E1
	public void PlayScoreFx()
	{
		this.PlayFX(this.scoreSound, this.scoreSoundVolume);
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000CB3F5 File Offset: 0x000C95F5
	public void PlayPlayerJoinFx()
	{
		this.PlayFX(this.playerJoinSound, 0.5f);
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000CB408 File Offset: 0x000C9608
	public void PlayPlayerLeaveFx()
	{
		this.PlayFX(this.playerLeaveSound, 0.5f);
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000CB41B File Offset: 0x000C961B
	public void PlayGameStartFx()
	{
		this.PlayFX(this.gameStartSound, this.gameStartVolume);
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000CB42F File Offset: 0x000C962F
	public void PlayGameEndFx()
	{
		this.PlayFX(this.gameEndSound, this.gameEndVolume);
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000CB443 File Offset: 0x000C9643
	private void PlayFX(AudioClip clip, float volume)
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = clip;
			this.audioSource.volume = volume;
			this.audioSource.Play();
		}
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000CB476 File Offset: 0x000C9676
	public void RefreshTime(string timeString)
	{
		this.timeRemainingLabel.text = timeString;
	}

	// Token: 0x040031B0 RID: 12720
	private MonkeBallGame game;

	// Token: 0x040031B1 RID: 12721
	public MonkeBallScoreboard.TeamDisplay[] teamDisplays;

	// Token: 0x040031B2 RID: 12722
	public TextMeshPro timeRemainingLabel;

	// Token: 0x040031B3 RID: 12723
	public AudioSource audioSource;

	// Token: 0x040031B4 RID: 12724
	public AudioClip scoreSound;

	// Token: 0x040031B5 RID: 12725
	public float scoreSoundVolume;

	// Token: 0x040031B6 RID: 12726
	public AudioClip playerJoinSound;

	// Token: 0x040031B7 RID: 12727
	public AudioClip playerLeaveSound;

	// Token: 0x040031B8 RID: 12728
	public AudioClip gameStartSound;

	// Token: 0x040031B9 RID: 12729
	public float gameStartVolume;

	// Token: 0x040031BA RID: 12730
	public AudioClip gameEndSound;

	// Token: 0x040031BB RID: 12731
	public float gameEndVolume;

	// Token: 0x02000600 RID: 1536
	[Serializable]
	public class TeamDisplay
	{
		// Token: 0x040031BC RID: 12732
		public TextMeshPro nameLabel;

		// Token: 0x040031BD RID: 12733
		public TextMeshPro scoreLabel;

		// Token: 0x040031BE RID: 12734
		public TextMeshPro playersLabel;
	}
}
