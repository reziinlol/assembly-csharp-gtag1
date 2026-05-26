using System;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class CosmeticCritterShadeHidden : CosmeticCritter
{
	// Token: 0x060004DD RID: 1245 RVA: 0x0001B1B6 File Offset: 0x000193B6
	public void SetCenterAndRadius(Vector3 center, float radius)
	{
		this.orbitCenter = center;
		this.orbitRadius = radius;
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x0001B1C6 File Offset: 0x000193C6
	public override void SetRandomVariables()
	{
		this.initialAngle = Random.Range(0f, 6.2831855f);
		this.orbitDirection = ((Random.value > 0.5f) ? 1f : -1f);
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x0001B1FC File Offset: 0x000193FC
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		float f = this.initialAngle + this.orbitDegreesPerSecond * num * this.orbitDirection;
		float y = this.verticalBobMagnitude * Mathf.Sin(num * this.verticalBobFrequency);
		base.transform.position = this.orbitCenter + new Vector3(this.orbitRadius * Mathf.Cos(f), y, this.orbitRadius * Mathf.Sin(f));
	}

	// Token: 0x0400056D RID: 1389
	[Space]
	[Tooltip("How quickly the Shade orbits around the point where it spawned (the spawner's position).")]
	[SerializeField]
	private float orbitDegreesPerSecond;

	// Token: 0x0400056E RID: 1390
	[Tooltip("The strength of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobMagnitude;

	// Token: 0x0400056F RID: 1391
	[Tooltip("The frequency of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobFrequency;

	// Token: 0x04000570 RID: 1392
	private Vector3 orbitCenter;

	// Token: 0x04000571 RID: 1393
	private float initialAngle;

	// Token: 0x04000572 RID: 1394
	private float orbitRadius;

	// Token: 0x04000573 RID: 1395
	private float orbitDirection;
}
