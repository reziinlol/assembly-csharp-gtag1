using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class SIGadgetChargeBlaster : MonoBehaviour, SIGadgetBlasterType
{
	// Token: 0x06000549 RID: 1353 RVA: 0x0001D8C5 File Offset: 0x0001BAC5
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001D8D2 File Offset: 0x0001BAD2
	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.currentCharge = 0f;
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001D8EC File Offset: 0x0001BAEC
	public void OnUpdateAuthority(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			if (this.CheckInput())
			{
				this.FireProjectile(0f, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
				return;
			}
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			if (this.CheckInput())
			{
				this.blaster.FireProjectileHaptics(this.chargeLevels[this.CurrentBlasterChargeLevel()].chargingHapticStrength, Time.fixedDeltaTime);
				return;
			}
			if (this.CurrentBlasterChargeLevel() > 0)
			{
				this.FireProjectile(this.currentCharge, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
			this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			return;
		case SIGadgetBlasterState.Cooldown:
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				if (this.CheckInput())
				{
					this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
					return;
				}
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001DA48 File Offset: 0x0001BC48
	public void OnUpdateRemote(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
		case SIGadgetBlasterState.Cooldown:
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x0001DA94 File Offset: 0x0001BC94
	public void SetStateShared()
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			this.currentCharge = 0f;
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge = 0f;
			this.blaster.blasterSource.clip = this.chargingClip;
			this.blaster.blasterSource.volume = this.chargeLevels[0].chargingVolume;
			this.blaster.blasterSource.loop = true;
			this.blaster.blasterSource.Play();
			break;
		case SIGadgetBlasterState.Cooldown:
			this.blaster.blasterSource.Stop();
			if (Time.time > this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.lastFired = Time.time;
			}
			break;
		}
		this.UpdateChargingVisuals();
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x0001DB78 File Offset: 0x0001BD78
	public void FireProjectile(float firedAtChargeLevel, int fireId, Vector3 position, Quaternion rotation)
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
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				firedAtChargeLevel,
				fireId,
				position,
				rotation
			});
		}
		if (Mathf.Abs(this.currentCharge - firedAtChargeLevel) <= this.maxChargeDiff)
		{
			this.currentCharge = firedAtChargeLevel;
		}
		int num = this.CurrentBlasterChargeLevel();
		this.blaster.firingSource.clip = this.chargeLevels[num].firingClip;
		this.blaster.firingSource.volume = this.chargeLevels[num].firingVolume;
		this.chargeLevels[num].fireFX.Play();
		SIGadgetBlasterProjectile projectilePrefab = this.chargeLevels[num].projectilePrefab;
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.FireProjectileHaptics(this.chargeLevels[num].firingHapticStrength, this.chargeLevels[num].firingHapticDuration);
		}
		this.currentCharge = 0f;
		this.blaster.InstantiateProjectile(projectilePrefab, position, rotation, fireId);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x0001DD18 File Offset: 0x0001BF18
	private void UpdateChargingVisuals()
	{
		bool flag = this.blaster.currentState == SIGadgetBlasterState.Charging;
		int num = this.CurrentBlasterChargeLevel();
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			bool flag2 = flag && i == num;
			if (this.chargeLevels[i].chargingFX.activeSelf != flag2)
			{
				this.chargeLevels[i].chargingFX.SetActive(flag2);
			}
		}
		if (this.blaster.blasterSource.clip != this.chargingClip)
		{
			this.blaster.blasterSource.clip = this.chargingClip;
		}
		this.blaster.blasterSource.volume = this.chargeLevels[num].chargingVolume;
		if (!flag && this.blaster.blasterSource.isPlaying)
		{
			this.blaster.blasterSource.Stop();
		}
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x0001DE04 File Offset: 0x0001C004
	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		float num;
		if (!GameEntityManager.ValidateDataType<float>(data[0], out num))
		{
			return;
		}
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[1], out fireId))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out rotation))
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
		this.FireProjectile(num, fireId, vector, rotation);
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x0001DEC8 File Offset: 0x0001C0C8
	public int CurrentBlasterChargeLevel()
	{
		int result = -1;
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			if (this.currentCharge < this.chargeLevels[i].chargeThreshold)
			{
				return result;
			}
			result = i;
		}
		return result;
	}

	// Token: 0x04000614 RID: 1556
	[SerializeField]
	private float fireCooldown = 0.2f;

	// Token: 0x04000615 RID: 1557
	[SerializeField]
	private float chargeRatePerSecond = 20f;

	// Token: 0x04000616 RID: 1558
	public float fireRateGracePercentage = 0.25f;

	// Token: 0x04000617 RID: 1559
	public float maxChargeDiff = 5f;

	// Token: 0x04000618 RID: 1560
	private float currentCharge;

	// Token: 0x04000619 RID: 1561
	public AudioClip chargingClip;

	// Token: 0x0400061A RID: 1562
	public SIGadgetChargeBlaster.BlasterChargeLevel[] chargeLevels;

	// Token: 0x0400061B RID: 1563
	private SIGadgetBlaster blaster;

	// Token: 0x020000E3 RID: 227
	[Serializable]
	public struct BlasterChargeLevel
	{
		// Token: 0x0400061C RID: 1564
		public float chargeThreshold;

		// Token: 0x0400061D RID: 1565
		public float chargingVolume;

		// Token: 0x0400061E RID: 1566
		public float firingVolume;

		// Token: 0x0400061F RID: 1567
		public float chargingHapticStrength;

		// Token: 0x04000620 RID: 1568
		public float firingHapticStrength;

		// Token: 0x04000621 RID: 1569
		public float firingHapticDuration;

		// Token: 0x04000622 RID: 1570
		public AudioClip firingClip;

		// Token: 0x04000623 RID: 1571
		public ParticleSystem fireFX;

		// Token: 0x04000624 RID: 1572
		public GameObject chargingFX;

		// Token: 0x04000625 RID: 1573
		public SIGadgetBlasterProjectile projectilePrefab;
	}
}
