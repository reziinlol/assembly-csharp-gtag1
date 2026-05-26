using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;

// Token: 0x02000434 RID: 1076
public class FusionRegionCrawler : MonoBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x1700028A RID: 650
	// (get) Token: 0x060019A7 RID: 6567 RVA: 0x0008F740 File Offset: 0x0008D940
	public int PlayerCountGlobal
	{
		get
		{
			return this.globalPlayerCount;
		}
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0008F748 File Offset: 0x0008D948
	public void Start()
	{
		this.regionRunner = base.gameObject.AddComponent<NetworkRunner>();
		this.regionRunner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
		base.StartCoroutine(this.OccasionalUpdate());
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x0008F77D File Offset: 0x0008D97D
	public IEnumerator OccasionalUpdate()
	{
		while (this.refreshPlayerCountAutomatically)
		{
			yield return this.UpdatePlayerCount();
			yield return new WaitForSeconds(this.UpdateFrequency);
		}
		yield break;
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x0008F78C File Offset: 0x0008D98C
	public IEnumerator UpdatePlayerCount()
	{
		int tempGlobalPlayerCount = 0;
		StartGameArgs startGameArgs = default(StartGameArgs);
		foreach (string fixedRegion in NetworkSystem.Instance.regionNames)
		{
			startGameArgs.CustomPhotonAppSettings = new FusionAppSettings();
			startGameArgs.CustomPhotonAppSettings.FixedRegion = fixedRegion;
			this.waitingForSessionListUpdate = true;
			this.regionRunner.JoinSessionLobby(SessionLobby.ClientServer, startGameArgs.CustomPhotonAppSettings.FixedRegion, null, null, new bool?(false), default(CancellationToken), true);
			while (this.waitingForSessionListUpdate)
			{
				yield return new WaitForEndOfFrame();
			}
			foreach (SessionInfo sessionInfo in this.sessionInfoCache)
			{
				tempGlobalPlayerCount += sessionInfo.PlayerCount;
			}
			tempGlobalPlayerCount += this.tempSessionPlayerCount;
		}
		string[] array = null;
		this.globalPlayerCount = tempGlobalPlayerCount;
		FusionRegionCrawler.PlayerCountUpdated onPlayerCountUpdated = this.OnPlayerCountUpdated;
		if (onPlayerCountUpdated != null)
		{
			onPlayerCountUpdated(this.globalPlayerCount);
		}
		yield break;
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x0008F79B File Offset: 0x0008D99B
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		if (this.waitingForSessionListUpdate)
		{
			this.sessionInfoCache = sessionList;
			this.waitingForSessionListUpdate = false;
		}
	}

	// Token: 0x060019AC RID: 6572 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x060019AD RID: 6573 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
	}

	// Token: 0x060019AE RID: 6574 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	// Token: 0x060019AF RID: 6575 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x060019B0 RID: 6576 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	// Token: 0x060019B1 RID: 6577 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	// Token: 0x060019B2 RID: 6578 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x060019B3 RID: 6579 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	// Token: 0x060019B4 RID: 6580 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x060019B5 RID: 6581 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x060019B6 RID: 6582 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	// Token: 0x060019B7 RID: 6583 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	// Token: 0x060019B8 RID: 6584 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x060019B9 RID: 6585 RVA: 0x000028C5 File Offset: 0x00000AC5
	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x060019BA RID: 6586 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060019BB RID: 6587 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x060019BC RID: 6588 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x0400245E RID: 9310
	public FusionRegionCrawler.PlayerCountUpdated OnPlayerCountUpdated;

	// Token: 0x0400245F RID: 9311
	private NetworkRunner regionRunner;

	// Token: 0x04002460 RID: 9312
	private List<SessionInfo> sessionInfoCache;

	// Token: 0x04002461 RID: 9313
	private bool waitingForSessionListUpdate;

	// Token: 0x04002462 RID: 9314
	private int globalPlayerCount;

	// Token: 0x04002463 RID: 9315
	private float UpdateFrequency = 10f;

	// Token: 0x04002464 RID: 9316
	private bool refreshPlayerCountAutomatically = true;

	// Token: 0x04002465 RID: 9317
	private int tempSessionPlayerCount;

	// Token: 0x02000435 RID: 1077
	// (Invoke) Token: 0x060019C1 RID: 6593
	public delegate void PlayerCountUpdated(int playerCount);
}
