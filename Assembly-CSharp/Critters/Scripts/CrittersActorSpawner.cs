using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x0200131E RID: 4894
	public class CrittersActorSpawner : MonoBehaviour
	{
		// Token: 0x06007B54 RID: 31572 RVA: 0x00284474 File Offset: 0x00282674
		private void Awake()
		{
			this.spawnPoint.OnSpawnChanged += this.HandleSpawnedActor;
		}

		// Token: 0x06007B55 RID: 31573 RVA: 0x0028448D File Offset: 0x0028268D
		private void OnEnable()
		{
			if (!CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Add(this);
			}
		}

		// Token: 0x06007B56 RID: 31574 RVA: 0x002844B5 File Offset: 0x002826B5
		private void OnDisable()
		{
			if (CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Remove(this);
			}
		}

		// Token: 0x06007B57 RID: 31575 RVA: 0x002844E0 File Offset: 0x002826E0
		public void ProcessLocal()
		{
			if (!CrittersManager.instance.LocalAuthority())
			{
				return;
			}
			if (this.nextSpawnTime <= (double)Time.time)
			{
				this.nextSpawnTime = (double)(Time.time + (float)this.spawnDelay);
				if (this.currentSpawnedObject == null || !this.currentSpawnedObject.isEnabled)
				{
					this.SpawnActor();
				}
			}
			if (this.currentSpawnedObject.IsNotNull())
			{
				if (!this.currentSpawnedObject.isEnabled)
				{
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.insideSpawnerCheck.bounds.Contains(this.currentSpawnedObject.transform.position))
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.VerifySpawnAttached())
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
				}
			}
		}

		// Token: 0x06007B58 RID: 31576 RVA: 0x002845DA File Offset: 0x002827DA
		public void DoReset()
		{
			this.currentSpawnedObject = null;
		}

		// Token: 0x06007B59 RID: 31577 RVA: 0x002845E3 File Offset: 0x002827E3
		private void HandleSpawnedActor(CrittersActor spawnedActor)
		{
			this.currentSpawnedObject = spawnedActor;
		}

		// Token: 0x06007B5A RID: 31578 RVA: 0x002845EC File Offset: 0x002827EC
		private void SpawnActor()
		{
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(this.actorType, this.subActorIndex);
			this.spawnPoint.SetSpawnedActor(crittersActor);
			if (crittersActor.IsNull())
			{
				return;
			}
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				crittersActor.GrabbedBy(this.spawnPoint, true, default(Quaternion), default(Vector3), false);
				return;
			}
			crittersActor.MoveActor(this.spawnPoint.transform.position, this.spawnPoint.transform.rotation, false, true, true);
			crittersActor.rb.linearVelocity = Vector3.zero;
			if (this.applyImpulseOnSpawn)
			{
				crittersActor.SetImpulse();
			}
		}

		// Token: 0x06007B5B RID: 31579 RVA: 0x00284698 File Offset: 0x00282898
		private bool VerifySpawnAttached()
		{
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				CrittersActor crittersActor;
				CrittersManager.instance.actorById.TryGetValue(this.currentSpawnedObject.parentActorId, out crittersActor);
				if (crittersActor.IsNull() || crittersActor != this.spawnPoint)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04008CB5 RID: 36021
		public CrittersActorSpawnerPoint spawnPoint;

		// Token: 0x04008CB6 RID: 36022
		public CrittersActor currentSpawnedObject;

		// Token: 0x04008CB7 RID: 36023
		public CrittersActor.CrittersActorType actorType;

		// Token: 0x04008CB8 RID: 36024
		public int subActorIndex = -1;

		// Token: 0x04008CB9 RID: 36025
		public Collider insideSpawnerCheck;

		// Token: 0x04008CBA RID: 36026
		public int spawnDelay = 5;

		// Token: 0x04008CBB RID: 36027
		public bool applyImpulseOnSpawn = true;

		// Token: 0x04008CBC RID: 36028
		public bool attachSpawnedObjectToSpawnLocation;

		// Token: 0x04008CBD RID: 36029
		private double nextSpawnTime;
	}
}
