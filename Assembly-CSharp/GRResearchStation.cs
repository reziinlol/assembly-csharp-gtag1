using System;
using System.Collections.Generic;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020007CA RID: 1994
public class GRResearchStation : MonoBehaviour
{
	// Token: 0x060032D0 RID: 13008 RVA: 0x001169E8 File Offset: 0x00114BE8
	public void Init(GRToolProgressionManager tree, GhostReactor ghostReactor)
	{
		this.toolProgressionManager = tree;
		this.toolProgressionManager.OnProgressionUpdated += this.ResearchTreeUpdated;
		this.reactor = ghostReactor;
		this.totalTools = 0;
		this.selectedToolIndex = 0;
		this._levelString = this.LevelText.text;
		this._costString = this.CostText.text;
		this._researchPointsString = this.ResearchPointsTex.text;
		this._requiredLevelString = this.RequiredLevelText.text;
		this.UpdateUI();
		this.SelectTool(0);
	}

	// Token: 0x060032D1 RID: 13009 RVA: 0x00116A7C File Offset: 0x00114C7C
	private void SelectTool(int index)
	{
		if (this.toolProgressionManager == null || this.totalTools == 0)
		{
			return;
		}
		if (index < this.totalTools && index > -1)
		{
			this.selectedToolIndex = index;
			this.selectedToolUpgrades = this.toolProgressionManager.GetToolUpgrades(this.supportedTools[this.selectedToolIndex]);
			this.SelectUpgrade(0);
			this.UpdateUI();
		}
	}

	// Token: 0x060032D2 RID: 13010 RVA: 0x00116AE3 File Offset: 0x00114CE3
	public void ResearchTreeUpdated()
	{
		this.supportedTools = this.toolProgressionManager.GetSupportedTools();
		this.totalTools = this.supportedTools.Count;
		this.SelectTool(this.selectedToolIndex);
		this.UpdateUI();
	}

	// Token: 0x060032D3 RID: 13011 RVA: 0x00116B19 File Offset: 0x00114D19
	public void UpdateUI()
	{
		this.UpdateToolName();
		this.UpdateUpgradeTitles();
		this.UpdateLocked();
		this.UpdateRequiredLevel();
		this.UpdateCost();
		this.UpdateResearchPoints(this.toolProgressionManager.GetNumberOfResearchPoints());
	}

	// Token: 0x060032D4 RID: 13012 RVA: 0x00116B4C File Offset: 0x00114D4C
	public void SelectUpgrade(int UpgradeIndex)
	{
		if (this.toolProgressionManager == null)
		{
			return;
		}
		this.selectedUpgradeIndex = UpgradeIndex;
		if (this.selectedToolUpgrades.Count > this.selectedUpgradeIndex)
		{
			this.currentlySelectedToolUpgrade = this.selectedToolUpgrades[this.selectedUpgradeIndex];
			this.currentlySelectedUpgradeMetadata = this.currentlySelectedToolUpgrade.partMetadata;
			this.SetUpgradeTextColors(this.selectedUpgradeIndex);
			this.UpdateDescriptionText(this.currentlySelectedUpgradeMetadata.description);
		}
		this.UpdateUI();
	}

