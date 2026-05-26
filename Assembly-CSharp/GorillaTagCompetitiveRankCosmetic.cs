using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000895 RID: 2197
public class GorillaTagCompetitiveRankCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x17000519 RID: 1305
	// (get) Token: 0x0600398D RID: 14733 RVA: 0x00139E3E File Offset: 0x0013803E
	// (set) Token: 0x0600398E RID: 14734 RVA: 0x00139E46 File Offset: 0x00138046
	public bool IsSpawned { get; set; }

	// Token: 0x1700051A RID: 1306
	// (get) Token: 0x0600398F RID: 14735 RVA: 0x00139E4F File Offset: 0x0013804F
	// (set) Token: 0x06003990 RID: 14736 RVA: 0x00139E57 File Offset: 0x00138057
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06003991 RID: 14737 RVA: 0x00139E60 File Offset: 0x00138060
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnRankedSubtierChanged += this.OnRankedScoreChanged;
		this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
	}

	// Token: 0x06003992 RID: 14738 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x00139EC6 File Offset: 0x001380C6
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.UpdateDisplayedCosmetic(-1, -1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x00139EED File Offset: 0x001380ED
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnRankedSubtierChanged -= this.OnRankedScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x00139F22 File Offset: 0x00138122
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06003996 RID: 14742 RVA: 0x00139F34 File Offset: 0x00138134
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnRankedSubtierChanged += this.OnRankedScoreChanged;
			this.OnRankedScoreChanged(this.myRig.GetCurrentRankedSubTier(false), this.myRig.GetCurrentRankedSubTier(true));
			return true;
		}
		return false;
	}

	// Token: 0x06003997 RID: 14743 RVA: 0x00139F9D File Offset: 0x0013819D
	private void OnRankedScoreChanged(int questRank, int pcRank)
	{
		this.UpdateDisplayedCosmetic(questRank, pcRank);
	}

	// Token: 0x06003998 RID: 14744 RVA: 0x00139FA8 File Offset: 0x001381A8
	private void UpdateDisplayedCosmetic(int questRank, int pcRank)
	{
		if (this.rankCosmetics == null)
		{
			return;
		}
		int num = this.usePCELO ? pcRank : questRank;
		if (num <= 0)
		{
			num = 0;
		}
		for (int i = 0; i < this.rankCosmetics.Length; i++)
		{
			this.rankCosmetics[i].SetActive(i == num);
		}
	}

	// Token: 0x04004971 RID: 18801
	[Tooltip("If enabled, display PC rank. Otherwise, display Quest rank")]
	[SerializeField]
	private bool usePCELO;

	// Token: 0x04004972 RID: 18802
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x04004973 RID: 18803
	[SerializeField]
	private VRRig myRig;

	// Token: 0x04004974 RID: 18804
	[SerializeField]
	private GameObject[] rankCosmetics;
}
