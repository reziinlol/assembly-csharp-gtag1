using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020009E1 RID: 2529
public class TakeMyHand_HandLink : HoldableObject, IGorillaSliceableSimple
{
	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x060040B1 RID: 16561 RVA: 0x00159CE7 File Offset: 0x00157EE7
	// (set) Token: 0x060040B2 RID: 16562 RVA: 0x00159CEF File Offset: 0x00157EEF
	public bool IsTentacleGrab { get; private set; }

	// Token: 0x170005FD RID: 1533
	// (get) Token: 0x060040B3 RID: 16563 RVA: 0x00159CF8 File Offset: 0x00157EF8
	// (set) Token: 0x060040B4 RID: 16564 RVA: 0x00159D00 File Offset: 0x00157F00
	public bool IsLocal { get; private set; }

	// Token: 0x060040B5 RID: 16565 RVA: 0x00159D0C File Offset: 0x00157F0C
	private void Start()
	{
		this.myOtherHandLink = (this.isLeftHand ? this.myRig.rightHandLink : this.myRig.leftHandLink);
		if (this.myRig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			this.IsLocal = true;
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	// Token: 0x060040B6 RID: 16566 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060040B7 RID: 16567 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060040B8 RID: 16568 RVA: 0x00159D7C File Offset: 0x00157F7C
	public void SliceUpdate()
	{
		this.interactionPoint.enabled = (this.isReadyForGrabbing && (this.myRig.transform.position - VRRig.LocalRig.transform.position).sqrMagnitude < 9f);
	}

	// Token: 0x060040B9 RID: 16569 RVA: 0x00159DD4 File Offset: 0x00157FD4
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.CanBeGrabbed())
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			(this.isLeftHand ? this.myRig.leftHolds : this.myRig.rightHolds).OnGrab(pointGrabbed, grabbingHand);
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink = (grabbingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
		if (takeMyHand_HandLink.isReadyForGrabbing && Time.time - takeMyHand_HandLink.gripPressedAtTimestamp < 0.1f)
		{
			takeMyHand_HandLink.LocalCreateLink(this);
		}
	}

	// Token: 0x060040BA RID: 16570 RVA: 0x00159E80 File Offset: 0x00158080
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.myRig.isOfflineVRRig)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = (releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			bool flag = false;
			HandLinkAuthorityStatus handLinkAuthorityStatus = GTPlayer.Instance.TakeMyHand_GetSelfHandLinkAuthority();
			int num;
			HandLinkAuthorityStatus chainAuthority = takeMyHand_HandLink.GetChainAuthority(out num);
			if (handLinkAuthorityStatus.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < handLinkAuthorityStatus.type)
			{
				flag = true;
			}
			else if (takeMyHand_HandLink.myOtherHandLink.grabbedLink != null)
			{
				int num2;
				HandLinkAuthorityStatus chainAuthority2 = takeMyHand_HandLink.myOtherHandLink.GetChainAuthority(out num2);
				if (chainAuthority2.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < chainAuthority2.type)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(takeMyHand_HandLink.isLeftHand).GetAverageVelocity(true, 0.15f, false);
				this.myRig.netView.SendRPC("DroppedByPlayer", this.myRig.OwningNetPlayer, new object[]
				{
					averageVelocity
				});
				this.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
			}
			takeMyHand_HandLink.BreakLink();
		}
		return true;
	}

	// Token: 0x060040BB RID: 16571 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x060040BC RID: 16572 RVA: 0x00159FA7 File Offset: 0x001581A7
	public override void DropItemCleanup()
	{
		if (this.grabbedLink != null)
		{
			this.grabbedLink.BreakLink();
		}
	}

	// Token: 0x060040BD RID: 16573 RVA: 0x00159FC2 File Offset: 0x001581C2
	public bool CanBeGrabbed()
	{
		return (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.WantsHoldingHandsDisabled()) && Time.time >= this.rejectGrabsUntilTimestamp && this.isReadyForGrabbing && this.grabbedPlayer == null;
	}

	// Token: 0x060040BE RID: 16574 RVA: 0x00159FFD File Offset: 0x001581FD
	public bool IsLinkActive()
	{
		return this.grabbedLink != null;
	}

	// Token: 0x060040BF RID: 16575 RVA: 0x0015A00C File Offset: 0x0015820C
	public bool TentacleTryCreateLink(TakeMyHand_HandLink remoteLink)
	{
		if (!this.myRig.isLocal || this.grabbedPlayer != null)
		{
			return false;
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
		{
			return false;
		}
		if (Time.time < this.rejectGrabsUntilTimestamp)
		{
			return false;
		}
		if (!remoteLink.CanBeGrabbed())
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return false;
		}
		this.IsTentacleGrab = true;
		this.grabbedLink = remoteLink;
		this.grabbedLink.TentacleOffset = Vector3.zero;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged != null)
		{
			onHandLinkChanged();
		}
		return true;
	}

	// Token: 0x170005FE RID: 1534
	// (get) Token: 0x060040C0 RID: 16576 RVA: 0x0015A0F0 File Offset: 0x001582F0
	// (set) Token: 0x060040C1 RID: 16577 RVA: 0x0015A0F8 File Offset: 0x001582F8
	public Vector3 TentacleOffset { get; set; }

	// Token: 0x170005FF RID: 1535
	// (get) Token: 0x060040C2 RID: 16578 RVA: 0x0015A101 File Offset: 0x00158301
	public Vector3 LinkPosition
	{
		get
		{
			return base.transform.position + this.TentacleOffset;
		}
	}

	// Token: 0x060040C3 RID: 16579 RVA: 0x0015A11C File Offset: 0x0015831C
	private void LocalCreateLink(TakeMyHand_HandLink remoteLink)
	{
		if (this.grabbedPlayer != null || !this.myRig.isLocal)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(remoteLink, this.isLeftHand);
		this.grabbedLink = remoteLink;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		this.TentacleOffset = Vector3.zero;
		if (remoteLink.IsTentacleGrab)
		{
			remoteLink.TentacleOffset = base.transform.position - remoteLink.transform.position;
		}
		else
		{
			remoteLink.TentacleOffset = Vector3.zero;
		}
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
		(this.isLeftHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer).GTPlayOneShot(this.audioOnGrab, 1f);
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	// Token: 0x060040C4 RID: 16580 RVA: 0x0015A253 File Offset: 0x00158453
	public void BreakLinkTo(TakeMyHand_HandLink targetLink)
	{
		if (this.grabbedLink == targetLink)
		{
			this.BreakLink();
		}
	}

	// Token: 0x060040C5 RID: 16581 RVA: 0x0015A26C File Offset: 0x0015846C
	public void BreakLink()
	{
		if (this.grabbedPlayer == null || this.grabbedLink == null)
		{
			return;
		}
		Vector3 velocity = this.myRig.LatestVelocity();
		GTPlayer.Instance.SetVelocity(velocity);
		this.IsTentacleGrab = false;
		this.TentacleOffset = Vector3.zero;
		this.grabbedLink = null;
		this.grabbedPlayer = null;
		this.grabbedHandIsLeft = false;
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isLeftHand);
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	// Token: 0x060040C6 RID: 16582 RVA: 0x0015A2F0 File Offset: 0x001584F0
	public static bool IsHandInChainWithOtherPlayer(TakeMyHand_HandLink startingLink, int targetPlayer)
	{
		TakeMyHand_HandLink takeMyHand_HandLink = startingLink;
		int num = 0;
		int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		while (takeMyHand_HandLink != null && num < roomPlayerCount)
		{
			if (takeMyHand_HandLink.myRig == null || takeMyHand_HandLink.myRig.creator == null)
			{
				return false;
			}
			if (takeMyHand_HandLink.myRig.creator.ActorNumber == targetPlayer)
			{
				return true;
			}
			TakeMyHand_HandLink takeMyHand_HandLink2 = null;
			RigContainer rigContainer;
			if (takeMyHand_HandLink.grabbedLink != null && takeMyHand_HandLink.grabbedLink.myOtherHandLink != null)
			{
				takeMyHand_HandLink2 = takeMyHand_HandLink.grabbedLink.myOtherHandLink;
			}
			else if (takeMyHand_HandLink.grabbedPlayer != null && VRRigCache.Instance.TryGetVrrig(takeMyHand_HandLink.grabbedPlayer, out rigContainer))
			{
				TakeMyHand_HandLink takeMyHand_HandLink3 = takeMyHand_HandLink.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
				if (takeMyHand_HandLink3 != null && takeMyHand_HandLink3.myOtherHandLink != null)
				{
					takeMyHand_HandLink2 = takeMyHand_HandLink3.myOtherHandLink;
				}
			}
			takeMyHand_HandLink = takeMyHand_HandLink2;
			num++;
		}
		return false;
	}

	// Token: 0x060040C7 RID: 16583 RVA: 0x0015A3EC File Offset: 0x001585EC
	public void LocalUpdate(bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool isReadyForGrabbing)
	{
		if (isGripPressed && !this.wasGripPressed)
		{
			this.gripPressedAtTimestamp = Time.time;
		}
		this.wasGripPressed = isGripPressed;
		this.isReadyForGrabbing = (isReadyForGrabbing && Time.time >= this.rejectGrabsUntilTimestamp);
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		if (this.grabbedLink != null)
		{
			if (!this.grabbedLink.isReadyForGrabbing && this.grabbedLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				this.BreakLink();
				return;
			}
			if ((!this.IsTentacleGrab && !isGripPressed) || !this.grabbedLink.myRig.gameObject.activeSelf)
			{
				this.BreakLink();
				return;
			}
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(this.grabbedPlayer))
			{
				this.BreakLink();
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.grabbedLink.myRig);
			GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
			if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
			{
				this.BreakLink();
				return;
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
			{
				this.BreakLink();
				return;
			}
		}
	}

	// Token: 0x060040C8 RID: 16584 RVA: 0x0015A533 File Offset: 0x00158733
	public void RejectGrabsFor(float duration)
	{
		this.rejectGrabsUntilTimestamp = Mathf.Max(this.rejectGrabsUntilTimestamp, Time.time + duration);
	}

	// Token: 0x060040C9 RID: 16585 RVA: 0x0015A54D File Offset: 0x0015874D
	public void Write(out bool isGroundedHand, out bool isGroundedButt, out int grabbedPlayerActorNumber, out bool grabbedHandIsLeft)
	{
		isGroundedHand = this.isGroundedHand;
		isGroundedButt = this.isGroundedButt;
		if (this.grabbedPlayer != null)
		{
			grabbedPlayerActorNumber = this.grabbedPlayer.ActorNumber;
			grabbedHandIsLeft = this.grabbedHandIsLeft;
			return;
		}
		grabbedPlayerActorNumber = 0;
		grabbedHandIsLeft = false;
	}

	// Token: 0x060040CA RID: 16586 RVA: 0x0015A588 File Offset: 0x00158788
	public void Read(Vector3 remoteHandLocalPos, Quaternion remoteBodyWorldRot, Vector3 remoteBodyWorldPos, bool isGroundedHand, bool isGroundedButt, bool isReadyForGrabbing, bool isTentacleGrab, int grabbedPlayerActorNumber, bool grabbedHandIsLeft)
	{
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		this.isReadyForGrabbing = isReadyForGrabbing;
		if (grabbedPlayerActorNumber == 0)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				(this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
			}
			bool flag = this.grabbedPlayer != null;
			this.grabbedPlayer = null;
			this.grabbedLink = null;
			if (flag)
			{
				Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
				if (onHandLinkChanged != null)
				{
					onHandLinkChanged();
				}
			}
		}
		else if (this.lastReadGrabbedPlayerActorNumber == grabbedPlayerActorNumber)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsValid && this.grabbedPlayer.ActorNumber == grabbedPlayerActorNumber && this.grabbedPlayer.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 7f))
			{
				if (this.grabbedHandIsLeft)
				{
					VRRig.LocalRig.leftHandLink.BreakLink();
				}
				else
				{
					VRRig.LocalRig.rightHandLink.BreakLink();
				}
			}
		}
		else
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this);
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(grabbedPlayerActorNumber);
			if (player != null)
			{
				bool flag2 = true;
				if (player.IsLocal && !isTentacleGrab && !(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).IsTentacleGrab)
				{
					flag2 = this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 0.25f);
				}
				if (!flag2)
				{
					(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).RejectGrabsFor(0.5f);
					bool flag3 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag3)
					{
						Action onHandLinkChanged2 = TakeMyHand_HandLink.OnHandLinkChanged;
						if (onHandLinkChanged2 != null)
						{
							onHandLinkChanged2();
						}
					}
				}
				else if (player == this.myRig.OwningNetPlayer)
				{
					bool flag4 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag4)
					{
						Action onHandLinkChanged3 = TakeMyHand_HandLink.OnHandLinkChanged;
						if (onHandLinkChanged3 != null)
						{
							onHandLinkChanged3();
						}
					}
				}
				else
				{
					this.grabbedPlayer = player;
					this.grabbedHandIsLeft = grabbedHandIsLeft;
					this.IsTentacleGrab = isTentacleGrab;
					this.CheckFormLinkWithRemoteGrab();
					Action onHandLinkChanged4 = TakeMyHand_HandLink.OnHandLinkChanged;
					if (onHandLinkChanged4 != null)
					{
						onHandLinkChanged4();
					}
				}
			}
			else
			{
				bool flag5 = this.grabbedPlayer != null;
				this.grabbedPlayer = null;
				this.grabbedLink = null;
				if (flag5)
				{
					Action onHandLinkChanged5 = TakeMyHand_HandLink.OnHandLinkChanged;
					if (onHandLinkChanged5 != null)
					{
						onHandLinkChanged5();
					}
				}
			}
		}
		this.lastReadGrabbedPlayerActorNumber = grabbedPlayerActorNumber;
	}

	// Token: 0x060040CB RID: 16587 RVA: 0x0015A828 File Offset: 0x00158A28
	private bool IsLocalGrabInRange(bool grabbedLeftHand, Vector3 handLocalPos, Quaternion bodyWorldRot, Vector3 bodyWorldPos, float tolerance)
	{
		return ((grabbedLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).transform.position - (bodyWorldPos + bodyWorldRot * handLocalPos)).IsShorterThan(tolerance);
	}

	// Token: 0x060040CC RID: 16588 RVA: 0x0015A868 File Offset: 0x00158A68
	private void CheckFormLinkWithRemoteGrab()
	{
		if (this.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.grabbedPlayer, out rigContainer))
			{
				TakeMyHand_HandLink takeMyHand_HandLink = this.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
				if (takeMyHand_HandLink.grabbedPlayer == this.myRig.creator)
				{
					this.grabbedLink = takeMyHand_HandLink;
					this.grabbedLink.grabbedLink = this;
				}
			}
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink2 = this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
		if (takeMyHand_HandLink2.isReadyForGrabbing)
		{
			takeMyHand_HandLink2.LocalCreateLink(this);
			return;
		}
		takeMyHand_HandLink2.RejectGrabsFor(0.5f);
	}

	// Token: 0x060040CD RID: 16589 RVA: 0x0015A920 File Offset: 0x00158B20
	public HandLinkAuthorityStatus GetChainAuthority(out int stepsToAuth)
	{
		TakeMyHand_HandLink takeMyHand_HandLink = this.grabbedLink;
		int num = 1;
		HandLinkAuthorityStatus handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, -1f, -1);
		stepsToAuth = -1;
		while (takeMyHand_HandLink != null && num < 10 && !takeMyHand_HandLink.IsLocal)
		{
			if (takeMyHand_HandLink.isGroundedHand)
			{
				stepsToAuth = num;
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded, -1f, -1);
			}
			if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ResidualHandGrounded && (double)(takeMyHand_HandLink.myRig.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, takeMyHand_HandLink.myRig.LastHandTouchedGroundAtNetworkTime, takeMyHand_HandLink.myRig.OwningNetPlayer.ActorNumber);
			}
			else if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ButtGrounded && takeMyHand_HandLink.isGroundedButt)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded, -1f, -1);
			}
			else if (handLinkAuthorityStatus.type == HandLinkAuthorityType.None)
			{
				HandLinkAuthorityStatus handLinkAuthorityStatus2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, takeMyHand_HandLink.myRig.LastTouchedGroundAtNetworkTime, takeMyHand_HandLink.myRig.OwningNetPlayer.ActorNumber);
				if (handLinkAuthorityStatus2 > handLinkAuthorityStatus)
				{
					stepsToAuth = num;
					handLinkAuthorityStatus = handLinkAuthorityStatus2;
				}
			}
			num++;
			takeMyHand_HandLink = takeMyHand_HandLink.myOtherHandLink.grabbedLink;
		}
		return handLinkAuthorityStatus;
	}

	// Token: 0x060040CE RID: 16590 RVA: 0x0015AA38 File Offset: 0x00158C38
	public void VisuallySnapHandsTogether()
	{
		if (this.grabbedLink == null)
		{
			return;
		}
		if (this.IsTentacleGrab || this.grabbedLink.IsTentacleGrab)
		{
			return;
		}
		if (this.grabbedLink.snapPositionCalculatedAtFrame == Time.frameCount)
		{
			this.snapPositionCalculatedAtFrame = Time.frameCount;
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = this.grabbedLink.transform.position;
		Vector3 a = (position + position2) / 2f;
		Vector3 b = (this.isLeftHand ? this.myRig.leftHand.rigTarget : this.myRig.rightHand.rigTarget).position - position;
		Vector3 b2 = (this.grabbedLink.isLeftHand ? this.grabbedLink.myRig.leftHand.rigTarget : this.grabbedLink.myRig.rightHand.rigTarget).position - position2;
		Vector3 targetWorldPos = a + b;
		Vector3 targetWorldPos2 = a + b2;
		this.myIK.OverrideTargetPos(this.isLeftHand, targetWorldPos);
		this.grabbedLink.myIK.OverrideTargetPos(this.grabbedLink.isLeftHand, targetWorldPos2);
	}

	// Token: 0x060040CF RID: 16591 RVA: 0x0015AB72 File Offset: 0x00158D72
	public void PlayVicariousTapHaptic()
	{
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnVicariousTap, this.hapticDurationOnVicariousTap);
	}

	// Token: 0x04005147 RID: 20807
	[FormerlySerializedAs("myPlayer")]
	[SerializeField]
	public VRRig myRig;

	// Token: 0x04005148 RID: 20808
	[FormerlySerializedAs("leftHand")]
	[SerializeField]
	private bool isLeftHand;

	// Token: 0x04005149 RID: 20809
	[SerializeField]
	public GorillaIK myIK;

	// Token: 0x0400514A RID: 20810
	private TakeMyHand_HandLink myOtherHandLink;

	// Token: 0x0400514B RID: 20811
	private bool isReadyForGrabbing;

	// Token: 0x0400514C RID: 20812
	public bool isGroundedHand;

	// Token: 0x0400514D RID: 20813
	public bool isGroundedButt;

	// Token: 0x0400514E RID: 20814
	private bool wasGripPressed;

	// Token: 0x0400514F RID: 20815
	private float gripPressedAtTimestamp;

	// Token: 0x04005150 RID: 20816
	private float rejectGrabsUntilTimestamp;

	// Token: 0x04005151 RID: 20817
	public TakeMyHand_HandLink grabbedLink;

	// Token: 0x04005152 RID: 20818
	public NetPlayer grabbedPlayer;

	// Token: 0x04005153 RID: 20819
	public bool grabbedHandIsLeft;

	// Token: 0x04005156 RID: 20822
	private const bool DEBUG_GRAB_ANYONE = false;

	// Token: 0x04005157 RID: 20823
	[SerializeField]
	private float hapticStrengthOnGrab;

	// Token: 0x04005158 RID: 20824
	[SerializeField]
	private float hapticDurationOnGrab;

	// Token: 0x04005159 RID: 20825
	[SerializeField]
	private float hapticStrengthOnVicariousTap;

	// Token: 0x0400515A RID: 20826
	[SerializeField]
	private float hapticDurationOnVicariousTap;

	// Token: 0x0400515B RID: 20827
	[SerializeField]
	private AudioClip audioOnGrab;

	// Token: 0x0400515C RID: 20828
	public InteractionPoint interactionPoint;

	// Token: 0x0400515D RID: 20829
	public static Action OnHandLinkChanged;

	// Token: 0x0400515F RID: 20831
	private int lastReadGrabbedPlayerActorNumber;

	// Token: 0x04005160 RID: 20832
	private int snapPositionCalculatedAtFrame = -1;
}
