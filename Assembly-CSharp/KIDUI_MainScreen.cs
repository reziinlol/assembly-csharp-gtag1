using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000BA9 RID: 2985
public class KIDUI_MainScreen : MonoBehaviour
{
	// Token: 0x06004AF8 RID: 19192 RVA: 0x00190891 File Offset: 0x0018EA91
	private void Awake()
	{
		KIDUI_MainScreen._featuresList.Clear();
		if (this._setupKidScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._initialised)
		{
			return;
		}
		this.InitialiseMainScreen();
	}

	// Token: 0x06004AF9 RID: 19193 RVA: 0x001908CA File Offset: 0x0018EACA
	private void OnEnable()
	{
		KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06004AFA RID: 19194 RVA: 0x001908F4 File Offset: 0x0018EAF4
	private void OnDisable()
	{
		KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.UpdatePermissionsAndFeaturesScreen));
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004AFB RID: 19195 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnDestroy()
	{
	}

	// Token: 0x06004AFC RID: 19196 RVA: 0x0019092C File Offset: 0x0018EB2C
	private void ConstructFeatureSettings()
	{
		for (int i = 0; i < this._displayOrder.Length; i++)
		{
			for (int j = 0; j < this._featureSetups.Count; j++)
			{
				if (this._featureSetups[j].linkedFeature == this._displayOrder[i])
				{
					this.CreateNewFeatureDisplay(this._featureSetups[j]);
					break;
				}
			}
		}
		this.UpdatePermissionsAndFeaturesScreen();
	}

