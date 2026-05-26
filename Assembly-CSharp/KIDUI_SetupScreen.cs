using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

// Token: 0x02000BB3 RID: 2995
public class KIDUI_SetupScreen : MonoBehaviour
{
	// Token: 0x06004B35 RID: 19253 RVA: 0x00192260 File Offset: 0x00190460
	private void Awake()
	{
		if (this._emailInputField == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email Input Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._confirmScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Confirm Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06004B36 RID: 19254 RVA: 0x001922C8 File Offset: 0x001904C8
	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "");
		this._emailInputField.text = @string;
		this._confirmButton.ResetButton();
		this.OnInputChanged(@string);
	}

	// Token: 0x06004B37 RID: 19255 RVA: 0x00192303 File Offset: 0x00190503
	private void OnDisable()
	{
		if (this._keyboard == null)
		{
			return;
		}
		this._keyboard.active = false;
	}

	// Token: 0x06004B38 RID: 19256 RVA: 0x0019231C File Offset: 0x0019051C
	public void OnStartSetup()
	{
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
					"enter_email"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004B39 RID: 19257 RVA: 0x001923A3 File Offset: 0x001905A3
	public void OnInputSelected()
	{
		Debug.LogFormat("[KID::UI::SETUP] Email Input Selected!", Array.Empty<object>());
	}

	// Token: 0x06004B3A RID: 19258 RVA: 0x001923B4 File Offset: 0x001905B4
	public void OnInputChanged(string newVal)
	{
		bool flag = !string.IsNullOrEmpty(newVal);
		if (flag)
		{
			flag = Regex.IsMatch(newVal, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
		}
		this._confirmButton.interactable = flag;
	}

	// Token: 0x06004B3B RID: 19259 RVA: 0x001923E6 File Offset: 0x001905E6
	public void OnSubmitEmailPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._confirmScreen.OnEmailSubmitted(this._emailInputField.text);
	}

	// Token: 0x06004B3C RID: 19260 RVA: 0x00192424 File Offset: 0x00190624
	public void OnBackPressed()
	{
		PlayerPrefs.SetString(KIDManager.GetEmailForUserPlayerPrefRef, this._emailInputField.text);
		PlayerPrefs.Save();
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Previous);
	}

	// Token: 0x04005E2D RID: 24109
	[SerializeField]
	private TMP_InputField _emailInputField;

	// Token: 0x04005E2E RID: 24110
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005E2F RID: 24111
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04005E30 RID: 24112
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005E31 RID: 24113
	[SerializeField]
	private TMP_Text _riftKeyboardMessage;

	// Token: 0x04005E32 RID: 24114
	private string _emailStr = string.Empty;

	// Token: 0x04005E33 RID: 24115
	private TouchScreenKeyboard _keyboard;
}
