using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class CornOnCobCosmetic : MonoBehaviour
{
	// Token: 0x06000D51 RID: 3409 RVA: 0x00048D30 File Offset: 0x00046F30
	protected void Awake()
	{
		this.emissionModule = this.particleSys.emission;
		this.maxBurstProbability = ((this.emissionModule.burstCount > 0) ? this.emissionModule.GetBurst(0).probability : 0.2f);
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x00048D80 File Offset: 0x00046F80
	protected void LateUpdate()
	{
		for (int i = 0; i < this.emissionModule.burstCount; i++)
		{
			ParticleSystem.Burst burst = this.emissionModule.GetBurst(i);
			burst.probability = this.maxBurstProbability * this.particleEmissionCurve.Evaluate(this.thermalReceiver.celsius);
			this.emissionModule.SetBurst(i, burst);
		}
		int particleCount = this.particleSys.particleCount;
		if (particleCount > this.previousParticleCount)
		{
			this.soundBankPlayer.Play();
		}
		this.previousParticleCount = particleCount;
	}

	// Token: 0x04000FF5 RID: 4085
	[Tooltip("The corn will start popping based on the temperature from this ThermalReceiver.")]
	public ThermalReceiver thermalReceiver;

	// Token: 0x04000FF6 RID: 4086
	[Tooltip("The particle system that will be emitted when the heat source is hot enough.")]
	public ParticleSystem particleSys;

	// Token: 0x04000FF7 RID: 4087
	[Tooltip("The curve that determines how many particles will be emitted based on the heat source's temperature.\n\nThe x-axis is the heat source's temperature and the y-axis is the number of particles to emit.")]
	public AnimationCurve particleEmissionCurve;

	// Token: 0x04000FF8 RID: 4088
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x04000FF9 RID: 4089
	private ParticleSystem.EmissionModule emissionModule;

	// Token: 0x04000FFA RID: 4090
	private float maxBurstProbability;

	// Token: 0x04000FFB RID: 4091
	private int previousParticleCount;
}
