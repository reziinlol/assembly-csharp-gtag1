using System;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class FeatherDusterHoldable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000DDF RID: 3551 RVA: 0x0004BF0B File Offset: 0x0004A10B
	public void Awake()
	{
		this.timeSinceLastSound = this.soundCooldown;
		this.emissionModule = this.particleFx.emission;
		this.initialRateOverTime = this.emissionModule.rateOverTimeMultiplier;
	}

	// Token: 0x06000DE0 RID: 3552 RVA: 0x0004BF3B File Offset: 0x0004A13B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.lastWorldPos = base.transform.position;
		this.lastSliceTime = Time.unscaledTime;
		this.emissionModule.rateOverTimeMultiplier = 0f;
	}

	// Token: 0x06000DE1 RID: 3553 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000DE2 RID: 3554 RVA: 0x0004BF70 File Offset: 0x0004A170
	public void SliceUpdate()
	{
		float unscaledTime = Time.unscaledTime;
		float num = Mathf.Max(unscaledTime - this.lastSliceTime, Mathf.Epsilon);
		this.lastSliceTime = unscaledTime;
		this.timeSinceLastSound += num;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		float num2 = (position - this.lastWorldPos).sqrMagnitude / num;
		this.emissionModule.rateOverTimeMultiplier = 0f;
		if (num2 >= this.collideMinSpeed * this.collideMinSpeed && Physics.OverlapSphereNonAlloc(position, this.overlapSphereRadius * transform.localScale.x, this.colliderResult, this.collisionLayer) > 0)
		{
			this.emissionModule.rateOverTimeMultiplier = this.initialRateOverTime;
			if (this.timeSinceLastSound >= this.soundCooldown)
			{
				this.soundBankPlayer.Play();
				this.timeSinceLastSound = 0f;
			}
		}
		this.lastWorldPos = position;
	}

	// Token: 0x04001097 RID: 4247
	public LayerMask collisionLayer;

	// Token: 0x04001098 RID: 4248
	public float overlapSphereRadius = 0.08f;

	// Token: 0x04001099 RID: 4249
	[Tooltip("Collision is not tested until this speed requirement is met.")]
	private float collideMinSpeed = 1f;

	// Token: 0x0400109A RID: 4250
	public ParticleSystem particleFx;

	// Token: 0x0400109B RID: 4251
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x0400109C RID: 4252
	[SerializeField]
	private float soundCooldown = 0.8f;

	// Token: 0x0400109D RID: 4253
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x0400109E RID: 4254
	private float initialRateOverTime;

	// Token: 0x0400109F RID: 4255
	private float timeSinceLastSound;

	// Token: 0x040010A0 RID: 4256
	private Vector3 lastWorldPos;

	// Token: 0x040010A1 RID: 4257
	private float lastSliceTime;

	// Token: 0x040010A2 RID: 4258
	private Collider[] colliderResult = new Collider[1];
}
