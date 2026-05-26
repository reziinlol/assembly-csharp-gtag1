using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DB RID: 219
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
[RequireComponent(typeof(SIGadgetBlasterType))]
public class SIGadgetBlaster : SIGadget, ITickSystemTick
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x06000521 RID: 1313 RVA: 0x0001CED3 File Offset: 0x0001B0D3
	public bool LocalEquippedOrActivated
	{
		get
		{
			return this.IsEquippedLocal() || this.activatedLocally;
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x06000522 RID: 1314 RVA: 0x0001CEE5 File Offset: 0x0001B0E5
	// (set) Token: 0x06000523 RID: 1315 RVA: 0x0001CEED File Offset: 0x0001B0ED
	public bool TickRunning { get; set; }

	// Token: 0x06000524 RID: 1316 RVA: 0x0001CEF8 File Offset: 0x0001B0F8
	protected override void OnEnable()
	{
		base.OnEnable();
		this.blasterType = base.GetComponent<SIGadgetBlasterType>();
		this.lastFired = 0f;
		this.environmentLayerMask = GTPlayer.Instance.locomotionEnabledLayers;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.StartGrabbing));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.StopGrabbing));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopGrabbing));
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000525 RID: 1317 RVA: 0x0001CFD4 File Offset: 0x0001B1D4
	private new void OnDisable()
	{
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0001CFE4 File Offset: 0x0001B1E4
	public void Tick()
	{
		if (this.projectilesToDespawn.Count <= 0)
		{
			return;
		}
		if (Time.time < this.projectilesToDespawnTimes.Peek() + 1f)
		{
			return;
		}
		SIGadgetBlasterProjectile sigadgetBlasterProjectile = this.projectilesToDespawn.Dequeue();
		this.activeProjectiles.RemoveIfContains(sigadgetBlasterProjectile);
		if (sigadgetBlasterProjectile == null || sigadgetBlasterProjectile.gameObject == null)
		{
			return;
		}
		SIGadgetBlaster.blasterProjectilePools[sigadgetBlasterProjectile.poolId].Add(sigadgetBlasterProjectile.gameObject);
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0001D064 File Offset: 0x0001B264
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this.blasterType.OnUpdateAuthority(dt);
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001D07C File Offset: 0x0001B27C
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetBlasterState sigadgetBlasterState = (SIGadgetBlasterState)this.gameEntity.GetState();
		if (sigadgetBlasterState != this.currentState)
		{
			this.SetStateShared(sigadgetBlasterState);
		}
		this.blasterType.OnUpdateRemote(dt);
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001D0B9 File Offset: 0x0001B2B9
	public void SetStateAuthority(SIGadgetBlasterState newState)
	{
		this.SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001D0DA File Offset: 0x0001B2DA
	private void SetStateShared(SIGadgetBlasterState newState)
	{
		if (newState == this.currentState || !SIGadgetBlaster.CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetBlasterState sigadgetBlasterState = this.currentState;
		this.currentState = newState;
		this.blasterType.SetStateShared();
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001D108 File Offset: 0x0001B308
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.blasterType.ApplyUpgradeNodes(withUpgrades);
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0001D116 File Offset: 0x0001B316
	public static bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	// Token: 0x0600052D RID: 1325 RVA: 0x0001D124 File Offset: 0x0001B324
	public bool CheckInput()
	{
		float sensitivity = this.wasActivated ? this.inputActivateThreshold : this.inputDeactivateThreshold;
		this.wasActivated = this.buttonActivatable.CheckInput(sensitivity);
		return this.wasActivated;
	}

	// Token: 0x0600052E RID: 1326 RVA: 0x0001D160 File Offset: 0x0001B360
	public int NextFireId()
	{
		int num = this.projectileId;
		this.projectileId = num + 1;
		return num;
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x0001D180 File Offset: 0x0001B380
	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID != 0)
		{
			if (rpcID != 1)
			{
				return;
			}
			if (data == null || data.Length < 2)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			SIGadgetBlasterProjectile sigadgetBlasterProjectile = null;
			for (int i = 0; i < this.activeProjectiles.Count; i++)
			{
				if (this.activeProjectiles[i].projectileId == num)
				{
					sigadgetBlasterProjectile = this.activeProjectiles[i];
					break;
				}
			}
			if (sigadgetBlasterProjectile == null)
			{
				return;
			}
			if (sigadgetBlasterProjectile.firedByPlayer != SIPlayer.Get(info.Sender.ActorNumber))
			{
				return;
			}
			sigadgetBlasterProjectile.GetComponent<SIGadgetProjectileType>().NetworkedProjectileHit(data);
			return;
		}
		else
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.blasterType.NetworkFireProjectile(data);
			return;
		}
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x0001D24B File Offset: 0x0001B44B
	public void StartGrabbing()
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.SetStateAuthority(SIGadgetBlasterState.Idle);
		}
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001D264 File Offset: 0x0001B464
	public void StopGrabbing()
	{
		this.SetStateShared(SIGadgetBlasterState.Idle);
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001D26D File Offset: 0x0001B46D
	public void DespawnProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.gameObject.SetActive(false);
		if (!this.projectilesToDespawn.Contains(projectile))
		{
			this.projectilesToDespawn.Enqueue(projectile);
			this.projectilesToDespawnTimes.Enqueue(Time.time);
		}
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001D2A8 File Offset: 0x0001B4A8
	public GameObject InstantiateProjectile(SIGadgetBlasterProjectile projectilePrefab, Vector3 position, Quaternion rotation, int thisFireId)
	{
		if (SIGadgetBlaster.blasterProjectilePools == null)
		{
			SIGadgetBlaster.blasterProjectilePools = new Dictionary<int, List<GameObject>>();
		}
		int instanceID = projectilePrefab.GetInstanceID();
		if (!SIGadgetBlaster.blasterProjectilePools.ContainsKey(instanceID))
		{
			SIGadgetBlaster.blasterProjectilePools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlaster.blasterProjectilePools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(projectilePrefab.gameObject, position, rotation);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		SIGadgetBlasterProjectile component = gameObject.GetComponent<SIGadgetBlasterProjectile>();
		component.transform.position = position;
		component.transform.rotation = rotation;
		component.parentBlaster = this;
		component.projectileId = thisFireId;
		component.firedByPlayer = (this.gameEntity.IsHeld() ? SIPlayer.Get(this.gameEntity.heldByActorNumber) : SIPlayer.Get(this.gameEntity.snappedByActorNumber));
		component.poolId = instanceID;
		this.activeProjectiles.Add(component);
		this.lastFired = Time.time;
		component.InitializeProjectile();
		return gameObject;
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001D3BB File Offset: 0x0001B5BB
	public void FireProjectileHaptics(float strength, float duration)
	{
		GorillaTagger.Instance.StartVibration(this.gameEntity.EquippedHandedness == EHandedness.Left, strength, duration);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001D3D8 File Offset: 0x0001B5D8
	public float CurrentFireRate()
	{
		int count = this.activeProjectiles.Count;
		if (count <= 1)
		{
			return 0f;
		}
		return (float)(count - 1) / (this.activeProjectiles[count - 1].timeSpawned - this.activeProjectiles[0].timeSpawned);
	}

	// Token: 0x040005E4 RID: 1508
	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectilePools;

	// Token: 0x040005E5 RID: 1509
	[NonSerialized]
	public const float PROJECTILE_MAX_LATENCY = 1f;

	// Token: 0x040005E6 RID: 1510
	private SIGadgetBlasterType blasterType;

	// Token: 0x040005E7 RID: 1511
	[NonSerialized]
	public SIGadgetBlasterState currentState;

	// Token: 0x040005E9 RID: 1513
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x040005EA RID: 1514
	[SerializeField]
	private float inputActivateThreshold = 0.35f;

	// Token: 0x040005EB RID: 1515
	[SerializeField]
	private float inputDeactivateThreshold = 0.25f;

	// Token: 0x040005EC RID: 1516
	public int maxProjectileCount = 10;

	// Token: 0x040005ED RID: 1517
	public float maxLagDistance = 5f;

	// Token: 0x040005EE RID: 1518
	private bool wasActivated;

	// Token: 0x040005EF RID: 1519
	[NonSerialized]
	public float lastFired;

	// Token: 0x040005F0 RID: 1520
	[NonSerialized]
	public int projectileCount;

	// Token: 0x040005F1 RID: 1521
	private int projectileId;

	// Token: 0x040005F2 RID: 1522
	[NonSerialized]
	public List<SIGadgetBlasterProjectile> activeProjectiles = new List<SIGadgetBlasterProjectile>();

	// Token: 0x040005F3 RID: 1523
	[NonSerialized]
	public Queue<SIGadgetBlasterProjectile> projectilesToDespawn = new Queue<SIGadgetBlasterProjectile>();

	// Token: 0x040005F4 RID: 1524
	[NonSerialized]
	public Queue<float> projectilesToDespawnTimes = new Queue<float>();

	// Token: 0x040005F5 RID: 1525
	public Transform firingPosition;

	// Token: 0x040005F6 RID: 1526
	public AudioSource firingSource;

	// Token: 0x040005F7 RID: 1527
	public AudioSource blasterSource;

	// Token: 0x040005F8 RID: 1528
	[NonSerialized]
	public LayerMask environmentLayerMask;

	// Token: 0x020000DC RID: 220
	public enum RPCCalls
	{
		// Token: 0x040005FA RID: 1530
		FireProjectile,
		// Token: 0x040005FB RID: 1531
		ProjectileHitPlayer
	}
}
