using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x02001207 RID: 4615
	public class PFXExtraAnimControls : MonoBehaviour
	{
		// Token: 0x0600739E RID: 29598 RVA: 0x0025A28C File Offset: 0x0025848C
		protected void Awake()
		{
			this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
			this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem.EmissionModule emission = this.particleSystems[i].emission;
				this.cachedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				this.adjustedEmitBursts[i] = new ParticleSystem.Burst[emission.burstCount];
				for (int j = 0; j < emission.burstCount; j++)
				{
					this.cachedEmitBursts[i][j] = emission.GetBurst(j);
					this.adjustedEmitBursts[i][j] = emission.GetBurst(j);
				}
				this.emissionModules[i] = emission;
			}
		}

		// Token: 0x0600739F RID: 29599 RVA: 0x0025A36C File Offset: 0x0025856C
		protected void LateUpdate()
		{
			for (int i = 0; i < this.emissionModules.Length; i++)
			{
				this.emissionModules[i].rateOverTimeMultiplier = this.emitRateMult;
				Mathf.Min(this.emissionModules[i].burstCount, this.cachedEmitBursts[i].Length);
				for (int j = 0; j < this.cachedEmitBursts[i].Length; j++)
				{
					this.adjustedEmitBursts[i][j].probability = this.cachedEmitBursts[i][j].probability * this.emitBurstProbabilityMult;
				}
				this.emissionModules[i].SetBursts(this.adjustedEmitBursts[i]);
			}
		}

		// Token: 0x0400840D RID: 33805
		public float emitRateMult = 1f;

		// Token: 0x0400840E RID: 33806
		public float emitBurstProbabilityMult = 1f;

		// Token: 0x0400840F RID: 33807
		[SerializeField]
		private ParticleSystem[] particleSystems;

		// Token: 0x04008410 RID: 33808
		private ParticleSystem.EmissionModule[] emissionModules;

		// Token: 0x04008411 RID: 33809
		private ParticleSystem.Burst[][] cachedEmitBursts;

		// Token: 0x04008412 RID: 33810
		private ParticleSystem.Burst[][] adjustedEmitBursts;
	}
}
