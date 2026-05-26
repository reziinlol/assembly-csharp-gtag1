using System;
using UnityEngine;

// Token: 0x020007C6 RID: 1990
public class GRRangedEnemyProjectile : MonoBehaviour, IGameEntityComponent, IGameHittable, IGameHitter
{
	// Token: 0x060032B1 RID: 12977 RVA: 0x00115FC4 File Offset: 0x001141C4
	private void Awake()
	{
		this.particleSystem = base.GetComponentInChildren<ParticleSystem>();
		this.audioSource = base.GetComponentInChildren<AudioSource>();
		this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
		this.hittable = base.GetComponentInChildren<GameHittable>();
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x0011601C File Offset: 0x0011421C
	private void Start()
	{
		if (this.projectileRigidbody != null)
		{
			this.projectileRigidbody.linearVelocity = base.transform.forward * this.projectileSpeed;
		}
		this.projectileHasImpacted = false;
		if (this.owningEntity != null)
		{
			Collider componentInChildren = base.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				Collider[] componentsInChildren = this.owningEntity.gameObject.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Physics.IgnoreCollision(componentInChildren, componentsInChildren[i]);
				}
			}
		}
	}

	// Token: 0x060032B3 RID: 12979 RVA: 0x001160A8 File Offset: 0x001142A8
	private void Update()
	{
		if (this.entity.IsAuthority() && this.projectileHasImpacted && Time.timeAsDouble > this.projectileImpactTime + (double)this.postImpactLifetime)
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x060032B4 RID: 12980 RVA: 0x001160FC File Offset: 0x001142FC
	public void OnEntityInit()
	{
		this.owningEntityNetID = (int)this.entity.createData;
		if (this.owningEntityNetID != 0)
		{
			this.owningEntity = this.FindOwningEntity();
			this.projectileLauncher = this.owningEntity.GetComponent<IGameProjectileLauncher>();
			if (this.projectileLauncher != null)
			{
				this.projectileLauncher.OnProjectileInit(this);
			}
		}
	}

	// Token: 0x060032B5 RID: 12981 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060032B6 RID: 12982 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060032B7 RID: 12983 RVA: 0x00116154 File Offset: 0x00114354
	private GameEntity FindOwningEntity()
	{
		if (this.owningEntityNetID != 0)
		{
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(this.owningEntityNetID);
			return gameEntityManager.GetGameEntity(entityIdFromNetId);
		}
		return null;
	}

	// Token: 0x060032B8 RID: 12984 RVA: 0x00116190 File Offset: 0x00114390
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.projectileHasImpacted)
		{
			if (this.canHitPlayer)
			{
				Vector3 position = base.transform.position;
				if ((VRRig.LocalRig.GetMouthPosition() - position).sqrMagnitude < this.projectileHitRadius * this.projectileHitRadius && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
				{
					this.lastHitPlayerTime = Time.time;
					GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Ranged, this.entity.id, VRRig.LocalRig.GetComponent<GRPlayer>(), position);
				}
				if (this.projectileLauncher != null)
				{
					this.projectileLauncher.OnProjectileHit(this, collision);
				}
			}
			this.projectileHasImpacted = true;
			this.projectileImpactTime = Time.timeAsDouble;
		}
	}

	// Token: 0x060032B9 RID: 12985 RVA: 0x00116254 File Offset: 0x00114454
	private void OnTriggerEnter(Collider collider)
	{
		if (!this.projectileHasImpacted)
		{
			GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
			if (component != null)
			{
				component.BlockHittable(this.projectileRigidbody.transform.position, this.projectileRigidbody.linearVelocity.normalized, this.hittable);
			}
		}
	}

	// Token: 0x060032BA RID: 12986 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060032BB RID: 12987 RVA: 0x001162A8 File Offset: 0x001144A8
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060032BC RID: 12988 RVA: 0x0011630C File Offset: 0x0011450C
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if (this.projectileRigidbody != null)
		{
			this.PlayImpactFX();
			this.projectileRigidbody.linearVelocity = hit.hitImpulse * (this.projectileRigidbody.linearVelocity.magnitude * 0.7f);
		}
	}

	// Token: 0x060032BD RID: 12989 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x060032BE RID: 12990 RVA: 0x0011636E File Offset: 0x0011456E
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if (this.projectileRigidbody != null)
		{
			this.PlayImpactFX();
			this.projectileRigidbody.linearVelocity = hit.hitImpulse;
		}
	}

	// Token: 0x060032BF RID: 12991 RVA: 0x001163A7 File Offset: 0x001145A7
	private void PlayImpactFX()
	{
		if (this.particleSystem != null)
		{
			this.particleSystem.Play();
		}
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x060032C0 RID: 12992 RVA: 0x001163DC File Offset: 0x001145DC
	public void OnSuccessfulHit(GameHitData hit)
	{
		this.PlayImpactFX();
	}

	// Token: 0x060032C1 RID: 12993 RVA: 0x001163E4 File Offset: 0x001145E4
	public void OnSuccessfulHitPlayer(GRPlayer player, Vector3 hitPosition)
	{
		this.PlayImpactFX();
		this.hitSFX.Play(null);
		if (this.applyFreezeEffect)
		{
			player.SetAsFrozen(4f);
		}
	}

	// Token: 0x040041E5 RID: 16869
	private int owningEntityNetID;

	// Token: 0x040041E6 RID: 16870
	private GameEntity entity;

	// Token: 0x040041E7 RID: 16871
	public GameEntity owningEntity;

	// Token: 0x040041E8 RID: 16872
	private IGameProjectileLauncher projectileLauncher;

	// Token: 0x040041E9 RID: 16873
	public Rigidbody projectileRigidbody;

	// Token: 0x040041EA RID: 16874
	private ParticleSystem particleSystem;

	// Token: 0x040041EB RID: 16875
	private AudioSource audioSource;

	// Token: 0x040041EC RID: 16876
	private MeshRenderer meshRenderer;

	// Token: 0x040041ED RID: 16877
	private GameHittable hittable;

	// Token: 0x040041EE RID: 16878
	public float projectileSpeed = 5f;

	// Token: 0x040041EF RID: 16879
	public float projectileHitRadius = 1f;

	// Token: 0x040041F0 RID: 16880
	public float postImpactLifetime = 2f;

	// Token: 0x040041F1 RID: 16881
	private bool projectileHasImpacted;

	// Token: 0x040041F2 RID: 16882
	private double projectileImpactTime;

	// Token: 0x040041F3 RID: 16883
	private float lastHitPlayerTime;

	// Token: 0x040041F4 RID: 16884
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x040041F5 RID: 16885
	public bool applyFreezeEffect;

	// Token: 0x040041F6 RID: 16886
	public bool canHitPlayer = true;

	// Token: 0x040041F7 RID: 16887
	public AbilitySound hitSFX;
}
