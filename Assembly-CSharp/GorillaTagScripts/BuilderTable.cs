using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BoingKit;
using CjLib;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000EDF RID: 3807
	public class BuilderTable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06005DD5 RID: 24021 RVA: 0x001DC58D File Offset: 0x001DA78D
		// (set) Token: 0x06005DD6 RID: 24022 RVA: 0x001DC595 File Offset: 0x001DA795
		public bool TickRunning { get; set; }

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06005DD7 RID: 24023 RVA: 0x001DC59E File Offset: 0x001DA79E
		[HideInInspector]
		public float gridSize
		{
			get
			{
				return this.pieceScale / 2f;
			}
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x001DC5AC File Offset: 0x001DA7AC
		private void ExecuteAction(BuilderAction action)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(action.pieceId);
			BuilderPiece piece2 = this.GetPiece(action.parentPieceId);
			int playerActorNumber = action.playerActorNumber;
			bool flag = PhotonNetwork.LocalPlayer.ActorNumber == action.playerActorNumber;
			switch (action.type)
			{
			case BuilderActionType.AttachToPlayer:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				RigContainer rigContainer;
				if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerActorNumber), out rigContainer))
				{
					string.Format("Execute Builder Action {0} {1} {2} {3} {4}", new object[]
					{
						action.localCommandId,
						action.type,
						action.pieceId,
						action.playerActorNumber,
						action.isLeftHand
					});
					return;
				}
				BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
				Transform parentHeld = action.isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
				piece.SetParentHeld(parentHeld, playerActorNumber, action.isLeftHand);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				BuilderPiece.State newState = flag ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed;
				piece.SetState(newState, false);
				if (!flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				if (flag)
				{
					BuilderPieceInteractor.instance.AddPieceToHeld(piece, action.isLeftHand, action.localPosition, action.localRotation);
					return;
				}
				break;
			}
			case BuilderActionType.DetachFromPlayer:
				if (flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				return;
			case BuilderActionType.AttachToPiece:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				Quaternion identity = Quaternion.identity;
				Vector3 zero = Vector3.zero;
				Vector3 position = piece.transform.position;
				Quaternion rotation = piece.transform.rotation;
				if (piece2 != null)
				{
					piece.BumpTwistToPositionRotation(action.twist, action.bumpOffsetx, action.bumpOffsetz, action.attachIndex, piece2.gridPlanes[action.parentAttachIndex], out zero, out identity, out position, out rotation);
				}
				piece.transform.SetPositionAndRotation(position, rotation);
				BuilderPiece.State stateWhenPlaced;
				if (piece2 == null)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedToArm;
				}
				else if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					stateWhenPlaced = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.state == BuilderPiece.State.Grabbed)
				{
					stateWhenPlaced = BuilderPiece.State.Grabbed;
				}
				else if (piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					stateWhenPlaced = BuilderPiece.State.GrabbedLocal;
				}
				else
				{
					stateWhenPlaced = BuilderPiece.State.AttachedToDropped;
				}
				BuilderPiece rootPiece = piece2.GetRootPiece();
				this.gridPlaneData.Clear();
				this.checkGridPlaneData.Clear();
				this.allPotentialPlacements.Clear();
				BuilderTable.tempPieceSet.Clear();
				QueryParameters queryParameters = new QueryParameters
				{
					layerMask = this.allPiecesMask
				};
				OverlapSphereCommand value = new OverlapSphereCommand(position, 1f, queryParameters);
				this.nearbyPiecesCommands[0] = value;
				OverlapSphereCommand.ScheduleBatch(this.nearbyPiecesCommands, this.nearbyPiecesResults, 1, 1024, default(JobHandle)).Complete();
				int num = 0;
				while (num < 1024 && this.nearbyPiecesResults[num].instanceID != 0)
				{
					BuilderPiece pieceInHand = piece;
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.nearbyPiecesResults[num].collider);
					if (builderPieceFromCollider != null && !BuilderTable.tempPieceSet.Contains(builderPieceFromCollider))
					{
						BuilderTable.tempPieceSet.Add(builderPieceFromCollider);
						if (this.CanPiecesPotentiallyOverlap(pieceInHand, rootPiece, stateWhenPlaced, builderPieceFromCollider))
						{
							for (int i = 0; i < builderPieceFromCollider.gridPlanes.Count; i++)
							{
								BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[i], -1);
								this.checkGridPlaneData.Add(builderGridPlaneData);
							}
						}
					}
					num++;
				}
				BuilderTableJobs.BuildTestPieceListForJob(piece, this.gridPlaneData);
				BuilderPotentialPlacement potentialPlacement = new BuilderPotentialPlacement
				{
					localPosition = zero,
					localRotation = identity,
					attachIndex = action.attachIndex,
					parentAttachIndex = action.parentAttachIndex,
					attachPiece = piece,
					parentPiece = piece2
				};
				this.CalcAllPotentialPlacements(this.gridPlaneData, this.checkGridPlaneData, potentialPlacement, this.allPotentialPlacements);
				piece.SetParentPiece(action.attachIndex, piece2, action.parentAttachIndex);
				for (int j = 0; j < this.allPotentialPlacements.Count; j++)
				{
					BuilderPotentialPlacement builderPotentialPlacement = this.allPotentialPlacements[j];
					BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex];
					BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
					BuilderAttachGridPlane movingParentGrid = builderAttachGridPlane.GetMovingParentGrid();
					bool flag2 = movingParentGrid != null;
					BuilderAttachGridPlane movingParentGrid2 = builderAttachGridPlane2.GetMovingParentGrid();
					bool flag3 = movingParentGrid2 != null;
					if (flag2 == flag3 && (!flag2 || !(movingParentGrid != movingParentGrid2)))
					{
						SnapOverlap newOverlap = this.builderPool.CreateSnapOverlap(builderAttachGridPlane2, builderPotentialPlacement.attachBounds);
						builderAttachGridPlane.AddSnapOverlap(newOverlap);
						SnapOverlap newOverlap2 = this.builderPool.CreateSnapOverlap(builderAttachGridPlane, builderPotentialPlacement.parentAttachBounds);
						builderAttachGridPlane2.AddSnapOverlap(newOverlap2);
					}
				}
				piece.transform.SetLocalPositionAndRotation(zero, identity);
				if (piece2 != null && piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					BuilderPiece rootPiece2 = piece2.GetRootPiece();
					BuilderPieceInteractor.instance.OnCountChangedForRoot(rootPiece2);
				}
				if (piece2 == null)
				{
					piece.SetActivateTimeStamp(action.timeStamp);
					piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
					this.SetIsDirty(true);
					if (flag)
					{
						BuilderPieceInteractor.instance.DisableCollisionsWithHands();
						return;
					}
				}
				else
				{
					if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
					{
						piece.SetState(BuilderPiece.State.AttachedToArm, false);
						return;
					}
					if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
					{
						piece.SetActivateTimeStamp(action.timeStamp);
						piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
						if (piece2 != null)
						{
							BuilderPiece attachedBuiltInPiece = piece2.GetAttachedBuiltInPiece();
							BuilderPiecePrivatePlot builderPiecePrivatePlot;
							if (attachedBuiltInPiece != null && attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
							{
								builderPiecePrivatePlot.OnPieceAttachedToPlot(piece);
							}
						}
						this.SetIsDirty(true);
						if (flag)
						{
							BuilderPieceInteractor.instance.DisableCollisionsWithHands();
							return;
						}
					}
					else
					{
						if (piece2.state == BuilderPiece.State.Grabbed)
						{
							piece.SetState(BuilderPiece.State.Grabbed, false);
							return;
						}
						if (piece2.state == BuilderPiece.State.GrabbedLocal)
						{
							piece.SetState(BuilderPiece.State.GrabbedLocal, false);
							return;
						}
						piece.SetState(BuilderPiece.State.AttachedToDropped, false);
						return;
					}
				}
				break;
			}
			case BuilderActionType.DetachFromPiece:
			{
				BuilderPiece piece3 = piece;
				bool flag4 = piece.state == BuilderPiece.State.GrabbedLocal;
				if (flag4)
				{
					piece3 = piece.GetRootPiece();
				}
				if (piece.state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.SetIsDirty(true);
					BuilderPiece attachedBuiltInPiece2 = piece.GetAttachedBuiltInPiece();
					BuilderPiecePrivatePlot builderPiecePrivatePlot2;
					if (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.TryGetPlotComponent(out builderPiecePrivatePlot2))
					{
						builderPiecePrivatePlot2.OnPieceDetachedFromPlot(piece);
					}
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				if (flag4)
				{
					BuilderPieceInteractor.instance.OnCountChangedForRoot(piece3);
					return;
				}
				break;
			}
			case BuilderActionType.MakePieceRoot:
				BuilderPiece.MakePieceRoot(piece);
				return;
			case BuilderActionType.DropPiece:
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				piece.SetState(BuilderPiece.State.Dropped, false);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				if (piece.rigidBody != null)
				{
					piece.rigidBody.position = action.localPosition;
					piece.rigidBody.rotation = action.localRotation;
					piece.rigidBody.linearVelocity = action.velocity;
					piece.rigidBody.angularVelocity = action.angVelocity;
					return;
				}
				break;
			case BuilderActionType.AttachToShelf:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				int attachIndex = action.attachIndex;
				bool isLeftHand = action.isLeftHand;
				int parentAttachIndex = action.parentAttachIndex;
				float x = action.velocity.x;
				piece.transform.localScale = Vector3.one;
				piece.SetState(isLeftHand ? BuilderPiece.State.OnConveyor : BuilderPiece.State.OnShelf, false);
				if (isLeftHand)
				{
					if (attachIndex >= 0 && attachIndex < this.conveyors.Count)
					{
						BuilderConveyor builderConveyor = this.conveyors[attachIndex];
						float num2 = x / builderConveyor.GetFrameMovement();
						if (PhotonNetwork.ServerTimestamp >= parentAttachIndex)
						{
							uint num3 = (uint)(PhotonNetwork.ServerTimestamp - parentAttachIndex);
							num2 += num3 / 1000f;
						}
						piece.shelfOwner = attachIndex;
						builderConveyor.OnShelfPieceCreated(piece, num2);
						return;
					}
				}
				else
				{
					if (attachIndex >= 0 && attachIndex < this.dispenserShelves.Count)
					{
						BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[attachIndex];
						piece.shelfOwner = attachIndex;
						builderDispenserShelf.OnShelfPieceCreated(piece, false);
						return;
					}
					piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x001DCE60 File Offset: 0x001DB060
		public static bool AreStatesCompatibleForOverlap(BuilderPiece.State stateA, BuilderPiece.State stateB, BuilderPiece rootA, BuilderPiece rootB)
		{
			switch (stateA)
			{
			case BuilderPiece.State.None:
				return false;
			case BuilderPiece.State.AttachedAndPlaced:
				return stateB == BuilderPiece.State.AttachedAndPlaced;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Dropped:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.OnConveyor:
				return (stateB == BuilderPiece.State.AttachedToDropped || stateB == BuilderPiece.State.Dropped || stateB == BuilderPiece.State.OnShelf || stateB == BuilderPiece.State.OnConveyor) && rootA.Equals(rootB);
			case BuilderPiece.State.Grabbed:
				return stateB == BuilderPiece.State.Grabbed && rootA.Equals(rootB);
			case BuilderPiece.State.Displayed:
				return false;
			case BuilderPiece.State.GrabbedLocal:
				return stateB == BuilderPiece.State.GrabbedLocal && rootA.heldInLeftHand == rootB.heldInLeftHand;
			case BuilderPiece.State.AttachedToArm:
			{
				if (stateB != BuilderPiece.State.AttachedToArm)
				{
					return false;
				}
				object obj = (rootA.parentPiece != null) ? rootA.parentPiece : rootA;
				BuilderPiece obj2 = (rootB.parentPiece != null) ? rootB.parentPiece : rootB;
				return obj.Equals(obj2);
			}
			default:
				return false;
			}
		}

		// Token: 0x17000904 RID: 2308
		// (get) Token: 0x06005DDA RID: 24026 RVA: 0x001DCF25 File Offset: 0x001DB125
		// (set) Token: 0x06005DDB RID: 24027 RVA: 0x001DCF2D File Offset: 0x001DB12D
		public int CurrentSaveSlot
		{
			get
			{
				return this.currentSaveSlot;
			}
			set
			{
				if (this.saveInProgress)
				{
					return;
				}
				if (!BuilderScanKiosk.IsSaveSlotValid(value))
				{
					this.currentSaveSlot = -1;
				}
				if (this.currentSaveSlot != value)
				{
					this.SetIsDirty(true);
				}
				this.currentSaveSlot = value;
			}
		}

		// Token: 0x06005DDC RID: 24028 RVA: 0x001DCF60 File Offset: 0x001DB160
		private void Awake()
		{
			if (BuilderTable.zoneToInstance == null)
			{
				BuilderTable.zoneToInstance = new Dictionary<GTZone, BuilderTable>(2);
			}
			if (!BuilderTable.zoneToInstance.TryAdd(this.tableZone, this))
			{
				Object.Destroy(this);
			}
			this.acceptableSqrDistFromCenter = Mathf.Pow(217f * this.pieceScale, 2f);
			if (this.buttonSnapRotation != null)
			{
				this.buttonSnapRotation.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreeRotation));
				this.buttonSnapRotation.SetPressed(this.useSnapRotation);
			}
			if (this.buttonSnapPosition != null)
			{
				this.buttonSnapPosition.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreePosition));
				this.buttonSnapPosition.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
			}
			if (this.buttonSaveLayout != null)
			{
				this.buttonSaveLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonSaveLayout));
			}
			if (this.buttonClearLayout != null)
			{
				this.buttonClearLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonClearLayout));
			}
			this.isSetup = false;
			this.nextPieceId = 10000;
			BuilderTable.placedLayer = LayerMask.NameToLayer("Gorilla Object");
			BuilderTable.heldLayerLocal = LayerMask.NameToLayer("Prop");
			BuilderTable.heldLayer = LayerMask.NameToLayer("BuilderProp");
			BuilderTable.droppedLayer = LayerMask.NameToLayer("BuilderProp");
			this.currSnapParams = this.pushAndEaseParams;
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
			this.inRoom = false;
			this.inBuilderZone = false;
			this.builderNetworking.SetTable(this);
			this.plotOwners = new Dictionary<int, int>(10);
			this.doesLocalPlayerOwnPlot = false;
			this.queuedBuildCommands = new List<BuilderTable.BuilderCommand>(1028);
			if (this.isTableMutable)
			{
				this.playerToArmShelfLeft = new Dictionary<int, int>(10);
				this.playerToArmShelfRight = new Dictionary<int, int>(10);
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.rollBackActions = new List<BuilderAction>(1028);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.droppedPieces = new List<BuilderPiece>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.droppedPieceData = new List<BuilderTable.DroppedPieceData>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.SetupMonkeBlocksRoom();
				this.gridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.checkGridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.nearbyPiecesResults = new NativeArray<ColliderHit>(1024, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.nearbyPiecesCommands = new NativeArray<OverlapSphereCommand>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.allPotentialPlacements = new List<BuilderPotentialPlacement>(1024);
			}
			else
			{
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(128);
				this.rollBackActions = new List<BuilderAction>(128);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(128);
			}
			this.SetupResources();
			if (!this.isTableMutable && this.linkedTerminal != null)
			{
				this.linkedTerminal.Init(this);
			}
		}

		// Token: 0x06005DDD RID: 24029 RVA: 0x00019E3F File Offset: 0x0001803F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005DDE RID: 24030 RVA: 0x00019E47 File Offset: 0x00018047
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06005DDF RID: 24031 RVA: 0x001DD243 File Offset: 0x001DB443
		public static bool TryGetBuilderTableForZone(GTZone zone, out BuilderTable table)
		{
			if (BuilderTable.zoneToInstance == null)
			{
				table = null;
				return false;
			}
			return BuilderTable.zoneToInstance.TryGetValue(zone, out table);
		}

		// Token: 0x06005DE0 RID: 24032 RVA: 0x001DD260 File Offset: 0x001DB460
		private void SetupMonkeBlocksRoom()
		{
			if (this.shelves == null)
			{
				this.shelves = new List<BuilderShelf>(64);
			}
			if (this.shelvesRoot != null)
			{
				this.shelvesRoot.GetComponentsInChildren<BuilderShelf>(this.shelves);
			}
			this.conveyors = new List<BuilderConveyor>(32);
			this.dispenserShelves = new List<BuilderDispenserShelf>(32);
			if (this.allShelvesRoot != null)
			{
				for (int i = 0; i < this.allShelvesRoot.Count; i++)
				{
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderConveyor>(BuilderTable.tempConveyors);
					this.conveyors.AddRange(BuilderTable.tempConveyors);
					BuilderTable.tempConveyors.Clear();
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderDispenserShelf>(BuilderTable.tempDispensers);
					this.dispenserShelves.AddRange(BuilderTable.tempDispensers);
					BuilderTable.tempDispensers.Clear();
				}
			}
			this.recyclers = new List<BuilderRecycler>(5);
			if (this.recyclerRoot != null)
			{
				for (int j = 0; j < this.recyclerRoot.Count; j++)
				{
					this.recyclerRoot[j].GetComponentsInChildren<BuilderRecycler>(BuilderTable.tempRecyclers);
					this.recyclers.AddRange(BuilderTable.tempRecyclers);
					BuilderTable.tempRecyclers.Clear();
				}
			}
			for (int k = 0; k < this.recyclers.Count; k++)
			{
				this.recyclers[k].recyclerID = k;
				this.recyclers[k].table = this;
			}
			this.dropZones = new List<BuilderDropZone>(6);
			this.dropZoneRoot.GetComponentsInChildren<BuilderDropZone>(this.dropZones);
			for (int l = 0; l < this.dropZones.Count; l++)
			{
				this.dropZones[l].dropZoneID = l;
				this.dropZones[l].table = this;
			}
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this;
			}
		}

		// Token: 0x06005DE1 RID: 24033 RVA: 0x001DD46C File Offset: 0x001DB66C
		private void SetupResources()
		{
			this.maxResources = new int[3];
			if (this.totalResources != null && this.totalResources.quantities != null)
			{
				for (int i = 0; i < this.totalResources.quantities.Count; i++)
				{
					if (this.totalResources.quantities[i].type >= BuilderResourceType.Basic && this.totalResources.quantities[i].type < BuilderResourceType.Count)
					{
						this.maxResources[(int)this.totalResources.quantities[i].type] += this.totalResources.quantities[i].count;
					}
				}
			}
			this.usedResources = new int[3];
			this.reservedResources = new int[3];
			if (this.totalReservedResources != null && this.totalReservedResources.quantities != null)
			{
				for (int j = 0; j < this.totalReservedResources.quantities.Count; j++)
				{
					if (this.totalReservedResources.quantities[j].type >= BuilderResourceType.Basic && this.totalReservedResources.quantities[j].type < BuilderResourceType.Count)
					{
						this.reservedResources[(int)this.totalReservedResources.quantities[j].type] += this.totalReservedResources.quantities[j].count;
					}
				}
			}
			this.plotMaxResources = new int[3];
			if (this.resourcesPerPrivatePlot != null && this.resourcesPerPrivatePlot.quantities != null)
			{
				for (int k = 0; k < this.resourcesPerPrivatePlot.quantities.Count; k++)
				{
					if (this.resourcesPerPrivatePlot.quantities[k].type >= BuilderResourceType.Basic && this.resourcesPerPrivatePlot.quantities[k].type < BuilderResourceType.Count)
					{
						this.plotMaxResources[(int)this.resourcesPerPrivatePlot.quantities[k].type] += this.resourcesPerPrivatePlot.quantities[k].count;
					}
				}
			}
			this.OnAvailableResourcesChange();
		}

		// Token: 0x06005DE2 RID: 24034 RVA: 0x001DD6B4 File Offset: 0x001DB8B4
		private void Start()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom != this.inRoom)
			{
				this.SetInRoom(NetworkSystem.Instance.InRoom);
			}
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			this.HandleOnZoneChanged();
			this.RequestTableConfiguration();
			this.FetchSharedBlocksStartingMapConfig();
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdate));
		}

		// Token: 0x06005DE3 RID: 24035 RVA: 0x001DD743 File Offset: 0x001DB943
		private void OnApplicationQuit()
		{
			this.ClearTable();
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x06005DE4 RID: 24036 RVA: 0x001DD754 File Offset: 0x001DB954
		private void OnDestroy()
		{
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdate));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (this.isTableMutable)
			{
				if (this.gridPlaneData.IsCreated)
				{
					this.gridPlaneData.Dispose();
				}
				if (this.checkGridPlaneData.IsCreated)
				{
					this.checkGridPlaneData.Dispose();
				}
				if (this.nearbyPiecesResults.IsCreated)
				{
					this.nearbyPiecesResults.Dispose();
				}
				if (this.nearbyPiecesCommands.IsCreated)
				{
					this.nearbyPiecesCommands.Dispose();
				}
			}
			this.DestroyData();
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x001DD810 File Offset: 0x001DBA10
		private void HandleOnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.tableZone);
			this.SetInBuilderZone(flag);
		}

		// Token: 0x06005DE6 RID: 24038 RVA: 0x001DD838 File Offset: 0x001DBA38
		public void InitIfNeeded()
		{
			if (!this.isSetup)
			{
				if (BuilderSetManager.instance == null)
				{
					return;
				}
				BuilderSetManager.instance.InitPieceDictionary();
				this.builderRenderer.BuildRenderer(BuilderSetManager.pieceList);
				this.baseGridPlanes.Clear();
				this.basePieces = new List<BuilderPiece>(1024);
				for (int i = 0; i < this.builtInPieceRoots.Count; i++)
				{
					this.builtInPieceRoots[i].SetActive(true);
					this.builtInPieceRoots[i].GetComponentsInChildren<BuilderPiece>(false, BuilderTable.tempPieces);
					this.basePieces.AddRange(BuilderTable.tempPieces);
				}
				this.allPrivatePlots = new List<BuilderPiecePrivatePlot>(20);
				this.CreateData();
				for (int j = 0; j < this.basePieces.Count; j++)
				{
					BuilderPiece builderPiece = this.basePieces[j];
					builderPiece.SetTable(this);
					builderPiece.pieceId = 5 + j;
					builderPiece.SetScale(this.pieceScale);
					builderPiece.SetupPiece(this.gridSize);
					builderPiece.OnCreate();
					builderPiece.SetState(BuilderPiece.State.OnShelf, true);
					this.baseGridPlanes.AddRange(builderPiece.gridPlanes);
					BuilderPiecePrivatePlot item;
					if (builderPiece.IsPrivatePlot() && builderPiece.TryGetPlotComponent(out item))
					{
						this.allPrivatePlots.Add(item);
					}
					this.AddPieceData(builderPiece);
				}
				this.builderPool = BuilderPool.instance;
				this.builderPool.Setup();
				base.StartCoroutine(this.builderPool.BuildFromPieceSets());
				if (this.isTableMutable)
				{
					for (int k = 0; k < this.conveyors.Count; k++)
					{
						this.conveyors[k].table = this;
						this.conveyors[k].shelfID = k;
						this.conveyors[k].Setup();
					}
					for (int l = 0; l < this.dispenserShelves.Count; l++)
					{
						this.dispenserShelves[l].table = this;
						this.dispenserShelves[l].shelfID = l;
						this.dispenserShelves[l].Setup();
					}
					this.conveyorManager.Setup(this);
					this.repelledPieceRoots = new HashSet<int>[this.repelHistoryLength];
					for (int m = 0; m < this.repelHistoryLength; m++)
					{
						this.repelledPieceRoots[m] = new HashSet<int>(10);
					}
					this.sharedBuildAreas = this.sharedBuildArea.GetComponents<BoxCollider>();
					BoxCollider[] array = this.sharedBuildAreas;
					for (int n = 0; n < array.Length; n++)
					{
						array[n].enabled = false;
					}
					this.sharedBuildArea.SetActive(false);
				}
				BoxCollider[] components = this.noBlocksArea.GetComponents<BoxCollider>();
				this.noBlocksAreas = new List<BuilderTable.BoxCheckParams>(components.Length);
				foreach (BoxCollider boxCollider in components)
				{
					boxCollider.enabled = true;
					BuilderTable.BoxCheckParams item2 = new BuilderTable.BoxCheckParams
					{
						center = boxCollider.transform.TransformPoint(boxCollider.center),
						halfExtents = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size) / 2f,
						rotation = boxCollider.transform.rotation
					};
					this.noBlocksAreas.Add(item2);
					boxCollider.enabled = false;
				}
				this.noBlocksArea.SetActive(false);
				this.isSetup = true;
			}
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x001DDBBA File Offset: 0x001DBDBA
		private void SetIsDirty(bool dirty)
		{
			if (this.isDirty != dirty)
			{
				UnityEvent<bool> onSaveDirtyChanged = this.OnSaveDirtyChanged;
				if (onSaveDirtyChanged != null)
				{
					onSaveDirtyChanged.Invoke(dirty);
				}
			}
			this.isDirty = dirty;
		}

		// Token: 0x06005DE8 RID: 24040 RVA: 0x001DDBE0 File Offset: 0x001DBDE0
		private void FixedUpdate()
		{
			if (this.tableState != BuilderTable.TableState.Ready && this.tableState != BuilderTable.TableState.WaitForMasterResync)
			{
				return;
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegisterFixed)
			{
				if (builderPieceFunctional != null)
				{
					this.fixedUpdateFunctionalComponents.Add(builderPieceFunctional);
				}
			}
			foreach (IBuilderPieceFunctional item in this.funcComponentsToUnregisterFixed)
			{
				this.fixedUpdateFunctionalComponents.Remove(item);
			}
			this.funcComponentsToRegisterFixed.Clear();
			this.funcComponentsToUnregisterFixed.Clear();
			foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.fixedUpdateFunctionalComponents)
			{
				builderPieceFunctional2.FunctionalPieceFixedUpdate();
			}
		}

		// Token: 0x06005DE9 RID: 24041 RVA: 0x001DDCEC File Offset: 0x001DBEEC
		public void Tick()
		{
			this.RunUpdate();
		}

		// Token: 0x06005DEA RID: 24042 RVA: 0x001DDCF4 File Offset: 0x001DBEF4
		private void RunUpdate()
		{
			this.InitIfNeeded();
			this.UpdateTableState();
			if (this.isTableMutable)
			{
				this.UpdateDroppedPieces(Time.deltaTime);
				this.repelHistoryIndex = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				int num = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				this.repelledPieceRoots[num].Clear();
			}
		}

		// Token: 0x06005DEB RID: 24043 RVA: 0x001DDD52 File Offset: 0x001DBF52
		public void AddQueuedCommand(BuilderTable.BuilderCommand cmd)
		{
			this.queuedBuildCommands.Add(cmd);
		}

		// Token: 0x06005DEC RID: 24044 RVA: 0x001DDD60 File Offset: 0x001DBF60
		public void ClearQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				this.queuedBuildCommands.Clear();
			}
			this.RemoveRollBackActions();
			if (this.rollBackBufferedCommands != null)
			{
				this.rollBackBufferedCommands.Clear();
			}
			this.RemoveRollForwardCommands();
		}

		// Token: 0x06005DED RID: 24045 RVA: 0x001DDD94 File Offset: 0x001DBF94
		public int GetNumQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				return this.queuedBuildCommands.Count;
			}
			return 0;
		}

		// Token: 0x06005DEE RID: 24046 RVA: 0x001DDDAB File Offset: 0x001DBFAB
		public void AddRollbackAction(BuilderAction action)
		{
			this.rollBackActions.Add(action);
		}

		// Token: 0x06005DEF RID: 24047 RVA: 0x001DDDB9 File Offset: 0x001DBFB9
		public void RemoveRollBackActions()
		{
			this.rollBackActions.Clear();
		}

		// Token: 0x06005DF0 RID: 24048 RVA: 0x001DDDC8 File Offset: 0x001DBFC8
		public void RemoveRollBackActions(int localCommandId)
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollBackActions[i].localCommandId == localCommandId)
				{
					this.rollBackActions.RemoveAt(i);
				}
			}
		}

		// Token: 0x06005DF1 RID: 24049 RVA: 0x001DDE14 File Offset: 0x001DC014
		public bool HasRollBackActionsForCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollBackActions.Count; i++)
			{
				if (this.rollBackActions[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005DF2 RID: 24050 RVA: 0x001DDE4E File Offset: 0x001DC04E
		public void AddRollForwardCommand(BuilderTable.BuilderCommand command)
		{
			this.rollForwardCommands.Add(command);
		}

		// Token: 0x06005DF3 RID: 24051 RVA: 0x001DDE5C File Offset: 0x001DC05C
		public void RemoveRollForwardCommands()
		{
			this.rollForwardCommands.Clear();
		}

		// Token: 0x06005DF4 RID: 24052 RVA: 0x001DDE6C File Offset: 0x001DC06C
		public void RemoveRollForwardCommands(int localCommandId)
		{
			for (int i = this.rollForwardCommands.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					this.rollForwardCommands.RemoveAt(i);
				}
			}
		}

		// Token: 0x06005DF5 RID: 24053 RVA: 0x001DDEB8 File Offset: 0x001DC0B8
		public bool HasRollForwardCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				if (this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005DF6 RID: 24054 RVA: 0x001DDEF4 File Offset: 0x001DC0F4
		public bool ShouldRollbackBufferCommand(BuilderTable.BuilderCommand cmd)
		{
			return cmd.type != BuilderTable.BuilderCommandType.Create && cmd.type != BuilderTable.BuilderCommandType.CreateArmShelf && this.rollBackActions.Count > 0 && (cmd.player == null || !cmd.player.IsLocal || !this.HasRollForwardCommand(cmd.localCommandId));
		}

		// Token: 0x06005DF7 RID: 24055 RVA: 0x001DDF4B File Offset: 0x001DC14B
		public void AddRollbackBufferedCommand(BuilderTable.BuilderCommand bufferedCmd)
		{
			this.rollBackBufferedCommands.Add(bufferedCmd);
		}

		// Token: 0x06005DF8 RID: 24056 RVA: 0x001DDF5C File Offset: 0x001DC15C
		private void ExecuteRollBackActions()
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				this.ExecuteAction(this.rollBackActions[i]);
			}
			this.rollBackActions.Clear();
		}

		// Token: 0x06005DF9 RID: 24057 RVA: 0x001DDFA0 File Offset: 0x001DC1A0
		private void ExecuteRollbackBufferedCommands()
		{
			for (int i = 0; i < this.rollBackBufferedCommands.Count; i++)
			{
				BuilderTable.BuilderCommand cmd = this.rollBackBufferedCommands[i];
				cmd.isQueued = false;
				cmd.canRollback = false;
				this.ExecuteBuildCommand(cmd);
			}
			this.rollBackBufferedCommands.Clear();
		}

		// Token: 0x06005DFA RID: 24058 RVA: 0x001DDFF4 File Offset: 0x001DC1F4
		private void ExecuteRollForwardCommands()
		{
			BuilderTable.tempRollForwardCommands.Clear();
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.tempRollForwardCommands.Add(this.rollForwardCommands[i]);
			}
			this.rollForwardCommands.Clear();
			for (int j = 0; j < BuilderTable.tempRollForwardCommands.Count; j++)
			{
				BuilderTable.BuilderCommand cmd = BuilderTable.tempRollForwardCommands[j];
				cmd.isQueued = true;
				cmd.canRollback = true;
				this.ExecuteBuildCommand(cmd);
			}
			BuilderTable.tempRollForwardCommands.Clear();
		}

		// Token: 0x06005DFB RID: 24059 RVA: 0x001DE084 File Offset: 0x001DC284
		private void UpdateRollForwardCommandData()
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollForwardCommands[i];
				if (builderCommand.type == BuilderTable.BuilderCommandType.Drop)
				{
					BuilderPiece piece = this.GetPiece(builderCommand.pieceId);
					if (piece != null && piece.rigidBody != null)
					{
						builderCommand.localPosition = piece.rigidBody.position;
						builderCommand.localRotation = piece.rigidBody.rotation;
						builderCommand.velocity = piece.rigidBody.linearVelocity;
						builderCommand.angVelocity = piece.rigidBody.angularVelocity;
						this.rollForwardCommands[i] = builderCommand;
					}
				}
			}
		}

		// Token: 0x06005DFC RID: 24060 RVA: 0x001DE13C File Offset: 0x001DC33C
		public bool TryRollbackAndReExecute(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				if (this.rollBackBufferedCommands.Count > 0)
				{
					this.UpdateRollForwardCommandData();
					this.ExecuteRollBackActions();
					this.ExecuteRollbackBufferedCommands();
					this.ExecuteRollForwardCommands();
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				else
				{
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005DFD RID: 24061 RVA: 0x001DE19B File Offset: 0x001DC39B
		public void RollbackFailedCommand(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				this.UpdateRollForwardCommandData();
				this.ExecuteRollBackActions();
				this.ExecuteRollbackBufferedCommands();
				this.RemoveRollForwardCommands(-1);
				this.ExecuteRollForwardCommands();
			}
		}

		// Token: 0x06005DFE RID: 24062 RVA: 0x001DE1C5 File Offset: 0x001DC3C5
		public BuilderTable.TableState GetTableState()
		{
			return this.tableState;
		}

		// Token: 0x06005DFF RID: 24063 RVA: 0x001DE1D0 File Offset: 0x001DC3D0
		public void SetTableState(BuilderTable.TableState newState)
		{
			this.InitIfNeeded();
			if (newState == this.tableState)
			{
				return;
			}
			BuilderTable.TableState tableState = this.tableState;
			this.tableState = newState;
			switch (this.tableState)
			{
			case BuilderTable.TableState.WaitingForInitalBuild:
				if (!this.isTableMutable && !NetworkSystem.Instance.IsMasterClient)
				{
					this.sharedBlocksMap = null;
					UnityEvent onMapCleared = this.OnMapCleared;
					if (onMapCleared == null)
					{
						return;
					}
					onMapCleared.Invoke();
					return;
				}
				break;
			case BuilderTable.TableState.ReceivingInitialBuild:
			case BuilderTable.TableState.ReceivingMasterResync:
			case BuilderTable.TableState.InitialBuild:
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.WaitForInitialBuildMaster:
				this.nextPieceId = 10000;
				if (this.isTableMutable)
				{
					this.BuildInitialTableForPlayer();
					return;
				}
				this.BuildSelectedSharedMap();
				return;
			case BuilderTable.TableState.WaitForMasterResync:
				this.ClearQueuedCommands();
				this.ResetConveyors();
				return;
			case BuilderTable.TableState.Ready:
				this.OnAvailableResourcesChange();
				if (!this.isTableMutable)
				{
					string arg = (this.sharedBlocksMap == null) ? "" : this.sharedBlocksMap.MapID;
					UnityEvent<string> onMapLoaded = this.OnMapLoaded;
					if (onMapLoaded != null)
					{
						onMapLoaded.Invoke(arg);
					}
					this.SetPendingMap(null);
					return;
				}
				break;
			case BuilderTable.TableState.BadData:
				this.ClearTable();
				this.ClearQueuedCommands();
				break;
			case BuilderTable.TableState.WaitingForSharedMapLoad:
				this.ClearTable();
				this.ClearQueuedCommands();
				this.builderNetworking.ResetSerializedTableForAllPlayers();
				return;
			default:
				return;
			}
		}

		// Token: 0x06005E00 RID: 24064 RVA: 0x001DE2FC File Offset: 0x001DC4FC
		public void SetPendingMap(string mapID)
		{
			this.pendingMapID = mapID;
		}

		// Token: 0x06005E01 RID: 24065 RVA: 0x001DE305 File Offset: 0x001DC505
		public string GetPendingMap()
		{
			return this.pendingMapID;
		}

		// Token: 0x06005E02 RID: 24066 RVA: 0x001DE30D File Offset: 0x001DC50D
		public string GetCurrentMapID()
		{
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap = this.sharedBlocksMap;
			if (sharedBlocksMap == null)
			{
				return null;
			}
			return sharedBlocksMap.MapID;
		}

		// Token: 0x06005E03 RID: 24067 RVA: 0x001DE320 File Offset: 0x001DC520
		public void LoadSharedMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (map.MapID.IsNullOrEmpty())
				{
					GTDev.LogWarning<string>("Invalid map to load", null);
					UnityEvent<string> onMapLoadFailed = this.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("Invalid Map ID");
					return;
				}
				else
				{
					if (this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.BadData)
					{
						this.builderNetworking.RequestLoadSharedBlocksMap(map.MapID);
						return;
					}
					UnityEvent<string> onMapLoadFailed2 = this.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("WAIT FOR LOAD IN PROGRESS");
					return;
				}
			}
			else
			{
				UnityEvent<string> onMapLoadFailed3 = this.OnMapLoadFailed;
				if (onMapLoadFailed3 == null)
				{
					return;
				}
				onMapLoadFailed3.Invoke("Not In Room");
				return;
			}
		}

		// Token: 0x06005E04 RID: 24068 RVA: 0x001DE3B8 File Offset: 0x001DC5B8
		public void SetInRoom(bool inRoom)
		{
			this.inRoom = inRoom;
			bool flag = inRoom && this.inBuilderZone;
			if (!inRoom)
			{
				this.pendingMapID = null;
				this.sharedBlocksMap = null;
				UnityEvent onMapCleared = this.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
			}
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient && this.isTableMutable)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient() && this.isTableMutable)
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x06005E05 RID: 24069 RVA: 0x001DE48C File Offset: 0x001DC68C
		public static bool IsLocalPlayerInBuilderZone()
		{
			GorillaTagger instance = GorillaTagger.Instance;
			ZoneEntityBSP zoneEntityBSP;
			if (instance == null)
			{
				zoneEntityBSP = null;
			}
			else
			{
				VRRig offlineVRRig = instance.offlineVRRig;
				zoneEntityBSP = ((offlineVRRig != null) ? offlineVRRig.zoneEntity : null);
			}
			ZoneEntityBSP zoneEntityBSP2 = zoneEntityBSP;
			BuilderTable builderTable;
			return !(zoneEntityBSP2 == null) && BuilderTable.TryGetBuilderTableForZone(zoneEntityBSP2.currentZone, out builderTable) && builderTable.IsInBuilderZone();
		}

		// Token: 0x06005E06 RID: 24070 RVA: 0x001DE4D9 File Offset: 0x001DC6D9
		public bool IsInBuilderZone()
		{
			return this.inBuilderZone;
		}

		// Token: 0x06005E07 RID: 24071 RVA: 0x001DE4E4 File Offset: 0x001DC6E4
		public void SetInBuilderZone(bool inBuilderZone)
		{
			this.inBuilderZone = inBuilderZone;
			this.ShowPieces(inBuilderZone);
			bool flag = this.inRoom && inBuilderZone;
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient())
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x06005E08 RID: 24072 RVA: 0x001DE588 File Offset: 0x001DC788
		private void ShowPieces(bool show)
		{
			if (this.builderRenderer != null)
			{
				this.builderRenderer.Show(show);
			}
			if (this.pieces == null || this.basePieces == null)
			{
				return;
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				this.pieces[i].SetDirectRenderersVisible(show);
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				this.basePieces[j].SetDirectRenderersVisible(show);
			}
		}

		// Token: 0x06005E09 RID: 24073 RVA: 0x001DE610 File Offset: 0x001DC810
		private void UpdateTableState()
		{
			switch (this.tableState)
			{
			case BuilderTable.TableState.InitialBuild:
			{
				BuilderTableNetworking.PlayerTableInitState localTableInit = this.builderNetworking.GetLocalTableInit();
				try
				{
					this.ClearTable();
					this.ClearQueuedCommands();
					byte[] array = GZipStream.UncompressBuffer(localTableInit.serializedTableState);
					localTableInit.totalSerializedBytes = array.Length;
					Array.Copy(array, 0, localTableInit.serializedTableState, 0, localTableInit.totalSerializedBytes);
					this.DeserializeTableState(localTableInit.serializedTableState, localTableInit.numSerializedBytes);
					if (this.tableState == BuilderTable.TableState.BadData)
					{
						return;
					}
					this.SetTableState(BuilderTable.TableState.ExecuteQueuedCommands);
					this.SetIsDirty(true);
					return;
				}
				catch (Exception)
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				break;
			}
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.Ready:
			{
				JobHandle jobHandle = default(JobHandle);
				if (this.isTableMutable)
				{
					this.conveyorManager.UpdateManager();
					jobHandle = this.conveyorManager.ConstructJobHandle();
					JobHandle.ScheduleBatchedJobs();
					foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
					{
						builderDispenserShelf.UpdateShelf();
					}
					foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
					{
						builderPiecePrivatePlot.UpdatePlot();
					}
					foreach (BuilderRecycler builderRecycler in this.recyclers)
					{
						builderRecycler.UpdateRecycler();
					}
					for (int i = this.shelfSliceUpdateIndex; i < this.dispenserShelves.Count; i += BuilderTable.SHELF_SLICE_BUCKETS)
					{
						this.dispenserShelves[i].UpdateShelfSliced();
					}
					this.shelfSliceUpdateIndex = (this.shelfSliceUpdateIndex + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegister)
				{
					if (builderPieceFunctional != null)
					{
						this.activeFunctionalComponents.Add(builderPieceFunctional);
					}
				}
				foreach (IBuilderPieceFunctional item in this.funcComponentsToUnregister)
				{
					this.activeFunctionalComponents.Remove(item);
				}
				this.funcComponentsToRegister.Clear();
				this.funcComponentsToUnregister.Clear();
				foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.activeFunctionalComponents)
				{
					if (builderPieceFunctional2 != null)
					{
						builderPieceFunctional2.FunctionalPieceUpdate();
					}
				}
				if (this.isTableMutable)
				{
					foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
					{
						builderResourceMeter.UpdateMeterFill();
					}
					this.CleanUpDroppedPiece();
					jobHandle.Complete();
					return;
				}
				return;
			}
			default:
				return;
			}
			for (int j = 0; j < this.queuedBuildCommands.Count; j++)
			{
				BuilderTable.BuilderCommand cmd = this.queuedBuildCommands[j];
				cmd.isQueued = true;
				this.ExecuteBuildCommand(cmd);
			}
			this.queuedBuildCommands.Clear();
			this.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x06005E0A RID: 24074 RVA: 0x001DE99C File Offset: 0x001DCB9C
		private void RouteNewCommand(BuilderTable.BuilderCommand cmd, bool force)
		{
			bool flag = this.ShouldExecuteCommand();
			if (force)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (flag && this.ShouldRollbackBufferCommand(cmd))
			{
				this.AddRollbackBufferedCommand(cmd);
				return;
			}
			if (flag)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (this.ShouldQueueCommand())
			{
				this.AddQueuedCommand(cmd);
				return;
			}
			this.ShouldDiscardCommand();
		}

		// Token: 0x06005E0B RID: 24075 RVA: 0x001DE9F4 File Offset: 0x001DCBF4
		private void ExecuteBuildCommand(BuilderTable.BuilderCommand cmd)
		{
			if (!this.isTableMutable && cmd.type != BuilderTable.BuilderCommandType.FunctionalStateChange)
			{
				return;
			}
			switch (cmd.type)
			{
			case BuilderTable.BuilderCommandType.Create:
				this.ExecutePieceCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.Place:
				this.ExecutePiecePlacedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Grab:
				this.ExecutePieceGrabbedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Drop:
				this.ExecutePieceDroppedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Remove:
				break;
			case BuilderTable.BuilderCommandType.Paint:
				this.ExecutePiecePainted(cmd);
				return;
			case BuilderTable.BuilderCommandType.Recycle:
				this.ExecutePieceRecycled(cmd);
				return;
			case BuilderTable.BuilderCommandType.ClaimPlot:
				this.ExecuteClaimPlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.FreePlot:
				this.ExecuteFreePlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.CreateArmShelf:
				this.ExecuteArmShelfCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.PlayerLeftRoom:
				this.ExecutePlayerLeftRoom(cmd);
				return;
			case BuilderTable.BuilderCommandType.FunctionalStateChange:
				this.ExecuteSetFunctionalPieceState(cmd);
				return;
			case BuilderTable.BuilderCommandType.SetSelection:
				this.ExecuteSetSelection(cmd);
				return;
			case BuilderTable.BuilderCommandType.Repel:
				this.ExecutePieceRepelled(cmd);
				break;
			default:
				return;
			}
		}

		// Token: 0x06005E0C RID: 24076 RVA: 0x001DEAC1 File Offset: 0x001DCCC1
		public void ClearTable()
		{
			this.ClearTableInternal();
		}

		// Token: 0x06005E0D RID: 24077 RVA: 0x001DEACC File Offset: 0x001DCCCC
		private void ClearTableInternal()
		{
			BuilderTable.tempDeletePieces.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				BuilderTable.tempDeletePieces.Add(this.pieces[i]);
			}
			if (this.isTableMutable)
			{
				this.droppedPieces.Clear();
				this.droppedPieceData.Clear();
			}
			for (int j = 0; j < BuilderTable.tempDeletePieces.Count; j++)
			{
				BuilderTable.tempDeletePieces[j].ClearParentPiece(false);
				BuilderTable.tempDeletePieces[j].ClearParentHeld();
				BuilderTable.tempDeletePieces[j].SetState(BuilderPiece.State.None, false);
				this.RemovePiece(BuilderTable.tempDeletePieces[j]);
			}
			for (int k = 0; k < BuilderTable.tempDeletePieces.Count; k++)
			{
				this.builderPool.DestroyPiece(BuilderTable.tempDeletePieces[k]);
			}
			BuilderTable.tempDeletePieces.Clear();
			this.pieces.Clear();
			this.pieceIDToIndexCache.Clear();
			this.nextPieceId = 10000;
			if (this.isTableMutable)
			{
				this.conveyorManager.OnClearTable();
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					builderDispenserShelf.OnClearTable();
				}
				for (int l = 0; l < this.repelHistoryLength; l++)
				{
					this.repelledPieceRoots[l].Clear();
				}
			}
			this.funcComponentsToRegister.Clear();
			this.funcComponentsToUnregister.Clear();
			this.activeFunctionalComponents.Clear();
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				foreach (BuilderAttachGridPlane builderAttachGridPlane in builderPiece.gridPlanes)
				{
					builderAttachGridPlane.OnReturnToPool(this.builderPool);
				}
			}
			if (this.isTableMutable)
			{
				this.ClearBuiltInPlots();
				this.playerToArmShelfLeft.Clear();
				this.playerToArmShelfRight.Clear();
				if (BuilderPieceInteractor.instance != null)
				{
					BuilderPieceInteractor.instance.RemovePiecesFromHands();
				}
			}
		}

		// Token: 0x06005E0E RID: 24078 RVA: 0x001DED38 File Offset: 0x001DCF38
		private void ClearBuiltInPlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.ClearPlot();
			}
			this.plotOwners.Clear();
			this.SetLocalPlayerOwnsPlot(false);
		}

		// Token: 0x06005E0F RID: 24079 RVA: 0x001DED9C File Offset: 0x001DCF9C
		private void OnDeserializeUpdatePlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.RecountPlotCost();
			}
		}

		// Token: 0x06005E10 RID: 24080 RVA: 0x001DEDEC File Offset: 0x001DCFEC
		public void BuildPiecesOnShelves()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (this.shelves == null)
			{
				return;
			}
			for (int i = 0; i < this.shelves.Count; i++)
			{
				if (this.shelves[i] != null)
				{
					this.shelves[i].Init();
				}
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int j = 0; j < this.shelves.Count; j++)
				{
					if (this.shelves[j].HasOpenSlot())
					{
						this.shelves[j].BuildNextPiece(this);
						if (this.shelves[j].HasOpenSlot())
						{
							flag = true;
						}
					}
				}
			}
		}

		// Token: 0x06005E11 RID: 24081 RVA: 0x001DEE9F File Offset: 0x001DD09F
		private void OnFinishedInitialTableBuild()
		{
			this.BuildPiecesOnShelves();
			this.SetTableState(BuilderTable.TableState.Ready);
			this.CreateArmShelvesForPlayersInBuilder();
		}

		// Token: 0x06005E12 RID: 24082 RVA: 0x001DEEB4 File Offset: 0x001DD0B4
		public int CreatePieceId()
		{
			int result = this.nextPieceId;
			if (this.nextPieceId == 2147483647)
			{
				this.nextPieceId = 20000;
			}
			this.nextPieceId++;
			return result;
		}

		// Token: 0x06005E13 RID: 24083 RVA: 0x001DEEE4 File Offset: 0x001DD0E4
		public void ResetConveyors()
		{
			if (this.isTableMutable)
			{
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					builderConveyor.ResetConveyorState();
				}
			}
		}

		// Token: 0x06005E14 RID: 24084 RVA: 0x001DEF3C File Offset: 0x001DD13C
		public void RequestCreateConveyorPiece(int newPieceType, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			BuilderConveyor builderConveyor = this.conveyors[shelfID];
			if (builderConveyor == null)
			{
				return;
			}
			Transform spawnTransform = builderConveyor.GetSpawnTransform();
			this.builderNetworking.CreateShelfPiece(newPieceType, spawnTransform.position, spawnTransform.rotation, materialType, BuilderPiece.State.OnConveyor, shelfID);
		}

		// Token: 0x06005E15 RID: 24085 RVA: 0x001DEF95 File Offset: 0x001DD195
		public void RequestCreateDispenserShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			this.builderNetworking.CreateShelfPiece(pieceType, position, rotation, materialType, BuilderPiece.State.OnShelf, shelfID);
		}

		// Token: 0x06005E16 RID: 24086 RVA: 0x001DEFD8 File Offset: 0x001DD1D8
		public void CreateConveyorPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID, int sendTimestamp)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			if (this.conveyors[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnConveyor,
				parentPieceId = shelfID,
				parentAttachIndex = sendTimestamp,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E17 RID: 24087 RVA: 0x001DF080 File Offset: 0x001DD280
		public void CreateDispenserShelfPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnShelf,
				parentPieceId = shelfID,
				isLeft = true,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E18 RID: 24088 RVA: 0x001DF126 File Offset: 0x001DD326
		public void RequestShelfSelection(int shelfId, int groupID, bool isConveyor)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestShelfSelection(shelfId, groupID, isConveyor);
		}

		// Token: 0x06005E19 RID: 24089 RVA: 0x001DF140 File Offset: 0x001DD340
		public void VerifySetSelections()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.conveyors)
			{
				builderConveyor.VerifySetSelection();
			}
			foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
			{
				builderDispenserShelf.VerifySetSelection();
			}
		}

		// Token: 0x06005E1A RID: 24090 RVA: 0x001DF1D8 File Offset: 0x001DD3D8
		public bool ValidateShelfSelectionParams(int shelfId, int displayGroupID, bool isConveyor, Player player)
		{
			bool flag = shelfId >= 0 && ((isConveyor && shelfId < this.conveyors.Count) || (!isConveyor && shelfId < this.dispenserShelves.Count)) && BuilderSetManager.instance.DoesPlayerOwnDisplayGroup(player, displayGroupID);
			if (PhotonNetwork.IsMasterClient)
			{
				if (isConveyor)
				{
					BuilderConveyor builderConveyor = this.conveyors[shelfId];
					bool flag2 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderConveyor.transform.position, false, true, 4f);
					flag = (flag && flag2);
				}
				else
				{
					BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[shelfId];
					bool flag3 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderDispenserShelf.transform.position, false, true, 4f);
					flag = (flag && flag3);
				}
			}
			return flag;
		}

		// Token: 0x06005E1B RID: 24091 RVA: 0x001DF294 File Offset: 0x001DD494
		private void SetConveyorSelection(int conveyorId, int setId)
		{
			BuilderConveyor builderConveyor = this.conveyors[conveyorId];
			if (builderConveyor == null)
			{
				return;
			}
			builderConveyor.SetSelection(setId);
		}

		// Token: 0x06005E1C RID: 24092 RVA: 0x001DF2C0 File Offset: 0x001DD4C0
		private void SetDispenserSelection(int conveyorId, int setId)
		{
			BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[conveyorId];
			if (builderDispenserShelf == null)
			{
				return;
			}
			builderDispenserShelf.SetSelection(setId);
		}

		// Token: 0x06005E1D RID: 24093 RVA: 0x001DF2EC File Offset: 0x001DD4EC
		public void ChangeSetSelection(int shelfID, int setID, bool isConveyor)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.SetSelection,
				parentPieceId = shelfID,
				pieceType = setID,
				isLeft = isConveyor,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E1E RID: 24094 RVA: 0x001DF340 File Offset: 0x001DD540
		public void ExecuteSetSelection(BuilderTable.BuilderCommand cmd)
		{
			bool isLeft = cmd.isLeft;
			int parentPieceId = cmd.parentPieceId;
			int pieceType = cmd.pieceType;
			if (isLeft)
			{
				this.SetConveyorSelection(parentPieceId, pieceType);
				return;
			}
			this.SetDispenserSelection(parentPieceId, pieceType);
		}

		// Token: 0x06005E1F RID: 24095 RVA: 0x001DF374 File Offset: 0x001DD574
		public bool ValidateFunctionalPieceState(int pieceID, byte state, NetPlayer player)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			return !(piece == null) && piece.functionalPieceComponent != null && (!NetworkSystem.Instance.IsMasterClient || player.IsMasterClient || this.IsPlayerHandNearAction(player, piece.transform.position, true, false, piece.functionalPieceComponent.GetInteractionDistace())) && piece.functionalPieceComponent.IsStateValid(state);
		}

		// Token: 0x06005E20 RID: 24096 RVA: 0x001DF3E4 File Offset: 0x001DD5E4
		public void OnFunctionalStateRequest(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			if (piece.functionalPieceComponent == null)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			piece.functionalPieceComponent.OnStateRequest(state, player, timeStamp);
		}

		// Token: 0x06005E21 RID: 24097 RVA: 0x001DF420 File Offset: 0x001DD620
		public void SetFunctionalPieceState(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FunctionalStateChange,
				pieceId = pieceID,
				twist = state,
				player = player,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E22 RID: 24098 RVA: 0x001DF46C File Offset: 0x001DD66C
		public void ExecuteSetFunctionalPieceState(BuilderTable.BuilderCommand cmd)
		{
			BuilderPiece piece = this.GetPiece(cmd.pieceId);
			if (piece == null)
			{
				return;
			}
			piece.SetFunctionalPieceState(cmd.twist, cmd.player, cmd.serverTimeStamp);
		}

		// Token: 0x06005E23 RID: 24099 RVA: 0x001DF4A8 File Offset: 0x001DD6A8
		public void RegisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegister.Add(component);
			}
		}

		// Token: 0x06005E24 RID: 24100 RVA: 0x001DF4B9 File Offset: 0x001DD6B9
		public void UnregisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToUnregister.Add(component);
			}
		}

		// Token: 0x06005E25 RID: 24101 RVA: 0x001DF4CA File Offset: 0x001DD6CA
		public void RegisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Add(component);
			}
		}

		// Token: 0x06005E26 RID: 24102 RVA: 0x001DF4DB File Offset: 0x001DD6DB
		public void UnregisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Remove(component);
			}
		}

		// Token: 0x06005E27 RID: 24103 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x06005E28 RID: 24104 RVA: 0x001DF4F0 File Offset: 0x001DD6F0
		public void CreatePiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = state,
				player = NetPlayer.Get(player)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E29 RID: 24105 RVA: 0x001DF558 File Offset: 0x001DD758
		public void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, playFX, recyclerID);
		}

		// Token: 0x06005E2A RID: 24106 RVA: 0x001DF584 File Offset: 0x001DD784
		public void RecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Recycle,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				player = NetPlayer.Get(player),
				isLeft = playFX,
				parentPieceId = recyclerID
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E2B RID: 24107 RVA: 0x001DF5E3 File Offset: 0x001DD7E3
		private bool ShouldExecuteCommand()
		{
			return this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster;
		}

		// Token: 0x06005E2C RID: 24108 RVA: 0x001DF5F9 File Offset: 0x001DD7F9
		private bool ShouldQueueCommand()
		{
			return this.tableState == BuilderTable.TableState.ReceivingInitialBuild || this.tableState == BuilderTable.TableState.ReceivingMasterResync || this.tableState == BuilderTable.TableState.InitialBuild || this.tableState == BuilderTable.TableState.ExecuteQueuedCommands;
		}

		// Token: 0x06005E2D RID: 24109 RVA: 0x001DF621 File Offset: 0x001DD821
		private bool ShouldDiscardCommand()
		{
			return this.tableState == BuilderTable.TableState.WaitingForInitalBuild || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster || this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x06005E2E RID: 24110 RVA: 0x001DF640 File Offset: 0x001DD840
		public bool DoesChainContainPiece(BuilderPiece targetPiece, BuilderPiece firstInChain, BuilderPiece nextInChain)
		{
			return !(targetPiece == null) && !(firstInChain == null) && (targetPiece.Equals(firstInChain) || (!(nextInChain == null) && (targetPiece.Equals(nextInChain) || (!(firstInChain == nextInChain) && this.DoesChainContainPiece(targetPiece, firstInChain, nextInChain.parentPiece)))));
		}

		// Token: 0x06005E2F RID: 24111 RVA: 0x001DF69C File Offset: 0x001DD89C
		public bool DoesChainContainChain(BuilderPiece chainARoot, BuilderPiece chainBAttachPiece)
		{
			if (chainARoot == null || chainBAttachPiece == null)
			{
				return false;
			}
			if (this.DoesChainContainPiece(chainARoot, chainBAttachPiece, chainBAttachPiece.parentPiece))
			{
				return true;
			}
			BuilderPiece builderPiece = chainARoot.firstChildPiece;
			while (builderPiece != null)
			{
				if (this.DoesChainContainChain(builderPiece, chainBAttachPiece))
				{
					return true;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
			return false;
		}

		// Token: 0x06005E30 RID: 24112 RVA: 0x001DF6F8 File Offset: 0x001DD8F8
		private bool IsPlayerHandNearAction(NetPlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 2.5f)
		{
			bool flag = true;
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				if (isLeftHand || checkBothHands)
				{
					flag = ((worldPosition - rigContainer.Rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius);
				}
				if (!isLeftHand || checkBothHands)
				{
					float sqrMagnitude = (worldPosition - rigContainer.Rig.rightHandTransform.position).sqrMagnitude;
					flag = (flag && sqrMagnitude < acceptableRadius * acceptableRadius);
				}
			}
			return flag;
		}

		// Token: 0x06005E31 RID: 24113 RVA: 0x001DF78C File Offset: 0x001DD98C
		public bool ValidatePlacePieceParams(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return false;
			}
			if (piece.heldByPlayerActorNumber != placedByPlayer.ActorNumber)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece2.isBuiltIntoTable)
			{
				return false;
			}
			if (twist > 3)
			{
				return false;
			}
			BuilderPiece piece3 = this.GetPiece(parentPieceId);
			if (!(piece3 != null))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(placedByPlayer.ActorNumber, piece2, piece3))
			{
				return false;
			}
			if (this.DoesChainContainChain(piece2, piece3))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			if (piece3 != null && (parentAttachIndex < 0 || parentAttachIndex >= piece3.gridPlanes.Count))
			{
				return false;
			}
			if (piece3 != null)
			{
				bool flag = (long)(twist % 2) == 1L;
				BuilderAttachGridPlane builderAttachGridPlane = piece2.gridPlanes[attachIndex];
				int num = flag ? builderAttachGridPlane.length : builderAttachGridPlane.width;
				int num2 = flag ? builderAttachGridPlane.width : builderAttachGridPlane.length;
				BuilderAttachGridPlane builderAttachGridPlane2 = piece3.gridPlanes[parentAttachIndex];
				int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
				int num4 = num3 - (builderAttachGridPlane2.width - 1);
				if ((int)bumpOffsetX < num4 - num || (int)bumpOffsetX > num3 + num)
				{
					return false;
				}
				int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
				int num6 = num5 - (builderAttachGridPlane2.length - 1);
				if ((int)bumpOffsetZ < num6 - num2 || (int)bumpOffsetZ > num5 + num2)
				{
					return false;
				}
			}
			if (placedByPlayer == null)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient && piece3 != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				Vector3 vector2;
				Quaternion rotation;
				piece2.BumpTwistToPositionRotation(twist, bumpOffsetX, bumpOffsetZ, attachIndex, piece3.gridPlanes[parentAttachIndex], out vector, out quaternion, out vector2, out rotation);
				Vector3 point = piece2.transform.InverseTransformPoint(piece.transform.position);
				Vector3 worldPosition = vector2 + rotation * point;
				if (!this.IsPlayerHandNearAction(placedByPlayer, worldPosition, piece.heldInLeftHand, false, 2.5f))
				{
					return false;
				}
				if (!this.ValidatePieceWorldTransform(vector2, rotation))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005E32 RID: 24114 RVA: 0x001DF9A0 File Offset: 0x001DDBA0
		public bool ValidatePlacePieceState(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			return !(piece2 == null) && !(this.GetPiece(parentPieceId) == null) && placedByPlayer != null && !piece2.GetRootPiece() != piece;
		}

		// Token: 0x06005E33 RID: 24115 RVA: 0x001DFA04 File Offset: 0x001DDC04
		public void ExecutePieceCreated(BuilderTable.BuilderCommand cmd)
		{
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateCreatePieceParams(cmd.pieceType, cmd.pieceId, cmd.state, cmd.materialType))
			{
				return;
			}
			BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, cmd.localPosition, cmd.localRotation, cmd.state, cmd.materialType, 0, this);
			if (!(builderPiece != null) || cmd.state != BuilderPiece.State.OnConveyor)
			{
				if (builderPiece != null && cmd.isLeft && cmd.state == BuilderPiece.State.OnShelf)
				{
					if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.dispenserShelves.Count)
					{
						return;
					}
					builderPiece.shelfOwner = cmd.parentPieceId;
					this.dispenserShelves[builderPiece.shelfOwner].OnShelfPieceCreated(builderPiece, true);
				}
				return;
			}
			if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.conveyors.Count)
			{
				return;
			}
			builderPiece.shelfOwner = cmd.parentPieceId;
			BuilderConveyor builderConveyor = this.conveyors[builderPiece.shelfOwner];
			int parentAttachIndex = cmd.parentAttachIndex;
			float timeOffset = 0f;
			if (PhotonNetwork.ServerTimestamp > parentAttachIndex)
			{
				timeOffset = (PhotonNetwork.ServerTimestamp - parentAttachIndex) / 1000f;
			}
			builderConveyor.OnShelfPieceCreated(builderPiece, timeOffset);
		}

		// Token: 0x06005E34 RID: 24116 RVA: 0x001DFB4B File Offset: 0x001DDD4B
		public void ExecutePieceRecycled(BuilderTable.BuilderCommand cmd)
		{
			this.RecyclePieceInternal(cmd.pieceId, false, cmd.isLeft, cmd.parentPieceId);
		}

		// Token: 0x06005E35 RID: 24117 RVA: 0x001DFB66 File Offset: 0x001DDD66
		private bool ValidateCreatePieceParams(int newPieceType, int newPieceId, BuilderPiece.State state, int materialType)
		{
			return !(this.GetPiecePrefab(newPieceType) == null) && !(this.GetPiece(newPieceId) != null);
		}

		// Token: 0x06005E36 RID: 24118 RVA: 0x001DFB8C File Offset: 0x001DDD8C
		private bool ValidateDeserializedRootPieceState(int pieceId, BuilderPiece.State state, int shelfOwner, int heldByActor, Vector3 localPosition, Quaternion localRotation)
		{
			switch (state)
			{
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. held piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			case BuilderPiece.State.Dropped:
				if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. dropped piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				if (!this.isTableMutable || shelfOwner == -1)
				{
					if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
					{
						return false;
					}
				}
				else if (shelfOwner < 0 || shelfOwner > this.dispenserShelves.Count - 1)
				{
					return false;
				}
				break;
			case BuilderPiece.State.OnConveyor:
				if (shelfOwner == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. OnConveyor piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (shelfOwner < 0 || shelfOwner > this.conveyors.Count - 1)
				{
					return false;
				}
				break;
			case BuilderPiece.State.AttachedToArm:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. AttachedToArm piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		// Token: 0x06005E37 RID: 24119 RVA: 0x001DFCD8 File Offset: 0x001DDED8
		private bool ValidateDeserializedChildPieceState(int pieceId, BuilderPiece.State state)
		{
			switch (state)
			{
			case BuilderPiece.State.AttachedAndPlaced:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				return true;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
			case BuilderPiece.State.AttachedToArm:
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. Invalid state {0} of child piece {1} in Immutable table", state, pieceId), null);
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06005E38 RID: 24120 RVA: 0x001DFD3C File Offset: 0x001DDF3C
		public bool ValidatePieceWorldTransform(Vector3 position, Quaternion rotation)
		{
			float num = 10000f;
			return position.IsValid(num) && rotation.IsValid() && (this.roomCenter.position - position).sqrMagnitude <= this.acceptableSqrDistFromCenter && this.ValidatePositionInArea(position);
		}

		// Token: 0x06005E39 RID: 24121 RVA: 0x001DFD90 File Offset: 0x001DDF90
		public bool ValidatePositionInArea(Vector3 position)
		{
			using (List<SimpleAABB>.Enumerator enumerator = this.m_areaBounds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsInBounds(position))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005E3A RID: 24122 RVA: 0x001DFDEC File Offset: 0x001DDFEC
		private BuilderPiece CreatePieceInternal(int newPieceType, int newPieceId, Vector3 position, Quaternion rotation, BuilderPiece.State state, int materialType, int activateTimeStamp, BuilderTable table)
		{
			if (this.GetPiecePrefab(newPieceType) == null)
			{
				return null;
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				this.nextPieceId = newPieceId + 1;
			}
			BuilderPiece builderPiece = this.builderPool.CreatePiece(newPieceType, false);
			builderPiece.SetScale(table.pieceScale);
			builderPiece.transform.SetPositionAndRotation(position, rotation);
			builderPiece.pieceType = newPieceType;
			builderPiece.pieceId = newPieceId;
			builderPiece.SetTable(table);
			builderPiece.gameObject.SetActive(true);
			builderPiece.SetupPiece(this.gridSize);
			builderPiece.OnCreate();
			builderPiece.activatedTimeStamp = ((state == BuilderPiece.State.AttachedAndPlaced) ? activateTimeStamp : 0);
			builderPiece.SetMaterial(materialType, true);
			builderPiece.SetState(state, true);
			this.AddPiece(builderPiece);
			return builderPiece;
		}

		// Token: 0x06005E3B RID: 24123 RVA: 0x001DFEA0 File Offset: 0x001DE0A0
		private void RecyclePieceInternal(int pieceId, bool ignoreHaptics, bool playFX, int recyclerId)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (playFX)
			{
				try
				{
					piece.PlayRecycleFx();
				}
				catch (Exception)
				{
				}
			}
			if (!ignoreHaptics)
			{
				BuilderPiece rootPiece = piece.GetRootPiece();
				if (rootPiece != null && rootPiece.IsHeldLocal())
				{
					GorillaTagger.Instance.StartVibration(piece.IsHeldInLeftHand(), GorillaTagger.Instance.tapHapticStrength, this.pushAndEaseParams.snapDelayTime * 2f);
				}
			}
			BuilderPiece builderPiece = piece.firstChildPiece;
			while (builderPiece != null)
			{
				int pieceId2 = builderPiece.pieceId;
				builderPiece = builderPiece.nextSiblingPiece;
				this.RecyclePieceInternal(pieceId2, true, playFX, recyclerId);
			}
			if (this.isTableMutable && recyclerId >= 0 && recyclerId < this.recyclers.Count)
			{
				this.recyclers[recyclerId].OnRecycleRequestedAtRecycler(piece);
			}
			if (piece.state == BuilderPiece.State.OnConveyor && piece.shelfOwner >= 0 && piece.shelfOwner < this.conveyors.Count)
			{
				this.conveyors[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			else if ((piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed) && piece.shelfOwner >= 0 && piece.shelfOwner < this.dispenserShelves.Count)
			{
				this.dispenserShelves[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			if (piece.isArmShelf && this.isTableMutable)
			{
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				int num;
				if (piece.heldInLeftHand && this.playerToArmShelfLeft.TryGetValue(piece.heldByPlayerActorNumber, out num) && num == piece.pieceId)
				{
					this.playerToArmShelfLeft.Remove(piece.heldByPlayerActorNumber);
				}
				int num2;
				if (!piece.heldInLeftHand && this.playerToArmShelfRight.TryGetValue(piece.heldByPlayerActorNumber, out num2) && num2 == piece.pieceId)
				{
					this.playerToArmShelfRight.Remove(piece.heldByPlayerActorNumber);
				}
			}
			else if (PhotonNetwork.LocalPlayer.ActorNumber == piece.heldByPlayerActorNumber)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			int pieceId3 = piece.pieceId;
			piece.ClearParentPiece(false);
			piece.ClearParentHeld();
			piece.SetState(BuilderPiece.State.None, false);
			this.RemovePiece(piece);
			this.builderPool.DestroyPiece(piece);
		}

		// Token: 0x06005E3C RID: 24124 RVA: 0x001E00FC File Offset: 0x001DE2FC
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			return BuilderSetManager.instance.GetPiecePrefab(pieceType);
		}

		// Token: 0x06005E3D RID: 24125 RVA: 0x001E010C File Offset: 0x001DE30C
		private bool ValidateAttachPieceParams(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int piecePlacement)
		{
			if (pieceId == parentId)
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece2 == null)
			{
				return false;
			}
			if ((piecePlacement & 262143) != piecePlacement)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (this.DoesChainContainChain(piece, piece2))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece.gridPlanes.Count)
			{
				return false;
			}
			if (parentAttachIndex < 0 || parentAttachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(piecePlacement, out b, out b2, out b3);
			bool flag = (long)(b % 2) == 1L;
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[attachIndex];
			int num = flag ? builderAttachGridPlane.length : builderAttachGridPlane.width;
			int num2 = flag ? builderAttachGridPlane.width : builderAttachGridPlane.length;
			BuilderAttachGridPlane builderAttachGridPlane2 = piece2.gridPlanes[parentAttachIndex];
			int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
			int num4 = num3 - (builderAttachGridPlane2.width - 1);
			if ((int)b2 < num4 - num || (int)b2 > num3 + num)
			{
				return false;
			}
			int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
			int num6 = num5 - (builderAttachGridPlane2.length - 1);
			return (int)b3 >= num6 - num2 && (int)b3 <= num5 + num2;
		}

		// Token: 0x06005E3E RID: 24126 RVA: 0x001E025C File Offset: 0x001DE45C
		private void AttachPieceInternal(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int placement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece == null)
			{
				return;
			}
			byte b;
			sbyte xOffset;
			sbyte zOffset;
			BuilderTable.UnpackPiecePlacement(placement, out b, out xOffset, out zOffset);
			Vector3 zero = Vector3.zero;
			Quaternion localRotation;
			if (piece2 != null && parentAttachIndex >= 0 && parentAttachIndex < piece2.gridPlanes.Count)
			{
				Vector3 vector;
				Quaternion quaternion;
				piece.BumpTwistToPositionRotation(b, xOffset, zOffset, attachIndex, piece2.gridPlanes[parentAttachIndex], out zero, out localRotation, out vector, out quaternion);
			}
			else
			{
				localRotation = Quaternion.Euler(0f, (float)b * 90f, 0f);
			}
			piece.SetParentPiece(attachIndex, piece2, parentAttachIndex);
			piece.transform.SetLocalPositionAndRotation(zero, localRotation);
		}

		// Token: 0x06005E3F RID: 24127 RVA: 0x001E0308 File Offset: 0x001DE508
		private void AttachPieceToActorInternal(int pieceId, int actorNumber, bool isLeftHand)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				return;
			}
			VRRig rig = rigContainer.Rig;
			BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
			Transform parentHeld = isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform;
			if (piece.isArmShelf)
			{
				if (!this.isTableMutable)
				{
					return;
				}
				parentHeld = (isLeftHand ? rig.builderArmShelfLeft.pieceAnchor : rig.builderArmShelfRight.pieceAnchor);
				if (isLeftHand)
				{
					rig.builderArmShelfLeft.piece = piece;
					piece.armShelf = rig.builderArmShelfLeft;
					int num;
					if (this.playerToArmShelfLeft.TryGetValue(actorNumber, out num) && num != pieceId)
					{
						BuilderPiece piece2 = this.GetPiece(num);
						if (piece2 != null && piece2.isArmShelf)
						{
							piece2.ClearParentHeld();
							this.playerToArmShelfLeft.Remove(actorNumber);
							Vector3 position;
							Quaternion rotation;
							piece2.transform.GetPositionAndRotation(out position, out rotation);
							if (!this.ValidatePieceWorldTransform(position, rotation))
							{
								this.RecyclePieceInternal(piece2.pieceId, true, false, -1);
							}
						}
					}
					this.playerToArmShelfLeft.TryAdd(actorNumber, pieceId);
				}
				else
				{
					rig.builderArmShelfRight.piece = piece;
					piece.armShelf = rig.builderArmShelfRight;
					int num2;
					if (this.playerToArmShelfRight.TryGetValue(actorNumber, out num2) && num2 != pieceId)
					{
						BuilderPiece piece3 = this.GetPiece(num2);
						if (piece3 != null && piece3.isArmShelf)
						{
							piece3.ClearParentHeld();
							this.playerToArmShelfRight.Remove(actorNumber);
							Vector3 position2;
							Quaternion rotation2;
							piece3.transform.GetPositionAndRotation(out position2, out rotation2);
							if (!this.ValidatePieceWorldTransform(position2, rotation2))
							{
								this.RecyclePieceInternal(piece3.pieceId, true, false, -1);
							}
						}
					}
					this.playerToArmShelfRight.TryAdd(actorNumber, pieceId);
				}
			}
			Vector3 localPosition = piece.transform.localPosition;
			Quaternion localRotation = piece.transform.localRotation;
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.SetParentHeld(parentHeld, actorNumber, isLeftHand);
			piece.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			BuilderPiece.State newState = player.IsLocal ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed;
			if (piece.isArmShelf)
			{
				newState = BuilderPiece.State.AttachedToArm;
				piece.transform.localScale = Vector3.one;
			}
			piece.SetState(newState, false);
			if (!player.IsLocal)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			if (player.IsLocal && !piece.isArmShelf)
			{
				BuilderPieceInteractor.instance.AddPieceToHeld(piece, isLeftHand, localPosition, localRotation);
			}
		}

		// Token: 0x06005E40 RID: 24128 RVA: 0x001E057C File Offset: 0x001DE77C
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
		}

		// Token: 0x06005E41 RID: 24129 RVA: 0x001E05AC File Offset: 0x001DE7AC
		public void PlacePiece(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			this.PiecePlacedInternal(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer, timeStamp, force);
		}

		// Token: 0x06005E42 RID: 24130 RVA: 0x001E05D4 File Offset: 0x001DE7D4
		public void PiecePlacedInternal(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			if (!force && placedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Place,
				pieceId = pieceId,
				bumpOffsetX = bumpOffsetX,
				bumpOffsetZ = bumpOffsetZ,
				twist = twist,
				attachPieceId = attachPieceId,
				parentPieceId = parentPieceId,
				attachIndex = attachIndex,
				parentAttachIndex = parentAttachIndex,
				player = placedByPlayer,
				canRollback = force,
				localCommandId = localCommandId,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005E43 RID: 24131 RVA: 0x001E068C File Offset: 0x001DE88C
		public void ExecutePiecePlacedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int attachPieceId = cmd.attachPieceId;
			int parentPieceId = cmd.parentPieceId;
			int parentAttachIndex = cmd.parentAttachIndex;
			int attachIndex = cmd.attachIndex;
			NetPlayer player = cmd.player;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			byte twist = cmd.twist;
			sbyte bumpOffsetX = cmd.bumpOffsetX;
			sbyte bumpOffsetZ = cmd.bumpOffsetZ;
			if ((player == null || !player.IsLocal) && !this.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return;
			}
			BuilderAction action = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction action2 = BuilderActions.CreateMakeRoot(localCommandId, attachPieceId);
			BuilderAction action3 = BuilderActions.CreateAttachToPiece(localCommandId, attachPieceId, cmd.parentPieceId, cmd.attachIndex, cmd.parentAttachIndex, bumpOffsetX, bumpOffsetZ, twist, actorNumber, cmd.serverTimeStamp);
			if (cmd.canRollback)
			{
				BuilderAction action4 = BuilderActions.CreateDetachFromPiece(localCommandId, attachPieceId, actorNumber);
				BuilderAction action5 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderAction action6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(action6);
				this.AddRollbackAction(action5);
				this.AddRollbackAction(action4);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
			this.ExecuteAction(action3);
			if (!cmd.isQueued)
			{
				piece2.PlayPlacementFx();
			}
		}

		// Token: 0x06005E44 RID: 24132 RVA: 0x001E07F0 File Offset: 0x001DE9F0
		public bool ValidateGrabPieceParams(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
		{
			float num = 10000f;
			if (!localPosition.IsValid(num) || !localRotation.IsValid())
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (grabbedByPlayer == null)
			{
				return false;
			}
			if (!piece.CanPlayerGrabPiece(grabbedByPlayer.ActorNumber, piece.transform.position))
			{
				return false;
			}
			if (localPosition.sqrMagnitude > 6400f)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				Vector3 position = piece.transform.position;
				if (!this.IsPlayerHandNearAction(grabbedByPlayer, position, isLeftHand, false, 2.5f))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005E45 RID: 24133 RVA: 0x001E0890 File Offset: 0x001DEA90
		public bool ValidateGrabPieceState(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, Player grabbedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			return !(piece == null) && piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.None;
		}

		// Token: 0x06005E46 RID: 24134 RVA: 0x001E08C8 File Offset: 0x001DEAC8
		public bool IsLocationWithinSharedBuildArea(Vector3 worldPosition)
		{
			Vector3 vector = this.sharedBuildArea.transform.InverseTransformPoint(worldPosition);
			foreach (BoxCollider boxCollider in this.sharedBuildAreas)
			{
				Vector3 vector2 = boxCollider.center + boxCollider.size / 2f;
				Vector3 vector3 = boxCollider.center - boxCollider.size / 2f;
				if (vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005E47 RID: 24135 RVA: 0x001E09A8 File Offset: 0x001DEBA8
		private bool NoBlocksCheck()
		{
			foreach (BuilderTable.BoxCheckParams boxCheckParams in this.noBlocksAreas)
			{
				DebugUtil.DrawBox(boxCheckParams.center, boxCheckParams.rotation, boxCheckParams.halfExtents * 2f, Color.magenta, true, DebugUtil.Style.Wireframe);
				int num = 0;
				num |= 1 << BuilderTable.placedLayer;
				int num2 = Physics.OverlapBoxNonAlloc(boxCheckParams.center, boxCheckParams.halfExtents, this.noBlocksCheckResults, boxCheckParams.rotation, num);
				for (int i = 0; i < num2; i++)
				{
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.noBlocksCheckResults[i]);
					if (builderPieceFromCollider != null && builderPieceFromCollider.GetTable() == this && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable)
					{
						GTDev.LogError<string>(string.Format("Builder Table found piece {0} {1} in NO BLOCK AREA {2}", builderPieceFromCollider.pieceId, builderPieceFromCollider.displayName, builderPieceFromCollider.transform.position), null);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06005E48 RID: 24136 RVA: 0x001E0AD8 File Offset: 0x001DECD8
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
		}

		// Token: 0x06005E49 RID: 24137 RVA: 0x001E0AF4 File Offset: 0x001DECF4
		public void GrabPiece(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			this.PieceGrabbedInternal(localCommandId, pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer, force);
		}

		// Token: 0x06005E4A RID: 24138 RVA: 0x001E0B08 File Offset: 0x001DED08
		public void PieceGrabbedInternal(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			if (!force && grabbedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Grab,
				pieceId = pieceId,
				attachPieceId = -1,
				isLeft = isLeftHand,
				localPosition = localPosition,
				localRotation = localRotation,
				player = grabbedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005E4B RID: 24139 RVA: 0x001E0B9C File Offset: 0x001DED9C
		public void ExecutePieceGrabbedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			bool isLeft = cmd.isLeft;
			NetPlayer player = cmd.player;
			Vector3 localPosition = cmd.localPosition;
			Quaternion localRotation = cmd.localRotation;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((player == null || !player.Equals(NetworkSystem.Instance.LocalPlayer)) && !this.ValidateGrabPieceParams(pieceId, isLeft, localPosition, localRotation, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			bool flag = PhotonNetwork.CurrentRoom.GetPlayer(piece.heldByPlayerActorNumber, false) != null;
			bool flag2 = BuilderPiece.IsDroppedState(piece.state);
			bool flag3 = piece.state == BuilderPiece.State.OnConveyor || piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed;
			BuilderAction action = BuilderActions.CreateAttachToPlayer(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, actorNumber, cmd.isLeft);
			BuilderAction action2 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			if (flag)
			{
				BuilderAction action3 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				if (cmd.canRollback)
				{
					BuilderAction action4 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
					this.AddRollbackAction(action4);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action3);
				this.ExecuteAction(action);
				return;
			}
			if (flag3)
			{
				BuilderAction action5;
				if (piece.state == BuilderPiece.State.OnConveyor)
				{
					int serverTimestamp = PhotonNetwork.ServerTimestamp;
					float splineProgressForPiece = this.conveyorManager.GetSplineProgressForPiece(piece);
					action5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, true, serverTimestamp, splineProgressForPiece);
				}
				else
				{
					if (piece.state == BuilderPiece.State.Displayed)
					{
						int actorNumber2 = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					}
					action5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, false, 0, 0f);
				}
				BuilderAction action6 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece = piece.GetRootPiece();
				BuilderAction action7 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action5);
					this.AddRollbackAction(action7);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action6);
				this.ExecuteAction(action);
				return;
			}
			if (flag2)
			{
				BuilderAction action8 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece2 = piece.GetRootPiece();
				BuilderAction action9 = BuilderActions.CreateDropPieceRollback(localCommandId, rootPiece2, actorNumber);
				BuilderAction action10 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece2.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action9);
					this.AddRollbackAction(action10);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action8);
				this.ExecuteAction(action);
				return;
			}
			if (piece.parentPiece != null)
			{
				BuilderAction action11 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction action12 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(action12);
					this.AddRollbackAction(action2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action11);
				this.ExecuteAction(action);
			}
		}

		// Token: 0x06005E4C RID: 24140 RVA: 0x001E0E70 File Offset: 0x001DF070
		public bool ValidateDropPieceParams(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer)
		{
			float num = 10000f;
			if (position.IsValid(num) && rotation.IsValid())
			{
				float num2 = 10000f;
				if (velocity.IsValid(num2))
				{
					float num3 = 10000f;
					if (angVelocity.IsValid(num3))
					{
						BuilderPiece piece = this.GetPiece(pieceId);
						if (piece == null)
						{
							return false;
						}
						if (piece.isBuiltIntoTable)
						{
							return false;
						}
						if (droppedByPlayer == null)
						{
							return false;
						}
						if (velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
						{
							return false;
						}
						if (angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
						{
							return false;
						}
						if ((this.roomCenter.position - position).sqrMagnitude > this.acceptableSqrDistFromCenter || !this.ValidatePositionInArea(position))
						{
							return false;
						}
						if (piece.state == BuilderPiece.State.AttachedToArm)
						{
							if (piece.parentPiece == null)
							{
								return false;
							}
							if (piece.parentPiece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
							{
								return false;
							}
						}
						else if (piece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
						{
							return false;
						}
						return !PhotonNetwork.IsMasterClient || this.IsPlayerHandNearAction(droppedByPlayer, position, piece.heldInLeftHand, false, 2.5f);
					}
				}
			}
			return false;
		}

		// Token: 0x06005E4D RID: 24141 RVA: 0x001E0F98 File Offset: 0x001DF198
		public bool ValidateDropPieceState(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			bool flag = piece.state == BuilderPiece.State.AttachedToArm;
			return (flag || piece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber) && (!flag || piece.parentPiece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber);
		}

		// Token: 0x06005E4E RID: 24142 RVA: 0x001E0FEE File Offset: 0x001DF1EE
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
		}

		// Token: 0x06005E4F RID: 24143 RVA: 0x001E100C File Offset: 0x001DF20C
		public void DropPiece(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			this.PieceDroppedInternal(localCommandId, pieceId, position, rotation, velocity, angVelocity, droppedByPlayer, force);
		}

		// Token: 0x06005E50 RID: 24144 RVA: 0x001E102C File Offset: 0x001DF22C
		public void PieceDroppedInternal(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			if (!force && droppedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Drop,
				pieceId = pieceId,
				parentPieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				velocity = velocity,
				angVelocity = angVelocity,
				player = droppedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005E51 RID: 24145 RVA: 0x001E10C8 File Offset: 0x001DF2C8
		public void ExecutePieceDroppedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if (!this.ValidateDropPieceParams(pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, cmd.player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm)
			{
				BuilderPiece parentPiece = piece.parentPiece;
				BuilderAction action = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction action2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
				if (cmd.canRollback)
				{
					BuilderAction action3 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
					this.AddRollbackAction(action3);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(action);
				this.ExecuteAction(action2);
				return;
			}
			BuilderAction action4 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction action5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
			if (cmd.canRollback)
			{
				BuilderAction action6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(action6);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(action4);
			this.ExecuteAction(action5);
		}

		// Token: 0x06005E52 RID: 24146 RVA: 0x001E11F4 File Offset: 0x001DF3F4
		public void ExecutePieceRepelled(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			int attachPieceId = cmd.attachPieceId;
			BuilderPiece piece = this.GetPiece(pieceId);
			Vector3 velocity = cmd.velocity;
			if (piece == null)
			{
				return;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return;
			}
			if (piece.state != BuilderPiece.State.Grabbed && piece.state != BuilderPiece.State.GrabbedLocal && piece.state != BuilderPiece.State.Dropped && piece.state != BuilderPiece.State.AttachedToDropped && piece.state != BuilderPiece.State.AttachedToArm)
			{
				return;
			}
			if (attachPieceId >= 0 && attachPieceId < this.dropZones.Count)
			{
				BuilderDropZone builderDropZone = this.dropZones[attachPieceId];
				builderDropZone.PlayEffect();
				if (builderDropZone.overrideDirection)
				{
					velocity = builderDropZone.GetRepelDirectionWorld() * BuilderTable.DROP_ZONE_REPEL;
				}
			}
			if (piece.heldByPlayerActorNumber >= 0)
			{
				BuilderAction action = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction action2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
				this.ExecuteAction(action);
				this.ExecuteAction(action2);
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm && piece.parentPiece != null)
			{
				BuilderAction action3 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction action4 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
				this.ExecuteAction(action3);
				this.ExecuteAction(action4);
				return;
			}
			BuilderAction action5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, velocity, cmd.angVelocity, actorNumber);
			this.ExecuteAction(action5);
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x001E1394 File Offset: 0x001DF594
		private void CleanUpDroppedPiece()
		{
			if (!PhotonNetwork.IsMasterClient || this.droppedPieces.Count <= BuilderTable.DROPPED_PIECE_LIMIT)
			{
				return;
			}
			BuilderPiece builderPiece = this.FindFirstSleepingPiece();
			if (builderPiece != null && builderPiece.state == BuilderPiece.State.Dropped)
			{
				this.RequestRecyclePiece(builderPiece, false, -1);
				return;
			}
			Debug.LogErrorFormat("Piece {0} in Dropped List is {1}", new object[]
			{
				builderPiece.pieceId,
				builderPiece.state
			});
		}

		// Token: 0x06005E54 RID: 24148 RVA: 0x001E140C File Offset: 0x001DF60C
		public void FreezeDroppedPiece(BuilderPiece piece)
		{
			int num = this.droppedPieces.IndexOf(piece);
			if (num >= 0)
			{
				BuilderTable.DroppedPieceData value = this.droppedPieceData[num];
				value.droppedState = BuilderTable.DroppedPieceState.Frozen;
				value.speedThreshCrossedTime = 0f;
				this.droppedPieceData[num] = value;
				if (piece.rigidBody != null)
				{
					piece.SetKinematic(true, false);
				}
				piece.forcedFrozen = true;
			}
		}

		// Token: 0x06005E55 RID: 24149 RVA: 0x001E1478 File Offset: 0x001DF678
		public void AddPieceToDropList(BuilderPiece piece)
		{
			this.droppedPieces.Add(piece);
			this.droppedPieceData.Add(new BuilderTable.DroppedPieceData
			{
				speedThreshCrossedTime = 0f,
				droppedState = BuilderTable.DroppedPieceState.Light,
				filteredSpeed = 0f
			});
		}

		// Token: 0x06005E56 RID: 24150 RVA: 0x001E14C8 File Offset: 0x001DF6C8
		private BuilderPiece FindFirstSleepingPiece()
		{
			if (this.droppedPieces.Count < 1)
			{
				return null;
			}
			BuilderPiece builderPiece = this.droppedPieces[0];
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieces[i].rigidBody != null && this.droppedPieces[i].rigidBody.IsSleeping())
				{
					BuilderPiece result = this.droppedPieces[i];
					this.droppedPieces.RemoveAt(i);
					this.droppedPieceData.RemoveAt(i);
					return result;
				}
			}
			BuilderPiece result2 = this.droppedPieces[0];
			this.droppedPieces.RemoveAt(0);
			this.droppedPieceData.RemoveAt(0);
			return result2;
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x001E1582 File Offset: 0x001DF782
		public void RemovePieceFromDropList(BuilderPiece piece)
		{
			if (piece.state == BuilderPiece.State.Dropped)
			{
				this.droppedPieces.Remove(piece);
			}
		}

		// Token: 0x06005E58 RID: 24152 RVA: 0x001E159C File Offset: 0x001DF79C
		private void UpdateDroppedPieces(float dt)
		{
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieceData[i].droppedState == BuilderTable.DroppedPieceState.Frozen && this.droppedPieces[i].state == BuilderPiece.State.Dropped)
				{
					BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[i];
					droppedPieceData.speedThreshCrossedTime += dt;
					if (droppedPieceData.speedThreshCrossedTime > 60f)
					{
						this.droppedPieces[i].forcedFrozen = false;
						this.droppedPieces[i].ClearCollisionHistory();
						this.droppedPieces[i].SetKinematic(false, true);
						droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Light;
						droppedPieceData.speedThreshCrossedTime = 0f;
					}
					this.droppedPieceData[i] = droppedPieceData;
				}
				else
				{
					Rigidbody rigidBody = this.droppedPieces[i].rigidBody;
					if (rigidBody != null)
					{
						BuilderTable.DroppedPieceData droppedPieceData2 = this.droppedPieceData[i];
						float magnitude = rigidBody.linearVelocity.magnitude;
						droppedPieceData2.filteredSpeed = droppedPieceData2.filteredSpeed * 0.95f + magnitude * 0.05f;
						switch (droppedPieceData2.droppedState)
						{
						case BuilderTable.DroppedPieceState.Light:
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed < 0.05f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0f)
							{
								rigidBody.mass = 10000f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Heavy;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						case BuilderTable.DroppedPieceState.Heavy:
							droppedPieceData2.speedThreshCrossedTime += dt;
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed > 0.075f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0.5f)
							{
								rigidBody.mass = 1f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Light;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						}
						this.droppedPieceData[i] = droppedPieceData2;
					}
				}
			}
		}

		// Token: 0x06005E59 RID: 24153 RVA: 0x001E17B5 File Offset: 0x001DF9B5
		private void SetLocalPlayerOwnsPlot(bool ownsPlot)
		{
			this.doesLocalPlayerOwnPlot = ownsPlot;
			UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
			if (onLocalPlayerClaimedPlot == null)
			{
				return;
			}
			onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
		}

		// Token: 0x06005E5A RID: 24154 RVA: 0x001E17D4 File Offset: 0x001DF9D4
		public void PlotClaimed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.ClaimPlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E5B RID: 24155 RVA: 0x001E1810 File Offset: 0x001DFA10
		public void ExecuteClaimPlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (pieceId == -1)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (this.plotOwners.TryAdd(player.ActorNumber, pieceId) && piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				builderPiecePrivatePlot.ClaimPlotForPlayerNumber(player.ActorNumber);
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(true);
				}
			}
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x001E1894 File Offset: 0x001DFA94
		public void PlayerLeftRoom(int playerActorNumber)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.PlayerLeftRoom,
				pieceId = playerActorNumber,
				player = null
			};
			bool force = this.tableState == BuilderTable.TableState.WaitForMasterResync;
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005E5D RID: 24157 RVA: 0x001E18D8 File Offset: 0x001DFAD8
		public void ExecutePlayerLeftRoom(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			int num = (player != null) ? player.ActorNumber : cmd.pieceId;
			this.FreePlotInternal(-1, num);
			int pieceId;
			if (this.playerToArmShelfLeft.TryGetValue(num, out pieceId))
			{
				this.RecyclePieceInternal(pieceId, true, false, -1);
			}
			this.playerToArmShelfLeft.Remove(num);
			int pieceId2;
			if (this.playerToArmShelfRight.TryGetValue(num, out pieceId2))
			{
				this.RecyclePieceInternal(pieceId2, true, false, -1);
			}
			this.playerToArmShelfRight.Remove(num);
		}

		// Token: 0x06005E5E RID: 24158 RVA: 0x001E1954 File Offset: 0x001DFB54
		public void PlotFreed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FreePlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E5F RID: 24159 RVA: 0x001E1990 File Offset: 0x001DFB90
		public void ExecuteFreePlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			this.FreePlotInternal(pieceId, player.ActorNumber);
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x001E19BC File Offset: 0x001DFBBC
		private void FreePlotInternal(int plotPieceId, int requestingPlayer)
		{
			if (plotPieceId == -1 && !this.plotOwners.TryGetValue(requestingPlayer, out plotPieceId))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(plotPieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				int ownerActorNumber = builderPiecePrivatePlot.GetOwnerActorNumber();
				this.plotOwners.Remove(ownerActorNumber);
				builderPiecePrivatePlot.FreePlot();
				if (ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(false);
				}
			}
		}

		// Token: 0x06005E61 RID: 24161 RVA: 0x001E1A30 File Offset: 0x001DFC30
		public bool DoesPlayerOwnPlot(int actorNum)
		{
			return this.plotOwners.ContainsKey(actorNum);
		}

		// Token: 0x06005E62 RID: 24162 RVA: 0x001E1A3E File Offset: 0x001DFC3E
		public void RequestPaintPiece(int pieceId, int materialType)
		{
			this.builderNetworking.RequestPaintPiece(pieceId, materialType);
		}

		// Token: 0x06005E63 RID: 24163 RVA: 0x001E1A4D File Offset: 0x001DFC4D
		public void PaintPiece(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			this.PaintPieceInternal(pieceId, materialType, paintingPlayer, force);
		}

		// Token: 0x06005E64 RID: 24164 RVA: 0x001E1A5C File Offset: 0x001DFC5C
		private void PaintPieceInternal(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			if (!force && paintingPlayer == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Paint,
				pieceId = pieceId,
				materialType = materialType,
				player = NetPlayer.Get(paintingPlayer)
			};
			this.RouteNewCommand(cmd, force);
		}

		// Token: 0x06005E65 RID: 24165 RVA: 0x001E1AB0 File Offset: 0x001DFCB0
		public void ExecutePiecePainted(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int materialType = cmd.materialType;
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece != null && !piece.isBuiltIntoTable)
			{
				piece.SetMaterial(materialType, false);
			}
		}

		// Token: 0x06005E66 RID: 24166 RVA: 0x001E1AEC File Offset: 0x001DFCEC
		public void CreateArmShelvesForPlayersInBuilder()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				foreach (Player player in this.builderNetworking.armShelfRequests)
				{
					if (player != null)
					{
						this.builderNetworking.RequestCreateArmShelfForPlayer(player);
					}
				}
				this.builderNetworking.armShelfRequests.Clear();
			}
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x001E1B74 File Offset: 0x001DFD74
		public void RemoveArmShelfForPlayer(Player player)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				this.builderNetworking.armShelfRequests.Remove(player);
				return;
			}
			int pieceId;
			if (this.playerToArmShelfLeft.TryGetValue(player.ActorNumber, out pieceId))
			{
				BuilderPiece piece = this.GetPiece(pieceId);
				this.playerToArmShelfLeft.Remove(player.ActorNumber);
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(pieceId, piece.transform.position, piece.transform.rotation, false, -1);
				}
				else
				{
					this.DropPieceForPlayerLeavingInternal(piece, player.ActorNumber);
				}
			}
			int pieceId2;
			if (this.playerToArmShelfRight.TryGetValue(player.ActorNumber, out pieceId2))
			{
				BuilderPiece piece2 = this.GetPiece(pieceId2);
				this.playerToArmShelfRight.Remove(player.ActorNumber);
				if (piece2.armShelf != null)
				{
					piece2.armShelf.piece = null;
					piece2.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(pieceId2, piece2.transform.position, piece2.transform.rotation, false, -1);
					return;
				}
				this.DropPieceForPlayerLeavingInternal(piece2, player.ActorNumber);
			}
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x001E1CC0 File Offset: 0x001DFEC0
		public void DropAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.DropPieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x001E1D20 File Offset: 0x001DFF20
		public void RecycleAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.RecyclePieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x001E1D80 File Offset: 0x001DFF80
		private void DropPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction action = BuilderActions.CreateDetachFromPlayer(-1, piece.pieceId, playerActorNumber);
			BuilderAction action2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x001E1DD7 File Offset: 0x001DFFD7
		private void RecyclePieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, false, -1);
		}

		// Token: 0x06005E6C RID: 24172 RVA: 0x001E1E04 File Offset: 0x001E0004
		private void DetachPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction action = BuilderActions.CreateDetachFromPiece(-1, piece.pieceId, playerActorNumber);
			BuilderAction action2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(action);
			this.ExecuteAction(action2);
		}

		// Token: 0x06005E6D RID: 24173 RVA: 0x001E1E5C File Offset: 0x001E005C
		public void CreateArmShelf(int pieceIdLeft, int pieceIdRight, int pieceType, Player player)
		{
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdLeft,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = true
			};
			this.RouteNewCommand(cmd, false);
			BuilderTable.BuilderCommand cmd2 = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdRight,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = false
			};
			this.RouteNewCommand(cmd2, false);
		}

		// Token: 0x06005E6E RID: 24174 RVA: 0x001E1EEC File Offset: 0x001E00EC
		public void ExecuteArmShelfCreated(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			bool isLeft = cmd.isLeft;
			if (this.GetPiece(cmd.pieceId) != null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				BuilderArmShelf builderArmShelf = isLeft ? rigContainer.Rig.builderArmShelfLeft : rigContainer.Rig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					if (builderArmShelf.piece != null)
					{
						if (builderArmShelf.piece.isArmShelf && builderArmShelf.piece.isActiveAndEnabled)
						{
							builderArmShelf.piece.armShelf = null;
							this.RecyclePiece(builderArmShelf.piece.pieceId, builderArmShelf.piece.transform.position, builderArmShelf.piece.transform.rotation, false, -1, PhotonNetwork.LocalPlayer);
						}
						else
						{
							builderArmShelf.piece = null;
						}
						BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece;
						builderPiece.armShelf = builderArmShelf;
						builderPiece.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.AddOrUpdate(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.AddOrUpdate(player.ActorNumber, cmd.pieceId);
						return;
					}
					else
					{
						BuilderPiece builderPiece2 = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece2;
						builderPiece2.armShelf = builderArmShelf;
						builderPiece2.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece2.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.TryAdd(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.TryAdd(player.ActorNumber, cmd.pieceId);
					}
				}
			}
		}

		// Token: 0x06005E6F RID: 24175 RVA: 0x001E2138 File Offset: 0x001E0338
		public void ClearLocalArmShelf()
		{
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig != null)
			{
				BuilderArmShelf builderArmShelf = offlineVRRig.builderArmShelfLeft;
				if (builderArmShelf != null)
				{
					BuilderPiece piece = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece != null)
					{
						piece.transform.SetParent(null);
					}
				}
				builderArmShelf = offlineVRRig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					BuilderPiece piece2 = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece2 != null)
					{
						piece2.transform.SetParent(null);
					}
				}
			}
		}

		// Token: 0x06005E70 RID: 24176 RVA: 0x001E21C0 File Offset: 0x001E03C0
		public void PieceEnteredDropZone(int pieceId, Vector3 worldPos, Quaternion worldRot, int dropZoneId)
		{
			Vector3 velocity = (this.roomCenter.position - worldPos).normalized * BuilderTable.DROP_ZONE_REPEL;
			BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Repel,
				pieceId = pieceId,
				parentPieceId = pieceId,
				attachPieceId = dropZoneId,
				localPosition = worldPos,
				localRotation = worldRot,
				velocity = velocity,
				angVelocity = Vector3.zero,
				player = NetworkSystem.Instance.MasterClient,
				canRollback = false
			};
			this.RouteNewCommand(cmd, false);
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x001E2264 File Offset: 0x001E0464
		public bool ValidateRepelPiece(BuilderPiece piece)
		{
			if (!this.isSetup)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return false;
			}
			if (piece.state == BuilderPiece.State.Grabbed || piece.state == BuilderPiece.State.GrabbedLocal || piece.state == BuilderPiece.State.Dropped || piece.state == BuilderPiece.State.AttachedToDropped || piece.state == BuilderPiece.State.AttachedToArm)
			{
				bool flag = false;
				for (int i = 0; i < this.repelHistoryLength; i++)
				{
					flag = (flag || this.repelledPieceRoots[i].Contains(piece.pieceId));
					if (flag)
					{
						return false;
					}
				}
				this.repelledPieceRoots[this.repelHistoryIndex].Add(piece.pieceId);
				return true;
			}
			return false;
		}

		// Token: 0x06005E72 RID: 24178 RVA: 0x001E2308 File Offset: 0x001E0508
		public void RepelPieceTowardTable(int pieceID)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			Vector3 position = piece.transform.position;
			Quaternion rotation = piece.transform.rotation;
			if (position.y < this.tableCenter.position.y)
			{
				position.y = this.tableCenter.position.y;
			}
			Vector3 linearVelocity = (this.tableCenter.position - position).normalized * BuilderTable.DROP_ZONE_REPEL;
			if (piece.IsHeldLocal())
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.transform.localScale = Vector3.one;
			piece.SetState(BuilderPiece.State.Dropped, false);
			piece.transform.SetLocalPositionAndRotation(position, rotation);
			if (piece.rigidBody != null)
			{
				piece.rigidBody.position = position;
				piece.rigidBody.rotation = rotation;
				piece.rigidBody.linearVelocity = linearVelocity;
				piece.rigidBody.AddForce(Vector3.up * (BuilderTable.DROP_ZONE_REPEL / 2f) * piece.rigidBody.mass, ForceMode.Impulse);
				piece.rigidBody.angularVelocity = Vector3.zero;
			}
		}

		// Token: 0x06005E73 RID: 24179 RVA: 0x001E2450 File Offset: 0x001E0650
		public BuilderPiece GetPiece(int pieceId)
		{
			int num;
			if (this.pieceIDToIndexCache.TryGetValue(pieceId, out num))
			{
				if (num >= 0 && num < this.pieces.Count)
				{
					return this.pieces[num];
				}
				this.pieceIDToIndexCache.Remove(pieceId);
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].pieceId == pieceId)
				{
					this.pieceIDToIndexCache.Add(pieceId, i);
					return this.pieces[i];
				}
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				if (this.basePieces[j].pieceId == pieceId)
				{
					return this.basePieces[j];
				}
			}
			return null;
		}

		// Token: 0x06005E74 RID: 24180 RVA: 0x001E2515 File Offset: 0x001E0715
		public void AddPiece(BuilderPiece piece)
		{
			this.pieces.Add(piece);
			this.UseResources(piece);
			this.AddPieceData(piece);
		}

		// Token: 0x06005E75 RID: 24181 RVA: 0x001E2532 File Offset: 0x001E0732
		public void RemovePiece(BuilderPiece piece)
		{
			this.pieces.Remove(piece);
			this.AddResources(piece);
			this.RemovePieceData(piece);
			this.pieceIDToIndexCache.Clear();
		}

		// Token: 0x06005E76 RID: 24182 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void CreateData()
		{
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void DestroyData()
		{
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x001138AD File Offset: 0x00111AAD
		private int AddPieceData(BuilderPiece piece)
		{
			return -1;
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void UpdatePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void RemovePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06005E7B RID: 24187 RVA: 0x001138AD File Offset: 0x00111AAD
		private int AddGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
			return -1;
		}

		// Token: 0x06005E7C RID: 24188 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void RemoveGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
		}

		// Token: 0x06005E7D RID: 24189 RVA: 0x001138AD File Offset: 0x00111AAD
		private int AddPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			return -1;
		}

		// Token: 0x06005E7E RID: 24190 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void RemovePrivatePlotData(BuilderPiecePrivatePlot plot)
		{
		}

		// Token: 0x06005E7F RID: 24191 RVA: 0x001E255A File Offset: 0x001E075A
		public void OnButtonFreeRotation(BuilderOptionButton button, bool isLeftHand)
		{
			this.useSnapRotation = !this.useSnapRotation;
			button.SetPressed(this.useSnapRotation);
		}

		// Token: 0x06005E80 RID: 24192 RVA: 0x001E2577 File Offset: 0x001E0777
		public void OnButtonFreePosition(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.usePlacementStyle == BuilderPlacementStyle.Float)
			{
				this.usePlacementStyle = BuilderPlacementStyle.SnapDown;
			}
			else if (this.usePlacementStyle == BuilderPlacementStyle.SnapDown)
			{
				this.usePlacementStyle = BuilderPlacementStyle.Float;
			}
			button.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
		}

		// Token: 0x06005E81 RID: 24193 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnButtonSaveLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x06005E82 RID: 24194 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnButtonClearLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x06005E83 RID: 24195 RVA: 0x001E25AC File Offset: 0x001E07AC
		public bool TryPlaceGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, List<BuilderAttachGridPlane> checkGridPlanes, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			Vector3 position = gridPlane.transform.position;
			Quaternion rotation = gridPlane.transform.rotation;
			if (this.gridSize <= 0f)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < checkGridPlanes.Count; i++)
			{
				BuilderAttachGridPlane checkGridPlane = checkGridPlanes[i];
				this.TryPlaceGridPlaneOnGridPlane(piece, gridPlane, position, rotation, checkGridPlane, ref potentialPlacement, ref result);
			}
			return result;
		}

		// Token: 0x06005E84 RID: 24196 RVA: 0x001E2620 File Offset: 0x001E0820
		public bool TryPlaceGridPlaneOnGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, Vector3 gridPlanePos, Quaternion gridPlaneRot, BuilderAttachGridPlane checkGridPlane, ref BuilderPotentialPlacement potentialPlacement, ref bool success)
		{
			if (checkGridPlane.male == gridPlane.male)
			{
				return false;
			}
			if (checkGridPlane.piece == gridPlane.piece)
			{
				return false;
			}
			Transform center = checkGridPlane.center;
			Vector3 position = center.position;
			float sqrMagnitude = (position - gridPlanePos).sqrMagnitude;
			float num = checkGridPlane.boundingRadius + gridPlane.boundingRadius;
			if (sqrMagnitude > num * num)
			{
				return false;
			}
			Quaternion rotation = center.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Quaternion quaternion2 = quaternion * gridPlaneRot;
			if (Vector3.Dot(Vector3.up, quaternion2 * Vector3.up) < this.currSnapParams.maxUpDotProduct)
			{
				return false;
			}
			Vector3 vector = quaternion * (gridPlanePos - position);
			float y = vector.y;
			float num2 = -Mathf.Abs(y);
			if (success && num2 < potentialPlacement.score)
			{
				return false;
			}
			if (Mathf.Abs(y) > 1f)
			{
				return false;
			}
			if ((gridPlane.male && y > this.currSnapParams.minOffsetY) || (!gridPlane.male && y < -this.currSnapParams.minOffsetY))
			{
				return false;
			}
			if (Mathf.Abs(y) > this.currSnapParams.maxOffsetY)
			{
				return false;
			}
			Quaternion quaternion3;
			Quaternion rotation2;
			BoingKit.QuaternionUtil.DecomposeSwingTwist(quaternion2, Vector3.up, out quaternion3, out rotation2);
			float maxTwistDotProduct = this.currSnapParams.maxTwistDotProduct;
			Vector3 lhs = rotation2 * Vector3.forward;
			float num3 = Vector3.Dot(lhs, Vector3.forward);
			float num4 = Vector3.Dot(lhs, Vector3.right);
			bool flag = Mathf.Abs(num3) > maxTwistDotProduct;
			bool flag2 = Mathf.Abs(num4) > maxTwistDotProduct;
			if (!flag && !flag2)
			{
				return false;
			}
			float y2;
			uint num5;
			if (flag)
			{
				y2 = ((num3 > 0f) ? 0f : 180f);
				num5 = ((num3 > 0f) ? 0U : 2U);
			}
			else
			{
				y2 = ((num4 > 0f) ? 90f : 270f);
				num5 = ((num4 > 0f) ? 1U : 3U);
			}
			int num6 = flag2 ? gridPlane.width : gridPlane.length;
			int num7 = flag2 ? gridPlane.length : gridPlane.width;
			float num8 = (num7 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num9 = (num6 % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num10 = (checkGridPlane.width % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num11 = (checkGridPlane.length % 2 == 0) ? (this.gridSize / 2f) : 0f;
			float num12 = num8 - num10;
			float num13 = num9 - num11;
			int num14 = Mathf.RoundToInt((vector.x - num12) / this.gridSize);
			int num15 = Mathf.RoundToInt((vector.z - num13) / this.gridSize);
			int num16 = num14 + Mathf.FloorToInt((float)num7 / 2f);
			int num17 = num15 + Mathf.FloorToInt((float)num6 / 2f);
			int num18 = num16 - (num7 - 1);
			int num19 = num17 - (num6 - 1);
			int num20 = Mathf.FloorToInt((float)checkGridPlane.width / 2f);
			int num21 = Mathf.FloorToInt((float)checkGridPlane.length / 2f);
			int num22 = num20 - (checkGridPlane.width - 1);
			int num23 = num21 - (checkGridPlane.length - 1);
			if (num18 > num20 || num16 < num22 || num19 > num21 || num17 < num23)
			{
				return false;
			}
			BuilderPiece rootPiece = checkGridPlane.piece.GetRootPiece();
			if (BuilderTable.ShareSameRoot(gridPlane.piece, rootPiece))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, gridPlane.piece, rootPiece))
			{
				return false;
			}
			BuilderPiece piece2 = checkGridPlane.piece;
			if (piece2 != null)
			{
				if (piece2.preventSnapUntilMoved > 0)
				{
					return false;
				}
				if (piece2.requestedParentPiece != null && BuilderTable.ShareSameRoot(piece, piece2.requestedParentPiece))
				{
					return false;
				}
			}
			Quaternion rhs = Quaternion.Euler(0f, y2, 0f);
			Quaternion lhs2 = rotation * rhs;
			float x = (float)num14 * this.gridSize + num12;
			float z = (float)num15 * this.gridSize + num13;
			Vector3 point = new Vector3(x, 0f, z);
			Vector3 a = position + rotation * point;
			Transform center2 = gridPlane.center;
			Quaternion quaternion4 = lhs2 * Quaternion.Inverse(center2.localRotation);
			Vector3 point2 = piece.transform.InverseTransformPoint(center2.position);
			Vector3 localPosition = a - quaternion4 * point2;
			potentialPlacement.localPosition = localPosition;
			potentialPlacement.localRotation = quaternion4;
			potentialPlacement.score = num2;
			success = true;
			potentialPlacement.parentPiece = piece2;
			potentialPlacement.parentAttachIndex = checkGridPlane.attachIndex;
			potentialPlacement.attachDistance = Mathf.Abs(y);
			potentialPlacement.attachPlaneNormal = Vector3.up;
			if (!checkGridPlane.male)
			{
				potentialPlacement.attachPlaneNormal *= -1f;
			}
			if (potentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				potentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(potentialPlacement.localPosition);
				potentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * potentialPlacement.localRotation;
			}
			potentialPlacement.parentAttachBounds.min.x = Mathf.Max(num22, num18);
			potentialPlacement.parentAttachBounds.min.y = Mathf.Max(num23, num19);
			potentialPlacement.parentAttachBounds.max.x = Mathf.Min(num20, num16);
			potentialPlacement.parentAttachBounds.max.y = Mathf.Min(num21, num17);
			Vector2Int v = Vector2Int.zero;
			Vector2Int v2 = Vector2Int.zero;
			v.x = potentialPlacement.parentAttachBounds.min.x - num14;
			v2.x = potentialPlacement.parentAttachBounds.max.x - num14;
			v.y = potentialPlacement.parentAttachBounds.min.y - num15;
			v2.y = potentialPlacement.parentAttachBounds.max.y - num15;
			potentialPlacement.twist = (byte)num5;
			potentialPlacement.bumpOffsetX = (sbyte)num14;
			potentialPlacement.bumpOffsetZ = (sbyte)num15;
			int offsetX = (num7 % 2 == 0) ? 1 : 0;
			int offsetY = (num6 % 2 == 0) ? 1 : 0;
			if (flag && num3 < 0f)
			{
				v = this.Rotate180(v, offsetX, offsetY);
				v2 = this.Rotate180(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 < 0f)
			{
				v = this.Rotate270(v, offsetX, offsetY);
				v2 = this.Rotate270(v2, offsetX, offsetY);
			}
			else if (flag2 && num4 > 0f)
			{
				v = this.Rotate90(v, offsetX, offsetY);
				v2 = this.Rotate90(v2, offsetX, offsetY);
			}
			potentialPlacement.attachBounds.min.x = Mathf.Min(v.x, v2.x);
			potentialPlacement.attachBounds.min.y = Mathf.Min(v.y, v2.y);
			potentialPlacement.attachBounds.max.x = Mathf.Max(v.x, v2.x);
			potentialPlacement.attachBounds.max.y = Mathf.Max(v.y, v2.y);
			return true;
		}

		// Token: 0x06005E85 RID: 24197 RVA: 0x001E2D9A File Offset: 0x001E0F9A
		private Vector2Int Rotate90(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y * -1 + offsetY, v.x);
		}

		// Token: 0x06005E86 RID: 24198 RVA: 0x001E2DB3 File Offset: 0x001E0FB3
		private Vector2Int Rotate270(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y, v.x * -1 + offsetX);
		}

		// Token: 0x06005E87 RID: 24199 RVA: 0x001E2DCC File Offset: 0x001E0FCC
		private Vector2Int Rotate180(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.x * -1 + offsetX, v.y * -1 + offsetY);
		}

		// Token: 0x06005E88 RID: 24200 RVA: 0x001E2DE9 File Offset: 0x001E0FE9
		public bool ShareSameRoot(BuilderAttachGridPlane plane, BuilderAttachGridPlane otherPlane)
		{
			return !(plane == null) && !(otherPlane == null) && !(otherPlane.piece == null) && BuilderTable.ShareSameRoot(plane.piece, otherPlane.piece);
		}

		// Token: 0x06005E89 RID: 24201 RVA: 0x001E2E20 File Offset: 0x001E1020
		public static bool ShareSameRoot(BuilderPiece piece, BuilderPiece otherPiece)
		{
			if (otherPiece == null || piece == null)
			{
				return false;
			}
			if (piece == otherPiece)
			{
				return true;
			}
			BuilderPiece builderPiece = piece;
			int num = 2048;
			while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
			{
				builderPiece = builderPiece.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			num = 2048;
			BuilderPiece builderPiece2 = otherPiece;
			while (builderPiece2.parentPiece != null && !builderPiece2.parentPiece.isBuiltIntoTable)
			{
				builderPiece2 = builderPiece2.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			return builderPiece == builderPiece2;
		}

		// Token: 0x06005E8A RID: 24202 RVA: 0x001E2EC0 File Offset: 0x001E10C0
		public bool TryPlacePieceOnTableNoDrop(bool leftHand, BuilderPiece testPiece, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			if (testPiece == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			return this.TryPlacePieceGridPlanesOnTableInternal(testPiece, this.maxPlacementChildDepth, checkGridPlanesMale, checkGridPlanesFemale, out potentialPlacement);
		}

		// Token: 0x06005E8B RID: 24203 RVA: 0x001E2F10 File Offset: 0x001E1110
		public bool TryPlacePieceOnTableNoDropJobs(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderPieceData> pieceData, NativeList<BuilderGridPlaneData> checkGridPlaneData, NativeList<BuilderPieceData> checkPieceData, out BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = Vector3.zero,
				worldToLocalRot = Quaternion.identity,
				localToWorldPos = Vector3.zero,
				localToWorldRot = Quaternion.identity,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			BuilderPotentialPlacementData builderPotentialPlacementData = default(BuilderPotentialPlacementData);
			bool flag = false;
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData2 = nativeQueue.Dequeue();
				if (!flag || builderPotentialPlacementData2.score > builderPotentialPlacementData.score)
				{
					builderPotentialPlacementData = builderPotentialPlacementData2;
					flag = true;
				}
			}
			if (flag)
			{
				potentialPlacement = builderPotentialPlacementData.ToPotentialPlacement(this);
			}
			if (flag)
			{
				nativeQueue.Clear();
				this.currSnapParams = this.overlapParams;
				Vector3 worldToLocalPos = -potentialPlacement.attachPiece.transform.position;
				Quaternion worldToLocalRot = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				Quaternion localToWorldRot = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
				Vector3 localToWorldPos = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
				new BuilderFindPotentialSnaps
				{
					gridSize = this.gridSize,
					currSnapParams = this.currSnapParams,
					gridPlanes = gridPlaneData,
					checkGridPlanes = checkGridPlaneData,
					worldToLocalPos = worldToLocalPos,
					worldToLocalRot = worldToLocalRot,
					localToWorldPos = localToWorldPos,
					localToWorldRot = localToWorldRot,
					potentialPlacements = nativeQueue.AsParallelWriter()
				}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
				while (!nativeQueue.IsEmpty())
				{
					BuilderPotentialPlacementData builderPotentialPlacementData3 = nativeQueue.Dequeue();
					if (builderPotentialPlacementData3.attachDistance < this.currSnapParams.maxBlockSnapDist)
					{
						allPlacements.Add(builderPotentialPlacementData3.ToPotentialPlacement(this));
					}
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x001E317C File Offset: 0x001E137C
		public bool CalcAllPotentialPlacements(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderGridPlaneData> checkGridPlaneData, BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			if (this == null)
			{
				return false;
			}
			bool result = false;
			this.currSnapParams = this.overlapParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			nativeQueue.Clear();
			Vector3 worldToLocalPos = -potentialPlacement.attachPiece.transform.position;
			Quaternion worldToLocalRot = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
			BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
			Quaternion localToWorldRot = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
			Vector3 localToWorldPos = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = worldToLocalPos,
				worldToLocalRot = worldToLocalRot,
				localToWorldPos = localToWorldPos,
				localToWorldRot = localToWorldRot,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData = nativeQueue.Dequeue();
				if (builderPotentialPlacementData.attachDistance < this.currSnapParams.maxBlockSnapDist)
				{
					allPlacements.Add(builderPotentialPlacementData.ToPotentialPlacement(this));
				}
			}
			nativeQueue.Dispose();
			return result;
		}

		// Token: 0x06005E8D RID: 24205 RVA: 0x001E32E8 File Offset: 0x001E14E8
		public bool CanPiecesPotentiallySnap(BuilderPiece pieceInHand, BuilderPiece piece)
		{
			BuilderPiece rootPiece = piece.GetRootPiece();
			return !(rootPiece == pieceInHand) && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece) && (!(piece.requestedParentPiece != null) || !BuilderTable.ShareSameRoot(pieceInHand, piece.requestedParentPiece)) && piece.preventSnapUntilMoved <= 0;
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x001E334C File Offset: 0x001E154C
		public bool CanPiecesPotentiallyOverlap(BuilderPiece pieceInHand, BuilderPiece rootWhenPlaced, BuilderPiece.State stateWhenPlaced, BuilderPiece otherPiece)
		{
			BuilderPiece rootPiece = otherPiece.GetRootPiece();
			if (rootPiece == pieceInHand)
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece))
			{
				return false;
			}
			if (otherPiece.requestedParentPiece != null && BuilderTable.ShareSameRoot(pieceInHand, otherPiece.requestedParentPiece))
			{
				return false;
			}
			if (otherPiece.preventSnapUntilMoved > 0)
			{
				return false;
			}
			BuilderPiece.State stateB = otherPiece.state;
			if (otherPiece.isBuiltIntoTable && !otherPiece.isArmShelf)
			{
				stateB = BuilderPiece.State.AttachedAndPlaced;
			}
			return BuilderTable.AreStatesCompatibleForOverlap(stateWhenPlaced, stateB, rootWhenPlaced, rootPiece);
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x001E33D5 File Offset: 0x001E15D5
		public void TryDropPiece(bool leftHand, BuilderPiece testPiece, Vector3 velocity, Vector3 angVelocity)
		{
			if (this == null)
			{
				return;
			}
			if (testPiece == null)
			{
				return;
			}
			this.RequestDropPiece(testPiece, testPiece.transform.position, testPiece.transform.rotation, velocity, angVelocity);
		}

		// Token: 0x06005E90 RID: 24208 RVA: 0x001E340C File Offset: 0x001E160C
		public bool TryPlacePieceGridPlanesOnTableInternal(BuilderPiece testPiece, int recurse, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			bool result = false;
			bool flag = false;
			if (testPiece != null && testPiece.gridPlanes != null && testPiece.gridPlanes.Count > 0 && testPiece.gridPlanes != null)
			{
				for (int i = 0; i < testPiece.gridPlanes.Count; i++)
				{
					List<BuilderAttachGridPlane> checkGridPlanes = testPiece.gridPlanes[i].male ? checkGridPlanesFemale : checkGridPlanesMale;
					BuilderPotentialPlacement builderPotentialPlacement;
					if (this.TryPlaceGridPlane(testPiece, testPiece.gridPlanes[i], checkGridPlanes, out builderPotentialPlacement))
					{
						if (builderPotentialPlacement.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag = true;
						}
						if (builderPotentialPlacement.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement;
							potentialPlacement.attachIndex = i;
							potentialPlacement.attachPiece = testPiece;
							result = true;
						}
					}
				}
			}
			if (recurse > 0)
			{
				BuilderPiece builderPiece = testPiece.firstChildPiece;
				while (builderPiece != null)
				{
					BuilderPotentialPlacement builderPotentialPlacement2;
					if (this.TryPlacePieceGridPlanesOnTableInternal(builderPiece, recurse - 1, checkGridPlanesMale, checkGridPlanesFemale, out builderPotentialPlacement2))
					{
						if (builderPotentialPlacement2.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag = true;
						}
						if (builderPotentialPlacement2.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement2;
							result = true;
						}
					}
					builderPiece = builderPiece.nextSiblingPiece;
				}
			}
			if (testPiece.preventSnapUntilMoved > 0 && !flag)
			{
				testPiece.preventSnapUntilMoved--;
				this.UpdatePieceData(testPiece);
			}
			return result;
		}

		// Token: 0x06005E91 RID: 24209 RVA: 0x001E3590 File Offset: 0x001E1790
		public void TryPlaceRandomlyOnTable(BuilderPiece piece)
		{
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[Random.Range(0, piece.gridPlanes.Count)];
			List<BuilderAttachGridPlane> list = this.baseGridPlanes;
			int num = Random.Range(0, list.Count);
			int i = 0;
			while (i < list.Count)
			{
				int index = (i + num) % list.Count;
				BuilderAttachGridPlane builderAttachGridPlane2 = list[index];
				if (builderAttachGridPlane2.male != builderAttachGridPlane.male && !(builderAttachGridPlane2.piece == builderAttachGridPlane.piece) && !this.ShareSameRoot(builderAttachGridPlane, builderAttachGridPlane2))
				{
					Vector3 zero = Vector3.zero;
					Quaternion identity = Quaternion.identity;
					BuilderPiece piece2 = builderAttachGridPlane2.piece;
					int attachIndex = builderAttachGridPlane2.attachIndex;
					Transform center = builderAttachGridPlane.center;
					Quaternion quaternion = builderAttachGridPlane2.transform.rotation * Quaternion.Inverse(center.localRotation);
					Vector3 point = piece.transform.InverseTransformPoint(center.position);
					Vector3 a = builderAttachGridPlane2.transform.position - quaternion * point;
					if (piece2 != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = piece2.gridPlanes[attachIndex];
						Vector3 lossyScale = builderAttachGridPlane3.transform.lossyScale;
						Vector3 b = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * Vector3.Scale(a - builderAttachGridPlane3.transform.position, b);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * quaternion;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x06005E92 RID: 24210 RVA: 0x001E374C File Offset: 0x001E194C
		public void UseResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.UseResource(cost.quantities[i]);
			}
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x001E3792 File Offset: 0x001E1992
		private void UseResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] += quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x001E37D4 File Offset: 0x001E19D4
		public void AddResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.AddResource(cost.quantities[i]);
			}
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x001E381A File Offset: 0x001E1A1A
		private void AddResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] -= quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x06005E96 RID: 24214 RVA: 0x001E385C File Offset: 0x001E1A5C
		public bool HasEnoughUnreservedResources(BuilderResources resources)
		{
			if (resources == null)
			{
				return false;
			}
			for (int i = 0; i < resources.quantities.Count; i++)
			{
				if (!this.HasEnoughUnreservedResource(resources.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005E97 RID: 24215 RVA: 0x001E38A4 File Offset: 0x001E1AA4
		public bool HasEnoughUnreservedResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + this.reservedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06005E98 RID: 24216 RVA: 0x001E38FC File Offset: 0x001E1AFC
		public bool HasEnoughResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return false;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				if (!this.HasEnoughResource(cost.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005E99 RID: 24217 RVA: 0x001E3948 File Offset: 0x001E1B48
		public bool HasEnoughResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06005E9A RID: 24218 RVA: 0x001E3984 File Offset: 0x001E1B84
		public int GetAvailableResources(BuilderResourceType type)
		{
			if (type < BuilderResourceType.Basic || type >= BuilderResourceType.Count)
			{
				return 0;
			}
			return this.maxResources[(int)type] - this.usedResources[(int)type];
		}

		// Token: 0x06005E9B RID: 24219 RVA: 0x001E39A4 File Offset: 0x001E1BA4
		private void OnAvailableResourcesChange()
		{
			if (this.isSetup && this.isTableMutable)
			{
				for (int i = 0; i < this.conveyors.Count; i++)
				{
					this.conveyors[i].OnAvailableResourcesChange();
				}
				foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
				{
					builderResourceMeter.OnAvailableResourcesChange();
				}
			}
		}

		// Token: 0x06005E9C RID: 24220 RVA: 0x001E3A2C File Offset: 0x001E1C2C
		public int GetPrivateResourceLimitForType(int type)
		{
			if (this.plotMaxResources == null)
			{
				return 0;
			}
			return this.plotMaxResources[type];
		}

		// Token: 0x06005E9D RID: 24221 RVA: 0x001E3A40 File Offset: 0x001E1C40
		private void WriteVector3(BinaryWriter writer, Vector3 data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
		}

		// Token: 0x06005E9E RID: 24222 RVA: 0x001E3A66 File Offset: 0x001E1C66
		private void WriteQuaternion(BinaryWriter writer, Quaternion data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
			writer.Write(data.w);
		}

		// Token: 0x06005E9F RID: 24223 RVA: 0x001E3A98 File Offset: 0x001E1C98
		private Vector3 ReadVector3(BinaryReader reader)
		{
			Vector3 result;
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			return result;
		}

		// Token: 0x06005EA0 RID: 24224 RVA: 0x001E3AD0 File Offset: 0x001E1CD0
		private Quaternion ReadQuaternion(BinaryReader reader)
		{
			Quaternion result;
			result.x = reader.ReadSingle();
			result.y = reader.ReadSingle();
			result.z = reader.ReadSingle();
			result.w = reader.ReadSingle();
			return result;
		}

		// Token: 0x06005EA1 RID: 24225 RVA: 0x001E3B14 File Offset: 0x001E1D14
		public static int PackPiecePlacement(byte twist, sbyte xOffset, sbyte zOffset)
		{
			int num = (int)(twist & 3);
			int num2 = (int)xOffset + 128;
			int num3 = (int)zOffset + 128;
			return num2 + (num3 << 8) + (num << 16);
		}

		// Token: 0x06005EA2 RID: 24226 RVA: 0x001E3B40 File Offset: 0x001E1D40
		public static void UnpackPiecePlacement(int packed, out byte twist, out sbyte xOffset, out sbyte zOffset)
		{
			int num = packed & 255;
			int num2 = packed >> 8 & 255;
			int num3 = packed >> 16 & 3;
			twist = (byte)num3;
			xOffset = (sbyte)(num - 128);
			zOffset = (sbyte)(num2 - 128);
		}

		// Token: 0x06005EA3 RID: 24227 RVA: 0x001E3B80 File Offset: 0x001E1D80
		private long PackSnapInfo(int attachGridIndex, int otherAttachGridIndex, Vector2Int min, Vector2Int max)
		{
			long num = (long)Mathf.Clamp(attachGridIndex, 0, 31);
			long num2 = (long)Mathf.Clamp(otherAttachGridIndex, 0, 31);
			long num3 = (long)Mathf.Clamp(min.x + 1024, 0, 2047);
			long num4 = (long)Mathf.Clamp(min.y + 1024, 0, 2047);
			long num5 = (long)Mathf.Clamp(max.x + 1024, 0, 2047);
			long num6 = (long)Mathf.Clamp(max.y + 1024, 0, 2047);
			return num + (num2 << 5) + (num3 << 10) + (num4 << 21) + (num5 << 32) + (num6 << 43);
		}

		// Token: 0x06005EA4 RID: 24228 RVA: 0x001E3C24 File Offset: 0x001E1E24
		private void UnpackSnapInfo(long packed, out int attachGridIndex, out int otherAttachGridIndex, out Vector2Int min, out Vector2Int max)
		{
			long num = packed & 31L;
			attachGridIndex = (int)num;
			num = (packed >> 5 & 31L);
			otherAttachGridIndex = (int)num;
			int x = (int)(packed >> 10 & 2047L) - 1024;
			int y = (int)(packed >> 21 & 2047L) - 1024;
			min = new Vector2Int(x, y);
			int x2 = (int)(packed >> 32 & 2047L) - 1024;
			int y2 = (int)(packed >> 43 & 2047L) - 1024;
			max = new Vector2Int(x2, y2);
		}

		// Token: 0x06005EA5 RID: 24229 RVA: 0x001E3CB1 File Offset: 0x001E1EB1
		private void OnTitleDataUpdate(string key)
		{
			if (key.Equals(this.SharedMapConfigTitleDataKey))
			{
				this.FetchSharedBlocksStartingMapConfig();
			}
		}

		// Token: 0x06005EA6 RID: 24230 RVA: 0x001E3CC7 File Offset: 0x001E1EC7
		private void FetchSharedBlocksStartingMapConfig()
		{
			if (!this.isTableMutable)
			{
				PlayFabTitleDataCache.Instance.GetTitleData(this.SharedMapConfigTitleDataKey, new Action<string>(this.OnGetStartingMapConfigSuccess), new Action<PlayFabError>(this.OnGetStartingMapConfigFail), false);
			}
		}

		// Token: 0x06005EA7 RID: 24231 RVA: 0x001E3CFC File Offset: 0x001E1EFC
		private void OnGetStartingMapConfigSuccess(string result)
		{
			this.ResetStartingMapConfig();
			if (result.IsNullOrEmpty())
			{
				return;
			}
			try
			{
				SharedBlocksManager.StartingMapConfig startingMapConfig = JsonUtility.FromJson<SharedBlocksManager.StartingMapConfig>(result);
				if (startingMapConfig.useMapID)
				{
					if (SharedBlocksManager.IsMapIDValid(startingMapConfig.mapID))
					{
						this.startingMapConfig.useMapID = true;
						this.startingMapConfig.mapID = startingMapConfig.mapID;
					}
					else
					{
						GTDev.LogError<string>(string.Format("BuilderTable {0} OnGetStartingMapConfigSuccess Title Data Default Map Config has Invalid Map ID", this.tableZone), null);
					}
				}
				else
				{
					this.startingMapConfig.pageNumber = Mathf.Max(startingMapConfig.pageNumber, 0);
					this.startingMapConfig.pageSize = Mathf.Max(startingMapConfig.pageSize, 1);
					if (!startingMapConfig.sortMethod.IsNullOrEmpty() && (startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.Top.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.NewlyCreated.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.RecentlyUpdated.ToString())))
					{
						this.startingMapConfig.sortMethod = startingMapConfig.sortMethod;
					}
					else
					{
						GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Unknown sort method " + startingMapConfig.sortMethod, null);
					}
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Exception Deserializing " + ex.Message, null);
			}
		}

		// Token: 0x06005EA8 RID: 24232 RVA: 0x001E3E94 File Offset: 0x001E2094
		private void OnGetStartingMapConfigFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigFail " + error.Error.ToString(), null);
			this.ResetStartingMapConfig();
		}

		// Token: 0x06005EA9 RID: 24233 RVA: 0x001E3ED4 File Offset: 0x001E20D4
		private void ResetStartingMapConfig()
		{
			this.startingMapConfig = new SharedBlocksManager.StartingMapConfig
			{
				pageNumber = 0,
				pageSize = 10,
				sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
				useMapID = false,
				mapID = null
			};
		}

		// Token: 0x06005EAA RID: 24234 RVA: 0x001E3F27 File Offset: 0x001E2127
		private void RequestTableConfiguration()
		{
			SharedBlocksManager.instance.OnGetTableConfiguration += this.OnGetTableConfiguration;
			SharedBlocksManager.instance.RequestTableConfiguration();
		}

		// Token: 0x06005EAB RID: 24235 RVA: 0x001E3F49 File Offset: 0x001E2149
		private void OnGetTableConfiguration(string configString)
		{
			SharedBlocksManager.instance.OnGetTableConfiguration -= this.OnGetTableConfiguration;
			if (!configString.IsNullOrEmpty())
			{
				this.ParseTableConfiguration(configString);
			}
		}

		// Token: 0x06005EAC RID: 24236 RVA: 0x001E3F70 File Offset: 0x001E2170
		private void ParseTableConfiguration(string dataRecord)
		{
			if (string.IsNullOrEmpty(dataRecord))
			{
				return;
			}
			BuilderTableConfiguration builderTableConfiguration = JsonUtility.FromJson<BuilderTableConfiguration>(dataRecord);
			if (builderTableConfiguration != null)
			{
				if (builderTableConfiguration.TableResourceLimits != null)
				{
					for (int i = 0; i < builderTableConfiguration.TableResourceLimits.Length; i++)
					{
						int num = builderTableConfiguration.TableResourceLimits[i];
						if (num >= 0)
						{
							this.maxResources[i] = num;
						}
					}
				}
				if (builderTableConfiguration.PlotResourceLimits != null)
				{
					for (int j = 0; j < builderTableConfiguration.PlotResourceLimits.Length; j++)
					{
						int num2 = builderTableConfiguration.PlotResourceLimits[j];
						if (num2 >= 0)
						{
							this.plotMaxResources[j] = num2;
						}
					}
				}
				int droppedPieceLimit = builderTableConfiguration.DroppedPieceLimit;
				if (droppedPieceLimit >= 0)
				{
					BuilderTable.DROPPED_PIECE_LIMIT = droppedPieceLimit;
				}
				if (builderTableConfiguration.updateCountdownDate != null && !string.IsNullOrEmpty(builderTableConfiguration.updateCountdownDate))
				{
					try
					{
						DateTime.Parse(builderTableConfiguration.updateCountdownDate, CultureInfo.InvariantCulture);
						BuilderTable.nextUpdateOverride = builderTableConfiguration.updateCountdownDate;
						goto IL_DC;
					}
					catch
					{
						BuilderTable.nextUpdateOverride = string.Empty;
						goto IL_DC;
					}
				}
				BuilderTable.nextUpdateOverride = string.Empty;
				IL_DC:
				this.OnAvailableResourcesChange();
				UnityEvent onTableConfigurationUpdated = this.OnTableConfigurationUpdated;
				if (onTableConfigurationUpdated == null)
				{
					return;
				}
				onTableConfigurationUpdated.Invoke();
			}
		}

		// Token: 0x06005EAD RID: 24237 RVA: 0x001E4080 File Offset: 0x001E2280
		private void DumpTableConfig()
		{
			BuilderTableConfiguration builderTableConfiguration = new BuilderTableConfiguration();
			Array.Clear(builderTableConfiguration.TableResourceLimits, 0, builderTableConfiguration.TableResourceLimits.Length);
			Array.Clear(builderTableConfiguration.PlotResourceLimits, 0, builderTableConfiguration.PlotResourceLimits.Length);
			foreach (BuilderResourceQuantity builderResourceQuantity in this.totalResources.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < (BuilderResourceType)builderTableConfiguration.TableResourceLimits.Length)
				{
					builderTableConfiguration.TableResourceLimits[(int)builderResourceQuantity.type] = builderResourceQuantity.count;
				}
			}
			foreach (BuilderResourceQuantity builderResourceQuantity2 in this.resourcesPerPrivatePlot.quantities)
			{
				if (builderResourceQuantity2.type >= BuilderResourceType.Basic && builderResourceQuantity2.type < (BuilderResourceType)builderTableConfiguration.PlotResourceLimits.Length)
				{
					builderTableConfiguration.PlotResourceLimits[(int)builderResourceQuantity2.type] = builderResourceQuantity2.count;
				}
			}
			builderTableConfiguration.DroppedPieceLimit = BuilderTable.DROPPED_PIECE_LIMIT;
			builderTableConfiguration.updateCountdownDate = "1/10/2025 16:00:00";
			string str = JsonUtility.ToJson(builderTableConfiguration);
			Debug.Log("Configuration Dump \n" + str);
		}

		// Token: 0x06005EAE RID: 24238 RVA: 0x001E41CC File Offset: 0x001E23CC
		private string GetSaveDataTimeKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2") + "Time";
		}

		// Token: 0x06005EAF RID: 24239 RVA: 0x001E41E9 File Offset: 0x001E23E9
		private string GetSaveDataKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2");
		}

		// Token: 0x06005EB0 RID: 24240 RVA: 0x001E4201 File Offset: 0x001E2401
		public void FindAndLoadSharedBlocksMap(string mapID)
		{
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundSharedBlocksMap));
		}

		// Token: 0x06005EB1 RID: 24241 RVA: 0x001E421A File Offset: 0x001E241A
		public string GetSharedBlocksMapID()
		{
			if (this.sharedBlocksMap != null)
			{
				return this.sharedBlocksMap.MapID;
			}
			return string.Empty;
		}

		// Token: 0x06005EB2 RID: 24242 RVA: 0x001E4238 File Offset: 0x001E2438
		private void FoundSharedBlocksMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (map == null || map.MapData.IsNullOrEmpty())
			{
				this.builderNetworking.LoadSharedBlocksFailedMaster((map == null) ? string.Empty : map.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			this.sharedBlocksMap = map;
			this.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
		}

		// Token: 0x06005EB3 RID: 24243 RVA: 0x001E42B4 File Offset: 0x001E24B4
		private void BuildInitialTableForPlayer()
		{
			if (NetworkSystem.Instance.IsNull() || !NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate || NetworkSystem.Instance.GetLocalPlayer() == null || !NetworkSystem.Instance.IsMasterClient)
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			SharedBlocksManager.instance.OnFetchPrivateScanComplete += this.OnFetchPrivateScanComplete;
			SharedBlocksManager.instance.RequestFetchPrivateScan(this.currentSaveSlot);
		}

		// Token: 0x06005EB4 RID: 24244 RVA: 0x001E4340 File Offset: 0x001E2540
		private void OnFetchPrivateScanComplete(int slot, bool success)
		{
			SharedBlocksManager.instance.OnFetchPrivateScanComplete -= this.OnFetchPrivateScanComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			string tableJson;
			if (!success || !SharedBlocksManager.instance.TryGetPrivateScanResponse(slot, out tableJson))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!this.BuildTableFromJson(tableJson, false))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			this.SetIsDirty(false);
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x06005EB5 RID: 24245 RVA: 0x001E43A4 File Offset: 0x001E25A4
		private void BuildSelectedSharedMap()
		{
			if (!NetworkSystem.Instance.IsNull() && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				if (this.sharedBlocksMap != null && !this.sharedBlocksMap.MapData.IsNullOrEmpty())
				{
					this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
					return;
				}
				if (SharedBlocksManager.IsMapIDValid(this.pendingMapID))
				{
					SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = this.pendingMapID
					};
					this.LoadSharedMap(map);
					return;
				}
				this.FindStartingMap();
			}
		}

		// Token: 0x06005EB6 RID: 24246 RVA: 0x001E4430 File Offset: 0x001E2630
		private void FindStartingMap()
		{
			if (this.hasStartingMap && Time.timeAsDouble < this.startingMapCacheTime + 60.0)
			{
				this.FoundDefaultSharedBlocksMap(true, this.startingMap);
				return;
			}
			if (this.getStartingMapInProgress)
			{
				return;
			}
			this.hasStartingMap = false;
			this.getStartingMapInProgress = true;
			if (this.startingMapConfig.useMapID && SharedBlocksManager.IsMapIDValid(this.startingMapConfig.mapID))
			{
				this.startingMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = this.startingMapConfig.mapID
				};
				SharedBlocksManager.instance.RequestMapDataFromID(this.startingMapConfig.mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
				return;
			}
			if (this.hasCachedTopMaps && Time.timeAsDouble <= this.lastGetTopMapsTime + 60.0)
			{
				this.ChooseMapFromList();
				return;
			}
			SharedBlocksManager.instance.OnGetPopularMapsComplete += this.FoundStartingMapList;
			if (!SharedBlocksManager.instance.RequestGetTopMaps(this.startingMapConfig.pageNumber, this.startingMapConfig.pageSize, this.startingMapConfig.sortMethod.ToString()))
			{
				this.FoundStartingMapList(false);
			}
		}

		// Token: 0x06005EB7 RID: 24247 RVA: 0x001E4554 File Offset: 0x001E2754
		private void FoundStartingMapList(bool success)
		{
			SharedBlocksManager.instance.OnGetPopularMapsComplete -= this.FoundStartingMapList;
			if (success && SharedBlocksManager.instance.LatestPopularMaps.Count > 0)
			{
				this.startingMapList.Clear();
				this.startingMapList.AddRange(SharedBlocksManager.instance.LatestPopularMaps);
				this.hasCachedTopMaps = (this.startingMapList.Count > 0);
				this.lastGetTopMapsTime = (double)Time.time;
				this.ChooseMapFromList();
				return;
			}
			this.FoundDefaultSharedBlocksMap(false, null);
		}

		// Token: 0x06005EB8 RID: 24248 RVA: 0x001E45DC File Offset: 0x001E27DC
		private void ChooseMapFromList()
		{
			int index = Random.Range(0, this.startingMapList.Count);
			this.startingMap = this.startingMapList[index];
			if (this.startingMap == null || !SharedBlocksManager.IsMapIDValid(this.startingMap.MapID))
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			SharedBlocksManager.instance.RequestMapDataFromID(this.startingMap.MapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
		}

		// Token: 0x06005EB9 RID: 24249 RVA: 0x001E4654 File Offset: 0x001E2854
		private void FoundTopMapData(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null || !SharedBlocksManager.IsMapIDValid(map.MapID) || map.MapID != this.startingMap.MapID)
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			this.hasStartingMap = true;
			this.startingMapCacheTime = Time.timeAsDouble;
			this.startingMap.MapData = map.MapData;
			this.FoundDefaultSharedBlocksMap(true, this.startingMap);
		}

		// Token: 0x06005EBA RID: 24250 RVA: 0x001E46C4 File Offset: 0x001E28C4
		private void FoundDefaultSharedBlocksMap(bool success, SharedBlocksManager.SharedBlocksMap map)
		{
			this.getStartingMapInProgress = false;
			if (success && !map.MapData.IsNullOrEmpty())
			{
				this.startingMapCacheTime = Time.timeAsDouble;
				this.startingMap = map;
				this.hasStartingMap = true;
				this.sharedBlocksMap = map;
				this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
				return;
			}
			this.TryBuildingFromTitleData();
		}

		// Token: 0x06005EBB RID: 24251 RVA: 0x001E4720 File Offset: 0x001E2920
		private void TryBuildingSharedBlocksMap(string mapData)
		{
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!this.BuildTableFromJson(mapData, true))
			{
				GTDev.LogWarning<string>("Unable to build shared blocks map", null);
				this.builderNetworking.LoadSharedBlocksFailedMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			base.StartCoroutine(this.CheckForNoBlocks());
		}

		// Token: 0x06005EBC RID: 24252 RVA: 0x001E4795 File Offset: 0x001E2995
		private IEnumerator CheckForNoBlocks()
		{
			yield return null;
			if (!this.NoBlocksCheck())
			{
				GTDev.LogError<string>("Failed No Blocks Check", null);
				this.builderNetworking.SharedBlocksOutOfBoundsMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				yield break;
			}
			this.OnFinishedInitialTableBuild();
			yield break;
		}

		// Token: 0x06005EBD RID: 24253 RVA: 0x001E47A4 File Offset: 0x001E29A4
		private void TryBuildingFromTitleData()
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete += this.OnGetTitleDataBuildComplete;
			SharedBlocksManager.instance.FetchTitleDataBuild();
		}

		// Token: 0x06005EBE RID: 24254 RVA: 0x001E47C8 File Offset: 0x001E29C8
		private void OnGetTitleDataBuildComplete(string titleDataBuild)
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete -= this.OnGetTitleDataBuildComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!titleDataBuild.IsNullOrEmpty())
			{
				if (!this.BuildTableFromJson(titleDataBuild, true))
				{
					this.tableData = new BuilderTableData();
				}
			}
			else
			{
				this.tableData = new BuilderTableData();
			}
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x06005EBF RID: 24255 RVA: 0x001E4828 File Offset: 0x001E2A28
		public void SaveTableForPlayer(string busyStr, string blocksErrStr)
		{
			if (SharedBlocksManager.instance.IsWaitingOnRequest())
			{
				this.SetIsDirty(true);
				UnityEvent<string> onSaveFailure = this.OnSaveFailure;
				if (onSaveFailure == null)
				{
					return;
				}
				onSaveFailure.Invoke(busyStr);
				return;
			}
			else
			{
				this.saveInProgress = true;
				if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
				{
					this.saveInProgress = false;
					return;
				}
				if (!this.isDirty)
				{
					this.saveInProgress = false;
					UnityEvent onSaveTimeUpdated = this.OnSaveTimeUpdated;
					if (onSaveTimeUpdated == null)
					{
						return;
					}
					onSaveTimeUpdated.Invoke();
					return;
				}
				else
				{
					if (this.NoBlocksCheck())
					{
						if (this.tableData == null)
						{
							this.tableData = new BuilderTableData();
						}
						this.SetIsDirty(false);
						this.tableData.numEdits++;
						string text = this.WriteTableToJson();
						text = Convert.ToBase64String(GZipStream.CompressString(text));
						SharedBlocksManager.instance.OnSavePrivateScanSuccess += this.OnSaveScanSuccess;
						SharedBlocksManager.instance.OnSavePrivateScanFailed += this.OnSaveScanFailure;
						SharedBlocksManager.instance.RequestSavePrivateScan(this.currentSaveSlot, text);
						return;
					}
					this.saveInProgress = false;
					this.SetIsDirty(true);
					UnityEvent<string> onSaveFailure2 = this.OnSaveFailure;
					if (onSaveFailure2 == null)
					{
						return;
					}
					onSaveFailure2.Invoke(blocksErrStr);
					return;
				}
			}
		}

		// Token: 0x06005EC0 RID: 24256 RVA: 0x001E493C File Offset: 0x001E2B3C
		private void OnSaveScanSuccess(int scan)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			UnityEvent onSaveSuccess = this.OnSaveSuccess;
			if (onSaveSuccess == null)
			{
				return;
			}
			onSaveSuccess.Invoke();
		}

		// Token: 0x06005EC1 RID: 24257 RVA: 0x001E498C File Offset: 0x001E2B8C
		private void OnSaveScanFailure(int scan, string message)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			this.SetIsDirty(true);
			UnityEvent<string> onSaveFailure = this.OnSaveFailure;
			if (onSaveFailure == null)
			{
				return;
			}
			onSaveFailure.Invoke(message);
		}

		// Token: 0x06005EC2 RID: 24258 RVA: 0x001E49E4 File Offset: 0x001E2BE4
		private string WriteTableToJson()
		{
			this.tableData.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.tableData.pieceType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedPieceType : this.pieces[i].pieceType);
					this.tableData.pieceId.Add(this.pieces[i].pieceId);
					this.tableData.parentId.Add((this.pieces[i].parentPiece == null) ? -1 : this.pieces[i].parentPiece.pieceId);
					this.tableData.attachIndex.Add(this.pieces[i].attachIndex);
					this.tableData.parentAttachIndex.Add((this.pieces[i].parentPiece == null) ? -1 : this.pieces[i].parentAttachIndex);
					this.tableData.placement.Add(this.pieces[i].GetPiecePlacement());
					this.tableData.materialType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedMaterialType : this.pieces[i].materialType);
					BuilderMovingSnapPiece component = this.pieces[i].GetComponent<BuilderMovingSnapPiece>();
					int item = (component == null) ? 0 : component.GetTimeOffset();
					this.tableData.timeOffset.Add(item);
					for (int j = 0; j < this.pieces[i].gridPlanes.Count; j++)
					{
						if (!(this.pieces[i].gridPlanes[j] == null))
						{
							for (SnapOverlap snapOverlap = this.pieces[i].gridPlanes[j].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								if (snapOverlap.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey item2 = BuilderTable.BuildOverlapKey(this.pieces[i].pieceId, snapOverlap.otherPlane.piece.pieceId, j, snapOverlap.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(item2))
									{
										BuilderTable.tempDuplicateOverlaps.Add(item2);
										long item3 = this.PackSnapInfo(j, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
										this.tableData.overlapingPieces.Add(this.pieces[i].pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(item3);
									}
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				if (!(builderPiece == null))
				{
					for (int k = 0; k < builderPiece.gridPlanes.Count; k++)
					{
						if (!(builderPiece.gridPlanes[k] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece.gridPlanes[k].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								if (snapOverlap2.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap2.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey item4 = BuilderTable.BuildOverlapKey(builderPiece.pieceId, snapOverlap2.otherPlane.piece.pieceId, k, snapOverlap2.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(item4))
									{
										BuilderTable.tempDuplicateOverlaps.Add(item4);
										long item5 = this.PackSnapInfo(k, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
										this.tableData.overlapingPieces.Add(builderPiece.pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(item5);
									}
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			this.tableData.numPieces = this.tableData.pieceType.Count;
			return JsonUtility.ToJson(this.tableData);
		}

		// Token: 0x06005EC3 RID: 24259 RVA: 0x001E4F38 File Offset: 0x001E3138
		private static BuilderTable.SnapOverlapKey BuildOverlapKey(int pieceId, int otherPieceId, int attachGridIndex, int otherAttachGridIndex)
		{
			BuilderTable.SnapOverlapKey result = default(BuilderTable.SnapOverlapKey);
			result.piece = (long)pieceId;
			result.piece <<= 32;
			result.piece |= (long)attachGridIndex;
			result.otherPiece = (long)otherPieceId;
			result.otherPiece <<= 32;
			result.otherPiece |= (long)otherAttachGridIndex;
			return result;
		}

		// Token: 0x06005EC4 RID: 24260 RVA: 0x001E4F94 File Offset: 0x001E3194
		private bool BuildTableFromJson(string tableJson, bool fromTitleData)
		{
			if (string.IsNullOrEmpty(tableJson))
			{
				return false;
			}
			this.tableData = null;
			try
			{
				this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
			}
			catch
			{
			}
			try
			{
				if (this.tableData == null)
				{
					tableJson = GZipStream.UncompressString(Convert.FromBase64String(tableJson));
					this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
			if (this.tableData == null)
			{
				return false;
			}
			if (this.tableData.version < 4)
			{
				return false;
			}
			int num = (this.tableData.pieceType == null) ? 0 : this.tableData.pieceType.Count;
			if (num == 0)
			{
				this.OnDeserializeUpdatePlots();
				return true;
			}
			if (this.tableData.pieceId == null || this.tableData.pieceId.Count != num || this.tableData.placement == null || this.tableData.placement.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch", null);
				return false;
			}
			if (num >= this.maxResources[0])
			{
				GTDev.LogError<string>(string.Format("BuildTableFromJson Failed sanity piece count check {0}", num), null);
				return false;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>(num);
			bool flag = this.tableData.timeOffset != null && this.tableData.timeOffset.Count > 0;
			if (flag && this.tableData.timeOffset.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch (Time Offsets)", null);
				return false;
			}
			int i = 0;
			while (i < this.tableData.pieceType.Count)
			{
				int num2 = this.CreatePieceId();
				if (!dictionary.TryAdd(this.tableData.pieceId[i], num2))
				{
					GTDev.LogError<string>("BuildTableFromJson Piece id duplicate in save", null);
					this.ClearTable();
					return false;
				}
				int num3 = (this.tableData.materialType != null && this.tableData.materialType.Count > i) ? this.tableData.materialType[i] : -1;
				int newPieceType = this.tableData.pieceType[i];
				int num4 = num3;
				bool flag2 = true;
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.tableData.pieceType[i]);
				if (piecePrefab == null)
				{
					this.ClearTable();
					return false;
				}
				if (fromTitleData)
				{
					goto IL_2B2;
				}
				if (num4 == -1 && piecePrefab.materialOptions != null)
				{
					int num5;
					Material material;
					int num6;
					piecePrefab.materialOptions.GetDefaultMaterial(out num5, out material, out num6);
					num4 = num5;
				}
				flag2 = BuilderSetManager.instance.IsPieceOwnedLocally(this.tableData.pieceType[i], num4);
				if (!fromTitleData && !flag2)
				{
					if (!piecePrefab.fallbackInfo.materialSwapThisPrefab)
					{
						if (piecePrefab.fallbackInfo.prefab == null)
						{
							goto IL_3E0;
						}
						newPieceType = piecePrefab.fallbackInfo.prefab.name.GetStaticHash();
					}
					num4 = -1;
				}
				goto IL_2B2;
				IL_3E0:
				i++;
				continue;
				IL_2B2:
				if (piecePrefab.cost != null && piecePrefab.cost.quantities != null)
				{
					for (int j = 0; j < piecePrefab.cost.quantities.Count; j++)
					{
						BuilderResourceQuantity builderResourceQuantity = piecePrefab.cost.quantities[j];
						if (!this.HasEnoughResource(builderResourceQuantity))
						{
							if (builderResourceQuantity.type == BuilderResourceType.Basic)
							{
								this.ClearTable();
								GTDev.LogError<string>("BuildTableFromJson saved table uses too many basic resource", null);
								return false;
							}
							GTDev.LogWarning<string>("BuildTableFromJson saved table uses too many functional or decorative resource", null);
						}
					}
				}
				int num7 = flag ? this.tableData.timeOffset[i] : 0;
				BuilderPiece builderPiece = this.CreatePieceInternal(newPieceType, num2, Vector3.zero, Quaternion.identity, BuilderPiece.State.AttachedAndPlaced, num4, NetworkSystem.Instance.ServerTimestamp - num7, this);
				if (builderPiece == null)
				{
					this.ClearTable();
					GTDev.LogError<string>(string.Format("Piece Type {0} is not defined", this.tableData.pieceType[i]), null);
					return false;
				}
				if (!fromTitleData && !flag2)
				{
					builderPiece.overrideSavedPiece = true;
					builderPiece.savedPieceType = this.tableData.pieceType[i];
					builderPiece.savedMaterialType = num3;
				}
				goto IL_3E0;
			}
			for (int k = 0; k < this.tableData.pieceType.Count; k++)
			{
				int parentAttachIndex = (this.tableData.parentAttachIndex == null || this.tableData.parentAttachIndex.Count <= k) ? -1 : this.tableData.parentAttachIndex[k];
				int attachIndex = (this.tableData.attachIndex == null || this.tableData.attachIndex.Count <= k) ? -1 : this.tableData.attachIndex[k];
				int valueOrDefault = dictionary.GetValueOrDefault(this.tableData.pieceId[k], -1);
				int parentId = -1;
				int num8;
				if (dictionary.TryGetValue(this.tableData.parentId[k], out num8))
				{
					parentId = num8;
				}
				else if (this.tableData.parentId[k] < 10000 && this.tableData.parentId[k] >= 5)
				{
					parentId = this.tableData.parentId[k];
				}
				this.AttachPieceInternal(valueOrDefault, attachIndex, parentId, parentAttachIndex, this.tableData.placement[k]);
			}
			foreach (BuilderPiece builderPiece2 in this.pieces)
			{
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece2.OnPlacementDeserialized();
				}
			}
			this.OnDeserializeUpdatePlots();
			BuilderTable.tempDuplicateOverlaps.Clear();
			if (this.tableData.overlapingPieces != null)
			{
				int num9 = 0;
				while (num9 < this.tableData.overlapingPieces.Count && num9 < this.tableData.overlappedPieces.Count && num9 < this.tableData.overlapInfo.Count)
				{
					int num10 = -1;
					int num11;
					if (dictionary.TryGetValue(this.tableData.overlapingPieces[num9], out num11))
					{
						num10 = num11;
					}
					else if (this.tableData.overlapingPieces[num9] < 10000 && this.tableData.overlapingPieces[num9] >= 5)
					{
						num10 = this.tableData.overlapingPieces[num9];
					}
					int num12 = -1;
					int num13;
					if (dictionary.TryGetValue(this.tableData.overlappedPieces[num9], out num13))
					{
						num12 = num13;
					}
					else if (this.tableData.overlappedPieces[num9] < 10000 && this.tableData.overlappedPieces[num9] >= 5)
					{
						num12 = this.tableData.overlappedPieces[num9];
					}
					if (num10 != -1 && num12 != -1)
					{
						long packed = this.tableData.overlapInfo[num9];
						BuilderPiece piece = this.GetPiece(num10);
						if (!(piece == null))
						{
							BuilderPiece piece2 = this.GetPiece(num12);
							if (!(piece2 == null))
							{
								int num14;
								int num15;
								Vector2Int min;
								Vector2Int max;
								this.UnpackSnapInfo(packed, out num14, out num15, out min, out max);
								if (num14 >= 0 && num14 < piece.gridPlanes.Count && num15 >= 0 && num15 < piece2.gridPlanes.Count)
								{
									BuilderTable.SnapOverlapKey item = BuilderTable.BuildOverlapKey(num10, num12, num14, num15);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(item))
									{
										BuilderTable.tempDuplicateOverlaps.Add(item);
										piece.gridPlanes[num14].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num15], new SnapBounds(min, max)));
									}
								}
							}
						}
					}
					num9++;
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			return true;
		}

		// Token: 0x06005EC5 RID: 24261 RVA: 0x001E577C File Offset: 0x001E397C
		public int SerializeTableState(byte[] bytes, int maxBytes)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.conveyors == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.conveyors.Count);
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					int selectedDisplayGroupID = builderConveyor.GetSelectedDisplayGroupID();
					binaryWriter.Write(selectedDisplayGroupID);
				}
			}
			if (this.dispenserShelves == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.dispenserShelves.Count);
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					int selectedDisplayGroupID2 = builderDispenserShelf.GetSelectedDisplayGroupID();
					binaryWriter.Write(selectedDisplayGroupID2);
				}
			}
			BuilderTable.childPieces.Clear();
			BuilderTable.rootPieces.Clear();
			BuilderTable.childPieces.EnsureCapacity(this.pieces.Count);
			BuilderTable.rootPieces.EnsureCapacity(this.pieces.Count);
			foreach (BuilderPiece builderPiece in this.pieces)
			{
				if (builderPiece.parentPiece == null)
				{
					BuilderTable.rootPieces.Add(builderPiece);
				}
				else
				{
					BuilderTable.childPieces.Add(builderPiece);
				}
			}
			binaryWriter.Write(BuilderTable.rootPieces.Count);
			for (int i = 0; i < BuilderTable.rootPieces.Count; i++)
			{
				BuilderPiece builderPiece2 = BuilderTable.rootPieces[i];
				binaryWriter.Write(builderPiece2.pieceType);
				binaryWriter.Write(builderPiece2.pieceId);
				binaryWriter.Write((byte)builderPiece2.state);
				if (builderPiece2.state == BuilderPiece.State.OnConveyor || builderPiece2.state == BuilderPiece.State.OnShelf || builderPiece2.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece2.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece2.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece2.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece2.materialType);
				long value = BitPackUtils.PackWorldPosForNetwork(builderPiece2.transform.localPosition);
				int value2 = BitPackUtils.PackQuaternionForNetwork(builderPiece2.transform.localRotation);
				binaryWriter.Write(value);
				binaryWriter.Write(value2);
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece2.functionalPieceState);
					binaryWriter.Write(builderPiece2.activatedTimeStamp);
				}
				if (builderPiece2.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece2));
				}
			}
			binaryWriter.Write(BuilderTable.childPieces.Count);
			for (int j = 0; j < BuilderTable.childPieces.Count; j++)
			{
				BuilderPiece builderPiece3 = BuilderTable.childPieces[j];
				binaryWriter.Write(builderPiece3.pieceType);
				binaryWriter.Write(builderPiece3.pieceId);
				int value3 = (builderPiece3.parentPiece == null) ? -1 : builderPiece3.parentPiece.pieceId;
				binaryWriter.Write(value3);
				binaryWriter.Write(builderPiece3.attachIndex);
				binaryWriter.Write(builderPiece3.parentAttachIndex);
				binaryWriter.Write((byte)builderPiece3.state);
				if (builderPiece3.state == BuilderPiece.State.OnConveyor || builderPiece3.state == BuilderPiece.State.OnShelf || builderPiece3.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece3.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece3.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece3.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece3.materialType);
				int piecePlacement = builderPiece3.GetPiecePlacement();
				binaryWriter.Write(piecePlacement);
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece3.functionalPieceState);
					binaryWriter.Write(builderPiece3.activatedTimeStamp);
				}
				if (builderPiece3.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece3));
				}
			}
			if (this.isTableMutable)
			{
				binaryWriter.Write(this.plotOwners.Count);
				using (Dictionary<int, int>.Enumerator enumerator4 = this.plotOwners.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<int, int> keyValuePair = enumerator4.Current;
						binaryWriter.Write(keyValuePair.Key);
						binaryWriter.Write(keyValuePair.Value);
					}
					goto IL_4F9;
				}
			}
			if (this.sharedBlocksMap == null || this.sharedBlocksMap.MapID == null || !SharedBlocksManager.IsMapIDValid(this.sharedBlocksMap.MapID))
			{
				for (int k = 0; k < BuilderTable.mapIDBuffer.Length; k++)
				{
					BuilderTable.mapIDBuffer[k] = 'a';
				}
			}
			else
			{
				for (int l = 0; l < BuilderTable.mapIDBuffer.Length; l++)
				{
					BuilderTable.mapIDBuffer[l] = this.sharedBlocksMap.MapID[l];
				}
			}
			binaryWriter.Write(BuilderTable.mapIDBuffer);
			IL_4F9:
			long position = memoryStream.Position;
			BuilderTable.overlapPieces.Clear();
			BuilderTable.overlapOtherPieces.Clear();
			BuilderTable.overlapPacked.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			foreach (BuilderPiece builderPiece4 in this.pieces)
			{
				if (!(builderPiece4 == null))
				{
					for (int m = 0; m < builderPiece4.gridPlanes.Count; m++)
					{
						if (!(builderPiece4.gridPlanes[m] == null))
						{
							for (SnapOverlap snapOverlap = builderPiece4.gridPlanes[m].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								BuilderTable.SnapOverlapKey item = BuilderTable.BuildOverlapKey(builderPiece4.pieceId, snapOverlap.otherPlane.piece.pieceId, m, snapOverlap.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(item))
								{
									BuilderTable.tempDuplicateOverlaps.Add(item);
									long item2 = this.PackSnapInfo(m, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece4.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(item2);
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece5 in this.basePieces)
			{
				if (!(builderPiece5 == null))
				{
					for (int n = 0; n < builderPiece5.gridPlanes.Count; n++)
					{
						if (!(builderPiece5.gridPlanes[n] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece5.gridPlanes[n].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								BuilderTable.SnapOverlapKey item3 = BuilderTable.BuildOverlapKey(builderPiece5.pieceId, snapOverlap2.otherPlane.piece.pieceId, n, snapOverlap2.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(item3))
								{
									BuilderTable.tempDuplicateOverlaps.Add(item3);
									long item4 = this.PackSnapInfo(n, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece5.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(item4);
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			binaryWriter.Write(BuilderTable.overlapPieces.Count);
			for (int num = 0; num < BuilderTable.overlapPieces.Count; num++)
			{
				binaryWriter.Write(BuilderTable.overlapPieces[num]);
				binaryWriter.Write(BuilderTable.overlapOtherPieces[num]);
				binaryWriter.Write(BuilderTable.overlapPacked[num]);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x06005EC6 RID: 24262 RVA: 0x001E6064 File Offset: 0x001E4264
		public void DeserializeTableState(byte[] bytes, int numBytes)
		{
			if (numBytes <= 0)
			{
				return;
			}
			Vector3 position;
			Quaternion rotation;
			VRRigCache.Instance.localRig.SpeakerHead.transform.GetPositionAndRotation(out position, out rotation);
			bool flag = this.ValidatePieceWorldTransform(position, rotation);
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentPeiceIds.Clear();
			BuilderTable.tempAttachIndexes.Clear();
			BuilderTable.tempParentAttachIndexes.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			BuilderTable.tempPiecePlacement.Clear();
			int num = binaryReader.ReadInt32();
			bool flag2 = this.conveyors != null;
			for (int i = 0; i < num; i++)
			{
				int selection = binaryReader.ReadInt32();
				if (flag2 && i < this.conveyors.Count)
				{
					this.conveyors[i].SetSelection(selection);
				}
			}
			int num2 = binaryReader.ReadInt32();
			bool flag3 = this.dispenserShelves != null;
			for (int j = 0; j < num2; j++)
			{
				int selection2 = binaryReader.ReadInt32();
				if (flag3 && j < this.dispenserShelves.Count)
				{
					this.dispenserShelves[j].SetSelection(selection2);
				}
			}
			int num3 = binaryReader.ReadInt32();
			for (int k = 0; k < num3; k++)
			{
				int newPieceType = binaryReader.ReadInt32();
				int num4 = binaryReader.ReadInt32();
				BuilderPiece.State state = (BuilderPiece.State)binaryReader.ReadByte();
				int num5 = binaryReader.ReadInt32();
				bool item = binaryReader.ReadByte() > 0;
				int materialType = binaryReader.ReadInt32();
				long data = binaryReader.ReadInt64();
				int data2 = binaryReader.ReadInt32();
				Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(data);
				Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data2);
				byte fState = (state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0;
				int activateTimeStamp = (state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0;
				int num6 = (state == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0;
				float num7 = 10000f;
				if (!vector.IsValid(num7) || !quaternion.IsValid() || !this.ValidateCreatePieceParams(newPieceType, num4, state, materialType))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num8 = -1;
				if (state == BuilderPiece.State.OnConveyor || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
				{
					num8 = num5;
					num5 = -1;
				}
				if ((num5 != actorNumber || flag) && this.ValidateDeserializedRootPieceState(num4, state, num8, num5, vector, quaternion))
				{
					BuilderPiece builderPiece = this.CreatePieceInternal(newPieceType, num4, vector, quaternion, state, materialType, activateTimeStamp, this);
					BuilderTable.tempPeiceIds.Add(num4);
					BuilderTable.tempParentActorNumbers.Add(num5);
					BuilderTable.tempInLeftHand.Add(item);
					builderPiece.SetFunctionalPieceState(fState, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					if (num8 >= 0 && this.isTableMutable)
					{
						builderPiece.shelfOwner = num8;
						if (state == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor = this.conveyors[num8];
							float timeOffset = 0f;
							if (PhotonNetwork.ServerTimestamp > num6)
							{
								timeOffset = (PhotonNetwork.ServerTimestamp - num6) / 1000f;
							}
							builderConveyor.OnShelfPieceCreated(builderPiece, timeOffset);
						}
						else if (state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num8].OnShelfPieceCreated(builderPiece, false);
						}
					}
				}
			}
			for (int l = 0; l < BuilderTable.tempPeiceIds.Count; l++)
			{
				if (BuilderTable.tempParentActorNumbers[l] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[l], BuilderTable.tempParentActorNumbers[l], BuilderTable.tempInLeftHand[l]);
				}
			}
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			int num9 = binaryReader.ReadInt32();
			for (int m = 0; m < num9; m++)
			{
				int newPieceType2 = binaryReader.ReadInt32();
				int num10 = binaryReader.ReadInt32();
				int item2 = binaryReader.ReadInt32();
				int item3 = binaryReader.ReadInt32();
				int item4 = binaryReader.ReadInt32();
				BuilderPiece.State state2 = (BuilderPiece.State)binaryReader.ReadByte();
				int num11 = binaryReader.ReadInt32();
				bool item5 = binaryReader.ReadByte() > 0;
				int materialType2 = binaryReader.ReadInt32();
				int item6 = binaryReader.ReadInt32();
				byte fState2 = (state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0;
				int activateTimeStamp2 = (state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0;
				int num12 = (state2 == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0;
				if (!this.ValidateCreatePieceParams(newPieceType2, num10, state2, materialType2))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num13 = -1;
				if (state2 == BuilderPiece.State.OnConveyor || state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
				{
					num13 = num11;
					num11 = -1;
				}
				if ((num11 != actorNumber || flag) && this.ValidateDeserializedChildPieceState(num10, state2))
				{
					BuilderPiece builderPiece2 = this.CreatePieceInternal(newPieceType2, num10, this.roomCenter.position, Quaternion.identity, state2, materialType2, activateTimeStamp2, this);
					builderPiece2.SetFunctionalPieceState(fState2, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					BuilderTable.tempPeiceIds.Add(num10);
					BuilderTable.tempParentPeiceIds.Add(item2);
					BuilderTable.tempAttachIndexes.Add(item3);
					BuilderTable.tempParentAttachIndexes.Add(item4);
					BuilderTable.tempParentActorNumbers.Add(num11);
					BuilderTable.tempInLeftHand.Add(item5);
					BuilderTable.tempPiecePlacement.Add(item6);
					if (num13 >= 0 && this.isTableMutable)
					{
						builderPiece2.shelfOwner = num13;
						if (state2 == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor2 = this.conveyors[num13];
							float timeOffset2 = 0f;
							if (PhotonNetwork.ServerTimestamp > num12)
							{
								timeOffset2 = (PhotonNetwork.ServerTimestamp - num12) / 1000f;
							}
							builderConveyor2.OnShelfPieceCreated(builderPiece2, timeOffset2);
						}
						else if (state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num13].OnShelfPieceCreated(builderPiece2, false);
						}
					}
				}
			}
			for (int n = 0; n < BuilderTable.tempPeiceIds.Count; n++)
			{
				if (!this.ValidateAttachPieceParams(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]))
				{
					this.RecyclePieceInternal(BuilderTable.tempPeiceIds[n], true, false, -1);
				}
				else
				{
					this.AttachPieceInternal(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]);
				}
			}
			for (int num14 = 0; num14 < BuilderTable.tempPeiceIds.Count; num14++)
			{
				if (BuilderTable.tempParentActorNumbers[num14] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[num14], BuilderTable.tempParentActorNumbers[num14], BuilderTable.tempInLeftHand[num14]);
				}
			}
			foreach (BuilderPiece builderPiece3 in this.pieces)
			{
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece3.OnPlacementDeserialized();
				}
			}
			if (this.isTableMutable)
			{
				this.plotOwners.Clear();
				this.doesLocalPlayerOwnPlot = false;
				int num15 = binaryReader.ReadInt32();
				for (int num16 = 0; num16 < num15; num16++)
				{
					int num17 = binaryReader.ReadInt32();
					int num18 = binaryReader.ReadInt32();
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (this.plotOwners.TryAdd(num17, num18) && this.GetPiece(num18).TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						builderPiecePrivatePlot.ClaimPlotForPlayerNumber(num17);
						if (num17 == PhotonNetwork.LocalPlayer.ActorNumber)
						{
							this.doesLocalPlayerOwnPlot = true;
						}
					}
				}
				UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
				if (onLocalPlayerClaimedPlot != null)
				{
					onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
				}
				this.OnDeserializeUpdatePlots();
			}
			else
			{
				BuilderTable.mapIDBuffer = binaryReader.ReadChars(BuilderTable.mapIDBuffer.Length);
				string mapID = new string(BuilderTable.mapIDBuffer);
				if (SharedBlocksManager.IsMapIDValid(mapID))
				{
					this.sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = mapID
					};
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			int num19 = binaryReader.ReadInt32();
			for (int num20 = 0; num20 < num19; num20++)
			{
				int pieceId = binaryReader.ReadInt32();
				int num21 = binaryReader.ReadInt32();
				long packed = binaryReader.ReadInt64();
				BuilderPiece piece = this.GetPiece(pieceId);
				if (!(piece == null))
				{
					BuilderPiece piece2 = this.GetPiece(num21);
					if (!(piece2 == null))
					{
						int num22;
						int num23;
						Vector2Int min;
						Vector2Int max;
						this.UnpackSnapInfo(packed, out num22, out num23, out min, out max);
						if (num22 >= 0 && num22 < piece.gridPlanes.Count && num23 >= 0 && num23 < piece2.gridPlanes.Count)
						{
							BuilderTable.SnapOverlapKey item7 = BuilderTable.BuildOverlapKey(pieceId, num21, num22, num23);
							if (!BuilderTable.tempDuplicateOverlaps.Contains(item7))
							{
								BuilderTable.tempDuplicateOverlaps.Add(item7);
								piece.gridPlanes[num22].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num23], new SnapBounds(min, max)));
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
		}

		// Token: 0x04006C81 RID: 27777
		public const GTZone BUILDER_ZONE = GTZone.monkeBlocks;

		// Token: 0x04006C82 RID: 27778
		private const int INITIAL_BUILTIN_PIECE_ID = 5;

		// Token: 0x04006C83 RID: 27779
		private const int INITIAL_CREATED_PIECE_ID = 10000;

		// Token: 0x04006C84 RID: 27780
		public static float MAX_DROP_VELOCITY = 20f;

		// Token: 0x04006C85 RID: 27781
		public static float MAX_DROP_ANG_VELOCITY = 50f;

		// Token: 0x04006C86 RID: 27782
		private const float MAX_DISTANCE_FROM_CENTER = 217f;

		// Token: 0x04006C87 RID: 27783
		private const float MAX_LOCAL_MAGNITUDE = 80f;

		// Token: 0x04006C88 RID: 27784
		public const float MAX_DISTANCE_FROM_HAND = 2.5f;

		// Token: 0x04006C89 RID: 27785
		public static float DROP_ZONE_REPEL = 2.25f;

		// Token: 0x04006C8A RID: 27786
		public static int placedLayer;

		// Token: 0x04006C8B RID: 27787
		public static int heldLayer;

		// Token: 0x04006C8C RID: 27788
		public static int heldLayerLocal;

		// Token: 0x04006C8D RID: 27789
		public static int droppedLayer;

		// Token: 0x04006C8E RID: 27790
		private float acceptableSqrDistFromCenter = 47089f;

		// Token: 0x04006C8F RID: 27791
		public float pieceScale = 0.04f;

		// Token: 0x04006C90 RID: 27792
		public GTZone tableZone = GTZone.monkeBlocks;

		// Token: 0x04006C91 RID: 27793
		[SerializeField]
		private string SharedMapConfigTitleDataKey = "SharedBlocksStartingMapConfig";

		// Token: 0x04006C92 RID: 27794
		public BuilderTableNetworking builderNetworking;

		// Token: 0x04006C93 RID: 27795
		public BuilderRenderer builderRenderer;

		// Token: 0x04006C94 RID: 27796
		[HideInInspector]
		public BuilderPool builderPool;

		// Token: 0x04006C95 RID: 27797
		public Transform tableCenter;

		// Token: 0x04006C96 RID: 27798
		public Transform roomCenter;

		// Token: 0x04006C97 RID: 27799
		public Transform worldCenter;

		// Token: 0x04006C98 RID: 27800
		public GameObject noBlocksArea;

		// Token: 0x04006C99 RID: 27801
		public List<GameObject> builtInPieceRoots;

		// Token: 0x04006C9A RID: 27802
		[Tooltip("Optional terminal to control loaded blocks")]
		public SharedBlocksTerminal linkedTerminal;

		// Token: 0x04006C9B RID: 27803
		[Tooltip("Can Blocks Be Placed and Grabbed")]
		public bool isTableMutable;

		// Token: 0x04006C9C RID: 27804
		public GameObject shelvesRoot;

		// Token: 0x04006C9D RID: 27805
		public GameObject dropZoneRoot;

		// Token: 0x04006C9E RID: 27806
		public List<GameObject> recyclerRoot;

		// Token: 0x04006C9F RID: 27807
		public List<GameObject> allShelvesRoot;

		// Token: 0x04006CA0 RID: 27808
		[NonSerialized]
		public List<BuilderConveyor> conveyors = new List<BuilderConveyor>();

		// Token: 0x04006CA1 RID: 27809
		[NonSerialized]
		public List<BuilderDispenserShelf> dispenserShelves = new List<BuilderDispenserShelf>();

		// Token: 0x04006CA2 RID: 27810
		public BuilderConveyorManager conveyorManager;

		// Token: 0x04006CA3 RID: 27811
		public List<BuilderResourceMeter> resourceMeters;

		// Token: 0x04006CA4 RID: 27812
		public GameObject sharedBuildArea;

		// Token: 0x04006CA5 RID: 27813
		private BoxCollider[] sharedBuildAreas;

		// Token: 0x04006CA6 RID: 27814
		public BuilderPiece armShelfPieceType;

		// Token: 0x04006CA7 RID: 27815
		[NonSerialized]
		public List<BuilderRecycler> recyclers;

		// Token: 0x04006CA8 RID: 27816
		[NonSerialized]
		public List<BuilderDropZone> dropZones;

		// Token: 0x04006CA9 RID: 27817
		private int shelfSliceUpdateIndex;

		// Token: 0x04006CAA RID: 27818
		public static int SHELF_SLICE_BUCKETS = 6;

		// Token: 0x04006CAB RID: 27819
		public float defaultTint = 1f;

		// Token: 0x04006CAC RID: 27820
		public float droppedTint = 0.75f;

		// Token: 0x04006CAD RID: 27821
		public float grabbedTint = 0.75f;

		// Token: 0x04006CAE RID: 27822
		public float shelfTint = 1f;

		// Token: 0x04006CAF RID: 27823
		public float potentialGrabTint = 0.75f;

		// Token: 0x04006CB0 RID: 27824
		public float paintingTint = 0.6f;

		// Token: 0x04006CB2 RID: 27826
		private List<BuilderTable.BoxCheckParams> noBlocksAreas;

		// Token: 0x04006CB3 RID: 27827
		private Collider[] noBlocksCheckResults = new Collider[64];

		// Token: 0x04006CB4 RID: 27828
		public LayerMask allPiecesMask;

		// Token: 0x04006CB5 RID: 27829
		public bool useSnapRotation;

		// Token: 0x04006CB6 RID: 27830
		public BuilderPlacementStyle usePlacementStyle;

		// Token: 0x04006CB7 RID: 27831
		public BuilderOptionButton buttonSnapRotation;

		// Token: 0x04006CB8 RID: 27832
		public BuilderOptionButton buttonSnapPosition;

		// Token: 0x04006CB9 RID: 27833
		public BuilderOptionButton buttonSaveLayout;

		// Token: 0x04006CBA RID: 27834
		public BuilderOptionButton buttonClearLayout;

		// Token: 0x04006CBB RID: 27835
		[HideInInspector]
		public List<BuilderAttachGridPlane> baseGridPlanes;

		// Token: 0x04006CBC RID: 27836
		private List<BuilderPiece> basePieces;

		// Token: 0x04006CBD RID: 27837
		[HideInInspector]
		public List<BuilderPiecePrivatePlot> allPrivatePlots;

		// Token: 0x04006CBE RID: 27838
		private int nextPieceId;

		// Token: 0x04006CBF RID: 27839
		[HideInInspector]
		public List<BuilderTable.BuildPieceSpawn> buildPieceSpawns;

		// Token: 0x04006CC0 RID: 27840
		[HideInInspector]
		public List<BuilderShelf> shelves;

		// Token: 0x04006CC1 RID: 27841
		[NonSerialized]
		public List<BuilderPiece> pieces = new List<BuilderPiece>(1024);

		// Token: 0x04006CC2 RID: 27842
		private Dictionary<int, int> pieceIDToIndexCache = new Dictionary<int, int>(1024);

		// Token: 0x04006CC3 RID: 27843
		[HideInInspector]
		public Dictionary<int, int> plotOwners;

		// Token: 0x04006CC4 RID: 27844
		private bool doesLocalPlayerOwnPlot;

		// Token: 0x04006CC5 RID: 27845
		public Dictionary<int, int> playerToArmShelfLeft;

		// Token: 0x04006CC6 RID: 27846
		public Dictionary<int, int> playerToArmShelfRight;

		// Token: 0x04006CC7 RID: 27847
		private HashSet<int> builderPiecesVisited = new HashSet<int>(128);

		// Token: 0x04006CC8 RID: 27848
		public BuilderResources totalResources;

		// Token: 0x04006CC9 RID: 27849
		[Tooltip("Resources reserved for conveyors and dispensers")]
		public BuilderResources totalReservedResources;

		// Token: 0x04006CCA RID: 27850
		public BuilderResources resourcesPerPrivatePlot;

		// Token: 0x04006CCB RID: 27851
		[NonSerialized]
		public int[] maxResources;

		// Token: 0x04006CCC RID: 27852
		private int[] plotMaxResources;

		// Token: 0x04006CCD RID: 27853
		[NonSerialized]
		public int[] usedResources;

		// Token: 0x04006CCE RID: 27854
		[NonSerialized]
		public int[] reservedResources;

		// Token: 0x04006CCF RID: 27855
		private List<int> playersInBuilder;

		// Token: 0x04006CD0 RID: 27856
		private List<IBuilderPieceFunctional> activeFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04006CD1 RID: 27857
		private List<IBuilderPieceFunctional> funcComponentsToRegister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006CD2 RID: 27858
		private List<IBuilderPieceFunctional> funcComponentsToUnregister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006CD3 RID: 27859
		private List<IBuilderPieceFunctional> fixedUpdateFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04006CD4 RID: 27860
		private List<IBuilderPieceFunctional> funcComponentsToRegisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006CD5 RID: 27861
		private List<IBuilderPieceFunctional> funcComponentsToUnregisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006CD6 RID: 27862
		private const int MAX_SPHERE_CHECK_RESULTS = 1024;

		// Token: 0x04006CD7 RID: 27863
		private NativeList<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x04006CD8 RID: 27864
		private NativeList<BuilderGridPlaneData> checkGridPlaneData;

		// Token: 0x04006CD9 RID: 27865
		private NativeArray<ColliderHit> nearbyPiecesResults;

		// Token: 0x04006CDA RID: 27866
		private NativeArray<OverlapSphereCommand> nearbyPiecesCommands;

		// Token: 0x04006CDB RID: 27867
		private List<BuilderPotentialPlacement> allPotentialPlacements;

		// Token: 0x04006CDC RID: 27868
		private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

		// Token: 0x04006CDD RID: 27869
		private BuilderTable.TableState tableState;

		// Token: 0x04006CDE RID: 27870
		private bool inRoom;

		// Token: 0x04006CDF RID: 27871
		private bool inBuilderZone;

		// Token: 0x04006CE0 RID: 27872
		private static int DROPPED_PIECE_LIMIT = 100;

		// Token: 0x04006CE1 RID: 27873
		public static string nextUpdateOverride = string.Empty;

		// Token: 0x04006CE2 RID: 27874
		private List<BuilderPiece> droppedPieces;

		// Token: 0x04006CE3 RID: 27875
		private List<BuilderTable.DroppedPieceData> droppedPieceData;

		// Token: 0x04006CE4 RID: 27876
		private HashSet<int>[] repelledPieceRoots;

		// Token: 0x04006CE5 RID: 27877
		private int repelHistoryLength = 3;

		// Token: 0x04006CE6 RID: 27878
		private int repelHistoryIndex;

		// Token: 0x04006CE7 RID: 27879
		private bool hasRequestedConfig;

		// Token: 0x04006CE8 RID: 27880
		private bool isDirty;

		// Token: 0x04006CE9 RID: 27881
		private bool saveInProgress;

		// Token: 0x04006CEA RID: 27882
		private int currentSaveSlot = -1;

		// Token: 0x04006CEB RID: 27883
		[HideInInspector]
		public UnityEvent OnSaveTimeUpdated;

		// Token: 0x04006CEC RID: 27884
		[HideInInspector]
		public UnityEvent<bool> OnSaveDirtyChanged;

		// Token: 0x04006CED RID: 27885
		[HideInInspector]
		public UnityEvent OnSaveSuccess;

		// Token: 0x04006CEE RID: 27886
		[HideInInspector]
		public UnityEvent<string> OnSaveFailure;

		// Token: 0x04006CEF RID: 27887
		[HideInInspector]
		public UnityEvent OnTableConfigurationUpdated;

		// Token: 0x04006CF0 RID: 27888
		[HideInInspector]
		public UnityEvent<bool> OnLocalPlayerClaimedPlot;

		// Token: 0x04006CF1 RID: 27889
		[HideInInspector]
		public UnityEvent OnMapCleared;

		// Token: 0x04006CF2 RID: 27890
		[HideInInspector]
		public UnityEvent<string> OnMapLoaded;

		// Token: 0x04006CF3 RID: 27891
		[HideInInspector]
		public UnityEvent<string> OnMapLoadFailed;

		// Token: 0x04006CF4 RID: 27892
		private List<BuilderTable.BuilderCommand> queuedBuildCommands;

		// Token: 0x04006CF5 RID: 27893
		private List<BuilderAction> rollBackActions;

		// Token: 0x04006CF6 RID: 27894
		private List<BuilderTable.BuilderCommand> rollBackBufferedCommands;

		// Token: 0x04006CF7 RID: 27895
		private List<BuilderTable.BuilderCommand> rollForwardCommands;

		// Token: 0x04006CF8 RID: 27896
		[OnEnterPlay_Clear]
		private static Dictionary<GTZone, BuilderTable> zoneToInstance;

		// Token: 0x04006CF9 RID: 27897
		private bool isSetup;

		// Token: 0x04006CFA RID: 27898
		public BuilderTable.SnapParams pushAndEaseParams;

		// Token: 0x04006CFB RID: 27899
		public BuilderTable.SnapParams overlapParams;

		// Token: 0x04006CFC RID: 27900
		private BuilderTable.SnapParams currSnapParams;

		// Token: 0x04006CFD RID: 27901
		public int maxPlacementChildDepth = 5;

		// Token: 0x04006CFE RID: 27902
		public List<SimpleAABB> m_areaBounds = new List<SimpleAABB>();

		// Token: 0x04006CFF RID: 27903
		private static List<BuilderPiece> tempPieces = new List<BuilderPiece>(256);

		// Token: 0x04006D00 RID: 27904
		private static List<BuilderConveyor> tempConveyors = new List<BuilderConveyor>(256);

		// Token: 0x04006D01 RID: 27905
		private static List<BuilderDispenserShelf> tempDispensers = new List<BuilderDispenserShelf>(256);

		// Token: 0x04006D02 RID: 27906
		private static List<BuilderRecycler> tempRecyclers = new List<BuilderRecycler>(5);

		// Token: 0x04006D03 RID: 27907
		private static List<BuilderTable.BuilderCommand> tempRollForwardCommands = new List<BuilderTable.BuilderCommand>(128);

		// Token: 0x04006D04 RID: 27908
		private static List<BuilderPiece> tempDeletePieces = new List<BuilderPiece>(1024);

		// Token: 0x04006D05 RID: 27909
		public const int MAX_PIECE_DATA = 2560;

		// Token: 0x04006D06 RID: 27910
		public const int MAX_GRID_PLANE_DATA = 10240;

		// Token: 0x04006D07 RID: 27911
		public const int MAX_PRIVATE_PLOT_DATA = 64;

		// Token: 0x04006D08 RID: 27912
		public const int MAX_PLAYER_DATA = 64;

		// Token: 0x04006D09 RID: 27913
		private BuilderTableData tableData;

		// Token: 0x04006D0A RID: 27914
		private int fetchConfigurationAttempts;

		// Token: 0x04006D0B RID: 27915
		private int maxRetries = 3;

		// Token: 0x04006D0C RID: 27916
		private SharedBlocksManager.SharedBlocksMap sharedBlocksMap;

		// Token: 0x04006D0D RID: 27917
		private string pendingMapID;

		// Token: 0x04006D0E RID: 27918
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x04006D0F RID: 27919
		private List<SharedBlocksManager.SharedBlocksMap> startingMapList = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x04006D10 RID: 27920
		private SharedBlocksManager.SharedBlocksMap startingMap;

		// Token: 0x04006D11 RID: 27921
		private bool hasStartingMap;

		// Token: 0x04006D12 RID: 27922
		private double startingMapCacheTime = double.MinValue;

		// Token: 0x04006D13 RID: 27923
		private bool getStartingMapInProgress;

		// Token: 0x04006D14 RID: 27924
		private bool hasCachedTopMaps;

		// Token: 0x04006D15 RID: 27925
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x04006D16 RID: 27926
		private static string personalBuildKey = "MyBuild";

		// Token: 0x04006D17 RID: 27927
		private static HashSet<BuilderTable.SnapOverlapKey> tempDuplicateOverlaps = new HashSet<BuilderTable.SnapOverlapKey>(16384);

		// Token: 0x04006D18 RID: 27928
		private static List<BuilderPiece> childPieces = new List<BuilderPiece>(4096);

		// Token: 0x04006D19 RID: 27929
		private static List<BuilderPiece> rootPieces = new List<BuilderPiece>(4096);

		// Token: 0x04006D1A RID: 27930
		private static List<int> overlapPieces = new List<int>(4096);

		// Token: 0x04006D1B RID: 27931
		private static List<int> overlapOtherPieces = new List<int>(4096);

		// Token: 0x04006D1C RID: 27932
		private static List<long> overlapPacked = new List<long>(4096);

		// Token: 0x04006D1D RID: 27933
		private static char[] mapIDBuffer = new char[8];

		// Token: 0x04006D1E RID: 27934
		private static Dictionary<long, int> snapOverlapSanity = new Dictionary<long, int>(16384);

		// Token: 0x04006D1F RID: 27935
		private static List<int> tempPeiceIds = new List<int>(4096);

		// Token: 0x04006D20 RID: 27936
		private static List<int> tempParentPeiceIds = new List<int>(4096);

		// Token: 0x04006D21 RID: 27937
		private static List<int> tempAttachIndexes = new List<int>(4096);

		// Token: 0x04006D22 RID: 27938
		private static List<int> tempParentAttachIndexes = new List<int>(4096);

		// Token: 0x04006D23 RID: 27939
		private static List<int> tempParentActorNumbers = new List<int>(4096);

		// Token: 0x04006D24 RID: 27940
		private static List<bool> tempInLeftHand = new List<bool>(4096);

		// Token: 0x04006D25 RID: 27941
		private static List<int> tempPiecePlacement = new List<int>(4096);

		// Token: 0x02000EE0 RID: 3808
		private struct BoxCheckParams
		{
			// Token: 0x04006D26 RID: 27942
			public Vector3 center;

			// Token: 0x04006D27 RID: 27943
			public Vector3 halfExtents;

			// Token: 0x04006D28 RID: 27944
			public Quaternion rotation;
		}

		// Token: 0x02000EE1 RID: 3809
		[Serializable]
		public class BuildPieceSpawn
		{
			// Token: 0x04006D29 RID: 27945
			public GameObject buildPiecePrefab;

			// Token: 0x04006D2A RID: 27946
			public int count = 1;
		}

		// Token: 0x02000EE2 RID: 3810
		public enum BuilderCommandType
		{
			// Token: 0x04006D2C RID: 27948
			Create,
			// Token: 0x04006D2D RID: 27949
			Place,
			// Token: 0x04006D2E RID: 27950
			Grab,
			// Token: 0x04006D2F RID: 27951
			Drop,
			// Token: 0x04006D30 RID: 27952
			Remove,
			// Token: 0x04006D31 RID: 27953
			Paint,
			// Token: 0x04006D32 RID: 27954
			Recycle,
			// Token: 0x04006D33 RID: 27955
			ClaimPlot,
			// Token: 0x04006D34 RID: 27956
			FreePlot,
			// Token: 0x04006D35 RID: 27957
			CreateArmShelf,
			// Token: 0x04006D36 RID: 27958
			PlayerLeftRoom,
			// Token: 0x04006D37 RID: 27959
			FunctionalStateChange,
			// Token: 0x04006D38 RID: 27960
			SetSelection,
			// Token: 0x04006D39 RID: 27961
			Repel
		}

		// Token: 0x02000EE3 RID: 3811
		public enum TableState
		{
			// Token: 0x04006D3B RID: 27963
			WaitingForZoneAndRoom,
			// Token: 0x04006D3C RID: 27964
			WaitingForInitalBuild,
			// Token: 0x04006D3D RID: 27965
			ReceivingInitialBuild,
			// Token: 0x04006D3E RID: 27966
			WaitForInitialBuildMaster,
			// Token: 0x04006D3F RID: 27967
			WaitForMasterResync,
			// Token: 0x04006D40 RID: 27968
			ReceivingMasterResync,
			// Token: 0x04006D41 RID: 27969
			InitialBuild,
			// Token: 0x04006D42 RID: 27970
			ExecuteQueuedCommands,
			// Token: 0x04006D43 RID: 27971
			Ready,
			// Token: 0x04006D44 RID: 27972
			BadData,
			// Token: 0x04006D45 RID: 27973
			WaitingForSharedMapLoad
		}

		// Token: 0x02000EE4 RID: 3812
		public enum DroppedPieceState
		{
			// Token: 0x04006D47 RID: 27975
			None = -1,
			// Token: 0x04006D48 RID: 27976
			Light,
			// Token: 0x04006D49 RID: 27977
			Heavy,
			// Token: 0x04006D4A RID: 27978
			Frozen
		}

		// Token: 0x02000EE5 RID: 3813
		private struct DroppedPieceData
		{
			// Token: 0x04006D4B RID: 27979
			public BuilderTable.DroppedPieceState droppedState;

			// Token: 0x04006D4C RID: 27980
			public float speedThreshCrossedTime;

			// Token: 0x04006D4D RID: 27981
			public float filteredSpeed;
		}

		// Token: 0x02000EE6 RID: 3814
		public struct BuilderCommand
		{
			// Token: 0x04006D4E RID: 27982
			public BuilderTable.BuilderCommandType type;

			// Token: 0x04006D4F RID: 27983
			public int pieceType;

			// Token: 0x04006D50 RID: 27984
			public int pieceId;

			// Token: 0x04006D51 RID: 27985
			public int attachPieceId;

			// Token: 0x04006D52 RID: 27986
			public int parentPieceId;

			// Token: 0x04006D53 RID: 27987
			public int parentAttachIndex;

			// Token: 0x04006D54 RID: 27988
			public int attachIndex;

			// Token: 0x04006D55 RID: 27989
			public Vector3 localPosition;

			// Token: 0x04006D56 RID: 27990
			public Quaternion localRotation;

			// Token: 0x04006D57 RID: 27991
			public byte twist;

			// Token: 0x04006D58 RID: 27992
			public sbyte bumpOffsetX;

			// Token: 0x04006D59 RID: 27993
			public sbyte bumpOffsetZ;

			// Token: 0x04006D5A RID: 27994
			public Vector3 velocity;

			// Token: 0x04006D5B RID: 27995
			public Vector3 angVelocity;

			// Token: 0x04006D5C RID: 27996
			public bool isLeft;

			// Token: 0x04006D5D RID: 27997
			public int materialType;

			// Token: 0x04006D5E RID: 27998
			public NetPlayer player;

			// Token: 0x04006D5F RID: 27999
			public BuilderPiece.State state;

			// Token: 0x04006D60 RID: 28000
			public bool isQueued;

			// Token: 0x04006D61 RID: 28001
			public bool canRollback;

			// Token: 0x04006D62 RID: 28002
			public int localCommandId;

			// Token: 0x04006D63 RID: 28003
			public int serverTimeStamp;
		}

		// Token: 0x02000EE7 RID: 3815
		[Serializable]
		public struct SnapParams
		{
			// Token: 0x04006D64 RID: 28004
			public float minOffsetY;

			// Token: 0x04006D65 RID: 28005
			public float maxOffsetY;

			// Token: 0x04006D66 RID: 28006
			public float maxUpDotProduct;

			// Token: 0x04006D67 RID: 28007
			public float maxTwistDotProduct;

			// Token: 0x04006D68 RID: 28008
			public float snapAttachDistance;

			// Token: 0x04006D69 RID: 28009
			public float snapDelayTime;

			// Token: 0x04006D6A RID: 28010
			public float snapDelayOffsetDist;

			// Token: 0x04006D6B RID: 28011
			public float unSnapDelayTime;

			// Token: 0x04006D6C RID: 28012
			public float unSnapDelayDist;

			// Token: 0x04006D6D RID: 28013
			public float maxBlockSnapDist;
		}

		// Token: 0x02000EE8 RID: 3816
		private struct SnapOverlapKey
		{
			// Token: 0x06005ECA RID: 24266 RVA: 0x001E6CAD File Offset: 0x001E4EAD
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int>(this.piece.GetHashCode(), this.otherPiece.GetHashCode());
			}

			// Token: 0x06005ECB RID: 24267 RVA: 0x001E6CCA File Offset: 0x001E4ECA
			public bool Equals(BuilderTable.SnapOverlapKey other)
			{
				return this.piece == other.piece && this.otherPiece == other.otherPiece;
			}

			// Token: 0x06005ECC RID: 24268 RVA: 0x001E6CEA File Offset: 0x001E4EEA
			public override bool Equals(object o)
			{
				return o is BuilderTable.SnapOverlapKey && this.Equals((BuilderTable.SnapOverlapKey)o);
			}

			// Token: 0x04006D6E RID: 28014
			public long piece;

			// Token: 0x04006D6F RID: 28015
			public long otherPiece;
		}
	}
}
