using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class SIBlasterSprayProjectile : MonoBehaviour
{
	// Token: 0x0600051C RID: 1308 RVA: 0x0001CC19 File Offset: 0x0001AE19
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0001CC28 File Offset: 0x0001AE28
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x0001CCEC File Offset: 0x0001AEEC
	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		float num = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			vector,
			playerHit.ActorNr
		});
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001CDB4 File Offset: 0x0001AFB4
	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 4)
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
		Vector3 vector2;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector2))
		{
			return;
		}
		int actorNumber;
		if (!GameEntityManager.ValidateDataType<int>(data[3], out actorNumber))
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
			Object.Instantiate<GameObject>(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		GTPlayer.Instance.ApplyKnockback(vector2.normalized, this.knockbackSpeed, true);
	}

	// Token: 0x040005E0 RID: 1504
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005E1 RID: 1505
	public float knockbackSpeed;

	// Token: 0x040005E2 RID: 1506
	public float verticalOffset = -0.133f;

	// Token: 0x040005E3 RID: 1507
	public float upwardsAngle = 30f;
}
