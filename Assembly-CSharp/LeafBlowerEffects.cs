using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class LeafBlowerEffects : MonoBehaviour, ISpawnable
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x060012BA RID: 4794 RVA: 0x00063A29 File Offset: 0x00061C29
	// (set) Token: 0x060012BB RID: 4795 RVA: 0x00063A31 File Offset: 0x00061C31
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x060012BC RID: 4796 RVA: 0x00063A3A File Offset: 0x00061C3A
	// (set) Token: 0x060012BD RID: 4797 RVA: 0x00063A42 File Offset: 0x00061C42
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060012BE RID: 4798 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00063A4C File Offset: 0x00061C4C
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.headToleranceAngleCos = Mathf.Cos(0.017453292f * this.headToleranceAngle);
		this.squareHitAngleCos = Mathf.Cos(0.017453292f * this.squareHitAngle);
		this.fan = rig.cosmeticReferences.Get(this.fanRef).GetComponent<CosmeticFan>();
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00063AA3 File Offset: 0x00061CA3
	public void StartFan()
	{
		this.fan.Run();
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00063AB0 File Offset: 0x00061CB0
	public void StopFan()
	{
		this.fan.Stop();
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00063ABD File Offset: 0x00061CBD
	public void UpdateEffects()
	{
		this.ProjectParticles();
		this.BlowFaces();
	}

	// Token: 0x060012C3 RID: 4803 RVA: 0x00063ACC File Offset: 0x00061CCC
	public void ProjectParticles()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.gunBarrel.transform.position, this.gunBarrel.transform.forward, out raycastHit, this.projectionRange, this.raycastLayers))
		{
			SpawnOnEnter component = raycastHit.collider.GetComponent<SpawnOnEnter>();
			if (component != null)
			{
				component.OnTriggerEnter(raycastHit.collider);
			}
			if (Vector3.Dot(raycastHit.normal, this.gunBarrel.transform.forward) < -this.squareHitAngleCos)
			{
				this.squareHitParticleSystem.transform.position = raycastHit.point;
				this.squareHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Play(true);
					return;
				}
			}
			else
			{
				this.angledHitParticleSystem.transform.position = raycastHit.point;
				this.angledHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Play(true);
					return;
				}
			}
		}
		else
		{
			this.StopEffects();
		}
	}

	// Token: 0x060012C4 RID: 4804 RVA: 0x00063C7E File Offset: 0x00061E7E
	public void StopEffects()
	{
		this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	// Token: 0x060012C5 RID: 4805 RVA: 0x00063C9C File Offset: 0x00061E9C
	public void BlowFaces()
	{
		Vector3 position = this.gunBarrel.transform.position;
		Vector3 forward = this.gunBarrel.transform.forward;
		if (NetworkSystem.Instance.InRoom)
		{
			using (IEnumerator<RigContainer> enumerator = VRRigCache.ActiveRigContainers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RigContainer rigContainer = enumerator.Current;
					this.TryBlowFace(rigContainer.Rig, position, forward);
				}
				return;
			}
		}
		this.TryBlowFace(VRRig.LocalRig, position, forward);
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00063D2C File Offset: 0x00061F2C
	private void TryBlowFace(VRRig rig, Vector3 origin, Vector3 directionNormalized)
	{
		Transform rigTarget = rig.head.rigTarget;
		Vector3 vector = rigTarget.position - origin;
		float num = Vector3.Dot(vector, directionNormalized);
		if (num < 0f || num > this.projectionRange)
		{
			return;
		}
		if ((vector - num * directionNormalized).IsLongerThan(this.projectionWidth))
		{
			return;
		}
		if (Vector3.Dot(-rigTarget.forward, vector.normalized) < this.headToleranceAngleCos)
		{
			return;
		}
		rig.GetComponent<GorillaMouthFlap>().EnableLeafBlower();
	}

	// Token: 0x040016E8 RID: 5864
	[SerializeField]
	private GameObject gunBarrel;

	// Token: 0x040016E9 RID: 5865
	[SerializeField]
	private float projectionRange;

	// Token: 0x040016EA RID: 5866
	[SerializeField]
	private float projectionWidth;

	// Token: 0x040016EB RID: 5867
	[SerializeField]
	private float headToleranceAngle;

	// Token: 0x040016EC RID: 5868
	[SerializeField]
	private LayerMask raycastLayers;

	// Token: 0x040016ED RID: 5869
	[SerializeField]
	private ParticleSystem angledHitParticleSystem;

	// Token: 0x040016EE RID: 5870
	[SerializeField]
	private ParticleSystem squareHitParticleSystem;

	// Token: 0x040016EF RID: 5871
	[SerializeField]
	private float squareHitAngle;

	// Token: 0x040016F0 RID: 5872
	[SerializeField]
	private CosmeticRefID fanRef;

	// Token: 0x040016F1 RID: 5873
	private float headToleranceAngleCos;

	// Token: 0x040016F2 RID: 5874
	private float squareHitAngleCos;

	// Token: 0x040016F3 RID: 5875
	private CosmeticFan fan;
}
