using System;
using TMPro;
using UnityEngine;

// Token: 0x02000897 RID: 2199
public class GorillaTagCompetitiveRankDisplay : MonoBehaviour
{
	// Token: 0x060039A0 RID: 14752 RVA: 0x0013A06E File Offset: 0x0013826E
	private void OnEnable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged += this.HandleRankedSubtierChanged;
		this.HandleRankedSubtierChanged(0, 0);
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x0013A08E File Offset: 0x0013828E
	private void OnDisable()
	{
		VRRig.LocalRig.OnRankedSubtierChanged -= this.HandleRankedSubtierChanged;
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x0013A0A8 File Offset: 0x001382A8
	public void HandleRankedSubtierChanged(int questSubTier, int pcSubTier)
	{
		float currentELO = RankedProgressionManager.Instance.GetCurrentELO();
		int progressionRankIndex = RankedProgressionManager.Instance.GetProgressionRankIndex(currentELO);
		this.UpdateRankIcons(progressionRankIndex);
		this.UpdateRankProgress(RankedProgressionManager.Instance.GetProgressionRankProgress());
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x0013A0E4 File Offset: 0x001382E4
	private void UpdateRankIcons(int currentRank)
	{
		this.currentRankSprite.sprite = RankedProgressionManager.Instance.GetProgressionRankIcon(currentRank);
		this.currentRank_Name.text = RankedProgressionManager.Instance.GetProgressionRankName().ToUpper();
		bool flag = currentRank < RankedProgressionManager.Instance.MaxRank;
		bool flag2 = currentRank > 0;
		this.nextRankSprite.gameObject.SetActive(flag);
		this.nextText.gameObject.SetActive(flag);
		this.nextRank_Name.gameObject.SetActive(flag);
		if (flag)
		{
			this.nextRankSprite.sprite = RankedProgressionManager.Instance.GetNextProgressionRankIcon(currentRank);
			this.nextRank_Name.text = RankedProgressionManager.Instance.GetNextProgressionRankName(currentRank).ToUpper();
		}
		this.prevRankSprite.gameObject.SetActive(flag2);
		this.prevText.gameObject.SetActive(flag2);
		this.prevRank_Name.gameObject.SetActive(flag2);
		if (flag2)
		{
			this.prevRankSprite.sprite = RankedProgressionManager.Instance.GetPrevProgressionRankIcon(currentRank);
			this.prevRank_Name.text = RankedProgressionManager.Instance.GetPrevProgressionRankName(currentRank).ToUpper();
		}
	}

	// Token: 0x060039A4 RID: 14756 RVA: 0x0013A204 File Offset: 0x00138404
	private void UpdateRankProgress(float percent)
	{
		percent = Mathf.Clamp01(percent);
		Vector2 size = this.progressBar.size;
		size.x = this.progressBarSize * percent;
		this.progressBar.size = size;
	}

	// Token: 0x0400497B RID: 18811
	[SerializeField]
	private SpriteRenderer progressBar;

	// Token: 0x0400497C RID: 18812
	[SerializeField]
	private float progressBarSize = 100f;

	// Token: 0x0400497D RID: 18813
	[SerializeField]
	private SpriteRenderer currentRankSprite;

	// Token: 0x0400497E RID: 18814
	[SerializeField]
	private SpriteRenderer prevRankSprite;

	// Token: 0x0400497F RID: 18815
	[SerializeField]
	private SpriteRenderer nextRankSprite;

	// Token: 0x04004980 RID: 18816
	[SerializeField]
	private TextMeshPro currentRank_Name;

	// Token: 0x04004981 RID: 18817
	[SerializeField]
	private TextMeshPro prevText;

	// Token: 0x04004982 RID: 18818
	[SerializeField]
	private TextMeshPro nextText;

	// Token: 0x04004983 RID: 18819
	[SerializeField]
	private TextMeshPro prevRank_Name;

	// Token: 0x04004984 RID: 18820
	[SerializeField]
	private TextMeshPro nextRank_Name;
}
