using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000821 RID: 2081
public class GRUIScoreboardEntry : MonoBehaviour
{
	// Token: 0x0600357F RID: 13695 RVA: 0x0012817D File Offset: 0x0012637D
	public void Setup(VRRig vrRig, int playerActorId, GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.playerActorId = playerActorId;
		this.Refresh(vrRig, screenType);
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x00128190 File Offset: 0x00126390
	private void Refresh(VRRig vrRig, GRUIScoreboard.ScoreboardScreen screenType)
	{
		GRPlayer grplayer = GRPlayer.Get(vrRig);
		if (!(vrRig != null) || !(grplayer != null))
		{
			this.playerNameLabel.text = "";
			this.playerCurrencyLabel.text = "";
			this.playerTitleLabel.text = "";
			this.playerCutLabel.text = "";
			this.currencySet = 0;
			return;
		}
		if (!this.playerNameLabel.text.Equals(vrRig.playerNameVisible))
		{
			this.playerNameLabel.text = vrRig.playerNameVisible;
		}
		if (screenType != GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			if (screenType == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
			{
				this.defaultUIParent.SetActive(false);
				this.shiftCutParent.SetActive(true);
				if (GhostReactor.instance.shiftManager != null && (GhostReactor.instance.shiftManager.ShiftActive || GhostReactor.instance.shiftManager.ShiftTotalEarned >= 0))
				{
					int num = Mathf.FloorToInt(grplayer.ShiftPlayTime / 60f);
					int num2 = Mathf.FloorToInt(grplayer.ShiftPlayTime - (float)(num * 60));
					this.playerTimeLabel.text = string.Format("{0:00}:{1:00}", num, num2);
					this.playerPercentageLabel.text = "%" + Mathf.Floor(grplayer.ShiftPlayTime / GhostReactor.instance.shiftManager.TotalPlayTime * 100f).ToString();
				}
				else
				{
					this.playerTimeLabel.text = "n/a";
					this.playerPercentageLabel.text = "n/a";
				}
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		else
		{
			this.defaultUIParent.SetActive(true);
			this.shiftCutParent.SetActive(false);
			if (grplayer.ShiftCredits != this.currencySet)
			{
				this.currencySet = grplayer.ShiftCredits;
				this.playerCurrencyLabel.text = this.currencySet.ToString();
			}
			string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
			if (titleNameAndGrade != this.titleSet)
			{
				this.titleSet = titleNameAndGrade;
				this.playerTitleLabel.text = this.titleSet;
			}
		}
		if (GhostReactor.instance.shiftManager == null || GhostReactor.instance.shiftManager.ShiftActive)
		{
			this.playerCutLabel.text = "-";
			return;
		}
		this.playerCutLabel.text = grplayer.LastShiftCut.ToString();
	}

	// Token: 0x04004610 RID: 17936
	[SerializeField]
	private TMP_Text playerNameLabel;

	// Token: 0x04004611 RID: 17937
	[SerializeField]
	private TMP_Text playerCutLabel;

	// Token: 0x04004612 RID: 17938
	public GameObject defaultUIParent;

	// Token: 0x04004613 RID: 17939
	[SerializeField]
	private TMP_Text playerTitleLabel;

	// Token: 0x04004614 RID: 17940
	[SerializeField]
	private TMP_Text playerCurrencyLabel;

	// Token: 0x04004615 RID: 17941
	public GameObject shiftCutParent;

	// Token: 0x04004616 RID: 17942
	[SerializeField]
	private TMP_Text playerTimeLabel;

	// Token: 0x04004617 RID: 17943
	[SerializeField]
	private TMP_Text playerPercentageLabel;

	// Token: 0x04004618 RID: 17944
	private int playerActorId = -1;

	// Token: 0x04004619 RID: 17945
	private int currencySet = -1;

	// Token: 0x0400461A RID: 17946
	private string titleSet = "";
}