	// Token: 0x06004AFD RID: 19197 RVA: 0x00190998 File Offset: 0x0018EB98
	private void CreateNewFeatureDisplay(KIDUI_MainScreen.FeatureToggleSetup setup)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(setup.linkedFeature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID::UI::MAIN] Failed to retrieve permission data for feature; [" + setup.linkedFeature.ToString() + "]", Array.Empty<object>());
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER)
		{
			if (permissionDataByFeature.Enabled)
			{
				return;
			}
			if (KIDManager.CheckFeatureOptIn(setup.linkedFeature, null).Item2)
			{
				return;
			}
		}
		if (setup.alwaysCheckFeatureSetting && KIDManager.CheckFeatureSettingEnabled(setup.linkedFeature))
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this._featurePrefab, this._featureRootTransform);
		KIDUIFeatureSetting component = gameObject.GetComponent<KIDUIFeatureSetting>();
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogFormat(string.Format("[KID::UI::MAIN_SCREEN] Adding new Locked Feature:  {0} Is enabled: {1}", setup.linkedFeature.ToString(), permissionDataByFeature.Enabled), Array.Empty<object>());
			component.CreateNewFeatureSettingGuardianManaged(setup, permissionDataByFeature.Enabled);
			if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
			{
				KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
			}
			KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
			return;
		}
		if (setup.requiresToggle)
		{
			component.CreateNewFeatureSettingWithToggle(setup, false, setup.alwaysCheckFeatureSetting);
		}
		else
		{
			component.CreateNewFeatureSettingWithoutToggle(setup, setup.alwaysCheckFeatureSetting);
		}
		if (!KIDUI_MainScreen._featuresList.ContainsKey(setup.linkedFeature))
		{
			KIDUI_MainScreen._featuresList.Add(setup.linkedFeature, new List<KIDUIFeatureSetting>());
		}
		KIDUI_MainScreen._featuresList[setup.linkedFeature].Add(component);
		this.ConstructAdditionalSetup(setup.linkedFeature, gameObject);
	}

	// Token: 0x06004AFE RID: 19198 RVA: 0x00190B34 File Offset: 0x0018ED34
	private void ConstructAdditionalSetup(EKIDFeatures feature, GameObject featureObject)
	{
	}

	// Token: 0x06004AFF RID: 19199 RVA: 0x00190B3C File Offset: 0x0018ED3C
	private void UpdatePermissionsAndFeaturesScreen()
	{
		int num = 0;
		Debug.LogFormat(string.Format("[KID::UI::MAIN] Updated Feature listings. To Update: [{0}]", KIDUI_MainScreen._featuresList.Count), Array.Empty<object>());
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(keyValuePair.Key);
				if (permissionDataByFeature == null)
				{
					Debug.LogErrorFormat("[KID::UI::MAIN] Failed to find permission data for feature: [" + keyValuePair.Key.ToString() + "]", Array.Empty<object>());
				}
				else if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					keyValuePair.Value[i].SetGuardianManagedState(permissionDataByFeature.Enabled);
				}
				else
				{
					bool isOptedIn = KIDManager.CheckFeatureOptIn(keyValuePair.Key, permissionDataByFeature).Item2;
					if (keyValuePair.Value[i].AlwaysCheckFeatureSetting)
					{
						isOptedIn = KIDManager.CheckFeatureSettingEnabled(keyValuePair.Key);
					}
					if (!keyValuePair.Value[i].GetHasToggle())
					{
						keyValuePair.Value[i].SetPlayerManagedState(permissionDataByFeature.Enabled, isOptedIn);
					}
				}
			}
		}
		int num2 = 0;
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair2 in KIDUI_MainScreen._featuresList)
		{
			for (int j = 0; j < keyValuePair2.Value.Count; j++)
			{
				num2++;
				Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(keyValuePair2.Key);
				if (keyValuePair2.Value[j].GetFeatureToggleState() || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PLAYER)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			if (!this._initialised)
			{
				this._titleFeaturePermissions.SetActive(false);
				this._titleGameFeatures.SetActive(true);
			}
			this._hasAllPermissions = true;
			this._getPermissionsButton.gameObject.SetActive(false);
			this._gettingPermissionsButton.gameObject.SetActive(false);
			this._requestPermissionsButton.gameObject.SetActive(false);
			this._permissionsTip.SetActive(false);
			this.SetButtonContainersVisibility(EGetPermissionsStatus.RequestedPermission);
		}
	}

	// Token: 0x06004B00 RID: 19200 RVA: 0x00190DC4 File Offset: 0x0018EFC4
	private bool IsFeatureToggledOn(EKIDFeatures permissionFeature)
	{
		List<KIDUIFeatureSetting> source;
		if (!KIDUI_MainScreen._featuresList.TryGetValue(permissionFeature, out source))
		{
			return true;
		}
		KIDUIFeatureSetting kiduifeatureSetting = source.FirstOrDefault<KIDUIFeatureSetting>();
		if (kiduifeatureSetting == null)
		{
			Debug.LogErrorFormat(string.Format("[KID::UI::MAIN] Empty list for permission Name [{0}]", permissionFeature), Array.Empty<object>());
			return false;
		}
		return kiduifeatureSetting.GetFeatureToggleState();
	}

	// Token: 0x06004B01 RID: 19201 RVA: 0x00190E14 File Offset: 0x0018F014
	public void InitialiseMainScreen()
	{
		if (this._initialised)
		{
			Debug.Log("[KID::MAIN_SCREEN] Already Initialised");
			return;
		}
		this.ConstructFeatureSettings();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		this._initialised = true;
	}

	// Token: 0x06004B02 RID: 19202 RVA: 0x00190E98 File Offset: 0x0018F098
	public void ShowMainScreen(EMainScreenStatus showStatus, KIDUI_Controller.Metrics_ShowReason reason)
	{
		this.ShowMainScreen(showStatus);
		this._mainScreenOpenedReason = reason;
		string value = reason.ToString().Replace("_", "-").ToLower();
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_game_settings",
			CustomTags = new string[]
			{
				"kid_setup",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment,
				KIDTelemetry.Open_MetricActionCustomTag
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen_shown_reason",
					value
				}
			}
		};
		foreach (Permission permission in KIDManager.GetAllPermissionsData())
		{
			telemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
			telemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
		}
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
	}

	// Token: 0x06004B03 RID: 19203 RVA: 0x00190FE4 File Offset: 0x0018F1E4
	public void ShowMainScreen(EMainScreenStatus showStatus)
	{
		KIDUI_MainScreen.ShownSettingsScreen = true;
		base.gameObject.SetActive(true);
		this.ConfigurePermissionsButtons();
		this.UpdateScreenStatus(showStatus, false);
	}

	// Token: 0x06004B04 RID: 19204 RVA: 0x00191008 File Offset: 0x0018F208
	public void UpdateScreenStatus(EMainScreenStatus showStatus, bool sendMetrics = false)
	{
		if (sendMetrics && showStatus == EMainScreenStatus.Updated)
		{
			string value = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment,
					KIDTelemetry.Updated_MetricActionCustomTag
				},
				BodyData = new Dictionary<string, string>
				{
					{
						"screen_shown_reason",
						value
					}
				}
			};
			foreach (Permission permission in KIDManager.GetAllPermissionsData())
			{
				telemetryData.BodyData.Add(KIDTelemetry.GetPermissionManagedByBodyData(permission.Name), permission.ManagedBy.ToString().ToLower());
				telemetryData.BodyData.Add(KIDTelemetry.GetPermissionEnabledBodyData(permission.Name), permission.Enabled.ToString().ToLower());
			}
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
		GameObject activeStatusObject = this.GetActiveStatusObject();
		this._declinedStatus.SetActive(false);
		this._timeoutStatus.SetActive(false);
		this._pendingStatus.SetActive(false);
		this._updatedStatus.SetActive(false);
		this._setupRequiredStatus.SetActive(false);
		this._missingStatus.SetActive(false);
		this._fullPlayerControlStatus.SetActive(false);
		switch (showStatus)
		{
		default:
			if (!this._hasAllPermissions)
			{
				this._missingStatus.SetActive(true);
			}
			else if (this._hasAllPermissions)
			{
				this._fullPlayerControlStatus.SetActive(true);
			}
			else
			{
				this._screenStatus = showStatus;
			}
			break;
		case EMainScreenStatus.Declined:
			this._declinedStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Pending:
			this._pendingStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Timedout:
			this._timeoutStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Setup:
			this._setupRequiredStatus.SetActive(true);
			this._screenStatus = showStatus;
			break;
		case EMainScreenStatus.Previous:
			if (activeStatusObject != null)
			{
				activeStatusObject.SetActive(true);
			}
			else
			{
				this._updatedStatus.SetActive(true);
			}
			break;
		case EMainScreenStatus.FullControl:
			this._fullPlayerControlStatus.SetActive(true);
			break;
		}
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06004B05 RID: 19205 RVA: 0x000440BC File Offset: 0x000422BC
	public void HideMainScreen()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004B06 RID: 19206 RVA: 0x001912A4 File Offset: 0x0018F4A4
	public void OnAskForPermission()
	{
		KIDUI_MainScreen.<OnAskForPermission>d__52 <OnAskForPermission>d__;
		<OnAskForPermission>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnAskForPermission>d__.<>4__this = this;
		<OnAskForPermission>d__.<>1__state = -1;
		<OnAskForPermission>d__.<>t__builder.Start<KIDUI_MainScreen.<OnAskForPermission>d__52>(ref <OnAskForPermission>d__);
	}

	// Token: 0x06004B07 RID: 19207 RVA: 0x001912DC File Offset: 0x0018F4DC
	public void OnSaveAndExit()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::KID_UI_MAINSCREEN] There is no session as such cannot opt into anything");
			KIDUI_Controller.Instance.CloseKIDScreens();
			return;
		}
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		for (int i = 0; i < allPermissionsData.Count; i++)
		{
			string name = allPermissionsData[i].Name;
			if (!(name == "multiplayer"))
			{
				if (!(name == "mods"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "voice-chat"))
						{
							if (!(name == "custom-username"))
							{
								Debug.LogError("[KID::UI::MainScreen] Unhandled permission when saving and exiting: [" + allPermissionsData[i].Name + "]");
							}
							else
							{
								this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Custom_Nametags, delegate(bool b, Permission p, bool hasOptedInPreviously)
								{
									GorillaComputer.instance.SetNametagSetting(b, p.ManagedBy, hasOptedInPreviously);
								});
							}
						}
						else
						{
							this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Voice_Chat, delegate(bool b, Permission p, bool hasOptedInPreviously)
							{
								GorillaComputer.instance.KID_SetVoiceChatSettingOnStart(b, p.ManagedBy, hasOptedInPreviously);
							});
						}
					}
				}
				else
				{
					this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Mods, null);
				}
			}
			else
			{
				this.UpdateOptInSetting(allPermissionsData[i], EKIDFeatures.Multiplayer, null);
			}
		}
		KIDManager.SendOptInPermissions();
		if (this._screenStatus != EMainScreenStatus.None)
		{
			string value = this._mainScreenOpenedReason.ToString().Replace("_", "-").ToLower();
			TelemetryData telemetryData = new TelemetryData
			{
				EventName = "kid_game_settings",
				CustomTags = new string[]
				{
					"kid_setup",
					KIDTelemetry.GameVersionCustomTag,
					KIDTelemetry.GameEnvironment
				},
				BodyData = new Dictionary<string, string>
				{
					{
						"screen_shown_reason",
						value
					},
					{
						"kid_status",
						this._screenStatus.ToString().ToLower()
					},
					{
						"button_pressed",
						"save_and_continue"
					}
				}
			};
			GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		}
		else
		{
			Debug.LogError("[KID::UI::MAIN_SCREEN] Trying to close k-ID Main Screen, but screen status is set to [None] - Invalid status, will not submit analytics");
		}
		KIDUI_Controller.Instance.CloseKIDScreens();
	}

	// Token: 0x06004B08 RID: 19208 RVA: 0x00191508 File Offset: 0x0018F708
	public int GetFeatureListingCount()
	{
		int num = 0;
		foreach (List<KIDUIFeatureSetting> list in KIDUI_MainScreen._featuresList.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x06004B09 RID: 19209 RVA: 0x00191564 File Offset: 0x0018F764
	private Task<bool> UpdateAndCheckForMissingPermissions()
	{
		KIDUI_MainScreen.<UpdateAndCheckForMissingPermissions>d__55 <UpdateAndCheckForMissingPermissions>d__;
		<UpdateAndCheckForMissingPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateAndCheckForMissingPermissions>d__.<>4__this = this;
		<UpdateAndCheckForMissingPermissions>d__.<>1__state = -1;
		<UpdateAndCheckForMissingPermissions>d__.<>t__builder.Start<KIDUI_MainScreen.<UpdateAndCheckForMissingPermissions>d__55>(ref <UpdateAndCheckForMissingPermissions>d__);
		return <UpdateAndCheckForMissingPermissions>d__.<>t__builder.Task;
	}

	// Token: 0x06004B0A RID: 19210 RVA: 0x001915A8 File Offset: 0x0018F7A8
	private void OnLanguageChanged()
	{
		foreach (KeyValuePair<EKIDFeatures, List<KIDUIFeatureSetting>> keyValuePair in KIDUI_MainScreen._featuresList)
		{
			List<KIDUIFeatureSetting> value = keyValuePair.Value;
			if (value != null)
			{
				for (int i = 0; i < value.Count; i++)
				{
					if (value[i] != null)
					{
						value[i].RefreshTextOnLanguageChanged();
					}
				}
			}
		}
	}

	// Token: 0x06004B0B RID: 19211 RVA: 0x0019162C File Offset: 0x0018F82C
	private void UpdateOptInSetting(Permission permissionData, EKIDFeatures feature, Action<bool, Permission, bool> onOptedIn)
	{
		bool item = KIDManager.CheckFeatureOptIn(feature, permissionData).Item2;
		bool flag = this.IsFeatureToggledOn(feature);
		Debug.Log(string.Format("[KID::UI::MainScreen] Update opt in for {0}. Has opted in: {1}. Toggled on: {2}", feature.ToString(), item, flag));
		KIDManager.SetFeatureOptIn(feature, flag);
		if (onOptedIn != null)
		{
			onOptedIn(flag, permissionData, item);
		}
	}

	// Token: 0x06004B0C RID: 19212 RVA: 0x00191689 File Offset: 0x0018F889
	public void OnConfirmedEmailAddress(string emailAddress)
	{
		this._emailAddress = emailAddress;
		Debug.LogFormat("[KID::UI::Main] Email has been confirmed: " + this._emailAddress, Array.Empty<object>());
	}

	// Token: 0x06004B0D RID: 19213 RVA: 0x001916AC File Offset: 0x0018F8AC
	private IEnumerable<string> CollectPermissionsToUpgrade()
	{
		return from permission in KIDManager.GetAllPermissionsData()
		where permission.ManagedBy == Permission.ManagedByEnum.GUARDIAN && !permission.Enabled
		select permission.Name;
	}

	// Token: 0x06004B0E RID: 19214 RVA: 0x00191708 File Offset: 0x0018F908
	private void ConfigurePermissionsButtons()
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS");
		if (!this._getPermissionsButton.gameObject.activeSelf && !this._gettingPermissionsButton.gameObject.activeSelf)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - GET PERMISSIONS IS DISABLED");
			return;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - CHECK SESSION STATUS: Is Default: [" + KIDManager.CurrentSession.IsDefault.ToString() + "]");
		this.SetButtonContainersVisibility(KIDUI_MainScreen.GetPermissionState());
	}

	// Token: 0x06004B0F RID: 19215 RVA: 0x0019177C File Offset: 0x0018F97C
	private void SetButtonContainersVisibility(EGetPermissionsStatus permissionStatus)
	{
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - PERMISSION STATE: [" + permissionStatus.ToString() + "]");
		this._defaultButtonsContainer.SetActive(permissionStatus == EGetPermissionsStatus.GetPermission);
		this._permissionsRequestingButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestingPermission);
		this._permissionsRequestedButtonContainer.SetActive(permissionStatus == EGetPermissionsStatus.RequestedPermission);
	}

	// Token: 0x06004B10 RID: 19216 RVA: 0x001917D8 File Offset: 0x0018F9D8
	private GameObject GetActiveStatusObject()
	{
		foreach (GameObject gameObject in new List<GameObject>
		{
			this._declinedStatus,
			this._timeoutStatus,
			this._pendingStatus,
			this._updatedStatus,
			this._setupRequiredStatus,
			this._fullPlayerControlStatus
		})
		{
			if (gameObject.activeInHierarchy)
			{
				return gameObject;
			}
		}
		return null;
	}

	// Token: 0x06004B11 RID: 19217 RVA: 0x0019187C File Offset: 0x0018FA7C
	private static EGetPermissionsStatus GetPermissionState()
	{
		if (!KIDManager.CurrentSession.IsDefault)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW REQUESTED");
			return EGetPermissionsStatus.RequestedPermission;
		}
		if (PlayerPrefs.GetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 0) == 0)
		{
			Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW DEFAULT");
			return EGetPermissionsStatus.GetPermission;
		}
		Debug.Log("[KID::MAIN_SCREEN] CONFIGURE BUTTONS - SHOW SWAPPED DEFAULT");
		return EGetPermissionsStatus.RequestingPermission;
	}

	// Token: 0x06004B12 RID: 19218 RVA: 0x001918BC File Offset: 0x0018FABC
	private void OnFeatureToggleChanged(EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			this.OnMultiplayerToggled();
			return;
		case EKIDFeatures.Custom_Nametags:
			this.OnCustomNametagsToggled();
			return;
		case EKIDFeatures.Voice_Chat:
			this.OnVoiceChatToggled();
			return;
		case EKIDFeatures.Mods:
			this.OnModToggleChanged();
			return;
		case EKIDFeatures.Groups:
			this.OnGroupToggleChanged();
			return;
		default:
			Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] Toggle NOT YET IMPLEMENTED for Feature: " + feature.ToString() + ".", Array.Empty<object>());
			return;
		}
	}

	// Token: 0x06004B13 RID: 19219 RVA: 0x0019192E File Offset: 0x0018FB2E
	private void OnMultiplayerToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MULTIPLAYER Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004B14 RID: 19220 RVA: 0x0019193F File Offset: 0x0018FB3F
	private void OnVoiceChatToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] VOICE CHAT Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004B15 RID: 19221 RVA: 0x00191950 File Offset: 0x0018FB50
	private void OnGroupToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] GROUPS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004B16 RID: 19222 RVA: 0x00191961 File Offset: 0x0018FB61
	private void OnModToggleChanged()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] MODS Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x06004B17 RID: 19223 RVA: 0x00191972 File Offset: 0x0018FB72
	private void OnCustomNametagsToggled()
	{
		Debug.LogErrorFormat("[KID::UI::MAIN_SCREEN] CUSTOM USERNAMES Toggle NOT YET IMPLEMENTED.", Array.Empty<object>());
	}

	// Token: 0x04005DDF RID: 24031
	public const string OPT_IN_SUFFIX = "-opt-in";

	// Token: 0x04005DE0 RID: 24032
	public static bool ShownSettingsScreen = false;

	// Token: 0x04005DE1 RID: 24033
	[SerializeField]
	private GameObject _kidScreensGroup;

	// Token: 0x04005DE2 RID: 24034
	[SerializeField]
	private KIDUI_SetupScreen _setupKidScreen;

	// Token: 0x04005DE3 RID: 24035
	[SerializeField]
	private KIDUI_SendUpgradeEmailScreen _sendUpgradeEmailScreen;

	// Token: 0x04005DE4 RID: 24036
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005DE5 RID: 24037
	[Header("Permission Request Buttons")]
	[SerializeField]
	private KIDUIButton _getPermissionsButton;

	// Token: 0x04005DE6 RID: 24038
	[SerializeField]
	private KIDUIButton _gettingPermissionsButton;

	// Token: 0x04005DE7 RID: 24039
	[SerializeField]
	private KIDUIButton _requestPermissionsButton;

	// Token: 0x04005DE8 RID: 24040
	[SerializeField]
	private GameObject _defaultButtonsContainer;

	// Token: 0x04005DE9 RID: 24041
	[SerializeField]
	private GameObject _permissionsRequestingButtonContainer;

	// Token: 0x04005DEA RID: 24042
	[SerializeField]
	private GameObject _permissionsRequestedButtonContainer;

	// Token: 0x04005DEB RID: 24043
	private bool _hasAllPermissions;

	// Token: 0x04005DEC RID: 24044
	[Header("Dynamic Feature Settings Setup")]
	[SerializeField]
	private GameObject _featurePrefab;

	// Token: 0x04005DED RID: 24045
	[SerializeField]
	private Transform _featureRootTransform;

	// Token: 0x04005DEE RID: 24046
	[SerializeField]
	private EKIDFeatures[] _displayOrder = new EKIDFeatures[4];

	// Token: 0x04005DEF RID: 24047
	[SerializeField]
	private List<KIDUI_MainScreen.FeatureToggleSetup> _featureSetups = new List<KIDUI_MainScreen.FeatureToggleSetup>();

	// Token: 0x04005DF0 RID: 24048
	[Header("Additional Feature-Specific Setup")]
	[SerializeField]
	private GameObject _voiceChatLabel;

	// Token: 0x04005DF1 RID: 24049
	[Header("Hide Permissions Tip")]
	[SerializeField]
	private GameObject _permissionsTip;

	// Token: 0x04005DF2 RID: 24050
	[Header("Titles")]
	[SerializeField]
	private GameObject _titleFeaturePermissions;

	// Token: 0x04005DF3 RID: 24051
	[SerializeField]
	private GameObject _titleGameFeatures;

	// Token: 0x04005DF4 RID: 24052
	[Header("Game Status Setup")]
	[SerializeField]
	private GameObject _missingStatus;

	// Token: 0x04005DF5 RID: 24053
	[SerializeField]
	private GameObject _updatedStatus;

	// Token: 0x04005DF6 RID: 24054
	[SerializeField]
	private GameObject _declinedStatus;

	// Token: 0x04005DF7 RID: 24055
	[SerializeField]
	private GameObject _pendingStatus;

	// Token: 0x04005DF8 RID: 24056
	[SerializeField]
	private GameObject _timeoutStatus;

	// Token: 0x04005DF9 RID: 24057
	[SerializeField]
	private GameObject _setupRequiredStatus;

	// Token: 0x04005DFA RID: 24058
	[SerializeField]
	private GameObject _fullPlayerControlStatus;

	// Token: 0x04005DFB RID: 24059
	private string _emailAddress;

	// Token: 0x04005DFC RID: 24060
	private bool _multiplayerEnabled;

	// Token: 0x04005DFD RID: 24061
	private bool _customNameEnabled;

	// Token: 0x04005DFE RID: 24062
	private bool _voiceChatEnabled;

	// Token: 0x04005DFF RID: 24063
	private bool _initialised;

	// Token: 0x04005E00 RID: 24064
	private KIDUI_Controller.Metrics_ShowReason _mainScreenOpenedReason;

	// Token: 0x04005E01 RID: 24065
	private EMainScreenStatus _screenStatus;

	// Token: 0x04005E02 RID: 24066
	private GameObject _eventSystemObj;

	// Token: 0x04005E03 RID: 24067
	private static Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>> _featuresList = new Dictionary<EKIDFeatures, List<KIDUIFeatureSetting>>();

	// Token: 0x02000BAA RID: 2986
	[Serializable]
	public struct FeatureToggleSetup
	{
		// Token: 0x04005E04 RID: 24068
		public EKIDFeatures linkedFeature;

		// Token: 0x04005E05 RID: 24069
		public string permissionName;

		// Token: 0x04005E06 RID: 24070
		public LocalizedString featureName;

		// Token: 0x04005E07 RID: 24071
		public bool requiresToggle;

		// Token: 0x04005E08 RID: 24072
		public bool alwaysCheckFeatureSetting;

		// Token: 0x04005E09 RID: 24073
		public LocalizedString enabledText;

		// Token: 0x04005E0A RID: 24074
		public LocalizedString disabledText;
	}
}
