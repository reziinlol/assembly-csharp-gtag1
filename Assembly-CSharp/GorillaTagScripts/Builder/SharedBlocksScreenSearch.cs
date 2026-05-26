using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FE3 RID: 4067
	public class SharedBlocksScreenSearch : SharedBlocksScreen, IGorillaSliceableSimple
	{
		// Token: 0x06006596 RID: 26006 RVA: 0x0020BD10 File Offset: 0x00209F10
		public override void OnSelectPressed()
		{
			if (SharedBlocksManager.IsMapIDValid(this.currentMapCode))
			{
				this.savedMapCode = this.currentMapCode;
				this.terminal.SelectMapIDAndOpenInfo(this.savedMapCode);
				return;
			}
			if (this.currentMapCode.Length < 8)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH", out text, "INVALID MAP ID LENGTH"))
				{
					Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_LENGTH]");
				}
				this.terminal.SetStatusText(text);
				return;
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID", out text2, "INVALID MAP ID"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_ERROR_INVALID_ID]");
			}
			this.terminal.SetStatusText(text2);
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x0020BDA9 File Offset: 0x00209FA9
		public override void OnDeletePressed()
		{
			if (this.currentMapCode.Length > 0)
			{
				this.currentMapCode = this.currentMapCode.Substring(0, this.currentMapCode.Length - 1);
				this.UpdateInput();
			}
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x0020BDDE File Offset: 0x00209FDE
		public override void OnNumberPressed(int number)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += number.ToString();
				this.UpdateInput();
			}
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x0020BE0C File Offset: 0x0020A00C
		public override void OnLetterPressed(string letter)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += letter;
				this.UpdateInput();
			}
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x0020BE34 File Offset: 0x0020A034
		public override void Show()
		{
			SharedBlocksManager.OnRecentMapIdsUpdated += this.DrawScreen;
			this.currentMapCode = string.Empty;
			this.DrawScreen();
			base.Show();
			this.RefreshPlayerCounter();
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnMapLoaded(table.GetCurrentMapID());
			}
		}

		// Token: 0x0600659B RID: 26011 RVA: 0x0020BEC0 File Offset: 0x0020A0C0
		public override void Hide()
		{
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			SharedBlocksManager.OnRecentMapIdsUpdated -= this.DrawScreen;
			base.Hide();
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x0020BF48 File Offset: 0x0020A148
		private void OnMapLoaded(string mapID)
		{
			string defaultResult = "LOADED MAP : " + (SharedBlocksManager.IsMapIDValid(mapID) ? SharedBlocksTerminal.MapIDToDisplayedString(mapID) : "NONE");
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale(SharedBlocksManager.IsMapIDValid(mapID) ? "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL" : "SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE", out text, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_LABEL]");
			}
			text = text.Replace("{mapDisplayName}", SharedBlocksTerminal.MapIDToDisplayedString(mapID));
			this.loadedMap.text = text;
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x0020BFBC File Offset: 0x0020A1BC
		private void OnMapCleared()
		{
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE", out text, "LOADED MAP : NONE"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_LOADED_NONE]");
			}
			this.loadedMap.text = text;
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x0020BFF4 File Offset: 0x0020A1F4
		private void UpdateInput()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			string defaultResult = "MAP SEARCH : ";
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH", out text, defaultResult))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_MAP_SEARCH]");
			}
			text += SharedBlocksTerminal.MapIDToDisplayedString(this.currentMapCode);
			this.inputText.text = text;
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x0020C046 File Offset: 0x0020A246
		public void SetMapCode(string mapCode)
		{
			if (mapCode == null)
			{
				this.currentMapCode = string.Empty;
			}
			else
			{
				this.currentMapCode = mapCode;
			}
			this.UpdateInput();
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x0020C065 File Offset: 0x0020A265
		public void SetInputTextEnabled(bool enabled)
		{
			if (enabled)
			{
				this.inputText.color = Color.white;
				return;
			}
			this.inputText.color = Color.gray;
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x0020C08C File Offset: 0x0020A28C
		private void DrawScreen()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			this.UpdateInput();
			string str;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_VOTES", out str, "RECENT VOTES"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_VOTES]");
			}
			this.sb.Clear();
			this.sb.Append(str + "\n");
			foreach (string mapID in SharedBlocksManager.GetRecentUpVotes())
			{
				if (SharedBlocksManager.IsMapIDValid(mapID))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(mapID));
					this.sb.Append("\n");
				}
			}
			this.recentList.text = this.sb.ToString();
			if (!LocalisationManager.TryGetKeyForCurrentLocale("SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL", out str, "MY MAPS"))
			{
				Debug.LogError("[LOCALIZATION::BUILDER_SCAN_KIOSK] Failed to get key for SHARE MY BLOCKS SEARCH TERMINAL localization [SHARE_BLOCKS_TERMINAL_SEARCH_MAPS_LABEL]");
			}
			this.sb.Clear();
			this.sb.Append(str + "\n");
			foreach (string mapID2 in SharedBlocksManager.GetLocalMapIDs())
			{
				if (SharedBlocksManager.IsMapIDValid(mapID2))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(mapID2));
					this.sb.Append("\n");
				}
			}
			this.myScanList.text = this.sb.ToString();
		}

		// Token: 0x060065A2 RID: 26018 RVA: 0x0020C224 File Offset: 0x0020A424
		private void RefreshPlayerCounter()
		{
			this.terminal.RefreshLobbyCount();
			this.playerCountText.text = this.terminal.GetLobbyText();
			this.playersInLobbyWarning.gameObject.SetActive(!this.terminal.AreAllPlayersInLobby());
		}

		// Token: 0x060065A3 RID: 26019 RVA: 0x0020C270 File Offset: 0x0020A470
		public void SliceUpdate()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x0020C278 File Offset: 0x0020A478
		public void OnEnable()
		{
			if (!this.updating)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = true;
			}
			this.RefreshPlayerCounter();
			RoomSystem.PlayersChangedEvent += new Action(this.PlayersChangedEvent);
		}

		// Token: 0x060065A5 RID: 26021 RVA: 0x0020C270 File Offset: 0x0020A470
		private void PlayersChangedEvent()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x060065A6 RID: 26022 RVA: 0x0020C2B1 File Offset: 0x0020A4B1
		public void OnDisable()
		{
			if (this.updating)
			{
				GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = false;
			}
			RoomSystem.PlayersChangedEvent -= new Action(this.PlayersChangedEvent);
		}

		// Token: 0x040074CE RID: 29902
		[SerializeField]
		private TMP_Text loadedMap;

		// Token: 0x040074CF RID: 29903
		[SerializeField]
		private TMP_Text inputText;

		// Token: 0x040074D0 RID: 29904
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x040074D1 RID: 29905
		[SerializeField]
		private TMP_Text recentList;

		// Token: 0x040074D2 RID: 29906
		[SerializeField]
		private TMP_Text myScanList;

		// Token: 0x040074D3 RID: 29907
		[SerializeField]
		private TMP_Text playerCountText;

		// Token: 0x040074D4 RID: 29908
		[SerializeField]
		private TMP_Text playersInLobbyWarning;

		// Token: 0x040074D5 RID: 29909
		private string currentMapCode;

		// Token: 0x040074D6 RID: 29910
		private string savedMapCode;

		// Token: 0x040074D7 RID: 29911
		private StringBuilder sb = new StringBuilder();

		// Token: 0x040074D8 RID: 29912
		private bool updating;
	}
}
