using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020000FE RID: 254
[RequireComponent(typeof(GameEntity))]
public abstract class SIGadget : MonoBehaviour, IGameEntityComponent, IPrefabRequirements, IGameActivatable, IGameStateProvider
{
	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060005EC RID: 1516 RVA: 0x00022193 File Offset: 0x00020393
	// (set) Token: 0x060005ED RID: 1517 RVA: 0x0002219B File Offset: 0x0002039B
	public SITechTreePageId PageId
	{
		get
		{
			return this.pageId;
		}
		set
		{
			this.pageId = value;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x060005EE RID: 1518 RVA: 0x000221A4 File Offset: 0x000203A4
	public IEnumerable<GameEntity> RequiredPrefabs
	{
		get
		{
			return this.additionalRequiredPrefabs;
		}
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x000221AC File Offset: 0x000203AC
	protected virtual void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x000221E6 File Offset: 0x000203E6
	protected virtual void OnUpdateAuthority(float dt)
	{
		this.SleepAfterDelay();
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x000221E6 File Offset: 0x000203E6
	protected virtual void OnUpdateRemote(float dt)
	{
		this.SleepAfterDelay();
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x000221EE File Offset: 0x000203EE
	protected virtual bool IsEquippedLocal()
	{
		return this.gameEntity.IsHeldByLocalPlayer() || this.gameEntity.IsSnappedByLocalPlayer();
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x0002220C File Offset: 0x0002040C
	protected Vector2 GetJoystickInput()
	{
		if (!this.ShouldProcessInput())
		{
			return default(Vector2);
		}
		return ControllerInputPoller.Primary2DAxis((this.gameEntity.heldByHandIndex == 0 || this.gameEntity.snappedJoint == SnapJointType.HandL) ? XRNode.LeftHand : XRNode.RightHand);
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00022254 File Offset: 0x00020454
	protected bool ShouldProcessInput()
	{
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			return true;
		}
		GamePlayer gamePlayer;
		if (this.gameEntity.IsSnappedByLocalPlayer() && GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out gamePlayer))
		{
			SnapJointType snappedJoint = this.gameEntity.snappedJoint;
			GameEntity gameEntity;
			if (snappedJoint != SnapJointType.HandL)
			{
				if (snappedJoint != SnapJointType.HandR)
				{
					gameEntity = null;
				}
				else
				{
					gameEntity = gamePlayer.GetGrabbedGameEntity(1);
				}
			}
			else
			{
				gameEntity = gamePlayer.GetGrabbedGameEntity(0);
			}
			GameEntity gameEntity2 = gameEntity;
			return !gameEntity2 || gameEntity2.GetComponent<IGameActivatable>() == null;
		}
		return false;
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x000222D4 File Offset: 0x000204D4
	public void SleepAfterDelay()
	{
		if (this.isSleeping || !this.shouldSleep)
		{
			return;
		}
		if (Time.time < this.timeReleased + this.sleepTime)
		{
			return;
		}
		base.GetComponent<Rigidbody>().isKinematic = true;
		this.isSleeping = true;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x0002230F File Offset: 0x0002050F
	public virtual SIUpgradeSet FilterUpgradeNodes(SIUpgradeSet upgrades)
	{
		return upgrades;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00022314 File Offset: 0x00020514
	public virtual void RefreshUpgradeVisuals(SIUpgradeSet withUpgrades)
	{
		foreach (SIGadget.UpgradeVisual upgradeVisual in this.UpgradeBasedVisuals)
		{
			upgradeVisual.Update(withUpgrades);
		}
		Action<SIUpgradeSet> onPostRefreshVisuals = this.OnPostRefreshVisuals;
		if (onPostRefreshVisuals == null)
		{
			return;
		}
		onPostRefreshVisuals(withUpgrades);
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00022358 File Offset: 0x00020558
	protected virtual void OnEnable()
	{
		if (!this.didApplyId)
		{
			GameObject gameObject = base.gameObject;
			gameObject.name = gameObject.name + "[" + SIGadget.uniqueId.ToString() + "]";
			this.didApplyId = true;
			SIGadget.uniqueId++;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Combine(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.timeReleased = Time.time;
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00022454 File Offset: 0x00020654
	protected virtual void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Remove(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Remove(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.LeaveAllExclusionZones();
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00022504 File Offset: 0x00020704
	public void GrabInitialization()
	{
		this.isSleeping = false;
		this.shouldSleep = false;
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		SuperInfectionManager component = this.gameEntity.manager.GetComponent<SuperInfectionManager>();
		if (((component != null) ? component.zoneSuperInfection : null) == null)
		{
			return;
		}
		bool isMine = SIPlayer.LocalPlayer.activePlayerGadgets.Contains(this.gameEntity.GetNetId());
		SIProgression.Instance.UpdateHeldGadgetsTelemetry(this.PageId, isMine, 1);
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00022580 File Offset: 0x00020780
	public void ReleaseInitialization()
	{
		this.shouldSleep = true;
		this.isSleeping = false;
		this.timeReleased = Time.time;
		if (!this.gameEntity.WasLastHeldByLocalPlayer())
		{
			return;
		}
		SuperInfectionManager component = this.gameEntity.manager.GetComponent<SuperInfectionManager>();
		if (((component != null) ? component.zoneSuperInfection : null) == null)
		{
			return;
		}
		bool isMine = SIPlayer.LocalPlayer.activePlayerGadgets.Contains(this.gameEntity.GetNetId());
		SIProgression.Instance.UpdateHeldGadgetsTelemetry(this.PageId, isMine, -1);
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x00022608 File Offset: 0x00020808
	public bool FindAttachedHand(out bool isLeft)
	{
		isLeft = false;
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.AttachedPlayerActorNr, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindSlotIndex(this.gameEntity.id);
		isLeft = (num == 0 || num == 2);
		return isLeft || num == 1 || num == 3;
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x0002265C File Offset: 0x0002085C
	public VRRig GetAttachedPlayerRig()
	{
		int attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		GamePlayer gamePlayer;
		if (attachedPlayerActorNr < 1 || !GamePlayer.TryGetGamePlayer(attachedPlayerActorNr, out gamePlayer))
		{
			return null;
		}
		return gamePlayer.rig;
	}

	// Token: 0x060005FF RID: 1535 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnEntityInit()
	{
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnEntityDestroy()
	{
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0002268C File Offset: 0x0002088C
	public virtual void OnEntityStateChange(long prevState, long newState)
	{
		foreach (IGameStateReceiver gameStateReceiver in this._gameStateReceivers)
		{
			gameStateReceiver.GameStateReceiverOnStateChanged(prevState, newState);
		}
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x06000603 RID: 1539 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ProcessAuthorityToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x000226E0 File Offset: 0x000208E0
	public void SendClientToAuthorityRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00022738 File Offset: 0x00020938
	public void SendClientToAuthorityRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x00022794 File Offset: 0x00020994
	public void SendAuthorityToClientRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.AuthorityToClientRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x000227EC File Offset: 0x000209EC
	public void SendAuthorityToClientRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.AuthorityToClientRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x00022848 File Offset: 0x00020A48
	public void SendClientToClientRPC(int rpcID)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.CallEntityRPC, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID
			});
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x000228A0 File Offset: 0x00020AA0
	public void SendClientToClientRPC(int rpcID, object[] data)
	{
		SuperInfectionManager simanagerForZone = SuperInfectionManager.GetSIManagerForZone(this.gameEntity.manager.zone);
		if (simanagerForZone != null)
		{
			simanagerForZone.CallRPC(SuperInfectionManager.ClientToClientRPC.CallEntityRPCData, new object[]
			{
				this.gameEntity.GetNetId(),
				rpcID,
				data
			});
		}
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x000228F9 File Offset: 0x00020AF9
	public void ApplyExclusionZone(SIExclusionZone exclusionZone)
	{
		if (!this.appliedExclusionZones.Contains(exclusionZone))
		{
			bool activeExclusionFlags = this._activeExclusionFlags != (SIExclusionType)0;
			this.appliedExclusionZones.Add(exclusionZone);
			this._activeExclusionFlags |= exclusionZone.exclusionType;
			if (!activeExclusionFlags)
			{
				this.HandleBlockedActionChanged(true);
			}
		}
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00022937 File Offset: 0x00020B37
	public void LeaveExclusionZone(SIExclusionZone exclusionZone)
	{
		if (this.appliedExclusionZones.Contains(exclusionZone))
		{
			this.appliedExclusionZones.Remove(exclusionZone);
			this.RecalcExclusionFlags();
			if (this._activeExclusionFlags == (SIExclusionType)0)
			{
				this.HandleBlockedActionChanged(false);
			}
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0002296C File Offset: 0x00020B6C
	private void LeaveAllExclusionZones()
	{
		foreach (SIExclusionZone siexclusionZone in this.appliedExclusionZones)
		{
			if (siexclusionZone != null)
			{
				siexclusionZone.ClearGadget(this);
			}
		}
		this.appliedExclusionZones.Clear();
		this._activeExclusionFlags = (SIExclusionType)0;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x000229DC File Offset: 0x00020BDC
	private void RecalcExclusionFlags()
	{
		SIExclusionType siexclusionType = (SIExclusionType)0;
		for (int i = 0; i < this.appliedExclusionZones.Count; i++)
		{
			siexclusionType |= this.appliedExclusionZones[i].exclusionType;
		}
		this._activeExclusionFlags = siexclusionType;
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00022A1C File Offset: 0x00020C1C
	protected bool IsBlocked()
	{
		return (this._activeExclusionFlags & SIExclusionType.AffectsOthers) > (SIExclusionType)0;
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x00022A29 File Offset: 0x00020C29
	protected bool IsBlocked(SIExclusionType flag)
	{
		return (this._activeExclusionFlags & flag) > (SIExclusionType)0;
	}

	// Token: 0x06000611 RID: 1553 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void HandleBlockedActionChanged(bool isBlocked)
	{
	}

	// Token: 0x06000612 RID: 1554 RVA: 0x00022A36 File Offset: 0x00020C36
	void IGameStateProvider.GameStateReceiverRegister(IGameStateReceiver receiver)
	{
		this._gameStateReceivers.Add(receiver);
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00022A44 File Offset: 0x00020C44
	void IGameStateProvider.GameStateReceiverUnregister(IGameStateReceiver receiver)
	{
		this._gameStateReceivers.Remove(receiver);
	}

	// Token: 0x04000773 RID: 1907
	public GameEntity gameEntity;

	// Token: 0x04000774 RID: 1908
	[Tooltip("Add additional required prefabs here.  These will be automatically added to the GameEntityManager factory.")]
	public GameEntity[] additionalRequiredPrefabs;

	// Token: 0x04000775 RID: 1909
	public float sleepTime = 10f;

	// Token: 0x04000776 RID: 1910
	private bool shouldSleep = true;

	// Token: 0x04000777 RID: 1911
	private bool isSleeping;

	// Token: 0x04000778 RID: 1912
	private float timeReleased;

	// Token: 0x04000779 RID: 1913
	protected bool activatedLocally;

	// Token: 0x0400077A RID: 1914
	[SerializeField]
	private SITechTreePageId pageId;

	// Token: 0x0400077B RID: 1915
	public Action<SIUpgradeSet> OnPostRefreshVisuals;

	// Token: 0x0400077C RID: 1916
	private static int uniqueId = 101;

	// Token: 0x0400077D RID: 1917
	private bool didApplyId;

	// Token: 0x0400077E RID: 1918
	[SerializeField]
	private SIGadget.UpgradeVisual[] UpgradeBasedVisuals;

	// Token: 0x0400077F RID: 1919
	private readonly List<SIExclusionZone> appliedExclusionZones = new List<SIExclusionZone>();

	// Token: 0x04000780 RID: 1920
	private SIExclusionType _activeExclusionFlags;

	// Token: 0x04000781 RID: 1921
	private List<IGameStateReceiver> _gameStateReceivers = new List<IGameStateReceiver>();

	// Token: 0x020000FF RID: 255
	[Serializable]
	private struct UpgradeVisual
	{
		// Token: 0x06000616 RID: 1558 RVA: 0x00022A8C File Offset: 0x00020C8C
		public void Update(SIUpgradeSet withUpgrades)
		{
			bool flag = true;
			if (this.appearRequirements.Length != 0)
			{
				flag = false;
				foreach (SIUpgradeType upgrade in this.appearRequirements)
				{
					if (withUpgrades.Contains(upgrade))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				foreach (SIUpgradeType upgrade2 in this.disappearRequirements)
				{
					if (withUpgrades.Contains(upgrade2))
					{
						flag = false;
						break;
					}
				}
			}
			GameObject[] array2 = this.objects;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].SetActive(flag);
			}
		}

		// Token: 0x04000782 RID: 1922
		public GameObject[] objects;

		// Token: 0x04000783 RID: 1923
		[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
		public SIUpgradeType[] appearRequirements;

		// Token: 0x04000784 RID: 1924
		[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
		public SIUpgradeType[] disappearRequirements;
	}
}
