using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000B78 RID: 2936
public class KIDMessagingController : MonoBehaviour
{
	// Token: 0x170006FB RID: 1787
	// (get) Token: 0x060049E6 RID: 18918 RVA: 0x0018BA77 File Offset: 0x00189C77
	private static string HasShownConfirmationScreenPlayerPref
	{
		get
		{
			return "hasShownKIDConfirmationScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x060049E7 RID: 18919 RVA: 0x0018BA8F File Offset: 0x00189C8F
	public void OnConfirmPressed()
	{
		this._closeMessageBox = true;
	}

	// Token: 0x060049E8 RID: 18920 RVA: 0x0018BA98 File Offset: 0x00189C98
	private void Awake()
	{
		if (KIDMessagingController.instance != null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to start a new [KIDMessagingController] but one already exists");
			Object.Destroy(this);
			return;
		}
		KIDMessagingController.instance = this;
	}

	// Token: 0x060049E9 RID: 18921 RVA: 0x0018BABE File Offset: 0x00189CBE
	private bool ShouldShowConfirmationScreen()
	{
		return !KIDManager.CurrentSession.IsDefault;
	}

	// Token: 0x060049EA RID: 18922 RVA: 0x0018BAD0 File Offset: 0x00189CD0
	private Task StartKIDConfirmationScreenInternal(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreenInternal>d__18 <StartKIDConfirmationScreenInternal>d__;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreenInternal>d__.<>4__this = this;
		<StartKIDConfirmationScreenInternal>d__.token = token;
		<StartKIDConfirmationScreenInternal>d__.<>1__state = -1;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreenInternal>d__18>(ref <StartKIDConfirmationScreenInternal>d__);
		return <StartKIDConfirmationScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x060049EB RID: 18923 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager kidaudioManager = KIDAudioManager.Instance;
		if (kidaudioManager == null)
		{
			return;
		}
		kidaudioManager.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x060049EC RID: 18924 RVA: 0x0018BB1C File Offset: 0x00189D1C
	public static Task StartKIDConfirmationScreen(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreen>d__20 <StartKIDConfirmationScreen>d__;
		<StartKIDConfirmationScreen>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreen>d__.token = token;
		<StartKIDConfirmationScreen>d__.<>1__state = -1;
		<StartKIDConfirmationScreen>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreen>d__20>(ref <StartKIDConfirmationScreen>d__);
		return <StartKIDConfirmationScreen>d__.<>t__builder.Task;
	}

	// Token: 0x060049ED RID: 18925 RVA: 0x0018BB60 File Offset: 0x00189D60
	private static Task<string> GetSetupConfirmationMessage()
	{
		KIDMessagingController.<GetSetupConfirmationMessage>d__21 <GetSetupConfirmationMessage>d__;
		<GetSetupConfirmationMessage>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetSetupConfirmationMessage>d__.<>1__state = -1;
		<GetSetupConfirmationMessage>d__.<>t__builder.Start<KIDMessagingController.<GetSetupConfirmationMessage>d__21>(ref <GetSetupConfirmationMessage>d__);
		return <GetSetupConfirmationMessage>d__.<>t__builder.Task;
	}

	// Token: 0x060049EE RID: 18926 RVA: 0x0018BB9C File Offset: 0x00189D9C
	private static string GetConfirmMessageFromTitleDataJson(string jsonTxt)
	{
		if (string.IsNullOrEmpty(jsonTxt))
		{
			Debug.LogError("[KID_MANAGER] Cannot get Confirmation Message. JSON is null or empty!");
			return null;
		}
		KIDMessagingTitleData kidmessagingTitleData = JsonConvert.DeserializeObject<KIDMessagingTitleData>(jsonTxt);
		if (kidmessagingTitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		if (string.IsNullOrEmpty(kidmessagingTitleData.KIDSetupConfirmation))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData] - [KIDSetupConfirmation] is null or empty. Json: \n" + jsonTxt);
			return null;
		}
		return kidmessagingTitleData.KIDSetupConfirmation;
	}

	// Token: 0x060049EF RID: 18927 RVA: 0x0018BC00 File Offset: 0x00189E00
	public static void ShowConnectionErrorScreen()
	{
		if (KIDMessagingController.instance == null || KIDMessagingController.instance.messageBox == null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] No message box");
			return;
		}
		KIDMessagingController.instance._closeMessageBox = false;
		KIDMessagingController.instance.messageBox.Header = "Connection Error";
		KIDMessagingController.instance.messageBox.Body = "Unable to connect to the internet. Please restart the game and try again.";
		KIDMessagingController.instance.messageBox.RightButton = "Quit";
		KIDMessagingController.instance.messageBox.ShowQuitButtonAsPrimary();
		KIDMessagingController.instance.messageBox.RightButtonCallback.RemoveAllListeners();
		KIDMessagingController.instance.messageBox.RightButtonCallback.AddListener(new UnityAction(Application.Quit));
		KIDMessagingController.instance.messageBox.gameObject.SetActive(true);
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(KIDMessagingController.instance.transform);
	}

	// Token: 0x04005CA8 RID: 23720
	private const string SHOWN_CONFIRMATION_SCREEN_PREFIX = "hasShownKIDConfirmationScreen-";

	// Token: 0x04005CA9 RID: 23721
	private const string CONFIRMATION_HEADER = "Thank you";

	// Token: 0x04005CAA RID: 23722
	private const string CONFIRMATION_BODY = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";

	// Token: 0x04005CAB RID: 23723
	private const string CONFIRMATION_BUTTON = "Continue";

	// Token: 0x04005CAC RID: 23724
	private const string KID_SETUP_CONFIRMATION_TITLE_KEY = "KID_SETUP_CONFIRMATION_TITLE";

	// Token: 0x04005CAD RID: 23725
	private const string KID_SETUP_CONFIRMATION_BODY_KEY = "KID_SETUP_CONFIRMATION_BODY";

	// Token: 0x04005CAE RID: 23726
	private const string KID_SETUP_CONFIRMATION_BUTTON_KEY = "KID_SETUP_CONFIRMATION_BUTTON";

	// Token: 0x04005CAF RID: 23727
	private static KIDMessagingController instance;

	// Token: 0x04005CB0 RID: 23728
	[SerializeField]
	private MessageBox messageBox;

	// Token: 0x04005CB1 RID: 23729
	private const string CONNECTION_ERROR_HEADER = "Connection Error";

	// Token: 0x04005CB2 RID: 23730
	private const string CONNECTION_ERROR_BODY = "Unable to connect to the internet. Please restart the game and try again.";

	// Token: 0x04005CB3 RID: 23731
	private const string CONNECTION_ERROR_BUTTON = "Quit";

	// Token: 0x04005CB4 RID: 23732
	private bool _closeMessageBox;
}
