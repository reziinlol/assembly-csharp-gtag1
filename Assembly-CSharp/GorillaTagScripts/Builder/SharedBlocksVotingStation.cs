using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FE8 RID: 4072
	public class SharedBlocksVotingStation : MonoBehaviour
	{
		// Token: 0x060065D7 RID: 26071 RVA: 0x0020D448 File Offset: 0x0020B648
		private void Start()
		{
			this.SetupLocalization();
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.table = builderTable;
				this.table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnLoadedMapChanged(this.table.GetCurrentMapID());
			}
			else
			{
				GTDev.LogWarning<string>("No Builder Table found for Voting Station", null);
			}
			base.GetComponentsInChildren<MeshRenderer>(false, this.meshes);
			this.upVoteButton.onPressButton.AddListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.AddListener(new UnityAction(this.OnDownVotePressed));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x060065D8 RID: 26072 RVA: 0x0020D538 File Offset: 0x0020B738
		private void OnDestroy()
		{
			this.upVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnDownVotePressed));
			if (this.table != null)
			{
				this.table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x060065D9 RID: 26073 RVA: 0x0020D5F8 File Offset: 0x0020B7F8
		private void SetupLocalization()
		{
			if (this._statusLocText == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Trying to set up Localization, but [_statusLocText] is NULL");
				return;
			}
			if (this._screenLocText == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Trying to set up Localization, but [_screenLocText] is NULL");
				return;
			}
			string text = "voting-status-index";
			string text2 = "map-name-index";
			string text3 = "map-name";
			this._statusIndexVar = (this._statusLocText.StringReference[text] as IntVariable);
			this._mapDisplayIndexVar = (this._screenLocText.StringReference[text2] as IntVariable);
			this._mapNameVar = (this._screenLocText.StringReference[text3] as StringVariable);
			if (this._statusIndexVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [IntVariable] with var-name [" + text + "]");
			}
			if (this._mapDisplayIndexVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [IntVariable] with var-name [" + text2 + "]");
			}
			if (this._mapNameVar == null)
			{
				Debug.LogError("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] Failed to find [StringVariable] with var-name [" + text3 + "]");
			}
		}

		// Token: 0x060065DA RID: 26074 RVA: 0x0020D6F4 File Offset: 0x0020B8F4
		private void OnZoneChanged()
		{
			bool enabled = ZoneManagement.instance.IsZoneActive(this.tableZone);
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				meshRenderer.enabled = enabled;
			}
		}

		// Token: 0x060065DB RID: 26075 RVA: 0x0020D758 File Offset: 0x0020B958
		private void OnUpVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this._statusIndexVar.Value = 2;
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.upVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, true, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.upVoteButton.pressedMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.upVoteButton.enabled = false;
				this.downVoteButton.enabled = true;
			}
		}

		// Token: 0x060065DC RID: 26076 RVA: 0x0020D814 File Offset: 0x0020BA14
		private void OnDownVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this._statusIndexVar.Value = 2;
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.downVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, false, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.downVoteButton.pressedMaterial;
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = false;
			}
		}

		// Token: 0x060065DD RID: 26077 RVA: 0x0020D8D0 File Offset: 0x0020BAD0
		private void OnVoteResponse(bool success, string message)
		{
			this.voteInProgress = false;
			if (success)
			{
				this._statusIndexVar.Value = 0;
				this.statusText.gameObject.SetActive(true);
			}
			else
			{
				int value;
				if (int.TryParse(message, out value))
				{
					this._statusIndexVar.Value = value;
				}
				else
				{
					this.statusText.text = message;
					Debug.Log("[LOCALIZATION::SHARED_BLOCKS_VOTING_STATION] WARNING: Passing in a non-int value for the [message]. This will not be localized!");
				}
				this.statusText.gameObject.SetActive(true);
				if (!this.loadedMapID.IsNullOrEmpty())
				{
					this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.upVoteButton.enabled = true;
					this.downVoteButton.enabled = true;
				}
			}
			this.clearStatusTime = Time.time + this.clearStatusDelay;
			this.waitingToClearStatus = true;
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x0020D9B2 File Offset: 0x0020BBB2
		private void LateUpdate()
		{
			if (this.waitingToClearStatus && Time.time > this.clearStatusTime)
			{
				this.waitingToClearStatus = false;
				this._statusIndexVar.Value = 2;
				this.statusText.gameObject.SetActive(false);
			}
		}

		// Token: 0x060065DF RID: 26079 RVA: 0x0020D9ED File Offset: 0x0020BBED
		private void OnLoadedMapChanged(string mapID)
		{
			this.loadedMapID = mapID;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x060065E0 RID: 26080 RVA: 0x0020DA0D File Offset: 0x0020BC0D
		private void OnMapCleared()
		{
			this.loadedMapID = null;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x060065E1 RID: 26081 RVA: 0x0020DA30 File Offset: 0x0020BC30
		private void UpdateScreen()
		{
			if (!this.loadedMapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(this.loadedMapID))
			{
				this._mapDisplayIndexVar.Value = 1;
				this._mapNameVar.Value = SharedBlocksTerminal.MapIDToDisplayedString(this.loadedMapID);
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = true;
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				return;
			}
			this._mapDisplayIndexVar.Value = 0;
			this.upVoteButton.enabled = false;
			this.downVoteButton.enabled = false;
			this.upVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
			this.downVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
		}

		// Token: 0x0400752C RID: 29996
		public const int VOTING_STATUS_INDEX_SUCCESS = 0;

		// Token: 0x0400752D RID: 29997
		public const int VOTING_STATUS_INDEX_NOT_LOGGED_IN = 1;

		// Token: 0x0400752E RID: 29998
		public const int VOTING_STATUS_INDEX_EMPTY = 2;

		// Token: 0x0400752F RID: 29999
		private const int MAP_DISPLAY_INDEX_NONE = 0;

		// Token: 0x04007530 RID: 30000
		private const int MAP_DISPLAY_INDEX_NAMED_MAP = 1;

		// Token: 0x04007531 RID: 30001
		[SerializeField]
		private TMP_Text screenText;

		// Token: 0x04007532 RID: 30002
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04007533 RID: 30003
		[SerializeField]
		private GorillaPressableButton upVoteButton;

		// Token: 0x04007534 RID: 30004
		[SerializeField]
		private GorillaPressableButton downVoteButton;

		// Token: 0x04007535 RID: 30005
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04007536 RID: 30006
		[SerializeField]
		private Material buttonDefaultMaterial;

		// Token: 0x04007537 RID: 30007
		[SerializeField]
		private Material buttonDisabledMaterial;

		// Token: 0x04007538 RID: 30008
		[Header("Localization Setup")]
		[SerializeField]
		private LocalizedText _statusLocText;

		// Token: 0x04007539 RID: 30009
		[SerializeField]
		private LocalizedText _screenLocText;

		// Token: 0x0400753A RID: 30010
		private BuilderTable table;

		// Token: 0x0400753B RID: 30011
		private string loadedMapID = string.Empty;

		// Token: 0x0400753C RID: 30012
		private bool voteInProgress;

		// Token: 0x0400753D RID: 30013
		private bool waitingToClearStatus;

		// Token: 0x0400753E RID: 30014
		private float clearStatusTime;

		// Token: 0x0400753F RID: 30015
		private float clearStatusDelay = 2f;

		// Token: 0x04007540 RID: 30016
		private IntVariable _statusIndexVar;

		// Token: 0x04007541 RID: 30017
		private IntVariable _mapDisplayIndexVar;

		// Token: 0x04007542 RID: 30018
		private StringVariable _mapNameVar;

		// Token: 0x04007543 RID: 30019
		private List<MeshRenderer> meshes = new List<MeshRenderer>(12);
	}
}
