using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using JetBrains.Annotations;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x02000545 RID: 1349
public class TransferrableObject : HoldableObject, ISelfValidator, IRequestableOwnershipGuardCallbacks, IPreDisable, ISpawnable, IBuildValidation
{
	// Token: 0x06002212 RID: 8722 RVA: 0x000B63A8 File Offset: 0x000B45A8
	public void FixTransformOverride()
	{
		this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06002214 RID: 8724 RVA: 0x000B63B6 File Offset: 0x000B45B6
	// (set) Token: 0x06002215 RID: 8725 RVA: 0x000B63BE File Offset: 0x000B45BE
	public VRRig myRig
	{
		get
		{
			return this._myRig;
		}
		private set
		{
			this._myRig = value;
		}
	}

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x06002216 RID: 8726 RVA: 0x000B63C7 File Offset: 0x000B45C7
	// (set) Token: 0x06002217 RID: 8727 RVA: 0x000B63CF File Offset: 0x000B45CF
	public bool isMyRigValid { get; private set; }

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002218 RID: 8728 RVA: 0x000B63D8 File Offset: 0x000B45D8
	// (set) Token: 0x06002219 RID: 8729 RVA: 0x000B63E0 File Offset: 0x000B45E0
	public VRRig myOnlineRig
	{
		get
		{
			return this._myOnlineRig;
		}
		private set
		{
			this._myOnlineRig = value;
			this.isMyOnlineRigValid = true;
		}
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x0600221A RID: 8730 RVA: 0x000B63F0 File Offset: 0x000B45F0
	// (set) Token: 0x0600221B RID: 8731 RVA: 0x000B63F8 File Offset: 0x000B45F8
	public bool isMyOnlineRigValid { get; private set; }

	// Token: 0x0600221C RID: 8732 RVA: 0x000B6404 File Offset: 0x000B4604
	public void SetTargetRig(VRRig rig)
	{
		if (rig == null)
		{
			this.targetRigSet = false;
			if (this.isSceneObject)
			{
				this.targetRig = rig;
				this.targetDockPositions = null;
				this.anchorOverrides = null;
				return;
			}
			if (this.myRig)
			{
				this.SetTargetRig(this.myRig);
			}
			if (this.myOnlineRig)
			{
				this.SetTargetRig(this.myOnlineRig);
			}
			return;
		}
		else
		{
			this.targetRigSet = true;
			this.targetRig = rig;
			BodyDockPositions component = rig.GetComponent<BodyDockPositions>();
			VRRigAnchorOverrides component2 = rig.GetComponent<VRRigAnchorOverrides>();
			if (!component)
			{
				Debug.LogError("There is no dock attached to this rig", this);
				return;
			}
			if (!component2)
			{
				Debug.LogError("There is no overrides attached to this rig", this);
				return;
			}
			this.anchorOverrides = component2;
			this.targetDockPositions = component;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			return;
		}
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x0600221D RID: 8733 RVA: 0x000B64D4 File Offset: 0x000B46D4
	public bool IsLocalOwnedWorldShareable
	{
		get
		{
			return this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine;
		}
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x000B64F8 File Offset: 0x000B46F8
	public void WorldShareableRequestOwnership()
	{
		if (this.worldShareableInstance != null && !this.worldShareableInstance.guard.isMine)
		{
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
	}

	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x0600221F RID: 8735 RVA: 0x000B6554 File Offset: 0x000B4754
	// (set) Token: 0x06002220 RID: 8736 RVA: 0x000B655C File Offset: 0x000B475C
	public bool isRigidbodySet { get; private set; }

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x06002221 RID: 8737 RVA: 0x000B6565 File Offset: 0x000B4765
	// (set) Token: 0x06002222 RID: 8738 RVA: 0x000B656D File Offset: 0x000B476D
	public bool shouldUseGravity { get; private set; }

	// Token: 0x06002223 RID: 8739 RVA: 0x000B6576 File Offset: 0x000B4776
	protected virtual void Awake()
	{
		if (this.isSceneObject)
		{
			this.IsSpawned = true;
			this.OnSpawn(null);
		}
	}

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x06002224 RID: 8740 RVA: 0x000B658E File Offset: 0x000B478E
	// (set) Token: 0x06002225 RID: 8741 RVA: 0x000B6596 File Offset: 0x000B4796
	public bool IsSpawned { get; set; }

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x06002226 RID: 8742 RVA: 0x000B659F File Offset: 0x000B479F
	// (set) Token: 0x06002227 RID: 8743 RVA: 0x000B65A7 File Offset: 0x000B47A7
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06002228 RID: 8744 RVA: 0x000B65B0 File Offset: 0x000B47B0
	public virtual void OnSpawn(VRRig rig)
	{
		try
		{
			if (!this.isSceneObject)
			{
				if (!rig)
				{
					Debug.LogError("Disabling TransferrableObject because could not find VRRig! \"" + base.transform.GetPath() + "\"", this);
					base.enabled = false;
					this.isMyRigValid = false;
					this.isMyOnlineRigValid = false;
					return;
				}
				this.myRig = (rig.isOfflineVRRig ? rig : null);
				this.myOnlineRig = (rig.isOfflineVRRig ? null : rig);
				this.targetDockPositions = rig.myBodyDockPositions;
			}
			else
			{
				this.myRig = null;
				this.myOnlineRig = null;
			}
			this.isMyRigValid = true;
			this.isMyOnlineRigValid = true;
			if (this.isSceneObject)
			{
				this.targetDockPositions = base.GetComponentInParent<BodyDockPositions>();
			}
			this.anchor = base.transform.parent;
			if (this.rigidbodyInstance == null)
			{
				this.rigidbodyInstance = base.GetComponent<Rigidbody>();
			}
			if (this.rigidbodyInstance != null)
			{
				this.isRigidbodySet = true;
				this.shouldUseGravity = this.rigidbodyInstance.useGravity;
			}
			this.audioSrc = base.GetComponent<AudioSource>();
			this.latched = false;
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.anchor == null)
			{
				this.InitialDockObject = base.transform.parent;
			}
			else
			{
				this.InitialDockObject = this.anchor.parent;
			}
			this.isGrabAnchorSet = (this.grabAnchor != null);
			if (this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = true;
						spawnable.CosmeticSelectedSide = this.CosmeticSelectedSide;
						spawnable.OnSpawn(this.myRig);
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x000B67C0 File Offset: 0x000B49C0
	public virtual void OnDespawn()
	{
		try
		{
			if (!this.isSceneObject)
			{
				foreach (ISpawnable spawnable in base.GetComponentsInChildren<ISpawnable>(true))
				{
					if (spawnable != this)
					{
						spawnable.IsSpawned = false;
						spawnable.OnDespawn();
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x000B6848 File Offset: 0x000B4A48
	private void SetInitMatrix()
	{
		this.initMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		if (this.handPoseLeft != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseLeftReferenceRotation * Quaternion.Inverse(this.handPoseLeft.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseLeftReferencePoint) - this.handPoseLeft.transform.position;
			this.leftHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.leftHandMatrix = this.initMatrix;
		}
		if (this.handPoseRight != null)
		{
			base.transform.localRotation = TransferrableObject.handPoseRightReferenceRotation * Quaternion.Inverse(this.handPoseRight.localRotation);
			base.transform.position += base.transform.parent.TransformPoint(TransferrableObject.handPoseRightReferencePoint) - this.handPoseRight.transform.position;
			this.rightHandMatrix = base.transform.LocalMatrixRelativeToParentWithScale();
		}
		else
		{
			this.rightHandMatrix = this.initMatrix;
		}
		base.transform.localPosition = this.initMatrix.Position();
		base.transform.localRotation = this.initMatrix.Rotation();
		this.positionInitialized = true;
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void Start()
	{
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000B69C0 File Offset: 0x000B4BC0
	internal virtual void OnEnable()
	{
		try
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			this.OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
		{
			this.previousItemState = (TransferrableObject.ItemStates)0;
			this.itemState = (TransferrableObject.ItemStates)0;
		}
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000B6A78 File Offset: 0x000B4C78
	public virtual void OnEnable_AfterAllCosmeticsSpawnedOrIsSceneObject()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (!base.enabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		try
		{
			TransferrableObjectManager.Register(this);
			this.transferrableItemSlotTransformOverride = base.GetComponent<TransferrableItemSlotTransformOverride>();
			if (!this.positionInitialized)
			{
				this.SetInitMatrix();
				this.positionInitialized = true;
			}
			if (this.isSceneObject)
			{
				if (!this.worldShareableInstance)
				{
					Debug.LogError("Missing Sharable Instance on Scene enabled object: " + base.gameObject.name);
				}
				else
				{
					this.worldShareableInstance.SyncToSceneObject(this);
					this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().AddCallbackTarget(this);
				}
			}
			else
			{
				if (!this.isSceneObject && !this.myRig && !this.myOnlineRig && !this.ownerRig)
				{
					this.ownerRig = base.GetComponentInParent<VRRig>(true);
					if (this.ownerRig.isOfflineVRRig)
					{
						this.myRig = this.ownerRig;
					}
					else
					{
						this.myOnlineRig = this.ownerRig;
					}
				}
				if (!this.myRig && this.myOnlineRig)
				{
					this.ownerRig = this.myOnlineRig;
					this.SetTargetRig(this.myOnlineRig);
				}
				if (!this.IsSpawned)
				{
					this.IsSpawned = true;
					this.OnSpawn((this.myRig != null) ? this.myRig : this.myOnlineRig);
				}
				if (this.myRig == null && this.myOnlineRig == null)
				{
					if (!this.isSceneObject)
					{
						base.gameObject.SetActive(false);
					}
				}
				else
				{
					this.objectIndex = this.targetDockPositions.ReturnTransferrableItemIndex(this.myIndex);
					if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
					{
						this.storedZone = BodyDockPositions.DropPositions.LeftArm;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
					{
						this.storedZone = BodyDockPositions.DropPositions.RightArm;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
					{
						this.storedZone = BodyDockPositions.DropPositions.LeftBack;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
					{
						this.storedZone = BodyDockPositions.DropPositions.RightBack;
					}
					else if (this.currentState == TransferrableObject.PositionState.OnChest)
					{
						this.storedZone = BodyDockPositions.DropPositions.Chest;
					}
					if (this.IsLocalObject())
					{
						this.ownerRig = GorillaTagger.Instance.offlineVRRig;
						this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
					}
					if (this.objectIndex == -1)
					{
						base.gameObject.SetActive(false);
					}
					else
					{
						if (this.currentState == TransferrableObject.PositionState.OnLeftArm && this.flipOnXForLeftArm)
						{
							Transform transform = this.GetAnchor(this.currentState);
							transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
						}
						this.initState = this.currentState;
						this.enabledOnFrame = Time.frameCount;
						this.startInterpolation = true;
						if (NetworkSystem.Instance.InRoom)
						{
							if (this.canDrop || this.shareable)
							{
								this.SpawnTransferableObjectViews();
								if (this.myRig)
								{
									if (this.myRig != null && this.worldShareableInstance != null)
									{
										this.OnWorldShareableItemSpawn();
									}
								}
							}
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x0600222E RID: 8750 RVA: 0x000B6DF0 File Offset: 0x000B4FF0
	internal virtual void OnDisable()
	{
		TransferrableObjectManager.Unregister(this);
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		this.enabledOnFrame = -1;
		base.transform.localScale = Vector3.one;
		try
		{
			if (!this.isSceneObject && this.IsLocalObject() && this.worldShareableInstance && !this.IsMyItem())
			{
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
				{
				});
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.Invalidate();
				this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
				if (this.targetDockPositions)
				{
					this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
				}
				if (!this.isSceneObject)
				{
					this.worldShareableInstance = null;
				}
			}
			this.PlayDestroyedOrDisabledEffect();
			if (this.isSceneObject)
			{
				this.IsSpawned = false;
				this.OnDespawn();
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception, this);
			base.enabled = false;
			base.gameObject.SetActive(false);
			Debug.LogError("TransferrableObject: Disabled & deactivated self because of the exception logged above. Path: " + base.transform.GetPathQ(), this);
		}
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000B6F68 File Offset: 0x000B5168
	protected new virtual void OnDestroy()
	{
		TransferrableObjectManager.Unregister(this);
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x000B6F70 File Offset: 0x000B5170
	public void CleanupDisable()
	{
		this.currentState = TransferrableObject.PositionState.None;
		this.enabledOnFrame = -1;
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			if (this.anchor != base.transform)
			{
				base.transform.parent = this.anchor;
			}
		}
		else
		{
			base.transform.parent = this.anchor;
		}
		this.interpState = TransferrableObject.InterpolateState.None;
		Transform transform = base.transform;
		Matrix4x4 defaultTransformationMatrix = this.GetDefaultTransformationMatrix();
		transform.SetLocalMatrixRelativeToParentWithXParity(defaultTransformationMatrix);
	}

	// Token: 0x06002231 RID: 8753 RVA: 0x000B6FFB File Offset: 0x000B51FB
	public virtual void PreDisable()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
		{
			this.previousItemState = (TransferrableObject.ItemStates)0;
			this.itemState = (TransferrableObject.ItemStates)0;
		}
		this.currentState = TransferrableObject.PositionState.None;
		this.interpState = TransferrableObject.InterpolateState.None;
		this.ResetToDefaultState();
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000B7030 File Offset: 0x000B5230
	public virtual Matrix4x4 GetDefaultTransformationMatrix()
	{
		TransferrableObject.PositionState positionState = this.currentState;
		if (positionState == TransferrableObject.PositionState.InLeftHand)
		{
			return this.leftHandMatrix;
		}
		if (positionState != TransferrableObject.PositionState.InRightHand)
		{
			return this.initMatrix;
		}
		return this.rightHandMatrix;
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000B7062 File Offset: 0x000B5262
	public virtual bool ShouldBeKinematic()
	{
		if (this.detatchOnGrab)
		{
			return this.currentState != TransferrableObject.PositionState.Dropped && this.currentState != TransferrableObject.PositionState.InLeftHand && this.currentState != TransferrableObject.PositionState.InRightHand;
		}
		return this.currentState != TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000B70A0 File Offset: 0x000B52A0
	private void SpawnShareableObject()
	{
		if (this.isSceneObject)
		{
			if (this.worldShareableInstance == null)
			{
				return;
			}
			this.worldShareableInstance.GetComponent<WorldShareableItem>().SetupSceneObjectOnNetwork(NetworkSystem.Instance.MasterClient);
			return;
		}
		else
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.SpawnTransferableObjectViews();
			if (!this.myRig)
			{
				return;
			}
			if (!this.canDrop && !this.shareable)
			{
				return;
			}
			if (this.myRig != null && this.worldShareableInstance != null)
			{
				this.OnWorldShareableItemSpawn();
			}
			return;
		}
	}

	// Token: 0x06002235 RID: 8757 RVA: 0x000B7134 File Offset: 0x000B5334
	public void SpawnTransferableObjectViews()
	{
		NetPlayer owner = NetworkSystem.Instance.LocalPlayer;
		if (!this.ownerRig.isOfflineVRRig)
		{
			owner = this.ownerRig.creator;
		}
		if (this.worldShareableInstance == null)
		{
			this.worldShareableInstance = this.targetDockPositions.AllocateSharableInstance(this.storedZone, owner);
		}
		GorillaTagger.OnPlayerSpawned(delegate
		{
			this.worldShareableInstance.SetupSharableObject(this.myIndex, owner, this.transform);
		});
	}

	// Token: 0x06002236 RID: 8758 RVA: 0x000B71B8 File Offset: 0x000B53B8
	public virtual void OnJoinedRoom()
	{
		if (this.isSceneObject)
		{
			this.worldShareableInstance == null;
			return;
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (!this.canDrop && !this.shareable)
		{
			return;
		}
		this.SpawnTransferableObjectViews();
		if (!this.myRig)
		{
			return;
		}
		if (this.myRig != null && this.worldShareableInstance != null)
		{
			this.OnWorldShareableItemSpawn();
		}
	}

	// Token: 0x06002237 RID: 8759 RVA: 0x000B7230 File Offset: 0x000B5430
	public virtual void OnLeftRoom()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.isSceneObject)
		{
			return;
		}
		if (!this.shareable && !this.allowWorldSharableInstance && !this.canDrop)
		{
			return;
		}
		if (base.gameObject.activeSelf && this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			else
			{
				this.worldShareableInstance.ResetViews();
				ObjectPools.instance.Destroy(this.worldShareableInstance.gameObject);
			}
			this.worldShareableInstance = null;
		}
		if (!this.IsLocalObject())
		{
			this.OnItemDestroyedOrDisabled();
			base.gameObject.Disable();
			return;
		}
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000B72FE File Offset: 0x000B54FE
	public bool IsLocalObject()
	{
		return this.myRig != null && this.myRig.isOfflineVRRig;
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000B7315 File Offset: 0x000B5515
	public void SetWorldShareableItem(WorldShareableItem item)
	{
		this.worldShareableInstance = item;
		this.OnWorldShareableItemSpawn();
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnWorldShareableItemSpawn()
	{
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void PlayDestroyedOrDisabledEffect()
	{
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000B7324 File Offset: 0x000B5524
	protected virtual void OnItemDestroyedOrDisabled()
	{
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.Invalidate();
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RemoveCallbackTarget(this);
			if (this.targetDockPositions)
			{
				this.targetDockPositions.DeallocateSharableInstance(this.worldShareableInstance);
			}
			Debug.LogError("Setting WSI to null in OnItemDestroyedOrDisabled", this);
			this.worldShareableInstance = null;
		}
		this.PlayDestroyedOrDisabledEffect();
		this.enabledOnFrame = -1;
		this.currentState = TransferrableObject.PositionState.None;
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000B739E File Offset: 0x000B559E
	public virtual void TriggeredLateUpdate()
	{
		if (this.IsLocalObject() && this.canDrop)
		{
			this.LocalMyObjectValidation();
		}
		if (this.IsMyItem())
		{
			this.LateUpdateLocal();
		}
		else
		{
			this.LateUpdateReplicated();
		}
		this.LateUpdateShared();
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000B73D2 File Offset: 0x000B55D2
	protected Transform DefaultAnchor()
	{
		if (this._isDefaultAnchorSet)
		{
			return this._defaultAnchor;
		}
		this._isDefaultAnchorSet = true;
		this._defaultAnchor = ((this.anchor == null) ? base.transform : this.anchor);
		return this._defaultAnchor;
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x000B7412 File Offset: 0x000B5612
	private Transform GetAnchor(TransferrableObject.PositionState pos)
	{
		if (this.grabAnchor == null)
		{
			return this.DefaultAnchor();
		}
		if (this.InHand())
		{
			return this.grabAnchor;
		}
		return this.DefaultAnchor();
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x000B7440 File Offset: 0x000B5640
	protected bool Attached()
	{
		bool flag = this.InHand() && this.detatchOnGrab;
		return !this.Dropped() && !flag;
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x000B7470 File Offset: 0x000B5670
	private Transform GetTargetStorageZone(BodyDockPositions.DropPositions state)
	{
		switch (state)
		{
		case BodyDockPositions.DropPositions.None:
			return null;
		case BodyDockPositions.DropPositions.LeftArm:
			return this.targetDockPositions.leftArmTransform;
		case BodyDockPositions.DropPositions.RightArm:
			return this.targetDockPositions.rightArmTransform;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.Chest:
			return this.targetDockPositions.chestTransform;
		case BodyDockPositions.DropPositions.LeftBack:
			return this.targetDockPositions.leftBackTransform;
		default:
			if (state == BodyDockPositions.DropPositions.RightBack)
			{
				return this.targetDockPositions.rightBackTransform;
			}
			break;
		}
		throw new ArgumentOutOfRangeException();
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000B74F1 File Offset: 0x000B56F1
	public static Transform GetTargetDock(TransferrableObject.PositionState state, VRRig rig)
	{
		return TransferrableObject.GetTargetDock(state, rig.myBodyDockPositions, rig.GetComponent<VRRigAnchorOverrides>());
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x000B7508 File Offset: 0x000B5708
	public static Transform GetTargetDock(TransferrableObject.PositionState state, BodyDockPositions dockPositions, VRRigAnchorOverrides anchorOverrides)
	{
		if (state <= TransferrableObject.PositionState.InRightHand)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftArmTransform);
			case TransferrableObject.PositionState.OnRightArm:
				return anchorOverrides.AnchorOverride(state, dockPositions.rightArmTransform);
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				break;
			case TransferrableObject.PositionState.InLeftHand:
				return anchorOverrides.AnchorOverride(state, dockPositions.leftHandTransform);
			default:
				if (state == TransferrableObject.PositionState.InRightHand)
				{
					return anchorOverrides.AnchorOverride(state, dockPositions.rightHandTransform);
				}
				break;
			}
		}
		else
		{
			if (state == TransferrableObject.PositionState.OnChest)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.chestTransform);
			}
			if (state == TransferrableObject.PositionState.OnLeftShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.leftBackTransform);
			}
			if (state == TransferrableObject.PositionState.OnRightShoulder)
			{
				return anchorOverrides.AnchorOverride(state, dockPositions.rightBackTransform);
			}
		}
		return null;
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x000B75AC File Offset: 0x000B57AC
	private void UpdateFollowXform()
	{
		if (!this.targetRigSet)
		{
			return;
		}
		Transform transform = this.GetAnchor(this.currentState);
		Transform transform2 = transform;
		try
		{
			transform2 = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
		}
		catch
		{
			Debug.LogError("anchorOverrides or targetDock has been destroyed", this);
			this.SetTargetRig(null);
		}
		if (this.currentState != TransferrableObject.PositionState.Dropped && this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		if (this.detatchOnGrab && (this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand))
		{
			base.transform.parent = null;
		}
		if (this.interpState == TransferrableObject.InterpolateState.None)
		{
			try
			{
				if (transform == null)
				{
					return;
				}
				this.startInterpolation |= (transform2 != transform.parent);
			}
			catch
			{
			}
			if (!this.startInterpolation && !this.isGrabAnchorSet && base.transform.parent != transform && transform != base.transform)
			{
				this.startInterpolation = true;
			}
			if (this.startInterpolation)
			{
				Vector3 position = base.transform.position;
				Quaternion rotation = base.transform.rotation;
				if (base.transform.parent != transform && transform != base.transform)
				{
					base.transform.parent = transform;
				}
				transform.parent = transform2;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				if (this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					if (this.flipOnXForLeftHand)
					{
						transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					else if (this.flipOnYForLeftHand)
					{
						transform.localScale = new Vector3(1f, -1f, 1f);
					}
					else
					{
						transform.localScale = Vector3.one;
					}
				}
				else
				{
					transform.localScale = Vector3.one;
				}
				if (Time.frameCount == this.enabledOnFrame || Time.frameCount == this.enabledOnFrame + 1)
				{
					Matrix4x4 rhs = this.GetDefaultTransformationMatrix();
					if ((this.currentState != TransferrableObject.PositionState.InLeftHand || !(this.handPoseLeft != null)) && this.currentState == TransferrableObject.PositionState.InRightHand)
					{
						this.handPoseRight != null;
					}
					Matrix4x4 matrix4x;
					if (this.transferrableItemSlotTransformOverride && this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out matrix4x))
					{
						rhs = matrix4x;
					}
					Matrix4x4 matrix = transform.localToWorldMatrix * rhs;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
					base.transform.localScale = matrix.lossyScale;
				}
				else
				{
					this.interpState = TransferrableObject.InterpolateState.Interpolating;
					if (this.IsMyItem() && this.useGrabType == TransferrableObject.GrabType.Free)
					{
						bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
						if (!flag)
						{
							GameObject rightHand = EquipmentInteractor.instance.rightHand;
						}
						else
						{
							GameObject leftHand = EquipmentInteractor.instance.leftHand;
						}
						Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig);
						this.SetupMatrixForFreeGrab(position, rotation, targetDock, flag);
					}
					this.interpDt = this.interpTime;
					this.interpStartRot = rotation;
					this.interpStartPos = position;
					base.transform.position = position;
					base.transform.rotation = rotation;
				}
				this.startInterpolation = false;
			}
		}
		if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
		{
			Matrix4x4 rhs2 = this.GetDefaultTransformationMatrix();
			if (this.transferrableItemSlotTransformOverride != null)
			{
				if (this.transferrableItemSlotTransformOverrideCachedMatrix == null)
				{
					Matrix4x4 value;
					this.transferrableItemSlotTransformOverrideApplicable = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, transform2, out value);
					this.transferrableItemSlotTransformOverrideCachedMatrix = new Matrix4x4?(value);
				}
				if (this.transferrableItemSlotTransformOverrideApplicable)
				{
					rhs2 = this.transferrableItemSlotTransformOverrideCachedMatrix.Value;
				}
			}
			float t = Mathf.Clamp((this.interpTime - this.interpDt) / this.interpTime, 0f, 1f);
			Mathf.SmoothStep(0f, 1f, t);
			Matrix4x4 matrix2 = transform.localToWorldMatrix * rhs2;
			Transform transform3 = base.transform;
			Vector3 vector = matrix2.Position();
			transform3.position = this.interpStartPos.LerpToUnclamped(vector, t);
			base.transform.rotation = Quaternion.Slerp(this.interpStartRot, matrix2.Rotation(), t);
			base.transform.localScale = rhs2.lossyScale;
			this.interpDt -= Time.deltaTime;
			if (this.interpDt <= 0f)
			{
				transform.parent = transform2;
				this.interpState = TransferrableObject.InterpolateState.None;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				if (this.flipOnXForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(-1f, 1f, 1f);
				}
				if (this.flipOnYForLeftHand && this.currentState == TransferrableObject.PositionState.InLeftHand)
				{
					transform.localScale = new Vector3(1f, -1f, 1f);
				}
				matrix2 = transform.localToWorldMatrix * rhs2;
				base.transform.SetLocalToWorldMatrixNoScale(matrix2);
				base.transform.localScale = rhs2.lossyScale;
			}
		}
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000B7AEC File Offset: 0x000B5CEC
	public virtual void DropItem()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
		this.currentState = TransferrableObject.PositionState.Dropped;
		if (this.worldShareableInstance)
		{
			this.worldShareableInstance.transferableObjectState = this.currentState;
		}
		if (this.canDrop)
		{
			base.transform.parent = null;
			if (this.anchor)
			{
				this.anchor.parent = this.InitialDockObject;
			}
			if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.isKinematic = true;
			}
		}
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000B7C14 File Offset: 0x000B5E14
	protected virtual void OnStateChanged()
	{
		if (this.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
		{
			int num = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
			if (!this.InHand() && num != 0)
			{
				TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
				if (syncOptions == TransferrableObject.SyncOptions.Bool)
				{
					this.ResetStateBools();
					return;
				}
				if (syncOptions != TransferrableObject.SyncOptions.Int)
				{
					return;
				}
				this.SetItemStateInt(0);
			}
		}
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000B7C6C File Offset: 0x000B5E6C
	protected virtual void LateUpdateShared()
	{
		this.disableItem = true;
		if (this.isSceneObject)
		{
			this.disableItem = false;
		}
		else
		{
			for (int i = 0; i < this.ownerRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.ownerRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
				{
					this.disableItem = false;
					break;
				}
			}
			if (this.disableItem)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		if (this.previousState != this.currentState)
		{
			this.previousState = this.currentState;
			if (!this.Attached())
			{
				base.transform.parent = null;
				if (!this.ShouldBeKinematic() && this.rigidbodyInstance.isKinematic)
				{
					this.rigidbodyInstance.isKinematic = false;
				}
			}
			if (this.currentState == TransferrableObject.PositionState.None)
			{
				this.ResetToHome();
			}
			this.transferrableItemSlotTransformOverrideCachedMatrix = null;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.interpState = TransferrableObject.InterpolateState.None;
			}
			this.OnStateChanged();
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			if (!this.canDrop || this.allowReparenting)
			{
				goto IL_15A;
			}
			if (base.transform.parent != null)
			{
				base.transform.parent = null;
			}
			try
			{
				if (this.anchor != null && this.anchor.parent != this.InitialDockObject)
				{
					this.anchor.parent = this.InitialDockObject;
				}
				goto IL_15A;
			}
			catch
			{
				goto IL_15A;
			}
		}
		if (this.currentState != TransferrableObject.PositionState.None)
		{
			this.UpdateFollowXform();
		}
		IL_15A:
		if (this.InHand() && !this.wasHeldShared)
		{
			UnityEvent onHeldShared = this.OnHeldShared;
			if (onHeldShared != null)
			{
				onHeldShared.Invoke();
			}
			this.wasHeldShared = true;
		}
		else if (!this.InHand() && !this.Dropped() && this.wasHeldShared)
		{
			UnityEvent onDockedShared = this.OnDockedShared;
			if (onDockedShared != null)
			{
				onDockedShared.Invoke();
			}
			this.wasHeldShared = false;
		}
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.rigidbodyInstance.isKinematic != this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
			if (this.worldShareableInstance)
			{
				if (this.currentState == TransferrableObject.PositionState.Dropped)
				{
					this.worldShareableInstance.EnableRemoteSync = true;
					return;
				}
				this.worldShareableInstance.EnableRemoteSync = !this.ShouldBeKinematic();
			}
		}
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000B7EA8 File Offset: 0x000B60A8
	public virtual void ResetToHome()
	{
		if (this.isSceneObject)
		{
			this.currentState = TransferrableObject.PositionState.None;
		}
		this.ResetXf();
		if (!this.isRigidbodySet)
		{
			return;
		}
		if (this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000B7EF4 File Offset: 0x000B60F4
	protected void ResetXf()
	{
		if (!this.positionInitialized)
		{
			this.initOffset = base.transform.localPosition;
			this.initRotation = base.transform.localRotation;
		}
		if (this.canDrop || this.allowWorldSharableInstance)
		{
			Transform transform = this.DefaultAnchor();
			if (base.transform != transform && base.transform.parent != transform)
			{
				base.transform.parent = transform;
			}
			if (this.ClearLocalPositionOnReset)
			{
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				base.transform.localScale = Vector3.one;
			}
			if (this.InitialDockObject)
			{
				this.anchor.localPosition = Vector3.zero;
				this.anchor.localRotation = Quaternion.identity;
				this.anchor.localScale = Vector3.one;
			}
			if (this.grabAnchor)
			{
				if (this.grabAnchor.parent != base.transform)
				{
					this.grabAnchor.parent = base.transform;
				}
				this.grabAnchor.localPosition = Vector3.zero;
				this.grabAnchor.localRotation = Quaternion.identity;
				this.grabAnchor.localScale = Vector3.one;
			}
			if (this.transferrableItemSlotTransformOverride)
			{
				Transform transformFromPositionState = this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState);
				if (transformFromPositionState)
				{
					base.transform.position = transformFromPositionState.position;
					base.transform.rotation = transformFromPositionState.rotation;
					return;
				}
				if (this.anchorOverrides != null)
				{
					Transform transform2 = this.GetAnchor(this.currentState);
					Transform targetDock = TransferrableObject.GetTargetDock(this.currentState, this.targetDockPositions, this.anchorOverrides);
					Matrix4x4 rhs = this.GetDefaultTransformationMatrix();
					Matrix4x4 matrix4x;
					if (this.transferrableItemSlotTransformOverride.GetTransformFromPositionState(this.currentState, this.advancedGrabState, targetDock, out matrix4x))
					{
						rhs = matrix4x;
					}
					Matrix4x4 matrix = transform2.localToWorldMatrix * rhs;
					base.transform.SetLocalToWorldMatrixNoScale(matrix);
					base.transform.localScale = matrix.lossyScale;
					return;
				}
			}
			else
			{
				base.transform.SetLocalMatrixRelativeToParent(this.GetDefaultTransformationMatrix());
			}
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000B8134 File Offset: 0x000B6334
	protected void ReDock()
	{
		if (this.IsMyItem())
		{
			this.currentState = this.initState;
		}
		if (this.rigidbodyInstance && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		this.ResetXf();
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000B818C File Offset: 0x000B638C
	private void HandleLocalInput()
	{
		Behaviour[] array2;
		if (this.Dropped())
		{
			foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject.activeSelf)
				{
					gameObject.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (gameObject2.activeSelf)
				{
					gameObject2.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			return;
		}
		if (!this.InHand())
		{
			foreach (GameObject gameObject3 in this.gameObjectsActiveOnlyWhileHeld)
			{
				if (gameObject3.activeSelf)
				{
					gameObject3.SetActive(false);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileHeld;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
			foreach (GameObject gameObject4 in this.gameObjectsActiveOnlyWhileDocked)
			{
				if (!gameObject4.activeSelf)
				{
					gameObject4.SetActive(true);
				}
			}
			array2 = this.behavioursEnabledOnlyWhileDocked;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
			return;
		}
		foreach (GameObject gameObject5 in this.gameObjectsActiveOnlyWhileHeld)
		{
			if (!gameObject5.activeSelf)
			{
				gameObject5.SetActive(true);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileHeld;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = true;
		}
		foreach (GameObject gameObject6 in this.gameObjectsActiveOnlyWhileDocked)
		{
			if (gameObject6.activeSelf)
			{
				gameObject6.SetActive(false);
			}
		}
		array2 = this.behavioursEnabledOnlyWhileDocked;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
		XRNode node = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? XRNode.LeftHand : XRNode.RightHand;
		this.indexTrigger = ControllerInputPoller.TriggerFloat(node);
		bool flag = !this.latched && this.indexTrigger >= this.myThreshold;
		bool flag2 = this.latched && this.indexTrigger < this.myThreshold - this.hysterisis;
		if (flag || this.testActivate)
		{
			this.testActivate = false;
			if (this.CanActivate())
			{
				this.OnActivate();
				return;
			}
		}
		else if (flag2 || this.testDeactivate)
		{
			this.testDeactivate = false;
			if (this.CanDeactivate())
			{
				this.OnDeactivate();
			}
		}
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void LocalMyObjectValidation()
	{
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000B8414 File Offset: 0x000B6614
	protected virtual void LocalPersistanceValidation()
	{
		if (this.maxDistanceFromOriginBeforeRespawn != 0f && Vector3.Distance(base.transform.position, this.originPoint.position) > this.maxDistanceFromOriginBeforeRespawn)
		{
			if (this.audioSrc != null && this.resetPositionAudioClip != null)
			{
				this.audioSrc.GTPlayOneShot(this.resetPositionAudioClip, 1f);
			}
			if (this.currentState != TransferrableObject.PositionState.Dropped)
			{
				this.DropItem();
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			base.transform.position = this.originPoint.position;
			if (!this.rigidbodyInstance.isKinematic)
			{
				this.rigidbodyInstance.linearVelocity = Vector3.zero;
			}
		}
		if (this.rigidbodyInstance && this.rigidbodyInstance.linearVelocity.sqrMagnitude > 10000f)
		{
			Debug.Log("Moving too fast, Assuming ive fallen out of the map. Ressetting position", this);
			this.ResetToHome();
		}
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000B8514 File Offset: 0x000B6714
	public void ObjectBeingTaken()
	{
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, true);
		}
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			EquipmentInteractor.instance.UpdateHandEquipment(null, false);
		}
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000B85B4 File Offset: 0x000B67B4
	protected virtual void LateUpdateLocal()
	{
		this.wasHover = this.isHover;
		this.isHover = false;
		this.LocalPersistanceValidation();
		if (NetworkSystem.Instance.InRoom)
		{
			if (!this.isSceneObject && this.IsLocalObject())
			{
				this.myRig.SetTransferrablePosStates(this.objectIndex, this.currentState);
				this.myRig.SetTransferrableItemStates(this.objectIndex, this.itemState);
				this.myRig.SetTransferrableDockPosition(this.objectIndex, this.storedZone);
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.transferableObjectState = this.currentState;
				this.worldShareableInstance.transferableObjectItemState = this.itemState;
			}
		}
		this.HandleLocalInput();
		if (this.InHand() && !this.wasHeldLocal)
		{
			UnityEvent onHeldLocal = this.OnHeldLocal;
			if (onHeldLocal != null)
			{
				onHeldLocal.Invoke();
			}
			this.wasHeldLocal = true;
			return;
		}
		if (!this.InHand() && !this.Dropped() && this.wasHeldLocal)
		{
			UnityEvent onDockedLocal = this.OnDockedLocal;
			if (onDockedLocal != null)
			{
				onDockedLocal.Invoke();
			}
			this.wasHeldLocal = false;
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000B86CC File Offset: 0x000B68CC
	protected void LateUpdateReplicatedSceneObject()
	{
		if (this.myOnlineRig != null)
		{
			this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		}
		if (this.worldShareableInstance != null)
		{
			this.currentState = this.worldShareableInstance.transferableObjectState;
			this.itemState = this.worldShareableInstance.transferableObjectItemState;
			this.worldShareableInstance.EnableRemoteSync = (!this.ShouldBeKinematic() || this.currentState == TransferrableObject.PositionState.Dropped);
		}
		if (this.isRigidbodySet && this.ShouldBeKinematic() && !this.rigidbodyInstance.isKinematic)
		{
			this.rigidbodyInstance.isKinematic = true;
		}
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000B8770 File Offset: 0x000B6970
	protected virtual void LateUpdateReplicated()
	{
		if (this.isSceneObject || this.shareable)
		{
			this.LateUpdateReplicatedSceneObject();
			return;
		}
		if (this.myOnlineRig == null)
		{
			return;
		}
		this.currentState = this.myOnlineRig.TransferrablePosStates(this.objectIndex);
		if (!this.ValidateState(this.currentState))
		{
			if (this.previousState == TransferrableObject.PositionState.None)
			{
				base.gameObject.Disable();
			}
			this.currentState = this.previousState;
		}
		if (this.isRigidbodySet)
		{
			this.rigidbodyInstance.isKinematic = this.ShouldBeKinematic();
		}
		bool flag = true;
		this.previousItemState = this.itemState;
		this.itemState = this.myOnlineRig.TransferrableItemStates(this.objectIndex);
		this.storedZone = this.myOnlineRig.TransferrableDockPosition(this.objectIndex);
		int num = this.myOnlineRig.ActiveTransferrableObjectIndexLength();
		for (int i = 0; i < num; i++)
		{
			if (this.myOnlineRig.ActiveTransferrableObjectIndex(i) == this.myIndex)
			{
				flag = false;
				foreach (GameObject gameObject in this.gameObjectsActiveOnlyWhileHeld)
				{
					bool flag2 = this.InHand();
					if (gameObject.activeSelf != flag2)
					{
						gameObject.SetActive(flag2);
					}
				}
				Behaviour[] array2 = this.behavioursEnabledOnlyWhileHeld;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = this.InHand();
				}
				foreach (GameObject gameObject2 in this.gameObjectsActiveOnlyWhileDocked)
				{
					bool flag3 = this.InHand();
					if (gameObject2.activeSelf == flag3)
					{
						gameObject2.SetActive(!flag3);
					}
				}
				array2 = this.behavioursEnabledOnlyWhileDocked;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].enabled = !this.InHand();
				}
			}
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.previousItemState != this.itemState)
		{
			int num2 = (int)(this.previousItemState & (TransferrableObject.ItemStates)(-65));
			int num3 = (int)(this.itemState & (TransferrableObject.ItemStates)(-65));
			if (num2 != num3)
			{
				this.OnNetworkItemStateChanged(num3);
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000B8984 File Offset: 0x000B6B84
	public virtual void ResetToDefaultState()
	{
		this.canAutoGrabLeft = true;
		this.canAutoGrabRight = true;
		this.wasHover = false;
		this.isHover = false;
		if (!this.IsLocalObject() && this.worldShareableInstance && !this.isSceneObject)
		{
			if (this.IsMyItem())
			{
				return;
			}
			this.worldShareableInstance.GetComponent<RequestableOwnershipGuard>().RequestOwnershipImmediately(delegate
			{
			});
		}
		this.ResetXf();
		TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
		if (syncOptions == TransferrableObject.SyncOptions.Bool)
		{
			this.ResetStateBools();
			return;
		}
		if (syncOptions != TransferrableObject.SyncOptions.Int)
		{
			return;
		}
		this.SetItemStateInt(0);
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000B8A28 File Offset: 0x000B6C28
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!(this.worldShareableInstance == null) && !this.worldShareableInstance.guard.isTrulyMine)
		{
			if (!this.IsGrabbable())
			{
				return;
			}
			this.worldShareableInstance.guard.RequestOwnershipImmediately(delegate
			{
			});
		}
		if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState != TransferrableObject.PositionState.OnLeftArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InRightHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabLeft = false;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.startInterpolation = true;
			}
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InLeftHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InLeftHand, EquipmentInteractor.instance.leftHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, true);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState != TransferrableObject.PositionState.OnRightArm)
		{
			if (this.currentState == TransferrableObject.PositionState.InLeftHand && this.disableStealing)
			{
				return;
			}
			this.canAutoGrabRight = false;
			if (this.interpState == TransferrableObject.InterpolateState.Interpolating)
			{
				this.startInterpolation = true;
			}
			this.interpState = TransferrableObject.InterpolateState.None;
			this.currentState = TransferrableObject.PositionState.InRightHand;
			if (this.transferrableItemSlotTransformOverride)
			{
				this.advancedGrabState = this.transferrableItemSlotTransformOverride.GetAdvancedItemStateFromHand(TransferrableObject.PositionState.InRightHand, EquipmentInteractor.instance.rightHand.transform, TransferrableObject.GetTargetDock(this.currentState, GorillaTagger.Instance.offlineVRRig));
			}
			EquipmentInteractor.instance.UpdateHandEquipment(this, false);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		if (this.rigidbodyInstance && !this.rigidbodyInstance.isKinematic && this.ShouldBeKinematic())
		{
			this.rigidbodyInstance.isKinematic = true;
		}
		PlayerGameEvents.GrabbedObject(this.interactEventName);
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000B8C80 File Offset: 0x000B6E80
	private void SetupMatrixForFreeGrab(Vector3 worldPosition, Quaternion worldRotation, Transform attachPoint, bool leftHand)
	{
		Quaternion rotation = attachPoint.transform.rotation;
		Vector3 position = attachPoint.transform.position;
		Quaternion localRotation = Quaternion.Inverse(rotation) * worldRotation;
		Vector3 localPosition = Quaternion.Inverse(rotation) * (worldPosition - position);
		this.OnHandMatrixUpdate(localPosition, localRotation, leftHand);
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x000B8CD3 File Offset: 0x000B6ED3
	protected void SetupHandMatrix(Vector3 leftHandPos, Quaternion leftHandRot, Vector3 rightHandPos, Quaternion rightHandRot)
	{
		this.leftHandMatrix = Matrix4x4.TRS(leftHandPos, leftHandRot, Vector3.one);
		this.rightHandMatrix = Matrix4x4.TRS(rightHandPos, rightHandRot, Vector3.one);
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
	{
	}

	// Token: 0x06002257 RID: 8791 RVA: 0x000B8CFC File Offset: 0x000B6EFC
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.IsMyItem())
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (!this.IsHeld())
		{
			return false;
		}
		if (releasingHand == EquipmentInteractor.instance.leftHand)
		{
			this.canAutoGrabLeft = true;
		}
		else
		{
			this.canAutoGrabRight = true;
		}
		if (zoneReleased != null)
		{
			bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.LeftArm;
			bool flag2 = this.currentState == TransferrableObject.PositionState.InRightHand && zoneReleased.dropPosition == BodyDockPositions.DropPositions.RightArm;
			if (flag || flag2)
			{
				return false;
			}
			if (this.targetDockPositions.DropZoneStorageUsed(zoneReleased.dropPosition) == -1 && zoneReleased.forBodyDock == this.targetDockPositions && (zoneReleased.dropPosition & this.dockPositions) != BodyDockPositions.DropPositions.None)
			{
				this.storedZone = zoneReleased.dropPosition;
			}
		}
		bool flag3 = false;
		this.interpState = TransferrableObject.InterpolateState.None;
		if (this.isSceneObject || this.canDrop || this.allowWorldSharableInstance)
		{
			if (!this.rigidbodyInstance)
			{
				return false;
			}
			if (this.worldShareableInstance)
			{
				this.worldShareableInstance.EnableRemoteSync = true;
			}
			if (!flag3)
			{
				this.currentState = TransferrableObject.PositionState.Dropped;
			}
			if (this.rigidbodyInstance.isKinematic && !this.ShouldBeKinematic())
			{
				this.rigidbodyInstance.isKinematic = false;
			}
			GorillaVelocityEstimator component = base.GetComponent<GorillaVelocityEstimator>();
			if (component != null && this.rigidbodyInstance != null)
			{
				this.rigidbodyInstance.linearVelocity = component.linearVelocity;
				this.rigidbodyInstance.angularVelocity = component.angularVelocity;
			}
		}
		else
		{
			bool flag4 = this.allowWorldSharableInstance;
		}
		this.DropItemCleanup();
		EquipmentInteractor.instance.ForceDropEquipment(this);
		PlayerGameEvents.DroppedObject(this.interactEventName);
		return true;
	}

	// Token: 0x06002258 RID: 8792 RVA: 0x000B8EB0 File Offset: 0x000B70B0
	public override void DropItemCleanup()
	{
		if (this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		BodyDockPositions.DropPositions dropPositions = this.storedZone;
		switch (dropPositions)
		{
		case BodyDockPositions.DropPositions.LeftArm:
			this.currentState = TransferrableObject.PositionState.OnLeftArm;
			return;
		case BodyDockPositions.DropPositions.RightArm:
			this.currentState = TransferrableObject.PositionState.OnRightArm;
			return;
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
			break;
		case BodyDockPositions.DropPositions.Chest:
			this.currentState = TransferrableObject.PositionState.OnChest;
			return;
		default:
			if (dropPositions == BodyDockPositions.DropPositions.LeftBack)
			{
				this.currentState = TransferrableObject.PositionState.OnLeftShoulder;
				return;
			}
			if (dropPositions != BodyDockPositions.DropPositions.RightBack)
			{
				return;
			}
			this.currentState = TransferrableObject.PositionState.OnRightShoulder;
			break;
		}
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x000B8F20 File Offset: 0x000B7120
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
		if (!this.IsGrabbable())
		{
			return;
		}
		if (!this.wasHover)
		{
			GorillaTagger.Instance.StartVibration(hoveringHand == EquipmentInteractor.instance.leftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
		}
		this.isHover = true;
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x000B8F84 File Offset: 0x000B7184
	protected void ActivateItemFX(float hapticStrength, float hapticDuration, int soundIndex, float soundVolume)
	{
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand;
		if (this.myRig.netView != null)
		{
			this.myRig.netView.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				soundIndex,
				flag,
				0.1f
			});
		}
		this.myRig.PlayHandTapLocal(soundIndex, flag, soundVolume);
		GorillaTagger.Instance.StartVibration(flag, hapticStrength, hapticDuration);
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void PlayNote(int note, float volume)
	{
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000B9005 File Offset: 0x000B7205
	public virtual bool AutoGrabTrue(bool leftGrabbingHand)
	{
		if (!leftGrabbingHand)
		{
			return this.canAutoGrabRight;
		}
		return this.canAutoGrabLeft;
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool CanActivate()
	{
		return true;
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000B9017 File Offset: 0x000B7217
	public virtual void OnActivate()
	{
		this.latched = true;
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x000B9020 File Offset: 0x000B7220
	public virtual void OnDeactivate()
	{
		this.latched = false;
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x000B9029 File Offset: 0x000B7229
	public virtual bool IsMyItem()
	{
		return GorillaTagger.Instance == null || (this.targetRig != null && this.targetRig == GorillaTagger.Instance.offlineVRRig);
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x000B9053 File Offset: 0x000B7253
	protected virtual bool IsHeld()
	{
		return EquipmentInteractor.instance != null && (EquipmentInteractor.instance.leftHandHeldEquipment == this || EquipmentInteractor.instance.rightHandHeldEquipment == this);
	}

	// Token: 0x06002263 RID: 8803 RVA: 0x000B9080 File Offset: 0x000B7280
	public virtual bool IsGrabbable()
	{
		return this.IsMyItem() || ((this.isSceneObject || this.shareable) && (this.isSceneObject || this.shareable) && (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None));
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x000B90DF File Offset: 0x000B72DF
	public bool InHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06002265 RID: 8805 RVA: 0x000B90F5 File Offset: 0x000B72F5
	public bool Dropped()
	{
		return this.currentState == TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x000B9104 File Offset: 0x000B7304
	public bool InLeftHand()
	{
		return this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x000B910F File Offset: 0x000B730F
	public bool InRightHand()
	{
		return this.currentState == TransferrableObject.PositionState.InRightHand;
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x000B911A File Offset: 0x000B731A
	public bool OnChest()
	{
		return this.currentState == TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x06002269 RID: 8809 RVA: 0x000B9126 File Offset: 0x000B7326
	public bool OnShoulder()
	{
		return this.currentState == TransferrableObject.PositionState.OnLeftShoulder || this.currentState == TransferrableObject.PositionState.OnRightShoulder;
	}

	// Token: 0x0600226A RID: 8810 RVA: 0x000B913E File Offset: 0x000B733E
	protected NetPlayer OwningPlayer()
	{
		if (this.myRig == null)
		{
			return this.myOnlineRig.netView.Owner;
		}
		return NetworkSystem.Instance.LocalPlayer;
	}

	// Token: 0x0600226B RID: 8811 RVA: 0x000B916C File Offset: 0x000B736C
	public bool ValidateState(TransferrableObject.PositionState state)
	{
		if (state <= TransferrableObject.PositionState.OnChest)
		{
			switch (state)
			{
			case TransferrableObject.PositionState.OnLeftArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnRightArm:
				if ((this.dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None)
				{
					return true;
				}
				return false;
			case TransferrableObject.PositionState.OnLeftArm | TransferrableObject.PositionState.OnRightArm:
				return false;
			case TransferrableObject.PositionState.InLeftHand:
				break;
			default:
				if (state != TransferrableObject.PositionState.InRightHand)
				{
					if (state != TransferrableObject.PositionState.OnChest)
					{
						return false;
					}
					if ((this.dockPositions & BodyDockPositions.DropPositions.Chest) != BodyDockPositions.DropPositions.None)
					{
						return true;
					}
					return false;
				}
				break;
			}
			return true;
		}
		if (state != TransferrableObject.PositionState.OnLeftShoulder)
		{
			if (state != TransferrableObject.PositionState.OnRightShoulder)
			{
				if (state == TransferrableObject.PositionState.Dropped)
				{
					return this.canDrop || this.shareable;
				}
			}
			else if ((this.dockPositions & BodyDockPositions.DropPositions.RightBack) != BodyDockPositions.DropPositions.None)
			{
				return true;
			}
		}
		else if ((this.dockPositions & BodyDockPositions.DropPositions.LeftBack) != BodyDockPositions.DropPositions.None)
		{
			return true;
		}
		return false;
	}

	// Token: 0x0600226C RID: 8812 RVA: 0x000B920C File Offset: 0x000B740C
	private void OnNetworkItemStateChanged(int stateBits)
	{
		TransferrableObject.SyncOptions syncOptions = this.networkedStateEvents;
		if (syncOptions != TransferrableObject.SyncOptions.Bool)
		{
			if (syncOptions != TransferrableObject.SyncOptions.Int)
			{
				return;
			}
			UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
			if (onItemStateIntChanged == null)
			{
				return;
			}
			onItemStateIntChanged.Invoke(stateBits);
		}
		else
		{
			int num = (int)(this.previousItemState & TransferrableObject.ItemStates.State0);
			int num2 = (int)(this.itemState & TransferrableObject.ItemStates.State0);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolFalse = this.OnItemStateBoolFalse;
				if (onItemStateBoolFalse != null)
				{
					onItemStateBoolFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolTrue = this.OnItemStateBoolTrue;
				if (onItemStateBoolTrue != null)
				{
					onItemStateBoolTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State1);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State1);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
				if (onItemStateBoolBFalse != null)
				{
					onItemStateBoolBFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
				if (onItemStateBoolBTrue != null)
				{
					onItemStateBoolBTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State2);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State2);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
				if (onItemStateBoolCFalse != null)
				{
					onItemStateBoolCFalse.Invoke();
				}
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
				if (onItemStateBoolCTrue != null)
				{
					onItemStateBoolCTrue.Invoke();
				}
			}
			num = (int)(this.previousItemState & TransferrableObject.ItemStates.State3);
			num2 = (int)(this.itemState & TransferrableObject.ItemStates.State3);
			if (num != num2 && num2 == 0)
			{
				UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
				if (onItemStateBoolDFalse == null)
				{
					return;
				}
				onItemStateBoolDFalse.Invoke();
				return;
			}
			else if (num != num2)
			{
				UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
				if (onItemStateBoolDTrue == null)
				{
					return;
				}
				onItemStateBoolDTrue.Invoke();
				return;
			}
		}
	}

	// Token: 0x0600226D RID: 8813 RVA: 0x000B933F File Offset: 0x000B753F
	public void ToggleNetworkedItemStateBool()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(1);
	}

	// Token: 0x0600226E RID: 8814 RVA: 0x000B9352 File Offset: 0x000B7552
	public void ToggleNetworkedItemStateBoolB()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(2);
	}

	// Token: 0x0600226F RID: 8815 RVA: 0x000B9365 File Offset: 0x000B7565
	public void ToggleNetworkedItemStateBoolC()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(4);
	}

	// Token: 0x06002270 RID: 8816 RVA: 0x000B9378 File Offset: 0x000B7578
	public void ToggleNetworkedItemStateBoolD()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.ToggleStateBit(8);
	}

	// Token: 0x06002271 RID: 8817 RVA: 0x000B938C File Offset: 0x000B758C
	protected void ResetStateBools()
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		if (!this.IsLocalObject())
		{
			return;
		}
		int bitmask = 15;
		this.SetStateBit(false, bitmask);
	}

	// Token: 0x06002272 RID: 8818 RVA: 0x000B93B7 File Offset: 0x000B75B7
	public void SetItemStateBool(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 1);
	}

	// Token: 0x06002273 RID: 8819 RVA: 0x000B93CB File Offset: 0x000B75CB
	public void SetItemStateBoolB(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 2);
	}

	// Token: 0x06002274 RID: 8820 RVA: 0x000B93DF File Offset: 0x000B75DF
	public void SetItemStateBoolC(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 4);
	}

	// Token: 0x06002275 RID: 8821 RVA: 0x000B93F3 File Offset: 0x000B75F3
	public void SetItemStateBoolD(bool newState)
	{
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Bool)
		{
			return;
		}
		this.SetStateBit(newState, 8);
	}

	// Token: 0x06002276 RID: 8822 RVA: 0x000B9408 File Offset: 0x000B7608
	private void SetStateBit(bool value, int bitmask)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		int num = (int)this.itemState;
		if (value)
		{
			num |= bitmask;
		}
		else
		{
			num &= ~bitmask;
		}
		TransferrableObject.ItemStates itemStates = (TransferrableObject.ItemStates)num;
		if (this.itemState != itemStates)
		{
			this.previousItemState = this.itemState;
			this.itemState = itemStates;
			this.OnNetworkItemStateChanged(num);
		}
	}

	// Token: 0x06002277 RID: 8823 RVA: 0x000B9458 File Offset: 0x000B7658
	private void ToggleStateBit(int bitmask)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		bool flag = (this.itemState & (TransferrableObject.ItemStates)bitmask) != (TransferrableObject.ItemStates)0;
		int num = (int)this.itemState;
		if (!flag)
		{
			num |= bitmask;
		}
		else
		{
			num &= ~bitmask;
		}
		this.previousItemState = this.itemState;
		this.itemState = (TransferrableObject.ItemStates)num;
		this.OnNetworkItemStateChanged(num);
	}

	// Token: 0x06002278 RID: 8824 RVA: 0x000B94A4 File Offset: 0x000B76A4
	public void SetItemStateInt(int newState)
	{
		if (!this.IsLocalObject())
		{
			return;
		}
		if (this.networkedStateEvents != TransferrableObject.SyncOptions.Int)
		{
			return;
		}
		newState = Mathf.Clamp(newState, 0, 63);
		int num = newState & -65;
		int num2 = (int)(this.itemState & TransferrableObject.ItemStates.Part0Held);
		TransferrableObject.ItemStates itemStates = (TransferrableObject.ItemStates)(num | num2);
		if (this.itemState != itemStates)
		{
			this.previousItemState = this.itemState;
			this.itemState = itemStates;
			this.OnNetworkItemStateChanged(num);
		}
	}

	// Token: 0x06002279 RID: 8825 RVA: 0x000B9508 File Offset: 0x000B7708
	public virtual void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (toPlayer != null && toPlayer.Equals(fromPlayer))
		{
			return;
		}
		if (object.Equals(fromPlayer, NetworkSystem.Instance.LocalPlayer) && this.IsHeld())
		{
			this.DropItem();
		}
		if (toPlayer == null)
		{
			this.SetTargetRig(null);
			return;
		}
		this.rigidbodyInstance.useGravity = (this.shouldUseGravity && object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer));
		if (!this.shareable && !this.isSceneObject)
		{
			return;
		}
		if (object.Equals(toPlayer, NetworkSystem.Instance.LocalPlayer))
		{
			if (GorillaTagger.Instance == null)
			{
				Debug.LogError("OnOwnershipTransferred has been initiated too quickly, The local player is not ready");
				return;
			}
			this.SetTargetRig(GorillaTagger.Instance.offlineVRRig);
			return;
		}
		else
		{
			VRRig exists = GorillaGameManager.StaticFindRigForPlayer(toPlayer);
			if (!exists)
			{
				Debug.LogError("failed to find target rig for ownershiptransfer");
				return;
			}
			this.SetTargetRig(exists);
			return;
		}
	}

	// Token: 0x0600227A RID: 8826 RVA: 0x000B95E0 File Offset: 0x000B77E0
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return false;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.allowPlayerStealing || this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.isSceneObject)
		{
			return false;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000B9698 File Offset: 0x000B7898
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(fromPlayer, out rigContainer))
		{
			return true;
		}
		if (Vector3.SqrMagnitude(base.transform.position - rigContainer.transform.position) > 16f)
		{
			Debug.Log("Player whos trying to get is too far, Denying takeover");
			return false;
		}
		if (this.currentState == TransferrableObject.PositionState.Dropped || this.currentState == TransferrableObject.PositionState.None)
		{
			return true;
		}
		if (this.canDrop)
		{
			if (this.ownerRig == null || this.ownerRig.creator == null)
			{
				return true;
			}
			if (this.ownerRig.creator.Equals(fromPlayer))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000B973C File Offset: 0x000B793C
	public void OnMyOwnerLeft()
	{
		if (this.currentState == TransferrableObject.PositionState.None || this.currentState == TransferrableObject.PositionState.Dropped)
		{
			return;
		}
		this.DropItem();
		if (this.anchor)
		{
			this.anchor.parent = this.InitialDockObject;
			this.anchor.localPosition = Vector3.zero;
			this.anchor.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000B97A3 File Offset: 0x000B79A3
	public void OnMyCreatorLeft()
	{
		this.OnItemDestroyedOrDisabled();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000B97B8 File Offset: 0x000B79B8
	public bool BuildValidationCheck()
	{
		int num = 0;
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightArm))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.Chest))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.LeftBack))
		{
			num++;
		}
		if (this.storedZone.HasFlag(BodyDockPositions.DropPositions.RightBack))
		{
			num++;
		}
		if (num > 1)
		{
			Debug.LogError("transferrableitem is starting with multiple storedzones: " + base.transform.parent.name, base.gameObject);
			return false;
		}
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((GTPlayer.LocomotionEnabledLayers & 1 << componentsInChildren[i].gameObject.layer) != 0)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Holdable cosmetic ",
					base.transform.name,
					" has a collider on a player movement layer! Players will fly around! Dear god, please fix! It's on the ",
					componentsInChildren[i].name,
					" collider"
				}), base.gameObject);
				return false;
			}
		}
		return true;
	}

	// Token: 0x04002D05 RID: 11525
	private VRRig _myRig;

	// Token: 0x04002D07 RID: 11527
	private VRRig _myOnlineRig;

	// Token: 0x04002D09 RID: 11529
	public bool latched;

	// Token: 0x04002D0A RID: 11530
	private float indexTrigger;

	// Token: 0x04002D0B RID: 11531
	public bool testActivate;

	// Token: 0x04002D0C RID: 11532
	public bool testDeactivate;

	// Token: 0x04002D0D RID: 11533
	[Tooltip("When the grip/trigger input is greater than this value the transferrable object is activated")]
	public float myThreshold = 0.8f;

	// Token: 0x04002D0E RID: 11534
	[Tooltip("When the grip/trigger input is less than (myThreshold - hysterisis) the transferrable object is deactivated")]
	public float hysterisis = 0.05f;

	// Token: 0x04002D0F RID: 11535
	[Tooltip("Set the x scale to -1 when held in left hand")]
	public bool flipOnXForLeftHand;

	// Token: 0x04002D10 RID: 11536
	[Tooltip("Set the y scale to -1 when held in left hand")]
	public bool flipOnYForLeftHand;

	// Token: 0x04002D11 RID: 11537
	[Tooltip("Set the x scale to -1 when docked on left arm")]
	public bool flipOnXForLeftArm;

	// Token: 0x04002D12 RID: 11538
	[Tooltip("disable grabbing the item from out of your other hand")]
	public bool disableStealing;

	// Token: 0x04002D13 RID: 11539
	[Tooltip("Allow other players to pick up this item")]
	public bool allowPlayerStealing;

	// Token: 0x04002D14 RID: 11540
	private TransferrableObject.PositionState initState;

	// Token: 0x04002D15 RID: 11541
	public TransferrableObject.ItemStates itemState;

	// Token: 0x04002D16 RID: 11542
	protected TransferrableObject.ItemStates previousItemState;

	// Token: 0x04002D17 RID: 11543
	protected const int HELD_BIT_MASK = 64;

	// Token: 0x04002D18 RID: 11544
	private const int BOOL_A_BITMASK = 1;

	// Token: 0x04002D19 RID: 11545
	private const int BOOL_B_BITMASK = 2;

	// Token: 0x04002D1A RID: 11546
	private const int BOOL_C_BITMASK = 4;

	// Token: 0x04002D1B RID: 11547
	private const int BOOL_D_BITMASK = 8;

	// Token: 0x04002D1C RID: 11548
	[DevInspectorShow]
	public BodyDockPositions.DropPositions storedZone;

	// Token: 0x04002D1D RID: 11549
	protected TransferrableObject.PositionState previousState;

	// Token: 0x04002D1E RID: 11550
	[DevInspectorYellow]
	[DevInspectorShow]
	public TransferrableObject.PositionState currentState;

	// Token: 0x04002D1F RID: 11551
	public BodyDockPositions.DropPositions dockPositions;

	// Token: 0x04002D20 RID: 11552
	[DevInspectorCyan]
	[DevInspectorShow]
	public AdvancedItemState advancedGrabState;

	// Token: 0x04002D21 RID: 11553
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig targetRig;

	// Token: 0x04002D22 RID: 11554
	[HideInInspector]
	public bool targetRigSet;

	// Token: 0x04002D23 RID: 11555
	public TransferrableObject.GrabType useGrabType;

	// Token: 0x04002D24 RID: 11556
	[DevInspectorShow]
	[DevInspectorCyan]
	public VRRig ownerRig;

	// Token: 0x04002D25 RID: 11557
	[DebugReadout]
	[NonSerialized]
	public BodyDockPositions targetDockPositions;

	// Token: 0x04002D26 RID: 11558
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04002D27 RID: 11559
	public bool canAutoGrabLeft;

	// Token: 0x04002D28 RID: 11560
	public bool canAutoGrabRight;

	// Token: 0x04002D29 RID: 11561
	[DevInspectorShow]
	public int objectIndex;

	// Token: 0x04002D2A RID: 11562
	[NonSerialized]
	public Transform anchor;

	// Token: 0x04002D2B RID: 11563
	[Tooltip("In Functional prefab, assign to the Collider to grab this object")]
	public InteractionPoint gripInteractor;

	// Token: 0x04002D2C RID: 11564
	[Tooltip("(Optional) Use this to override the transform used when the object is in the hand.\nExample: 'GHOST BALLOON' uses child 'grabPtAnchor' which is the end of the balloon's string.")]
	public Transform grabAnchor;

	// Token: 0x04002D2D RID: 11565
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Left mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseLeft;

	// Token: 0x04002D2E RID: 11566
	[Tooltip("(Optional) Use this (with the GorillaHandClosed_Right mesh) to intuitively define how\nthe player holds this object, by placing a representation of their hand gripping it.")]
	public Transform handPoseRight;

	// Token: 0x04002D2F RID: 11567
	[HideInInspector]
	public bool isGrabAnchorSet;

	// Token: 0x04002D30 RID: 11568
	private static Vector3 handPoseRightReferencePoint = new Vector3(-0.0141f, 0.0065f, -0.278f);

	// Token: 0x04002D31 RID: 11569
	private static Quaternion handPoseRightReferenceRotation = Quaternion.Euler(-2.058f, -17.2f, 65.05f);

	// Token: 0x04002D32 RID: 11570
	private static Vector3 handPoseLeftReferencePoint = new Vector3(0.0136f, 0.0045f, -0.2809f);

	// Token: 0x04002D33 RID: 11571
	private static Quaternion handPoseLeftReferenceRotation = Quaternion.Euler(-0.58f, 21.356f, -63.965f);

	// Token: 0x04002D34 RID: 11572
	public TransferrableItemSlotTransformOverride transferrableItemSlotTransformOverride;

	// Token: 0x04002D35 RID: 11573
	public int myIndex;

	// Token: 0x04002D36 RID: 11574
	[Tooltip("(Optional) objects to enable when held in hand and disable when not in hand")]
	public GameObject[] gameObjectsActiveOnlyWhileHeld;

	// Token: 0x04002D37 RID: 11575
	[Tooltip("(Optional) objects to disable when held in hand and enable when not in hand")]
	public GameObject[] gameObjectsActiveOnlyWhileDocked;

	// Token: 0x04002D38 RID: 11576
	[Tooltip("(Optional) components to enable when held in hand and disable when not in hand")]
	public Behaviour[] behavioursEnabledOnlyWhileHeld;

	// Token: 0x04002D39 RID: 11577
	[Tooltip("(Optional) components to disable when held in hand and enable when not in hand")]
	public Behaviour[] behavioursEnabledOnlyWhileDocked;

	// Token: 0x04002D3A RID: 11578
	[SerializeField]
	protected internal WorldShareableItem worldShareableInstance;

	// Token: 0x04002D3B RID: 11579
	private float interpTime = 0.2f;

	// Token: 0x04002D3C RID: 11580
	private float interpDt;

	// Token: 0x04002D3D RID: 11581
	private Vector3 interpStartPos;

	// Token: 0x04002D3E RID: 11582
	private Quaternion interpStartRot;

	// Token: 0x04002D3F RID: 11583
	protected int enabledOnFrame = -1;

	// Token: 0x04002D40 RID: 11584
	protected Vector3 initOffset;

	// Token: 0x04002D41 RID: 11585
	protected Quaternion initRotation;

	// Token: 0x04002D42 RID: 11586
	private Matrix4x4 initMatrix = Matrix4x4.identity;

	// Token: 0x04002D43 RID: 11587
	private Matrix4x4 leftHandMatrix = Matrix4x4.identity;

	// Token: 0x04002D44 RID: 11588
	private Matrix4x4 rightHandMatrix = Matrix4x4.identity;

	// Token: 0x04002D45 RID: 11589
	private bool positionInitialized;

	// Token: 0x04002D46 RID: 11590
	public bool isSceneObject;

	// Token: 0x04002D47 RID: 11591
	public Rigidbody rigidbodyInstance;

	// Token: 0x04002D4A RID: 11594
	public bool canDrop;

	// Token: 0x04002D4B RID: 11595
	[Tooltip("completely drop the item instead of auto-returning to a stored zone")]
	public bool allowReparenting;

	// Token: 0x04002D4C RID: 11596
	[Tooltip("(Scene object) has a worldSharableInstance")]
	public bool shareable;

	// Token: 0x04002D4D RID: 11597
	[Tooltip("(Balloon) Unparent this object from the rig when grabbed")]
	public bool detatchOnGrab;

	// Token: 0x04002D4E RID: 11598
	[Tooltip("(Balloon) is this cosmetic droppable in the world")]
	public bool allowWorldSharableInstance;

	// Token: 0x04002D4F RID: 11599
	[ItemCanBeNull]
	public Transform originPoint;

	// Token: 0x04002D50 RID: 11600
	[ItemCanBeNull]
	public float maxDistanceFromOriginBeforeRespawn;

	// Token: 0x04002D51 RID: 11601
	public AudioClip resetPositionAudioClip;

	// Token: 0x04002D52 RID: 11602
	public float maxDistanceFromTargetPlayerBeforeRespawn;

	// Token: 0x04002D53 RID: 11603
	private bool wasHover;

	// Token: 0x04002D54 RID: 11604
	private bool isHover;

	// Token: 0x04002D55 RID: 11605
	private bool disableItem;

	// Token: 0x04002D56 RID: 11606
	protected bool loaded;

	// Token: 0x04002D57 RID: 11607
	public bool ClearLocalPositionOnReset;

	// Token: 0x04002D58 RID: 11608
	[SerializeField]
	protected TransferrableObject.SyncOptions networkedStateEvents;

	// Token: 0x04002D59 RID: 11609
	[SerializeField]
	protected bool resetOnDocked = true;

	// Token: 0x04002D5A RID: 11610
	[SerializeField]
	protected string boolADebugName;

	// Token: 0x04002D5B RID: 11611
	[SerializeField]
	protected UnityEvent OnItemStateBoolTrue;

	// Token: 0x04002D5C RID: 11612
	[SerializeField]
	protected UnityEvent OnItemStateBoolFalse;

	// Token: 0x04002D5D RID: 11613
	[SerializeField]
	protected string boolBDebugName;

	// Token: 0x04002D5E RID: 11614
	[SerializeField]
	protected UnityEvent OnItemStateBoolBTrue;

	// Token: 0x04002D5F RID: 11615
	[SerializeField]
	protected UnityEvent OnItemStateBoolBFalse;

	// Token: 0x04002D60 RID: 11616
	[SerializeField]
	protected string boolCDebugName;

	// Token: 0x04002D61 RID: 11617
	[SerializeField]
	protected UnityEvent OnItemStateBoolCTrue;

	// Token: 0x04002D62 RID: 11618
	[SerializeField]
	protected UnityEvent OnItemStateBoolCFalse;

	// Token: 0x04002D63 RID: 11619
	[SerializeField]
	protected string boolDDebugName;

	// Token: 0x04002D64 RID: 11620
	[SerializeField]
	protected UnityEvent OnItemStateBoolDTrue;

	// Token: 0x04002D65 RID: 11621
	[SerializeField]
	protected UnityEvent OnItemStateBoolDFalse;

	// Token: 0x04002D66 RID: 11622
	[SerializeField]
	protected UnityEvent<int> OnItemStateIntChanged;

	// Token: 0x04002D67 RID: 11623
	[FormerlySerializedAs("OnUndocked")]
	[SerializeField]
	private UnityEvent OnHeldLocal;

	// Token: 0x04002D68 RID: 11624
	[SerializeField]
	private UnityEvent OnHeldShared;

	// Token: 0x04002D69 RID: 11625
	[FormerlySerializedAs("OnDocked")]
	[SerializeField]
	private UnityEvent OnDockedLocal;

	// Token: 0x04002D6A RID: 11626
	[FormerlySerializedAs("OnDockedLocal")]
	[SerializeField]
	private UnityEvent OnDockedShared;

	// Token: 0x04002D6B RID: 11627
	private bool wasHeldLocal;

	// Token: 0x04002D6C RID: 11628
	private bool wasHeldShared;

	// Token: 0x04002D6D RID: 11629
	[Tooltip("(Optional) name broadcast by PlayerGameEvents")]
	public string interactEventName;

	// Token: 0x04002D6E RID: 11630
	public const int kPositionStateCount = 8;

	// Token: 0x04002D6F RID: 11631
	[DevInspectorShow]
	public TransferrableObject.InterpolateState interpState;

	// Token: 0x04002D70 RID: 11632
	public bool startInterpolation;

	// Token: 0x04002D71 RID: 11633
	public Transform InitialDockObject;

	// Token: 0x04002D72 RID: 11634
	private AudioSource audioSrc;

	// Token: 0x04002D75 RID: 11637
	protected Transform _defaultAnchor;

	// Token: 0x04002D76 RID: 11638
	protected bool _isDefaultAnchorSet;

	// Token: 0x04002D77 RID: 11639
	private Matrix4x4? transferrableItemSlotTransformOverrideCachedMatrix;

	// Token: 0x04002D78 RID: 11640
	private bool transferrableItemSlotTransformOverrideApplicable;

	// Token: 0x02000546 RID: 1350
	public enum SyncOptions
	{
		// Token: 0x04002D7A RID: 11642
		None,
		// Token: 0x04002D7B RID: 11643
		Bool,
		// Token: 0x04002D7C RID: 11644
		Int
	}

	// Token: 0x02000547 RID: 1351
	public enum ItemStates
	{
		// Token: 0x04002D7E RID: 11646
		State0 = 1,
		// Token: 0x04002D7F RID: 11647
		State1,
		// Token: 0x04002D80 RID: 11648
		State2 = 4,
		// Token: 0x04002D81 RID: 11649
		State3 = 8,
		// Token: 0x04002D82 RID: 11650
		State4 = 16,
		// Token: 0x04002D83 RID: 11651
		State5 = 32,
		// Token: 0x04002D84 RID: 11652
		Part0Held = 64,
		// Token: 0x04002D85 RID: 11653
		Part1Held = 128
	}

	// Token: 0x02000548 RID: 1352
	public enum GrabType
	{
		// Token: 0x04002D87 RID: 11655
		Default,
		// Token: 0x04002D88 RID: 11656
		Free
	}

	// Token: 0x02000549 RID: 1353
	[Flags]
	public enum PositionState
	{
		// Token: 0x04002D8A RID: 11658
		OnLeftArm = 1,
		// Token: 0x04002D8B RID: 11659
		OnRightArm = 2,
		// Token: 0x04002D8C RID: 11660
		InLeftHand = 4,
		// Token: 0x04002D8D RID: 11661
		InRightHand = 8,
		// Token: 0x04002D8E RID: 11662
		OnChest = 16,
		// Token: 0x04002D8F RID: 11663
		OnLeftShoulder = 32,
		// Token: 0x04002D90 RID: 11664
		OnRightShoulder = 64,
		// Token: 0x04002D91 RID: 11665
		Dropped = 128,
		// Token: 0x04002D92 RID: 11666
		None = 0
	}

	// Token: 0x0200054A RID: 1354
	public enum InterpolateState
	{
		// Token: 0x04002D94 RID: 11668
		None,
		// Token: 0x04002D95 RID: 11669
		Interpolating
	}
}
