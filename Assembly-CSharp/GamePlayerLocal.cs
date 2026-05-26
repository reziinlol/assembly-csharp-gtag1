using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006E0 RID: 1760
public class GamePlayerLocal : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06002C75 RID: 11381 RVA: 0x000EFF60 File Offset: 0x000EE160
	private void Awake()
	{
		GamePlayerLocal.instance = this;
		this.hands = new GamePlayerLocal.HandData[2];
		this.inputData = new GamePlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GamePlayerLocal.InputData(32);
		}
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		GamePlayerLocal._LoadSnappedPlayerPrefsToCache(this.gamePlayer);
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x000EFFD5 File Offset: 0x000EE1D5
	private void OnJoinRoom()
	{
		this.gamePlayer.MigrateHeldActorNumbers();
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x000EFFE4 File Offset: 0x000EE1E4
	public void OnUpdateInteract()
	{
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(this.currGameEntityManager, j);
		}
	}

	// Token: 0x06002C78 RID: 11384 RVA: 0x000F002C File Offset: 0x000EE22C
	public void DebugSlotsReport(string header)
	{
		try
		{
			string format = "[SlotsReport] {0} | currManager={1} localActor={2} | slots: ";
			GameEntityManager gameEntityManager = this.currGameEntityManager;
			object arg = (gameEntityManager != null) ? new GTZone?(gameEntityManager.zone) : null;
			Player localPlayer = PhotonNetwork.LocalPlayer;
			string text = string.Format(format, header, arg, (localPlayer != null) ? localPlayer.ActorNumber : -1);
			for (int i = 0; i < 4; i++)
			{
				GameEntity gameEntity;
				GamePlayer.SlotData slotData;
				if (this.gamePlayer.TryGetSlotEntity(i, out gameEntity))
				{
					if (gameEntity != null)
					{
						string str = text;
						string format2 = "[{0}: id={1} '{2}' type={3} mgr={4}] ";
						object[] array = new object[5];
						array[0] = i;
						array[1] = gameEntity.id.index;
						array[2] = gameEntity.name;
						array[3] = gameEntity.typeId;
						int num = 4;
						GameEntityManager manager = gameEntity.manager;
						array[num] = ((manager != null) ? new GTZone?(manager.zone) : null);
						text = str + string.Format(format2, array);
					}
					else
					{
						text += string.Format("[{0}: STALE entity returned by TryGetSlotEntity!] ", i);
					}
				}
				else if (this.gamePlayer.TryGetSlotData(i, out slotData))
				{
					string str2 = text;
					string format3 = "[{0}: rawId={1} mgr={2} ORPHANED_SLOT_DATA] ";
					object arg2 = i;
					object arg3 = slotData.entityId.index;
					GameEntityManager entityManager = slotData.entityManager;
					text = str2 + string.Format(format3, arg2, arg3, (entityManager != null) ? new GTZone?(entityManager.zone) : null);
				}
				else
				{
					text += string.Format("[{0}: empty] ", i);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06002C79 RID: 11385 RVA: 0x000F01D4 File Offset: 0x000EE3D4
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GamePlayerLocal.InputDataMotion data = default(GamePlayerLocal.InputDataMotion);
		data.position = ControllerInputPoller.DevicePosition(xrnode);
		data.rotation = ControllerInputPoller.DeviceRotation(xrnode);
		data.velocity = ControllerInputPoller.DeviceVelocity(xrnode);
		data.angVelocity = ControllerInputPoller.DeviceAngularVelocity(xrnode);
		data.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(data);
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x000F0240 File Offset: 0x000EE440
	private void UpdateHand(GameEntityManager emptyHandManager, int handIndex)
	{
		GameEntityManager gameEntityManager;
		if (!this.gamePlayer.GetGrabbedGameEntityIdAndManager(handIndex, out gameEntityManager).IsValid())
		{
			this.UpdateHandEmpty(emptyHandManager, handIndex);
			return;
		}
		this.UpdateHandHolding(gameEntityManager, handIndex);
	}

	// Token: 0x06002C7B RID: 11387 RVA: 0x000F0278 File Offset: 0x000EE478
	public void MigrateToEntityManager(GameEntityManager newEntityManager)
	{
		if (this.currGameEntityManager == newEntityManager && !this.pendingFullMigration)
		{
			return;
		}
		this.pendingFullMigration = false;
		this.DebugSlotsReport(string.Format("Pre-Migrate to zone={0}", (newEntityManager != null) ? new GTZone?(newEntityManager.zone) : null));
		for (int i = 0; i < 4; i++)
		{
			GamePlayer.SlotData slotData;
			if (this.gamePlayer.TryGetSlotData(i, out slotData) && !(slotData.entityManager == null) && !(slotData.entityManager == newEntityManager))
			{
				GameEntity gameEntity = slotData.entityManager.GetGameEntity(slotData.entityId);
				if (!(gameEntity == null) && gameEntity.IsScenePlaced)
				{
					slotData.entityManager.ReleaseScenePlacedHold(gameEntity);
					this.gamePlayer.ClearSlot(i);
					if (GamePlayer.IsGrabSlot(i))
					{
						this.ClearGrabbed(i);
					}
				}
			}
		}
		if (newEntityManager.IsAuthority())
		{
			this.gamePlayer.AuthorityMigrateToEntityManager(newEntityManager);
		}
		this.currGameEntityManager = newEntityManager;
		List<GameEntityCreateData> entityData;
		if (this.joinWithItemsSentForCurrentMigration)
		{
			this.joinWithItemsSentForCurrentMigration = false;
		}
		else if (GamePlayerLocal.TryGetMigrationRecoveryList(newEntityManager, out entityData))
		{
			this.currGameEntityManager.RequestMigrationRecovery(entityData);
		}
		this.DebugSlotsReport(string.Format("Post-Migrate to zone={0}", (newEntityManager != null) ? new GTZone?(newEntityManager.zone) : null));
	}

	// Token: 0x06002C7C RID: 11388 RVA: 0x000F03C8 File Offset: 0x000EE5C8
	public void SetGrabbed(GameEntityId gameBallId, int handIndex)
	{
		GamePlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = (gameBallId.IsValid() ? 0.0 : handData.gripPressedTime);
		this.hands[handIndex] = handData;
		if (handIndex == 0)
		{
			EquipmentInteractor.instance.disableLeftGrab = gameBallId.IsValid();
			return;
		}
		EquipmentInteractor.instance.disableRightGrab = gameBallId.IsValid();
	}

	// Token: 0x06002C7D RID: 11389 RVA: 0x000F043C File Offset: 0x000EE63C
	public void ClearGrabbedIfHeld(GameEntityId gameBallId, GameEntityManager manager)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.gamePlayer.IsInSlot(i, gameBallId.index, manager))
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002C7E RID: 11390 RVA: 0x000F0471 File Offset: 0x000EE671
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex);
	}

	// Token: 0x06002C7F RID: 11391 RVA: 0x000F0480 File Offset: 0x000EE680
	private void UpdateStuckState()
	{
		bool disableMovement = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGrabbedGameEntityId(i).IsValid())
			{
				disableMovement = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = disableMovement;
	}

	// Token: 0x06002C80 RID: 11392 RVA: 0x000F04C8 File Offset: 0x000EE6C8
	private void UpdateHandEmpty(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			bool flag = GamePlayer.IsLeftHand(handIndex) ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand));
			if (flag)
			{
				bool gripWasHeld = this.hands[handIndex].gripWasHeld;
			}
			GamePlayerLocal.HandData handData = this.hands[handIndex];
			handData.gripWasHeld = flag;
			this.hands[handIndex] = handData;
			return;
		}
		if (this.gamePlayer.IsGrabbingDisabled())
		{
			return;
		}
		GamePlayerLocal.HandData handData2 = this.hands[handIndex];
		bool flag2 = GamePlayer.IsLeftHand(handIndex);
		bool flag3 = flag2 ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand));
		double timeAsDouble = Time.timeAsDouble;
		bool flag4 = flag3 && !handData2.gripWasHeld;
		if (flag4)
		{
			handData2.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData2.gripPressedTime;
		handData2.gripWasHeld = flag3;
		bool flag5 = flag2 ? ControllerInputPoller.GetIndexPressed(XRNode.LeftHand) : ControllerInputPoller.GetIndexPressed(XRNode.RightHand);
		if (flag5 && !handData2.gripWasHeld)
		{
			handData2.triggerPressedTime = timeAsDouble;
		}
		double num2 = timeAsDouble - handData2.triggerPressedTime;
		handData2.triggerWasHeld = flag5;
		this.hands[handIndex] = handData2;
		if (flag3 && num < 0.15000000596046448)
		{
			Transform handTransform = this.gamePlayer.GetHandTransform(handIndex);
			Vector3 position = handTransform.position;
			Vector3 vector = Vector3.Lerp(position, this.GetFingerTransform(handIndex).position, 0.5f);
			Vector3 b = position;
			Quaternion rotation = handTransform.rotation;
			bool flag6;
			GameEntityId gameEntityId = gameEntityManager.TryGrabLocal(position, vector, flag2, out b, out flag6);
			if (flag4)
			{
				if (gameEntityId.IsValid())
				{
					gameEntityManager.GetGameEntity(gameEntityId);
				}
				else
				{
					gameEntityManager.LogGrabDiagnostics(position, flag2, handIndex);
				}
			}
			if (gameEntityId.IsValid())
			{
				Vector3 a = flag6 ? vector : position;
				Transform transform = handTransform;
				GameEntity gameEntity = gameEntityManager.GetGameEntity(gameEntityId);
				Vector3 position2 = gameEntity.transform.position + (a - b);
				Quaternion rotation2 = gameEntity.transform.rotation;
				GameGrabbable component = gameEntity.GetComponent<GameGrabbable>();
				GameGrab gameGrab;
				if (component && component.GetBestGrabPoint(position, rotation, handIndex, out gameGrab))
				{
					position2 = gameGrab.position;
					rotation2 = gameGrab.rotation;
				}
				Vector3 vector2 = transform.InverseTransformPoint(position2);
				Quaternion quaternion = Quaternion.Inverse(transform.rotation) * rotation2;
				gameEntityManager.RequestGrabEntity(gameEntityId, flag2, vector2, quaternion);
				if (gameEntity.GetComponent<GameEntityDelayedDestroy>() == null)
				{
					GamePlayerLocal.SetGrabSlotRecoveryData(handIndex, gameEntity.typeId, gameEntity.createData, vector2, quaternion);
					int num3 = 1 - handIndex;
					if (GamePlayer.IsGrabSlot(num3) && GamePlayerLocal.slotsRecoveryData[num3].entityTypeId == gameEntity.typeId)
					{
						GamePlayerLocal.SetSlotRecoveryData(num3, -1, 0L);
					}
				}
			}
		}
		if (flag5 && num2 < 0.15000000596046448)
		{
			Vector3 position3 = this.gamePlayer.GetHandTransform(handIndex).position;
			GameTriggerInteractable gameTriggerInteractable = null;
			float num4 = float.MaxValue;
			int num5 = 0;
			while (num5 < GameTriggerInteractable.LocalInteractableTriggers.Count && !GameTriggerInteractable.LocalInteractableTriggers[num5].triggerInteractionActive)
			{
				if (GameTriggerInteractable.LocalInteractableTriggers[num5].PointWithinInteractableArea(position3))
				{
					float magnitude = (GameTriggerInteractable.LocalInteractableTriggers[num5].interactableCenter.position - position3).magnitude;
					if (magnitude <= num4)
					{
						num4 = magnitude;
						gameTriggerInteractable = GameTriggerInteractable.LocalInteractableTriggers[num5];
					}
				}
				num5++;
			}
			if (gameTriggerInteractable != null)
			{
				gameTriggerInteractable.BeginTriggerInteraction(handIndex);
			}
		}
		if (!flag5)
		{
			this.ClearTriggerInteractables(handIndex);
		}
	}

	// Token: 0x06002C81 RID: 11393 RVA: 0x000F0884 File Offset: 0x000EEA84
	private void UpdateHandHolding(GameEntityManager gameEntityManager, int handIndex)
	{
		if (gameEntityManager == null)
		{
			return;
		}
		XRNode xrnode = this.GetXRNode(handIndex);
		bool flag = GamePlayer.IsLeftHand(handIndex);
		if (!(flag ? (EquipmentInteractor.instance.isLeftGrabbing && ControllerInputPoller.GetGrab(XRNode.LeftHand)) : (EquipmentInteractor.instance.isRightGrabbing && ControllerInputPoller.GetGrab(XRNode.RightHand))))
		{
			GameEntityId grabbedGameEntityId = this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(grabbedGameEntityId);
			GamePlayerLocal.SetSlotRecoveryData(handIndex, -1, 0L);
			GameSnappable component = gameEntity.GetComponent<GameSnappable>();
			if (component != null)
			{
				SuperInfectionSnapPoint superInfectionSnapPoint = component.BestSnapPoint();
				if (superInfectionSnapPoint != null)
				{
					gameEntityManager.RequestSnapEntity(grabbedGameEntityId, flag, superInfectionSnapPoint.jointType);
					int slot;
					if (gameEntity.GetComponent<GameEntityDelayedDestroy>() == null && GameSnappable.TryGetJointToSnapIndex(superInfectionSnapPoint.jointType, out slot))
					{
						GamePlayerLocal.SetSlotRecoveryData(slot, gameEntity.typeId, gameEntity.createData);
						GamePlayerLocal.SaveSnapSlotsRateLimited();
					}
					return;
				}
			}
			GameDockable component2 = gameEntity.GetComponent<GameDockable>();
			if (component2 != null)
			{
				GameEntityId gameEntityId = component2.BestDock();
				if (gameEntityId != GameEntityId.Invalid)
				{
					Transform dockablePoint = component2.GetDockablePoint();
					Quaternion quaternion = Quaternion.Inverse(Quaternion.Inverse(component2.transform.rotation) * dockablePoint.rotation);
					Vector3 vector = quaternion * -component2.transform.InverseTransformPoint(dockablePoint.position);
					GameEntity gameEntity2 = gameEntityManager.GetGameEntity(gameEntityId);
					if (gameEntity2 != null)
					{
						GameDock component3 = gameEntity2.GetComponent<GameDock>();
						if (component3 != null)
						{
							Transform dockMarker = component3.dockMarker;
							Vector3 position = dockMarker.transform.TransformPoint(vector);
							vector = gameEntity2.transform.InverseTransformPoint(position);
							Quaternion rhs = dockMarker.rotation * quaternion;
							quaternion = Quaternion.Inverse(gameEntity2.transform.rotation) * rhs;
						}
					}
					gameEntityManager.RequestAttachEntity(grabbedGameEntityId, gameEntityId, 0, vector, quaternion);
					return;
				}
			}
			Vector3 vector2 = ControllerInputPoller.DeviceAngularVelocity(xrnode);
			Quaternion rhs2 = ControllerInputPoller.DeviceRotation(xrnode);
			Quaternion handRotOffset = GTPlayer.Instance.GetHandRotOffset(flag);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GamePlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector3 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector3 = rotation * vector3;
			vector3 *= transform.localScale.x;
			vector2 = rotation * rhs2 * handRotOffset * vector2;
			this.gamePlayer.GetGrabbedGameEntityId(handIndex);
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector3 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			gameEntityManager.RequestThrowEntity(grabbedGameEntityId, flag, GTPlayer.Instance.HeadCenterPosition, vector3, vector2);
		}
		this.ClearTriggerInteractables(handIndex);
	}

	// Token: 0x06002C82 RID: 11394 RVA: 0x000C8B9C File Offset: 0x000C6D9C
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x06002C83 RID: 11395 RVA: 0x000F0B70 File Offset: 0x000EED70
	private Transform GetFingerTransform(int handIndex)
	{
		GorillaTagger gorillaTagger = GorillaTagger.Instance;
		Transform result;
		if (handIndex != 0)
		{
			if (handIndex != 1)
			{
				result = null;
			}
			else
			{
				result = gorillaTagger.rightHandTriggerCollider.transform;
			}
		}
		else
		{
			result = gorillaTagger.leftHandTriggerCollider.transform;
		}
		return result;
	}

	// Token: 0x06002C84 RID: 11396 RVA: 0x000F0BAC File Offset: 0x000EEDAC
	public Vector3 GetHandVelocity(int handIndex)
	{
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		GamePlayerLocal.InputData inputData = this.inputData[handIndex];
		Vector3 vector = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
		vector = rotation * vector;
		return vector * base.transform.localScale.x;
	}

	// Token: 0x06002C85 RID: 11397 RVA: 0x000F0C24 File Offset: 0x000EEE24
	public Vector3 GetHandAngularVelocity(int handIndex)
	{
		object obj = (handIndex == 0) ? 4 : 5;
		Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
		object node = obj;
		Quaternion rotation2 = ControllerInputPoller.DeviceRotation(node);
		Vector3 point = ControllerInputPoller.DeviceAngularVelocity(node);
		return rotation * -(Quaternion.Inverse(rotation2) * point);
	}

	// Token: 0x06002C86 RID: 11398 RVA: 0x000F0C73 File Offset: 0x000EEE73
	public float GetHandSpeed(int handIndex)
	{
		return this.inputData[handIndex].GetMaxSpeed(0f, 0.05f);
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x000F0C8C File Offset: 0x000EEE8C
	public static bool IsHandHolding(XRNode xrNode)
	{
		return GamePlayerLocal.instance.gamePlayer.IsSlotOccupied((xrNode == XRNode.LeftHand) ? 0 : 1);
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x000C8BD7 File Offset: 0x000C6DD7
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x000C8BF3 File Offset: 0x000C6DF3
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x000F0CA8 File Offset: 0x000EEEA8
	public void ClearTriggerInteractables(int handIndex)
	{
		for (int i = 0; i < GameTriggerInteractable.LocalInteractableTriggers.Count; i++)
		{
			if (GameTriggerInteractable.LocalInteractableTriggers[i].triggerInteractionActive && GameTriggerInteractable.LocalInteractableTriggers[i].handIndex == handIndex)
			{
				GameTriggerInteractable.LocalInteractableTriggers[i].EndTriggerInteraction();
			}
		}
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x000F0D00 File Offset: 0x000EEF00
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetSlotRecoveryData(int slot, int typeId, long createData)
	{
		if (!GamePlayer.IsSlot(slot))
		{
			return;
		}
		if (typeId == -2147483647)
		{
			return;
		}
		GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[slot];
		slotRecoveryData.entityTypeId = typeId;
		slotRecoveryData.createData = createData;
		GamePlayerLocal.slotsRecoveryData[slot] = slotRecoveryData;
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x000F0D48 File Offset: 0x000EEF48
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetGrabSlotRecoveryData(int slot, int typeId, long createData, Vector3 pos, Quaternion rot)
	{
		if (!GamePlayer.IsGrabSlot(slot))
		{
			return;
		}
		GamePlayerLocal.SetSlotRecoveryData(slot, typeId, createData);
		GamePlayerLocal.GrabSlotExtraRecoveryData grabSlotExtraRecoveryData = GamePlayerLocal.grabSlotsExtraRecoveryData[slot];
		grabSlotExtraRecoveryData.pos = pos;
		grabSlotExtraRecoveryData.rot = rot;
		GamePlayerLocal.grabSlotsExtraRecoveryData[slot] = grabSlotExtraRecoveryData;
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x000F0D8F File Offset: 0x000EEF8F
	internal static void SaveSnapSlotsRateLimited()
	{
		if (GamePlayerLocal.snapSlotsSave_isQueued)
		{
			return;
		}
		if (GamePlayerLocal.snapSlotsSave_lastTime + 2f < Time.unscaledTime)
		{
			GamePlayerLocal._SaveSnapSlotsImmediately();
			return;
		}
		GamePlayerLocal.snapSlotsSave_isQueued = true;
		GTDelayedExec.Add(GamePlayerLocal.instance, 2f, 0);
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x000F0DC9 File Offset: 0x000EEFC9
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		GamePlayerLocal._SaveSnapSlotsImmediately();
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x000F0DD0 File Offset: 0x000EEFD0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _SaveSnapSlotsImmediately()
	{
		GamePlayerLocal.snapSlotsSave_isQueued = false;
		GamePlayerLocal.snapSlotsSave_lastTime = Time.unscaledTime;
		int num = GamePlayerLocal._SnapSlotsSave_GetHash(GamePlayerLocal.slotsRecoveryData);
		if (num == GamePlayerLocal.snapSlotsSave_lastSavedHash)
		{
			return;
		}
		GamePlayerLocal.snapSlotsSave_lastSavedHash = num;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder(true))
		{
			for (int i = 2; i <= 3; i++)
			{
				GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[i];
				if (slotRecoveryData.entityTypeId != -1)
				{
					utf16ValueStringBuilder.Append(i);
					utf16ValueStringBuilder.Append(",");
					utf16ValueStringBuilder.Append(slotRecoveryData.entityTypeId);
					utf16ValueStringBuilder.Append(",");
					utf16ValueStringBuilder.Append(slotRecoveryData.createData);
					utf16ValueStringBuilder.Append("|");
				}
			}
			PlayerPrefs.SetString("GT_SnappedItems_V1", utf16ValueStringBuilder.ToString());
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06002C90 RID: 11408 RVA: 0x000F0EB4 File Offset: 0x000EF0B4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _LoadSnappedPlayerPrefsToCache(GamePlayer gamePlayer)
	{
		for (int i = 0; i < 4; i++)
		{
			GamePlayerLocal.slotsRecoveryData[i] = new GamePlayerLocal.SlotRecoveryData
			{
				entityTypeId = -1,
				createData = 0L
			};
		}
		for (int j = 0; j < 2; j++)
		{
			GamePlayerLocal.grabSlotsExtraRecoveryData[j] = new GamePlayerLocal.GrabSlotExtraRecoveryData
			{
				pos = Vector3.zero,
				rot = Quaternion.identity
			};
		}
		string[] array = PlayerPrefs.GetString("GT_SnappedItems_V1").Split('|', StringSplitOptions.RemoveEmptyEntries);
		for (int k = 0; k < array.Length; k++)
		{
			string[] array2 = array[k].Split(',', StringSplitOptions.None);
			int num;
			int entityTypeId;
			long createData;
			if (array2.Length >= 3 && int.TryParse(array2[0], out num) && num < 4 && GamePlayer.IsSnapSlot(num) && int.TryParse(array2[1], out entityTypeId) && long.TryParse(array2[2], out createData))
			{
				GamePlayerLocal.slotsRecoveryData[num] = new GamePlayerLocal.SlotRecoveryData
				{
					entityTypeId = entityTypeId,
					createData = createData
				};
			}
		}
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x000F0FC4 File Offset: 0x000EF1C4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int _SnapSlotsSave_GetHash(GamePlayerLocal.SlotRecoveryData[] slotsCache)
	{
		int num = 67466746;
		for (int i = 2; i <= 3; i++)
		{
			GamePlayerLocal.SlotRecoveryData slotRecoveryData = slotsCache[i];
			num = StaticHash.Compute(num, i.GetStaticHash(), slotRecoveryData.entityTypeId.GetStaticHash(), slotRecoveryData.createData.GetStaticHash());
		}
		return num;
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x000F1010 File Offset: 0x000EF210
	public static bool TryGetMigrationRecoveryList(GameEntityManager newEntityManager, out List<GameEntityCreateData> out_recoveryList)
	{
		out_recoveryList = GamePlayerLocal._migrationRecoveryList;
		GamePlayerLocal._migrationRecoveryList.Clear();
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		for (int i = 0; i < 4; i++)
		{
			GamePlayerLocal.SlotRecoveryData slotRecoveryData = GamePlayerLocal.slotsRecoveryData[i];
			int entityTypeId = slotRecoveryData.entityTypeId;
			bool flag = entityTypeId != -1 && newEntityManager.FactoryPrefabById(entityTypeId) != null;
			GamePlayer.SlotData slotData;
			bool flag2 = gamePlayer.TryGetSlotData(i, out slotData);
			if (flag || flag2)
			{
				bool flag3 = newEntityManager != null && newEntityManager == slotData.entityManager;
				GameEntity gameEntity = flag3 ? newEntityManager.GetGameEntity(slotData.entityId) : null;
				bool flag4 = gameEntity != null;
				int num = flag4 ? gameEntity.typeId : -1;
				bool flag5 = num != -1;
				bool flag6 = entityTypeId == num;
				if (!flag3 || !flag4 || !flag6)
				{
					string message = flag ? "[GamePlayerLocal]  TryGetMigrationRecoveryList: Recovering from mismatch between migrated entities and recovery data." : "[GamePlayerLocal]  ERROR!!!  TryGetMigrationRecoveryList: UNRECOVERABLE mismatch between migrated entities and recovery data.";
					Debug.unityLogger.Log(flag ? LogType.Log : LogType.Error, message);
					if (flag)
					{
						long createData = slotRecoveryData.createData;
						if (newEntityManager.LocalValidateMigrationRecoveryItem(entityTypeId, ref createData))
						{
							bool flag7 = false;
							for (int j = 0; j < 4; j++)
							{
								GamePlayer.SlotData slotData2;
								if (j != i && gamePlayer.TryGetSlotData(j, out slotData2) && !(slotData2.entityManager == null))
								{
									GameEntity gameEntity2 = slotData2.entityManager.GetGameEntity(slotData2.entityId);
									if (gameEntity2 != null && gameEntity2.typeId == entityTypeId)
									{
										flag7 = true;
										break;
									}
								}
							}
							if (flag7)
							{
								GamePlayerLocal.SetSlotRecoveryData(i, -1, 0L);
							}
							else
							{
								GamePlayerLocal._migrationRecoveryList.Add(new GameEntityCreateData
								{
									entityTypeId = entityTypeId,
									position = (GamePlayer.IsGrabSlot(i) ? GamePlayerLocal.grabSlotsExtraRecoveryData[i].pos : Vector3.zero),
									rotation = (GamePlayer.IsGrabSlot(i) ? GamePlayerLocal.grabSlotsExtraRecoveryData[i].rot : Quaternion.identity),
									createData = createData,
									createdByEntityId = -1,
									slotIndex = i
								});
							}
						}
					}
				}
			}
		}
		return GamePlayerLocal._migrationRecoveryList.Count > 0;
	}

	// Token: 0x040038CD RID: 14541
	private const string preLog = "[GamePlayerLocal]  ";

	// Token: 0x040038CE RID: 14542
	private const string preErr = "[GamePlayerLocal]  ERROR!!!  ";

	// Token: 0x040038CF RID: 14543
	public GamePlayer gamePlayer;

	// Token: 0x040038D0 RID: 14544
	private GamePlayerLocal.HandData[] hands;

	// Token: 0x040038D1 RID: 14545
	public const int MAX_INPUT_HISTORY = 32;

	// Token: 0x040038D2 RID: 14546
	private GamePlayerLocal.InputData[] inputData;

	// Token: 0x040038D3 RID: 14547
	private const string SNAP_SLOTS_SAVE_KEY = "GT_SnappedItems_V1";

	// Token: 0x040038D4 RID: 14548
	private const float SNAP_SLOTS_SAVE__INTERVAL = 2f;

	// Token: 0x040038D5 RID: 14549
	[OnEnterPlay_Set(false)]
	private static bool snapSlotsSave_isQueued;

	// Token: 0x040038D6 RID: 14550
	[OnEnterPlay_Set(0)]
	private static int snapSlotsSave_lastSavedHash;

	// Token: 0x040038D7 RID: 14551
	[OnEnterPlay_Set(0)]
	private static int snapSlotsSave_frameWhenQueued;

	// Token: 0x040038D8 RID: 14552
	[OnEnterPlay_Set(0f)]
	private static float snapSlotsSave_lastTime;

	// Token: 0x040038D9 RID: 14553
	private static readonly GamePlayerLocal.SlotRecoveryData[] slotsRecoveryData = new GamePlayerLocal.SlotRecoveryData[4];

	// Token: 0x040038DA RID: 14554
	private static readonly GamePlayerLocal.GrabSlotExtraRecoveryData[] grabSlotsExtraRecoveryData = new GamePlayerLocal.GrabSlotExtraRecoveryData[2];

	// Token: 0x040038DB RID: 14555
	[OnEnterPlay_SetNull]
	public static volatile GamePlayerLocal instance;

	// Token: 0x040038DC RID: 14556
	[NonSerialized]
	public GameEntityManager currGameEntityManager;

	// Token: 0x040038DD RID: 14557
	[NonSerialized]
	internal bool joinWithItemsSentForCurrentMigration;

	// Token: 0x040038DE RID: 14558
	[NonSerialized]
	internal bool pendingFullMigration;

	// Token: 0x040038DF RID: 14559
	private static readonly List<GameEntityCreateData> _migrationRecoveryList = new List<GameEntityCreateData>(4);

	// Token: 0x020006E1 RID: 1761
	private enum HandGrabState
	{
		// Token: 0x040038E1 RID: 14561
		Empty,
		// Token: 0x040038E2 RID: 14562
		Holding
	}

	// Token: 0x020006E2 RID: 1762
	private struct HandData
	{
		// Token: 0x040038E3 RID: 14563
		public GamePlayerLocal.HandGrabState grabState;

		// Token: 0x040038E4 RID: 14564
		public bool gripWasHeld;

		// Token: 0x040038E5 RID: 14565
		public bool triggerWasHeld;

		// Token: 0x040038E6 RID: 14566
		public double gripPressedTime;

		// Token: 0x040038E7 RID: 14567
		public double triggerPressedTime;
	}

	// Token: 0x020006E3 RID: 1763
	public struct InputDataMotion
	{
		// Token: 0x040038E8 RID: 14568
		public double time;

		// Token: 0x040038E9 RID: 14569
		public Vector3 position;

		// Token: 0x040038EA RID: 14570
		public Quaternion rotation;

		// Token: 0x040038EB RID: 14571
		public Vector3 velocity;

		// Token: 0x040038EC RID: 14572
		public Vector3 angVelocity;
	}

	// Token: 0x020006E4 RID: 1764
	public class InputData
	{
		// Token: 0x06002C95 RID: 11413 RVA: 0x000F125C File Offset: 0x000EF45C
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GamePlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x06002C96 RID: 11414 RVA: 0x000F1277 File Offset: 0x000EF477
		public void AddInput(GamePlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x06002C97 RID: 11415 RVA: 0x000F12A4 File Offset: 0x000EF4A4
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x06002C98 RID: 11416 RVA: 0x000F1320 File Offset: 0x000EF520
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 a = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GamePlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					a += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return a / (float)num3;
		}

		// Token: 0x040038ED RID: 14573
		public int maxInputs;

		// Token: 0x040038EE RID: 14574
		public List<GamePlayerLocal.InputDataMotion> inputMotionHistory;
	}

	// Token: 0x020006E5 RID: 1765
	public struct SlotRecoveryData
	{
		// Token: 0x040038EF RID: 14575
		public int entityTypeId;

		// Token: 0x040038F0 RID: 14576
		public long createData;
	}

	// Token: 0x020006E6 RID: 1766
	public struct GrabSlotExtraRecoveryData
	{
		// Token: 0x040038F1 RID: 14577
		public Vector3 pos;

		// Token: 0x040038F2 RID: 14578
		public Quaternion rot;
	}
}
