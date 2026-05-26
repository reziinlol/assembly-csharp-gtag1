using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074D RID: 1869
public class GRBossEyeStare : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002F59 RID: 12121 RVA: 0x00101EE6 File Offset: 0x001000E6
	private void Awake()
	{
		this.boss = base.GetComponentInParent<GREnemyBossMoon>();
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x00101EF4 File Offset: 0x001000F4
	private void OnEnable()
	{
		this.lastLocalRot = base.transform.localEulerAngles;
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x000DCF3F File Offset: 0x000DB13F
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x00101F10 File Offset: 0x00100110
	public void SliceUpdate()
	{
		if (this.boss.CurrAbility != this.lastAbility)
		{
			this.lastLocalRot = base.transform.localEulerAngles;
		}
		if (this.noUpdateAbilities.Contains(this.boss.CurrAbility))
		{
			this.lastLocalRot = base.transform.localEulerAngles;
			this.lastAbility = this.boss.CurrAbility;
			return;
		}
		if (base.transform.localEulerAngles != this.lastLocalRot)
		{
			this.lastLocalRot = base.transform.localEulerAngles;
			if (!this.noUpdateAbilities.Contains(this.boss.CurrAbility))
			{
				this.noUpdateAbilities.Add(this.boss.CurrAbility);
			}
			this.lastAbility = this.boss.CurrAbility;
			return;
		}
		if (this.closestPlayer == null || Time.time > this.lastCheck + this.checkForClosestPlayerCooldown)
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			float num = float.MaxValue;
			for (int i = 0; i < this.rigs.Count; i++)
			{
				float sqrMagnitude = (base.transform.position - this.rigs[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					this.closestPlayer = this.rigs[i].transform;
				}
			}
			this.lastCheck = Time.time;
		}
		this.lastAbility = this.boss.CurrAbility;
		if (this.closestPlayer == null)
		{
			return;
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Vector3.up, (this.closestPlayer.position - base.transform.position).normalized) * Quaternion.Euler(this.rotOffset), this.lerpAmount);
		this.lastLocalRot = base.transform.localEulerAngles;
	}

	// Token: 0x04003CC6 RID: 15558
	private Vector3 lastLocalRot;

	// Token: 0x04003CC7 RID: 15559
	private List<GRAbilityBase> noUpdateAbilities = new List<GRAbilityBase>();

	// Token: 0x04003CC8 RID: 15560
	private GREnemyBossMoon boss;

	// Token: 0x04003CC9 RID: 15561
	private GRAbilityBase lastAbility;

	// Token: 0x04003CCA RID: 15562
	private float lastCheck;

	// Token: 0x04003CCB RID: 15563
	private float checkForClosestPlayerCooldown = 1f;

	// Token: 0x04003CCC RID: 15564
	private Transform closestPlayer;

	// Token: 0x04003CCD RID: 15565
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x04003CCE RID: 15566
	public float lerpAmount = 0.3f;

	// Token: 0x04003CCF RID: 15567
	public Vector3 rotOffset;
}
