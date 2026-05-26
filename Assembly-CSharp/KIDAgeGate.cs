using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000B2A RID: 2858
public class KIDAgeGate : MonoBehaviour
{
	// Token: 0x170006D2 RID: 1746
	// (get) Token: 0x06004875 RID: 18549 RVA: 0x00183416 File Offset: 0x00181616
	public static int UserAge
	{
		get
		{
			return KIDAgeGate._ageValue;
		}
	}

	// Token: 0x170006D3 RID: 1747
	// (get) Token: 0x06004876 RID: 18550 RVA: 0x0018341D File Offset: 0x0018161D
	// (set) Token: 0x06004877 RID: 18551 RVA: 0x00183424 File Offset: 0x00181624
	public static bool DisplayedScreen { get; private set; }

	// Token: 0x06004878 RID: 18552 RVA: 0x0018342C File Offset: 0x0018162C
	private void Awake()
	{
		if (KIDAgeGate._activeReference != null)
		{
			Debug.LogError("[KID::Age_Gate] Age Gate already exists, this is a duplicate, deleting the new one");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		KIDAgeGate._activeReference = this;
	}

	// Token: 0x06004879 RID: 18553 RVA: 0x00183458 File Offset: 0x00181658
	private void Start()
	{
		KIDAgeGate.<Start>d__29 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDAgeGate.<Start>d__29>(ref <Start>d__);
	}

	// Token: 0x0600487A RID: 18554 RVA: 0x00183487 File Offset: 0x00181687
	private void OnDestroy()
	{
		this.requestCancellationSource.Cancel();
	}

	// Token: 0x0600487B RID: 18555 RVA: 0x00183494 File Offset: 0x00181694
	public static Task BeginAgeGate()
	{
		KIDAgeGate.<BeginAgeGate>d__31 <BeginAgeGate>d__;
		<BeginAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<BeginAgeGate>d__.<>1__state = -1;
		<BeginAgeGate>d__.<>t__builder.Start<KIDAgeGate.<BeginAgeGate>d__31>(ref <BeginAgeGate>d__);
		return <BeginAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600487C RID: 18556 RVA: 0x001834D0 File Offset: 0x001816D0
	private Task StartAgeGate()
	{
		KIDAgeGate.<StartAgeGate>d__32 <StartAgeGate>d__;
		<StartAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeGate>d__.<>4__this = this;
		<StartAgeGate>d__.<>1__state = -1;
		<StartAgeGate>d__.<>t__builder.Start<KIDAgeGate.<StartAgeGate>d__32>(ref <StartAgeGate>d__);
		return <StartAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600487D RID: 18557 RVA: 0x00183514 File Offset: 0x00181714
	private Task InitialiseAgeGate()
	{
		KIDAgeGate.<InitialiseAgeGate>d__33 <InitialiseAgeGate>d__;
		<InitialiseAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitialiseAgeGate>d__.<>4__this = this;
		<InitialiseAgeGate>d__.<>1__state = -1;
		<InitialiseAgeGate>d__.<>t__builder.Start<KIDAgeGate.<InitialiseAgeGate>d__33>(ref <InitialiseAgeGate>d__);
		return <InitialiseAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600487E RID: 18558 RVA: 0x00183558 File Offset: 0x00181758
	private Task ProcessAgeGate()
	{
		KIDAgeGate.<ProcessAgeGate>d__34 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ProcessAgeGate>d__.<>4__this = this;
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGate>d__34>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x0600487F RID: 18559 RVA: 0x0018359C File Offset: 0x0018179C
	private Task<bool> ProcessAgeGateConfirmation()
	{
		KIDAgeGate.<ProcessAgeGateConfirmation>d__35 <ProcessAgeGateConfirmation>d__;
		<ProcessAgeGateConfirmation>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ProcessAgeGateConfirmation>d__.<>4__this = this;
		<ProcessAgeGateConfirmation>d__.<>1__state = -1;
		<ProcessAgeGateConfirmation>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGateConfirmation>d__35>(ref <ProcessAgeGateConfirmation>d__);
		return <ProcessAgeGateConfirmation>d__.<>t__builder.Task;
	}

	// Token: 0x06004880 RID: 18560 RVA: 0x001835E0 File Offset: 0x001817E0
	private Task WaitForAgeChoice()
	{
		KIDAgeGate.<WaitForAgeChoice>d__36 <WaitForAgeChoice>d__;
		<WaitForAgeChoice>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAgeChoice>d__.<>4__this = this;
		<WaitForAgeChoice>d__.<>1__state = -1;
		<WaitForAgeChoice>d__.<>t__builder.Start<KIDAgeGate.<WaitForAgeChoice>d__36>(ref <WaitForAgeChoice>d__);
		return <WaitForAgeChoice>d__.<>t__builder.Task;
	}

	// Token: 0x06004881 RID: 18561 RVA: 0x00183623 File Offset: 0x00181823
	public static void OnConfirmAgePressed(int currentAge)
	{
		KIDAgeGate._hasChosenAge = true;
	}

	// Token: 0x06004882 RID: 18562 RVA: 0x0018362B File Offset: 0x0018182B
	private void OnAgeGateCompleted()
	{
		this.FinaliseAgeGateAndContinue();
	}

	// Token: 0x06004883 RID: 18563 RVA: 0x00183633 File Offset: 0x00181833
	private void FinaliseAgeGateAndContinue()
	{
		if (this.requestCancellationSource.IsCancellationRequested)
		{
			return;
		}
		Debug.Log("[KID::AGE_GATE] Age gate completed");
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06004884 RID: 18564 RVA: 0x00183658 File Offset: 0x00181858
	private void QuitGame()
	{
		Debug.Log("[KID] QUIT PRESSED");
		Application.Quit();
	}

	// Token: 0x06004885 RID: 18565 RVA: 0x0018366C File Offset: 0x0018186C
	private void AppealAge()
	{
		KIDAgeGate.<AppealAge>d__41 <AppealAge>d__;
		<AppealAge>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AppealAge>d__.<>4__this = this;
		<AppealAge>d__.<>1__state = -1;
		<AppealAge>d__.<>t__builder.Start<KIDAgeGate.<AppealAge>d__41>(ref <AppealAge>d__);
	}

	// Token: 0x06004886 RID: 18566 RVA: 0x001836A4 File Offset: 0x001818A4
	private void AppealRejected()
	{
		Debug.Log("[KID] APPEAL REJECTED");
		string messageTitle = "UNDER AGE";
		string messageBody = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";
		string messageConfirmation = "Hold any face button to appeal";
		this._pregameMessageReference.ShowMessage(messageTitle, messageBody, messageConfirmation, new Action(this.AppealAge), 0.25f, 0f);
	}

	// Token: 0x06004887 RID: 18567 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RefreshChallengeStatus()
	{
	}

	// Token: 0x06004888 RID: 18568 RVA: 0x001836F1 File Offset: 0x001818F1
	public static void SetAgeGateConfig(GetRequirementsData response)
	{
		KIDAgeGate._ageGateConfig = response;
	}

	// Token: 0x06004889 RID: 18569 RVA: 0x001836FC File Offset: 0x001818FC
	public void OnWhyAgeGateButtonPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"why_age_gate"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		this._uiParent.SetActive(false);
		PrivateUIRoom.AddUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(true);
	}

	// Token: 0x0600488A RID: 18570 RVA: 0x0018379F File Offset: 0x0018199F
	public void OnWhyAgeGateButtonBackPressed()
	{
		this._uiParent.SetActive(true);
		PrivateUIRoom.RemoveUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(false);
	}

	// Token: 0x0600488B RID: 18571 RVA: 0x001837CC File Offset: 0x001819CC
	public void OnLearnMoreAboutKIDPressed()
	{
		this._metrics_LearnMorePressed = true;
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"learn_more_url"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		Application.OpenURL("https://whyagegate.com/");
	}

	// Token: 0x04005AC7 RID: 23239
	private const string LEARN_MORE_URL = "https://whyagegate.com/";

	// Token: 0x04005AC8 RID: 23240
	private const string DEFAULT_AGE_VALUE_STRING = "SET AGE";

	// Token: 0x04005AC9 RID: 23241
	private const int MINIMUM_PLATFORM_AGE = 13;

	// Token: 0x04005ACA RID: 23242
	[Header("Age Gate Settings")]
	[SerializeField]
	private PreGameMessage _pregameMessageReference;

	// Token: 0x04005ACB RID: 23243
	[SerializeField]
	private KIDUI_AgeDiscrepancyScreen _ageDiscrepancyScreen;

	// Token: 0x04005ACC RID: 23244
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x04005ACD RID: 23245
	[SerializeField]
	private AgeSliderWithProgressBar _ageSlider;

	// Token: 0x04005ACE RID: 23246
	[SerializeField]
	private GameObject _confirmationUI;

	// Token: 0x04005ACF RID: 23247
	[SerializeField]
	private KIDAgeGateConfirmation _confirmationUIManager;

	// Token: 0x04005AD0 RID: 23248
	[SerializeField]
	private TMP_Text _confirmationAgeText;

	// Token: 0x04005AD1 RID: 23249
	[SerializeField]
	private GameObject _whyAgeGateScreen;

	// Token: 0x04005AD2 RID: 23250
	private const string strBlockAccessTitle = "UNDER AGE";

	// Token: 0x04005AD3 RID: 23251
	private const string strBlockAccessMessage = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";

	// Token: 0x04005AD4 RID: 23252
	private const string strBlockAccessConfirm = "Hold any face button to appeal";

	// Token: 0x04005AD5 RID: 23253
	private const string strVerifyAgeTitle = "VERIFY AGE";

	// Token: 0x04005AD6 RID: 23254
	private const string strVerifyAgeMessage = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";

	// Token: 0x04005AD7 RID: 23255
	private const string strDiscrepancyMessage = "You entered {0} for your age,\nbut your Meta account says you should be {1}. You could be logged into the wrong Meta account on this device.\n\nWe will use the lowest age ({2})\nif you Continue.";

	// Token: 0x04005AD8 RID: 23256
	private static KIDAgeGate _activeReference;

	// Token: 0x04005AD9 RID: 23257
	private static GetRequirementsData _ageGateConfig;

	// Token: 0x04005ADA RID: 23258
	private static int _ageValue;

	// Token: 0x04005ADB RID: 23259
	private CancellationTokenSource requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04005ADC RID: 23260
	private static bool _hasChosenAge;

	// Token: 0x04005ADE RID: 23262
	private bool _metrics_LearnMorePressed;
}
