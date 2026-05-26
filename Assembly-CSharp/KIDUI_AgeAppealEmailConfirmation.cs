using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000B8F RID: 2959
public class KIDUI_AgeAppealEmailConfirmation : MonoBehaviour
{
	// Token: 0x06004A80 RID: 19072 RVA: 0x0018E0D1 File Offset: 0x0018C2D1
	private void OnEnable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Combine(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004A81 RID: 19073 RVA: 0x0018E0F3 File Offset: 0x0018C2F3
	private void OnDisable()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004A82 RID: 19074 RVA: 0x0018E128 File Offset: 0x0018C328
	public void ShowAgeAppealConfirmationScreen(bool hasChallenge, int newAge, string emailToConfirm)
	{
		this.hasChallenge = hasChallenge;
		this.newAgeToAppeal = newAge;
		this._confirmText.text = (this.hasChallenge ? this.CONFIRM_PARENT_EMAIL : this.CONFIRM_YOUR_EMAIL);
		this._emailText.text = emailToConfirm;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004A83 RID: 19075 RVA: 0x0018E17C File Offset: 0x0018C37C
	public void OnConfirmPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{
					"button_pressed",
					"confirm"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		if (this.hasChallenge)
		{
			this.StartAgeAppealChallengeEmail();
			return;
		}
		this.StartAgeAppealEmail();
	}

	// Token: 0x06004A84 RID: 19076 RVA: 0x0018E22C File Offset: 0x0018C42C
	public void OnBackPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_age_appeal_confirm_email",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"email_type",
					this.hasChallenge ? "under_dac" : "over_dac"
				},
				{
					"button_pressed",
					"go_back"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAgeToAppeal);
	}

	// Token: 0x06004A85 RID: 19077 RVA: 0x0018E2EC File Offset: 0x0018C4EC
	private void StartAgeAppealChallengeEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16 <StartAgeAppealChallengeEmail>d__;
		<StartAgeAppealChallengeEmail>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartAgeAppealChallengeEmail>d__.<>4__this = this;
		<StartAgeAppealChallengeEmail>d__.<>1__state = -1;
		<StartAgeAppealChallengeEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealChallengeEmail>d__16>(ref <StartAgeAppealChallengeEmail>d__);
	}

	// Token: 0x06004A86 RID: 19078 RVA: 0x0018E324 File Offset: 0x0018C524
	private Task StartAgeAppealEmail()
	{
		KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealEmail>d__17 <StartAgeAppealEmail>d__;
		<StartAgeAppealEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeAppealEmail>d__.<>4__this = this;
		<StartAgeAppealEmail>d__.<>1__state = -1;
		<StartAgeAppealEmail>d__.<>t__builder.Start<KIDUI_AgeAppealEmailConfirmation.<StartAgeAppealEmail>d__17>(ref <StartAgeAppealEmail>d__);
		return <StartAgeAppealEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06004A87 RID: 19079 RVA: 0x0018E368 File Offset: 0x0018C568
	private void NotifyOfEmailResult(bool success)
	{
		if (this._successScreen == null)
		{
			Debug.LogError("[KID::AGE_APPEAL_EMAIL] _successScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		this._hasCompletedSendEmailRequest = true;
		if (success)
		{
			base.gameObject.SetActive(false);
			this._successScreen.ShowSuccessScreenAppeal(this._emailText.text);
			return;
		}
	}

	// Token: 0x06004A88 RID: 19080 RVA: 0x0018E3BB File Offset: 0x0018C5BB
	private void ShowErrorScreen()
	{
		Debug.LogErrorFormat("[KID::UI::Setup] K-ID Confirmation Failed - Failed to send email", Array.Empty<object>());
		base.gameObject.SetActive(false);
		this._errorScreen.ShowAgeAppealEmailErrorScreen(this.hasChallenge, this.newAgeToAppeal, this._emailText.text);
	}

	// Token: 0x04005D3C RID: 23868
	[SerializeField]
	private TMP_Text _confirmText;

	// Token: 0x04005D3D RID: 23869
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x04005D3E RID: 23870
	private string CONFIRM_PARENT_EMAIL = "Please confirm your parent or guardian's email address.";

	// Token: 0x04005D3F RID: 23871
	private string CONFIRM_YOUR_EMAIL = "Please confirm your email address.";

	// Token: 0x04005D40 RID: 23872
	private bool hasChallenge = true;

	// Token: 0x04005D41 RID: 23873
	private int newAgeToAppeal;

	// Token: 0x04005D42 RID: 23874
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x04005D43 RID: 23875
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04005D44 RID: 23876
	[SerializeField]
	private KIDUI_AgeAppealEmailError _errorScreen;

	// Token: 0x04005D45 RID: 23877
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04005D46 RID: 23878
	[SerializeField]
	private int _minimumDelay = 1000;
}
