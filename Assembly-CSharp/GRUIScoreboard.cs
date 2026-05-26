using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200081F RID: 2079
public class GRUIScoreboard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003577 RID: 13687 RVA: 0x00127FC6 File Offset: 0x001261C6
	public void SliceUpdate()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			this.Refresh(GhostReactor.instance.vrRigs);
		}
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x00127FE4 File Offset: 0x001261E4
	public void Refresh(List<VRRig> vrRigs)
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			GhostReactor.instance.shiftManager.CalculatePlayerPercentages();
		}
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (!(this.entries[i] == null))
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.entries[i].gameObject.SetActive(true);
					this.entries[i].Setup(vrRigs[i], vrRigs[i].OwningNetPlayer.ActorNumber, this.currentScreen);
				}
				else
				{
					this.entries[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x001280C4 File Offset: 0x001262C4
	public void SwitchToScreen(GRUIScoreboard.ScoreboardScreen screenType)
	{
		this.currentScreen = screenType;
		GRUIScoreboard.ScoreboardScreen scoreboardScreen = this.currentScreen;
		if (scoreboardScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.infoTextParent.SetActive(true);
			this.calcTextParent.SetActive(false);
			this.buttonText.text = "SHOW CUT CALC";
			return;
		}
		if (scoreboardScreen != GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation)
		{
			return;
		}
		this.infoTextParent.SetActive(false);
		this.calcTextParent.SetActive(true);
		this.buttonText.text = "SHOW INFO";
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x00128138 File Offset: 0x00126338
	public void SwitchState()
	{
		if (this.currentScreen == GRUIScoreboard.ScoreboardScreen.DefaultInfo)
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation);
		}
		else
		{
			this.SwitchToScreen(GRUIScoreboard.ScoreboardScreen.DefaultInfo);
		}
		this.Refresh(GhostReactor.instance.vrRigs);
		GhostReactor.instance.UpdateRemoteScoreboardScreen(this.currentScreen);
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x00128172 File Offset: 0x00126372
	public static bool ValidPage(GRUIScoreboard.ScoreboardScreen screen)
	{
		return screen == GRUIScoreboard.ScoreboardScreen.DefaultInfo || screen == GRUIScoreboard.ScoreboardScreen.ShiftCutCalculation;
	}

	// Token: 0x04004607 RID: 17927
	public List<GRUIScoreboardEntry> entries;

	// Token: 0x04004608 RID: 17928
	public TMP_Text total;

	// Token: 0x04004609 RID: 17929
	public TMP_Text buttonText;

	// Token: 0x0400460A RID: 17930
	public GRUIScoreboard.ScoreboardScreen currentScreen;

	// Token: 0x0400460B RID: 17931
	public GameObject infoTextParent;

	// Token: 0x0400460C RID: 17932
	public GameObject calcTextParent;

	// Token: 0x02000820 RID: 2080
	public enum ScoreboardScreen
	{
		// Token: 0x0400460E RID: 17934
		DefaultInfo,
		// Token: 0x0400460F RID: 17935
		ShiftCutCalculation
	}
}
