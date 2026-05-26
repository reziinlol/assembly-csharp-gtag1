using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000F60 RID: 3936
	public class CustomMapsRoomMapDisplay : MonoBehaviour
	{
		// Token: 0x06006212 RID: 25106 RVA: 0x001FA750 File Offset: 0x001F8950
		public void Start()
		{
			this.roomMapNameText.text = this.noRoomMapString;
			this.roomMapStatusText.text = this.notLoadedStatusString;
			this.roomMapLabelText.gameObject.SetActive(true);
			this.roomMapNameText.gameObject.SetActive(true);
			this.roomMapStatusLabelText.gameObject.SetActive(false);
			this.roomMapStatusText.gameObject.SetActive(false);
			NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
			CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
			CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		}

		// Token: 0x06006213 RID: 25107 RVA: 0x001FA848 File Offset: 0x001F8A48
		public void OnDestroy()
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnectedFromRoom;
			CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		}

		// Token: 0x06006214 RID: 25108 RVA: 0x001FA8AD File Offset: 0x001F8AAD
		private void OnJoinedRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06006215 RID: 25109 RVA: 0x001FA8AD File Offset: 0x001F8AAD
		private void OnDisconnectedFromRoom()
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06006216 RID: 25110 RVA: 0x001FA8AD File Offset: 0x001F8AAD
		private void OnRoomMapChanged(ModId roomMapModId)
		{
			this.UpdateRoomMap();
		}

		// Token: 0x06006217 RID: 25111 RVA: 0x001FA8B8 File Offset: 0x001F8AB8
		private Task UpdateRoomMap()
		{
			CustomMapsRoomMapDisplay.<UpdateRoomMap>d__18 <UpdateRoomMap>d__;
			<UpdateRoomMap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateRoomMap>d__.<>4__this = this;
			<UpdateRoomMap>d__.<>1__state = -1;
			<UpdateRoomMap>d__.<>t__builder.Start<CustomMapsRoomMapDisplay.<UpdateRoomMap>d__18>(ref <UpdateRoomMap>d__);
			return <UpdateRoomMap>d__.<>t__builder.Task;
		}

		// Token: 0x06006218 RID: 25112 RVA: 0x001FA8FC File Offset: 0x001F8AFC
		private void OnMapLoadComplete(bool success)
		{
			if (success)
			{
				this.roomMapStatusText.text = this.readyToPlayStatusString;
				this.roomMapStatusText.color = this.readyToPlayStatusStringColor;
				return;
			}
			this.roomMapStatusText.text = this.loadFailedStatusString;
			this.roomMapStatusText.color = this.loadFailedStatusStringColor;
		}

		// Token: 0x06006219 RID: 25113 RVA: 0x001FA951 File Offset: 0x001F8B51
		private void OnMapLoadProgress(MapLoadStatus status, int progress, string message)
		{
			if (status - MapLoadStatus.Downloading <= 1)
			{
				this.roomMapStatusText.text = this.loadingStatusString;
				this.roomMapStatusText.color = this.loadingStatusStringColor;
			}
		}

		// Token: 0x040070D8 RID: 28888
		[SerializeField]
		private TMP_Text roomMapLabelText;

		// Token: 0x040070D9 RID: 28889
		[SerializeField]
		private TMP_Text roomMapNameText;

		// Token: 0x040070DA RID: 28890
		[SerializeField]
		private TMP_Text roomMapStatusLabelText;

		// Token: 0x040070DB RID: 28891
		[SerializeField]
		private TMP_Text roomMapStatusText;

		// Token: 0x040070DC RID: 28892
		[SerializeField]
		private string noRoomMapString = "NONE";

		// Token: 0x040070DD RID: 28893
		[SerializeField]
		private string notLoadedStatusString = "NOT LOADED";

		// Token: 0x040070DE RID: 28894
		[SerializeField]
		private string loadingStatusString = "LOADING...";

		// Token: 0x040070DF RID: 28895
		[SerializeField]
		private string readyToPlayStatusString = "READY!";

		// Token: 0x040070E0 RID: 28896
		[SerializeField]
		private string loadFailedStatusString = "LOAD FAILED";

		// Token: 0x040070E1 RID: 28897
		[SerializeField]
		private Color notLoadedStatusStringColor = Color.red;

		// Token: 0x040070E2 RID: 28898
		[SerializeField]
		private Color loadingStatusStringColor = Color.yellow;

		// Token: 0x040070E3 RID: 28899
		[SerializeField]
		private Color readyToPlayStatusStringColor = Color.green;

		// Token: 0x040070E4 RID: 28900
		[SerializeField]
		private Color loadFailedStatusStringColor = Color.red;
	}
}
