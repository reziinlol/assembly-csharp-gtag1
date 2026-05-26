using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F4E RID: 3918
	public class CustomMapModeSelector : GameModeSelectorButtonLayout
	{
		// Token: 0x060061C6 RID: 25030 RVA: 0x001F883A File Offset: 0x001F6A3A
		private void Awake()
		{
			CustomMapModeSelector.instances.AddIfNew(this);
		}

		// Token: 0x060061C7 RID: 25031 RVA: 0x001F8848 File Offset: 0x001F6A48
		public void OnEnable()
		{
			if (GorillaComputer.instance != null)
			{
				this.SetupButtons();
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
			}
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnRoomHostSwitched;
			NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnDisconnected;
			this.roomHostDescriptionText.SetActive(false);
			this.roomHostText.gameObject.SetActive(false);
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
			{
				this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x001F8928 File Offset: 0x001F6B28
		public void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnRoomHostSwitched;
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnDisconnected;
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x001F8992 File Offset: 0x001F6B92
		private void OnJoinedRoom()
		{
			this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x001F89A4 File Offset: 0x001F6BA4
		private void OnRoomHostSwitched(NetPlayer newRoomHost)
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate)
			{
				return;
			}
			CustomMapModeSelector.reusableString = this.notInRoomHostString;
			if (!newRoomHost.IsNull)
			{
				this.roomHostDescriptionText.SetActive(true);
				CustomMapModeSelector.reusableString = newRoomHost.DefaultName;
				if (GorillaComputer.instance.NametagsEnabled && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
				{
					RigContainer rigContainer;
					if (newRoomHost.IsLocal)
					{
						CustomMapModeSelector.reusableString = newRoomHost.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(newRoomHost, out rigContainer))
					{
						CustomMapModeSelector.reusableString = rigContainer.Rig.playerNameVisible;
					}
				}
			}
			this.roomHostText.text = this.roomHostLabel + CustomMapModeSelector.reusableString;
			this.roomHostText.gameObject.SetActive(true);
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x001F8A6A File Offset: 0x001F6C6A
		private void OnDisconnected()
		{
			this.roomHostText.gameObject.SetActive(false);
			this.roomHostDescriptionText.SetActive(false);
		}

		// Token: 0x060061CC RID: 25036 RVA: 0x001F8A8C File Offset: 0x001F6C8C
		public static void ResetButtons()
		{
			CustomMapModeSelector.gamemodes = new List<GameModeType>
			{
				GameModeType.Casual
			};
			CustomMapModeSelector.defaultGamemodeForLoadedMap = GameModeType.Casual;
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.SetupButtons();
			}
			GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
		}

		// Token: 0x060061CD RID: 25037 RVA: 0x001F8B10 File Offset: 0x001F6D10
		public static void SetAvailableGameModes(int[] availableModes, int defaultMode)
		{
			CustomMapModeSelector.gamemodes.Clear();
			CustomMapModeSelector.gamemodes.Add(GameModeType.Casual);
			if (availableModes != null)
			{
				foreach (int item in availableModes)
				{
					CustomMapModeSelector.gamemodes.Add((GameModeType)item);
				}
			}
			CustomMapModeSelector.defaultGamemodeForLoadedMap = (GameModeType)defaultMode;
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.SetupButtons();
			}
			GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
		}

		// Token: 0x060061CE RID: 25038 RVA: 0x001F8BB8 File Offset: 0x001F6DB8
		protected override void SetupButtons()
		{
			CustomMapModeSelector.<SetupButtons>d__16 <SetupButtons>d__;
			<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SetupButtons>d__.<>4__this = this;
			<SetupButtons>d__.<>1__state = -1;
			<SetupButtons>d__.<>t__builder.Start<CustomMapModeSelector.<SetupButtons>d__16>(ref <SetupButtons>d__);
		}

		// Token: 0x060061CF RID: 25039 RVA: 0x001F8BF0 File Offset: 0x001F6DF0
		public static void RefreshHostName()
		{
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

		// Token: 0x0400705B RID: 28763
		[SerializeField]
		private TMP_Text roomHostText;

		// Token: 0x0400705C RID: 28764
		[SerializeField]
		private GameObject roomHostDescriptionText;

		// Token: 0x0400705D RID: 28765
		[SerializeField]
		private string notInRoomHostString = "-NOT IN ROOM-";

		// Token: 0x0400705E RID: 28766
		[SerializeField]
		private string roomHostLabel = "ROOM HOST: ";

		// Token: 0x0400705F RID: 28767
		private static List<GameModeType> gamemodes = new List<GameModeType>
		{
			GameModeType.Casual
		};

		// Token: 0x04007060 RID: 28768
		private static GameModeType defaultGamemodeForLoadedMap = GameModeType.Casual;

		// Token: 0x04007061 RID: 28769
		private static List<CustomMapModeSelector> instances = new List<CustomMapModeSelector>();

		// Token: 0x04007062 RID: 28770
		private static string reusableString = "";
	}
}
