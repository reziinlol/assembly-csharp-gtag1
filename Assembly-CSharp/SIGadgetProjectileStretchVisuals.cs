using System;
using UnityEngine;

// Token: 0x020000E5 RID: 229
[RequireComponent(typeof(SIGadgetBlasterProjectile))]
public class SIGadgetProjectileStretchVisuals : MonoBehaviourTick
{
	// Token: 0x0600055D RID: 1373 RVA: 0x0001E2FC File Offset: 0x0001C4FC
	public new void OnEnable()
	{
		base.OnEnable();
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
		this.totalLength = (this.frontStretch.position - this.rearStretch.position).magnitude;
		this.distancePerFrame = this.projectile.startingVelocity * Time.fixedDeltaTime;
		this.maxStretchRatio = this.distancePerFrame / this.totalLength * this.framesPerPosition;
		this.timeSpawned = Time.time;
		this.maxSizeReached = false;
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, 1f);
		this.frontDistance = (this.frontStretch.position - base.transform.position).magnitude;
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x0001E3F8 File Offset: 0x0001C5F8
	public override void Tick()
	{
		if (this.maxSizeReached)
		{
			return;
		}
		float num = (Time.time - this.timeSpawned) * this.projectile.startingVelocity / this.totalLength / 2f + 1f;
		if (num >= this.maxStretchRatio)
		{
			num = this.maxStretchRatio;
			this.maxSizeReached = true;
		}
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, -(num - 1f) * this.frontDistance);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, num);
	}

	// Token: 0x04000637 RID: 1591
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x04000638 RID: 1592
	public GameObject baseVisuals;

	// Token: 0x04000639 RID: 1593
	public Transform frontStretch;

	// Token: 0x0400063A RID: 1594
	public Transform rearStretch;

	// Token: 0x0400063B RID: 1595
	public float framesPerPosition;

	// Token: 0x0400063C RID: 1596
	private float totalLength;

	// Token: 0x0400063D RID: 1597
	private float distancePerFrame;

	// Token: 0x0400063E RID: 1598
	private float maxStretchRatio;

	// Token: 0x0400063F RID: 1599
	private bool maxSizeReached;

	// Token: 0x04000640 RID: 1600
	private float frontDistance;

	// Token: 0x04000641 RID: 1601
	private float timeSpawned;
}
