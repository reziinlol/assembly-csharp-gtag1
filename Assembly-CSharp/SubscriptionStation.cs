using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000DBC RID: 3516
[Obsolete("DEPRECATED! Use SubscriptionKiosk instead")]
public class SubscriptionStation : MonoBehaviour
{
	// Token: 0x0600563E RID: 22078 RVA: 0x001BFDF8 File Offset: 0x001BDFF8
	private void Awake()
	{
		this.formatString = this.screenText.text;
		this.screenText.text = string.Format(this.formatString, new object[]
		{
			"*",
			"*",
			"*",
			"*"
		});
	}

	// Token: 0x0600563F RID: 22079 RVA: 0x001BFE54 File Offset: 0x001BE054
	private void UpdateScreen()
	{
		Debug.Log(":::SubscriptionStation::UpdateScreen");
		bool flag = SubscriptionManager.GetSubscriptionDetails(VRRig.LocalRig).tier > 0;
		int daysAccrued = SubscriptionManager.GetSubscriptionDetails(VRRig.LocalRig).daysAccrued;
		bool subsOnlyMatchmaking = SubscriptionManager.SubsOnlyMatchmaking;
		bool showGoldNameTag = VRRig.LocalRig.ShowGoldNameTag;
		if (flag)
		{
			this.screenText.text = string.Format(this.formatString, new object[]
			{
				"Y",
				subsOnlyMatchmaking ? "Y" : "N",
				showGoldNameTag ? "Y" : "N",
				daysAccrued
			});
			return;
		}
		this.screenText.text = string.Format(this.formatString, new object[]
		{
			"N",
			"*",
			"*",
			"*"
		});
	}

	// Token: 0x06005640 RID: 22080 RVA: 0x001BFF2E File Offset: 0x001BE12E
	public void ToggleSubscriptionStatus()
	{
		SubscriptionManager.ForceRecheck();
		this.UpdateScreen();
	}

	// Token: 0x06005641 RID: 22081 RVA: 0x001BFF3B File Offset: 0x001BE13B
	public void ToggleSubsOnly()
	{
		SubscriptionManager.SubsOnlyMatchmaking = !SubscriptionManager.SubsOnlyMatchmaking;
		this.UpdateScreen();
	}

	// Token: 0x06005642 RID: 22082 RVA: 0x001BFF50 File Offset: 0x001BE150
	public void ToggleSubsDecoration()
	{
		this.UpdateScreen();
	}

	// Token: 0x04006614 RID: 26132
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x04006615 RID: 26133
	private string formatString;
}
