using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007E4 RID: 2020
public class GRShuttle : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600337A RID: 13178 RVA: 0x0011B99C File Offset: 0x00119B9C
	public void Awake()
	{
		this.shuttleUI.Setup(null, null);
		if (this.entryCardScanner != null)
		{
			this.entryCardScanner.requireSpecificPlayer = true;
			this.entryCardScanner.restrictToPlayer = null;
		}
		if (this.departCardScanner != null)
		{
			this.departCardScanner.requireSpecificPlayer = true;
			this.departCardScanner.restrictToPlayer = null;
		}
		this.state = GRShuttleState.Docked;
	}

	// Token: 0x0600337B RID: 13179 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600337C RID: 13180 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600337D RID: 13181 RVA: 0x0011BA09 File Offset: 0x00119C09
	public void Init(int shuttleId)
	{
		this.shuttleId = shuttleId;
		this.StopMoveFx();
	}

	// Token: 0x0600337E RID: 13182 RVA: 0x0011BA18 File Offset: 0x00119C18
	public void SetBay(GRBay bay)
	{
		this.shuttleBay = bay;
	}

	// Token: 0x0600337F RID: 13183 RVA: 0x0011BA21 File Offset: 0x00119C21
	public void SetReactor(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003380 RID: 13184 RVA: 0x0011BA2A File Offset: 0x00119C2A
	public void SetLocation(GRShuttleGroupLoc location)
	{
		this.location = location;
		this.targetSection = this.ClampTargetSection(this.targetSection);
	}

	// Token: 0x06003381 RID: 13185 RVA: 0x0011BA45 File Offset: 0x00119C45
	public void Setup(GhostReactor reactor, GRShuttleGroupLoc location, int employeeIndex)
	{
		this.reactor = reactor;
		this.location = location;
		this.employeeIndex = employeeIndex;
		this.SetOwner(null);
		this.targetSection = this.ClampTargetSection(this.targetSection);
	}

	// Token: 0x06003382 RID: 13186 RVA: 0x0011BA78 File Offset: 0x00119C78
	public int GetTargetFloor()
	{
		if (this.specificDestinationShuttle != null)
		{
			return this.specificDestinationShuttle.specificFloor;
		}
		if (this.targetSection < 0 || this.targetSection >= GRShuttle.sectionFloors.Length)
		{
			return 0;
		}
		return GRShuttle.sectionFloors[this.targetSection];
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x0011BAC5 File Offset: 0x00119CC5
	public GRShuttleState GetState()
	{
		return this.state;
	}

	// Token: 0x06003384 RID: 13188 RVA: 0x0011BACD File Offset: 0x00119CCD
	public NetPlayer GetOwner()
	{
		return this.shuttleOwner;
	}

	// Token: 0x06003385 RID: 13189 RVA: 0x0011BAD8 File Offset: 0x00119CD8
	public void SetOwner(NetPlayer player)
	{
		this.shuttleOwner = player;
		this.shuttleUI.Setup(this.reactor, player);
		this.entryCardScanner.restrictToPlayer = player;
		this.departCardScanner.restrictToPlayer = player;
		if (this.shuttleBay != null)
		{
			this.shuttleBay.Refresh();
		}
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x0011BB2F File Offset: 0x00119D2F
	public void SliceUpdate()
	{
		this.UpdateState();
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x0011BB37 File Offset: 0x00119D37
	public void Refresh()
	{
		this.shuttleUI.RefreshUI();
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void JoinShuttleRoomLocalPlayer(GRShuttle sourceShuttle, GRShuttle destShuttle)
	{
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x0011BB44 File Offset: 0x00119D44
	public static void TeleportLocalPlayer(GRShuttle sourceShuttle, GRShuttle destShuttle)
	{
		sourceShuttle.friendCollider.RefreshPlayersWithinBounds();
		if (!sourceShuttle.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			return;
		}
		GTPlayer instance = GTPlayer.Instance;
		VRRig localRig = VRRig.LocalRig;
		float angle = destShuttle.transform.rotation.eulerAngles.y - sourceShuttle.transform.rotation.eulerAngles.y;
		Vector3 b = localRig.transform.position - instance.transform.position;
		Vector3 position = sourceShuttle.transform.InverseTransformPoint(instance.transform.position);
		position.x *= 0.8f;
		position.z *= 0.8f;
		Vector3 position2 = destShuttle.transform.TransformPoint(position);
		instance.TeleportTo(position2, instance.transform.rotation, false, false);
		instance.turnParent.transform.RotateAround(instance.headCollider.transform.position, sourceShuttle.transform.up, angle);
		localRig.transform.position = instance.transform.position + b;
		instance.InitializeValues();
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x0011BC80 File Offset: 0x00119E80
	public void SetState(GRShuttleState newState, bool force = false)
	{
		if (this.state == newState && !force)
		{
			return;
		}
		switch (this.state)
		{
		case GRShuttleState.Docked:
			if (this.shuttleBay != null)
			{
				this.shuttleBay.Refresh();
			}
			break;
		case GRShuttleState.PostMove:
			if (this.specificDestinationShuttle != null)
			{
				this.OpenDoorLocal();
			}
			else
			{
				this.CloseDoorLocal();
			}
			break;
		case GRShuttleState.PostArrive:
			this.OpenDoorLocal();
			break;
		}
		this.state = newState;
		this.stateStartTime = Time.timeAsDouble;
		switch (this.state)
		{
		case GRShuttleState.Docked:
			if (this.shuttleBay != null)
			{
				this.shuttleBay.Refresh();
			}
			this.StopMoveFx();
			break;
		case GRShuttleState.PreMove:
			this.CloseDoorLocal();
			this.takeOffSound.Play(null);
			if (this.specificDestinationShuttle != null)
			{
				GRPlayer grplayer = GRPlayer.Get(GRElevatorManager.LowestActorNumberInElevator(this.friendCollider, this.specificDestinationShuttle.friendCollider));
				this.shuttleOwner = grplayer.gamePlayer.rig.OwningNetPlayer;
			}
			GRShuttle.TryStartLocalPlayerShuttleMove(this.shuttleId, this.shuttleOwner);
			this.StartMoveFx();
			return;
		case GRShuttleState.Moving:
			this.moveSound.Play(null);
			return;
		case GRShuttleState.PostMove:
			break;
		case GRShuttleState.Arriving:
			this.CloseDoorLocal();
			this.moveSound.Play(null);
			return;
		case GRShuttleState.PostArrive:
			this.landSound.Play(null);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x0011BDF4 File Offset: 0x00119FF4
	private void UpdateState()
	{
		double timeAsDouble = Time.timeAsDouble;
		switch (this.state)
		{
		case GRShuttleState.PreMove:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Moving, false);
				return;
			}
			break;
		case GRShuttleState.Moving:
			if (timeAsDouble > this.stateStartTime + 5.0)
			{
				this.SetState(GRShuttleState.PostMove, false);
				return;
			}
			break;
		case GRShuttleState.PostMove:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Docked, false);
				return;
			}
			break;
		case GRShuttleState.Arriving:
			if (timeAsDouble > this.stateStartTime + 2.0)
			{
				this.SetState(GRShuttleState.PostArrive, false);
				return;
			}
			break;
		case GRShuttleState.PostArrive:
			if (timeAsDouble > this.stateStartTime + 1.0)
			{
				this.SetState(GRShuttleState.Docked, false);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x0011BEB6 File Offset: 0x0011A0B6
	public void RequestArrival()
	{
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleArrive, this.shuttleId);
	}

	// Token: 0x0600338D RID: 13197 RVA: 0x0011BED0 File Offset: 0x0011A0D0
	private void StartMoveFx()
	{
		if (this.windowFx != null)
		{
			this.windowFx.Play();
		}
		for (int i = 0; i < this.hideOnMove.Count; i++)
		{
			this.hideOnMove[i].SetActive(false);
		}
		for (int j = 0; j < this.showOnMove.Count; j++)
		{
			this.showOnMove[j].SetActive(true);
		}
	}

	// Token: 0x0600338E RID: 13198 RVA: 0x0011BF48 File Offset: 0x0011A148
	private void StopMoveFx()
	{
		if (this.windowFx != null)
		{
			this.windowFx.Stop();
		}
		for (int i = 0; i < this.hideOnMove.Count; i++)
		{
			this.hideOnMove[i].SetActive(true);
		}
		for (int j = 0; j < this.showOnMove.Count; j++)
		{
			this.showOnMove[j].SetActive(false);
		}
	}

	// Token: 0x0600338F RID: 13199 RVA: 0x0011BFC0 File Offset: 0x0011A1C0
	public bool IsPodUnlocked()
	{
		if (this.specificDestinationShuttle != null)
		{
			return true;
		}
		if (this.shuttleOwner == null)
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(this.shuttleOwner);
		return !(grplayer == null) && grplayer.IsDropPodUnlocked();
	}

	// Token: 0x06003390 RID: 13200 RVA: 0x0011C004 File Offset: 0x0011A204
	public int GetMaxDropFloor()
	{
		if (this.shuttleOwner == null)
		{
			return 0;
		}
		GRPlayer grplayer = GRPlayer.Get(this.shuttleOwner);
		if (grplayer == null)
		{
			return 0;
		}
		return grplayer.GetMaxDropFloor();
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x0011C038 File Offset: 0x0011A238
	public void OnShuttleMove()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleLaunch, this.shuttleId);
	}

	// Token: 0x06003392 RID: 13202 RVA: 0x0011C05C File Offset: 0x0011A25C
	public void OnShuttleMoveActorNr(int actorNr)
	{
		if (this.state != GRShuttleState.Docked || actorNr != this.shuttleOwner.ActorNumber || this.GetTargetFloor() > this.GetMaxDropFloor())
		{
			this.departCardScanner.onFailed.Invoke();
			return;
		}
		this.departCardScanner.onSucceeded.Invoke();
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleLaunch, this.shuttleId);
	}

	// Token: 0x06003393 RID: 13203 RVA: 0x0011C0C6 File Offset: 0x0011A2C6
	public void TargetLevelUp()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleTargetLevelUp, this.shuttleId);
	}

	// Token: 0x06003394 RID: 13204 RVA: 0x0011C0E9 File Offset: 0x0011A2E9
	public void TargetLevelDown()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleTargetLevelDown, this.shuttleId);
	}

	// Token: 0x06003395 RID: 13205 RVA: 0x0011C10C File Offset: 0x0011A30C
	private GRShuttle GetTargetShuttle()
	{
		if (this.specificDestinationShuttle != null)
		{
			return this.specificDestinationShuttle;
		}
		if (this.shuttleOwner == null)
		{
			return null;
		}
		GRShuttle drillShuttleForPlayer = GRElevatorManager._instance.GetDrillShuttleForPlayer(this.shuttleOwner.ActorNumber);
		GRShuttle stagingShuttleForPlayer = GRElevatorManager._instance.GetStagingShuttleForPlayer(this.shuttleOwner.ActorNumber);
		if (this.location != GRShuttleGroupLoc.Drill)
		{
			return drillShuttleForPlayer;
		}
		return stagingShuttleForPlayer;
	}

	// Token: 0x06003396 RID: 13206 RVA: 0x0011C170 File Offset: 0x0011A370
	public bool IsPlayerOwner(GRPlayer player)
	{
		return GRPlayer.Get(this.GetOwner()) == player;
	}

	// Token: 0x06003397 RID: 13207 RVA: 0x0011C184 File Offset: 0x0011A384
	public bool IsShuttleInteractableByPlayer(GRPlayer player, bool ignoreOwnership)
	{
		if (!ignoreOwnership && !this.IsPlayerOwner(player) && this.specificDestinationShuttle == null)
		{
			return false;
		}
		if (this.entryCardScanner == null)
		{
			return true;
		}
		if (this.departCardScanner == null)
		{
			return true;
		}
		bool flag = GameEntityManager.IsPlayerHandNearPosition(player.gamePlayer, this.entryCardScanner.transform.position, false, true, 16f);
		bool flag2 = GameEntityManager.IsPlayerHandNearPosition(player.gamePlayer, this.departCardScanner.transform.position, false, true, 16f);
		return flag || flag2;
	}

	// Token: 0x06003398 RID: 13208 RVA: 0x0011C214 File Offset: 0x0011A414
	public bool IsPlayerOwner(NetPlayer player)
	{
		return this.GetOwner() == player;
	}

	// Token: 0x06003399 RID: 13209 RVA: 0x0011C220 File Offset: 0x0011A420
	public void ToggleDoor()
	{
		if (this.state != GRShuttleState.Docked)
		{
			return;
		}
		if (this.entryDoor.doorState == GRDoor.DoorState.Closed)
		{
			this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleOpen, this.shuttleId);
			return;
		}
		if (this.entryDoor.doorState == GRDoor.DoorState.Open)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble > this.lastCloseTime + 5.0)
			{
				this.lastCloseTime = timeAsDouble;
				this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleClose, this.shuttleId);
			}
		}
	}

	// Token: 0x0600339A RID: 13210 RVA: 0x0011C2A4 File Offset: 0x0011A4A4
	public void ToggleDoorActorNr(int actorNr)
	{
		if (this.state == GRShuttleState.Docked && this.GetOwner() != null && this.GetOwner().ActorNumber == actorNr && GRPlayer.Get(this.shuttleOwner).IsDropPodUnlocked())
		{
			IDCardScanner idcardScanner = this.entryCardScanner;
			if (idcardScanner != null)
			{
				idcardScanner.onSucceeded.Invoke();
			}
			this.ToggleDoor();
			return;
		}
		IDCardScanner idcardScanner2 = this.entryCardScanner;
		if (idcardScanner2 == null)
		{
			return;
		}
		idcardScanner2.onFailed.Invoke();
	}

	// Token: 0x0600339B RID: 13211 RVA: 0x0011C314 File Offset: 0x0011A514
	public void EmergencyOpenDoor()
	{
		if (this.state == GRShuttleState.Docked)
		{
			if (PhotonNetwork.InRoom)
			{
				this.reactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.ShuttleOpen, this.shuttleId);
				return;
			}
			this.OpenDoorLocal();
		}
	}

	// Token: 0x0600339C RID: 13212 RVA: 0x0011C344 File Offset: 0x0011A544
	public void OnOpenDoor()
	{
		if (this.entryDoor.doorState == GRDoor.DoorState.Closed && this.entryCardScanner != null)
		{
			this.entryCardScanner.onSucceeded.Invoke();
		}
		this.OpenDoorLocal();
	}

	// Token: 0x0600339D RID: 13213 RVA: 0x0011C377 File Offset: 0x0011A577
	public void OpenDoorLocal()
	{
		if (this.entryDoor != null && this.entryDoor.doorState == GRDoor.DoorState.Closed)
		{
			this.entryDoor.SetDoorState(GRDoor.DoorState.Open);
		}
		if (this.shuttleBay != null)
		{
			this.shuttleBay.SetOpen(true);
		}
	}

	// Token: 0x0600339E RID: 13214 RVA: 0x0011C3B4 File Offset: 0x0011A5B4
	public void CloseDoorLocal()
	{
		if (this.entryDoor != null && this.entryDoor.doorState == GRDoor.DoorState.Open)
		{
			this.entryDoor.SetDoorState(GRDoor.DoorState.Closed);
		}
	}

	// Token: 0x0600339F RID: 13215 RVA: 0x0011C3D8 File Offset: 0x0011A5D8
	public void OnCloseDoor()
	{
		if (this.entryDoor.doorState == GRDoor.DoorState.Open && this.entryCardScanner != null)
		{
			this.entryCardScanner.onSucceeded.Invoke();
		}
		this.CloseDoorLocal();
	}

	// Token: 0x060033A0 RID: 13216 RVA: 0x0011C40C File Offset: 0x0011A60C
	public void OnLaunch()
	{
		if (this.GetTargetFloor() > this.GetMaxDropFloor())
		{
			return;
		}
		this.SetState(GRShuttleState.PreMove, false);
		if (this.departCardScanner != null)
		{
			this.departCardScanner.onSucceeded.Invoke();
		}
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x0011C448 File Offset: 0x0011A648
	public void OnArrive()
	{
		this.SetState(GRShuttleState.Arriving, false);
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x0011C452 File Offset: 0x0011A652
	public void OnTargetLevelUp()
	{
		this.targetSection = this.ClampTargetSection(this.targetSection - 1);
		if (this.shuttleUI != null)
		{
			this.shuttleUI.RefreshUI();
		}
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x0011C47B File Offset: 0x0011A67B
	public void OnTargetLevelDown()
	{
		this.targetSection = this.ClampTargetSection(this.targetSection + 1);
		if (this.shuttleUI != null)
		{
			this.shuttleUI.RefreshUI();
		}
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x0011C4A4 File Offset: 0x0011A6A4
	private int ClampTargetSection(int newTargetSection)
	{
		if (this.location == GRShuttleGroupLoc.Staging)
		{
			newTargetSection = Mathf.Clamp(newTargetSection, 1, GRShuttle.sectionFloors.Length - 1);
		}
		else
		{
			newTargetSection = 0;
		}
		return newTargetSection;
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x0011C4C8 File Offset: 0x0011A6C8
	public static void TryStartLocalPlayerShuttleMove(int currShuttleId, NetPlayer shuttleOwner)
	{
		GRPlayer local = GRPlayer.GetLocal();
		if (local == null)
		{
			return;
		}
		GRShuttle shuttle = GRElevatorManager.GetShuttle(currShuttleId);
		if (shuttle == null)
		{
			return;
		}
		if (!GRElevatorManager.IsPlayerInShuttle(local.gamePlayer.rig.OwningNetPlayer.ActorNumber, shuttle, null))
		{
			return;
		}
		if (shuttleOwner != null && shuttleOwner.GetPlayerRef() != null)
		{
			local.shuttleData.ownerUserId = shuttleOwner.UserId;
		}
		else
		{
			local.shuttleData.ownerUserId = VRRig.LocalRig.OwningNetPlayer.UserId;
		}
		local.shuttleData.currShuttleId = currShuttleId;
		local.shuttleData.targetShuttleId = -1;
		local.shuttleData.targetLevel = shuttle.GetTargetFloor();
		GRShuttle.SetPlayerShuttleState(local, GRPlayer.ShuttleState.Moving);
	}

	// Token: 0x060033A6 RID: 13222 RVA: 0x0011C580 File Offset: 0x0011A780
	public static void UpdateGRPlayerShuttle(GRPlayer player)
	{
		if (player == null)
		{
			return;
		}
		GRPlayer.ShuttleData shuttleData = player.shuttleData;
		if (shuttleData == null || shuttleData.state == GRPlayer.ShuttleState.Idle)
		{
			return;
		}
		if (!player.gamePlayer.IsLocal())
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		double num = shuttleData.stateStartTime;
		if (shuttleData.state != GRPlayer.ShuttleState.Idle && timeAsDouble > num + 10.0)
		{
			GRShuttle.CancelPlayerShuttle(player);
			return;
		}
		switch (shuttleData.state)
		{
		case GRPlayer.ShuttleState.Moving:
			if (timeAsDouble > num + 3.0)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.JoinRoom);
				return;
			}
			break;
		case GRPlayer.ShuttleState.WaitForLeaveRoom:
			if (!PhotonNetwork.InRoom)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeadPlayer);
				return;
			}
			break;
		case GRPlayer.ShuttleState.JoinRoom:
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeadPlayer);
				return;
			}
			GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.WaitForLeaveRoom);
			return;
		case GRPlayer.ShuttleState.WaitForLeadPlayer:
			player.shuttleData.targetShuttleId = -1;
			if (PhotonNetwork.InRoom)
			{
				player.shuttleData.targetShuttleId = GRShuttle.CalcTargetShuttleId(player.shuttleData.currShuttleId, player.shuttleData.ownerUserId);
			}
			if (player.shuttleData.targetShuttleId != -1)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Teleport);
				return;
			}
			break;
		case GRPlayer.ShuttleState.Teleport:
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId).zone);
			if (timeAsDouble > num + 1.0 && (managerForZone == null || managerForZone.IsZoneActive()))
			{
				int num2 = GRShuttle.CalcTargetShuttleId(player.shuttleData.currShuttleId, player.shuttleData.ownerUserId);
				if (num2 == player.shuttleData.targetShuttleId)
				{
					GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.PostTeleport);
					return;
				}
				if (num2 != -1)
				{
					player.shuttleData.currShuttleId = player.shuttleData.targetShuttleId;
					player.shuttleData.targetShuttleId = num2;
					GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.TeleportToMyShuttleSafety);
					return;
				}
			}
			break;
		}
		case GRPlayer.ShuttleState.TeleportToMyShuttleSafety:
			GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.PostTeleport);
			return;
		case GRPlayer.ShuttleState.PostTeleport:
			if (timeAsDouble > num + 1.0)
			{
				GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060033A7 RID: 13223 RVA: 0x0011C770 File Offset: 0x0011A970
	public static int CalcTargetShuttleId(int currShuttleId, string ownerUserId)
	{
		GRShuttle shuttle = GRElevatorManager.GetShuttle(currShuttleId);
		if (shuttle.specificDestinationShuttle != null)
		{
			return shuttle.specificDestinationShuttle.shuttleId;
		}
		GRPlayer fromUserId = GRPlayer.GetFromUserId(ownerUserId);
		if (fromUserId != null)
		{
			bool isOnDrillovator = shuttle.GetTargetFloor() >= 0;
			GRShuttle assignedShuttle = fromUserId.GetAssignedShuttle(isOnDrillovator);
			if (assignedShuttle != null)
			{
				return assignedShuttle.shuttleId;
			}
		}
		return -1;
	}

	// Token: 0x060033A8 RID: 13224 RVA: 0x0011C7D4 File Offset: 0x0011A9D4
	public static void CancelPlayerShuttle(GRPlayer player)
	{
		GRPlayer.ShuttleState shuttleState = player.shuttleData.state;
		if (shuttleState - GRPlayer.ShuttleState.Moving > 3)
		{
			if (shuttleState - GRPlayer.ShuttleState.Teleport <= 2)
			{
				GRShuttle shuttle = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
				if (shuttle != null)
				{
					shuttle.OpenDoorLocal();
				}
			}
		}
		else
		{
			GRShuttle shuttle2 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			if (shuttle2 != null)
			{
				shuttle2.OpenDoorLocal();
			}
		}
		GRShuttle.SetPlayerShuttleState(player, GRPlayer.ShuttleState.Idle);
	}

	// Token: 0x060033A9 RID: 13225 RVA: 0x0011C844 File Offset: 0x0011AA44
	public static void SetPlayerShuttleState(GRPlayer player, GRPlayer.ShuttleState newState)
	{
		GRPlayer.ShuttleData shuttleData = player.shuttleData;
		shuttleData.state = newState;
		shuttleData.stateStartTime = Time.timeAsDouble;
		switch (shuttleData.state)
		{
		case GRPlayer.ShuttleState.Moving:
		case GRPlayer.ShuttleState.WaitForLeaveRoom:
		case GRPlayer.ShuttleState.WaitForLeadPlayer:
			break;
		case GRPlayer.ShuttleState.JoinRoom:
		{
			GRShuttle shuttle = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle targetShuttle = shuttle.GetTargetShuttle();
			if (targetShuttle != null && !NetworkSystem.Instance.SessionIsPrivate && shuttle.shuttleOwner.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				GRElevatorManager.LeadShuttleJoin(shuttle.friendCollider, targetShuttle.friendCollider, targetShuttle.joinTrigger, shuttle.GetTargetFloor());
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.Teleport:
		{
			GRShuttle shuttle2 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle shuttle3 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle3 != null)
			{
				GRShuttle.TeleportLocalPlayer(shuttle2, shuttle3);
				shuttle3.CloseDoorLocal();
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.TeleportToMyShuttleSafety:
		{
			GRShuttle shuttle4 = GRElevatorManager.GetShuttle(player.shuttleData.currShuttleId);
			GRShuttle shuttle5 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle5 != null)
			{
				GRShuttle.TeleportLocalPlayer(shuttle4, shuttle5);
				shuttle5.CloseDoorLocal();
				return;
			}
			break;
		}
		case GRPlayer.ShuttleState.PostTeleport:
		{
			GRShuttle shuttle6 = GRElevatorManager.GetShuttle(player.shuttleData.targetShuttleId);
			if (shuttle6 != null)
			{
				shuttle6.RequestArrival();
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x04004343 RID: 17219
	public const int InvalidId = -1;

	// Token: 0x04004344 RID: 17220
	private const int MAX_DEPTH = 29;

	// Token: 0x04004345 RID: 17221
	public GTZone zone;

	// Token: 0x04004346 RID: 17222
	public GRShuttleUI shuttleUI;

	// Token: 0x04004347 RID: 17223
	public GRDoor entryDoor;

	// Token: 0x04004348 RID: 17224
	private GRShuttleGroupLoc location;

	// Token: 0x04004349 RID: 17225
	private int employeeIndex;

	// Token: 0x0400434A RID: 17226
	public AbilitySound takeOffSound;

	// Token: 0x0400434B RID: 17227
	public AbilitySound moveSound;

	// Token: 0x0400434C RID: 17228
	public AbilitySound landSound;

	// Token: 0x0400434D RID: 17229
	public GorillaFriendCollider friendCollider;

	// Token: 0x0400434E RID: 17230
	public GorillaNetworkJoinTrigger joinTrigger;

	// Token: 0x0400434F RID: 17231
	public GRShuttle specificDestinationShuttle;

	// Token: 0x04004350 RID: 17232
	public int specificFloor = -1;

	// Token: 0x04004351 RID: 17233
	public ParticleSystem windowFx;

	// Token: 0x04004352 RID: 17234
	public List<GameObject> hideOnMove;

	// Token: 0x04004353 RID: 17235
	public List<GameObject> showOnMove;

	// Token: 0x04004354 RID: 17236
	public BoxCollider inShuttleVolume;

	// Token: 0x04004355 RID: 17237
	public IDCardScanner entryCardScanner;

	// Token: 0x04004356 RID: 17238
	public IDCardScanner departCardScanner;

	// Token: 0x04004357 RID: 17239
	[NonSerialized]
	public int shuttleId;

	// Token: 0x04004358 RID: 17240
	private GhostReactor reactor;

	// Token: 0x04004359 RID: 17241
	private int targetSection;

	// Token: 0x0400435A RID: 17242
	private GRShuttleState state;

	// Token: 0x0400435B RID: 17243
	private double stateStartTime;

	// Token: 0x0400435C RID: 17244
	private GRBay shuttleBay;

	// Token: 0x0400435D RID: 17245
	private NetPlayer shuttleOwner;

	// Token: 0x0400435E RID: 17246
	private double lastCloseTime;

	// Token: 0x0400435F RID: 17247
	private static int[] sectionFloors = new int[]
	{
		-1,
		0,
		4,
		9,
		14,
		19,
		24,
		29
	};
}
