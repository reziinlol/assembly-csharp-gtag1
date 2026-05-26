using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000287 RID: 647
public class GrabbyTentacleNetworking : MonoBehaviourPunCallbacks
{
	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06001163 RID: 4451 RVA: 0x0005D7BA File Offset: 0x0005B9BA
	// (set) Token: 0x06001164 RID: 4452 RVA: 0x0005D7C1 File Offset: 0x0005B9C1
	public static GrabbyTentacleNetworking Instance { get; private set; }

	// Token: 0x06001165 RID: 4453 RVA: 0x0005D7CC File Offset: 0x0005B9CC
	private void Awake()
	{
		if (GrabbyTentacleNetworking.Instance != null && GrabbyTentacleNetworking.Instance != this)
		{
			Debug.LogWarning("[GrabbyTentacleNetworking] duplicate instance on " + base.name);
			return;
		}
		GrabbyTentacleNetworking.Instance = this;
		if (this.tablePhotonView == null)
		{
			this.tablePhotonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x0005D829 File Offset: 0x0005BA29
	private void OnDestroy()
	{
		if (GrabbyTentacleNetworking.Instance == this)
		{
			GrabbyTentacleNetworking.Instance = null;
		}
	}

	// Token: 0x06001167 RID: 4455 RVA: 0x0005D83E File Offset: 0x0005BA3E
	public void Register(GrabbyTentacleController controller)
	{
		this.registeredController = controller;
	}

	// Token: 0x06001168 RID: 4456 RVA: 0x0005D847 File Offset: 0x0005BA47
	public void Unregister(GrabbyTentacleController controller)
	{
		if (this.registeredController == controller)
		{
			this.registeredController = null;
		}
	}

	// Token: 0x06001169 RID: 4457 RVA: 0x0005D85E File Offset: 0x0005BA5E
	public void SendGrab(int tentacleIndex, Player targetPlayer)
	{
		if (!PhotonNetwork.IsMasterClient || this.tablePhotonView == null || targetPlayer == null)
		{
			return;
		}
		this.tablePhotonView.RPC("ApplyTargetRPC", RpcTarget.All, new object[]
		{
			tentacleIndex,
			targetPlayer
		});
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x0005D8A0 File Offset: 0x0005BAA0
	[PunRPC]
	public void ApplyTargetRPC(int tentacleIndex, Player targetPlayer, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ApplyTargetRPC");
		if (info.Sender == null || !info.Sender.IsMasterClient)
		{
			return;
		}
		if (targetPlayer == null || this.registeredController == null)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(targetPlayer, out rigContainer) || rigContainer == null)
		{
			return;
		}
		bool isLocalPlayer = targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		this.registeredController.OnGrabReceived(tentacleIndex, rigContainer.Rig, isLocalPlayer);
	}

	// Token: 0x040014C4 RID: 5316
	[SerializeField]
	private PhotonView tablePhotonView;

	// Token: 0x040014C5 RID: 5317
	private GrabbyTentacleController registeredController;
}
