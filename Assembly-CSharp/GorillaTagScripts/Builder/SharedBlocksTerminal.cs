using System;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FE4 RID: 4068
	public class SharedBlocksTerminal : MonoBehaviour
	{
		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x060065A8 RID: 26024 RVA: 0x0020C2F8 File Offset: 0x0020A4F8
		public SharedBlocksManager.SharedBlocksMap SelectedMap
		{
			get
			{
				return this.selectedMap;
			}
		}

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x060065A9 RID: 26025 RVA: 0x0020C300 File Offset: 0x0020A500
		public bool IsTerminalLocked
		{
			get
			{
				return this.isTerminalLocked;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x060065AA RID: 26026 RVA: 0x0020C308 File Offset: 0x0020A508
		private int playersInLobby
		{
			get
			{
				return this.lobbyTrigger.playerIDsCurrentlyTouching.Count;
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x060065AB RID: 26027 RVA: 0x0020C31A File Offset: 0x0020A51A
		public bool IsDriver
		{
			get
			{
				return this.localState.driverID == NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
		}

		// Token: 0x060065AC RID: 26028 RVA: 0x0020C338 File Offset: 0x0020A538
		public BuilderTable GetTable()
		{
			return this.linkedTable;
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x060065AD RID: 26029 RVA: 0x0020C340 File Offset: 0x0020A540
		public int GetDriverID
		{
			get
			{
				return this.localState.driverID;
			}
		}

		// Token: 0x060065AE RID: 26030 RVA: 0x0020C350 File Offset: 0x0020A550
		public static string MapIDToDisplayedString(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return "____-____";
			}
			int num = 4;
			SharedBlocksTerminal.sb.Clear();
			if (mapID.Length > num)
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0, num));
				SharedBlocksTerminal.sb.Append("-");
				SharedBlocksTerminal.sb.Append(mapID.Substring(num));
				int repeatCount = 9 - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', repeatCount);
			}
			else
			{
				SharedBlocksTerminal.sb.Append(mapID.Substring(0));
				int repeatCount2 = num - SharedBlocksTerminal.sb.Length;
				SharedBlocksTerminal.sb.Append('_', repeatCount2);
				SharedBlocksTerminal.sb.Append("-____");
			}
			return SharedBlocksTerminal.sb.ToString();
		}

		// Token: 0x060065AF RID: 26031 RVA: 0x0020C41C File Offset: 0x0020A61C
		public void Init(BuilderTable table)
		{
			if (this.hasInitialized)
			{
				return;
			}
			this.localState = new SharedBlocksTerminal.SharedBlocksTerminalState
			{
				state = SharedBlocksTerminal.TerminalState.NoStatus,
				driverID = -2
			};
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.AddListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			this.terminalControlButton.onPressButton.AddListener(new UnityAction(this.OnTerminalControlPressed));
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.linkedTable = table;
			table.linkedTerminal = this;
			this.linkedTable.OnMapLoaded.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
			this.linkedTable.OnMapLoadFailed.AddListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
			this.linkedTable.OnMapCleared.AddListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
			this.hasInitialized = true;
		}

		// Token: 0x060065B0 RID: 26032 RVA: 0x0020C538 File Offset: 0x0020A738
		private void Start()
		{
			BuilderTable table;
			if (!this.hasInitialized && BuilderTable.TryGetBuilderTableForZone(this.tableZone, out table))
			{
				this.Init(table);
				return;
			}
			Debug.LogWarning("Could not find builder table for zone " + this.tableZone.ToString());
		}

		// Token: 0x060065B1 RID: 26033 RVA: 0x0020C584 File Offset: 0x0020A784
		private void LateUpdate()
		{
			if (this.localState.driverID == -2)
			{
				return;
			}
			if (GorillaComputer.instance == null)
			{
				return;
			}
			if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
			{
				return;
			}
			this.useNametags = GorillaComputer.instance.NametagsEnabled;
			this.RefreshDriverNickname();
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x0020C5E0 File Offset: 0x0020A7E0
		private void OnDestroy()
		{
			GameEvents.OnSharedBlocksKeyboardButtonPressedEvent.RemoveListener(new UnityAction<SharedBlocksKeyboardBindings>(this.PressButton));
			if (this.terminalControlButton != null)
			{
				this.terminalControlButton.onPressButton.RemoveListener(new UnityAction(this.OnTerminalControlPressed));
			}
			if (NetworkSystem.Instance != null)
			{
				NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
				NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
			}
			if (this.linkedTable != null)
			{
				this.linkedTable.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoaded));
				this.linkedTable.OnMapLoadFailed.RemoveListener(new UnityAction<string>(this.OnSharedBlocksMapLoadFailed));
				this.linkedTable.OnMapCleared.RemoveListener(new UnityAction(this.OnSharedBlocksMapLoadStart));
			}
		}

		// Token: 0x060065B3 RID: 26035 RVA: 0x0020C6E0 File Offset: 0x0020A8E0
		private void RefreshActiveScreen()
		{
			if (this.localState.driverID == -2)
			{
				if (this.currentScreen != this.noDriverScreen)
				{
					if (this.currentScreen != null)
					{
						this.currentScreen.Hide();
					}
					this.currentScreen = this.noDriverScreen;
					this.currentScreen.Show();
				}
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			if (this.currentScreen != this.searchScreen)
			{
				if (this.currentScreen != null)
				{
					this.currentScreen.Hide();
				}
				this.currentScreen = this.searchScreen;
				this.currentScreen.Show();
			}
		}

		// Token: 0x060065B4 RID: 26036 RVA: 0x0020C794 File Offset: 0x0020A994
		private void SetTerminalState(SharedBlocksTerminal.TerminalState state)
		{
			this.localState.state = state;
			string statusText = "";
			if (this.localState.driverID == -2)
			{
				this.statusMessageText.gameObject.SetActive(false);
				return;
			}
			switch (state)
			{
			case SharedBlocksTerminal.TerminalState.NoStatus:
				this.statusMessageText.gameObject.SetActive(false);
				return;
			case SharedBlocksTerminal.TerminalState.Searching:
			{
				string defaultResult = "SEARCHING...";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_SEARCH", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_SEARCH]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.NotFound:
			{
				string defaultResult = "MAP NOT FOUND";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.Found:
			{
				string defaultResult = "MAP FOUND. PRESS 'ENTER' TO LOAD";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.Loading:
			{
				string defaultResult = "LOADING...";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOADING", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.LoadSuccess:
			{
				string defaultResult = "LOAD SUCCESS";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS]");
				}
				this.SetStatusText(statusText);
				return;
			}
			case SharedBlocksTerminal.TerminalState.LoadFail:
			{
				string defaultResult = "LOAD FAILED";
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED", out statusText, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED]");
				}
				this.SetStatusText(statusText);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060065B5 RID: 26037 RVA: 0x0020C8FB File Offset: 0x0020AAFB
		public void SelectMapIDAndOpenInfo(string mapID)
		{
			if (this.awaitingWebRequest)
			{
				return;
			}
			this.selectedMap = null;
			this.awaitingWebRequest = true;
			this.requestedMapID = mapID;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.OnPlayerMapRequestComplete));
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x0020C93C File Offset: 0x0020AB3C
		private void OnPlayerMapRequestComplete(SharedBlocksManager.SharedBlocksMap response)
		{
			if (this.awaitingWebRequest)
			{
				this.awaitingWebRequest = false;
				this.requestedMapID = null;
				if (this.IsDriver)
				{
					if (response == null || response.MapID == null)
					{
						this.SetTerminalState(SharedBlocksTerminal.TerminalState.NotFound);
						return;
					}
					this.selectedMap = response;
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Found);
				}
			}
		}

		// Token: 0x060065B7 RID: 26039 RVA: 0x0020C988 File Offset: 0x0020AB88
		private bool CanChangeMapState(bool load, out string disallowedReason)
		{
			disallowedReason = "";
			if (!NetworkSystem.Instance.InRoom)
			{
				disallowedReason = "MUST BE IN A ROOM BEFORE  " + (load ? "" : "UN") + "LOADING A MAP.";
				string text = load ? "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD" : "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD";
				string text2;
				if (!LocalisationManager.TryGetKeyForCurrentLocale(text, out text2, disallowedReason))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [" + text + "]");
				}
				disallowedReason = text2;
				return false;
			}
			this.RefreshLobbyCount();
			if (!this.AreAllPlayersInLobby())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE LOBBY BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				string text3 = load ? "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD" : "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD";
				string text4;
				if (!LocalisationManager.TryGetKeyForCurrentLocale(text3, out text4, disallowedReason))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [" + text3 + "]");
				}
				disallowedReason = text4;
				return false;
			}
			return true;
		}

		// Token: 0x060065B8 RID: 26040 RVA: 0x0020CA63 File Offset: 0x0020AC63
		public void SetStatusText(string text)
		{
			this.statusMessageText.text = text;
			this.statusMessageText.gameObject.SetActive(true);
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x0020CA82 File Offset: 0x0020AC82
		private bool IsLocalPlayerInLobby()
		{
			return base.isActiveAndEnabled && this.lobbyTrigger.playerIDsCurrentlyTouching.Contains(VRRig.LocalRig.creator.UserId);
		}

		// Token: 0x060065BA RID: 26042 RVA: 0x0020CAB2 File Offset: 0x0020ACB2
		public bool AreAllPlayersInLobby()
		{
			return base.isActiveAndEnabled && this.playersInLobby == this.playersInRoom;
		}

		// Token: 0x060065BB RID: 26043 RVA: 0x0020CACC File Offset: 0x0020ACCC
		public string GetLobbyText()
		{
			string defaultResult = "PLAYERS IN ROOM {0}\nPLAYERS IN LOBBY {1}";
			string format;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT", out format, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for MONKE BLOCKS SCAN KIOSK localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT]");
			}
			return string.Format(format, this.playersInRoom, this.playersInLobby);
		}

		// Token: 0x060065BC RID: 26044 RVA: 0x0020CB14 File Offset: 0x0020AD14
		public void RefreshLobbyCount()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom)
			{
				this.playersInRoom = NetworkSystem.Instance.RoomPlayerCount;
				return;
			}
			this.playersInRoom = 0;
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x0020CB48 File Offset: 0x0020AD48
		public void PressButton(SharedBlocksKeyboardBindings buttonPressed)
		{
			if (!this.IsDriver)
			{
				string statusText;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER", out statusText, "NOT TERMINAL CONTROLLER"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER]");
				}
				this.SetStatusText(statusText);
				return;
			}
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Searching || this.localState.state == SharedBlocksTerminal.TerminalState.Loading)
			{
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.up)
			{
				this.OnUpButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.down)
			{
				this.OnDownButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.delete)
			{
				this.OnDeleteButtonPressed();
				return;
			}
			if (buttonPressed == SharedBlocksKeyboardBindings.enter)
			{
				this.OnSelectButtonPressed();
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.zero && buttonPressed <= SharedBlocksKeyboardBindings.nine)
			{
				this.OnNumberPressed((int)buttonPressed);
				return;
			}
			if (buttonPressed >= SharedBlocksKeyboardBindings.A && buttonPressed <= SharedBlocksKeyboardBindings.Z)
			{
				this.OnLetterPressed(buttonPressed.ToString());
			}
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x0020CBFD File Offset: 0x0020ADFD
		private void OnUpButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnUpPressed();
			}
		}

		// Token: 0x060065BF RID: 26047 RVA: 0x0020CC18 File Offset: 0x0020AE18
		private void OnDownButtonPressed()
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDownPressed();
			}
		}

		// Token: 0x060065C0 RID: 26048 RVA: 0x0020CC33 File Offset: 0x0020AE33
		private void OnSelectButtonPressed()
		{
			if (this.localState.state == SharedBlocksTerminal.TerminalState.Found)
			{
				this.OnLoadMapPressed();
				return;
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnSelectPressed();
			}
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x0020CC63 File Offset: 0x0020AE63
		private void OnDeleteButtonPressed()
		{
			if (this.localState.state != SharedBlocksTerminal.TerminalState.Loading && this.localState.state != SharedBlocksTerminal.TerminalState.Searching)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			}
			if (this.currentScreen != null)
			{
				this.currentScreen.OnDeletePressed();
			}
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnBackButtonPressed()
		{
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x0020CCA1 File Offset: 0x0020AEA1
		private void OnNumberPressed(int number)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnNumberPressed(number);
			}
		}

		// Token: 0x060065C4 RID: 26052 RVA: 0x0020CCBD File Offset: 0x0020AEBD
		private void OnLetterPressed(string letter)
		{
			if (this.currentScreen != null)
			{
				this.currentScreen.OnLetterPressed(letter);
			}
		}

		// Token: 0x060065C5 RID: 26053 RVA: 0x0020CCDC File Offset: 0x0020AEDC
		private void OnTerminalControlPressed()
		{
			if (this.isTerminalLocked)
			{
				if (this.IsDriver)
				{
					if (NetworkSystem.Instance.InRoom)
					{
						this.linkedTable.builderNetworking.RequestBlocksTerminalControl(false);
						return;
					}
					this.SetTerminalDriver(-2);
					return;
				}
			}
			else
			{
				if (NetworkSystem.Instance.InRoom)
				{
					this.linkedTable.builderNetworking.RequestBlocksTerminalControl(true);
					return;
				}
				this.SetTerminalDriver(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			}
		}

		// Token: 0x060065C6 RID: 26054 RVA: 0x0020CD54 File Offset: 0x0020AF54
		public void OnLoadMapPressed()
		{
			if (!this.IsDriver)
			{
				string statusText;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER", out statusText, "NOT TERMINAL CONTROLLER"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER]");
				}
				this.SetStatusText(statusText);
				return;
			}
			if (this.currentScreen == null || this.selectedMap == null)
			{
				string statusText2;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION", out statusText2, "NO MAP SELECTED"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION]");
				}
				this.SetStatusText(statusText2);
				return;
			}
			if (this.awaitingWebRequest || this.isLoadingMap)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS", out text, "BLOCKS LOAD ALREADY IN PROGRESS"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS]");
				}
				this.SetStatusText("BLOCKS LOAD ALREADY IN PROGRESS");
				return;
			}
			string statusText3;
			if (!this.CanChangeMapState(true, out statusText3))
			{
				this.SetStatusText(statusText3);
				return;
			}
			if (this.linkedTable != null)
			{
				if (Time.time > this.lastLoadTime + this.loadMapCooldown)
				{
					string text2;
					if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_LOADING", out text2, "LOADING BLOCKS ..."))
					{
						Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
					}
					this.SetStatusText("LOADING BLOCKS ...");
					this.isLoadingMap = true;
					this.lastLoadTime = Time.time;
					this.linkedTable.LoadSharedMap(this.selectedMap);
					return;
				}
				int num = Mathf.RoundToInt(this.lastLoadTime + this.loadMapCooldown - Time.time);
				string defaultResult = string.Format("PLEASE WAIT {0} SECONDS BEFORE LOADING ANOTHER MAP", num);
				string text3;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_STATUS_WAIT", out text3, defaultResult))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS TERMINAL localization [SHARE_BLOCKS_TERMINAL_STATUS_LOADING]");
				}
				text3 = text3.Replace("{time}", num.ToString());
				this.SetStatusText(text3);
			}
		}

		// Token: 0x060065C7 RID: 26055 RVA: 0x0020CEEA File Offset: 0x0020B0EA
		public bool IsPlayerDriver(Player player)
		{
			return player.ActorNumber == this.localState.driverID;
		}

		// Token: 0x060065C8 RID: 26056 RVA: 0x0020CEFF File Offset: 0x0020B0FF
		public bool ValidateTerminalControlRequest(bool locked, int playerNumber)
		{
			if (locked && playerNumber == -2)
			{
				return false;
			}
			if (this.localState.driverID == -2)
			{
				return locked;
			}
			return this.localState.driverID == playerNumber;
		}

		// Token: 0x060065C9 RID: 26057 RVA: 0x0020CF2A File Offset: 0x0020B12A
		private void OnDriverNameChanged()
		{
			this.RefreshDriverNickname();
		}

		// Token: 0x060065CA RID: 26058 RVA: 0x0020CF34 File Offset: 0x0020B134
		public void SetTerminalDriver(int playerNum)
		{
			if (playerNum != -2)
			{
				if (this.localState.driverID != -2 && this.localState.driverID != playerNum)
				{
					GTDev.LogWarning<string>(string.Format("Shared BlocksTerminal SetTerminalDriver cannot set {0} as driver while {1} is driver", playerNum, this.localState.driverID), null);
					return;
				}
				this.localState.driverID = playerNum;
				NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerNum);
				RigContainer rigContainer;
				if (netPlayerByID != null && VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					this.driverRig = rigContainer.Rig;
					this.driverRig.OnPlayerNameVisibleChanged += this.OnDriverNameChanged;
				}
				this.isTerminalLocked = true;
				this.UpdateTerminalButton();
				this.RefreshActiveScreen();
				this.searchScreen.SetInputTextEnabled(this.IsDriver);
				if (this.IsDriver && this.awaitingWebRequest)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Searching);
					this.searchScreen.SetMapCode(this.requestedMapID);
				}
				else if (this.isLoadingMap)
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
					this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				}
				else
				{
					this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				}
			}
			else
			{
				if (this.driverRig != null)
				{
					this.driverRig.OnPlayerNameVisibleChanged -= this.OnDriverNameChanged;
					this.driverRig = null;
				}
				this.localState.driverID = -2;
				this.isTerminalLocked = false;
				this.UpdateTerminalButton();
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
				this.RefreshActiveScreen();
			}
			this.RefreshDriverNickname();
		}

		// Token: 0x060065CB RID: 26059 RVA: 0x0020D0B4 File Offset: 0x0020B2B4
		private void RefreshDriverNickname()
		{
			StringVariable stringVariable = this._currentDriverLoc.StringReference["playerName"] as StringVariable;
			if (this.localState.driverID == -2)
			{
				this.currentDriverLabel.gameObject.SetActive(false);
				stringVariable.Value = "";
				this.currentDriverText.text = "";
				this.currentDriverText.gameObject.SetActive(false);
				return;
			}
			bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
			if (NetworkSystem.Instance.InRoom)
			{
				NetPlayer player = NetworkSystem.Instance.GetPlayer(this.localState.driverID);
				if (player != null && this.useNametags && flag)
				{
					RigContainer rigContainer;
					if (player.IsLocal)
					{
						stringVariable.Value = player.NickName;
						this.currentDriverText.text = player.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						stringVariable.Value = rigContainer.Rig.playerNameVisible;
						this.currentDriverText.text = rigContainer.Rig.playerNameVisible;
					}
					else
					{
						stringVariable.Value = player.DefaultName;
						this.currentDriverText.text = player.DefaultName;
					}
				}
				else
				{
					stringVariable.Value = "";
					this.currentDriverText.text = "";
				}
			}
			else
			{
				stringVariable.Value = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
				this.currentDriverText.text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
			}
			this.currentDriverLabel.gameObject.SetActive(true);
		}

		// Token: 0x060065CC RID: 26060 RVA: 0x0020D282 File Offset: 0x0020B482
		public bool ValidateLoadMapRequest(string mapID, int playerNum)
		{
			return playerNum == this.localState.driverID && this.AreAllPlayersInLobby() && SharedBlocksManager.IsMapIDValid(mapID);
		}

		// Token: 0x060065CD RID: 26061 RVA: 0x0020D2A4 File Offset: 0x0020B4A4
		private void OnJoinedRoom()
		{
			this.cachedLocalPlayerID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			this.ResetTerminalControl();
		}

		// Token: 0x060065CE RID: 26062 RVA: 0x0020D2C1 File Offset: 0x0020B4C1
		private void OnReturnedToSinglePlayer()
		{
			if (this.localState.driverID != this.cachedLocalPlayerID)
			{
				this.ResetTerminalControl();
			}
			else
			{
				this.localState.driverID = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			}
			this.cachedLocalPlayerID = -1;
		}

		// Token: 0x060065CF RID: 26063 RVA: 0x0020D2FF File Offset: 0x0020B4FF
		public void ResetTerminalControl()
		{
			this.localState.driverID = -2;
			this.isTerminalLocked = false;
			this.selectedMap = null;
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.NoStatus);
			this.RefreshActiveScreen();
			this.UpdateTerminalButton();
		}

		// Token: 0x060065D0 RID: 26064 RVA: 0x0020D32F File Offset: 0x0020B52F
		private void UpdateTerminalButton()
		{
			this.terminalControlButton.isOn = this.isTerminalLocked;
			this.terminalControlButton.UpdateColor();
		}

		// Token: 0x060065D1 RID: 26065 RVA: 0x0020D350 File Offset: 0x0020B550
		private void OnSharedBlocksMapLoaded(string mapID)
		{
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(mapID);
			}
			if (SharedBlocksManager.IsMapIDValid(mapID))
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadSuccess);
			}
			else if (this.localState.state != SharedBlocksTerminal.TerminalState.LoadFail)
			{
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			}
			this.isLoadingMap = false;
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x0020D39E File Offset: 0x0020B59E
		private void OnSharedBlocksMapLoadFailed(string message)
		{
			this.SetTerminalState(SharedBlocksTerminal.TerminalState.LoadFail);
			this.SetStatusText(message);
			this.isLoadingMap = false;
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x0020D3B8 File Offset: 0x0020B5B8
		private void OnSharedBlocksMapLoadStart()
		{
			if (this.linkedTable == null)
			{
				return;
			}
			if (!this.IsDriver)
			{
				this.searchScreen.SetMapCode(this.linkedTable.GetPendingMap());
				this.SetTerminalState(SharedBlocksTerminal.TerminalState.Loading);
				this.isLoadingMap = true;
				this.lastLoadTime = Time.time;
			}
		}

		// Token: 0x040074D9 RID: 29913
		public const string SHARE_BLOCKS_TERMINAL_PROMPT_KEY = "SHARE_BLOCKS_TERMINAL_PROMPT";

		// Token: 0x040074DA RID: 29914
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON";

		// Token: 0x040074DB RID: 29915
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_AVAILABLE_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_AVAILABLE";

		// Token: 0x040074DC RID: 29916
		public const string SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_LOCKED_KEY = "SHARE_BLOCKS_TERMINAL_CONTROL_BUTTON_LOCKED";

		// Token: 0x040074DD RID: 29917
		public const string SHARE_BLOCKS_TERMINAL_STATUS_SEARCH_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_SEARCH";

		// Token: 0x040074DE RID: 29918
		public const string SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_MAP_FOUND";

		// Token: 0x040074DF RID: 29919
		public const string SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_MAP_NOT_FOUND";

		// Token: 0x040074E0 RID: 29920
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOADING_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOADING";

		// Token: 0x040074E1 RID: 29921
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOAD_SUCCESS";

		// Token: 0x040074E2 RID: 29922
		public const string SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_LOAD_FAILED";

		// Token: 0x040074E3 RID: 29923
		public const string SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_NOT_CONTROLLER";

		// Token: 0x040074E4 RID: 29924
		public const string SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_NO_SELECTION";

		// Token: 0x040074E5 RID: 29925
		public const string SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_IN_PROGRESS";

		// Token: 0x040074E6 RID: 29926
		public const string SHARE_BLOCKS_TERMINAL_STATUS_WAIT_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_WAIT";

		// Token: 0x040074E7 RID: 29927
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_LOAD";

		// Token: 0x040074E8 RID: 29928
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_LOBBY_UNLOAD";

		// Token: 0x040074E9 RID: 29929
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_LOAD";

		// Token: 0x040074EA RID: 29930
		public const string SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD_KEY = "SHARE_BLOCKS_TERMINAL_STATUS_DISALLOWED_ROOM_UNLOAD";

		// Token: 0x040074EB RID: 29931
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL";

		// Token: 0x040074EC RID: 29932
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE";

		// Token: 0x040074ED RID: 29933
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH";

		// Token: 0x040074EE RID: 29934
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_VOTES_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_VOTES";

		// Token: 0x040074EF RID: 29935
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL";

		// Token: 0x040074F0 RID: 29936
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT";

		// Token: 0x040074F1 RID: 29937
		public const string SHARE_BLOCKS_TERMINAL_ERROR_TITLE_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_TITLE";

		// Token: 0x040074F2 RID: 29938
		public const string SHARE_BLOCKS_TERMINAL_ERROR_INSTRUCTIONS_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_INSTRUCTIONS";

		// Token: 0x040074F3 RID: 29939
		public const string SHARE_BLOCKS_TERMINAL_ERROR_BACK_KEY = "SHARE_BLOCKS_TERMINAL_ERROR_BACK";

		// Token: 0x040074F4 RID: 29940
		public const string SHARE_BLOCKS_TERMINAL_INFO_TITLE_KEY = "SHARE_BLOCKS_TERMINAL_INFO_TITLE";

		// Token: 0x040074F5 RID: 29941
		public const string SHARE_BLOCKS_TERMINAL_INFO_DATA_KEY = "SHARE_BLOCKS_TERMINAL_INFO_DATA";

		// Token: 0x040074F6 RID: 29942
		public const string SHARE_BLOCKS_TERMINAL_INFO_ENTER_KEY = "SHARE_BLOCKS_TERMINAL_INFO_ENTER";

		// Token: 0x040074F7 RID: 29943
		public const string SHARE_BLOCKS_TERMINAL_OTHER_DRIVER_KEY = "SHARE_BLOCKS_TERMINAL_OTHER_DRIVER";

		// Token: 0x040074F8 RID: 29944
		public const string SHARE_BLOCKS_TERMINAL_CONTROLLER_LABEL_KEY = "SHARE_BLOCKS_TERMINAL_CONTROLLER_LABEL";

		// Token: 0x040074F9 RID: 29945
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_LOBBY_TEXT_FORMAT";

		// Token: 0x040074FA RID: 29946
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH";

		// Token: 0x040074FB RID: 29947
		public const string SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID_KEY = "SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID";

		// Token: 0x040074FC RID: 29948
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x040074FD RID: 29949
		[SerializeField]
		private TMP_Text currentMapSelectionText;

		// Token: 0x040074FE RID: 29950
		[SerializeField]
		private TMP_Text statusMessageText;

		// Token: 0x040074FF RID: 29951
		[SerializeField]
		private TMP_Text currentDriverText;

		// Token: 0x04007500 RID: 29952
		[SerializeField]
		private TMP_Text currentDriverLabel;

		// Token: 0x04007501 RID: 29953
		[SerializeField]
		private LocalizedText _currentDriverLoc;

		// Token: 0x04007502 RID: 29954
		[SerializeField]
		private SharedBlocksScreen noDriverScreen;

		// Token: 0x04007503 RID: 29955
		[SerializeField]
		private SharedBlocksScreenSearch searchScreen;

		// Token: 0x04007504 RID: 29956
		[SerializeField]
		private GorillaPressableButton terminalControlButton;

		// Token: 0x04007505 RID: 29957
		[SerializeField]
		private float loadMapCooldown = 30f;

		// Token: 0x04007506 RID: 29958
		[SerializeField]
		private GorillaFriendCollider lobbyTrigger;

		// Token: 0x04007507 RID: 29959
		private SharedBlocksManager.SharedBlocksMap selectedMap;

		// Token: 0x04007508 RID: 29960
		private SharedBlocksScreen currentScreen;

		// Token: 0x04007509 RID: 29961
		private BuilderTable linkedTable;

		// Token: 0x0400750A RID: 29962
		public const int NO_DRIVER_ID = -2;

		// Token: 0x0400750B RID: 29963
		private bool awaitingWebRequest;

		// Token: 0x0400750C RID: 29964
		private string requestedMapID;

		// Token: 0x0400750D RID: 29965
		public const string POINTER = "> ";

		// Token: 0x0400750E RID: 29966
		public Action<bool> OnMapLoadComplete;

		// Token: 0x0400750F RID: 29967
		private bool isTerminalLocked;

		// Token: 0x04007510 RID: 29968
		private SharedBlocksTerminal.SharedBlocksTerminalState localState;

		// Token: 0x04007511 RID: 29969
		private int cachedLocalPlayerID = -1;

		// Token: 0x04007512 RID: 29970
		private bool isLoadingMap;

		// Token: 0x04007513 RID: 29971
		private float lastLoadTime;

		// Token: 0x04007514 RID: 29972
		private bool useNametags;

		// Token: 0x04007515 RID: 29973
		private bool hasInitialized;

		// Token: 0x04007516 RID: 29974
		private static StringBuilder sb = new StringBuilder();

		// Token: 0x04007517 RID: 29975
		private VRRig driverRig;

		// Token: 0x04007518 RID: 29976
		private static List<VRRig> tempRigs = new List<VRRig>(16);

		// Token: 0x04007519 RID: 29977
		private int playersInRoom;

		// Token: 0x02000FE5 RID: 4069
		public enum ScreenType
		{
			// Token: 0x0400751B RID: 29979
			NO_DRIVER,
			// Token: 0x0400751C RID: 29980
			SEARCH,
			// Token: 0x0400751D RID: 29981
			LOADING,
			// Token: 0x0400751E RID: 29982
			ERROR,
			// Token: 0x0400751F RID: 29983
			SCAN_INFO,
			// Token: 0x04007520 RID: 29984
			OTHER_DRIVER
		}

		// Token: 0x02000FE6 RID: 4070
		public enum TerminalState
		{
			// Token: 0x04007522 RID: 29986
			NoStatus,
			// Token: 0x04007523 RID: 29987
			Searching,
			// Token: 0x04007524 RID: 29988
			NotFound,
			// Token: 0x04007525 RID: 29989
			Found,
			// Token: 0x04007526 RID: 29990
			Loading,
			// Token: 0x04007527 RID: 29991
			LoadSuccess,
			// Token: 0x04007528 RID: 29992
			LoadFail
		}

		// Token: 0x02000FE7 RID: 4071
		public class SharedBlocksTerminalState
		{
			// Token: 0x04007529 RID: 29993
			public SharedBlocksTerminal.ScreenType currentScreen;

			// Token: 0x0400752A RID: 29994
			public SharedBlocksTerminal.TerminalState state;

			// Token: 0x0400752B RID: 29995
			public int driverID;
		}
	}
}
