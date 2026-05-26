using System;
using System.Collections;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TMPro;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class RotatingQuestBadge : MonoBehaviour, ISpawnable
{
	// Token: 0x17000195 RID: 405
	// (get) Token: 0x0600103C RID: 4156 RVA: 0x00057087 File Offset: 0x00055287
	// (set) Token: 0x0600103D RID: 4157 RVA: 0x0005708F File Offset: 0x0005528F
	public bool IsSpawned { get; set; }

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x0600103E RID: 4158 RVA: 0x00057098 File Offset: 0x00055298
	// (set) Token: 0x0600103F RID: 4159 RVA: 0x000570A0 File Offset: 0x000552A0
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001040 RID: 4160 RVA: 0x000570AC File Offset: 0x000552AC
	public void OnSpawn(VRRig rig)
	{
		if (this.forWardrobe && !this.myRig)
		{
			this.TryGetRig();
			return;
		}
		this.myRig = rig;
		this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
		this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
	}

	// Token: 0x06001041 RID: 4161 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x06001042 RID: 4162 RVA: 0x00057105 File Offset: 0x00055305
	private void OnEnable()
	{
		if (this.forWardrobe)
		{
			this.SetBadgeLevel(-1);
			if (!this.TryGetRig())
			{
				base.StartCoroutine(this.DoFindRig());
			}
		}
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x0005712B File Offset: 0x0005532B
	private void OnDisable()
	{
		if (this.forWardrobe && this.myRig)
		{
			this.myRig.OnQuestScoreChanged -= this.OnProgressScoreChanged;
			this.myRig = null;
		}
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x00057160 File Offset: 0x00055360
	private IEnumerator DoFindRig()
	{
		WaitForSeconds intervalWait = new WaitForSeconds(0.1f);
		while (!this.TryGetRig())
		{
			yield return intervalWait;
		}
		yield break;
	}

	// Token: 0x06001045 RID: 4165 RVA: 0x00057170 File Offset: 0x00055370
	private bool TryGetRig()
	{
		GorillaTagger instance = GorillaTagger.Instance;
		this.myRig = ((instance != null) ? instance.offlineVRRig : null);
		if (this.myRig)
		{
			this.myRig.OnQuestScoreChanged += this.OnProgressScoreChanged;
			this.OnProgressScoreChanged(this.myRig.GetCurrentQuestScore());
			return true;
		}
		return false;
	}

	// Token: 0x06001046 RID: 4166 RVA: 0x000571CC File Offset: 0x000553CC
	private void OnProgressScoreChanged(int score)
	{
		score = Mathf.Clamp(score, 0, 99999);
		this.displayField.text = score.ToString();
		this.UpdateBadge(score);
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x000571F8 File Offset: 0x000553F8
	private void UpdateBadge(int score)
	{
		int num = -1;
		int badgeLevel = -1;
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			if (this.badgeLevels[i].requiredPoints <= score && this.badgeLevels[i].requiredPoints > num)
			{
				num = this.badgeLevels[i].requiredPoints;
				badgeLevel = i;
			}
		}
		this.SetBadgeLevel(badgeLevel);
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x00057260 File Offset: 0x00055460
	private void SetBadgeLevel(int level)
	{
		level = Mathf.Clamp(level, 0, this.badgeLevels.Length - 1);
		for (int i = 0; i < this.badgeLevels.Length; i++)
		{
			this.badgeLevels[i].badge.SetActive(i == level);
		}
	}

	// Token: 0x0400136A RID: 4970
	[SerializeField]
	private TextMeshPro displayField;

	// Token: 0x0400136B RID: 4971
	[SerializeField]
	private bool forWardrobe;

	// Token: 0x0400136C RID: 4972
	[SerializeField]
	private VRRig myRig;

	// Token: 0x0400136D RID: 4973
	[SerializeField]
	private RotatingQuestBadge.BadgeLevel[] badgeLevels;

	// Token: 0x0200025F RID: 607
	[Serializable]
	public struct BadgeLevel
	{
		// Token: 0x04001370 RID: 4976
		public GameObject badge;

		// Token: 0x04001371 RID: 4977
		public int requiredPoints;
	}
}
