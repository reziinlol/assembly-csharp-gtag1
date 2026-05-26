using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000BA1 RID: 2977
public class KIDUI_Controller : MonoBehaviour
{
	// Token: 0x17000709 RID: 1801
	// (get) Token: 0x06004AC8 RID: 19144 RVA: 0x0018FA8A File Offset: 0x0018DC8A
	public static KIDUI_Controller Instance
	{
		get
		{
			return KIDUI_Controller._instance;
		}
	}

	// Token: 0x1700070A RID: 1802
	// (get) Token: 0x06004AC9 RID: 19145 RVA: 0x0018FA91 File Offset: 0x0018DC91
	public static bool IsKIDUIActive
	{
		get
		{
			return !(KIDUI_Controller.Instance == null) && KIDUI_Controller.Instance._isKidUIActive;
		}
	}

	// Token: 0x1700070B RID: 1803
	// (get) Token: 0x06004ACA RID: 19146 RVA: 0x0018FAAC File Offset: 0x0018DCAC
	private static string EtagOnCloseBlackScreenPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr))
			{
				KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr = "closeBlackScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr;
		}
	}

	// Token: 0x06004ACB RID: 19147 RVA: 0x0018FADA File Offset: 0x0018DCDA
	private void Awake()
	{
		KIDUI_Controller._instance = this;
		Debug.LogFormat("[KID::UI::CONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	// Token: 0x06004ACC RID: 19148 RVA: 0x0018FAF1 File Offset: 0x0018DCF1
	private void OnDestroy()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004ACD RID: 19149 RVA: 0x0018FB14 File Offset: 0x0018DD14
	public Task StartKIDScreens(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<StartKIDScreens>d__20 <StartKIDScreens>d__;
		<StartKIDScreens>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDScreens>d__.<>4__this = this;
		<StartKIDScreens>d__.cancellationToken = cancellationToken;
		<StartKIDScreens>d__.<>1__state = -1;
		<StartKIDScreens>d__.<>t__builder.Start<KIDUI_Controller.<StartKIDScreens>d__20>(ref <StartKIDScreens>d__);
		return <StartKIDScreens>d__.<>t__builder.Task;
	}

	// Token: 0x06004ACE RID: 19150 RVA: 0x0018FB60 File Offset: 0x0018DD60
	public void CloseKIDScreens()
	{
		this.SaveEtagOnCloseScreen();
		this._isKidUIActive = false;
		this._mainKIDScreen.HideMainScreen();
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		Object.DestroyImmediate(base.gameObject);
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	// Token: 0x06004ACF RID: 19151 RVA: 0x0018FBD8 File Offset: 0x0018DDD8
	public void UpdateScreenStatus()
	{
		EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
		KIDUI_MainScreen mainKIDScreen = this._mainKIDScreen;
		if (mainKIDScreen == null)
		{
			return;
		}
		mainKIDScreen.UpdateScreenStatus(screenStatusFromSession, true);
	}

	// Token: 0x06004AD0 RID: 19152 RVA: 0x0018FC00 File Offset: 0x0018DE00
	public void NotifyOfEmailResult(bool success)
	{
		if (this._confirmScreen == null)
		{
			Debug.LogError("[KID::UI_CONTROLLER] _confirmScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		if (success)
		{
			PlayerPrefs.SetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 1);
			PlayerPrefs.Save();
		}
		Debug.Log("[KID::UI_CONTROLLER] Notifying user about email result. Showing confirm screen.");
		this._confirmScreen.NotifyOfResult(success);
	}

	// Token: 0x06004AD1 RID: 19153 RVA: 0x0018FC50 File Offset: 0x0018DE50
	private EMainScreenStatus GetScreenStatusFromSession()
	{
		EMainScreenStatus result;
		switch (KIDManager.CurrentSession.SessionStatus)
		{
		case SessionStatus.PASS:
			if (this.ShouldShowScreenOnPermissionChange())
			{
				result = EMainScreenStatus.Updated;
			}
			else if (KIDManager.PreviousStatus == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				result = EMainScreenStatus.Declined;
			}
			else
			{
				result = EMainScreenStatus.Missing;
			}
			break;
		case SessionStatus.PROHIBITED:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Status is PROHIBITED but is trying to show k-ID screens");
			result = EMainScreenStatus.Declined;
			break;
		case SessionStatus.CHALLENGE:
		case SessionStatus.CHALLENGE_SESSION_UPGRADE:
		case SessionStatus.PENDING_AGE_APPEAL:
			if (string.IsNullOrEmpty(PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "")))
			{
				result = EMainScreenStatus.Setup;
			}
			else
			{
				result = EMainScreenStatus.Pending;
			}
			break;
		default:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Unknown status");
			result = EMainScreenStatus.None;
			break;
		}
		return result;
	}

	// Token: 0x06004AD2 RID: 19154 RVA: 0x0018FCDC File Offset: 0x0018DEDC
	private Task<bool> ShouldShowKIDScreen(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<ShouldShowKIDScreen>d__25 <ShouldShowKIDScreen>d__;
		<ShouldShowKIDScreen>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ShouldShowKIDScreen>d__.<>4__this = this;
		<ShouldShowKIDScreen>d__.cancellationToken = cancellationToken;
		<ShouldShowKIDScreen>d__.<>1__state = -1;
		<ShouldShowKIDScreen>d__.<>t__builder.Start<KIDUI_Controller.<ShouldShowKIDScreen>d__25>(ref <ShouldShowKIDScreen>d__);
		return <ShouldShowKIDScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06004AD3 RID: 19155 RVA: 0x0018FD27 File Offset: 0x0018DF27
	private bool ShouldShowScreenOnPermissionChange()
	{
		this._lastEtagOnClose = this.GetLastBlackScreenEtag();
		string lastEtagOnClose = this._lastEtagOnClose;
		TMPSession currentSession = KIDManager.CurrentSession;
		return lastEtagOnClose != (((currentSession != null) ? currentSession.Etag : null) ?? string.Empty);
	}

	// Token: 0x06004AD4 RID: 19156 RVA: 0x0018FD5A File Offset: 0x0018DF5A
	private string GetLastBlackScreenEtag()
	{
		return PlayerPrefs.GetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, "");
	}

	// Token: 0x06004AD5 RID: 19157 RVA: 0x0018FD6B File Offset: 0x0018DF6B
	private void SaveEtagOnCloseScreen()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] Trying to save Pre-Game Screen ETAG, but [CurrentSession] is null");
			return;
		}
		PlayerPrefs.SetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, KIDManager.CurrentSession.Etag);
		PlayerPrefs.Save();
	}

	// Token: 0x06004AD6 RID: 19158 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005DAD RID: 23981
	private const string CLOSE_BLACK_SCREEN_ETAG_PLAYER_PREF_PREFIX = "closeBlackScreen-";

	// Token: 0x04005DAE RID: 23982
	private const string FIRST_TIME_POST_CHANGE_PLAYER_PREF = "hasShownFirstTimePostChange-";

	// Token: 0x04005DAF RID: 23983
	private static KIDUI_Controller _instance;

	// Token: 0x04005DB0 RID: 23984
	[SerializeField]
	private KIDUI_MainScreen _mainKIDScreen;

	// Token: 0x04005DB1 RID: 23985
	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	// Token: 0x04005DB2 RID: 23986
	[SerializeField]
	private List<string> _PermissionsWithToggles = new List<string>();

	// Token: 0x04005DB3 RID: 23987
	[SerializeField]
	private List<EKIDFeatures> _inaccessibleSettings = new List<EKIDFeatures>
	{
		EKIDFeatures.Multiplayer,
		EKIDFeatures.Mods
	};

	// Token: 0x04005DB4 RID: 23988
	private KIDUI_Controller.Metrics_ShowReason _showReason;

	// Token: 0x04005DB5 RID: 23989
	private bool _isKidUIActive;

	// Token: 0x04005DB6 RID: 23990
	private static string etagOnCloseBlackScreenPlayerPrefStr;

	// Token: 0x04005DB7 RID: 23991
	private string _lastEtagOnClose;

	// Token: 0x02000BA2 RID: 2978
	public enum Metrics_ShowReason
	{
		// Token: 0x04005DB9 RID: 23993
		None,
		// Token: 0x04005DBA RID: 23994
		Inaccessible,
		// Token: 0x04005DBB RID: 23995
		Guardian_Disabled,
		// Token: 0x04005DBC RID: 23996
		Permissions_Changed,
		// Token: 0x04005DBD RID: 23997
		Default_Session,
		// Token: 0x04005DBE RID: 23998
		No_Session
	}
}
