using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaGameModes;
using GorillaTagScripts;
using GorillaTagScripts.VirtualStumpCustomMaps;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaNetworking
{
	// Token: 0x02001049 RID: 4169
	public class GorillaComputer : MonoBehaviour, IMatchmakingCallbacks, IGorillaSliceableSimple
	{
		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06006839 RID: 26681 RVA: 0x0021A128 File Offset: 0x00218328
		public string versionMismatch
		{
			get
			{
				if (this._lastLocaleChecked_Version != null && this._lastLocaleChecked_Version == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedVersionMismatch))
				{
					return this._cachedVersionMismatch;
				}
				string defaultResult = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";
				string cachedVersionMismatch;
				LocalisationManager.TryGetKeyForCurrentLocale("VERSION_MISMATCH", out cachedVersionMismatch, defaultResult);
				this._lastLocaleChecked_Version = LocalisationManager.CurrentLanguage;
				this._cachedVersionMismatch = cachedVersionMismatch;
				return this._cachedVersionMismatch;
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x0600683A RID: 26682 RVA: 0x0021A198 File Offset: 0x00218398
		public string unableToConnect
		{
			get
			{
				if (this._lastLocaleChecked_Connect != null && this._lastLocaleChecked_Connect == LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedUnableToConnect))
				{
					return this._cachedUnableToConnect;
				}
				string defaultResult = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";
				string cachedUnableToConnect;
				LocalisationManager.TryGetKeyForCurrentLocale("CONNECTION_ISSUE", out cachedUnableToConnect, defaultResult);
				this._lastLocaleChecked_Connect = LocalisationManager.CurrentLanguage;
				this._cachedUnableToConnect = cachedUnableToConnect;
				return this._cachedUnableToConnect;
			}
		}

		// Token: 0x0600683B RID: 26683 RVA: 0x0021A205 File Offset: 0x00218405
		public DateTime GetServerTime()
		{
			return this.startupTime + TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
		}

		// Token: 0x0600683C RID: 26684 RVA: 0x0021A21D File Offset: 0x0021841D
		public void AddSeverTime(int m)
		{
			this.startupTime = this.startupTime.AddMinutes((double)m);
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x0600683D RID: 26685 RVA: 0x0021A232 File Offset: 0x00218432
		// (set) Token: 0x0600683E RID: 26686 RVA: 0x0021A23A File Offset: 0x0021843A
		public string[] allowedMapsToJoin
		{
			get
			{
				return this._allowedMapsToJoin;
			}
			set
			{
				this._allowedMapsToJoin = value;
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x0600683F RID: 26687 RVA: 0x0021A243 File Offset: 0x00218443
		public string VStumpRoomPrepend
		{
			get
			{
				return this.virtualStumpRoomPrepend;
			}
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06006840 RID: 26688 RVA: 0x0021A24C File Offset: 0x0021844C
		public GorillaComputer.ComputerState currentState
		{
			get
			{
				GorillaComputer.ComputerState result;
				this.stateStack.TryPeek(out result);
				return result;
			}
		}

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06006841 RID: 26689 RVA: 0x0021A268 File Offset: 0x00218468
		public string NameTagPlayerPref
		{
			get
			{
				if (PlayFabAuthenticator.instance == null)
				{
					Debug.LogError("Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
					return "nameTagsOn";
				}
				return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
		}

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06006842 RID: 26690 RVA: 0x0021A29F File Offset: 0x0021849F
		// (set) Token: 0x06006843 RID: 26691 RVA: 0x0021A2A7 File Offset: 0x002184A7
		public bool NametagsEnabled { get; private set; }

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06006844 RID: 26692 RVA: 0x0021A2B0 File Offset: 0x002184B0
		// (set) Token: 0x06006845 RID: 26693 RVA: 0x0021A2B8 File Offset: 0x002184B8
		public GorillaComputer.RedemptionResult RedemptionStatus
		{
			get
			{
				return this.redemptionResult;
			}
			set
			{
				this.redemptionResult = value;
				this.UpdateScreen();
			}
		}

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06006846 RID: 26694 RVA: 0x0021A2C7 File Offset: 0x002184C7
		// (set) Token: 0x06006847 RID: 26695 RVA: 0x0021A2CF File Offset: 0x002184CF
		public string RedemptionCode
		{
			get
			{
				return this.redemptionCode;
			}
			set
			{
				this.redemptionCode = value;
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06006848 RID: 26696 RVA: 0x0021A2D8 File Offset: 0x002184D8
		// (set) Token: 0x06006849 RID: 26697 RVA: 0x0021A2E0 File Offset: 0x002184E0
		public DateTimeOffset? RedemptionRestrictionTime { get; set; }

		// Token: 0x0600684A RID: 26698 RVA: 0x0021A2EC File Offset: 0x002184EC
		private void Awake()
		{
			if (GorillaComputer.instance == null)
			{
				GorillaComputer.instance = this;
				GorillaComputer.hasInstance = true;
			}
			else if (GorillaComputer.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			Debug.Log(string.Concat(new string[]
			{
				"==== GORILLA TAG - VERSION: ",
				this.version,
				", BUILD NUMBER: ",
				this.buildCode,
				", BUILD DATE: ",
				this.buildDate,
				" ====\r\n.\r\n.               _______\r\n.              /       \\\r\n.             /  _____  \\\r\n.            / / _   _ \\ \\\r\n.           [ | (O) (O) | ]\r\n.            | \\  . .  / |\r\n.     _______|  | _._ |  |_______\r\n.    /        \\  \\___/  /        \\\r\n.\r\n.\r\n"
			}));
			this._activeOrderList = this.OrderList;
			this.defaultUpdateCooldown = this.updateCooldown;
		}

		// Token: 0x0600684B RID: 26699 RVA: 0x0021A395 File Offset: 0x00218595
		private void Start()
		{
			Debug.Log("Computer Init");
			this.Initialise();
		}

		// Token: 0x0600684C RID: 26700 RVA: 0x0021A3A7 File Offset: 0x002185A7
		public void OnEnable()
		{
			KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x0600684D RID: 26701 RVA: 0x0021A3D2 File Offset: 0x002185D2
		public void OnDisable()
		{
			KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
			KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x0600684E RID: 26702 RVA: 0x0021A3FE File Offset: 0x002185FE
		protected void OnDestroy()
		{
			if (GorillaComputer.instance == this)
			{
				GorillaComputer.hasInstance = false;
				GorillaComputer.instance = null;
			}
			KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x0600684F RID: 26703 RVA: 0x0021A430 File Offset: 0x00218630
		public void SliceUpdate()
		{
			if ((this.internetFailure && Time.realtimeSinceStartup < this.lastCheckedWifi + this.checkIfConnectedSeconds) || (!this.internetFailure && Time.realtimeSinceStartup < this.lastCheckedWifi + this.checkIfDisconnectedSeconds))
			{
				if (!this.internetFailure && this.isConnectedToMaster && Time.realtimeSinceStartup > this.lastUpdateTime + this.updateCooldown)
				{
					this.deltaTime = Time.realtimeSinceStartup - this.lastUpdateTime;
					this.lastUpdateTime = Time.realtimeSinceStartup;
					this.UpdateScreen();
				}
				return;
			}
			this.lastCheckedWifi = Time.realtimeSinceStartup;
			this.stateUpdated = false;
			if (!this.CheckInternetConnection())
			{
				string defaultResult = "NO WIFI OR LAN CONNECTION DETECTED.";
				string failMessage;
				LocalisationManager.TryGetKeyForCurrentLocale("NO_CONNECTION", out failMessage, defaultResult);
				this.UpdateFailureText(failMessage);
				this.internetFailure = true;
				return;
			}
			if (this.internetFailure)
			{
				if (this.CheckInternetConnection())
				{
					this.internetFailure = false;
				}
				this.RestoreFromFailureState();
				this.UpdateScreen();
				return;
			}
			if (this.isConnectedToMaster && Time.realtimeSinceStartup > this.lastUpdateTime + this.updateCooldown)
			{
				this.deltaTime = Time.realtimeSinceStartup - this.lastUpdateTime;
				this.lastUpdateTime = Time.realtimeSinceStartup;
				this.UpdateScreen();
			}
		}

		// Token: 0x06006850 RID: 26704 RVA: 0x0021A560 File Offset: 0x00218760
		private void Initialise()
		{
			GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
			RoomSystem.JoinedRoomEvent += new Action(GorillaComputer.OnFirstJoinedRoom_IncrementSessionCount);
			RoomSystem.JoinedRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.LeftRoomEvent += new Action(this.UpdateScreen);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.PlayerCountChangedCallback);
			LocalisationManager.RegisterOnLanguageChanged(delegate
			{
				this.RefreshFunctionNames();
				this.UpdateGameModeText();
			});
			this.RefreshFunctionNames();
			this.InitialiseRoomScreens();
			this.InitialiseStrings();
			this.InitialiseAllRoomStates();
			this.UpdateScreen();
			byte[] bytes = new byte[]
			{
				Convert.ToByte(64)
			};
			this.virtualStumpRoomPrepend = Encoding.ASCII.GetString(bytes);
			this.initialized = true;
		}

		// Token: 0x06006851 RID: 26705 RVA: 0x0021A664 File Offset: 0x00218864
		private void InitialiseRoomScreens()
		{
			this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
			this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent, null);
		}

		// Token: 0x06006852 RID: 26706 RVA: 0x0021A6BC File Offset: 0x002188BC
		private void InitialiseStrings()
		{
			this.roomToJoin = "";
			this.redText = "";
			this.blueText = "";
			this.greenText = "";
			this.currentName = "";
			this.savedName = "";
		}

		// Token: 0x06006853 RID: 26707 RVA: 0x0021A70C File Offset: 0x0021890C
		private void InitialiseAllRoomStates()
		{
			this.SwitchState(GorillaComputer.ComputerState.Startup, true);
			this.InitialiseLanguageScreen();
			this.InitializeNameState();
			this.InitializeRoomState();
			this.InitializeTurnState();
			this.InitializeStartupState();
			this.InitializeQueueState();
			this.InitializeMicState();
			this.InitializeGroupState();
			this.InitializeVoiceState();
			this.InitializeAutoMuteState();
			this.InitializeGameMode();
			this.InitializeVisualsState();
			this.InitializeCreditsState();
			this.InitializeTimeState();
			this.InitializeSupportState();
			this.InitializeTroopState();
			this.InitializeKIdState();
			this.InitializeRedeemState();
		}

		// Token: 0x06006854 RID: 26708 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void InitializeStartupState()
		{
		}

		// Token: 0x06006855 RID: 26709 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void InitializeRoomState()
		{
		}

		// Token: 0x06006856 RID: 26710 RVA: 0x0021A790 File Offset: 0x00218990
		private void InitializeColorState()
		{
			this.redValue = PlayerPrefs.GetFloat("redValue", 0f);
			this.greenValue = PlayerPrefs.GetFloat("greenValue", 0f);
			this.blueValue = PlayerPrefs.GetFloat("blueValue", 0f);
			this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
			this.redText = Mathf.Floor(this.redValue * 9f).ToString();
			this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
			this.colorCursorLine = 0;
			GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
		}

		// Token: 0x06006857 RID: 26711 RVA: 0x0021A85C File Offset: 0x00218A5C
		private void InitializeNameState()
		{
			int @int = PlayerPrefs.GetInt("nameTagsOn", -1);
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (@int == -1)
				{
					this.NametagsEnabled = permissionDataByFeature.Enabled;
				}
				else
				{
					this.NametagsEnabled = (@int > 0);
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = (permissionDataByFeature.Enabled && @int > 0);
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
			NetworkSystem.Instance.SetMyNickName(this.savedName);
			this.currentName = this.savedName;
			VRRigCache.Instance.localRig.Rig.UpdateName();
			this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
			this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
			for (int i = 0; i < this.exactOneWeek.Length; i++)
			{
				this.exactOneWeek[i] = this.exactOneWeek[i].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int j = 0; j < this.anywhereOneWeek.Length; j++)
			{
				this.anywhereOneWeek[j] = this.anywhereOneWeek[j].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
			for (int k = 0; k < this.anywhereTwoWeek.Length; k++)
			{
				this.anywhereTwoWeek[k] = this.anywhereTwoWeek[k].ToLower().TrimEnd(new char[]
				{
					'\r',
					'\n'
				});
			}
		}

		// Token: 0x06006858 RID: 26712 RVA: 0x0021AA28 File Offset: 0x00218C28
		private void InitializeTurnState()
		{
			GorillaSnapTurn.LoadSettingsFromPlayerPrefs();
		}

		// Token: 0x06006859 RID: 26713 RVA: 0x0021AA30 File Offset: 0x00218C30
		private void InitializeMicState()
		{
			this.pttType = PlayerPrefs.GetString("pttType", "OPEN MIC");
			if (this.pttType == "ALL CHAT")
			{
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x0600685A RID: 26714 RVA: 0x0021AA84 File Offset: 0x00218C84
		private void InitializeAutoMuteState()
		{
			int @int = PlayerPrefs.GetInt("autoMute", 1);
			if (@int == 0)
			{
				this.autoMuteType = "OFF";
				return;
			}
			if (@int == 1)
			{
				this.autoMuteType = "MODERATE";
				return;
			}
			if (@int == 2)
			{
				this.autoMuteType = "AGGRESSIVE";
			}
		}

		// Token: 0x0600685B RID: 26715 RVA: 0x0021AACC File Offset: 0x00218CCC
		private void InitializeQueueState()
		{
			this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
			this.allowedInCompetitive = (PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1);
			if (!this.allowedInCompetitive && this.currentQueue == "COMPETITIVE")
			{
				PlayerPrefs.SetString("currentQueue", "DEFAULT");
				PlayerPrefs.Save();
				this.currentQueue = "DEFAULT";
			}
		}

		// Token: 0x0600685C RID: 26716 RVA: 0x0021AB3B File Offset: 0x00218D3B
		private void InitializeGroupState()
		{
			this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
			this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
			this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
		}

		// Token: 0x0600685D RID: 26717 RVA: 0x0021AB74 File Offset: 0x00218D74
		private void InitializeTroopState()
		{
			bool flag = false;
			this.troopToJoin = (this.troopName = PlayerPrefs.GetString("troopName", string.Empty));
			if (!this.rememberTroopQueueState)
			{
				bool flag2 = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
				bool flag3 = this.currentQueue != "DEFAULT" && this.currentQueue != "COMPETITIVE" && this.currentQueue != "MINIGAMES";
				if (flag2 || flag3)
				{
					this.currentQueue = "DEFAULT";
					PlayerPrefs.SetInt("troopQueueActive", 0);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
					PlayerPrefs.Save();
				}
			}
			this.troopQueueActive = (PlayerPrefs.GetInt("troopQueueActive", 0) == 1);
			if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
			{
				this.troopQueueActive = false;
				PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
				this.currentQueue = "DEFAULT";
				PlayerPrefs.SetString("currentQueue", this.currentQueue);
				flag = true;
			}
			if (this.troopQueueActive)
			{
				base.StartCoroutine(this.HandleInitialTroopQueueState());
			}
			if (flag)
			{
				PlayerPrefs.Save();
			}
		}

		// Token: 0x0600685E RID: 26718 RVA: 0x0021AC9F File Offset: 0x00218E9F
		private IEnumerator HandleInitialTroopQueueState()
		{
			Debug.Log("HandleInitialTroopQueueState()");
			while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				yield return null;
			}
			this.RequestTroopPopulation(false);
			while (this.currentTroopPopulation < 0)
			{
				yield return null;
			}
			if (this.currentTroopPopulation < 2)
			{
				Debug.Log("Low population - starting in DEFAULT queue");
				this.JoinDefaultQueue();
			}
			yield break;
		}

		// Token: 0x0600685F RID: 26719 RVA: 0x0021ACB0 File Offset: 0x00218EB0
		private void InitializeVoiceState()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			string text = PlayerPrefs.GetString("voiceChatOn", "");
			string defaultValue = "FALSE";
			switch (permissionDataByFeature.ManagedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(text))
				{
					defaultValue = (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
				}
				else
				{
					defaultValue = text;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (permissionDataByFeature.Enabled)
				{
					text = (string.IsNullOrEmpty(text) ? "FALSE" : text);
					defaultValue = text;
				}
				else
				{
					defaultValue = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				defaultValue = "FALSE";
				break;
			}
			this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", defaultValue);
		}

		// Token: 0x06006860 RID: 26720 RVA: 0x0021AD56 File Offset: 0x00218F56
		public void InitializeGameMode(string gameMode)
		{
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(gameMode, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(gameMode);
			this.didInitializeGameMode = true;
		}

		// Token: 0x06006861 RID: 26721 RVA: 0x0021AD88 File Offset: 0x00218F88
		private void InitializeGameMode()
		{
			if (this.didInitializeGameMode)
			{
				return;
			}
			GorillaComputer.sessionCount = PlayerPrefs.GetInt("sessionCount", -1);
			string text = PlayerPrefs.GetString("currentGameModePostSI");
			if (GorillaComputer.sessionCount == -1)
			{
				GorillaComputer.sessionCount = ((text.Length == 0) ? 0 : 100);
				PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
				text = GameModeType.Infection.ToString();
				PlayerPrefs.SetString("currentGameModePostSI", text);
				PlayerPrefs.Save();
			}
			else if (GorillaComputer.sessionCount == 3)
			{
				GorillaComputer.sessionCount++;
				PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
				if (!text.StartsWith("Super"))
				{
					text = ((text == GameModeType.Casual.ToString()) ? GameModeType.SuperCasual.ToString() : GameModeType.SuperInfect.ToString());
					PlayerPrefs.SetString("currentGameModePostSI", text);
				}
				PlayerPrefs.Save();
			}
			GameModeType gameModeType;
			try
			{
				gameModeType = Enum.Parse<GameModeType>(text, true);
			}
			catch
			{
				gameModeType = GameModeType.SuperInfect;
				text = GameModeType.SuperInfect.ToString();
			}
			if (!GameMode.GameModeZoneMapping.AllModes.Contains(gameModeType) || gameModeType == GameModeType.None || gameModeType == GameModeType.Count)
			{
				Debug.Log("[GT/GorillaComputer]  InitializeGameMode: Falling back to default game mode " + string.Format("\"{0}\" because stored game mode \"{1}\" is not available in any zone.", GameModeType.SuperInfect, gameModeType));
				PlayerPrefs.SetString("currentGameModePostSI", GameModeType.SuperInfect.ToString());
				PlayerPrefs.Save();
				text = GameModeType.SuperInfect.ToString();
			}
			this.leftHanded = (PlayerPrefs.GetInt("leftHanded", 0) == 1);
			this.OnModeSelectButtonPress(text, this.leftHanded);
			GameModePages.SetSelectedGameModeShared(text);
		}

		// Token: 0x06006862 RID: 26722 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void InitializeCreditsState()
		{
		}

		// Token: 0x06006863 RID: 26723 RVA: 0x0021AF48 File Offset: 0x00219148
		private void InitializeTimeState()
		{
			BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
		}

		// Token: 0x06006864 RID: 26724 RVA: 0x0021AF57 File Offset: 0x00219157
		private void InitializeSupportState()
		{
			this.displaySupport = false;
		}

		// Token: 0x06006865 RID: 26725 RVA: 0x0021AF60 File Offset: 0x00219160
		private void InitializeVisualsState()
		{
			this.disableParticles = (PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE");
			GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
			this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
		}

		// Token: 0x06006866 RID: 26726 RVA: 0x0021AFB4 File Offset: 0x002191B4
		private void InitializeRedeemState()
		{
			this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
		}

		// Token: 0x06006867 RID: 26727 RVA: 0x0021AFBD File Offset: 0x002191BD
		private bool CheckInternetConnection()
		{
			return Application.internetReachability > NetworkReachability.NotReachable;
		}

		// Token: 0x06006868 RID: 26728 RVA: 0x0021AFC8 File Offset: 0x002191C8
		public void OnConnectedToMasterStuff()
		{
			if (!this.isConnectedToMaster)
			{
				this.isConnectedToMaster = true;
				GorillaServer.Instance.ReturnCurrentVersion(new ReturnCurrentVersionRequest
				{
					CurrentVersion = NetworkSystemConfig.AppVersionStripped,
					UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest)
				}, new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion), new Action<PlayFabError>(GorillaComputer.OnErrorShared));
				if (this.startupMillis == 0L && !this.tryGetTimeAgain)
				{
					this.GetCurrentTime();
				}
				bool safety = PlayFabAuthenticator.instance.GetSafety();
				if (!KIDManager.KidEnabledAndReady && !KIDManager.HasSession)
				{
					this.SetComputerSettingsBySafety(safety, new GorillaComputer.ComputerState[]
					{
						GorillaComputer.ComputerState.Voice,
						GorillaComputer.ComputerState.AutoMute,
						GorillaComputer.ComputerState.Name,
						GorillaComputer.ComputerState.Group
					}, false);
				}
			}
		}

		// Token: 0x06006869 RID: 26729 RVA: 0x0021B078 File Offset: 0x00219278
		private void OnReturnCurrentVersion(ExecuteFunctionResult result)
		{
			JsonObject jsonObject = (JsonObject)result.FunctionResult;
			if (jsonObject == null)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			object obj;
			if (jsonObject.TryGetValue("SynchTime", out obj))
			{
				Debug.Log("message value is: " + (string)obj);
			}
			if (jsonObject.TryGetValue("Fail", out obj) && (bool)obj)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("ResultCode", out obj) && (ulong)obj != 0UL)
			{
				this.GeneralFailureMessage(this.versionMismatch);
				return;
			}
			if (jsonObject.TryGetValue("QueueStats", out obj))
			{
				JsonObject jsonObject2 = (JsonObject)obj;
				string str = "QueueStats: ";
				JsonObject jsonObject3 = jsonObject2;
				Debug.Log(str + ((jsonObject3 != null) ? jsonObject3.ToString() : null));
				if (jsonObject2.TryGetValue("TopTroops", out obj))
				{
					this.topTroops.Clear();
					foreach (object obj2 in ((JsonArray)obj))
					{
						this.topTroops.Add(obj2.ToString());
					}
				}
				if (jsonObject2.TryGetValue("TopVstumpMapIds", out obj))
				{
					this.topVstumpMaps.Clear();
					foreach (object obj3 in ((JsonArray)obj))
					{
						this.topVstumpMaps.Add(obj3.ToString());
					}
				}
			}
			if (jsonObject.TryGetValue("BannedUsers", out obj))
			{
				this.usersBanned = int.Parse((string)obj);
			}
			this.UpdateScreen();
		}

		// Token: 0x0600686A RID: 26730 RVA: 0x0021B240 File Offset: 0x00219440
		public void PressButton(GorillaKeyboardBindings buttonPressed)
		{
			if (this.currentState == GorillaComputer.ComputerState.Startup)
			{
				this.ProcessStartupState(buttonPressed);
				this.UpdateScreen();
				return;
			}
			this.RequestTroopPopulation(false);
			bool flag = true;
			if (buttonPressed == GorillaKeyboardBindings.up)
			{
				flag = false;
				this.DecreaseState();
			}
			else if (buttonPressed == GorillaKeyboardBindings.down)
			{
				flag = false;
				this.IncreaseState();
			}
			if (flag)
			{
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Name:
					this.ProcessNameState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Turn:
					this.ProcessTurnState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Mic:
					this.ProcessMicState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Room:
					this.ProcessRoomState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Queue:
					this.ProcessQueueState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Group:
					this.ProcessGroupState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Voice:
					this.ProcessVoiceState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.ProcessAutoMuteState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Credits:
					this.ProcessCreditsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.ProcessVisualsState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.ProcessNameWarningState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Support:
					this.ProcessSupportState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Troop:
					this.ProcessTroopState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.KID:
					this.ProcessKIdState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.ProcessRedemptionState(buttonPressed);
					break;
				case GorillaComputer.ComputerState.Language:
					this.ProcessLanguageState(buttonPressed);
					break;
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x0600686B RID: 26731 RVA: 0x0021B388 File Offset: 0x00219588
		public void OnModeSelectButtonPress(string gameMode, bool leftHand)
		{
			this.lastPressedGameMode = gameMode;
			this.lastPressedGameModeType = (GameModeType)GameMode.gameModeKeyByName.GetValueOrDefault(gameMode, 11);
			PlayerPrefs.SetString("currentGameModePostSI", gameMode);
			if (leftHand != this.leftHanded)
			{
				PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
				this.leftHanded = leftHand;
			}
			PlayerPrefs.Save();
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
				return;
			}
			this.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x0600686C RID: 26732 RVA: 0x0021B3FF File Offset: 0x002195FF
		public void SetGameModeWithoutButton(string gameMode)
		{
			this.currentGameMode.Value = gameMode;
			this.UpdateGameModeText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
		}

		// Token: 0x0600686D RID: 26733 RVA: 0x0021B41F File Offset: 0x0021961F
		public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.primaryTriggersByZone[trigger.networkZone] = trigger;
		}

		// Token: 0x0600686E RID: 26734 RVA: 0x0021B434 File Offset: 0x00219634
		private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], out result);
			return result;
		}

		// Token: 0x0600686F RID: 26735 RVA: 0x0021B46C File Offset: 0x0021966C
		public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
		{
			GorillaNetworkJoinTrigger result;
			this.primaryTriggersByZone.TryGetValue(zone, out result);
			return result;
		}

		// Token: 0x06006870 RID: 26736 RVA: 0x0021B48C File Offset: 0x0021968C
		public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
		{
			foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
			{
				if (gameModeString.StartsWith(keyValuePair.Key))
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x06006871 RID: 26737 RVA: 0x0021B4F4 File Offset: 0x002196F4
		public void OnGroupJoinButtonPress(int mapJoinIndex, GorillaFriendCollider chosenFriendJoinCollider)
		{
			Debug.Log("On Group button press. Map:" + mapJoinIndex.ToString() + " - collider: " + chosenFriendJoinCollider.name);
			if (mapJoinIndex >= this.allowedMapsToJoin.Length)
			{
				this.roomNotAllowed = true;
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
			if (!FriendshipGroupDetection.Instance.IsInParty)
			{
				if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
				{
					PhotonNetworkController.Instance.FriendIDList = new List<string>(chosenFriendJoinCollider.playerIDsCurrentlyTouching);
					foreach (string str in this.networkController.FriendIDList)
					{
						Debug.Log("Friend ID:" + str);
					}
					PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
					RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
					PhotonNetwork.SendAllOutgoingCommands();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby, null, false);
					this.currentStateIndex = 0;
					this.SwitchState(this.GetState(this.currentStateIndex), true);
				}
				return;
			}
			if (selectedMapJoinTrigger != null && selectedMapJoinTrigger.CanPartyJoin())
			{
				PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty, null, false);
				this.currentStateIndex = 0;
				this.SwitchState(this.GetState(this.currentStateIndex), true);
				return;
			}
			this.UpdateScreen();
		}

		// Token: 0x06006872 RID: 26738 RVA: 0x0021B6E8 File Offset: 0x002198E8
		public void CompQueueUnlockButtonPress()
		{
			this.allowedInCompetitive = true;
			PlayerPrefs.SetInt("allowedInCompetitive", 1);
			PlayerPrefs.Save();
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.RequestUnlockCompetitiveQueue(true);
			}
		}

		// Token: 0x06006873 RID: 26739 RVA: 0x0021B71C File Offset: 0x0021991C
		private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
		{
			if (this.currentComputerState == GorillaComputer.ComputerState.Mic && this.currentComputerState != newState)
			{
				this.updateCooldown = this.defaultUpdateCooldown;
			}
			else if (newState == GorillaComputer.ComputerState.Mic)
			{
				this.updateCooldown = this.micUpdateCooldown;
			}
			if (this.previousComputerState != this.currentComputerState)
			{
				this.previousComputerState = this.currentComputerState;
			}
			this.currentComputerState = newState;
			if (this.LoadingRoutine != null)
			{
				base.StopCoroutine(this.LoadingRoutine);
			}
			if (clearStack)
			{
				this.stateStack.Clear();
			}
			this.stateStack.Push(newState);
		}

		// Token: 0x06006874 RID: 26740 RVA: 0x0021B7A8 File Offset: 0x002199A8
		private void PopState()
		{
			this.currentComputerState = this.previousComputerState;
			if (this.stateStack.Count <= 1)
			{
				Debug.LogError("Can't pop into an empty stack");
				return;
			}
			this.stateStack.Pop();
			this.UpdateScreen();
		}

		// Token: 0x06006875 RID: 26741 RVA: 0x0021B7E1 File Offset: 0x002199E1
		private void SwitchToWarningState()
		{
			this.warningConfirmationInputString = string.Empty;
			this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
		}

		// Token: 0x06006876 RID: 26742 RVA: 0x0021B7F7 File Offset: 0x002199F7
		private void SwitchToLoadingState()
		{
			this.SwitchState(GorillaComputer.ComputerState.Loading, false);
		}

		// Token: 0x06006877 RID: 26743 RVA: 0x0021B802 File Offset: 0x00219A02
		private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
		{
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x06006878 RID: 26744 RVA: 0x0021B818 File Offset: 0x00219A18
		private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.enter:
				return;
			case GorillaKeyboardBindings.option1:
				this.colorCursorLine = 0;
				return;
			case GorillaKeyboardBindings.option2:
				this.colorCursorLine = 1;
				return;
			case GorillaKeyboardBindings.option3:
				this.colorCursorLine = 2;
				return;
			default:
			{
				int num = (int)buttonPressed;
				if (num < 10)
				{
					switch (this.colorCursorLine)
					{
					case 0:
						this.redText = num.ToString();
						this.redValue = (float)num / 9f;
						PlayerPrefs.SetFloat("redValue", this.redValue);
						break;
					case 1:
						this.greenText = num.ToString();
						this.greenValue = (float)num / 9f;
						PlayerPrefs.SetFloat("greenValue", this.greenValue);
						break;
					case 2:
						this.blueText = num.ToString();
						this.blueValue = (float)num / 9f;
						PlayerPrefs.SetFloat("blueValue", this.blueValue);
						break;
					}
					GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
					PlayerPrefs.Save();
					if (NetworkSystem.Instance.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
						{
							this.redValue,
							this.greenValue,
							this.blueValue
						});
					}
				}
				return;
			}
			}
		}

		// Token: 0x06006879 RID: 26745 RVA: 0x0021B978 File Offset: 0x00219B78
		public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (this.currentName.Length > 0 && this.NametagsEnabled)
					{
						this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (this.currentName != this.savedName && this.currentName != "" && this.NametagsEnabled)
					{
						this.CheckAutoBanListForPlayerName(this.currentName);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.UpdateNametagSetting(!this.NametagsEnabled, true);
					return;
				default:
					if (this.NametagsEnabled && this.currentName.Length < 12 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
					{
						string str = this.currentName;
						string str2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							str2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							str2 = num.ToString();
						}
						this.currentName = str + str2;
					}
					break;
				}
			}
		}

		// Token: 0x0600687A RID: 26746 RVA: 0x0021BA8C File Offset: 0x00219C8C
		private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.delete:
				if (flag && ((this.playerInVirtualStump && this.roomToJoin.Length > 1) || (!this.playerInVirtualStump && this.roomToJoin.Length > 0)))
				{
					this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
					return;
				}
				break;
			case GorillaKeyboardBindings.enter:
				if (flag && ((!this.playerInVirtualStump && this.roomToJoin != "") || (this.playerInVirtualStump && this.roomToJoin.Length > 1)))
				{
					this.CheckAutoBanListForRoomName(this.roomToJoin);
					return;
				}
				break;
			case GorillaKeyboardBindings.option1:
				if (FriendshipGroupDetection.Instance.IsInParty)
				{
					FriendshipGroupDetection.Instance.LeaveParty();
					this.DisconnectAfterDelay(1f);
					return;
				}
				NetworkSystem.Instance.ReturnToSinglePlayer();
				return;
			case GorillaKeyboardBindings.option2:
				this.RequestUpdatedPermissions();
				return;
			case GorillaKeyboardBindings.option3:
				break;
			default:
				if (flag && this.roomToJoin.Length < 10)
				{
					string str = this.roomToJoin;
					string str2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						str2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						str2 = num.ToString();
					}
					this.roomToJoin = str + str2;
				}
				break;
			}
		}

		// Token: 0x0600687B RID: 26747 RVA: 0x0021BBE8 File Offset: 0x00219DE8
		private void DisconnectAfterDelay(float seconds)
		{
			GorillaComputer.<DisconnectAfterDelay>d__378 <DisconnectAfterDelay>d__;
			<DisconnectAfterDelay>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisconnectAfterDelay>d__.seconds = seconds;
			<DisconnectAfterDelay>d__.<>1__state = -1;
			<DisconnectAfterDelay>d__.<>t__builder.Start<GorillaComputer.<DisconnectAfterDelay>d__378>(ref <DisconnectAfterDelay>d__);
		}

		// Token: 0x0600687C RID: 26748 RVA: 0x0021BC20 File Offset: 0x00219E20
		private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				GorillaSnapTurn.UpdateAndSaveTurnFactor((int)buttonPressed);
				return;
			}
			string text = string.Empty;
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				text = "SNAP";
				break;
			case GorillaKeyboardBindings.option2:
				text = "SMOOTH";
				break;
			case GorillaKeyboardBindings.option3:
				text = "NONE";
				break;
			}
			if (text.Length > 0)
			{
				GorillaSnapTurn.UpdateAndSaveTurnType(text);
			}
		}

		// Token: 0x0600687D RID: 26749 RVA: 0x0021BC80 File Offset: 0x00219E80
		private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.pttType = "OPEN MIC";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option2:
				this.pttType = "PUSH TO TALK";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			case GorillaKeyboardBindings.option3:
				this.pttType = "PUSH TO MUTE";
				PlayerPrefs.SetString("pttType", this.pttType);
				PlayerPrefs.Save();
				return;
			default:
				return;
			}
		}

		// Token: 0x0600687E RID: 26750 RVA: 0x0021BD08 File Offset: 0x00219F08
		private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.JoinQueue("DEFAULT", false);
				return;
			case GorillaKeyboardBindings.option2:
				this.JoinQueue("MINIGAMES", false);
				return;
			case GorillaKeyboardBindings.option3:
				if (this.allowedInCompetitive)
				{
					this.JoinQueue("COMPETITIVE", false);
				}
				return;
			default:
				return;
			}
		}

		// Token: 0x0600687F RID: 26751 RVA: 0x0021BD64 File Offset: 0x00219F64
		public void JoinTroop(string newTroopName)
		{
			if (this.IsValidTroopName(newTroopName))
			{
				this.currentTroopPopulation = -1;
				this.troopName = newTroopName;
				PlayerPrefs.SetString("troopName", this.troopName);
				if (this.troopQueueActive)
				{
					this.currentQueue = this.GetQueueNameForTroop(this.troopName);
					PlayerPrefs.SetString("currentQueue", this.currentQueue);
				}
				PlayerPrefs.Save();
				this.JoinTroopQueue();
			}
		}

		// Token: 0x06006880 RID: 26752 RVA: 0x0021BDCD File Offset: 0x00219FCD
		public void JoinTroopQueue()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.currentTroopPopulation = -1;
				this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
				this.RequestTroopPopulation(true);
			}
		}

		// Token: 0x06006881 RID: 26753 RVA: 0x0021BE00 File Offset: 0x0021A000
		private void RequestTroopPopulation(bool forceUpdate = false)
		{
			if (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
			{
				return;
			}
			if (!this.hasRequestedInitialTroopPopulation || forceUpdate)
			{
				if (this.nextPopulationCheckTime > Time.realtimeSinceStartup)
				{
					return;
				}
				this.nextPopulationCheckTime = Time.realtimeSinceStartup + this.troopPopulationCheckCooldown;
				this.hasRequestedInitialTroopPopulation = true;
				GorillaServer.Instance.ReturnQueueStats(new ReturnQueueStatsRequest
				{
					queueName = this.troopName
				}, delegate(ExecuteFunctionResult result)
				{
					Debug.Log("Troop pop received");
					object obj;
					if (((JsonObject)result.FunctionResult).TryGetValue("PlayerCount", out obj))
					{
						this.currentTroopPopulation = int.Parse(obj.ToString());
						if (this.currentComputerState == GorillaComputer.ComputerState.Queue)
						{
							this.UpdateScreen();
							return;
						}
					}
					else
					{
						this.currentTroopPopulation = 0;
					}
				}, delegate(PlayFabError error)
				{
					Debug.LogError(string.Format("Error requesting troop population: {0}", error));
					this.currentTroopPopulation = -1;
				});
			}
		}

		// Token: 0x06006882 RID: 26754 RVA: 0x0021BE7E File Offset: 0x0021A07E
		public void JoinDefaultQueue()
		{
			this.JoinQueue("DEFAULT", false);
		}

		// Token: 0x06006883 RID: 26755 RVA: 0x0021BE8C File Offset: 0x0021A08C
		public void LeaveTroop()
		{
			if (this.IsValidTroopName(this.troopName))
			{
				this.troopToJoin = this.troopName;
			}
			this.currentTroopPopulation = -1;
			this.troopName = string.Empty;
			PlayerPrefs.SetString("troopName", this.troopName);
			if (this.troopQueueActive)
			{
				this.JoinDefaultQueue();
			}
			PlayerPrefs.Save();
		}

		// Token: 0x06006884 RID: 26756 RVA: 0x0021BEE8 File Offset: 0x0021A0E8
		public string GetCurrentTroop()
		{
			if (this.troopQueueActive)
			{
				return this.troopName;
			}
			return this.currentQueue;
		}

		// Token: 0x06006885 RID: 26757 RVA: 0x0021BEFF File Offset: 0x0021A0FF
		public int GetCurrentTroopPopulation()
		{
			if (this.troopQueueActive)
			{
				return this.currentTroopPopulation;
			}
			return -1;
		}

		// Token: 0x06006886 RID: 26758 RVA: 0x0021BF14 File Offset: 0x0021A114
		private void JoinQueue(string queueName, bool isTroopQueue = false)
		{
			this.currentQueue = queueName;
			this.troopQueueActive = isTroopQueue;
			this.currentTroopPopulation = -1;
			PlayerPrefs.SetString("currentQueue", this.currentQueue);
			PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
			PlayerPrefs.Save();
		}

		// Token: 0x06006887 RID: 26759 RVA: 0x0021BF64 File Offset: 0x0021A164
		private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.one:
				this.groupMapJoin = "FOREST";
				this.groupMapJoinIndex = 0;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.two:
				this.groupMapJoin = "CAVE";
				this.groupMapJoinIndex = 1;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.three:
				this.groupMapJoin = "CANYON";
				this.groupMapJoinIndex = 2;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.four:
				this.groupMapJoin = "CITY";
				this.groupMapJoinIndex = 3;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			case GorillaKeyboardBindings.five:
				this.groupMapJoin = "CLOUDS";
				this.groupMapJoinIndex = 4;
				PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
				PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
				PlayerPrefs.Save();
				break;
			default:
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
				}
				break;
			}
			this.roomFull = false;
		}

		// Token: 0x06006888 RID: 26760 RVA: 0x0021C0F4 File Offset: 0x0021A2F4
		private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.limitOnlineScreens)
			{
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups);
			bool flag2 = this.IsValidTroopName(this.troopName);
			if (flag)
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.delete:
					if (!flag2 && this.troopToJoin.Length > 0)
					{
						this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
						return;
					}
					break;
				case GorillaKeyboardBindings.enter:
					if (!flag2)
					{
						this.CheckAutoBanListForTroopName(this.troopToJoin);
						return;
					}
					break;
				case GorillaKeyboardBindings.option1:
					this.JoinTroopQueue();
					return;
				case GorillaKeyboardBindings.option2:
					this.JoinDefaultQueue();
					return;
				case GorillaKeyboardBindings.option3:
					this.LeaveTroop();
					return;
				default:
					if (!flag2 && this.troopToJoin.Length < 12)
					{
						string str = this.troopToJoin;
						string str2;
						if (buttonPressed >= GorillaKeyboardBindings.up)
						{
							str2 = buttonPressed.ToString();
						}
						else
						{
							int num = (int)buttonPressed;
							str2 = num.ToString();
						}
						this.troopToJoin = str + str2;
						return;
					}
					break;
				}
			}
			else
			{
				switch (buttonPressed)
				{
				case GorillaKeyboardBindings.option1:
					break;
				case GorillaKeyboardBindings.option2:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
					{
						this.ProcessScreen_SetupKID();
						return;
					}
					this.RequestUpdatedPermissions();
					return;
				case GorillaKeyboardBindings.option3:
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06006889 RID: 26761 RVA: 0x0021C21D File Offset: 0x0021A41D
		private bool IsValidTroopName(string troop)
		{
			return !string.IsNullOrEmpty(troop) && troop.Length <= 12 && (this.allowedInCompetitive || troop != "COMPETITIVE");
		}

		// Token: 0x0600688A RID: 26762 RVA: 0x0002230F File Offset: 0x0002050F
		private string GetQueueNameForTroop(string troop)
		{
			return troop;
		}

		// Token: 0x0600688B RID: 26763 RVA: 0x0021C248 File Offset: 0x0021A448
		private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
		{
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				if (buttonPressed != GorillaKeyboardBindings.option1)
				{
					if (buttonPressed == GorillaKeyboardBindings.option2)
					{
						this.SetVoice(false, true);
					}
				}
				else
				{
					this.SetVoice(true, true);
				}
			}
			else if (buttonPressed != GorillaKeyboardBindings.option2)
			{
				if (buttonPressed == GorillaKeyboardBindings.option3)
				{
					if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
					{
						return;
					}
					this.ProcessScreen_SetupKID();
				}
			}
			else if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
			{
				this.ProcessScreen_SetupKID();
			}
			else
			{
				this.RequestUpdatedPermissions();
			}
			RigContainer.RefreshAllRigVoices();
		}

		// Token: 0x0600688C RID: 26764 RVA: 0x0021C2B8 File Offset: 0x0021A4B8
		private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
		{
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.autoMuteType = "AGGRESSIVE";
				PlayerPrefs.SetInt("autoMute", 2);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option2:
				this.autoMuteType = "MODERATE";
				PlayerPrefs.SetInt("autoMute", 1);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			case GorillaKeyboardBindings.option3:
				this.autoMuteType = "OFF";
				PlayerPrefs.SetInt("autoMute", 0);
				PlayerPrefs.Save();
				RigContainer.RefreshAllRigVoices();
				break;
			}
			this.UpdateScreen();
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x0021C348 File Offset: 0x0021A548
		private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed < GorillaKeyboardBindings.up)
			{
				this.instrumentVolume = (float)buttonPressed / 50f;
				PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
				PlayerPrefs.Save();
				return;
			}
			switch (buttonPressed)
			{
			case GorillaKeyboardBindings.option1:
				this.disableParticles = false;
				PlayerPrefs.SetString("disableParticles", "FALSE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				return;
			case GorillaKeyboardBindings.option2:
				this.disableParticles = true;
				PlayerPrefs.SetString("disableParticles", "TRUE");
				PlayerPrefs.Save();
				GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
				break;
			case GorillaKeyboardBindings.option3:
				break;
			default:
				return;
			}
		}

		// Token: 0x0600688E RID: 26766 RVA: 0x0021C3F3 File Offset: 0x0021A5F3
		private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.creditsView.ProcessButtonPress(buttonPressed);
			}
		}

		// Token: 0x0600688F RID: 26767 RVA: 0x0021C406 File Offset: 0x0021A606
		private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.displaySupport = true;
			}
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x0021C414 File Offset: 0x0021A614
		private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
			{
				return;
			}
			if (buttonPressed != GorillaKeyboardBindings.delete)
			{
				if (buttonPressed == GorillaKeyboardBindings.enter)
				{
					if (this.redemptionCode != "")
					{
						if (this.redemptionCode.Length < 8)
						{
							this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
							return;
						}
						CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
						return;
					}
					else if (this.RedemptionStatus != GorillaComputer.RedemptionResult.Success)
					{
						this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
						return;
					}
				}
				else if (this.redemptionCode.Length < 8 && (buttonPressed < GorillaKeyboardBindings.up || buttonPressed > GorillaKeyboardBindings.option3))
				{
					string str = this.redemptionCode;
					string str2;
					if (buttonPressed >= GorillaKeyboardBindings.up)
					{
						str2 = buttonPressed.ToString();
					}
					else
					{
						int num = (int)buttonPressed;
						str2 = num.ToString();
					}
					this.redemptionCode = str + str2;
				}
			}
			else if (this.redemptionCode.Length > 0)
			{
				this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
				return;
			}
		}

		// Token: 0x06006891 RID: 26769 RVA: 0x0021C500 File Offset: 0x0021A700
		private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
		{
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				this.PopState();
				return;
			}
			if (buttonPressed == GorillaKeyboardBindings.delete)
			{
				if (this.warningConfirmationInputString.Length > 0)
				{
					this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
					return;
				}
			}
			else if (this.warningConfirmationInputString.Length < 3)
			{
				this.warningConfirmationInputString += buttonPressed.ToString();
			}
		}

		// Token: 0x06006892 RID: 26770 RVA: 0x0021C58C File Offset: 0x0021A78C
		public void UpdateScreen()
		{
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.WrongVersion)
			{
				this.UpdateFunctionScreen();
				switch (this.currentState)
				{
				case GorillaComputer.ComputerState.Startup:
					this.StartupScreen();
					break;
				case GorillaComputer.ComputerState.Name:
					this.NameScreen();
					break;
				case GorillaComputer.ComputerState.Turn:
					this.TurnScreen();
					break;
				case GorillaComputer.ComputerState.Mic:
					this.MicScreen();
					break;
				case GorillaComputer.ComputerState.Room:
					this.RoomScreen();
					break;
				case GorillaComputer.ComputerState.Queue:
					this.QueueScreen();
					break;
				case GorillaComputer.ComputerState.Group:
					this.GroupScreen();
					break;
				case GorillaComputer.ComputerState.Voice:
					this.VoiceScreen();
					break;
				case GorillaComputer.ComputerState.AutoMute:
					this.AutomuteScreen();
					break;
				case GorillaComputer.ComputerState.Credits:
					this.CreditsScreen();
					break;
				case GorillaComputer.ComputerState.Visuals:
					this.VisualsScreen();
					break;
				case GorillaComputer.ComputerState.Time:
					this.TimeScreen();
					break;
				case GorillaComputer.ComputerState.NameWarning:
					this.NameWarningScreen();
					break;
				case GorillaComputer.ComputerState.Loading:
					this.LoadingScreen();
					break;
				case GorillaComputer.ComputerState.Support:
					this.SupportScreen();
					break;
				case GorillaComputer.ComputerState.Troop:
					this.TroopScreen();
					break;
				case GorillaComputer.ComputerState.KID:
					this.KIdScreen();
					break;
				case GorillaComputer.ComputerState.Redemption:
					this.RedemptionScreen();
					break;
				case GorillaComputer.ComputerState.Language:
					this.LanguageScreen();
					break;
				}
			}
			this.UpdateGameModeText();
		}

		// Token: 0x06006893 RID: 26771 RVA: 0x0021C6C4 File Offset: 0x0021A8C4
		private void LoadingScreen()
		{
			GorillaComputer.<>c__DisplayClass404_0 CS$<>8__locals1 = new GorillaComputer.<>c__DisplayClass404_0();
			CS$<>8__locals1.<>4__this = this;
			string defaultResult = "LOADING";
			LocalisationManager.TryGetKeyForCurrentLocale("LOADING_SCREEN", out CS$<>8__locals1.result, defaultResult);
			this.screenText.Set(CS$<>8__locals1.result);
			this.LoadingRoutine = base.StartCoroutine(CS$<>8__locals1.<LoadingScreen>g__LoadingScreenLocal|0());
		}

		// Token: 0x06006894 RID: 26772 RVA: 0x0021C71C File Offset: 0x0021A91C
		private void NameWarningScreen()
		{
			string defaultResult = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN", out text, defaultResult);
			this.screenText.Set(text);
			if (this.warningConfirmationInputString.ToLower() == "yes")
			{
				defaultResult = "\n\nPRESS ANY KEY TO CONTINUE";
				LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_CONFIRMATION", out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			defaultResult = "\n\nTYPE 'YES' TO CONFIRM:";
			LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_TYPE_YES", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			this.screenText.Append(this.warningConfirmationInputString);
		}

		// Token: 0x06006895 RID: 26773 RVA: 0x0021C7B8 File Offset: 0x0021A9B8
		private void SupportScreen()
		{
			this.screenText.Set("");
			if (this.displaySupport)
			{
				string text = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
				string text2;
				if (text == "PC")
				{
					text2 = "OCULUS PC";
				}
				else
				{
					text2 = text;
				}
				text = text2;
				string key;
				if (!(text == "OCULUS PC"))
				{
					if (!(text == "STEAM"))
					{
						if (!(text == "PSVR"))
						{
							if (!(text == "PICO"))
							{
								if (!(text == "QUEST"))
								{
									key = "UNKNOWN_PLATFORM";
								}
								else
								{
									key = "PLATFORM_QUEST";
								}
							}
							else
							{
								key = "PLATFORM_PICO";
							}
						}
						else
						{
							key = "PLATFORM_PSVR";
						}
					}
					else
					{
						key = "PLATFORM_STEAM";
					}
				}
				else
				{
					key = "PLATFORM_OCULUS_PC";
				}
				string text3;
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text3, text);
				text = text3;
				string defaultResult = "SUPPORT";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out text3, defaultResult);
				this.screenText.Append(text3);
				defaultResult = "\n\nPLAYER ID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLAYERID", out text3, defaultResult);
				this.screenText.Append(text3 + "  ");
				this.screenText.Append(PlayFabAuthenticator.instance.GetPlayFabPlayerId());
				defaultResult = "\nVERSION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_VERSION", out text3, defaultResult);
				this.screenText.Append(text3 + " ");
				this.screenText.Append(this.version.ToUpper());
				defaultResult = "\nPLATFORM";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLATFORM", out text3, defaultResult);
				this.screenText.Append(text3 + " ");
				this.screenText.Append(text);
				defaultResult = "\nBUILD DATE";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_BUILD_DATE", out text3, defaultResult);
				this.screenText.Append(text3 + " ");
				this.screenText.Append(this.buildDate);
				defaultResult = "\nSESSION ID";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID", out text3, defaultResult);
				string sessionId = MothershipClientApiUnity.SessionId;
				string str = sessionId;
				int num = sessionId.LastIndexOf('-');
				if (num >= 0)
				{
					string str2 = sessionId.Substring(0, num);
					string str3 = "\n            ";
					text2 = sessionId;
					int num2 = num + 1;
					str = str2 + str3 + text2.Substring(num2, text2.Length - num2);
				}
				this.screenText.Append(text3 + " ");
				this.screenText.Append(str);
				if (KIDManager.KidEnabled)
				{
					defaultResult = "\nk-ID ACCOUNT TYPE:";
					LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_KID_ACCOUNT_TYPE", out text3, defaultResult);
					this.screenText.Append(text3.TrailingSpace());
					this.screenText.Append(KIDManager.GetActiveAccountStatusNiceString().ToUpper());
					return;
				}
			}
			else
			{
				string defaultResult2 = "SUPPORT";
				string str4;
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out str4, defaultResult2);
				this.screenText.Append(str4);
				defaultResult2 = "\n\nPRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL", out str4, defaultResult2);
				this.screenText.Append(str4);
				defaultResult2 = "\n\n\n\n<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER THAN ANOTHER AXIOM</color>";
				LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL_WARNING", out str4, defaultResult2);
				this.screenText.Append(str4);
			}
		}

		// Token: 0x06006896 RID: 26774 RVA: 0x0021CADC File Offset: 0x0021ACDC
		private void TimeScreen()
		{
			string defaultResult = "UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: {currentSetting}.\nTIME OF DAY: {currentTimeOfDay}.\n";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("TIME_SCREEN", out text, defaultResult);
			text = text.Replace("{currentSetting}", BetterDayNightManager.instance.currentSetting.ToString().ToUpper()).Replace("{currentTimeOfDay}", BetterDayNightManager.instance.currentTimeOfDay.ToUpper());
			this.screenText.Set(text);
		}

		// Token: 0x06006897 RID: 26775 RVA: 0x0021CB4C File Offset: 0x0021AD4C
		private void CreditsScreen()
		{
			this.screenText.Set(this.creditsView.GetScreenText());
		}

		// Token: 0x06006898 RID: 26776 RVA: 0x0021CB64 File Offset: 0x0021AD64
		private void VisualsScreen()
		{
			string defaultResult = "UPDATE ITEMS SETTINGS.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_INTRO", out text, defaultResult);
			this.screenText.Set(text.TrailingSpace());
			defaultResult = "PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_OPTIONS", out text, defaultResult);
			this.screenText.Append(text);
			defaultResult = "\n\nITEM PARTICLES ON:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_CURRENT", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string text2 = this.disableParticles ? "FALSE" : "TRUE";
			LocalisationManager.TryGetKeyForCurrentLocale(text2, out text, text2);
			this.screenText.Append(text);
			defaultResult = "\nINSTRUMENT VOLUME:";
			LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_VOLUME", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			this.screenText.Append(Mathf.CeilToInt(this.instrumentVolume * 50f).ToString());
		}

		// Token: 0x06006899 RID: 26777 RVA: 0x0021CC50 File Offset: 0x0021AE50
		private void VoiceScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
			{
				string defaultResult = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK.";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_INTRO", out text, defaultResult);
				this.screenText.Set(text);
				defaultResult = "\nPRESS OPTION 1 = HUMAN VOICES.\nPRESS OPTION 2 = MONKE VOICES.";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_OPTIONS", out text, defaultResult);
				this.screenText.Append(text);
				defaultResult = "\n\nVOICE TYPE:";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_CURRENT", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				string key = (this.voiceChatOn == "TRUE") ? "VOICE_OPTION_HUMAN" : ((this.voiceChatOn == "FALSE") ? "VOICE_OPTION_MONKE" : "VOICE_OPTION_OFF");
				defaultResult = ((this.voiceChatOn == "TRUE") ? "HUMAN" : ((this.voiceChatOn == "FALSE") ? "MONKE" : "OFF"));
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.VoiceScreen_KIdProhibited();
				return;
			}
			this.VoiceScreen_Permission();
		}

		// Token: 0x0600689A RID: 26778 RVA: 0x0021CD70 File Offset: 0x0021AF70
		private void AutomuteScreen()
		{
			string defaultResult = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_INTRO", out text, defaultResult);
			this.screenText.Set(text);
			defaultResult = "\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_OPTIONS", out text, defaultResult);
			this.screenText.Append(text);
			defaultResult = "\n\nCURRENT AUTOMOD LEVEL: ";
			LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_CURRENT", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string key = "AUTOMOD_OFF";
			string a = this.autoMuteType;
			if (!(a == "OFF"))
			{
				if (!(a == "MODERATE"))
				{
					if (a == "AGGRESSIVE")
					{
						key = "AUTOMOD_AGGRESSIVE";
					}
				}
				else
				{
					key = "AUTOMOD_MODERATE";
				}
			}
			else
			{
				key = "AUTOMOD_OFF";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, this.autoMuteType);
			this.screenText.Append(text);
		}

		// Token: 0x0600689B RID: 26779 RVA: 0x0021CE4C File Offset: 0x0021B04C
		private void GroupScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string text = "";
			string str = (this.allowedMapsToJoin.Length > 1) ? this.groupMapJoin : this.allowedMapsToJoin[0].ToUpper();
			string str2 = "";
			string defaultResult;
			if (this.allowedMapsToJoin.Length > 1)
			{
				defaultResult = "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CAVE, 3: CANYON, 4: CITY, 5: CLOUDS.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_DESTINATIONS", out text, defaultResult);
				str2 = text;
			}
			defaultResult = "\n\nACTIVE ZONE WILL BE:";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ACTIVE_ZONES", out text, defaultResult);
			string text2 = text.TrailingSpace();
			text2 = text2 + str + str2;
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
				string str3 = "";
				if (selectedMapJoinTrigger.CanPartyJoin())
				{
					defaultResult = "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>";
					LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_CANNOT_JOIN", out text, defaultResult);
					str3 = text;
				}
				defaultResult = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.";
				LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_PARTY", out text, defaultResult);
				this.screenText.Set(text);
				text2 += str3;
				this.screenText.Append(text2);
				return;
			}
			defaultResult = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.";
			LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_NOPARTY", out text, defaultResult);
			this.screenText.Set(text);
			this.screenText.Append(text2);
		}

		// Token: 0x0600689C RID: 26780 RVA: 0x0021CF80 File Offset: 0x0021B180
		private void MicScreen()
		{
			if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.MicScreen_KIdProhibited();
				return;
			}
			bool flag = false;
			string str = "";
			if (Microphone.devices.Length == 0)
			{
				flag = true;
				str = "NO MICROPHONE DETECTED";
			}
			if (flag)
			{
				string str2;
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_MIC_DISABLED", out str2, "MIC DISABLED: ");
				this.screenText.Set(str2 + str);
				return;
			}
			string defaultResult = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_OPTIONS", out text, defaultResult);
			this.screenText.Set(text);
			defaultResult = "\n\nCURRENT MIC SETTING:";
			LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_CURRENT", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string key = "";
			string a = this.pttType;
			if (!(a == "PUSH TO MUTE"))
			{
				if (!(a == "PUSH TO TALK"))
				{
					if (!(a == "OPEN MIC"))
					{
						if (a == "ALL CHAT")
						{
							key = "OPEN_MIC";
						}
					}
					else
					{
						key = "OPEN_MIC";
					}
				}
				else
				{
					key = "PUSH_TO_TALK_MIC";
				}
			}
			else
			{
				key = "PUSH_TO_MUTE_MIC";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, this.pttType);
			this.screenText.Append(text);
			if (this.pttType == "PUSH TO MUTE")
			{
				defaultResult = "- MIC IS OPEN.\n- HOLD ANY FACE BUTTON TO MUTE.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP", out text, defaultResult);
				this.screenText.Append(text);
			}
			else if (this.pttType == "PUSH TO TALK")
			{
				defaultResult = "- MIC IS MUTED.\n- HOLD ANY FACE BUTTON TO TALK.\n\n";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_TALK_TOOLTIP", out text, defaultResult);
				this.screenText.Append(text);
			}
			else
			{
				this.screenText.Append("\n\n\n");
			}
			if (this.speakerLoudness == null)
			{
				this.speakerLoudness = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
			}
			if (this.speakerLoudness != null)
			{
				float num = Mathf.Sqrt(this.speakerLoudness.LoudnessNormalized);
				if (num <= 0.01f)
				{
					this.micInputTestTimer += this.deltaTime;
				}
				else
				{
					this.micInputTestTimer = 0f;
				}
				if (this.pttType != "OPEN MIC")
				{
					bool flag2 = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
					bool flag3 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
					bool flag4 = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
					bool flag5 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
					bool flag6 = flag2 || flag3 || flag4 || flag5;
					if (flag6 && this.pttType == "PUSH TO MUTE")
					{
						defaultResult = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text, defaultResult);
						this.screenText.Append(text);
						return;
					}
					if (!flag6 && this.pttType == "PUSH TO TALK")
					{
						defaultResult = "INPUT TEST: ";
						LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text, defaultResult);
						this.screenText.Append(text);
						return;
					}
				}
				if (this.micInputTestTimer >= this.micInputTestTimerThreshold)
				{
					defaultResult = "NO MIC INPUT DETECTED. CHECK MIC SETTINGS IN THE OPERATING SYSTEM.";
					LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_NO_MIC", out text, defaultResult);
					this.screenText.Append(text);
					return;
				}
				defaultResult = "INPUT TEST: ";
				LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out text, defaultResult);
				this.screenText.Append(text);
				for (int i = 0; i < Mathf.FloorToInt(num * 50f); i++)
				{
					this.screenText.Append("|");
				}
			}
		}

		// Token: 0x0600689D RID: 26781 RVA: 0x0021D2D0 File Offset: 0x0021B4D0
		private void QueueScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			string defaultResult = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN", out text, defaultResult);
			this.screenText.Set(text.TrailingSpace());
			if (this.allowedInCompetitive)
			{
				defaultResult = "COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN.";
				LocalisationManager.TryGetKeyForCurrentLocale("COMPETITIVE_DESC", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				defaultResult = "PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_ALL_QUEUES", out text, defaultResult);
				this.screenText.Append(text);
			}
			else
			{
				defaultResult = "BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY.";
				LocalisationManager.TryGetKeyForCurrentLocale("BEAT_OBSTACLE_COURSE", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				defaultResult = "PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
				LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_DEFAULT_QUEUES", out text, defaultResult);
				this.screenText.Append(text);
			}
			defaultResult = "\n\nCURRENT QUEUE:";
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_QUEUE", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			string a = this.currentQueue;
			string key;
			if (!(a == "DEFAULT"))
			{
				if (a == "COMPETITIVE")
				{
					key = "COMPETITIVE_QUEUE";
					goto IL_137;
				}
				if (a == "MINIGAMES")
				{
					key = "MINIGAMES_QUEUE";
					goto IL_137;
				}
			}
			key = "DEFAULT_QUEUE";
			IL_137:
			defaultResult = this.currentQueue;
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
			this.screenText.Append(text);
		}

		// Token: 0x0600689E RID: 26782 RVA: 0x0021D434 File Offset: 0x0021B634
		private void TroopScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
			bool flag2 = this.IsValidTroopName(this.troopName);
			this.screenText.Set(string.Empty);
			string text = "";
			string defaultResult;
			if (flag)
			{
				defaultResult = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INTRO", out text, defaultResult);
				this.screenText.Set(text);
				if (!flag2)
				{
					defaultResult = " PRESS ENTER TO JOIN OR CREATE A TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INSTRUCTIONS", out text, defaultResult);
					this.screenText.Append(text);
				}
			}
			defaultResult = "\n\nCURRENT TROOP: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_TROOP", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			if (flag2)
			{
				this.screenText.Append(this.troopName ?? "");
				if (flag)
				{
					bool flag3 = this.currentTroopPopulation > -1;
					if (this.troopQueueActive)
					{
						defaultResult = "\n  -IN TROOP QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_IN_QUEUE", out text, defaultResult);
						this.screenText.Append(text);
						if (flag3)
						{
							defaultResult = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, defaultResult);
							this.screenText.Append(text.TrailingSpace());
							this.screenText.Append(Mathf.Max(1, this.currentTroopPopulation).ToString());
						}
						defaultResult = "\n\nPRESS OPTION 2 FOR DEFAULT QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DEFAULT_QUEUE", out text, defaultResult);
						this.screenText.Append(text);
					}
					else
					{
						defaultResult = "\n  -IN {currentQueue} QUEUE-";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_QUEUE", out text, defaultResult);
						string a = this.currentQueue;
						string key;
						if (!(a == "DEFAULT"))
						{
							if (a == "MINIGAMES")
							{
								key = "MINIGAMES_QUEUE";
								goto IL_209;
							}
							if (a == "COMPETITIVE")
							{
								key = "COMPETITIVE_QUEUE";
								goto IL_209;
							}
						}
						key = "DEFAULT_QUEUE";
						IL_209:
						defaultResult = this.currentQueue;
						string newValue;
						LocalisationManager.TryGetKeyForCurrentLocale(key, out newValue, defaultResult);
						text = text.Replace("{currentQueue}", newValue);
						this.screenText.Append(text);
						if (flag3)
						{
							defaultResult = "\n\nPLAYERS IN TROOP: ";
							LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out text, defaultResult);
							this.screenText.Append(text.TrailingSpace());
							this.screenText.Append(Mathf.Max(1, this.currentTroopPopulation).ToString());
						}
						defaultResult = "\n\nPRESS OPTION 1 FOR TROOP QUEUE.";
						LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_TROOP_QUEUE", out text, defaultResult);
						this.screenText.Append(text);
					}
					defaultResult = "\nPRESS OPTION 3 TO LEAVE YOUR TROOP.";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_LEAVE", out text, defaultResult);
					this.screenText.Append(text);
				}
			}
			else
			{
				defaultResult = "-NOT IN TROOP-";
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_NOT_IN_TROOP", out text, defaultResult);
				this.screenText.Append(text);
			}
			if (flag)
			{
				if (!flag2)
				{
					defaultResult = "\n\nTROOP TO JOIN: ";
					LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_JOIN_TROOP", out text, defaultResult);
					this.screenText.Append(text.TrailingSpace());
					this.screenText.Append(this.troopToJoin);
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.TroopScreen_KIdProhibited();
					return;
				}
				this.TroopScreen_Permission();
			}
		}

		// Token: 0x0600689F RID: 26783 RVA: 0x0021D794 File Offset: 0x0021B994
		private void TurnScreen()
		{
			string defaultResult = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING.";
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN", out text2, defaultResult);
			text += text2.TrailingSpace();
			defaultResult = "PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURNING_SPEED", out text2, defaultResult);
			text += text2;
			defaultResult = "\n CURRENT TURN TYPE: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_TYPE", out text2, defaultResult);
			text += text2;
			string key = "TURN_TYPE_NO_TURN";
			string turnType = GorillaSnapTurn.CachedSnapTurnRef.turnType;
			if (!(turnType == "SNAP"))
			{
				if (!(turnType == "SMOOTH"))
				{
					if (!(turnType == "NONE"))
					{
						Debug.LogError("[LOCALIZATION::GORILLA_COMPUTER::TURN] Could not match [" + GorillaSnapTurn.CachedSnapTurnRef.turnType + "] to any case. Defaulting to NO_TURN");
					}
					else
					{
						key = "TURN_TYPE_NO_TURN";
					}
				}
				else
				{
					key = "TURN_TYPE_SMOOTH_TURN";
				}
			}
			else
			{
				key = "TURN_TYPE_SNAP_TURN";
			}
			LocalisationManager.TryGetKeyForCurrentLocale(key, out text2, GorillaSnapTurn.CachedSnapTurnRef.turnType);
			text += text2;
			defaultResult = "\nCURRENT TURN SPEED: ";
			LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_SPEED", out text2, defaultResult);
			text += text2;
			text += GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
			this.screenText.Set(text);
		}

		// Token: 0x060068A0 RID: 26784 RVA: 0x0021D8D4 File Offset: 0x0021BAD4
		private void NameScreen()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
			{
				string defaultResult = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\n";
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN", out text, defaultResult);
				this.screenText.Set(text);
				defaultResult = "CURRENT NAME: ";
				LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_NAME", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(this.savedName);
				if (this.NametagsEnabled)
				{
					defaultResult = "NEW NAME: ";
					LocalisationManager.TryGetKeyForCurrentLocale("NEW_NAME", out text, defaultResult);
					this.screenText.Append(text.TrailingSpace());
					this.screenText.Append(this.currentName);
				}
				defaultResult = "PRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: ";
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_TOGGLE_NAMETAGS", out text, defaultResult);
				string key = this.NametagsEnabled ? "ON_KEY" : "OFF_KEY";
				this.screenText.Append(text.TrailingSpace());
				defaultResult = (this.NametagsEnabled ? "ON" : "OFF");
				LocalisationManager.TryGetKeyForCurrentLocale(key, out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				this.NameScreen_KIdProhibited();
				return;
			}
			this.NameScreen_Permission();
		}

		// Token: 0x060068A1 RID: 26785 RVA: 0x0021DA04 File Offset: 0x0021BC04
		private void StartupScreen()
		{
			string text = string.Empty;
			if (KIDManager.GetActiveAccountStatus() == AgeStatusType.DIGITALMINOR)
			{
				text = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
				string text2;
				if (LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_MANAGED", out text2, text))
				{
					text = text2;
				}
			}
			string empty = string.Empty;
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_INTRO", out text3, "GORILLA OS\n\n");
			this.screenText.Set(text3);
			this.screenText.Append(text);
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PLAYERS_ONLINE", out text3, "{playersOnline} PLAYERS ONLINE\n\n");
			this.screenText.Append(text3.Replace("{playersOnline}", HowManyMonke.ThisMany.ToString()));
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_USERS_BANNED", out text3, "{usersBanned} USERS BANNED YESTERDAY\n\n");
			this.screenText.Append(text3.Replace("{usersBanned}", this.usersBanned.ToString()));
			LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PRESS_KEY", out text3, "PRESS ANY KEY TO BEGIN");
			this.screenText.Append(text3);
		}

		// Token: 0x060068A2 RID: 26786 RVA: 0x0021DAEC File Offset: 0x0021BCEC
		private void ColourScreen()
		{
			string str;
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_SELECT_INTRO", out str, "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.");
			this.screenText.Set(str);
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_RED", out str, "RED");
			this.screenText.Append("\n\n");
			this.screenText.Append(str);
			this.screenText.Append(Mathf.FloorToInt(this.redValue * 9f).ToString() + ((this.colorCursorLine == 0) ? "<--" : ""));
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_GREEN", out str, "GREEN");
			this.screenText.Append("\n\n");
			this.screenText.Append(str);
			this.screenText.Append(Mathf.FloorToInt(this.greenValue * 9f).ToString() + ((this.colorCursorLine == 1) ? "<--" : ""));
			LocalisationManager.TryGetKeyForCurrentLocale("COLOR_BLUE", out str, "BLUE");
			this.screenText.Append("\n\n");
			this.screenText.Append(str);
			this.screenText.Append(Mathf.FloorToInt(this.blueValue * 9f).ToString() + ((this.colorCursorLine == 2) ? "<--" : ""));
		}

		// Token: 0x060068A3 RID: 26787 RVA: 0x0021DC5C File Offset: 0x0021BE5C
		private void RoomScreen()
		{
			if (this.limitOnlineScreens)
			{
				this.LimitedOnlineFunctionalityScreen();
				return;
			}
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
			Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer, null).Item2;
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer) && item;
			this.screenText.Set("");
			string text = "";
			string defaultResult;
			if (flag)
			{
				defaultResult = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE.";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_INTRO", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
			}
			defaultResult = "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_OPTION", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
				{
					defaultResult = "YOUR GROUP WILL TRAVEL WITH YOU.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_GROUP_TRAVEL", out text, defaultResult);
					this.screenText.Append(text.TrailingSpace());
				}
				else
				{
					defaultResult = "<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_PARTY_WARNING", out text, defaultResult);
					this.screenText.Append(text);
				}
			}
			defaultResult = "\n\nCURRENT ROOM:";
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TEXT_CURRENT_ROOM", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			if (NetworkSystem.Instance.InRoom)
			{
				this.screenText.Append(NetworkSystem.Instance.RoomName.TrailingSpace());
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
					string text2 = (activeGameMode != null) ? activeGameMode.GameModeNameRoomLabel() : null;
					if (!string.IsNullOrEmpty(text2))
					{
						this.screenText.Append(text2 ?? "");
					}
				}
				defaultResult = "\n\nPLAYERS IN ROOM:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_IN_ROOM", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(NetworkSystem.Instance.RoomPlayerCount.ToString());
			}
			else
			{
				defaultResult = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, defaultResult);
				this.screenText.Append(text);
				defaultResult = "\n\nPLAYERS ONLINE:";
				LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_ONLINE", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(HowManyMonke.ThisMany.ToString());
			}
			if (flag)
			{
				defaultResult = "\n\nROOM TO JOIN:";
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TO_JOIN", out text, defaultResult);
				this.screenText.Append(text.TrailingSpace());
				this.screenText.Append(this.roomToJoin);
				if (this.roomFull)
				{
					defaultResult = "\n\nROOM FULL. JOIN ROOM FAILED.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_FULL", out text, defaultResult);
					this.screenText.Append(text);
					return;
				}
				if (this.roomNotAllowed)
				{
					defaultResult = "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ROOM_JOIN_NOT_ALLOWED", out text, defaultResult);
					this.screenText.Append(text);
					return;
				}
			}
			else
			{
				if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
				{
					this.RoomScreen_KIdProhibited();
					return;
				}
				this.RoomScreen_Permission();
			}
		}

		// Token: 0x060068A4 RID: 26788 RVA: 0x0021DF64 File Offset: 0x0021C164
		private void RedemptionScreen()
		{
			string defaultResult = "TYPE REDEMPTION CODE AND PRESS ENTER";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_INTRO", out text, defaultResult);
			this.screenText.Set(text);
			defaultResult = "\n\nCODE: " + this.redemptionCode;
			LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_LABEL", out text, defaultResult);
			this.screenText.Append(text.TrailingSpace());
			this.screenText.Append(this.redemptionCode);
			switch (this.RedemptionStatus)
			{
			case GorillaComputer.RedemptionResult.Empty:
				break;
			case GorillaComputer.RedemptionResult.Invalid:
				defaultResult = "\n\nINVALID CODE";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_INVALID", out text, defaultResult);
				this.screenText.Append(text);
				return;
			case GorillaComputer.RedemptionResult.Checking:
				defaultResult = "\n\nVALIDATING...";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_VALIDATING", out text, defaultResult);
				this.screenText.Append(text);
				return;
			case GorillaComputer.RedemptionResult.AlreadyUsed:
				defaultResult = "\n\nCODE ALREADY CLAIMED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_ALREADY_USED", out text, defaultResult);
				this.screenText.Append(text);
				return;
			case GorillaComputer.RedemptionResult.TooEarly:
				defaultResult = "CODE IS NOT REDEEMABLE UNTIL";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_TOO_EARLY", out text, defaultResult);
				this.screenText.Append((this.RedemptionRestrictionTime != null) ? ("\n\n" + text + "\n" + this.RedemptionRestrictionTime.Value.ToLocalTime().ToString("f").ToUpper()) : ("\n\n" + text + "\n[MISSING]"));
				return;
			case GorillaComputer.RedemptionResult.TooLate:
				defaultResult = "CODE EXPIRED";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_TOO_LATE", out text, defaultResult);
				this.screenText.Append((this.RedemptionRestrictionTime != null) ? ("\n\n" + text + "\n" + this.RedemptionRestrictionTime.Value.ToLocalTime().ToString("f").ToUpper()) : ("\n\n" + text + "\n[MISSING]"));
				return;
			case GorillaComputer.RedemptionResult.Success:
				defaultResult = "\n\nSUCCESSFULLY CLAIMED!";
				LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_SUCCESS", out text, defaultResult);
				this.screenText.Append(text);
				break;
			default:
				return;
			}
		}

		// Token: 0x060068A5 RID: 26789 RVA: 0x0021E178 File Offset: 0x0021C378
		private void LimitedOnlineFunctionalityScreen()
		{
			string defaultResult = "NOT AVAILABLE IN RANKED PLAY";
			string str;
			LocalisationManager.TryGetKeyForCurrentLocale("LIMITED_ONLINE_FUNC", out str, defaultResult);
			this.screenText.Set(str);
		}

		// Token: 0x060068A6 RID: 26790 RVA: 0x0021E1A8 File Offset: 0x0021C3A8
		private void UpdateGameModeText()
		{
			string defaultResult = "CURRENT MODE";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_MODE", out text, defaultResult);
			this.currentGameModeText.Value = text;
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				defaultResult = "-NOT IN ROOM-";
				LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out text, defaultResult);
				WatchableStringSO watchableStringSO = this.currentGameModeText;
				watchableStringSO.Value += text;
				return;
			}
			WatchableStringSO watchableStringSO2 = this.currentGameModeText;
			watchableStringSO2.Value = watchableStringSO2.Value + "\n" + GorillaGameManager.instance.GameModeName();
		}

		// Token: 0x060068A7 RID: 26791 RVA: 0x0021E23F File Offset: 0x0021C43F
		private void UpdateFunctionScreen()
		{
			this.functionSelectText.Set(this.GetOrderListForScreen(this.currentState));
		}

		// Token: 0x060068A8 RID: 26792 RVA: 0x0021E258 File Offset: 0x0021C458
		private void CheckAutoBanListForRoomName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadRoomName(nameToCheck);
		}

		// Token: 0x060068A9 RID: 26793 RVA: 0x0021E267 File Offset: 0x0021C467
		private void CheckAutoBanListForPlayerName(string nameToCheck)
		{
			this.SwitchToLoadingState();
			this.CheckForBadPlayerName(nameToCheck);
		}

		// Token: 0x060068AA RID: 26794 RVA: 0x0021E276 File Offset: 0x0021C476
		private void CheckAutoBanListForTroopName(string nameToCheck)
		{
			if (this.IsValidTroopName(this.troopToJoin))
			{
				this.SwitchToLoadingState();
				this.CheckForBadTroopName(nameToCheck);
			}
		}

		// Token: 0x060068AB RID: 26795 RVA: 0x0021E293 File Offset: 0x0021C493
		private void CheckForBadRoomName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = true,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnRoomNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x060068AC RID: 26796 RVA: 0x0021E2D3 File Offset: 0x0021C4D3
		private void CheckForBadPlayerName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = false
			}, new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x060068AD RID: 26797 RVA: 0x0021E313 File Offset: 0x0021C513
		private void CheckForBadTroopName(string nameToCheck)
		{
			GorillaServer.Instance.CheckForBadName(new CheckForBadNameRequest
			{
				name = nameToCheck,
				forRoom = false,
				forTroop = true
			}, new Action<ExecuteFunctionResult>(this.OnTroopNameChecked), new Action<PlayFabError>(this.OnErrorNameCheck));
		}

		// Token: 0x060068AE RID: 26798 RVA: 0x0021E354 File Offset: 0x0021C554
		private void OnRoomNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
					{
						FriendshipGroupDetection.Instance.LeaveParty();
					}
					if (this.playerInVirtualStump)
					{
						CustomMapManager.UnloadMap(false);
					}
					this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
					break;
				case 1:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					this.SwitchToWarningState();
					break;
				case 2:
					this.roomToJoin = "";
					this.roomToJoin += (this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "");
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x060068AF RID: 26799 RVA: 0x0021E47C File Offset: 0x0021C67C
		private void OnPlayerNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					NetworkSystem.Instance.SetMyNickName(this.currentName);
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					break;
				case 1:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					this.SwitchToWarningState();
					break;
				case 2:
					NetworkSystem.Instance.SetMyNickName("gorilla");
					CustomMapsTerminal.RequestDriverNickNameRefresh();
					this.currentName = "gorilla";
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			this.SetLocalNameTagText(this.currentName);
			this.savedName = this.currentName;
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x060068B0 RID: 26800 RVA: 0x0021E5B8 File Offset: 0x0021C7B8
		private void OnTroopNameChecked(ExecuteFunctionResult result)
		{
			object obj;
			if (((JsonObject)result.FunctionResult).TryGetValue("result", out obj))
			{
				switch (int.Parse(obj.ToString()))
				{
				case 0:
					this.JoinTroop(this.troopToJoin);
					break;
				case 1:
					this.troopToJoin = string.Empty;
					this.SwitchToWarningState();
					break;
				case 2:
					this.troopToJoin = string.Empty;
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
					break;
				}
			}
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
		}

		// Token: 0x060068B1 RID: 26801 RVA: 0x0021E63F File Offset: 0x0021C83F
		private void OnErrorNameCheck(PlayFabError error)
		{
			if (this.currentState == GorillaComputer.ComputerState.Loading)
			{
				this.PopState();
			}
			GorillaComputer.OnErrorShared(error);
		}

		// Token: 0x060068B2 RID: 26802 RVA: 0x0021E658 File Offset: 0x0021C858
		public bool CheckAutoBanListForName(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			foreach (string value in this.anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(value) >= 0)
				{
					return false;
				}
			}
			foreach (string value2 in this.anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(value2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] array = this.exactOneWeek;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060068B3 RID: 26803 RVA: 0x0021E718 File Offset: 0x0021C918
		public void UpdateColor(float red, float green, float blue)
		{
			this.redValue = Mathf.Clamp(red, 0f, 1f);
			this.greenValue = Mathf.Clamp(green, 0f, 1f);
			this.blueValue = Mathf.Clamp(blue, 0f, 1f);
		}

		// Token: 0x060068B4 RID: 26804 RVA: 0x0021E767 File Offset: 0x0021C967
		public void UpdateFailureText(string failMessage)
		{
			GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.EnableFailedState(failMessage);
			this.functionSelectText.EnableFailedState(failMessage);
		}

		// Token: 0x060068B5 RID: 26805 RVA: 0x0021E798 File Offset: 0x0021C998
		private void RestoreFromFailureState()
		{
			GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
			PhotonNetworkController.Instance.UpdateTriggerScreens();
			this.screenText.DisableFailedState();
			this.functionSelectText.DisableFailedState();
		}

		// Token: 0x060068B6 RID: 26806 RVA: 0x0021E7C6 File Offset: 0x0021C9C6
		public void GeneralFailureMessage(string failMessage)
		{
			this.isConnectedToMaster = false;
			NetworkSystem.Instance.SetWrongVersion();
			this.UpdateFailureText(failMessage);
			this.UpdateScreen();
		}

		// Token: 0x060068B7 RID: 26807 RVA: 0x0021E7E8 File Offset: 0x0021C9E8
		private static void OnErrorShared(PlayFabError error)
		{
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			else if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
			if (error.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						GorillaComputer.instance.GeneralFailureMessage(string.Concat(new string[]
						{
							"YOUR ACCOUNT ",
							PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
							" HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: ",
							keyValuePair.Key,
							"\nHOURS LEFT: ",
							((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()
						}));
						return;
					}
					GorillaComputer.instance.GeneralFailureMessage("YOUR ACCOUNT " + PlayFabAuthenticator.instance.GetPlayFabPlayerId() + " HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (error.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value[0] != "Indefinite")
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						}
						else
						{
							GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
						}
					}
				}
			}
		}

		// Token: 0x060068B8 RID: 26808 RVA: 0x0021EA40 File Offset: 0x0021CC40
		private void DecreaseState()
		{
			this.currentStateIndex--;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex--;
			}
			if (this.currentStateIndex < 0)
			{
				this.currentStateIndex = this.FunctionsCount - 1;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x060068B9 RID: 26809 RVA: 0x0021EAA4 File Offset: 0x0021CCA4
		private void IncreaseState()
		{
			this.currentStateIndex++;
			if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
			{
				this.currentStateIndex++;
			}
			if (this.currentStateIndex >= this.FunctionsCount)
			{
				this.currentStateIndex = 0;
			}
			this.SwitchState(this.GetState(this.currentStateIndex), true);
		}

		// Token: 0x060068BA RID: 26810 RVA: 0x0021EB08 File Offset: 0x0021CD08
		public GorillaComputer.ComputerState GetState(int index)
		{
			GorillaComputer.ComputerState state;
			try
			{
				state = this._activeOrderList[index].State;
			}
			catch
			{
				state = this._activeOrderList[0].State;
			}
			return state;
		}

		// Token: 0x060068BB RID: 26811 RVA: 0x0021EB50 File Offset: 0x0021CD50
		public int GetStateIndex(GorillaComputer.ComputerState state)
		{
			return this._activeOrderList.FindIndex((GorillaComputer.StateOrderItem s) => s.State == state);
		}

		// Token: 0x060068BC RID: 26812 RVA: 0x0021EB84 File Offset: 0x0021CD84
		public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int stateIndex = this.GetStateIndex(currentState);
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				stringBuilder.Append(this.FunctionNames[i]);
				if (i == stateIndex)
				{
					stringBuilder.Append(this.Pointer);
				}
				if (i < this.FunctionsCount - 1)
				{
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060068BD RID: 26813 RVA: 0x0021EBF1 File Offset: 0x0021CDF1
		private void GetCurrentTime()
		{
			this.tryGetTimeAgain = true;
			PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure), null, null);
		}

		// Token: 0x060068BE RID: 26814 RVA: 0x0021EC20 File Offset: 0x0021CE20
		private void OnGetTimeSuccess(GetTimeResult result)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = result.Time - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated == null)
			{
				return;
			}
			onServerTimeUpdated();
		}

		// Token: 0x060068BF RID: 26815 RVA: 0x0021EC88 File Offset: 0x0021CE88
		private void OnGetTimeFailure(PlayFabError error)
		{
			this.startupMillis = (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double)(Time.realtimeSinceStartup * 1000f));
			this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double)Time.realtimeSinceStartup);
			Action onServerTimeUpdated = this.OnServerTimeUpdated;
			if (onServerTimeUpdated != null)
			{
				onServerTimeUpdated();
			}
			if (error.Error == PlayFabErrorCode.NotAuthenticated)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				return;
			}
			if (error.Error == PlayFabErrorCode.AccountBanned)
			{
				GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
			}
		}

		// Token: 0x060068C0 RID: 26816 RVA: 0x0021ED1B File Offset: 0x0021CF1B
		private void PlayerCountChangedCallback(NetPlayer player)
		{
			this.UpdateScreen();
		}

		// Token: 0x060068C1 RID: 26817 RVA: 0x0021ED23 File Offset: 0x0021CF23
		private static void OnFirstJoinedRoom_IncrementSessionCount()
		{
			RoomSystem.JoinedRoomEvent -= new Action(GorillaComputer.OnFirstJoinedRoom_IncrementSessionCount);
			GorillaComputer.sessionCount++;
			PlayerPrefs.SetInt("sessionCount", GorillaComputer.sessionCount);
			PlayerPrefs.Save();
		}

		// Token: 0x060068C2 RID: 26818 RVA: 0x0021ED60 File Offset: 0x0021CF60
		public void SetNameBySafety(bool isSafety)
		{
			if (!isSafety)
			{
				return;
			}
			PlayerPrefs.SetString("playerNameBackup", this.currentName);
			this.currentName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			this.savedName = this.currentName;
			NetworkSystem.Instance.SetMyNickName(this.currentName);
			this.SetLocalNameTagText(this.currentName);
			PlayerPrefs.SetString("playerName", this.currentName);
			PlayerPrefs.Save();
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
		}

		// Token: 0x060068C3 RID: 26819 RVA: 0x0021EE3A File Offset: 0x0021D03A
		public void SetLocalNameTagText(string newName)
		{
			VRRig.LocalRig.SetNameTagText(newName);
		}

		// Token: 0x060068C4 RID: 26820 RVA: 0x0021EE48 File Offset: 0x0021D048
		public void SetComputerSettingsBySafety(bool isSafety, GorillaComputer.ComputerState[] toFilterOut, bool shouldHide)
		{
			this._activeOrderList = this.OrderList;
			if (!isSafety)
			{
				this._activeOrderList = this.OrderList;
				if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
				{
					for (int i = 0; i < toFilterOut.Length; i++)
					{
						if (this._filteredStates.Contains(toFilterOut[i]))
						{
							this._filteredStates.Remove(toFilterOut[i]);
						}
					}
				}
			}
			else if (shouldHide)
			{
				for (int j = 0; j < toFilterOut.Length; j++)
				{
					if (!this._filteredStates.Contains(toFilterOut[j]))
					{
						this._filteredStates.Add(toFilterOut[j]);
					}
				}
			}
			if (this._filteredStates.Count > 0)
			{
				int k = 0;
				int num = this._activeOrderList.Count;
				while (k < num)
				{
					if (this._filteredStates.Contains(this._activeOrderList[k].State))
					{
						this._activeOrderList.RemoveAt(k);
						k--;
						num--;
					}
					k++;
				}
			}
			this.FunctionsCount = this._activeOrderList.Count;
			this.FunctionNames.Clear();
			this._activeOrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int l = 0; l < this.FunctionsCount; l++)
			{
				int num2 = this.highestCharacterCount - this.FunctionNames[l].Length;
				for (int m = 0; m < num2; m++)
				{
					List<string> functionNames = this.FunctionNames;
					int index = l;
					functionNames[index] += " ";
				}
			}
			this.UpdateScreen();
		}

		// Token: 0x060068C5 RID: 26821 RVA: 0x0021EFDA File Offset: 0x0021D1DA
		public void KID_SetVoiceChatSettingOnStart(bool voiceChatEnabled, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			this.SetVoice(voiceChatEnabled, !hasOptedInPreviously);
		}

		// Token: 0x060068C6 RID: 26822 RVA: 0x0021EFEC File Offset: 0x0021D1EC
		private void SetVoice(bool setting, bool saveSetting = true)
		{
			this.voiceChatOn = (setting ? "TRUE" : "FALSE");
			if (setting && !KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, null).Item2)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
				KIDManager.SendOptInPermissions();
			}
			if (!saveSetting)
			{
				return;
			}
			PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
			PlayerPrefs.Save();
		}

		// Token: 0x060068C7 RID: 26823 RVA: 0x0021F045 File Offset: 0x0021D245
		public bool CheckVoiceChatEnabled()
		{
			return this.voiceChatOn == "TRUE";
		}

		// Token: 0x060068C8 RID: 26824 RVA: 0x0021F058 File Offset: 0x0021D258
		private void SetVoiceChatBySafety(bool voiceChatEnabled, Permission.ManagedByEnum managedBy)
		{
			bool isSafety = !voiceChatEnabled;
			this.SetComputerSettingsBySafety(isSafety, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Voice,
				GorillaComputer.ComputerState.AutoMute,
				GorillaComputer.ComputerState.Mic
			}, false);
			string value = PlayerPrefs.GetString("voiceChatOn", "");
			if (KIDManager.KidEnabledAndReady)
			{
				Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
				if (permissionDataByFeature != null)
				{
					ValueTuple<bool, bool> valueTuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
					if (valueTuple.Item1 && !valueTuple.Item2)
					{
						value = "FALSE";
					}
				}
				else
				{
					Debug.LogErrorFormat("[KID] Could not find permission data for [" + EKIDFeatures.Voice_Chat.ToStandardisedString() + "]", Array.Empty<object>());
				}
			}
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (string.IsNullOrEmpty(value))
				{
					this.voiceChatOn = (voiceChatEnabled ? "TRUE" : "FALSE");
				}
				else
				{
					this.voiceChatOn = value;
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled)
				{
					if (string.IsNullOrEmpty(value))
					{
						this.voiceChatOn = "TRUE";
					}
					else
					{
						this.voiceChatOn = value;
					}
				}
				else
				{
					this.voiceChatOn = "FALSE";
				}
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.voiceChatOn = "FALSE";
				break;
			}
			RigContainer.RefreshAllRigVoices();
			Debug.Log("[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [" + voiceChatEnabled.ToString() + "]");
		}

		// Token: 0x060068C9 RID: 26825 RVA: 0x0021F184 File Offset: 0x0021D384
		public void SetNametagSetting(bool setting, Permission.ManagedByEnum managedBy, bool hasOptedInPreviously)
		{
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				return;
			}
			if (managedBy == Permission.ManagedByEnum.GUARDIAN)
			{
				int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
				setting = (setting && @int == 1);
				this.UpdateNametagSetting(setting, false);
				return;
			}
			setting = (PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1);
			this.UpdateNametagSetting(setting, !hasOptedInPreviously && setting);
		}

		// Token: 0x060068CA RID: 26826 RVA: 0x0021F1E0 File Offset: 0x0021D3E0
		public static void RegisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Combine(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x060068CB RID: 26827 RVA: 0x0021F1F7 File Offset: 0x0021D3F7
		public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
		{
			GorillaComputer.onNametagSettingChangedAction = (Action<bool>)Delegate.Remove(GorillaComputer.onNametagSettingChangedAction, callback);
		}

		// Token: 0x060068CC RID: 26828 RVA: 0x0021F210 File Offset: 0x0021D410
		private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
		{
			if (newSettingValue)
			{
				KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
			}
			this.NametagsEnabled = newSettingValue;
			NetworkSystem.Instance.SetMyNickName(this.NametagsEnabled ? this.savedName : NetworkSystem.Instance.GetMyDefaultName());
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
				{
					this.redValue,
					this.greenValue,
					this.blueValue
				});
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action != null)
			{
				action(this.NametagsEnabled);
			}
			if (!saveSetting)
			{
				return;
			}
			int value = this.NametagsEnabled ? 1 : 0;
			PlayerPrefs.SetInt(this.NameTagPlayerPref, value);
			PlayerPrefs.Save();
		}

		// Token: 0x060068CD RID: 26829 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
		{
		}

		// Token: 0x060068CE RID: 26830 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		// Token: 0x060068CF RID: 26831 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060068D0 RID: 26832 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnJoinedRoom()
		{
		}

		// Token: 0x060068D1 RID: 26833 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060068D2 RID: 26834 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnLeftRoom()
		{
		}

		// Token: 0x060068D3 RID: 26835 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IMatchmakingCallbacks.OnPreLeavingRoom()
		{
		}

		// Token: 0x060068D4 RID: 26836 RVA: 0x0021F2DB File Offset: 0x0021D4DB
		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
			if (returnCode == 32765)
			{
				this.roomFull = true;
			}
		}

		// Token: 0x060068D5 RID: 26837 RVA: 0x0021F2EC File Offset: 0x0021D4EC
		public void SetInVirtualStump(bool inVirtualStump)
		{
			this.playerInVirtualStump = inVirtualStump;
			this.roomToJoin = (this.playerInVirtualStump ? (this.virtualStumpRoomPrepend + this.roomToJoin) : this.roomToJoin.RemoveAll(this.virtualStumpRoomPrepend, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x060068D6 RID: 26838 RVA: 0x0021F328 File Offset: 0x0021D528
		public bool IsPlayerInVirtualStump()
		{
			return this.playerInVirtualStump;
		}

		// Token: 0x060068D7 RID: 26839 RVA: 0x0021F330 File Offset: 0x0021D530
		public void SetLimitOnlineScreens(bool isLimited)
		{
			this.limitOnlineScreens = isLimited;
			this.UpdateScreen();
		}

		// Token: 0x060068D8 RID: 26840 RVA: 0x0021F33F File Offset: 0x0021D53F
		private void InitializeKIdState()
		{
			KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
		}

		// Token: 0x060068D9 RID: 26841 RVA: 0x0021F352 File Offset: 0x0021D552
		private void UpdateKidState()
		{
			this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
		}

		// Token: 0x060068DA RID: 26842 RVA: 0x0021F35B File Offset: 0x0021D55B
		private void RequestUpdatedPermissions()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				return;
			}
			if (Time.realtimeSinceStartup < this._nextUpdateAttemptTime)
			{
				return;
			}
			this._waitingForUpdatedSession = true;
			this.UpdateSession();
		}

		// Token: 0x060068DB RID: 26843 RVA: 0x0021F38C File Offset: 0x0021D58C
		private void UpdateSession()
		{
			GorillaComputer.<UpdateSession>d__487 <UpdateSession>d__;
			<UpdateSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdateSession>d__.<>4__this = this;
			<UpdateSession>d__.<>1__state = -1;
			<UpdateSession>d__.<>t__builder.Start<GorillaComputer.<UpdateSession>d__487>(ref <UpdateSession>d__);
		}

		// Token: 0x060068DC RID: 26844 RVA: 0x0021F3C3 File Offset: 0x0021D5C3
		private void OnSessionUpdate_GorillaComputer()
		{
			this.UpdateKidState();
			this.UpdateScreen();
		}

		// Token: 0x060068DD RID: 26845 RVA: 0x0021F3D1 File Offset: 0x0021D5D1
		private void ProcessScreen_SetupKID()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				Debug.LogError("[KID] Unable to start k-ID Flow. Kid is disabled");
				return;
			}
		}

		// Token: 0x060068DE RID: 26846 RVA: 0x0021F3E8 File Offset: 0x0021D5E8
		private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
		{
			string defaultResult = "PARENT/GUARDIAN PERMISSION REQUIRED TO ";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PERMISSION_NEEDED", out text, defaultResult);
			this.screenText.Append(text);
			this.screenText.Append(featureDescription + "!");
			if (this._waitingForUpdatedSession)
			{
				defaultResult = "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_WAITING_PERMISSION", out text, defaultResult);
				this.screenText.Append(text);
				return true;
			}
			if (Time.realtimeSinceStartup >= this._nextUpdateAttemptTime)
			{
				defaultResult = "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_REFRESH_PERMISSIONS", out text, defaultResult);
				this.screenText.Append(text);
			}
			else
			{
				defaultResult = "CHECK AGAIN IN {time} SECONDS!";
				LocalisationManager.TryGetKeyForCurrentLocale("KID_CHECK_AGAIN_COOLDOWN", out text, defaultResult);
				text = text.Replace("{time}", ((int)(this._nextUpdateAttemptTime - Time.realtimeSinceStartup)).ToString());
				this.screenText.Append(text);
			}
			return false;
		}

		// Token: 0x060068DF RID: 26847 RVA: 0x0021F4C8 File Offset: 0x0021D6C8
		private void ProhibitedMessage(string verb)
		{
			"\n\nYOU ARE NOT ALLOWED TO " + verb + " IN YOUR JURISDICTION.";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("KID_PROHIBITED_MESSAGE", out text, "SET CUSTOM NICKNAMES");
			text = text.Replace("{verb}", verb);
			this.screenText.Append(text);
		}

		// Token: 0x060068E0 RID: 26848 RVA: 0x0021F514 File Offset: 0x0021D714
		private void RoomScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string defaultResult = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
				string str;
				LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_DISABLED", out str, defaultResult);
				this.screenText.Set(str);
				return;
			}
			this.screenText.Set("");
			string defaultResult2 = "CREATE OR JOIN PRIVATE ROOMS";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out featureDescription, defaultResult2);
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x060068E1 RID: 26849 RVA: 0x0021F57C File Offset: 0x0021D77C
		private void RoomScreen_KIdProhibited()
		{
			string defaultResult = "CREATE OR JOIN PRIVATE ROOMS";
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out verb, defaultResult);
			this.ProhibitedMessage(verb);
		}

		// Token: 0x060068E2 RID: 26850 RVA: 0x0021F5A4 File Offset: 0x0021D7A4
		private void VoiceScreen_Permission()
		{
			string defaultResult = "VOICE TYPE: \"MONKE\"\n\n";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_CURRENT_VOICE", out text, defaultResult);
			this.screenText.Set(text);
			if (!KIDManager.KidEnabled)
			{
				defaultResult = "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
				LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_DISABLED", out text, defaultResult);
				this.screenText.Append(text);
				return;
			}
			defaultResult = "ENABLE HUMAN VOICE CHAT";
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out text, defaultResult);
			this.GuardianConsentMessage("OPTION 3", text);
		}

		// Token: 0x060068E3 RID: 26851 RVA: 0x0021F61C File Offset: 0x0021D81C
		private void VoiceScreen_KIdProhibited()
		{
			string defaultResult = "USE THE VOICE CHAT";
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_PROHIBITED_VERB", out verb, defaultResult);
			this.ProhibitedMessage(verb);
		}

		// Token: 0x060068E4 RID: 26852 RVA: 0x0021F644 File Offset: 0x0021D844
		private void MicScreen_Permission()
		{
			this.screenText.Set("");
			string defaultResult = "ENABLE HUMAN VOICE CHAT";
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out featureDescription, defaultResult);
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x060068E5 RID: 26853 RVA: 0x0021F682 File Offset: 0x0021D882
		private void MicScreen_KIdProhibited()
		{
			this.VoiceScreen_KIdProhibited();
		}

		// Token: 0x060068E6 RID: 26854 RVA: 0x0021F68C File Offset: 0x0021D88C
		private void NameScreen_Permission()
		{
			if (!KIDManager.KidEnabled)
			{
				string defaultResult = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
				string str;
				LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_DISABLED", out str, defaultResult);
				this.screenText.Append(str);
				return;
			}
			this.screenText.Set("");
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out featureDescription, "SET CUSTOM NICKNAMES");
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x060068E7 RID: 26855 RVA: 0x0021F6F0 File Offset: 0x0021D8F0
		private void NameScreen_KIdProhibited()
		{
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out verb, "SET CUSTOM NICKNAMES");
			this.ProhibitedMessage(verb);
		}

		// Token: 0x060068E8 RID: 26856 RVA: 0x0021F718 File Offset: 0x0021D918
		private void OnKIDSessionUpdated_CustomNicknames(bool showCustomNames, Permission.ManagedByEnum managedBy)
		{
			bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
			this.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
			{
				GorillaComputer.ComputerState.Name
			}, false);
			int @int = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
			bool flag2 = @int > 0;
			switch (managedBy)
			{
			case Permission.ManagedByEnum.PLAYER:
				if (showCustomNames)
				{
					this.NametagsEnabled = (@int == -1 || flag2);
				}
				else
				{
					this.NametagsEnabled = (@int != -1 && flag2);
				}
				break;
			case Permission.ManagedByEnum.GUARDIAN:
				this.NametagsEnabled = (showCustomNames && (flag2 || @int == -1));
				break;
			case Permission.ManagedByEnum.PROHIBITED:
				this.NametagsEnabled = false;
				break;
			}
			if (this.NametagsEnabled)
			{
				NetworkSystem.Instance.SetMyNickName(this.savedName);
			}
			Action<bool> action = GorillaComputer.onNametagSettingChangedAction;
			if (action == null)
			{
				return;
			}
			action(this.NametagsEnabled);
		}

		// Token: 0x060068E9 RID: 26857 RVA: 0x0021F7E4 File Offset: 0x0021D9E4
		private void TroopScreen_Permission()
		{
			this.screenText.Set("");
			if (!KIDManager.KidEnabled)
			{
				string str;
				LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DISABLED", out str, "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW");
				this.screenText.Append(str);
				return;
			}
			string featureDescription;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_DESC", out featureDescription, "JOIN TROOPS");
			this.GuardianConsentMessage("OPTION 3", featureDescription);
		}

		// Token: 0x060068EA RID: 26858 RVA: 0x0021F848 File Offset: 0x0021DA48
		private void TroopScreen_KIdProhibited()
		{
			string verb;
			LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_PROHIBITED_VERB", out verb, "CREATE OR JOIN TROOPS");
			this.ProhibitedMessage(verb);
		}

		// Token: 0x060068EB RID: 26859 RVA: 0x0021F86E File Offset: 0x0021DA6E
		private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.option1 && this._currentScreentState == GorillaComputer.EKidScreenState.Ready)
			{
				this.RequestUpdatedPermissions();
			}
		}

		// Token: 0x060068EC RID: 26860 RVA: 0x0021F883 File Offset: 0x0021DA83
		private void KIdScreen()
		{
			if (!KIDManager.KidEnabledAndReady)
			{
				return;
			}
			if (!KIDManager.HasSession)
			{
				this.GuardianConsentMessage("OPTION 3", "");
				return;
			}
			this.KIdScreen_DisplayPermissions();
		}

		// Token: 0x060068ED RID: 26861 RVA: 0x0021F8AC File Offset: 0x0021DAAC
		private void KIdScreen_DisplayPermissions()
		{
			AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
			string str = (!KIDManager.InitialisationSuccessful) ? "NOT READY" : activeAccountStatus.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("k-ID Account Status:\t" + str);
			if (activeAccountStatus == (AgeStatusType)0)
			{
				stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
				this.screenText.Set(stringBuilder.ToString());
				return;
			}
			if (this._waitingForUpdatedSession)
			{
				stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
				this.screenText.Set(stringBuilder.ToString());
				return;
			}
			stringBuilder.AppendLine("\nPermissions:");
			List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
			int count = allPermissionsData.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				if (this._interestedPermissionNames.Contains(allPermissionsData[i].Name))
				{
					string text = allPermissionsData[i].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">";
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						num.ToString(),
						"] ",
						text,
						allPermissionsData[i].Name,
						"</color>"
					}));
					num++;
				}
			}
			stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
			this.screenText.Set(stringBuilder.ToString());
		}

		// Token: 0x060068EE RID: 26862 RVA: 0x0021FA11 File Offset: 0x0021DC11
		private string GetLocalisedLanguageScreen()
		{
			return this.GetLanguageScreenLocalisation();
		}

		// Token: 0x060068EF RID: 26863 RVA: 0x0021FA1C File Offset: 0x0021DC1C
		private void GetLangaugesList(ref string langStr)
		{
			this._languagesDisplaySB.Clear();
			int maxLength = 12;
			int num = 3;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, Locale> keyValuePair in LocalisationManager.GetAllBindings())
			{
				num2++;
				string text = LocalisationManager.LocaleToFriendlyString(keyValuePair.Value, false).ToUpper();
				string value = string.Format("{0}) {1}", keyValuePair.Key, text);
				stringBuilder.Append(value);
				int remainingChars = this.GetRemainingChars(text, maxLength);
				stringBuilder.Append(' ', remainingChars);
				if (num2 >= num)
				{
					this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
					stringBuilder.Clear();
					num2 = 0;
				}
			}
			this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
			langStr = langStr + this._languagesDisplaySB.ToString() + "\n";
		}

		// Token: 0x060068F0 RID: 26864 RVA: 0x0021FB20 File Offset: 0x0021DD20
		private int GetRemainingChars(string value, int maxLength)
		{
			int result;
			if (value == "日本語")
			{
				result = ((LocalisationManager.CurrentLanguage.Identifier.Code == "ja") ? 7 : 7);
			}
			else
			{
				result = Mathf.Clamp(maxLength - value.Length, 0, maxLength);
			}
			return result;
		}

		// Token: 0x060068F1 RID: 26865 RVA: 0x0021FB74 File Offset: 0x0021DD74
		private string GetLanguageScreenLocalisation()
		{
			string text = "";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_TITLE", out text2, "CHOOSE YOUR LANGUAGE\n");
			text += text2;
			this.GetLangaugesList(ref text);
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_INSTRUCTIONS", out text2, "PRESS NUMBER KEYS TO CHOOSE A LANGUAGE\n");
			text += text2;
			LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_CURRENT_LANGUAGE", out text2, "CURRENT LANGUAGE: ");
			text = text + text2.TrailingSpace() + LocalisationManager.LocaleToFriendlyString(null, false).ToUpper();
			return text;
		}

		// Token: 0x060068F2 RID: 26866 RVA: 0x0021FBEF File Offset: 0x0021DDEF
		private void InitialiseLanguageScreen()
		{
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}

		// Token: 0x060068F3 RID: 26867 RVA: 0x0021FC0D File Offset: 0x0021DE0D
		private void LanguageScreen()
		{
			this.screenText.Set(this.GetLocalisedLanguageScreen());
		}

		// Token: 0x060068F4 RID: 26868 RVA: 0x0021FC20 File Offset: 0x0021DE20
		private void ProcessLanguageState(GorillaKeyboardBindings buttonPressed)
		{
			int binding;
			if (!buttonPressed.FromNumberBindingToInt(out binding))
			{
				return;
			}
			Locale locale;
			if (!LocalisationManager.TryGetLocaleBinding(binding, out locale))
			{
				return;
			}
			LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, true);
			this.RefreshFunctionNames();
		}

		// Token: 0x060068F5 RID: 26869 RVA: 0x0021FC64 File Offset: 0x0021DE64
		private void OnLanguageChanged()
		{
			if (this._previousLocalisationSetting == LocalisationManager.CurrentLanguage)
			{
				Debug.Log("[LOCALISATION::GORILLA_COMPUTER] Language changed, but no different to previous setting [" + this._previousLocalisationSetting.ToString() + "]");
				return;
			}
			this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
			this.RefreshFunctionNames();
		}

		// Token: 0x060068F6 RID: 26870 RVA: 0x0021FCB4 File Offset: 0x0021DEB4
		private void RefreshFunctionNames()
		{
			this.FunctionNames.Clear();
			this.FunctionsCount = this.OrderList.Count;
			this.highestCharacterCount = int.MinValue;
			this.OrderList.ForEach(delegate(GorillaComputer.StateOrderItem s)
			{
				string name = s.GetName();
				if (name.Length > this.highestCharacterCount)
				{
					this.highestCharacterCount = name.Length;
				}
				this.FunctionNames.Add(name);
			});
			for (int i = 0; i < this.FunctionsCount; i++)
			{
				int num = this.highestCharacterCount - this.FunctionNames[i].Length;
				for (int j = 0; j < num; j++)
				{
					List<string> functionNames = this.FunctionNames;
					int index = i;
					functionNames[index] += " ";
				}
			}
		}

		// Token: 0x040077AD RID: 30637
		private const string VERSION_MISMATCH_KEY = "VERSION_MISMATCH";

		// Token: 0x040077AE RID: 30638
		private const string CONNECTION_ISSUE_KEY = "CONNECTION_ISSUE";

		// Token: 0x040077AF RID: 30639
		private const string NO_CONNECTION_KEY = "NO_CONNECTION";

		// Token: 0x040077B0 RID: 30640
		private const string STARTUP_INTRO_KEY = "STARTUP_INTRO";

		// Token: 0x040077B1 RID: 30641
		private const string STARTUP_PLAYERS_ONLINE_KEY = "STARTUP_PLAYERS_ONLINE";

		// Token: 0x040077B2 RID: 30642
		private const string STARTUP_USERS_BANNED_KEY = "STARTUP_USERS_BANNED";

		// Token: 0x040077B3 RID: 30643
		private const string STARTUP_PRESS_KEY_KEY = "STARTUP_PRESS_KEY";

		// Token: 0x040077B4 RID: 30644
		private const string STARTUP_PRESS_KEY_SHORT_KEY = "STARTUP_PRESS_KEY_SHORT";

		// Token: 0x040077B5 RID: 30645
		private const string STARTUP_MANAGED_KEY = "STARTUP_MANAGED";

		// Token: 0x040077B6 RID: 30646
		private const string COLOR_SELECT_INTRO_KEY = "COLOR_SELECT_INTRO";

		// Token: 0x040077B7 RID: 30647
		private const string CURRENT_SELECTED_LANGUAGE_KEY = "CURRENT_SELECTED_LANGUAGE";

		// Token: 0x040077B8 RID: 30648
		private const string CHANGE_TO_KEY = "CHANGE_TO";

		// Token: 0x040077B9 RID: 30649
		private const string CONFIRM_LANGUAGE_KEY = "CONFIRM_LANGUAGE";

		// Token: 0x040077BA RID: 30650
		private const string COLOR_RED_KEY = "COLOR_RED";

		// Token: 0x040077BB RID: 30651
		private const string COLOR_GREEN_KEY = "COLOR_GREEN";

		// Token: 0x040077BC RID: 30652
		private const string COLOR_BLUE_KEY = "COLOR_BLUE";

		// Token: 0x040077BD RID: 30653
		private const string ROOM_INTRO_KEY = "ROOM_INTRO";

		// Token: 0x040077BE RID: 30654
		private const string ROOM_OPTION_KEY = "ROOM_OPTION";

		// Token: 0x040077BF RID: 30655
		private const string ROOM_TEXT_CURRENT_ROOM_KEY = "ROOM_TEXT_CURRENT_ROOM";

		// Token: 0x040077C0 RID: 30656
		private const string PLAYERS_IN_ROOM_KEY = "PLAYERS_IN_ROOM";

		// Token: 0x040077C1 RID: 30657
		private const string NOT_IN_ROOM_KEY = "NOT_IN_ROOM";

		// Token: 0x040077C2 RID: 30658
		private const string PLAYERS_ONLINE_KEY = "PLAYERS_ONLINE";

		// Token: 0x040077C3 RID: 30659
		private const string ROOM_TO_JOIN_KEY = "ROOM_TO_JOIN";

		// Token: 0x040077C4 RID: 30660
		private const string ROOM_FULL_KEY = "ROOM_FULL";

		// Token: 0x040077C5 RID: 30661
		private const string ROOM_JOIN_NOT_ALLOWED_KEY = "ROOM_JOIN_NOT_ALLOWED";

		// Token: 0x040077C6 RID: 30662
		private const string LANGUAGE_KEY = "LANGUAGE";

		// Token: 0x040077C7 RID: 30663
		private const string NAME_SCREEN_KEY = "NAME_SCREEN";

		// Token: 0x040077C8 RID: 30664
		private const string CURRENT_NAME_KEY = "CURRENT_NAME";

		// Token: 0x040077C9 RID: 30665
		private const string NEW_NAME_KEY = "NEW_NAME";

		// Token: 0x040077CA RID: 30666
		private const string TURN_SCREEN_KEY = "TURN_SCREEN";

		// Token: 0x040077CB RID: 30667
		private const string TURN_SCREEN_TURNING_SPEED_KEY = "TURN_SCREEN_TURNING_SPEED";

		// Token: 0x040077CC RID: 30668
		private const string TURN_SCREEN_TURN_TYPE_KEY = "TURN_SCREEN_TURN_TYPE";

		// Token: 0x040077CD RID: 30669
		private const string TURN_SCREEN_TURN_SPEED_KEY = "TURN_SCREEN_TURN_SPEED";

		// Token: 0x040077CE RID: 30670
		private const string TURN_TYPE_SNAP_TURN_KEY = "TURN_TYPE_SNAP_TURN";

		// Token: 0x040077CF RID: 30671
		private const string TURN_TYPE_SMOOTH_TURN_KEY = "TURN_TYPE_SMOOTH_TURN";

		// Token: 0x040077D0 RID: 30672
		private const string TURN_TYPE_NO_TURN_KEY = "TURN_TYPE_NO_TURN";

		// Token: 0x040077D1 RID: 30673
		private const string QUEUE_SCREEN_KEY = "QUEUE_SCREEN";

		// Token: 0x040077D2 RID: 30674
		private const string BEAT_OBSTACLE_COURSE_KEY = "BEAT_OBSTACLE_COURSE";

		// Token: 0x040077D3 RID: 30675
		private const string COMPETITIVE_DESC_KEY = "COMPETITIVE_DESC";

		// Token: 0x040077D4 RID: 30676
		private const string QUEUE_SCREEN_ALL_QUEUES_KEY = "QUEUE_SCREEN_ALL_QUEUES";

		// Token: 0x040077D5 RID: 30677
		private const string QUEUE_SCREEN_DEFAULT_QUEUES_KEY = "QUEUE_SCREEN_DEFAULT_QUEUES";

		// Token: 0x040077D6 RID: 30678
		private const string CURRENT_QUEUE_KEY = "CURRENT_QUEUE";

		// Token: 0x040077D7 RID: 30679
		private const string DEFAULT_QUEUE_KEY = "DEFAULT_QUEUE";

		// Token: 0x040077D8 RID: 30680
		private const string MINIGAMES_QUEUE_KEY = "MINIGAMES_QUEUE";

		// Token: 0x040077D9 RID: 30681
		private const string COMPETITIVE_QUEUE_KEY = "COMPETITIVE_QUEUE";

		// Token: 0x040077DA RID: 30682
		private const string MIC_SCREEN_INTRO_KEY = "MIC_SCREEN_INTRO";

		// Token: 0x040077DB RID: 30683
		private const string MIC_SCREEN_OPTIONS_KEY = "MIC_SCREEN_OPTIONS";

		// Token: 0x040077DC RID: 30684
		private const string MIC_SCREEN_CURRENT_KEY = "MIC_SCREEN_CURRENT";

		// Token: 0x040077DD RID: 30685
		private const string MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP";

		// Token: 0x040077DE RID: 30686
		private const string MIC_SCREEN_MIC_DISABLED_KEY = "MIC_SCREEN_MIC_DISABLED";

		// Token: 0x040077DF RID: 30687
		private const string MIC_SCREEN_NO_MIC_KEY = "MIC_SCREEN_NO_MIC";

		// Token: 0x040077E0 RID: 30688
		private const string MIC_SCREEN_NO_PERMISSIONS_KEY = "MIC_SCREEN_NO_PERMISSIONS";

		// Token: 0x040077E1 RID: 30689
		private const string MIC_SCREEN_PUSH_TO_TALK_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_TALK_TOOLTIP";

		// Token: 0x040077E2 RID: 30690
		private const string MIC_SCREEN_INPUT_TEST_LABEL_KEY = "MIC_SCREEN_INPUT_TEST_LABEL";

		// Token: 0x040077E3 RID: 30691
		private const string MIC_SCREEN_INPUT_TEST_NO_MIC_KEY = "MIC_SCREEN_INPUT_TEST_NO_MIC";

		// Token: 0x040077E4 RID: 30692
		private const string ALL_CHAT_MIC_KEY = "ALL_CHAT_MIC";

		// Token: 0x040077E5 RID: 30693
		private const string PUSH_TO_TALK_MIC_KEY = "PUSH_TO_TALK_MIC";

		// Token: 0x040077E6 RID: 30694
		private const string PUSH_TO_MUTE_MIC_KEY = "PUSH_TO_MUTE_MIC";

		// Token: 0x040077E7 RID: 30695
		private const string OPEN_MIC_KEY = "OPEN_MIC";

		// Token: 0x040077E8 RID: 30696
		private const string AUTOMOD_SCREEN_INTRO_KEY = "AUTOMOD_SCREEN_INTRO";

		// Token: 0x040077E9 RID: 30697
		private const string AUTOMOD_SCREEN_OPTIONS_KEY = "AUTOMOD_SCREEN_OPTIONS";

		// Token: 0x040077EA RID: 30698
		private const string AUTOMOD_SCREEN_CURRENT_KEY = "AUTOMOD_SCREEN_CURRENT";

		// Token: 0x040077EB RID: 30699
		private const string AUTOMOD_AGGRESSIVE_KEY = "AUTOMOD_AGGRESSIVE";

		// Token: 0x040077EC RID: 30700
		private const string AUTOMOD_MODERATE_KEY = "AUTOMOD_MODERATE";

		// Token: 0x040077ED RID: 30701
		private const string AUTOMOD_OFF_KEY = "AUTOMOD_OFF";

		// Token: 0x040077EE RID: 30702
		private const string VOICE_CHAT_SCREEN_INTRO_OLD_KEY = "VOICE_CHAT_SCREEN_INTRO_OLD";

		// Token: 0x040077EF RID: 30703
		private const string VOICE_CHAT_SCREEN_OPTIONS_OLD_KEY = "VOICE_CHAT_SCREEN_OPTIONS_OLD";

		// Token: 0x040077F0 RID: 30704
		private const string VOICE_CHAT_SCREEN_CURRENT_OLD_KEY = "VOICE_CHAT_SCREEN_CURRENT_OLD";

		// Token: 0x040077F1 RID: 30705
		private const string TRUE_KEY = "TRUE";

		// Token: 0x040077F2 RID: 30706
		private const string FALSE_KEY = "FALSE";

		// Token: 0x040077F3 RID: 30707
		private const string VOICE_CHAT_SCREEN_INTRO_KEY = "VOICE_CHAT_SCREEN_INTRO";

		// Token: 0x040077F4 RID: 30708
		private const string VOICE_CHAT_SCREEN_OPTIONS_KEY = "VOICE_CHAT_SCREEN_OPTIONS";

		// Token: 0x040077F5 RID: 30709
		private const string VOICE_CHAT_SCREEN_CURRENT_KEY = "VOICE_CHAT_SCREEN_CURRENT";

		// Token: 0x040077F6 RID: 30710
		private const string VOICE_OPTION_HUMAN_KEY = "VOICE_OPTION_HUMAN";

		// Token: 0x040077F7 RID: 30711
		private const string VOICE_OPTION_MONKE_KEY = "VOICE_OPTION_MONKE";

		// Token: 0x040077F8 RID: 30712
		private const string VOICE_OPTION_OFF_KEY = "VOICE_OPTION_OFF";

		// Token: 0x040077F9 RID: 30713
		private const string VISUALS_SCREEN_INTRO_KEY = "VISUALS_SCREEN_INTRO";

		// Token: 0x040077FA RID: 30714
		private const string VISUALS_SCREEN_OPTIONS_KEY = "VISUALS_SCREEN_OPTIONS";

		// Token: 0x040077FB RID: 30715
		private const string VISUALS_SCREEN_CURRENT_KEY = "VISUALS_SCREEN_CURRENT";

		// Token: 0x040077FC RID: 30716
		private const string VISUALS_SCREEN_VOLUME_KEY = "VISUALS_SCREEN_VOLUME";

		// Token: 0x040077FD RID: 30717
		private const string CREDITS_KEY = "CREDITS";

		// Token: 0x040077FE RID: 30718
		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		// Token: 0x040077FF RID: 30719
		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		// Token: 0x04007800 RID: 30720
		private const string TIME_SCREEN_KEY = "TIME_SCREEN";

		// Token: 0x04007801 RID: 30721
		private const string GROUP_SCREEN_LIMITED_OLD_KEY = "GROUP_SCREEN_LIMITED_OLD";

		// Token: 0x04007802 RID: 30722
		private const string GROUP_SCREEN_FULL_OLD_KEY = "GROUP_SCREEN_FULL_OLD";

		// Token: 0x04007803 RID: 30723
		private const string GROUP_SCREEN_SELECTION_OLD_KEY = "GROUP_SCREEN_SELECTION_OLD";

		// Token: 0x04007804 RID: 30724
		private const string PLATFORM_STEAM_KEY = "PLATFORM_STEAM";

		// Token: 0x04007805 RID: 30725
		private const string PLATFORM_QUEST_KEY = "PLATFORM_QUEST";

		// Token: 0x04007806 RID: 30726
		private const string PLATFORM_PSVR_KEY = "PLATFORM_PSVR";

		// Token: 0x04007807 RID: 30727
		private const string PLATFORM_PICO_KEY = "PLATFORM_PICO";

		// Token: 0x04007808 RID: 30728
		private const string PLATFORM_OCULUS_PC_KEY = "PLATFORM_OCULUS_PC";

		// Token: 0x04007809 RID: 30729
		private const string SUPPORT_SCREEN_INTRO_KEY = "SUPPORT_SCREEN_INTRO";

		// Token: 0x0400780A RID: 30730
		private const string SUPPORT_SCREEN_DETAILS_PLAYER_ID_KEY = "SUPPORT_SCREEN_DETAILS_PLAYERID";

		// Token: 0x0400780B RID: 30731
		private const string SUPPORT_SCREEN_DETAILS_VERSION_KEY = "SUPPORT_SCREEN_DETAILS_VERSION";

		// Token: 0x0400780C RID: 30732
		private const string SUPPORT_SCREEN_DETAILS_PLATFORM_KEY = "SUPPORT_SCREEN_DETAILS_PLATFORM";

		// Token: 0x0400780D RID: 30733
		private const string SUPPORT_SCREEN_DETAILS_BUILD_DATE_KEY = "SUPPORT_SCREEN_DETAILS_BUILD_DATE";

		// Token: 0x0400780E RID: 30734
		private const string SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID_KEY = "SUPPORT_SCREEN_DETAILS_MOTHERSHIP_SESSION_ID";

		// Token: 0x0400780F RID: 30735
		private const string SUPPORT_SCREEN_INITIAL_KEY = "SUPPORT_SCREEN_INITIAL";

		// Token: 0x04007810 RID: 30736
		private const string SUPPORT_SCREEN_INITIAL_WARNING_KEY = "SUPPORT_SCREEN_INITIAL_WARNING";

		// Token: 0x04007811 RID: 30737
		private const string OCULUS_BUILD_CODE_KEY = "OCULUS_BUILD_CODE";

		// Token: 0x04007812 RID: 30738
		private const string LOADING_SCREEN_KEY = "LOADING_SCREEN";

		// Token: 0x04007813 RID: 30739
		private const string WARNING_SCREEN_KEY = "WARNING_SCREEN";

		// Token: 0x04007814 RID: 30740
		private const string WARNING_SCREEN_CONFIRMATION_KEY = "WARNING_SCREEN_CONFIRMATION";

		// Token: 0x04007815 RID: 30741
		private const string WARNING_SCREEN_TYPE_YES_KEY = "WARNING_SCREEN_TYPE_YES";

		// Token: 0x04007816 RID: 30742
		private const string FUNCTION_ROOM_KEY = "FUNCTION_ROOM";

		// Token: 0x04007817 RID: 30743
		private const string FUNCTION_NAME_KEY = "FUNCTION_NAME";

		// Token: 0x04007818 RID: 30744
		private const string FUNCTION_COLOR_KEY = "FUNCTION_COLOR";

		// Token: 0x04007819 RID: 30745
		private const string FUNCTION_TURN_KEY = "FUNCTION_TURN";

		// Token: 0x0400781A RID: 30746
		private const string FUNCTION_MIC_KEY = "FUNCTION_MIC";

		// Token: 0x0400781B RID: 30747
		private const string FUNCTION_QUEUE_KEY = "FUNCTION_QUEUE";

		// Token: 0x0400781C RID: 30748
		private const string FUNCTION_GROUP_KEY = "FUNCTION_GROUP";

		// Token: 0x0400781D RID: 30749
		private const string FUNCTION_VOICE_KEY = "FUNCTION_VOICE";

		// Token: 0x0400781E RID: 30750
		private const string FUNCTION_AUTOMOD_KEY = "FUNCTION_AUTOMOD";

		// Token: 0x0400781F RID: 30751
		private const string FUNCTION_ITEMS_KEY = "FUNCTION_ITEMS";

		// Token: 0x04007820 RID: 30752
		private const string FUNCTION_CREDITS_KEY = "FUNCTION_CREDITS";

		// Token: 0x04007821 RID: 30753
		private const string FUNCTION_LANGUAGE_KEY = "FUNCTION_LANGUAGE";

		// Token: 0x04007822 RID: 30754
		private const string FUNCTION_SUPPORT_KEY = "FUNCTION_SUPPORT";

		// Token: 0x04007823 RID: 30755
		private const string COMPUTER_KEYBOARD_DELETE_KEY = "COMPUTER_KEYBOARD_DELETE";

		// Token: 0x04007824 RID: 30756
		private const string COMPUTER_KEYBOARD_ENTER_KEY = "COMPUTER_KEYBOARD_ENTER";

		// Token: 0x04007825 RID: 30757
		private const string COMPUTER_KEYBOARD_OPTION1_KEY = "COMPUTER_KEYBOARD_OPTION1";

		// Token: 0x04007826 RID: 30758
		private const string COMPUTER_KEYBOARD_OPTION2_KEY = "COMPUTER_KEYBOARD_OPTION2";

		// Token: 0x04007827 RID: 30759
		private const string COMPUTER_KEYBOARD_OPTION3_KEY = "COMPUTER_KEYBOARD_OPTION3";

		// Token: 0x04007828 RID: 30760
		private const string WARNING_SCREEN_YES_INPUT_KEY = "WARNING_SCREEN_YES_INPUT";

		// Token: 0x04007829 RID: 30761
		private const string GROUP_SCREEN_ENTER_PARTY_KEY = "GROUP_SCREEN_ENTER_PARTY";

		// Token: 0x0400782A RID: 30762
		private const string GROUP_SCREEN_ENTER_NOPARTY_KEY = "GROUP_SCREEN_ENTER_NOPARTY";

		// Token: 0x0400782B RID: 30763
		private const string GROUP_SCREEN_CANNOT_JOIN_KEY = "GROUP_SCREEN_CANNOT_JOIN";

		// Token: 0x0400782C RID: 30764
		private const string GROUP_SCREEN_ACTIVE_ZONES_KEY = "GROUP_SCREEN_ACTIVE_ZONES";

		// Token: 0x0400782D RID: 30765
		private const string GROUP_SCREEN_DESTINATIONS_KEY = "GROUP_SCREEN_DESTINATIONS";

		// Token: 0x0400782E RID: 30766
		private const string NAME_SCREEN_TOGGLE_NAMETAGS_KEY = "NAME_SCREEN_TOGGLE_NAMETAGS";

		// Token: 0x0400782F RID: 30767
		private const string NAME_SCREEN_KID_PROHIBITED_VERB_KEY = "NAME_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x04007830 RID: 30768
		private const string NAME_SCREEN_DISABLED_KEY = "NAME_SCREEN_DISABLED";

		// Token: 0x04007831 RID: 30769
		private const string ON_KEY = "ON_KEY";

		// Token: 0x04007832 RID: 30770
		private const string OFF_KEY = "OFF_KEY";

		// Token: 0x04007833 RID: 30771
		private const string KID_PROHIBITED_MESSAGE_KEY = "KID_PROHIBITED_MESSAGE";

		// Token: 0x04007834 RID: 30772
		private const string KID_PERMISSION_NEEDED_KEY = "KID_PERMISSION_NEEDED";

		// Token: 0x04007835 RID: 30773
		private const string KID_WAITING_PERMISSION_KEY = "KID_WAITING_PERMISSION";

		// Token: 0x04007836 RID: 30774
		private const string KID_REFRESH_PERMISSIONS_KEY = "KID_REFRESH_PERMISSIONS";

		// Token: 0x04007837 RID: 30775
		private const string KID_CHECK_AGAIN_COOLDOWN_KEY = "KID_CHECK_AGAIN_COOLDOWN";

		// Token: 0x04007838 RID: 30776
		private const string STARTUP_TROOP_TEXT_KEY = "STARTUP_TROOP_TEXT";

		// Token: 0x04007839 RID: 30777
		private const string ROOM_GROUP_TRAVEL_KEY = "ROOM_GROUP_TRAVEL";

		// Token: 0x0400783A RID: 30778
		private const string ROOM_PARTY_WARNING_KEY = "ROOM_PARTY_WARNING";

		// Token: 0x0400783B RID: 30779
		private const string ROOM_GAME_LABEL_KEY = "ROOM_GAME_LABEL";

		// Token: 0x0400783C RID: 30780
		private const string ROOM_SCREEN_KID_PROHIBITED_VERB_KEY = "ROOM_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x0400783D RID: 30781
		private const string ROOM_SCREEN_DISABLED_KEY = "ROOM_SCREEN_DISABLED";

		// Token: 0x0400783E RID: 30782
		private const string REDEMPTION_INTRO_KEY = "REDEMPTION_INTRO";

		// Token: 0x0400783F RID: 30783
		private const string REDEMPTION_CODE_LABEL_KEY = "REDEMPTION_CODE_LABEL";

		// Token: 0x04007840 RID: 30784
		private const string REDEMPTION_CODE_INVALID_KEY = "REDEMPTION_CODE_INVALID";

		// Token: 0x04007841 RID: 30785
		private const string REDEMPTION_CODE_VALIDATING_KEY = "REDEMPTION_CODE_VALIDATING";

		// Token: 0x04007842 RID: 30786
		private const string REDEMPTION_CODE_ALREADY_USED_KEY = "REDEMPTION_CODE_ALREADY_USED";

		// Token: 0x04007843 RID: 30787
		private const string REDEMPTION_CODE_TOO_EARLY_KEY = "REDEMPTION_CODE_TOO_EARLY";

		// Token: 0x04007844 RID: 30788
		private const string REDEMPTION_CODE_TOO_LATE_KEY = "REDEMPTION_CODE_TOO_LATE";

		// Token: 0x04007845 RID: 30789
		private const string REDEMPTION_CODE_SUCCESS_KEY = "REDEMPTION_CODE_SUCCESS";

		// Token: 0x04007846 RID: 30790
		private const string LIMITED_ONLINE_FUNC_KEY = "LIMITED_ONLINE_FUNC";

		// Token: 0x04007847 RID: 30791
		private const string CURRENT_MODE_KEY = "CURRENT_MODE";

		// Token: 0x04007848 RID: 30792
		private const string SUPPORT_META_ACCOUNT_TYPE_KEY = "SUPPORT_META_ACCOUNT_TYPE";

		// Token: 0x04007849 RID: 30793
		private const string SUPPORT_FINAL_QUEST_ONE_KEY = "SUPPORT_FINAL_QUEST_ONE";

		// Token: 0x0400784A RID: 30794
		private const string SUPPORT_KID_ACCOUNT_TYPE_KEY = "SUPPORT_KID_ACCOUNT_TYPE";

		// Token: 0x0400784B RID: 30795
		private const string VOICE_SCREEN_KID_PROHIBITED_VERB_KEY = "VOICE_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x0400784C RID: 30796
		private const string VOICE_SCREEN_DISABLED_KEY = "VOICE_SCREEN_DISABLED";

		// Token: 0x0400784D RID: 30797
		private const string MIC_SCREEN_GUARDIAN_FEATURE_DESC_KEY = "VOICE_SCREEN_GUARDIAN_FEATURE_DESC";

		// Token: 0x0400784E RID: 30798
		private const string VOICE_SCREEN_KID_CURRENT_VOICE_KEY = "VOICE_SCREEN_KID_CURRENT_VOICE";

		// Token: 0x0400784F RID: 30799
		private const string MIC_SCREEN_PUSH_KEY_INSTRUCTIONS_KEY = "MIC_SCREEN_PUSH_KEY_INSTRUCTIONS";

		// Token: 0x04007850 RID: 30800
		private const string TROOP_SCREEN_INTRO_KEY = "TROOP_SCREEN_INTRO";

		// Token: 0x04007851 RID: 30801
		private const string TROOP_SCREEN_INSTRUCTIONS_KEY = "TROOP_SCREEN_INSTRUCTIONS";

		// Token: 0x04007852 RID: 30802
		private const string TROOP_SCREEN_CURRENT_TROOP_KEY = "TROOP_SCREEN_CURRENT_TROOP";

		// Token: 0x04007853 RID: 30803
		private const string TROOP_SCREEN_IN_QUEUE_KEY = "TROOP_SCREEN_IN_QUEUE";

		// Token: 0x04007854 RID: 30804
		private const string TROOP_SCREEN_PLAYERS_IN_TROOP_KEY = "TROOP_SCREEN_PLAYERS_IN_TROOP";

		// Token: 0x04007855 RID: 30805
		private const string TROOP_SCREEN_DEFAULT_QUEUE_KEY = "TROOP_SCREEN_DEFAULT_QUEUE";

		// Token: 0x04007856 RID: 30806
		private const string TROOP_SCREEN_CURRENT_QUEUE_KEY = "TROOP_SCREEN_CURRENT_QUEUE";

		// Token: 0x04007857 RID: 30807
		private const string TROOP_SCREEN_TROOP_QUEUE_KEY = "TROOP_SCREEN_TROOP_QUEUE";

		// Token: 0x04007858 RID: 30808
		private const string TROOP_SCREEN_LEAVE_KEY = "TROOP_SCREEN_LEAVE";

		// Token: 0x04007859 RID: 30809
		private const string TROOP_SCREEN_NOT_IN_TROOP_KEY = "TROOP_SCREEN_NOT_IN_TROOP";

		// Token: 0x0400785A RID: 30810
		private const string TROOP_SCREEN_JOIN_TROOP_KEY = "TROOP_SCREEN_JOIN_TROOP";

		// Token: 0x0400785B RID: 30811
		private const string TROOP_SCREEN_KID_PROHIBITED_VERB_KEY = "TROOP_SCREEN_KID_PROHIBITED_VERB";

		// Token: 0x0400785C RID: 30812
		private const string TROOP_SCREEN_DISABLED_KEY = "TROOP_SCREEN_DISABLED";

		// Token: 0x0400785D RID: 30813
		private const string TROOP_SCREEN_KID_DESC_KEY = "TROOP_SCREEN_KID_DESC";

		// Token: 0x0400785E RID: 30814
		private const bool HIDE_SCREENS = false;

		// Token: 0x0400785F RID: 30815
		public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";

		// Token: 0x04007860 RID: 30816
		[OnEnterPlay_SetNull]
		public static volatile GorillaComputer instance;

		// Token: 0x04007861 RID: 30817
		[OnEnterPlay_Set(false)]
		public static bool hasInstance = false;

		// Token: 0x04007862 RID: 30818
		[OnEnterPlay_SetNull]
		private static Action<bool> onNametagSettingChangedAction;

		// Token: 0x04007863 RID: 30819
		public bool tryGetTimeAgain;

		// Token: 0x04007864 RID: 30820
		public Material unpressedMaterial;

		// Token: 0x04007865 RID: 30821
		public Material pressedMaterial;

		// Token: 0x04007866 RID: 30822
		public string currentTextField;

		// Token: 0x04007867 RID: 30823
		public float buttonFadeTime;

		// Token: 0x04007868 RID: 30824
		public string offlineTextInitialString;

		// Token: 0x04007869 RID: 30825
		public GorillaText screenText;

		// Token: 0x0400786A RID: 30826
		public GorillaText functionSelectText;

		// Token: 0x0400786B RID: 30827
		public GorillaText wallScreenText;

		// Token: 0x0400786C RID: 30828
		private Locale _lastLocaleChecked_Version;

		// Token: 0x0400786D RID: 30829
		private Locale _lastLocaleChecked_Connect;

		// Token: 0x0400786E RID: 30830
		private string _cachedVersionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";

		// Token: 0x0400786F RID: 30831
		private string _cachedUnableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";

		// Token: 0x04007870 RID: 30832
		public Material wrongVersionMaterial;

		// Token: 0x04007871 RID: 30833
		public MeshRenderer wallScreenRenderer;

		// Token: 0x04007872 RID: 30834
		public MeshRenderer computerScreenRenderer;

		// Token: 0x04007873 RID: 30835
		public long startupMillis;

		// Token: 0x04007874 RID: 30836
		public DateTime startupTime;

		// Token: 0x04007875 RID: 30837
		public GameModeType lastPressedGameModeType;

		// Token: 0x04007876 RID: 30838
		public string lastPressedGameMode;

		// Token: 0x04007877 RID: 30839
		public WatchableStringSO currentGameMode;

		// Token: 0x04007878 RID: 30840
		public WatchableStringSO currentGameModeText;

		// Token: 0x04007879 RID: 30841
		public int includeUpdatedServerSynchTest;

		// Token: 0x0400787A RID: 30842
		public PhotonNetworkController networkController;

		// Token: 0x0400787B RID: 30843
		public float updateCooldown = 1f;

		// Token: 0x0400787C RID: 30844
		private float defaultUpdateCooldown;

		// Token: 0x0400787D RID: 30845
		private float micUpdateCooldown = 0.01f;

		// Token: 0x0400787E RID: 30846
		public float lastUpdateTime;

		// Token: 0x0400787F RID: 30847
		private float deltaTime;

		// Token: 0x04007880 RID: 30848
		public bool isConnectedToMaster;

		// Token: 0x04007881 RID: 30849
		public bool internetFailure;

		// Token: 0x04007882 RID: 30850
		public string[] _allowedMapsToJoin;

		// Token: 0x04007883 RID: 30851
		public bool limitOnlineScreens;

		// Token: 0x04007884 RID: 30852
		[Header("State vars")]
		public bool stateUpdated;

		// Token: 0x04007885 RID: 30853
		public bool screenChanged;

		// Token: 0x04007886 RID: 30854
		public bool initialized;

		// Token: 0x04007887 RID: 30855
		public List<GorillaComputer.StateOrderItem> OrderList = new List<GorillaComputer.StateOrderItem>
		{
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Language, "Lang"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits),
			new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support)
		};

		// Token: 0x04007888 RID: 30856
		public string Pointer = "<-";

		// Token: 0x04007889 RID: 30857
		public int highestCharacterCount;

		// Token: 0x0400788A RID: 30858
		public List<string> FunctionNames = new List<string>();

		// Token: 0x0400788B RID: 30859
		public int FunctionsCount;

		// Token: 0x0400788C RID: 30860
		[Header("Room vars")]
		public string roomToJoin;

		// Token: 0x0400788D RID: 30861
		public bool roomFull;

		// Token: 0x0400788E RID: 30862
		public bool roomNotAllowed;

		// Token: 0x0400788F RID: 30863
		[Header("Mic vars")]
		public string pttType;

		// Token: 0x04007890 RID: 30864
		private GorillaSpeakerLoudness speakerLoudness;

		// Token: 0x04007891 RID: 30865
		private float micInputTestTimer;

		// Token: 0x04007892 RID: 30866
		public float micInputTestTimerThreshold = 10f;

		// Token: 0x04007893 RID: 30867
		[Header("Automute vars")]
		public string autoMuteType;

		// Token: 0x04007894 RID: 30868
		[Header("Queue vars")]
		public string currentQueue;

		// Token: 0x04007895 RID: 30869
		public bool allowedInCompetitive;

		// Token: 0x04007896 RID: 30870
		[Header("Group Vars")]
		public string groupMapJoin;

		// Token: 0x04007897 RID: 30871
		public int groupMapJoinIndex;

		// Token: 0x04007898 RID: 30872
		public GorillaFriendCollider friendJoinCollider;

		// Token: 0x04007899 RID: 30873
		[Header("Troop vars")]
		public string troopName;

		// Token: 0x0400789A RID: 30874
		public bool troopQueueActive;

		// Token: 0x0400789B RID: 30875
		public string troopToJoin;

		// Token: 0x0400789C RID: 30876
		private bool rememberTroopQueueState;

		// Token: 0x0400789D RID: 30877
		[Header("Join Triggers")]
		public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();

		// Token: 0x0400789E RID: 30878
		public string voiceChatOn;

		// Token: 0x0400789F RID: 30879
		[Header("Mode select vars")]
		public ModeSelectButton[] modeSelectButtons;

		// Token: 0x040078A0 RID: 30880
		public string version;

		// Token: 0x040078A1 RID: 30881
		public string buildDate;

		// Token: 0x040078A2 RID: 30882
		public string buildCode;

		// Token: 0x040078A3 RID: 30883
		[Header("Cosmetics")]
		public bool disableParticles;

		// Token: 0x040078A4 RID: 30884
		public float instrumentVolume;

		// Token: 0x040078A5 RID: 30885
		public bool perfMode;

		// Token: 0x040078A6 RID: 30886
		public bool isSubcribed;

		// Token: 0x040078A7 RID: 30887
		[Header("Credits")]
		public CreditsView creditsView;

		// Token: 0x040078A8 RID: 30888
		[Header("Handedness")]
		public bool leftHanded;

		// Token: 0x040078A9 RID: 30889
		[Header("Name state vars")]
		public string savedName;

		// Token: 0x040078AA RID: 30890
		public string currentName;

		// Token: 0x040078AB RID: 30891
		public TextAsset exactOneWeekFile;

		// Token: 0x040078AC RID: 30892
		public TextAsset anywhereOneWeekFile;

		// Token: 0x040078AD RID: 30893
		public TextAsset anywhereTwoWeekFile;

		// Token: 0x040078AE RID: 30894
		private List<GorillaComputer.ComputerState> _filteredStates = new List<GorillaComputer.ComputerState>();

		// Token: 0x040078AF RID: 30895
		private List<GorillaComputer.StateOrderItem> _activeOrderList = new List<GorillaComputer.StateOrderItem>();

		// Token: 0x040078B0 RID: 30896
		private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();

		// Token: 0x040078B1 RID: 30897
		private GorillaComputer.ComputerState currentComputerState;

		// Token: 0x040078B2 RID: 30898
		private GorillaComputer.ComputerState previousComputerState;

		// Token: 0x040078B3 RID: 30899
		private int currentStateIndex;

		// Token: 0x040078B4 RID: 30900
		private int usersBanned;

		// Token: 0x040078B5 RID: 30901
		private float redValue;

		// Token: 0x040078B6 RID: 30902
		private string redText;

		// Token: 0x040078B7 RID: 30903
		private float blueValue;

		// Token: 0x040078B8 RID: 30904
		private string blueText;

		// Token: 0x040078B9 RID: 30905
		private float greenValue;

		// Token: 0x040078BA RID: 30906
		private string greenText;

		// Token: 0x040078BB RID: 30907
		private int colorCursorLine;

		// Token: 0x040078BC RID: 30908
		private string warningConfirmationInputString = string.Empty;

		// Token: 0x040078BD RID: 30909
		private bool displaySupport;

		// Token: 0x040078BE RID: 30910
		private string[] exactOneWeek;

		// Token: 0x040078BF RID: 30911
		private string[] anywhereOneWeek;

		// Token: 0x040078C0 RID: 30912
		private string[] anywhereTwoWeek;

		// Token: 0x040078C1 RID: 30913
		private GorillaComputer.RedemptionResult redemptionResult;

		// Token: 0x040078C2 RID: 30914
		private string redemptionCode = "";

		// Token: 0x040078C3 RID: 30915
		private bool playerInVirtualStump;

		// Token: 0x040078C4 RID: 30916
		private string virtualStumpRoomPrepend = "";

		// Token: 0x040078C5 RID: 30917
		private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);

		// Token: 0x040078C6 RID: 30918
		private Coroutine LoadingRoutine;

		// Token: 0x040078C7 RID: 30919
		private List<string> topTroops = new List<string>();

		// Token: 0x040078C8 RID: 30920
		private bool hasRequestedInitialTroopPopulation;

		// Token: 0x040078C9 RID: 30921
		private int currentTroopPopulation = -1;

		// Token: 0x040078CA RID: 30922
		private List<string> topVstumpMaps = new List<string>();

		// Token: 0x040078CD RID: 30925
		private float lastCheckedWifi;

		// Token: 0x040078CE RID: 30926
		private float checkIfDisconnectedSeconds = 10f;

		// Token: 0x040078CF RID: 30927
		private float checkIfConnectedSeconds = 1f;

		// Token: 0x040078D0 RID: 30928
		private bool didInitializeGameMode;

		// Token: 0x040078D1 RID: 30929
		private static int sessionCount = -1;

		// Token: 0x040078D2 RID: 30930
		private const bool k_debug_shouldResetSessionCount = false;

		// Token: 0x040078D3 RID: 30931
		private const bool k_debug_shouldResetGameMode = false;

		// Token: 0x040078D4 RID: 30932
		private const string k_sessionCountKey = "sessionCount";

		// Token: 0x040078D5 RID: 30933
		internal const GameModeType k_defaultGameMode = GameModeType.SuperInfect;

		// Token: 0x040078D6 RID: 30934
		internal const GameModeType k_noobGameMode = GameModeType.Infection;

		// Token: 0x040078D7 RID: 30935
		private const int k_noobSessionCountThreshold = 4;

		// Token: 0x040078D8 RID: 30936
		private float troopPopulationCheckCooldown = 3f;

		// Token: 0x040078D9 RID: 30937
		private float nextPopulationCheckTime;

		// Token: 0x040078DA RID: 30938
		public Action OnServerTimeUpdated;

		// Token: 0x040078DB RID: 30939
		private const string ENABLED_COLOUR = "#85ffa5";

		// Token: 0x040078DC RID: 30940
		private const string DISABLED_COLOUR = "\"RED\"";

		// Token: 0x040078DD RID: 30941
		private const string FAMILY_PORTAL_URL = "k-id.com/code";

		// Token: 0x040078DE RID: 30942
		private float _updateAttemptCooldown = 15f;

		// Token: 0x040078DF RID: 30943
		private float _nextUpdateAttemptTime;

		// Token: 0x040078E0 RID: 30944
		private bool _waitingForUpdatedSession;

		// Token: 0x040078E1 RID: 30945
		private GorillaComputer.EKidScreenState _currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;

		// Token: 0x040078E2 RID: 30946
		private string[] _interestedPermissionNames = new string[]
		{
			"custom-username",
			"voice-chat",
			"join-groups"
		};

		// Token: 0x040078E3 RID: 30947
		private const string LANG_SCREEN_TITLE_KEY = "LANG_SCREEN_TITLE";

		// Token: 0x040078E4 RID: 30948
		private const string LANG_SCREEN_INSTRUCTIONS_KEY = "LANG_SCREEN_INSTRUCTIONS";

		// Token: 0x040078E5 RID: 30949
		private const string LANG_SCREEN_CURRENT_LANGUAGE_KEY = "LANG_SCREEN_CURRENT_LANGUAGE";

		// Token: 0x040078E6 RID: 30950
		private StringBuilder _languagesDisplaySB = new StringBuilder();

		// Token: 0x040078E7 RID: 30951
		private Locale _previousLocalisationSetting;

		// Token: 0x0200104A RID: 4170
		public enum ComputerState
		{
			// Token: 0x040078E9 RID: 30953
			Startup,
			// Token: 0x040078EA RID: 30954
			Color,
			// Token: 0x040078EB RID: 30955
			Name,
			// Token: 0x040078EC RID: 30956
			Turn,
			// Token: 0x040078ED RID: 30957
			Mic,
			// Token: 0x040078EE RID: 30958
			Room,
			// Token: 0x040078EF RID: 30959
			Queue,
			// Token: 0x040078F0 RID: 30960
			Group,
			// Token: 0x040078F1 RID: 30961
			Voice,
			// Token: 0x040078F2 RID: 30962
			AutoMute,
			// Token: 0x040078F3 RID: 30963
			Credits,
			// Token: 0x040078F4 RID: 30964
			Visuals,
			// Token: 0x040078F5 RID: 30965
			Time,
			// Token: 0x040078F6 RID: 30966
			NameWarning,
			// Token: 0x040078F7 RID: 30967
			Loading,
			// Token: 0x040078F8 RID: 30968
			Support,
			// Token: 0x040078F9 RID: 30969
			Troop,
			// Token: 0x040078FA RID: 30970
			KID,
			// Token: 0x040078FB RID: 30971
			Redemption,
			// Token: 0x040078FC RID: 30972
			Language
		}

		// Token: 0x0200104B RID: 4171
		private enum NameCheckResult
		{
			// Token: 0x040078FE RID: 30974
			Success,
			// Token: 0x040078FF RID: 30975
			Warning,
			// Token: 0x04007900 RID: 30976
			Ban
		}

		// Token: 0x0200104C RID: 4172
		public enum RedemptionResult
		{
			// Token: 0x04007902 RID: 30978
			Empty,
			// Token: 0x04007903 RID: 30979
			Invalid,
			// Token: 0x04007904 RID: 30980
			Checking,
			// Token: 0x04007905 RID: 30981
			AlreadyUsed,
			// Token: 0x04007906 RID: 30982
			TooEarly,
			// Token: 0x04007907 RID: 30983
			TooLate,
			// Token: 0x04007908 RID: 30984
			Success
		}

		// Token: 0x0200104D RID: 4173
		[Serializable]
		public class StateOrderItem
		{
			// Token: 0x060068FE RID: 26878 RVA: 0x0022005A File Offset: 0x0021E25A
			public StateOrderItem()
			{
			}

			// Token: 0x060068FF RID: 26879 RVA: 0x00220078 File Offset: 0x0021E278
			public StateOrderItem(GorillaComputer.ComputerState state)
			{
				this.State = state;
			}

			// Token: 0x06006900 RID: 26880 RVA: 0x0022009D File Offset: 0x0021E29D
			public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
			{
				this.State = state;
				this.OverrideName = overrideName;
			}

			// Token: 0x06006901 RID: 26881 RVA: 0x002200CC File Offset: 0x0021E2CC
			public string GetName()
			{
				if (this._previousLocale == LocalizationSettings.SelectedLocale && !string.IsNullOrEmpty(this._cachedTranslation))
				{
					return this._cachedTranslation;
				}
				if (this.StringReference == null || this.StringReference.IsEmpty)
				{
					return this.GetPreLocalisedName();
				}
				this._previousLocale = LocalizationSettings.SelectedLocale;
				string localizedString = this.StringReference.GetLocalizedString();
				this._cachedTranslation = ((localizedString != null) ? localizedString.ToUpper() : null);
				if (string.IsNullOrEmpty(this._cachedTranslation))
				{
					if (LocalisationManager.ApplicationRunning)
					{
						string[] array = new string[5];
						array[0] = "[LOCALIZATION::STATE_ORDER_ITEM] Failed to get translation for selected locale [";
						int num = 1;
						Locale previousLocale = this._previousLocale;
						array[num] = (((previousLocale != null) ? previousLocale.LocaleName : null) ?? "NULL");
						array[2] = ", for item [";
						array[3] = this.State.GetName<GorillaComputer.ComputerState>();
						array[4] = "]";
						Debug.LogError(string.Concat(array));
					}
					this._cachedTranslation = "";
				}
				return this._cachedTranslation;
			}

			// Token: 0x06006902 RID: 26882 RVA: 0x002201BC File Offset: 0x0021E3BC
			public string GetPreLocalisedName()
			{
				if (!string.IsNullOrEmpty(this.OverrideName))
				{
					return this.OverrideName.ToUpper();
				}
				return this.State.ToString().ToUpper();
			}

			// Token: 0x04007909 RID: 30985
			public GorillaComputer.ComputerState State;

			// Token: 0x0400790A RID: 30986
			[Tooltip("Case not important - ToUpper applied at runtime")]
			public string OverrideName = "";

			// Token: 0x0400790B RID: 30987
			public LocalizedString StringReference;

			// Token: 0x0400790C RID: 30988
			private Locale _previousLocale;

			// Token: 0x0400790D RID: 30989
			private string _cachedTranslation = "";
		}

		// Token: 0x0200104E RID: 4174
		private enum EKidScreenState
		{
			// Token: 0x0400790F RID: 30991
			Ready,
			// Token: 0x04007910 RID: 30992
			Show_OTP,
			// Token: 0x04007911 RID: 30993
			Show_Setup_Screen
		}
	}
}
