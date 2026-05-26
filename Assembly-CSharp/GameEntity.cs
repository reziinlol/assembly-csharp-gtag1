using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using GorillaTag.Gravity;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006AE RID: 1710
public class GameEntity : MonoBehaviour
{
	// Token: 0x17000435 RID: 1077
	// (get) Token: 0x06002A9A RID: 10906 RVA: 0x000E4973 File Offset: 0x000E2B73
	// (set) Token: 0x06002A9B RID: 10907 RVA: 0x000E497B File Offset: 0x000E2B7B
	[DebugReadout]
	public GameEntityId id { get; internal set; }

	// Token: 0x17000436 RID: 1078
	// (get) Token: 0x06002A9C RID: 10908 RVA: 0x000E4984 File Offset: 0x000E2B84
	// (set) Token: 0x06002A9D RID: 10909 RVA: 0x000E498C File Offset: 0x000E2B8C
	[DebugReadout]
	public int typeId { get; internal set; }

	// Token: 0x17000437 RID: 1079
	// (get) Token: 0x06002A9E RID: 10910 RVA: 0x000E4995 File Offset: 0x000E2B95
	// (set) Token: 0x06002A9F RID: 10911 RVA: 0x000E499D File Offset: 0x000E2B9D
	[DebugReadout]
	public long createData { get; set; }

	// Token: 0x17000438 RID: 1080
	// (get) Token: 0x06002AA0 RID: 10912 RVA: 0x000E49A6 File Offset: 0x000E2BA6
	// (set) Token: 0x06002AA1 RID: 10913 RVA: 0x000E49AE File Offset: 0x000E2BAE
	[DebugReadout]
	public GameEntityId createdByEntityId { get; set; }

	// Token: 0x17000439 RID: 1081
	// (get) Token: 0x06002AA2 RID: 10914 RVA: 0x000E49B7 File Offset: 0x000E2BB7
	// (set) Token: 0x06002AA3 RID: 10915 RVA: 0x000E49BF File Offset: 0x000E2BBF
	[DebugReadout]
	public int heldByActorNumber { get; internal set; }

