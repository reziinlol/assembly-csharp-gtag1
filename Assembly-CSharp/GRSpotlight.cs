using System;
using UnityEngine;

// Token: 0x020007E5 RID: 2021
public class GRSpotlight : MonoBehaviourTick
{
	// Token: 0x060033AC RID: 13228 RVA: 0x0011C9CC File Offset: 0x0011ABCC
	private void Awake()
	{
		this.yStart = base.transform.rotation.eulerAngles.y;
		this.xStart = base.transform.rotation.eulerAngles.x;
		this.timeOffset = Random.value * 360f;
		this.yFrequency += Random.value / 100f;
		this.xFrequency += Random.value / 100f;
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x0011CA58 File Offset: 0x0011AC58
	public override void Tick()
	{
		base.transform.eulerAngles = new Vector3(this.xStart + this.xAmplitude * Mathf.Sin(Time.time * this.xFrequency), this.yStart + this.yAmplitude * Mathf.Cos(Time.time * this.yFrequency), 0f);
	}

	// Token: 0x04004360 RID: 17248
	public float yAmplitude = 75f;

	// Token: 0x04004361 RID: 17249
	public float xAmplitude = 40f;

	// Token: 0x04004362 RID: 17250
	public float yFrequency = 0.2f;

	// Token: 0x04004363 RID: 17251
	public float xFrequency = 0.3f;

	// Token: 0x04004364 RID: 17252
	private float yStart;

	// Token: 0x04004365 RID: 17253
	private float xStart;

	// Token: 0x04004366 RID: 17254
	private float timeOffset;
}
