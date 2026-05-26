using System;
using TMPro;
using UnityEngine;

// Token: 0x0200089B RID: 2203
public class GorillaTagCompetitiveScoreboardLine : MonoBehaviour
{
	// Token: 0x060039B2 RID: 14770 RVA: 0x0013A5F4 File Offset: 0x001387F4
	public void SetPlayer(string playerName, Sprite icon)
	{
		this.playerNameDisplay.text = playerName;
		this.rankSprite.sprite = icon;
	}

	// Token: 0x060039B3 RID: 14771 RVA: 0x0013A610 File Offset: 0x00138810
	public void SetScore(float untaggedTime, int tagCount)
	{
		int num = Mathf.FloorToInt(untaggedTime);
		int num2 = num / 60;
		int num3 = num % 60;
		this.untaggedTimeDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
		this.tagCountDisplay.text = tagCount.ToString();
	}

	// Token: 0x060039B4 RID: 14772 RVA: 0x0013A65F File Offset: 0x0013885F
	public void SetPredictedResult(GorillaTagCompetitiveScoreboard.PredictedResult result)
	{
		this.resultSprite.sprite = this.resultSprites[(int)result];
	}

	// Token: 0x060039B5 RID: 14773 RVA: 0x0013A674 File Offset: 0x00138874
	public void DisplayPredictedResults(bool bShow)
	{
		this.resultSprite.gameObject.SetActive(bShow);
	}

	// Token: 0x060039B6 RID: 14774 RVA: 0x0013A687 File Offset: 0x00138887
	public void SetInfected(bool infected)
	{
		this.playerNameDisplay.color = (infected ? Color.red : Color.white);
	}

	// Token: 0x04004998 RID: 18840
	public SpriteRenderer rankSprite;

	// Token: 0x04004999 RID: 18841
	public TMP_Text playerNameDisplay;

	// Token: 0x0400499A RID: 18842
	public TMP_Text untaggedTimeDisplay;

	// Token: 0x0400499B RID: 18843
	public TMP_Text tagCountDisplay;

	// Token: 0x0400499C RID: 18844
	public SpriteRenderer resultSprite;

	// Token: 0x0400499D RID: 18845
	public Sprite[] resultSprites;
}