	// Token: 0x060032D5 RID: 13013 RVA: 0x00116BD0 File Offset: 0x00114DD0
	private void SetUpgradeTextColors(int index)
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			this.UpgradeButton[i].isOn = false;
			this.UpgradeButton[i].UpdateColor();
		}
		this.UpgradeButton[index].isOn = true;
		this.UpgradeButton[index].UpdateColor();
	}

	// Token: 0x060032D6 RID: 13014 RVA: 0x00116C28 File Offset: 0x00114E28
	private void UpdateUpgradeTitles()
	{
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.totalTools >= this.selectedToolIndex && this.selectedToolUpgrades.Count > i)
			{
				this.UpgradeTitlesText[i].text = this.selectedToolUpgrades[i].partMetadata.name;
			}
			else
			{
				this.UpgradeTitlesText[i].text = null;
			}
		}
	}

	// Token: 0x060032D7 RID: 13015 RVA: 0x00116C98 File Offset: 0x00114E98
	public void UpdateLocked()
	{
		if (this.currentlySelectedToolUpgrade.unlocked)
		{
			this.UnlockedText.color = this.unlockedToolColor;
			this.UnlockedText.text = "UNLOCKED";
		}
		else
		{
			this.UnlockedText.color = this.lockedToolColor;
			this.UnlockedText.text = "LOCKED";
		}
		for (int i = 0; i < this.UpgradeTitlesText.Length; i++)
		{
			if (this.totalTools >= this.selectedToolIndex && this.selectedToolUpgrades.Count > i)
			{
				bool unlocked = this.selectedToolUpgrades[i].unlocked;
				this.UpgradeTitlesText[i].color = (unlocked ? this.unlockedToolColor : this.lockedToolColor);
				this.LockedImage[i].gameObject.SetActive(!unlocked);
			}
			else
			{
				this.UpgradeTitlesText[i].color = Color.black;
				this.LockedImage[i].gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x060032D8 RID: 13016 RVA: 0x00116D98 File Offset: 0x00114F98
	public void UpdateRequiredLevel()
	{
		int requiredEmployeeLevel = this.toolProgressionManager.GetRequiredEmployeeLevel(this.currentlySelectedToolUpgrade.requiredEmployeeLevel);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(requiredEmployeeLevel);
		int num = 0;
		GRPlayer grplayer = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
		if (grplayer != null)
		{
			num = GhostReactorProgression.GetTitleLevel(grplayer.CurrentProgression.redeemedPoints);
		}
		string titleNameFromLevel2 = GhostReactorProgression.GetTitleNameFromLevel(num);
		this.RequiredLevelText.text = string.Format(this._requiredLevelString, titleNameFromLevel);
		this.LevelText.text = string.Format(this._levelString, titleNameFromLevel2);
		this.RequiredLevelText.color = ((num >= requiredEmployeeLevel) ? this.unlockedToolColor : this.lockedToolColor);
	}

	// Token: 0x060032D9 RID: 13017 RVA: 0x00116E43 File Offset: 0x00115043
	public void UpdateDescriptionText(string description)
	{
		this.DescriptionText.text = description;
	}

	// Token: 0x060032DA RID: 13018 RVA: 0x00116E54 File Offset: 0x00115054
	public void UpdateCost()
	{
		if (this.selectedToolUpgrades != null && this.selectedToolUpgrades.Count > 0 && this.selectedToolUpgrades.Count > this.selectedUpgradeIndex)
		{
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			int researchCost = this.selectedToolUpgrades[this.selectedUpgradeIndex].researchCost;
			this.CostText.text = string.Format(this._costString, researchCost);
			this.CostText.color = ((numberOfResearchPoints >= researchCost) ? this.unlockedToolColor : this.lockedToolColor);
		}
	}

	// Token: 0x060032DB RID: 13019 RVA: 0x00116EE6 File Offset: 0x001150E6
	public void UpdateToolName()
	{
		if (this.supportedTools.Count > 0)
		{
			this.ToolNameText.text = GRUtils.GetToolName(this.supportedTools[this.selectedToolIndex]);
		}
	}

	// Token: 0x060032DC RID: 13020 RVA: 0x00116F17 File Offset: 0x00115117
	public void UpdateResearchPoints(int ResearchPoints)
	{
		this.ResearchPointsTex.text = string.Format(this._researchPointsString, ResearchPoints);
	}

	// Token: 0x060032DD RID: 13021 RVA: 0x00116F35 File Offset: 0x00115135
	public void MFDButton0Pressed()
	{
		this.SelectUpgrade(0);
	}

	// Token: 0x060032DE RID: 13022 RVA: 0x00116F3E File Offset: 0x0011513E
	public void MFDButton1Pressed()
	{
		this.SelectUpgrade(1);
	}

	// Token: 0x060032DF RID: 13023 RVA: 0x00116F47 File Offset: 0x00115147
	public void MFDButton2Pressed()
	{
		this.SelectUpgrade(2);
	}

	// Token: 0x060032E0 RID: 13024 RVA: 0x00116F50 File Offset: 0x00115150
	public void MFDButton3Pressed()
	{
		this.SelectUpgrade(3);
	}

	// Token: 0x060032E1 RID: 13025 RVA: 0x00116F59 File Offset: 0x00115159
	public void MFDButton4Pressed()
	{
		this.SelectUpgrade(4);
	}

	// Token: 0x060032E2 RID: 13026 RVA: 0x00116F62 File Offset: 0x00115162
	public void MFDButton5Pressed()
	{
		this.SelectUpgrade(5);
	}

	// Token: 0x060032E3 RID: 13027 RVA: 0x00116F6B File Offset: 0x0011516B
	public void NextToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex + 1) % this.totalTools;
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x060032E4 RID: 13028 RVA: 0x00116F8E File Offset: 0x0011518E
	public void PreviousToolButtonPressed()
	{
		this.selectedToolIndex = (this.selectedToolIndex - 1).PositiveModulo(this.totalTools);
		this.SelectTool(this.selectedToolIndex);
	}

	// Token: 0x060032E5 RID: 13029 RVA: 0x00116FB5 File Offset: 0x001151B5
	public void UpgradeButtonPressed()
	{
		UnityEvent onSucceeded = this.scanner.onSucceeded;
		if (onSucceeded != null)
		{
			onSucceeded.Invoke();
		}
		GhostReactorProgression.instance.UnlockProgressionTreeNode(this.toolProgressionManager.GetTreeId(), this.currentlySelectedToolUpgrade.id, this.reactor);
	}

	// Token: 0x060032E6 RID: 13030 RVA: 0x00116FF3 File Offset: 0x001151F3
	public void ResearchCompleted(bool success, string researchID)
	{
		this.UpdateUI();
	}

	// Token: 0x0400420E RID: 16910
	public Color selectedUpgradeColor = Color.yellow;

	// Token: 0x0400420F RID: 16911
	public Color unselectedUpgradeColor = Color.black;

	// Token: 0x04004210 RID: 16912
	public Color lockedToolColor = Color.red;

	// Token: 0x04004211 RID: 16913
	public Color unlockedToolColor = Color.green;

	// Token: 0x04004212 RID: 16914
	private int selectedUpgradeIndex;

	// Token: 0x04004213 RID: 16915
	[SerializeField]
	private IDCardScanner scanner;

	// Token: 0x04004214 RID: 16916
	[SerializeField]
	private TMP_Text BonusText;

	// Token: 0x04004215 RID: 16917
	[SerializeField]
	private TMP_Text CostText;

	// Token: 0x04004216 RID: 16918
	[SerializeField]
	private TMP_Text DescriptionText;

	// Token: 0x04004217 RID: 16919
	[SerializeField]
	private TMP_Text LevelText;

	// Token: 0x04004218 RID: 16920
	[SerializeField]
	private TMP_Text ResearchPointsTex;

	// Token: 0x04004219 RID: 16921
	[SerializeField]
	private TMP_Text RequiredLevelText;

	// Token: 0x0400421A RID: 16922
	[SerializeField]
	private TMP_Text ToolNameText;

	// Token: 0x0400421B RID: 16923
	[SerializeField]
	private TMP_Text UnlockedText;

	// Token: 0x0400421C RID: 16924
	[SerializeField]
	private TMP_Text[] UpgradePointerText;

	// Token: 0x0400421D RID: 16925
	[SerializeField]
	private TMP_Text[] UpgradeTitlesText;

	// Token: 0x0400421E RID: 16926
	[SerializeField]
	private Image[] LockedImage;

	// Token: 0x0400421F RID: 16927
	[SerializeField]
	private GorillaPressableButton[] UpgradeButton;

	// Token: 0x04004220 RID: 16928
	private string _costString;

	// Token: 0x04004221 RID: 16929
	private string _levelString;

	// Token: 0x04004222 RID: 16930
	private string _researchPointsString;

	// Token: 0x04004223 RID: 16931
	private string _requiredLevelString;

	// Token: 0x04004224 RID: 16932
	private int selectedToolIndex;

	// Token: 0x04004225 RID: 16933
	private int totalTools;

	// Token: 0x04004226 RID: 16934
	[NonSerialized]
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04004227 RID: 16935
	[NonSerialized]
	private List<GRTool.GRToolType> supportedTools = new List<GRTool.GRToolType>();

	// Token: 0x04004228 RID: 16936
	[NonSerialized]
	private List<GRToolProgressionTree.GRToolProgressionNode> selectedToolUpgrades = new List<GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04004229 RID: 16937
	[NonSerialized]
	private GRToolProgressionTree.GRToolProgressionNode currentlySelectedToolUpgrade = new GRToolProgressionTree.GRToolProgressionNode();

	// Token: 0x0400422A RID: 16938
	[NonSerialized]
	private GRToolProgressionManager.ToolProgressionMetaData currentlySelectedUpgradeMetadata = new GRToolProgressionManager.ToolProgressionMetaData();

	// Token: 0x0400422B RID: 16939
	[NonSerialized]
	private GhostReactor reactor;
}
