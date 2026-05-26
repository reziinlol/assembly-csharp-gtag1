using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DF RID: 223
[RequireComponent(typeof(SIGadgetProjectileType))]
[RequireComponent(typeof(Rigidbody))]
public class SIGadgetBlasterProjectile : MonoBehaviourTick
{
	// Token: 0x0600053C RID: 1340 RVA: 0x0001D485 File Offset: 0x0001B685
	public override void Tick()
	{
		if (Time.time > this.timeSpawned + this.maxLifetime)
		{
			this.parentBlaster.DespawnProjectile(this);
		}
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001D4A8 File Offset: 0x0001B6A8
	public void InitializeProjectile()
	{
		this.rb.angularVelocity = Vector3.zero;
		this.rb.linearVelocity = base.transform.forward * this.startingVelocity;
		this.timeSpawned = Time.realtimeSinceStartup;
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponentInChildren<AudioSource>();
		}
		this.audioSource.time = 0f;
		this.projectileType = base.GetComponent<SIGadgetProjectileType>();
		SIGadgetProjectileModifier[] components = base.GetComponents<SIGadgetProjectileModifier>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ModifyProjectile(this);
		}
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001D548 File Offset: 0x0001B748
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SIExclusionZone>() != null && Time.realtimeSinceStartup > this.timeSpawned + 0.02f)
		{
			if (this.exclusionZoneDespawnEffect != null)
			{
				SIGadgetBlasterProjectile.SpawnExplosion(this.exclusionZoneDespawnEffect, base.transform.position, base.transform.rotation);
			}
			this.DespawnProjectile();
			return;
		}
		SIPlayer componentInParent = other.GetComponentInParent<SIPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent == this.firedByPlayer)
		{
			return;
		}
		if (this.firedByPlayer != SIPlayer.LocalPlayer || componentInParent == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectileType.LocalProjectileHit(componentInParent);
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001D5FC File Offset: 0x0001B7FC
	private void OnCollisionEnter(Collision collision)
	{
		this.projectileType.LocalProjectileHit(null);
		HitTargetNetworkState hitTargetNetworkState;
		if (collision.collider.gameObject.TryGetComponent<HitTargetNetworkState>(out hitTargetNetworkState))
		{
			hitTargetNetworkState.TargetHit((Time.time - this.timeSpawned) * this.startingVelocity * -base.transform.forward + base.transform.position, base.transform.position);
		}
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001D672 File Offset: 0x0001B872
	public void DespawnProjectile()
	{
		this.parentBlaster.DespawnProjectile(this);
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001D680 File Offset: 0x0001B880
	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, bool adjustForDirection = true)
	{
		this.KnockbackWithHaptics(directionAndMagnitude, this.hapticHitStrength, this.hapticHitDuration, adjustForDirection);
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x0001D698 File Offset: 0x0001B898
	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, float hapticStrength, float hapticDuration, bool adjustForDirection = true)
	{
		SIPlayer.LocalPlayer.PlayerKnockback(directionAndMagnitude, true, true);
		SIPlayer.LocalPlayer.NotifyBlasterHit();
		if (adjustForDirection)
		{
			Vector3 from = GorillaTagger.Instance.leftHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			Vector3 from2 = GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			float num = 0.5f;
			float num2 = 45f;
			float num3 = Vector3.Angle(from, directionAndMagnitude);
			float num4 = Vector3.Angle(from2, directionAndMagnitude);
			float hapticStrength2 = (1f - Mathf.Max(num3 - num2, 0f) / (180f - num2)) * num + (1f - num);
			float hapticStrength3 = (1f - Mathf.Max(num4 - num2, 0f) / (180f - num2)) * num + (1f - num);
			SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength2, hapticDuration, true);
			SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength3, hapticDuration, true);
			return;
		}
		SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength, hapticDuration, true);
		SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength, hapticDuration, true);
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x0001D7BC File Offset: 0x0001B9BC
	public static GameObject SpawnExplosion(GameObject explosionPrefab, Vector3 position, Quaternion rotation)
	{
		if (SIGadgetBlasterProjectile.blasterProjectileExplosionPools == null)
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools = new Dictionary<int, List<GameObject>>();
		}
		if (SIGadgetBlasterProjectile.explosionTypeKey == null)
		{
			SIGadgetBlasterProjectile.explosionTypeKey = new Dictionary<GameObject, int>();
		}
		int instanceID = explosionPrefab.GetInstanceID();
		if (!SIGadgetBlasterProjectile.blasterProjectileExplosionPools.ContainsKey(instanceID))
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlasterProjectile.blasterProjectileExplosionPools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(explosionPrefab, position, rotation);
			SIGadgetBlasterProjectile.explosionTypeKey.Add(gameObject, instanceID);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		return gameObject;
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x0001D878 File Offset: 0x0001BA78
	public static void DespawnExplosion(GameObject explosion)
	{
		SIGadgetBlasterProjectile.blasterProjectileExplosionPools[SIGadgetBlasterProjectile.explosionTypeKey[explosion]].Add(explosion);
		explosion.SetActive(false);
	}

	// Token: 0x04000602 RID: 1538
	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectileExplosionPools;

	// Token: 0x04000603 RID: 1539
	[OnEnterPlay_SetNull]
	public static Dictionary<GameObject, int> explosionTypeKey;

	// Token: 0x04000604 RID: 1540
	[NonSerialized]
	public int poolId;

	// Token: 0x04000605 RID: 1541
	public SIGadgetProjectileType projectileType;

	// Token: 0x04000606 RID: 1542
	public Rigidbody rb;

	// Token: 0x04000607 RID: 1543
	public GameObject hitEffect;

	// Token: 0x04000608 RID: 1544
	public GameObject hitEffectPlayer;

	// Token: 0x04000609 RID: 1545
	public float maxLifetime = 10f;

	// Token: 0x0400060A RID: 1546
	[NonSerialized]
	public float timeSpawned;

	// Token: 0x0400060B RID: 1547
	public float hapticHitStrength = 0.75f;

	// Token: 0x0400060C RID: 1548
	public float hapticHitDuration = 0.1f;

	// Token: 0x0400060D RID: 1549
	[NonSerialized]
	public SIGadgetBlaster parentBlaster;

	// Token: 0x0400060E RID: 1550
	[NonSerialized]
	public int projectileId;

	// Token: 0x0400060F RID: 1551
	[NonSerialized]
	public SIPlayer firedByPlayer;

	// Token: 0x04000610 RID: 1552
	public float startingVelocity;

	// Token: 0x04000611 RID: 1553
	public const float EXCLUSION_ZONE_MINIMUM_LIFETIME = 0.02f;

	// Token: 0x04000612 RID: 1554
	public GameObject exclusionZoneDespawnEffect;

	// Token: 0x04000613 RID: 1555
	private AudioSource audioSource;
}
