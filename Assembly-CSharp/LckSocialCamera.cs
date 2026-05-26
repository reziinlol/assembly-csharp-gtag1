using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using GorillaTag;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000401 RID: 1025
[NetworkBehaviourWeaved(1)]
public class LckSocialCamera : NetworkComponent, IGorillaSliceableSimple
{
	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001840 RID: 6208 RVA: 0x0008A061 File Offset: 0x00088261
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe ref LckSocialCamera.CameraData _networkedData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing LckSocialCamera._networkedData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ref *(LckSocialCamera.CameraData*)(this.Ptr + 0);
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001841 RID: 6209 RVA: 0x0008A086 File Offset: 0x00088286
	public VRRig VrRig
	{
		get
		{
			return this._vrrig;
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001842 RID: 6210 RVA: 0x0008A08E File Offset: 0x0008828E
	// (set) Token: 0x06001843 RID: 6211 RVA: 0x0008A096 File Offset: 0x00088296
	public LCKSocialCameraFollower SocialCameraFollower { get; private set; }

	// Token: 0x06001844 RID: 6212 RVA: 0x0008A0A0 File Offset: 0x000882A0
	public override void OnSpawned()
	{
		if (base.IsLocallyOwned)
		{
			this._localOwnedState = LckSocialCamera.CameraState.Empty;
			this.visible = false;
			this.recording = false;
			this.IsOnNeck = false;
			return;
		}
		if (base.Runner != null)
		{
			LckSocialCamera.CameraState currentState = this._networkedData.currentState;
			this.ApplyVisualState(currentState);
			this._networkOwnedState = currentState;
		}
	}

	// Token: 0x06001845 RID: 6213 RVA: 0x0008A0FA File Offset: 0x000882FA
	public unsafe override void WriteDataFusion()
	{
		*this._networkedData = new LckSocialCamera.CameraData(this._localOwnedState);
	}

	// Token: 0x06001846 RID: 6214 RVA: 0x0008A112 File Offset: 0x00088312
	public override void ReadDataFusion()
	{
		if (this.m_isCorrupted)
		{
			return;
		}
		this.ReadDataShared(this._networkedData.currentState);
	}

	// Token: 0x06001847 RID: 6215 RVA: 0x0008A12E File Offset: 0x0008832E
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this._localOwnedState);
	}

