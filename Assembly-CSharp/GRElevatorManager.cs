using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000766 RID: 1894
[NetworkBehaviourWeaved(0)]
public class GRElevatorManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x06002FF0 RID: 12272 RVA: 0x00104106 File Offset: 0x00102306
	public bool InPrivateRoom
	{
		get
		{
			return NetworkSystem.Instance.SessionIsPrivate;
		}
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06002FF1 RID: 12273 RVA: 0x00104112 File Offset: 0x00102312
	// (set) Token: 0x06002FF2 RID: 12274 RVA: 0x0010411A File Offset: 0x0010231A
	public bool TickRunning { get; set; }

	// Token: 0x06002FF3 RID: 12275 RVA: 0x00104124 File Offset: 0x00102324
	protected override void Awake()
	{
		base.Awake();
		if (GRElevatorManager._instance != null)
		{
			Debug.LogError("Multiple elevator managers! This should never happen!");
			return;
		}
		GRElevatorManager._instance = this;
		this.currentState = GRElevatorManager.ElevatorSystemState.InLocation;
		this.currentLocation = GRElevatorManager.ElevatorLocation.Stump;
		this.destination = GRElevatorManager.ElevatorLocation.Stump;
		this.elevatorByLocation = new Dictionary<GRElevatorManager.ElevatorLocation, GRElevator>();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.elevatorByLocation[this.allElevators[i].location] = this.allElevators[i];
		}
		this.actorIds = new List<int>();
		this.mainStagingShuttle.specificFloor = -1;
		this.mainDrillShuttle.specificFloor = 0;
		this.allShuttles = new List<GRShuttle>(64);
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			GRElevatorManager.GRShuttleGroup grshuttleGroup = this.shuttleGroups[j];
			for (int k = 0; k < grshuttleGroup.ghostReactorStagingShuttles.Count; k++)
			{
				this.allShuttles.Add(grshuttleGroup.ghostReactorStagingShuttles[k]);
				grshuttleGroup.ghostReactorStagingShuttles[k].SetLocation(grshuttleGroup.location);
			}
		}
		this.allShuttles.Add(this.mainStagingShuttle);
		this.allShuttles.Add(this.mainDrillShuttle);
		for (int l = 0; l < this.allShuttles.Count; l++)
		{
			this.allShuttles[l].Init(l);
		}
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x0010429C File Offset: 0x0010249C
	protected override void Start()
	{
		base.Start();
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerAdded;
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerRemoved;
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x00104314 File Offset: 0x00102514
	protected void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnLeftRoom;
		NetworkSystem.Instance.OnPlayerJoined -= this.OnPlayerAdded;
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerRemoved;
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x0010438A File Offset: 0x0010258A
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		TickSystem<object>.AddTickCallback(this);
		this.DestinationVideoPlayer.loopPointReached += this.DisableVideoScreens;
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x001043B5 File Offset: 0x001025B5
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
		this.DestinationVideoPlayer.loopPointReached -= this.DisableVideoScreens;
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x001043E0 File Offset: 0x001025E0
	private void DisableVideoScreens(VideoPlayer source)
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].videoDisplay.SetActive(false);
		}
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x0010441C File Offset: 0x0010261C
	public void Tick()
	{
		if (!this.cosmeticsInitialized)
		{
			this.CheckInitializationState();
			return;
		}
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].PhysicalElevatorUpdate();
		}
		this.ProcessElevatorSystemState();
		if (this.justTeleported)
		{
			this.justTeleported = false;
			GTPlayer.Instance.disableMovement = false;
		}
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x0010447F File Offset: 0x0010267F
	private void CheckInitializationState()
	{
		this.cosmeticsInitialized = true;
		if (GRElevatorManager.InControlOfElevator())
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, GRElevatorManager.ElevatorLocation.Stump);
		}
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x00104498 File Offset: 0x00102698
	public void ProcessElevatorSystemState()
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			if (this.currentLocation == this.destination && this.waitForZoneLoadFallbackTimer >= 0f && this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.waitForZoneLoadFallbackTimer += Time.deltaTime;
				if (this.waitForZoneLoadFallbackTimer >= this.waitForZoneLoadFallbackMaxTime)
				{
					this.OnReachedDestination();
				}
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
		{
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			double time = this.GetTime();
			if (this.elevatorByLocation[this.currentLocation].DoorsFullyClosed() && time >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.WaitingToTeleport, GRElevatorManager.ElevatorLocation.None);
				return;
			}
			if (time >= this.destinationButtonLastPressedTime + (double)this.destinationButtonlastPressedDelay && !this.elevatorByLocation[this.currentLocation].DoorIsClosing())
			{
				this.destinationButtonLastPressedTime = time;
				this.CloseAllElevators();
				return;
			}
			break;
		}
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			if (!GRElevatorManager.InControlOfElevator())
			{
				return;
			}
			if (this.GetTime() >= this.doorsFullyClosedTime + (double)this.doorsFullyClosedDelay && !this.waitingForRemoteTeleport)
			{
				this.ActivateElevating();
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x001045C8 File Offset: 0x001027C8
	public void ActivateElevating()
	{
		if (PhotonNetwork.InRoom)
		{
			this.photonView.RPC("RemoteActivateTeleport", RpcTarget.All, new object[]
			{
				(int)this.currentLocation,
				(int)this.destination,
				GRElevatorManager.LowestActorNumberInElevator()
			});
			return;
		}
		this.ActivateTeleport(this.currentLocation, this.destination, -1, this.GetTime());
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x00104638 File Offset: 0x00102838
	public void LeadElevatorJoin()
	{
		GRElevatorManager.LeadElevatorJoin(this.elevatorByLocation[this.currentLocation].friendCollider, this.elevatorByLocation[this.destination].friendCollider, this.elevatorByLocation[this.destination].joinTrigger);
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x0010468C File Offset: 0x0010288C
	public static void LeadElevatorJoin(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			sourceFriendCollider.RefreshPlayersWithinBounds();
			destinationFriendCollider.RefreshPlayersWithinBounds();
			PhotonNetworkController.Instance.FriendIDList = new List<string>(sourceFriendCollider.playerIDsCurrentlyTouching);
			PhotonNetworkController.Instance.FriendIDList.AddRange(destinationFriendCollider.playerIDsCurrentlyTouching);
			foreach (string text in PhotonNetworkController.Instance.FriendIDList)
			{
			}
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			RoomSystem.SendElevatorFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, sourceFriendCollider, destinationFriendCollider);
			PhotonNetwork.SendAllOutgoingCommands();
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.JoinWithElevator, null, false);
		}
		GRElevatorManager.JoinPublicRoom();
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x001047CC File Offset: 0x001029CC
	public static void LeadShuttleJoin(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger, int targetLevel)
	{
		sourceFriendCollider.RefreshPlayersWithinBounds();
		destinationFriendCollider.RefreshPlayersWithinBounds();
		GorillaComputer.instance.friendJoinCollider = destinationFriendCollider;
		GorillaComputer.instance.UpdateScreen();
		if (NetworkSystem.Instance.InRoom)
		{
			PhotonNetworkController.Instance.FriendIDList = new List<string>(sourceFriendCollider.playerIDsCurrentlyTouching);
			PhotonNetworkController.Instance.FriendIDList.AddRange(destinationFriendCollider.playerIDsCurrentlyTouching);
			foreach (string text in PhotonNetworkController.Instance.FriendIDList)
			{
			}
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			RoomSystem.SendShuttleFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, sourceFriendCollider, destinationFriendCollider);
			PhotonNetwork.SendAllOutgoingCommands();
			List<ValueTuple<string, string>> additionalCustomProperties = null;
			if (targetLevel >= 0)
			{
				int joinDepthSectionFromLevel = GhostReactor.GetJoinDepthSectionFromLevel(targetLevel);
				additionalCustomProperties = new List<ValueTuple<string, string>>
				{
					new ValueTuple<string, string>("ghostReactorDepth", joinDepthSectionFromLevel.ToString())
				};
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.JoinWithElevator, additionalCustomProperties, false);
		}
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(destinationJoinTrigger, JoinType.Solo, null, false);
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x0010495C File Offset: 0x00102B5C
	public void UpdateElevatorState(GRElevatorManager.ElevatorSystemState newState, GRElevatorManager.ElevatorLocation location = GRElevatorManager.ElevatorLocation.None)
	{
		switch (this.currentState)
		{
		case GRElevatorManager.ElevatorSystemState.Dormant:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.elevatorByLocation[this.currentLocation].PlayDing();
				this.OpenElevator(this.destination);
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.maxDoorClosingTime = this.GetTime();
				this.destinationButtonLastPressedTime = this.GetTime();
				this.doorsFullyClosedTime = this.GetTime();
				if (this.destination != this.currentLocation)
				{
					this.destination = location;
					this.PlayDestinationVideo(this.destination);
				}
				this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.InLocation:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				if (location == this.currentLocation)
				{
					this.OpenElevator(this.currentLocation);
				}
				else
				{
					this.CloseAllElevators();
				}
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.destinationButtonLastPressedTime = this.GetTime();
					this.maxDoorClosingTime = this.GetTime();
					this.PlayDestinationVideo(this.destination);
				}
				else
				{
					if (this.elevatorByLocation[this.destination].DoorIsClosing())
					{
						this.OpenElevator(this.currentLocation);
					}
					newState = this.currentState;
				}
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
				this.OpenElevator(location);
				this.elevatorByLocation[this.currentLocation].PlayDing();
				break;
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.PlayDestinationVideo(this.destination);
				}
				break;
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				this.doorsFullyClosedTime = this.GetTime();
				if (this.currentLocation != GRElevatorManager.ElevatorLocation.None)
				{
					this.elevatorByLocation[this.currentLocation].PlayElevatorMoving();
					this.elevatorByLocation[this.currentLocation].PlayElevatorMusic(0f);
				}
				this.elevatorByLocation[this.destination].PlayElevatorMoving();
				break;
			}
			break;
		case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
			switch (newState)
			{
			case GRElevatorManager.ElevatorSystemState.Dormant:
				this.CloseAllElevators();
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.elevatorByLocation[this.destination].PlayElevatorStopped();
				break;
			case GRElevatorManager.ElevatorSystemState.InLocation:
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnReachedDestination));
				this.waitForZoneLoadFallbackTimer = 0.01f;
				this.elevatorByLocation[this.currentLocation].PlayElevatorStopped();
				this.currentLocation = location;
				break;
			}
			case GRElevatorManager.ElevatorSystemState.DestinationPressed:
			case GRElevatorManager.ElevatorSystemState.WaitingToTeleport:
				if (location != this.currentLocation)
				{
					this.destination = location;
					this.PlayDestinationVideo(this.destination);
				}
				else
				{
					this.OpenElevator(location);
					newState = GRElevatorManager.ElevatorSystemState.InLocation;
				}
				break;
			}
			break;
		}
		this.currentState = newState;
		this.UpdateUI();
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x00104D08 File Offset: 0x00102F08
	private void PlayDestinationVideo(GRElevatorManager.ElevatorLocation destination)
	{
		VideoClip clipForDestination = this.getClipForDestination(destination);
		if (this.DestinationVideoPlayer.isPlaying && this.DestinationVideoPlayer.clip != clipForDestination)
		{
			this.DestinationVideoPlayer.Stop();
			this.DisableVideoScreens(this.DestinationVideoPlayer);
		}
		if (clipForDestination != null && this.currentLocation != GRElevatorManager.ElevatorLocation.None)
		{
			this.DestinationVideoPlayer.clip = clipForDestination;
			this.DestinationVideoPlayer.SetTargetAudioSource(0, this.DestinationVideoPlayerAudioSource);
			this.DestinationVideoPlayer.Play();
			this.DestinationVideoPlayerAudioSource.transform.position = this.elevatorByLocation[this.currentLocation].videoAudio.transform.position;
			this.elevatorByLocation[this.currentLocation].videoDisplay.SetActive(true);
		}
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x00104DDC File Offset: 0x00102FDC
	private VideoClip getClipForDestination(GRElevatorManager.ElevatorLocation destination)
	{
		for (int i = 0; i < this.DestinationVideos.Length; i++)
		{
			if (this.DestinationVideos[i].Destination == destination)
			{
				return this.DestinationVideos[i].VideoClip;
			}
		}
		return null;
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x00104E24 File Offset: 0x00103024
	public void UpdateUI()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].outerText.text = "ELEVATOR LOCATION:\n" + this.currentLocation.ToString().ToUpper();
			GRElevatorManager.ElevatorSystemState elevatorSystemState = this.currentState;
			if (elevatorSystemState > GRElevatorManager.ElevatorSystemState.InLocation)
			{
				if (elevatorSystemState - GRElevatorManager.ElevatorSystemState.DestinationPressed <= 1)
				{
					if (this.destination != this.currentLocation)
					{
						this.allElevators[i].innerText.text = "NEXT STOP:\n" + this.destination.ToString().ToUpper();
					}
					else
					{
						this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
					}
				}
			}
			else
			{
				this.allElevators[i].innerText.text = "CHOOSE DESTINATION";
			}
		}
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x00104F14 File Offset: 0x00103114
	public static void RegisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = elevator;
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x00104F3A File Offset: 0x0010313A
	public static void DeregisterElevator(GRElevator elevator)
	{
		if (GRElevatorManager._instance == null)
		{
			return;
		}
		GRElevatorManager._instance.elevatorByLocation[elevator.location] = null;
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x00104F60 File Offset: 0x00103160
	public static void ElevatorButtonPressed(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		if (GRElevatorManager._instance != null)
		{
			GRElevatorManager._instance.ElevatorButtonPressedInternal(type, location);
			if (!GRElevatorManager._instance.IsMine && NetworkSystem.Instance.InRoom)
			{
				GRElevatorManager._instance.photonView.RPC("RemoteElevatorButtonPress", RpcTarget.MasterClient, new object[]
				{
					(int)type,
					(int)location
				});
			}
		}
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x00104FCC File Offset: 0x001031CC
	private void ElevatorButtonPressedInternal(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		GRElevator grelevator;
		if (!this.elevatorByLocation.TryGetValue(location, out grelevator) || grelevator == null)
		{
			Debug.LogWarning(string.Format("[GRElevatorManager] No elevator registered for location '{0}'. Elevator may not be enabled yet or is missing from allElevators.", location), this);
			return;
		}
		grelevator.PressButtonVisuals(type);
		grelevator.PlayButtonPress();
		if (base.IsMine)
		{
			this.ProcessElevatorButtonPress(type, location);
		}
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x00105028 File Offset: 0x00103228
	public void ProcessElevatorButtonPress(GRElevator.ButtonType type, GRElevatorManager.ElevatorLocation location)
	{
		switch (type)
		{
		case GRElevator.ButtonType.Stump:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.Stump);
				return;
			}
			break;
		case GRElevator.ButtonType.City:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.City);
				return;
			}
			break;
		case GRElevator.ButtonType.GhostReactor:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.GhostReactor);
				return;
			}
			break;
		case GRElevator.ButtonType.Open:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				if (this.currentState == GRElevatorManager.ElevatorSystemState.DestinationPressed)
				{
					if (this.GetTime() >= this.maxDoorClosingTime + (double)this.doorMaxClosingDelay)
					{
						break;
					}
					this.destinationButtonLastPressedTime = this.GetTime();
					this.doorsFullyClosedTime = this.GetTime();
				}
				this.OpenElevator(location);
				return;
			}
			break;
		case GRElevator.ButtonType.Close:
			this.CloseAllElevators();
			break;
		case GRElevator.ButtonType.Summon:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport && this.currentState != GRElevatorManager.ElevatorSystemState.DestinationPressed)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, location);
				return;
			}
			break;
		case GRElevator.ButtonType.MonkeBlocks:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.MonkeBlocks);
				return;
			}
			break;
		case GRElevator.ButtonType.VIMExperience1:
			if (this.currentState != GRElevatorManager.ElevatorSystemState.WaitingToTeleport)
			{
				this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.DestinationPressed, GRElevatorManager.ElevatorLocation.VIMExperience1);
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x00105130 File Offset: 0x00103330
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.doorsFullyClosedTime);
		stream.SendNext(this.destinationButtonLastPressedTime);
		stream.SendNext(this.maxDoorClosingTime);
		stream.SendNext((int)this.currentLocation);
		stream.SendNext((int)this.destination);
		stream.SendNext((int)this.currentState);
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			stream.SendNext((int)this.allElevators[i].state);
		}
		for (int j = 0; j < this.allShuttles.Count; j++)
		{
			stream.SendNext((byte)this.allShuttles[j].GetState());
			bool flag = this.allShuttles[j].specificDestinationShuttle == null;
			NetPlayer owner = this.allShuttles[j].GetOwner();
			int num = (!flag || owner == null) ? -1 : owner.ActorNumber;
			stream.SendNext(num);
		}
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x0010524C File Offset: 0x0010344C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		double d = (double)stream.ReceiveNext();
		if (!double.IsNaN(d) && !double.IsInfinity(d))
		{
			this.doorsFullyClosedTime = d;
		}
		d = (double)stream.ReceiveNext();
		if (!double.IsNaN(d) && !double.IsInfinity(d))
		{
			this.destinationButtonLastPressedTime = d;
		}
		d = (double)stream.ReceiveNext();
		if (!double.IsNaN(d) && !double.IsInfinity(d))
		{
			this.maxDoorClosingTime = d;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation = this.currentLocation;
		int num = (int)stream.ReceiveNext();
		if (num >= 0 && num <= 8)
		{
			this.currentLocation = (GRElevatorManager.ElevatorLocation)num;
		}
		GRElevatorManager.ElevatorLocation elevatorLocation2 = this.destination;
		num = (int)stream.ReceiveNext();
		if (num >= 0 && num <= 8)
		{
			this.destination = (GRElevatorManager.ElevatorLocation)num;
		}
		num = (int)stream.ReceiveNext();
		if (num >= 0 && num < 5)
		{
			GRElevatorManager.ElevatorSystemState elevatorSystemState = (GRElevatorManager.ElevatorSystemState)num;
			if (elevatorSystemState != this.currentState && elevatorSystemState == GRElevatorManager.ElevatorSystemState.DestinationPressed)
			{
				this.PlayDestinationVideo(this.destination);
			}
			this.currentState = (GRElevatorManager.ElevatorSystemState)num;
		}
		this.UpdateUI();
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			num = (int)stream.ReceiveNext();
			if (num >= 0 && num < 8)
			{
				this.allElevators[i].UpdateRemoteState((GRElevator.ElevatorState)num);
			}
		}
		for (int j = 0; j < this.allShuttles.Count; j++)
		{
			byte b = (byte)stream.ReceiveNext();
			int num2 = (int)stream.ReceiveNext();
			if (b >= 0 && b < 7)
			{
				this.allShuttles[j].SetState((GRShuttleState)b, false);
			}
			if (this.allShuttles[j].specificDestinationShuttle == null && num2 != -1)
			{
				NetPlayer owner = NetPlayer.Get(num2);
				this.allShuttles[j].SetOwner(owner);
			}
		}
	}

	// Token: 0x0600300B RID: 12299 RVA: 0x0010541E File Offset: 0x0010361E
	[PunRPC]
	public void RemoteElevatorButtonPress(int elevatorButtonPressed, int elevatorLocation, PhotonMessageInfo info)
	{
		if (!base.IsMine || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteElevatorButtonPress))
		{
			return;
		}
		if (elevatorLocation < 0 || elevatorLocation >= 8)
		{
			return;
		}
		if (elevatorButtonPressed < 0 || elevatorButtonPressed >= 12)
		{
			return;
		}
		this.ElevatorButtonPressedInternal((GRElevator.ButtonType)elevatorButtonPressed, (GRElevatorManager.ElevatorLocation)elevatorLocation);
	}

	// Token: 0x0600300C RID: 12300 RVA: 0x00105454 File Offset: 0x00103654
	[PunRPC]
	public void RemoteActivateTeleport(int elevatorStartLocation, int elevatorDestinationLocation, int lowestActorNumber, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || this.m_RpcSpamChecks.IsSpamming(GRElevatorManager.RPC.RemoteActivateTeleport))
		{
			return;
		}
		if (elevatorStartLocation < 0 || elevatorStartLocation >= 8 || elevatorDestinationLocation < 0 || elevatorDestinationLocation >= 8)
		{
			return;
		}
		if (!this.waitingForRemoteTeleport)
		{
			base.StartCoroutine(this.TeleportDelay((GRElevatorManager.ElevatorLocation)elevatorStartLocation, (GRElevatorManager.ElevatorLocation)elevatorDestinationLocation, lowestActorNumber, info.SentServerTime));
		}
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x001054AE File Offset: 0x001036AE
	private IEnumerator TeleportDelay(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double sentServerTime)
	{
		this.timeLastTeleported = (double)Time.time;
		this.waitingForRemoteTeleport = true;
		this.lastTeleportSource = start;
		yield return new WaitForSeconds((float)(PhotonNetwork.Time - (sentServerTime + 0.75)));
		this.RefreshTeleportingPlayersJoinTime();
		yield return new WaitForSeconds(0.25f);
		this.waitingForRemoteTeleport = false;
		this.ActivateTeleport(start, destination, lowestActorNumber, sentServerTime);
		yield break;
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x001054DC File Offset: 0x001036DC
	public void ActivateTeleport(GRElevatorManager.ElevatorLocation start, GRElevatorManager.ElevatorLocation destination, int lowestActorNumber, double photonServerTime)
	{
		GRElevator grelevator = this.elevatorByLocation[start];
		GRElevator grelevator2 = this.elevatorByLocation[destination];
		if (grelevator == null || grelevator2 == null)
		{
			return;
		}
		grelevator.friendCollider.RefreshPlayersWithinBounds();
		if (!PhotonNetwork.InRoom)
		{
			this.RefreshTeleportingPlayersJoinTime();
		}
		if (!grelevator.friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
		{
			this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
			return;
		}
		this.elevatorByLocation[destination].collidersAndVisuals.SetActive(true);
		if (this.DestinationVideoPlayer.isPlaying)
		{
			this.elevatorByLocation[destination].videoDisplay.SetActive(true);
			this.DestinationVideoPlayerAudioSource.transform.position = this.elevatorByLocation[destination].videoAudio.transform.position;
		}
		float num = grelevator2.transform.rotation.eulerAngles.y - grelevator.transform.rotation.eulerAngles.y;
		GTPlayer instance = GTPlayer.Instance;
		VRRig localRig = VRRig.LocalRig;
		Vector3 b = localRig.transform.position - instance.transform.position;
		Vector3 b2 = instance.headCollider.transform.position - instance.transform.position;
		Vector3 a = grelevator2.transform.TransformPoint(grelevator.transform.InverseTransformPoint(instance.transform.position));
		Vector3 point = localRig.transform.position - grelevator.transform.position;
		point.x *= 0.8f;
		point.z *= 0.8f;
		a = grelevator2.transform.position + (Quaternion.Euler(0f, num, 0f) * point - b) + localRig.headConstraint.rotation * localRig.head.trackingPositionOffset;
		Vector3 b3 = Vector3.zero;
		Vector3 vector = grelevator2.transform.position + (Quaternion.Euler(0f, num, 0f) * point - b) + b2 - grelevator2.transform.position;
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		if (Physics.SphereCastNonAlloc(grelevator2.transform.position, instance.headCollider.radius * 1.5f, vector, this.correctionRaycastHit, magnitude * 1.05f, this.correctionRaycastMask) > 0)
		{
			b3 = vector * instance.headCollider.radius * -1.5f;
		}
		instance.TeleportTo(a + b3, instance.transform.rotation, false, false);
		instance.turnParent.transform.RotateAround(instance.headCollider.transform.position, base.transform.up, num);
		localRig.transform.position = instance.transform.position + b;
		instance.InitializeValues();
		this.justTeleported = true;
		instance.disableMovement = true;
		GorillaComputer.instance.allowedMapsToJoin = this.elevatorByLocation[destination].joinTrigger.myCollider.myAllowedMapsToJoin;
		this.lastTeleportSource = start;
		this.lastLowestActorNr = lowestActorNumber;
		if (!this.InPrivateRoom && lowestActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.LeadElevatorJoin();
		}
		this.UpdateElevatorState(GRElevatorManager.ElevatorSystemState.InLocation, destination);
		grelevator2.PlayElevatorMusic(grelevator.musicAudio.time);
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x00105898 File Offset: 0x00103A98
	public void CloseAllElevators()
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			if (!this.allElevators[i].DoorIsClosing())
			{
				this.allElevators[i].UpdateLocalState(GRElevator.ElevatorState.DoorBeginClosing);
			}
		}
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x001058E0 File Offset: 0x00103AE0
	public void OpenElevator(GRElevatorManager.ElevatorLocation location)
	{
		for (int i = 0; i < this.allElevators.Count; i++)
		{
			this.allElevators[i].UpdateLocalState((this.allElevators[i].location == location) ? GRElevator.ElevatorState.DoorBeginOpening : GRElevator.ElevatorState.DoorBeginClosing);
		}
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x0010592C File Offset: 0x00103B2C
	public double GetTime()
	{
		double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time);
		if (this.doorsFullyClosedTime > num || this.destinationButtonLastPressedTime > num || this.maxDoorClosingTime > num || num - this.doorsFullyClosedTime > 10.0 || num - this.destinationButtonLastPressedTime > 10.0 || num - this.maxDoorClosingTime > 20.0)
		{
			this.doorsFullyClosedTime = num;
			this.destinationButtonLastPressedTime = num;
			this.maxDoorClosingTime = num;
		}
		return num;
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x001059B8 File Offset: 0x00103BB8
	public static bool ValidElevatorNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (RoomSystem.WasRoomPrivate)
		{
			return false;
		}
		if (actorNr == GRElevatorManager._instance.lastLowestActorNr)
		{
			return true;
		}
		if (GRElevatorManager._instance.lastTeleportSource == GRElevatorManager.ElevatorLocation.None)
		{
			return false;
		}
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.lastTeleportSource].friendCollider;
		if ((double)Time.time < GRElevatorManager._instance.timeLastTeleported + 3.0)
		{
			friendCollider.RefreshPlayersWithinBounds();
			friendCollider2.RefreshPlayersWithinBounds();
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		return netPlayer != null && (friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId));
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x00105A94 File Offset: 0x00103C94
	public static bool ValidShuttleNetworking(int actorNr)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		if (RoomSystem.WasRoomPrivate)
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(actorNr);
		if (grplayer == null)
		{
			return false;
		}
		GRShuttle shuttle = GRElevatorManager.GetShuttle(grplayer.shuttleData.currShuttleId);
		GRShuttle shuttle2 = GRElevatorManager.GetShuttle(grplayer.shuttleData.targetShuttleId);
		if (shuttle == null)
		{
			return false;
		}
		if (shuttle2 == null)
		{
			shuttle2 = GRElevatorManager.GetShuttle(GRShuttle.CalcTargetShuttleId(grplayer.shuttleData.currShuttleId, grplayer.shuttleData.ownerUserId));
			if (shuttle2 == null)
			{
				return false;
			}
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		if (netPlayer == null)
		{
			return false;
		}
		if (netPlayer == shuttle.GetOwner())
		{
			return true;
		}
		GorillaFriendCollider friendCollider = shuttle2.friendCollider;
		GorillaFriendCollider friendCollider2 = shuttle.friendCollider;
		friendCollider.RefreshPlayersWithinBounds();
		friendCollider2.RefreshPlayersWithinBounds();
		return friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x00105B88 File Offset: 0x00103D88
	public static bool IsPlayerInShuttle(int actorNr, GRShuttle currShuttle, GRShuttle targetShuttle)
	{
		if (GRElevatorManager._instance == null)
		{
			return false;
		}
		NetPlayer netPlayer = NetPlayer.Get(actorNr);
		if (netPlayer == null)
		{
			return false;
		}
		bool flag = false;
		if (currShuttle != null)
		{
			GorillaFriendCollider friendCollider = currShuttle.friendCollider;
			if (friendCollider != null)
			{
				friendCollider.RefreshPlayersWithinBounds();
			}
			flag = friendCollider.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
		}
		bool flag2 = false;
		if (targetShuttle != null)
		{
			GorillaFriendCollider friendCollider2 = targetShuttle.friendCollider;
			if (friendCollider2 != null)
			{
				friendCollider2.RefreshPlayersWithinBounds();
			}
			friendCollider2.playerIDsCurrentlyTouching.Contains(netPlayer.UserId);
		}
		return flag || flag2;
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x00105C20 File Offset: 0x00103E20
	public static int LowestActorNumberInElevator()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		GorillaFriendCollider friendCollider2 = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].friendCollider;
		friendCollider.RefreshPlayersWithinBounds();
		friendCollider2.RefreshPlayersWithinBounds();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || friendCollider2.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x00105CE0 File Offset: 0x00103EE0
	public static int LowestActorNumberInElevator(GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider)
	{
		sourceFriendCollider.RefreshPlayersWithinBounds();
		destinationFriendCollider.RefreshPlayersWithinBounds();
		int num = int.MaxValue;
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			if (num > allNetPlayers[i].ActorNumber && (sourceFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || destinationFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
			{
				num = allNetPlayers[i].ActorNumber;
			}
		}
		return num;
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x00105D58 File Offset: 0x00103F58
	private void RefreshTeleportingPlayersJoinTime()
	{
		GorillaFriendCollider friendCollider = GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.currentLocation].friendCollider;
		this.actorIds.Clear();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			RigContainer rigContainer;
			if (friendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) && VRRigCache.Instance.TryGetVrrig(allNetPlayers[i], out rigContainer))
			{
				rigContainer.Rig.ResetTimeSpawned();
			}
		}
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x00105DD9 File Offset: 0x00103FD9
	public static bool InControlOfElevator()
	{
		return !NetworkSystem.Instance.InRoom || GRElevatorManager._instance.IsMine;
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x00105DF3 File Offset: 0x00103FF3
	public static void JoinPublicRoom()
	{
		PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GRElevatorManager._instance.elevatorByLocation[GRElevatorManager._instance.destination].joinTrigger, JoinType.Solo, null, false);
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x00105E24 File Offset: 0x00104024
	public void OnReachedDestination()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnReachedDestination));
		this.elevatorByLocation[this.destination].PlayElevatorStopped();
		if (this.currentLocation == this.destination)
		{
			this.OpenElevator(this.currentLocation);
			this.elevatorByLocation[this.currentLocation].PlayDing();
		}
		this.waitForZoneLoadFallbackTimer = -1f;
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x00105EA8 File Offset: 0x001040A8
	public static GRShuttle GetShuttle(int shuttleId)
	{
		if (GRElevatorManager._instance == null)
		{
			return null;
		}
		return GRElevatorManager._instance.GetShuttleById(shuttleId);
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x00105EC4 File Offset: 0x001040C4
	public void InitShuttles(GhostReactor reactor)
	{
		for (int i = 0; i < this.allShuttles.Count; i++)
		{
			this.allShuttles[i].SetReactor(reactor);
		}
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x00105EFC File Offset: 0x001040FC
	public GRShuttle GetPlayerShuttle(GRShuttleGroupLoc shuttleGroupLoc, int shuttleIndex)
	{
		int i = 0;
		while (i < this.shuttleGroups.Count)
		{
			if (this.shuttleGroups[i].location == shuttleGroupLoc)
			{
				if (shuttleIndex < 0 || shuttleIndex >= this.shuttleGroups[i].ghostReactorStagingShuttles.Count)
				{
					Debug.LogErrorFormat("Invalid Shuttle Index {0} of {1}", new object[]
					{
						shuttleIndex,
						this.shuttleGroups[i].ghostReactorStagingShuttles.Count
					});
					return null;
				}
				return this.shuttleGroups[i].ghostReactorStagingShuttles[shuttleIndex];
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x00105FA8 File Offset: 0x001041A8
	public GRShuttle GetDrillShuttleForPlayer(int actorNumber)
	{
		return this.GetShuttleForPlayer(actorNumber, GRShuttleGroupLoc.Drill);
	}

	// Token: 0x0600301F RID: 12319 RVA: 0x00105FB2 File Offset: 0x001041B2
	public GRShuttle GetStagingShuttleForPlayer(int actorNumber)
	{
		return this.GetShuttleForPlayer(actorNumber, GRShuttleGroupLoc.Staging);
	}

	// Token: 0x06003020 RID: 12320 RVA: 0x00105FBC File Offset: 0x001041BC
	public GRShuttle GetShuttleForPlayer(int actorNumber, GRShuttleGroupLoc shuttleGroupLoc)
	{
		for (int i = 0; i < this.shuttleGroups.Count; i++)
		{
			if (this.shuttleGroups[i].location == shuttleGroupLoc)
			{
				for (int j = 0; j < this.shuttleGroups[i].ghostReactorStagingShuttles.Count; j++)
				{
					GRShuttle grshuttle = this.shuttleGroups[i].ghostReactorStagingShuttles[j];
					if (!(grshuttle == null))
					{
						NetPlayer owner = grshuttle.GetOwner();
						if (owner != null && owner.ActorNumber == actorNumber)
						{
							return grshuttle;
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x0010604C File Offset: 0x0010424C
	public GRShuttle GetShuttleById(int shuttleId)
	{
		for (int i = 0; i < this.allShuttles.Count; i++)
		{
			if (this.allShuttles[i].shuttleId == shuttleId)
			{
				return this.allShuttles[i];
			}
		}
		return null;
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x00106094 File Offset: 0x00104294
	private int AddPlayer(NetPlayer netPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return -1;
		}
		int num = -1;
		List<GRShuttle> ghostReactorStagingShuttles = this.shuttleGroups[0].ghostReactorStagingShuttles;
		for (int i = 0; i < ghostReactorStagingShuttles.Count; i++)
		{
			if (ghostReactorStagingShuttles[i].GetOwner() == null)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return -1;
		}
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			this.shuttleGroups[j].ghostReactorStagingShuttles[num].SetOwner(netPlayer);
		}
		return num;
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x0010611C File Offset: 0x0010431C
	private void RemovePlayer(NetPlayer netPlayer)
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		int num = -1;
		List<GRShuttle> ghostReactorStagingShuttles = this.shuttleGroups[0].ghostReactorStagingShuttles;
		for (int i = 0; i < ghostReactorStagingShuttles.Count; i++)
		{
			if (ghostReactorStagingShuttles[i].GetOwner() == netPlayer)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return;
		}
		for (int j = 0; j < this.shuttleGroups.Count; j++)
		{
			this.shuttleGroups[j].ghostReactorStagingShuttles[num].SetOwner(null);
		}
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x001061A4 File Offset: 0x001043A4
	public void OnLeftRoom()
	{
		for (int i = 0; i < this.shuttleGroups.Count; i++)
		{
			for (int j = 0; j < this.shuttleGroups[i].ghostReactorStagingShuttles.Count; j++)
			{
				GRShuttle grshuttle = this.shuttleGroups[i].ghostReactorStagingShuttles[j];
				if (!(grshuttle == null))
				{
					grshuttle.SetOwner(null);
				}
			}
		}
	}

	// Token: 0x06003025 RID: 12325 RVA: 0x00106210 File Offset: 0x00104410
	public void OnPlayerAdded(NetPlayer player)
	{
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			return;
		}
		this.AddPlayer(player);
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x0010622F File Offset: 0x0010442F
	public void OnPlayerRemoved(NetPlayer player)
	{
		if (!PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			return;
		}
		this.RemovePlayer(player);
	}

	// Token: 0x06003027 RID: 12327 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void WriteDataFusion()
	{
	}

	// Token: 0x06003028 RID: 12328 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ReadDataFusion()
	{
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04003D87 RID: 15751
	public PhotonView photonView;

	// Token: 0x04003D88 RID: 15752
	public static GRElevatorManager _instance;

	// Token: 0x04003D89 RID: 15753
	public Dictionary<GRElevatorManager.ElevatorLocation, GRElevator> elevatorByLocation;

	// Token: 0x04003D8A RID: 15754
	public List<GRElevator> allElevators;

	// Token: 0x04003D8B RID: 15755
	[SerializeField]
	private GRElevatorManager.ElevatorLocation destination;

	// Token: 0x04003D8C RID: 15756
	[SerializeField]
	private GRElevatorManager.ElevatorLocation currentLocation;

	// Token: 0x04003D8D RID: 15757
	private GRElevatorManager.ElevatorLocation lastTeleportSource = GRElevatorManager.ElevatorLocation.None;

	// Token: 0x04003D8E RID: 15758
	public GRElevatorManager.ElevatorSystemState currentState;

	// Token: 0x04003D8F RID: 15759
	private double timeLastTeleported;

	// Token: 0x04003D90 RID: 15760
	private bool cosmeticsInitialized;

	// Token: 0x04003D91 RID: 15761
	[SerializeField]
	private List<GRElevatorManager.GRShuttleGroup> shuttleGroups;

	// Token: 0x04003D92 RID: 15762
	public GRShuttle mainStagingShuttle;

	// Token: 0x04003D93 RID: 15763
	public GRShuttle mainDrillShuttle;

	// Token: 0x04003D94 RID: 15764
	private List<GRShuttle> allShuttles;

	// Token: 0x04003D95 RID: 15765
	public float destinationButtonlastPressedDelay = 3f;

	// Token: 0x04003D96 RID: 15766
	public float doorsFullyClosedDelay = 3f;

	// Token: 0x04003D97 RID: 15767
	public float doorMaxClosingDelay = 12f;

	// Token: 0x04003D98 RID: 15768
	public double destinationButtonLastPressedTime;

	// Token: 0x04003D99 RID: 15769
	public double doorsFullyClosedTime;

	// Token: 0x04003D9A RID: 15770
	public double maxDoorClosingTime;

	// Token: 0x04003D9B RID: 15771
	private List<int> actorIds;

	// Token: 0x04003D9C RID: 15772
	public CallLimitersList<CallLimiter, GRElevatorManager.RPC> m_RpcSpamChecks = new CallLimitersList<CallLimiter, GRElevatorManager.RPC>();

	// Token: 0x04003D9D RID: 15773
	private bool justTeleported;

	// Token: 0x04003D9E RID: 15774
	private bool waitingForRemoteTeleport;

	// Token: 0x04003D9F RID: 15775
	private int lastLowestActorNr;

	// Token: 0x04003DA0 RID: 15776
	private RaycastHit[] correctionRaycastHit = new RaycastHit[1];

	// Token: 0x04003DA1 RID: 15777
	public LayerMask correctionRaycastMask;

	// Token: 0x04003DA2 RID: 15778
	private float waitForZoneLoadFallbackTimer;

	// Token: 0x04003DA3 RID: 15779
	public float waitForZoneLoadFallbackMaxTime = 5f;

	// Token: 0x04003DA5 RID: 15781
	[SerializeField]
	private GRElevatorManager.DestinationVideo[] DestinationVideos;

	// Token: 0x04003DA6 RID: 15782
	[SerializeField]
	private VideoPlayer DestinationVideoPlayer;

	// Token: 0x04003DA7 RID: 15783
	[SerializeField]
	private AudioSource DestinationVideoPlayerAudioSource;

	// Token: 0x02000767 RID: 1895
	[Serializable]
	public class GRShuttleGroup
	{
		// Token: 0x04003DA8 RID: 15784
		public GRShuttleGroupLoc location;

		// Token: 0x04003DA9 RID: 15785
		public List<GRShuttle> ghostReactorStagingShuttles;
	}

	// Token: 0x02000768 RID: 1896
	public enum ElevatorSystemState
	{
		// Token: 0x04003DAB RID: 15787
		Dormant,
		// Token: 0x04003DAC RID: 15788
		InLocation,
		// Token: 0x04003DAD RID: 15789
		DestinationPressed,
		// Token: 0x04003DAE RID: 15790
		WaitingToTeleport,
		// Token: 0x04003DAF RID: 15791
		Teleporting,
		// Token: 0x04003DB0 RID: 15792
		None
	}

	// Token: 0x02000769 RID: 1897
	public enum RPC
	{
		// Token: 0x04003DB2 RID: 15794
		RemoteElevatorButtonPress,
		// Token: 0x04003DB3 RID: 15795
		RemoteActivateTeleport
	}

	// Token: 0x0200076A RID: 1898
	public enum ElevatorLocation
	{
		// Token: 0x04003DB5 RID: 15797
		Stump,
		// Token: 0x04003DB6 RID: 15798
		City,
		// Token: 0x04003DB7 RID: 15799
		GhostReactor,
		// Token: 0x04003DB8 RID: 15800
		MonkeBlocks,
		// Token: 0x04003DB9 RID: 15801
		VIMExperience1,
		// Token: 0x04003DBA RID: 15802
		VIMExperience2,
		// Token: 0x04003DBB RID: 15803
		VIMExperience3,
		// Token: 0x04003DBC RID: 15804
		VIMExperience4,
		// Token: 0x04003DBD RID: 15805
		None
	}

	// Token: 0x0200076B RID: 1899
	[Serializable]
	public struct DestinationVideo
	{
		// Token: 0x04003DBE RID: 15806
		public GRElevatorManager.ElevatorLocation Destination;

		// Token: 0x04003DBF RID: 15807
		public VideoClip VideoClip;
	}
}
