using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000B39 RID: 2873
public class KIDManager : MonoBehaviour
{
	// Token: 0x170006DB RID: 1755
	// (get) Token: 0x060048BC RID: 18620 RVA: 0x0018475B File Offset: 0x0018295B
	public static KIDManager Instance
	{
		get
		{
			return KIDManager._instance;
		}
	}

	// Token: 0x170006DC RID: 1756
	// (get) Token: 0x060048BD RID: 18621 RVA: 0x00184762 File Offset: 0x00182962
	// (set) Token: 0x060048BE RID: 18622 RVA: 0x00184769 File Offset: 0x00182969
	public static bool InitialisationComplete { get; private set; } = false;

	// Token: 0x170006DD RID: 1757
	// (get) Token: 0x060048BF RID: 18623 RVA: 0x00184771 File Offset: 0x00182971
	// (set) Token: 0x060048C0 RID: 18624 RVA: 0x00184778 File Offset: 0x00182978
	public static bool InitialisationSuccessful { get; private set; } = false;

	// Token: 0x170006DE RID: 1758
	// (get) Token: 0x060048C1 RID: 18625 RVA: 0x00184780 File Offset: 0x00182980
	// (set) Token: 0x060048C2 RID: 18626 RVA: 0x00184787 File Offset: 0x00182987
	public static TMPSession CurrentSession { get; private set; }

	// Token: 0x170006DF RID: 1759
	// (get) Token: 0x060048C3 RID: 18627 RVA: 0x0018478F File Offset: 0x0018298F
	// (set) Token: 0x060048C4 RID: 18628 RVA: 0x00184796 File Offset: 0x00182996
	public static SessionStatus PreviousStatus { get; private set; }

	// Token: 0x170006E0 RID: 1760
	// (get) Token: 0x060048C5 RID: 18629 RVA: 0x0018479E File Offset: 0x0018299E
	// (set) Token: 0x060048C6 RID: 18630 RVA: 0x001847A5 File Offset: 0x001829A5
	public static GetRequirementsData _ageGateRequirements { get; private set; }

	// Token: 0x170006E1 RID: 1761
	// (get) Token: 0x060048C7 RID: 18631 RVA: 0x001847AD File Offset: 0x001829AD
	public static bool KidTitleDataReady
	{
		get
		{
			return KIDManager._titleDataReady;
		}
	}

	// Token: 0x170006E2 RID: 1762
	// (get) Token: 0x060048C8 RID: 18632 RVA: 0x001847B4 File Offset: 0x001829B4
	public static bool KidEnabled
	{
		get
		{
			return KIDManager.KidTitleDataReady && KIDManager._useKid;
		}
	}

	// Token: 0x170006E3 RID: 1763
	// (get) Token: 0x060048C9 RID: 18633 RVA: 0x001847C4 File Offset: 0x001829C4
	public static bool KidEnabledAndReady
	{
		get
		{
			return KIDManager.KidEnabled && KIDManager.InitialisationSuccessful;
		}
	}

	// Token: 0x170006E4 RID: 1764
	// (get) Token: 0x060048CA RID: 18634 RVA: 0x001847D4 File Offset: 0x001829D4
	public static bool HasSession
	{
		get
		{
			return KIDManager.CurrentSession != null && KIDManager.CurrentSession.SessionId != Guid.Empty;
		}
	}

