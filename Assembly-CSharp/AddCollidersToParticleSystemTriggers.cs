using System;
using UnityEngine;

// Token: 0x020005DD RID: 1501
public class AddCollidersToParticleSystemTriggers : MonoBehaviour
{
	// Token: 0x0600255D RID: 9565 RVA: 0x000C6474 File Offset: 0x000C4674
	private void Update()
	{
		this.count = 0;
		while (this.count < 6)
		{
			this.index++;
			if (this.index >= this.collidersToAdd.Length)
			{
				if (BetterDayNightManager.instance.collidersToAddToWeatherSystems.Count >= this.index - this.collidersToAdd.Length)
				{
					this.index = 0;
				}
				else
				{
					this.particleSystemToUpdate.trigger.SetCollider(this.count, BetterDayNightManager.instance.collidersToAddToWeatherSystems[this.index - this.collidersToAdd.Length]);
				}
			}
			if (this.index < this.collidersToAdd.Length)
			{
				this.particleSystemToUpdate.trigger.SetCollider(this.count, this.collidersToAdd[this.index]);
			}
			this.count++;
		}
	}

	// Token: 0x040030DD RID: 12509
	public Collider[] collidersToAdd;

	// Token: 0x040030DE RID: 12510
	public ParticleSystem particleSystemToUpdate;

	// Token: 0x040030DF RID: 12511
	private int count;

	// Token: 0x040030E0 RID: 12512
	private int index;
}
