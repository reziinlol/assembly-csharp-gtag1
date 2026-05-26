using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000468 RID: 1128
[RequireComponent(typeof(PhotonView), typeof(NetworkObject))]
[NetworkBehaviourWeaved(0)]
public class NetworkView : NetworkBehaviour, IStateAuthorityChanged, IPublicFacingInterface, IPunOwnershipCallbacks
{
	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06001B57 RID: 6999 RVA: 0x00094B20 File Offset: 0x00092D20
	public bool IsMine
	{
		get
		{
			return this.punView != null && this.punView.IsMine;
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06001B58 RID: 7000 RVA: 0x00094B3D File Offset: 0x00092D3D
	public bool IsValid
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06001B59 RID: 7001 RVA: 0x00094B3D File Offset: 0x00092D3D
	public bool HasView
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001B5A RID: 7002 RVA: 0x00094B4B File Offset: 0x00092D4B
	public bool IsRoomView
	{
		get
		{
			return this.punView.IsRoomView;
		}
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001B5B RID: 7003 RVA: 0x00094B58 File Offset: 0x00092D58
	public PhotonView GetView
	{
		get
		{
			return this.punView;
		}
	}

	// Token: 0x170002DF RID: 735
	// (get) Token: 0x06001B5C RID: 7004 RVA: 0x00094B60 File Offset: 0x00092D60
	public NetPlayer Owner
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.punView.Owner);
		}
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x06001B5D RID: 7005 RVA: 0x00094B77 File Offset: 0x00092D77
	public int ViewID
	{
		get
		{
			return this.punView.ViewID;
		}
	}

	// Token: 0x170002E1 RID: 737
	// (get) Token: 0x06001B5E RID: 7006 RVA: 0x00094B84 File Offset: 0x00092D84
	// (set) Token: 0x06001B5F RID: 7007 RVA: 0x00094B91 File Offset: 0x00092D91
	internal OwnershipOption OwnershipTransfer
	{
		get
		{
			return this.punView.OwnershipTransfer;
		}
		set
		{
			this.punView.OwnershipTransfer = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnershipTransfer = value;
			}
		}
	}

	// Token: 0x170002E2 RID: 738
	// (get) Token: 0x06001B60 RID: 7008 RVA: 0x00094BB9 File Offset: 0x00092DB9
	// (set) Token: 0x06001B61 RID: 7009 RVA: 0x00094BC6 File Offset: 0x00092DC6
	public int OwnerActorNr
	{
		get
		{
			return this.punView.OwnerActorNr;
		}
		set
		{
			this.punView.OwnerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnerActorNr = value;
			}
		}
	}

	// Token: 0x170002E3 RID: 739
	// (get) Token: 0x06001B62 RID: 7010 RVA: 0x00094BEE File Offset: 0x00092DEE
	// (set) Token: 0x06001B63 RID: 7011 RVA: 0x00094BFB File Offset: 0x00092DFB
	public int ControllerActorNr
	{
		get
		{
			return this.punView.ControllerActorNr;
		}
		set
		{
			this.punView.ControllerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.ControllerActorNr = value;
			}
		}
	}

	// Token: 0x06001B64 RID: 7012 RVA: 0x00094C24 File Offset: 0x00092E24
	private void GetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		if (components.Length > 1)
		{
			if (components[0].Synchronization == ViewSynchronization.UnreliableOnChange)
			{
				this.punView = components[0];
				this.reliableView = components[1];
			}
			else if (components[0].Synchronization == ViewSynchronization.ReliableDeltaCompressed)
			{
				this.reliableView = components[0];
				this.punView = components[1];
			}
		}
		else
		{
			this.punView = components[0];
		}
		if (this.punView == null)
		{
			this.punView = base.GetComponent<PhotonView>();
		}
		if (this.fusionView == null)
		{
			this.fusionView = base.GetComponent<NetworkObject>();
		}
	}

	// Token: 0x06001B65 RID: 7013 RVA: 0x00094CB9 File Offset: 0x00092EB9
	protected virtual void Awake()
	{
		this.GetViews();
	}

	// Token: 0x06001B66 RID: 7014 RVA: 0x00094CC1 File Offset: 0x00092EC1
	protected virtual void Start()
	{
		if (this._sceneObject)
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		}
	}

	// Token: 0x06001B67 RID: 7015 RVA: 0x00094CDC File Offset: 0x00092EDC
	public void SendRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Player playerRef = (targetPlayer as PunNetPlayer).PlayerRef;
		this.punView.RPC(method, playerRef, parameters);
	}

	// Token: 0x06001B68 RID: 7016 RVA: 0x00094D03 File Offset: 0x00092F03
	public void SendRPC(string method, RpcTarget target, params object[] parameters)
	{
		this.punView.RPC(method, target, parameters);
	}

	// Token: 0x06001B69 RID: 7017 RVA: 0x00094D14 File Offset: 0x00092F14
	public void SendRPC(string method, int target, params object[] parameters)
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null || !currentRoom.Players.ContainsKey(target))
		{
			return;
		}
		this.punView.RPC(method, currentRoom.Players[target], parameters);
	}

	// Token: 0x06001B6A RID: 7018 RVA: 0x00094D52 File Offset: 0x00092F52
	public override void Spawned()
	{
		base.Spawned();
		this._spawned = true;
	}

	// Token: 0x06001B6B RID: 7019 RVA: 0x00094D61 File Offset: 0x00092F61
	public void RequestOwnership()
	{
		this.GetView.RequestOwnership();
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x00094D6E File Offset: 0x00092F6E
	public void ReleaseOwnership()
	{
		this.changingStatAuth = true;
		base.Object.ReleaseStateAuthority();
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x00094D82 File Offset: 0x00092F82
	public virtual void StateAuthorityChanged()
	{
		if (this.changingStatAuth)
		{
			this.changingStatAuth = false;
		}
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x000028C5 File Offset: 0x00000AC5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000028C5 File Offset: 0x00000AC5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x0400258B RID: 9611
	[SerializeField]
	private PhotonView punView;

	// Token: 0x0400258C RID: 9612
	[SerializeField]
	private PhotonView reliableView;

	// Token: 0x0400258D RID: 9613
	[SerializeField]
	internal NetworkObject fusionView;

	// Token: 0x0400258E RID: 9614
	[SerializeField]
	protected bool _sceneObject;

	// Token: 0x0400258F RID: 9615
	private bool _spawned;

	// Token: 0x04002590 RID: 9616
	private bool changingStatAuth;
}