	// Token: 0x06001848 RID: 6216 RVA: 0x0008A144 File Offset: 0x00088344
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner || this.m_isCorrupted)
		{
			return;
		}
		LckSocialCamera.CameraState newState = (LckSocialCamera.CameraState)stream.ReceiveNext();
		this.ReadDataShared(newState);
	}

	// Token: 0x06001849 RID: 6217 RVA: 0x0008A180 File Offset: 0x00088380
	private void ReadDataShared(LckSocialCamera.CameraState newState)
	{
		if (newState != this._networkOwnedState)
		{
			this.ApplyVisualState(newState);
			this._networkOwnedState = newState;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x0600184A RID: 6218 RVA: 0x0008A199 File Offset: 0x00088399
	// (set) Token: 0x0600184B RID: 6219 RVA: 0x0008A1B7 File Offset: 0x000883B7
	public bool IsOnNeck
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localOwnedState : this._networkOwnedState, LckSocialCamera.CameraState.OnNeck);
		}
		set
		{
			if (!base.IsLocallyOwned)
			{
				return;
			}
			this._localOwnedState = LckSocialCamera.SetFlag(this._localOwnedState, LckSocialCamera.CameraState.OnNeck, value);
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x0600184C RID: 6220 RVA: 0x0008A1D5 File Offset: 0x000883D5
	// (set) Token: 0x0600184D RID: 6221 RVA: 0x0008A1F3 File Offset: 0x000883F3
	public bool visible
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localOwnedState : this._networkOwnedState, LckSocialCamera.CameraState.Visible);
		}
		set
		{
			if (!base.IsLocallyOwned)
			{
				return;
			}
			this._localOwnedState = LckSocialCamera.SetFlag(this._localOwnedState, LckSocialCamera.CameraState.Visible, value);
		}
	}

	// Token: 0x17000266 RID: 614
	// (get) Token: 0x0600184E RID: 6222 RVA: 0x0008A211 File Offset: 0x00088411
	// (set) Token: 0x0600184F RID: 6223 RVA: 0x0008A22F File Offset: 0x0008842F
	public bool recording
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localOwnedState : this._networkOwnedState, LckSocialCamera.CameraState.Recording);
		}
		set
		{
			if (!base.IsLocallyOwned)
			{
				return;
			}
			this._localOwnedState = LckSocialCamera.SetFlag(this._localOwnedState, LckSocialCamera.CameraState.Recording, value);
		}
	}

	// Token: 0x06001850 RID: 6224 RVA: 0x0008A250 File Offset: 0x00088450
	private void ApplyVisualState(LckSocialCamera.CameraState newState)
	{
		if (this.m_isCorrupted)
		{
			return;
		}
		bool flag = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.Visible);
		bool flag2 = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.Recording);
		bool flag3 = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.OnNeck);
		if (!base.IsLocallyOwned)
		{
			IGtCameraVisuals cameraVisuals = this.m_CameraVisuals;
			if (cameraVisuals != null)
			{
				cameraVisuals.SetNetworkedVisualsActive(flag);
			}
			IGtCameraVisuals cameraVisuals2 = this.m_CameraVisuals;
			if (cameraVisuals2 != null)
			{
				cameraVisuals2.SetRecordingState(flag2);
			}
			if (this.m_cameraType == LckSocialCamera.CameraType.Tablet)
			{
				if (flag3)
				{
					this.SocialCameraFollower.SetParentToRig();
					return;
				}
				this.SocialCameraFollower.SetParentNull();
			}
			return;
		}
		IGtCameraVisuals cameraVisuals3 = this.m_CameraVisuals;
		if (cameraVisuals3 != null)
		{
			cameraVisuals3.SetVisualsActive(false);
		}
		IGtCameraVisuals cameraVisuals4 = this.m_CameraVisuals;
		if (cameraVisuals4 == null)
		{
			return;
		}
		cameraVisuals4.SetRecordingState(false);
	}

	// Token: 0x06001851 RID: 6225 RVA: 0x0008A2F1 File Offset: 0x000884F1
	private static bool GetFlag(LckSocialCamera.CameraState currentState, LckSocialCamera.CameraState flag)
	{
		return currentState.HasFlag(flag);
	}

	// Token: 0x06001852 RID: 6226 RVA: 0x0008A304 File Offset: 0x00088504
	private static LckSocialCamera.CameraState SetFlag(LckSocialCamera.CameraState currentState, LckSocialCamera.CameraState flag, bool shouldBeSet)
	{
		if (shouldBeSet)
		{
			return currentState | flag;
		}
		return currentState & ~flag;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x0008A314 File Offset: 0x00088514
	protected override void Awake()
	{
		base.Awake();
		if (this.CameraVisuals != null && !this.CameraVisuals.TryGetComponent<IGtCameraVisuals>(out this.m_CameraVisuals))
		{
			Debug.LogError("LCK: LckSocialCamera failed to find IGtCameraVisuals component on CameraVisuals");
		}
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			return;
		}
		ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccesfullSpawn);
		succesfullSpawnEvent.Add(inAction);
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x0008A398 File Offset: 0x00088598
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.m_lckDelegateRegistered)
		{
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Remove(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
		}
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x0008A3C8 File Offset: 0x000885C8
	private void OnSuccesfullSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		this._vrrig = rig.Rig;
		LCKSocialCameraFollower lcksocialCameraFollower = (this.m_cameraType == LckSocialCamera.CameraType.Cococam) ? rig.LckCococamFollower : rig.LCKTabletFollower;
		this._scaleTransform = lcksocialCameraFollower.ScaleTransform;
		this.CameraVisuals = lcksocialCameraFollower.CameraVisualsRoot;
		this.m_CameraVisuals = this.CameraVisuals.GetComponent<IGtCameraVisuals>();
		if (!base.IsLocallyOwned && lcksocialCameraFollower.GetComponent<ILckCosmeticDependantPlayerIdSupplier>() != null)
		{
			lcksocialCameraFollower.GetComponent<ILckCosmeticDependantPlayerIdSupplier>().UpdatePlayerId();
		}
		this.SocialCameraFollower = lcksocialCameraFollower;
		this.m_isCorrupted = false;
		if (!this._vrrig.isOfflineVRRig)
		{
			lcksocialCameraFollower.SetNetworkController(this);
			return;
		}
		LckSocialCameraManager instance = LckSocialCameraManager.Instance;
		if (!(instance != null))
		{
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Combine(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
			this.m_lckDelegateRegistered = true;
			return;
		}
		LckSocialCamera.CameraType cameraType = this.m_cameraType;
		if (cameraType == LckSocialCamera.CameraType.Cococam)
		{
			instance.SetLckSocialCococamCamera(this);
			return;
		}
		if (cameraType != LckSocialCamera.CameraType.Tablet)
		{
			throw new ArgumentOutOfRangeException();
		}
		instance.SetLckSocialTabletCamera(this);
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x0008A4C0 File Offset: 0x000886C0
	public void SliceUpdate()
	{
		if (this._vrrig.IsNull())
		{
			return;
		}
		if (this.m_cameraType != LckSocialCamera.CameraType.Tablet)
		{
			if (this.m_cameraType == LckSocialCamera.CameraType.Cococam)
			{
				this.SocialCameraFollower.transform.localScale = Vector3.one * this._vrrig.scaleFactor;
			}
			return;
		}
		if (this.IsOnNeck)
		{
			this.SocialCameraFollower.transform.localScale = Vector3.one * 0.3f;
			return;
		}
		this.SocialCameraFollower.transform.localScale = Vector3.one * 0.3f * this._vrrig.scaleFactor;
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x0008A569 File Offset: 0x00088769
	public new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x0008A580 File Offset: 0x00088780
	public new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.m_isCorrupted)
		{
			return;
		}
		if (this.SocialCameraFollower.IsNotNull())
		{
			this.SocialCameraFollower.RemoveNetworkController(this);
		}
		this._scaleTransform = null;
		this.CameraVisuals = null;
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x0008A5D4 File Offset: 0x000887D4
	private void OnManagerSpawned(LckSocialCameraManager manager)
	{
		LckSocialCamera.CameraType cameraType = this.m_cameraType;
		if (cameraType == LckSocialCamera.CameraType.Cococam)
		{
			manager.SetLckSocialCococamCamera(this);
			return;
		}
		if (cameraType != LckSocialCamera.CameraType.Tablet)
		{
			throw new ArgumentOutOfRangeException();
		}
		manager.SetLckSocialTabletCamera(this);
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x0008A606 File Offset: 0x00088806
	public void TurnOff()
	{
		this.m_isCorrupted = true;
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x0008A62A File Offset: 0x0008882A
	[WeaverGenerated]
	public unsafe override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		*this._networkedData = this.__networkedData;
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x0008A647 File Offset: 0x00088847
	[WeaverGenerated]
	public unsafe override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this.__networkedData = *this._networkedData;
	}

	// Token: 0x04002373 RID: 9075
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x04002374 RID: 9076
	[SerializeField]
	public GameObject CameraVisuals;

	// Token: 0x04002375 RID: 9077
	[SerializeField]
	private VRRig _vrrig;

	// Token: 0x04002376 RID: 9078
	[SerializeField]
	private VRRigSerializer m_rigNetworkController;

	// Token: 0x04002377 RID: 9079
	[SerializeField]
	private LckSocialCamera.CameraType m_cameraType;

	// Token: 0x04002379 RID: 9081
	private bool m_isCorrupted = true;

	// Token: 0x0400237A RID: 9082
	private bool m_lckDelegateRegistered;

	// Token: 0x0400237B RID: 9083
	private IGtCameraVisuals m_CameraVisuals;

	// Token: 0x0400237C RID: 9084
	private LckSocialCamera.CameraState _localOwnedState;

	// Token: 0x0400237D RID: 9085
	private LckSocialCamera.CameraState _networkOwnedState;

	// Token: 0x0400237E RID: 9086
	[WeaverGenerated]
	[DefaultForProperty("_networkedData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private LckSocialCamera.CameraData __networkedData;

	// Token: 0x02000402 RID: 1026
	private enum CameraState
	{
		// Token: 0x04002380 RID: 9088
		Empty,
		// Token: 0x04002381 RID: 9089
		Visible,
		// Token: 0x04002382 RID: 9090
		Recording,
		// Token: 0x04002383 RID: 9091
		OnNeck = 4
	}

	// Token: 0x02000403 RID: 1027
	private enum CameraType
	{
		// Token: 0x04002385 RID: 9093
		Cococam,
		// Token: 0x04002386 RID: 9094
		Tablet
	}

	// Token: 0x02000404 RID: 1028
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct CameraData : INetworkStruct
	{
		// Token: 0x0600185E RID: 6238 RVA: 0x0008A660 File Offset: 0x00088860
		public CameraData(LckSocialCamera.CameraState state)
		{
			this.currentState = state;
		}

		// Token: 0x04002387 RID: 9095
		[FieldOffset(0)]
		public LckSocialCamera.CameraState currentState;
	}
}
