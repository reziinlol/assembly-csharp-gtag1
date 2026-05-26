using System;
using UnityEngine;

// Token: 0x020007E9 RID: 2025
public class GRSummonerEgg : MonoBehaviour
{
	// Token: 0x060033BE RID: 13246 RVA: 0x0011CDAA File Offset: 0x0011AFAA
	private void Awake()
	{
		this.summonedEntity = base.GetComponent<GRSummonedEntity>();
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x0011CDB8 File Offset: 0x0011AFB8
	private void Start()
	{
		this.hatchTime = Random.Range(this.minHatchTime, this.maxHatchTime);
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = false;
			component.position = base.transform.position;
			component.rotation = base.transform.rotation;
			component.linearVelocity = Vector3.up * 2f;
			component.angularVelocity = Vector3.zero;
			component.constraints |= (RigidbodyConstraints)10;
		}
		base.Invoke("HatchEgg", this.hatchTime);
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x0011CE58 File Offset: 0x0011B058
	public void HatchEgg()
	{
		GRBreakable component = base.GetComponent<GRBreakable>();
		if (component)
		{
			component.BreakLocal();
		}
		if (this.entity.IsAuthority())
		{
			Vector3 position = this.entity.transform.position + this.spawnOffset;
			Quaternion identity = Quaternion.identity;
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntity gameEntity = this.entityPrefabToSpawn;
			if (this.lootTableToSpawn != null)
			{
				this.lootTableToSpawn.TryForRandomItem(this.entity, out gameEntity, 0);
			}
			gameEntityManager.RequestCreateItem(gameEntity.name.GetStaticHash(), position, identity, 0L, (this.summonedEntity != null) ? this.summonedEntity.GetSummonerID() : GameEntityId.Invalid);
		}
		base.Invoke("DestroySelf", 2f);
		this.hatchSound.Play(this.hatchAudio);
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x0011CF3C File Offset: 0x0011B13C
	public void DestroySelf()
	{
		if (this.entity.IsAuthority())
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x04004371 RID: 17265
	public GameEntity entity;

	// Token: 0x04004372 RID: 17266
	public AudioSource hatchAudio;

	// Token: 0x04004373 RID: 17267
	public AbilitySound hatchSound;

	// Token: 0x04004374 RID: 17268
	public GameEntity entityPrefabToSpawn;

	// Token: 0x04004375 RID: 17269
	public GRBreakableItemSpawnConfig lootTableToSpawn;

	// Token: 0x04004376 RID: 17270
	public Vector3 spawnOffset = new Vector3(0f, 0f, 0.3f);

	// Token: 0x04004377 RID: 17271
	public float minHatchTime = 3f;

	// Token: 0x04004378 RID: 17272
	public float maxHatchTime = 6f;

	// Token: 0x04004379 RID: 17273
	private float hatchTime = 2f;

	// Token: 0x0400437A RID: 17274
	private GRSummonedEntity summonedEntity;
}
