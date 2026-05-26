using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020007A7 RID: 1959
public class GRHazardTower : MonoBehaviour, IGameEntityComponent, IGameProjectileLauncher
{
	// Token: 0x06003206 RID: 12806 RVA: 0x00112FD4 File Offset: 0x001111D4
	public void OnEntityInit()
	{
		this.gameEntity.MinTimeBetweenTicks = 0.5f;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnThink));
		this.senseNearby.Setup(this.fireFrom, this.gameEntity);
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003208 RID: 12808 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003209 RID: 12809 RVA: 0x00113030 File Offset: 0x00111230
	public void OnThink()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.nextFireTime)
		{
			return;
		}
		GRHazardTower.tempRigs.Clear();
		GRHazardTower.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRHazardTower.tempRigs);
		this.senseNearby.UpdateNearby(GRHazardTower.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (vrrig == null)
		{
			return;
		}
		Vector3 vector = vrrig.transform.position;
		Vector3 b = Vector3.up * 0.1f;
		vector += b;
		GhostReactorManager.Get(this.gameEntity).RequestFireProjectile(this.gameEntity.id, this.fireFrom.position, vector, PhotonNetwork.Time + 0.0);
		this.nextFireTime = timeAsDouble + (double)this.fireCooldownTime;
	}

	// Token: 0x0600320A RID: 12810 RVA: 0x0011311C File Offset: 0x0011131C
	public void OnFire(Vector3 fireFromPos, Vector3 fireAtPos, double fireAtTime)
	{
		Vector3 forward;
		if (this.gameEntity.IsAuthority() && GREnemyRanged.CalculateLaunchDirection(fireFromPos, fireAtPos, this.projectileSpeed, out forward))
		{
			this.gameEntity.manager.RequestCreateItem(this.projectilePrefab.name.GetStaticHash(), fireFromPos, Quaternion.LookRotation(forward, Vector3.up), (long)this.gameEntity.GetNetId());
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextFireTime = timeAsDouble + (double)this.fireCooldownTime;
	}

	// Token: 0x0600320B RID: 12811 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
	}

	// Token: 0x0600320C RID: 12812 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}

	// Token: 0x040040F6 RID: 16630
	public GameEntity gameEntity;

	// Token: 0x040040F7 RID: 16631
	public GRSenseNearby senseNearby;

	// Token: 0x040040F8 RID: 16632
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x040040F9 RID: 16633
	public float projectileSpeed;

	// Token: 0x040040FA RID: 16634
	public GameEntity projectilePrefab;

	// Token: 0x040040FB RID: 16635
	public Transform fireFrom;

	// Token: 0x040040FC RID: 16636
	public float fireChargeTime;

	// Token: 0x040040FD RID: 16637
	public float fireCooldownTime;

	// Token: 0x040040FE RID: 16638
	private double nextFireTime;

	// Token: 0x040040FF RID: 16639
	private static List<VRRig> tempRigs = new List<VRRig>(16);
}
