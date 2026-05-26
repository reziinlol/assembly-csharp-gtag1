using System;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000454 RID: 1108
[NetworkBehaviourWeaved(0)]
public abstract class NetworkComponent : NetworkView, IPunObservable, IStateAuthorityChanged, IPublicFacingInterface, IOnPhotonViewOwnerChange, IPhotonViewCallback, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	// Token: 0x06001A81 RID: 6785 RVA: 0x00093BFB File Offset: 0x00091DFB
	internal virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.AddToNetwork();
	}

	// Token: 0x06001A82 RID: 6786 RVA: 0x00093C09 File Offset: 0x00091E09
	internal virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06001A83 RID: 6787 RVA: 0x00093C17 File Offset: 0x00091E17
	protected override void Start()
	{
		base.Start();
		this.AddToNetwork();
	}

	// Token: 0x06001A84 RID: 6788 RVA: 0x00093C25 File Offset: 0x00091E25
	private void AddToNetwork()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06001A85 RID: 6789 RVA: 0x00093C2D File Offset: 0x00091E2D
	public override void Spawned()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnSpawned();
		}
	}

	// Token: 0x06001A86 RID: 6790 RVA: 0x00093C41 File Offset: 0x00091E41
	public override void FixedUpdateNetwork()
	{
		this.WriteDataFusion();
	}

	// Token: 0x06001A87 RID: 6791 RVA: 0x00093C49 File Offset: 0x00091E49
	public override void Render()
	{
		if (!base.HasStateAuthority)
		{
			this.ReadDataFusion();
		}
	}

	// Token: 0x06001A88 RID: 6792
	public abstract void WriteDataFusion();

	// Token: 0x06001A89 RID: 6793
	public abstract void ReadDataFusion();

	// Token: 0x06001A8A RID: 6794 RVA: 0x00093C59 File Offset: 0x00091E59
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.OnSpawned();
	}

	// Token: 0x06001A8B RID: 6795 RVA: 0x00093C61 File Offset: 0x00091E61
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			this.WriteDataPUN(stream, info);
			return;
		}
		if (stream.IsReading)
		{
			this.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06001A8C RID: 6796
	protected abstract void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001A8D RID: 6797
	protected abstract void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001A8E RID: 6798 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnSpawned()
	{
	}

	// Token: 0x06001A8F RID: 6799 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
	}

	// Token: 0x06001A90 RID: 6800 RVA: 0x00093C84 File Offset: 0x00091E84
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(newMasterClient));
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x00093C98 File Offset: 0x00091E98
	public override void StateAuthorityChanged()
	{
		base.StateAuthorityChanged();
		if (base.Object == null)
		{
			return;
		}
		if (base.Object.StateAuthority == default(PlayerRef))
		{
			return;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
			return;
		}
		this.OnOwnerSwitched(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x00093D0E File Offset: 0x00091F0E
	public void OnMasterClientSwitch(NetPlayer newMaster)
	{
		this.StateAuthorityChanged();
	}

	// Token: 0x06001A93 RID: 6803 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnOwnerChange(Player newOwner, Player previousOwner)
	{
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06001A98 RID: 6808 RVA: 0x00093D16 File Offset: 0x00091F16
	public bool IsLocallyOwned
	{
		get
		{
			return base.IsMine;
		}
	}

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06001A99 RID: 6809 RVA: 0x00093D1E File Offset: 0x00091F1E
	public bool ShouldWriteObjectData
	{
		get
		{
			return NetworkSystem.Instance.ShouldWriteObjectData(base.gameObject);
		}
	}

	// Token: 0x170002B2 RID: 690
	// (get) Token: 0x06001A9A RID: 6810 RVA: 0x00093D30 File Offset: 0x00091F30
	public bool ShouldUpdateobject
	{
		get
		{
			return NetworkSystem.Instance.ShouldUpdateObject(base.gameObject);
		}
	}

	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x06001A9B RID: 6811 RVA: 0x00093D42 File Offset: 0x00091F42
	public int OwnerID
	{
		get
		{
			return NetworkSystem.Instance.GetOwningPlayerID(base.gameObject);
		}
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x00093D5C File Offset: 0x00091F5C
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x00093D68 File Offset: 0x00091F68
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
