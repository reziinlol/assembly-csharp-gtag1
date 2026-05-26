using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
[NetworkBehaviourWeaved(0)]
public class WorldShareableItem : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000323 RID: 803
	// (get) Token: 0x06001DE5 RID: 7653 RVA: 0x000A1020 File Offset: 0x0009F220
	// (set) Token: 0x06001DE6 RID: 7654 RVA: 0x000A1028 File Offset: 0x0009F228
	[DevInspectorShow]
	public TransferrableObject.PositionState transferableObjectState { get; set; }

	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001DE7 RID: 7655 RVA: 0x000A1031 File Offset: 0x0009F231
	// (set) Token: 0x06001DE8 RID: 7656 RVA: 0x000A1039 File Offset: 0x0009F239
	public TransferrableObject.ItemStates transferableObjectItemState { get; set; }

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001DE9 RID: 7657 RVA: 0x000A1042 File Offset: 0x0009F242
	// (set) Token: 0x06001DEA RID: 7658 RVA: 0x000A104A File Offset: 0x0009F24A
	public TransferrableObject.PositionState transferableObjectStateNetworked { get; set; }

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001DEB RID: 7659 RVA: 0x000A1053 File Offset: 0x0009F253
	// (set) Token: 0x06001DEC RID: 7660 RVA: 0x000A105B File Offset: 0x0009F25B
	public TransferrableObject.ItemStates transferableObjectItemStateNetworked { get; set; }

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001DED RID: 7661 RVA: 0x000A1064 File Offset: 0x0009F264
	// (set) Token: 0x06001DEE RID: 7662 RVA: 0x000A106C File Offset: 0x0009F26C
	[DevInspectorShow]
	public WorldTargetItem target
	{
		get
		{
			return this._target;
		}
		set
		{
			this._target = value;
		}
	}

	// Token: 0x06001DEF RID: 7663 RVA: 0x000A1075 File Offset: 0x0009F275
	protected override void Awake()
	{
		base.Awake();
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.teleportSerializer = base.GetComponent<TransformViewTeleportSerializer>();
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06001DF0 RID: 7664 RVA: 0x000A10A5 File Offset: 0x0009F2A5
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		if (GTAppState.isQuitting)
		{
			return;
		}
		base.OnEnable();
		this.guard.AddCallbackTarget(this);
		WorldShareableItemManager.Register(this);
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06001DF1 RID: 7665 RVA: 0x000A10E0 File Offset: 0x0009F2E0
	internal override void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		if (this.target == null || !this.target.transferrableObject.isSceneObject)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ViewID = 0;
		}
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
		this.guard.RemoveCallbackTarget(this);
		this.rpcCallBack = null;
		this.onOwnerChangeCb = null;
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x06001DF2 RID: 7666 RVA: 0x000A1160 File Offset: 0x0009F360
	public void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		WorldShareableItemManager.Unregister(this);
	}

	// Token: 0x06001DF3 RID: 7667 RVA: 0x000A1170 File Offset: 0x0009F370
	public void SetupSharableViewIDs(NetPlayer player, int slotID)
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		int num = player.ActorNumber * 1000 + 990 + slotID * 2;
		this.guard.giveCreatorAbsoluteAuthority = true;
		if (num != photonView.ViewID)
		{
			photonView.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2;
			photonView2.ViewID = player.ActorNumber * 1000 + 990 + slotID * 2 + 1;
			this.guard.SetCreator(player);
		}
	}

	// Token: 0x06001DF4 RID: 7668 RVA: 0x000A11FC File Offset: 0x0009F3FC
	public void ResetViews()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		PhotonView[] components = base.GetComponents<PhotonView>();
		PhotonView photonView = components[0];
		PhotonView photonView2 = components[1];
		photonView.ViewID = 0;
		photonView2.ViewID = 0;
	}

	// Token: 0x06001DF5 RID: 7669 RVA: 0x000A122C File Offset: 0x0009F42C
	public void SetupSharableObject(int itemIDx, NetPlayer owner, Transform targetXform)
	{
		if (this.target != null)
		{
			Debug.LogError("ERROR!!!  WorldShareableItem.SetupSharableObject: target is expected to be null before this call. In scene path = \"" + base.transform.GetPathQ() + "\"", this);
			return;
		}
		this.target = WorldTargetItem.GenerateTargetFromPlayerAndID(owner, itemIDx);
		if (this.target.targetObject != targetXform)
		{
			Debug.LogError(string.Format("The target object found a transform that does not match the target transform, this should never happen. owner: {0} itemIDx: {1} targetXformPath: {2}, target.targetObject: {3}", new object[]
			{
				owner,
				itemIDx,
				targetXform.GetPath(),
				this.target.targetObject.GetPath()
			}));
		}
		TransferrableObject component = this.target.targetObject.GetComponent<TransferrableObject>();
		this.validShareable = (component.canDrop || component.shareable || component.allowWorldSharableInstance);
		if (!this.validShareable)
		{
			Debug.LogError(string.Format("tried to setup an invalid shareable {0} {1} {2}", owner, itemIDx, targetXform.GetPath()));
			base.gameObject.SetActive(false);
			this.Invalidate();
			return;
		}
		this.guard.AddCallbackTarget(component);
		this.guard.giveCreatorAbsoluteAuthority = true;
		component.SetWorldShareableItem(this);
	}

	// Token: 0x06001DF6 RID: 7670 RVA: 0x000A1346 File Offset: 0x0009F546
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		base.OnPhotonInstantiate(info);
	}

	// Token: 0x06001DF7 RID: 7671 RVA: 0x000A1350 File Offset: 0x0009F550
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		if (this.onOwnerChangeCb != null)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(newOwner);
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(previousOwner);
			this.onOwnerChangeCb(player, player2);
		}
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001DF8 RID: 7672 RVA: 0x000A138A File Offset: 0x0009F58A
	// (set) Token: 0x06001DF9 RID: 7673 RVA: 0x000A1392 File Offset: 0x0009F592
	[DevInspectorShow]
	public bool EnableRemoteSync
	{
		get
		{
			return this.enableRemoteSync;
		}
		set
		{
			this.enableRemoteSync = value;
		}
	}

	// Token: 0x06001DFA RID: 7674 RVA: 0x000A139C File Offset: 0x0009F59C
	public void TriggeredUpdate()
	{
		if (!this.IsTargetValid())
		{
			return;
		}
		if (this.guard.isTrulyMine)
		{
			Vector3 position;
			Quaternion rotation;
			this.target.targetObject.GetPositionAndRotation(out position, out rotation);
			base.transform.SetPositionAndRotation(position, rotation);
			return;
		}
		if (!base.IsMine && this.EnableRemoteSync)
		{
			Vector3 position2;
			Quaternion rotation2;
			base.transform.GetPositionAndRotation(out position2, out rotation2);
			this.target.targetObject.SetPositionAndRotation(position2, rotation2);
		}
	}

	// Token: 0x06001DFB RID: 7675 RVA: 0x000A1412 File Offset: 0x0009F612
	public void SyncToSceneObject(TransferrableObject transferrableObject)
	{
		this.target = WorldTargetItem.GenerateTargetFromWorldSharableItem(null, -2, transferrableObject.transform);
		base.transform.parent = null;
	}

	// Token: 0x06001DFC RID: 7676 RVA: 0x000A1434 File Offset: 0x0009F634
	public void SetupSceneObjectOnNetwork(NetPlayer owner)
	{
		this.guard.SetOwnership(owner, false, false);
	}

	// Token: 0x06001DFD RID: 7677 RVA: 0x000A1444 File Offset: 0x0009F644
	public bool IsTargetValid()
	{
		return this.target != null;
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x000A144F File Offset: 0x0009F64F
	public void Invalidate()
	{
		this.target = null;
		this.transferableObjectState = TransferrableObject.PositionState.None;
		this.transferableObjectItemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x000A1468 File Offset: 0x0009F668
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer == null)
		{
			return;
		}
		WorldShareableItem.CachedData cachedData;
		if (this.cachedDatas.TryGetValue(toPlayer, out cachedData))
		{
			this.transferableObjectState = cachedData.cachedTransferableObjectState;
			this.transferableObjectItemState = cachedData.cachedTransferableObjectItemState;
			this.cachedDatas.Remove(toPlayer);
		}
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x000A14AE File Offset: 0x0009F6AE
	public override void WriteDataFusion()
	{
		this.transferableObjectItemStateNetworked = this.transferableObjectItemState;
		this.transferableObjectStateNetworked = this.transferableObjectState;
	}

	// Token: 0x06001E01 RID: 7681 RVA: 0x000A14C8 File Offset: 0x0009F6C8
	public override void ReadDataFusion()
	{
		this.transferableObjectItemState = this.transferableObjectItemStateNetworked;
		this.transferableObjectState = this.transferableObjectStateNetworked;
	}

	// Token: 0x06001E02 RID: 7682 RVA: 0x000A14E2 File Offset: 0x0009F6E2
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.transferableObjectState);
		stream.SendNext(this.transferableObjectItemState);
	}

	// Token: 0x06001E03 RID: 7683 RVA: 0x000A1508 File Offset: 0x0009F708
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (player != this.guard.actualOwner)
		{
			Debug.Log("Blocking info from non owner");
			this.cachedDatas.AddOrUpdate(player, new WorldShareableItem.CachedData
			{
				cachedTransferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext(),
				cachedTransferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext()
			});
			return;
		}
		this.transferableObjectState = (TransferrableObject.PositionState)stream.ReceiveNext();
		this.transferableObjectItemState = (TransferrableObject.ItemStates)stream.ReceiveNext();
	}

	// Token: 0x06001E04 RID: 7684 RVA: 0x000A159A File Offset: 0x0009F79A
	[PunRPC]
	internal void RPCWorldShareable(PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		MonkeAgent.IncrementRPCCall(info, "RPCWorldShareable");
		if (this.rpcCallBack == null)
		{
			return;
		}
		this.rpcCallBack();
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x00023994 File Offset: 0x00021B94
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return true;
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x06001E07 RID: 7687 RVA: 0x00023994 File Offset: 0x00021B94
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return true;
	}

	// Token: 0x06001E08 RID: 7688 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x06001E09 RID: 7689 RVA: 0x000A15CC File Offset: 0x0009F7CC
	public void SetWillTeleport()
	{
		this.teleportSerializer.SetWillTeleport();
	}

	// Token: 0x06001E0B RID: 7691 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001E0C RID: 7692 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x0400283E RID: 10302
	private bool validShareable = true;

	// Token: 0x0400283F RID: 10303
	public RequestableOwnershipGuard guard;

	// Token: 0x04002840 RID: 10304
	private TransformViewTeleportSerializer teleportSerializer;

	// Token: 0x04002841 RID: 10305
	[DevInspectorShow]
	[CanBeNull]
	private WorldTargetItem _target;

	// Token: 0x04002842 RID: 10306
	public WorldShareableItem.OnOwnerChangeDelegate onOwnerChangeCb;

	// Token: 0x04002843 RID: 10307
	public Action rpcCallBack;

	// Token: 0x04002844 RID: 10308
	private bool enableRemoteSync = true;

	// Token: 0x04002845 RID: 10309
	public Dictionary<NetPlayer, WorldShareableItem.CachedData> cachedDatas = new Dictionary<NetPlayer, WorldShareableItem.CachedData>();

	// Token: 0x020004D1 RID: 1233
	// (Invoke) Token: 0x06001E0E RID: 7694
	public delegate void Delegate();

	// Token: 0x020004D2 RID: 1234
	// (Invoke) Token: 0x06001E12 RID: 7698
	public delegate void OnOwnerChangeDelegate(NetPlayer newOwner, NetPlayer prevOwner);

	// Token: 0x020004D3 RID: 1235
	public struct CachedData
	{
		// Token: 0x04002846 RID: 10310
		public TransferrableObject.PositionState cachedTransferableObjectState;

		// Token: 0x04002847 RID: 10311
		public TransferrableObject.ItemStates cachedTransferableObjectItemState;
	}
}