	// Token: 0x1700043A RID: 1082
	// (get) Token: 0x06002AA4 RID: 10916 RVA: 0x000E49C8 File Offset: 0x000E2BC8
	// (set) Token: 0x06002AA5 RID: 10917 RVA: 0x000E49D0 File Offset: 0x000E2BD0
	[DebugReadout]
	public int snappedByActorNumber { get; internal set; }

	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x06002AA6 RID: 10918 RVA: 0x000E49D9 File Offset: 0x000E2BD9
	[DebugReadout]
	public int slotIndex
	{
		get
		{
			if (this.heldByHandIndex == -1)
			{
				return GameSnappable.GetJointToSnapIndex(this.snappedJoint);
			}
			return this.heldByHandIndex;
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x06002AA7 RID: 10919 RVA: 0x000E49F6 File Offset: 0x000E2BF6
	// (set) Token: 0x06002AA8 RID: 10920 RVA: 0x000E49FE File Offset: 0x000E2BFE
	[DebugReadout]
	public SnapJointType snappedJoint { get; internal set; }

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06002AA9 RID: 10921 RVA: 0x000E4A07 File Offset: 0x000E2C07
	// (set) Token: 0x06002AAA RID: 10922 RVA: 0x000E4A0F File Offset: 0x000E2C0F
	[DebugReadout]
	public int heldByHandIndex { get; internal set; }

	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06002AAB RID: 10923 RVA: 0x000E4A18 File Offset: 0x000E2C18
	// (set) Token: 0x06002AAC RID: 10924 RVA: 0x000E4A20 File Offset: 0x000E2C20
	[DebugReadout]
	public int lastHeldByActorNumber { get; internal set; }

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x06002AAD RID: 10925 RVA: 0x000E4A29 File Offset: 0x000E2C29
	// (set) Token: 0x06002AAE RID: 10926 RVA: 0x000E4A31 File Offset: 0x000E2C31
	[DebugReadout]
	public int onlyGrabActorNumber { get; internal set; }

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x06002AAF RID: 10927 RVA: 0x000E4A3A File Offset: 0x000E2C3A
	// (set) Token: 0x06002AB0 RID: 10928 RVA: 0x000E4A42 File Offset: 0x000E2C42
	[DebugReadout]
	public GameEntityId attachedToEntityId { get; internal set; }

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x06002AB1 RID: 10929 RVA: 0x000E4A4B File Offset: 0x000E2C4B
	// (set) Token: 0x06002AB2 RID: 10930 RVA: 0x000E4A53 File Offset: 0x000E2C53
	public bool IsScenePlaced { get; internal set; }

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x06002AB3 RID: 10931 RVA: 0x000E4A5C File Offset: 0x000E2C5C
	// (remove) Token: 0x06002AB4 RID: 10932 RVA: 0x000E4A94 File Offset: 0x000E2C94
	public event GameEntity.StateChangedEvent OnStateChanged;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06002AB5 RID: 10933 RVA: 0x000E4ACC File Offset: 0x000E2CCC
	// (remove) Token: 0x06002AB6 RID: 10934 RVA: 0x000E4B04 File Offset: 0x000E2D04
	public event GameEntity.EntityDestroyedEvent onEntityDestroyed;

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000E4B3C File Offset: 0x000E2D3C
	private void Awake()
	{
		this.id = GameEntityId.Invalid;
		this.rigidBody = base.GetComponent<Rigidbody>();
		if (this.gravityController == null)
		{
			this.gravityController = base.GetComponent<MonkeGravityController>();
			if (this.gravityController == null)
			{
				this.gravityController = base.gameObject.AddComponent<MonkeGravityController>();
			}
		}
		this.heldByActorNumber = -1;
		this.heldByHandIndex = -1;
		this.onlyGrabActorNumber = -1;
		this.snappedByActorNumber = -1;
		this.attachedToEntityId = GameEntityId.Invalid;
		this.entityComponents = new List<IGameEntityComponent>(1);
		base.GetComponentsInChildren<IGameEntityComponent>(this.entityComponents);
		this.entitySerialize = new List<IGameEntitySerialize>(1);
		base.GetComponentsInChildren<IGameEntitySerialize>(this.entitySerialize);
		if (this.builtInEntities != null)
		{
			for (int i = 0; i < this.builtInEntities.Count; i++)
			{
				this.builtInEntities[i].isBuiltIn = true;
			}
		}
		XSceneRefTarget xsceneRefTarget;
		if (base.TryGetComponent<XSceneRefTarget>(out xsceneRefTarget) && xsceneRefTarget.UniqueID > 0)
		{
			this.IsScenePlaced = true;
			GameEntityManager.RegisterScenePlacedEntity(this);
		}
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x000E4C40 File Offset: 0x000E2E40
	private void Start()
	{
		if (this.IsScenePlaced && !PhotonNetwork.InRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x000E4C60 File Offset: 0x000E2E60
	public void Create(GameEntityManager manager, int netId, int typeId)
	{
		this.manager = manager;
		this.typeId = typeId;
		if (this.builtInEntities != null)
		{
			bool flag = netId < -1 && netId != int.MinValue;
			for (int i = 0; i < this.builtInEntities.Count; i++)
			{
				int netId2 = flag ? (netId - 1 - i) : (netId + 1 + i);
				manager.AddGameEntity(netId2, this.builtInEntities[i]);
				this.builtInEntities[i].Create(manager, netId2, -1);
			}
		}
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x000E4CE4 File Offset: 0x000E2EE4
	public void Init(long createData, int createdByEntityNetId)
	{
		this.createData = createData;
		this.createdByEntityId = this.manager.GetEntityIdFromNetId(createdByEntityNetId);
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityInit();
		}
		for (int j = 0; j < this.builtInEntities.Count; j++)
		{
			this.builtInEntities[j].Init(0L, -1);
		}
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000E4D5C File Offset: 0x000E2F5C
	public void OnDestroy()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < this.entityComponents.Count; i++)
		{
			this.entityComponents[i].OnEntityDestroy();
		}
		GameEntity.EntityDestroyedEvent entityDestroyedEvent = this.onEntityDestroyed;
		if (entityDestroyedEvent != null)
		{
			entityDestroyedEvent(this);
		}
		if (this.IsScenePlaced)
		{
			GameEntityManager.UnregisterScenePlacedEntity(this);
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000E4DB8 File Offset: 0x000E2FB8
	public GameEntity.RendererSet GetGrabbableRenderers()
	{
		if (this._grabbableRenderers == null)
		{
			this._grabbableRenderers = new GameEntity.RendererSet();
			this._meshFilters = new List<MeshFilter>();
			base.GetComponentsInChildren<MeshFilter>(true, this._meshFilters);
			base.GetComponentsInChildren<SkinnedMeshRenderer>(true, this._grabbableRenderers.skinnedRenderers);
			List<SkinnedMeshRenderer> skinnedRenderers = this._grabbableRenderers.skinnedRenderers;
			this.<GetGrabbableRenderers>g__RemoveNotOwnedComponents|103_0<MeshFilter>(this._meshFilters);
			this.<GetGrabbableRenderers>g__RemoveNotOwnedComponents|103_0<SkinnedMeshRenderer>(skinnedRenderers);
			foreach (GameObject y in this.ignoreObjectGrabRenderers)
			{
				for (int j = 0; j < this._meshFilters.Count; j++)
				{
					if (this._meshFilters[j].gameObject == y)
					{
						this._meshFilters.RemoveAtSwapBack(j--);
					}
				}
				for (int k = 0; k < skinnedRenderers.Count; k++)
				{
					if (skinnedRenderers[k].gameObject == y)
					{
						skinnedRenderers.RemoveAtSwapBack(k--);
					}
				}
			}
			foreach (MeshFilter meshFilter in this._meshFilters)
			{
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				if (component != null)
				{
					this._grabbableRenderers.renderers.Add(new ValueTuple<MeshFilter, MeshRenderer>(meshFilter, component));
				}
			}
		}
		return this._grabbableRenderers;
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x000E4F30 File Offset: 0x000E3130
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.linearVelocity;
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000E4F54 File Offset: 0x000E3154
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.GTPlayOneShot(this.catchSound, 1f);
		}
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000E4FA4 File Offset: 0x000E31A4
	public void PlayThrowFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.GTPlayOneShot(this.throwSound, 1f);
		}
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000E4FF4 File Offset: 0x000E31F4
	public void PlaySnapFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.volume = this.snapSoundVolume;
			this.audioSource.GTPlayOneShot(this.snapSound, 1f);
		}
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000E5043 File Offset: 0x000E3243
	private bool IsGamePlayer(Collider collider)
	{
		return GamePlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000E5052 File Offset: 0x000E3252
	public long GetState()
	{
		return this.state;
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x000E505A File Offset: 0x000E325A
	public void RequestState(GameEntityId id, long newState)
	{
		this.manager.RequestState(id, newState);
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000E5069 File Offset: 0x000E3269
	public bool IsAuthority()
	{
		return this.manager.IsAuthority();
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000E5076 File Offset: 0x000E3276
	public bool IsValidToMigrate()
	{
		return this.manager.IsEntityValidToMigrate(this);
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000E5084 File Offset: 0x000E3284
	public void SetState(long newState)
	{
		if (this.state != newState)
		{
			long prevState = this.state;
			this.state = newState;
			GameEntity.StateChangedEvent onStateChanged = this.OnStateChanged;
			if (onStateChanged != null)
			{
				onStateChanged(prevState, newState);
			}
			for (int i = 0; i < this.entityComponents.Count; i++)
			{
				this.entityComponents[i].OnEntityStateChange(prevState, newState);
			}
		}
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x000E50E4 File Offset: 0x000E32E4
	public GameEntityId MigrateToEntityManager(GameEntityManager newManager)
	{
		if (this.IsScenePlaced)
		{
			if (this.manager != null)
			{
				this.manager.ReleaseScenePlacedHold(this);
			}
			return this.id;
		}
		this.manager.RemoveGameEntity(this);
		this.manager = newManager;
		GameEntityId gameEntityId = newManager.AddGameEntity(this);
		this.id = gameEntityId;
		this.manager.InitItemLocal(this, this.createData, -1);
		return gameEntityId;
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x000E5150 File Offset: 0x000E3350
	public void MigrateHeldBy(int actorNumber)
	{
		if (this.heldByActorNumber >= 0)
		{
			this.heldByActorNumber = actorNumber;
		}
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x000E5162 File Offset: 0x000E3362
	public void MigrateSnappedBy(int actorNumber)
	{
		if (this.snappedByActorNumber >= 0)
		{
			this.snappedByActorNumber = actorNumber;
		}
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x000E5174 File Offset: 0x000E3374
	public int GetNetId(GameEntityId gameEntityId)
	{
		return this.manager.GetNetIdFromEntityId(gameEntityId);
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x000E5182 File Offset: 0x000E3382
	public int GetNetId()
	{
		return this.manager.GetNetIdFromEntityId(this.id);
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x000E5198 File Offset: 0x000E3398
	public static GameEntity Get(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameEntity component = transform.GetComponent<GameEntity>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x000E51DC File Offset: 0x000E33DC
	public bool IsHeldByLocalPlayer()
	{
		return this.heldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002ACE RID: 10958 RVA: 0x000E51F5 File Offset: 0x000E33F5
	public bool IsSnappedByLocalPlayer()
	{
		return this.snappedByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06002ACF RID: 10959 RVA: 0x000E520E File Offset: 0x000E340E
	public bool IsHeldOrSnappedByLocalPlayer
	{
		get
		{
			return this.AttachedPlayerActorNr == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06002AD0 RID: 10960 RVA: 0x000E5227 File Offset: 0x000E3427
	public bool IsHeld()
	{
		return this.heldByActorNumber != -1;
	}

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x06002AD1 RID: 10961 RVA: 0x000E5235 File Offset: 0x000E3435
	public bool IsSnappedToHand
	{
		get
		{
			return (this.snappedJoint & (SnapJointType.HandL | SnapJointType.HandR)) > SnapJointType.None;
		}
	}

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x06002AD2 RID: 10962 RVA: 0x000E5242 File Offset: 0x000E3442
	public int AttachedPlayerActorNr
	{
		get
		{
			if (this.heldByActorNumber == -1)
			{
				return this.snappedByActorNumber;
			}
			return this.heldByActorNumber;
		}
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000E525C File Offset: 0x000E345C
	public int GetLastHeldByPlayerForEntityID(GameEntityId gameEntityId)
	{
		GameEntity gameEntity = this.manager.GetGameEntity(gameEntityId);
		if (gameEntity != null)
		{
			return gameEntity.lastHeldByActorNumber;
		}
		return 0;
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x000E5287 File Offset: 0x000E3487
	public bool WasLastHeldByLocalPlayer()
	{
		return this.lastHeldByActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000E52A0 File Offset: 0x000E34A0
	public bool IsAttachedToPlayer(NetPlayer player)
	{
		return player != null && (this.heldByActorNumber == player.ActorNumber || this.snappedByActorNumber == player.ActorNumber);
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x06002AD6 RID: 10966 RVA: 0x000E52C5 File Offset: 0x000E34C5
	public int EquippedSlotIndex
	{
		get
		{
			if (this.heldByHandIndex != -1)
			{
				return this.heldByHandIndex;
			}
			if ((this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return 2;
			}
			if ((this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return -1;
			}
			return 3;
		}
	}

	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x06002AD7 RID: 10967 RVA: 0x000E52F0 File Offset: 0x000E34F0
	public EHandedness EquippedHandedness
	{
		get
		{
			if (this.heldByHandIndex == 0 || (this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return EHandedness.Left;
			}
			if (this.heldByHandIndex != 1 && (this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return EHandedness.None;
			}
			return EHandedness.Right;
		}
	}

	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06002AD8 RID: 10968 RVA: 0x000E531C File Offset: 0x000E351C
	public XRNode EquippedHandXRNode
	{
		get
		{
			if (this.heldByHandIndex == 0 || (this.snappedJoint & SnapJointType.HandL) != SnapJointType.None)
			{
				return XRNode.LeftHand;
			}
			if (this.heldByHandIndex != 1 && (this.snappedJoint & SnapJointType.HandR) == SnapJointType.None)
			{
				return (XRNode)(-1);
			}
			return XRNode.RightHand;
		}
	}

	// Token: 0x06002ADA RID: 10970 RVA: 0x000E5378 File Offset: 0x000E3578
	[CompilerGenerated]
	private void <GetGrabbableRenderers>g__RemoveNotOwnedComponents|103_0<T>(List<T> components) where T : Component
	{
		for (int i = 0; i < components.Count; i++)
		{
			if (this.manager.GetParentEntity<GameEntity>(components[i].transform) != this)
			{
				components.RemoveAtSwapBack(i--);
			}
		}
	}

	// Token: 0x04003747 RID: 14151
	public const int Invalid = -1;

	// Token: 0x04003748 RID: 14152
	public const int ScenePlacedTypeId = -2147483647;

	// Token: 0x0400374D RID: 14157
	public List<GameEntity> builtInEntities;

	// Token: 0x0400374E RID: 14158
	[NonSerialized]
	public bool isBuiltIn;

	// Token: 0x0400374F RID: 14159
	public bool pickupable = true;

	// Token: 0x04003750 RID: 14160
	public float pickupRangeFromSurface;

	// Token: 0x04003751 RID: 14161
	[Tooltip("Renderers on these objects are ignored when determining grab bounds")]
	public GameObject[] ignoreObjectGrabRenderers;

	// Token: 0x04003752 RID: 14162
	public bool canHoldingPlayerUpdateState;

	// Token: 0x04003753 RID: 14163
	public bool canLastHoldingPlayerUpdateState;

	// Token: 0x04003754 RID: 14164
	public bool canSnapPlayerUpdateState;

	// Token: 0x04003755 RID: 14165
	public AudioSource audioSource;

	// Token: 0x04003756 RID: 14166
	public AudioClip catchSound;

	// Token: 0x04003757 RID: 14167
	public float catchSoundVolume = 0.5f;

	// Token: 0x04003758 RID: 14168
	public AudioClip throwSound;

	// Token: 0x04003759 RID: 14169
	public float throwSoundVolume = 0.5f;

	// Token: 0x0400375A RID: 14170
	public AudioClip snapSound;

	// Token: 0x0400375B RID: 14171
	public float snapSoundVolume = 0.5f;

	// Token: 0x0400375C RID: 14172
	private Rigidbody rigidBody;

	// Token: 0x0400375D RID: 14173
	[SerializeField]
	public MonkeGravityController gravityController;

	// Token: 0x04003765 RID: 14181
	[NonSerialized]
	public GameEntityManager manager;

	// Token: 0x04003766 RID: 14182
	internal bool shouldDestroyOnZoneExit;

	// Token: 0x04003768 RID: 14184
	[NonSerialized]
	internal bool scenePlacedInitialized;

	// Token: 0x04003769 RID: 14185
	[NonSerialized]
	internal Vector3 scenePlacedHomePosition;

	// Token: 0x0400376A RID: 14186
	[NonSerialized]
	internal Quaternion scenePlacedHomeRotation;

	// Token: 0x0400376B RID: 14187
	[NonSerialized]
	internal float scenePlacedHomeScale;

	// Token: 0x0400376C RID: 14188
	public Action OnGrabbed;

	// Token: 0x0400376D RID: 14189
	public Action OnReleased;

	// Token: 0x0400376E RID: 14190
	public Action OnSnapped;

	// Token: 0x0400376F RID: 14191
	public Action OnUnsnapped;

	// Token: 0x04003770 RID: 14192
	public Action OnAttached;

	// Token: 0x04003771 RID: 14193
	public Action OnDetached;

	// Token: 0x04003772 RID: 14194
	public Action OnTick;

	// Token: 0x04003773 RID: 14195
	public float MinTimeBetweenTicks;

	// Token: 0x04003774 RID: 14196
	[NonSerialized]
	public float LastTickTime;

	// Token: 0x04003777 RID: 14199
	private long state;

	// Token: 0x04003778 RID: 14200
	private List<IGameEntityComponent> entityComponents;

	// Token: 0x04003779 RID: 14201
	public List<IGameEntitySerialize> entitySerialize;

	// Token: 0x0400377A RID: 14202
	private GameEntity.RendererSet _grabbableRenderers;

	// Token: 0x0400377B RID: 14203
	private List<MeshFilter> _meshFilters;

	// Token: 0x020006AF RID: 1711
	public class RendererSet
	{
		// Token: 0x0400377C RID: 14204
		[TupleElementNames(new string[]
		{
			"filter",
			"renderer"
		})]
		public List<ValueTuple<MeshFilter, MeshRenderer>> renderers = new List<ValueTuple<MeshFilter, MeshRenderer>>();

		// Token: 0x0400377D RID: 14205
		public List<SkinnedMeshRenderer> skinnedRenderers = new List<SkinnedMeshRenderer>();
	}

	// Token: 0x020006B0 RID: 1712
	// (Invoke) Token: 0x06002ADD RID: 10973
	public delegate void StateChangedEvent(long prevState, long nextState);

	// Token: 0x020006B1 RID: 1713
	// (Invoke) Token: 0x06002AE1 RID: 10977
	public delegate void EntityDestroyedEvent(GameEntity entity);
}
