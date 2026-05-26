using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000B93 RID: 2963
public class KIDUI_AgeDiscrepancyScreen : MonoBehaviour
{
	// Token: 0x06004A91 RID: 19089 RVA: 0x0018E803 File Offset: 0x0018CA03
	private void Awake()
	{
		this.CheckLocalizationReferences();
	}

	// Token: 0x06004A92 RID: 19090 RVA: 0x0018E80C File Offset: 0x0018CA0C
	public Task ShowAgeDiscrepancyScreenWithAwait(string description)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__8 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.description = description;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__8>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x06004A93 RID: 19091 RVA: 0x0018E858 File Offset: 0x0018CA58
	public Task ShowAgeDiscrepancyScreenWithAwait(int userAge, int accAge, int lowestAge)
	{
		KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__9 <ShowAgeDiscrepancyScreenWithAwait>d__;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>4__this = this;
		<ShowAgeDiscrepancyScreenWithAwait>d__.userAge = userAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.accAge = accAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.lowestAge = lowestAge;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>1__state = -1;
		<ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<ShowAgeDiscrepancyScreenWithAwait>d__9>(ref <ShowAgeDiscrepancyScreenWithAwait>d__);
		return <ShowAgeDiscrepancyScreenWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x06004A94 RID: 19092 RVA: 0x0018E8B4 File Offset: 0x0018CAB4
	private Task WaitForCompletion()
	{
		KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__10 <WaitForCompletion>d__;
		<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForCompletion>d__.<>4__this = this;
		<WaitForCompletion>d__.<>1__state = -1;
		<WaitForCompletion>d__.<>t__builder.Start<KIDUI_AgeDiscrepancyScreen.<WaitForCompletion>d__10>(ref <WaitForCompletion>d__);
		return <WaitForCompletion>d__.<>t__builder.Task;
	}

	// Token: 0x06004A95 RID: 19093 RVA: 0x0018E8F7 File Offset: 0x0018CAF7
	public void OnHoldComplete()
	{
		this._hasCompleted = true;
	}

	// Token: 0x06004A96 RID: 19094 RVA: 0x0018DF22 File Offset: 0x0018C122
	public void OnQuitPressed()
	{
		Application.Quit();
	}

	// Token: 0x06004A97 RID: 19095 RVA: 0x0018E900 File Offset: 0x0018CB00
	private void CheckLocalizationReferences()
	{
		if (this._bodyLocStr != null && this._userAgeVar != null && this._accountAgeVar != null && this._lowestAgeVar != null)
		{
			return;
		}
		if (this._bodyTextLoc == null)
		{
			Debug.LogError("[LOCALIZATION::KIDUI_AGE_DISCREPANCY_SCREEN] [_bodyTextLoc] is not set, unable to localize smart string");
			return;
		}
		this._bodyLocStr = this._bodyTextLoc.StringReference;
		this._userAgeVar = (this._bodyLocStr["user-age"] as IntVariable);
		this._accountAgeVar = (this._bodyLocStr["account-age"] as IntVariable);
		this._lowestAgeVar = (this._bodyLocStr["lowest-age"] as IntVariable);
	}

	// Token: 0x04005D56 RID: 23894
	[SerializeField]
	private TMP_Text _descriptionText;

	// Token: 0x04005D57 RID: 23895
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _bodyTextLoc;

	// Token: 0x04005D58 RID: 23896
	private bool _hasCompleted;

	// Token: 0x04005D59 RID: 23897
	private LocalizedString _bodyLocStr;

	// Token: 0x04005D5A RID: 23898
	private IntVariable _userAgeVar;

	// Token: 0x04005D5B RID: 23899
	private IntVariable _accountAgeVar;

	// Token: 0x04005D5C RID: 23900
	private IntVariable _lowestAgeVar;
}
