using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000BA6 RID: 2982
public class KIDUI_EmailSuccess : MonoBehaviour
{
	// Token: 0x06004AE2 RID: 19170 RVA: 0x00190234 File Offset: 0x0018E434
	public void ShowSuccessScreen(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"email_sent"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004AE3 RID: 19171 RVA: 0x001902C8 File Offset: 0x0018E4C8
	public void ShowSuccessScreenAppeal(string email)
	{
		this._emailTxt.text = email;
		base.gameObject.SetActive(true);
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_appeal",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"age_appeal_email_sent"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004AE4 RID: 19172 RVA: 0x0019035B File Offset: 0x0018E55B
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x06004AE5 RID: 19173 RVA: 0x0018DF22 File Offset: 0x0018C122
	public void OnCloseGame()
	{
		Application.Quit();
	}

	// Token: 0x04005DCB RID: 24011
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x04005DCC RID: 24012
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
