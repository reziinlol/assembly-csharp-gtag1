using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020006DC RID: 1756
public class GamePlayer : MonoBehaviour
{
	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06002C26 RID: 11302 RVA: 0x000EEC15 File Offset: 0x000ECE15
	// (set) Token: 0x06002C27 RID: 11303 RVA: 0x000EEC1D File Offset: 0x000ECE1D
	public bool DidJoinWithItems { get; set; }

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06002C28 RID: 11304 RVA: 0x000EEC26 File Offset: 0x000ECE26
	// (set) Token: 0x06002C29 RID: 11305 RVA: 0x000EEC2E File Offset: 0x000ECE2E
	public bool AdditionalDataInitialized { get; set; }

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06002C2A RID: 11306 RVA: 0x000EEC37 File Offset: 0x000ECE37
	public bool IsSubscribed
	{
		get
		{
			if (Time.frameCount != this._lastSubscriptionCheck)
			{
				this._isSubscribed = SubscriptionManager.IsPlayerSubscribed(this.rig);
				this._lastSubscriptionCheck = Time.frameCount;
			}
			return this._isSubscribed;
		}
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x000EEC68 File Offset: 0x000ECE68
	private void Awake()
	{
		this.handTransforms[0] = this.leftHand;
		this.handTransforms[1] = this.rightHand;
		for (int i = 0; i < this.slots.Length; i++)
		{
			this.slots[i].entityId = GameEntityId.Invalid;
		}
		this.newJoinZoneLimiter = new CallLimiter(10, 10f, 0.5f);
		this.netImpulseLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netGrabLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netThrowLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netStateLimiter = new CallLimiter(25, 1f, 0.5f);
		this.netSnapLimiter = new CallLimiter(25, 1f, 0.5f);
		if (this.snapPointManager == null)
		{
			this.snapPointManager = base.GetComponentInChildren<SuperInfectionSnapPointManager>(true);
			if (this.snapPointManager == null)
			{
				Debug.LogError("[GamePlayer]  ERROR!!!  Snappoints cannot function because the required `SuperInfectionSnapPointManager` could found in children.", this);
			}
		}
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x000EED78 File Offset: 0x000ECF78
	public void Clear()
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager != null)
			{
				this.slots[i].entityManager.RequestThrowEntity(this.slots[i].entityId, GamePlayer.IsLeftHand(i), GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearGrabbed(i);
		}
		for (int j = 2; j <= 3; j++)
		{
			if (this.slots[j].entityId != GameEntityId.Invalid && this.slots[j].entityManager != null)
			{
				bool isLeftHand = j != 2;
				GameEntityId entityId = this.slots[j].entityId;
				GameEntityManager entityManager = this.slots[j].entityManager;
				entityManager.RequestGrabEntity(entityId, isLeftHand, Vector3.zero, Quaternion.identity);
				entityManager.RequestThrowEntity(entityId, isLeftHand, GTPlayer.Instance.HeadCenterPosition, Vector3.zero, Vector3.zero);
			}
			this.ClearSlot(j);
		}
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x000EEEBC File Offset: 0x000ED0BC
	public void ResetData()
	{
		for (int i = 0; i < 4; i++)
		{
			this.ClearSlot(i);
		}
		this.DidJoinWithItems = false;
		this.AdditionalDataInitialized = false;
		this.SetInitializePlayer(false);
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x06002C2F RID: 11311 RVA: 0x000EEEF1 File Offset: 0x000ED0F1
	private void Start()
	{
		GamePlayer.InitializeStaticLookupCaches();
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x000EEEF8 File Offset: 0x000ED0F8
	public void MigrateHeldActorNumbers()
	{
		int actorNumber = this.rig.OwningNetPlayer.ActorNumber;
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityManager != null)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				if (gameEntity != null)
				{
					if (i <= 1)
					{
						gameEntity.MigrateHeldBy(actorNumber);
					}
					else
					{
						gameEntity.MigrateSnappedBy(actorNumber);
					}
				}
			}
		}
	}

	// Token: 0x06002C31 RID: 11313 RVA: 0x000EEF80 File Offset: 0x000ED180
	public void SetGrabbed(GameEntityId gameBallId, int handIndex, GameEntityManager gameEntityManager)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return;
		}
		this.SetSlot(handIndex, gameBallId, gameEntityManager);
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x000EEF94 File Offset: 0x000ED194
	public void SetSnapped(GameEntityId entityId, int slotIndex, GameEntityManager gameEntityManager)
	{
		if (entityId.IsValid())
		{
			this.ClearSnappedIfSnapped(entityId, gameEntityManager);
			this.ClearGrabbedIfHeld(entityId, gameEntityManager);
		}
		this.SetSlot(slotIndex, entityId, gameEntityManager);
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x000EEFB8 File Offset: 0x000ED1B8
	public void SetSlot(int slotIndex, GameEntityId entityId, GameEntityManager manager)
	{
		if (slotIndex < 0 || slotIndex >= 4)
		{
			return;
		}
		if (entityId.IsValid())
		{
			manager.GetGameEntity(entityId);
		}
		GamePlayer.SlotData slotData = this.slots[slotIndex];
		slotData.entityId = entityId;
		slotData.entityManager = manager;
		this.slots[slotIndex] = slotData;
	}

	// Token: 0x06002C34 RID: 11316 RVA: 0x000EF00C File Offset: 0x000ED20C
	public void ClearZone(GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager == manager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased();
					}
				}
				this.ClearSlot(i);
			}
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			this.DidJoinWithItems = false;
		}
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x000EF0B8 File Offset: 0x000ED2B8
	public void ClearGrabbedIfHeld(GameEntityId gameBallId, GameEntityManager manager)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId == gameBallId && this.slots[i].entityManager == manager)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x000EF10C File Offset: 0x000ED30C
	public void ClearSnappedIfSnapped(GameEntityId gameBallId, GameEntityManager manager)
	{
		for (int i = 2; i <= 3; i++)
		{
			if (this.slots[i].entityId == gameBallId && this.slots[i].entityManager == manager)
			{
				this.ClearSlot(i);
			}
		}
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x000EF15E File Offset: 0x000ED35E
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameEntityId.Invalid, handIndex, null);
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x000EF16D File Offset: 0x000ED36D
	public void ClearSlot(int slotIndex)
	{
		this.SetSlot(slotIndex, GameEntityId.Invalid, null);
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x000EF17C File Offset: 0x000ED37C
	public bool IsGrabbingDisabled()
	{
		return this.grabbingDisabled;
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x000EF184 File Offset: 0x000ED384
	public void DisableGrabbing(bool disable)
	{
		this.grabbingDisabled = disable;
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x000EF18D File Offset: 0x000ED38D
	internal bool IsSlotOccupied(int slotIndex)
	{
		return this.slots[slotIndex].entityId.index != -1;
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x000EF1AB File Offset: 0x000ED3AB
	public bool IsHoldingEntity(GameEntityId gameEntityId, bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand)) == gameEntityId;
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x000EF1BF File Offset: 0x000ED3BF
	public bool IsHoldingEntity(GameEntityManager gameEntityManager, bool isLeftHand)
	{
		return gameEntityManager.GetGameEntity(this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand))) != null;
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x000EF1D9 File Offset: 0x000ED3D9
	public bool IsHoldingEntity(GameEntityId gameEntityId)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(true)) == gameEntityId || this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(false)) == gameEntityId;
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x000EF203 File Offset: 0x000ED403
	public void RequestDropAllSnapped()
	{
		this.Clear();
		this.snapPointManager.DropAllSnappedAuthority();
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x000EF216 File Offset: 0x000ED416
	public List<GameEntityId> HeldAndSnappedItems(GameEntityManager manager)
	{
		return this.IterateHeldAndSnappedItems(manager).ToList<GameEntityId>();
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x000EF224 File Offset: 0x000ED424
	public IEnumerable<GameEntityId> IterateHeldAndSnappedItems(GameEntityManager manager)
	{
		int num;
		for (int i = 0; i < 4; i = num)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager == manager)
			{
				yield return this.slots[i].entityId;
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x000EF23B File Offset: 0x000ED43B
	public List<GameEntity> HeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		return this.IterateHeldAndSnappedEntities(ignoreEntitiesInManager).ToList<GameEntity>();
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x000EF249 File Offset: 0x000ED449
	public IEnumerable<GameEntity> IterateHeldAndSnappedEntities(GameEntityManager ignoreEntitiesInManager = null)
	{
		int num;
		for (int i = 0; i < 4; i = num)
		{
			if (this.slots[i].entityId != GameEntityId.Invalid && this.slots[i].entityManager != null)
			{
				if (this.slots[i].entityManager != ignoreEntitiesInManager)
				{
					GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
					yield return gameEntity;
				}
				else
				{
					this.slots[i].entityManager.GetGameEntity(this.slots[i].entityId);
				}
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x000EF260 File Offset: 0x000ED460
	public void DeleteGrabbedEntityLocal(int handIndex)
	{
		if (this.slots[handIndex].entityId != GameEntityId.Invalid && this.slots[handIndex].entityManager != null)
		{
			GameEntity gameEntity = this.slots[handIndex].entityManager.GetGameEntity(this.slots[handIndex].entityId);
			if (gameEntity != null)
			{
				if (gameEntity != null)
				{
					Action onReleased = gameEntity.OnReleased;
					if (onReleased != null)
					{
						onReleased();
					}
				}
				this.slots[handIndex].entityManager.DestroyItemLocal(this.slots[handIndex].entityId);
			}
		}
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x000EF314 File Offset: 0x000ED514
	public int AuthorityMigrateToEntityManager(GameEntityManager newEntityManager)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			GameEntityId entityId = this.slots[i].entityId;
			if (entityId != GameEntityId.Invalid && this.slots[i].entityManager != newEntityManager)
			{
				GameEntity gameEntity = this.slots[i].entityManager.GetGameEntity(entityId);
				if (gameEntity != null)
				{
					if (gameEntity.IsScenePlaced)
					{
						GameEntityManager entityManager = this.slots[i].entityManager;
						if (entityManager != null)
						{
							entityManager.ReleaseScenePlacedHold(gameEntity);
						}
						this.ClearSlot(i);
					}
					else
					{
						GameEntityId entityId2 = gameEntity.MigrateToEntityManager(newEntityManager);
						GamePlayer.SlotData slotData = this.slots[i];
						slotData.entityManager = newEntityManager;
						slotData.entityId = entityId2;
						this.slots[i] = slotData;
						num++;
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x000EF3FD File Offset: 0x000ED5FD
	internal bool IsInSlot(int slotIndex, int entityIndex, GameEntityManager manager)
	{
		return this.slots[slotIndex].entityId.index == entityIndex && this.slots[slotIndex].entityManager == manager;
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x000EF431 File Offset: 0x000ED631
	internal bool TryGetSlotData(int slotIndex, out GamePlayer.SlotData out_slotData)
	{
		out_slotData = this.slots[slotIndex];
		return out_slotData.entityId.index != -1;
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x000EF458 File Offset: 0x000ED658
	internal bool TryGetSlotEntity(int slotIndex, out GameEntity out_entity)
	{
		GamePlayer.SlotData slotData;
		if (!this.TryGetSlotData(slotIndex, out slotData))
		{
			out_entity = null;
			return false;
		}
		out_entity = slotData.entityManager.GetGameEntity(slotData.entityId);
		return out_entity != null;
	}

	// Token: 0x06002C49 RID: 11337 RVA: 0x000EF490 File Offset: 0x000ED690
	public GameEntityId GetGameEntityId(bool isLeftHand)
	{
		return this.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(isLeftHand));
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x000EF49E File Offset: 0x000ED69E
	public GameEntityId GetGrabbedGameEntityId(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return GameEntityId.Invalid;
		}
		return this.slots[handIndex].entityId;
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x000EF4BF File Offset: 0x000ED6BF
	public GameEntityId GetGrabbedGameEntityIdAndManager(int handIndex, out GameEntityManager manager)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			manager = null;
			return GameEntityId.Invalid;
		}
		manager = this.slots[handIndex].entityManager;
		return this.slots[handIndex].entityId;
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x000EF4F8 File Offset: 0x000ED6F8
	public GameEntity GetGrabbedGameEntity(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1 || this.slots[handIndex].entityManager == null)
		{
			return null;
		}
		return this.slots[handIndex].entityManager.GetGameEntity(this.GetGrabbedGameEntityId(handIndex));
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x000EF548 File Offset: 0x000ED748
	public int FindSlotIndex(GameEntityId entityId)
	{
		int num = -1;
		int num2 = 0;
		while (num2 < 4 && num == -1)
		{
			num = ((this.slots[num2].entityId == entityId) ? num2 : -1);
			num2++;
		}
		return num;
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x000EF588 File Offset: 0x000ED788
	public int FindHandIndex(GameEntityId entityId)
	{
		for (int i = 0; i <= 1; i++)
		{
			if (this.slots[i].entityId == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x000EF5C0 File Offset: 0x000ED7C0
	public int FindSnapIndex(GameEntityId entityId)
	{
		for (int i = 2; i <= 3; i++)
		{
			if (this.slots[i].entityId == entityId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x000C8513 File Offset: 0x000C6713
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x000C8519 File Offset: 0x000C6719
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x000EF5F8 File Offset: 0x000ED7F8
	[Obsolete("Method `GamePlayer.TryGetGamePlayer(Player)` is obsolete, use `TryGetGamePlayer(Player, out GamePlayer)` instead.")]
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		if (player == null)
		{
			return null;
		}
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom != null && currentRoom.GetPlayer(actorNumber, false) == null)
		{
			return null;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x000EF644 File Offset: 0x000ED844
	public static GamePlayer GetGamePlayer(Player player)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(player, out result);
		return result;
	}

	// Token: 0x06002C54 RID: 11348 RVA: 0x000EF65B File Offset: 0x000ED85B
	public static bool TryGetGamePlayer(Player player, out GamePlayer gamePlayer)
	{
		if (player == null)
		{
			gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(player.ActorNumber, out gamePlayer);
	}

	// Token: 0x06002C55 RID: 11349 RVA: 0x000EF674 File Offset: 0x000ED874
	[Obsolete("Method `GamePlayer.GetGamePlayer(actorNum)` is obsolete, use `TryGetGamePlayer(actorNum, out GamePlayer)` instead.")]
	public static GamePlayer GetGamePlayer(int actorNumber)
	{
		GamePlayer result;
		GamePlayer.TryGetGamePlayer(actorNumber, out result);
		return result;
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x000EF68C File Offset: 0x000ED88C
	public static bool TryGetGamePlayer(int actorNumber, out GamePlayer out_gamePlayer)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || VRRigCache.Instance == null || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			out_gamePlayer = null;
			return false;
		}
		return GamePlayer.TryGetGamePlayer(rigContainer.Rig, out out_gamePlayer);
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x000EF6D8 File Offset: 0x000ED8D8
	public static bool TryGetGamePlayer(VRRig rig, out GamePlayer out_gamePlayer)
	{
		if (rig == null)
		{
			out_gamePlayer = null;
			return false;
		}
		out_gamePlayer = rig.GetComponent<GamePlayer>();
		return out_gamePlayer != null;
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x000EF704 File Offset: 0x000ED904
	public static GamePlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GamePlayer component = transform.GetComponent<GamePlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x000EF740 File Offset: 0x000ED940
	public Transform GetHandTransform(int handIndex)
	{
		if (handIndex < 0 || handIndex > 1)
		{
			return null;
		}
		return this.handTransforms[handIndex];
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x000EF754 File Offset: 0x000ED954
	public bool TryGetSlotXform(int slotIndex, out Transform slotXform)
	{
		if (GamePlayer.IsGrabSlot(slotIndex))
		{
			slotXform = this.handTransforms[slotIndex];
		}
		else if (GamePlayer.IsSnapSlot(slotIndex))
		{
			SnapJointType snapIndexToJoint = GameSnappable.GetSnapIndexToJoint(slotIndex);
			SuperInfectionSnapPoint superInfectionSnapPoint = (this.snapPointManager != null) ? this.snapPointManager.FindSnapPoint(snapIndexToJoint) : null;
			slotXform = ((superInfectionSnapPoint != null) ? superInfectionSnapPoint.transform : null);
		}
		else
		{
			slotXform = null;
		}
		return slotXform != null;
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x000EF7C3 File Offset: 0x000ED9C3
	public bool IsLocal()
	{
		return GamePlayerLocal.instance != null && GamePlayerLocal.instance.gamePlayer == this;
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x000EF7E8 File Offset: 0x000ED9E8
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player, GameEntityManager manager)
	{
		string str = "";
		for (int i = 0; i < 4; i++)
		{
			if (this.slots[i].entityManager == manager)
			{
				int netIdFromEntityId = manager.GetNetIdFromEntityId(this.slots[i].entityId);
				writer.Write(netIdFromEntityId);
				long value = 0L;
				if (netIdFromEntityId != -1)
				{
					GameEntity gameEntity = manager.GetGameEntity(this.slots[i].entityId);
					if (gameEntity != null)
					{
						str += string.Format(" [{0}: {1}/{2}]", i, gameEntity.gameObject.name, netIdFromEntityId);
						value = BitPackUtils.PackHandPosRotForNetwork(gameEntity.transform.localPosition, gameEntity.transform.localRotation);
					}
				}
				writer.Write(value);
			}
			else
			{
				writer.Write(-1);
				writer.Write(0L);
			}
		}
		writer.Write(this.AdditionalDataInitialized);
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x000EF8E0 File Offset: 0x000EDAE0
	public static void DeserializeNetworkState(BinaryReader reader, GamePlayer gamePlayer, GameEntityManager manager)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = reader.ReadInt32();
			long num2 = reader.ReadInt64();
			if (num != -1)
			{
				GameEntityId entityIdFromNetId = manager.GetEntityIdFromNetId(num);
				if (entityIdFromNetId.IsValid())
				{
					GameEntity gameEntity = manager.GetGameEntity(entityIdFromNetId);
					if (num2 != 0L && !(gameEntity == null))
					{
						Vector3 localPosition;
						Quaternion localRotation;
						BitPackUtils.UnpackHandPosRotFromNetwork(num2, out localPosition, out localRotation);
						if (gamePlayer != null && gamePlayer.rig.OwningNetPlayer != null)
						{
							if (GamePlayer.IsGrabSlot(i))
							{
								manager.GrabEntityOnCreate(entityIdFromNetId, GamePlayer.IsLeftHand(i), localPosition, localRotation, gamePlayer.rig.OwningNetPlayer);
							}
							else
							{
								int jointType = -1;
								if (i == 2)
								{
									jointType = 1;
								}
								else if (i == 3)
								{
									jointType = 4;
								}
								manager.SnapEntityOnCreate(entityIdFromNetId, i == 2, localPosition, localRotation, jointType, gamePlayer.rig.OwningNetPlayer);
							}
						}
					}
				}
			}
		}
		bool initializePlayer = reader.ReadBoolean();
		if (gamePlayer != null)
		{
			gamePlayer.SetInitializePlayer(initializePlayer);
		}
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x000EF9D1 File Offset: 0x000EDBD1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsSlot(int i)
	{
		return i >= 0 && i < 4;
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000EF9DD File Offset: 0x000EDBDD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsGrabSlot(int i)
	{
		return i >= 0 && i <= 1;
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x000EF9EC File Offset: 0x000EDBEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsSnapSlot(int i)
	{
		return i >= 2 && i <= 3;
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x000EF9FB File Offset: 0x000EDBFB
	internal static void InitializeStaticLookupCaches()
	{
		GamePlayer.lookupCache_actorNum_to_gamePlayer = new ValueTuple<int, GamePlayer>[20];
		GamePlayer.lookupCache_rigInstanceId_to_gamePlayer = new ValueTuple<int, GamePlayer>[20];
		if (VRRigCache.isInitialized)
		{
			GamePlayer.UpdateStaticLookupCaches();
		}
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x000EFA24 File Offset: 0x000EDC24
	internal static void UpdateStaticLookupCaches()
	{
		if (GamePlayer.lookupCache_actorNum_to_gamePlayer == null)
		{
			return;
		}
		List<VRRig> list;
		using (ListPool<VRRig>.Get(out list))
		{
			if (list.Capacity < 20)
			{
				list.Capacity = 20;
			}
			VRRigCache.Instance.GetActiveRigs(list);
			if (list.Count > GamePlayer.lookupCache_actorNum_to_gamePlayer.Length)
			{
				int newSize = list.Count * 2;
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_actorNum_to_gamePlayer, newSize);
				Array.Resize<ValueTuple<int, GamePlayer>>(ref GamePlayer.lookupCache_rigInstanceId_to_gamePlayer, newSize);
			}
			GamePlayer.staticLookupCachesCount = list.Count;
			if (GamePlayer.staticLookupCachesCount >= 1)
			{
				VRRig vrrig = list[0];
				if (vrrig == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) The VRRig at index 0 is expected to be the local rig but is null.");
				}
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(actorNumber, gamePlayer);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[0] = new ValueTuple<int, GamePlayer>(vrrig.GetInstanceID(), gamePlayer);
			}
			for (int i = 1; i < GamePlayer.staticLookupCachesCount; i++)
			{
				VRRig vrrig2 = list[i];
				if (vrrig2 == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) An entry from `VRRigCache.Instance.GetActiveRigs(activeRigs)` is null but is expected to be ready and all entries not null at this stage.");
				}
				GamePlayer component = vrrig2.GetComponent<GamePlayer>();
				if (component == null)
				{
					throw new NullReferenceException("[GT/GamePlayer::_VRRigCache_OnActiveRigsChanged]  ERROR!!!  (should never happen) Could not get GamePlayer from rig which is expected to be ready at this stage.");
				}
				NetPlayer owningNetPlayer = vrrig2.OwningNetPlayer;
				int item = (owningNetPlayer != null) ? owningNetPlayer.ActorNumber : int.MinValue;
				GamePlayer.lookupCache_actorNum_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(item, component);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[i] = new ValueTuple<int, GamePlayer>(vrrig2.GetInstanceID(), component);
			}
			for (int j = GamePlayer.staticLookupCachesCount; j < GamePlayer.lookupCache_actorNum_to_gamePlayer.Length; j++)
			{
				GamePlayer.lookupCache_actorNum_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
				GamePlayer.lookupCache_rigInstanceId_to_gamePlayer[j] = new ValueTuple<int, GamePlayer>(0, null);
			}
		}
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x000EFC0C File Offset: 0x000EDE0C
	public void SetInitializePlayer(bool initialized)
	{
		bool additionalDataInitialized = this.AdditionalDataInitialized;
		this.AdditionalDataInitialized = initialized;
		if (!additionalDataInitialized && this.AdditionalDataInitialized)
		{
			Action onPlayerInitialized = this.OnPlayerInitialized;
			if (onPlayerInitialized == null)
			{
				return;
			}
			onPlayerInitialized();
		}
	}

	// Token: 0x04003898 RID: 14488
	private const string preLog = "[GamePlayer]  ";

	// Token: 0x04003899 RID: 14489
	private const string preErr = "[GamePlayer]  ERROR!!!  ";

	// Token: 0x0400389A RID: 14490
	public VRRig rig;

	// Token: 0x0400389B RID: 14491
	public Transform leftHand;

	// Token: 0x0400389C RID: 14492
	public Transform rightHand;

	// Token: 0x0400389D RID: 14493
	public SuperInfectionSnapPointManager snapPointManager;

	// Token: 0x0400389E RID: 14494
	private readonly Transform[] handTransforms = new Transform[2];

	// Token: 0x0400389F RID: 14495
	private readonly GamePlayer.SlotData[] slots = new GamePlayer.SlotData[4];

	// Token: 0x040038A0 RID: 14496
	public const int MAX_HANDS = 2;

	// Token: 0x040038A1 RID: 14497
	public const int LEFT_HAND = 0;

	// Token: 0x040038A2 RID: 14498
	public const int RIGHT_HAND = 1;

	// Token: 0x040038A3 RID: 14499
	public const int GRAB_SLOT_FIRST = 0;

	// Token: 0x040038A4 RID: 14500
	public const int GRAB_SLOT_LAST = 1;

	// Token: 0x040038A5 RID: 14501
	public const int SNAP_SLOTS_COUNT = 2;

	// Token: 0x040038A6 RID: 14502
	public const int SNAP_SLOTS_FIRST = 2;

	// Token: 0x040038A7 RID: 14503
	public const int SNAP_SLOTS_LAST = 3;

	// Token: 0x040038A8 RID: 14504
	public const int SNAP_SLOT_HAND_L = 2;

	// Token: 0x040038A9 RID: 14505
	public const int SNAP_SLOT_HAND_R = 3;

	// Token: 0x040038AA RID: 14506
	public const int SLOTS_COUNT = 4;

	// Token: 0x040038AB RID: 14507
	public CallLimiter newJoinZoneLimiter;

	// Token: 0x040038AC RID: 14508
	public CallLimiter netImpulseLimiter;

	// Token: 0x040038AD RID: 14509
	public CallLimiter netGrabLimiter;

	// Token: 0x040038AE RID: 14510
	public CallLimiter netThrowLimiter;

	// Token: 0x040038AF RID: 14511
	public CallLimiter netStateLimiter;

	// Token: 0x040038B0 RID: 14512
	public CallLimiter netSnapLimiter;

	// Token: 0x040038B3 RID: 14515
	private int _lastSubscriptionCheck;

	// Token: 0x040038B4 RID: 14516
	private bool _isSubscribed;

	// Token: 0x040038B5 RID: 14517
	public Action OnPlayerInitialized;

	// Token: 0x040038B6 RID: 14518
	public Action OnPlayerLeftZone;

	// Token: 0x040038B7 RID: 14519
	private bool grabbingDisabled;

	// Token: 0x040038B8 RID: 14520
	private const bool _k_MATTO__USE_STATIC_CACHE = false;

	// Token: 0x040038B9 RID: 14521
	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_actorNum_to_gamePlayer;

	// Token: 0x040038BA RID: 14522
	[OnEnterPlay_SetNull]
	private static ValueTuple<int, GamePlayer>[] lookupCache_rigInstanceId_to_gamePlayer;

	// Token: 0x040038BB RID: 14523
	[OnEnterPlay_Set(0)]
	private static int staticLookupCachesCount;

	// Token: 0x040038BC RID: 14524
	public const int INVALID_ACTOR_NUMBER = -2147483648;

	// Token: 0x020006DD RID: 1757
	public struct SlotData
	{
		// Token: 0x040038BD RID: 14525
		public GameEntityId entityId;

		// Token: 0x040038BE RID: 14526
		public GameEntityManager entityManager;
	}
}
