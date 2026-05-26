using System;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000835 RID: 2101
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x06003602 RID: 13826 RVA: 0x0012AE1C File Offset: 0x0012901C
	public NetworkView NetView
	{
		get
		{
			return this.netView;
		}
	}

	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x06003603 RID: 13827 RVA: 0x0012AE24 File Offset: 0x00129024
	// (set) Token: 0x06003604 RID: 13828 RVA: 0x0012AE2C File Offset: 0x0012902C
	protected virtual object data { get; set; }

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x06003605 RID: 13829 RVA: 0x0012AE35 File Offset: 0x00129035
	public bool IsLocallyOwned
	{
		get
		{
			return this.netView.IsMine;
		}
	}

	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x06003606 RID: 13830 RVA: 0x0012AE42 File Offset: 0x00129042
	public bool IsValid
	{
		get
		{
			return this.netView.IsValid;
		}
	}

	// Token: 0x06003607 RID: 13831 RVA: 0x0012AE4F File Offset: 0x0012904F
	private void Awake()
	{
		if (this.netView == null)
		{
			this.netView = base.GetComponent<NetworkView>();
		}
	}

	// Token: 0x06003608 RID: 13832 RVA: 0x0012AE6C File Offset: 0x0012906C
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.netView == null || !this.netView.IsValid)
		{
			return;
		}
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x06003609 RID: 13833 RVA: 0x0012AEA4 File Offset: 0x001290A4
	public override void Spawned()
	{
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(base.Object.StateAuthority.PlayerId, base.Runner.Tick.Raw);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x0012AEE4 File Offset: 0x001290E4
	private void ProcessSpawn(PhotonMessageInfoWrapped wrappedInfo)
	{
		this.successfullInstantiate = this.OnSpawnSetupCheck(wrappedInfo, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			GameObject gameObject = this.targetObject;
			IWrappedSerializable wrappedSerializable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IWrappedSerializable;
			if (wrappedSerializable != null)
			{
				this.serializeTarget = wrappedSerializable;
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccesfullySpawned(wrappedInfo);
			return;
		}
		this.FailedToSpawn();
	}

	// Token: 0x0600360B RID: 13835 RVA: 0x0012AF5F File Offset: 0x0012915F
	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x0600360C RID: 13836
	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	// Token: 0x0600360D RID: 13837 RVA: 0x0012AF76 File Offset: 0x00129176
	private void FailedToSpawn()
	{
		Debug.LogError("Failed to network instantiate");
		MonkeAgentCleanup.RegisterForDestroy(this.netView.GetView);
		this.netView.GetView.ObservedComponents.Remove(this);
	}

	// Token: 0x0600360E RID: 13838
	protected abstract void OnFailedSpawn();

	// Token: 0x0600360F RID: 13839 RVA: 0x0012AC25 File Offset: 0x00128E25
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x06003610 RID: 13840 RVA: 0x0012AFA9 File Offset: 0x001291A9
	public override void FixedUpdateNetwork()
	{
		this.data = this.serializeTarget.OnSerializeWrite();
	}

	// Token: 0x06003611 RID: 13841 RVA: 0x0012AFBC File Offset: 0x001291BC
	public override void Render()
	{
		if (!base.Object.HasStateAuthority)
		{
			this.serializeTarget.OnSerializeRead(this.data);
		}
	}

	// Token: 0x06003612 RID: 13842 RVA: 0x0012AFDC File Offset: 0x001291DC
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.serializeTarget.OnSerializeWrite(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeRead(stream, info);
	}

	// Token: 0x06003613 RID: 13843 RVA: 0x0012B028 File Offset: 0x00129228
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x06003614 RID: 13844 RVA: 0x0012B028 File Offset: 0x00129228
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x06003615 RID: 13845
	protected abstract void OnBeforeDespawn();

	// Token: 0x06003616 RID: 13846 RVA: 0x0012B030 File Offset: 0x00129230
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.netView.GetView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	// Token: 0x06003617 RID: 13847 RVA: 0x0012B060 File Offset: 0x00129260
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget target = targetOthers ? RpcTarget.Others : RpcTarget.MasterClient;
		this.netView.SendRPC(rpcName, target, data);
	}

	// Token: 0x06003618 RID: 13848 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
	}

	// Token: 0x06003619 RID: 13849 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x0012B083 File Offset: 0x00129283
	public void SendRPC(string rpcName, NetPlayer targetPlayer, params object[] data)
	{
		this.netView.GetView.RPC(rpcName, ((PunNetPlayer)targetPlayer).PlayerRef, data);
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x000028C5 File Offset: 0x00000AC5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x000028C5 File Offset: 0x00000AC5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x0400469B RID: 18075
	protected bool successfullInstantiate;

	// Token: 0x0400469C RID: 18076
	protected IWrappedSerializable serializeTarget;

	// Token: 0x0400469D RID: 18077
	private Type targetType;

	// Token: 0x0400469E RID: 18078
	protected GameObject targetObject;

	// Token: 0x0400469F RID: 18079
	[SerializeField]
	protected NetworkView netView;
}
