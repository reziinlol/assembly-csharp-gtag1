using System;
using UnityEngine;

// Token: 0x020009FE RID: 2558
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x06004168 RID: 16744 RVA: 0x0015E285 File Offset: 0x0015C485
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x06004169 RID: 16745 RVA: 0x0015E2A0 File Offset: 0x0015C4A0
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float x = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(x, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x0600416A RID: 16746 RVA: 0x0015E340 File Offset: 0x0015C540
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x0600416B RID: 16747 RVA: 0x0015E39C File Offset: 0x0015C59C
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
			else
			{
				base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
		}
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x0400530D RID: 21261
	public bool setRandomly;

	// Token: 0x0400530E RID: 21262
	public float zRange;

	// Token: 0x0400530F RID: 21263
	public float maxValue;

	// Token: 0x04005310 RID: 21264
	public float minValue;

	// Token: 0x04005311 RID: 21265
	public Vector3 startingLocation;

	// Token: 0x04005312 RID: 21266
	public int valueIndex;

	// Token: 0x04005313 RID: 21267
	public float valueImReporting;

	// Token: 0x04005314 RID: 21268
	public GorillaTriggerBox gorilla;

	// Token: 0x04005315 RID: 21269
	private float startingZ;
}
