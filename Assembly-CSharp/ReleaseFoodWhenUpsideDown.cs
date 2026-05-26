using System;
using Critters.Scripts;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class ReleaseFoodWhenUpsideDown : MonoBehaviour
{
	// Token: 0x06000391 RID: 913 RVA: 0x00014A91 File Offset: 0x00012C91
	private void Awake()
	{
		this.latch = false;
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00014A9C File Offset: 0x00012C9C
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.dispenser.heldByPlayer)
		{
			return;
		}
		if (Vector3.Angle(base.transform.up, Vector3.down) < this.angle)
		{
			if (this.latch)
			{
				return;
			}
			this.latch = true;
			if (this.nextSpawnTime > (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)))
			{
				return;
			}
			this.nextSpawnTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) + (double)this.spawnDelay;
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.Food, this.foodSubIndex);
			if (!crittersActor.IsNull())
			{
				CrittersFood crittersFood = (CrittersFood)crittersActor;
				crittersFood.MoveActor(this.spawnPoint.position, this.spawnPoint.rotation, false, true, true);
				crittersFood.SetImpulseVelocity(Vector3.zero, Vector3.zero);
				crittersFood.SpawnData(this.maxFood, this.startingFood, this.startingSize);
				return;
			}
		}
		else
		{
			this.latch = false;
		}
	}

	// Token: 0x04000415 RID: 1045
	public CrittersFoodDispenser dispenser;

	// Token: 0x04000416 RID: 1046
	public float angle = 30f;

	// Token: 0x04000417 RID: 1047
	private bool latch;

	// Token: 0x04000418 RID: 1048
	public Transform spawnPoint;

	// Token: 0x04000419 RID: 1049
	public float maxFood;

	// Token: 0x0400041A RID: 1050
	public float startingFood;

	// Token: 0x0400041B RID: 1051
	public float startingSize;

	// Token: 0x0400041C RID: 1052
	public int foodSubIndex;

	// Token: 0x0400041D RID: 1053
	public float spawnDelay = 0.6f;

	// Token: 0x0400041E RID: 1054
	private double nextSpawnTime;
}
