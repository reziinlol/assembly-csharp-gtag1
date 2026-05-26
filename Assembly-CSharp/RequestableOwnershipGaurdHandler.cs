using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fusion;
using Fusion.Sockets;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x0200038D RID: 909
internal class RequestableOwnershipGaurdHandler : IPunOwnershipCallbacks, IInRoomCallbacks, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x060015EF RID: 5615 RVA: 0x00080068 File Offset: 0x0007E268
	static RequestableOwnershipGaurdHandler()
	{
		PhotonNetwork.AddCallbackTarget(RequestableOwnershipGaurdHandler.callbackInstance);
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x00080092 File Offset: 0x0007E292
	internal static void RegisterView(NetworkView view, RequestableOwnershipGuard guard)
	{
		if (view == null || RequestableOwnershipGaurdHandler.gaurdedViews.Contains(view))
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Add(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Add(view, guard);
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x000800C3 File Offset: 0x0007E2C3
	internal static void RemoveView(NetworkView view)
	{
		if (view == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.gaurdedViews.Remove(view);
		RequestableOwnershipGaurdHandler.guardingLookup.Remove(view);
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000800E8 File Offset: 0x0007E2E8
	internal static void RegisterViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RegisterView(views[i], guard);
		}
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x00080110 File Offset: 0x0007E310
	public static void RemoveViews(NetworkView[] views, RequestableOwnershipGuard guard)
	{
		for (int i = 0; i < views.Length; i++)
		{
			RequestableOwnershipGaurdHandler.RemoveView(views[i]);
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x00080138 File Offset: 0x0007E338
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		NetworkView networkView = RequestableOwnershipGaurdHandler.gaurdedViews.FirstOrDefault((NetworkView p) => p.GetView == targetView);
		RequestableOwnershipGuard requestableOwnershipGuard;
		if (networkView.IsNull() || !RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard) || requestableOwnershipGuard.IsNull())
		{
			return;
		}
		NetPlayer currentOwner = requestableOwnershipGuard.currentOwner;
		Player player = (currentOwner != null) ? currentOwner.GetPlayerRef() : null;
		int num = (player != null) ? player.ActorNumber : 0;
		if (num == 0 || previousOwner != player)
		{
			GTDev.LogWarning<string>("Ownership transferred but the previous owner didn't initiate the request, Switching back", null);
			targetView.OwnerActorNr = num;
			targetView.ControllerActorNr = num;
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x000801D7 File Offset: 0x0007E3D7
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x000801D7 File Offset: 0x0007E3D7
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.OnHostChangedShared();
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000801E0 File Offset: 0x0007E3E0
	private void OnHostChangedShared()
	{
		foreach (NetworkView networkView in RequestableOwnershipGaurdHandler.gaurdedViews)
		{
			RequestableOwnershipGuard requestableOwnershipGuard;
			if (!RequestableOwnershipGaurdHandler.guardingLookup.TryGetValue(networkView, out requestableOwnershipGuard))
			{
				break;
			}
			if (networkView.Owner != null && requestableOwnershipGuard.currentOwner != null && !object.Equals(networkView.Owner, requestableOwnershipGuard.currentOwner))
			{
				networkView.OwnerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
				networkView.ControllerActorNr = requestableOwnershipGuard.currentOwner.ActorNumber;
			}
		}
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x04002040 RID: 8256
	private static HashSet<NetworkView> gaurdedViews = new HashSet<NetworkView>();

	// Token: 0x04002041 RID: 8257
	private static readonly RequestableOwnershipGaurdHandler callbackInstance = new RequestableOwnershipGaurdHandler();

	// Token: 0x04002042 RID: 8258
	private static Dictionary<NetworkView, RequestableOwnershipGuard> guardingLookup = new Dictionary<NetworkView, RequestableOwnershipGuard>();
}
