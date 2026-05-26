using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000B67 RID: 2919
internal class MockWarningServer : WarningsServer
{
	// Token: 0x170006F8 RID: 1784
	// (get) Token: 0x06004993 RID: 18835 RVA: 0x00189D44 File Offset: 0x00187F44
	public static string ShownScreenPlayerPref
	{
		get
		{
			return "screen-shown-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x06004994 RID: 18836 RVA: 0x00189D5C File Offset: 0x00187F5C
	private void Awake()
	{
		if (WarningsServer.Instance == null)
		{
			WarningsServer.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06004995 RID: 18837 RVA: 0x00189D7C File Offset: 0x00187F7C
	private PlayerAgeGateWarningStatus CreateWarningStatus(string header, string body, MockWarningServer.ButtonSetup? leftButtonSetup, MockWarningServer.ButtonSetup? rightButtonSetup, EImageVisibility showImage, Action leftButtonCallback, Action rightButtonCallback)
	{
		PlayerAgeGateWarningStatus result;
		result.header = header;
		result.body = body;
		result.leftButtonText = string.Empty;
		result.rightButtonText = string.Empty;
		result.leftButtonResult = WarningButtonResult.None;
		result.rightButtonResult = WarningButtonResult.None;
		result.noWarningResult = WarningButtonResult.None;
		result.showImage = showImage;
		result.onLeftButtonPressedAction = leftButtonCallback;
		result.onRightButtonPressedAction = rightButtonCallback;
		if (leftButtonSetup != null)
		{
			result.leftButtonText = leftButtonSetup.Value.buttonText;
			result.leftButtonResult = leftButtonSetup.Value.buttonResult;
		}
		if (rightButtonSetup != null)
		{
			result.rightButtonText = rightButtonSetup.Value.buttonText;
			result.rightButtonResult = rightButtonSetup.Value.buttonResult;
		}
		return result;
	}

	// Token: 0x06004996 RID: 18838 RVA: 0x00189E44 File Offset: 0x00188044
	public override Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token)
	{
		MockWarningServer.<FetchPlayerData>d__12 <FetchPlayerData>d__;
		<FetchPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<FetchPlayerData>d__.<>4__this = this;
		<FetchPlayerData>d__.token = token;
		<FetchPlayerData>d__.<>1__state = -1;
		<FetchPlayerData>d__.<>t__builder.Start<MockWarningServer.<FetchPlayerData>d__12>(ref <FetchPlayerData>d__);
		return <FetchPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x06004997 RID: 18839 RVA: 0x00189E90 File Offset: 0x00188090
	public override Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token)
	{
		MockWarningServer.<GetOptInFollowUpMessage>d__13 <GetOptInFollowUpMessage>d__;
		<GetOptInFollowUpMessage>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<GetOptInFollowUpMessage>d__.<>4__this = this;
		<GetOptInFollowUpMessage>d__.token = token;
		<GetOptInFollowUpMessage>d__.<>1__state = -1;
		<GetOptInFollowUpMessage>d__.<>t__builder.Start<MockWarningServer.<GetOptInFollowUpMessage>d__13>(ref <GetOptInFollowUpMessage>d__);
		return <GetOptInFollowUpMessage>d__.<>t__builder.Task;
	}

	// Token: 0x06004998 RID: 18840 RVA: 0x00189EDC File Offset: 0x001880DC
	private bool ShouldShowWarningScreen(int phase, bool inOptInCohort)
	{
		if (PlayerPrefs.GetInt(string.Format("phase-{0}-{1}", phase, MockWarningServer.ShownScreenPlayerPref), 0) == 0)
		{
			return true;
		}
		switch (phase)
		{
		default:
			return false;
		case 2:
			return inOptInCohort;
		case 3:
		case 4:
		case 5:
			return true;
		}
	}

	// Token: 0x04005C52 RID: 23634
	private const string SHOWN_SCREEN_PREFIX = "screen-shown-";

	// Token: 0x04005C53 RID: 23635
	private const string KID_WARNING_TITLE_KEY = "KID_WARNING_TITLE";

	// Token: 0x04005C54 RID: 23636
	private const string KID_WARNING_CONTINUE_KEY = "KID_WARNING_CONTINUE";

	// Token: 0x04005C55 RID: 23637
	private const string KID_WARNING_PHASE_THREE_IN_COHORT_KEY = "KID_WARNING_PHASE_THREE_IN_COHORT";

	// Token: 0x04005C56 RID: 23638
	private const string KID_WARNING_PHASE_FOUR_RETURNING_PLAYER_KEY = "KID_WARNING_PHASE_FOUR_RETURNING_PLAYER";

	// Token: 0x04005C57 RID: 23639
	private const string KID_WARNING_OPT_IN_FOLLOW_MESSAGE_KEY = "KID_WARNING_OPT_IN_FOLLOW_MESSAGE";

	// Token: 0x04005C58 RID: 23640
	private const string KID_WARNING_FOLLOW_UP_YAY_KEY = "KID_WARNING_FOLLOW_UP_YAY";

	// Token: 0x02000B68 RID: 2920
	public struct ButtonSetup
	{
		// Token: 0x0600499A RID: 18842 RVA: 0x00189F31 File Offset: 0x00188131
		public ButtonSetup(string txt, WarningButtonResult result)
		{
			this.buttonText = txt;
			this.buttonResult = result;
		}

		// Token: 0x04005C59 RID: 23641
		public string buttonText;

		// Token: 0x04005C5A RID: 23642
		public WarningButtonResult buttonResult;
	}
}