	// Token: 0x170006E5 RID: 1765
	// (get) Token: 0x060048CB RID: 18635 RVA: 0x001847F3 File Offset: 0x001829F3
	public static string PreviousStatusPlayerPrefRef
	{
		get
		{
			return "previous-status-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x170006E6 RID: 1766
	// (get) Token: 0x060048CC RID: 18636 RVA: 0x0018480B File Offset: 0x00182A0B
	// (set) Token: 0x060048CD RID: 18637 RVA: 0x00184812 File Offset: 0x00182A12
	public static bool HasOptedInToKID { get; private set; }

	// Token: 0x170006E7 RID: 1767
	// (get) Token: 0x060048CE RID: 18638 RVA: 0x0018481A File Offset: 0x00182A1A
	private static string KIDSetupPlayerPref
	{
		get
		{
			return "KID-Setup-";
		}
	}

	// Token: 0x170006E8 RID: 1768
	// (get) Token: 0x060048CF RID: 18639 RVA: 0x00184821 File Offset: 0x00182A21
	// (set) Token: 0x060048D0 RID: 18640 RVA: 0x00184828 File Offset: 0x00182A28
	public static string DbgLocale { get; set; }

	// Token: 0x170006E9 RID: 1769
	// (get) Token: 0x060048D1 RID: 18641 RVA: 0x00184830 File Offset: 0x00182A30
	public static string DebugKIDLocalePlayerPrefRef
	{
		get
		{
			return KIDManager._debugKIDLocalePlayerPrefRef;
		}
	}

	// Token: 0x170006EA RID: 1770
	// (get) Token: 0x060048D2 RID: 18642 RVA: 0x00184837 File Offset: 0x00182A37
	public static string GetEmailForUserPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDManager.parentEmailForUserPlayerPrefRef))
			{
				KIDManager.parentEmailForUserPlayerPrefRef = "k-id_EmailAddress" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDManager.parentEmailForUserPlayerPrefRef;
		}
	}

	// Token: 0x170006EB RID: 1771
	// (get) Token: 0x060048D3 RID: 18643 RVA: 0x00184865 File Offset: 0x00182A65
	public static string GetChallengedBeforePlayerPrefRef
	{
		get
		{
			return "k-id_ChallengedBefore" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	// Token: 0x060048D4 RID: 18644 RVA: 0x00184880 File Offset: 0x00182A80
	private void Awake()
	{
		if (KIDManager._instance != null)
		{
			Debug.LogError("Trying to create new instance of [KIDManager], but one already exists. Destroying object [" + base.gameObject.name + "].");
			Object.Destroy(base.gameObject);
			return;
		}
		KIDManager._instance = this;
		KIDManager.DbgLocale = PlayerPrefs.GetString(KIDManager._debugKIDLocalePlayerPrefRef, "");
	}

	// Token: 0x060048D5 RID: 18645 RVA: 0x001848E0 File Offset: 0x00182AE0
	private void Start()
	{
		KIDManager.<Start>d__70 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDManager.<Start>d__70>(ref <Start>d__);
	}

	// Token: 0x060048D6 RID: 18646 RVA: 0x0018490F File Offset: 0x00182B0F
	private void OnDestroy()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x060048D7 RID: 18647 RVA: 0x0018491C File Offset: 0x00182B1C
	public static string GetActiveAccountStatusNiceString()
	{
		switch (KIDManager.GetActiveAccountStatus())
		{
		case AgeStatusType.DIGITALMINOR:
			return "Digital Minor";
		case AgeStatusType.DIGITALYOUTH:
			return "Digital Youth";
		case AgeStatusType.LEGALADULT:
			return "Legal Adult";
		default:
			return "UNKNOWN";
		}
	}

	// Token: 0x060048D8 RID: 18648 RVA: 0x0018495C File Offset: 0x00182B5C
	public static AgeStatusType GetActiveAccountStatus()
	{
		if (KIDManager.CurrentSession != null)
		{
			return KIDManager.CurrentSession.AgeStatus;
		}
		if (!PlayFabAuthenticator.instance.GetSafety())
		{
			return AgeStatusType.LEGALADULT;
		}
		return AgeStatusType.DIGITALMINOR;
	}

	// Token: 0x060048D9 RID: 18649 RVA: 0x00184981 File Offset: 0x00182B81
	public static List<Permission> GetAllPermissionsData()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::MANAGER] There is no current session. Unless the age-gate has not yet finished there should always be a session even if it is the default session");
			return new List<Permission>();
		}
		return KIDManager.CurrentSession.GetAllPermissions();
	}

	// Token: 0x060048DA RID: 18650 RVA: 0x001849A4 File Offset: 0x00182BA4
	public static bool TryGetAgeStatusTypeFromAge(int age, out AgeStatusType ageType)
	{
		if (KIDManager._ageGateRequirements == null)
		{
			Debug.LogError("[KID::MANAGER] [_ageGateRequirements] is not set - need to Get AgeGate Requirements first");
			ageType = AgeStatusType.DIGITALMINOR;
			return false;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.DigitalConsentAge)
		{
			ageType = AgeStatusType.DIGITALMINOR;
			return true;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.CivilAge)
		{
			ageType = AgeStatusType.DIGITALYOUTH;
			return true;
		}
		ageType = AgeStatusType.LEGALADULT;
		return true;
	}

	// Token: 0x060048DB RID: 18651 RVA: 0x001849FC File Offset: 0x00182BFC
	[return: TupleElementNames(new string[]
	{
		"requiresOptIn",
		"hasOptedInPreviously"
	})]
	public static ValueTuple<bool, bool> CheckFeatureOptIn(EKIDFeatures feature, Permission permissionData = null)
	{
		if (permissionData == null)
		{
			permissionData = KIDManager.GetPermissionDataByFeature(feature);
			if (permissionData == null)
			{
				Debug.LogError("[KID::MANAGER] Unable to retrieve permission data for feature [" + feature.ToStandardisedString() + "]");
				return new ValueTuple<bool, bool>(false, false);
			}
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		bool item = true;
		if (KIDManager.CurrentSession != null)
		{
			item = KIDManager.CurrentSession.HasOptedInToPermission(feature);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			return new ValueTuple<bool, bool>(false, item);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PLAYER && permissionData.Enabled)
		{
			return new ValueTuple<bool, bool>(false, true);
		}
		return new ValueTuple<bool, bool>(true, item);
	}

	// Token: 0x060048DC RID: 18652 RVA: 0x00184A90 File Offset: 0x00182C90
	public static void SetFeatureOptIn(EKIDFeatures feature, bool optedIn)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID] Trying to set Feature Opt in for feature [" + feature.ToStandardisedString() + "] but permission data could not be found. Assumed is opt-in", Array.Empty<object>());
			return;
		}
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] CurrentSession is null, cannot set feature opt-in. Returning.");
			return;
		}
		switch (permissionDataByFeature.ManagedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			KIDManager.CurrentSession.OptInToPermission(feature, optedIn);
			return;
		case Permission.ManagedByEnum.GUARDIAN:
			KIDManager.CurrentSession.OptInToPermission(feature, permissionDataByFeature.Enabled);
			return;
		case Permission.ManagedByEnum.PROHIBITED:
			KIDManager.CurrentSession.OptInToPermission(feature, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060048DD RID: 18653 RVA: 0x00184B20 File Offset: 0x00182D20
	public static bool CheckFeatureSettingEnabled(EKIDFeatures feature)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogError("[KID::MANAGER] Unable to permissions for feature [" + feature.ToStandardisedString() + "]");
			return false;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return false;
		}
		bool item = KIDManager.CheckFeatureOptIn(feature, null).Item2;
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
		case EKIDFeatures.Mods:
			return item;
		case EKIDFeatures.Custom_Nametags:
			return item && GorillaComputer.instance.NametagsEnabled;
		case EKIDFeatures.Voice_Chat:
			return item && GorillaComputer.instance.CheckVoiceChatEnabled();
		case EKIDFeatures.Groups:
			return permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN || permissionDataByFeature.Enabled;
		default:
			Debug.LogError("[KID::MANAGER] Tried finding feature setting for [" + feature.ToStandardisedString() + "] but failed.");
			return false;
		}
	}

	// Token: 0x060048DE RID: 18654 RVA: 0x00184BDC File Offset: 0x00182DDC
	private static Task<GetPlayerData_Data> TryGetPlayerData(bool forceRefresh)
	{
		KIDManager.<TryGetPlayerData>d__81 <TryGetPlayerData>d__;
		<TryGetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<TryGetPlayerData>d__.forceRefresh = forceRefresh;
		<TryGetPlayerData>d__.<>1__state = -1;
		<TryGetPlayerData>d__.<>t__builder.Start<KIDManager.<TryGetPlayerData>d__81>(ref <TryGetPlayerData>d__);
		return <TryGetPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x060048DF RID: 18655 RVA: 0x00184C20 File Offset: 0x00182E20
	private static Task<GetRequirementsData> TryGetRequirements()
	{
		KIDManager.<TryGetRequirements>d__82 <TryGetRequirements>d__;
		<TryGetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<TryGetRequirements>d__.<>1__state = -1;
		<TryGetRequirements>d__.<>t__builder.Start<KIDManager.<TryGetRequirements>d__82>(ref <TryGetRequirements>d__);
		return <TryGetRequirements>d__.<>t__builder.Task;
	}

	// Token: 0x060048E0 RID: 18656 RVA: 0x00184C5C File Offset: 0x00182E5C
	private static Task<VerifyAgeData> TryVerifyAgeResponse()
	{
		KIDManager.<TryVerifyAgeResponse>d__83 <TryVerifyAgeResponse>d__;
		<TryVerifyAgeResponse>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<TryVerifyAgeResponse>d__.<>1__state = -1;
		<TryVerifyAgeResponse>d__.<>t__builder.Start<KIDManager.<TryVerifyAgeResponse>d__83>(ref <TryVerifyAgeResponse>d__);
		return <TryVerifyAgeResponse>d__.<>t__builder.Task;
	}

	// Token: 0x060048E1 RID: 18657 RVA: 0x00184C98 File Offset: 0x00182E98
	[return: TupleElementNames(new string[]
	{
		"success",
		"exception"
	})]
	private static Task<ValueTuple<bool, string>> TrySendChallengeEmailRequest()
	{
		KIDManager.<TrySendChallengeEmailRequest>d__84 <TrySendChallengeEmailRequest>d__;
		<TrySendChallengeEmailRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendChallengeEmailRequest>d__.<>1__state = -1;
		<TrySendChallengeEmailRequest>d__.<>t__builder.Start<KIDManager.<TrySendChallengeEmailRequest>d__84>(ref <TrySendChallengeEmailRequest>d__);
		return <TrySendChallengeEmailRequest>d__.<>t__builder.Task;
	}

	// Token: 0x060048E2 RID: 18658 RVA: 0x00184CD4 File Offset: 0x00182ED4
	private static Task<bool> TrySendOptInPermissions()
	{
		KIDManager.<TrySendOptInPermissions>d__85 <TrySendOptInPermissions>d__;
		<TrySendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySendOptInPermissions>d__.<>1__state = -1;
		<TrySendOptInPermissions>d__.<>t__builder.Start<KIDManager.<TrySendOptInPermissions>d__85>(ref <TrySendOptInPermissions>d__);
		return <TrySendOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x060048E3 RID: 18659 RVA: 0x00184D10 File Offset: 0x00182F10
	public static Task<ValueTuple<bool, string>> TrySendUpgradeSessionChallengeEmail()
	{
		KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86 <TrySendUpgradeSessionChallengeEmail>d__;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendUpgradeSessionChallengeEmail>d__.<>1__state = -1;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Start<KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86>(ref <TrySendUpgradeSessionChallengeEmail>d__);
		return <TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x060048E4 RID: 18660 RVA: 0x00184D4C File Offset: 0x00182F4C
	public static Task<bool> TrySetHasConfirmedStatus()
	{
		KIDManager.<TrySetHasConfirmedStatus>d__87 <TrySetHasConfirmedStatus>d__;
		<TrySetHasConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySetHasConfirmedStatus>d__.<>1__state = -1;
		<TrySetHasConfirmedStatus>d__.<>t__builder.Start<KIDManager.<TrySetHasConfirmedStatus>d__87>(ref <TrySetHasConfirmedStatus>d__);
		return <TrySetHasConfirmedStatus>d__.<>t__builder.Task;
	}

	// Token: 0x060048E5 RID: 18661 RVA: 0x00184D88 File Offset: 0x00182F88
	public static Task<UpgradeSessionData> TryUpgradeSession(List<string> requestedPermissions)
	{
		KIDManager.<TryUpgradeSession>d__88 <TryUpgradeSession>d__;
		<TryUpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<TryUpgradeSession>d__.requestedPermissions = requestedPermissions;
		<TryUpgradeSession>d__.<>1__state = -1;
		<TryUpgradeSession>d__.<>t__builder.Start<KIDManager.<TryUpgradeSession>d__88>(ref <TryUpgradeSession>d__);
		return <TryUpgradeSession>d__.<>t__builder.Task;
	}

	// Token: 0x060048E6 RID: 18662 RVA: 0x00184DCC File Offset: 0x00182FCC
	public static Task<AttemptAgeUpdateData> TryAttemptAgeUpdate(int age)
	{
		KIDManager.<TryAttemptAgeUpdate>d__89 <TryAttemptAgeUpdate>d__;
		<TryAttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<TryAttemptAgeUpdate>d__.age = age;
		<TryAttemptAgeUpdate>d__.<>1__state = -1;
		<TryAttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<TryAttemptAgeUpdate>d__89>(ref <TryAttemptAgeUpdate>d__);
		return <TryAttemptAgeUpdate>d__.<>t__builder.Task;
	}

	// Token: 0x060048E7 RID: 18663 RVA: 0x00184E10 File Offset: 0x00183010
	public static Task<bool> TryAppealAge(string email, int newAge)
	{
		KIDManager.<TryAppealAge>d__90 <TryAppealAge>d__;
		<TryAppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryAppealAge>d__.email = email;
		<TryAppealAge>d__.newAge = newAge;
		<TryAppealAge>d__.<>1__state = -1;
		<TryAppealAge>d__.<>t__builder.Start<KIDManager.<TryAppealAge>d__90>(ref <TryAppealAge>d__);
		return <TryAppealAge>d__.<>t__builder.Task;
	}

	// Token: 0x060048E8 RID: 18664 RVA: 0x00184E5C File Offset: 0x0018305C
	public static Task UpdateSession(Action<bool> getDataCompleted = null)
	{
		KIDManager.<UpdateSession>d__91 <UpdateSession>d__;
		<UpdateSession>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateSession>d__.getDataCompleted = getDataCompleted;
		<UpdateSession>d__.<>1__state = -1;
		<UpdateSession>d__.<>t__builder.Start<KIDManager.<UpdateSession>d__91>(ref <UpdateSession>d__);
		return <UpdateSession>d__.<>t__builder.Task;
	}

	// Token: 0x060048E9 RID: 18665 RVA: 0x00184EA0 File Offset: 0x001830A0
	private static Task<bool> CheckWarningScreensOptedIn()
	{
		KIDManager.<CheckWarningScreensOptedIn>d__92 <CheckWarningScreensOptedIn>d__;
		<CheckWarningScreensOptedIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<CheckWarningScreensOptedIn>d__.<>1__state = -1;
		<CheckWarningScreensOptedIn>d__.<>t__builder.Start<KIDManager.<CheckWarningScreensOptedIn>d__92>(ref <CheckWarningScreensOptedIn>d__);
		return <CheckWarningScreensOptedIn>d__.<>t__builder.Task;
	}

	// Token: 0x060048EA RID: 18666 RVA: 0x00184EDB File Offset: 0x001830DB
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitialiseBootFlow()
	{
		if (PlayerPrefs.GetInt(KIDManager.KIDSetupPlayerPref, 0) != 0)
		{
			return;
		}
		PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
	}

	// Token: 0x060048EB RID: 18667 RVA: 0x00184EF8 File Offset: 0x001830F8
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void InitialiseKID()
	{
		KIDManager.<InitialiseKID>d__94 <InitialiseKID>d__;
		<InitialiseKID>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<InitialiseKID>d__.<>1__state = -1;
		<InitialiseKID>d__.<>t__builder.Start<KIDManager.<InitialiseKID>d__94>(ref <InitialiseKID>d__);
	}

	// Token: 0x060048EC RID: 18668 RVA: 0x00184F28 File Offset: 0x00183128
	private static bool UpdatePermissions(TMPSession newSession)
	{
		if (newSession == null || !newSession.IsValidSession)
		{
			Debug.LogError("[KID::MANAGER] A NULL or Invalid Session was received!");
			return false;
		}
		KIDManager.CurrentSession = newSession;
		if (KIDUI_Controller.IsKIDUIActive)
		{
			KIDManager.PreviousStatus = KIDManager.CurrentSession.SessionStatus;
			PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
			PlayerPrefs.Save();
		}
		if (!KIDManager.CurrentSession.IsDefault)
		{
			PlayerPrefs.SetInt(KIDManager.KIDSetupPlayerPref, 1);
			PlayerPrefs.Save();
		}
		KIDManager.OnSessionUpdated();
		if (KIDUI_Controller.Instance)
		{
			KIDUI_Controller.Instance.UpdateScreenStatus();
		}
		return true;
	}

	// Token: 0x060048ED RID: 18669 RVA: 0x00184FB4 File Offset: 0x001831B4
	private static void ClearSession()
	{
		KIDManager.CurrentSession = null;
		KIDManager.DeleteStoredPermissions();
	}

	// Token: 0x060048EE RID: 18670 RVA: 0x000028C5 File Offset: 0x00000AC5
	private static void DeleteStoredPermissions()
	{
	}

	// Token: 0x060048EF RID: 18671 RVA: 0x00184FC1 File Offset: 0x001831C1
	public static CancellationTokenSource ResetCancellationToken()
	{
		KIDManager._requestCancellationSource.Dispose();
		KIDManager._requestCancellationSource = new CancellationTokenSource();
		return KIDManager._requestCancellationSource;
	}

	// Token: 0x060048F0 RID: 18672 RVA: 0x00184FDC File Offset: 0x001831DC
	public static Permission GetPermissionDataByFeature(EKIDFeatures feature)
	{
		if (KIDManager.CurrentSession == null)
		{
			if (!PlayFabAuthenticator.instance.GetSafety())
			{
				return new Permission(feature.ToStandardisedString(), true, Permission.ManagedByEnum.PLAYER);
			}
			return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
		}
		else
		{
			Permission result;
			if (!KIDManager.CurrentSession.TryGetPermission(feature, out result))
			{
				Debug.LogError("[KID::MANAGER] Failed to retreive permission from session for [" + feature.ToStandardisedString() + "]. Assuming disabled permission");
				return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
			}
			return result;
		}
	}

	// Token: 0x060048F1 RID: 18673 RVA: 0x0018490F File Offset: 0x00182B0F
	public static void CancelToken()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	// Token: 0x060048F2 RID: 18674 RVA: 0x00185058 File Offset: 0x00183258
	public static Task<bool> UseKID()
	{
		KIDManager.<UseKID>d__101 <UseKID>d__;
		<UseKID>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UseKID>d__.<>1__state = -1;
		<UseKID>d__.<>t__builder.Start<KIDManager.<UseKID>d__101>(ref <UseKID>d__);
		return <UseKID>d__.<>t__builder.Task;
	}

	// Token: 0x060048F3 RID: 18675 RVA: 0x00185094 File Offset: 0x00183294
	public static Task<int> CheckKIDPhase()
	{
		KIDManager.<CheckKIDPhase>d__102 <CheckKIDPhase>d__;
		<CheckKIDPhase>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<CheckKIDPhase>d__.<>1__state = -1;
		<CheckKIDPhase>d__.<>t__builder.Start<KIDManager.<CheckKIDPhase>d__102>(ref <CheckKIDPhase>d__);
		return <CheckKIDPhase>d__.<>t__builder.Task;
	}

	// Token: 0x060048F4 RID: 18676 RVA: 0x001850D0 File Offset: 0x001832D0
	public static Task<DateTime?> CheckKIDNewPlayerDateTime()
	{
		KIDManager.<CheckKIDNewPlayerDateTime>d__103 <CheckKIDNewPlayerDateTime>d__;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder = AsyncTaskMethodBuilder<DateTime?>.Create();
		<CheckKIDNewPlayerDateTime>d__.<>1__state = -1;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder.Start<KIDManager.<CheckKIDNewPlayerDateTime>d__103>(ref <CheckKIDNewPlayerDateTime>d__);
		return <CheckKIDNewPlayerDateTime>d__.<>t__builder.Task;
	}

	// Token: 0x060048F5 RID: 18677 RVA: 0x0018510C File Offset: 0x0018330C
	private static bool GetIsEnabled(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return false;
		}
		bool result;
		if (!bool.TryParse(kidtitleData.KIDEnabled, out result))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDEnabled': [KIDEnabled] to bool.");
			return false;
		}
		return result;
	}

	// Token: 0x060048F6 RID: 18678 RVA: 0x00185154 File Offset: 0x00183354
	private static int GetPhase(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return 0;
		}
		return kidtitleData.KIDPhase;
	}

	// Token: 0x060048F7 RID: 18679 RVA: 0x00185184 File Offset: 0x00183384
	private static DateTime? GetNewPlayerDateTime(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		DateTime value;
		if (!DateTime.TryParse(kidtitleData.KIDNewPlayerIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDNewPlayerIsoTimestamp': [KIDNewPlayerIsoTimestamp] to DateTime.");
			return null;
		}
		return new DateTime?(value);
	}

	// Token: 0x060048F8 RID: 18680 RVA: 0x001851E8 File Offset: 0x001833E8
	public static bool IsAdult()
	{
		return KIDManager.CurrentSession.IsValidSession && KIDManager.CurrentSession.AgeStatus == AgeStatusType.LEGALADULT;
	}

	// Token: 0x060048F9 RID: 18681 RVA: 0x00185208 File Offset: 0x00183408
	public static bool HasAllPermissions()
	{
		List<Permission> allPermissions = KIDManager.CurrentSession.GetAllPermissions();
		for (int i = 0; i < allPermissions.Count; i++)
		{
			if (allPermissions[i].ManagedBy == Permission.ManagedByEnum.GUARDIAN || !allPermissions[i].Enabled)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060048FA RID: 18682 RVA: 0x00185254 File Offset: 0x00183454
	public static Task<bool> SetKIDOptIn()
	{
		KIDManager.<SetKIDOptIn>d__109 <SetKIDOptIn>d__;
		<SetKIDOptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SetKIDOptIn>d__.<>1__state = -1;
		<SetKIDOptIn>d__.<>t__builder.Start<KIDManager.<SetKIDOptIn>d__109>(ref <SetKIDOptIn>d__);
		return <SetKIDOptIn>d__.<>t__builder.Task;
	}

	// Token: 0x060048FB RID: 18683 RVA: 0x00185290 File Offset: 0x00183490
	[return: TupleElementNames(new string[]
	{
		"success",
		"message"
	})]
	public static Task<ValueTuple<bool, string>> SetAndSendEmail(string email)
	{
		KIDManager.<SetAndSendEmail>d__110 <SetAndSendEmail>d__;
		<SetAndSendEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<SetAndSendEmail>d__.email = email;
		<SetAndSendEmail>d__.<>1__state = -1;
		<SetAndSendEmail>d__.<>t__builder.Start<KIDManager.<SetAndSendEmail>d__110>(ref <SetAndSendEmail>d__);
		return <SetAndSendEmail>d__.<>t__builder.Task;
	}

	// Token: 0x060048FC RID: 18684 RVA: 0x001852D4 File Offset: 0x001834D4
	public static Task<bool> SendOptInPermissions()
	{
		KIDManager.<SendOptInPermissions>d__111 <SendOptInPermissions>d__;
		<SendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SendOptInPermissions>d__.<>1__state = -1;
		<SendOptInPermissions>d__.<>t__builder.Start<KIDManager.<SendOptInPermissions>d__111>(ref <SendOptInPermissions>d__);
		return <SendOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x060048FD RID: 18685 RVA: 0x00185310 File Offset: 0x00183510
	public static bool HasPermissionToUseFeature(EKIDFeatures feature)
	{
		if (!KIDManager.KidEnabledAndReady)
		{
			return !PlayFabAuthenticator.instance.GetSafety();
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		return (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
	}

	// Token: 0x060048FE RID: 18686 RVA: 0x0018535C File Offset: 0x0018355C
	private static Task<bool> WaitForAuthentication()
	{
		KIDManager.<WaitForAuthentication>d__113 <WaitForAuthentication>d__;
		<WaitForAuthentication>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAuthentication>d__.<>1__state = -1;
		<WaitForAuthentication>d__.<>t__builder.Start<KIDManager.<WaitForAuthentication>d__113>(ref <WaitForAuthentication>d__);
		return <WaitForAuthentication>d__.<>t__builder.Task;
	}

	// Token: 0x060048FF RID: 18687 RVA: 0x00185398 File Offset: 0x00183598
	[return: TupleElementNames(new string[]
	{
		"ageStatus",
		"resp"
	})]
	private static Task<ValueTuple<AgeStatusType, TMPSession>> AgeGateFlow(GetPlayerData_Data newPlayerData)
	{
		KIDManager.<AgeGateFlow>d__114 <AgeGateFlow>d__;
		<AgeGateFlow>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<AgeStatusType, TMPSession>>.Create();
		<AgeGateFlow>d__.newPlayerData = newPlayerData;
		<AgeGateFlow>d__.<>1__state = -1;
		<AgeGateFlow>d__.<>t__builder.Start<KIDManager.<AgeGateFlow>d__114>(ref <AgeGateFlow>d__);
		return <AgeGateFlow>d__.<>t__builder.Task;
	}

	// Token: 0x06004900 RID: 18688 RVA: 0x001853DC File Offset: 0x001835DC
	private static Task<VerifyAgeData> ProcessAgeGate()
	{
		KIDManager.<ProcessAgeGate>d__115 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDManager.<ProcessAgeGate>d__115>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	// Token: 0x06004901 RID: 18689 RVA: 0x00185417 File Offset: 0x00183617
	public static string GetOptInKey(EKIDFeatures feature)
	{
		return feature.ToStandardisedString() + "-opt-in-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
	}

	// Token: 0x06004902 RID: 18690 RVA: 0x00185438 File Offset: 0x00183638
	private static Task<GetPlayerData_Data> Server_GetPlayerData(bool forceRefresh, Action failureCallback)
	{
		KIDManager.<Server_GetPlayerData>d__130 <Server_GetPlayerData>d__;
		<Server_GetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<Server_GetPlayerData>d__.forceRefresh = forceRefresh;
		<Server_GetPlayerData>d__.failureCallback = failureCallback;
		<Server_GetPlayerData>d__.<>1__state = -1;
		<Server_GetPlayerData>d__.<>t__builder.Start<KIDManager.<Server_GetPlayerData>d__130>(ref <Server_GetPlayerData>d__);
		return <Server_GetPlayerData>d__.<>t__builder.Task;
	}

	// Token: 0x06004903 RID: 18691 RVA: 0x00185484 File Offset: 0x00183684
	private static Task<bool> Server_SetConfirmedStatus()
	{
		KIDManager.<Server_SetConfirmedStatus>d__131 <Server_SetConfirmedStatus>d__;
		<Server_SetConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetConfirmedStatus>d__.<>1__state = -1;
		<Server_SetConfirmedStatus>d__.<>t__builder.Start<KIDManager.<Server_SetConfirmedStatus>d__131>(ref <Server_SetConfirmedStatus>d__);
		return <Server_SetConfirmedStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06004904 RID: 18692 RVA: 0x001854C0 File Offset: 0x001836C0
	private static Task<UpgradeSessionData> Server_UpgradeSession(global::UpgradeSessionRequest request)
	{
		KIDManager.<Server_UpgradeSession>d__132 <Server_UpgradeSession>d__;
		<Server_UpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<Server_UpgradeSession>d__.request = request;
		<Server_UpgradeSession>d__.<>1__state = -1;
		<Server_UpgradeSession>d__.<>t__builder.Start<KIDManager.<Server_UpgradeSession>d__132>(ref <Server_UpgradeSession>d__);
		return <Server_UpgradeSession>d__.<>t__builder.Task;
	}

	// Token: 0x06004905 RID: 18693 RVA: 0x00185504 File Offset: 0x00183704
	private static Task<VerifyAgeData> Server_VerifyAge(VerifyAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_VerifyAge>d__133 <Server_VerifyAge>d__;
		<Server_VerifyAge>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<Server_VerifyAge>d__.request = request;
		<Server_VerifyAge>d__.failureCallback = failureCallback;
		<Server_VerifyAge>d__.<>1__state = -1;
		<Server_VerifyAge>d__.<>t__builder.Start<KIDManager.<Server_VerifyAge>d__133>(ref <Server_VerifyAge>d__);
		return <Server_VerifyAge>d__.<>t__builder.Task;
	}

	// Token: 0x06004906 RID: 18694 RVA: 0x00185550 File Offset: 0x00183750
	private static Task<AttemptAgeUpdateData> Server_AttemptAgeUpdate(AttemptAgeUpdateRequest request, Action failureCallback)
	{
		KIDManager.<Server_AttemptAgeUpdate>d__134 <Server_AttemptAgeUpdate>d__;
		<Server_AttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<Server_AttemptAgeUpdate>d__.request = request;
		<Server_AttemptAgeUpdate>d__.<>1__state = -1;
		<Server_AttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<Server_AttemptAgeUpdate>d__134>(ref <Server_AttemptAgeUpdate>d__);
		return <Server_AttemptAgeUpdate>d__.<>t__builder.Task;
	}

	// Token: 0x06004907 RID: 18695 RVA: 0x00185594 File Offset: 0x00183794
	private static Task<bool> Server_AppealAge(AppealAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_AppealAge>d__135 <Server_AppealAge>d__;
		<Server_AppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_AppealAge>d__.request = request;
		<Server_AppealAge>d__.<>1__state = -1;
		<Server_AppealAge>d__.<>t__builder.Start<KIDManager.<Server_AppealAge>d__135>(ref <Server_AppealAge>d__);
		return <Server_AppealAge>d__.<>t__builder.Task;
	}

	// Token: 0x06004908 RID: 18696 RVA: 0x001855D8 File Offset: 0x001837D8
	private static Task<ValueTuple<bool, string>> Server_SendChallengeEmail(SendChallengeEmailRequest request)
	{
		KIDManager.<Server_SendChallengeEmail>d__136 <Server_SendChallengeEmail>d__;
		<Server_SendChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<Server_SendChallengeEmail>d__.request = request;
		<Server_SendChallengeEmail>d__.<>1__state = -1;
		<Server_SendChallengeEmail>d__.<>t__builder.Start<KIDManager.<Server_SendChallengeEmail>d__136>(ref <Server_SendChallengeEmail>d__);
		return <Server_SendChallengeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06004909 RID: 18697 RVA: 0x0018561C File Offset: 0x0018381C
	private static Task<bool> Server_SetOptInPermissions(SetOptInPermissionsRequest request, Action failureCallback)
	{
		KIDManager.<Server_SetOptInPermissions>d__137 <Server_SetOptInPermissions>d__;
		<Server_SetOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetOptInPermissions>d__.request = request;
		<Server_SetOptInPermissions>d__.failureCallback = failureCallback;
		<Server_SetOptInPermissions>d__.<>1__state = -1;
		<Server_SetOptInPermissions>d__.<>t__builder.Start<KIDManager.<Server_SetOptInPermissions>d__137>(ref <Server_SetOptInPermissions>d__);
		return <Server_SetOptInPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x0600490A RID: 18698 RVA: 0x00185668 File Offset: 0x00183868
	private static Task<bool> Server_OptIn()
	{
		KIDManager.<Server_OptIn>d__138 <Server_OptIn>d__;
		<Server_OptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_OptIn>d__.<>1__state = -1;
		<Server_OptIn>d__.<>t__builder.Start<KIDManager.<Server_OptIn>d__138>(ref <Server_OptIn>d__);
		return <Server_OptIn>d__.<>t__builder.Task;
	}

	// Token: 0x0600490B RID: 18699 RVA: 0x001856A4 File Offset: 0x001838A4
	private static Task<GetRequirementsData> Server_GetRequirements()
	{
		KIDManager.<Server_GetRequirements>d__139 <Server_GetRequirements>d__;
		<Server_GetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<Server_GetRequirements>d__.<>1__state = -1;
		<Server_GetRequirements>d__.<>t__builder.Start<KIDManager.<Server_GetRequirements>d__139>(ref <Server_GetRequirements>d__);
		return <Server_GetRequirements>d__.<>t__builder.Task;
	}

	// Token: 0x0600490C RID: 18700 RVA: 0x001856E0 File Offset: 0x001838E0
	[return: TupleElementNames(new string[]
	{
		"code",
		"responseModel",
		"errorMessage"
	})]
	private static Task<ValueTuple<long, T, string>> KIDServerWebRequest<T, Q>(string endpoint, string operationType, Q requestData, string queryParams = null, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where T : class where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequest>d__140<T, Q> <KIDServerWebRequest>d__;
		<KIDServerWebRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<long, T, string>>.Create();
		<KIDServerWebRequest>d__.endpoint = endpoint;
		<KIDServerWebRequest>d__.operationType = operationType;
		<KIDServerWebRequest>d__.requestData = requestData;
		<KIDServerWebRequest>d__.queryParams = queryParams;
		<KIDServerWebRequest>d__.maxRetries = maxRetries;
		<KIDServerWebRequest>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequest>d__.<>1__state = -1;
		<KIDServerWebRequest>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequest>d__140<T, Q>>(ref <KIDServerWebRequest>d__);
		return <KIDServerWebRequest>d__.<>t__builder.Task;
	}

	// Token: 0x0600490D RID: 18701 RVA: 0x00185750 File Offset: 0x00183950
	private static Task<long> KIDServerWebRequestNoResponse<Q>(string endpoint, string operationType, Q requestData, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequestNoResponse>d__141<Q> <KIDServerWebRequestNoResponse>d__;
		<KIDServerWebRequestNoResponse>d__.<>t__builder = AsyncTaskMethodBuilder<long>.Create();
		<KIDServerWebRequestNoResponse>d__.endpoint = endpoint;
		<KIDServerWebRequestNoResponse>d__.operationType = operationType;
		<KIDServerWebRequestNoResponse>d__.requestData = requestData;
		<KIDServerWebRequestNoResponse>d__.maxRetries = maxRetries;
		<KIDServerWebRequestNoResponse>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequestNoResponse>d__.<>1__state = -1;
		<KIDServerWebRequestNoResponse>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequestNoResponse>d__141<Q>>(ref <KIDServerWebRequestNoResponse>d__);
		return <KIDServerWebRequestNoResponse>d__.<>t__builder.Task;
	}

	// Token: 0x0600490E RID: 18702 RVA: 0x001857B4 File Offset: 0x001839B4
	public static void RegisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Combine(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x0600490F RID: 18703 RVA: 0x001857CB File Offset: 0x001839CB
	public static void UnregisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Remove(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	// Token: 0x06004910 RID: 18704 RVA: 0x001857E2 File Offset: 0x001839E2
	public static void RegisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06004911 RID: 18705 RVA: 0x001857F9 File Offset: 0x001839F9
	public static void UnregisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	// Token: 0x06004912 RID: 18706 RVA: 0x00185810 File Offset: 0x00183A10
	public static void RegisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06004913 RID: 18707 RVA: 0x00185827 File Offset: 0x00183A27
	public static void UnregisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	// Token: 0x06004914 RID: 18708 RVA: 0x0018583E File Offset: 0x00183A3E
	public static void RegisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06004915 RID: 18709 RVA: 0x00185855 File Offset: 0x00183A55
	public static void UnregisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	// Token: 0x06004916 RID: 18710 RVA: 0x0018586C File Offset: 0x00183A6C
	public static void RegisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06004917 RID: 18711 RVA: 0x00185883 File Offset: 0x00183A83
	public static void UnregisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	// Token: 0x06004918 RID: 18712 RVA: 0x0018589A File Offset: 0x00183A9A
	public static void RegisterSessionUpdatedCallback_UGC(Action<bool, Permission.ManagedByEnum> callback)
	{
		KIDManager._onSessionUpdated_UGC = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_UGC, callback);
	}

	// Token: 0x06004919 RID: 18713 RVA: 0x001858B4 File Offset: 0x00183AB4
	public static Task<bool> WaitForAndUpdateNewSession(bool forceRefresh)
	{
		KIDManager.<WaitForAndUpdateNewSession>d__168 <WaitForAndUpdateNewSession>d__;
		<WaitForAndUpdateNewSession>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAndUpdateNewSession>d__.forceRefresh = forceRefresh;
		<WaitForAndUpdateNewSession>d__.<>1__state = -1;
		<WaitForAndUpdateNewSession>d__.<>t__builder.Start<KIDManager.<WaitForAndUpdateNewSession>d__168>(ref <WaitForAndUpdateNewSession>d__);
		return <WaitForAndUpdateNewSession>d__.<>t__builder.Task;
	}

	// Token: 0x0600491A RID: 18714 RVA: 0x001858F8 File Offset: 0x00183AF8
	private static bool HasSessionChanged(TMPSession newSession)
	{
		if (newSession == null)
		{
			return false;
		}
		if (KIDManager.CurrentSession == null)
		{
			return true;
		}
		if (!newSession.IsValidSession)
		{
			return false;
		}
		if (newSession.IsDefault)
		{
			Debug.LogError(string.Format("[KID::MANAGER] DEBUG - New Session Is Default! Age: [{0}]", newSession.Age));
			return false;
		}
		return KIDManager.CurrentSession.IsDefault || !newSession.Etag.Equals(KIDManager.CurrentSession.Etag);
	}

	// Token: 0x0600491B RID: 18715 RVA: 0x0018596C File Offset: 0x00183B6C
	private static void OnSessionUpdated()
	{
		Action onSessionUpdated_AnyPermission = KIDManager._onSessionUpdated_AnyPermission;
		if (onSessionUpdated_AnyPermission != null)
		{
			onSessionUpdated_AnyPermission();
		}
		bool voiceChatEnabled = false;
		bool joinGroupsEnabled = false;
		bool customUsernamesEnabled = false;
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		int count = allPermissionsData.Count;
		for (int i = 0; i < count; i++)
		{
			Permission permission = allPermissionsData[i];
			string name = permission.Name;
			if (!(name == "voice-chat"))
			{
				if (!(name == "custom-username"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "multiplayer"))
						{
							if (!(name == "mods"))
							{
								Debug.Log("[KID] Tried updating permission with name [" + permission.Name + "] but did not match any of the set cases. Unable to process");
							}
							else if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_UGC = KIDManager._onSessionUpdated_UGC;
								if (onSessionUpdated_UGC != null)
								{
									onSessionUpdated_UGC(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
						}
						else
						{
							if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_Multiplayer = KIDManager._onSessionUpdated_Multiplayer;
								if (onSessionUpdated_Multiplayer != null)
								{
									onSessionUpdated_Multiplayer(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
							bool enabled = permission.Enabled;
						}
					}
					else
					{
						if (KIDManager.HasPermissionChanged(permission))
						{
							Action<bool, Permission.ManagedByEnum> onSessionUpdated_PrivateRooms = KIDManager._onSessionUpdated_PrivateRooms;
							if (onSessionUpdated_PrivateRooms != null)
							{
								onSessionUpdated_PrivateRooms(permission.Enabled, permission.ManagedBy);
							}
							KIDManager._previousPermissionSettings[permission.Name] = permission;
						}
						joinGroupsEnabled = permission.Enabled;
					}
				}
				else
				{
					if (KIDManager.HasPermissionChanged(permission))
					{
						Action<bool, Permission.ManagedByEnum> onSessionUpdated_CustomUsernames = KIDManager._onSessionUpdated_CustomUsernames;
						if (onSessionUpdated_CustomUsernames != null)
						{
							onSessionUpdated_CustomUsernames(permission.Enabled, permission.ManagedBy);
						}
						KIDManager._previousPermissionSettings[permission.Name] = permission;
					}
					customUsernamesEnabled = permission.Enabled;
				}
			}
			else
			{
				if (KIDManager.HasPermissionChanged(permission))
				{
					Action<bool, Permission.ManagedByEnum> onSessionUpdated_VoiceChat = KIDManager._onSessionUpdated_VoiceChat;
					if (onSessionUpdated_VoiceChat != null)
					{
						onSessionUpdated_VoiceChat(permission.Enabled, permission.ManagedBy);
					}
					KIDManager._previousPermissionSettings[permission.Name] = permission;
				}
				voiceChatEnabled = permission.Enabled;
			}
		}
		GorillaTelemetry.PostKidEvent(joinGroupsEnabled, voiceChatEnabled, customUsernamesEnabled, KIDManager.CurrentSession.AgeStatus, GTKidEventType.permission_update);
	}

	// Token: 0x0600491C RID: 18716 RVA: 0x00185BA0 File Offset: 0x00183DA0
	private static bool HasPermissionChanged(Permission newValue)
	{
		Permission permission;
		if (KIDManager._previousPermissionSettings.TryGetValue(newValue.Name, out permission))
		{
			return permission.Enabled != newValue.Enabled || permission.ManagedBy != newValue.ManagedBy;
		}
		KIDManager._previousPermissionSettings.Add(newValue.Name, newValue);
		return true;
	}

	// Token: 0x04005B2E RID: 23342
	public const string MULTIPLAYER_PERMISSION_NAME = "multiplayer";

	// Token: 0x04005B2F RID: 23343
	public const string UGC_PERMISSION_NAME = "mods";

	// Token: 0x04005B30 RID: 23344
	public const string PRIVATE_ROOM_PERMISSION_NAME = "join-groups";

	// Token: 0x04005B31 RID: 23345
	public const string VOICE_CHAT_PERMISSION_NAME = "voice-chat";

	// Token: 0x04005B32 RID: 23346
	public const string CUSTOM_USERNAME_PERMISSION_NAME = "custom-username";

	// Token: 0x04005B33 RID: 23347
	public const string PREVIOUS_STATUS_PREF_KEY_PREFIX = "previous-status-";

	// Token: 0x04005B34 RID: 23348
	public const string KID_DATA_KEY = "KIDData";

	// Token: 0x04005B35 RID: 23349
	private const string KID_EMAIL_KEY = "k-id_EmailAddress";

	// Token: 0x04005B36 RID: 23350
	private const int SECONDS_BETWEEN_UPDATE_ATTEMPTS = 30;

	// Token: 0x04005B37 RID: 23351
	private const string KID_SETUP_FLAG = "KID-Setup-";

	// Token: 0x04005B38 RID: 23352
	[OnEnterPlay_SetNull]
	private static KIDManager _instance;

	// Token: 0x04005B3D RID: 23357
	private static string _emailAddress;

	// Token: 0x04005B3E RID: 23358
	private static CancellationTokenSource _requestCancellationSource = new CancellationTokenSource();

	// Token: 0x04005B3F RID: 23359
	private static bool _titleDataReady = false;

	// Token: 0x04005B40 RID: 23360
	private static bool _useKid = false;

	// Token: 0x04005B41 RID: 23361
	private static int _kIDPhase = 0;

	// Token: 0x04005B42 RID: 23362
	private static DateTime? _kIDNewPlayerDateTime = null;

	// Token: 0x04005B46 RID: 23366
	private static string _debugKIDLocalePlayerPrefRef = "KID_SPOOF_LOCALE";

	// Token: 0x04005B47 RID: 23367
	private static string parentEmailForUserPlayerPrefRef;

	// Token: 0x04005B48 RID: 23368
	[OnEnterPlay_SetNull]
	private static Action _sessionUpdatedCallback = null;

	// Token: 0x04005B49 RID: 23369
	[OnEnterPlay_SetNull]
	private static Action _onKIDInitialisationComplete = null;

	// Token: 0x04005B4A RID: 23370
	public static KIDManager.OnEmailResultReceived onEmailResultReceived;

	// Token: 0x04005B4B RID: 23371
	private const string KID_GET_SESSION = "GetPlayerData";

	// Token: 0x04005B4C RID: 23372
	private const string KID_VERIFY_AGE = "VerifyAge";

	// Token: 0x04005B4D RID: 23373
	private const string KID_UPGRADE_SESSION = "UpgradeSession";

	// Token: 0x04005B4E RID: 23374
	private const string KID_SEND_CHALLENGE_EMAIL = "SendChallengeEmail";

	// Token: 0x04005B4F RID: 23375
	private const string KID_ATTEMPT_AGE_UPDATE = "AttemptAgeUpdate";

	// Token: 0x04005B50 RID: 23376
	private const string KID_APPEAL_AGE = "AppealAge";

	// Token: 0x04005B51 RID: 23377
	private const string KID_OPT_IN = "OptIn";

	// Token: 0x04005B52 RID: 23378
	private const string KID_GET_REQUIREMENTS = "GetRequirements";

	// Token: 0x04005B53 RID: 23379
	private const string KID_SET_CONFIRMED_STATUS = "SetConfirmedStatus";

	// Token: 0x04005B54 RID: 23380
	private const string KID_SET_OPT_IN_PERMISSIONS = "SetOptInPermissions";

	// Token: 0x04005B55 RID: 23381
	private const string KID_FORCE_REFRESH = "sessionRefresh";

	// Token: 0x04005B56 RID: 23382
	private const int MAX_RETRIES_FOR_CRITICAL_KID_SERVER_REQUESTS = 3;

	// Token: 0x04005B57 RID: 23383
	private const int MAX_RETRIES_FOR_NORMAL_KID_SERVER_REQUESTS = 2;

	// Token: 0x04005B58 RID: 23384
	public const string KID_PERMISSION__VOICE_CHAT = "voice-chat";

	// Token: 0x04005B59 RID: 23385
	public const string KID_PERMISSION__CUSTOM_NAMES = "custom-username";

	// Token: 0x04005B5A RID: 23386
	public const string KID_PERMISSION__PRIVATE_ROOMS = "join-groups";

	// Token: 0x04005B5B RID: 23387
	public const string KID_PERMISSION__MULTIPLAYER = "multiplayer";

	// Token: 0x04005B5C RID: 23388
	public const string KID_PERMISSION__UGC = "mods";

	// Token: 0x04005B5D RID: 23389
	private const float MAX_SESSION_UPDATE_TIME = 600f;

	// Token: 0x04005B5E RID: 23390
	private const int TIME_BETWEEN_SESSION_UPDATE_ATTEMPTS = 30;

	// Token: 0x04005B5F RID: 23391
	[OnEnterPlay_SetNull]
	private static Action _onSessionUpdated_AnyPermission;

	// Token: 0x04005B60 RID: 23392
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_VoiceChat;

	// Token: 0x04005B61 RID: 23393
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_CustomUsernames;

	// Token: 0x04005B62 RID: 23394
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_PrivateRooms;

	// Token: 0x04005B63 RID: 23395
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_Multiplayer;

	// Token: 0x04005B64 RID: 23396
	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_UGC;

	// Token: 0x04005B65 RID: 23397
	private static bool _isUpdatingNewSession = false;

	// Token: 0x04005B66 RID: 23398
	[OnEnterPlay_SetNull]
	private static Dictionary<string, Permission> _previousPermissionSettings = new Dictionary<string, Permission>();

	// Token: 0x02000B3A RID: 2874
	// (Invoke) Token: 0x06004920 RID: 18720
	public delegate void OnEmailResultReceived(bool result);
}
