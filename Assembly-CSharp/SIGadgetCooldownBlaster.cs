using System;
using UnityEngine;

// Token: 0x020000E4 RID: 228
public class SIGadgetCooldownBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x06000554 RID: 1364 RVA: 0x0001DF3E File Offset: 0x0001C13E
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001DF4C File Offset: 0x0001C14C
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.blaster.firingSource.clip = this.firingClip;
		this.blaster.firingSource.volume = this.firingVolume;
		this.blaster.firingSource.loop = false;
		this.blaster.blasterSource.clip = this.cooldownClip;
		this.blaster.blasterSource.volume = this.cooldownVolume;
		this.blaster.blasterSource.loop = false;
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001DFE0 File Offset: 0x0001C1E0
	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Cooldown)
			{
				return;
			}
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.FireProjectileHaptics(this.availableToFireHapticStrength, 0.02f);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
		}
		else
		{
			if (!this.CheckInput())
			{
				this.triggerHeldDown = false;
				return;
			}
			if (!this.triggerHeldDown)
			{
				this.triggerHeldDown = true;
				this.FireProjectile(this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
		}
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001E09C File Offset: 0x0001C29C
	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
		}
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0001E0BC File Offset: 0x0001C2BC
	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.cooldownIndicator.sharedMaterial = this.readyToFireMaterial;
			return;
		}
		if (currentState != SIGadgetBlasterState.Cooldown)
		{
			return;
		}
		this.blaster.lastFired = Time.time;
		this.cooldownIndicator.sharedMaterial = this.onCooldownMaterial;
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001E110 File Offset: 0x0001C310
	public void FireProjectile(int fireId, Vector3 position, Quaternion rotation)
	{
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			if (Time.time < this.blaster.lastFired + this.fireCooldown)
			{
				return;
			}
			this.blaster.FireProjectileHaptics(this.firingHapticStrength, this.firingHapticDuration);
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				fireId,
				position,
				rotation
			});
		}
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.blasterSource.time = 0f;
		this.blaster.blasterSource.Play();
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001E200 File Offset: 0x0001C400
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 3)
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out fireId))
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
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rotation))
		{
			return;
		}
		if ((vector - this.blaster.firingPosition.position).magnitude > this.blaster.maxLagDistance)
		{
			return;
		}
		if (this.blaster.CurrentFireRate() > 1f / this.fireCooldown * (1f + this.fireRateGracePercentage))
		{
			return;
		}
		this.FireProjectile(fireId, vector, rotation);
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x04000626 RID: 1574
	public SIGadgetBlasterProjectile projectilePrefab;

	// Token: 0x04000627 RID: 1575
	public float fireCooldown = 0.5f;

	// Token: 0x04000628 RID: 1576
	public float fireRateGracePercentage = 0.25f;

	// Token: 0x04000629 RID: 1577
	public float availableToFireHapticStrength = 0.1f;

	// Token: 0x0400062A RID: 1578
	public float availableToFireHapticDuration = 0.01f;

	// Token: 0x0400062B RID: 1579
	public float firingHapticStrength = 0.25f;

	// Token: 0x0400062C RID: 1580
	public float firingHapticDuration = 0.01f;

	// Token: 0x0400062D RID: 1581
	public AudioClip firingClip;

	// Token: 0x0400062E RID: 1582
	public AudioClip cooldownClip;

	// Token: 0x0400062F RID: 1583
	public float firingVolume;

	// Token: 0x04000630 RID: 1584
	public float cooldownVolume;

	// Token: 0x04000631 RID: 1585
	public ParticleSystem fireFX;

	// Token: 0x04000632 RID: 1586
	public MeshRenderer cooldownIndicator;

	// Token: 0x04000633 RID: 1587
	public Material readyToFireMaterial;

	// Token: 0x04000634 RID: 1588
	public Material onCooldownMaterial;

	// Token: 0x04000635 RID: 1589
	private bool triggerHeldDown;

	// Token: 0x04000636 RID: 1590
	private SIGadgetBlaster blaster;
}
