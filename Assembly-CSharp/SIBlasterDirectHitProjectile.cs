using System;
using UnityEngine;

// Token: 0x020000D5 RID: 213
public class SIBlasterDirectHitProjectile : MonoBehaviour, SIGadgetProjectileType
{
	// Token: 0x0600050B RID: 1291 RVA: 0x0001C265 File Offset: 0x0001A465
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001C274 File Offset: 0x0001A474
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001C338 File Offset: 0x0001A538
	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			playerHit.ActorNr
		});
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001C3A0 File Offset: 0x0001A5A0
	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 3)
		{
			return;
		}
		int num;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		int actorNumber;
		if (!GameEntityManager.ValidateDataType<int>(data[2], out actorNumber))
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		SIPlayer x = SIPlayer.Get(actorNumber);
		if (x == null)
		{
			return;
		}
		if (x != SIPlayer.LocalPlayer)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		float num2 = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num2 - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.KnockbackWithHaptics(vector2.normalized * this.knockbackSpeed, true);
	}

	// Token: 0x040005D1 RID: 1489
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005D2 RID: 1490
	public float knockbackSpeed;

	// Token: 0x040005D3 RID: 1491
	public float upwardsAngle = 30f;
}
