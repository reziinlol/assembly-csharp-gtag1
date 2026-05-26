using System;
using System.Collections;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000AAC RID: 2732
internal class VirtualStumpSerializer : GorillaSerializer
{
	// Token: 0x17000669 RID: 1641
	// (get) Token: 0x060045CA RID: 17866 RVA: 0x0012ACB9 File Offset: 0x00128EB9
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x060045CB RID: 17867 RVA: 0x00179B88 File Offset: 0x00177D88
	protected void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x060045CC RID: 17868 RVA: 0x00179BF8 File Offset: 0x00177DF8
	private void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		int driverID = CustomMapsTerminal.GetDriverID();
		if (leavingPlayer.ActorNumber == driverID)
		{
			CustomMapsTerminal.SetTerminalControlStatus(false, -2, true);
		}
	}

	// Token: 0x060045CD RID: 17869 RVA: 0x00179C2A File Offset: 0x00177E2A
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			VirtualStumpSerializer.roomInitialized = true;
			return;
		}
		Debug.Log("[VirtualStumpSerializer::OnJoinedRoom] Requesting Room Initialization...");
		VirtualStumpSerializer.waitingForRoomInitialization = true;
		base.SendRPC("RequestRoomInitialization_RPC", false, Array.Empty<object>());
	}

	// Token: 0x060045CE RID: 17870 RVA: 0x00179C60 File Offset: 0x00177E60
	private void OnLeftRoom()
	{
		Debug.Log("[VirtualStumpSerializer::OnLeftRoom]...");
		VirtualStumpSerializer.roomInitialized = false;
	}

	// Token: 0x060045CF RID: 17871 RVA: 0x00179C72 File Offset: 0x00177E72
	public static bool IsWaitingForRoomInit()
	{
		return VirtualStumpSerializer.waitingForRoomInitialization || !VirtualStumpSerializer.roomInitialized;
	}

	// Token: 0x060045D0 RID: 17872 RVA: 0x00179C88 File Offset: 0x00177E88
	[PunRPC]
	private void RequestRoomInitialization_RPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestRoomInitialization_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player.CheckSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization))
		{
			return;
		}
		player.ReceivedSingleCallRPC(NetPlayer.SingleCallRPC.CMS_RequestRoomInitialization);
		long id = CustomMapManager.GetRoomMapId()._id;
		base.SendRPC("InitializeRoom_RPC", info.Sender, new object[]
		{
			CustomMapsTerminal.CurrentScreen,
			CustomMapsTerminal.GetDriverID(),
			CustomMapsTerminal.LocalModDetailsID,
			id
		});
	}

	// Token: 0x060045D1 RID: 17873 RVA: 0x00179D24 File Offset: 0x00177F24
	[PunRPC]
	private void InitializeRoom_RPC(int currentScreen, int driverID, long modDetailsID, long loadedMapModID, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "InitializeRoom_RPC");
		if (!info.Sender.IsMasterClient || !VirtualStumpSerializer.waitingForRoomInitialization)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		CustomMapsTerminal.UpdateFromDriver(currentScreen, modDetailsID, driverID);
		if (loadedMapModID > 0L)
		{
			CustomMapManager.SetRoomMap(loadedMapModID);
		}
		VirtualStumpSerializer.roomInitialized = true;
		VirtualStumpSerializer.waitingForRoomInitialization = false;
		Debug.Log("[VStumpSerializer.InitializeRPC] Room initialization finished.");
	}

	// Token: 0x060045D2 RID: 17874 RVA: 0x00179D94 File Offset: 0x00177F94
	public void LoadMapSynced(long modId)
	{
		CustomMapManager.SetRoomMap(modId);
		CustomMapManager.LoadMap(new ModId(modId));
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("SetRoomMap_RPC", true, new object[]
			{
				modId
			});
		}
	}

	// Token: 0x060045D3 RID: 17875 RVA: 0x00179DE6 File Offset: 0x00177FE6
	public void UnloadMapSynced()
	{
		CustomMapManager.UnloadMap(true);
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			base.SendRPC("UnloadMap_RPC", true, Array.Empty<object>());
		}
	}

	// Token: 0x060045D4 RID: 17876 RVA: 0x00179E18 File Offset: 0x00178018
	[PunRPC]
	private void SetRoomMap_RPC(long modId, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SetRoomMap_RPC");
		if (modId <= 0L)
		{
			return;
		}
		if (info.Sender.ActorNumber != this.photonView.OwnerActorNr && info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (modId != this.detailsScreen.currentMapMod.Id._id)
		{
			return;
		}
		CustomMapManager.SetRoomMap(modId);
	}

	// Token: 0x060045D5 RID: 17877 RVA: 0x00179E80 File Offset: 0x00178080
	[PunRPC]
	private void UnloadMap_RPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "UnloadMap_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		if (!CustomMapManager.AreAllPlayersInVirtualStump())
		{
			return;
		}
		CustomMapManager.UnloadMap(true);
	}

	// Token: 0x060045D6 RID: 17878 RVA: 0x00179EAF File Offset: 0x001780AF
	public void RequestTerminalControlStatusChange(bool lockedStatus)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RequestTerminalControlStatusChange_RPC", false, new object[]
		{
			lockedStatus
		});
	}

	// Token: 0x060045D7 RID: 17879 RVA: 0x00179EDC File Offset: 0x001780DC
	[PunRPC]
	private void RequestTerminalControlStatusChange_RPC(bool lockedStatus, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestTerminalControlStatusChange_RPC");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[19].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		if (!player.IsNull && CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(lockedStatus, info.Sender.ActorNumber);
		}
	}

	// Token: 0x060045D8 RID: 17880 RVA: 0x00179F71 File Offset: 0x00178171
	public void SetTerminalControlStatus(bool locked, int playerID)
	{
		if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		base.SendRPC("SetTerminalControlStatus_RPC", true, new object[]
		{
			locked,
			playerID
		});
	}

	// Token: 0x060045D9 RID: 17881 RVA: 0x00179FB0 File Offset: 0x001781B0
	[PunRPC]
	private void SetTerminalControlStatus_RPC(bool locked, int driverID, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SetTerminalControlStatus_RPC");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (driverID != -2 && NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[16].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(locked, driverID, false);
	}

	// Token: 0x060045DA RID: 17882 RVA: 0x0017A036 File Offset: 0x00178236
	public void SendTerminalStatus()
	{
		if (!NetworkSystem.Instance.InRoom || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.statusUpdateCoroutine != null)
		{
			base.StopCoroutine(this.statusUpdateCoroutine);
		}
		this.statusUpdateCoroutine = base.StartCoroutine(this.WaitToSendStatus());
	}

	// Token: 0x060045DB RID: 17883 RVA: 0x0017A072 File Offset: 0x00178272
	private IEnumerator WaitToSendStatus()
	{
		yield return new WaitForSeconds(0.5f);
		base.SendRPC("UpdateScreen_RPC", true, new object[]
		{
			CustomMapsTerminal.CurrentScreen,
			CustomMapsTerminal.LocalModDetailsID,
			CustomMapsTerminal.GetDriverID()
		});
		yield break;
	}

	// Token: 0x060045DC RID: 17884 RVA: 0x0017A084 File Offset: 0x00178284
	[PunRPC]
	private void UpdateScreen_RPC(int currentScreen, long modDetailsID, int driverID, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "UpdateScreen_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID() || !CustomMapManager.IsRemotePlayerInVirtualStump(info.Sender.UserId))
		{
			return;
		}
		if (currentScreen < -1 || currentScreen > 6)
		{
			return;
		}
		if (NetworkSystem.Instance.GetPlayer(driverID) == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[17].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.UpdateFromDriver(currentScreen, modDetailsID, driverID);
	}

	// Token: 0x060045DD RID: 17885 RVA: 0x0017A129 File Offset: 0x00178329
	public void RefreshDriverNickName()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		base.SendRPC("RefreshDriverNickName_RPC", true, Array.Empty<object>());
	}

	// Token: 0x060045DE RID: 17886 RVA: 0x0017A14C File Offset: 0x0017834C
	[PunRPC]
	private void RefreshDriverNickName_RPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RefreshDriverNickName_RPC");
		if (info.Sender.ActorNumber != CustomMapsTerminal.GetDriverID())
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[18].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
	}

	// Token: 0x04005842 RID: 22594
	[SerializeField]
	private VirtualStumpBarrierSFX barrierSFX;

	// Token: 0x04005843 RID: 22595
	[SerializeField]
	private CustomMapsDisplayScreen detailsScreen;

	// Token: 0x04005844 RID: 22596
	private static bool waitingForRoomInitialization;

	// Token: 0x04005845 RID: 22597
	private static bool roomInitialized;

	// Token: 0x04005846 RID: 22598
	private bool sendModList;

	// Token: 0x04005847 RID: 22599
	private bool forceNewSearch;

	// Token: 0x04005848 RID: 22600
	private bool waitToSendStatus;

	// Token: 0x04005849 RID: 22601
	private bool sendNewStatus;

	// Token: 0x0400584A RID: 22602
	private const float STATUS_UPDATE_INTERVAL = 0.5f;

	// Token: 0x0400584B RID: 22603
	private Coroutine statusUpdateCoroutine;
}
