using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000165 RID: 357
[Serializable]
public class SITechTreeNode
{
	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x0600096A RID: 2410 RVA: 0x000326B1 File Offset: 0x000308B1
	// (set) Token: 0x0600096B RID: 2411 RVA: 0x000326B9 File Offset: 0x000308B9
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x0600096C RID: 2412 RVA: 0x000326C4 File Offset: 0x000308C4
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			return edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC && (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x000326F0 File Offset: 0x000308F0
	public bool IsAllowed
	{
		get
		{
			return (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x0600096E RID: 2414 RVA: 0x00032701 File Offset: 0x00030901
	public bool IsDispensableGadget
	{
		get
		{
			return this.IsValid && this.unlockedGadgetPrefab && this.IsAllowed;
		}
	}

	// Token: 0x04000B6D RID: 2925
	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	// Token: 0x04000B6E RID: 2926
	public SIUpgradeType upgradeType;

	// Token: 0x04000B6F RID: 2927
	public string nickName;

	// Token: 0x04000B70 RID: 2928
	public string description;

	// Token: 0x04000B71 RID: 2929
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000B72 RID: 2930
	public SIUpgradeType[] parentUpgrades;

	// Token: 0x04000B73 RID: 2931
	public GameEntity unlockedGadgetPrefab;

	// Token: 0x04000B74 RID: 2932
	public SIResource.ResourceCost[] nodeCost;

	// Token: 0x04000B75 RID: 2933
	public bool costOverride;
}
