using System;
using Critters.Scripts;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class CrittersActorSpawnerShim : MonoBehaviour
{
	// Token: 0x06000197 RID: 407 RVA: 0x00009F38 File Offset: 0x00008138
	[ContextMenu("Copy Spawner Data To Shim")]
	private CrittersActorSpawner CopySpawnerDataInPrefab()
	{
		CrittersActorSpawner component = base.gameObject.GetComponent<CrittersActorSpawner>();
		this.spawnerPointTransform = component.spawnPoint.transform;
		this.actorType = component.actorType;
		this.subActorIndex = component.subActorIndex;
		this.insideSpawnerBounds = (BoxCollider)component.insideSpawnerCheck;
		this.spawnDelay = component.spawnDelay;
		this.applyImpulseOnSpawn = component.applyImpulseOnSpawn;
		this.attachSpawnedObjectToSpawnLocation = component.attachSpawnedObjectToSpawnLocation;
		this.colliderTrigger = base.gameObject.GetComponent<BoxCollider>();
		return component;
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00009FC4 File Offset: 0x000081C4
	[ContextMenu("Replace Spawner With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersActorSpawner crittersActorSpawner = this.CopySpawnerDataInPrefab();
		if (crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersActorSpawner.spawnPoint);
		Object.DestroyImmediate(crittersActorSpawner);
	}

	// Token: 0x040001AD RID: 429
	public Transform spawnerPointTransform;

	// Token: 0x040001AE RID: 430
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x040001AF RID: 431
	public int subActorIndex;

	// Token: 0x040001B0 RID: 432
	public BoxCollider insideSpawnerBounds;

	// Token: 0x040001B1 RID: 433
	public int spawnDelay;

	// Token: 0x040001B2 RID: 434
	public bool applyImpulseOnSpawn;

	// Token: 0x040001B3 RID: 435
	public bool attachSpawnedObjectToSpawnLocation;

	// Token: 0x040001B4 RID: 436
	public BoxCollider colliderTrigger;
}
