using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class SIBlasterSplashProjectile : MonoBehaviour, SIGadgetProjectileType
{
	// Token: 0x06000516 RID: 1302 RVA: 0x0001C647 File Offset: 0x0001A847
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001C658 File Offset: 0x0001A858
	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (!this.projectile.firedByPlayer == SIPlayer.LocalPlayer)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
			this.projectile.DespawnProjectile();
			return;
		}
		this.rigList.Clear();
		VRRigCache.Instance.GetActiveRigs(this.rigList);
		Vector3 position = this.projectile.transform.position;
		for (int i = this.rigList.Count - 1; i >= 0; i--)
		{
			if ((this.rigList[i].transform.position - position).magnitude < this.splashHitDistance)
			{
				Vector3 position2 = this.rigList[i].head.rigTarget.position;
				Vector3 position3 = this.rigList[i].bodyTransform.position;
				if (Physics.RaycastNonAlloc(position, position2 - position, this.hits, this.splashHitDistance, this.projectile.parentBlaster.environmentLayerMask, QueryTriggerInteraction.Ignore) != 0 && Physics.RaycastNonAlloc(position, position3 - position, this.hits, this.splashHitDistance, this.projectile.parentBlaster.environmentLayerMask, QueryTriggerInteraction.Ignore) != 0)
				{
					this.rigList.RemoveAt(i);
				}
			}
			else
			{
				this.rigList.RemoveAt(i);
			}
		}
		if (this.rigList.Count <= 0)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
			this.projectile.DespawnProjectile();
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		this.TriggerSplashHitPlayers(this.rigList);
		this.projectile.DespawnProjectile();
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001C880 File Offset: 0x0001AA80
	public void TriggerSplashHitPlayers(List<VRRig> hitPlayers)
	{
		int[] array = new int[hitPlayers.Count];
		Vector3[] array2 = new Vector3[hitPlayers.Count];
		for (int i = 0; i < hitPlayers.Count; i++)
		{
			array[i] = ((hitPlayers[i] != null && hitPlayers[i].OwningNetPlayer != null) ? hitPlayers[i].OwningNetPlayer.ActorNumber : -1);
			Vector3 vector = hitPlayers[i].transform.position - this.projectile.transform.position;
			float num = Mathf.Max(0f, 1f - Mathf.Max(0f, vector.magnitude - this.fullSplashRadius) / (this.splashHitDistance - this.fullSplashRadius));
			array2[i] = vector.normalized * this.knockbackSpeed * num;
			if (hitPlayers[i] != null && hitPlayers[i].isLocal && num > 0f)
			{
				this.SplashHitLocalPlayer(array2[i]);
			}
		}
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			array,
			array2
		});
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001C9E8 File Offset: 0x0001ABE8
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
		if (!vector.IsFinite())
		{
			return;
		}
		int[] array;
		if (!GameEntityManager.ValidateDataType<int[]>(data[2], out array))
		{
			return;
		}
		Vector3[] array2;
		if (!GameEntityManager.ValidateDataType<Vector3[]>(data[3], out array2))
		{
			return;
		}
		for (int i = 0; i < array2.Length; i++)
		{
			if (!array2[i].IsFinite())
			{
				return;
			}
		}
		if (array.Length > VRRigCache.Instance.GetAllRigs().Length)
		{
			return;
		}
		if (array.Length != array2.Length)
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		bool flag = false;
		for (int j = 0; j < array.Length; j++)
		{
			SIPlayer x = SIPlayer.Get(array[j]);
			if (x != null && x == SIPlayer.LocalPlayer)
			{
				flag = true;
				this.SplashHitLocalPlayer(array2[j]);
			}
		}
		if (flag)
		{
			SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffectPlayer, vector, base.transform.rotation);
			return;
		}
		SIGadgetBlasterProjectile.SpawnExplosion(this.projectile.hitEffect, vector, base.transform.rotation);
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x0001CB3C File Offset: 0x0001AD3C
	public void SplashHitLocalPlayer(Vector3 directionAndMagnitude)
	{
		if (directionAndMagnitude.magnitude > this.knockbackSpeed * 1.05f)
		{
			return;
		}
		SIPlayer.LocalPlayer.NotifyBlasterSplashHit();
		float num = Vector3.Angle(directionAndMagnitude.normalized, Vector3.up);
		Vector3 a = Vector3.RotateTowards(directionAndMagnitude.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.KnockbackWithHaptics(a * directionAndMagnitude.magnitude, directionAndMagnitude.magnitude / this.knockbackSpeed * this.projectile.hapticHitStrength, this.projectile.hapticHitDuration, true);
	}

	// Token: 0x040005D9 RID: 1497
	public float knockbackSpeed;

	// Token: 0x040005DA RID: 1498
	public float fullSplashRadius;

	// Token: 0x040005DB RID: 1499
	public float splashHitDistance;

	// Token: 0x040005DC RID: 1500
	public float upwardsAngle = 30f;

	// Token: 0x040005DD RID: 1501
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005DE RID: 1502
	private List<VRRig> rigList = new List<VRRig>();

	// Token: 0x040005DF RID: 1503
	private RaycastHit[] hits = new RaycastHit[20];
}
